// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System;
using System.Collections.Generic;
using System.Web.Mvc;
using EdFi.Dashboards.Presentation.Architecture.Mvc.AreaRegistration;
using EdFi.Dashboards.Presentation.Core.Controllers;
using EdFi.Dashboards.Presentation.Web.Utilities;

namespace EdFi.Dashboards.Presentation.Core.Areas.LocalEducationAgency
{
    public class LocalEducationAgencyAreaRegistration : EdFiAreaRegistrationBase
    {
        public override string AreaName
        {
            get { return "LocalEducationAgency"; }
        }

        public static string LocalEducationAgencyPrefix
        {
            // If you change this value, make a corresponding change in the SiteAvailableModule class in the EdFi.Dashboards.Presentation.Architecture project.
            get { return "Districts"; }
        }

        protected override List<RouteMapping> GetDefaultRouteMappings()
        {
            var routeMappings = new List<RouteMapping>();

            routeMappings.Add(new RouteMapping(
                "LocalEducationAgency_OperationalDashboard",
                LocalEducationAgencyAreaRegistration.LocalEducationAgencyPrefix + "/{localEducationAgency}/OperationalDashboard/{operationalDashboardSubtype}",
                new
                    {
                        action = "Get",
                        controller = "DomainMetric",
                        renderingMode = "Metric",
                        operationalDashboardSubtype = "Staff"
                    },
                new
                    {
                        localEducationAgency = RoutingPatterns.LocalEducationAgency,
                        operationalDashboardSubtype = RoutingPatterns.OperationalDashboardSubType,
                    }
                ));

            routeMappings.Add(new RouteMapping(
                "LocalEducationAgency_Metrics",
                LocalEducationAgencyAreaRegistration.LocalEducationAgencyPrefix + "/{localEducationAgency}/Metrics/{metricName}-{metricVariantId}",
                new {action = "Get", controller = "DomainMetric", renderingMode = "Metric"},
                new
                    {
                        localEducationAgency = RoutingPatterns.LocalEducationAgency,
                        metricName = RoutingPatterns.MetricName,
                        metricVariantId = RoutingPatterns.Id,
                    }
                ));

            routeMappings.Add(new RouteMapping(
                "LocalEducationAgency_MetricsDrilldown",
                LocalEducationAgencyAreaRegistration.LocalEducationAgencyPrefix + "/{localEducationAgency}/Metrics/{metricName}-{metricVariantId}/{controller}",
                new { action = "Get"},
                new
                {
                    localEducationAgency = RoutingPatterns.LocalEducationAgency,
                    metricName = RoutingPatterns.MetricName,
                    metricVariantId = RoutingPatterns.Id,
                    controller = RoutingPatterns.Resource
                }
                ));

            routeMappings.Add(new RouteMapping(
                "LocalEducationAgency_GoalPlanning",
                LocalEducationAgencyAreaRegistration.LocalEducationAgencyPrefix + "/{localEducationAgency}/GoalPlanning/{metricName}-{metricVariantId}",
                new { action = "Get", controller = "GoalPlanning", renderingMode = "GoalPlanning" },
                new
                    {
                        localEducationAgency = RoutingPatterns.LocalEducationAgency,
                        metricName = RoutingPatterns.MetricName,
                        metricVariantId = RoutingPatterns.Id,
                    }
                ));

            // due to the fact that I cant add a namespace reroute to the resources route because of duplicate home controllers
            routeMappings.Add(new RouteMapping(
                "LocalEducationAgency_Entry",
                LocalEducationAgencyAreaRegistration.LocalEducationAgencyPrefix + "/{localEducationAgency}/Entry",
                new { action = "Get", controller = "Entry" },
                new
                {
                    localEducationAgency = RoutingPatterns.LocalEducationAgency,
                    controller = RoutingPatterns.Resource
                },
                new string[]
                    {
                        typeof (MasterPageController).Namespace + ".*",
                    }
                ));

            routeMappings.Add(new RouteMapping(
                "LocalEducationAgency_Demographics",
                LocalEducationAgencyAreaRegistration.LocalEducationAgencyPrefix + "/{localEducationAgency}/Demographics/{controller}/{demographic}",
                new { action = "Get", demographic = UrlParameter.Optional },
                new
                    {
                        localEducationAgency = RoutingPatterns.LocalEducationAgency,
                        controller = RoutingPatterns.Resource
                    }
                ));

            routeMappings.Add(new RouteMapping(
                "LocalEducationAgency_StudentSchoolCategory",
                LocalEducationAgencyAreaRegistration.LocalEducationAgencyPrefix + "/{localEducationAgency}/SchoolCategories/{controller}/{schoolCategory}",
                new { action = "Get", schoolCategory = UrlParameter.Optional },
                new
                {
                    localEducationAgency = RoutingPatterns.LocalEducationAgency,
                    controller = RoutingPatterns.Resource
                }
                ));

            routeMappings.Add(new RouteMapping(
                "LocalEducationAgency_StudentList",
                LocalEducationAgencyAreaRegistration.LocalEducationAgencyPrefix + "/{localEducationAgency}/{controller}/Staff/{staff}-{staffUSI}/{studentListType}/{sectionOrCohortId}",
                new { action = "Get", studentListType = "None", sectionOrCohortId = UrlParameter.Optional },
                new
                {
                    localEducationAgency = RoutingPatterns.LocalEducationAgency,
                    staff = RoutingPatterns.Staff,
                    staffUSI = RoutingPatterns.Id,
                    controller = RoutingPatterns.Resource,
                }
                ));

            routeMappings.Add(new RouteMapping(
                "LocalEducationAgency_WatchListDemographic",
                LocalEducationAgencyPrefix + "/{localEducationAgency}/Demographics/{controller}/{demographic}/{studentListType}/{sectionOrCohortId}",
                new { action = "Get", studentListType = UrlParameter.Optional, sectionOrCohortId = UrlParameter.Optional },
                new
                {
                    localEducationAgency = RoutingPatterns.LocalEducationAgency,
                    controller = RoutingPatterns.Resource,
                }
                ));

            routeMappings.Add(new RouteMapping(
                "LocalEducationAgency_WatchListSchoolCategory",
                LocalEducationAgencyPrefix + "/{localEducationAgency}/SchoolCategories/{controller}/{schoolCategory}/{studentListType}/{sectionOrCohortId}",
                new { action = "Get", studentListType = "None", sectionOrCohortId = UrlParameter.Optional },
                new
                {
                    localEducationAgency = RoutingPatterns.LocalEducationAgency,
                    controller = RoutingPatterns.Resource,
                }
                ));

            routeMappings.Add(new RouteMapping(
                "LocalEducationAgency_Resources",
                LocalEducationAgencyAreaRegistration.LocalEducationAgencyPrefix + "/{localEducationAgency}/{controller}",
                new { action = "Get", controller = "Home" },
                new
                    {
                        localEducationAgency = RoutingPatterns.LocalEducationAgency,
                        controller = RoutingPatterns.Resource,
                    }
                ));

            routeMappings.Add(new RouteMapping(
                "LocalEducationAgency_ApiResources",
                RoutingPrefixes.Api_V1 + LocalEducationAgencyAreaRegistration.LocalEducationAgencyPrefix + "/{localEducationAgency}/{controller}/{resourceIdentifier}",
                new { action = "Get", controller = "Home", resourceIdentifier = UrlParameter.Optional },
                new
                    {
                        localEducationAgency = RoutingPatterns.LocalEducationAgency,
                        controller = RoutingPatterns.Resource,
                        resourceIdentifier = ".*",
                    }
                ));

            routeMappings.Add(new RouteMapping(
                "LocalEducationAgency_Default", // Route name
                "LocalEducationAgency/{controller}/{action}/{id}", // URL with parameters
                new { controller = "Home", action = "Get", id = UrlParameter.Optional }, // Parameter defaults
                new[]
                    {
                        typeof (LocalEducationAgencyAreaRegistration).Namespace + ".*",
                        typeof (MasterPageController).Namespace + ".*",
                    }
                ));

            return routeMappings;
        }
    }
}
