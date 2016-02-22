using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EdFi.Dashboards.Core;
using EdFi.Dashboards.Metric.Data.Entities;
using EdFi.Dashboards.Metric.Resources.Models;
using EdFi.Dashboards.Resources.Common;
using EdFi.Dashboards.Resources.Models.Charting;
using MetricStateType = EdFi.Dashboards.Metric.Resources.Models.MetricStateType;

namespace EdFi.Dashboards.Resources.StudentSchool.Detail
{
    public interface IHistoricalLearningObjectiveProvider
    {
        string GetCampusGoal(int metricVariantId, double baseLineValue);
        ChartData.StripLine GetStripeLine(int metricVariantId, double baseLineValue);
        double GetDataPointValue(int metricVariantId, MetricInstanceExtendedProperty property);
        string GetDataPointToolTip(int metricVariantId, MetricInstanceExtendedProperty property, IEnumerable<MetricInstanceExtendedProperty> numeratorData, IEnumerable<MetricInstanceExtendedProperty> denominatorData);
        MetricStateType GetDataPointState(int metricVariantId, MetricInstanceExtendedProperty property, double baseLineValue);
        string GetDataPointLabel(int metricVariantId, MetricInstanceExtendedProperty property);
    }

    //TODO: This should be a chain of responsibility for handling the different metric ids
    // right now it just uses inheritance
    public class HistoricalLearningObjectiveProvider : IHistoricalLearningObjectiveProvider
    {
        protected const string CampusGoalFormat = "School Goal: {0}";
        protected const string ValueFormat = "{0:P1}";

        public virtual string GetCampusGoal(int metricVariantId, double baseLineValue)
        {
            if (metricVariantId == (int)StudentMetricEnum.DIBELS)
                return null;

            return String.Format(CampusGoalFormat, String.Format(ValueFormat, baseLineValue));
        }

        public virtual ChartData.StripLine GetStripeLine(int metricVariantId, double baseLineValue)
        {
            if (metricVariantId == (int)StudentMetricEnum.DIBELS)
                return null;

            return new ChartData.StripLine() { Value = baseLineValue, Tooltip = GetCampusGoal(metricVariantId, baseLineValue) };
        }

        public virtual double GetDataPointValue(int metricVariantId, MetricInstanceExtendedProperty property)
        {
            return Convert.ToDouble(property.Value);
        }

        public virtual string GetDataPointToolTip(int metricVariantId, MetricInstanceExtendedProperty property, IEnumerable<MetricInstanceExtendedProperty> numeratorData, IEnumerable<MetricInstanceExtendedProperty> denominatorData)
        {
            switch (metricVariantId)
            {
                case (int)StudentMetricEnum.DIBELS:
                    return property.Value;
            }

            var doubleValue = Convert.ToDouble(property.Value);
            return String.Format(ValueFormat, doubleValue);
        }

        public virtual MetricStateType GetDataPointState(int metricVariantId, MetricInstanceExtendedProperty property, double baseLineValue)
        {
            switch (metricVariantId)
            {
                case (int)StudentMetricEnum.DIBELS:
                    return (MetricStateType)baseLineValue;
            }

            return MetricStateType.None;
        }

        public virtual string GetDataPointLabel(int metricVariantId, MetricInstanceExtendedProperty property)
        {
            switch (metricVariantId)
            {
                case (int)StudentMetricEnum.DIBELS:
                    return property.Value;
            }

            var doubleValue = Convert.ToDouble(property.Value);
            return String.Format(ValueFormat, doubleValue);
        }
    }
}
