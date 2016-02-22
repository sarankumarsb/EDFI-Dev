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
    public class ExportAllStudentMetricsForStaffSectionRequest
    {
        public long StaffUSI { get; set; }
        public int SchoolId { get; set; }
        public long TeacherSectionId { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExportAllStudentMetricsForStaffSectionRequest"/> using the specified parameters.
        /// </summary>
        /// <returns>A new <see cref="ExportAllStudentMetricsForStaffSectionRequest"/> instance.</returns>
        public static ExportAllStudentMetricsForStaffSectionRequest Create(long staffUSI, int schoolId, long teacherSectionId)
        {
            return new ExportAllStudentMetricsForStaffSectionRequest { StaffUSI = staffUSI, SchoolId = schoolId, TeacherSectionId = teacherSectionId };
        }
    }

    public interface IExportAllStudentMetricsForStaffSectionService : IService<ExportAllStudentMetricsForStaffSectionRequest, StudentExportAllModel> { }

    public class ExportAllStudentMetricsForStaffSectionService : IExportAllStudentMetricsForStaffSectionService
    {
        private readonly IRepository<TeacherStudentSection> teacherStudentSectionRepository;
        private readonly IRepository<TeacherSection> teacherSectionRepository;
        private readonly IRepository<StudentSchoolMetricInstanceSet> studentSchoolMetricInstanceSetRepository;
        private readonly IRepository<MetricInstance> metricInstanceRepository;
        private readonly IRepository<StudentInformation> studentInformationRepository;
        private readonly IRootMetricNodeResolver rootMetricNodeResolver;

        public ExportAllStudentMetricsForStaffSectionService(
            IRepository<TeacherStudentSection> teacherStudentSectionRepository,
            IRepository<TeacherSection> teacherSectionRepository,
            IRepository<StudentSchoolMetricInstanceSet> studentSchoolMetricInstanceSetRepository,
            IRepository<MetricInstance> metricInstanceRepository,
            IRepository<StudentInformation> studentInformationRepository,
            IRootMetricNodeResolver rootMetricNodeResolver)
        {
            this.teacherStudentSectionRepository = teacherStudentSectionRepository;
            this.teacherSectionRepository = teacherSectionRepository;
            this.studentSchoolMetricInstanceSetRepository = studentSchoolMetricInstanceSetRepository;
            this.metricInstanceRepository = metricInstanceRepository;
            this.studentInformationRepository = studentInformationRepository;
            this.rootMetricNodeResolver = rootMetricNodeResolver;
        }

        [CanBeAuthorizedBy(EdFiClaimTypes.ViewAllStudents, EdFiClaimTypes.ViewMyStudents)]
        public StudentExportAllModel Get(ExportAllStudentMetricsForStaffSectionRequest request)
        {
            int schoolId = request.SchoolId;
            long teacherSectionId = request.TeacherSectionId;
            long staffUSI = request.StaffUSI;


            //Lets get the metric tree that applies to this.
            var rootMetricNode = rootMetricNodeResolver.GetRootMetricNodeForStudent(schoolId);

            if (rootMetricNode == null)
                throw new InvalidOperationException("No metric metadata available for exporting a hierarchy.");

            var metricsOfTypeGranular = rootMetricNode.DescendantsOrSelf.Where(y => y.MetricType == MetricType.GranularMetric);

            //Lets get the DATA from the DB...
            var query = (from ts in teacherSectionRepository.GetAll()
                         join tss in teacherStudentSectionRepository.GetAll() on ts.TeacherSectionId equals tss.TeacherSectionId
                         join se in studentSchoolMetricInstanceSetRepository.GetAll() on new { ts.SchoolId, tss.StudentUSI } equals new { se.SchoolId, se.StudentUSI }
                         join si in studentInformationRepository.GetAll() on tss.StudentUSI equals si.StudentUSI
                         join m in metricInstanceRepository.GetAll() on se.MetricInstanceSetKey equals m.MetricInstanceSetKey
                         where ts.SchoolId == schoolId 
                         && ts.StaffUSI == staffUSI
                         && ts.TeacherSectionId == teacherSectionId
                         && se.SchoolId == schoolId
                         select new
                         {
                             tss.StudentUSI,
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
