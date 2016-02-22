using System;
using System.Collections.Generic;
using EdFi.Dashboards.Resource.Models.Common;

namespace EdFi.Dashboards.Resources.Models.Plugins
{
    [Serializable]
    public class PluginMenu
    {
        public PluginMenu()
        {
            ResourceModels = new List<ResourceModel>();
        }

        public string Area { get; set; }
        public List<ResourceModel> ResourceModels { get; set; }
    }
}
