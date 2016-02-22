// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System;
using System.Collections.Generic;

namespace EdFi.Dashboards.Resources.Security.Tests
{
    public class SafeMethodOnlySecurityAssertionProvider : ISecurityAssertionProvider
    {
        #region Empty Assertion Implementations

        public void SchoolMustBeAssociatedWithLocalEducationAgency(int schoolId, int leaId)
        {
            // Do no verification, just let calls proceed
        }

        public void SchoolMustBeAssociatedWithSection(int schoolId, long sectionId)
        {
            // Do no verification, just let calls proceed
        }

        public void EducationOrganizationMustBeAssociatedWithCohort(int schoolId, long cohortId)
        {
            // Do no verification, just let calls proceed
        }
        public void EducationOrganizationMustBeAssociatedWithCustomStudentList(int schoolId, long cohortId)
        {
            // Do no verification, just let calls proceed
        }

        public void SchoolMustBeAssociatedWithStaff(int schoolId, long staffUSI)
        {
            // Do no verification, just let calls proceed
        }

        public void SchoolMustBeAssociatedWithStudent(int schooldId, long studentUSI)
        {
            // Do no verification, just let calls proceed
        }

        public void StaffTeacherMustBeAssociatedWithSection(long staffUSI, long sectionId)
        {
            // Do no verification, just let calls proceed
        }

        public void StaffMustBeAssociatedWithCohort(long staffUSI, long cohortId)
        {
            // Do no verification, just let calls proceed
        }
        public void StaffMustBeAssociatedWithCustomStudentList(long staffUSI, long cohortId)
        {
            // Do no verification, just let calls proceed
        }

        public bool CurrentUserMustHaveClaimOnOrganization(int educationOrganizationId, string claimType)
        {
            throw new NotImplementedException();
        }

        public bool CurrentUserMustHaveClaimOnAnySchoolsOfStudent(long studentUSI, string claimType)
        {
            return true;
        }

        public bool CurrentUserMustHaveClaimOnStaff(long staffUSI, string claimType)
        {
            // Do no verification, just let calls proceed
            return true;
        }

        public bool CurrentUserMustHaveLinkToStudentAtSchoolWithClaim(long studentUSI, string claimType)
        {
            return true;
        }

        public bool StaffMemberMustHaveLinkToStudent(long staffUSI, long studentUSI)
        {
            // Do no verification, just let calls proceed
            return true;
        }

        public void LocalEducationAgencyMustBeAssociatedWithStaff(int localEducationAgencyId, long staffUSI)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<int> GetEducationOrganizationHierarchy(int educationOrganizationId)
        {
            throw new NotImplementedException();
        }

        #endregion



        void ISecurityAssertionProvider.StaffMemberMustHaveLinkToStudent(long staffUSI, long studentUSI)
        {
            throw new NotImplementedException();
    }

        public void CurrentUserMustHaveClaimWithinEducationOrganizationHierarchy(int educationOrganizationId, string claimType)
        {
            throw new NotImplementedException();
        }

        public void CurrentUserMustHaveClaimWithinHierarchyOfAnySchoolsOfStudent(long studentUSI, string claimType)
        {
            throw new NotImplementedException();
        }

        void ISecurityAssertionProvider.CurrentUserMustHaveClaimOnStaff(long staffUSI, string claimType)
        {
            throw new NotImplementedException();
        }

        void ISecurityAssertionProvider.CurrentUserMustHaveLinkToStudentAtAnySchoolWithClaim(long studentUSI, string claimType)
        {
            throw new NotImplementedException();
        }

        public void CurrentUserMustHaveClaimOnEducationOrganization(int educationOrganizationId, string claimType)
        {
            throw new NotImplementedException();
        }


        public void EducationOrganizationMustBeAssociatedWithCustomMetricsBasedWatchList(int schoolId, long cohortId)
        {
            throw new NotImplementedException();
        }

        public void StaffMustBeAssociatedWithMetricsBasedWatchList(long staffUSI, long cohortId)
        {
            throw new NotImplementedException();
        }
    }
}
