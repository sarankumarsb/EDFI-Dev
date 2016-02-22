using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using EdFi.Dashboards.Common;
using EdFi.Dashboards.Data.Entities;
using EdFi.Dashboards.Data.Repository;
using EdFi.Dashboards.Metric.Resources.Providers;
using EdFi.Dashboards.Resources.Application;
using EdFi.Dashboards.Resources.Models.Student.Detail;
using EdFi.Dashboards.Resources.Security.Common;
using EdFi.Dashboards.SecurityTokenService.Authentication;
using EdFi.Dashboards.Warehouse.Data.Entities;

namespace EdFi.Dashboards.Resources.StudentSchool.Detail
{

    // request
    public class StudentObjectiveGradeStandardRequest
    {
        public long StudentUSI { get; set; }
        public int SchoolId { get; set; }
        public int MetricVariantId { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="StudentObjectiveGradeStandardRequest"/> using the specified parameters.
        /// </summary>
        /// <returns>A new <see cref="StudentObjectiveGradeStandardRequest"/> instance.</returns>
        public static StudentObjectiveGradeStandardRequest Create(long studentUSI, int schoolId, int metricVariantId)
        {
            return new StudentObjectiveGradeStandardRequest { StudentUSI = studentUSI, SchoolId = schoolId, MetricVariantId = metricVariantId };
        }
    }

    // interface
    public interface IStudentObjectiveGradeStandardService : IService<StudentObjectiveGradeStandardRequest, List<StudentObjectiveGradeStandardModel>> { }

    // service
    public class StudentObjectiveGradeStandardService : IStudentObjectiveGradeStandardService
    {
        private readonly IStudentMetricLearningStandardMetaDataService serviceObjective;
        private readonly IRepository<StudentSchoolInformation> repositoryStudent;
        private readonly IRepository<StudentMetricLearningStandard> repositoryStandardCurrent;
        private readonly IRepository<StudentMetricLearningStandardHistorical> repositoryStandardHistory;
        private readonly IRepository<StudentMetricBenchmarkAssessment> repositoryBenchmarkCurrent;
        private readonly IRepository<StudentMetricBenchmarkAssessmentHistorical> repositoryBenchmarkHistory;
        private readonly IMetricNodeResolver metricNodeResolver;
        private readonly IGradeLevelUtilitiesProvider gradeLevelUtilitiesProvider;
        private readonly IWarehouseAvailabilityProviderResource providerWarehouseAvailability;

        public StudentObjectiveGradeStandardService(IStudentMetricLearningStandardMetaDataService serviceObjective,
                                                    IRepository<StudentSchoolInformation> repositoryStudent,
                                                    IRepository<StudentMetricBenchmarkAssessment> repositoryBenchmarkCurrent,
                                                    IRepository<StudentMetricLearningStandard> repositoryStandardCurrent,
                                                    IWarehouseAvailabilityProviderResource providerWarehouseAvailability,
                                                    IRepository<StudentMetricBenchmarkAssessmentHistorical> repositoryBenchmarkHistory,
                                                    IRepository<StudentMetricLearningStandardHistorical> repositoryStandardHistory,
                                                    IMetricNodeResolver metricNodeResolver,
                                                    IGradeLevelUtilitiesProvider gradeLevelUtilitiesProvider)
        {
            this.serviceObjective = serviceObjective;
            this.repositoryStudent = repositoryStudent;
            this.repositoryBenchmarkCurrent = repositoryBenchmarkCurrent;
            this.repositoryStandardCurrent = repositoryStandardCurrent;
            this.providerWarehouseAvailability = providerWarehouseAvailability;
            this.repositoryBenchmarkHistory = repositoryBenchmarkHistory;
            this.repositoryStandardHistory = repositoryStandardHistory;
            this.metricNodeResolver = metricNodeResolver;
            this.gradeLevelUtilitiesProvider = gradeLevelUtilitiesProvider;
        }

        [CanBeAuthorizedBy(EdFiClaimTypes.ViewAllStudents, EdFiClaimTypes.ViewMyStudents)]
        public List<StudentObjectiveGradeStandardModel> Get(StudentObjectiveGradeStandardRequest request)
        {
            long studentUsi = request.StudentUSI;
            int schoolId = request.SchoolId;
            int metricVariantId = request.MetricVariantId;
            int metricId = metricNodeResolver.ResolveMetricId(metricVariantId);

            // get student current grade level
            var studentInformation = repositoryStudent.GetAll().SingleOrDefault(x => x.StudentUSI == studentUsi && x.SchoolId == schoolId);
            Debug.Assert(studentInformation != null, "Student Information should not be null.");

            string studentGradeLevel = studentInformation.GradeLevel;
            int studentGradeSortCurrent = gradeLevelUtilitiesProvider.FormatGradeLevelForSorting(studentGradeLevel);

            // get objective meta data model
            var modelObjectives = serviceObjective.Get(new StudentMetricLearningStandardMetaDataRequest { MetricVariantId = metricVariantId }).ToList();

            // get current standard records
            var standardsCurrent = (from data in repositoryStandardCurrent.GetAll()
                                    where data.StudentUSI == studentUsi &&
                                          data.SchoolId == schoolId &&
                                          data.MetricId == metricId
                                    select data
                                    ).ToList();

            // group standards current
            var standardsCurrentGrouped = (from data in standardsCurrent
                                           group data by new { data.LearningObjective, data.GradeLevel, data.LearningStandard }
                                               into grouped
                                               select new StandardCurrent
                                               {
                                                   Objective = grouped.Key.LearningObjective,
                                                   GradeLevel = grouped.Key.GradeLevel,
                                                   Standard = grouped.Key.LearningStandard,
                                                   AssessmentLast = grouped.OrderByDescending(t => t.DateAdministration).First(),
                                                   Assessments = grouped.ToList()
                                               }
                                           ).ToList();

            // translate metric ids for benchmark assessments
            int metricIdBenchmark = GetBenchmarkMetricId(metricId);

            // create benchmark list
            var benchmarkList = new List<StudentObjectiveGradeStandardModel.BenchmarkModel>();

            // get current benchmark records
            CreateCurrentBenchmarks(benchmarkList, studentUsi, schoolId, metricIdBenchmark, studentGradeLevel);

            // get history
            var checkHistory = providerWarehouseAvailability.Get();
            List<StandardHistory> standardsHistoryGrouped = null;
            short yearLast = 0;

            if (checkHistory)
            {
                yearLast = 
                    GetHistory(
                    studentUsi, schoolId, metricId, metricIdBenchmark, yearLast, studentGradeSortCurrent, benchmarkList, modelObjectives, out standardsHistoryGrouped);
            }

            // construct model
            var model = new List<StudentObjectiveGradeStandardModel>();

            // walk objective model and populate
            GetObjectives(modelObjectives, studentGradeSortCurrent, standardsCurrentGrouped, standardsHistoryGrouped, benchmarkList, model, checkHistory, yearLast);

            // return list
            return model;

        }

        private void CreateCurrentBenchmarks(List<StudentObjectiveGradeStandardModel.BenchmarkModel> benchmarkList,
            long studentUsi, int schoolId, int metricIdBenchmark, string studentGradeLevel)
        {
            var benchmarkCurrent = (from data in repositoryBenchmarkCurrent.GetAll()
                where data.StudentUSI == studentUsi &&
                      data.SchoolId == schoolId &&
                      data.MetricId == metricIdBenchmark
                select data).ToList();

            // create model for current benchmarks
            var benchmarkModel = new StudentObjectiveGradeStandardModel.BenchmarkModel
            {
                GradeLevel = studentGradeLevel,
                Assessments =
                    benchmarkCurrent.Select(b => new StudentObjectiveGradeStandardModel.BenchmarkAssessmentModel
                    {
                        DateAdministration = b.Date.Date,
                        AssessmentTitle = b.AssessmentTitle,
                        Version = b.Version,
                        Value = b.Value,
                        ValueType = b.ValueType,
                        TrendDirection = b.TrendDirection
                    }).ToList()
            };
            benchmarkList.Add(benchmarkModel);
        }

        private short GetHistory(long studentUsi, int schoolId, int metricId, int metricIdBenchmark, short yearLast,
            int studentGradeSortCurrent, List<StudentObjectiveGradeStandardModel.BenchmarkModel> benchmarkList, 
            List<StudentMetricLearningStandardMetaDataModel> modelObjectives, out List<StandardHistory> standardsHistoryGrouped)
        {
            // get historical standard records
            var standardsHistory = (from data in repositoryStandardHistory.GetAll()
                where data.StudentUSI == studentUsi &&
                      data.SchoolId == schoolId &&
                      data.MetricId == metricId
                select data).ToList();

            // get benchmark history
            var benchmarkHistory = (from data in repositoryBenchmarkHistory.GetAll()
                where data.StudentUSI == studentUsi &&
                      data.SchoolId == schoolId &&
                      data.MetricId == metricIdBenchmark
                select data).ToList();

            // get latest historical school year
            if (benchmarkHistory.Count > 0)
            {
                yearLast = benchmarkHistory.Max(x => x.SchoolYear);
            }

            // create model for two previous years benchmarks
            if (yearLast > 0)
            {
                CreateHistoricalBenchmarks(benchmarkList, benchmarkHistory, yearLast, studentGradeSortCurrent);
            }

            // group standards history for last two years
            standardsHistoryGrouped = (from data in standardsHistory
                where data.SchoolYear >= yearLast - 1
                group data by new {data.SchoolYear, data.LearningObjective, data.GradeLevel, data.LearningStandard}
                into grouped
                select new StandardHistory
                    {
                        Year = grouped.Key.SchoolYear,
                        Objective = grouped.Key.LearningObjective,
                        GradeLevel = grouped.Key.GradeLevel,
                        GradeSort = gradeLevelUtilitiesProvider.FormatGradeLevelForSorting(grouped.Key.GradeLevel),
                        Standard = grouped.Key.LearningStandard,
                        AssessmentLast = grouped.OrderByDescending(t => t.DateAdministration).First(),
                        Assessments = grouped.ToList()
                    }
                ).ToList();

            // check history against meta-data
            CheckHistoryAgainstMetaData(modelObjectives, standardsHistoryGrouped, metricId, studentGradeSortCurrent);

            return yearLast;
        }

        private static void CreateHistoricalBenchmarks(
            List<StudentObjectiveGradeStandardModel.BenchmarkModel> benchmarkList,
            List<StudentMetricBenchmarkAssessmentHistorical> benchmarkHistory, short yearLast, int studentGradeSortCurrent)
        {
            // last year
            var benchmarkModel = new StudentObjectiveGradeStandardModel.BenchmarkModel
            {
                GradeLevel = Utilities.GetGradeLevelFromSort(studentGradeSortCurrent - 1),
                Assessments = benchmarkHistory.Where(r => r.SchoolYear == yearLast)
                    .Select(b => new StudentObjectiveGradeStandardModel.BenchmarkAssessmentModel
                    {
                        DateAdministration = b.Date.Date,
                        AssessmentTitle = b.AssessmentTitle,
                        Version = b.Version,
                        Value = b.Value,
                        ValueType = b.ValueType,
                        TrendDirection = b.TrendDirection
                    }).ToList()
            };
            benchmarkList.Add(benchmarkModel);

            // previous year
            if (benchmarkHistory.Any(r => r.SchoolYear == yearLast - 1))
            {
                benchmarkModel = new StudentObjectiveGradeStandardModel.BenchmarkModel
                {
                    GradeLevel = Utilities.GetGradeLevelFromSort(studentGradeSortCurrent - 2),
                    Assessments = benchmarkHistory.Where(r => r.SchoolYear == yearLast - 1)
                        .Select(b => new StudentObjectiveGradeStandardModel.BenchmarkAssessmentModel
                        {
                            DateAdministration = b.Date.Date,
                            AssessmentTitle = b.AssessmentTitle,
                            Version = b.Version,
                            Value = b.Value,
                            ValueType = b.ValueType,
                            TrendDirection = b.TrendDirection
                        }).ToList()
                };
                benchmarkList.Add(benchmarkModel);
            }
        }

        private static void CheckHistoryAgainstMetaData(
            List<StudentMetricLearningStandardMetaDataModel> modelObjectives, IEnumerable<StandardHistory> standardsHistoryGrouped, 
            int metricId, int studentGradeSortCurrent)
        {
            foreach (var history in standardsHistoryGrouped.Where(x => x.GradeSort >= studentGradeSortCurrent - 2))
            {
                // find objective
                var objective = modelObjectives.SingleOrDefault(x => x.LearningObjective == history.Objective);

                // If objective exists
                if (objective != null)
                {
                    // Find grade
                    var grade = objective.Grades.SingleOrDefault(x => x.GradeLevel == history.GradeLevel);

                    // If grade exists
                    if (grade != null)
                    {
                        // Find standard
                        var standard = grade.Standards.SingleOrDefault(x => x.LearningStandard == history.Standard);

                        // If standard does not exist
                        if (standard == null)
                        {
                            // Add standard
                            grade.Standards.Add(new StudentMetricLearningStandardMetaDataModel.StandardModel
                            {
                                GradeLevel = history.GradeLevel,
                                LearningStandard = history.Standard,
                                Description = history.Assessments.First().Description
                            });
                        }
                    }
                    else
                    {
                        // Add new grade and standard
                        var newGrade = new StudentMetricLearningStandardMetaDataModel.GradeModel
                        {
                            GradeLevel = history.GradeLevel,
                            GradeSort = history.GradeSort
                        };
                        newGrade.Standards.Add(new StudentMetricLearningStandardMetaDataModel.StandardModel
                        {
                            GradeLevel = history.GradeLevel,
                            LearningStandard = history.Standard,
                            Description = history.Assessments.First().Description
                        });

                        objective.Grades.Add(newGrade);
                    }
                }
                else // create objective
                {
                    // Add new objective, grade, and standard
                    var newObjective = new StudentMetricLearningStandardMetaDataModel(metricId)
                    {
                        LearningObjective = history.Objective
                    };
                    var newGrade = new StudentMetricLearningStandardMetaDataModel.GradeModel
                    {
                        GradeLevel = history.GradeLevel,
                        GradeSort = history.GradeSort
                    };
                    var newStandard = new StudentMetricLearningStandardMetaDataModel.StandardModel
                    {
                        GradeLevel = history.GradeLevel,
                        LearningStandard = history.Standard,
                        Description = history.Assessments.First().Description
                    };
                    newGrade.Standards.Add(newStandard);
                    newObjective.Grades.Add(newGrade);
                    modelObjectives.Add(newObjective);
                }
            }
        }

        private static void GetObjectives(
            IEnumerable<StudentMetricLearningStandardMetaDataModel> modelObjectives, int studentGradeSortCurrent, List<StandardCurrent> standardsCurrentGrouped,
            List<StandardHistory> standardsHistoryGrouped, List<StudentObjectiveGradeStandardModel.BenchmarkModel> benchmarkList, 
            List<StudentObjectiveGradeStandardModel> model, bool checkHistory, short yearLast)
        {
            foreach (var objectiveMeta in modelObjectives.OrderBy(x => x.LearningObjective))
            {
                var objectiveStudent = new StudentObjectiveGradeStandardModel
                {
                    ObjectiveDescription = objectiveMeta.LearningObjective
                };

                GetGradeLevels(studentGradeSortCurrent, standardsCurrentGrouped, standardsHistoryGrouped, objectiveMeta, objectiveStudent, checkHistory, yearLast);

                // add if grades exist
                if (objectiveStudent.Grades.Count > 0)
                {
                    // add benchmarks to objective
                    objectiveStudent.Benchmarks = benchmarkList;
                    // collect objective
                    model.Add(objectiveStudent);
                }
            }
        }

        private static void GetGradeLevels(int studentGradeSortCurrent, List<StandardCurrent> standardsCurrentGrouped,
            List<StandardHistory> standardsHistoryGrouped, StudentMetricLearningStandardMetaDataModel objectiveMeta, 
            StudentObjectiveGradeStandardModel objectiveStudent, bool checkHistory, short yearLast)
        {
            // allow grades for greater than two back
            foreach (
                var gradeMeta in
                    objectiveMeta.Grades.Where(g => g.GradeSort >= studentGradeSortCurrent - 2).OrderBy(x => x.GradeSort))
            {
                var gradeStudent = new StudentObjectiveGradeStandardModel.GradeModel
                {
                    GradeLevel = gradeMeta.GradeLevel,
                    GradeSort = gradeMeta.GradeSort
                };

                GetStandards(standardsCurrentGrouped, standardsHistoryGrouped, gradeMeta, objectiveStudent, gradeStudent,
                    checkHistory, yearLast);

                // roll-up last assessments into objective/grade indicator
                GetIndicator(gradeStudent);

                // add grade
                if ((gradeStudent.GradeSort <= studentGradeSortCurrent) || (!string.IsNullOrEmpty(gradeStudent.Value)))
                {
                    objectiveStudent.Grades.Add(gradeStudent);
                }
            }
        }

        private static void GetStandards(
            List<StandardCurrent> standardsCurrentGrouped, List<StandardHistory> standardsHistoryGrouped, StudentMetricLearningStandardMetaDataModel.GradeModel gradeMeta,
            StudentObjectiveGradeStandardModel objectiveStudent, StudentObjectiveGradeStandardModel.GradeModel gradeStudent, bool checkHistory, short yearLast)
        {
            // add standards
            foreach (var standardMeta in gradeMeta.Standards.OrderBy(x => x.LearningStandard))
            {
                // create standard model for objective / grade
                var standardStudent = new StudentObjectiveGradeStandardModel.StandardModel
                {
                    LearningStandard = standardMeta.LearningStandard,
                    Description = standardMeta.Description,
                    GradeLevel = standardMeta.GradeLevel
                };

                // add current assessments 
                GetCurrentAssessments(standardsCurrentGrouped, objectiveStudent, gradeStudent, standardStudent);

                // add historical assessments
                GetHistoricalAssessments(standardsHistoryGrouped, objectiveStudent, gradeStudent, standardStudent, checkHistory, yearLast);

                // add standard
                gradeStudent.Standards.Add(standardStudent);
            }
        }

        private static void GetCurrentAssessments(IEnumerable<StandardCurrent> standardsCurrentGrouped,
            StudentObjectiveGradeStandardModel objectiveStudent, StudentObjectiveGradeStandardModel.GradeModel gradeStudent,
            StudentObjectiveGradeStandardModel.StandardModel standardStudent)
        {
            var assessmentSearchCurrent = standardsCurrentGrouped.SingleOrDefault(g =>
                g.Objective == objectiveStudent.ObjectiveDescription &&
                g.GradeLevel == gradeStudent.GradeLevel &&
                g.Standard == standardStudent.LearningStandard);

            if (assessmentSearchCurrent != null)
            {
                // add assessments
                foreach (var assessment in assessmentSearchCurrent.Assessments)
                {
                    standardStudent.Assessments.Add(new StudentObjectiveGradeStandardModel.AssessmentModel
                    {
                        AssessmentTitle = assessment.AssessmentTitle,
                        DateAdministration = assessment.DateAdministration.Date,
                        Value = assessment.Value,
                        MetricStateTypeId = assessment.MetricStateTypeId
                    }
                    );
                }

                // get last assessment 
                standardStudent.AssessmentLast = new StudentObjectiveGradeStandardModel.AssessmentModel
                {
                    AssessmentTitle = assessmentSearchCurrent.AssessmentLast.AssessmentTitle,
                    DateAdministration = assessmentSearchCurrent.AssessmentLast.DateAdministration.Date,
                    Value = assessmentSearchCurrent.AssessmentLast.Value,
                    MetricStateTypeId = assessmentSearchCurrent.AssessmentLast.MetricStateTypeId
                };
            }
        }

        private static void GetHistoricalAssessments(
            List<StandardHistory> standardsHistoryGrouped, StudentObjectiveGradeStandardModel objectiveStudent,
            StudentObjectiveGradeStandardModel.GradeModel gradeStudent, StudentObjectiveGradeStandardModel.StandardModel standardStudent,
            bool checkHistory, short yearLast)
        {
            if (checkHistory)
            {
                // for each year
                for (int year = yearLast - 1; year <= yearLast; year++)
                {
                    var assessmentSearchHistory = standardsHistoryGrouped.SingleOrDefault(g =>
                        g.Year == year &&
                        g.Objective == objectiveStudent.ObjectiveDescription &&
                        g.GradeLevel == gradeStudent.GradeLevel &&
                        g.Standard == standardStudent.LearningStandard);

                    if (assessmentSearchHistory != null)
                    {
                        // add assessments
                        foreach (var assessment in assessmentSearchHistory.Assessments)
                        {
                            standardStudent.Assessments.Add(new StudentObjectiveGradeStandardModel.AssessmentModel
                            {
                                    AssessmentTitle = assessment.AssessmentTitle,
                                    DateAdministration = assessment.DateAdministration.Date,
                                    Value = assessment.Value,
                                    MetricStateTypeId = assessment.MetricStateTypeId
                                }
                            );
                        }

                        // get last assessment 
                        if (standardStudent.AssessmentLast == null)
                        {
                            standardStudent.AssessmentLast = new StudentObjectiveGradeStandardModel.AssessmentModel
                            {
                                AssessmentTitle = assessmentSearchHistory.AssessmentLast.AssessmentTitle,
                                DateAdministration = assessmentSearchHistory.AssessmentLast.DateAdministration.Date,
                                Value = assessmentSearchHistory.AssessmentLast.Value,
                                MetricStateTypeId = assessmentSearchHistory.AssessmentLast.MetricStateTypeId
                            };
                        }
                    }
                }
            }
        }

        private static void GetIndicator(StudentObjectiveGradeStandardModel.GradeModel gradeStudent)
        {
            // get count of latest assessments
            int countAssessments = gradeStudent.Standards.Count(a => a.AssessmentLast != null);
            if (countAssessments != 0)
            {
                // identify metric state ids for > 70
                var metricStateIds70 = new[] { 1, 6 };

                // count assessments with score > 70
                int countAssessments70 = gradeStudent.Standards.Where(a => a.AssessmentLast != null)
                    .Count(l => metricStateIds70.Contains(l.AssessmentLast.MetricStateTypeId.GetValueOrDefault()));

                gradeStudent.Value = countAssessments70 + " of " + countAssessments;

                // calculate % > 70
                decimal score = (decimal)countAssessments70 / countAssessments;

                // map % to metric state id
                if (score >= .90m)
                {
                    gradeStudent.MetricStateTypeId = 6;
                }
                else if (score >= .70m)
                {
                    gradeStudent.MetricStateTypeId = 1;
                }
                else
                {
                    gradeStudent.MetricStateTypeId = 3;
                }
            }
        }

        // resolve metric id for benchmark
        public static int GetBenchmarkMetricId(int metricId)
        {
            switch (metricId)
            {
                case 1230:
                    return 74;
                case 1233:
                    return 75;
                case 1234:
                    return 76;
                case 1236:
                    return 80;
                default:
                    return 0;
            }
        }

    }

    public class StandardHistory
    {
        public short Year { get; set; }
        public string Objective { get; set; }
        public string GradeLevel { get; set; }
        public int GradeSort { get; set; }
        public string Standard { get; set; }
        public StudentMetricLearningStandardHistorical AssessmentLast { get; set; }
        public List<StudentMetricLearningStandardHistorical> Assessments { get; set; }
    }

    public class StandardCurrent
    {
        public short Year { get; set; }
        public string Objective { get; set; }
        public string GradeLevel { get; set; }
        public int GradeSort { get; set; }
        public string Standard { get; set; }
        public StudentMetricLearningStandard AssessmentLast { get; set; }
        public List<StudentMetricLearningStandard> Assessments { get; set; }
    }
}
