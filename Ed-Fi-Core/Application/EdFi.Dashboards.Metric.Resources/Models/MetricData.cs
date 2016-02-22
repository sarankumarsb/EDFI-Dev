// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System;
using System.Collections.Generic;
using EdFi.Dashboards.Common;
using EdFi.Dashboards.Common.Utilities;
using EdFi.Dashboards.Metric.Data.Entities;

namespace EdFi.Dashboards.Metric.Resources.Models
{
    public class MetricDataContainer
    {
        public MetricDataContainer(IEnumerable<MetricData> metricData )
        {
            this.metricData = metricData;
        }

        private readonly IEnumerable<MetricData> metricData;

        public MetricData GetMetricData(MetricMetadataNode metricMetadataNode)
        {
            foreach (var md in metricData)
            {
                if (md.CanSupplyMetricData(metricMetadataNode))
                    return md;
            }

            throw new NotSupportedException("Can't resolve metric data for metric metadata node '" + metricMetadataNode.MetricNodeId + "'.");
        }
    }

    public abstract class MetricData
    {
        public MetricData() { } // For legacy testing purposes

        private IEnumerable<MetricInstance> metricInstances;
        private IEnumerable<MetricInstanceExtendedProperty> metricInstanceExtendedProperties;
        private IEnumerable<MetricGoal> metricGoals;
        private IEnumerable<Metric.Data.Entities.MetricIndicator> metricIndicators;
        private IEnumerable<MetricInstanceFootnote> metricInstanceFootnotes;
        private IEnumerable<Metric.Data.Entities.MetricComponent> metricComponents;

        public abstract bool CanSupplyMetricData(MetricMetadataNode metricMetadataNode);

        public Dictionary<int, MetricInstance> MetricInstancesByMetricId { get; private set; }

        public IEnumerable<MetricInstance> MetricInstances
        {
            get { return metricInstances; }
            set
            {
                MetricInstancesByMetricId = new Dictionary<int, MetricInstance>();
                MetricInstancesByMetricId.AddRange(value, instance => instance.MetricId);
                metricInstances = value;
            } 
        }
        
        public MultiValueDictionary<int, MetricInstanceExtendedProperty> MetricInstanceExtendedPropertiesByMetricId { get; private set; }

        public IEnumerable<MetricInstanceExtendedProperty> MetricInstanceExtendedProperties
        {
            get { return metricInstanceExtendedProperties; }
            set
            {
                MetricInstanceExtendedPropertiesByMetricId = new MultiValueDictionary<int, MetricInstanceExtendedProperty>().CreateMap(value, instance => instance.MetricId);
                metricInstanceExtendedProperties = value;
            }
        }

        public Dictionary<int, MetricGoal> MetricGoalsByMetricId { get; private set; }

        public IEnumerable<MetricGoal> MetricGoals
        {
            get { return metricGoals; }
            set
            {
                MetricGoalsByMetricId = new Dictionary<int, MetricGoal>();
                MetricGoalsByMetricId.AddRange(value, instance => instance.MetricId);

                metricGoals = value;
            } 
        }

        public MultiValueDictionary<int, Metric.Data.Entities.MetricIndicator> MetricIndicatorsByMetricId { get; private set; }

        public IEnumerable<Metric.Data.Entities.MetricIndicator> MetricIndicators
        {
            get { return metricIndicators; }
            set
            {
                MetricIndicatorsByMetricId = new MultiValueDictionary<int, Metric.Data.Entities.MetricIndicator>().CreateMap(value, instance => instance.MetricId);
                metricIndicators = value;
            }
        }

        public MultiValueDictionary<int, MetricInstanceFootnote> MetricInstanceFootnotesByMetricId { get; private set; }

        public IEnumerable<MetricInstanceFootnote> MetricInstanceFootnotes
        {
            get { return metricInstanceFootnotes; }
            set
            {
                MetricInstanceFootnotesByMetricId = new MultiValueDictionary<int, MetricInstanceFootnote>().CreateMap(value, instance => instance.MetricId);
                metricInstanceFootnotes = value;
            } 
        }

        public IEnumerable<MetricFootnoteDescriptionType> MetricFootnoteDescriptionTypes { get; set; }

        public MultiValueDictionary<int, Metric.Data.Entities.MetricComponent> MetricComponentsByMetricId { get; private set; }

        public IEnumerable<Metric.Data.Entities.MetricComponent> MetricComponents
        {
            get { return metricComponents; }
            set
            {
                MetricComponentsByMetricId = new MultiValueDictionary<int, Metric.Data.Entities.MetricComponent>().CreateMap(value, instance => instance.MetricId);
                metricComponents = value;
            }
        }
    
        public virtual void InitializeEmptyCollections()
        {
            MetricInstancesByMetricId = new Dictionary<int, MetricInstance>();
            metricInstances = new List<MetricInstance>();
            MetricInstanceExtendedPropertiesByMetricId = new MultiValueDictionary<int, MetricInstanceExtendedProperty>();
            metricInstanceExtendedProperties = new List<MetricInstanceExtendedProperty>();
            MetricGoalsByMetricId = new Dictionary<int, MetricGoal>();
            metricGoals = new List<MetricGoal>();
            MetricIndicatorsByMetricId = new MultiValueDictionary<int, Data.Entities.MetricIndicator>();
            metricIndicators = new List<Data.Entities.MetricIndicator>();
            MetricInstanceFootnotesByMetricId = new MultiValueDictionary<int, MetricInstanceFootnote>();
            metricInstanceFootnotes = new List<MetricInstanceFootnote>();
            MetricFootnoteDescriptionTypes = new List<MetricFootnoteDescriptionType>();
            MetricComponentsByMetricId = new MultiValueDictionary<int, Data.Entities.MetricComponent>();
            metricComponents = new List<Data.Entities.MetricComponent>();
        }
    }
}
