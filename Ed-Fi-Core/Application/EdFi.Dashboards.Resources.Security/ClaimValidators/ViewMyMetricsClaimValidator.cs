// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using EdFi.Dashboards.SecurityTokenService.Authentication;

namespace EdFi.Dashboards.Resources.Security.ClaimValidators
{
    public abstract class ViewMyMetricsClaimValidator : ClaimValidatorBase
    {
        protected ViewMyMetricsClaimValidator(ISecurityAssertionProvider securityAssertionProvider, IClaimValidator next) :
            base(securityAssertionProvider, next)
        {
            ClaimType = EdFiClaimTypes.ViewMyMetrics;
        }
    }

    public class ViewMyMetricsClaimValidatorLocalEducationAgency : ViewMyMetricsClaimValidator
    {
        public ViewMyMetricsClaimValidatorLocalEducationAgency(ISecurityAssertionProvider securityAssertionProvider, IClaimValidator next) :
            base(securityAssertionProvider, next)
        {
            HandledSignatureKey = ClaimValidatorRequest.LocalEducationAgencySignature;
        }

        protected override object HandleRequest(ClaimValidatorRequest request)
        {
            var leaId = request.GetLocalEducationAgencyId();
            //View my metrics is currently siloed.
            SecurityAssertionProvider.CurrentUserMustHaveClaimOnEducationOrganization(leaId, ClaimType);
            return null;
            //throw new UserAccessDeniedException(Implementations.SecurityAssertionProvider.NoEducationOrganizationPermissionErrorMessage);
        }
    }

    public class ViewMyMetricsClaimValidatorSchool : ViewMyMetricsClaimValidator
    {
        public ViewMyMetricsClaimValidatorSchool(ISecurityAssertionProvider securityAssertionProvider, IClaimValidator next) :
            base(securityAssertionProvider, next)
        {
            HandledSignatureKey = ClaimValidatorRequest.SchoolSignature;
        }

        protected override object HandleRequest(ClaimValidatorRequest request)
        {
            var schoolId = request.GetSchoolId();
            //View my metrics is currently siloed.
            SecurityAssertionProvider.CurrentUserMustHaveClaimOnEducationOrganization(schoolId, ClaimType);
            return null;

            //throw new UserAccessDeniedException(Implementations.SecurityAssertionProvider.NoEducationOrganizationPermissionErrorMessage);
        }
    }
}
