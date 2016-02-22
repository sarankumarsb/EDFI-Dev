// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
namespace EdFi.Dashboards.Resources.Navigation
{
    public interface ISchoolAreaLinks 
    {
        string Default(int schoolId, string school = null, object additionalValues = null);
        string Entry(string localEducationAgency, int schoolId, string school = null, object additionalValues = null);
        string Information(int schoolId, string school = null, object additionalValues = null);
        string Overview(int schoolId, string school = null, object additionalValues = null);
        string StudentsByGrade(int schoolId, string school = null, object additionalValues = null);
        string Teachers(int schoolId, string school = null, object additionalValues = null);
        string Staff(int schoolId, string school = null, object additionalValues = null);
        string Metrics(int schoolId, int metricVariantId, string school = null, object additionalValues = null);
        string MetricsDrilldown(int schoolId, int metricVariantId, string drilldownName, string school = null, object additionalValues = null);
        string OperationalDashboard(int schoolId, string school = null, object additionalValues = null);
        string GoalPlanning(int schoolId, int metricVariantId, string school = null, object additionalValues = null);
        string PublishGoals(int schoolId, string school = null, object additionalValues = null);
        string ProfileThumbnail(int schoolId);
        string Image(int schoolId);
        string ListImage(int schoolId);
        string ExportAllMetrics(int schoolId, object additionalValues = null);
        string ExportMetricList(int schoolId, int metricVariantId, object additionalValues = null);
        string ExportStudentDemographicList(int schoolId, string demographic, object additionalValues = null);
        string StudentDemographicList(int schoolId, string demographic = null, object additionalValues = null);
        string ExportStudentGradeList(int schoolId, string grade, object additionalValues = null);
        string StudentGradeList(int schoolId, string school = null, string grade = null, object additionalValues = null);
        string Resource(int schoolId, string resourceName, object additionalValues = null);
        string CustomMetricsBasedWatchListGrade(int schoolId, string resourceName, string grade, int? sectionOrCohortId = null, string studentListType = null, object additionalValues = null);
        string CustomMetricsBasedWatchListDemographic(int schoolId, string resourceName, string demographic, int? sectionOrCohortId = null, string studentListType = null, object additionalValues = null);
    }
}