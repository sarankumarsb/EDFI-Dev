// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using EdFi.Dashboards.Core;
using EdFi.Dashboards.Core.Providers.Context;
using EdFi.Dashboards.Metric.Resources.Models;
using EdFi.Dashboards.Metric.Resources.Providers;
using EdFi.Dashboards.Metric.Resources.Services;
using EdFi.Dashboards.Resources.Metric;

namespace EdFi.Dashboards.Presentation.Core.Providers.Metric
{
    public class MetricNodeResolver : IMetricNodeResolver
    {
        private readonly IEdFiDashboardContextProvider dashboardContextProvider;
        private readonly IMetricMetadataTreeService metricMetadataTreeService;
        private readonly IRootMetricNodeResolver rootMetricNodeResolver;

        public MetricNodeResolver(IEdFiDashboardContextProvider dashboardContextProvider, 
            IMetricMetadataTreeService metricMetadataTreeService, 
            IRootMetricNodeResolver rootMetricNodeResolver)
        {
            this.dashboardContextProvider = dashboardContextProvider;
            this.metricMetadataTreeService = metricMetadataTreeService;
            this.rootMetricNodeResolver = rootMetricNodeResolver;
        }

        public int ResolveMetricId(int metricVariantId)
        {
            return GetDefaultMetricMetadataNode(metricVariantId).MetricId;
        }

        public MetricVariantType ResolveMetricVariantType(int metricVariantId)
        {
            return GetDefaultMetricMetadataNode(metricVariantId).MetricVariantType;
        }

        private MetricMetadataNode GetDefaultMetricMetadataNode(int metricVariantId)
        {
            var metaDataTree = metricMetadataTreeService.Get(MetricMetadataTreeRequest.Create());
            var metricVariants = metaDataTree.AllNodesByMetricVariantId.ValuesByKey(metricVariantId);
            if (!metricVariants.Any())
                throw new NullReferenceException(String.Format("The metric variant id '{0}' is unknown.", metricVariantId));

            return metricVariants.First();
        }

        public MetricMetadataNode ResolveFromMetricVariantId(int metricVariantId)
        {
            var context = dashboardContextProvider.GetEdFiDashboardContext();

            //Based on DomainEntity context lets grab the appropriate root.
            switch (context.MetricInstanceSetType)
            {
                case MetricInstanceSetType.StudentSchool:
                    return GetMetricNodeForStudentFromMetricVariantId(context.SchoolId.Value, metricVariantId);

                case MetricInstanceSetType.Staff:
                case MetricInstanceSetType.School:
                    return GetMetricNodeForSchoolFromMetricVariantId(context.SchoolId.Value, metricVariantId);

                case MetricInstanceSetType.LocalEducationAgency:
                    return GetMetricNodeForLocalEducationAgencyMetricVariantId(metricVariantId);

                default:
                    throw new NotSupportedException("Can't resolve Metric Metadata Node for Domain Entity Type '" + context.MetricInstanceSetType + "'.");
            }
        }

        public MetricMetadataNode GetMetricNodeForStudentFromMetricVariantId(int schoolId, int metricVariantId)
        {
            var metaDataTree = metricMetadataTreeService.Get(MetricMetadataTreeRequest.Create());

            var rootNodeIdForStudent = (int)rootMetricNodeResolver.GetMetricHierarchyRoot(schoolId);
            var nodeForStudentOverview = metaDataTree.AllNodesByMetricVariantId.ValuesByKey(metricVariantId).SingleOrDefault(x => x.RootNodeId == rootNodeIdForStudent);

            return nodeForStudentOverview;
        }

        public MetricMetadataNode GetMetricNodeForSchoolFromMetricVariantId(int schoolId, int metricVariantId)
        {
            var metaDataTree = metricMetadataTreeService.Get(MetricMetadataTreeRequest.Create());

            var rootNodeIdForSchool = (int)rootMetricNodeResolver.GetMetricHierarchyRoot(schoolId);
            var nodeForSchoolOverview = metaDataTree.AllNodesByMetricVariantId.ValuesByKey(metricVariantId).SingleOrDefault(x => x.RootNodeId == rootNodeIdForSchool);
            return nodeForSchoolOverview;
        }

        public MetricMetadataNode GetMetricNodeForLocalEducationAgencyMetricVariantId(int metricVariantId)
        {
            var metaDataTree = metricMetadataTreeService.Get(MetricMetadataTreeRequest.Create());

            const int rootNodeIdForLocalEducationAgency = (int)MetricHierarchyRoot.LocalEducationAgency;

            var nodeForLocalEducationAgencyOverview = metaDataTree.AllNodesByMetricVariantId.ValuesByKey(metricVariantId).SingleOrDefault(x => x.RootNodeId == rootNodeIdForLocalEducationAgency);

            return nodeForLocalEducationAgencyOverview;
        }

        public IEnumerable<MetricMetadataNode> ResolveFromMetricId(int metricId)
        {
            var context = dashboardContextProvider.GetEdFiDashboardContext();

            //Based on DomainEntity context lets grab the appropriate root.
            switch (context.MetricInstanceSetType)
            {
                case MetricInstanceSetType.StudentSchool:
                    return GetMetricNodesForStudentFromMetricId(context.SchoolId.Value, metricId);

                case MetricInstanceSetType.Staff:
                case MetricInstanceSetType.School:
                    return GetMetricNodesForSchoolFromMetricId(context.SchoolId.Value, metricId);
                
                case MetricInstanceSetType.LocalEducationAgency:
                    return GetMetricNodesForLocalEducationAgencyFromMetricId(metricId);
                
                default:
                    throw new NotSupportedException("Can't resolve Metric Metadata Node for Domain Entity Type '" + context.MetricInstanceSetType + "'.");
            }
        }

        public IEnumerable<MetricMetadataNode> GetMetricNodesForStudentFromMetricId(int schoolId, int metricId)
        {
            var metaDataTree = metricMetadataTreeService.Get(MetricMetadataTreeRequest.Create());

            var rootNodeIdForStudent = (int)rootMetricNodeResolver.GetMetricHierarchyRoot(schoolId);
            var nodeForStudentOverview = metaDataTree.AllNodesByMetricId.ValuesByKey(metricId).Where(x => x.RootNodeId == rootNodeIdForStudent);

            return nodeForStudentOverview;
        }

        public IEnumerable<MetricMetadataNode> GetMetricNodesForSchoolFromMetricId(int schoolId, int metricId)
        {
            var metaDataTree = metricMetadataTreeService.Get(MetricMetadataTreeRequest.Create());

            var rootNodeIdForSchool = (int)rootMetricNodeResolver.GetMetricHierarchyRoot(schoolId);
            var nodeForSchoolOverview = metaDataTree.AllNodesByMetricId.ValuesByKey(metricId).Where(x => x.RootNodeId == rootNodeIdForSchool);
            return nodeForSchoolOverview;
        }

        public IEnumerable<MetricMetadataNode> GetMetricNodesForLocalEducationAgencyFromMetricId(int metricId)
        {
            var metaDataTree = metricMetadataTreeService.Get(MetricMetadataTreeRequest.Create());

            const int rootNodeIdForLocalEducationAgency = (int) MetricHierarchyRoot.LocalEducationAgency;

            var nodeForLocalEducationAgencyOverview = metaDataTree.AllNodesByMetricId.ValuesByKey(metricId).Where(x => x.RootNodeId == rootNodeIdForLocalEducationAgency);

            return nodeForLocalEducationAgencyOverview;
        }
    }
}