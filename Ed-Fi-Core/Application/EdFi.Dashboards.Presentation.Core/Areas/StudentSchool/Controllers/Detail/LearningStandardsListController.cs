using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using EdFi.Dashboards.Core.Providers.Context;
using EdFi.Dashboards.Presentation.Core.Areas.StudentSchool.Models.Detail.LearningStandardsList;
using EdFi.Dashboards.Resources.Models.Student.Detail;
using EdFi.Dashboards.Resources.StudentSchool;
using EdFi.Dashboards.Resources.StudentSchool.Detail;

namespace EdFi.Dashboards.Presentation.Core.Areas.StudentSchool.Controllers.Detail
{
    public class LearningStandardsListController : Controller
    {
        private readonly IStudentObjectiveGradeStandardService service;
        private readonly IGradeLevelUtilitiesProvider gradeLevelUtilitiesProvider;

        public LearningStandardsListController(IStudentObjectiveGradeStandardService service,
            IGradeLevelUtilitiesProvider gradeLevelUtilitiesProvider)
        {
            this.service = service;
            this.gradeLevelUtilitiesProvider = gradeLevelUtilitiesProvider;
        }

        public ActionResult Get(EdFiDashboardContext context)
        {
            var serviceResponseModel = service.Get(StudentObjectiveGradeStandardRequest.Create(
                                                        context.StudentUSI.GetValueOrDefault(),
                                                        context.SchoolId.GetValueOrDefault(),
                                                        context.MetricVariantId.GetValueOrDefault()));

            // map service model to UI grid model
            if (!serviceResponseModel.Any())
            {
                var model = new LearningStandardsModel();
                return View(model);
            }
            else
            {
                var benchmarks = serviceResponseModel.First().Benchmarks;
                var gridSchema = new Dictionary<string, string> { { "mastery", "Mastery of Standard" } };
                var assessementScoreRowValues = new Dictionary<string, LearningStandardsDataItemValue>();
                string gradeCurrent = benchmarks[0].GradeLevel;

                // add list to cache grade-date association
                var gradeDates = new Dictionary<string, string>();
                var gradesToAdd = new List<int>();

                // add benchmark assessments to grid schema
                AddBenchmarkAssessmentsToGridSchema(benchmarks, gridSchema, gradeDates, assessementScoreRowValues);

                //Add the assessment scores row
                var gridData = new List<LearningStandardsDataItem>
                {
                    new LearningStandardsDataItem
                    {
                        name = "Assessment Score",
                        level = 0,
                        tag = "domain-0",
                        values = assessementScoreRowValues
                    }
                };

                //add the rest of the rows
                const int domainCount = 1;
                GetGridData(serviceResponseModel, domainCount, gradeDates, gradesToAdd, gridSchema, gradeCurrent, gridData);

                // add prior grades to schema
                if (gradesToAdd.Count > 0)
                {
                    if (gradesToAdd.Count > 1)
                    {
                        gradesToAdd.Sort();
                        gradesToAdd.Reverse();
                    }

                    foreach (int grade in gradesToAdd)
                    {
                        string level = Resources.Utilities.GetGradeLevelFromSort(grade);
                        gridSchema.Add(GetIdentifier(level), level);
                    }
                }

                // put schema and grid data into wrapping class to send to the view
                var model = new LearningStandardsModel
                {
                    schema = gridSchema,
                    data = gridData
                };

                return View(model);

            }
        }

        private void AddBenchmarkAssessmentsToGridSchema(
            IEnumerable<StudentObjectiveGradeStandardModel.BenchmarkModel> benchmarks, Dictionary<string, string> gridSchema, Dictionary<string, string> gradeDates,
            Dictionary<string, LearningStandardsDataItemValue> assessementScoreRowValues)
        {
            foreach (var benchmark in benchmarks)
            {
                // grade column
                string gradeIdentifier = GetIdentifier(benchmark.GradeLevel);
                gridSchema.Add(gradeIdentifier, benchmark.GradeLevel);

                // assessment columns
                foreach (var assessment in benchmark.Assessments.OrderByDescending(x => x.DateAdministration))
                {
                    string dateCode = assessment.DateAdministration.GetHashCode().ToString(CultureInfo.InvariantCulture);
                    string titleCode = assessment.AssessmentTitle.GetHashCode().ToString(CultureInfo.InvariantCulture);

                    // cache grade-date association
                    if (!gradeDates.ContainsKey(dateCode))
                        gradeDates.Add(dateCode, gradeIdentifier);

                    // add assessment to schema
                    string assessmentIdentifier = gradeIdentifier + ":assessment-" + dateCode + '-' + titleCode;
                    gridSchema.Add(assessmentIdentifier, assessment.DateAdministration.ToShortDateString());
                    assessementScoreRowValues.Add(assessmentIdentifier,
                        new LearningStandardsDataItemValue
                        {
                            displayValue = String.Format("{0:P0}", double.Parse(assessment.Value)),
                            value = assessment.Value,
                            type = "text"
                        });
                }
            }
        }

        private void GetGridData(
            IEnumerable<StudentObjectiveGradeStandardModel> serviceResponseModel, int domainCount, Dictionary<string, string> gradeDates, List<int> gradesToAdd,
            Dictionary<string, string> gridSchema, string gradeCurrent, List<LearningStandardsDataItem> gridData)
        {
            foreach (var studentObjectiveGradeStandard in serviceResponseModel)
            {
                var currentDomain = "domain-" + domainCount++;
                var objectiveValues = new Dictionary<string, LearningStandardsDataItemValue>();
                var gradeItems = new List<LearningStandardsDataItem>();

                GetLevels(gradeDates, gradesToAdd, gridSchema, gradeCurrent, studentObjectiveGradeStandard, objectiveValues, currentDomain, gradeItems);

                var row = new LearningStandardsDataItem
                {
                    name = studentObjectiveGradeStandard.ObjectiveDescription,
                    level = 0,
                    tag = currentDomain,
                    linkToHeaders = "mastery",
                    values = objectiveValues,
                    children = gradeItems
                };

                gridData.Add(row);
            }
        }

        private void GetLevels(Dictionary<string, string> gradeDates, List<int> gradesToAdd, Dictionary<string, string> gridSchema, string gradeCurrent,
            StudentObjectiveGradeStandardModel studentObjectiveGradeStandard, Dictionary<string, LearningStandardsDataItemValue> objectiveValues, string currentDomain,
            List<LearningStandardsDataItem> gradeItems)
        {
            foreach (var grade in studentObjectiveGradeStandard.Grades)
            {
                string gradeIdentifier = GetIdentifier(grade.GradeLevel);
                if (!string.IsNullOrEmpty(grade.Value))
                    objectiveValues.Add(gradeIdentifier,
                        new LearningStandardsDataItemValue {displayValue = grade.Value, type = "text"});
                else
                    objectiveValues.Add(gradeIdentifier,
                        new LearningStandardsDataItemValue {displayValue = GetTemplateType(-1), type = "template"});

                var standards = new List<LearningStandardsDataItem>();
                GetStandards(gradeDates, gradesToAdd, gridSchema, gradeCurrent, currentDomain, grade, gradeIdentifier, standards);

                gradeItems.Add(new LearningStandardsDataItem
                {
                    name = grade.GradeLevel,
                    level = 1,
                    tag = currentDomain + ":" + gradeIdentifier,
                    linkToHeaders = gradeIdentifier + ":",
                    children = standards
                });
            }
        }

        private void GetStandards(Dictionary<string, string> gradeDates, List<int> gradesToAdd, Dictionary<string, string> gridSchema, string gradeCurrent,
            string currentDomain, StudentObjectiveGradeStandardModel.GradeModel grade, string gradeIdentifier, List<LearningStandardsDataItem> standards)
        {
            var standardCount = 0;
            foreach (var standard in grade.Standards)
            {
                var assessmentValues = new Dictionary<string, LearningStandardsDataItemValue>();

                // add mastery
                var masteryItem = new LearningStandardsDataItemValue {type = "template"};
                if (standard.AssessmentLast != null)
                {
                    masteryItem.displayValue =
                        GetTemplateType(standard.AssessmentLast.MetricStateTypeId.GetValueOrDefault());
                    masteryItem.tooltip = standard.AssessmentLast.Value;
                }
                else
                {
                    masteryItem.displayValue = GetTemplateType(-1);
                }

                assessmentValues.Add("mastery", masteryItem);

                // add assessments
                GetAssessments(gradeDates, gradesToAdd, gridSchema, gradeCurrent, grade, gradeIdentifier, standard, assessmentValues);

                standards.Add(new LearningStandardsDataItem
                {
                    name = standard.LearningStandard + "-" + standard.Description,
                    level = 2,
                    tag = currentDomain + ":" + gradeIdentifier + ":standard-" + standardCount++,
                    values = assessmentValues
                });
            }
        }

        private void GetAssessments(Dictionary<string, string> gradeDates, List<int> gradesToAdd, Dictionary<string, string> gridSchema, string gradeCurrent,
            StudentObjectiveGradeStandardModel.GradeModel grade, string gradeIdentifier, StudentObjectiveGradeStandardModel.StandardModel standard, 
            Dictionary<string, LearningStandardsDataItemValue> assessmentValues)
        {
            foreach (var assessment in standard.Assessments)
            {
                // check for date grade association
                string dateCode = assessment.DateAdministration.GetHashCode().ToString(CultureInfo.InvariantCulture);
                string titleCode = assessment.AssessmentTitle.GetHashCode().ToString(CultureInfo.InvariantCulture);
                string assessmentIdentifier = gradeIdentifier + ":assessment-" + dateCode + '-' + titleCode;
                // change grade identifier if out of grade
                if (gradeDates.ContainsKey(dateCode))
                {
                    if (gradeDates[dateCode] != gradeIdentifier)
                    {
                        assessmentIdentifier = gradeDates[dateCode] + ":assessment-" + dateCode + '-' + titleCode;
                        // check for roll-up grade column if out of grade
                        var gradeSort = gradeLevelUtilitiesProvider.FormatGradeLevelForSorting(grade.GradeLevel);
                        if ((!gradesToAdd.Contains(gradeSort)) &&
                            (!gridSchema.ContainsKey(gradeIdentifier)) &&
                            (gradeSort < gradeLevelUtilitiesProvider.FormatGradeLevelForSorting(gradeCurrent)))
                        {
                            gradesToAdd.Add(gradeSort);
                        }
                    }
                }

                if (!assessmentValues.ContainsKey(assessmentIdentifier))
                    assessmentValues.Add(
                        assessmentIdentifier,
                        new LearningStandardsDataItemValue
                        {
                            displayValue = GetTemplateType(assessment.MetricStateTypeId.GetValueOrDefault()),
                            type = "template",
                            tooltip = assessment.Value
                        });
            }
        }

        private string GetIdentifier(string input)
        {
            return input.Replace(" ", "-");
        }

        private string GetTemplateType(int metricStateTypeId)
        {
            switch (metricStateTypeId)
            {
                case 1:
                    return "med";
                case 3:
                    return "low";
                case 6:
                    return "high";
                case -1:
                    return "none";
            }

            return string.Empty;
        }
    }
}
