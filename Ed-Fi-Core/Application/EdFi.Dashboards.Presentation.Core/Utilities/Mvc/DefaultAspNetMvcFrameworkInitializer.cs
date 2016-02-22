using System.Reflection;
using System.ServiceModel.Security;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.WebPages;
using Castle.Windsor;
using EdFi.Dashboards.Presentation.Architecture.Mvc.ActionFilters;
using EdFi.Dashboards.Presentation.Architecture.Mvc.AreaRegistration;
using EdFi.Dashboards.Presentation.Architecture.Mvc.Core;
#if DEBUG
using EdFi.Dashboards.Presentation.Architecture.Mvc.Core.Development;
#endif
using EdFi.Dashboards.Presentation.Architecture.Mvc.ValueProviders;
using EdFi.Dashboards.Presentation.Core.Controllers;
using EdFi.Dashboards.Presentation.Web.Utilities;

namespace EdFi.Dashboards.Presentation.Core.Utilities.Mvc
{
    /// <summary>
    /// Performs all initialization of the ASP.NET MVC framework.
    /// </summary>
    public interface IAspNetMvcFrameworkInitializer
    {
        /// <summary>
        /// Initializes the ASP.NET MVC framework for use.
        /// </summary>
        /// <param name="container">The Castle Windsor container instance to use to resolve dependencies needed by ASP.NET MVC.</param>
        /// <param name="webApplicationAssembly">The web application assembly containing the primary view artifacts.</param>
        void Initialize(IWindsorContainer container, Assembly webApplicationAssembly);
    }

    /// <summary>
    /// Provides a default initialization of the ASP.NET MVC framework, using the Template Method pattern
    /// to allow for customization of the various steps of initialization process.
    /// </summary>
    public class DefaultAspNetMvcFrameworkInitializer : IAspNetMvcFrameworkInitializer
    {
        /// <summary>
        /// Initializes the ASP.NET MVC framework for use.
        /// </summary>
        /// <param name="container">The Castle Windsor container instance to use to resolve dependencies needed by ASP.NET MVC.</param>
        /// <param name="webApplicationAssembly">The web application assembly containing the primary view artifacts.</param>
        public virtual void Initialize(IWindsorContainer container, Assembly webApplicationAssembly)
        {
            // Logic in this method must execute first, before routes and areas registration
            RegisterVirtualPathProviders();

            RegisterAreas();

            InitializeDependencyResolver(container);

            InitializeControllerBuilder();

            InitializeViewEngines(webApplicationAssembly);

            InitializeValueProviderFactories();

            RegisterFilters();

            RegisterRoutes();

            InitializeModelBinders();
        }


        protected virtual void RegisterVirtualPathProviders()
        {
#if DEBUG
            //Should be first line before routes and areas registration
            HostingEnvironment.RegisterVirtualPathProvider(
                new EdFiDevelopmentVirtualPathsProviderDecorator(HostingEnvironment.VirtualPathProvider));
#endif
        }
        
        protected virtual void InitializeModelBinders()
        {
            // Register the extended model binder (to support enum binding)
            ModelBinders.Binders.DefaultBinder = new EdFiModelBinder();
        }

        /// <summary>
        /// Registers non-area routes (and "Ignores", such as for axd's).
        /// </summary>
        protected virtual void RegisterRoutes()
        {
            var routes = RouteTable.Routes;

            routes.IgnoreRoute("{*allaxd}", new { allaxd = @".*\.axd(/.*)?" });

            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            // Register routes
            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "Home", action = "Get", id = UrlParameter.Optional }, // Parameter defaults
                new[]
                    {
                        "EdFi.Dashboards.Presentation.Web.Controllers.*",
                        typeof(MasterPageController).Namespace + ".*"
                    }
                );
        }

        /// <summary>
        /// Registers all filters (including global filters, as appropriate).
        /// </summary>
        protected virtual void RegisterFilters()
        {
            GlobalFilters.Filters.Add(new DashboardContextActionFilter());
            GlobalFilters.Filters.Add(new LogAndHandleErrorAttribute { ExceptionType = typeof(SecurityAccessDeniedException), View = "SecurityAccessDeniedException" });
        }

        /// <summary>
        /// Initializes value provider factories (for translating between incoming route values and their corresponding identifiers on the request models).
        /// </summary>
        protected virtual void InitializeValueProviderFactories()
        {
            // Add value providers to convert route values for model binding
            int pos = 0;

            var localEducationAgencyIdValueProviderFactory = new LocalEducationAgencyIdValueProviderFactory();
            var schoolIdValueProviderFactory = new SchoolIdValueProviderFactory(localEducationAgencyIdValueProviderFactory);
            var metricIdFromOperationalDashboardSubtypeValueProviderFactory =
                new MetricIdFromOperationalDashboardSubtypeValueProviderFactory(schoolIdValueProviderFactory);

            ValueProviderFactories.Factories.Insert(pos++, localEducationAgencyIdValueProviderFactory);
            ValueProviderFactories.Factories.Insert(pos++, schoolIdValueProviderFactory);
            ValueProviderFactories.Factories.Insert(pos++, metricIdFromOperationalDashboardSubtypeValueProviderFactory);
        }

        /// <summary>
        /// Initializes view engines.
        /// </summary>
        /// <param name="webApplicationAssembly">The web application assembly containing the primary views artifacts.</param>
        protected virtual void InitializeViewEngines(Assembly webApplicationAssembly)
        {
            // Register view engines
            ViewEngines.Engines.Clear();
#if DEBUG_VIEWS
            // Add in the standard MVC view engine during development for faster feedback times on view changes
            var razorViewEngine = new RazorViewEngine();
            ViewEngines.Engines.Add(razorViewEngine);
#endif
            var engine = new RazorGeneratorEngine().Create(webApplicationAssembly);
            ViewEngines.Engines.Add(engine); // Additional Engines are added by RazorGeneratorMvcStart in each assembly App_Start folder

            VirtualPathFactoryManager.RegisterVirtualPathFactory(engine);
        }

        /// <summary>
        /// Sets the controller factory and adds default namespaces.
        /// </summary>
        protected virtual void InitializeControllerBuilder()
        {
            ControllerBuilder.Current.SetControllerFactory(new EdFiControllerFactory());
            ControllerBuilder.Current.DefaultNamespaces.Add("EdFi.Dashboards.Presentation.Web.*");
            ControllerBuilder.Current.DefaultNamespaces.Add("EdFi.Dashboards.Presentation.Core.*");
        }

        /// <summary>
        /// Integrates Castle Windsor with ASP.NET MVC's dependency resolver.
        /// </summary>
        /// <param name="container">The Castle Windsor container instance to use to resolve dependencies needed by ASP.NET MVC.</param>
        protected virtual void InitializeDependencyResolver(IWindsorContainer container)
        {
            DependencyResolver.SetResolver(new WindsorDependencyResolver(container));
        }

        /// <summary>
        /// Registers routes for all areas.
        /// </summary>
        protected virtual void RegisterAreas()
        {
            AreaRegistration.RegisterAllAreas();
        }
    }
}
