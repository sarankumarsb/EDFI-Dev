using System;
using Coypu;
using EdFi.Dashboards.Presentation.Core.UITests.Support.Coypu;
using EdFi.Dashboards.Presentation.Core.UITests.Support.RestSharp;
using RestSharp;
using TechTalk.SpecFlow;

namespace EdFi.Dashboards.Presentation.Core.UITests.Support.SpecFlow
{
    /// <summary>
    /// Contains methods that provide access to test information in the context of the current SpecFlow <see cref="ScenarioContext"/>.
    /// </summary>
    public static class ScenarioContextExtensions
    {
        private const string UserProfileNameKey = "userProfileName";
        private const string SchoolTypeKey = "schoolType";

        /// <summary>
        /// Gets the user profile associated with the current scenario context.
        /// </summary>
        /// <param name="context">The <see cref="ScenarioContext"/> of the current test scenario.</param>
        /// <param name="safe">Indicates that if no matching user profile is found, should return a "null" object (not a null reference).</param>
        /// <returns>The <see cref="UserProfile"/> associated with the scenario; otherwise <b>null</b> (or a "null" object).</returns>
        public static UserProfile GetUserProfile(this ScenarioContext context, bool safe = false)
        {
            string userProfileName = context.GetUserProfileName();

            if (userProfileName == null)
                return safe ? UserProfile.Null : null;

            UserProfile userProfile;

            // If no user profile found, return null (or Null object, if requested)
            if (!TestSessionContext.Current.UserProfiles.TryGetValue(userProfileName, out userProfile))
                return safe ? UserProfile.Null : null;

            return userProfile;
        }

        /// <summary>
        /// Gets the <see cref="RestClient"/> instance associated with the current scenario context.
        /// </summary>
        /// <param name="context">The <see cref="ScenarioContext"/> of the current test scenario.</param>
        /// <returns>The <see cref="RestClient"/> associated with the scenario; otherwise <b>null</b>.</returns>
        /// <exception cref="InvalidOperationException">Occurs if there is no user profile associated with the current <see cref="ScenarioContext"/>.</exception>
        public static RestClient GetRestClient(this ScenarioContext context)
        {
            string userProfileName = context.GetUserProfileName();

            if (userProfileName == null)
                throw new InvalidOperationException("There is no user profile associated with the current scenario, and thus no RestClient can be initialized.");

            return RestClientContext.For(userProfileName);
        }

        /// <summary>
        /// Gets the <see cref="BrowserSession"/> instance associated with the current scenario context.
        /// </summary>
        /// <param name="context">The <see cref="ScenarioContext"/> of the current test scenario.</param>
        /// <returns>The <see cref="BrowserSession"/> associated with the scenario; otherwise <b>null</b>.</returns>
        public static BrowserSession GetBrowser(this ScenarioContext context)
        {
            string userProfileName = context.GetUserProfileName();

            return BrowserContext.For(userProfileName);
        }

        /// <summary>
        /// Gets the name of the user profile associated with the current scenario.
        /// </summary>
        /// <param name="context">The <see cref="ScenarioContext"/> of the current test scenario.</param>
        /// <returns>The name of the user profile associated with the current scenario (if assigned); otherwise <b>null</b>.</returns>
        public static string GetUserProfileName(this ScenarioContext context)
        {
            string userProfileName;
            context.TryGetValue(UserProfileNameKey, out userProfileName);
            return userProfileName;
        }

        /// <summary>
        /// Sets the user profile associated with the current scenario.
        /// </summary>
        /// <param name="context">The <see cref="ScenarioContext"/> of the current test scenario.</param>
        /// <param name="userProfileName">The name of the user profile under which the test should be executed.</param>
        public static void SetUserProfileName(this ScenarioContext context, string userProfileName)
        {
            context.Set(userProfileName, UserProfileNameKey);
        }

        /// <summary>
        /// Gets the school type associated with the current scenario.
        /// </summary>
        /// <param name="context">The <see cref="ScenarioContext"/> of the current test scenario.</param>
        /// <returns>The type of school associated with the current scenario (if assigned); otherwise <b>null</b>.</returns>
        public static SchoolType GetSchoolType(this ScenarioContext context)
        {
            SchoolType schoolType;
            context.TryGetValue(SchoolTypeKey, out schoolType);
            return schoolType;
        }

        /// <summary>
        /// Sets the school type associated with the current scenario.
        /// </summary>
        /// <param name="context">The <see cref="ScenarioContext"/> of the current test scenario.</param>
        /// <param name="schoolType">The type of school that is in context for the current scenario.</param>
        public static void SetSchoolType(this ScenarioContext context, SchoolType schoolType)
        {
            context.Set(schoolType, SchoolTypeKey);
        }
    }
}
