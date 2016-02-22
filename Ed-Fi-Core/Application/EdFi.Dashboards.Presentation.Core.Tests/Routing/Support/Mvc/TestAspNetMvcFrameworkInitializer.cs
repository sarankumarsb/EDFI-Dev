using System;
using System.Linq;
using System.Reflection;
using System.Web.Compilation;
using System.Web.Mvc;
using System.Web.Routing;
using EdFi.Dashboards.Presentation.Core.Utilities.Mvc;

namespace EdFi.Dashboards.Presentation.Core.Tests.Routing.Support.Mvc
{
    public abstract class TestAspNetMvcFrameworkInitializer : DefaultAspNetMvcFrameworkInitializer
    {
        private readonly Type[] markerInterfacesForWebAssemblies;
        private readonly string baseUrl;

        public TestEdFiControllerTypeCache ControllerTypeCache { get; set; }

        public TestAspNetMvcFrameworkInitializer(Type[] markerInterfacesForWebAssemblies, string baseUrl)
        {
            this.markerInterfacesForWebAssemblies = markerInterfacesForWebAssemblies;
            this.baseUrl = baseUrl;
        }

        public override void Initialize(Castle.Windsor.IWindsorContainer container, Assembly webApplicationAssembly)
        {
            // Do some really ugly things with reflection to get MVC to behave outside of ASP.NET infrastructure
            Trick_AspNet_Mvc_Into_Thinking_It_Has_Been_Initialized();

            // Perform all default MVC initialization steps
            base.Initialize(container, webApplicationAssembly);

            // Initialize components for route generation
            InitializeRouteGenerationForTesting(baseUrl);
        }

        protected override void RegisterVirtualPathProviders()
        {
            // Not needed... don't do anything
        }

        protected override void RegisterAreas()
        {
            FindAndRegisterAllAreas();
        }

        protected override void InitializeDependencyResolver(Castle.Windsor.IWindsorContainer container)
        {
            // Not needed... don't do anything.
        }

        protected override void InitializeControllerBuilder()
        {
            // Let base class initialize everything first (so we don't duplicate the namespaces logic in here)
            base.InitializeControllerBuilder();

            // Build our own controller type cache (removed from ASP.NET horrid internal dependencies)
            ControllerTypeCache = GetControllerTypeCache(markerInterfacesForWebAssemblies);

            // Initialize MVC controller factory, as per main project's Global.asax.cs
            ControllerBuilder.Current.SetControllerFactory(new TestEdFiControllerFactory(ControllerTypeCache));
        }

        protected override void InitializeViewEngines(Assembly webApplicationAssembly)
        {
            // Not needed... don't do anything.
        } 

        protected override void InitializeValueProviderFactories()
        {
            ValueProviderFactories.Factories.Clear();
            ValueProviderFactories.Factories.Insert(0, new TestIdValueProviderFactory());
        }

        protected override void RegisterFilters()
        {
            // Not needed... don't do anything.
        }

        #region Support - Area Registration

        private void FindAndRegisterAllAreas()
        {
            foreach (Type markerInterface in markerInterfacesForWebAssemblies)
            {
                var allRegistrations =
                    from t in markerInterface.Assembly.GetTypes()
                    where IsAreaRegistration(t)
                    select t;

                foreach (var r in allRegistrations)
                {
                    var areaRegistration = Activator.CreateInstance(r) as AreaRegistration;
                    RegisterRoutesForArea(areaRegistration);
                }
            }
        }

        private static bool IsAreaRegistration(Type t)
        {
            Type areaRegistrationType = typeof(AreaRegistration);
            return areaRegistrationType.IsAssignableFrom(t);
        }

        private void RegisterRoutesForArea(AreaRegistration areaRegistration)
        {
            var context = new AreaRegistrationContext(areaRegistration.AreaName, RouteTable.Routes, null);

            string areaNamespace = areaRegistration.GetType().Namespace;

            if (areaNamespace != null)
                context.Namespaces.Add(areaNamespace + ".*");

            areaRegistration.RegisterArea(context);
        }

        #endregion

        private TestEdFiControllerTypeCache GetControllerTypeCache(Type[] markerInterfaces)
        {
            var assemblies = markerInterfaces.Select(t => t.Assembly).ToArray();
            var controllerTypeCache = new TestEdFiControllerTypeCache(assemblies);
            return controllerTypeCache;
        }

        private static void Trick_AspNet_Mvc_Into_Thinking_It_Has_Been_Initialized()
        {
            // Do not read this method - Move along.  Nothing Happening Here.
            var preStartInitStageProperty = typeof(BuildManager)
                .GetProperty("PreStartInitStage", BindingFlags.NonPublic | BindingFlags.Static);
            preStartInitStageProperty.SetValue(null, 2, null);

            var theBuildManagerField = typeof(BuildManager)
                .GetField("_theBuildManager", BindingFlags.NonPublic | BindingFlags.Static);
            var theBuildManager = theBuildManagerField.GetValue(null);

            var topLevelFilesCompiledStartedField = typeof(BuildManager)
                .GetField("_topLevelFilesCompiledStarted", BindingFlags.NonPublic | BindingFlags.Instance);
            topLevelFilesCompiledStartedField.SetValue(theBuildManager, true);
        }

        protected abstract void InitializeRouteGenerationForTesting(string baseUrl);
    }
}
