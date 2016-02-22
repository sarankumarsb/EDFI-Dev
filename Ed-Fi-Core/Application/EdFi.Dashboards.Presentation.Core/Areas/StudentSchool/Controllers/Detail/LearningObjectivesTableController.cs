using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using EdFi.Dashboards.Core.Providers.Context;
using EdFi.Dashboards.Metric.Resources.Models;
using EdFi.Dashboards.Resources.Models.CustomGrid;
using EdFi.Dashboards.Resources.StudentSchool.Detail;

namespace EdFi.Dashboards.Presentation.Core.Areas.StudentSchool.Controllers.Detail
{
    public class LearningObjectivesTableController : Controller
    {
        private ILearningObjectiveService service;

        public LearningObjectivesTableController(ILearningObjectiveService service)
        {
            this.service = service;
        }

        public ActionResult Get(EdFiDashboardContext context)
        {
            var learningObjectives = service.Get(new LearningObjectiveRequest()
                                        {
                                            StudentUSI = context.StudentUSI.GetValueOrDefault(),
                                            SchoolId = context.SchoolId.GetValueOrDefault(),
                                            MetricVariantId = context.MetricVariantId.GetValueOrDefault()
                                        });


            var model = new GridTable { MetricVariantId = context.MetricVariantId.GetValueOrDefault() };

            if (learningObjectives == null || learningObjectives.AssessmentTitles.Count == 0)
            {
                return View(model);
            }

            var orderedAssessmentTitles = GetOrderedAssessmentTitles(learningObjectives.AssessmentTitles);

            #region Headers
            model.Columns = new List<Column>{
                new Column { IsVisibleByDefault = true, IsFixedColumn = true,
                Children= new List<Column>{
                        new ImageColumn{ Src = "LeftGrayCorner.png", IsVisibleByDefault=true, IsFixedColumn = true },
                        new TextColumn{ DisplayName = learningObjectives.InventoryName, IsVisibleByDefault=true, IsFixedColumn = true},
                    }
                },
                new TextColumn{DisplayName="Spacer",  IsVisibleByDefault=true, IsFixedColumn = true,
                    Children= new List<Column>{
                        new TextColumn{ IsVisibleByDefault=true, IsFixedColumn = true},
                    }
                }
            };

            //For the Dynamic Columns (this is the Header which is empty "no text")
            var parentColumn = new TextColumn { DisplayName = "Reading Assessments", IsVisibleByDefault = true };
            foreach (var learningObjectiveTitle in orderedAssessmentTitles)
            {
                parentColumn.Children.Add(new TextColumn { DisplayName = learningObjectiveTitle, IsVisibleByDefault = true });
            }

            model.Columns.Add(parentColumn);

            #endregion

            #region Rows

            var orderedLearningObjectives = learningObjectives.LearningObjectiveSkills.OrderBy(los => los.SectionName).ThenBy(los => los.SkillName);

            foreach (var learningObjectiveSection in orderedLearningObjectives)
            {
                var row = new List<object>();

                //Learning Objective section cell
                row.Add(new CellItem<double> { DV = "", V = 0 });
                row.Add(new ObjectiveTextCellItem<string> { O = string.Format("{0}: {1}", learningObjectiveSection.SectionName, learningObjectiveSection.SkillName), V = string.Format("{0}: {1}", learningObjectiveSection.SectionName, learningObjectiveSection.SkillName) });

                //Spacer
                row.Add(new SpacerCellItem<double> { DV = "", V = 0 });

                foreach (var learningObjectiveAssessmentTitle in orderedAssessmentTitles)
                {
                    var cell = new ObjectiveCellItem<int> { DV = "", V = -1 };

                    if (learningObjectiveSection.SkillValues.Any(skill => skill.Title == learningObjectiveAssessmentTitle))
                    {
                        var learningObjectiveSkill = learningObjectiveSection.SkillValues.Where(skill => skill.Title == learningObjectiveAssessmentTitle).First();

                        cell.V = ChangeValueForSorting(learningObjectiveSkill.MetricStateTypeId);
                        cell.ST = (learningObjectiveSkill.MetricStateTypeId != null) ? learningObjectiveSkill.MetricStateTypeId.Value : (int)MetricStateType.None;
                        cell.DV = learningObjectiveSkill.Value;
                    }

                    row.Add(cell);
                }

                model.Rows.Add(row);
            }
            #endregion

            return View(model);
        }

        private static int ChangeValueForSorting(int? metricStateId)
        {
            if (metricStateId == null)
                return 99;
            if (metricStateId == 6)
                return 0;
            if (metricStateId == 0)
                return 99;
            return metricStateId.Value;
        }

        private IList<string> GetOrderedAssessmentTitles(IList<string> assessmentTitles)
        {
            var orderedAssessmentTitles = new List<string>();

            if (assessmentTitles.Contains("BOY"))
                orderedAssessmentTitles.Add("BOY");

            if (assessmentTitles.Contains("MOY"))
                orderedAssessmentTitles.Add("MOY");

            if (assessmentTitles.Contains("EOY"))
                orderedAssessmentTitles.Add("EOY");

            return orderedAssessmentTitles;
        }
    }
}
