using System;
using System.Linq;
using Cassette;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;


namespace EdFi.Dashboards.Presentation.Core.Plugins.Utilities.CastleWindsor
{
    public abstract class WebDefaultConventionInstaller<TMarker> : IWindsorInstaller
    {        
        public virtual void Install(IWindsorContainer container, IConfigurationStore store)
        {
            var markerAssembly = typeof(TMarker).Assembly;

            var controllerInstaller = new EdFi.Dashboards.Presentation.Architecture.CastleWindsor.ControllerInstaller<TMarker>();

            container.Install(new IWindsorInstaller[] { controllerInstaller });

            var cassetteConfigType = markerAssembly.GetTypes().SingleOrDefault(x => IsAssignableFromGenericInterface(x, typeof(IConfiguration<>)));
            if (cassetteConfigType != null)
                container.Register(
                    Component.For<IConfiguration<BundleCollection>>()                             
                             .ImplementedBy(cassetteConfigType)
                             );
        }

        private static bool IsAssignableFromGenericInterface(Type type, Type interfaceType)
        {
            if (type == null)
                throw new ArgumentNullException("type");

            return type.GetInterfaces().Where(it => it.IsGenericType).Any(it => it.GetGenericTypeDefinition() == interfaceType);
        }
    }
}


