using System.Reflection;

namespace EdFi.Dashboards.Presentation.Core.Plugins.Helpers
{
    public static class CassetteHelper
    {
        public static string GetStyleBundleName()
        {
            return Assembly.GetCallingAssembly().GetName().Name + ".Styles";
        }

        public static string GetScriptBundleName()
        {
            return Assembly.GetCallingAssembly().GetName().Name + ".Scripts";
        }
    }
}
