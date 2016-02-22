using System.IO;
using System.Linq;
using System.Reflection;
using System.Web.Compilation;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.WebPages;
using EdFi.Dashboards.Presentation.Architecture.Mvc.Core;
using EdFi.Dashboards.Infrastructure.Plugins.Helpers;
using EdFi.Dashboards.Resources.CastleWindsorInstallers;
using RazorGenerator.Mvc;

[assembly: WebActivator.PostApplicationStartMethod(typeof(EdFi.Dashboards.Presentation.Core.App_Start.PluginViewEngineStart), "Start")]
namespace EdFi.Dashboards.Presentation.Core.App_Start
{
    public static class PluginViewEngineStart
    {
        public static void Start()
        {
            var assemblies = PluginHelper.GetWebPluginAssemblies();

            foreach (var assembly in assemblies)
                RegisterViewEngine(assembly);
        }

        public static void RegisterViewEngine(Assembly assembly)
        {            
            var engine = new RazorGeneratorEngine().Create(assembly);

            ViewEngines.Engines.Insert(0, engine); //Additional Engines are added by RazorGeneratorMvcStart in each assembly App_Start folder

            VirtualPathFactoryManager.RegisterVirtualPathFactory(engine);
        }
    }
}

