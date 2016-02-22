// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
namespace EdFi.Dashboards.Resources.Navigation
{
    public interface IAdminAreaLinks 
    {
        string Default(int localEducationAgencyId, object additionalValues = null);
        string Configuration(int localEducationAgencyId, object additionalValues = null);
        string LogInAs(int localEducationAgencyId, object additionalValues = null);
        string TitleClaimSet(int localEducationAgencyId, object additionalValues = null);
        string ExportTitleClaimSet(int localEducationAgencyId, object additionalValues = null);
        string MetricThreshold(int localEducationAgencyId, object additionalValues = null);
        string PhotoManagement(int localEducationAgencyId, object additionalValues = null);
        string Resource(int localEducationAgencyId, string resourceName, object additionalValues = null);
    }
}