using System.IO;
using System.Text.RegularExpressions;
using Cassette;
using Cassette.HtmlTemplates;
using Cassette.Scripts;
using Cassette.Stylesheets;

namespace EdFi.Dashboards.Presentation.Web
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
            
        }

        protected virtual void BundleStyleSheets(BundleCollection bundles)
        {
            bundles.Add<StylesheetBundle>("jquery-ui", "App_Themes/Theme1/jQueryLightness/jquery-ui.custom.css");
            bundles.AddPerSubDirectory<StylesheetBundle>("Content/StyleSheets");
        }
    }
}