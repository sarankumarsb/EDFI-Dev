using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.WebPages;
using EdFi.Dashboards.Presentation.Web.Providers.Metric;

namespace EdFi.Dashboards.Presentation.Web.Utilities.MetricRendering
{
    public class RazorGeneratorMetricTemplatesPathProvider : MetricTemplateVirtualPathsProviderBase
    {
        protected override IEnumerable<string> GetTemplatesFromVirtualPath(string virtualFolderPath, Func<string, bool> shouldProcessVirtualPath)
        {
            var files = from type in typeof(Marker_EdFi_Dashboards_Presentation_Web).Assembly.GetTypes()
                        where typeof (WebPageRenderingBase).IsAssignableFrom(type)
                        let pageVirtualPath = type.GetCustomAttributes(inherit: false).OfType<PageVirtualPathAttribute>().FirstOrDefault()
                        where pageVirtualPath != null && pageVirtualPath.VirtualPath.StartsWith(virtualFolderPath)
                        select pageVirtualPath.VirtualPath;

            return files;
        }
    }
}
