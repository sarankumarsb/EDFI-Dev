using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using EdFi.Dashboards.Common;
using EdFi.Dashboards.Core;
using EdFi.Dashboards.Data.Entities;
using EdFi.Dashboards.Data.Repository;
using EdFi.Dashboards.Metric.Data.Entities;
using EdFi.Dashboards.Resource.Models.Common;
using EdFi.Dashboards.Resources.Common;
using EdFi.Dashboards.Resources.Metric;
using EdFi.Dashboards.Resources.Models.Common;
using EdFi.Dashboards.Resources.Models.CustomGrid;
using EdFi.Dashboards.Resources.Models.School;
using EdFi.Dashboards.Resources.Models.Student;
using EdFi.Dashboards.Resources.Navigation;
using EdFi.Dashboards.Resources.Security.Common;
using EdFi.Dashboards.Resources.Staff;
using EdFi.Dashboards.Resources.StudentSchool;
using EdFi.Dashboards.SecurityTokenService.Authentication;
using MetricType = EdFi.Dashboards.Metric.Resources.Models.MetricType;

namespace EdFi.Dashboards.Resources.School
{
    public class ExportStudentDemographicListRequest
    {
        public int SchoolId { get; set; }
        [AuthenticationIgnore("Could be added later, but user probably should have to see ALL students for this school")]
        public string Demographic { get; set; }

        public static ExportStudentDemographicListRequest Create(int schoolId, string demographic)
        {
            return new ExportStudentDemographicListRequest { SchoolId = schoolId, Demographic = demographic };
        }
    }

    public interface IExportStudentDemographicListService : IService<ExportStudentDemographicListRequest, StudentExportAllModel> { }

    public class ExportStudentDemographicListService : IExportStudentDemographicListService
    {
        private readonly IRepository<StudentInformation> studentInformationRepository;
        private readonly IRepository<StudentSchoolInformation> studentSchoolInformationRepository;
        private readonly IRepository<StudentIndicator> studentIndicatorRepository;
        private readonly IRepository<MetricInstance> metricInstanceRepository;
        private readonly IRepository<StudentSchoolMetricInstanceSet> studentSchoolMetricInstanceSetRepository;
        private readonly IRootMetricNodeResolver rootMetricNodeResolver;
        private readonly IGradeLevelUtilitiesProvider gradeLevelUtilitiesProvider;

        private const string femaleStr = "Female";
        private const string maleStr = "Male";
        private const string hispanicStr = "Hispanic/Latino";
        private const string twoOrMoreStr = "Two or More";
        private const string lateEnrollmentStr = "Late Enrollment";
        private const string yesStr = "YES";

        public ExportStudentDemographicListService(IRepository<StudentInformation> studentInformationRepository,
                                             IRepository<StudentSchoolInformation> studentSchoolInformationRepository,
                                             IRepository<StudentIndicator> studentIndicatorRepository,
                                             IRepository<StudentSchoolMetricInstanceSet> studentSchoolMetricInstanceSetRepository,
                                             IRepository<MetricInstance> metricInstanceRepository,
                                             IRootMetricNodeResolver rootMetricNodeResolver,
                                             IGradeLevelUtilitiesProvider gradeLevelUtilitiesProvider)
        {
            this.studentInformationRepository = studentInformationRepository;
            this.studentSchoolInformationRepository = studentSchoolInformationRepository;
            this.studentIndicatorRepository = studentIndicatorRepository;
            this.metricInstanceRepository = metricInstanceRepository;
            this.studentSchoolMetricInstanceSetRepository = studentSchoolMetricInstanceSetRepository;
            this.rootMetricNodeResolver = rootMetricNodeResolver;
            this.gradeLevelUtilitiesProvider = gradeLevelUtilitiesProvider;
        }
        
        [CanBeAuthorizedBy(EdFiClaimTypes.ViewOperationalDashboard, EdFiClaimTypes.ViewAllMetrics, EdFiClaimTypes.ViewMyStudents)]
        public StudentExportAllModel Get(ExportStudentDemographicListRequest request)
        {

            var demographic = request.Demographic.Replace('-', ' ').Replace("   ", " - ");
            demographic = demographic.Replace("Hispanic Latino", "Hispanic/Latino");
            demographic = demographic.Replace("Gifted Talented", "Gifted/Talented");

            //Lets get the metric tree that applies to this.
            var rootMetricNode = rootMetricNodeResolver.GetRootMetricNodeForStudent(request.SchoolId);

            if (rootMetricNode == null)
                throw new InvalidOperationException("No metric metadata available to be able to export a hierarchy.");

            var metricsOfTypeGranular = rootMetricNode.DescendantsOrSelf.Where(y => y.MetricType == MetricType.GranularMetric);

            
            //get all the students for this school
            var students = (from s in studentInformationRepository.GetAll()
                                                       join ssi in studentSchoolInformationRepository.GetAll() on s.StudentUSI equals ssi.StudentUSI
                                                       where ssi.SchoolId == request.SchoolId
                                                       select new { s, ssi });


            //decide which student list to get based on demographic
            switch (demographic)
            {
                case hispanicStr:
                    students = students.Where(student => student.s.HispanicLatinoEthnicity == yesStr);
                    break;
                case femaleStr:
                case maleStr:
                    students = students.Where(student => student.s.Gender == demographic);
                    break;
                case lateEnrollmentStr:
                    students = students.Where(student => student.ssi.LateEnrollment == yesStr);
                    break;
                case twoOrMoreStr:
                    students = students.Where(student => student.s.Race != null && student.s.Race.Contains(",") && student.s.HispanicLatinoEthnicity != yesStr);
                    break;
                default:
                    students = students.Where(x => (x.s.Race != null && x.s.Race == demographic && x.s.HispanicLatinoEthnicity != yesStr) || studentIndicatorRepository.GetAll().Any(si => x.s.StudentUSI == si.StudentUSI && si.Status && si.Name == demographic));
                    break;
            }

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
