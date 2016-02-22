using System.Collections.Generic;
using System.Web.Mvc;
using EdFi.Dashboards.Core.Providers.Context;
using EdFi.Dashboards.Metric.Resources.Models;
using EdFi.Dashboards.Resources.Models.CustomGrid;
using EdFi.Dashboards.Resources.StudentSchool.Detail;

namespace EdFi.Dashboards.Presentation.Core.Areas.StudentSchool.Controllers.Detail
{
    public class LearningStandardsTableController : Controller
    {
        private ILearningStandardService service;

        public LearningStandardsTableController(ILearningStandardService service)
        {
            this.service = service;
        }

        protected virtual string StudentExpectationsColumnHeading
        {
            get { return "Student Expectations";  }
        }

        protected virtual string BenchmarkAssessmentsColumnHeading
        {
            get { return "Benchmark Assessments"; }
        }

        public ActionResult Get(EdFiDashboardContext context)
        {
            var learningStandards = service.Get(new LearningStandardRequest()
            {
                StudentUSI = context.StudentUSI.GetValueOrDefault(),
                SchoolId = context.SchoolId.GetValueOrDefault(),
                MetricVariantId = context.MetricVariantId.GetValueOrDefault()
            });


            var model = new GridTable { MetricVariantId = context.MetricVariantId.GetValueOrDefault() };
            if (learningStandards == null || learningStandards.Count == 0)
            {
                return View(model);
            }

            /*Preparing headers*/
            #region Headers
            model.Columns = new List<Column>{
				new Column { IsVisibleByDefault = true, IsFixedColumn = true,
                Children= new List<Column>{
						new ImageColumn{ Src = "LeftGrayCorner.png", IsVisibleByDefault=true, IsFixedColumn = true},
                        new TextColumn{ DisplayName = StudentExpectationsColumnHeading, IsVisibleByDefault=true, IsFixedColumn = true},
                    }
                },
				new TextColumn{DisplayName="Spacer",  IsVisibleByDefault=true, IsFixedColumn = true,
                    Children= new List<Column>{
                        new TextColumn{IsVisibleByDefault=true, IsFixedColumn = true},
                    }
				}
			};

            //For the Dynamic Columns (this is the Header which is empty "no text")
            var parentColumn = new TextColumn { DisplayName = BenchmarkAssessmentsColumnHeading, IsVisibleByDefault = true };
            foreach (var learningStandardAssessment in learningStandards[0].Assessments)
                parentColumn.Children.Add(new TextColumn { DisplayName = learningStandardAssessment.DateAdministration.ToString("MMMM d, yyyy"), Tooltip = learningStandardAssessment.AssessmentTitle, IsVisibleByDefault = true });

            model.Columns.Add(parentColumn);
            #endregion

            #region Rows
            foreach (var learningStandard in learningStandards)
            {
                var row = new List<object>();
                //First cells (Spacer,Expectation,Spacer)
                row.Add(new CellItem<double> { DV = "", V = 0 });
                row.Add(new ObjectiveTextCellItem<string> { O = learningStandard.LearningStandard + " " + learningStandard.Description, V = learningStandard.LearningStandard });
                //Spacer
                row.Add(new SpacerCellItem<double> { DV = "", V = 0 });

                foreach (var learningStandardAssessment in learningStandard.Assessments)
                {
                    var cell = new ObjectiveCellItem<int> { DV = "", V = -1 };
                    if (learningStandardAssessment.Administered)
                    {
                        cell.V = ChangeValueForSorting(learningStandardAssessment.MetricStateTypeId);
                        cell.ST = (learningStandardAssessment.MetricStateTypeId != null)
                                    ? learningStandardAssessment.MetricStateTypeId.Value
                                    : (int)MetricStateType.None;
                        cell.DV = learningStandardAssessment.Value;

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
    }
}
