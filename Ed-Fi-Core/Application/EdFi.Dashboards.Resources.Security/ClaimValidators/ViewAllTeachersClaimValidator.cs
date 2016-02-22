// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using EdFi.Dashboards.SecurityTokenService.Authentication;
using EdFi.Dashboards.Resources.Security.Implementations;

namespace EdFi.Dashboards.Resources.Security.ClaimValidators
{
    public abstract class ViewAllTeachersClaimValidator : ClaimValidatorBase
    {
        protected ViewAllTeachersClaimValidator(ISecurityAssertionProvider securityAssertionProvider, IClaimValidator next) :
            base(securityAssertionProvider, next)
        {
            ClaimType = EdFiClaimTypes.ViewAllTeachers;
        }
    }

    public class ViewAllTeachersClaimValidatorLocalEducationAgency : ViewAllTeachersClaimValidator
    {
        public ViewAllTeachersClaimValidatorLocalEducationAgency(ISecurityAssertionProvider securityAssertionProvider, IClaimValidator next) :
            base(securityAssertionProvider, next)
        {
            HandledSignatureKey = ClaimValidatorRequest.LocalEducationAgencySignature;
        }

        protected override object HandleRequest(ClaimValidatorRequest request)
        {
            ValidateClaimLocalEducationAgency(request, ClaimType);

            return null;
        }
    }

    public class ViewAllTeachersClaimValidatorSchool : ViewAllTeachersClaimValidator
    {
        public ViewAllTeachersClaimValidatorSchool(ISecurityAssertionProvider securityAssertionProvider, IClaimValidator next) :
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

    public class ViewAllTeachersClaimValidatorStaff : ViewAllTeachersClaimValidator
    {
        public ViewAllTeachersClaimValidatorStaff(ISecurityAssertionProvider securityAssertionProvider, IClaimValidator next) :
            base(securityAssertionProvider, next)
        {
            HandledSignatureKey = ClaimValidatorRequest.StaffSignature;
        }

        protected override object HandleRequest(ClaimValidatorRequest request)
        {
            ValidateClaimStaff(request, ClaimType);

            return null;
        }
    }

    public class ViewAllTeachersClaimValidatorSchoolStaff : ViewAllTeachersClaimValidator
    {
        public ViewAllTeachersClaimValidatorSchoolStaff(ISecurityAssertionProvider securityAssertionProvider, IClaimValidator next) :
            base(securityAssertionProvider, next)
        {
            HandledSignatureKey = ClaimValidatorRequest.SchoolStaffSignature;
        }

        protected override object HandleRequest(ClaimValidatorRequest request)
        {
            ValidateClaimSchoolStaff(request, ClaimType);

            return null;
        }
    }
}
