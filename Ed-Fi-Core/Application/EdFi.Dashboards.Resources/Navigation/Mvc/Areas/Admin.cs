// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System.Reflection;

namespace EdFi.Dashboards.Resources.Navigation.Mvc.Areas
{
    public class Admin : SiteAreaBase, IAdminAreaLinks
    {
        public virtual string Default(int localEducationAgencyId, object additionalValues = null)
        {
            return Configuration(localEducationAgencyId, additionalValues);
        }

        public string Configuration(int localEducationAgencyId, object additionalValues = null)
        {
            return BuildResourceUrl(additionalValues, MethodBase.GetCurrentMethod(), localEducationAgencyId);
        }

        public string LogInAs(int localEducationAgencyId, object additionalValues = null)
        {
            return BuildResourceUrl(additionalValues, MethodBase.GetCurrentMethod(), localEducationAgencyId);
        }

        public string TitleClaimSet(int localEducationAgencyId, object additionalValues = null)
        {
            return BuildResourceUrl(additionalValues, MethodBase.GetCurrentMethod(), localEducationAgencyId);
        }

        public string ExportTitleClaimSet(int localEducationAgencyId, object additionalValues = null)
        {
            return BuildResourceUrl(additionalValues, MethodBase.GetCurrentMethod(), localEducationAgencyId);
        }

        public string MetricThreshold(int localEducationAgencyId, object additionalValues = null)
        {
            return BuildResourceUrl(additionalValues, MethodBase.GetCurrentMethod(), localEducationAgencyId);
        }

        public string PhotoManagement(int localEducationAgencyId, object additionalValues = null)
        {
            return BuildResourceUrl(additionalValues, MethodBase.GetCurrentMethod(), localEducationAgencyId);
        }

        public string Resource(int localEducationAgencyId, string resourceName, object additionalValues = null)
        {
            return BuildResourceUrl(additionalValues, resourceName, MethodBase.GetCurrentMethod(), localEducationAgencyId);
        }
    }
}
