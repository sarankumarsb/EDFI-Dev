// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using EdFi.Dashboards.Common;
using EdFi.Dashboards.Common.Utilities;
using EdFi.Dashboards.Metric.Resources.Providers;
using EdFi.Dashboards.Resources.Metric;
using EdFi.Dashboards.Resources.Metric.MetricInstanceSetKeyResolvers;
using EdFi.Dashboards.Resources.Metric.Requests;
using EdFi.Dashboards.Resources.Security.Common;

namespace EdFi.Dashboards.Resources.CastleWindsorInstallers
{
    /// <summary>
    /// Registers the presentation services with the Castle Windsor Inversion of Control container using a convention 
    /// where {<i>subject</i>}Service is registered as the implementation for I{<i>subject</i>}Service.
    /// </summary>
    public class GenericServiceInstaller<TMarker> : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            var genericType = typeof (TMarker);

            var allLocalTypes = genericType.Assembly.GetTypes();

            RegisterServices(container, allLocalTypes);
        }

        private static void RegisterServices(IWindsorContainer container, IEnumerable<Type> allTypes)
        {
            if (allTypes == null)
                return;

            var servicesToRegister =
                from serviceType in allTypes
                    .Where(t => t.Name.StartsWith("I")
                                && t.Name.EndsWith("Service"))
                                // TODO: GKM - && t.Name.EndsWith("ResourceHandler"))
                from concreteType in allTypes
                    .Where(t => t.Name == serviceType.Name.Substring(1) && t.Namespace == serviceType.Namespace)
                select new
                {
                    ServiceType = serviceType,
                    ConcreteType = concreteType
                };

            foreach (var pair in servicesToRegister)
            {
                if (!container.Kernel.HasComponent(pair.ServiceType))
                    container.Register(
                        Component
                            .For(pair.ServiceType)
                            .ImplementedBy(pair.ConcreteType)
                            .Interceptors(SecurityGlobals.DomainServiceStageInterceptorKey)
                        );
            }

            RegisterGenericServices(container, allTypes, typeof(IService<,>));
            // GKM - TODO: RegisterGenericServices(container, allTypes, typeof(IGetHandler<,>));
            RegisterGenericServices(container, allTypes, typeof(IPostHandler<,>));
            RegisterGenericServices(container, allTypes, typeof(IPutHandler<,>));
            RegisterGenericServices(container, allTypes, typeof(IDeleteHandler<>));

            // Register domain-specific metric services
            var metricServicesToRegister =
                from requestType in MetricRequestTypes
                select new
                {
                    ServiceType = typeof(IDomainMetricService<>).MakeGenericType(requestType),
                    ConcreteType = typeof(DomainMetricService<>).MakeGenericType(requestType),
                };

            foreach (var pair in metricServicesToRegister)
            {
                if (!container.Kernel.HasComponent(pair.ServiceType))
                    container.Register(
                        Component
                            .For(pair.ServiceType)
                            .ImplementedBy(pair.ConcreteType)
                            .Interceptors(SecurityGlobals.DomainServiceStageInterceptorKey)
                        );
            }
        }

        private static void RegisterGenericServices(IWindsorContainer container, IEnumerable<Type> allTypes, Type interfaceType)
        {
            // Find request/response services
            var concreteServiceImplementations =
                (from concreteType in allTypes
                 let serviceTypes = concreteType.GetInterfaces().Where(x => x.Name == interfaceType.Name).ToArray()
                 let closedServiceType = concreteType.GetInterface("I" + concreteType.Name)
                 where !concreteType.IsAbstract && serviceTypes.Length > 0
                 select new { ServiceTypes = serviceTypes, ClosedServiceType = closedServiceType, ConcreteType = concreteType});

            var genericServicesToRegister =
                (from csi in concreteServiceImplementations
                from serviceType in csi.ServiceTypes
                 let genericArgs = serviceType.GetGenericArguments()
                 let genericArgsBaseTypes =
                    genericArgs.Length == 1 ?
                        new[] 
                        {
                            // Deal with Request-only service interfaces (no return model)
                            genericArgs[0].GetBaseRequestTypeForServiceRegistration(),
                        }
                        :
                        new[] 
                        {
                            // Deal with Request/Response service interfaces
                            genericArgs[0].GetBaseRequestTypeForServiceRegistration(),
                            genericArgs[1].GetBaseModelTypeForServiceRegistration(),
                        }

                 select new
                            {
                                ServiceType = interfaceType.MakeGenericType(genericArgsBaseTypes), //The base service type IService<BaseRequest,BaseResponseModel>.
                                ClosedServiceType = csi.ClosedServiceType,
                                ConcreteType = csi.ConcreteType,
                                foo = serviceType,//The real service type IService<BaseRequest, ExtendedResponseModel>. If any, if not it is the same one as ServiceType.
                                arg = serviceType.GetGenericArguments()
                            }).ToArray().OrderBy(x => x.ToString());

            var componentRegistrationsByServiceType = new Dictionary<Type, ComponentRegistration<object>>();

            foreach (var pair in genericServicesToRegister)
            {
                if (!container.Kernel.HasComponent(pair.ServiceType))
                {
                    ComponentRegistration<object> registration;

                    if (componentRegistrationsByServiceType.TryGetValue(pair.ConcreteType, out registration))
                    {
                        // Simply forward the second service type to the concrete type, rather than having a second complete registration
                        registration.Forward(pair.ServiceType);
                    }
                    else
                    {
                        componentRegistrationsByServiceType.Add(pair.ConcreteType,
                            Component
                                .For(pair.ServiceType)
                                .ImplementedBy(pair.ConcreteType)
                                .Named(pair.ConcreteType.FullName + " - Generic - " + interfaceType.FullName)
                                .Interceptors(SecurityGlobals.DomainServiceStageInterceptorKey)
                        );
                    }
                }

                // Register services, accessible by closed service type as well.
                if (pair.ClosedServiceType != null &&
                    !container.Kernel.HasComponent(pair.ClosedServiceType))
                {
                    container.Register(
                        Component
                            .For(pair.ClosedServiceType)
                            .ImplementedBy(pair.ConcreteType)
                            .Named(pair.ConcreteType.FullName + " - Closed - " + interfaceType.FullName)
                            .Interceptors(SecurityGlobals.DomainServiceStageInterceptorKey)
                        );
                }
            }

            var registrations = componentRegistrationsByServiceType.Values.Select(r => r).ToArray();
            container.Register(registrations);
        }

        private static IEnumerable<Type> _metricRequestTypes;

        private static IEnumerable<Type> MetricRequestTypes
        {
            get
            {
                if (_metricRequestTypes == null)
                {
                    // TODO: Deferred - Will need to widen the search to include extension assemblies
                    var genericType = typeof(TMarker);
                    var assembly = genericType.Assembly;

                    _metricRequestTypes = MetricRequestTypesHelper.GetMetricRequestTypes(assembly);
                }

                return _metricRequestTypes;
            }
        }
    }
}
