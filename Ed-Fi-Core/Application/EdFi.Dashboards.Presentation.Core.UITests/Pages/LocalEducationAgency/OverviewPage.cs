using EdFi.Dashboards.Presentation.Architecture.Mvc.Controllers;
using EdFi.Dashboards.Presentation.Core.Tests.Routing.Support;
using EdFi.Dashboards.Presentation.Core.UITests.Attributes;
using EdFi.Dashboards.Presentation.Core.UITests.Support;
using EdFi.Dashboards.Resources.LocalEducationAgency;
using EdFi.Dashboards.Resources.Models.LocalEducationAgency.Overview;

namespace EdFi.Dashboards.Presentation.Core.UITests.Pages.LocalEducationAgency
{
    [AssociatedController(typeof(ServicePassthroughController<OverviewRequest, OverviewModel>))]
    public class OverviewPage : PageBase<OverviewModel>
    {
        public override void Visit(bool forceNavigation = false)
        {
            if (!IsCurrent() || forceNavigation)
            {
                string url = Website.LocalEducationAgency.Overview(
                    int.MaxValue, new {localEducationAgency = TestSessionContext.Current.Configuration.LocalEducationAgency});
                
                Browser.Visit(url);
            }
        }

        public virtual string AccountabilityRatingLabel
        {
            get { return Browser.FindCss(".accountability-rating p span").Text; }
        }

        public virtual string AccountabilityRating
        {
            get { return Browser.FindCss(".accountability-rating p span:nth-child(2)").Text; }
        }
    }
}
