using System;
using System.Reflection;
using EdFi.Dashboards.Resources.Images;
using EdFi.Dashboards.Resources.Images.Navigation;

namespace EdFi.Dashboards.Resources.Navigation.Mvc.Areas
{
    public class LocalEducationAgency : SiteAreaBase, ILocalEducationAgencyAreaLinks
    {
        private readonly IImageLinkProvider imageLinkProvider;

        public LocalEducationAgency(IImageLinkProvider imageLinkProvider)
        {
            this.imageLinkProvider = imageLinkProvider;
        }

        public virtual string Default(int localEducationAgencyId, object additionalValues = null)
        {
            return Overview(localEducationAgencyId, additionalValues);
        }

        public virtual string Entry(string localEducationAgency, object additionalValues = null)
        {
            return BuildResourceUrl(additionalValues, MethodBase.GetCurrentMethod(), localEducationAgency);
        }

        public virtual string Home(string localEducationAgency, object additionalValues = null)
        {
            return BuildResourceUrl(additionalValues, MethodBase.GetCurrentMethod(), localEducationAgency);
        }

        public virtual string SchoolCategoryList(int localEducationAgencyId, object additionalValues = null)
        {
            return BuildResourceUrl(additionalValues, MethodBase.GetCurrentMethod(), localEducationAgencyId);
        }

        public virtual string Information(int localEducationAgencyId, object additionalValues = null)
        {
            return BuildResourceUrl(additionalValues, MethodBase.GetCurrentMethod(), localEducationAgencyId);
        }

        public virtual string Overview(int localEducationAgencyId, object additionalValues = null)
        {
            return BuildResourceUrl(additionalValues, MethodBase.GetCurrentMethod(), localEducationAgencyId);
        }

        public virtual string Metrics(int localEducationAgencyId, int metricVariantId, object additionalValues = null)
        {
            return BuildMetricUrl(additionalValues, MethodBase.GetCurrentMethod(), localEducationAgencyId, metricVariantId);
        }

        public virtual string MetricsDrilldown(int localEducationAgencyId, int metricVariantId, string drilldownName, object additionalValues = null)
        {
            return BuildMetricDrilldownUrl(additionalValues, drilldownName, MethodBase.GetCurrentMethod(), localEducationAgencyId, metricVariantId);
        }

        public virtual string OperationalDashboard(int localEducationAgencyId, object additionalValues = null)
        {
            return BuildUrlUsingMethodNameAsRouteSuffix(additionalValues, MethodBase.GetCurrentMethod(), localEducationAgencyId);
        }

        public virtual string GoalPlanning(int localEducationAgencyId, int metricVariantId, object additionalValues = null)
        {
            return BuildUrlUsingMethodNameAsRouteSuffix(additionalValues, MethodBase.GetCurrentMethod(), localEducationAgencyId, metricVariantId);
        }

        public virtual string PublishGoals(int localEducationAgencyId, object additionalValues = null)
        {
            return BuildResourceUrl(additionalValues, MethodBase.GetCurrentMethod(), localEducationAgencyId);
        }

        public virtual string ExportAllMetrics(int localEducationAgencyId, object additionalValues = null)
        {
            return BuildResourceUrl(additionalValues, MethodBase.GetCurrentMethod(), localEducationAgencyId);
        }

        public virtual string ExportMetricList(int localEducationAgencyId, int metricVariantId, object additionalValues = null)
        {
            return BuildMetricDrilldownUrl(additionalValues, "ExportMetricList", MethodBase.GetCurrentMethod(), localEducationAgencyId, metricVariantId);
        }

        public virtual string ExportStudentDemographicList(int localEducationAgencyId, string demographic = null, object additionalValues = null)
        {
            return BuildUrl("LocalEducationAgency_Demographics", MethodBase.GetCurrentMethod().Name, additionalValues, MethodBase.GetCurrentMethod(), localEducationAgencyId, demographic);
        }

        public virtual string ExportStudentList(int localEducationAgencyId, long staffUSI, string staff = null, long? sectionOrCohortId = null, string studentListType = null, object additionalValues = null)
        {
            return BuildUrl("LocalEducationAgency_StudentList", MethodBase.GetCurrentMethod().Name, additionalValues, MethodBase.GetCurrentMethod(), localEducationAgencyId, staffUSI, staff, sectionOrCohortId, studentListType);
        }
   
        public virtual string ExportStudentSchoolCategoryList(int localEducationAgencyId, string schoolCategory = null, object additionalValues = null)
        {
            return BuildUrl("LocalEducationAgency_StudentSchoolCategory", MethodBase.GetCurrentMethod().Name, additionalValues, MethodBase.GetCurrentMethod(), localEducationAgencyId, schoolCategory);
        }

        public virtual string StudentList(int localEducationAgencyId, long staffUSI, string staff = null, long? sectionOrCohortId = null, string studentListType = null, object additionalValues = null)
        {
            return BuildUrl("LocalEducationAgency_StudentList", MethodBase.GetCurrentMethod().Name, additionalValues, MethodBase.GetCurrentMethod(), localEducationAgencyId, staffUSI, staff, sectionOrCohortId, studentListType);
        }

        public string StudentDemographicList(int localEducationAgencyId, string demographic = null, object additionalValues = null)
        {
            return BuildUrl("LocalEducationAgency_Demographics", MethodBase.GetCurrentMethod().Name, additionalValues, MethodBase.GetCurrentMethod(), localEducationAgencyId, demographic);
        }

        public string StudentSchoolCategoryList(int localEducationAgencyId, string schoolCategory = null, object additionalValues = null)
        {
            return BuildUrl("LocalEducationAgency_StudentSchoolCategory", MethodBase.GetCurrentMethod().Name, additionalValues, MethodBase.GetCurrentMethod(), localEducationAgencyId, schoolCategory);
        }

        public virtual string ProfileThumbnail(int localEducationAgencyId)
        {
            return imageLinkProvider.GetImageLink(new LocalEducationAgencyImageRequest { LocalEducationAgencyId = localEducationAgencyId, DisplayType = "Thumb" });
        }

        public virtual string Image(int localEducationAgencyId)
        {
            return imageLinkProvider.GetImageLink(new LocalEducationAgencyImageRequest { LocalEducationAgencyId = localEducationAgencyId });
        }

        public virtual string ListImage(int localEducationAgencyId)
        {
            return imageLinkProvider.GetImageLink(new LocalEducationAgencyImageRequest { LocalEducationAgencyId = localEducationAgencyId, DisplayType = "List" });
        }

        public virtual string Resource(int localEducationAgencyId, string resourceName, object additionalValues = null)
        {
            return BuildResourceUrl(additionalValues, resourceName, MethodBase.GetCurrentMethod(), localEducationAgencyId);
        }

        public virtual string ApiResource(int localEducationAgencyId, object resourceIdentifier, string resourceName)
        {
            return ApiResource(localEducationAgencyId, resourceIdentifier, resourceName, null);
        }

        public virtual string ApiResource(int localEducationAgencyId, object resourceIdentifier, string resourceName, object additionalValues = null)
        {
            return BuildApiResourceUrl(additionalValues, resourceName, MethodBase.GetCurrentMethod(), localEducationAgencyId, resourceIdentifier);
        }

        public string CustomMetricsBasedWatchListDemographic(int localEducationAgencyId, long? staffUSI, string resourceName, string demographic, int? sectionOrCohortId = null, string studentListType = null, object additionalValues = null)
        {
            return BuildUrl("LocalEducationAgency_WatchListDemographic", resourceName, additionalValues,
                MethodBase.GetCurrentMethod(), localEducationAgencyId, staffUSI, resourceName, demographic, sectionOrCohortId,
                studentListType);
        }

        public string CustomMetricsBasedWatchListSchoolCategory(int localEducationAgencyId, long? staffUSI, string resourceName, string schoolCategory, int? sectionOrCohortId = null, string studentListType = null, object additionalValues = null)
        {
            return BuildUrl("LocalEducationAgency_WatchListSchoolCategory", resourceName, additionalValues,
                MethodBase.GetCurrentMethod(), localEducationAgencyId, staffUSI, resourceName, schoolCategory,
                sectionOrCohortId, studentListType);
        }
    }
}
