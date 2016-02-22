// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System.Collections.Generic;
using System.Linq;

namespace EdFi.Dashboards.Resources.Security
{
    public interface IAuthorizationInformationProvider
    {
        List<long> GetSchoolSectionIds(int schoolId);
        List<long> GetEducationOrganizationCohortIds(int educationOrganizationId);
        List<long> GetEducationOrganizationCustomStudentListIds(int educationOrganizationId);
        List<long> GetEducationOrganizationCustomMetricsBasedWatchListIds(int educationOrganizationId);
        List<long> GetSchoolStudentUSIs(int schoolId);
        List<long> GetSchoolStaffUSIs(int schoolId);
        List<long> GetLocalEducationAgencyStaffUSIs(int localEducationAgencyId);
        HashSet<long> GetAllStaffStudentUSIs(long staffUSI);
        List<long> GetTeacherStudentUSIs(long staffUSI);
        List<long> GetTeacherSectionIds(long staffUSI);
        List<long> GetStaffCohortIds(long staffUSI);
        List<long> GetStaffCustomStudentListIds(long staffUSI);
        List<long> GetStaffCustomMetricsBasedWatchListIds(long staffUSI);
        List<long> GetStaffCohortStudentUSIs(long staffUSI);
        List<int> GetStudentSchoolIds(long studentUSI);
        List<long> GetPrincipalStudentUSIs(long staffUSI);
        List<int> GetAssociatedEducationOrganizations(long staffUSI);
        bool IsStudentAssociatedWithStaffAtSchool(long studentUSI, long staffUSI, int schoolId);
        bool IsStudentAssociatedWithSchool(long studentUSI, int schoolId);

        IQueryable<long> GetIQueryableForPrincipalStudentUSIs(long staffUSI);
    }
}
