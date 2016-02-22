// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using EdFi.Dashboards.Common;
using EdFi.Dashboards.Metric.Rendering;

namespace EdFi.Dashboards.Presentation.Web.Providers.Metric
{
    public class MetricTemplatesProvider : IMetricTemplatesProvider
    {
        private readonly IMetricTemplateVirtualPathsProvider[] templateVirtualPathProviders;
        private readonly IMetricTemplateConventionsProvider metricConventionsProvider;

        public MetricTemplatesProvider(IMetricTemplateVirtualPathsProvider[] templateVirtualPathProviders,
            IMetricTemplateConventionsProvider metricConventionsProvider)
        {
            this.templateVirtualPathProviders = templateVirtualPathProviders;
            this.metricConventionsProvider = metricConventionsProvider;
            metricTemplates = new Lazy<TemplateLookup>(InitTemplatesMetadata);
        }

        //lazy but thread-safe initialization
        private readonly Lazy<TemplateLookup> metricTemplates;

        private TemplateLookup InitTemplatesMetadata()
        {
            var templates = new List<MetricTemplateMetadata>();

            var metricTemplateVirtualPaths = new HashSet<string>();

            // Collect all the shared metric template virtual paths from the providers
            foreach (var virtualPathProvider in templateVirtualPathProviders)
            {
                var templateFiles = virtualPathProvider.GetMetricTemplates();
                metricTemplateVirtualPaths.AddRange(templateFiles);
            }

            // Create the metadata for each template
            templates.AddRange(
                (from p in metricTemplateVirtualPaths
                    orderby p
                    let tokens = GetTokens(p)
                    select new MetricTemplateMetadata
                            {
                                Name = Path.GetFileNameWithoutExtension(p),
                                RelativePath =
                                    GetRelativePath(p, metricConventionsProvider.TemplatesVirtualPath),
                                FullPath = p,
                                Tokens = tokens,
                                TemplateGroupingId = MetricTemplateMetadata.GetTemplateGroupingId(tokens)
                            }));

            var lookupData = templates
                .GroupBy(x => x.TemplateGroupingId)
                .ToDictionary(g => g.Key, 
                              g => g.Select(x => x).ToArray());

            return new TemplateLookup(lookupData);
        }

        public IEnumerable<MetricTemplateMetadata> GetMetricTemplatesMetadata()
        {
            return metricTemplates.Value.SelectMany(x => x.Value);
        }

        public ITemplateLookup GetGroupedMetricTemplatesMetadata()
        {
            return metricTemplates.Value;
        }

        private string GetRelativePath(string filePath, params string[] possibleBasePaths)
        {
            foreach (string possibleBasePath in possibleBasePaths)
            {
                if (!filePath.StartsWith(possibleBasePath, StringComparison.OrdinalIgnoreCase))
                    continue;

                string basePath = possibleBasePath;

                if (possibleBasePath.Last() != '/')
                    basePath += @"/";

                return filePath.Replace(basePath, string.Empty);
            }

            // This should never happen
            throw new Exception(
                string.Format(
                    "The template file '{0}' is not from the same tree as any of the specified template base paths: {1}.",
                    filePath, string.Join(", ", possibleBasePaths)));
        }

        private IDictionary<string, string> _sharedFiles;

        public IDictionary<string, string> GetSharedFiles()
        {
            if (_sharedFiles == null)
            {
                _sharedFiles = new Dictionary<string, string>();

                var sharedTemplateVirtualPaths = new HashSet<string>();

                // Collect all the shared metric template virtual paths from the providers
                foreach (var virtualPathProvider in templateVirtualPathProviders)
                {
                    var allSharedFiles = virtualPathProvider.GetSharedTemplates();
                    sharedTemplateVirtualPaths.AddRange(allSharedFiles);
                }

                var sharedFiles =
                    from p in sharedTemplateVirtualPaths
                    orderby p
                    select new
                    {
                        Key = Path.GetFileNameWithoutExtension(p),
                        Value = p
                    };

                foreach (var sharedFile in sharedFiles)
                    _sharedFiles.Add(sharedFile.Key, sharedFile.Value);
            }

            return _sharedFiles;
        }

        private Dictionary<string, string> GetTokens(string fileName)
        {
            if (fileName == null)
                throw new ArgumentNullException("fileName");

            var tokenValuesByName = new Dictionary<string, string>();

            string templatesBasePath = metricConventionsProvider.TemplatesVirtualPath;

            // Make sure the base path contains the trailing slash
            if (!templatesBasePath.EndsWith(@"/"))
                templatesBasePath += @"/";

            // Strip base path off the file path
            if (!fileName.StartsWith(templatesBasePath)) // && !fileName.StartsWith(templateExtensionsBasePath))
                throw new ArgumentException(
                    string.Format("The template file '{0}' is not from the same tree as the specified templates base path of '{1}'.", //", or the extensions template base path of '{2}'.",
                        fileName, templatesBasePath), fileName);
                        //fileName, templatesBasePath, templateExtensionsBasePath), fileName);

            // Remove the extension
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
            
            // Get the relative path
            string relativePathOnly =
                VirtualPathUtility.GetDirectory(fileName)
                    .Replace(templatesBasePath, string.Empty)
                    .TrimEnd('/');

            // Break path down into it's folders
            string[] pathParts = relativePathOnly.Split('/');

            if (pathParts.Length > 2)
                throw new ArgumentException(
                    string.Format("'{0}' is located deeper in the directory structure than is allowed by convention.", relativePathOnly));

            // Return no tokens when NoTemplate.cshtml and others without 2 path parts
            if(pathParts.Length < 2)
                return tokenValuesByName;

            //Get domain entity type
            tokenValuesByName.Add("MetricInstanceSetType", pathParts[0]);

            //Get the rendering mode
            tokenValuesByName.Add("RenderingMode", pathParts[1]);
            
            //turn the rest into an array
            var tokens = fileNameWithoutExtension.Split('.');

            //MetricType by convention should always be next
            tokenValuesByName.Add("MetricType", tokens[0]);

            if (tokens.Length <= 1)
                return tokenValuesByName;

            var i = 1;
            if (tokens[1].Equals("Close", StringComparison.OrdinalIgnoreCase))
            {
                tokenValuesByName.Add("Open", "false");
                i = 2;
            }
            else if (tokens[1].Equals("Open", StringComparison.OrdinalIgnoreCase))
            {
                tokenValuesByName.Add("Open", "true");
                i = 2;
            }
            else
            {
                tokenValuesByName.Add("Open", "true");
            }

            //get the rest of the unordered rendering context values
            for (; i < tokens.Length; i++)
            {
                if (tokens[i].Contains('_'))
                    tokenValuesByName.Add(tokens[i].Substring(0, tokens[i].IndexOf('_')), tokens[i].Substring(tokens[i].IndexOf('_') + 1));
                else
                {
                    if (tokens[i].Contains("Level"))
                        tokenValuesByName.Add("Depth", tokens[i]);
                    else if (tokens[i].Equals("Disabled", StringComparison.OrdinalIgnoreCase))
                        tokenValuesByName.Add("Enabled",  "false");
                    else if (tokens[i].Equals("NullValue", StringComparison.OrdinalIgnoreCase))
                        tokenValuesByName.Add("NullValue", "true");
                }
            }

            return tokenValuesByName;
        }
    }

    public class TemplateLookup : Dictionary<string, MetricTemplateMetadata[]>, ITemplateLookup
    {
        public TemplateLookup(IDictionary<string, MetricTemplateMetadata[]> dictionary) : base(dictionary)
        {}
    }

   
}