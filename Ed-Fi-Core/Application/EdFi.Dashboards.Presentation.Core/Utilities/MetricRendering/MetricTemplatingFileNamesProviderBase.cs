// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.WebPages;
using EdFi.Dashboards.Presentation.Core;

namespace EdFi.Dashboards.Presentation.Web.Providers.Metric
{
    public abstract class MetricTemplateVirtualPathsProviderBase : IMetricTemplateVirtualPathsProvider
    {
        public IMetricTemplateConventionsProvider MetricTemplateConventionsProvider { get; set; }

        public IEnumerable<string> GetMetricTemplates()
        {
            return GetTemplatePaths(d => !MetricTemplateConventionsProvider.IsSharedFolderLocation(d));
        }

        public IEnumerable<string> GetSharedTemplates()
        {
            return GetTemplatePaths(MetricTemplateConventionsProvider.IsSharedFolderLocation);
        }

        private IEnumerable<string> GetTemplatePaths(Func<string, bool> shouldProcessFolder)
        {
            return GetTemplatesFromVirtualPath(MetricTemplateConventionsProvider.TemplatesVirtualPath, shouldProcessFolder);
        }

        protected abstract IEnumerable<string> GetTemplatesFromVirtualPath(string virtualFolderPath, Func<string, bool> shouldProcessVirtualPath);
    }

    public class RazorGeneratorMetricTemplatesPathProvider : MetricTemplateVirtualPathsProviderBase
    {
        protected override IEnumerable<string> GetTemplatesFromVirtualPath(string virtualFolderPath, Func<string, bool> shouldProcessVirtualPath)
        {
            var files = from type in typeof(Marker_EdFi_Dashboards_Presentation_Core).Assembly.GetTypes()
                        where typeof(WebPageRenderingBase).IsAssignableFrom(type)
                        let pageVirtualPath = type.GetCustomAttributes(inherit: false).OfType<PageVirtualPathAttribute>().FirstOrDefault()
                        where pageVirtualPath != null && pageVirtualPath.VirtualPath.StartsWith(virtualFolderPath)
                        select pageVirtualPath.VirtualPath;

            return files;
        }
    }
}
