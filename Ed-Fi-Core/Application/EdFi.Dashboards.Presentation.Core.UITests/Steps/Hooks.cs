using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BoDi;
using EdFi.Dashboards.Common;
using EdFi.Dashboards.Common.Utilities;
using EdFi.Dashboards.Infrastructure.CastleWindsorInstallers;
using EdFi.Dashboards.Infrastructure.Implementations;
using EdFi.Dashboards.Presentation.Core.UITests.Pages;
using EdFi.Dashboards.Presentation.Core.UITests.Pages.Error;
using EdFi.Dashboards.Presentation.Core.UITests.Support;
using EdFi.Dashboards.Presentation.Core.UITests.Support.Coypu;
using EdFi.Dashboards.Presentation.Core.UITests.Support.Mvc;
using EdFi.Dashboards.Presentation.Core.UITests.Support.SpecFlow;
using EdFi.Dashboards.Presentation.Web;
using TechTalk.SpecFlow;
using Component = Castle.MicroKernel.Registration.Component;

namespace EdFi.Dashboards.Presentation.Core.UITests.Steps
{
    [Binding]
    public class Hooks
    {
        
        
        // For additional details on SpecFlow hooks see http://go.specflow.org/doc-hooks

        private IObjectContainer container;

        public Hooks(IObjectContainer container)
        {
            this.container = container;
        }

        [BeforeTestRun]
        public static void BeforeTestRun()
        {
            EnsureMvcInitialized();
            InitializePageRegistrations(); // Do reflection work for pages once
        }

        [BeforeScenario]
        public void BeforeScenario()
        {
            // Register all page typs with the new container instance before each scenario
            RegisterPageTypes(); 
        }

        [AfterScenario]
        public void AfterScenario()
        {
            if (ScenarioContext.Current.TestError != null)
            {
                try
                {
                    var errorPage = container.Resolve<ErrorPage>();

                    if (errorPage.IsOnErrorPage())
                        errorPage.ShowErrorDetails();
                }
                finally 
                {
                    ScenarioContext.Current.GetBrowser().SaveScreenshot();
                }
            }
            
        }


        [AfterTestRun]
        public static void AfterTestRun()
        {
            BrowserContext.DisposeAllBrowsers();
        }

        #region Page Registration

        private static MethodInfo registrationMethod;

        private static Dictionary<Tuple<Type, Type>, MethodInfo> genericRegistrationMethods =
            new Dictionary<Tuple<Type, Type>, MethodInfo>();

        private static List<Type[]> pageTypeRegistrations;

        private static bool pageTypesInitialized = false;

        private static void InitializePageRegistrations()
        {
            if (pageTypesInitialized)
                return;

            var concretePageTypes =
                (from pt in typeof(Hooks).Assembly.GetTypes()
                 where !pt.IsAbstract
                       && typeof(PageBase).IsAssignableFrom(pt)
                 select new
                     {
                         PageType = pt,
                         HasAttributes =
                     (from a in pt.GetCustomAttributes(false)
                      where a.GetType().Name.StartsWith("Associated")
                      select a)
                     .Any(),
                     })
                    .ToList();

            var overridesByBaseType =
                (from possibleBaseType in concretePageTypes
                 from possibleDerivedType in concretePageTypes
                 where possibleBaseType.PageType.IsAssignableFrom(possibleDerivedType.PageType)
                       && !possibleDerivedType.HasAttributes
                 // Indicates page is a customization of behavior, rather than a page on its own
                 select new
                     {
                         BaseType = possibleBaseType,
                         DerivedType = possibleDerivedType
                     })
                    .ToDictionary(pair => pair.BaseType, pair => pair.DerivedType);

            pageTypeRegistrations = new List<Type[]>();

            // Add override pages first
            foreach (var kvp in overridesByBaseType)
                pageTypeRegistrations.Add(new[] {kvp.Value.PageType, kvp.Key.PageType});

            // Register all remaining page types
            foreach (var pageType in concretePageTypes.Where(t => !overridesByBaseType.ContainsKey(t)))
                pageTypeRegistrations.Add(new[] {pageType.PageType, pageType.PageType});

            registrationMethod = typeof(IObjectContainer).GetMethod("RegisterTypeAs", new[] {typeof(string)});

            pageTypesInitialized = true;
        }

        private void RegisterPageTypes()
        {
            foreach (var types in pageTypeRegistrations)
            {
                var registrationKey = new Tuple<Type, Type>(types[0], types[1]);

                MethodInfo genericRegistrationMethod;

                if (!genericRegistrationMethods.TryGetValue(registrationKey, out genericRegistrationMethod))
                {
                    genericRegistrationMethod = registrationMethod.MakeGenericMethod(types[0], types[1]);
                    genericRegistrationMethods[registrationKey] = genericRegistrationMethod;
                }

                // Register the page
                genericRegistrationMethod.Invoke(container, new[] {Type.Missing});
            }
        }

        #endregion

        private static bool mvcInitialized;

        private static void EnsureMvcInitialized()
        {
            if (mvcInitialized)
                return;

            InitializeInversionOfControl();

            // TODO: Should this be done with a conventions class?
            // Identify assemblies to search for MVC web artifacts
            var markerInterfacesForWebAssemblies =
                new[]
                    {
                        typeof(Marker_EdFi_Dashboards_Presentation_Web),
                        typeof(Marker_EdFi_Dashboards_Presentation_Core)
                    };

            var webApplicationAssembly = typeof(Marker_EdFi_Dashboards_Presentation_Web).Assembly;

            var testConfiguration = TestSessionContext.Current.Configuration;

            var initializer = new UITestAspNetMvcFrameworkInitializer(markerInterfacesForWebAssemblies, testConfiguration.BaseUrl);
            initializer.Initialize(IoC.Container, webApplicationAssembly);

            mvcInitialized = true;
        }

        private static void InitializeInversionOfControl()
        {
            // Initialize the IoC container using the main Web.config file
            var containerFactory = new InversionOfControlContainerFactory(
                new ExternalConfigFileSectionProvider("web.config"),    // Use main web.config
                new NameValueCollectionConfigValueProvider());          // Use "empty" appSettings

            var container = containerFactory.CreateContainer(
                // Initialize the service locator with the container 
                // (enabling installers to access container through IoC during registration process)
                IoC.Initialize);

            // Register the wrapped service locator singleton instance
            container.Register(
                Component.For<IServiceLocator>()
                    .Instance(IoC.WrappedServiceLocator));
        }
    }
}
