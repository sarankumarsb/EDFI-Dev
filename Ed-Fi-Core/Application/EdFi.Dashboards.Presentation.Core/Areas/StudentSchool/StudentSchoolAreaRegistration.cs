// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System.Collections.Generic;
using System.Web.Mvc;
using EdFi.Dashboards.Presentation.Architecture.Mvc.AreaRegistration;
using EdFi.Dashboards.Presentation.Core.Areas.LocalEducationAgency;
using EdFi.Dashboards.Presentation.Core.Controllers;
using EdFi.Dashboards.Presentation.Web.Utilities;

namespace EdFi.Dashboards.Presentation.Core.Areas.StudentSchool
{
    public class StudentSchoolAreaRegistration : EdFiAreaRegistrationBase
    {
        public override string AreaName
        {
            get { return "StudentSchool"; }
        }

        protected override List<RouteMapping> GetDefaultRouteMappings()
        {
            var routeMappings = new List<RouteMapping>();

            routeMappings.Add(new RouteMapping(
                "StudentSchool_Metrics",
                LocalEducationAgencyAreaRegistration.LocalEducationAgencyPrefix + "/{localEducationAgency}/Schools/{school}/Students/{student}-{studentUSI}/Metrics/{metricName}-{metricVariantId}",
                new { action = "Get", controller = "DomainMetric", renderingMode = "Metric" },
                new
                {
                    localEducationAgency = RoutingPatterns.LocalEducationAgency,
                    school = RoutingPatterns.School,
                    student = RoutingPatterns.Student,
                    studentUSI = RoutingPatterns.Id,
                    metricName = RoutingPatterns.MetricName,
                    metricVariantId = RoutingPatterns.Id
                }
                ));

            //Drilldowns
            routeMappings.Add(new RouteMapping(
                "StudentSchool_MetricsDrilldown",
                LocalEducationAgencyAreaRegistration.LocalEducationAgencyPrefix + "/{localEducationAgency}/Schools/{school}/Students/{student}-{studentUSI}/Metrics/{metricName}-{metricVariantId}/{controller}",
                new { action = "Get" },
                new
                {
                    localEducationAgency = RoutingPatterns.LocalEducationAgency,
                    school = RoutingPatterns.School,
                    student = RoutingPatterns.Student,
                    studentUSI = RoutingPatterns.Id,
                    metricName = RoutingPatterns.MetricName,
                    metricVariantId = RoutingPatterns.Id,
                    controller = RoutingPatterns.Resource
                }
                ));

            routeMappings.Add(new RouteMapping(
                "StudentSchool_Resources",
                LocalEducationAgencyAreaRegistration.LocalEducationAgencyPrefix + "/{localEducationAgency}/Schools/{school}/Students/{student}-{studentUSI}/{controller}",
                new { action = "Get", controller = "Overview" },
                new
                    {
                        localEducationAgency = RoutingPatterns.LocalEducationAgency,
                        school = RoutingPatterns.School,
                        student = RoutingPatterns.Student,
                        studentUSI = RoutingPatterns.Id,
                        controller = RoutingPatterns.Resource
                    }
                ));

            routeMappings.Add(new RouteMapping(
                "StudentSchool_Default",
                "StudentSchool/{controller}/{action}/{id}",
                new { controller = "Home", action = "Get", id = UrlParameter.Optional }, // Parameter defaults
                new[]
                    {
                        typeof(StudentSchoolAreaRegistration).Namespace + ".*",
                        typeof(MasterPageController).Namespace + ".*",
                    }));

            return routeMappings;
        }
    }
}
