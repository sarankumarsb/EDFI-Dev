using EdFi.Dashboards.Presentation.Core.Areas.Admin.Controllers;
using EdFi.Dashboards.Presentation.Core.Tests.Routing.Support;
using EdFi.Dashboards.Presentation.Core.UITests.Attributes;
using EdFi.Dashboards.Presentation.Core.UITests.Pages.School;
using EdFi.Dashboards.Presentation.Core.UITests.Pages.Staff;
using EdFi.Dashboards.Presentation.Core.UITests.Support;
using EdFi.Dashboards.Presentation.Core.UITests.Support.SpecFlow;
using EdFi.Dashboards.Resources.Models.School.Information;
using TechTalk.SpecFlow;

namespace EdFi.Dashboards.Presentation.Core.UITests.Pages.Admin
{
    [AssociatedController(typeof (ConfigurationController))]
    public class ConfigurationPage : PageBase
    {
        protected virtual string SystemMessageBoxCss
        {
            get { return "#SystemMessage"; }
        }

        protected virtual string SaveButtonCss
        {
            get { return "#saveButton"; }
        }

        protected virtual string KillSwitchActivatorCss
        {
            get { return "#IsKillSwitchActivated"; }
        }

        public override void Visit(bool forceNavigation = false)
        {
            if (!IsCurrent() || forceNavigation)
                Website.Admin.Configuration(TestSessionContext.Current.Configuration.LocalEducationAgencyId);
        }

        public void ActivateKillSwitch()
        {
            Browser.FindCss(KillSwitchActivatorCss).Check();
            Browser.FindCss(SaveButtonCss).Click();
        }

        public void DeactivateKillSwitch()
        {
            Browser.FindCss(KillSwitchActivatorCss).Uncheck();
            Browser.FindCss(SaveButtonCss).Click();
        }

        public void SetSystemMessage(string message)
        {
            Browser.FindCss(SystemMessageBoxCss).FillInWith(message);
            Browser.FindCss(SaveButtonCss).Click();
        }
    }
}
