// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Castle.Core;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using EdFi.Dashboards.Common;
using EdFi.Dashboards.Common.Utilities;
using EdFi.Dashboards.Infrastructure;
using EdFi.Dashboards.Infrastructure.Implementations.Caching;
using EdFi.Dashboards.Metric.Resources;
using EdFi.Dashboards.Metric.Resources.Services;
using EdFi.Dashboards.Resources.Metric.Requests;

namespace EdFi.Dashboards.Resources.CastleWindsorInstallers
{
    public class MetricServiceInstaller : IWindsorInstaller
    {
        private const string metricCacheInterceptorKey = "Metric Cache Interceptor";
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            var allTypesForMetrics = Assembly.GetAssembly(typeof(Marker_EdFi_Dashboards_Metric_Resources)).GetTypes();

            RegisterInterceptors(container);

            RegisterServices(container, allTypesForMetrics);
        }

        private void RegisterInterceptors(IWindsorContainer container)
        {
            var namedComponent = Component.For(typeof(StageInterceptor)).UsingFactoryMethod(CreateStageInterceptor).Named(metricCacheInterceptorKey);
            container.Register(namedComponent);
        }

        private StageInterceptor CreateStageInterceptor()
        {
            var stages = new Lazy<IInterceptorStage>[1];
            stages[0] = new Lazy<IInterceptorStage>(IoC.Resolve<CacheInterceptor>);
            return new StageInterceptor(stages);
        }

        private static void RegisterServices(IWindsorContainer container, IEnumerable<Type> allTypes)
        {
            if (allTypes == null)
                return;

            var servicesToRegister =
                from serviceType in allTypes
                    .Where(t => t.Name.StartsWith("I") 
                                && t.Name.EndsWith("Service"))
                from concreteType in allTypes
                    .Where(t => t.Name == serviceType.Name.Substring(1) && t.Namespace == serviceType.Namespace)
                select new
                        {
                            ServiceType = serviceType, 
                            ConcreteType = concreteType
                        };

            servicesToRegister.Each(
                pair =>
                    container.Register(
                        Component
                            .For(pair.ServiceType)
                            .ImplementedBy(pair.ConcreteType)
                            .Interceptors(InterceptorReference.ForKey(metricCacheInterceptorKey)).Anywhere
                        )
                    );

            
            // Register closed IMetricService<TRequest> implementations
            var metricServicesToRegister =
                from requestType in MetricRequestTypes
                select new
                           {
                               ServiceType = typeof(IMetricService<>).MakeGenericType(requestType),
                               ConcreteType = typeof(MetricService<>).MakeGenericType(requestType),
                           };

            foreach (var pair in metricServicesToRegister)
            {
                container.Register(
                    Component
                        .For(pair.ServiceType)
                        .ImplementedBy(pair.ConcreteType)
                        .Interceptors(InterceptorReference.ForKey(metricCacheInterceptorKey)).Anywhere
                    );
            }

            // Register closed IMetricDataService<TRequest> implementations
            var metricDataServicesToRegister =
                from requestType in MetricRequestTypes
                select new
                {
                    ServiceType = typeof(IMetricDataService<>).MakeGenericType(requestType),
                    ConcreteType = typeof(MetricDataService<>).MakeGenericType(requestType),
                };

            foreach (var pair in metricDataServicesToRegister)
            {
                container.Register(
                    Component
                        .For(pair.ServiceType)
                        .ImplementedBy(pair.ConcreteType)
                        .Interceptors(InterceptorReference.ForKey(metricCacheInterceptorKey)).Anywhere
                    );
            }

            // Register closed IMetricDataProvider<TRequest> implementations
            var metricDataProvidersToRegister =
                from requestType in MetricRequestTypes
                select new
                {
                    ServiceType = typeof(IMetricDataProvider<>).MakeGenericType(requestType),
                    ConcreteType = typeof(CurrentYearMetricDataProvider<>).MakeGenericType(requestType),
                };

            foreach (var pair in metricDataProvidersToRegister)
            {
                container.Register(
                    Component
                        .For(pair.ServiceType)
                        .ImplementedBy(pair.ConcreteType)
                    );
            }


            // Find request/response services
            var genericServicesToRegister =
                (from concreteType in allTypes
                let serviceType = concreteType.GetInterface(typeof(IService<,>).Name)
                let closedServiceType = concreteType.GetInterface("I" + concreteType.Name)
                where !concreteType.IsAbstract && serviceType != null
                     //Don't register generic types like DomainMetricService / IDomainMetricService, because we can't use DomainMetricService as the concrete type.
                     //If you want to do this, look at IMetricDataProvider to figure out how to register the generic types.
                   && !concreteType.IsGenericType
                let genericArgsBaseTypes = 
                    new [] 
                    {
                        // Safe Assumption: IService implementations always have 2 generic arguments
                        serviceType.GetGenericArguments()[0].GetBaseRequestTypeForServiceRegistration(),
                        serviceType.GetGenericArguments()[1].GetBaseModelTypeForServiceRegistration(),
                    }
                select new
                {
                    ServiceType = typeof(IService<,>).MakeGenericType(genericArgsBaseTypes),
                    ClosedServiceType = closedServiceType,
                    ConcreteType = concreteType
                }).ToArray().OrderBy(x => x.ToString());

            foreach (var pair in genericServicesToRegister)
            {
                if (!container.Kernel.HasComponent(pair.ServiceType))
                    container.Register(
                        Component
                            .For(pair.ServiceType)
                            .ImplementedBy(pair.ConcreteType)
                            .Named(pair.ConcreteType.FullName + " - Generic")
                            .Interceptors(InterceptorReference.ForKey(metricCacheInterceptorKey)).Anywhere
                        );

                // Register services, accessible by closed service type as well.
                if (pair.ClosedServiceType != null &&
                    !container.Kernel.HasComponent(pair.ClosedServiceType))
                {
                    container.Register(
                        Component
                            .For(pair.ClosedServiceType)
                            .ImplementedBy(pair.ConcreteType)
                            .Named(pair.ConcreteType.FullName + " - Closed")
                            .Interceptors(InterceptorReference.ForKey(metricCacheInterceptorKey)).Anywhere
                        );
                }
            }
        }

        private static IEnumerable<Type> _metricRequestTypes;

        private static IEnumerable<Type> MetricRequestTypes
        {
            get
            {
                if (_metricRequestTypes == null)
                {
                    // TODO: Deferred - Will need to widen the search to include extension assemblies
                    var assembly = typeof(Marker_EdFi_Dashboards_Resources).Assembly;

                    _metricRequestTypes = MetricRequestTypesHelper.GetMetricRequestTypes(assembly);
                }

                return _metricRequestTypes;
            }
        }
    }
}
