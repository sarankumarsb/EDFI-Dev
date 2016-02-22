// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System.Collections.Generic;
using System.Web.Mvc;
using EdFi.Dashboards.Presentation.Architecture.Mvc.AreaRegistration;
using EdFi.Dashboards.Presentation.Core.Areas.LocalEducationAgency;
using EdFi.Dashboards.Presentation.Core.Controllers;
using EdFi.Dashboards.Presentation.Web.Utilities;

namespace EdFi.Dashboards.Presentation.Core.Areas.Staff
{
    public class StaffAreaRegistration : EdFiAreaRegistrationBase
    {
        public override string AreaName
        {
            get { return "Staff"; }
        }

        protected override List<RouteMapping> GetDefaultRouteMappings()
        {
            return new List<RouteMapping>
            {
                new RouteMapping(
                    "Staff_Resources",
                    LocalEducationAgencyAreaRegistration.LocalEducationAgencyPrefix +
                    "/{localEducationAgency}/Schools/{school}/Staff/{staff}-{staffUSI}/{controller}/{assessmentSubType}/{studentListType}/{sectionOrCohortId}",
                    new
                    {
                        action = "Get",
                        controller = "GeneralOverview",
                        assessmentSubType = "General",
                        studentListType = "None",
                        sectionOrCohortId = UrlParameter.Optional
                    },
                    new
                    {
                        localEducationAgency = RoutingPatterns.LocalEducationAgency,
                        school = RoutingPatterns.School,
                        staff = RoutingPatterns.Staff,
                        staffUSI = RoutingPatterns.Id,
                        controller = RoutingPatterns.Resource,
                        assessmentSubType = RoutingPatterns.AssessmentSubType
                    }
                ),
                new RouteMapping(
                    "Staff_LocalEducationAgency_Resources",
                    LocalEducationAgencyAreaRegistration.LocalEducationAgencyPrefix +
                    "/{localEducationAgency}/Staff/{staff}-{staffUSI}/{controller}/{assessmentSubType}/{studentListType}/{sectionOrCohortId}",
                    new
                    {
                        action = "Get",
                        controller = "GeneralOverview",
                        assessmentSubType = "General",
                        studentListType = "None",
                        sectionOrCohortId = UrlParameter.Optional
                    },
                    new
                    {
                        localEducationAgency = RoutingPatterns.LocalEducationAgency,
                        staff = RoutingPatterns.Staff,
                        staffUSI = RoutingPatterns.Id,
                        controller = RoutingPatterns.Resource,
                        assessmentSubType = RoutingPatterns.AssessmentSubType
                    }
                ),
                new RouteMapping(
                    "Staff_Default",
                    "Staff/{controller}/{action}/{id}",
                    new {controller = "Home", action = "Get", id = UrlParameter.Optional}, // Parameter defaults
                    new[]
                    {
                        typeof (StaffAreaRegistration).Namespace + ".*",
                        typeof (MasterPageController).Namespace + ".*",
                    }
                )
            };
        }
    }
}
