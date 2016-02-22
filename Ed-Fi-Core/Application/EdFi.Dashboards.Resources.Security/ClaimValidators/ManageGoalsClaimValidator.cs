// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using EdFi.Dashboards.SecurityTokenService.Authentication;

namespace EdFi.Dashboards.Resources.Security.ClaimValidators
{
    public abstract class ManageGoalsClaimValidator : ClaimValidatorBase
    {
        protected ManageGoalsClaimValidator(ISecurityAssertionProvider securityAssertionProvider, IClaimValidator next) :
            base(securityAssertionProvider, next)
        {
            ClaimType = EdFiClaimTypes.ManageGoals;
        }
    }

    public class ManageGoalsClaimValidatorLocalEducationAgency : ManageGoalsClaimValidator
    {
        public ManageGoalsClaimValidatorLocalEducationAgency(ISecurityAssertionProvider securityAssertionProvider, IClaimValidator next) :
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

    public class ManageGoalsClaimValidatorLocalEducationAgencyMetric : ManageGoalsClaimValidator
    {
        public ManageGoalsClaimValidatorLocalEducationAgencyMetric(ISecurityAssertionProvider securityAssertionProvider, IClaimValidator next) :
            base(securityAssertionProvider, next)
        {
            HandledSignatureKey = ClaimValidatorRequest.LocalEducationAgencyMetricSignature;
        }

        protected override object HandleRequest(ClaimValidatorRequest request)
        {
            ValidateClaimLocalEducationAgencyMetric(request, ClaimType);

            // TODO: GKM - Do we need to secure specific metrics (such as Operational Dashboards), or does Manage Goals allow them to read those metrics too?

            return null;
        }
    }

    public class ManageGoalsClaimValidatorSchool : ManageGoalsClaimValidator
    {
        public ManageGoalsClaimValidatorSchool(ISecurityAssertionProvider securityAssertionProvider, IClaimValidator next) :
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
