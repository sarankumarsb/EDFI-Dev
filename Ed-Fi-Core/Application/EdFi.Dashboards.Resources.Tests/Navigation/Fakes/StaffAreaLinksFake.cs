// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System.Reflection;
using EdFi.Dashboards.Resources.Navigation;

namespace EdFi.Dashboards.Resources.Tests.Navigation.Fakes
{
    public class StaffAreaLinksFake : SiteAreaFakeBase, IStaffAreaLinks
    {
        public string Default(int schoolId, long staffUSI, string staff = null, long? sectionOrCohortId = null, string studentListType = null, object additionalValues = null)
        {
            return BuildUrl(GetType().Name, additionalValues, MethodBase.GetCurrentMethod(), schoolId, staffUSI, staff, sectionOrCohortId, studentListType);
        }

        public string GeneralOverview(int schoolId, long staffUSI, string staff = null, long? sectionOrCohortId = null, string studentListType = null, object additionalValues = null)
        {
            return BuildUrl(GetType().Name, additionalValues, MethodBase.GetCurrentMethod(), schoolId, staffUSI, staff, sectionOrCohortId, studentListType);
        }

        public string PriorYear(int schoolId, long staffUSI, string staff = null, long? sectionOrCohortId = null, string studentListType = null, object additionalValues = null)
        {
            return BuildUrl(GetType().Name, additionalValues, MethodBase.GetCurrentMethod(), schoolId, staffUSI, staff, sectionOrCohortId, studentListType);
        }

        public string SubjectSpecificOverview(int schoolId, long staffUSI, string staff = null, long? sectionOrCohortId = null, string studentListType = null, object additionalValues = null)
        {
            return BuildUrl(GetType().Name, additionalValues, MethodBase.GetCurrentMethod(), schoolId, staffUSI, staff, sectionOrCohortId, studentListType);
        }

        public string AssessmentDetails(int schoolId, long staffUSI, string staff = null, long? sectionOrCohortId = null, string studentListType = null, string subjectArea = null, string assessmentSubType = null, object additionalValues = null)
        {
            return BuildUrl(GetType().Name, additionalValues, MethodBase.GetCurrentMethod(), schoolId, staffUSI, staff, sectionOrCohortId, studentListType, subjectArea, assessmentSubType);
        }

        public string ProfileThumbnail(int schoolId, long staffUSI, string gender)
        {
            return BuildUrl(GetType().Name, null, MethodBase.GetCurrentMethod(), schoolId, staffUSI, gender);
        }

        public string Image(int schoolId, long staffUSI, string gender)
        {
            return BuildUrl(GetType().Name, null, MethodBase.GetCurrentMethod(), schoolId, staffUSI, gender);
        }

        public string ListImage(int schoolId, long staffUSI, string gender)
        {
            return BuildUrl(GetType().Name, null, MethodBase.GetCurrentMethod(), schoolId, staffUSI, gender);
        }

        public virtual string ExportAllMetrics(int schoolId, long staffUSI, string staff = null, long? sectionOrCohortId = null, string studentListType = null, string subjectArea = null, string assessmentSubType = null, object additionalValues = null)
        {
            return BuildUrl(GetType().Name, additionalValues, MethodBase.GetCurrentMethod(), schoolId, staffUSI, staff, sectionOrCohortId, studentListType, additionalValues);
        }

        public string Resource(int schoolId, long staffUSI, string resourceName, object additionalValues = null)
        {
            return BuildUrl(GetType().Name, additionalValues, MethodBase.GetCurrentMethod(), schoolId, staffUSI, additionalValues);
        }

        public string CustomStudentList(int schoolId, long staffUSI, string staff = null, object additionalValues = null)
        {
            return BuildUrl(GetType().Name, additionalValues, MethodBase.GetCurrentMethod(), schoolId, staffUSI, additionalValues);
        }

        public string LocalEducationAgencyCustomStudentList(int localEducationAgencyId, long staffUSI, string staff = null, object additionalValues = null)
        {
            return BuildUrl(GetType().Name, additionalValues, MethodBase.GetCurrentMethod(), localEducationAgencyId, staffUSI, additionalValues);
        }

        public string MetricsBasedWatchList(int schoolId, long staffUSI, string resourceName, int? metricsBasedWatchListId = null, string staff = null,
            object additionalValues = null)
        {
            return BuildUrl(GetType().Name, resourceName, additionalValues, MethodBase.GetCurrentMethod(), schoolId, staffUSI, staff,
                metricsBasedWatchListId);
        }

        public string CustomMetricsBasedWatchList(int schoolId, long staffUSI, string resourceName, int? metricsBasedWatchListId = null, string staff = null, string studentListType = null, object additionalValues = null)
        {
            return BuildUrl(GetType().Name, resourceName, additionalValues, MethodBase.GetCurrentMethod(), schoolId,
                staffUSI, resourceName, metricsBasedWatchListId, staff, studentListType);
        }
    }
}
