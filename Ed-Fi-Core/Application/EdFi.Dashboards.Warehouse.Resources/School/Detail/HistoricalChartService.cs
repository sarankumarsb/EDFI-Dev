// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using EdFi.Dashboards.Common;
using EdFi.Dashboards.Common.Utilities;
using EdFi.Dashboards.Resources;
using EdFi.Dashboards.Resources.Metric.Requests;
using EdFi.Dashboards.Data.Repository;
using EdFi.Dashboards.Resources.Models.Charting;
using EdFi.Dashboards.Metric.Resources.Providers;
using EdFi.Dashboards.Metric.Resources.Services;
using EdFi.Dashboards.Metric.Resources.Models;
using EdFi.Dashboards.Resources.Security.Common;
using EdFi.Dashboards.SecurityTokenService.Authentication;
using EdFi.Dashboards.Warehouse.Data.Entities;
using EdFi.Dashboards.Warehouse.Resources.Application;

namespace EdFi.Dashboards.Warehouse.Resources.School.Detail
{
	public class HistoricalChartRequest
    {
        public int SchoolId { get; set; }
        public int MetricVariantId { get; set; }
        [AuthenticationIgnore("Title is not a relevant property outside of forming summaries")]
        public string Title { get; set; }
        
        [AuthenticationIgnore("PeriodId does not affect the results of the request in a way requiring it to be secured.")]
        public int? PeriodId { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="HistoricalChartRequest"/> using the specified parameters.
        /// </summary>
        /// <returns>A new <see cref="HistoricalChartRequest"/> instance.</returns>
        public static HistoricalChartRequest Create(int schoolId, int metricVariantId, int? periodId, string title) 
		{
            return new HistoricalChartRequest { SchoolId = schoolId, MetricVariantId = metricVariantId, PeriodId = periodId, Title = title };
		}
	}

    public interface IHistoricalChartService : IService<HistoricalChartRequest, HistoricalChartModel> { }

    public class HistoricalChartService : IHistoricalChartService
	{
        private const string seriesName = "Historical";
        private const string schoolGoalFormat = "School Goal: {0}";
        private const string intValueType = "System.Int32";
        private const string tooltipFormat = "Value: {0}<br/>{1}<br/>Start Date:  {2}<br/>End Date:  {3}";
        private readonly IRepository<SchoolMetricHistorical> schoolMetricHistoricalRepository;
        private readonly IMetricGoalProvider metricGoalProvider;
        private readonly IMetricInstanceSetKeyResolver<SchoolMetricInstanceSetRequest> metricInstanceSetKeyResolver;
        private readonly IMetricStateProvider metricStateProvider;
        private readonly ITrendRenderingDispositionProvider trendRenderingDispositionProvider;
        private readonly IMetricNodeResolver metricNodeResolver;
        private readonly IWarehouseAvailabilityProvider warehouseAvailabilityProvider;

        public HistoricalChartService(IRepository<SchoolMetricHistorical> schoolMetricHistoricalRepository,
                                        IMetricGoalProvider metricGoalProvider,
                                        IMetricStateProvider metricStateProvider,
                                        IMetricInstanceSetKeyResolver<SchoolMetricInstanceSetRequest> metricInstanceSetKeyResolver,
                                        ITrendRenderingDispositionProvider trendRenderingDispositionProvider,
                                        IMetricNodeResolver metricNodeResolver,
                                        IWarehouseAvailabilityProvider warehouseAvailabilityProvider)
        {
            this.metricStateProvider = metricStateProvider;
            this.metricInstanceSetKeyResolver = metricInstanceSetKeyResolver;
            this.schoolMetricHistoricalRepository = schoolMetricHistoricalRepository;
            this.metricGoalProvider = metricGoalProvider;
            this.trendRenderingDispositionProvider = trendRenderingDispositionProvider;
            this.metricNodeResolver = metricNodeResolver;
            this.warehouseAvailabilityProvider = warehouseAvailabilityProvider;
        }


        [CanBeAuthorizedBy(EdFiClaimTypes.ViewAllMetrics, EdFiClaimTypes.ViewMyMetrics)]
        public HistoricalChartModel Get(HistoricalChartRequest request)
        {
            if (!warehouseAvailabilityProvider.Get())
            {
                return new HistoricalChartModel();
            }

            int schoolId = request.SchoolId;
            int metricVariantId = request.MetricVariantId;
            var metricMetadataNode = metricNodeResolver.GetMetricNodeForSchoolFromMetricVariantId(schoolId, metricVariantId);
            int metricId = metricMetadataNode.MetricId;
            
            if (String.IsNullOrEmpty(metricMetadataNode.Format))
                throw new ArgumentNullException(string.Format("Format is null for metric variant Id:{0}", metricVariantId));

            if (metricMetadataNode.NumeratorDenominatorFormat == null)
                throw new ArgumentNullException(string.Format("Numerator/denominator format is null for metric variant Id:{0}", metricVariantId));

            var availablePeriods = GetAvailablePeriods(schoolId, metricId);

            int periodId;
            if (request.PeriodId == null)
            {
                if (availablePeriods.Count == 0)
                    return new HistoricalChartModel();
                periodId = availablePeriods.Max(x => x.Id);
            }
            else
            {
                periodId = (int)request.PeriodId;
            }

	        var model = new HistoricalChartModel();

			//Getting the AvailablePeriods for this metric...
            model.AvailablePeriods = availablePeriods;

            //Setting the DrillDownTitle..
            model.DrillDownTitle = model.AvailablePeriods.Single(x => x.Id == periodId).Text;

            string valueType;
            //Build the needed series and return the chart data.
            var series = new ChartData.Series
                             {
                                 Name = seriesName,
                                 Points = GetPeriodData(schoolId, metricMetadataNode, periodId, out valueType)
                             };
            model.ChartData.SeriesCollection.Add(series);
            model.ChartData.ChartTitle = metricMetadataNode.Actions.GetChartTitle(request.Title);

            //Get the school goal...
            var metricInstanceSetKey = metricInstanceSetKeyResolver.GetMetricInstanceSetKey(SchoolMetricInstanceSetRequest.Create(schoolId, metricVariantId));
            var metricGoal = metricGoalProvider.GetMetricGoal(metricInstanceSetKey, metricId);
            if (metricGoal != null)
            {
                string schoolGoalValue = metricGoal.Value.ToString();
                if (valueType == intValueType)
                {
                    schoolGoalValue = Convert.ToInt32(metricGoal.Value).ToString();
                }

                //If we have a goal then add a StripLine.
                var stripLine = new ChartData.StripLine
                                    {
                                        Value = Convert.ToDouble(metricGoal.Value),
                                        Tooltip = string.Format(schoolGoalFormat, String.Format(metricMetadataNode.Format, InstantiateValue.FromValueType(schoolGoalValue, valueType))),
                                        IsNegativeThreshold = metricGoal.Interpretation == TrendInterpretation.Inverse,
                                    };
                model.ChartData.StripLines.Add(stripLine);
            }


			return model;
		}

        private List<HistoricalChartModel.PeriodItem> GetAvailablePeriods(int schoolId, int metricId)
		{
            var data = (from d in schoolMetricHistoricalRepository.GetAll()
						where d.SchoolId == schoolId && d.MetricId == metricId
						group d by d.PeriodTypeId into g
						select new
						       	{
						       		PeriodTypeId = g.Key, 
									g.First().PeriodType
						       	});

			return (from p in data.ToList()
                    orderby p.PeriodTypeId
                    select new HistoricalChartModel.PeriodItem
					{
						Id = p.PeriodTypeId,
						Text = p.PeriodType,
					}
				   ).ToList();
		}

        private List<ChartData.Point> GetPeriodData(int schoolId, MetricMetadataNode metricMetadataNode, int periodId, out string valueType)
		{
            var data = (from d in schoolMetricHistoricalRepository.GetAll()
                        where d.SchoolId == schoolId &&
                              d.MetricId == metricMetadataNode.MetricId && 
                              d.PeriodTypeId == periodId 
                        orderby d.PeriodIdentifierId
                        select new
                                   {
                                       d.Value,
                                       d.Numerator,
                                       d.Denominator,
                                       d.StartDate,
                                       d.EndDate,
                                       d.ValueType,
                                       d.Context,
                                       d.TrendDirection,
                                       d.MetricStateTypeId
                                   }).ToList();

            var valueTypeValue = data.FirstOrDefault(x => !String.IsNullOrEmpty(x.ValueType));
            valueType = valueTypeValue != null ? valueTypeValue.ValueType : String.Empty;

            return (from d in data
                    select new ChartData.Point
                                {
                                    Value = (String.IsNullOrEmpty(d.Value)) ? 0 : Convert.ToDouble(d.Value),
                                    ValueType = d.ValueType,
                                    Numerator = d.Numerator,
                                    Denominator = d.Denominator,
                                    Tooltip = String.IsNullOrEmpty(d.Value) ? String.Empty :
                                                            String.Format(tooltipFormat,
                                                                            String.Format(metricMetadataNode.Format, InstantiateValue.FromValueType(d.Value, d.ValueType)),
                                                                            String.Format(metricMetadataNode.NumeratorDenominatorFormat, d.Numerator, d.Denominator),
                                                                            d.StartDate.ToShortDateString(),
                                                                            d.EndDate.ToShortDateString()).Replace(" ", "&nbsp;"),
                                    Label = d.Context,//d.PeriodType + " " + d.PeriodIdentifierId,
                                    Trend = d.TrendDirection.HasValue ? trendRenderingDispositionProvider.GetTrendRenderingDisposition((TrendDirection)d.TrendDirection, (TrendInterpretation)metricMetadataNode.TrendInterpretation.Value) : TrendEvaluation.None,
                                    State = (d.MetricStateTypeId != null) ? (MetricStateType)d.MetricStateTypeId.Value : metricStateProvider.GetState(metricMetadataNode.MetricId, d.Value, d.ValueType).StateType,
                                }).ToList();
		}
	}
}
