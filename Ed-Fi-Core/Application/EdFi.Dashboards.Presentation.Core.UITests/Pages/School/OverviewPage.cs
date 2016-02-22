using System;
using EdFi.Dashboards.Presentation.Architecture.Mvc.Controllers;
using EdFi.Dashboards.Presentation.Core.Tests.Routing.Support;
using EdFi.Dashboards.Presentation.Core.UITests.Attributes;
using EdFi.Dashboards.Presentation.Core.UITests.Support.SpecFlow;
using EdFi.Dashboards.Resources.Models.School.Overview;
using EdFi.Dashboards.Resources.School;
using TechTalk.SpecFlow;

namespace EdFi.Dashboards.Presentation.Core.UITests.Pages.School
{
    [AssociatedController(typeof(ServicePassthroughController<OverviewRequest,OverviewModel>))]
    public class OverviewPage : SchoolBasedPageBase<OverviewPage, OverviewModel>
    {
        public override void Visit(bool forceNavigation = false)
        {
            if (!IsCurrent() || forceNavigation)
            {
                var userProfile = ScenarioContext.Current.GetUserProfile(safe: true);

                int schoolId = userProfile.GetSchoolId(schoolType);

                if (schoolId != 0)
                    Browser.Visit(Website.School.Overview(schoolId));
                else
                    throw new InvalidOperationException("Cannot navigate to the school overview page for the current user because the user profile is not associated with a specific school in the test user profile.");
            }
        }
    }
}
