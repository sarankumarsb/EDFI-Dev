using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using EdFi.Dashboards.Resources.Navigation;

namespace EdFi.Dashboards.Resources.Tests.Navigation.Fakes
{
    public class ApplicationAreaLinksFake : SiteAreaFakeBase, IApplicationAreaLinks
    {
        public string Feedback(int? localEducationAgencyId, object additionalValues = null)
        {
            return BuildUrl(GetType().Name, additionalValues, MethodBase.GetCurrentMethod(), localEducationAgencyId);
        }
    }
}
