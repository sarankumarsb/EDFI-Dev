using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using EdFi.Dashboards.Resource.Models.Common;

namespace EdFi.Dashboards.Resources.Plugins
{
    public interface IPluginResourcesProvider
    {
        IEnumerable<ResourceModel> Get(string areaName);
    }

    public class PluginResourcesProvider : IPluginResourcesProvider
    {
        public IPluginManifest[] PluginManifests { get; set; }

        public IEnumerable<ResourceModel> Get(string areaName)
        {
            var resources = new List<ResourceModel>();

            if (PluginManifests == null)
                return resources;

            foreach (var pluginManifest in PluginManifests)
            {
	            if (pluginManifest.PluginMenus == null)
		            continue;

	            var menuItemsForCurrentArea = pluginManifest.PluginMenus
					.Where(x => String.Equals(x.Area, areaName, StringComparison.CurrentCultureIgnoreCase))
		            .Where(x => x.ResourceModels != null)
		            .SelectMany(x => x.ResourceModels);

				if (menuItemsForCurrentArea != null)
					resources.AddRange(menuItemsForCurrentArea);
            }

            return resources;
        }
    }
}
