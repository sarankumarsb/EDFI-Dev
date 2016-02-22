// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;
using System.Linq;
using EdFi.Dashboards.Common;
using EdFi.Dashboards.Resources.Models.Common;
using EdFi.Dashboards.Resources.Models.Staff;
using EdFi.Dashboards.SecurityTokenService.Authentication;

namespace EdFi.Dashboards.Resources.Security.ClaimValidators
{
    public abstract class AccessOrganizationClaimValidator : ClaimValidatorBase
    {
        protected AccessOrganizationClaimValidator(ISecurityAssertionProvider securityAssertionProvider, IClaimValidator next) :
            base(securityAssertionProvider, next)
        {
            ClaimType = EdFiClaimTypes.AccessOrganization;
        }

        protected void CheckLea(ClaimValidatorRequest request)
        {
            var lea = request.GetNullableIdByName(ClaimValidatorRequest.LocalEducationAgencyParameterName);
            if (null == lea) return;

            var userInfo = UserInformation.Current;
            //Look at LEA or higher.
            var claimValidatorLeaEdorgs = SecurityAssertionProvider.GetEducationOrganizationHierarchy((int)lea);
            if (userInfo.AssociatedOrganizations.Any(
                n => ((n.ClaimTypes.Contains(ClaimType)) &&
                    (claimValidatorLeaEdorgs.Contains(n.EducationOrganizationId) || //The user has an explict claim for the LEA or State level
                    (SecurityAssertionProvider.GetEducationOrganizationHierarchy(n.EducationOrganizationId).Contains((int)lea)))))) //The user has the lea within the hierarchy of one of thier claims.
                //Note: Can not look higher than LEA in this last check because the State agency ID always comes back in the hierarchy.
                return;

            throw new UserAccessDeniedException(Implementations.SecurityAssertionProvider.NoEducationOrganizationPermissionErrorMessage);
        }
    }

    public class AccessOrganizationClaimValidatorLocalEducationAgency : AccessOrganizationClaimValidator
    {
        public AccessOrganizationClaimValidatorLocalEducationAgency(ISecurityAssertionProvider securityAssertionProvider, IClaimValidator next) :
            base(securityAssertionProvider, next)
        {
            HandledSignatureKey = ClaimValidatorRequest.LocalEducationAgencySignature;
        }

        protected override object HandleRequest(ClaimValidatorRequest request)
        {
            CheckLea(request);

            return null;
        }
    }

    public class AccessOrganizationClaimValidatorSchool : AccessOrganizationClaimValidator
    {
        public AccessOrganizationClaimValidatorSchool(ISecurityAssertionProvider securityAssertionProvider, IClaimValidator next) :
            base(securityAssertionProvider, next)
        {
            HandledSignatureKey = ClaimValidatorRequest.SchoolSignature;
        }

        protected override object HandleRequest(ClaimValidatorRequest request)
        {
            ValidateClaimSchool(request, ClaimType);

            return null;
        }
    }

    public class AccessOrganizationClaimValidatorSchoolStaff : AccessOrganizationClaimValidator
    {
        public AccessOrganizationClaimValidatorSchoolStaff(ISecurityAssertionProvider securityAssertionProvider, IClaimValidator next) :
            base(securityAssertionProvider, next)
        {
            HandledSignatureKey = ClaimValidatorRequest.SchoolStaffSignature;
        }

        protected override object HandleRequest(ClaimValidatorRequest request)
        {
            //ValidateClaimSchoolStaff(request, ClaimType);
            ValidateClaimSchoolUser(request, ClaimType);

            return null;
        }
    }

    //N.B. The LocalEducationAgency and School parameters for the LevelCrumbService are nullable.
    //If the school is non null test the school.
    //If the school is null and lea is non null, test lea.
    //If school and lea are null, throw an exception.
    //
    public class AccessOrganizationClaimValidatorLocalEducationAgencySchool : AccessOrganizationClaimValidator
    {
        public AccessOrganizationClaimValidatorLocalEducationAgencySchool(ISecurityAssertionProvider securityAssertionProvider, IClaimValidator next) :
            base(securityAssertionProvider, next)
        {
            HandledSignatureKey = ClaimValidatorRequest.LocalEducationAgencySchoolSignature;
        }

        protected override object HandleRequest(ClaimValidatorRequest request)
        {
            var schoolId = request.GetParameterValueByName(ClaimValidatorRequest.SchoolParameterName);
            if (schoolId != null)
            {
                ValidateClaimSchool(request, ClaimType);
                return null;
            }

            var leaId = request.GetParameterValueByName(ClaimValidatorRequest.LocalEducationAgencyParameterName);
            if (leaId != null)
            {
                CheckLea(request);
                return null;
            }

            throw new ArgumentException("SchoolId and LocalEducationAgency can not both be null.");
        }
    }

    public class AccessOrganizationClaimValidatorSchoolStaffStudentList : AccessOrganizationClaimValidator
    {
        public AccessOrganizationClaimValidatorSchoolStaffStudentList(ISecurityAssertionProvider securityAssertionProvider, IClaimValidator next) :
            base(securityAssertionProvider, next)
        {
            HandledSignatureKey = ClaimValidatorRequest.SchoolStaffStudentListSignature;
        }

        protected override object HandleRequest(ClaimValidatorRequest request)
        {

            ValidateCurrentUserIsStaff(request);

            var studentListType = request.GetParameterValueByName(ClaimValidatorRequest.StudentListTypeParameterName);

            // DJWhite 4 Jan 2012: Default null or empty to NONE which is equivalent to ALL.
            if (String.IsNullOrEmpty(studentListType)) studentListType = StudentListType.None.ToString();

            switch (studentListType.ToLower())
            {
                case ClaimValidatorRequest.StudentListEnumSection:
                    ValidateClaimSchoolStaffSection(request, ClaimType);
                    break;
                case ClaimValidatorRequest.StudentListEnumCohort:
                    ValidateClaimSchoolCohortStaff(request, ClaimType);
                    break;
                case ClaimValidatorRequest.StudentListEnumCustomStudentList:
                    ValidateClaimSchoolCustomStudentListStaff(request, ClaimType);
                    break;
                case ClaimValidatorRequest.StudentListEnumMetricsBasedWatchList:
                case ClaimValidatorRequest.CustomMetricsBasedWatchListParameterName:
                    ValidateClaimSchoolCustomMetricsBasedWatchListStaff(request, ClaimType);
                    break;
                case ClaimValidatorRequest.StudentListEnumAll:
                case ClaimValidatorRequest.StudentListEnumNone:
                    ValidateClaimSchoolStaff(request, ClaimType);
                    break;
                default:
                    throw new UserAccessDeniedException(ClaimValidatorRequest.InvalidParameterErrorMessage);
            }

            return null;
        }
    }

    public class AccessOrganizationClaimValidatorLocalEducationAgencyStaff : AccessOrganizationClaimValidator
    {
        public AccessOrganizationClaimValidatorLocalEducationAgencyStaff(ISecurityAssertionProvider securityAssertionProvider, IClaimValidator next) :
            base(securityAssertionProvider, next)
        {
            HandledSignatureKey = ClaimValidatorRequest.LocalEducationAgencyStaffSignature;
        }

        protected override object HandleRequest(ClaimValidatorRequest request)
        {
            ValidateClaimLocalEducationAgencyStaff(request, ClaimType);

            return null;
        }
    }
}
