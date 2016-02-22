using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Xml.Serialization;
using System.Linq;
using Coypu.Drivers;
using EdFi.Dashboards.Infrastructure.Implementations;
using EdFi.Dashboards.Metric.Resources.Models;
using EdFi.Dashboards.Presentation.Core.Tests.Routing.Support;
using EdFi.Dashboards.Presentation.Core.UITests.Support.Configuration;
using EdFi.Dashboards.Presentation.Core.UITests.Support.SpecFlow;
using EdFi.Dashboards.Resources.Models.LocalEducationAgency;
using EdFi.Dashboards.Resources.Models.LocalEducationAgency.Information;
using Newtonsoft.Json;
using RestSharp;
using TechTalk.SpecFlow;

namespace EdFi.Dashboards.Presentation.Core.UITests.Support
{
    /// <summary>
    /// Provides context (user profiles, configuration values, etc.) for a test session.
    /// </summary>
    public class TestSessionContext
    {
        private static TestSessionContext _testSessionContext;

        /// <summary>
        /// Gets the current context for the executing test session.
        /// </summary>
        public static TestSessionContext Current
        {
            get
            {
                if (_testSessionContext == null)
                    _testSessionContext = new TestSessionContext();

                return _testSessionContext;
            }
        }

        #region UserProfiles

        /// <summary>
        /// Holds the value for the <see cref="UserProfiles"/> property.
        /// </summary>
        private IDictionary<string, UserProfile> _userProfiles;

        /// <summary>
        /// Gets the user profiles, keyed by profile name.
        /// </summary>
        public IDictionary<string, UserProfile> UserProfiles
        {
            get
            {
                if (_userProfiles == null)
                {
                    // Identify the file containing the user profiles
                    string localEducationAgency = Configuration.LocalEducationAgency;
                    string userProfilesFilename = "UserProfiles.xml";

                    // Deserialize the user profiles into a list
                    XmlSerializer serializer = new XmlSerializer(typeof(List<LocalEducationAgency>));
                    string content = File.ReadAllText(userProfilesFilename);
                    var localEducationAgencies = serializer.Deserialize(new StringReader(content)) as List<LocalEducationAgency>;

                    var userProfiles =
                        (from lea in localEducationAgencies
                         where lea.Code == localEducationAgency
                         select lea.UserProfiles)
                         .SingleOrDefault();

                    if (userProfiles == null)
                    {
                        throw new Exception(string.Format("Unable to find user profiles for Local Education Agency '{0}' in file '{1}'.", 
                            localEducationAgency, userProfilesFilename));
                    }

                    // Load the user profiles into a Dictionary, keyed by ProfileName
                    _userProfiles = new Dictionary<string, UserProfile>(StringComparer.OrdinalIgnoreCase);

                    foreach (var userProfile in userProfiles)
                        _userProfiles[userProfile.ProfileName] = userProfile;
                }

                return _userProfiles;
            }
        }

        #endregion

        #region Configuration

        /// <summary>
        /// Holds the value for the <see cref="Configuration"/> property.
        /// </summary>
        private TestConfiguration _configuration;

        /// <summary>
        /// Gets the configuration values for the test session.
        /// </summary>
        public TestConfiguration Configuration
        {
            get
            {
                if (_configuration == null)
                {
                    // Load real configuration file data
                    string configFileName = "UITesting.config";

                    var configSectionProvider = new ExternalConfigFileSectionProvider(configFileName);
                    var appSettings = configSectionProvider.GetSection("appSettings") as AppSettingsSection;

                    _configuration =
                        new TestConfiguration
                            {
                                ServerAddress = GetAppSetting(appSettings, "serverAddress"),
                                ApplicationPath = GetAppSetting(appSettings, "applicationPath"),
                                LocalEducationAgency = GetAppSetting(appSettings, "localEducationAgency"),
                                LocalEducationAgencyId = Convert.ToInt32(GetAppSetting(appSettings, "localEducationAgencyId")),
                                TimeoutSeconds = Convert.ToInt32(GetAppSetting(appSettings, "timeoutSeconds") ?? "20"),
                                ScreenshotImagePath = GetAppSetting(appSettings, "screenshotImagePath"),
                            };

                    // Set the browser type
                    string browserTypeText = appSettings.Settings["browserType"].Value;
                    _configuration.BrowserType = GetBrowserType(browserTypeText);
                }

                return _configuration;
            }
        }

        private static string GetAppSetting(AppSettingsSection appSettings, string name)
        {
            var element = appSettings.Settings[name];

            if (element == null)
                return null;
                
            return element.Value;
        }

        private static Browser GetBrowserType(string browserType)
        {
            FieldInfo browserTypeField = typeof(Browser).GetField(browserType, BindingFlags.Public | BindingFlags.Static);

            if (browserTypeField != null)
                return browserTypeField.GetValue(null) as Browser;

            // Default to Firefox (it's the most well-behaved)
            return Browser.Firefox;
        }

        #endregion

        private static MetricMetadataTree _metadataTree;

        public static MetricMetadataTree MetadataTree
        {
            get
            {
                if (_metadataTree == null)
                {
                    string localEducationAgency = TestSessionContext.Current.Configuration.LocalEducationAgency;
                    string leaInfoUrl = Website.LocalEducationAgency.Information(int.MaxValue, new { localEducationAgency });

                    var restClient = ScenarioContext.Current.GetRestClient();

                    var leaInfoRequest = new RestRequest(leaInfoUrl.ToRelativeDashboardPath());
                    var informationModel = restClient.Execute<InformationModel>(leaInfoRequest).Data;

                    var metadataRequest = new RestRequest(Website.LocalEducationAgency.ApiResource(informationModel.LocalEducationAgencyId, null, "MetricMetadata"));
                    _metadataTree = restClient.Execute<MetricMetadataTree>(metadataRequest).Data;
                }

                return _metadataTree;
            }
        }

        private static IEnumerable<SchoolCategoryModel> _schools;

        public static IEnumerable<SchoolCategoryModel> Schools
        {
            get
            {
                if (_schools == null)
                {
                    string localEducationAgency = TestSessionContext.Current.Configuration.LocalEducationAgency;
                    string schoolListUrl = Website.LocalEducationAgency.SchoolCategoryList(int.MaxValue, new {localEducationAgency});

                    var request = new RestRequest(schoolListUrl.ToRelativeDashboardPath());
                    var client = ScenarioContext.Current.GetRestClient();
                    var response = client.Execute(request);

                    try
                    {
                        _schools = JsonConvert.DeserializeObject<IEnumerable<SchoolCategoryModel>>(response.Content);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(string.Format("Unable to deserialize web service response as JSON: {0}", response.Content), ex);
                    }
                }

                return _schools;
            }
        }
    }
}
