// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;
using System.Linq;
using EdFi.Dashboards.Metric.Resources.Models;
using EdFi.Dashboards.Metric.Resources.Providers;
using EdFi.Dashboards.Resources.Security.Common;
using EdFi.Dashboards.SecurityTokenService.Authentication;
using log4net;

namespace EdFi.Dashboards.Resources.Metric
{
    public interface IMetricCorrelationProvider
    {
        MetricCorrelationProvider.MetricRenderingContext GetRenderingParentMetricVariantIdForSchool(int localEducationAgencyMetricVariantId, int schoolId);
        MetricCorrelationProvider.MetricRenderingContext GetRenderingParentMetricVariantIdForStudent(int schoolMetricVariantId, int schoolId);
    }

    public class MetricCorrelationProvider : IMetricCorrelationProvider
    {
        private static ILog logger = LogManager.GetLogger(typeof (MetricCorrelationProvider));
        private readonly IRootMetricNodeResolver rootMetricNodeResolver;

        public MetricCorrelationProvider(IRootMetricNodeResolver rootMetricNodeResolver)
        {
            this.rootMetricNodeResolver = rootMetricNodeResolver;
        }

        [CanBeAuthorizedBy(EdFiClaimTypes.ViewAllMetrics, EdFiClaimTypes.ViewMyMetrics)]
        public MetricRenderingContext GetRenderingParentMetricVariantIdForSchool(int localEducationAgencyMetricVariantId, int schoolId)
        {
            //Get the root Overview Metric from the current LEA. For The LEA we need to get the parent node of the overview because we can sometimes need the Academic Dashboard or the Operational Dashboard.
            var rootNodeForLEA = rootMetricNodeResolver.GetRootMetricNodeForLocalEducationAgency().Parent;

            //var currentNodeForLEAMetric = rootNodeForLEA.DescendantsOrSelf.SingleOrDefault(x => x.MetricId == localEducationAgencyMetricId);
            var currentNodeForLEAMetric = rootNodeForLEA.FindInDescendantsOrSelfByMetricVariantId(localEducationAgencyMetricVariantId).SingleOrDefault();

            if (currentNodeForLEAMetric == null)
				throw new InvalidOperationException(string.Format("Could not find metric variant Id:({0}) in the Local Education Agency Overview tree.", localEducationAgencyMetricVariantId));

            //Same thing we need the parent because we might be looking for a Academic Dashboard or an Operational one.
            var rootNodeForSchool = rootMetricNodeResolver.GetRootMetricNodeForSchool(schoolId).Parent;

            if (currentNodeForLEAMetric.ChildDomainEntityMetricId.HasValue)
            {
                //Look for the child/overview node that contains the metric we are looking for.
                //This is needed so we can calculate the main context page where the metric lives.
                var overviewSchoolNode = FindOverviewMetricMetadataNodeInRoot(rootNodeForSchool, currentNodeForLEAMetric.ChildDomainEntityMetricId, currentNodeForLEAMetric.MetricVariantType);

                var result = FindRenderingParent(overviewSchoolNode, currentNodeForLEAMetric.ChildDomainEntityMetricId.Value, currentNodeForLEAMetric.MetricVariantType);
                if (result != null)
                    return result;
            }


            //If we are still here we don't have a correlated metric id so lets get the overview metric to display in context.
            var overviewLEANode = FindOverviewMetricMetadataNodeInRoot(rootNodeForLEA, currentNodeForLEAMetric.MetricId, currentNodeForLEAMetric.MetricVariantType);
            foreach (var overviewNode in overviewLEANode.Descendants)
            {
                var parentWithLEAMetric = overviewNode.FindInDescendantsByMetricVariantId(localEducationAgencyMetricVariantId).SingleOrDefault();

                if (parentWithLEAMetric != null) //We found the parent who has the metric.
                {
                    if (overviewNode.ChildDomainEntityMetricId.HasValue)
                    {
                        var correlatedParentMetric = rootNodeForSchool.FindInDescendantsOrSelfByMetricId(overviewNode.ChildDomainEntityMetricId.Value).SingleOrDefault(x => x.MetricVariantType == currentNodeForLEAMetric.MetricVariantType);
                        if (correlatedParentMetric == null)
                            correlatedParentMetric = rootNodeForSchool.FindInDescendantsOrSelfByMetricId(overviewNode.ChildDomainEntityMetricId.Value).SingleOrDefault();
                        
                        if (correlatedParentMetric != null)
                            return new MetricRenderingContext
                                       {
                                           ContextMetricVariantId = correlatedParentMetric.MetricVariantId,
                                       };
                    }
                }
            }

            //If we are still here no correlated node or parent so return empty one.
            logger.DebugFormat("GetRenderingParentMetricIdForSchool: Could not find correlation metric for metric variant Id:{0} and school Id: {1}", localEducationAgencyMetricVariantId, schoolId);
            return new MetricRenderingContext();
        }

        [CanBeAuthorizedBy(EdFiClaimTypes.ViewAllStudents, EdFiClaimTypes.ViewMyStudents)]
        public MetricRenderingContext GetRenderingParentMetricVariantIdForStudent(int schoolMetricVariantId, int schoolId)
        {
            var rootOverviewSchoolMetric = rootMetricNodeResolver.GetRootMetricNodeForSchool(schoolId);
            var currentNodeForSchoolMetric = rootOverviewSchoolMetric.FindInDescendantsOrSelfByMetricVariantId(schoolMetricVariantId).SingleOrDefault();

            if (currentNodeForSchoolMetric==null)
				throw new InvalidOperationException(string.Format("Could not find metric variant Id:({0}) in the School Overview tree.", schoolMetricVariantId));

            var rootOverviewNodeForStudent = rootMetricNodeResolver.GetRootMetricNodeForStudent(schoolId);

            if (currentNodeForSchoolMetric.ChildDomainEntityMetricId.HasValue)
            {
                var result = FindRenderingParent(rootOverviewNodeForStudent, currentNodeForSchoolMetric.ChildDomainEntityMetricId.Value, currentNodeForSchoolMetric.MetricVariantType);
                if (result != null)
                    return result;
            }


            foreach (var parentNode in rootOverviewSchoolMetric.Descendants)
            {
                var parentWithSchoolMetric = parentNode.FindInDescendantsByMetricVariantId(schoolMetricVariantId).SingleOrDefault();
                if (parentWithSchoolMetric != null) //We found the parent who has the metric.
                {
                    if (parentNode.ChildDomainEntityMetricId.HasValue)
                    {
                        var correlatedParentMetric = rootOverviewNodeForStudent.FindInDescendantsOrSelfByMetricId(parentNode.ChildDomainEntityMetricId.Value).SingleOrDefault(x => x.MetricVariantType == parentNode.MetricVariantType);
                        if (correlatedParentMetric == null)
                            correlatedParentMetric = rootOverviewNodeForStudent.FindInDescendantsOrSelfByMetricId(parentNode.ChildDomainEntityMetricId.Value).SingleOrDefault();

                        if (correlatedParentMetric != null)
                            return new MetricRenderingContext
                                       {
                                           ContextMetricVariantId = correlatedParentMetric.MetricVariantId,
                                       };
                    }
                }
            }

            //If we are still here no correlated node or parent so return empty one.
            logger.DebugFormat("GetRenderingParentMetricIdForStudent: Could not find correlation metric for metric variant Id:{0} and school Id: {1}", schoolMetricVariantId, schoolId);
            return new MetricRenderingContext();
        }

		protected virtual MetricMetadataNode FindOverviewMetricMetadataNodeInRoot(MetricMetadataNode root, int? metricIdToFind, MetricVariantType metricVariantType)
        {
            if (metricIdToFind == null)
                return null;


            return root.Children.FirstOrDefault(overview => overview.FindInDescendantsOrSelfByMetricId(metricIdToFind.Value).SingleOrDefault(x => x.MetricVariantType == metricVariantType) != null);
        }

		protected virtual MetricRenderingContext FindRenderingParent(MetricMetadataNode root, int? metricIdToFind, MetricVariantType metricVariantType)
        {
            if (root == null || !metricIdToFind.HasValue)
                return new MetricRenderingContext();

            //We are at the overview collection level.
            foreach (var renderingParent in root.Children)
            {
                var tempMetric = renderingParent.FindInDescendantsOrSelfByMetricId(metricIdToFind.Value).SingleOrDefault(x => x.MetricVariantType == metricVariantType);
                if (tempMetric == null)
                    tempMetric = renderingParent.FindInDescendantsOrSelfByMetricId(metricIdToFind.Value).SingleOrDefault();
                if (tempMetric != null)//We found it
                    return new MetricRenderingContext
                                {
                                    ContextMetricVariantId = renderingParent.MetricVariantId,
                                    MetricVariantId = tempMetric.MetricVariantId
                                };
            }

            return null;
        }

        [Serializable]
        public class MetricRenderingContext
        {
            /// <summary>
            /// Gets sets the correlated metricVariantId that represents the top level page context.
            /// </summary>
            public int? ContextMetricVariantId { get; set; }

            /// <summary>
            /// Gets sets the correlated metricVariantId for the current metric.
            /// </summary>
            public int? MetricVariantId { get; set; }
        }
    }
}
