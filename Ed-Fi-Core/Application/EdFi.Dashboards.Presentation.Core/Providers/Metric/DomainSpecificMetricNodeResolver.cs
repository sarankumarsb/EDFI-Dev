// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System;
using EdFi.Dashboards.Core;
using EdFi.Dashboards.Core.Providers.Context;
using EdFi.Dashboards.Common;
using EdFi.Dashboards.Metric.Resources.Models;
using EdFi.Dashboards.Metric.Resources.Services;
using EdFi.Dashboards.Resources.Metric;

namespace EdFi.Dashboards.Presentation.Core.Providers.Metric
{
    public class DomainSpecificMetricNodeResolver : IDomainSpecificMetricNodeResolver
    {
        private readonly IEdFiDashboardContextProvider dashboardContextProvider;
        private readonly IMetricMetadataTreeService metricMetadataTreeService;
        private readonly IRootMetricNodeResolver rootMetricNodeResolver;

        public DomainSpecificMetricNodeResolver(IEdFiDashboardContextProvider dashboardContextProvider,
            IMetricMetadataTreeService metricMetadataTreeService,
            IRootMetricNodeResolver rootMetricNodeResolver)
        {
            this.dashboardContextProvider = dashboardContextProvider;
            this.metricMetadataTreeService = metricMetadataTreeService;
            this.rootMetricNodeResolver = rootMetricNodeResolver;
        }

        public MetricMetadataNode GetOperationalDashboardMetricNode(MetricInstanceSetType metricInstanceSetType = MetricInstanceSetType.None, int? schoolId = null)
        {
            // If we're not given the set type, get it from the request context
            if (metricInstanceSetType == MetricInstanceSetType.None)
            {
                //Based on DomainEntity context lets grab the appropriate root.
                EdFiDashboardContext context = dashboardContextProvider.GetEdFiDashboardContext();

                metricInstanceSetType = context.MetricInstanceSetType;

                if (metricInstanceSetType == MetricInstanceSetType.School)
                    schoolId = context.SchoolId;
            }

            var metaDataTree = metricMetadataTreeService.Get(MetricMetadataTreeRequest.Create());

            switch (metricInstanceSetType)
            {
                case MetricInstanceSetType.School:
                    if (Convert.ToInt32(schoolId) == 0)
                        throw new Exception("School Id was not provided (or possibly found in dashboard context) for determining the school-level operational dashboard metric node.");

                    var schoolCategory = rootMetricNodeResolver.GetMetricHierarchyRoot(schoolId.Value);

                    switch (schoolCategory)
                    {
                        case MetricHierarchyRoot.Elementary:
                            return metaDataTree.AllNodesByMetricNodeId.GetValueOrDefault((int)SchoolMetricNodeIdEnum.ElementarychoolOperationalDashboard);
                        case MetricHierarchyRoot.MiddleSchool:
                            return metaDataTree.AllNodesByMetricNodeId.GetValueOrDefault((int)SchoolMetricNodeIdEnum.MiddleSchoolOperationalDashboard);
                        case MetricHierarchyRoot.HighSchool:
                            return metaDataTree.AllNodesByMetricNodeId.GetValueOrDefault((int)SchoolMetricNodeIdEnum.HighSchoolOperationalDashboard);
                    }
                    break;

                case MetricInstanceSetType.LocalEducationAgency:
                    return metaDataTree.AllNodesByMetricNodeId.GetValueOrDefault((int)LocalEducationAgencyMetricNodeIdEnum.OperationalDashboard);

                case MetricInstanceSetType.StudentSchool:
                case MetricInstanceSetType.Staff:
                default:
                    throw new NotSupportedException("Metric Instance Set Type '" + metricInstanceSetType + "' is not supported.");
            }

            // Should never get here, but need to make the compiler happy
            throw new NotSupportedException("Metric Instance Set Type '" + metricInstanceSetType + "' is not supported.");
        }

        public MetricMetadataNode GetAdvancedCoursePotentialMetricNode()
        {
            //Based on DomainEntity context lets grab the appropriate root.
            var context = dashboardContextProvider.GetEdFiDashboardContext();
            var metaDataTree = metricMetadataTreeService.Get(MetricMetadataTreeRequest.Create());

            switch (context.MetricInstanceSetType)
            {
                case MetricInstanceSetType.StudentSchool:
                    var schoolCategory = rootMetricNodeResolver.GetMetricHierarchyRoot(context.SchoolId.Value);
                    switch (schoolCategory)
                    {
                        case MetricHierarchyRoot.MiddleSchool:
                            return metaDataTree.AllNodesByMetricNodeId.GetValueOrDefault((int)SchoolMetricNodeIdEnum.MiddleAdvancedCoursePotential);
                        case MetricHierarchyRoot.HighSchool:
                            return metaDataTree.AllNodesByMetricNodeId.GetValueOrDefault((int)SchoolMetricNodeIdEnum.HighAdvancedCoursePotential);
                    }
                    break;
                case MetricInstanceSetType.LocalEducationAgency:
                case MetricInstanceSetType.School:
                case MetricInstanceSetType.Staff:
                default:
                    throw new NotSupportedException("Metric Instance Set Type '" + context.MetricInstanceSetType + "' is not supported.");
            }

            //Will probably never get here...
            throw new NotSupportedException("Metric Instance Set Type '" + context.MetricInstanceSetType + "' is not supported.");
        }

        public MetricMetadataNode GetAdvancedCourseEnrollmentMetricNode()
        {
            //Based on DomainEntity context lets grab the appropriate root.
            var context = dashboardContextProvider.GetEdFiDashboardContext();
            var metaDataTree = metricMetadataTreeService.Get(MetricMetadataTreeRequest.Create());

            switch (context.MetricInstanceSetType)
            {
                case MetricInstanceSetType.StudentSchool:
                    var schoolCategory = rootMetricNodeResolver.GetMetricHierarchyRoot(context.SchoolId.Value);
                    switch (schoolCategory)
                    {
                        case MetricHierarchyRoot.MiddleSchool:
                            return metaDataTree.AllNodesByMetricNodeId.GetValueOrDefault((int)SchoolMetricNodeIdEnum.MiddleAdvancedCourseEnrollment);
                        case MetricHierarchyRoot.HighSchool:
                            return metaDataTree.AllNodesByMetricNodeId.GetValueOrDefault((int)SchoolMetricNodeIdEnum.HighAdvancedCourseEnrollment);
                    }
                    break;
                case MetricInstanceSetType.LocalEducationAgency:
                case MetricInstanceSetType.School:
                case MetricInstanceSetType.Staff:
                default:
                    throw new NotSupportedException("Metric Instance Set Type '" + context.MetricInstanceSetType + "' is not supported.");
            }

            //Will probably never get here...
            throw new NotSupportedException("Metric Instance Set Type '" + context.MetricInstanceSetType + "' is not supported.");
        }

        public MetricMetadataNode GetSchoolHighSchoolGraduationPlan()
        {
            //Based on DomainEntity context lets grab the appropriate root.
            var context = dashboardContextProvider.GetEdFiDashboardContext();
            var metaDataTree = metricMetadataTreeService.Get(MetricMetadataTreeRequest.Create());

            switch (context.MetricInstanceSetType)
            {
                case MetricInstanceSetType.School:
                    var schoolCategory = rootMetricNodeResolver.GetMetricHierarchyRoot(context.SchoolId.Value);
                    switch (schoolCategory)
                    {
                        case MetricHierarchyRoot.HighSchool:
                            return metaDataTree.AllNodesByMetricNodeId.GetValueOrDefault((int)SchoolMetricNodeIdEnum.SchoolHighSchoolGraduationPlan);
                    }
                    break;
                case MetricInstanceSetType.LocalEducationAgency:
                case MetricInstanceSetType.StudentSchool:
                case MetricInstanceSetType.Staff:
                default:
                    throw new NotSupportedException("Metric Instance Set Type '" + context.MetricInstanceSetType + "' is not supported.");
            }

            // Should never get here...
            throw new NotSupportedException("Metric Instance Set Type '" + context.MetricInstanceSetType + "' is not supported.");
        }

        private enum LocalEducationAgencyMetricNodeIdEnum
        {
            OperationalDashboard = 1675,
        }

        private enum SchoolMetricNodeIdEnum
        {
            HighSchoolOperationalDashboard = 290,
            MiddleSchoolOperationalDashboard = 846,
            ElementarychoolOperationalDashboard = 820,

            HighAdvancedCoursePotential = 1663,
            MiddleAdvancedCoursePotential = 1665,
            //None for elementary ElementaryAdvancedCoursePotential = ???,

            HighAdvancedCourseEnrollment = 1664,
            MiddleAdvancedCourseEnrollment = 1666,
            //None for elementary ElementaryAdvancedCoursePotential = ???,

            SchoolHighSchoolGraduationPlan = 1883, //MetricId:1470
        }
    }
}