// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using EdFi.Dashboards.SecurityTokenService.Authentication;
using EdFi.Dashboards.Resources.Security.Implementations;

namespace EdFi.Dashboards.Resources.Security.ClaimValidators
{
    public abstract class ViewOperationalDashboardClaimValidator : ClaimValidatorBase
    {
        protected ViewOperationalDashboardClaimValidator(ISecurityAssertionProvider securityAssertionProvider, IClaimValidator next) :
            base(securityAssertionProvider, next)
        {
            ClaimType = EdFiClaimTypes.ViewOperationalDashboard;
        }
    }

    public class ViewOperationalDashboardClaimValidatorLocalEducationAgency : ViewOperationalDashboardClaimValidator
    {
        public ViewOperationalDashboardClaimValidatorLocalEducationAgency(ISecurityAssertionProvider securityAssertionProvider, IClaimValidator next) :
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

    public class ViewOperationalDashboardClaimValidatorSchool : ViewOperationalDashboardClaimValidator
    {
        public ViewOperationalDashboardClaimValidatorSchool(ISecurityAssertionProvider securityAssertionProvider, IClaimValidator next) :
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
