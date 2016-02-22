// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System.Collections.Generic;
using System.Web.Mvc;
using EdFi.Dashboards.Presentation.Architecture.Mvc.AreaRegistration;
using EdFi.Dashboards.Presentation.Core.Areas.LocalEducationAgency;
using EdFi.Dashboards.Presentation.Core.Controllers;
using EdFi.Dashboards.Presentation.Web.Utilities;

namespace EdFi.Dashboards.Presentation.Core.Areas.School
{
    public class SchoolAreaRegistration : EdFiAreaRegistrationBase
    {
        public override string AreaName
        {
            get { return "School"; }
        }

        protected override List<RouteMapping> GetDefaultRouteMappings()
        {
            var routeMappings = new List<RouteMapping>();

            // Operational dashboard
            routeMappings.Add(new RouteMapping(
                "School_OperationalDashboard",
                LocalEducationAgencyAreaRegistration.LocalEducationAgencyPrefix + "/{localEducationAgency}/Schools/{school}/OperationalDashboard/{operationalDashboardSubtype}",
                new { action = "Get", controller = "DomainMetric", renderingMode = "Metric", operationalDashboardSubtype = "Staff" }, 
                new
                    {
                        localEducationAgency = RoutingPatterns.LocalEducationAgency,
                        school = RoutingPatterns.School,
                        operationalDashboardSubtype = RoutingPatterns.OperationalDashboardSubType,
                    }
                ));

            //Metrics
            routeMappings.Add(new RouteMapping(
                "School_Metrics",
                LocalEducationAgencyAreaRegistration.LocalEducationAgencyPrefix + "/{localEducationAgency}/Schools/{school}/Metrics/{metricName}-{metricVariantId}",
                new {action = "Get", controller = "DomainMetric", renderingMode = "Metric"},
                new
                    {
                        localEducationAgency = RoutingPatterns.LocalEducationAgency,
                        school = RoutingPatterns.School,
                        metricName = RoutingPatterns.MetricName,
                        metricVariantId = RoutingPatterns.Id,
                    }
                ));

            //Drilldowns
            routeMappings.Add(new RouteMapping(
                "School_MetricsDrilldown",
                LocalEducationAgencyAreaRegistration.LocalEducationAgencyPrefix + "/{localEducationAgency}/Schools/{school}/Metrics/{metricName}-{metricVariantId}/{controller}",
                new { action = "Get" },
                new
                {
                    localEducationAgency = RoutingPatterns.LocalEducationAgency,
                    school = RoutingPatterns.School,
                    metricName = RoutingPatterns.MetricName,
                    metricVariantId = RoutingPatterns.Id,
                    controller = RoutingPatterns.Resource
                }
                ));

            //Goal Planning
            routeMappings.Add(new RouteMapping(
                "School_GoalPlanning",
                LocalEducationAgencyAreaRegistration.LocalEducationAgencyPrefix + "/{localEducationAgency}/Schools/{school}/GoalPlanning/{metricName}-{metricVariantId}",
                new { action = "Get", controller = "GoalPlanning", renderingMode = "GoalPlanning" },
                new
                {
                    localEducationAgency = RoutingPatterns.LocalEducationAgency,
                    school = RoutingPatterns.School,
                    metricName = RoutingPatterns.MetricName,
                    metricVariantId = RoutingPatterns.Id,
                }
                ));

            // due to the fact that I cant add a namespace reroute to the resources route because of duplicate home controllers
            routeMappings.Add(new RouteMapping(
                "School_Entry",
                LocalEducationAgencyAreaRegistration.LocalEducationAgencyPrefix + "/{localEducationAgency}/Schools/{school}/Entry",
                new { action = "Get", controller = "Entry" },
                new
                {
                    localEducationAgency = RoutingPatterns.LocalEducationAgency,
                    school = RoutingPatterns.School,
                    controller = RoutingPatterns.Resource,
                },
                new string[]
                    {
                        typeof (MasterPageController).Namespace + ".*",
                    }
                ));
            //Main
            routeMappings.Add(new RouteMapping(
                "School_Demographics",
                LocalEducationAgencyAreaRegistration.LocalEducationAgencyPrefix + "/{localEducationAgency}/Schools/{school}/Demographics/{controller}/{demographic}",
                new { action = "Get", demographic = UrlParameter.Optional },
                new
                {
                    localEducationAgency = RoutingPatterns.LocalEducationAgency,
                    school = RoutingPatterns.School,
                    controller = RoutingPatterns.Resource,
                }
                ));

            routeMappings.Add(new RouteMapping(
                "School_WatchListDemographics",
                LocalEducationAgencyAreaRegistration.LocalEducationAgencyPrefix + "/{localEducationAgency}/Schools/{school}/Demographics/{controller}/{demographic}/{studentListType}/{sectionOrCohortId}",
                new { action = "Get", demographic = UrlParameter.Optional, studentListType = "None", sectionOrCohortId = UrlParameter.Optional },
                new
                {
                    localEducationAgency = RoutingPatterns.LocalEducationAgency,
                    school = RoutingPatterns.School,
                    controller = RoutingPatterns.Resource,
                }
                ));

            routeMappings.Add(new RouteMapping(
                "School_Grades",
                LocalEducationAgencyAreaRegistration.LocalEducationAgencyPrefix + "/{localEducationAgency}/Schools/{school}/Grades/{controller}/{grade}",
                new { action = "Get", grade = "All" },
                new
                {
                    localEducationAgency = RoutingPatterns.LocalEducationAgency,
                    school = RoutingPatterns.School,
                    controller = RoutingPatterns.Resource,
                }
                ));

            routeMappings.Add(new RouteMapping(
                "School_WatchListGrades",
                LocalEducationAgencyAreaRegistration.LocalEducationAgencyPrefix + "/{localEducationAgency}/Schools/{school}/Grades/{controller}/{grade}/{studentListType}/{sectionOrCohortId}",
                new { action = "Get", grade = "All", studentListType = "None", sectionOrCohortId = UrlParameter.Optional },
                new
                {
                    localEducationAgency = RoutingPatterns.LocalEducationAgency,
                    school = RoutingPatterns.School,
                    controller = RoutingPatterns.Resource,
                }
                ));

            routeMappings.Add(new RouteMapping(
                "School_Resources",
                LocalEducationAgencyAreaRegistration.LocalEducationAgencyPrefix + "/{localEducationAgency}/Schools/{school}/{controller}",
                new { action = "Get", controller = "Overview" },
                new
                    {
                        localEducationAgency = RoutingPatterns.LocalEducationAgency,
                        school = RoutingPatterns.School,
                        controller = RoutingPatterns.Resource,
                    }
                ));

            // This allows the MasterPageController to be resolved.
            routeMappings.Add(new RouteMapping(
                "School_Default", // Route name
                "School/{controller}/{action}/{id}", // URL with parameters
                new { controller = "Home", action = "Get", id = UrlParameter.Optional }, // Parameter defaults
                new[]
                    {
                        typeof(SchoolAreaRegistration).Namespace + ".*",
                        typeof(MasterPageController).Namespace + ".*",
                    }));

            return routeMappings;
        }
    }
}
