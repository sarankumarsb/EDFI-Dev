// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;
using Castle.Windsor;

namespace EdFi.Dashboards.Common.Utilities
{
    /// <summary>
    /// Provides static methods for obtaining services in a decoupled fashion.
    /// </summary>
    public static class IoC
    {
        private static CastleWindsorServiceLocator serviceLocatorInstance;

        /// <summary>
        /// Initializes the static members of the <see cref="IoC"/> class.
        /// </summary>
        static IoC()
        {
            serviceLocatorInstance = new CastleWindsorServiceLocator();
        }

        /// <summary>
        /// Gets the wrapped service locator's Castle Windsor container, or a new one initialized from the "castle" configuration section (if available).
        /// </summary>
        public static IWindsorContainer Container
        {
            get { return serviceLocatorInstance.Container; }
        }

        /// <summary>
        /// Gets the wrapped service locator.
        /// </summary>
        public static IServiceLocator WrappedServiceLocator
        {
            get { return serviceLocatorInstance; }
        }

        /// <summary>
        /// Initializes the wrapped service locator from a Castle Windsor container.
        /// </summary>
        /// <param name="container">The Castle Windsor container.</param>
        public static void Initialize(IWindsorContainer container)
        {
            serviceLocatorInstance.Initialize(container);
        }

        /// <summary>
        /// Initializes the wrapped service locator from a configuration section.
        /// </summary>
        /// <param name="configSectionName">The configuration section name.</param>
        public static void Initialize(string configSectionName)
        {
            serviceLocatorInstance.Initialize(configSectionName);
        }

        /// <summary>
        /// Locates and returns the service implementing the specified type.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> of the service.</param>
        /// <returns>The service implementation.</returns>
        public static object Resolve(Type type)
        {
            return serviceLocatorInstance.Resolve(type);
        }

        /// <summary>
        /// Locates and returns the service type (not the instance) that implements the specified type.
        /// </summary>
        /// <param name="serviceType">The <see cref="Type"/> of the service.</param>
        /// <returns>The implementation <see cref="Type"/> (not the instance) if found; otherwise <b>null</b>.</returns>
        public static Type ResolveImplementationType(Type serviceType)
        {
            return serviceLocatorInstance.ResolveImplementationType(serviceType);
        }

        /// <summary>
        /// Locates and returns the service implementing the type specified by the generic parameter.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/> of the service.</typeparam>
        /// <returns>The service implementation.</returns>
        public static T Resolve<T>()
        {
            return serviceLocatorInstance.Resolve<T>();
        }

        /// <summary>
        /// Locates and returns the named service implementing the type specified by the generic parameter.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/> of the service.</typeparam>
        /// <returns>The named service implementation.</returns>
        public static T Resolve<T>(string key)
        {
            return serviceLocatorInstance.Resolve<T>(key);
        }

        /// <summary>
        /// Locates and returns all services implementing the type specified by the generic parameter.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/> of the service.</typeparam>
        /// <returns>All implementations of the service.</returns>
        public static T[] ResolveAll<T>()
        {
            return Container.ResolveAll<T>();
        }

        /// <summary>
        /// Releases the component from the container, preventing possible memory leaks.
        /// </summary>
        /// <param name="instance">The instance of the component to release, for garbage collection.</param>
        public static void Release(object instance)
        {
            serviceLocatorInstance.Release(instance);
        }

        #region Deprecated members

        //public static void RegisterServiceInstance<T>(string key, T instance)
        //{
        //    serviceLocatorInstance.RegisterServiceInstance(key, instance);
        //}

        //public static void RegisterServiceInstance<T>(T instance)
        //{
        //    serviceLocatorInstance.RegisterServiceInstance(instance);
        //}

        //public static T GetServiceInstance<T>(string key)
        //{
        //    return serviceLocatorInstance.GetServiceInstance<T>(key);
        //}

        #endregion
    }
}