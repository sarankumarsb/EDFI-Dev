using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Cassette;
using Cassette.Scripts;
using Cassette.Stylesheets;

namespace EdFi.Dashboards.Presentation.Core.Plugins.Utilities
{
    public class CassetteDefaultConventionBundleConfiguration<TMarker> : IConfiguration<BundleCollection>
    {
        public virtual void Configure(BundleCollection bundles)
        {
            // TODO: Configure your bundles here...
            // Please read http://getcassette.net/documentation/configuration

            // This default configuration treats each file as a separate 'bundle'.
            // In production the content will be minified, but the files are not combined.
            // So you probably want to tweak these defaults!


            //Add Embeded resources for the plugin.
            var pluginAssembly = typeof(TMarker).Assembly;
            var embeddedResourceNames = pluginAssembly.GetManifestResourceNames();

            var baseNamespace = pluginAssembly.GetName().Name;
            var embeddedStyleResources = embeddedResourceNames.Where(x => x.EndsWith(".css", StringComparison.InvariantCultureIgnoreCase)).Select(x => x.Replace(baseNamespace + ".", string.Empty)).ToArray();
            var embeddedJsResources = embeddedResourceNames.Where(x => x.EndsWith(".js", StringComparison.InvariantCultureIgnoreCase)).Select(x => x.Replace(baseNamespace + ".", string.Empty)).ToArray();

            if (embeddedStyleResources.Any())
                bundles.AddEmbeddedResources<StylesheetBundle>(baseNamespace + ".Styles", pluginAssembly, baseNamespace, embeddedStyleResources);

            if (embeddedJsResources.Any())
                bundles.AddEmbeddedResources<ScriptBundle>(baseNamespace + ".Scripts", pluginAssembly, baseNamespace, embeddedJsResources);


            // To combine files, try something like this instead:
            //   bundles.Add<StylesheetBundle>("Content");
            // In production mode, all of ~/Content will be combined into a single bundle.

            // If you want a bundle per folder, try this:
            //   bundles.AddPerSubDirectory<ScriptBundle>("Scripts");
            // Each immediate sub-directory of ~/Scripts will be combined into its own bundle.
            // This is useful when there are lots of scripts for different areas of the website.
        }
    }
}
