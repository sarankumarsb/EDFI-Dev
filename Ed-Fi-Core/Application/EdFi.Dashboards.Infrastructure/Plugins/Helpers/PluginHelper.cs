using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web.Hosting;
using Castle.MicroKernel.Registration;

namespace EdFi.Dashboards.Infrastructure.Plugins.Helpers

{
    public static class PluginHelper
    {
        public static IEnumerable<Assembly> GetAllPluginAssemblies()
        {
            return GetPluginAssemblies("EdFi.Dashboards.Plugins.*.dll");

        }

        public static IEnumerable<Assembly> GetWebPluginAssemblies()
        {
            return GetPluginAssemblies("EdFi.Dashboards.Plugins.*.Web.dll");
        }

        public static string GetPluginPath()
        { 
            return HostingEnvironment.MapPath("~/plugins");
        }

        public static IEnumerable<IWindsorInstaller> GetPluginInstallers()
        {
            var installers = new List<IWindsorInstaller>();

            var pluginAssemblies = GetAllPluginAssemblies().ToList();

            if (!pluginAssemblies.Any())
                return installers;

            foreach (var assembly in pluginAssemblies)
            {
                var types = assembly.GetTypes().Where(t => t.GetInterface(typeof(IWindsorInstaller).Name) != null);

                installers.AddRange(from type in types where type != null select (IWindsorInstaller)Activator.CreateInstance(type));
            }

            return installers;
        }

        private static IEnumerable<Assembly> GetPluginAssemblies(string filter)
        {
            var pluginFolder = GetPluginFolderInfo();

            if (pluginFolder == null)
                return new List<Assembly>();

            return pluginFolder.GetFiles(filter, SearchOption.AllDirectories)
                    .Select(x => AssemblyName.GetAssemblyName(x.FullName))
                    .Select(x => Assembly.Load(x.FullName));
        }

        private static DirectoryInfo GetPluginFolderInfo()
        {
            string pluginsPath = GetPluginPath();

            if (!Directory.Exists(pluginsPath)) { 
                // If we dont have a plugins folder then ignore. We probably don't have plugins to install.
                //throw new DirectoryNotFoundException("Plugins");
                return null;
            }

            return new DirectoryInfo(pluginsPath);
        }
    }
}
