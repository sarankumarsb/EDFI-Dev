using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace EdFi.Dashboards.Resources.Navigation.Mvc.Areas
{
    public class Error : SiteAreaBase, IErrorAreaLinks
    {
        public string Default(string localEducationAgency, object additionalValues = null)
        {
            return NotFound(localEducationAgency, additionalValues);
        }

        public string NotFound(string localEducationAgency, object additionalValues = null)
        {
            if (String.IsNullOrEmpty(localEducationAgency))
                return ("~/NotFound.htm").Resolve();

            return BuildResourceUrl(additionalValues, MethodBase.GetCurrentMethod(), localEducationAgency);
        }

        public virtual string ErrorPage(string localEducationAgency, object additionalValues = null)
        {
            if (String.IsNullOrEmpty(localEducationAgency))
                return BuildUrl("Error_Default", "Error", additionalValues, MethodBase.GetCurrentMethod());

            return BuildResourceUrl(additionalValues, "Error", MethodBase.GetCurrentMethod(), localEducationAgency);
        }
    }
}
