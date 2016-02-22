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
    public class SchoolMetricRouteProvider : ChainOfResponsibilityBase<IMetricRouteProvider, MetricRouteProviderChainRequest, IEnumerable<Link>>, IMetricRouteProvider
    {
        private readonly IRootMetricNodeResolver rootMetricNodeResolver;
        private readonly ISchoolAreaLinks schoolLinks;
        private readonly IDomainSpecificMetricNodeResolver domainSpecificMetricNodeResolver;

        /// <summary>
        /// Initializes a new instance of the <see cref="SchoolMetricRouteProvider"/> class with the next <see cref="IMetricRouteProvider"/> in the chain, and the <see cref="IRootMetricNodeResolver"/> for locating the root metric node.
        /// </summary>
        /// <param name="next">The next IMetricRouteProvider in the chain.</param>
        /// <param name="rootMetricNodeResolver">The service to use to locate the root metric node for rendering purposes.</param>
        /// <param name="schoolLinks">Provides strongly-typed URL generation for the School area.</param>
        /// <param name="domainSpecificMetricNodeResolver">The service used to locate the Operational Dashboard metric node.</param>
        public SchoolMetricRouteProvider(IMetricRouteProvider next, IRootMetricNodeResolver rootMetricNodeResolver, ISchoolAreaLinks schoolLinks,
            IDomainSpecificMetricNodeResolver domainSpecificMetricNodeResolver) 
            : base(next)
        {
            this.rootMetricNodeResolver = rootMetricNodeResolver;
            this.schoolLinks = schoolLinks;
            this.domainSpecificMetricNodeResolver = domainSpecificMetricNodeResolver;
        }

        public IEnumerable<Link> GetRoute(MetricInstanceSetRequestBase metricInstanceSetRequest, MetricMetadataNode metadataNode)
        {
            return base.ProcessRequest(MetricRouteProviderChainRequest.Create(metricInstanceSetRequest, metadataNode));
        }

        protected override bool CanHandleRequest(MetricRouteProviderChainRequest request)
        {
            var metricInstanceSetRequest = request.MetricInstanceSetRequest as SchoolMetricInstanceSetRequest;

            if (metricInstanceSetRequest != null)
                return true;

            return false;
        }

        private MetricMetadataNode GetRootMetricNode(int schoolId)
        {
            return rootMetricNodeResolver.GetRootMetricNodeForSchool(schoolId);
        }

        private MetricMetadataNode GetOperationalDashboardMetricNode(int schoolId)
        {
            return domainSpecificMetricNodeResolver.GetOperationalDashboardMetricNode(MetricInstanceSetType.School, schoolId);
        }

        protected override IEnumerable<Link> HandleRequest(MetricRouteProviderChainRequest request)
        {
            var metricInstanceSetRequest = request.MetricInstanceSetRequest as SchoolMetricInstanceSetRequest;

            int operationalDashboardMetricNodeId = GetOperationalDashboardMetricNode(metricInstanceSetRequest.SchoolId).MetricNodeId;
            int rootMetricNodeId = GetRootMetricNode(metricInstanceSetRequest.SchoolId).MetricNodeId;

            var rootNodeIds = new List<int> { rootMetricNodeId, operationalDashboardMetricNodeId };

            // Is this the Overview metric? (Redirect to the Overview resource)
            if (request.MetadataNode.MetricNodeId == rootMetricNodeId)
            {
                string link = schoolLinks.Overview(metricInstanceSetRequest.SchoolId);
                yield return new Link { Rel = LinkRel.AsResource, Href = link };
                yield return new Link { Rel = LinkRel.Web, Href = link };
            }
            else if (request.MetadataNode.MetricNodeId == operationalDashboardMetricNodeId)
            {
                string link = schoolLinks.OperationalDashboard(metricInstanceSetRequest.SchoolId);
                yield return new Link { Rel = LinkRel.AsResource, Href = link };
                yield return new Link { Rel = LinkRel.Web, Href = link };
            }
            // Is this an immediate child of the Overview metric? (Appropriate for rendering directly)
            else if (rootNodeIds.Contains(request.MetadataNode.Parent.MetricNodeId))
            {
                string link = schoolLinks.Metrics(metricInstanceSetRequest.SchoolId, request.MetadataNode.MetricVariantId);
                yield return new Link { Rel = LinkRel.AsResource, Href = link };
                yield return new Link { Rel = LinkRel.Web, Href = link };
            }
            else
            {
                // Look up the tree and find the first child of the overview metric
                var topLevelNode = request.MetadataNode.Ancestors.First(n => rootNodeIds.Contains(n.Parent.MetricNodeId));

                yield return new Link { Rel = LinkRel.AsResource, Href = schoolLinks.Metrics(metricInstanceSetRequest.SchoolId, request.MetadataNode.MetricVariantId) };
                yield return new Link { Rel = LinkRel.Web, Href = schoolLinks.Metrics(metricInstanceSetRequest.SchoolId, topLevelNode.MetricVariantId).MetricAnchor(request.MetadataNode.MetricVariantId) };
            }
        }
    }
}
