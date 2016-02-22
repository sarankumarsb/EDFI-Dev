using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Coypu;
using Coypu.Drivers;
using OpenQA.Selenium.PhantomJS;

namespace EdFi.Dashboards.Presentation.Core.UITests.Support.Coypu
{
    public static class BrowserContext
    {
        private static IDictionary<string, BrowserSession> _browsersByUserProfile
            = new Dictionary<string, BrowserSession>(StringComparer.OrdinalIgnoreCase);

        public static BrowserSession For(string userProfileName)
        {
            BrowserSession browser;

            if (string.IsNullOrWhiteSpace(userProfileName))
                userProfileName = "unspecified";

            if (_browsersByUserProfile.TryGetValue(userProfileName, out browser))
                return browser;

            var newBrowser = CreateNewBrowserSession();
            _browsersByUserProfile[userProfileName] = newBrowser;

            //Console.WriteLine("Browser for " + userProfileName + ": " + newBrowser.GetHashCode());

            return newBrowser;
        }

        private static BrowserSession CreateNewBrowserSession()
        {
            BrowserSession browser;

            var configuration =
                new SessionConfiguration
                    {
                        Timeout = TimeSpan.FromSeconds(TestSessionContext.Current.Configuration.TimeoutSeconds),
                        Browser = TestSessionContext.Current.Configuration.BrowserType,
                    };

            browser = new BrowserSession(configuration);
            return browser;
        }

        public static void DisposeAllBrowsers()
        {
            var browserInfos =
                (from kvp in _browsersByUserProfile
                 select new {UserProfileName = kvp.Key, Browser = kvp.Value})
                    .ToList();

            foreach (var browserInfo in browserInfos)
            {
                browserInfo.Browser.Dispose();
                _browsersByUserProfile.Remove(browserInfo.UserProfileName);
            }
        }
    }
}
