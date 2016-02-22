// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using EdFi.Dashboards.SecurityTokenService.Authentication;
using EdFi.Dashboards.Resources.Security.Implementations;

namespace EdFi.Dashboards.Resources.Security.ClaimValidators
{
    public abstract class ViewAllMetricsClaimValidator : ClaimValidatorBase
    {
        protected ViewAllMetricsClaimValidator(ISecurityAssertionProvider securityAssertionProvider, IClaimValidator next) :
            base(securityAssertionProvider, next)
        {
            ClaimType = EdFiClaimTypes.ViewAllMetrics;
        }
    }

    public class ViewAllMetricsClaimValidatorLocalEducationAgency : ViewAllMetricsClaimValidator
    {
        public ViewAllMetricsClaimValidatorLocalEducationAgency(ISecurityAssertionProvider securityAssertionProvider, IClaimValidator next) :
            base(securityAssertionProvider, next)
        {
            HandledSignatureKey = ClaimValidatorRequest.LocalEducationAgencySignature;
        }

        protected override object HandleRequest(ClaimValidatorRequest request)
        {
            return ValidateClaimLocalEducationAgency(request, ClaimType);
        }
    }

    public class ViewAllMetricsClaimValidatorSchool : ViewAllMetricsClaimValidator
    {
        public ViewAllMetricsClaimValidatorSchool(ISecurityAssertionProvider securityAssertionProvider, IClaimValidator next) :
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
}
