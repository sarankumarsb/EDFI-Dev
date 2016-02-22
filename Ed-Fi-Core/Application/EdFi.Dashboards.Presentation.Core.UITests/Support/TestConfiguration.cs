using Coypu.Drivers;

namespace EdFi.Dashboards.Presentation.Core.UITests.Support
{
    public class TestConfiguration
    {
        private string _serverAddress = "serverAddressMissingInAppConfig";

        /// <summary>
        /// Gets the address of the server (i.e. "localhost", an IP address, or a fully qualified domain name).
        /// </summary>
        public string ServerAddress
        {
            get { return _serverAddress; }
            set { _serverAddress = value; }
        }

        private string _applicationPath = "applicationPathMissingInAppConfig";

        /// <summary>
        /// Gets the path of the application on the server (i.e. an empty string for the root site, or a virtual directory such as "/EdFiDashboardDev/").
        /// </summary>
        public string ApplicationPath
        {
            get { return _applicationPath; }
            set 
            {
                // Ensure that application name is not wrapped with forward slashes
                _applicationPath = value == null ? string.Empty : value.Trim('/');
            }
        }

        /// <summary>
        /// Gets the local education agency code for use in identifying the user profiles to use for the test (i.e. "LubbockISD").
        /// </summary>
        public string LocalEducationAgency { get; set; }

        /// <summary>
        /// Gets the local education agency identifier.
        /// </summary>
        public int LocalEducationAgencyId { get; set; }

        /// <summary>
        /// Gets or sets the browser type to use for the testing.
        /// </summary>
        public Browser BrowserType { get; set; }

        /// <summary>
        /// Gets or sets the browser timeout period in seconds.
        /// </summary>
        public int TimeoutSeconds { get; set; }

        /// <summary>
        /// Gets the fully constructed base URL for the website to be tested (return value contains trailing forward slash).
        /// </summary>
        public string BaseUrl
        {
            get
            {
                string baseUrl = string.Format("https://{0}/{1}", ServerAddress, ApplicationPath);

                // Make sure returned value ends with a trailing forwad slash
                if (!baseUrl.EndsWith("/"))
                    return baseUrl + "/";

                return baseUrl;
            }
        }

        public string ScreenshotImagePath { get; set; }
    }
}