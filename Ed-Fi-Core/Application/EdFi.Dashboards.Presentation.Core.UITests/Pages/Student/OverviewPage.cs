using System;
using System.Collections.Generic;
using System.Linq;
using Coypu;
using EdFi.Dashboards.Presentation.Architecture.Mvc.Controllers;
using EdFi.Dashboards.Presentation.Core.UITests.Attributes;
using EdFi.Dashboards.Resources.Models.Student.Overview;
using EdFi.Dashboards.Resources.StudentSchool;

namespace EdFi.Dashboards.Presentation.Core.UITests.Pages.Student
{
    [AssociatedController(typeof(ServicePassthroughController<OverviewRequest, OverviewModel>))]
    public class OverviewPage : PageBase<OverviewModel>
    {
        public override void Visit(bool forceNavigation = false)
        {
            throw new NotImplementedException("Need to incorporate school type into page to navigate to THE correct student.");

            //if (!IsCurrent())
            //{
            //    var userProfile = ScenarioContext.Current.GetUserProfile(safe: true);

            //    if (userProfile.SchoolId != null && userProfile.StudentUSI != null)
            //        Browser.Visit(Website.StudentSchool.Overview(userProfile.SchoolId.Value, userProfile.StudentUSI.Value, "student-name-here"));
            //    else
            //        throw new InvalidOperationException("Cannot navigate to the school overview page for the current user because the user profile is not associated with a specific school in the test user profile.");
            //}
        }
        public string GetStudentNameOnStudentPage(BrowserSession browser)
        {
            string studentName = browser.FindCss(StudentFullNameSelectorCss).Text;
            return studentName;
        }
    }
}
