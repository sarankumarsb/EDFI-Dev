// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web.Hosting;
using Castle.Core.Logging;
using Castle.MicroKernel;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.Resolvers.SpecializedResolvers;
using Castle.Windsor;
using Castle.Windsor.Configuration.Interpreters;
using EdFi.Dashboards.Common;
using EdFi.Dashboards.Common.Configuration;
using EdFi.Dashboards.Infrastructure.Plugins.Helpers;

namespace EdFi.Dashboards.Infrastructure.CastleWindsorInstallers
{
    // NOTE: This class is only in this assembly because it has a dependency on IConfigValueProvider/IConfigSectionProvider
    // and we cannot currently create a reference from Common to Infrastructure or it will create a cyclical dependency.

    /// <summary>
    /// Creates and initializes a Castle Windsor container using the installers configured for the application.
    /// </summary>
    public class InversionOfControlContainerFactory
    {
        private readonly IConfigSectionProvider configSectionProvider;
        private readonly IConfigValueProvider configValueProvider;

        public InversionOfControlContainerFactory(IConfigSectionProvider configSectionProvider, IConfigValueProvider configValueProvider)
        {
            this.configSectionProvider = configSectionProvider;
            this.configValueProvider = configValueProvider;
        }

        /// <summary>
        /// Creates the Castle Windsor container using the "castle" section from the application configuration file (if it exists),
        /// and then executes all the "installers" to register all components with the container. 
        /// </summary>
        /// <returns>The newly initialized container.</returns>
        public IWindsorContainer CreateContainer()
        {
            return CreateContainer(null);
        }

        /// <summary>
        /// Creates the Castle Windsor container using the "castle" section from the application configuration file (if it exists),
        /// calls the provided <paramref name="onExecutingInstallers"/> delegate, and then executes all the "installers" to register 
        /// all components with the container. 
        /// </summary>
        /// <param name="onExecutingInstallers">An action delegate to be executed after the container is created, but before any of 
        /// the installers are executed.  Provides an opportunity for the caller to install facilities, subresolvers, etc. prior
        /// to the full registration of components.</param>
        /// <returns>The newly initialized container.</returns>
        public IWindsorContainer CreateContainer(Action<IWindsorContainer> onExecutingInstallers)
        {
            var castleSection = configSectionProvider.GetSection("castle");

            // Create and install components into the container from the web.config file
            WindsorContainer container = (castleSection == null 
                || string.IsNullOrWhiteSpace(
                    ((ConfigurationSection) castleSection)
                        .SectionInformation.GetRawXml())) 
                ? new WindsorContainer() 
                : new WindsorContainer(new XmlInterpreter());

            if (onExecutingInstallers != null)
                onExecutingInstallers(container);

            // Install other components into the container, using installers
            container.Install(GetInstallers());

            return container;
        }

        private IWindsorInstaller[] GetInstallers()
        {
            IEnumerable<string> installerTypeNames = GetInstallerTypeNames();

            var installers = new List<IWindsorInstaller>();

            //Add plugin installers first just incase we need to override anything.
            installers.AddRange(PluginHelper.GetPluginInstallers());

            foreach (string installerTypeName in installerTypeNames)
            {
                Type t = Type.GetType(installerTypeName);

                if (t == null)
                    throw new InvalidOperationException(string.Format("Could not locate configured type '{0}'.", installerTypeName));

                var installer = Activator.CreateInstance(t) as IWindsorInstaller;

                if (installer == null)
                    throw new InvalidOperationException(string.Format("Configured type '{0}' does not implement IWindsorInstaller.", installerTypeName));

                installers.Add(installer);
            }

            return installers.ToArray();
        }

        private IEnumerable<string> GetInstallerTypeNames() {
            IEnumerable<string> installerTypeNames;

            // First check for new installer mechanism (config section)
            var inversionOfControlConfig = configSectionProvider.GetSection(InversionOfControlConfiguration.SectionName) as InversionOfControlConfiguration;

            if (inversionOfControlConfig == null)
            {
                // Revert to legacy mechanism of parsing out an appSetting value
                string installersText = configValueProvider.GetValue("WindsorInstallers");
                
                installerTypeNames =
                    from rawInstaller in installersText.Split(';')
                    select rawInstaller.Trim('\r', '\n', ' ', '\t');
            }
            else
            {
                // Get installers from "inversionOfControl" config section
                installerTypeNames =
                    from i in inversionOfControlConfig.Installers.OfType<WindsorInstaller>()
                    select i.TypeName;
            }

            return installerTypeNames;
        }
    }
}
