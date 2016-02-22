using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.UI.DataVisualization.Charting;
using EdFi.Dashboards.Core;
using EdFi.Dashboards.Core.Providers.Context;
using EdFi.Dashboards.Metric.Resources.Providers;
using EdFi.Dashboards.Presentation.Core.Models.Shared.Detail;
using EdFi.Dashboards.Resources.Models.Charting;
using EdFi.Dashboards.Resources.StudentSchool.Detail;

namespace EdFi.Dashboards.Presentation.Core.Areas.StudentSchool.Controllers.Detail
{
    public class HistoricalLearningObjectivesChartController : Controller
    {
        private readonly IHistoricalLearningObjectivesChartService historicalLearningObjectiveService;

        public HistoricalLearningObjectivesChartController(IHistoricalLearningObjectivesChartService historicalLearningObjectiveService)
        {
            this.historicalLearningObjectiveService = historicalLearningObjectiveService;
        }

        public virtual ActionResult Get(EdFiDashboardContext context, string title)
        {
            var metricVariantId = context.MetricVariantId.GetValueOrDefault();

            var historicalLearningObjChart = historicalLearningObjectiveService.Get(new HistoricalLearningObjectivesChartRequest()
                                                                                {
                                                                                    StudentUSI = context.StudentUSI.GetValueOrDefault(),
                                                                                    SchoolId = context.SchoolId.GetValueOrDefault(),
                                                                                    MetricVariantId = metricVariantId,
                                                                                    Title = title
                                                                                });

            var chartViewModel = new ChartViewModel
                                     {
                                         ChartData = historicalLearningObjChart,
                                         Width =  740,
                                         Height = 250,
                                         EachSeriesInNewChartArea = false,
                                         ChartType = SeriesChartType.Column,
                                         DisplayLegend = false,
                                         YMin = GetChartDisplayAxisYMin(metricVariantId),
                                         YMax = GetChartDisplayAxisYMax(metricVariantId),
                                         AxisYInterval = GetChartDisplayAxisYInterval(metricVariantId),
                                         AxisYLabelFormat = GetChartDisplayAxisYLabelFormat(metricVariantId),
                                         AxisYCustomLabels = GetChartDisplayAxisYCustomLabels(metricVariantId)
                                     };

            return View(chartViewModel);
        }

        protected virtual double GetChartDisplayAxisYMin(int metricVariantId)
        {
            return 0;
        }

        protected virtual double GetChartDisplayAxisYMax(int metricVariantId)
        {
            return metricVariantId == (int)StudentMetricEnum.DIBELS ? 500 : 1;
        }

        protected virtual double? GetChartDisplayAxisYInterval(int metricVariantId)
        {
            return metricVariantId == (int)StudentMetricEnum.DIBELS ? 100 : (double?)null;
        }

        protected virtual string GetChartDisplayAxisYLabelFormat(int metricVariantId)
        {
            return metricVariantId == (int)StudentMetricEnum.DIBELS ? "{0}" : string.Empty;
        }

        protected virtual List<CustomLabels.CustomLabel> GetChartDisplayAxisYCustomLabels(int metricVariantId)
        {
            return null;
        }

    }
}
