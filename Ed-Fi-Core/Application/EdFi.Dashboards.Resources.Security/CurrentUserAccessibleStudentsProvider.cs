using System.Collections.Generic;
using System.Linq;
using EdFi.Dashboards.Resources.Security.Common;
using EdFi.Dashboards.SecurityTokenService.Authentication;

namespace EdFi.Dashboards.Resources.Security
{
    public interface ICurrentUserAccessibleStudentsProvider
    {
        AccessibleStudents GetAccessibleStudents(int? educationOrganization, bool isSearch);
    }

    public class CurrentUserAccessibleStudentsProvider : ICurrentUserAccessibleStudentsProvider
    {
        private readonly IAuthorizationInformationProvider authorizationInformationProvider;
        private readonly ICurrentUserClaimInterrogator currentUserClaimInterrogator;

        public CurrentUserAccessibleStudentsProvider(IAuthorizationInformationProvider authorizationInformationProvider, ICurrentUserClaimInterrogator currentUserClaimInterrogator)
        {
            this.authorizationInformationProvider = authorizationInformationProvider;
            this.currentUserClaimInterrogator = currentUserClaimInterrogator;
        }

        public virtual AccessibleStudents GetAccessibleStudents(int? educationOrganization, bool isSearch)
        {
            //If an education organization is not provided, then the user MUST have a state level claim.
            if (educationOrganization == null && !isSearch)
            {
                return new AccessibleStudents
                {
                    CanAccessAllStudents = currentUserClaimInterrogator.HasClaimForStateAgency(EdFiClaimTypes.ViewAllStudents),
                    StudentUSIs = new HashSet<long>()
                };
            }

            if (isSearch && currentUserClaimInterrogator.HasClaimForStateAgency(EdFiClaimTypes.ViewAllStudents))
            {
                return new AccessibleStudents
                {
                    CanAccessAllStudents = true,
                    StudentUSIs = new HashSet<long>()
                };
            }

            //If this isn't a search, the request is siloed to the current LEA, if they have ViewAllSudents for the LEA mark that they can see them all.
            if (!isSearch && currentUserClaimInterrogator.HasClaimForLocalEducationAgencyWithinEducationOrganizationHierarchy(EdFiClaimTypes.ViewAllStudents, (int)educationOrganization))
            {
                // they can see all students
                var result = new AccessibleStudents { CanAccessAllStudents = true, StudentUSIs = new HashSet<long>() };
                return result;
            }

            if ( //Regular calls, check context
                !isSearch && (currentUserClaimInterrogator.HasClaimWithinEducationOrganizationHierarchy(EdFiClaimTypes.ViewAllStudents, (int)educationOrganization)
                || currentUserClaimInterrogator.HasClaimWithinEducationOrganizationHierarchy(EdFiClaimTypes.ViewMyStudents, (int)educationOrganization))
                //Search specific, check all associated edOrgs. TODO: Verify performance of multi LEA searches. WE CHANGED TO HASHSET<int> for faster performance.
                || (isSearch && (currentUserClaimInterrogator.HasClaimForSearch(EdFiClaimTypes.ViewAllStudents) || currentUserClaimInterrogator.HasClaimForSearch(EdFiClaimTypes.ViewMyStudents))))
            {
                var result = new AccessibleStudents
                {
                    CanAccessAllStudents = false,
                    //StudentUSIs = authorizationInformationProvider.GetPrincipalStudentUSIs(UserInformation.Current.StaffUSI).ToArray(),
                    //Returns all student usi's available to the user at any school they have approporiate claims associations for.
                    StudentUSIs = authorizationInformationProvider.GetAllStaffStudentUSIs(UserInformation.Current.StaffUSI),
                };
                return result;
            }

            return new AccessibleStudents { CanAccessAllStudents = false, StudentUSIs = new HashSet<long>() };
        }

        /* oldish code
        public AccessibleStudents GetAccessibleStudents()
        {
            var user = UserInformation.Current;

            if (!EdFiDashboardContext.Current.LocalEducationAgencyId.HasValue)
                throw new Exception(string.Format("There is no Local Education Agency Id in context."));

            int localEducationAgencyId = EdFiDashboardContext.Current.LocalEducationAgencyId.Value;

            //If you are implementing a State wide system then you can/must add logic to set the CanAccessAllStudents.

            // TODO: GKM - This "CanAccessAllStudents" property is valid only for a single district user scenario.  This really should be tied to a specific
            // education organization ID, and the entirety of the logic should be executed to get a complete list of student IDs for 
            // a particular staff member, regardless of district or school.
            if (user.HasLocalEducationAgencyClaim(localEducationAgencyId, EdFiClaimTypes.ViewAllStudents))
            {
                // they can see all students
                var result = new AccessibleStudents { CanAccessAllStudentsAtLEA = true, StudentUSIs = new int[] { } };
                return result;
            }

            if (user.HasClaimOnAnyOrganization(EdFiClaimTypes.ViewAllStudents) || user.HasClaimOnAnyOrganization(EdFiClaimTypes.ViewMyStudents))
            {
                var result = new AccessibleStudents
                {
                    CanAccessAllStudents = false,
                    StudentUSIs = authorizationInformationProvider.GetPrincipalStudentUSIs(user.StaffUSI).ToArray(),
                    //StudentUSIs = authorizationInformationProvider.GetAllStaffStudentUSIs(user.StaffUSI).ToArray(),
                };
                return result;
            }

            // Return an empty list (no accessible students)
            return new AccessibleStudents { CanAccessAllStudents = false, StudentUSIs = new int[] { } };
        }
        */
    }

    public struct AccessibleStudents
    {
        public bool CanAccessAllStudents { get; set; }
        public HashSet<long> StudentUSIs { get; set; }

        public bool CanAccessStudent(long studentUSI)
        {
            return CanAccessAllStudents || StudentUSIs.Contains(studentUSI);
        }
    }
}

