// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;
using System.Reflection;
using EdFi.Dashboards.Resources.Images;
using EdFi.Dashboards.Resources.Images.Navigation;

namespace EdFi.Dashboards.Resources.Navigation.Mvc.Areas
{
    public class School : SiteAreaBase, ISchoolAreaLinks
    {
        private readonly IImageLinkProvider imageLinkProvider;

        public School(IImageLinkProvider imageLinkProvider)
        {
            this.imageLinkProvider = imageLinkProvider;
        }

        public virtual string Default(int schoolId, string school = null, object additionalValues = null)
        {
            return Overview(schoolId, school, additionalValues);
        }

        public virtual string Entry(string localEducationAgency, int schoolId, string school = null, object additionalValues = null)
        {
            return BuildResourceUrl(additionalValues, MethodBase.GetCurrentMethod(), localEducationAgency, schoolId, school);
        }

        public virtual string Information(int schoolId, string school = null, object additionalValues = null)
        {
            return BuildResourceUrl(additionalValues, MethodBase.GetCurrentMethod(), schoolId, school);
        }

        public virtual string Overview(int schoolId, string school = null, object additionalValues = null)
        {
            return BuildResourceUrl(additionalValues, MethodBase.GetCurrentMethod(), schoolId, school);
        }

        public virtual string StudentsByGrade(int schoolId, string school = null, object additionalValues = null)
        {
            return BuildResourceUrl(additionalValues, MethodBase.GetCurrentMethod(), schoolId, school);
        }

        public virtual string Teachers(int schoolId, string school = null, object additionalValues = null)
        {
            return BuildResourceUrl(additionalValues, MethodBase.GetCurrentMethod(), schoolId, school);
        }

        public virtual string Staff(int schoolId, string school = null, object additionalValues = null)
        {
            return BuildResourceUrl(additionalValues, MethodBase.GetCurrentMethod(), schoolId, school);
        }

        public virtual string Metrics(int schoolId, int metricVariantId, string school = null, object additionalValues = null)
        {
            return BuildMetricUrl(additionalValues, MethodBase.GetCurrentMethod(), schoolId, metricVariantId, school);
        }

        public virtual string MetricsDrilldown(int schoolId, int metricVariantId, string drilldownName, string school = null, object additionalValues = null)
        {
            return BuildMetricDrilldownUrl(additionalValues, drilldownName, MethodBase.GetCurrentMethod(), schoolId, metricVariantId, school);
        }

        public virtual string OperationalDashboard(int schoolId, string school = null, object additionalValues = null)
        {
            return BuildUrlUsingMethodNameAsRouteSuffix(additionalValues, MethodBase.GetCurrentMethod(), schoolId, school);
        }

        public virtual string GoalPlanning(int schoolId, int metricVariantId, string school = null, object additionalValues = null)
        {
            return BuildUrlUsingMethodNameAsRouteSuffix(additionalValues, MethodBase.GetCurrentMethod(), schoolId, metricVariantId, school);
        }

        public virtual string PublishGoals(int schoolId, string school = null, object additionalValues = null)
        {
            return BuildResourceUrl(additionalValues, MethodBase.GetCurrentMethod(), schoolId, school);
        }

        public virtual string ProfileThumbnail(int schoolId)
        {
            return imageLinkProvider.GetImageLink(new SchoolImageRequest { SchoolId = schoolId, DisplayType = "Thumb" });
        }

        public virtual string Image(int schoolId)
        {
            return imageLinkProvider.GetImageLink(new SchoolImageRequest { SchoolId = schoolId });
        }

        public virtual string ListImage(int schoolId)
        {
            return imageLinkProvider.GetImageLink(new SchoolImageRequest { SchoolId = schoolId, DisplayType = "List" });
        }

        public virtual string ExportAllMetrics(int schoolId, object additionalValues = null)
        {
            return BuildResourceUrl(additionalValues, MethodBase.GetCurrentMethod(), schoolId);
        }

        public virtual string ExportMetricList(int schoolId, int metricVariantId, object additionalValues = null)
        {
            return BuildMetricDrilldownUrl(additionalValues, "ExportMetricList", MethodBase.GetCurrentMethod(), schoolId, metricVariantId);
        }

        public virtual string StudentDemographicList(int schoolId, string demographic = null, object additionalValues = null)
        {
            return BuildUrl("School_Demographics", MethodBase.GetCurrentMethod().Name, additionalValues, MethodBase.GetCurrentMethod(), schoolId, demographic);
        }

        public virtual string ExportStudentDemographicList(int schoolId, string demographic = null, object additionalValues = null)
        {
            return BuildUrl("School_Demographics", MethodBase.GetCurrentMethod().Name, additionalValues, MethodBase.GetCurrentMethod(), schoolId, demographic);
        }
        
        public virtual string ExportStudentGradeList(int schoolId, string grade, object additionalValues = null)
        {
            return BuildUrl("School_Grades", MethodBase.GetCurrentMethod().Name, additionalValues, MethodBase.GetCurrentMethod(), schoolId, grade);
        }

        public virtual string StudentGradeList(int schoolId, string school = null, string grade = null, object additionalValues = null)
        {
            return BuildUrl("School_Grades", MethodBase.GetCurrentMethod().Name, additionalValues, MethodBase.GetCurrentMethod(), schoolId, school, grade);
        }

        public virtual string Resource(int schoolId, string resourceName, object additionalValues = null)
        {
            return BuildResourceUrl(additionalValues, resourceName, MethodBase.GetCurrentMethod(), schoolId);
        }

        public string CustomMetricsBasedWatchListGrade(int schoolId, string resourceName, string grade, int? sectionOrCohortId = null, string studentListType = null, object additionalValues = null)
        {
            return BuildUrl("School_WatchListGrades", resourceName, additionalValues, MethodBase.GetCurrentMethod(),
                schoolId, resourceName, grade, sectionOrCohortId, studentListType);
        }

        public string CustomMetricsBasedWatchListDemographic(int schoolId, string resourceName, string demographic, int? sectionOrCohortId = null, string studentListType = null, object additionalValues = null)
        {
            return BuildUrl("School_WatchListDemographics", resourceName, additionalValues,
                MethodBase.GetCurrentMethod(), schoolId, resourceName, demographic, sectionOrCohortId, studentListType);
        }
    }
}
