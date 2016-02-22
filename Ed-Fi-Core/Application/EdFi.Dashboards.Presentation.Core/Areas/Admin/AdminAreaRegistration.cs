// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System.Collections.Generic;
using System.Web.Mvc;
using EdFi.Dashboards.Presentation.Architecture.Mvc.AreaRegistration;
using EdFi.Dashboards.Presentation.Core.Areas.LocalEducationAgency;
using EdFi.Dashboards.Presentation.Core.Controllers;
using EdFi.Dashboards.Presentation.Web.Utilities;

namespace EdFi.Dashboards.Presentation.Core.Areas.Admin
{
    public class AdminAreaRegistration : EdFiAreaRegistrationBase
    {
        public override string AreaName
        {
            get { return "Admin"; }
        }

        protected override List<RouteMapping> GetDefaultRouteMappings()
        {
            var routeMappings = new List<RouteMapping>();

            routeMappings.Add(new RouteMapping(
                "Admin_Other_Action_Methods",
                LocalEducationAgencyAreaRegistration.LocalEducationAgencyPrefix + "/{localEducationAgency}/Admin/{controller}/{action}",
                new { action = "Get" },
                new
                    {
                        localEducationAgency = RoutingPatterns.LocalEducationAgency,
                        controller = RoutingPatterns.Resource,
                        action = "(EditSingleClaimSet|EditBatchClaimSet|Delete)"
                    }
                ));

            routeMappings.Add(new RouteMapping(
                "Admin_Resources",
                LocalEducationAgencyAreaRegistration.LocalEducationAgencyPrefix + "/{localEducationAgency}/Admin/{controller}",
                new { action = "Get" },
                new
                    {
                        localEducationAgency = RoutingPatterns.LocalEducationAgency,
                        controller = RoutingPatterns.Resource
                    }
                ));

            routeMappings.Add(new RouteMapping(
                "Admin_Default",
                "Admin/{controller}/{action}/{id}",
                new { controller = "Home", action = "Get", id = UrlParameter.Optional }, // Parameter defaults
                new[]
                    {
                        typeof(AdminAreaRegistration).Namespace + ".*",
                        typeof(MasterPageController).Namespace + ".*",
                    }
                ));

            return routeMappings;
        }
    }
}
