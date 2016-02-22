using System;
using System.Collections.Generic;
using System.Web.Mvc;
using EdFi.Dashboards.Core;
using EdFi.Dashboards.Core.Providers.Context;
using EdFi.Dashboards.Metric.Resources.Providers;
using EdFi.Dashboards.Resources.Models.CustomGrid;
using EdFi.Dashboards.Resources.School.Detail;

namespace EdFi.Dashboards.Presentation.Core.Areas.School.Controllers.Detail
{
    public class StaffMetricTableController : Controller
    {
        private readonly IStaffMetricListService service;

        public StaffMetricTableController(IStaffMetricListService service)
        {
            this.service = service;
        }

        public ActionResult Get(EdFiDashboardContext context)
        {
            var metricVariantId = context.MetricVariantId.GetValueOrDefault();
            var results = service.Get(new StaffMetricListRequest() { SchoolId = context.SchoolId.GetValueOrDefault(), MetricVariantId = metricVariantId });

            var model = new GridDataWithFootnotes { MetricFootnotes = results.MetricFootnotes };

            //Grouping headers and underlying columns.
            model.Columns = new List<Column> { 
                new Column{IsVisibleByDefault=true, 
                            IsFixedColumn=true,
                    Children= new List<Column>{
                        new  ImageColumn{ Src = "LeftGrayCorner.png", IsVisibleByDefault=true, IsFixedColumn = true },
                        new TextColumn{DisplayName="Staff", IsVisibleByDefault=true, IsFixedColumn = true},
                        new TextColumn{DisplayName="E-Mail", IsVisibleByDefault=true, IsFixedColumn = true},
                        new TextColumn{DisplayName= results.MetricValueLabel, IsVisibleByDefault=true, IsFixedColumn = true},
                    },
                },
                new TextColumn{DisplayName="Spacer", IsVisibleByDefault=true, IsFixedColumn = true,
                    Children= new List<Column>{new TextColumn{IsVisibleByDefault=true, IsFixedColumn = true}},
                },
                new TextColumn{DisplayName="EXPERIENCE",  IsVisibleByDefault=true,
                    Children= new List<Column>{
                        new TextColumn{DisplayName="Experience", IsVisibleByDefault=true},
                    },
                },
                new TextColumn{DisplayName="Spacer", IsVisibleByDefault=true,
                    Children= new List<Column>{new TextColumn{IsVisibleByDefault=true}},
                },
                new TextColumn{DisplayName="EDUCATION",  IsVisibleByDefault=true,
                    Children= new List<Column>{
                        new TextColumn{DisplayName="Education", IsVisibleByDefault=true},
                    },
                },
            };

            //Create the rows.
            foreach (var t in results.StaffMetrics)
            {
                var row = new List<object>();

                //First cells (Spacer, Staff, Email, Metric)
                row.Add(new CellItem<double> { DV = string.Empty, V = 0 });
                row.Add(new TeacherCellItem<string>
                {
                    V = t.Name,
                    DV = t.Name,
                    TId = t.StaffUSI,
                    LUId = results.UniqueListId,
                    CId = results.SchoolId,
                    Url = t.Href
                });
                row.Add(new EmailCellItem<string> { V = t.Email, M = t.Email });

                var cellType = typeof(CellItem<>).MakeGenericType(new Type[] { t.Value.GetType() });
                dynamic cellForMetricValue = Activator.CreateInstance(cellType);
                cellForMetricValue.V = t.Value;
                cellForMetricValue.DV = t.DisplayValue;
                row.Add(cellForMetricValue);

                //Spacer
                row.Add(new SpacerCellItem<double> { DV = string.Empty, V = 0 });
                row.Add(new YearsOfExperienceCellItem<int> { V = t.Experience, Y = t.Experience });
                //Spacer
                row.Add(new SpacerCellItem<double> { DV = string.Empty, V = 0 });
                row.Add(new HighestLevelOfEducationCellItem<string> { V = t.Education, E = t.Education });

                model.Rows.Add(row);
            }

            return View(model);
        }

    }
}
