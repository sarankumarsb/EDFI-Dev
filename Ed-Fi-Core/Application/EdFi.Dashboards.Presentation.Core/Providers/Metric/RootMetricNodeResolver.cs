// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using EdFi.Dashboards.Core;
using EdFi.Dashboards.Core.Providers.Context;
using EdFi.Dashboards.Metric.Resources.Models;
using EdFi.Dashboards.Metric.Resources.Services;
using EdFi.Dashboards.Resources.Metric;
using EdFi.Dashboards.Resources.School;

namespace EdFi.Dashboards.Presentation.Core.Providers.Metric
{
    public class RootMetricNodeResolver : IRootMetricNodeResolver
    {
        private readonly IEdFiDashboardContextProvider dashboardContextProvider;
        private readonly IMetricMetadataTreeService metricMetadataTreeService;
        private readonly ISchoolCategoryProvider schoolCategoryProvider;

        public RootMetricNodeResolver(IEdFiDashboardContextProvider dashboardContextProvider, 
            IMetricMetadataTreeService metricMetadataTreeService, 
            ISchoolCategoryProvider schoolCategoryProvider)
        {
            this.schoolCategoryProvider = schoolCategoryProvider;
            this.metricMetadataTreeService = metricMetadataTreeService;
            this.dashboardContextProvider = dashboardContextProvider;
        }

        public virtual MetricMetadataNode GetRootMetricNode()  // TODO: GKM - Review this for possible use of a base request type (similar to the metric instance set request), and elimination of the dashboard context injection.
        {
            //Based on DomainEntity context lets grab the appropriate root.
            var context = dashboardContextProvider.GetEdFiDashboardContext();

            switch (context.MetricInstanceSetType)
            {
                case MetricInstanceSetType.Staff:
                case MetricInstanceSetType.StudentSchool:
                    return GetRootMetricNodeForStudent(context.SchoolId.Value);
                case MetricInstanceSetType.School:
                    return GetRootMetricNodeForSchool(context.SchoolId.Value);
                case MetricInstanceSetType.LocalEducationAgency:
                    return GetRootMetricNodeForLocalEducationAgency();
                default:
                    throw new NotSupportedException("Metric instance set type '" + context.MetricInstanceSetType + "' is not supported.");
            }
        }

        public virtual MetricMetadataNode GetRootMetricNodeForStudent(int schoolId)
        {
            var rootNodeIdForStudent = (int)GetMetricHierarchyRoot(schoolId);
            const int metricVariantIdForStudentOverview = (int)StudentMetricEnum.Overview;

            var nodeForStudentOverview = MetricMetadata.AllNodesByMetricVariantId.ValuesByKey(metricVariantIdForStudentOverview).SingleOrDefault(x => x.RootNodeId == rootNodeIdForStudent);

            return nodeForStudentOverview;
        }

        private Dictionary<int, MetricMetadataNode> metricMetadataNodesBySchoolId = new Dictionary<int, MetricMetadataNode>();

        public virtual MetricMetadataNode GetRootMetricNodeForSchool(int schoolId)
        {
            MetricMetadataNode value;

            if (metricMetadataNodesBySchoolId.TryGetValue(schoolId, out value))
                return value;

            var rootNodeIdForSchool = (int)GetMetricHierarchyRoot(schoolId);
            const int metricVariantIdForSchoolOverview = (int)SchoolMetricEnum.Overview;

            var nodeForSchoolOverview = MetricMetadata.AllNodesByMetricVariantId.ValuesByKey(metricVariantIdForSchoolOverview).SingleOrDefault(x => x.RootNodeId == rootNodeIdForSchool);

            metricMetadataNodesBySchoolId[schoolId] = nodeForSchoolOverview;
            return nodeForSchoolOverview;
        }

        private MetricMetadataTree _metricMetadata;

        public virtual MetricMetadataTree MetricMetadata
        {
            get
            {
                if (_metricMetadata == null)
                    _metricMetadata = metricMetadataTreeService.Get(MetricMetadataTreeRequest.Create());

                return _metricMetadata;
            }
        }

        private MetricMetadataNode _rootMetricNodeForLocalEducationAgency;

        public virtual MetricMetadataNode GetRootMetricNodeForLocalEducationAgency()
        {
            if (_rootMetricNodeForLocalEducationAgency != null)
                return _rootMetricNodeForLocalEducationAgency;

            const int rootNodeIdForLocalEducationAgency = (int)MetricHierarchyRoot.LocalEducationAgency;
            const int metricVariantIdForLocalEducationAgencyOverview = (int)LocalEducationAgencyMetricEnum.Overview;

            var nodeForLocalEducationAgencyOverview = MetricMetadata.AllNodesByMetricVariantId.ValuesByKey(metricVariantIdForLocalEducationAgencyOverview).SingleOrDefault(x => x.RootNodeId == rootNodeIdForLocalEducationAgency);

            _rootMetricNodeForLocalEducationAgency = nodeForLocalEducationAgencyOverview;
            
            return _rootMetricNodeForLocalEducationAgency;
        }

        private Dictionary<int, MetricHierarchyRoot> metricHierarchyRootsBySchoolId = new Dictionary<int, MetricHierarchyRoot>();

        public virtual MetricHierarchyRoot GetMetricHierarchyRoot(int schoolId)
        {
            MetricHierarchyRoot value;

            if (metricHierarchyRootsBySchoolId.TryGetValue(schoolId, out value))
                return value;

            var category = schoolCategoryProvider.GetSchoolCategoryType(schoolId);

            switch (category)
            {
                case SchoolCategory.Elementary:
                    value = MetricHierarchyRoot.Elementary;
                    break;
                case SchoolCategory.MiddleSchool:
                    value = MetricHierarchyRoot.MiddleSchool;
                    break;
                case SchoolCategory.HighSchool:
                    value = MetricHierarchyRoot.HighSchool;
                    break;
                default:
                    value = MetricHierarchyRoot.HighSchool;
                    break;
            }

            metricHierarchyRootsBySchoolId[schoolId] = value;
            return value;
        }
    }
}