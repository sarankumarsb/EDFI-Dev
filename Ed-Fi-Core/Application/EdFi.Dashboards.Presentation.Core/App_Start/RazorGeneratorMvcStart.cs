using System.Web.Mvc;
using System.Web.WebPages;
using EdFi.Dashboards.Presentation.Architecture.Mvc.Core;

[assembly: WebActivator.PostApplicationStartMethod(typeof(EdFi.Dashboards.Presentation.Core.App_Start.RazorGeneratorMvcStart), "Start")]

namespace EdFi.Dashboards.Presentation.Core.App_Start
{
    public static class RazorGeneratorMvcStart
    {
        public static void Start()
        {
            var engine = new RazorGeneratorEngine().Create<Marker_EdFi_Dashboards_Presentation_Core>();

            // Add the Core Engine to the end of this list as EdFi.Dashboards.Presentation.Web Engine should be first
            ViewEngines.Engines.Add(engine);

            VirtualPathFactoryManager.RegisterVirtualPathFactory(engine);
        }
    }
}
