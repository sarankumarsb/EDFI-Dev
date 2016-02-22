// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;
using System.Reflection;
using EdFi.Dashboards.Resources.Navigation;

namespace EdFi.Dashboards.Resources.Tests.Navigation.Fakes
{
    public class LocalEducationAgencyAreaLinksFake : SiteAreaFakeBase, ILocalEducationAgencyAreaLinks
    {
        public string Default(int localEducationAgencyId, object additionalValues = null)
        {
            return BuildUrl(GetType().Name, additionalValues, MethodBase.GetCurrentMethod(), localEducationAgencyId);
        }

        public string Entry(string localEducationAgency, object additionalValues)
        {
            return BuildUrl(GetType().Name, additionalValues, MethodBase.GetCurrentMethod(), localEducationAgency);
        }

        public string Home(string localEducationAgency, object additionalValues = null)
        {
            return BuildUrl(GetType().Name, additionalValues, MethodBase.GetCurrentMethod(), localEducationAgency);
        }

        public string SchoolCategoryList(int localEducationAgencyId, object additionalValues = null)
        {
            return BuildUrl(GetType().Name, additionalValues, MethodBase.GetCurrentMethod(), localEducationAgencyId);
        }

        public string Information(int localEducationAgencyId, object additionalValues = null)
        {
            return BuildUrl(GetType().Name, additionalValues, MethodBase.GetCurrentMethod(), localEducationAgencyId);
        }

        public string Overview(int localEducationAgencyId, object additionalValues = null)
        {
            return BuildUrl(GetType().Name, additionalValues, MethodBase.GetCurrentMethod(), localEducationAgencyId);
        }

        public string Metrics(int localEducationAgencyId, int metricId, object additionalValues = null)
        {
            return BuildUrl(GetType().Name, additionalValues, MethodBase.GetCurrentMethod(), localEducationAgencyId, metricId);
        }

        public string MetricsDrilldown(int localEducationAgencyId, int metricId, string drilldownName, object additionalValues = null)
        {
            return BuildUrl(GetType().Name, additionalValues, MethodBase.GetCurrentMethod(), localEducationAgencyId, metricId, new { controller = drilldownName });
        }

        public string OperationalDashboard(int localEducationAgencyId, object additionalValues)
        {
            return BuildUrl(GetType().Name, additionalValues, MethodBase.GetCurrentMethod(), localEducationAgencyId);
        }

        public string GoalPlanning(int localEducationAgencyId, int metricId, object additionalValues)
        {
            return BuildUrl(GetType().Name, additionalValues, MethodBase.GetCurrentMethod(), localEducationAgencyId, metricId);
        }

        public  string PublishGoals(int localEducationAgencyId, object additionalValues = null)
        {
            return BuildUrl(GetType().Name, additionalValues, MethodBase.GetCurrentMethod(), localEducationAgencyId);
        }

        public string ExportAllMetrics(int localEducationAgencyId, object additionalValues)
        {
            return BuildUrl(GetType().Name, additionalValues, MethodBase.GetCurrentMethod(), localEducationAgencyId);
        }

        public string ExportMetricList(int localEducationAgencyId, int metricId, object additionalValues)
        {
            return BuildUrl(GetType().Name, additionalValues, MethodBase.GetCurrentMethod(), localEducationAgencyId, metricId);
        }

        public string ExportStudentList(int localEducationAgencyId, long staffUSI, string staff = null, long? sectionOrCohortId = null, string studentListType = null, object additionalValues = null)
        {
            return BuildUrl(GetType().Name, additionalValues, MethodBase.GetCurrentMethod(), localEducationAgencyId);
        }

        public string ExportStudentDemographicList(int localEducationAgencyId, string demographic, object additionalValues = null)
        {
            return BuildUrl(GetType().Name, additionalValues, MethodBase.GetCurrentMethod(), localEducationAgencyId, demographic);
        }

        public string ExportStudentSchoolCategoryList(int localEducationAgencyId, string schoolCategory, object additionalValues = null)
        {
            return BuildUrl(GetType().Name, additionalValues, MethodBase.GetCurrentMethod(), localEducationAgencyId, schoolCategory);
        }

        public string StudentList(int localEducationAgencyId, long staffUSI, string staff = null, long? sectionOrCohortId = null, string studentListType = null, object additionalValues = null)
        {
            return BuildUrl(GetType().Name, additionalValues, MethodBase.GetCurrentMethod(), localEducationAgencyId);
        }

        public string MetricBasedWatchList(int localEducationAgencyId, long staffUSI, int watchListId, string staff = null, object additionalValues = null)
        {
            return BuildUrl(GetType().Name, additionalValues, MethodBase.GetCurrentMethod(), localEducationAgencyId, staffUSI, watchListId);
        }

        public string StudentDemographicList(int localEducationAgencyId, string demographic, object additionalValues = null)
        {
            return BuildUrl(GetType().Name, additionalValues, MethodBase.GetCurrentMethod(), localEducationAgencyId, demographic);
        }

        public string StudentSchoolCategoryList(int localEducationAgencyId, string schoolCategory, object additionalValues = null)
        {
            return BuildUrl(GetType().Name, additionalValues, MethodBase.GetCurrentMethod(), localEducationAgencyId, schoolCategory);
        }

        public string ProfileThumbnail(int localEducationAgencyId)
        {
            return BuildUrl(GetType().Name, new { display = "Thumb", format = "image" }, MethodBase.GetCurrentMethod(), localEducationAgencyId);
        }

        public string Image(int localEducationAgencyId)
        {
            return BuildUrl(GetType().Name, new { display = String.Empty, format = "image" }, MethodBase.GetCurrentMethod(), localEducationAgencyId);
        }

        public string ListImage(int localEducationAgencyId)
        {
            return BuildUrl(GetType().Name, new { display = "List", format = "image" }, MethodBase.GetCurrentMethod(), localEducationAgencyId);
        }

        public virtual string Resource(int localEducationAgencyId, string resourceName, object additionalValues = null)
        {
            return BuildUrl(resourceName, additionalValues, MethodBase.GetCurrentMethod(), localEducationAgencyId);
        }

        public string ApiResource(int localEducationAgencyId, object resourceIdentifier, string resourceName)
        {
            return ApiResource(localEducationAgencyId, resourceIdentifier, resourceName, null);
        }

        public string ApiResource(int localEducationAgencyId, object resourceIdentifier, string resourceName, object additionalValues)
        {
            return BuildUrl(resourceName, additionalValues, MethodBase.GetCurrentMethod(), localEducationAgencyId, resourceIdentifier);
        }

        public string CustomMetricsBasedWatchListDemographic(int localEducationAgencyId, long? staffUSI, string resourceName, string demographic, int? metricBasedWatchListId = null, string studentListType = null, object additionalValues = null)
        {
            return BuildResourceUrl(additionalValues, resourceName, MethodBase.GetCurrentMethod(), localEducationAgencyId, staffUSI, resourceName, demographic, metricBasedWatchListId, studentListType);
        }

        public string CustomMetricsBasedWatchListSchoolCategory(int localEducationAgencyId, long? staffUSI, string resourceName, string level, int? sectionOrCohortId = null, string studentListType = null, object additionalValues = null)
        {
            return BuildResourceUrl(additionalValues, resourceName, MethodBase.GetCurrentMethod(), localEducationAgencyId, staffUSI, resourceName, level, sectionOrCohortId, studentListType);
        }
    }
}
