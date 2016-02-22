using System.Collections.Generic;
using System.Web.Mvc;
using EdFi.Dashboards.Presentation.Architecture.Mvc.AreaRegistration;
using EdFi.Dashboards.Presentation.Core.Areas.LocalEducationAgency;
using EdFi.Dashboards.Presentation.Core.Controllers;
using EdFi.Dashboards.Presentation.Web.Utilities;

namespace EdFi.Dashboards.Presentation.Core.Areas.Error
{
    public class ErrorAreaRegistration : EdFiAreaRegistrationBase
    {        
        public override string AreaName
        {
            get { return "Error"; }
        }

        protected override List<RouteMapping> GetDefaultRouteMappings()
        {
            var routeMappings = new List<RouteMapping>();

            routeMappings.Add(new RouteMapping(
                "Error_Resources",
                LocalEducationAgencyAreaRegistration.LocalEducationAgencyPrefix + "/{localEducationAgency}/Error/{controller}",
                new {action = "Get"},
                new
                    {
                        localEducationAgency = RoutingPatterns.LocalEducationAgency
                    }
                ));

            // This allows the MasterPageController to be resolved.
            routeMappings.Add(new RouteMapping(
                "Error_Default", // Route name
                "Error/{controller}/{action}/{id}", // URL with parameters
                new {action = "Get", controller = "Error", id = UrlParameter.Optional}, // Parameter defaults
                new[]
                    {
                        typeof (ErrorAreaRegistration).Namespace + ".*",
                        typeof (MasterPageController).Namespace + ".*",
                    }
                ));

            return routeMappings;
        }
    }
}