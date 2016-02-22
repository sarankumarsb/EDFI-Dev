// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;
using EdFi.Dashboards.Common;
using EdFi.Dashboards.SecurityTokenService.Authentication;

namespace EdFi.Dashboards.Resources.Security.ClaimValidators
{
    public abstract class ClaimValidatorBase : ChainOfResponsibilityBase<IClaimValidator, ClaimValidatorRequest, object>, IClaimValidator
    {
        protected ClaimValidatorBase(ISecurityAssertionProvider securityAssertionProvider, IClaimValidator next)
            : base(next)
        {
            SecurityAssertionProvider = securityAssertionProvider;
            Next = next;
        }

        public string HandledSignatureKey { get; set; }
        public string ClaimType { get; set; }
        public ISecurityAssertionProvider SecurityAssertionProvider { get; set; }

        /// <summary>
        /// Indicates whether the current implementation can handle the signature of the method invocation 
        /// (the parameter set being passed).
        /// </summary>
        /// <param name="request">The request needing validation for the current method invocation.</param>
        /// <returns><b>true</b> if the implementation can handle the validation; otherwise <b>false</b>.</returns>
        protected override bool CanHandleRequest(ClaimValidatorRequest request)
        {
            var signatureKeyToBeValidated = request.BuildSignatureKey();

            return signatureKeyToBeValidated == HandledSignatureKey;
        }

        public object ValidateRequest(ClaimValidatorRequest request)
        {
            return ProcessRequest(request);
        }

        public string GetValidatorKey()
        {

            var signature = HandledSignatureKey;
            var claimName = ClaimHelper.GetClaimShortName(ClaimType);

            var key = string.Format("{0}:{1}", claimName, signature).ToLower();

            return key;
        }

        public object ValidateLocalEducationAgencySchoolIfPresent(ClaimValidatorRequest request)
        {
            if (request.FindParameterByName(ClaimValidatorRequest.LocalEducationAgencyParameterName))
            {
                var schoolId = request.GetSchoolId();
                var leaId = request.GetLocalEducationAgencyId();
                if (leaId != 0)
                    SecurityAssertionProvider.SchoolMustBeAssociatedWithLocalEducationAgency(schoolId, leaId);
            }

            return null;
        }

        public object ValidateCurrentUserIsStaff(ClaimValidatorRequest request)
        {
            var presentStaff = request.GetNullableLongIdByName(ClaimValidatorRequest.StaffParameterName);
            if (!presentStaff.HasValue)
                return null;
            var currentStaff = UserInformation.Current.StaffUSI;
            if (presentStaff == currentStaff) return null;

            throw new UserAccessDeniedException(Implementations.SecurityAssertionProvider.NoStaffPermissionErrorMessage);
        }
        
        public object ValidateClaimLocalEducationAgency(ClaimValidatorRequest request, string claimType)
        {
            var leaId = request.GetLocalEducationAgencyId();
            CheckRequestForValidId(leaId, "LocalEducationAgencyId");
            SecurityAssertionProvider.CurrentUserMustHaveClaimWithinEducationOrganizationHierarchy(leaId, claimType);

            return null;
        }

        public object ValidateClaimLocalEducationAgencyMetric(ClaimValidatorRequest request, string claimType)
        {
            var leaId = request.GetLocalEducationAgencyId();
            CheckRequestForValidId(leaId, "LocalEducationAgencyId");
            SecurityAssertionProvider.CurrentUserMustHaveClaimWithinEducationOrganizationHierarchy(leaId, claimType);

            return null;
        }

        public object ValidateClaimSchool(ClaimValidatorRequest request, string claimType)
        {
            var schoolId = request.GetSchoolId();
            CheckRequestForValidId(schoolId, "SchoolId");
            SecurityAssertionProvider.CurrentUserMustHaveClaimWithinEducationOrganizationHierarchy(schoolId, claimType);
            return null;
        }

        public object ValidateClaimSchoolStaffSection(ClaimValidatorRequest request, string claimType)
        {
            var schoolId = request.GetSchoolId();
            var sectionId = request.GetSectionId();
            var staffUSI = request.GetStaffUSI();

            CheckRequestForValidId(schoolId, "SchoolId");
            CheckRequestForValidId(sectionId, "SectionId");
            CheckRequestForValidId(staffUSI, "StaffUSI");

            SecurityAssertionProvider.CurrentUserMustHaveClaimWithinEducationOrganizationHierarchy(schoolId, claimType);
            SecurityAssertionProvider.StaffTeacherMustBeAssociatedWithSection(staffUSI, sectionId);
            SecurityAssertionProvider.SchoolMustBeAssociatedWithSection(schoolId, sectionId);

            //DJWhite, 9 Dec 2011.  This is redundant, as the school association is explicit in the claims.
            //SecurityAssertionProvider.SchoolMustBeAssociatedWithStaff(request);

            ValidateLocalEducationAgencySchoolIfPresent(request);

            return null;
        }

        public object ValidateClaimSchoolCohortStaff(ClaimValidatorRequest request, string claimType)
        {
            var schoolId = request.GetSchoolId();
            var cohortId = request.GetCohortId();
            var staffUSI = request.GetStaffUSI();

            CheckRequestForValidId(schoolId, "SchoolId");
            CheckRequestForValidId(cohortId, "CohortId");
            CheckRequestForValidId(staffUSI, "StaffUSI");

            SecurityAssertionProvider.CurrentUserMustHaveClaimWithinEducationOrganizationHierarchy(schoolId, claimType);
            SecurityAssertionProvider.StaffMustBeAssociatedWithCohort(staffUSI, cohortId);
            SecurityAssertionProvider.EducationOrganizationMustBeAssociatedWithCohort(schoolId, cohortId);

            //DJWhite, 9 Dec 2011.  This is redundant, as the school association is implicit in the claims.
            //SecurityAssertionProvider.SchoolMustBeAssociatedWithStaff(request);

            ValidateLocalEducationAgencySchoolIfPresent(request);

            return null;
        }

        public object ValidateClaimLocalEducationAgencyCohortStaff(ClaimValidatorRequest request, string claimType)
        {
            var localEducationAgencyId = request.GetLocalEducationAgencyId();
            var cohortId = request.GetCohortId();
            var staffUSI = request.GetStaffUSI();

            CheckRequestForValidId(localEducationAgencyId, "LocalEducationAgencyId");
            CheckRequestForValidId(cohortId, "CohortId");
            CheckRequestForValidId(staffUSI, "StaffUSI");

            SecurityAssertionProvider.CurrentUserMustHaveClaimWithinEducationOrganizationHierarchy(localEducationAgencyId, claimType);
            SecurityAssertionProvider.StaffMustBeAssociatedWithCohort(staffUSI, cohortId);
            SecurityAssertionProvider.EducationOrganizationMustBeAssociatedWithCohort(localEducationAgencyId, cohortId);

            return null;
        }

        public object ValidateClaimLocalEducationAgencyCohort(ClaimValidatorRequest request, string claimType)
        {
            var localEducationAgencyId = request.GetLocalEducationAgencyId();
            var cohortId = request.GetCohortId();

            CheckRequestForValidId(localEducationAgencyId, "LocalEducationAgencyId");
            CheckRequestForValidId(cohortId, "CohortId");

            SecurityAssertionProvider.CurrentUserMustHaveClaimWithinEducationOrganizationHierarchy(localEducationAgencyId, claimType);
            SecurityAssertionProvider.EducationOrganizationMustBeAssociatedWithCohort(localEducationAgencyId, cohortId);

            return null;
        }

        public object ValidateClaimSchoolCustomStudentList(ClaimValidatorRequest request, string claimType)
        {
            var schoolId = request.GetSchoolId();
            var customStudentListId = request.GetCustomStudentListId();

            CheckRequestForValidId(schoolId, "SchoolId");
            CheckRequestForValidId(customStudentListId, "CustomStudentListId");

            SecurityAssertionProvider.EducationOrganizationMustBeAssociatedWithCustomStudentList(schoolId, customStudentListId);
            ValidateLocalEducationAgencySchoolIfPresent(request);

            return null;
        }

        public object ValidateClaimSchoolCustomStudentListStaff(ClaimValidatorRequest request, string claimType)
        {
            var schoolId = request.GetSchoolId();
            var customStudentListId = request.GetCustomStudentListId();
            var staffUSI = request.GetStaffUSI();

            CheckRequestForValidId(schoolId, "SchoolId");
            CheckRequestForValidId(customStudentListId, "CustomStudentListId");
            CheckRequestForValidId(staffUSI, "StaffUSI");

            SecurityAssertionProvider.CurrentUserMustHaveClaimWithinEducationOrganizationHierarchy(schoolId, claimType);
            SecurityAssertionProvider.StaffMustBeAssociatedWithCustomStudentList(staffUSI, customStudentListId);
            SecurityAssertionProvider.EducationOrganizationMustBeAssociatedWithCustomStudentList(schoolId, customStudentListId);
            
            ValidateLocalEducationAgencySchoolIfPresent(request);

            return null;
        }

        public object ValidateClaimSchoolCustomMetricsBasedWatchListStaff(ClaimValidatorRequest request, string claimType)
        {
            var schoolId = request.GetSchoolId();
            var customMetricsBasedWatchListId = request.GetCustomMetricsBasedWatchListId();
            var staffUSI = UserInformation.Current.StaffUSI;

            CheckRequestForValidId(schoolId, "SchoolId");
            CheckRequestForValidId(customMetricsBasedWatchListId, "MetricBasedWatchListId");

            SecurityAssertionProvider.CurrentUserMustHaveClaimWithinEducationOrganizationHierarchy(schoolId, claimType);
            SecurityAssertionProvider.StaffMustBeAssociatedWithMetricsBasedWatchList(staffUSI, customMetricsBasedWatchListId);
            SecurityAssertionProvider.EducationOrganizationMustBeAssociatedWithCustomMetricsBasedWatchList(schoolId, customMetricsBasedWatchListId);

            ValidateLocalEducationAgencySchoolIfPresent(request);

            return null;
        }

        public object ValidateClaimSchoolCustomMetricsBasedWatchListStaffIfPresent(ClaimValidatorRequest request, string claimType)
        {
            var schoolId = request.GetSchoolId();
            var customMetricsBasedWatchListId = request.GetCustomMetricsBasedWatchListId();
            var staffUSI = UserInformation.Current.StaffUSI;
            var watchListIdPresent =
                request.FindParameterByName(ClaimValidatorRequest.CustomMetricsBasedWatchListParameterName) &&
                customMetricsBasedWatchListId > 0;

            CheckRequestForValidId(schoolId, "SchoolId");

            if (watchListIdPresent)
                CheckRequestForValidId(customMetricsBasedWatchListId, "MetricBasedWatchListId");

            SecurityAssertionProvider.CurrentUserMustHaveClaimWithinEducationOrganizationHierarchy(schoolId, claimType);

            if (watchListIdPresent)
            {
                SecurityAssertionProvider.StaffMustBeAssociatedWithMetricsBasedWatchList(staffUSI, customMetricsBasedWatchListId);
                SecurityAssertionProvider.EducationOrganizationMustBeAssociatedWithCustomMetricsBasedWatchList(schoolId, customMetricsBasedWatchListId);
            }

            ValidateLocalEducationAgencySchoolIfPresent(request);

            return null;
        }

        public object ValidateClaimLocalEducationAgencyCustomMetricsBasedWatchListStaff(ClaimValidatorRequest request, string claimType)
        {
            var localEducationAgencyId = request.GetLocalEducationAgencyId();
            var customMetricsBasedWatchListId = request.GetCustomMetricsBasedWatchListId();
            var staffUSI = UserInformation.Current.StaffUSI;

            CheckRequestForValidId(localEducationAgencyId, "LocalEducationAgencyId");
            CheckRequestForValidId(customMetricsBasedWatchListId, "MetricBasedWatchListId");

            SecurityAssertionProvider.CurrentUserMustHaveClaimWithinEducationOrganizationHierarchy(localEducationAgencyId, claimType);
            SecurityAssertionProvider.StaffMustBeAssociatedWithMetricsBasedWatchList(staffUSI, customMetricsBasedWatchListId);
            SecurityAssertionProvider.EducationOrganizationMustBeAssociatedWithCustomMetricsBasedWatchList(localEducationAgencyId, customMetricsBasedWatchListId);

            return null;
        }
        

        public object ValidateClaimLocalEducationAgencyCustomMetricsBasedWatchListStaffIfPresent(ClaimValidatorRequest request, string claimType)
        {
            var localEducationAgencyId = request.GetLocalEducationAgencyId();
            var customMetricsBasedWatchListId = request.GetCustomMetricsBasedWatchListId();
            var staffUSI = UserInformation.Current.StaffUSI;
            var watchListIdPresent =
                request.FindParameterByName(ClaimValidatorRequest.CustomMetricsBasedWatchListParameterName) &&
                customMetricsBasedWatchListId > 0;

            CheckRequestForValidId(localEducationAgencyId, "LocalEducationAgencyId");

            if (watchListIdPresent)
                CheckRequestForValidId(customMetricsBasedWatchListId, "MetricBasedWatchListId");

            SecurityAssertionProvider.CurrentUserMustHaveClaimWithinEducationOrganizationHierarchy(localEducationAgencyId, claimType);

            if (!watchListIdPresent) return null;

            SecurityAssertionProvider.StaffMustBeAssociatedWithMetricsBasedWatchList(staffUSI, customMetricsBasedWatchListId);
            SecurityAssertionProvider.EducationOrganizationMustBeAssociatedWithCustomMetricsBasedWatchList(localEducationAgencyId, customMetricsBasedWatchListId);

			return null;
		}

        public object ValidateClaimLocalEducationAgencyCustomStudentListStaff(ClaimValidatorRequest request, string claimType)
        {
            var localEducationAgencyId = request.GetLocalEducationAgencyId();
            var customStudentListId = request.GetCustomStudentListId();
            var staffUSI = request.GetStaffUSI();

            CheckRequestForValidId(localEducationAgencyId, "LocalEducationAgencyId");
            CheckRequestForValidId(customStudentListId, "CustomStudentListId");
            CheckRequestForValidId(staffUSI, "StaffUSI");

            SecurityAssertionProvider.CurrentUserMustHaveClaimWithinEducationOrganizationHierarchy(localEducationAgencyId, claimType);
            SecurityAssertionProvider.StaffMustBeAssociatedWithCustomStudentList(staffUSI, customStudentListId);
            SecurityAssertionProvider.EducationOrganizationMustBeAssociatedWithCustomStudentList(localEducationAgencyId, customStudentListId);

            return null;
        }

        // this version checks claims and then database
        public object ValidateClaimSchoolStaff(ClaimValidatorRequest request, string claimType)
        {
            var schoolId = request.GetSchoolId();
            var staffUSI = request.GetStaffUSI();

            CheckRequestForValidId(schoolId, "SchoolId");
            CheckRequestForValidId(staffUSI, "StaffUSI");

            SecurityAssertionProvider.CurrentUserMustHaveClaimWithinEducationOrganizationHierarchy(schoolId, claimType);
            SecurityAssertionProvider.SchoolMustBeAssociatedWithStaff(schoolId, staffUSI);

            ValidateLocalEducationAgencySchoolIfPresent(request);

            return null;
        }

        // this version only checks claims
        public object ValidateClaimSchoolUser(ClaimValidatorRequest request, string claimType)
        {
            var schoolId = request.GetSchoolId();

            CheckRequestForValidId(schoolId, "SchoolId");

            SecurityAssertionProvider.CurrentUserMustHaveClaimWithinEducationOrganizationHierarchy(schoolId, claimType);

            ValidateLocalEducationAgencySchoolIfPresent(request);

            return null;
        }

        public object ValidateClaimLocalEducationAgencyStaff(ClaimValidatorRequest request, string claimType)
        {
            var localEducationAgencyId = request.GetLocalEducationAgencyId();
            var staffUSI = request.GetStaffUSI();

            CheckRequestForValidId(localEducationAgencyId, "LocalEducationAgencyId");
            CheckRequestForValidId(staffUSI, "StaffUSI");

            SecurityAssertionProvider.CurrentUserMustHaveClaimWithinEducationOrganizationHierarchy(localEducationAgencyId, claimType);
            SecurityAssertionProvider.LocalEducationAgencyMustBeAssociatedWithStaff(localEducationAgencyId, staffUSI);

            return null;
        }

        public object ValidateClaimSchoolStudentByOrganizationAssociation(ClaimValidatorRequest request, string claimType)
        {
            var schoolId = request.GetSchoolId();
            var studentUSI = request.GetStudentUSI();

            CheckRequestForValidId(schoolId, "SchoolId");
            CheckRequestForValidId(studentUSI, "StudentUSI");

            SecurityAssertionProvider.CurrentUserMustHaveClaimWithinEducationOrganizationHierarchy(schoolId, claimType);
            SecurityAssertionProvider.SchoolMustBeAssociatedWithStudent(schoolId, studentUSI);

            ValidateLocalEducationAgencySchoolIfPresent(request);

            return null;
        }

        public object ValidateClaimSchoolStudentByStudentListAssociation(ClaimValidatorRequest request, string claimType)
        {
            var schoolId = request.GetSchoolId();
            var staff = UserInformation.Current.StaffUSI;
            var studentUSI = request.GetStudentUSI();

            CheckRequestForValidId(schoolId, "SchoolId");
            CheckRequestForValidId(studentUSI, "StudentUSI");

            SecurityAssertionProvider.CurrentUserMustHaveClaimWithinEducationOrganizationHierarchy(schoolId, claimType);
            // EDFIDB-139
            if (UserInformation.Current.UserType == 1)
                SecurityAssertionProvider.StaffMemberMustHaveLinkToStudent(staff, studentUSI);

            ValidateLocalEducationAgencySchoolIfPresent(request);

            return null;
        }

        public object ValidateClaimSchoolSection(ClaimValidatorRequest request, string claimType)
        {
            var schoolId = request.GetSchoolId();
            var sectionId = request.GetSectionId();

            CheckRequestForValidId(schoolId, "SchoolId");
            CheckRequestForValidId(sectionId, "SectionId");

            SecurityAssertionProvider.CurrentUserMustHaveClaimWithinEducationOrganizationHierarchy(schoolId, claimType);
            SecurityAssertionProvider.SchoolMustBeAssociatedWithSection(schoolId, sectionId);

            ValidateLocalEducationAgencySchoolIfPresent(request);

            return null;
        }

        public object ValidateClaimSchoolCohort(ClaimValidatorRequest request, string claimType)
        {
            var schoolId = request.GetSchoolId();
            var cohortId = request.GetCohortId();

            CheckRequestForValidId(schoolId, "SchoolId");
            CheckRequestForValidId(cohortId, "CohortId");

            SecurityAssertionProvider.CurrentUserMustHaveClaimWithinEducationOrganizationHierarchy(schoolId, claimType);
            SecurityAssertionProvider.EducationOrganizationMustBeAssociatedWithCohort(schoolId, cohortId);

            ValidateLocalEducationAgencySchoolIfPresent(request);

            return null;
        }

        public object ValidateClaimStaff(ClaimValidatorRequest request, string claimType)
        {
            var staffUSI = request.GetStaffUSI();
            CheckRequestForValidId(staffUSI, "StaffUSI");
            SecurityAssertionProvider.CurrentUserMustHaveClaimOnStaff(staffUSI, claimType);

            return null;
        }

        public object ValidateClaimStaffWatchList(ClaimValidatorRequest request, string claimType)
        {
            var staffUSI = request.GetStaffUSI();
            var metricsBasedWatchListId = request.GetCustomMetricsBasedWatchListId();

            CheckRequestForValidId(staffUSI, "StaffUSI");
            CheckRequestForValidId(metricsBasedWatchListId, "MetricBasedWatchListId");

            SecurityAssertionProvider.CurrentUserMustHaveClaimOnStaff(staffUSI, claimType);
            SecurityAssertionProvider.StaffMustBeAssociatedWithMetricsBasedWatchList(staffUSI, metricsBasedWatchListId);

            return null;
        }

        public object ValidateClaimStudentByOrganizationAssociation(ClaimValidatorRequest request, string claimType)
        {
            var studentUSI = request.GetStudentUSI();
            CheckRequestForValidId(studentUSI, "StudentUSI");
            SecurityAssertionProvider.CurrentUserMustHaveClaimWithinHierarchyOfAnySchoolsOfStudent(studentUSI, claimType);

            return null;
        }

        // The specified student must be associated with the current user at a specific school.
        public object ValidateClaimStudentByCurrentUser(ClaimValidatorRequest request, string claimType)
        {
            var studentUSI = request.GetStudentUSI();
            CheckRequestForValidId(studentUSI, "StudentUSI");
            SecurityAssertionProvider.CurrentUserMustHaveLinkToStudentAtAnySchoolWithClaim(studentUSI, claimType);

            return null;
        }

		private static void CheckRequestForValidId(long idValue, string idName)
        {
            if (idValue == default(long))
                throw new ArgumentException(string.Format("Unable to validate claims against an uninitialized {0} value.", idName));
        }

    }
}
