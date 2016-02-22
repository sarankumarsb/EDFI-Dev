using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using EdFi.Dashboards.Common;
using EdFi.Dashboards.Data.Entities;
using EdFi.Dashboards.Data.Repository;
using EdFi.Dashboards.Metric.Data.Entities;
using EdFi.Dashboards.Metric.Resources.Models;
using EdFi.Dashboards.Resources.Metric;
using EdFi.Dashboards.Resources.Models.Common;
using EdFi.Dashboards.Resources.Security.Common;
using EdFi.Dashboards.Resources.StudentSchool;
using EdFi.Dashboards.SecurityTokenService.Authentication;
using MetricType = EdFi.Dashboards.Metric.Resources.Models.MetricType;

namespace EdFi.Dashboards.Resources.LocalEducationAgency
{
    public class ExportStudentSchoolCategoryListRequest
    {
        public int LocalEducationAgencyId { get; set; }
        [AuthenticationIgnore("Could be added later, but user probably should have to see ALL students for this school")]
        public string SchoolCategory { get; set; }

        public static ExportStudentSchoolCategoryListRequest Create(int localEducationAgencyId, string schoolCategory)
        {
            return new ExportStudentSchoolCategoryListRequest { LocalEducationAgencyId = localEducationAgencyId, SchoolCategory = schoolCategory };
        }
    }

    public interface IExportStudentSchoolCategoryListService : IService<ExportStudentSchoolCategoryListRequest, StudentExportAllModel> { }

    public class ExportStudentSchoolCategoryListService : IExportStudentSchoolCategoryListService
    {
        private static ConcurrentDictionary<int, IEnumerable<MetricMetadataNode>> schoolRootMetricNode = new ConcurrentDictionary<int, IEnumerable<MetricMetadataNode>>();
        private static readonly object schoolRootMetricNodeLockObject = new object();

        private readonly IRepository<SchoolInformation> schoolInformationRepository;
        private readonly IGradeLevelUtilitiesProvider gradeLevelUtilitiesProvider;
        private readonly IRepository<StudentInformation> studentInformationRepository;
        private readonly IRepository<StudentSchoolInformation> studentSchoolInformationRepository;
        private readonly IRepository<MetricInstance> metricInstanceRepository;
        private readonly IRepository<StudentSchoolMetricInstanceSet> studentSchoolMetricInstanceSetRepository;
        private readonly IRootMetricNodeResolver rootMetricNodeResolver;

        public ExportStudentSchoolCategoryListService(IRepository<StudentInformation> studentInformationRepository,
                                                      IRepository<StudentSchoolInformation> studentSchoolInformationRepository,
                                                      IRepository<StudentSchoolMetricInstanceSet> studentSchoolMetricInstanceSetRepository,
                                                      IRepository<MetricInstance> metricInstanceRepository,
                                                      IRootMetricNodeResolver rootMetricNodeResolver,
                                                      IRepository<SchoolInformation> schoolInformationRepository,
                                                      IGradeLevelUtilitiesProvider gradeLevelUtilitiesProvider)
        {
            this.studentInformationRepository = studentInformationRepository;
            this.studentSchoolInformationRepository = studentSchoolInformationRepository;
            this.metricInstanceRepository = metricInstanceRepository;
            this.studentSchoolMetricInstanceSetRepository = studentSchoolMetricInstanceSetRepository;
            this.rootMetricNodeResolver = rootMetricNodeResolver;
            this.schoolInformationRepository = schoolInformationRepository;
            this.gradeLevelUtilitiesProvider = gradeLevelUtilitiesProvider;
        }
        
        [CanBeAuthorizedBy(EdFiClaimTypes.ViewAllStudents, EdFiClaimTypes.ViewMyStudents)]
        public StudentExportAllModel Get(ExportStudentSchoolCategoryListRequest request)
        {
            var category = request.SchoolCategory.Replace('-', '_').Replace("___", " - ");

            //get all the students for this school
            var students = (from s in studentInformationRepository.GetAll()
                            join ssi in studentSchoolInformationRepository.GetAll() on s.StudentUSI equals ssi.StudentUSI
                            join sir in schoolInformationRepository.GetAll() on ssi.SchoolId equals sir.SchoolId
                            where sir.LocalEducationAgencyId == request.LocalEducationAgencyId && sir.SchoolCategory.Contains(category)
                            select new { s, ssi });

            //Let's get the data that we need.
            var data = (from student in students
                        join si in studentInformationRepository.GetAll() on student.s.StudentUSI equals si.StudentUSI
                        join se in studentSchoolMetricInstanceSetRepository.GetAll() on new { student.ssi.SchoolId, student.s.StudentUSI } equals new { se.SchoolId, se.StudentUSI }
                        join mi in metricInstanceRepository.GetAll() on se.MetricInstanceSetKey equals mi.MetricInstanceSetKey
                        join sir in schoolInformationRepository.GetAll() on student.ssi.SchoolId equals sir.SchoolId
                        where sir.LocalEducationAgencyId == request.LocalEducationAgencyId && student.ssi.SchoolId == sir.SchoolId && se.SchoolId == sir.SchoolId
                        select new
                        {
                            si.StudentUSI,
                            si.FirstName,
                            si.MiddleName,
                            si.LastSurname,
                            student.ssi.GradeLevel,
                            sir.SchoolId,
                            sir.Name,
                            mi.MetricId,
                            mi.Value
                        }).ToList();

            var dataGroupedByStudent = (from s in data
                                        group s by new {s.StudentUSI, s.SchoolId}
                                            into g
                                            select new
                                            {
                                                g.First().StudentUSI,
                                                g.First().FirstName,
                                                g.First().MiddleName,
                                                g.First().LastSurname,
                                                g.First().SchoolId,
                                                g.First().GradeLevel,
                                                g.First().Name,
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
                                  new KeyValuePair<string, object>("School", student.Name)
                              };

                //Lets get the metric tree that applies to this.
                if (!schoolRootMetricNode.ContainsKey(student.SchoolId))
                {
                    lock (schoolRootMetricNodeLockObject)
                    {
                        var rootMetricNode = rootMetricNodeResolver.GetRootMetricNodeForStudent(student.SchoolId);

                        if (rootMetricNode == null)
                            throw new InvalidOperationException("No metric metadata available to be able to export a hierarchy.");

                        schoolRootMetricNode[student.SchoolId] = rootMetricNode.DescendantsOrSelf.Where(y => y.MetricType == MetricType.GranularMetric);
                    }
                }

                var metricsOfTypeGranular = schoolRootMetricNode[student.SchoolId];

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
