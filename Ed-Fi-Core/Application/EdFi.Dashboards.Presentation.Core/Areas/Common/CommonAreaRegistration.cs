// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System.Collections.Generic;
using System.Web.Mvc;
using EdFi.Dashboards.Presentation.Architecture.Mvc.AreaRegistration;
using EdFi.Dashboards.Presentation.Core.Controllers;

namespace EdFi.Dashboards.Presentation.Core.Areas.Common
{
    public class CommonAreaRegistration : EdFiAreaRegistrationBase
    {
        public override string AreaName
        {
            get { return "Common"; }
        }

        protected override List<RouteMapping> GetDefaultRouteMappings()
        {
            var routeMappings = new List<RouteMapping>();

            routeMappings.Add(new RouteMapping(
                "Common_Default", // Route name
                "Common/{controller}/{action}/{id}", // URL with parameters
                new { controller = "Home", action = "Get", id = UrlParameter.Optional }, // Parameter defaults
                new[]
                    {
                        typeof (CommonAreaRegistration).Namespace + ".*",
                        typeof (MasterPageController).Namespace + ".*",
                    }
                ));

            return routeMappings;
        }
    }
}
