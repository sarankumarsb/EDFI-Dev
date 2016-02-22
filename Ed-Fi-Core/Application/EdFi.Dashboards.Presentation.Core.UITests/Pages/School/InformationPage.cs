using System;
using EdFi.Dashboards.Presentation.Architecture.Mvc.Controllers;
using EdFi.Dashboards.Presentation.Core.UITests.Attributes;
using EdFi.Dashboards.Presentation.Core.UITests.Support.SpecFlow;
using EdFi.Dashboards.Resources.Models.School.Information;
using EdFi.Dashboards.Resources.School;
using TechTalk.SpecFlow;

namespace EdFi.Dashboards.Presentation.Core.UITests.Pages.School
{
    [AssociatedController(typeof(ServicePassthroughController<InformationRequest, InformationModel>))]
    public class InformationPage : SchoolBasedPageBase<InformationPage, InformationModel>
    {
        /// <summary>
        /// Gets the format specifier for locating the link (using the link's href) whose text contains the desired value.
        /// </summary>
        protected virtual string LinkCssFormat
        {
            get { return "span.value > a[href='{0}']"; }
        }

        public override void Visit(bool forceNavigation = false)
        {
            if (!IsCurrent() || forceNavigation)
            {
                // Navigate to the school overview page first
                Container.Resolve<OverviewPage>().For(schoolType).Visit();
                
                // Navigate to the School Information using hyperlinks
                ScenarioContext.Current.GetBrowser().FindLink("School Information").Click();
            }
        }

        public virtual decimal? PercentFemales
        {
            get { return GetLinkedPercentageValue("Female"); }
        }

        public virtual decimal? PercentMales
        {
            get { return GetLinkedPercentageValue("Male"); }
        }

        public virtual decimal? PercentHispanicLatino
        {
            get { return GetLinkedPercentageValue("Hispanic-Latino"); }
        }

        //The next Three Methods are broken as of the ADA change, causing the CSS selector to be incompatible. Did not fix, as they are not in use.
        public virtual decimal? PercentHighSchoolGraduationPlanMinimum
        {
            get { return GetLinkedPercentageValue("Minimum"); }
        }

        public virtual decimal? PercentHighSchoolGraduationPlanRecommended
        {
            get { return GetLinkedPercentageValue("Recommended"); }
        }

        public virtual decimal? PercentHighSchoolGraduationPlanDistinguised
        {
            get { return GetLinkedPercentageValue("Distinguished"); }
        }

        protected virtual decimal? GetLinkedPercentageValue(string labelText)
        {
            var link = Browser.FindLink(labelText);

            string href = link["href"];

            // Find value link by finding the non-bold class with the same "href"

            string linkText = Browser.FindCss(string.Format(LinkCssFormat, href)).Text;

            return Convert.ToDecimal(linkText.TrimEnd('%'));
        }
    }
}
