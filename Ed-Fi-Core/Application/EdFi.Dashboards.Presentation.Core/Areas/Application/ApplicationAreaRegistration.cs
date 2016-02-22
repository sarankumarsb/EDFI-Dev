using System.Collections.Generic;
using System.Web.Mvc;
using EdFi.Dashboards.Presentation.Architecture.Mvc.AreaRegistration;
using EdFi.Dashboards.Presentation.Core.Areas.LocalEducationAgency;
using EdFi.Dashboards.Presentation.Core.Controllers;
using EdFi.Dashboards.Presentation.Web.Utilities;

namespace EdFi.Dashboards.Presentation.Core.Areas.Application
{
    public class ApplicationAreaRegistration : EdFiAreaRegistrationBase
    {
        public override string AreaName
        {
            get { return "Application"; }
        }

        protected override List<RouteMapping> GetDefaultRouteMappings()
        {
            var routeMappings = new List<RouteMapping>();

            routeMappings.Add(new RouteMapping(
                "Application_Resources",
                LocalEducationAgencyAreaRegistration.LocalEducationAgencyPrefix + "/{localEducationAgency}/Application/{controller}",
                new { action = "Get" },
                new
                {
                    localEducationAgency = RoutingPatterns.LocalEducationAgency,
                    controller = RoutingPatterns.Resource
                }
                ));
            
            routeMappings.Add(new RouteMapping(
                "Application_Default", // Route name
                "Application/{controller}/{action}/{id}", // URL with parameters
                new { controller = "Home", action = "Get", id = UrlParameter.Optional }, // Parameter defaults
                new[]
                    {
                        typeof (ApplicationAreaRegistration).Namespace + ".*",
                        typeof (MasterPageController).Namespace + ".*",
                    }
                ));

            return routeMappings;
        }
    }
}
