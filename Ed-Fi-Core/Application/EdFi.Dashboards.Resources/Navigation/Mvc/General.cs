using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace EdFi.Dashboards.Resources.Navigation.Mvc
{
    public class General : SiteAreaBase, IGeneralLinks
    {
        public string Logout()
        {
            return BuildUrl("Default", null, MethodBase.GetCurrentMethod());
        }

        public string MetricsBasedWatchList(string resourceName, int? id = null, object additionalValues = null)
        {
            return BuildUrl("Default", resourceName, additionalValues, MethodBase.GetCurrentMethod(), resourceName, id);
        }

	    public string Resource(string resourceName, object additionalValues = null)
	    {
		    return BuildUrl("Default", resourceName, additionalValues, MethodBase.GetCurrentMethod(), resourceName);
	    }
    }
}
