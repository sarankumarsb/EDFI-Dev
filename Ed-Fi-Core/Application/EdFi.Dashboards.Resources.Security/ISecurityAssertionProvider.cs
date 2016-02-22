// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System.Collections.Generic;

namespace EdFi.Dashboards.Resources.Security
{
    public interface ISecurityAssertionProvider
    {
        //School Specific Assertions
        void SchoolMustBeAssociatedWithLocalEducationAgency(int schoolId, int leaId);
        void SchoolMustBeAssociatedWithSection(int schoolId, long sectionId);
        void EducationOrganizationMustBeAssociatedWithCohort(int schoolId, long cohortId);
        void EducationOrganizationMustBeAssociatedWithCustomStudentList(int schoolId, long cohortId);
        void EducationOrganizationMustBeAssociatedWithCustomMetricsBasedWatchList(int schoolId, long cohortId);
        void SchoolMustBeAssociatedWithStaff(int schoolId, long staffUSI);
        void SchoolMustBeAssociatedWithStudent(int schoolId, long studentUSI);

        //Staff Specific Assertions
        void StaffTeacherMustBeAssociatedWithSection(long staffUSI, long sectionId);
        void StaffMustBeAssociatedWithCohort(long staffUSI, long cohortId);
        void StaffMustBeAssociatedWithCustomStudentList(long staffUSI, long cohortId);
        void StaffMustBeAssociatedWithMetricsBasedWatchList(long staffUSI, long cohortId);
        void StaffMemberMustHaveLinkToStudent(long staffUSI, long studentUSI);
        
        //Current User Specific Assertions
        void CurrentUserMustHaveClaimWithinEducationOrganizationHierarchy(int educationOrganizationId, string claimType);
        void CurrentUserMustHaveClaimWithinHierarchyOfAnySchoolsOfStudent(long studentUSI, string claimType);
        void CurrentUserMustHaveClaimOnStaff(long staffUSI, string claimType);
        void CurrentUserMustHaveLinkToStudentAtAnySchoolWithClaim(long studentUSI, string claimType);
        void CurrentUserMustHaveClaimOnEducationOrganization(int educationOrganizationId, string claimType);

        // Local Education Agency Assertions
        void LocalEducationAgencyMustBeAssociatedWithStaff(int localEducationAgencyId, long staffUSI);

        IEnumerable<int> GetEducationOrganizationHierarchy(int educationOrganizationId);
    }
}
