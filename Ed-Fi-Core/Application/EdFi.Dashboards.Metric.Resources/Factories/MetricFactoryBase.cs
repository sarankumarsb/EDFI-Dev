// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System.Collections.Generic;
using System.Linq;
using EdFi.Dashboards.Common;
using EdFi.Dashboards.Common.Utilities;
using EdFi.Dashboards.Infrastructure;
using EdFi.Dashboards.Metric.Resources.Models;
using EdFi.Dashboards.Metric.Resources.Providers;
using EdFi.Dashboards.Metric.Resources.Services;
using EdFi.Dashboards.Resource.Models.Common;

namespace EdFi.Dashboards.Metric.Resources.Factories
{
    public abstract class MetricFactoryBase<T>
    {
        private readonly IMetricRouteProvider metricRouteProvider;
        private readonly IMetricActionRouteProvider metricActionRouteProvider;
        private readonly ISerializer serializer;
        private readonly IUnderConstructionProvider underConstructionProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="MetricFactoryBase{T}"/> class.
        /// </summary>
        /// <param name="metricRouteProvider">Provider that will generate application-specific routes for the individual metrics.</param>
        /// <param name="metricActionRouteProvider">Gets or sets the provider to generate metric action links.</param>
        /// <param name="serializer">The data serializer.</param>
        /// <param name="underConstructionProvider"></param>
        public MetricFactoryBase(IMetricRouteProvider metricRouteProvider, IMetricActionRouteProvider metricActionRouteProvider,
            ISerializer serializer, IUnderConstructionProvider underConstructionProvider)
        {
            this.metricRouteProvider = metricRouteProvider;
            this.metricActionRouteProvider = metricActionRouteProvider;
            this.serializer = serializer;
            this.underConstructionProvider = underConstructionProvider;
        }

        /// <summary>
        /// Creates a new metric of type T.
        /// </summary>
        /// <param name="metadataNode"></param>
        /// <param name="metricData"></param>
        /// <param name="parent"></param>
        /// <param name="metricInstanceSetRequest"></param>
        /// <returns></returns>
        public abstract T CreateMetric(MetricInstanceSetRequestBase metricInstanceSetRequest, MetricMetadataNode metadataNode, MetricData metricData, MetricBase parent);

        protected void SetMetricBaseMetricMetadataValues(MetricBase metric, MetricInstanceSetRequestBase metricInstanceSetRequest, MetricMetadataNode metadataNode, MetricBase parent)
        {
            metric.MetricNodeId = metadataNode.MetricNodeId;
            metric.MetricVariantId = metadataNode.MetricVariantId;
            metric.MetricId = metadataNode.MetricId;
            metric.MetricType = metadataNode.MetricType;
            metric.Parent = parent;
            metric.Name = metadataNode.Name;
            metric.ShortName = metadataNode.ShortName;
            metric.Description = metadataNode.Description;
            metric.DisplayName = metadataNode.DisplayName;
            metric.DomainEntityType = metadataNode.DomainEntityType;
            metric.MetricVariantType = metadataNode.MetricVariantType;

            //TODO:  This flag should be stored in MetricDB and on the metadataNode, not using this provider.
            metric.IsUnderConstruction = underConstructionProvider.IsMetricUnderConstruction(metric.MetricId);

            metric.ChildDomainEntityMetricId = metadataNode.ChildDomainEntityMetricId;
            metric.Enabled = metadataNode.Enabled;
            metric.Actions = serializer.DeepClone((List<MetricAction>)metadataNode.Actions); // Actions must be copied because they might be "filtered" out by an interceptor based on user's claims
            metric.NumeratorDenominatorFormat = metadataNode.NumeratorDenominatorFormat;
            metric.ToolTip = metadataNode.Tooltip;
            metric.TrendDirection = metadataNode.TrendInterpretation;

            SetMetricLinks(metric, metricInstanceSetRequest, metadataNode);
        }

        protected virtual void SetMetricLinks(MetricBase metric, MetricInstanceSetRequestBase metricInstanceSetRequest, MetricMetadataNode metadataNode)
        {
            // go ahead and calculate all the routes since we'll be doing it anyways at the end of this method
            var links = metricRouteProvider.GetRoute(metricInstanceSetRequest, metadataNode).ToList();

            var webLink = links.FirstOrDefault(l => l.Rel == LinkRel.Web);

            if (webLink != null)
                metric.Url = webLink.Href;

            var selfResourceLink = links.FirstOrDefault(l => l.Rel == LinkRel.AsResource);

            if (selfResourceLink != null)
                metric.ResourceUrl = selfResourceLink.Rel;

            var otherLinks = links.Where(l => l.Rel != LinkRel.AsResource && l.Rel != LinkRel.Web);

            metric.Links = otherLinks.ToList();

            SetMetricActionUrl(metric, metricInstanceSetRequest, metadataNode);
        }

        protected virtual void SetMetricActionUrl(MetricBase metric, MetricInstanceSetRequestBase metricInstanceSetRequest, MetricMetadataNode metadataNode)
        {
            foreach (MetricAction action in metric.Actions)
                action.Url = metricActionRouteProvider.GetRoute(metricInstanceSetRequest, action);
        }

        protected virtual void SetMetricBaseMetricInstanceValues(MetricBase metric, MetricMetadataNode metadataNode, MetricData metricData)
        {
            var instance = metricData.MetricInstancesByMetricId.GetValueOrDefault(metadataNode.MetricId);

            metric.Context = instance == null ? null : instance.Context;

            if (metricData.MetricIndicators != null)
                metric.Indicators = (from m in metricData.MetricIndicatorsByMetricId.ValuesByKey(metadataNode.MetricId)
                                     select new Models.MetricIndicator
                                                {
                                                    IndicatorTypeId = m.IndicatorTypeId,
                                                }).ToList();

            if (metricData.MetricInstanceFootnotes != null)
                metric.Footnotes = (from m in metricData.MetricInstanceFootnotesByMetricId.ValuesByKey(metadataNode.MetricId)
                                    where m.FootnoteTypeId == (int)MetricFootnoteType.MetricFootnote //metric footnotes not drilldown footnotes
                                    select new MetricFootnote
                                               {
                                                   FootnoteTypeId = (MetricFootnoteType)m.FootnoteTypeId,
                                                   FootnoteNumber = m.Count.GetValueOrDefault(),
                                                   FootnoteText = m.FootnoteText
                                               }).ToList();
        }
    }
}