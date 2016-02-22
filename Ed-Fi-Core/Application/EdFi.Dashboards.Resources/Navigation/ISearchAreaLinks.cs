// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
namespace EdFi.Dashboards.Resources.Navigation
{
    public interface ISearchAreaLinks 
    {
        /// <summary>
        /// Gets the URL for the Search Results page.
        /// </summary>
        string Results(int localEducationAgencyId, object additionalValues = null);

        string QuickSearchWebService(int localEducationAgencyId, int? schoolId, object additionalValues = null);
        string SearchWebService(int localEducationAgencyId, int? schoolId, object additionalValues = null);
        string LogUserActionWebService(int localEducationAgencyId, int? schoolId, object additionalValues = null);
        string LogUserSortingActionWebService(int localEducationAgencyId, int? schoolId, object additionalValues = null);
    }
}