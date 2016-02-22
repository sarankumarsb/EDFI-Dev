// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
namespace EdFi.Dashboards.Resources.Navigation
{
    public interface ILocalEducationAgencyAreaLinks
    {
        string Default(int localEducationAgencyId, object additionalValues = null);
        string Entry(string localEducationAgency, object additionalValues = null);
        string Home(string localEducationAgency, object additionalValues = null);
        string SchoolCategoryList(int localEducationAgencyId, object additionalValues = null);
        string Information(int localEducationAgencyId, object additionalValues = null);
        string Overview(int localEducationAgencyId, object additionalValues = null);
        string Metrics(int localEducationAgencyId, int metricVariantId, object additionalValues = null);
        string MetricsDrilldown(int localEducationAgencyId, int metricVariantId, string drilldownName, object additionalValues = null);
        string OperationalDashboard(int localEducationAgencyId, object additionalValues = null);
        string GoalPlanning(int localEducationAgencyId, int metricVariantId, object additionalValues = null);
        string PublishGoals(int localEducationAgencyId, object additionalValues = null);
        string ExportAllMetrics(int localEducationAgencyId, object additionalValues = null);
        string ExportMetricList(int localEducationAgencyId, int metricVariantId, object additionalValues = null);
        string ExportStudentDemographicList(int localEducationAgencyId, string demographic, object additionalValues = null);
        string ExportStudentList(int localEducationAgencyId, long staffUSI, string staff = null, long? sectionOrCohortId = null, string studentListType = null, object additionalValues = null);
        string ExportStudentSchoolCategoryList(int localEducationAgencyId, string schoolCategory, object additionalValues = null);
        string StudentList(int localEducationAgencyId, long staffUSI, string staff = null, long? sectionOrCohortId = null, string studentListType = null, object additionalValues = null);
        string StudentDemographicList(int localEducationAgencyId, string demographic = null, object additionalValues = null);
        string StudentSchoolCategoryList(int localEducationAgencyId, string schoolCategory = null, object additionalValues = null);
        string ProfileThumbnail(int localEducationAgencyId);
        string Image(int localEducationAgencyId);
        string ListImage(int localEducationAgencyId);
        string Resource(int localEducationAgencyId, string resourceName, object additionalValues = null);
        string ApiResource(int localEducationAgencyId, object resourceIdentifier, string resourceName);
        string ApiResource(int localEducationAgencyId, object resourceIdentifier, string resourceName, object additionalValues);
        string CustomMetricsBasedWatchListDemographic(int localEducationAgencyId, long? staffUSI, string resourceName, string demographic, int? sectionOrCohortId = null, string studentListType = null, object additionalValues = null);
        string CustomMetricsBasedWatchListSchoolCategory(int localEducationAgencyId, long? staffUSI, string resourceName, string schoolCategory, int? sectionOrCohortId = null, string studentListType = null, object additionalValues = null);
    }
}