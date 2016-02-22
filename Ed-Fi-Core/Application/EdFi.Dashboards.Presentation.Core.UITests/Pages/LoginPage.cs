using EdFi.Dashboards.Presentation.Core.Tests.Routing.Support;
using EdFi.Dashboards.Presentation.Core.UITests.Attributes;
using EdFi.Dashboards.Presentation.Core.UITests.Support;
using OpenQA.Selenium;

namespace EdFi.Dashboards.Presentation.Core.UITests.Pages
{
    /// <summary>
    /// Provides programmatic control of the login page.
    /// </summary>
    [AssociatedUrl(@"Login\.aspx")]
    public class LoginPage : PageBase
    {
        /// <summary>
        /// Gets the text of the label associated with the username input control.
        /// </summary>
        protected virtual string UsernameLabelText
        {
            get { return "Username"; }
        }

        /// <summary>
        /// Gets the text of the label associated with the password input control.
        /// </summary>
        protected virtual string PasswordLabelText
        {
            get { return "Password"; }
        }

        /// <summary>
        /// Gets the text, value, name or id of the "Login" button.
        /// </summary>
        protected virtual string LoginButtonSelector
        {
            get { return ".btn"; }
        }

        /// <summary>
        /// Navigates to the login page, logging out first if necessary.
        /// </summary>
        public override void Visit(bool forceNavigation = false)
        {
            // Are we on the login page?  If not, logout, then proceed to the login page
            if (!IsCurrent() || forceNavigation)
                Browser.Visit(Website.General.Logout());

            string localEducationAgency = TestSessionContext.Current.Configuration.LocalEducationAgency;
            string entryUrl = Website.LocalEducationAgency.Entry(localEducationAgency);

            Browser.Visit(entryUrl); // Force redirect to STS for login
        }

        /// <summary>
        /// Sets the value assigned to the username input control.
        /// </summary>
        public virtual string Username
        {
            set
            {
                Browser.FillIn(UsernameLabelText).With(value);
            }
        }

        /// <summary>
        /// Sets the value assigned to the password input control.
        /// </summary>
        public virtual string Password
        {
            set
            {
                Browser.FillIn(PasswordLabelText).With(value);
            }
        }

        /// <summary>
        /// Generates a click on the login button.
        /// </summary>
        public virtual void ClickLoginButton()
        {
            var loginButton = Browser.FindCss(LoginButtonSelector);
            loginButton.Click();
        }

        /// <summary>
        /// Sets the username and password control values and then clicks the login button.
        /// </summary>
        /// <param name="username">The username to be entered.</param>
        /// <param name="password">The password to be entered.</param>
        public virtual void Login(string username, string password)
        {
            Visit();

            Username = username;
            Password = password;

            ClickLoginButton();
        }

        /// <summary>
        /// Gets the error message, if found, or an empty string.
        /// </summary>
        public virtual string ErrorMessage
        {
            get
            {
                var elementScope = Browser.FindCss("span.error");

                if (elementScope.Exists())
                    return elementScope.Text;

                return string.Empty;
            }
        }

        /// <summary>
        /// Simulates a user hitting the ENTER key to submit the form.
        /// </summary>
        public virtual void HitEnter()
        {
            Browser.FindField(PasswordLabelText).SendKeys(Keys.Return);
        }
    }
}
