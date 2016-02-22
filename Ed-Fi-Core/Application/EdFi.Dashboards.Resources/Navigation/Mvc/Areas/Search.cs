// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System.Reflection;

namespace EdFi.Dashboards.Resources.Navigation.Mvc.Areas
{
    public class Search : SiteAreaBase, ISearchAreaLinks
    {
        /// <summary>
        /// Gets the URL for the Search Results page.
        /// </summary>
        public virtual string Results(int localEducationAgencyId, object additionalValues = null)
        {
            return BuildResourceUrl(additionalValues, MethodBase.GetCurrentMethod(), localEducationAgencyId);
        }

        public virtual string QuickSearchWebService(int localEducationAgencyId, int? schoolId, object additionalValues = null)
        {
            if (schoolId == null)
                return BuildResourceUrl(additionalValues, "QuickSearch", MethodBase.GetCurrentMethod(), localEducationAgencyId);
            return BuildResourceUrl(additionalValues, "QuickSearch", MethodBase.GetCurrentMethod(), localEducationAgencyId, schoolId);
            //return "~/Search/QuickSearch?LocalEducationAgencyId=" + localEducationAgencyId;
        }
        public virtual string SearchWebService(int localEducationAgencyId, int? schoolId, object additionalValues = null)
        {
            if (schoolId == null)
                return BuildResourceUrl(additionalValues, "Search", MethodBase.GetCurrentMethod(), localEducationAgencyId);
            return BuildResourceUrl(additionalValues, "Search", MethodBase.GetCurrentMethod(), localEducationAgencyId, schoolId);
            //return "~/Search/Search?LocalEducationAgencyId=" + localEducationAgencyId;
        }

        public virtual string LogUserActionWebService(int localEducationAgencyId, int? schoolId, object additionalValues = null)
        {
            if (schoolId == null)
                return BuildResourceUrl(additionalValues, "LogUserAction", MethodBase.GetCurrentMethod(), localEducationAgencyId);
            return BuildResourceUrl(additionalValues, "LogUserAction", MethodBase.GetCurrentMethod(), localEducationAgencyId, schoolId);
            //return "~/Search/LogUserAction";
        }
        public virtual string LogUserSortingActionWebService(int localEducationAgencyId, int? schoolId, object additionalValues = null)
        {
            if (schoolId == null)
                return BuildResourceUrl(additionalValues, "LogUserSortingAction", MethodBase.GetCurrentMethod(), localEducationAgencyId);
            return BuildResourceUrl(additionalValues, "LogUserSortingAction", MethodBase.GetCurrentMethod(), localEducationAgencyId, schoolId);
            //return "~/Search/LogUserSortingAction?LocalEducationAgencyId=" + localEducationAgencyId;
        }
    }
}
