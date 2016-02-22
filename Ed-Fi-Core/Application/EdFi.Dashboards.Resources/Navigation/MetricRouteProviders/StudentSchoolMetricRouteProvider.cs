// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System.Collections.Generic;
using System.Linq;
using EdFi.Dashboards.Common;
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
    public class StudentSchoolMetricRouteProvider : ChainOfResponsibilityBase<IMetricRouteProvider, MetricRouteProviderChainRequest, IEnumerable<Link>>, IMetricRouteProvider
    {
        private readonly IRootMetricNodeResolver rootMetricNodeResolver;
        private readonly IStudentSchoolAreaLinks studentSchoolLinks;

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalEducationAgencyMetricRouteProvider"/> class with the next <see cref="IMetricRouteProvider"/> in the chain, and the <see cref="IRootMetricNodeResolver"/> for locating the root metric node.
        /// </summary>
        /// <param name="next">The next IMetricRouteProvider in the chain.</param>
        /// <param name="rootMetricNodeResolver">The service to use to locate the root metric node for rendering purposes.</param>
        /// <param name="studentSchoolLinks">Provides strongly-typed URL generation for the StudentSchool area.</param>
        public StudentSchoolMetricRouteProvider(IMetricRouteProvider next, IRootMetricNodeResolver rootMetricNodeResolver, IStudentSchoolAreaLinks studentSchoolLinks) 
            : base(next)
        {
            this.rootMetricNodeResolver = rootMetricNodeResolver;
            this.studentSchoolLinks = studentSchoolLinks;
        }

        public IEnumerable<Link> GetRoute(MetricInstanceSetRequestBase metricInstanceSetRequest, MetricMetadataNode metadataNode)
        {
            return base.ProcessRequest(MetricRouteProviderChainRequest.Create(metricInstanceSetRequest, metadataNode));
        }

        protected override bool CanHandleRequest(MetricRouteProviderChainRequest request)
        {
            var metricInstanceSetRequest = request.MetricInstanceSetRequest as StudentSchoolMetricInstanceSetRequest;

            if (metricInstanceSetRequest != null)
                return true;

            return false;
        }

        // TODO: Review performance of this method
        protected override IEnumerable<Link> HandleRequest(MetricRouteProviderChainRequest request)
        {
            return GetLinks(request);
        }

        private List<Link> GetLinks(MetricRouteProviderChainRequest request)
        {
            List<Link> links = new List<Link>();

            var metricInstanceSetRequest = request.MetricInstanceSetRequest as StudentSchoolMetricInstanceSetRequest;

            var rootMetricNode = rootMetricNodeResolver.GetRootMetricNodeForStudent(metricInstanceSetRequest.SchoolId);

            // Is this the Overview metric? (Redirect to the Overview resource)
            if (request.MetadataNode.MetricNodeId == rootMetricNode.MetricNodeId)
            {
                string href = studentSchoolLinks.Overview(metricInstanceSetRequest.SchoolId, metricInstanceSetRequest.StudentUSI);
                links.Add( new Link { Rel = LinkRel.AsResource, Href = href });
                links.Add( new Link { Rel = LinkRel.Web, Href = href });
            }
            // Is this an immediate child of the Overview metric? (Appropriate for rendering directly)
            else if (request.MetadataNode.Parent.MetricNodeId == rootMetricNode.MetricNodeId)
            {
                string href = studentSchoolLinks.Metrics(metricInstanceSetRequest.SchoolId, metricInstanceSetRequest.StudentUSI, request.MetadataNode.MetricVariantId);
                links.Add(new Link { Rel = LinkRel.AsResource, Href = href });
                links.Add(new Link { Rel = LinkRel.Web, Href = href });
            }
            else
            {
                // Look up the tree and find the first child of the overview metric
                var topLevelNode = request.MetadataNode.Ancestors.First(n => n.Parent.MetricNodeId == rootMetricNode.MetricNodeId);

                links.Add(new Link { Rel = LinkRel.AsResource, Href = studentSchoolLinks.Metrics(metricInstanceSetRequest.SchoolId, metricInstanceSetRequest.StudentUSI, request.MetadataNode.MetricVariantId) });
                links.Add(new Link { Rel = LinkRel.Web, Href = studentSchoolLinks.Metrics(metricInstanceSetRequest.SchoolId, metricInstanceSetRequest.StudentUSI, topLevelNode.MetricVariantId).MetricAnchor(request.MetadataNode.MetricVariantId) });
            }
            return links;
        }
    }
}
