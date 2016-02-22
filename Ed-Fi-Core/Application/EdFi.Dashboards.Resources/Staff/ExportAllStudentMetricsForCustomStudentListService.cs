// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using EdFi.Dashboards.Application.Data.Entities;
using EdFi.Dashboards.Common;
using EdFi.Dashboards.Data.Entities;
using EdFi.Dashboards.Data.Repository;
using EdFi.Dashboards.Metric.Data.Entities;
using EdFi.Dashboards.Resources.Metric;
using EdFi.Dashboards.Resources.Models.Common;
using EdFi.Dashboards.Resources.Security.Common;
using EdFi.Dashboards.SecurityTokenService.Authentication;
using MetricType = EdFi.Dashboards.Metric.Resources.Models.MetricType;

namespace EdFi.Dashboards.Resources.Staff
{
    public class ExportAllStudentMetricsForCustomStudentListRequest
    {
        public long StaffUSI { get; set; }
        public int SchoolId { get; set; }
        public string StudentListType { get; set; }
        public long SectionOrCohortId { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExportAllStudentMetricsForStaffSectionRequest"/> using the specified parameters.
        /// </summary>
        /// <returns>A new <see cref="ExportAllStudentMetricsForStaffSectionRequest"/> instance.</returns>
        public static ExportAllStudentMetricsForCustomStudentListRequest Create(long staffUSI, int schoolId, string studentListType, long sectionOrCohortId)
        {
            return new ExportAllStudentMetricsForCustomStudentListRequest { StaffUSI = staffUSI, SchoolId = schoolId, StudentListType = studentListType, SectionOrCohortId = sectionOrCohortId };
        }
    }

    public interface IExportAllStudentMetricsForCustomStudentListService : IService<ExportAllStudentMetricsForCustomStudentListRequest, StudentExportAllModel> { }

    public class ExportAllStudentMetricsForCustomStudentListService : IExportAllStudentMetricsForCustomStudentListService
    {
        private readonly IRepository<StudentSchoolMetricInstanceSet> studentSchoolMetricInstanceSetRepository;
        private readonly IRepository<MetricInstance> metricInstanceRepository;
        private readonly IRepository<StudentInformation> studentInformationRepository;
        private readonly IRepository<StaffCustomStudentListStudent> staffCustomStudentListStudentRepository;
        private readonly IRootMetricNodeResolver rootMetricNodeResolver;

        public ExportAllStudentMetricsForCustomStudentListService(
            IRepository<StudentSchoolMetricInstanceSet> studentSchoolMetricInstanceSetRepository,
            IRepository<MetricInstance> metricInstanceRepository,
            IRepository<StudentInformation> studentInformationRepository,
            IRepository<StaffCustomStudentListStudent> staffCustomStudentListStudentRepository,
            IRootMetricNodeResolver rootMetricNodeResolver)
        {
            this.studentSchoolMetricInstanceSetRepository = studentSchoolMetricInstanceSetRepository;
            this.metricInstanceRepository = metricInstanceRepository;
            this.studentInformationRepository = studentInformationRepository;
            this.staffCustomStudentListStudentRepository = staffCustomStudentListStudentRepository;
            this.rootMetricNodeResolver = rootMetricNodeResolver;
        }

        [CanBeAuthorizedBy(EdFiClaimTypes.ViewAllStudents, EdFiClaimTypes.ViewMyStudents)]
        public StudentExportAllModel Get(ExportAllStudentMetricsForCustomStudentListRequest request)
        {
            int schoolId = request.SchoolId;
            long customStudentListId = request.SectionOrCohortId;


            //Lets get the metric tree that applies to this.
            var rootMetricNode = rootMetricNodeResolver.GetRootMetricNodeForStudent(schoolId);

            if (rootMetricNode == null)
                throw new InvalidOperationException("No metric metadata available for exporting a hierarchy.");

            var metricsOfTypeGranular = rootMetricNode.DescendantsOrSelf.Where(y => y.MetricType == MetricType.GranularMetric);


            //need to break up this function because the tables are from 2 different DBs (App & LEA)
            long[] studentUSIs = Enumerable.ToArray(staffCustomStudentListStudentRepository.GetAll().Where(x => x.StaffCustomStudentListId == customStudentListId).Select(x => x.StudentUSI));

            //Lets get the DATA from the DB...
            var query = (from se in studentSchoolMetricInstanceSetRepository.GetAll() 
                         join si in studentInformationRepository.GetAll() on se.StudentUSI equals si.StudentUSI
                         join m in metricInstanceRepository.GetAll() on se.MetricInstanceSetKey equals m.MetricInstanceSetKey
                         where se.SchoolId == schoolId 
                         && studentUSIs.Contains(si.StudentUSI)
                         select new
                         {
                             si.StudentUSI,
                             StudentName = Utilities.FormatPersonNameByLastName(si.FirstName, si.MiddleName, si.LastSurname),
                             m.MetricId,
                             m.Value
                         }
                        ).ToList();

            var groupedStudents = (from s in query
                                  group s by s.StudentUSI
                                      into g
                                      select new
                                      {
                                          StudentUSI = g.Key,
                                          g.First().StudentName,
                                          metrics = g.Select(x => new { x.MetricId, x.Value })
                                      }).OrderBy(x=>x.StudentName);

            var rows = new List<StudentExportAllModel.Row>();
            foreach (var student in groupedStudents)
            {
                var cells = new List<KeyValuePair<string, object>>
                                {
                                    new KeyValuePair<string, object>("Student Name", student.StudentName)
                                };
                
                foreach (var granularMetricNode in metricsOfTypeGranular)
                {
                    var studentMetric = student.metrics.SingleOrDefault(x => x.MetricId == granularMetricNode.MetricId);
                    var meaningfullMetricName = Utilities.Metrics.GetMetricName(granularMetricNode);
                    cells.Add(new KeyValuePair<string, object>(meaningfullMetricName, (studentMetric == null) ? string.Empty : studentMetric.Value));
                }

                rows.Add(new StudentExportAllModel.Row
                             {
                                 StudentUSI = student.StudentUSI,
                                 Cells = cells
                             });
            }

            return new StudentExportAllModel { Rows = rows };
        }
    }
}
