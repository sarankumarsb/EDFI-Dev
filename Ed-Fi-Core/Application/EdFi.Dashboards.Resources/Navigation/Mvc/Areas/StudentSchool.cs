// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;
using System.Reflection;
using EdFi.Dashboards.Resources.Images;
using EdFi.Dashboards.Resources.Images.Navigation;

namespace EdFi.Dashboards.Resources.Navigation.Mvc.Areas
{
    public class StudentSchool : SiteAreaBase, IStudentSchoolAreaLinks
    {
        private readonly IImageLinkProvider imageLinkProvider;

        public StudentSchool(IImageLinkProvider imageLinkProvider)
        {
            this.imageLinkProvider = imageLinkProvider;
        }

        public virtual string Default(int schoolId, long studentUSI, string student = null)
        {
            return Overview(schoolId, studentUSI, student);
        }

        public virtual string Information(int schoolId, long studentUSI, string student = null, object additionalValues = null)
        {
            return BuildResourceUrl(additionalValues, MethodBase.GetCurrentMethod(), schoolId, studentUSI, student);
        }

        public virtual string Overview(int schoolId, long studentUSI, string student = null, object additionalValues = null)
        {
            return BuildResourceUrl(additionalValues, MethodBase.GetCurrentMethod(), schoolId, studentUSI, student);
        }

        public virtual string Metrics(int schoolId, long studentUSI, int metricVariantId, string student = null, object additionalValues = null)
        {
            return BuildMetricUrl(additionalValues, MethodBase.GetCurrentMethod(), schoolId, studentUSI, metricVariantId, student);
        }

        public virtual string MetricsDrilldown(int schoolId, long studentUSI, int metricVariantId, string drilldownName, string student = null, object additionalValues = null)
        {
            return BuildMetricDrilldownUrl(additionalValues, drilldownName, MethodBase.GetCurrentMethod(), schoolId, studentUSI, metricVariantId, drilldownName, student);
        }

        public virtual string AcademicProfile(int schoolId, long studentUSI, string student = null, object additionalValues = null)
        {
            return BuildResourceUrl(additionalValues, MethodBase.GetCurrentMethod(), schoolId, studentUSI, student);
        }

        public virtual string ProfileThumbnail(int schoolId, long studentUSI, string gender, string student = null)
        {
            return imageLinkProvider.GetImageLink(new StudentSchoolImageRequest { SchoolId = schoolId, StudentUSI = studentUSI, Gender = gender, DisplayType = "Thumb" });
        }

        public virtual string Image(int schoolId, long studentUSI, string gender, string student = null)
        {
            return imageLinkProvider.GetImageLink(new StudentSchoolImageRequest {SchoolId = schoolId, StudentUSI = studentUSI, Gender = gender});
        }

        public virtual string ListImage(int schoolId, long studentUSI, string gender, string student = null)
        {
            return imageLinkProvider.GetImageLink(new StudentSchoolImageRequest { SchoolId = schoolId, StudentUSI = studentUSI, Gender = gender, DisplayType = "List" });
        }

        public virtual string ExportAllMetrics(int schoolId, long studentUSI, string student = null, object additionalValues = null)
        {
            return BuildResourceUrl(additionalValues, MethodBase.GetCurrentMethod(), schoolId, studentUSI, student);
        }
        
        public virtual string Resource(int schoolId, long studentUSI, string resourceName, object additionalValues = null)
        {
            return BuildResourceUrl(additionalValues, resourceName, MethodBase.GetCurrentMethod(), schoolId, studentUSI);
        }
    }
}
