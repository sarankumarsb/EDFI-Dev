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
using EdFi.Dashboards.Resources.StudentSchool;
using EdFi.Dashboards.SecurityTokenService.Authentication;
using MetricType = EdFi.Dashboards.Metric.Resources.Models.MetricType;

namespace EdFi.Dashboards.Resources.School
{
    public class ExportStudentGradeListRequest
    {
        public int SchoolId { get; set; }
        [AuthenticationIgnore("Could be added later, but user probably should have to see ALL students for this school")]
        public string Grade { get; set; }

        public static ExportStudentGradeListRequest Create(int schoolId, string grade)
        {
            return new ExportStudentGradeListRequest { SchoolId = schoolId, Grade = grade };
        }
    }

    public interface IExportStudentGradeListService : IService<ExportStudentGradeListRequest, StudentExportAllModel> { }

    public class ExportStudentGradeListService : IExportStudentGradeListService
    {
        private readonly IRepository<StudentInformation> studentInformationRepository;
        private readonly IRepository<StudentSchoolInformation> studentSchoolInformationRepository;
        private readonly IRepository<MetricInstance> metricInstanceRepository;
        private readonly IRepository<StudentSchoolMetricInstanceSet> studentSchoolMetricInstanceSetRepository;
        private readonly IRootMetricNodeResolver rootMetricNodeResolver;
        private readonly IGradeLevelUtilitiesProvider gradeLevelUtilitiesProvider;


        public ExportStudentGradeListService(IRepository<StudentInformation> studentInformationRepository,
                                             IRepository<StudentSchoolInformation> studentSchoolInformationRepository,
                                             IRepository<StudentSchoolMetricInstanceSet> studentSchoolMetricInstanceSetRepository,
                                             IRepository<MetricInstance> metricInstanceRepository,
                                             IRootMetricNodeResolver rootMetricNodeResolver,
                                             IGradeLevelUtilitiesProvider gradeLevelUtilitiesProvider)
        {
            this.studentInformationRepository = studentInformationRepository;
            this.studentSchoolInformationRepository = studentSchoolInformationRepository;
            this.metricInstanceRepository = metricInstanceRepository;
            this.studentSchoolMetricInstanceSetRepository = studentSchoolMetricInstanceSetRepository;
            this.rootMetricNodeResolver = rootMetricNodeResolver;
            this.gradeLevelUtilitiesProvider = gradeLevelUtilitiesProvider;
        }
        
        [CanBeAuthorizedBy(EdFiClaimTypes.ViewAllStudents, EdFiClaimTypes.ViewMyStudents)]
        public StudentExportAllModel Get(ExportStudentGradeListRequest request)
        {
            //added All-Students to All replace below because at some point in time, changes came into the code base that started sending
            //All-Students instead of All when the list wanted to export the all the students, instead of adding more cluter to the 
            //linq that populates the students object below I put in the following check.
            var grade = request.Grade.Replace("All-Students", "All").Replace('-', ' ').Replace("   ", " - ");

            //Lets get the metric tree that applies to this.
            var rootMetricNode = rootMetricNodeResolver.GetRootMetricNodeForStudent(request.SchoolId);

            if (rootMetricNode == null)
                throw new InvalidOperationException("No metric metadata available to be able to export a hierarchy.");

            var metricsOfTypeGranular = rootMetricNode.DescendantsOrSelf.Where(y => y.MetricType == MetricType.GranularMetric);

            
            //get all the students for this school
            var students = (from s in studentInformationRepository.GetAll()
                                                       join ssi in studentSchoolInformationRepository.GetAll() on s.StudentUSI equals ssi.StudentUSI
                                                       where ssi.SchoolId == request.SchoolId
                                                             && (ssi.GradeLevel == grade || grade == "All")
                                                       select new { s, ssi });

            //Let's get the data that we need.
            var data = (from student in students
                        join si in studentInformationRepository.GetAll() on student.s.StudentUSI equals si.StudentUSI
                        join se in studentSchoolMetricInstanceSetRepository.GetAll() on new { student.ssi.SchoolId, student.s.StudentUSI } equals new { se.SchoolId, se.StudentUSI }
                        join mi in metricInstanceRepository.GetAll() on se.MetricInstanceSetKey equals mi.MetricInstanceSetKey
                        where student.ssi.SchoolId == request.SchoolId
                              && se.SchoolId == request.SchoolId
                        select new
                        {
                            si.StudentUSI,
                            si.FirstName,
                            si.MiddleName,
                            si.LastSurname,
                            student.ssi.GradeLevel,
                            mi.MetricId,
                            mi.Value
                        }).ToList();

            var dataGroupedByStudent = (from s in data
                                        group s by s.StudentUSI
                                            into g
                                            select new
                                            {
                                                g.First().StudentUSI,
                                                g.First().FirstName,
                                                g.First().MiddleName,
                                                g.First().LastSurname,
                                                g.First().GradeLevel,
                                                metric = g
                                            }).OrderBy(x => x.LastSurname).ThenBy(x => x.FirstName);

            var listOfRows = new List<StudentExportAllModel.Row>();
            //For each student that we have we are going to traverse the granular metrics in tree order and add the metrics to the corresponding row slot.
            foreach (var student in dataGroupedByStudent)
            {
                var row = new List<KeyValuePair<string, object>>
                              {
                                  new KeyValuePair<string, object>("Student Name", Utilities.FormatPersonNameByLastName(student.FirstName, student.MiddleName, student.LastSurname)),
                                  new KeyValuePair<string, object>("Grade Level", gradeLevelUtilitiesProvider.FormatGradeLevelForDisplay(student.GradeLevel)),
                              };

                foreach (var granularMetricNode in metricsOfTypeGranular)
                {
                    var studentMetric = student.metric.SingleOrDefault(x => x.MetricId == granularMetricNode.MetricId);

                    row.Add(new KeyValuePair<string, object>(Utilities.Metrics.GetMetricName(granularMetricNode), (studentMetric == null) ? string.Empty : studentMetric.Value));
                }

                listOfRows.Add(new StudentExportAllModel.Row { Cells = row, StudentUSI = student.StudentUSI });
            }

            return new StudentExportAllModel { Rows = listOfRows };
        }
    }
}
