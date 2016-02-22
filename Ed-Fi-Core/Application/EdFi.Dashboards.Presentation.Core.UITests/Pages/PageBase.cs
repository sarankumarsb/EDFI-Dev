using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using BoDi;
using Coypu;
using Coypu.Drivers.Selenium;
using EdFi.Dashboards.Presentation.Core.UITests.Attributes;
using EdFi.Dashboards.Presentation.Core.UITests.Support;
using EdFi.Dashboards.Presentation.Core.UITests.Support.Coypu;
using EdFi.Dashboards.Presentation.Core.UITests.Support.SpecFlow;
using EdFi.Dashboards.Presentation.Core.Tests.Routing.Support;
using OpenQA.Selenium;
using RestSharp;
using TechTalk.SpecFlow;

using LEA_OverviewPage = EdFi.Dashboards.Presentation.Core.UITests.Pages.LocalEducationAgency.OverviewPage;
using School_OverviewPage = EdFi.Dashboards.Presentation.Core.UITests.Pages.School.OverviewPage;
using Student_OverviewPage = EdFi.Dashboards.Presentation.Core.UITests.Pages.Student.OverviewPage;

namespace EdFi.Dashboards.Presentation.Core.UITests.Pages
{
    public abstract class PageBase
    {
        public class StudentFirstAndLastNames
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
        }

        public static string StudentNameContextKey
        {
            get { return "studentName"; }
        }
        public static string StudentFullNameSelectorCss
        {
            get { return ".entity-title"; } // Using Element selector rather than ID selector due to presence of dot in ID.
        }
        protected static string SearchBoxSelectorCss
        {
            get { return "#quicksearch"; }
        }
        protected static string SearchResultsFirstStudentSelectorCss
        {
            get { return "#quickSearchStudents > ol > li > a"; } // ":nth-child(2) > li:nth-child(1) > a:nth-child(1)"; }
        }
        protected static string SearchArrowCss
        {
            get { return "#nav-main > form:nth-child(2) > button:nth-child(3)"; }
        }

        /// <summary>
        /// Gets the native WebDriver implementation for the browser session based on the user profile of the current scenario.
        /// </summary>
        protected IWebDriver Driver
        {
            get { return Browser.Driver.Native as IWebDriver; }
        }
       

        /// <summary>
        /// Gets the browser session based on the user profile of the current scenario.
        /// </summary>
        public BrowserSession Browser
        {
            get
            {
                // Get browser for the current user
                return ScenarioContext.Current.GetBrowser();
            }
        }

        /// <summary>
        /// Gets the REST client based on the user profile of the current scenario.
        /// </summary>
        public RestClient RestClient
        {
            get
            {
                // Get REST client for current user
                return ScenarioContext.Current.GetRestClient();
            }
        }

        /// <summary>
        /// Gets the IoC container for the current scenario.
        /// </summary>
        public static IObjectContainer Container
        {
            get
            {
                // Get the current scenario's IoC container
                return ScenarioContext.Current.GetBindingInstance(typeof(IObjectContainer)) as IObjectContainer;
            }
        }

        // This is exposed as a static member of the base class due to extensibility problems of trying
        // to override a common behavior that really needs to be defined at the PageBase level so that
        // all pages can access the functionality.
        private static string _systemMessageHeaderClassCss = "div.system-message";

        /// <summary>
        /// Gets the CSS class used to display the system message.
        /// </summary>
        protected static string SystemMessageHeaderClassCss
        {
            get { return _systemMessageHeaderClassCss; }
            set { _systemMessageHeaderClassCss = value; }
        }

        /// <summary>
        /// Identifies whether the webpage represented by the Page object is the current webpage in the browser.
        /// </summary>
        /// <returns><b>true</b> if the Page object represents the current browser webpage; otherwise <b>false</b>.</returns>
        public virtual bool IsCurrent(Options options = null, bool showDiagnostics = false)
        {
            var attributes = GetType().GetCustomAttributes(true);

            foreach (var attribute in attributes)
            {
                var associatedControllerAttribute = attribute as AssociatedControllerAttribute;

                if (associatedControllerAttribute != null)
                {
                    Type[] associatedControllerTypes = associatedControllerAttribute.ControllerTypes;

                    return GetIsCurrentRobustly(associatedControllerTypes, options, showDiagnostics);
                }

                var associatedUrlAttribute = attribute as AssociatedUrlAttribute;

                if (associatedUrlAttribute != null)
                {
                    string pattern = associatedUrlAttribute.RegexPattern;

                    if (Regex.IsMatch(Browser.Location.AbsolutePath, pattern))
                        return true;
                        
                    return false;
                }
            }

            throw new NotImplementedException("Page class '{0}' does not have a supported 'AssociatedXxxx' attribute for identifying the associated web page.");
        }

        private bool GetIsCurrentRobustly(Type[] associatedControllerTypes, Options options, bool showDiagnostics)
        {
            DateTime startTime = DateTime.Now;

            bool isCurrent = false;
            Type mappedControllerType;

            var actualOptions = options ?? Make_It.Do_It_Now;

            do
            {
                string virtualPath = Browser.Location.AbsoluteUri.ToVirtual();

                // Make sure path got virtualized.  If not, it's not on the website, and we're done
                if (!virtualPath.StartsWith("~"))
                    return false;

                mappedControllerType = virtualPath.Route().GetMappedControllerType();

                foreach (var associatedControllerType in associatedControllerTypes)
                {
                    isCurrent |= mappedControllerType == associatedControllerType;
                }

                // If we're not current, delay momentarily before retrying
                if (!isCurrent)
                    Thread.Sleep(actualOptions.RetryInterval);
            } while ((DateTime.Now - startTime) <= actualOptions.Timeout);

            // For debugging:
            if (!isCurrent && showDiagnostics)
            {
                Debug.WriteLine("{0} is not the current page.\n\tExpected: {1}\n\tActual: {2}",
                    this.GetType().Name, 
                    string.Join(", ", associatedControllerTypes.Select(t => t.FullName)), 
                    mappedControllerType.FullName);
            }

            return isCurrent;
        }

        /// <summary>
        /// Navigates to the webpage represented by the Page object.
        /// </summary>
        /// <param name="forceNavigation">Indicates whether to force browser navigation, even if the target page is 
        /// already the current page (useful for scenarios where we want to explicitly force the browser to make a 
        /// request from server).
        /// </param>
        public abstract void Visit(bool forceNavigation = false);

        /// <summary>
        /// Navigates to the specified Academic Dashboard page.
        /// </summary>
        /// <param name="academicDashboardType">The academic dashboard type (e.g. LEA, School, Student).</param>
        /// <param name="linkText">The text of the link for the desired Academic Dashboard subpage.</param>
        /// <param name="schoolType">The type of school Id to be used for navigation.</param>
        /// <param name="forceNavigation">Indicates whether to force browser navigation, even if the target page is 
        /// already the current page (useful for scenarios where we want to explicitly force the browser to make a 
        /// request from server).
        /// </param>
        public static void VisitAcademicDashboardSection(AcademicDashboardType academicDashboardType, string linkText, SchoolType schoolType = SchoolType.Unspecified, bool forceNavigation = false)
        {
            // TODO: revisit this navigation
            // TODO: consider adding page to context for use by MetricSteps?
            PageBase page = null;

            switch (academicDashboardType)
            {
                case AcademicDashboardType.LocalEducationAgency:
                    page = Container.Resolve<LEA_OverviewPage>();
                    break;
                case AcademicDashboardType.School:
                    page = Container.Resolve<School_OverviewPage>().For(schoolType);
                    break;
                //case AcademicDashboardType.Student:
                //    page = Container.Resolve<Student_OverviewPage>().For(schoolType);
                //    break;
                default:
                    throw new NotSupportedException(string.Format("Cannot yet visit the {0} academic dashboard yet.", academicDashboardType.ToString()));
            }

            // Navigate to the Overview page of the appropriate academic dashboard
            if (!page.IsCurrent() || forceNavigation)
                page.Visit(forceNavigation);

            var browser = ScenarioContext.Current.GetBrowser();
            
            // Try to find the desired link on the Academic Dashboard submenu
            if (!TryClickLink(browser, linkText, Make_It.Wait_1_Second))
            {
                // Try to click the Academic Dashboard
                if (!TryClickLink(browser, "Academic Dashboard"))
                {
                    Thread.Sleep(2000);
                    ScenarioContext.Current.GetBrowser().SaveScreenshot(string.Format("Trying to find the '{0}' link", "Academic Dashboard"));
                    throw new Exception("Unable to locate the Academic Dashboard link.");
                }

                // Try to click the target submenu again
                if (!TryClickLink(browser, linkText))
                {
                    Thread.Sleep(2000);
                    ScenarioContext.Current.GetBrowser().SaveScreenshot(string.Format("Trying to find the '{0}' link", linkText));
                    throw new Exception("Unable to locate the " + linkText + " link.");
                }
            }
        }

        /// <summary>
        /// Gets the text of the current system message.
        /// </summary>
        /// <returns>The text of the currently displayed system message; otherwise <b>null</b>.</returns>
        public string GetSystemMessageText()
        {
            try
            {
                return Browser.FindCss(SystemMessageHeaderClassCss, Make_It.Wait_1_Second).Text;
            }
            catch (Exception)
            {
                return null;
            }
        }
        
        /// <summary>
        /// Refreshes the web page.
        /// </summary>
        public void RefreshPage()
        {
            this.Visit(true);
        }

        /// <summary>
        /// Attempts to click a link with the specified text.
        /// </summary>
        /// <param name="linkText">The text of the link to find and click.</param>
        /// <returns><b>true</b> if the link was found and clicked; otherwise <b>false</b>.</returns>
        private static bool TryClickLink(BrowserSession browser, string linkText, Options options = null)
        {
            if (options == null)
                options = Make_It.Wait_1_Second;

            try
            {
                var link = browser.FindLink(linkText, options);
                link.Click();
                
                return true;
            }
            catch (MissingHtmlException)
            {
                return false;
            }

            //if (!link.Exists(options))
            //    return false;

            //link.Click(options);
            //return true;

            //int retryCount = 0;

            //while (retryCount++ < 10)
            //{
            //    var targetLinks = browser.FindAllCss("a>span").ToList();

            //    SnapshotElementScope targetLink =
            //        (from l in targetLinks
            //         where l.Text.NormalizeSpacing() == linkText
            //         select l)
            //            .SingleOrDefault();

            //    if (targetLink != null)
            //    {
            //        targetLink.Click();
            //        return true;
            //    }

            //    Thread.Sleep(1000);
            //}

            //return false;
        }

        protected virtual void TrimStrings(ref string string1)
        {
            string1 = string1.Trim();
        }

        protected virtual void TrimStrings(ref string string1, ref string string2)
        {
            string1 = string1.Trim();
            string2 = string2.Trim();
        }

        protected virtual void TrimStrings(ref string string1, ref string string2, ref string string3)
        {
            string1 = string1.Trim();
            string2 = string2.Trim();
            string3 = string3.Trim();
        }

        protected virtual void TrimStrings(ref string string1, ref string string2, ref string string3, ref string string4)
        {
            string1 = string1.Trim();
            string2 = string2.Trim();
            string3 = string3.Trim();
            string4 = string4.Trim();
        }

        /// <summary>
        /// Searches for student on-page
        /// </summary>
        /// <param name="browser"></param>
        /// <param name="searchText">Text which is searched</param>
        /// <returns>Name of student selected</returns>

        public static void SearchStudents(BrowserSession browser, string searchText)
        {
            browser.FindCss(SearchBoxSelectorCss).FillInWith(searchText);
            browser.FindCss(SearchArrowCss).Click();
        }

    }
}