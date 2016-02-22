// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
namespace EdFi.Dashboards.Resources.Navigation
{
    public interface IStudentSchoolAreaLinks 
    {
        string Default(int schoolId, long studentUSI, string student = null);
        string Information(int schoolId, long studentUSI, string student = null, object additionalValues = null);
        string Overview(int schoolId, long studentUSI, string student = null, object additionalValues = null);
        string Metrics(int schoolId, long studentUSI, int metricVariantId, string student = null, object additionalValues = null);
        string MetricsDrilldown(int schoolId, long studentUSI, int metricVariantId, string drilldownName, string student = null, object additionalValues = null);
        string AcademicProfile(int schoolId, long studentUSI, string student = null, object additionalValues = null);
        string ProfileThumbnail(int schoolId, long studentUSI, string gender, string student = null);
        string Image(int schoolId, long studentUSI, string gender, string student = null);
        string ListImage(int schoolId, long studentUSI, string gender, string student = null);
        string ExportAllMetrics(int schoolId, long studentUSI, string student = null, object additionalValues = null);
        string Resource(int schoolId, long studentUSI, string resourceName, object additionalValues = null);
    }
}