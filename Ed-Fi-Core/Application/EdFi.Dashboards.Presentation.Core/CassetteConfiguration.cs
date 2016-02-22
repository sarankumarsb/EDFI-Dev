using System.Text.RegularExpressions;
using Cassette;
using Cassette.HtmlTemplates;
using Cassette.Scripts;
using Cassette.Stylesheets;
using EdFi.Dashboards.Common.Utilities;

namespace EdFi.Dashboards.Presentation.Core
{
    /// <summary>
    /// Configures the Cassette asset bundles for the web application.
    /// </summary>
    public class CassetteBundleConfiguration : IConfiguration<BundleCollection>
    {
        public void Configure(BundleCollection bundles)
        {
            // TODO: Configure your bundles here...
            // Please read http://getcassette.net/documentation/configuration

            // This default configuration treats each file as a separate 'bundle'.
            // In production the content will be minified, but the files are not combined.
            // So you probably want to tweak these defaults!
            //bundles.AddPerIndividualFile<StylesheetBundle>("Content");
            //bundles.AddPerIndividualFile<ScriptBundle>("Scripts");
            BundleScripts(bundles);
            BundleStyleSheets(bundles);
            BundleHtmlTemplates(bundles);
            BundlePluginResources(bundles);

            // To combine files, try something like this instead:
            //   bundles.Add<StylesheetBundle>("Content");
            // In production mode, all of ~/Content will be combined into a single bundle.

            // If you want a bundle per folder, try this:
            //   bundles.AddPerSubDirectory<ScriptBundle>("Scripts");
            // Each immediate sub-directory of ~/Scripts will be combined into its own bundle.
            // This is useful when there are lots of scripts for different areas of the website.
        }

        protected virtual void BundleScripts(BundleCollection bundles)
        {
            bundles.AddPerSubDirectory<ScriptBundle>("Core_Content/Scripts");
        }

        protected virtual void BundleStyleSheets(BundleCollection bundles)
        {
            bundles.AddPerSubDirectory<StylesheetBundle>("App_Themes");
            bundles.Add<StylesheetBundle>("Core_Content/StyleSheets", new FileSearch()
            {
                Exclude = new Regex(@"[/\\]{0,1}_[^/\\]*[\.]{1}"),
                Pattern = "*.scss"
            });
        }

        private void BundlePluginResources(BundleCollection bundles)
        {
            //Cassette uses TinyIoC so we can't use contructor injection and we are better off using the IoC.ResolveAll to get an array of registered Cassette Configutations.
            var cassetteConfugurations = IoC.ResolveAll<IConfiguration<BundleCollection>>();
            foreach (var cassetteConfuguration in cassetteConfugurations)
                cassetteConfuguration.Configure(bundles);
        }

        protected virtual void BundleHtmlTemplates(BundleCollection bundles)
        {
            bundles.AddPerSubDirectory<HtmlTemplateBundle>("Core_Content/HtmlTemplates");
        }
    }
}