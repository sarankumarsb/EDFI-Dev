// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Castle.Core;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using EdFi.Dashboards.Common.Utilities;
using EdFi.Dashboards.Common.Utilities.CastleWindsorInstallers;
using EdFi.Dashboards.Core;
using EdFi.Dashboards.Core.Providers.Context;
using EdFi.Dashboards.Data.Entities;
using EdFi.Dashboards.Data.Providers;
using EdFi.Dashboards.Infrastructure;
using EdFi.Dashboards.Infrastructure.CastleWindsorInstallers;
using EdFi.Dashboards.Infrastructure.Data;
using EdFi.Dashboards.Infrastructure.Implementations;
using EdFi.Dashboards.Infrastructure.Implementations.Caching;
using EdFi.Dashboards.Metric.Rendering;
using EdFi.Dashboards.Metric.Resources;
using EdFi.Dashboards.Metric.Resources.Factories;
using EdFi.Dashboards.Metric.Resources.InitializationActivity;
using EdFi.Dashboards.Metric.Resources.Providers;
using EdFi.Dashboards.Metric.Resources.Services;
using EdFi.Dashboards.Presentation.Architecture;
using EdFi.Dashboards.Presentation.Architecture.Mvc.AreaRegistration;
using EdFi.Dashboards.Presentation.Architecture.Providers;
using EdFi.Dashboards.Presentation.Core.Providers.Context;
using EdFi.Dashboards.Presentation.Core.Providers.Metric;
using EdFi.Dashboards.Presentation.Core.Providers.Models;
using EdFi.Dashboards.Presentation.Core.Providers.Session;
using EdFi.Dashboards.Presentation.Core.Utilities.Mvc;
using EdFi.Dashboards.Presentation.Web.Architecture;
using EdFi.Dashboards.Presentation.Web.Providers.Metric;
using EdFi.Dashboards.Resources.Application;
using EdFi.Dashboards.Resources;
using EdFi.Dashboards.Resources.Common;
using EdFi.Dashboards.Resources.Images.ContentProvider;
using EdFi.Dashboards.Resources.Images.Navigation;
using EdFi.Dashboards.Resources.InitializationActivities;
using EdFi.Dashboards.Resources.LocalEducationAgency;
using EdFi.Dashboards.Resources.Metric;
using EdFi.Dashboards.Resources.Metric.AdditionalMetricProviders;
using EdFi.Dashboards.Resources.Metric.MetricInstanceSetKeyResolvers;
using EdFi.Dashboards.Resources.Metric.Requests;
using EdFi.Dashboards.Resources.Navigation;
using EdFi.Dashboards.Resources.Navigation.MetricRouteProviders;
using EdFi.Dashboards.Resources.Navigation.Mvc;
using EdFi.Dashboards.Resources.Navigation.Mvc.Areas;
using EdFi.Dashboards.Resources.Navigation.RouteValueProviders;
using EdFi.Dashboards.Resources.Navigation.UserEntryProviders;
using EdFi.Dashboards.Resources.Photo;
using EdFi.Dashboards.Resources.Photo.Implementations;
using EdFi.Dashboards.Resources.Photo.Implementations.NameIdentifier;
using EdFi.Dashboards.Resources.Photo.Implementations.PackageReader;
using EdFi.Dashboards.Resources.Photo.Implementations.Storage;
using EdFi.Dashboards.Resources.Plugins;
using EdFi.Dashboards.Resources.School;
using EdFi.Dashboards.Resources.Search;
using EdFi.Dashboards.Resources.Security;
using EdFi.Dashboards.Resources.Security.Common;
using EdFi.Dashboards.Resources.Security.Implementations;
using EdFi.Dashboards.Resources.Staff;
using EdFi.Dashboards.Resources.StudentMetrics;
using EdFi.Dashboards.Resources.StudentSchool;
using EdFi.Dashboards.Resources.StudentSchool.Detail;
using EdFi.Dashboards.SecurityTokenService.Authentication;
using EdFi.Dashboards.Warehouse.Data.Entities;
using EdFi.Dashboards.Warehouse.Resources;
using EdFi.Dashboards.Warehouse.Resources.Application;
using EdFi.Dashboards.Warehouse.Resources.Metric;
using EdFi.Dashboards.Warehouse.Resources.Metric.AdditionalPriorYearMetricProviders;
using EdFi.Dashboards.Warehouse.Resources.School.Detail;
using EdFi.Dashboards.Warehouse.Resources.Staff;
using IMetricFlagProvider = EdFi.Dashboards.Metric.Resources.Providers.IMetricFlagProvider;
using Component = Castle.MicroKernel.Registration.Component;
using EdFi.Dashboards.SecurityTokenService.Authentication.Implementations;
using EdFi.Dashboards.SecurityTokenService.Authentication.Analytics;

namespace EdFi.Dashboards.Presentation.Core.Utilities.CastleWindsor
{
    public abstract class ConfigurationSpecificInstallerBase : RegistrationMethodsInstallerBase
    {
        protected const string baseCacheInterceptorKey = "Base Cache Interceptor";
        protected const string defaultDatabaseSelector = "Default Database Selector";
        protected const string dataWarehouseDatabaseSelector = "Data Warehouse Database Selector";
        protected const string dataWarehouseDatabaseDataProvider = "DataWarehouseIDataProvider";

        [Preregister]
        protected virtual void RegisterInterceptors(IWindsorContainer container)
        {
            var namedComponent =
                Component.For(typeof (StageInterceptor))
                    .UsingFactoryMethod(CreateStageInterceptor)
                    .Named(baseCacheInterceptorKey);
            container.Register(namedComponent);
        }

        private StageInterceptor CreateStageInterceptor()
        {
            var stages = new Lazy<IInterceptorStage>[1];
            stages[0] = new Lazy<IInterceptorStage>(IoC.Resolve<CacheInterceptor>);
            return new StageInterceptor(stages);
        }

        [Preregister]
        protected virtual void RegisterIConfigSectionProvider(IWindsorContainer container)
        {
            container.Register(Component
                .For<IConfigSectionProvider>()
                .ImplementedBy<AppConfigSectionProvider>());
        }

        [Preregister]
        protected virtual void RegisterIConfigValueProvider(IWindsorContainer container)
        {
            container.Register(Component
                .For<IConfigValueProvider>()
                .ImplementedBy<AppConfigValueProvider>());
        }

        protected virtual void RegisterICacheProvider(IWindsorContainer container)
        {
            container.Register(Component
                .For<ICacheProvider>()
                .ImplementedBy<AspNetCacheProvider>());
        }

        protected virtual void RegisterICookieProvider(IWindsorContainer container)
        {
            container.Register(Component
                .For<ICookieProvider>()
                .ImplementedBy<HttpContextCookieProvider>());
        }

        protected virtual void RegisterICurrentUserProvider(IWindsorContainer container)
        {
            container.Register(Component
                .For<ICurrentUserProvider>()
                .ImplementedBy<HttpContextUserProviderProvider>());
        }

        protected virtual void RegisterICurrentUrlProvider(IWindsorContainer container)
        {
            container.Register(Component
                .For<ICurrentUrlProvider>()
                .ImplementedBy<AspNetCurrentUrlProvider>());
        }

        protected virtual void RegisterIWatchListLinkProvider(IWindsorContainer container)
        {
            container.Register(Component
                .For<IWatchListLinkProvider>()
                .ImplementedBy<WatchListLinkProvider>());
        }

        protected virtual void RegisterIWatchListSearchLinkProvider(IWindsorContainer container)
        {
            container.Register(Component
                .For<IWatchListSearchLinkProvider>()
                .ImplementedBy<WatchListSearchLinkProvider>());
        }

        protected virtual void RegisterIMetricsBasedWatchListSelectionProvider(IWindsorContainer container)
        {
            container.Register(Component
                .For<IMetricsBasedWatchListSelectionProvider>()
                .ImplementedBy<MetricsBasedWatchListSelectionProvider>());
        }

        protected virtual void RegisterIDirectory(IWindsorContainer container)
        {
            container.Register(Component
                .For<IDirectory>()
                .ImplementedBy<DirectoryWrapper>());
        }

        protected virtual void RegisterIHttpServerProvider(IWindsorContainer container)
        {
            container.Register(Component
                .For<IHttpServerProvider>()
                .ImplementedBy<HttpServerProvider>());
        }

        protected virtual void RegisterIImageContentProvider(IWindsorContainer container)
        {
            var assemblyTypes = typeof (Marker_EdFi_Dashboards_Resources).Assembly.GetTypes();

            var chainTypes = (from t in assemblyTypes
                let serviceType = t.GetInterface(typeof (IImageContentProvider).Name)
                where serviceType != null && !t.IsAbstract && t.Name.EndsWith("GenericImageContentProvider")
                select t);

            var chainRegistrar = new ChainOfResponsibilityRegistrar(container);
            chainRegistrar.RegisterChainOf<IImageContentProvider, NullImageProvider>(chainTypes.ToArray(),
                "ImageContentProviderChain");
        }

        protected virtual void RegisterIImageLinkProvider(IWindsorContainer container)
        {
            var assemblyTypes = typeof (Marker_EdFi_Dashboards_Resources).Assembly.GetTypes();

            var chainTypes = (from t in assemblyTypes
                let serviceType = t.GetInterface(typeof (IImageLinkProvider).Name)
                where serviceType != null && !t.IsAbstract && t.Name.EndsWith("RouteImageLinkProvider")
                select t);

            var chainRegistrar = new ChainOfResponsibilityRegistrar(container);
            chainRegistrar.RegisterChainOf<IImageLinkProvider, NullImageLinkProvider>(chainTypes.ToArray(),
                "ImageLinkProviderChain");
        }

        protected virtual void RegisterISerializer(IWindsorContainer container)
        {
            container.Register(Component
                .For<ISerializer>()
                .ImplementedBy<BinaryFormatterSerializer>());
        }

        protected virtual void RegisterISessionStateProvider(IWindsorContainer container)
        {
            container.Register(Component
                .For<ISessionStateProvider>()
                .ImplementedBy<AspNetSessionStateProvider>());
        }

        protected virtual void RegisterISiteAvailableService(IWindsorContainer container)
        {
            container.Register(Component
                .For<ISiteAvailableProvider>()
                .ImplementedBy<SiteAvailableProvider>());
        }

        protected virtual void RegisterISystemMessageProvider(IWindsorContainer container)
        {
            container.Register(Component
                .For<ISystemMessageProvider>()
                .ImplementedBy<SystemMessageProvider>());
        }

        protected virtual void RegisterICsvSerializer(IWindsorContainer container)
        {
            container.Register(Component
                .For<ICsvSerializer>()
                .ImplementedBy<CsvSerializer>());
        }

        protected virtual void RegisterICurrentUserClaimInterrogator(IWindsorContainer container)
        {
            container.Register(Component
                .For<ICurrentUserClaimInterrogator>()
                .Configuration(Attrib.ForName("inspectionBehavior").Eq("none")) //disable property injection
                .ImplementedBy<CurrentUserClaimInterrogator>());
        }

        protected virtual void RegisterICurrentUserAccessibleStudentsProvider(IWindsorContainer container)
        {
            container.Register(Component
                .For<ICurrentUserAccessibleStudentsProvider>()
                .ImplementedBy<CurrentUserAccessibleStudentsProvider>());
        }

        protected virtual void RegisterICacheInterceptor(IWindsorContainer container)
        {
            container.Register(Component
                .For(typeof (CacheInterceptor))
                .ImplementedBy(typeof (CacheInterceptor)));
        }

        protected virtual void RegisterICacheKeyGenerator(IWindsorContainer container)
        {
            container.Register(Component
                .For(typeof (ICacheKeyGenerator))
                .ImplementedBy(typeof (CacheKeyGenerator)));
        }

        protected virtual void RegisterIHttpRequestProvider(IWindsorContainer container)
        {
            container.Register(Component
                .For<IHttpRequestProvider>()
                .ImplementedBy<HttpRequestProvider>());
        }

        protected virtual void RegisterIHttpContextItemsProvider(IWindsorContainer container)
        {
            container.Register(Component
                .For<IHttpContextItemsProvider>()
                .ImplementedBy<HttpContextItemsProvider>());
        }

        protected virtual void RegisterIEdFiDashboardContextProvider(IWindsorContainer container)
        {
            container.Register(Component
                .For<IEdFiDashboardContextProvider>()
                .ImplementedBy<EdFiDashboardContextProvider>());
        }

        protected virtual void RegisterIRootMetricNodeResolver(IWindsorContainer container)
        {
            container.Register(Component
                .For<IRootMetricNodeResolver>()
                .ImplementedBy<RootMetricNodeResolver>());
        }

        protected virtual void RegisterIMetricNodeResolver(IWindsorContainer container)
        {
            container.Register(Component
                .For<IMetricNodeResolver>()
                .ImplementedBy<MetricNodeResolver>());
        }

        protected virtual void RegisterIDomainSpecificMetricNodeResolver(IWindsorContainer container)
        {
            container.Register(Component
                .For<IDomainSpecificMetricNodeResolver>()
                .ImplementedBy<DomainSpecificMetricNodeResolver>());
        }

        protected virtual void RegisterITrendRenderingDispositionProvider(IWindsorContainer container)
        {
            container.Register(Component
                .For<ITrendRenderingDispositionProvider>()
                .ImplementedBy<TrendRenderingDispositionProvider>());
        }

        protected virtual void RegisterIMetricFlagProvider(IWindsorContainer container)
        {
            container.Register(Component
                .For<IMetricFlagProvider>()
                .ImplementedBy<MetricFlagProvider>());
        }

        protected virtual void RegisterIMetricInstanceSetKeyResolver(IWindsorContainer container)
        {
            container.Register(Component
                .For(typeof (IMetricInstanceSetKeyResolver<>))
                .ImplementedBy(typeof (SsisMultipleHashMetricInstanceSetKeyResolverConvertLongToInt<>)));

            //Note: To implement the LookupMetricInstanceSetKeyResolver uncoment the lines below.
            //container.Register(Component
            //    .For(typeof(IMetricInstanceSetKeyResolver<LocalEducationAgencyMetricInstanceSetRequest>))
            //    .ImplementedBy(typeof(LocalEducationAgencyLookupMetricInstanceSetKeyResolver)));
            //container.Register(Component
            //    .For(typeof(IMetricInstanceSetKeyResolver<SchoolMetricInstanceSetRequest>))
            //    .ImplementedBy(typeof(SchoolLookupMetricInstanceSetKeyResolver)));
            //container.Register(Component
            //    .For(typeof(IMetricInstanceSetKeyResolver<StudentSchoolMetricInstanceSetRequest>))
            //    .ImplementedBy(typeof(StudentSchoolLookupMetricInstanceSetKeyResolver)));
        }

        protected virtual void RegisterIMetricInstanceTreeFactory(IWindsorContainer container)
        {
            // NOTE: If there are no initialization activities and/or initialization activity
            // data providers present in the searched assembly(s), instantiation of the
            // MetricInstanceTreeFactory will fail, as castle does not construct an empty
            // array.  The solution is to dig into Castle a little more to see if there is
            // a way to deal with this infrastructure code (i.e. facilities?), or to add
            // one "null" implementation of IMetricInitializationActivity and/or
            // IMetricInitializationActivityDataProvider (that do absolutely nothing, but
            // are present to enable Castle to create and inject the array).
            container.Register(Component
                .For<IMetricInstanceTreeFactory>()
                .ImplementedBy<MetricInstanceTreeFactory>());

            //container.Register(Component
            //    .For<EdFi.Dashboards.Warehouse.Resources.Metric.PriorYearMetricInstanceFactory>()
            //    .ImplementedBy<EdFi.Dashboards.Warehouse.Resources.Metric.PriorYearMetricInstanceFactory>());
        }

        protected virtual void RegisterIContainerMetricFactory(IWindsorContainer container)
        {
            container.Register(Component
                .For<IContainerMetricFactory>()
                .ImplementedBy<ContainerMetricFactory>());
        }

        protected virtual void RegisterIAggregateMetricFactory(IWindsorContainer container)
        {
            container.Register(Component
                .For<IAggregateMetricFactory>()
                .ImplementedBy<AggregateMetricFactory>());
        }

        protected virtual void RegisterIGranularMetricFactory(IWindsorContainer container)
        {
            container.Register(Component
                .For<IGranularMetricFactory>()
                .ImplementedBy<GranularMetricFactory>());
        }

        //protected virtual void RegisterIMetricInstanceSetKeyResolver(IWindsorContainer container)
        //{
        //    container.Register(Component
        //        .For<IMetricInstanceSetKeyResolver>()
        //        .ImplementedBy<MD5HashMetricInstanceSetKeyResolver>());
        //        //.ImplementedBy<LookupMetricInstanceSetKeyResolver>());
        //}

        protected virtual void RegisterIMetricRenderingEngine(IWindsorContainer container)
        {
            container.Register(Component
                .For<IMetricRenderingEngine>()
                .ImplementedBy<MetricRenderingEngine>());
        }

        protected virtual void RegisterIMetricRenderingContextProvider(IWindsorContainer container)
        {
            container.Register(Component
                .For<IMetricRenderingContextProvider>()
                .ImplementedBy<MetricRenderingContextProvider>());
        }

        protected virtual void RegisterIMetricTemplateBinder(IWindsorContainer container)
        {
            container.Register(Component
                .For<IMetricTemplateBinder>()
                .ImplementedBy<MetricTemplateBinder>());
        }

        protected virtual void RegisterIMetricRenderer(IWindsorContainer container)
        {
            container.Register(Component
                .For<IMetricRenderer>()
                .ImplementedBy<AspNetMvcMetricRenderer>()
                .LifeStyle.Transient);
        }

        protected virtual void RegisterIMetricTemplatesProvider(IWindsorContainer container)
        {
            container.Register(Component
                .For<IMetricTemplatesProvider>()
                .ImplementedBy<MetricTemplatesProvider>());
        }

        protected virtual void RegisterIMetricTemplateConventionsProvider(IWindsorContainer container)
        {
            container.Register(Component
                .For<IMetricTemplateConventionsProvider>()
                .ImplementedBy<RazorMetricTemplateConventionsProvider>());
        }

        protected virtual void RegisterIMetricInitializationActivityDataProviders(IWindsorContainer container)
        {
            var assemblyTypes = typeof (Marker_EdFi_Dashboards_Resources).Assembly.GetTypes();

            var allInitializationActivityDataProviderTypes = (from t in assemblyTypes
                where typeof (IMetricInitializationActivityDataProvider).IsAssignableFrom(t)
                select t).ToList();

            foreach (Type dataProvider in allInitializationActivityDataProviderTypes)
            {
                if (!container.Kernel.HasComponent(dataProvider.FullName))
                {
                    container.Register(Component
                        .For<IMetricInitializationActivityDataProvider>()
                        .ImplementedBy(dataProvider));
                }
            }
        }

        protected virtual void RegisterIMetricInitializationActivities(IWindsorContainer container)
        {
            var assemblyTypes = typeof (Marker_EdFi_Dashboards_Resources).Assembly.GetTypes();

            var allInitializationActivityTypes = (from t in assemblyTypes
                where typeof (IMetricInitializationActivity).IsAssignableFrom(t)
                select t).ToList();

            var typesThatDontHaveDependencies = from t in allInitializationActivityTypes
                let customAttrib = t.GetCustomAttributes(true)
                where customAttrib.Length == 0
                select t;

            var typesThatHaveDependencies =
                (from t in allInitializationActivityTypes
                    let customAttrib = t.GetCustomAttributes(true)
                    where customAttrib.Length > 0
                    select t)
                    .ToList();

            List<Type> orderedTypesToRegister = new List<Type>();
            //Adding the ones that don't have dependencies.
            orderedTypesToRegister.AddRange(typesThatDontHaveDependencies);
            //Adding the ones that have dependencies.
            OrderDependencies(typesThatHaveDependencies, orderedTypesToRegister, typesThatHaveDependencies.Count, 0);

            foreach (var type in orderedTypesToRegister)
            {
                var componentRegistration = Component
                    .For<IMetricInitializationActivity>()
                    .ImplementedBy(type);

                container.Register(componentRegistration);
            }

            // Second implementation, may not be safe anymore due to possibility of initialization
            // activities implementing multiple interfaces to also initialize custom data
            ////Register in the container in the specific order.
            //container.Register(
            //    AllTypes.From(orderedTypesToRegister)
            //    .BasedOn<IMetricInitializationActivity>()
            //    .WithService
            //    .FirstInterface());

            /*Original code*/
            //container.Register(AllTypes.FromAssembly(Assembly.GetAssembly(typeof (Marker_EdFi_Dashboards_Resources)))
            //    .BasedOn<IMetricInitializationActivity>()
            //    .WithService.FirstInterface());
        }

        protected void OrderDependencies(List<Type> typesWithDependencies, List<Type> dependencyOrderedList,
            int timesCanRecurse, int timesHasRecursed)
        {
            //If we dont have types that have dependencies then get out.
            if (typesWithDependencies.Count == 0)
                return;

            timesHasRecursed++;
            var dependenciesThatCouldNotRegister = new List<Type>();

            foreach (var t in typesWithDependencies)
            {
                var customAttributes = t.GetCustomAttributes(true);
                var dependencyAttribute =
                    customAttributes.OfType<InitializationActivityDependencyAttribute>()
                        .Select(customAttribute => customAttribute)
                        .FirstOrDefault();

                //If all dependencies are in the list then we can register this type.
                if (AreDependenciesMet(dependencyOrderedList, dependencyAttribute.DependentTypes.ToList()))
                    dependencyOrderedList.Add(t);
                else
                    dependenciesThatCouldNotRegister.Add(t);
            }

            //We need a way to get out of this so:
            if (timesHasRecursed > timesCanRecurse)
            {
                var dependencyNamesThatCouldNotGetRegistered = dependenciesThatCouldNotRegister.Aggregate(string.Empty,
                    (current, t) => current + (t.ToString() + ", "));

                throw new Exception(
                    "Something is wrong with the dependencies in the Initialization Activities. The following dependencies could not be met:" +
                    dependencyNamesThatCouldNotGetRegistered);
            }

            //If we have dependencies that could not get registered in this run lets try again.
            if (dependenciesThatCouldNotRegister.Count > 0)
                OrderDependencies(dependenciesThatCouldNotRegister, dependencyOrderedList, timesCanRecurse,
                    timesHasRecursed);
        }

        private bool AreDependenciesMet(List<Type> dependencyOrderedList, List<Type> dependencies)
        {
            bool dependencyMet = true;

            foreach (var dependency in dependencies)
                if (!dependencyOrderedList.Contains(dependency))
                    dependencyMet = false;

            return dependencyMet;
        }

        protected virtual void RegisterWarehouseAvailabilityProvider(IWindsorContainer container)
        {
            container.Register(Component
                .For<IWarehouseAvailabilityProvider>()
                .ImplementedBy<WarehouseAvailabilityProvider>());
        }

        protected virtual void RegisterWarehouseAvailabilityProviderResource(IWindsorContainer container)
        {
            container.Register(Component
                .For<IWarehouseAvailabilityProviderResource>()
                .ImplementedBy<WarehouseAvailabilityProviderResource>());
        }

        protected virtual void RegisterMaxPriorYearProvider(IWindsorContainer container)
        {
            container.Register(Component
                .For<IMaxPriorYearProvider>()
                .ImplementedBy<MaxPriorYearProvider>());
        }

        protected virtual void RegisterIMetricDataProvider(IWindsorContainer container)
        {
            container.Register(
                Component.For<IMetricDataProvider<LocalEducationAgencyMetricInstanceSetRequest>>()
                    .ImplementedBy(typeof (LocalEducationAgencyMetricDataProvider)));
            container.Register(
                Component.For<IMetricDataProvider<SchoolMetricInstanceSetRequest>>()
                    .ImplementedBy(typeof (SchoolMetricDataProvider)));
            container.Register(
                Component.For<IMetricDataProvider<StudentSchoolMetricInstanceSetRequest>>()
                    .ImplementedBy(typeof (StudentSchoolMetricDataProvider)));
        }

        protected virtual void RegisterIMetricStateProvider(IWindsorContainer container)
        {
            container.Register(Component
                .For<IMetricStateProvider>()
                .ImplementedBy<MetricStateProvider>());
        }

        protected virtual void RegisterIMetricGoalProvider(IWindsorContainer container)
        {
            container.Register(Component
                .For<IMetricGoalProvider>()
                .ImplementedBy<MetricGoalProvider>());
        }

        protected virtual void RegisterIFile(IWindsorContainer container)
        {
            container.Register(Component
                .For<IFile>()
                .ImplementedBy<FileWrapper>());
        }

        protected virtual void RegisterIGradeStateProvider(IWindsorContainer container)
        {
            container.Register(Component
                .For<IGradeStateProvider>()
                .ImplementedBy<GradeStateProvider>());
        }

        protected virtual void RegisterIMetricCorrelationProvider(IWindsorContainer container)
        {
            container.Register(Component
                .For<IMetricCorrelationProvider>()
                .ImplementedBy<MetricCorrelationProvider>());
        }

        protected virtual void RegisterIFileBasedImageContentProvider(IWindsorContainer container)
        {
            container.Register(Component
                .For<IFileBasedImageContentProvider>()
                .ImplementedBy<FileBasedImageContentProvider>());
        }

        protected virtual void RegisterIStudentListUtilitiesProvider(IWindsorContainer container)
        {
            container.Register(Component
                .For<IStudentListUtilitiesProvider>()
                .ImplementedBy<StudentListUtilitiesProvider>());
        }

        protected virtual void RegisterISchoolCategoryProvider(IWindsorContainer container)
        {
            container.Register(Component
                .For<ISchoolCategoryProvider>()
                .ImplementedBy<SchoolCategoryProvider>()
                .Interceptors(InterceptorReference.ForKey(baseCacheInterceptorKey)).Anywhere);
        }

        protected virtual void RegisterICodeIdProvider(IWindsorContainer container)
        {
            container.Register(Component
                .For<ICodeIdProvider>()
                .ImplementedBy<CodeIdProvider>()
                .Interceptors(InterceptorReference.ForKey(baseCacheInterceptorKey)).Anywhere);
        }

        protected virtual void RegisterIPreviousNextSessionProvider(IWindsorContainer container)
        {
            container.Register(Component
                .For<IPreviousNextSessionProvider>()
                .ImplementedBy<PreviousNextSessionProvider>());
        }

        /// <summary>
        /// Registers the concrete class for the IMetricsBasedWatchListDataProvider
        /// interface.
        /// </summary>
        /// <param name="container">The windsor container used to register the interface implementation.</param>
        protected virtual void RegisterIMetricsBasedWatchListDataProvider(IWindsorContainer container)
        {
            container.Register(Component
                .For<IMetricsBasedWatchListDataProvider>()
                .ImplementedBy<MetricsBasedWatchListDataProvider>());
        }

        protected virtual void RegisterIUniqueListIdProvider(IWindsorContainer container)
        {
            container.Register(Component
                .For<IUniqueListIdProvider>()
                .ImplementedBy<UniqueListIdProvider>());
        }

        protected virtual void RegisterIUserContextInterceptor(IWindsorContainer container)
        {
            container.Register(Component
                .For(typeof (UserContextInterceptor))
                .ImplementedBy(typeof (UserContextInterceptor)));
        }

        protected virtual void RegisterIUserContextApplicators(IWindsorContainer container)
        {
            var assemblyTypes = typeof (Marker_EdFi_Dashboards_Resources).Assembly.GetTypes();

            var allUserContextApplicatorTypes = (from t in assemblyTypes
                where typeof (IUserContextApplicator).IsAssignableFrom(t)
                      && !t.IsInterface && !t.IsAbstract
                select t).ToList();

            foreach (var applicatorType in allUserContextApplicatorTypes)
            {
                var componentRegistration = Component
                    .For<IUserContextApplicator>()
                    .ImplementedBy(applicatorType);

                container.Register(componentRegistration);
            }
        }

        protected virtual void RegisterIMetricTreeToIEnumerableOfKeyValuePairProvider(IWindsorContainer container)
        {
            container.Register(Component
                .For<IMetricTreeToIEnumerableOfKeyValuePairProvider>()
                .ImplementedBy<MetricTreeToIEnumerableOfKeyValuePairProvider>());
        }

        protected virtual void RegisterIAspNetMvcFrameworkInitializer(IWindsorContainer container)
        {
            container.Register(Component
                .For<IAspNetMvcFrameworkInitializer>()
                .ImplementedBy<DefaultAspNetMvcFrameworkInitializer>());
        }

        protected virtual void RegisterIAccommodationProvider(IWindsorContainer container)
        {
            container.Register(Component
                .For<IAccommodationProvider>()
                .ImplementedBy<AccommodationProvider>());
        }

        protected virtual void RegisterIRouteValuesPreparer(IWindsorContainer container)
        {
            container.Register(Component
                .For<IRouteValuesPreparer>()
                .ImplementedBy<RouteValuesPreparer>());
        }

        protected virtual void RegisterIMetricRouteProvider(IWindsorContainer container)
        {
            var assemblyTypes = typeof (Marker_EdFi_Dashboards_Resources).Assembly.GetTypes();

            var chainTypes = (from t in assemblyTypes
                where
                    typeof (IMetricRouteProvider).IsAssignableFrom(t) &&
                    t != typeof (IMetricRouteProvider)
                select t);

            var chainRegistrar = new ChainOfResponsibilityRegistrar(container);
            chainRegistrar.RegisterChainOf<IMetricRouteProvider, NullMetricRouteProvider>(chainTypes.ToArray());
        }

        protected virtual void RegisterIAdditionalMetricProvider(IWindsorContainer container)
        {
            var assemblyTypes = typeof (Marker_EdFi_Dashboards_Resources).Assembly.GetTypes();

            var chainTypes = (from t in assemblyTypes
                where
                    typeof (IAdditionalMetricProvider).IsAssignableFrom(t) &&
                    t != typeof (IAdditionalMetricProvider)
                select t);

            var chainRegistrar = new ChainOfResponsibilityRegistrar(container);
            chainRegistrar.RegisterChainOf<IAdditionalMetricProvider, NullAdditionalMetricProvider>(chainTypes.ToArray());
        }

        protected virtual void RegisterIPriorYearStudentMetricsProvider(IWindsorContainer container)
        {
            container.Register(Component
                .For<IPriorYearStudentMetricsProvider>()
                .ImplementedBy<PriorYearStudentMetricsProvider>());
        }

        protected virtual void RegisterIAdditionalPriorYearMetricProvider(IWindsorContainer container)
        {
            var assemblyTypes = typeof (Marker_EdFi_Dashboards_Warehouse_Resources).Assembly.GetTypes();

            var chainTypes = (from t in assemblyTypes
                where
                    typeof (IAdditionalPriorYearMetricProvider).IsAssignableFrom(t) &&
                    t != typeof (IAdditionalPriorYearMetricProvider)
                select t);

            var chainRegistrar = new ChainOfResponsibilityRegistrar(container);
            chainRegistrar.RegisterChainOf<IAdditionalPriorYearMetricProvider, NullAdditionalPriorYearMetricProvider>(
                chainTypes.ToArray());
        }

        protected virtual void RegisterIUserEntryProvider(IWindsorContainer container)
        {
            var assemblyTypes = typeof (Marker_EdFi_Dashboards_Resources).Assembly.GetTypes();

            // The order of chain of responsibility providers is important
            var chainTypes = (from t in assemblyTypes
                where
                    typeof (IUserEntryProvider).IsAssignableFrom(t) &&
                    t != typeof (IUserEntryProvider)
                orderby t.GetCustomAttributes(true)
                    .Where(a => a is ChainOfResponsibilityOrderAttribute)
                    .Select(a => ((ChainOfResponsibilityOrderAttribute) a).Order)
                    .SingleOrDefault()
                select t);

            var chainRegistrar = new ChainOfResponsibilityRegistrar(container);
            chainRegistrar.RegisterChainOf<IUserEntryProvider, NullUserEntryProvider>(chainTypes.ToArray());
        }

        protected virtual void RegisterIMetricActionRouteProvider(IWindsorContainer container)
        {
            var assemblyTypes = typeof (Marker_EdFi_Dashboards_Resources).Assembly.GetTypes();

            var chainTypes = (from t in assemblyTypes
                where
                    typeof (IMetricActionRouteProvider).IsAssignableFrom(t) &&
                    t != typeof (IMetricActionRouteProvider)
                select t);

            var chainRegistrar = new ChainOfResponsibilityRegistrar(container);
            chainRegistrar.RegisterChainOf<IMetricActionRouteProvider, NullMetricActionRouteProvider>(
                chainTypes.ToArray());
        }

        protected virtual void RegisterSiteAreaLinks(IWindsorContainer container)
        {
            container.Register(Component
                .For<IAdminAreaLinks>()
                .ImplementedBy<Admin>());

            container.Register(Component
                .For<IApplicationAreaLinks>()
                .ImplementedBy<Resources.Navigation.Mvc.Areas.Application>());

            container.Register(Component
                .For<ILocalEducationAgencyAreaLinks>()
                .ImplementedBy<LocalEducationAgency>());

            container.Register(Component
                .For<ISchoolAreaLinks>()
                .ImplementedBy<School>());

            container.Register(Component
                .For<IStudentSchoolAreaLinks>()
                .ImplementedBy<StudentSchool>());

            container.Register(Component
                .For<IStaffAreaLinks>()
                .ImplementedBy<Staff>());

            container.Register(Component
                .For<ICommonLinks>()
                .ImplementedBy<EdFi.Dashboards.Resources.Navigation.Mvc.Areas.Common>());

            container.Register(Component
                .For<ISearchAreaLinks>()
                .ImplementedBy<Search>());

            container.Register(Component
                .For<IErrorAreaLinks>()
                .ImplementedBy<EdFi.Dashboards.Resources.Navigation.Mvc.Areas.Error>());
        }

        protected virtual void RegisterIGeneralLinks(IWindsorContainer container)
        {
            container.Register(Component
                .For<IGeneralLinks>()
                .ImplementedBy<General>());
        }

        protected virtual void RegisterILocalEducationAgencyContextProvider(IWindsorContainer container)
        {
            //container.Register(
            //    Component.For<ILocalEducationAgencyContextProvider>()
            //        .ImplementedBy<HttpRequestLocalEducationAgencyContextProvider>());

            var chainTypes = new List<Type>()
            {
                typeof (HttpContextItemsLeaCodeProvider),
                typeof (DashboardContextLeaCodeProvider),
                typeof (HttpRequestUrlLeaCodeProvider),
                typeof (HttpRequestContextLeaCodeProvider)
            };

            var chainRegistrar = new ChainOfResponsibilityRegistrar(container);
            chainRegistrar.RegisterChainOf<ILocalEducationAgencyContextProvider, NullLeaCodeProvider>(
                chainTypes.ToArray());
        }

        protected virtual void RegisterIDbConnectionStringSelector(IWindsorContainer container)
        {
            //container.Register(Component
            //    .For<IDbConnectionStringSelector>()
            //    .Instance(new NamedDbConnectionStringSelector("Multi_LEA")));
            container.Register(Component
                .For<IDbConnectionStringSelector>()
                .ImplementedBy<LocalEducationAgencyContextConnectionStringSelector>().IsDefault());

            container.Register(Component
                .For<IDbConnectionStringSelector>()
                .ImplementedBy<DefaultDbConnectionStringSelector>().Named(defaultDatabaseSelector));

            container.Register(Component
                .For<IDbConnectionStringSelector>()
                .Instance(new NamedDbConnectionStringSelector("DataWarehouse")).Named(dataWarehouseDatabaseSelector));
        }

        protected virtual void RegisterISubsonicDataProvider(IWindsorContainer container)
        {
            container.Register(Component
                .For<ISubsonicDataProviderProvider>()
                .ImplementedBy<SubsonicDataProviderProvider>().IsDefault());

            container.Register(Component
                .For<ISubsonicDataProviderProvider>()
                .ImplementedBy<SubsonicDataProviderProvider>()
                .DependsOn(Dependency.OnComponent("dbConnectionStringSelector", dataWarehouseDatabaseSelector))
                .Named(dataWarehouseDatabaseDataProvider));
        }

        protected virtual void RegisterICacheInitializers(IWindsorContainer container)
        {
            // Register cache initializers as themselves
            container.Register(
                Classes
                    .FromAssemblyContaining<Marker_EdFi_Dashboards_Resources>()
                    .BasedOn<ICacheInitializer>());
        }

        protected virtual void RegisterIListMetadataProvider(IWindsorContainer container)
        {
            container.Register(Component
                .For<IListMetadataProvider>()
                .ImplementedBy<DatabaseListMetadataProvider>());
        }

        protected virtual void RegisterIClassroomMetricsProvider(IWindsorContainer container)
        {
            container.Register(Component
                .For<IClassroomMetricsProvider>()
                .ImplementedBy<ClassroomMetricsProvider>());
        }

        protected virtual void RegisterIPriorYearClassroomMetricsProvider(IWindsorContainer container)
        {
            container.Register(Component
                .For<IPriorYearClassroomMetricsProvider>()
                .ImplementedBy<PriorYearClassroomMetricsProvider>());
        }

        protected virtual void RegisterIAssessmentDetailsProvider(IWindsorContainer container)
        {
            container.Register(Component
                .For<IAssessmentDetailsProvider>()
                .ImplementedBy<AssessmentDetailsProvider>());
        }

        protected virtual void RegisterIAssessmentBenchmarkDetailsProvider(IWindsorContainer container)
        {
            container.Register(Component
                .For<IAssessmentBenchmarkDetailsProvider>()
                .ImplementedBy<AssessmentBenchmarkDetailsProvider>());
        }

        protected virtual void RegisterIAssessmentReadingDetailsProvider(IWindsorContainer container)
        {
            container.Register(Component
                .For<IAssessmentReadingDetailsProvider>()
                .ImplementedBy<AssessmentReadingDetailsProvider>());
        }

        protected virtual void RegisterIMetadataListIdResolver(IWindsorContainer container)
        {
            //container.Register(Component
            //    .For<IMetadataListIdResolver>()
            //    .ImplementedBy<MetadataListIdResolver>());

            container.Register(Component
                .For<IMetadataListIdResolver, IDatabaseMetadataListIdResolver>()
                .ImplementedBy<DatabaseMetadataListIdResolver>());
        }

        protected virtual void RegisterISubjectSpecificOverviewMetricComponentProvider(IWindsorContainer container)
        {
            container.Register(Component
                .For<ISubjectSpecificOverviewMetricComponentProvider>()
                .ImplementedBy<SubjectSpecificOverviewMetricComponentProvider>());
        }

        protected virtual void RegisterIHistoricalLearningObjectiveProvider(IWindsorContainer container)
        {
            container.Register(Component
                .For<IHistoricalLearningObjectiveProvider>()
                .ImplementedBy<HistoricalLearningObjectiveProvider>());
        }

        protected virtual void RegisterIStaffViewProvider(IWindsorContainer container)
        {
            container.Register(Component
                .For<IStaffViewProvider>()
                .ImplementedBy<StaffViewProvider>());
        }

        protected virtual void RegisterIStudentMetricsProvider(IWindsorContainer container)
        {
            container.Register(Component
                .For<IStudentMetricsProvider>()
                .ImplementedBy<StudentMetricsProvider>());
        }

        protected virtual void RegisterIClassroomViewProvider(IWindsorContainer container)
        {
            container.Register(Component
                .For<IClassroomViewProvider>()
                .ImplementedBy<ClassroomViewProvider>());
        }

        protected virtual void RegisterIStaffInformationLookupKeyProvider(IWindsorContainer container)
        {
            container.Register(Component
                .For<IStaffInformationLookupKeyProvider>()
                .ImplementedBy<StaffInformationLookupKeyProvider>());
        }

        protected virtual void RegisterIAssessmentHistorySubjectAreaProvider(IWindsorContainer container)
        {
            container.Register(Component
                .For<IAssessmentHistorySubjectAreaProvider>()
                .ImplementedBy<AssessmentHistorySubjectAreaProvider>());
        }

        protected virtual void RegisterIGradeLevelUtilities(IWindsorContainer container)
        {
            container.Register(Component
                .For<IGradeLevelUtilitiesProvider>()
                .ImplementedBy<GradeLevelUtilitiesProvider>());
        }

        protected virtual void RegisterIPhotoProcessor(IWindsorContainer container)
        {
            container.Register(Component
                .For<IPhotoProcessor>()
                .ImplementedBy<PhotoProcessor>());
        }

        protected virtual void RegisterIArchiveParser(IWindsorContainer container)
        {
            var assemblyTypes = typeof (Marker_EdFi_Dashboards_Resources).Assembly.GetTypes();

            var chainTypes = (from t in assemblyTypes
                let serviceType = t.GetInterface(typeof (IArchiveParser).Name)
                where
                    serviceType != null && !t.IsAbstract && t.Name.EndsWith("ArchiveParser") && !t.Name.Contains("Null")
                select t);

            var chainRegistrar = new ChainOfResponsibilityRegistrar(container);
            chainRegistrar.RegisterChainOf<IArchiveParser, NullArchiveParser>(chainTypes.ToArray(), "ArchiveParserChain");
        }

        protected virtual void RegisterIPackageReader(IWindsorContainer container)
        {
            var assemblyTypes = typeof (Marker_EdFi_Dashboards_Resources).Assembly.GetTypes();

            var chainTypes = (from t in assemblyTypes
                let serviceType = t.GetInterface(typeof (IPackageReader).Name)
                where
                    serviceType != null && !t.IsAbstract && t.Name.EndsWith("PackageReader") && !t.Name.Contains("Null")
                select t);

            var chainRegistrar = new ChainOfResponsibilityRegistrar(container);
            chainRegistrar.RegisterChainOf<IPackageReader, NullPackageReader>(chainTypes.ToArray(), "PackageReaderChain");
        }

        protected virtual void RegisterIIdentifierProvider(IWindsorContainer container)
        {
            var assemblyTypes = typeof (Marker_EdFi_Dashboards_Resources).Assembly.GetTypes();

            var chainTypes = (from t in assemblyTypes
                let serviceType = t.GetInterface(typeof (IIdentifierProvider).Name)
                where
                    serviceType != null && !t.IsAbstract && t.Name.EndsWith("IdentifierProvider") &&
                    !t.Name.Contains("Null")
                select t);

            var chainRegistrar = new ChainOfResponsibilityRegistrar(container);
            chainRegistrar.RegisterChainOf<IIdentifierProvider, NullIdentifierProvider>(chainTypes.ToArray(),
                "IdentifierProviderChain");
        }

        protected virtual void RegisterIPhotoResizer(IWindsorContainer container)
        {
            container.Register(Component
                .For<IPhotoResizer>()
                .ImplementedBy<PhotoResizer>());
        }

        protected virtual void RegisterIPhotoStorage(IWindsorContainer container)
        {
            container.Register(Component
                .For<IPhotoStorage>()
                .ImplementedBy<FilePhotoStorage>());
        }

        protected virtual void RegisterIFileSystemBasedImagePathProvider(IWindsorContainer container)
        {
            container.Register(Component
                .For<IFileSystemBasedImagePathProvider>()
                .ImplementedBy<FileSystemBasedImagePathProvider>());
        }

        protected virtual void RegisterIUnderConstructionProvider(IWindsorContainer container)
        {
            container.Register(Component
                .For<IUnderConstructionProvider>()
                .ImplementedBy<UnderConstructionProvider>());
        }

        protected virtual void RegisterIEdFiGridModelProvider(IWindsorContainer container)
        {
            container.Register(Component
                .For<IEdFiGridModelProvider>()
                .ImplementedBy<EdFiGridModelProvider>());
        }

        protected virtual void RegisterILocalEducationAgencySearchProvider(IWindsorContainer container)
        {
            container.Register(Component
                .For<ILocalEducationAgencySearchProvider>()
                .ImplementedBy<LocalEducationAgencySearchProvider>());
        }

        protected virtual void RegisterPluginResourcesProvider(IWindsorContainer container)
        {
            container.Register(Component
                .For<IPluginResourcesProvider>()
                .ImplementedBy<PluginResourcesProvider>());
        }

        protected virtual void RegisterIStateAssessmentMetricIdGroupingProvider(IWindsorContainer container)
        {
            container.Register(Component
                .For<IStateAssessmentMetricIdGroupingProvider>()
                .ImplementedBy<StateAssessmentMetricIdGroupingProvider>());
    }

        protected virtual void RegisterISignInRequestMessageProvider(IWindsorContainer container)
        {
            container.Register(Component
                .For<ISignInRequestMessageProvider>()
                .ImplementedBy<SecurityTokenServiceSignInRequestMessageProvider>());
        }

        protected virtual void RegisterIClaimsAuthenticationManagerProvider(IWindsorContainer container)
        {
            //STS claims enrichment - when DashboardClaimsGetOutputClaimsIdentityProvider for IGetOutputClaimsIdentityProvider
            container.Register(Component
                .For<IClaimsAuthenticationManagerProvider>()
                .ImplementedBy<PassThroughClaimsAuthenticationManagerProvider>());
        }

        protected virtual void RegisterIWimpProvider(IWindsorContainer container)
        {
            container.Register(Component
                .For<IWimpProvider>()
                .ImplementedBy<Base64WctxWimpProvider>());
        }

        protected virtual void RegisterIGetImpersonatedClaimsDataProvider(IWindsorContainer container)
        {
            container.Register(Component
                .For<IGetImpersonatedClaimsDataProvider>()
                .ImplementedBy<GetImpersonatedClaimsDataProvider>());
        }

        protected virtual void RegisterIUserClaimsProvider(IWindsorContainer container)
        {
            container.Register(Component
                .For<IUserClaimsProvider>()
                .ImplementedBy<DashboardUserClaimsProvider<ClaimsSet, EdFiUserSecurityDetails>>());
        }

        protected virtual void RegisterIDashboardUserClaimsInformationProvider(IWindsorContainer container)
        {
            container.Register(Component
                .For<IDashboardUserClaimsInformationProvider<EdFiUserSecurityDetails>>()
                .ImplementedBy<QEduDashboardUserClaimsInformationProvider>());
                //.ImplementedBy<DashboardUserClaimsInformationProvider>());
        }

        protected virtual void RegisterIUserRolesProvider(IWindsorContainer container)
        {
            var registrar = new ChainOfResponsibilityRegistrar(container);

            var types = new[]
            {
                typeof (PositionTitleUserClaimSetsProvider),
                typeof (StaffCategoryUserClaimSetsProvider)
            };

            registrar
                .RegisterChainOf
                <IUserClaimSetsProvider<ClaimsSet, EdFiUserSecurityDetails>,
                    NullUserClaimSetsProvider<ClaimsSet, EdFiUserSecurityDetails>>(types);
        }

        protected virtual void RegisterIRoleBasedClaimsProvider(IWindsorContainer container)
        {
            container.Register(Component
                .For<IClaimSetBasedClaimsProvider<ClaimsSet>>()
                .ImplementedBy<ClaimSetBasedClaimsProvider>());
        }

        protected virtual void RegisterIAuthenticationProvider(IWindsorContainer container)
        {
            container.Register(Component
                .For<IAuthenticationProvider>()
                .ImplementedBy<AlwaysValidAuthenticationProvider>()
                .DynamicParameters((k, d) => d["emailDomain"] = "edfi.org"));
        }

        protected virtual void RegisterIClaimsIssuedTrackingManager(IWindsorContainer container)
        {
            container.Register(Component
                .For<IClaimsIssuedTrackingEventProvider>()
                .ImplementedBy<ClaimsIssuedTrackingEventProvider>());
        }

        protected virtual void RegisterIStaffInformationProvider(IWindsorContainer container)
        {
            container.Register(Component
                .For<IStaffInformationProvider>()
                .ImplementedBy<QEduLoginInformationProvider>());
                //.ImplementedBy<StaffInformationProvider>());
        }

        protected virtual void RegisterIRequestUrlBaseProvider(IWindsorContainer container)
        {
            container.Register(Component
                .For<IRequestUrlBaseProvider>()
                .ImplementedBy<RequestUrlBaseProvider>());
        }

        protected virtual void RegisterIStudentMetricFilters(IWindsorContainer container)
        {
            var assembly = typeof(Marker_EdFi_Dashboards_Resources).Assembly;

            container.Register(
                Classes.FromAssembly(assembly).BasedOn<IStudentMetricFilter>().WithService.FirstInterface());
        }

        protected virtual void RegisterITabFactory(IWindsorContainer container)
        {
            container.Register(Component
                .For<ITabFactory>()
                .ImplementedBy<TabFactory>());
        }

        protected virtual void RegisterIStudentWatchListManager(IWindsorContainer container)
        {
            container.Register(Component.For<IStudentWatchListManager>()
                                        .ImplementedBy<StudentWatchListManager>());
        }
    }
}
