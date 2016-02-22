// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Castle.DynamicProxy;
using EdFi.Dashboards.Core;
using EdFi.Dashboards.Infrastructure;
using EdFi.Dashboards.Metric.Resources.Models;
using EdFi.Dashboards.Metric.Resources.Providers;
using EdFi.Dashboards.Metric.Resources.Services;
using EdFi.Dashboards.Resources.Metric.Requests;
using EdFi.Dashboards.Resources.Security.Common;
using EdFi.Dashboards.SecurityTokenService.Authentication;

namespace EdFi.Dashboards.Resources.Security
{
    public class MetricInterceptor : FilteringInterceptorBase, IInterceptorStage
    {
        private readonly IMetricActionUrlAuthorizationProvider authorizationProvider;
        private readonly ICurrentUserClaimInterrogator currentUserClaimInterrogator;

        public MetricInterceptor(IMetricActionUrlAuthorizationProvider authorizationProvider, ICurrentUserClaimInterrogator currentUserClaimInterrogator)
        {
            this.authorizationProvider = authorizationProvider;
            this.currentUserClaimInterrogator = currentUserClaimInterrogator;
        }

        public StageResult BeforeExecution(IInvocation invocation, bool topLevelIntercept)
        {
            var filterResults = invocation.Method.GetCustomAttributes(typeof(ApplyMetricSecurityAttribute), false).Length > 0;
            var stageResult = new StageResult {State = filterResults, Proceed = true};

            if (filterResults)
            {
                //Dynamic so that type can determine method execution at runtime.
                dynamic request = invocation.Arguments[0];

                int metricId = request.MetricVariantId;

                if (!currentUserClaimInterrogator.HasClaimForMetricWithinEducationOrganizationHierarchy(metricId, GetEducationalOrganization(request)))
                {
                    stageResult.Proceed = false; // TODO: GKM - Throw exception here instead of return default value?

                    // We're "modifying" this result to be default(T) where T is the method invocation result type, so mark derived results as not cacheable
                    MarkCurrentResultProcessedForModification();
                }
            }

            return stageResult;
        }

		private static int GetEducationalOrganization(SchoolMetricInstanceSetRequest request)
        {
            return request.SchoolId;
        }

		private static int GetEducationalOrganization(StudentSchoolMetricInstanceSetRequest request)
        {
            return request.SchoolId;
        }

		private static int GetEducationalOrganization(LocalEducationAgencyMetricInstanceSetRequest request)
        {
            return request.LocalEducationAgencyId;
        }

        public void AfterExecution(IInvocation invocation, bool topLevelIntercept, StageResult state)
        {
            var filterResults = (bool) state.State;

            if (!filterResults)
                return;

            var metricTree = invocation.ReturnValue as MetricTree;

            if (metricTree == null)
                return;

            var metricBaseToDisplay = metricTree.RootNode;

            // We're processing this item for modification based on user permissions, so indicate that caching of derived results is not allowed
            MarkCurrentResultProcessedForModification();

            var request = invocation.Arguments[0] as MetricInstanceSetRequestBase;

            if (request == null)
                throw new ArgumentException(string.Format("Unexpected metric instance set request type '{0}' does not inherit from base class '{1}'.", 
                    invocation.Arguments[0].GetType().Name, typeof(MetricInstanceSetRequestBase).Name));

            // Note: Use of 'dynamic' keyword for request variable has been removed due to performance considerations.  
            // Less "future-proof" explicit type checks have been used here due to frequency of execution.
            var leaRequest = request as LocalEducationAgencyMetricInstanceSetRequest;

            if (leaRequest != null)
            {
                // now that we have the tree back, remove the root operational metric which will remove all of the children metrics
                RemoveInaccessableOperationalMetrics(metricBaseToDisplay, leaRequest);
                RemoveInaccessableMetricActions(metricBaseToDisplay, GetEducationalOrganization(leaRequest));
            }
            else
            {
                var schoolRequest = request as SchoolMetricInstanceSetRequest;

                if (schoolRequest != null)
                {
                    // now that we have the tree back, remove the root operational metric which will remove all of the children metrics
                    RemoveInaccessableOperationalMetrics(metricBaseToDisplay, schoolRequest);
                    RemoveInaccessableMetricActions(metricBaseToDisplay, GetEducationalOrganization(schoolRequest));
                }
                else
                {
                    var studentSchoolRequest = request as StudentSchoolMetricInstanceSetRequest;

                    if (studentSchoolRequest != null)
                    {
                        // now that we have the tree back, remove the root operational metric which will remove all of the children metrics
                        RemoveInaccessableOperationalMetrics(metricBaseToDisplay, studentSchoolRequest);
                        RemoveInaccessableMetricActions(metricBaseToDisplay, GetEducationalOrganization(studentSchoolRequest));
                    }
                    else
                    {
                        // Safety measure, but this should never happen (unless a new metric instance set type is added)
                        throw new ArgumentException(string.Format("Unexpected metric instance set request type '{0}'.", invocation.Arguments[0].GetType().Name));
                    }
                }
            }
            
            RemoveLinksThatUserShouldNotSee(invocation, metricBaseToDisplay);
        }

        private void RemoveLinksThatUserShouldNotSee(IInvocation invocation, MetricBase metricBaseToDisplay)
        {
            if (!invocation.Arguments.Any())
                return;

            var request = invocation.Arguments[0];
            
            if (request.GetType() != typeof(LocalEducationAgencyMetricInstanceSetRequest))
                return;

            // In the absence of filtering of drilldowns at the LEA level, we are using the ViewAllMetrics claim to prevent access to the drilldowns. 
            var hasClaim =
                currentUserClaimInterrogator.HasClaimForLocalEducationAgencyWithinEducationOrganizationHierarchy(
                    EdFiClaimTypes.ViewAllMetrics,
                    ((LocalEducationAgencyMetricInstanceSetRequest)request).LocalEducationAgencyId);

            if (hasClaim) 
                return;

            var container = metricBaseToDisplay as ContainerMetric;

            if (container != null)
            {
                foreach (var m in container.DescendantsOrSelf)
                {
                    m.Actions = new List<MetricAction>();
                }
            }
        }

        public static bool OuterLayerCall(MethodBase methodBase)
        {
            var stackTrace = new StackTrace();
            var stackFrames = stackTrace.GetFrames();
            
            int i = 0;

            foreach (var stackFrame in stackFrames)
            {
                i++;
                if (i < 3)
                    continue;

                var stackMethod = stackFrame.GetMethod();
                if (stackMethod.DeclaringType == methodBase.DeclaringType && stackMethod.Name == methodBase.Name)
                    return false;
            }
            return true;
        }

        private void RemoveInaccessableOperationalMetrics(MetricBase metricBaseToDisplay, SchoolMetricInstanceSetRequest request)
        {
            if (!currentUserClaimInterrogator.HasClaimForMetricWithinEducationOrganizationHierarchy((int)SchoolMetricEnum.OperationsDashboard, request.SchoolId))
            {
                RemoveTreeNode(metricBaseToDisplay, (int)SchoolMetricEnum.OperationsDashboard, request.MetricVariantId);
            }
        }

        private void RemoveInaccessableOperationalMetrics(MetricBase metricBaseToDisplay, StudentSchoolMetricInstanceSetRequest request)
        {
            //Do nothing here Student metrics don't have Operational Metrics.
            //But we need this method.
        }

        private void RemoveInaccessableOperationalMetrics(MetricBase metricBaseToDisplay, LocalEducationAgencyMetricInstanceSetRequest request)
        {
            if (!currentUserClaimInterrogator.HasClaimForMetricWithinEducationOrganizationHierarchy((int)LocalEducationAgencyMetricEnum.OperationsDashboard, request.LocalEducationAgencyId))
            {
                RemoveTreeNode(metricBaseToDisplay, (int)LocalEducationAgencyMetricEnum.OperationsDashboard, request.MetricVariantId);
            }
        }

        private void RemoveInaccessableMetricActions(MetricBase metricBaseToDisplay, int educationalOrganization)
        {
            var i = 0;
            while (i < metricBaseToDisplay.Actions.Count)
            {
                var metricAction = metricBaseToDisplay.Actions[i];
                if (!authorizationProvider.CurrentUserHasAccessToPath(metricAction.Url, educationalOrganization))
                    metricBaseToDisplay.Actions.RemoveAt(i);
                else
                    i++;
            }

            var container = metricBaseToDisplay as ContainerMetric;
            if (container != null)
                foreach (var child in container.Children)
                    RemoveInaccessableMetricActions(child, educationalOrganization);
        }

        private void RemoveTreeNode(MetricBase parent, int idToFind, int metricVariantIdToRemove)
        {

            var parentAsContainer = parent as ContainerMetric;
            
            if (parentAsContainer==null)
                return;


            if (metricVariantIdToRemove == parentAsContainer.MetricVariantId)
            {
                parentAsContainer.Children = new List<MetricBase>();
                parentAsContainer.Name = "You do not have access to this resource.";
                return;
            }

            MetricBase metricToRemove = null;
            foreach (var child in parentAsContainer.Children)
            {
                if (child.MetricVariantId == idToFind)
                {
                    metricToRemove = child;
                    break;
                }

                var childAsContainer = child as ContainerMetric;
                if (childAsContainer != null)
                    RemoveTreeNode(childAsContainer, idToFind, metricVariantIdToRemove);

            }

            if (metricToRemove != null)
            {
                //We have a IEnumerable so we have to supply a new collection. *
                var metricsToPreserve = parentAsContainer.Children.Where(x => x.MetricId != metricToRemove.MetricId);

                parentAsContainer.Children = metricsToPreserve.ToList();
            }
        }
    }
}
