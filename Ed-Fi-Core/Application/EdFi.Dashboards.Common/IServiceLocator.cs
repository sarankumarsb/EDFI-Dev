using System;

namespace EdFi.Dashboards.Common
{
    /// <summary>
    /// Provides methods for obtaining services in a decoupled fashion.
    /// </summary>
    public interface IServiceLocator
    {
        /// <summary>
        /// Locates and returns the service implementing the specified type.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> of the service.</param>
        /// <returns>The service implementation.</returns>
        object Resolve(Type type);

        /// <summary>
        /// Locates and returns the service type (not the instance) that implements the specified type.
        /// </summary>
        /// <param name="serviceType">The <see cref="Type"/> of the service.</param>
        /// <returns>The implementation <see cref="Type"/> (not the instance) if found; otherwise <b>null</b>.</returns>
        Type ResolveImplementationType(Type serviceType);
 
        /// <summary>
        /// Locates and returns the service implementing the type specified by the generic parameter.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/> of the service.</typeparam>
        /// <returns>The service implementation.</returns>
        T Resolve<T>();

        /// <summary>
        /// Locates and returns the named service implementing the type specified by the generic parameter.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/> of the service.</typeparam>
        /// <returns>The named service implementation.</returns>
        T Resolve<T>(string key);

        /// <summary>
        /// Releases the component from the container, preventing possible memory leaks.
        /// </summary>
        /// <param name="instance">The instance of the component to release, for garbage collection.</param>
        void Release(object instance);

        #region Deprecated members

        //void RegisterServiceInstance<T>(string key, T instance);
        //void RegisterServiceInstance<T>(T instance);
        //T GetServiceInstance<T>(string key);

        #endregion
    }
}