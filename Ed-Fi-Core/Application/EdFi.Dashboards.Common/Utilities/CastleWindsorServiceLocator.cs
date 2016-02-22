using System;
using System.Configuration;
using System.Linq;
using System.Reflection;
using Castle.Core.Resource;
using Castle.MicroKernel.Context;
using Castle.Windsor;
using Castle.Windsor.Configuration.Interpreters;

namespace EdFi.Dashboards.Common.Utilities
{
    /// <summary>
    /// Implements the service locator pattern using Castle Windsor.
    /// </summary>
    public class CastleWindsorServiceLocator : IServiceLocator
    {
        private IWindsorContainer _container;

        /// <summary>
        /// Gets the Castle Windsor container used by the service locator, or a new one initialized from the "castle" configuration section (if available).
        /// </summary>
        public IWindsorContainer Container
        {
            get
            {
                if (_container == null)
                {
                    ConfigResource configResource = null;

                    try { configResource = new ConfigResource("castle"); }
                    catch (ConfigurationErrorsException) { }

                    _container = configResource == null ? new WindsorContainer() : new WindsorContainer(new XmlInterpreter(configResource));
                    if (configResource != null)
                        configResource.Dispose();
                }

                return _container;
            }
        }

        /// <summary>
        /// Initializes the service locator from a Castle Windsor container.
        /// </summary>
        /// <param name="container">The Castle Windsor container.</param>
        public void Initialize(IWindsorContainer container)
        {
            if (this._container != null)
                this._container.Dispose();

            this._container = container;
        }

        /// <summary>
        /// Initializes the service locator from a configuration section.
        /// </summary>
        /// <param name="configSectionName">The configuration section name.</param>
        public void Initialize(string configSectionName)
        {
            // Go ahead and initialize the container from the default config file section
            IWindsorContainer container = new WindsorContainer(
                new XmlInterpreter(new ConfigResource(configSectionName)));

            Initialize(container);
        }

        /// <summary>
        /// Locates and returns the service implementing the specified type.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> of the service.</param>
        /// <returns>The service implementation.</returns>
        public object Resolve(Type type)
        {
            if (Container == null)
                throw new InvalidOperationException("Castle Windsor container has not been initialized.");

            return Container.Resolve(type);
        }

        /// <summary>
        /// Locates and returns the service implementing the type specified by the generic parameter.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/> of the service.</typeparam>
        /// <returns>The service implementation.</returns>
        public T Resolve<T>()
        {
            if (Container == null)
                throw new InvalidOperationException("Castle Windsor container has not been initialized.");

            return (T)Container.Resolve(typeof(T));
        }

        /// <summary>
        /// Locates and returns the named service implementing the type specified by the generic parameter.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/> of the service.</typeparam>
        /// <returns>The named service implementation.</returns>
        public T Resolve<T>(string key)
        {
            if (Container == null)
                throw new InvalidOperationException("Castle Windsor container has not been initialized.");

            return (T)Container.Resolve(key, typeof(T));
        }

        /// <summary>
        /// Releases the component from the container, preventing possible memory leaks.
        /// </summary>
        /// <param name="instance">The instance of the component to release, for garbage collection.</param>
        public void Release(object instance)
        {
            if (Container == null)
                throw new InvalidOperationException("Castle Windsor container has not been initialized.");

            Container.Release(instance);
        }

        #region Deprecated methods

        //public void RegisterServiceInstance<T>(string key, T instance)
        //{
        //    Container.Kernel.RemoveComponent(key);
        //    Container.Kernel.Register(Component.For<T>().Named(key).Instance(instance));

        //}

        //public void RegisterServiceInstance<T>(T instance)
        //{
        //    Container.Kernel.Register(Component.For<T>().Instance(instance));
        //}

        //public T GetServiceInstance<T>(string key)
        //{
        //    return (T)container.Kernel.Resolve(key, typeof(T));
        //}

        #endregion

        /// <summary>
        /// Locates and returns the service type (not the instance) that implements the specified type.
        /// </summary>
        /// <param name="serviceType">The <see cref="Type"/> of the service.</param>
        /// <returns>The implementation <see cref="Type"/> (not the instance) if found; otherwise <b>null</b>.</returns>
        public Type ResolveImplementationType(Type serviceType)
        {
            if (Container == null)
                throw new InvalidOperationException("Castle Windsor container has not been initialized.");

            var handler =
                (from h in Container.Kernel.GetAssignableHandlers(serviceType)
                select h)
                .FirstOrDefault();

            if (handler.ComponentModel.Implementation.ContainsGenericParameters)
            {
                // Invoke protected method on Kernel to get the creation context for handling generic types
                // Equivalent to --> var context = CreateCreationContext(handler, service, additionalArguments);
                var createCreationContextMethod = Container.Kernel.GetType().GetMethod("CreateCreationContext", BindingFlags.NonPublic | BindingFlags.Instance);

                // Be tolerant of different version of Castle that may have additional arguments (which can be null in this scenario)
                int parameterCount = createCreationContextMethod.GetParameters().Count();

                // Assign values to first two arguments, leave the rest null (however many there are in Castle version being invoked)
                // NOTE: This is fragile code given that it's reflecting on a non-public method, but is necessary to achieve the desired behavior.
                var parameters = new object[parameterCount];
                parameters[0] = handler;
                parameters[1] = serviceType;

                var context = createCreationContextMethod.Invoke(Container.Kernel, parameters) as CreationContext;

                // If all the generic arguments are actually generic parameters, then don't close the generic type (leave it open for the caller to close)
                // This means that an open generic was intentionally registered with the container.
                if (context.GenericArguments.All(t => t.IsGenericParameter))
                    return handler.ComponentModel.Implementation;

                // Close the generic type with the arguments provided (which are not generic parameters themselves)
                var implementationType = handler.ComponentModel.Implementation.MakeGenericType(context.GenericArguments);
                return implementationType;
            }
            else
            {
                return handler.ComponentModel.Implementation;
            }
        }
    }
}