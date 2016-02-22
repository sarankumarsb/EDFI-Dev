// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System.Collections.Generic;

namespace EdFi.Dashboards.Presentation.Web.Providers.Metric
{
    /// <summary>
    /// Provides methods for obtaining the physical paths for templates and other shared files used for metric template rendering.
    /// </summary>
    public interface IMetricTemplateVirtualPathsProvider
    {
        ///// <summary>
        ///// Gets the base path for the metric templates.
        ///// </summary>
        ///// <returns>A string containing the physical folder path.</returns>
        //string GetMetricTemplatesBasePath();

        ///// <summary>
        ///// Gets the base path for the metric template extensions.
        ///// </summary>
        ///// <returns>A string containing the physical folder path to the extensions.</returns>
        //string GetMetricTemplateExtensionsBasePath();

        /// <summary>
        /// Gets the physical file paths of all the metric templates found.
        /// </summary>
        /// <returns>An array of physical filenames.</returns>
        IEnumerable<string> GetMetricTemplates();

        /// <summary>
        /// Gets the physical file paths of all the shared files found (i.e. controls, partial views, etc.).
        /// </summary>
        /// <returns>An array of physical filenames.</returns>
        IEnumerable<string> GetSharedTemplates();
    }
}