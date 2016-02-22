// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System.Collections.Generic;
using System.Web.Mvc;
using EdFi.Dashboards.Presentation.Architecture.Mvc.AreaRegistration;
using EdFi.Dashboards.Presentation.Core.Areas.LocalEducationAgency;
using EdFi.Dashboards.Presentation.Core.Controllers;
using EdFi.Dashboards.Presentation.Web.Utilities;

namespace EdFi.Dashboards.Presentation.Core.Areas.Search
{
    public class SearchAreaRegistration : EdFiAreaRegistrationBase
    {
        public override string AreaName
        {
            get { return "Search"; }
        }

        protected override List<RouteMapping> GetDefaultRouteMappings()
        {
            var routeMappings = new List<RouteMapping>();

            routeMappings.Add(new RouteMapping(
                "Search_Resources",
                LocalEducationAgencyAreaRegistration.LocalEducationAgencyPrefix + "/{localEducationAgency}/Search/{controller}",
                new { action = "Get" },
                new
                {
                    localEducationAgency = RoutingPatterns.LocalEducationAgency,
                    controller = RoutingPatterns.Resource
                }
                ));

            routeMappings.Add(new RouteMapping(
                "Search_default",
                "Search/{controller}/{action}/{id}",
                new { controller = "Home", action = "Get", id = UrlParameter.Optional }, // Parameter defaults
                new[]
                    {
                        typeof(SearchAreaRegistration).Namespace + ".*",
                        typeof(MasterPageController).Namespace + ".*",
                    }
                ));

            return routeMappings;
        }
    }
}
