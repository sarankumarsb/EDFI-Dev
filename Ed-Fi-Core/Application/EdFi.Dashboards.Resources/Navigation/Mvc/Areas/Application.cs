using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace EdFi.Dashboards.Resources.Navigation.Mvc.Areas
{
    public class Application : SiteAreaBase, IApplicationAreaLinks
    {
        public virtual string Feedback(int? localEducationAgencyId, object additionalValues = null)
        {
            if (!localEducationAgencyId.HasValue)
                return BuildUrl("Application_Default", "Feedback", additionalValues, MethodBase.GetCurrentMethod());

            return BuildResourceUrl(additionalValues, MethodBase.GetCurrentMethod(), localEducationAgencyId);
        }
    }
}
