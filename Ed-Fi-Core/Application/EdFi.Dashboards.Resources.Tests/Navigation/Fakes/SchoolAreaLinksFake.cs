// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;
using System.Reflection;
using EdFi.Dashboards.Resources.Navigation;

namespace EdFi.Dashboards.Resources.Tests.Navigation.Fakes
{
    public class SchoolAreaLinksFake : SiteAreaFakeBase, ISchoolAreaLinks
    {
        public string Default(int schoolId, string school = null, object additionalValues = null)
        {
            return BuildUrl(GetType().Name, additionalValues, MethodBase.GetCurrentMethod(), schoolId, school);
        }

        public string Entry(string localEducationAgency, int schoolId, string school = null, object additionalValues = null)
        {
            return BuildUrl(GetType().Name, additionalValues, MethodBase.GetCurrentMethod(), localEducationAgency, schoolId, school);
        }

        public string Information(int schoolId, string school = null, object additionalValues = null)
        {
            return BuildUrl(GetType().Name, additionalValues, MethodBase.GetCurrentMethod(), schoolId, school);
        }

        public string Overview(int schoolId, string school = null, object additionalValues = null)
        {
            return BuildUrl(GetType().Name, additionalValues, MethodBase.GetCurrentMethod(), schoolId, school);
        }

        public string StudentsByGrade(int schoolId, string school = null, object additionalValues = null)
        {
            return BuildUrl(GetType().Name, additionalValues, MethodBase.GetCurrentMethod(), schoolId, school);
        }

        public string Teachers(int schoolId, string school = null, object additionalValues = null)
        {
            return BuildUrl(GetType().Name, additionalValues, MethodBase.GetCurrentMethod(), schoolId, school);
        }

        public string Staff(int schoolId, string school = null, object additionalValues = null)
        {
            return BuildUrl(GetType().Name, additionalValues, MethodBase.GetCurrentMethod(), schoolId, school);
        }

        public string Metrics(int schoolId, int metricId, string school = null, object additionalValues = null)
        {
            return BuildUrl(GetType().Name, additionalValues, MethodBase.GetCurrentMethod(), schoolId, metricId, school);
        }

        public string MetricsDrilldown(int schoolId, int metricId, string drilldownName, string school = null, object additionalValues = null)
        {
            return BuildUrl(GetType().Name, additionalValues, MethodBase.GetCurrentMethod(), schoolId, metricId, school, new { controller = drilldownName });
        }

        public string OperationalDashboard(int schoolId, string school, object additionalValues)
        {
            return BuildUrl(GetType().Name, additionalValues, MethodBase.GetCurrentMethod(), schoolId, school);
        }

        public string GoalPlanning(int schoolId, int metricId, string school = null, object additionalValues = null)
        {
            return BuildUrl(GetType().Name, additionalValues, MethodBase.GetCurrentMethod(), schoolId, metricId, school);
        }

        public string PublishGoals(int schoolId, string school = null, object additionalValues = null)
        {
            return BuildUrl(GetType().Name, additionalValues, MethodBase.GetCurrentMethod(), schoolId, school);
        }

        public string ProfileThumbnail(int schoolId)
        {
            return BuildUrl(GetType().Name, null, MethodBase.GetCurrentMethod(), schoolId);
        }

        public string Image(int schoolId)
        {
            return BuildUrl(GetType().Name, null, MethodBase.GetCurrentMethod(), schoolId);
        }

        public string ListImage(int schoolId)
        {
            return BuildUrl(GetType().Name, null, MethodBase.GetCurrentMethod(), schoolId);
        }

        public virtual string ExportAllMetrics(int schoolId, object additionalValues = null)
        {
            return BuildUrl(GetType().Name, additionalValues, MethodBase.GetCurrentMethod(), schoolId);
        }

        public virtual string ExportMetricList(int schoolId, int metricId, object additionalValues = null)
        {
            return BuildUrl(GetType().Name, additionalValues, MethodBase.GetCurrentMethod(), schoolId, metricId);
        }

        public virtual string Resource(int schoolId, string resourceName, object additionalValues = null)
        {
            return BuildUrl(GetType().Name, additionalValues, MethodBase.GetCurrentMethod(), schoolId);
        }

        public virtual string StudentDemographicList(int schoolId, string demographic, object additionalValues = null)
        {
            return BuildUrl(GetType().Name, additionalValues, MethodBase.GetCurrentMethod(), schoolId, demographic);
        }

        public virtual string ExportStudentDemographicList(int schoolId, string demographic, object additionalValues = null)
        {
            return BuildUrl(GetType().Name, additionalValues, MethodBase.GetCurrentMethod(), schoolId, demographic);
        }

        public virtual string StudentGradeList(int schoolId, string school = null, string grade = null, object additionalValues = null)
        {
            return BuildUrl(GetType().Name, additionalValues, MethodBase.GetCurrentMethod(), schoolId, school, grade);
        }

        public virtual string ExportStudentGradeList(int schoolId, string grade, object additionalValues = null)
        {
            return BuildUrl(GetType().Name, additionalValues, MethodBase.GetCurrentMethod(), schoolId, grade);
        }

        public string CustomMetricsBasedWatchListGrade(int schoolId, string resourceName, string grade, int? sectionOrCohortId = null, string studentListType = null, object additionalValues = null)
        {
            return BuildUrl("School_WatchListGrades", resourceName, additionalValues, MethodBase.GetCurrentMethod(), schoolId, resourceName, grade, sectionOrCohortId, studentListType);
        }

        public string CustomMetricsBasedWatchListDemographic(int schoolId, string resourceName, string demographic, int? sectionOrCohortId = null, string studentListType = null, object additionalValues = null)
        {
            return BuildUrl("School_WatchListDemographics", resourceName, additionalValues,
                MethodBase.GetCurrentMethod(), schoolId, resourceName, demographic, sectionOrCohortId, studentListType);
        }
    }
}