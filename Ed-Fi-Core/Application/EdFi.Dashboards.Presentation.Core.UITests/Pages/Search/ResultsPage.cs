
using System.Collections.Generic;
using EdFi.Dashboards.Presentation.Architecture.Mvc.Controllers;
using EdFi.Dashboards.Presentation.Core.UITests.Attributes;
using EdFi.Dashboards.Presentation.Core.UITests.Pages.Staff;
using EdFi.Dashboards.Presentation.Core.UITests.Support.Coypu;
using EdFi.Dashboards.Resources.Models.School.Information;
using EdFi.Dashboards.Resources.Models.Staff;
using EdFi.Dashboards.Resources.Staff;

namespace EdFi.Dashboards.Presentation.Core.UITests.Pages.Search
{
    [AssociatedController(typeof (ServicePassthroughController<GeneralOverviewRequest, GeneralOverviewModel>))]
    public class ResultsPage : SearchBasedPageBase<GeneralOverviewPage, InformationModel>
    {
        protected new virtual string SearchBoxSelectorCss
        {
            get { return "#AutoComplete"; }
        }
        
        protected virtual string StudentNamesSelectorCss
        {
            get { return ".MetricAlternatingOddRowStyle td:nth-child({0}) a[class ='MetricCellLinkStyle']"; }
           //               ↑ Class of TR               ↑ 2nd TD in each row (name)    ↑ a tag class (contains student name)
            // Firstname = 2, Middlename = 3, Lastname = 4
        }


        public override void Visit(bool forceNavigation = false)
        {
            throw new System.NotImplementedException();
        }

        public StudentFirstAndLastNames SelectFirstSearchResult()
        {
            var names = new StudentFirstAndLastNames();
            // Save names
            var firstResultFirstName = Browser.FindCss(string.Format(StudentNamesSelectorCss, "1"));
            names.FirstName = firstResultFirstName.Text;

            var firstResultLastName = Browser.FindCss(string.Format(StudentNamesSelectorCss, "1"));
            names.LastName = firstResultLastName.Text;
            // Click on search result
            firstResultFirstName.Click();
            return names;
        }
        
    }
}
