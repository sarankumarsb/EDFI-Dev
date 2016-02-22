// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
namespace EdFi.Dashboards.Resources.Navigation
{
    public interface IStaffAreaLinks 
    {
        // TODO: Deferred - Add optional school name to signatures for optimization of link generation?
        string Default(int schoolId, long staffUSI, string staff = null, long? sectionOrCohortId = null, string studentListType = null, object additionalValues = null);
        string GeneralOverview(int schoolId, long staffUSI, string staff = null, long? sectionOrCohortId = null, string studentListType = null, object additionalValues = null);
        string PriorYear(int schoolId, long staffUSI, string staff = null, long? sectionOrCohortId = null, string studentListType = null, object additionalValues = null);
        string SubjectSpecificOverview(int schoolId, long staffUSI, string staff = null, long? sectionOrCohortId = null, string studentListType = null, object additionalValues = null);
        string AssessmentDetails(int schoolId, long staffUSI, string staff = null, long? sectionOrCohortId = null, string studentListType = null, string subjectArea = null, string assessmentSubType = null, object additionalValues = null);
        string ProfileThumbnail(int schoolId, long staffUSI, string gender);
        string Image(int schoolId, long staffUSI, string gender);
        string ListImage(int schoolId, long staffUSI, string gender);
        string ExportAllMetrics(int schoolId, long staffUSI, string staff = null, long? sectionOrCohortId = null, string studentListType = null, string subjectArea = null, string assessmentSubType = null, object additionalValues = null);
        string Resource(int schoolId, long staffUSI, string resourceName, object additionalValues = null);
        string CustomStudentList(int schoolId, long staffUSI, string staff = null, object additionalValues = null);
        string LocalEducationAgencyCustomStudentList(int localEducationAgencyId, long staffUSI, string staff = null, object additionalValues = null);
        string CustomMetricsBasedWatchList(int schoolId, long staffUSI, string resourceName, int? metricsBasedWatchListId = null, string staff = null, string studentListType = null, object additionalValues = null);
    }
}