// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System.Collections.Generic;
using System.Linq;
using EdFi.Dashboards.Common;
using EdFi.Dashboards.Core;
using EdFi.Dashboards.Infrastructure;
using EdFi.Dashboards.Metric.Resources.Models;
using EdFi.Dashboards.Metric.Resources.Providers;
using EdFi.Dashboards.Metric.Resources.Services;
using EdFi.Dashboards.Resource.Models.Common;
using EdFi.Dashboards.Resources.Metric;
using EdFi.Dashboards.Resources.Metric.Requests;
using EdFi.Dashboards.Resources.Navigation.Support;

namespace EdFi.Dashboards.Resources.Navigation.MetricRouteProviders
{
    public class LocalEducationAgencyMetricRouteProvider : ChainOfResponsibilityBase<IMetricRouteProvider, MetricRouteProviderChainRequest, IEnumerable<Link>>, IMetricRouteProvider
    {
        private readonly IRootMetricNodeResolver rootMetricNodeResolver;
        private readonly ILocalEducationAgencyAreaLinks localEducationAgencyLinks;
        private readonly IDomainSpecificMetricNodeResolver domainSpecificMetricNodeResolver;

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalEducationAgencyMetricRouteProvider"/> class with the next <see cref="IMetricRouteProvider"/> in the chain, and the <see cref="IRootMetricNodeResolver"/> for locating the root metric node.
        /// </summary>
        /// <param name="next">The next IMetricRouteProvider in the chain.</param>
        /// <param name="rootMetricNodeResolver">The service to use to locate the root metric node for rendering purposes.</param>
        /// <param name="localEducationAgencyLinks">Provides strongly-typed URL generation for the LocalEducationAgency area.</param>
        /// <param name="domainSpecificMetricNodeResolver">The service used to locate the Operational Dashboard metric node.</param>
        public LocalEducationAgencyMetricRouteProvider(IMetricRouteProvider next, IRootMetricNodeResolver rootMetricNodeResolver,
            ILocalEducationAgencyAreaLinks localEducationAgencyLinks, 
            IDomainSpecificMetricNodeResolver domainSpecificMetricNodeResolver)
            : base(next)
        {
            this.rootMetricNodeResolver = rootMetricNodeResolver;
            this.localEducationAgencyLinks = localEducationAgencyLinks;
            this.domainSpecificMetricNodeResolver = domainSpecificMetricNodeResolver;
        }

        public IEnumerable<Link> GetRoute(MetricInstanceSetRequestBase metricInstanceSetRequest, MetricMetadataNode metadataNode)
        {
            return base.ProcessRequest(MetricRouteProviderChainRequest.Create(metricInstanceSetRequest, metadataNode));
        }

        protected override bool CanHandleRequest(MetricRouteProviderChainRequest request)
        {
            var metricInstanceSetRequest = request.MetricInstanceSetRequest as LocalEducationAgencyMetricInstanceSetRequest;

            if (metricInstanceSetRequest != null)
                return true;

            return false;
        }


        private MetricMetadataNode _rootMetricNode;

        private MetricMetadataNode RootMetricNode
        {
            get
            {
                if (_rootMetricNode == null)
                    _rootMetricNode = rootMetricNodeResolver.GetRootMetricNodeForLocalEducationAgency();

                return _rootMetricNode;
            }
        }

        private MetricMetadataNode _operationalDashboardMetricNode;

        private MetricMetadataNode OperationalDashboardMetricNode
        {
            get
            {
                if (_operationalDashboardMetricNode == null)
                    _operationalDashboardMetricNode = domainSpecificMetricNodeResolver.GetOperationalDashboardMetricNode(MetricInstanceSetType.LocalEducationAgency);

                return _operationalDashboardMetricNode;
            }
        }

        protected override IEnumerable<Link> HandleRequest(MetricRouteProviderChainRequest request)
        {
            var metricInstanceSetRequest = request.MetricInstanceSetRequest as LocalEducationAgencyMetricInstanceSetRequest;

            var rootNodeIds = new List<int> { RootMetricNode.MetricNodeId, OperationalDashboardMetricNode.MetricNodeId };

            // Is this the Overview metric? (Redirect to the Overview resource)
            if (request.MetadataNode.MetricNodeId == RootMetricNode.MetricNodeId)
            {
                string href = localEducationAgencyLinks.Overview(metricInstanceSetRequest.LocalEducationAgencyId);
                yield return new Link { Rel = LinkRel.AsResource, Href = href };
                yield return new Link { Rel = LinkRel.Web, Href = href };
            }
            else if (request.MetadataNode.MetricNodeId == OperationalDashboardMetricNode.MetricNodeId)
            {
                string href = localEducationAgencyLinks.OperationalDashboard(metricInstanceSetRequest.LocalEducationAgencyId);
                yield return new Link { Rel = LinkRel.AsResource, Href = href };
                yield return new Link { Rel = LinkRel.Web, Href = href };
            }
            // Is this an immediate child of the Overview or Operational Dashboard metric? (Appropriate for rendering directly)
            else if (rootNodeIds.Contains(request.MetadataNode.Parent.MetricNodeId))
            {
                string href = localEducationAgencyLinks.Metrics(metricInstanceSetRequest.LocalEducationAgencyId, request.MetadataNode.MetricVariantId);
                yield return new Link { Rel = LinkRel.AsResource, Href = href };
                yield return new Link { Rel = LinkRel.Web, Href = href };
            }
            else
            {
                // Look up the tree and find the first child of the overview/operational dashboard metric
                var topLevelNode = request.MetadataNode.Ancestors.First(n => rootNodeIds.Contains(n.Parent.MetricNodeId));

                yield return new Link { Rel = LinkRel.AsResource, Href = localEducationAgencyLinks.Metrics(metricInstanceSetRequest.LocalEducationAgencyId, request.MetadataNode.MetricVariantId) };
                yield return new Link { Rel = LinkRel.Web, Href = localEducationAgencyLinks.Metrics(metricInstanceSetRequest.LocalEducationAgencyId, topLevelNode.MetricVariantId).MetricAnchor(request.MetadataNode.MetricVariantId) };
            }
        }
    }
}
