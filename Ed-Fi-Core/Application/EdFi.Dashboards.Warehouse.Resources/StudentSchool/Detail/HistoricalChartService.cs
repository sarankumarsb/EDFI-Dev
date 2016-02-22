// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using EdFi.Dashboards.Common;
using EdFi.Dashboards.Common.Utilities;
using EdFi.Dashboards.Metric.Resources.Models;
using EdFi.Dashboards.Metric.Resources.Providers;
using EdFi.Dashboards.Metric.Resources.Services;
using EdFi.Dashboards.Resources;
using EdFi.Dashboards.Resources.Metric.Requests;
using EdFi.Dashboards.Data.Repository;
using EdFi.Dashboards.Resources.Models.Charting;
using EdFi.Dashboards.Resources.Security.Common;
using EdFi.Dashboards.SecurityTokenService.Authentication;
using EdFi.Dashboards.Warehouse.Data.Entities;
using EdFi.Dashboards.Warehouse.Resources.Application;

namespace EdFi.Dashboards.Warehouse.Resources.StudentSchool.Detail
{
    public class HistoricalChartRequest
    {
        public long StudentUSI { get; set; }
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
        public static HistoricalChartRequest Create(long studentUSI, int schoolId, int metricVariantId, int? periodId, string title) 
		{
            return new HistoricalChartRequest { StudentUSI = studentUSI, SchoolId = schoolId, MetricVariantId = metricVariantId, PeriodId = periodId, Title = title };
		}
	}

    public interface IHistoricalChartService : IService<HistoricalChartRequest, HistoricalChartModel> { }

    public class HistoricalChartService : IHistoricalChartService
    {
        private readonly IRepository<StudentMetricHistorical> studentMetricHistoricalRepository;
        private readonly IMetricGoalProvider metricGoalProvider;
        private readonly IMetricInstanceSetKeyResolver<StudentSchoolMetricInstanceSetRequest> metricInstanceSetKeyResolver;
        private readonly IMetricStateProvider metricStateProvider;
        private readonly ITrendRenderingDispositionProvider trendRenderingDispositionProvider;
        private readonly IMetricNodeResolver metricNodeResolver;
        private readonly IWarehouseAvailabilityProvider warehouseAvailabilityProvider;

        public HistoricalChartService(IRepository<StudentMetricHistorical> studentMetricHistoricalRepository,
                                        IMetricGoalProvider metricGoalProvider,
                                        IMetricStateProvider metricStateProvider,
                                        IMetricInstanceSetKeyResolver<StudentSchoolMetricInstanceSetRequest> metricInstanceSetKeyResolver,
                                        ITrendRenderingDispositionProvider trendRenderingDispositionProvider,
                                        IMetricNodeResolver metricNodeResolver,
                                        IWarehouseAvailabilityProvider warehouseAvailabilityProvider)
        {
            this.metricStateProvider = metricStateProvider;
            this.metricInstanceSetKeyResolver = metricInstanceSetKeyResolver;
            this.studentMetricHistoricalRepository = studentMetricHistoricalRepository;
            this.metricGoalProvider = metricGoalProvider;
            this.trendRenderingDispositionProvider = trendRenderingDispositionProvider;
            this.metricNodeResolver = metricNodeResolver;
            this.warehouseAvailabilityProvider = warehouseAvailabilityProvider;
        }

        [CanBeAuthorizedBy(EdFiClaimTypes.ViewAllStudents, EdFiClaimTypes.ViewMyStudents)]
        public HistoricalChartModel Get(HistoricalChartRequest request)
        {
            if (!warehouseAvailabilityProvider.Get())
            {
                return new HistoricalChartModel();
            }

            var studentUSI = request.StudentUSI;
            int schoolId = request.SchoolId;
            int metricVariantId = request.MetricVariantId;
            var metricMetadataNode = metricNodeResolver.GetMetricNodeForStudentFromMetricVariantId(schoolId, metricVariantId);
            int metricId = metricMetadataNode.MetricId;
            
            if (String.IsNullOrEmpty(metricMetadataNode.Format))
                throw new ArgumentNullException(string.Format("Format is null for metric variant Id:{0}", metricVariantId));

            int periodId;
            if (request.PeriodId == null)
            {

                var availablePeriods = GetAvailablePeriods(studentUSI, metricId);

                if (availablePeriods.Count == 0)
                    return new HistoricalChartModel();

                //For the default period we get the max one available.
                periodId = availablePeriods.Max(x => x.Id);
            }
            else
            {
                periodId = (int)request.PeriodId;
            }

            var model = new HistoricalChartModel
                            {
                                //Getting the AvailablePeriods for this metric...
                                AvailablePeriods = GetAvailablePeriods(studentUSI, metricId)
                            };
            //Setting the DrillDownTitle..
            model.DrillDownTitle = model.AvailablePeriods.Single(x => x.Id == periodId).Text;
            model.ChartData.ChartTitle = metricMetadataNode.Actions.GetChartTitle(request.Title);

            string valueType;
            //Build the needed series and return the chart data.
            var series = new ChartData.Series
                             {
                                 Name = "Historical", Points = GetPeriodData(studentUSI, metricMetadataNode, periodId, out valueType)
                             };
            model.ChartData.SeriesCollection.Add(series);

            //Get the metric Goal...
            var metricInstanceSetKey = metricInstanceSetKeyResolver.GetMetricInstanceSetKey(StudentSchoolMetricInstanceSetRequest.Create(schoolId, studentUSI, metricVariantId));
            var metricGoal = metricGoalProvider.GetMetricGoal(metricInstanceSetKey,metricId);
            if (metricGoal != null)
            {
                var schoolGoalValue = metricGoal.Value.ToString();
                if (valueType == "System.Int32")
                {
                    schoolGoalValue = Convert.ToInt32(metricGoal.Value).ToString();
                }

                //If we have a goal then add a StripLine.
                var stripLine = new ChartData.StripLine
                                        {
                                            Value = Convert.ToDouble(metricGoal.Value),
                                            Tooltip = string.Format("School Goal: {0}", String.Format(metricMetadataNode.Format, InstantiateValue.FromValueType(schoolGoalValue, valueType))),
                                            IsNegativeThreshold = metricGoal.Interpretation==TrendInterpretation.Inverse
                                        };
                model.ChartData.StripLines.Add(stripLine);
            }

            return model;
        }

        private List<HistoricalChartModel.PeriodItem> GetAvailablePeriods(long studentUSI, int metricId)
        {
            var data = (from d in studentMetricHistoricalRepository.GetAll()
                        where d.StudentUSI == studentUSI 
                                && d.MetricId == metricId
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

        private List<ChartData.Point> GetPeriodData(long studentUSI, MetricMetadataNode metricMetadataNode, int periodId, out string valueType)
        {
            var data = (from d in studentMetricHistoricalRepository.GetAll()
                        where d.StudentUSI == studentUSI 
                            && d.MetricId == metricMetadataNode.MetricId 
                            && d.PeriodTypeId == periodId
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
                        Value = (string.IsNullOrEmpty(d.Value)) ? 0 : Convert.ToDouble(d.Value),
                        ValueType = d.ValueType,
                        Numerator = d.Numerator,
                        Denominator = d.Denominator,
                        Tooltip = String.IsNullOrEmpty(d.Value) ? String.Empty :
                                                String.Format("Value: {0}<br/>Start Date:  {1}<br/>End Date:  {2}",
                                                               String.Format(metricMetadataNode.Format, InstantiateValue.FromValueType(d.Value, d.ValueType)),
                                                               d.StartDate.ToShortDateString(),
                                                               d.EndDate.ToShortDateString()).Replace(" ", "&nbsp;"),
                        Label = d.Context,//d.PeriodType + " " + d.PeriodIdentifierId,
                        Trend = d.TrendDirection.HasValue ? trendRenderingDispositionProvider.GetTrendRenderingDisposition((TrendDirection)d.TrendDirection, (TrendInterpretation)metricMetadataNode.TrendInterpretation.Value) : TrendEvaluation.None,
                        State = (d.MetricStateTypeId != null) ? (MetricStateType)d.MetricStateTypeId.Value : metricStateProvider.GetState(metricMetadataNode.MetricId, d.Value, d.ValueType).StateType,
                    }).ToList();
        }
    }
}
