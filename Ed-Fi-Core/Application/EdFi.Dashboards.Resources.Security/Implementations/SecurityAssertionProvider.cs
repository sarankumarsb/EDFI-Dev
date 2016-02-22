// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System.Collections.Generic;
using System.Linq;
using EdFi.Dashboards.Common;
using EdFi.Dashboards.Common.Utilities;
using EdFi.Dashboards.Resources.School;
using EdFi.Dashboards.Resources.Security.Common;
using EdFi.Dashboards.SecurityTokenService.Authentication;

namespace EdFi.Dashboards.Resources.Security.Implementations
{

    public class SecurityAssertionProvider : ISecurityAssertionProvider
    {
        public SecurityAssertionProvider(IAuthorizationInformationProvider authorizationInformationProvider,
                                          ICurrentUserClaimInterrogator currentUserClaimInterrogator)
        {
            _authorizationInformationProvider = authorizationInformationProvider;
            this.currentUserClaimInterrogator = currentUserClaimInterrogator;
        }

        private readonly IAuthorizationInformationProvider _authorizationInformationProvider;
        private readonly ICurrentUserClaimInterrogator currentUserClaimInterrogator;
        private IIdNameService _schoolIdNameService;
        private IIdNameService SchoolIdNameService { get { return _schoolIdNameService ?? (_schoolIdNameService = IoC.Resolve<IIdNameService>()); } }

        private readonly Dictionary<int, List<long>> studentUSIsBySchoolId = new Dictionary<int, List<long>>();
        private readonly Dictionary<int, List<long>> staffUSIsBySchoolId = new Dictionary<int, List<long>>();
        private readonly Dictionary<int, List<long>> sectionIdsBySchoolId = new Dictionary<int, List<long>>();
        private readonly Dictionary<int, List<long>> cohortIdsByEducationOrganizationId = new Dictionary<int, List<long>>();
        private readonly Dictionary<int, List<long>> customStudentListIdsByEducationOrganizationId = new Dictionary<int, List<long>>();
        private readonly Dictionary<int, List<long>> customMetricsBasedWatchListIdsByEducationOrganizationId = new Dictionary<int, List<long>>();
        private readonly Dictionary<long, List<long>> cohortIdsByStaff = new Dictionary<long, List<long>>();
        private readonly Dictionary<long, List<long>> customStudentListIdsByStaff = new Dictionary<long, List<long>>();
        private readonly Dictionary<long, List<long>> customMetricsBasedWatchListIdsByStaff = new Dictionary<long, List<long>>();
        private readonly Dictionary<long, List<long>> sectionIdsByTeacher = new Dictionary<long, List<long>>();
        private readonly Dictionary<long, List<long>> studentUSIsByTeacher = new Dictionary<long, List<long>>();
        private readonly Dictionary<long, List<long>> studentUSIsBySpecialist = new Dictionary<long, List<long>>();
        private readonly Dictionary<int, List<long>> staffUSIsByLocalEducationAgencyId = new Dictionary<int, List<long>>(); 

        // DJWhite: 6 Feb 2012:  Moved definition of error messages to SecurityAssertionProvider, as this 
        // is the location where the security exceptions are thrown.
        //
        public const string NoCohortPermissionErrorMessage =
            "You do not have permissions to view information about this cohort.";
        public const string NoCustomStudentListPermissionErrorMessage = "You do not have permissions to view information about this watch list (custom cohort).";

        public const string NoCustomMetricsBasedWatchListPermissionErrorMessage = "You do not have permissions to view information about this metrics based watch list.";

        public const string NoEducationOrganizationPermissionErrorMessage =
            "You do not have permissions to view information about the education organization.";

        public const string NoSchoolPermissionErrorMessage =
            "You do not have permissions to view information about school.";

        public const string NoSectionPermissionErrorMessage =
            "You do not have permissions to view information about this section.";

        public const string NoStaffPermissionErrorMessage =
            "You do not have permissions to view information about this staff member.";

        public const string NoStudentPermissionErrorMessage =
            "You do not have permissions to view information about this student.";

        // DJWhite: 26 Jan 2012:  Need to be able to explicitly set this service to a mock, for testing purposes only.
        // This method must not appear on the interface.
        //
        public void SetSchoolIdNameService(IIdNameService schoolIdNameService)
        {
            this._schoolIdNameService = schoolIdNameService;
        }

        #region School

        public void SchoolMustBeAssociatedWithLocalEducationAgency(int schoolId, int leaId)
        {
            var request = IdNameRequest.Create(schoolId);
            var model = SchoolIdNameService.Get(request);
            if ((leaId == 0) ||
                (model.LocalEducationAgencyId == leaId))
                return;
          
            throw new UserAccessDeniedException(NoSchoolPermissionErrorMessage);
        }

        public void SchoolMustBeAssociatedWithSection(int schoolId, long sectionId)
        {
            var schoolSections = GetSectionIdsForSchool(schoolId);

            if (schoolSections.Contains(sectionId))
                return;

            throw new UserAccessDeniedException(NoSectionPermissionErrorMessage);
        }

        public void SchoolMustBeAssociatedWithStaff(int schoolId, long staffUSI)
        {
            //In the case that we could not validate the user against the Claim then we try to validate against the domain data.
            var schoolStaff = GetStaffUSIsForSchool(schoolId);
            if (schoolStaff.Contains(staffUSI))
                return;

            //We should first test to see if we can validate against the Claim.
            if (currentUserClaimInterrogator.HasClaimWithinEducationOrganizationHierarchy(EdFiClaimTypes.AccessOrganization,schoolId))
                return;

            

            throw new UserAccessDeniedException(NoStaffPermissionErrorMessage);
        }

        public void SchoolMustBeAssociatedWithStudent(int schoolId, long studentUSI)
        {
            if (_authorizationInformationProvider.IsStudentAssociatedWithSchool(studentUSI, schoolId)) 
                return;

            throw new UserAccessDeniedException(NoStudentPermissionErrorMessage);
            }

        #endregion

        #region Current User

        /// <summary>
        /// True if the Current User has the specified claim on any of the education organizations of the staff member.
        /// </summary>
        /// <param name="staffUsi"></param>
        /// <param name="claimType"></param>
        /// <returns></returns>
        public void CurrentUserMustHaveClaimOnStaff(long staffUsi, string claimType)
        {
            var staffOrganizations = _authorizationInformationProvider.GetAssociatedEducationOrganizations(staffUsi);
            if (
                staffOrganizations.Any(
                    id => currentUserClaimInterrogator.HasClaimWithinEducationOrganizationHierarchy(claimType, id)))
            {
                return;
            }

            throw new UserAccessDeniedException(NoStaffPermissionErrorMessage);
        }

        /// <summary>
        /// Indicates whether the claim supplied is present for the current user for
        /// the current education organization or at an educational organization above this one in the education organization hierarchy.
        /// e.g. School => LocalEducationAgency => StateAgency etc.
        /// </summary>
        /// <param name="educationOrganizationId">The educational organization that is the base for the hierarchical check.</param>
        /// <param name="claimType">The claim type to be checked.</param>
        /// <returns><b>true</b> if the user has the claim within the hierarchy of the specified education organization; otherwise <b>false</b>.</returns>
        public void CurrentUserMustHaveClaimWithinEducationOrganizationHierarchy(int educationOrganizationId,
                                                                                 string claimType)
        {
            if (currentUserClaimInterrogator.HasClaimWithinEducationOrganizationHierarchy(claimType,educationOrganizationId))
            {
                return;
            }

            throw new UserAccessDeniedException(NoEducationOrganizationPermissionErrorMessage);
        }

        /// <summary>
        /// Indicates whether the current user has the claim specified in the hierarchy of any of the schools of the student specified.
        /// </summary>
        /// <param name="studentUSI">The USI for the student who's schools we are going to check.</param>
        /// <param name="claimType">The claim type to be checked.</param>
        public void CurrentUserMustHaveClaimWithinHierarchyOfAnySchoolsOfStudent(long studentUSI, string claimType)
        {
            var schools = _authorizationInformationProvider.GetStudentSchoolIds(studentUSI);

            if (
                schools.Any(
                    school =>
                    currentUserClaimInterrogator.HasClaimWithinEducationOrganizationHierarchy(claimType, school)))
            {
                return;
            }

            throw new UserAccessDeniedException(NoStudentPermissionErrorMessage);
        }

        public void CurrentUserMustHaveLinkToStudentAtAnySchoolWithClaim(long student, string claimType)
        {
            var staff = UserInformation.Current.StaffUSI;
            var schools = _authorizationInformationProvider.GetStudentSchoolIds(student);

            if (
                schools
                    .Where(school => 
                        currentUserClaimInterrogator.HasClaimWithinEducationOrganizationHierarchy(claimType, school))
                    .Any(school => 
                        _authorizationInformationProvider.IsStudentAssociatedWithStaffAtSchool(student, staff, school)))
            {
                return;
            }

            throw new UserAccessDeniedException(NoStudentPermissionErrorMessage);
        }

        public void CurrentUserMustHaveClaimOnEducationOrganization(int educationOrganizationId, string claimType)
        {
            if (currentUserClaimInterrogator.HasClaimOnEducationOrganization(claimType, educationOrganizationId)) return;
            throw new UserAccessDeniedException(NoEducationOrganizationPermissionErrorMessage);
        }

        public IEnumerable<int> GetEducationOrganizationHierarchy(int educationOrganizationId)
        {
            return currentUserClaimInterrogator.GetEducationOrganizationHierarchy(educationOrganizationId);
        }

        #endregion

        #region Staff

        public void StaffMemberMustHaveLinkToStudent(long staffUsi, long studentUsi)
        {
            if ((!GetStudentUSIsForTeacher(staffUsi).Contains(studentUsi)) &&
                (!GetStudentUSIsForSpecialist(staffUsi).Contains(studentUsi)))
            {
            throw new UserAccessDeniedException(NoStudentPermissionErrorMessage);
        }
        }

        public void StaffTeacherMustBeAssociatedWithSection(long staffUSI, long sectionId)
        {
            var staffSections = GetSectionIdsForTeacher(staffUSI);

            if (staffSections.Contains(sectionId))
                return;

            throw new UserAccessDeniedException(NoSectionPermissionErrorMessage);
        }

        public void StaffMustBeAssociatedWithCohort(long staffUSI, long cohortId)
        {
            var staffCohorts = GetCohortIdsForStaff(staffUSI);

            if (staffCohorts.Contains(cohortId))
                return;

            throw new UserAccessDeniedException(NoCohortPermissionErrorMessage);
        }
        public void StaffMustBeAssociatedWithCustomStudentList(long staffUSI, long cohortId)
        {
            var staffCustomStudentLists = GetCustomStudentListIdsForStaff(staffUSI);

            if (staffCustomStudentLists.Contains(cohortId))
                return;

            throw new UserAccessDeniedException(NoCustomStudentListPermissionErrorMessage);
        }

        public void StaffMustBeAssociatedWithMetricsBasedWatchList(long staffUSI, long cohortId)
        {
            var staffMetricsBasedWatchLists = GetCustomMetricsBasedWatchListIdsForStaff(staffUSI);

            if (staffMetricsBasedWatchLists.Contains(cohortId))
                return;

            throw new UserAccessDeniedException(NoCustomMetricsBasedWatchListPermissionErrorMessage);
        }

        #endregion

        #region Local Education Agency

        public void LocalEducationAgencyMustBeAssociatedWithStaff(int localEducationAgencyId, long staffUSI)
        {
            

            //In the case that we could not validate the user against the Claim then we try to validate against the domain data.
            var localEducationAgencyStaff = GetStaffUSIsForLocalEducationAgency(localEducationAgencyId);
            if (localEducationAgencyStaff.Contains(staffUSI))
                return;

            //We should first test to see if we can validate against the Claim.
            if (currentUserClaimInterrogator.HasClaimWithinEducationOrganizationHierarchy(EdFiClaimTypes.AccessOrganization, localEducationAgencyId))
                return;

            throw new UserAccessDeniedException(NoStaffPermissionErrorMessage);
        }

        #endregion

        #region Education Organization

        public void EducationOrganizationMustBeAssociatedWithCohort(int educationOrganizationId, long cohortId)
        {
            var educationOrganizationCohorts = GetCohortIdsForEducationOrganization(educationOrganizationId);

            if (educationOrganizationCohorts.Contains(cohortId))
                return;

            throw new UserAccessDeniedException(NoCohortPermissionErrorMessage);
        }

        public void EducationOrganizationMustBeAssociatedWithCustomStudentList(int educationOrganizationId, long cohortId)
        {
            var educationOrganizationCustomStudentLists = GetCustomStudentListIdsForEducationOrganization(educationOrganizationId);

            if (educationOrganizationCustomStudentLists.Contains(cohortId))
                return;

            throw new UserAccessDeniedException(NoCustomStudentListPermissionErrorMessage);
        }

        public void EducationOrganizationMustBeAssociatedWithCustomMetricsBasedWatchList(int educationOrganizationId, long cohortId)
        {
            var educationOrganizationCustomMetricsBasedWatchLists = GetCustomMetricsBasedWatchListForEducationOrganization(educationOrganizationId);

            if (educationOrganizationCustomMetricsBasedWatchLists.Contains(cohortId))
                return;

            throw new UserAccessDeniedException(NoCustomMetricsBasedWatchListPermissionErrorMessage);
        }
    
        #endregion

        #region Gets

        public List<long> GetSectionsForStaffAtSchool(int staff, int school)
        {
            var schoolSections = GetSectionIdsForSchool(staff);
            var staffSections = GetSectionIdsForTeacher(staff);

            var result = from section in staffSections
                         where schoolSections.Contains(section)
                         select section;

            return result.ToList();
        }

        public List<long> GetCohortsForStaffAtSchool(int staff, int school)
        {
            var schoolCohorts = GetCohortIdsForEducationOrganization(staff);
            var staffCohorts = GetCohortIdsForStaff(staff);

            var result = from cohort in staffCohorts
                         where schoolCohorts.Contains(cohort)
                         select cohort;

            return result.ToList();
        }

        public HashSet<long> GetAllStudentsForAllUserOrganizations(UserInformation userInfo)
        {
            var result = new HashSet<long>();

            var userOrgs = userInfo.AssociatedOrganizations;
            foreach (var userOrg in userOrgs)
            {
                var schoolStudents = GetStudentUSIsForSchool(userOrg.EducationOrganizationId);
                result.UnionWith(schoolStudents);
            }

            return result;
        }

        private List<long> GetStudentUSIsForSchool(int schoolId)
        {
            if (!studentUSIsBySchoolId.ContainsKey(schoolId))
            {
                var studentUSIs = _authorizationInformationProvider.GetSchoolStudentUSIs(schoolId);
                studentUSIsBySchoolId[schoolId] = studentUSIs;
            }

            return studentUSIsBySchoolId[schoolId];
        }

        private List<long> GetStaffUSIsForSchool(int schoolId)
        {
            if (!staffUSIsBySchoolId.ContainsKey(schoolId))
            {
                var staffUSIs = _authorizationInformationProvider.GetSchoolStaffUSIs(schoolId);
                staffUSIsBySchoolId[schoolId] = staffUSIs;
            }

            return staffUSIsBySchoolId[schoolId];
        }

        private List<long> GetStaffUSIsForLocalEducationAgency(int localEducationAgencyId)
        {
            if (!staffUSIsByLocalEducationAgencyId.ContainsKey(localEducationAgencyId))
            {
                var staffUSIs = _authorizationInformationProvider.GetLocalEducationAgencyStaffUSIs(localEducationAgencyId);
                staffUSIsByLocalEducationAgencyId[localEducationAgencyId] = staffUSIs;
            }

            return staffUSIsByLocalEducationAgencyId[localEducationAgencyId];
        }

        private List<long> GetSectionIdsForSchool(int schoolId)
        {
            if (!sectionIdsBySchoolId.ContainsKey(schoolId))
            {
                var sectionIds = _authorizationInformationProvider.GetSchoolSectionIds(schoolId);
                sectionIdsBySchoolId[schoolId] = sectionIds;
            }

            return sectionIdsBySchoolId[schoolId];
        }

        protected List<long> GetCohortIdsForStaff(long staffUSI)
        {
            if (cohortIdsByStaff.ContainsKey(staffUSI))
                return cohortIdsByStaff[staffUSI];

            var cohortIds = _authorizationInformationProvider.GetStaffCohortIds(staffUSI);
            cohortIdsByStaff[staffUSI] = cohortIds;

            return cohortIdsByStaff[staffUSI];
        }

        protected List<long> GetCustomStudentListIdsForStaff(long staffUSI)
        {
            var customStudentListIds = _authorizationInformationProvider.GetStaffCustomStudentListIds(staffUSI);
            customStudentListIdsByStaff[staffUSI] = customStudentListIds;

            return customStudentListIdsByStaff[staffUSI];
        }

        protected List<long> GetCustomMetricsBasedWatchListIdsForStaff(long staffUSI)
        {
            var customMetricsBasedWatchListIds = _authorizationInformationProvider.GetStaffCustomMetricsBasedWatchListIds(staffUSI);
            customMetricsBasedWatchListIdsByStaff[staffUSI] = customMetricsBasedWatchListIds;

            return customMetricsBasedWatchListIdsByStaff[staffUSI];
        } 

        protected List<long> GetSectionIdsForTeacher(long staffUSI)
        {
            if (!sectionIdsByTeacher.ContainsKey(staffUSI))
            {
                var sectionIds = _authorizationInformationProvider.GetTeacherSectionIds(staffUSI);
                sectionIdsByTeacher[staffUSI] = sectionIds;
            }

            return sectionIdsByTeacher[staffUSI];
        }

        protected List<long> GetStudentUSIsForTeacher(long staffUSI)
        {
            if (!studentUSIsByTeacher.ContainsKey(staffUSI))
            {
                var studentUSIs = _authorizationInformationProvider.GetTeacherStudentUSIs(staffUSI);
                studentUSIsByTeacher[staffUSI] = studentUSIs;
                return studentUSIs;
            }

            return studentUSIsByTeacher[staffUSI];
        }

        protected List<long> GetStudentUSIsForSpecialist(long staffUSI)
        {
            if (!studentUSIsBySpecialist.ContainsKey(staffUSI))
            {
                var studentUSIs = _authorizationInformationProvider.GetStaffCohortStudentUSIs(staffUSI);
                studentUSIsBySpecialist[staffUSI] = studentUSIs;
                return studentUSIs;
            }

            return studentUSIsBySpecialist[staffUSI];
        }

        private List<long> GetCohortIdsForEducationOrganization(int educationOrganizationId)
        {
            if (!cohortIdsByEducationOrganizationId.ContainsKey(educationOrganizationId))
            {
                var cohortIds = _authorizationInformationProvider.GetEducationOrganizationCohortIds(educationOrganizationId);
                cohortIdsByEducationOrganizationId[educationOrganizationId] = cohortIds;
            }

            return cohortIdsByEducationOrganizationId[educationOrganizationId];
        }
        private List<long> GetCustomStudentListIdsForEducationOrganization(int educationOrganizationId)
        {
            var customStudentListIds = _authorizationInformationProvider.GetEducationOrganizationCustomStudentListIds(educationOrganizationId);
            customStudentListIdsByEducationOrganizationId[educationOrganizationId] = customStudentListIds;

            return customStudentListIdsByEducationOrganizationId[educationOrganizationId];
        }

        private List<long> GetCustomMetricsBasedWatchListForEducationOrganization(int educationOrganizationId)
        {
            var customMetricsBasedWatchListIds = _authorizationInformationProvider.GetEducationOrganizationCustomMetricsBasedWatchListIds(educationOrganizationId);
            customMetricsBasedWatchListIdsByEducationOrganizationId[educationOrganizationId] = customMetricsBasedWatchListIds;

            return customMetricsBasedWatchListIdsByEducationOrganizationId[educationOrganizationId];
        }

        #endregion
    }
}
