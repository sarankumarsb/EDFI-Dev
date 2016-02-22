// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System;
using System.IO;
using System.Web;

namespace EdFi.Dashboards.Presentation.Web.Providers.Metric
{
    public interface IMetricTemplateConventionsProvider {
        string TemplatesVirtualPath { get; }
        string[] FileExtensions { get; }
        bool IsSharedFolderLocation(string virtualFolderPath);
    }

    public class RazorMetricTemplateConventionsProvider : IMetricTemplateConventionsProvider
    {
        //private readonly string _extensionsTemplateVirtualPath = "~/Extensions/Views/MetricTemplates/";
            //= HttpContext.Current.Server.MapPath("~/Extensions/Views/MetricTemplates/");

        private readonly string _templatesVirtualPath = "~/Views/MetricTemplates/";
            //= HttpContext.Current.Server.MapPath("~/Views/MetricTemplates/");

        public string TemplatesVirtualPath
        {
            get { return _templatesVirtualPath; }
        }

        //protected string TemplateExtensionsFilePath
        //{
        //    get { return _extensionsTemplateVirtualPath; }
        //}

        // For additional performance gains, only look for CSharp Razor views...
        private string[] _fileExtensions = new[] { "cshtml" };  // new[] { "cshtml", "vbhtml" };

        public string[] FileExtensions
        {
            get { return _fileExtensions; } 
        }

        public bool IsSharedFolderLocation(string virtualFolderPath)
        {
            if (virtualFolderPath == null)
                throw new ArgumentNullException("virtualFolderPath");

            return virtualFolderPath.Equals(TemplatesVirtualPath, StringComparison.OrdinalIgnoreCase)  // Templates in root metric templates folder are considered "shared"
                //|| virtualFolderPath.Equals(TemplateExtensionsFilePath, StringComparison.OrdinalIgnoreCase)
                   || Path.GetFileNameWithoutExtension(virtualFolderPath).Equals("Shared", StringComparison.OrdinalIgnoreCase);
        }
    }
}