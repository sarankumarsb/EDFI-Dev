// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System.Reflection;
using EdFi.Dashboards.Resources.Navigation;

namespace EdFi.Dashboards.Resources.Tests.Navigation.Fakes
{
    public class StudentSchoolAreaLinksFake : SiteAreaFakeBase, IStudentSchoolAreaLinks
    {
        public string Default(int schoolId, long studentUSI, string student = null)
        {
            return BuildUrl(GetType().Name, null, MethodBase.GetCurrentMethod(), schoolId, studentUSI, student);
        }

        public string Information(int schoolId, long studentUSI, string student = null, object additionalValues = null)
        {
            return BuildUrl(GetType().Name, additionalValues, MethodBase.GetCurrentMethod(), schoolId, studentUSI, student);
        }

        public string Overview(int schoolId, long studentUSI, string student = null, object additionalValues = null)
        {
            return BuildUrl(GetType().Name, additionalValues, MethodBase.GetCurrentMethod(), schoolId, studentUSI, student);
        }

        public string Metrics(int schoolId, long studentUSI, int metricId, string student = null, object additionalValues = null)
        {
            return BuildUrl(GetType().Name, additionalValues, MethodBase.GetCurrentMethod(), schoolId, studentUSI, metricId, student);
        }

        public string MetricsDrilldown(int schoolId, long studentUSI, int metricId, string drilldownName, string student = null, object additionalValues = null)
        {
            return BuildUrl(GetType().Name, additionalValues, MethodBase.GetCurrentMethod(), schoolId, studentUSI, metricId, new { controller = drilldownName });
        }

        public string AcademicProfile(int schoolId, long studentUSI, string student = null, object additionalValues = null)
        {
            return BuildUrl(GetType().Name, additionalValues, MethodBase.GetCurrentMethod(), schoolId, studentUSI, student);
        }

        public string ProfileThumbnail(int schoolId, long studentUSI, string gender, string student = null)
        {
            return BuildUrl(GetType().Name, null, MethodBase.GetCurrentMethod(), schoolId, studentUSI, gender, student);
        }

        public string Image(int schoolId, long studentUSI, string gender, string student = null)
        {
            return BuildUrl(GetType().Name, null, MethodBase.GetCurrentMethod(), schoolId, studentUSI, gender, student);
        }

        public string ListImage(int schoolId, long studentUSI, string gender, string student = null)
        {
            return BuildUrl(GetType().Name, null, MethodBase.GetCurrentMethod(), schoolId, studentUSI, gender, student);
        }

        public virtual string ExportAllMetrics(int schoolId, long studentUSI, string student = null, object additionalValues = null)
        {
            return BuildUrl(GetType().Name, additionalValues, MethodBase.GetCurrentMethod(), schoolId, studentUSI, student);
        }
        public virtual string Resource(int schoolId, long studentUSI, string resourceName, object additionalValues = null)
        {
            return BuildUrl(GetType().Name, additionalValues, MethodBase.GetCurrentMethod(), schoolId, studentUSI);
        }
    }
}
