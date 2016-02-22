// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
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
    public class ExportAllStudentsForStaffsSectionsAndCohortsRequest
    {
        public long StaffUSI { get; set; }
        public int SchoolId { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExportAllStudentsForStaffsSectionsAndCohortsRequest"/> using the specified parameters.
        /// </summary>
        /// <returns>A new <see cref="ExportAllStudentsForStaffsSectionsAndCohortsRequest"/> instance.</returns>
        public static ExportAllStudentsForStaffsSectionsAndCohortsRequest Create(long staffUSI, int schoolId) 
        {
            return new ExportAllStudentsForStaffsSectionsAndCohortsRequest { StaffUSI = staffUSI, SchoolId = schoolId };
        }
    }

    public interface IExportAllStudentsForStaffsSectionsAndCohortsService : IService<ExportAllStudentsForStaffsSectionsAndCohortsRequest, StudentExportAllModel> { }

    public class ExportAllStudentsForStaffsSectionsAndCohortsService : IExportAllStudentsForStaffsSectionsAndCohortsService
    {
        private readonly IRepository<StaffStudentAssociation> staffStudentAssociationRepository;
        private readonly IRepository<StudentSchoolMetricInstanceSet> studentSchoolMetricInstanceSetRepository;
        private readonly IRepository<MetricInstance> metricInstanceRepository;
        private readonly IRepository<StudentInformation> studentInformationRepository;
        private readonly IRootMetricNodeResolver rootMetricNodeResolver;

        public ExportAllStudentsForStaffsSectionsAndCohortsService(
            IRepository<StaffStudentAssociation> staffStudentAssociationRepository, 
            IRepository<StudentSchoolMetricInstanceSet> studentSchoolMetricInstanceSetRepository,
            IRepository<MetricInstance> metricInstanceRepository,
            IRepository<StudentInformation> studentInformationRepository,
            IRootMetricNodeResolver rootMetricNodeResolver
            )
        {
            this.staffStudentAssociationRepository = staffStudentAssociationRepository;
            this.studentSchoolMetricInstanceSetRepository = studentSchoolMetricInstanceSetRepository;
            this.metricInstanceRepository = metricInstanceRepository;
            this.studentInformationRepository = studentInformationRepository;
            this.rootMetricNodeResolver = rootMetricNodeResolver;
        }

        [CanBeAuthorizedBy(EdFiClaimTypes.ViewAllStudents, EdFiClaimTypes.ViewMyStudents)]
        public StudentExportAllModel Get(ExportAllStudentsForStaffsSectionsAndCohortsRequest request)
        {
            long staffUSI = request.StaffUSI;
            int schoolId = request.SchoolId;

            //Lets get the metric tree that applies to this.
            var rootMetricNode = rootMetricNodeResolver.GetRootMetricNodeForStudent(schoolId);

            if (rootMetricNode == null)
                throw new InvalidOperationException("No metric metadata available for exporting a hierarchy.");

            var metricsOfTypeGranular = rootMetricNode.DescendantsOrSelf.Where(y => y.MetricType == MetricType.GranularMetric);

            //Lets get the DATA from the DB...
            var query = (from s in staffStudentAssociationRepository.GetAll()
                         join se in studentSchoolMetricInstanceSetRepository.GetAll() on new { s.SchoolId, s.StudentUSI } equals new { se.SchoolId, se.StudentUSI }
                         join si in studentInformationRepository.GetAll() on s.StudentUSI equals si.StudentUSI
                         join m in metricInstanceRepository.GetAll() on se.MetricInstanceSetKey equals m.MetricInstanceSetKey
                         where s.SchoolId == schoolId && s.StaffUSI == staffUSI
                         select new
                         {
                             s.StudentUSI,
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
