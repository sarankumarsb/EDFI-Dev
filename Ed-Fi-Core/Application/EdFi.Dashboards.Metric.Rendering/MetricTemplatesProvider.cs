// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace EdFi.Dashboards.Metric.Rendering
{
    public interface IMetricTemplatesProvider
    {
        /// <summary>
        /// Get all the templates for applying to the metrics.
        /// </summary>
        /// <returns></returns>
        IEnumerable<MetricTemplateMetadata> GetMetricTemplatesMetadata();

        /// <summary>
        /// Gets the physical file paths (by file name), for shared metric template files.
        /// </summary>
        /// <returns>A dictionary of physical file paths keyed by filename without extension.</returns>
        IDictionary<string, string> GetSharedFiles();

        ///// <summary>
        ///// Gets the template to be used between groups of metrics.
        ///// </summary>
        ///// <returns></returns>
        //string GetMetricGroupDividerTemplate();

        ///// <summary>
        ///// Gets the name of the template to be used to hold dynamic content client-side.
        ///// </summary>
        ///// <returns></returns>
        //string GetDynamicContentContainerTemplate();

        ITemplateLookup GetGroupedMetricTemplatesMetadata();
    }

    public interface ITemplateLookup
    {
        MetricTemplateMetadata[] this[string key] { get; set; }
    }
}
