#region Namespaces Galore

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using Castle.MicroKernel.Registration;
using EdFi.Dashboards.Common;
using EdFi.Dashboards.Common.Utilities;
using EdFi.Dashboards.Infrastructure.CastleWindsorInstallers;
using EdFi.Dashboards.Infrastructure.Implementations;
using EdFi.Dashboards.Presentation.Core.Areas.LocalEducationAgency.Controllers;
using EdFi.Dashboards.Presentation.Core.Areas.LocalEducationAgency.Controllers.Detail;
using EdFi.Dashboards.Presentation.Core.Areas.School.Controllers;
using EdFi.Dashboards.Presentation.Core.Areas.School.Controllers.Detail;
using EdFi.Dashboards.Presentation.Core.Areas.Search.Controllers;
using EdFi.Dashboards.Presentation.Core.Areas.Staff.Controllers;
using EdFi.Dashboards.Presentation.Core.Areas.StudentSchool.Controllers;
using EdFi.Dashboards.Presentation.Core.Areas.StudentSchool.Controllers.Detail;
using EdFi.Dashboards.Presentation.Core.Controllers;
using EdFi.Dashboards.Presentation.Core.Tests.Routing.Support;
using EdFi.Dashboards.Presentation.Core.Tests.Routing.Support.Mvc;
using EdFi.Dashboards.Presentation.Web;
using EdFi.Dashboards.Resources.Navigation;
using EdFi.Dashboards.Testing;
using NUnit.Framework;
using System.Web.Routing;
using HistoricalChartServicesController = EdFi.Dashboards.Presentation.Core.Areas.StudentSchool.Controllers.Detail.HistoricalChartServicesController;
using MetricMetadataController = EdFi.Dashboards.Presentation.Core.Areas.LocalEducationAgency.Controllers.MetricMetadataController;
using SessionStudentListController = EdFi.Dashboards.Presentation.Core.Areas.LocalEducationAgency.Controllers.SessionStudentListController;
using StudentSchoolCategoryListController = EdFi.Dashboards.Presentation.Core.Areas.LocalEducationAgency.Controllers.StudentSchoolCategoryListController;

#endregion

// -------------------------------------------------------------------------------
// NOTE: Please move all "using" statements for namespaces into the region above.
// -------------------------------------------------------------------------------
namespace EdFi.Dashboards.Presentation.Core.Tests.Routing
{
    /// <summary>
    /// Provides a base partial class for implementing route mapping tests.
    /// </summary>
    /// <remarks>The class is abstract to prevent any of the route fixtures from executing
    /// in the context of the <b>EdFi.Dashboards.Presentation.Core.Tests</b> assembly
    /// because this wouldn't give the overrides that are defined in the derived concrete
    /// test fixture class a chance to prevent a base test method from executing.
    /// 
    /// The class is partial to allow additional route tests for other areas to be added 
    /// to the same execution scope in order to perform coverage analysis to identify 
    /// which routes and controllers are not covered by the tests.
    /// </remarks>
    public abstract partial class When_resolving_routes_to_controllers : TestFixtureBase
    {
        protected HashSet<RouteBase> UntestedRoutes;
        private HashSet<Type> UntestedControllers;
        protected List<string> IgnoredRoutes;
        protected List<Type> IgnoredControllers;

        const string protocol = "https";
        private string serverAddress = "somehost";    // Overridden by app.config settings
        private string applicationPath = "someapp";   // Overridden by app.config settings

        private string baseUrl = "https://somehost/someapp/";

        protected void Ignore<T>() where T : Controller
        {
            if (IgnoredControllers.Contains(typeof(T)))
                return;

            IgnoredControllers.Add(typeof(T));
        }

        protected virtual List<Type> GetIgnoredControllers()
        {
            return new List<Type>
                       {
                           //Layout Controllers have no route:
                           typeof(Areas.Admin.Controllers.AdminLayoutController),
                           typeof(SearchLayoutController),
                           typeof(StaffLayoutController),
                           typeof(StudentSchoolLayoutController),
                           typeof(SchoolLayoutController),
                           typeof(LocalEducationAgencyLayoutController),
                           //--------------------------------
                           typeof(PreviousNextController), // This is a core controller, not invoked via a route
                           typeof(MasterPageController),   // This is a core controller, not invoked via a route
                           typeof(BenchmarkHistoricalChartServiceController), // Called directly from StudentSchool\Views\Detail\BenchmarkHistoricalChart\Get.cshtml, no route
                           typeof(LearningObjectivesTableController),         // Called directly from StudentSchool\Views\Detail\LearningObjectivesTabe\Get.cshtml, no route
                           typeof(Areas.Admin.Controllers.TitleClaimSetController),                   // Called directly from Get.cshtml
                           typeof(SchoolPriorYearMetricExportController),     // Called directly from Get.cshtml
                           typeof(StudentPriorYearMetricExportController),    // Called directly from Get.cshtml
                           typeof(LearningObjectivesExportController),        // Called directly from Get.cshtml
                           typeof(LearningStandardsExportController),         // Called directly from Get.cshtml
                           typeof(StudentSchoolCategoryListController),       // Completely Unused. No search results, no usages found.
                           typeof(StudentGradeListController),                // Completely Unused. No search results, no usages found.
                           typeof(KeepAliveController),                       // Completely Unused. No search restuls, no usages found.
                           typeof(StudentPriorYearMetricTableController),     // Metric Table Controller has no route. Called in School\Views\Detail\StudentPriorYearMetricTable\Get.cshtml ? 
                           typeof(StaffPriorYearMetricTableController),       // Metric Table Controller has no route. Called in School\Views\Detail\StaffPriorYearMetricTable\Get.cshtml ?
                           typeof(SchoolPriorYearMetricTableController),      // Metric Table Controller has no route. Called in LocalEducationAgency\Views\Detail\SchoolPriorYearMetricTable\Get.cshtml ?
                           typeof(Areas.School.Controllers.Detail.HistoricalChartServicesController),         // Called directly from School\Views\Detail\HistoricalChart\Get.cshtml
                           typeof(Areas.StudentSchool.Controllers.Detail.HistoricalChartServicesController),  // Called directly from StudentSchool\Views\Detail\HistoricalChart\Get.cshtml
                           typeof(CourseHistoryListController), // Completely Unused. No search results, no usages found.
                           typeof(Areas.Admin.Controllers.PhotoManagementController), // Completely Unused. No search results or usages.
                           typeof(SessionStudentListController), // Either unused or called directly from Views\Shared\CustomStudentList.cshtml
                           typeof(GoalPlanningSchoolMetricTableController), // Called for in a JS, and in one Get.generated.cs file. Stands to reason that it has no route, as it is a metric.
                           typeof(MetricMetadataController), //Metric metadata. Called from various files, no route.
                           typeof(EdFiGridBaseController), // No route will ever reach the base controller directly
                           typeof(EdFiGridExportController), // Called directly from Get.cshtml; form posts data that is used for export
                           typeof(MetricsBasedWatchListUnshareController), // TODO: Write a test for this later (Brandon)
                           typeof(MetricsBasedWatchListDescriptionController), // TODO: Write a test for this later (Brandon)
						   typeof(LearningStandardsListController), //TODO: Write a test for this later (Neil)
#if DEBUG
                           typeof(CacheController), // This has no route
#endif

                       };
        }

        protected virtual List<string> GetIgnoredRoutes()
        {
            return new List<string>
                       {
                           "Application/{controller}/{action}/{id}", // No controllers exist to match in this area, why does it exist?
                           "EdOrgNetwork/{controller}/{action}/{id}",
                           "LocalEducationAgency/{controller}/{action}/{id}",
                           "School/{controller}/{action}/{id}",
                           "Search/{controller}/{action}/{id}",
                           "Staff/{controller}/{action}/{id}",
                           "StudentSchool/{controller}/{action}/{id}", //All of these are stock routes.
						   "Districts/{localEducationAgency}/SchoolCategories/{controller}/{schoolCategory}", //TODO: Write a test for this later LocalEducationAgency_StudentSchoolCategory
						   "Districts/{localEducationAgency}/Demographics/{controller}/{demographic}/{studentListType}/{sectionOrCohortId}", //TODO: Write a test for this later LocalEducationAgency_WatchListDemographic
							"Districts/{localEducationAgency}/SchoolCategories/{controller}/{schoolCategory}/{studentListType}/{sectionOrCohortId}", //TODO: Write a test for this later LocalEducationAgency_WatchListSchoolCategory
							"Districts/{localEducationAgency}/Schools/{school}/Demographics/{controller}/{demographic}/{studentListType}/{sectionOrCohortId}", //TODO: Write a test for this later School_WatchListDemographics
							"Districts/{localEducationAgency}/Schools/{school}/Grades/{controller}/{grade}/{studentListType}/{sectionOrCohortId}", //TODO: Write a test for this later School_WatchListGrades

#if !DEBUG
                           "api/v1/Districts/{localEducationAgency}/{controller}/{resourceIdentifier}", //Only needed during release
                           "api/v1/Networks/{edOrgNetwork}/{controller}/{resourceIdentifier}", // see the LEA version
#endif
                       };
        }

        #region Support methods

        private void InitializeConfigValues()
        {
            var configSectionProvider = new ExternalConfigFileSectionProvider("app.config");
            var sectionObj = configSectionProvider.GetSection("appSettings");
            var section = sectionObj as AppSettingsSection;

            serverAddress = section.Settings["serverAddress"] == null ? "serverAddressMissingInAppConfig" : section.Settings["serverAddress"].Value;
            applicationPath = section.Settings["applicationPath"] == null ? "applicationPathMissingInAppConfig" : section.Settings["applicationPath"].Value;

            baseUrl = string.Format("{0}://{1}{2}", "https", serverAddress, WrapWithForwardSlashes(applicationPath));
        }

        private static string WrapWithForwardSlashes(string applicationPath)
        {
            if (string.IsNullOrWhiteSpace(applicationPath))
                return "/";

            // Tolerate incorrect app path format (should already contain leading and trailing /'s)
            return "/" + applicationPath.TrimStart('/').TrimEnd('/') + "/";
        }

        private static void InitializeInversionOfControl()
        {
            // Initialize the IoC container using the main Web.config file
            var containerFactory = new InversionOfControlContainerFactory(
                new ExternalConfigFileSectionProvider("web.config"),    // Use main web.config
                new NameValueCollectionConfigValueProvider());          // Use "empty" appSettings

            // Initialize the service locator with the container 
            // (enabling installers to access container through IoC during registration process)
            var container = containerFactory.CreateContainer(IoC.Initialize);

            container.Register(
                Component.For<IServiceLocator>()
                    .Instance(IoC.WrappedServiceLocator));

        }

        #endregion

        #region Before/After Processing

        // Initialize callbacks to enable tracking of what controllers/routes have or have not been tested
        public override void RunOnceBeforeAny()
        {
            base.RunOnceBeforeAny();

            // Create delegate to remove routes from UntestedRoutes hash set as they are tested
            RouteTestingExtensions.RouteMatchedCallback += 
                routeData => UntestedRoutes.Remove(routeData.Route);

            RouteTestingExtensions.ControllerTypeTestedCallback +=
                type => UntestedControllers.Remove(type);
        }

        // Check to see if any routes/controllers have not been tested
        public override void RunOnceAfterAll()
        {
            base.RunOnceAfterAll();

            string coverageMessage = null;

            // Remove ignored routes
            if (UntestedRoutes.Count > 0)
                UntestedRoutes.RemoveWhere(r => IgnoredRoutes.Contains(((Route)r).Url));

            // Verify all routes were tested
            if (UntestedRoutes.Count > 0)
            {
                int maxNameLength = UntestedRoutes.Max(
                    r =>
                        {
                            var areaName = ((Route) r).DataTokens["area"] as string;
                            return (areaName ?? "").Length;
                        });

                string untestedRoutes = string.Join("\r\n", UntestedRoutes.Select(r =>
                    string.Format("Area: {0}  Url: {1}",
                      ((((Route)r).DataTokens["area"] as string) + new string(' ', maxNameLength)).Substring(0, maxNameLength),
                      ((Route)r).Url)));

                coverageMessage +=
                    string.Format("The following routes were not covered by any unit tests:\r\n-------------------------------------------------------------\r\n{0}\r\n",
                        untestedRoutes);
            }

            // Remove ignored controllers
            if (UntestedControllers.Count > 0)
                UntestedControllers.RemoveWhere(c => IgnoredControllers.Contains(c));

            // Verify all controllers were tested
            if (UntestedControllers.Count > 0)
            {
                int maxNameLength = UntestedControllers.Max(t => t.Name.Length);

                string untestedControllers = string.Join("\r\n", UntestedControllers.Select(t =>
                    string.Format("Name: {0}  Namespace: {1}",
                      (t.Name + new string(' ', maxNameLength)).Substring(0, maxNameLength),
                      t.Namespace)));

                // If there is already text, add a couple of CR/LFs to create vertical spacing between sections.
                if (coverageMessage != null)
                    coverageMessage += "\r\n";

                coverageMessage +=
                    string.Format("The following controllers were not covered by any unit tests:\r\n-------------------------------------------------------------\r\n{0}\r\n",
                        untestedControllers);
            }

            if (coverageMessage != null)
            {
                throw new AssertionException(coverageMessage); 
            }
        }
        #endregion

        protected override void EstablishContext()
        {
            // Identify routes and controllers to be excluded here
            IgnoredRoutes = GetIgnoredRoutes();
            IgnoredControllers = GetIgnoredControllers();

            InitializeConfigValues();
            InitializeInversionOfControl();

            // Identify assemblies to search for MVC web artifacts
            // TODO: Should this be done with a conventions class?
            var markerInterfacesForWebAssemblies =
                new[]
                    {
                        typeof(Marker_EdFi_Dashboards_Presentation_Web),
                        typeof(Marker_EdFi_Dashboards_Presentation_Core)
                    };

            var webApplicationAssembly = typeof(Marker_EdFi_Dashboards_Presentation_Web).Assembly;

            // Initialize MVC
            var initializer = EnsureMvcFrameworkInitialized(markerInterfacesForWebAssemblies, webApplicationAssembly);

            // Make note of routes and controllers to be tested
            UntestedRoutes = new HashSet<RouteBase>(RouteTable.Routes.Where(r => r.GetType() == typeof(Route))); // Exclude things like IgnoreRouteInternal
            UntestedControllers = new HashSet<Type>(initializer.ControllerTypeCache.ControllerTypes);

            // Initialize link generators
            var routeValuesPreparer = new RouteValuesPreparer(new TestRouteValueProvider().ToArray());
            var httpRequestProviderFake = new HttpRequestProviderFake(protocol, serverAddress, applicationPath);
            InitializeLinkGenerators(routeValuesPreparer, httpRequestProviderFake);
        }

        private static RouteTestAspNetMvcFrameworkInitializer mvcFrameworkInitializer;

        private RouteTestAspNetMvcFrameworkInitializer EnsureMvcFrameworkInitialized(Type[] markerInterfacesForWebAssemblies,
                                                                                     Assembly webApplicationAssembly)
        {
            // If we've already initialized MVC in this execution scope, quit now
            if (mvcFrameworkInitializer != null)
                return mvcFrameworkInitializer;

            // Intialize the MVC framework
            mvcFrameworkInitializer = new RouteTestAspNetMvcFrameworkInitializer(markerInterfacesForWebAssemblies, baseUrl);
            mvcFrameworkInitializer.Initialize(IoC.Container, webApplicationAssembly);
            
            return mvcFrameworkInitializer;
        }

        protected void InitializeLinkGenerators(RouteValuesPreparer routeValuesPreparer, HttpRequestProviderFake httpRequestProviderFake)
        {
            var protectedMethods = this.GetType().GetMethods(BindingFlags.Instance | BindingFlags.NonPublic);

            var linkGeneratorInitializers =
                from m in protectedMethods
                where Regex.IsMatch(m.Name, "^Initialize(.*)LinkGenerator$")
                select m;

            foreach (var initializer in linkGeneratorInitializers)
            {
                initializer.Invoke(this, new object[] {routeValuesPreparer, httpRequestProviderFake});
            }
        }

        // This fixture performs all the "Acts" (i.e. Arrange/Act/Assert) in the individual test methods rather than in ExecuteTest
        protected override void ExecuteTest() { }
    }
}
