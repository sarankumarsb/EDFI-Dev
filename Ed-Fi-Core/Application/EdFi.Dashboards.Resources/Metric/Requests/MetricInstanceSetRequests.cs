// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using EdFi.Dashboards.Core;
using EdFi.Dashboards.Metric.Resources.Services;
using EdFi.Dashboards.Resources.Security.Common;
using EdFi.Dashboards.SecurityTokenService.Authentication;

namespace EdFi.Dashboards.Resources.Metric.Requests
{
    [MvcArea("LocalEducationAgency")]
    [CanBeAuthorizedBy(EdFiClaimTypes.ViewAllMetrics, EdFiClaimTypes.ViewMyMetrics)]
    public class LocalEducationAgencyMetricInstanceSetRequest : MetricInstanceSetRequestBase
    {
        [AuthenticationIgnore("MetricInstanceSetTypeId can not be validated.")]
        public override int MetricInstanceSetTypeId { get { return (int) MetricInstanceSetType.LocalEducationAgency; } }

        public int LocalEducationAgencyId { get; set; }

        public static LocalEducationAgencyMetricInstanceSetRequest Create(int localEducationAgencyId, int metricVariantId)
        {
            return new LocalEducationAgencyMetricInstanceSetRequest() { LocalEducationAgencyId = localEducationAgencyId, MetricVariantId = metricVariantId };
        }
    }

    [MvcArea("School")]
    [CanBeAuthorizedBy(EdFiClaimTypes.ViewAllMetrics, EdFiClaimTypes.ViewMyMetrics)]
    public class SchoolMetricInstanceSetRequest : MetricInstanceSetRequestBase
    {
        [AuthenticationIgnore("MetricInstanceSetTypeId can not be validated.")]
        public override int MetricInstanceSetTypeId { get { return (int)MetricInstanceSetType.School; } }

        public int SchoolId { get; set; }

        public static SchoolMetricInstanceSetRequest Create(int schoolId, int metricVariantId)
        {
            return new SchoolMetricInstanceSetRequest() { SchoolId = schoolId, MetricVariantId = metricVariantId };
        }
    }

    [MvcArea("StudentSchool")]
    [CanBeAuthorizedBy(EdFiClaimTypes.ViewAllStudents, EdFiClaimTypes.ViewMyStudents)]
    public class StudentSchoolMetricInstanceSetRequest : MetricInstanceSetRequestBase
    {
        [AuthenticationIgnore("MetricInstanceSetTypeId can not be validated.")]
        public override int MetricInstanceSetTypeId { get { return (int)MetricInstanceSetType.StudentSchool; } }

        public long StudentUSI { get; set; }
        public int SchoolId { get; set; }

        public static StudentSchoolMetricInstanceSetRequest Create(int schoolId, long studentUSI, int metricVariantId)
        {
            return new StudentSchoolMetricInstanceSetRequest() { SchoolId = schoolId, StudentUSI = studentUSI, MetricVariantId = metricVariantId };
        }
    }
}