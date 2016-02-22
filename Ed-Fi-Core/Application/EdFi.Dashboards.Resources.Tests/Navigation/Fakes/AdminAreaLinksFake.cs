// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System.Reflection;
using EdFi.Dashboards.Resources.Navigation;

namespace EdFi.Dashboards.Resources.Tests.Navigation.Fakes
{
    public class AdminAreaLinksFake : SiteAreaFakeBase, IAdminAreaLinks
    {
        public string Default(int localEducationAgencyId, object additionalValues = null)
        {
            return BuildUrl(GetType().Name, additionalValues, MethodBase.GetCurrentMethod(), localEducationAgencyId);
        }

        public string Configuration(int localEducationAgencyId, object additionalValues = null)
        {
            return BuildUrl(GetType().Name, additionalValues, MethodBase.GetCurrentMethod(), localEducationAgencyId);
        }

        public string LogInAs(int localEducationAgencyId, object additionalValues = null)
        {
            return BuildUrl(GetType().Name, additionalValues, MethodBase.GetCurrentMethod(), localEducationAgencyId);
        }

        public string TitleClaimSet(int localEducationAgencyId, object additionalValues = null)
        {
            return BuildUrl(GetType().Name, additionalValues, MethodBase.GetCurrentMethod(), localEducationAgencyId);
        }

        public string ExportTitleClaimSet(int localEducationAgencyId, object additionalValues = null)
        {
            return BuildUrl(GetType().Name, additionalValues, MethodBase.GetCurrentMethod(), localEducationAgencyId);
        }

        public string MetricThreshold(int localEducationAgencyId, object additionalValues = null)
        {
            return BuildUrl(GetType().Name, additionalValues, MethodBase.GetCurrentMethod(), localEducationAgencyId);
        }

        public string PhotoManagement(int localEducationAgencyId, object additionalValues = null)
        {
            return BuildUrl(GetType().Name, additionalValues, MethodBase.GetCurrentMethod(), localEducationAgencyId);
        }
		
		public string Resource(int localEducationAgencyId, string resourceName, object additionalValues = null)
		{
			return BuildUrl(GetType().Name, additionalValues, MethodBase.GetCurrentMethod(), localEducationAgencyId);
		}
    }
}
