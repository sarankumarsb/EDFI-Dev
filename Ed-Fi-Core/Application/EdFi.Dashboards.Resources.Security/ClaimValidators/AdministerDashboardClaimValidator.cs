// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using EdFi.Dashboards.SecurityTokenService.Authentication;
using EdFi.Dashboards.Resources.Security.Implementations;

namespace EdFi.Dashboards.Resources.Security.ClaimValidators
{
    public abstract class AdministerDashboardClaimValidator : ClaimValidatorBase
    {
        protected AdministerDashboardClaimValidator(ISecurityAssertionProvider securityAssertionProvider, IClaimValidator next) :
            base(securityAssertionProvider, next)
        {
            ClaimType = EdFiClaimTypes.AdministerDashboard;
        }
    }

    public class AdministerDashboardClaimValidatorLocalEducationAgency : AdministerDashboardClaimValidator
    {
        public AdministerDashboardClaimValidatorLocalEducationAgency(ISecurityAssertionProvider securityAssertionProvider, IClaimValidator next) :
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

    public class AdministerDashboardClaimValidatorConfiguration : AdministerDashboardClaimValidator
    {
        public AdministerDashboardClaimValidatorConfiguration(ISecurityAssertionProvider securityAssertionProvider, IClaimValidator next) :
            base(securityAssertionProvider, next)
        {
            HandledSignatureKey = "iskillswitchactivated|links|localeducationagencyid|localeducationagencyname|resourceurl|systemmessage|url";
        }

        protected override object HandleRequest(ClaimValidatorRequest request)
        {
            ValidateClaimLocalEducationAgency(request, ClaimType);

            return null;
        }
    }

    public class AdministerDashboardClaimValidatorTitleClaimSet : AdministerDashboardClaimValidator
    {
        public AdministerDashboardClaimValidatorTitleClaimSet(ISecurityAssertionProvider securityAssertionProvider, IClaimValidator next) :
            base(securityAssertionProvider, next)
        {
            HandledSignatureKey = "claimset|claimsetmaps|currentoperation|edorgpositiontitles|fileinputstream|filename|ispost|issuccess|links|localeducationagencyid|messages|positiontitle|possibleclaimsets|resourceurl|url";
        }

        protected override object HandleRequest(ClaimValidatorRequest request)
        {
            ValidateClaimLocalEducationAgency(request, ClaimType);

            return null;
        }
    }

    public class AdministerDashboardClaimValidatorSchool : AdministerDashboardClaimValidator
    {
        public AdministerDashboardClaimValidatorSchool(ISecurityAssertionProvider securityAssertionProvider, IClaimValidator next) :
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

    public class AdministerDashboardClaimValidatorStaff : AdministerDashboardClaimValidator
    {
        public AdministerDashboardClaimValidatorStaff(ISecurityAssertionProvider securityAssertionProvider, IClaimValidator next) :
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
}
