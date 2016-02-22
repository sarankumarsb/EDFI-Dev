// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;
using System.Linq;
using EdFi.Dashboards.Common;
using EdFi.Dashboards.Metric.Resources.Factories;
using EdFi.Dashboards.Metric.Resources.Models;
using EdFi.Dashboards.Metric.Resources.Providers;
using EdFi.Dashboards.Resources;
using EdFi.Dashboards.Resources.Security.Common;

namespace EdFi.Dashboards.Metric.Resources.Services
{
    public interface IMetricService<TRequest> : IService<TRequest, MetricBase>
        where TRequest : MetricInstanceSetRequestBase { }

    public class MetricService<TRequest> : IMetricService<TRequest> 
        where TRequest : MetricInstanceSetRequestBase
    {
        private readonly IMetricMetadataTreeService metricMetadataTreeService;
        private readonly IMetricDataService<TRequest> metricDataService;
        private readonly IMetricInstanceTreeFactory metricInstanceTreeFactory;
        private readonly IMetricNodeResolver metricNodeResolver;

        public MetricService(IMetricMetadataTreeService metricMetadataTreeService,
                                IMetricDataService<TRequest> metricDataService, 
                                IMetricInstanceTreeFactory metricInstanceTreeFactory,
                                IMetricNodeResolver metricNodeResolver)
        {
            this.metricInstanceTreeFactory = metricInstanceTreeFactory;
            this.metricNodeResolver = metricNodeResolver;
            this.metricDataService = metricDataService;
            this.metricMetadataTreeService = metricMetadataTreeService;
        }

        public MetricBase Get(TRequest request)
        {
            //Resolve the MetricNodeId.
            var node = metricNodeResolver.ResolveFromMetricVariantId(request.MetricVariantId);
            
            if (node == null)
                throw new MetricNodeNotFoundException(request.MetricVariantId, string.Format("Unable to resolve the metric node from metric variant id '{0}'.", request.MetricVariantId));

            int metricNodeId = node.MetricNodeId;

            var metricMetadataTree = metricMetadataTreeService.Get(MetricMetadataTreeRequest.Create());
            var metricMetadataNode = metricMetadataTree.AllNodesByMetricNodeId.GetValueOrDefault(metricNodeId);

            var metricDataCollection = metricDataService.Get(request);

            return metricInstanceTreeFactory.CreateTree(request, metricMetadataNode, metricDataCollection);
        }
    }
}
