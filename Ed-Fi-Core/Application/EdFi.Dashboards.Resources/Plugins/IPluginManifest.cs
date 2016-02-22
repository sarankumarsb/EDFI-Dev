using System.Collections.Generic;
using EdFi.Dashboards.Resources.Models.Plugins;

namespace EdFi.Dashboards.Resources.Plugins
{
    public interface IPluginManifest
    {
        string Name { get; }
        string Version { get; }
        IEnumerable<PluginMenu> PluginMenus { get; }
    }
}
