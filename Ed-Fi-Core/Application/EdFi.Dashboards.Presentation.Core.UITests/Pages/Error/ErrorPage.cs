using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EdFi.Dashboards.Presentation.Architecture.Mvc.Controllers;
using EdFi.Dashboards.Presentation.Core.UITests.Attributes;
using EdFi.Dashboards.Presentation.Core.UITests.Pages.Staff;
using EdFi.Dashboards.Resources.Models.School.Information;
using EdFi.Dashboards.Resources.Models.Staff;
using EdFi.Dashboards.Resources.Staff;

namespace EdFi.Dashboards.Presentation.Core.UITests.Pages.Error
{
    [AssociatedController(typeof (ServicePassthroughController<GeneralOverviewRequest, GeneralOverviewModel>))]
    public class ErrorPage : ErrorBasedPageBase<GeneralOverviewPage, InformationModel>
    {
        protected virtual string ErrorLinkCss
        {
            get { return "#ExceptionDetailsLink"; }
        }
        
        
        public override void Visit(bool forceNavigation = false)
        {
            throw new System.NotImplementedException();
        }

        public void ShowErrorDetails()
        {
            Browser.FindCss(ErrorLinkCss).Click();
        }
        public bool IsOnErrorPage() //TODO: Improve this method
        {
            if (Browser.FindCss(ErrorLinkCss).Exists())
            {
                return true;
            }
            return false;
        }

    }
}

