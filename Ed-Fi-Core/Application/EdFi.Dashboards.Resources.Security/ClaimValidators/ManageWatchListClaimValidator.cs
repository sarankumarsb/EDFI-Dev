using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EdFi.Dashboards.Common;
using EdFi.Dashboards.Resources.Common;
using EdFi.Dashboards.SecurityTokenService.Authentication;

namespace EdFi.Dashboards.Resources.Security.ClaimValidators
{
    public abstract class ManageWatchListClaimValidator : ClaimValidatorBase
    {
        protected ManageWatchListClaimValidator(ISecurityAssertionProvider securityAssertionProvider,
            IClaimValidator next) : base(securityAssertionProvider, next)
        {
            ClaimType = EdFiClaimTypes.ManageWatchList;
        }

        protected void CheckEducationOrganization(ClaimValidatorRequest request)
        {
            var schoolId = request.GetNullableIdByName(ClaimValidatorRequest.SchoolParameterName);
            var localEducationAgencyId =
                request.GetNullableIdByName(ClaimValidatorRequest.LocalEducationAgencyParameterName);
            var userInfo = UserInformation.Current;

            if (schoolId.HasUsableValue())
            {
                var claimValidatorSchoolEdOrgs =
                    SecurityAssertionProvider.GetEducationOrganizationHierarchy(schoolId.Value);

                if (
                    userInfo.AssociatedOrganizations.Any(
                        org =>
                            org.ClaimTypes.Contains(ClaimType) &&
                            (claimValidatorSchoolEdOrgs.Contains(org.EducationOrganizationId) ||
                            SecurityAssertionProvider.GetEducationOrganizationHierarchy(org.EducationOrganizationId)
                                .Contains(schoolId.Value))))
                {
                    return;
                }
            }
            else if (localEducationAgencyId.HasUsableValue())
            {
                var claimValidatorLocalEducationAgencyEdOrgs =
                    SecurityAssertionProvider.GetEducationOrganizationHierarchy(localEducationAgencyId.Value);

                if (
                    userInfo.AssociatedOrganizations.Any(
                        org =>
                            org.ClaimTypes.Contains(ClaimType) &&
                            (claimValidatorLocalEducationAgencyEdOrgs.Contains(org.EducationOrganizationId) ||
                            SecurityAssertionProvider.GetEducationOrganizationHierarchy(org.EducationOrganizationId)
                                .Contains(localEducationAgencyId.Value))))
                {
                    return;
                }
            }
            
            throw new UserAccessDeniedException(Implementations.SecurityAssertionProvider.NoEducationOrganizationPermissionErrorMessage);
        }
    }

    public class ManageWatchListClaimValidatorLocalEducationAgencySchoolStaff : ManageWatchListClaimValidator
    {
        public ManageWatchListClaimValidatorLocalEducationAgencySchoolStaff(
            ISecurityAssertionProvider securityAssertionProvider, IClaimValidator next)
            : base(securityAssertionProvider, next)
        {
            HandledSignatureKey = ClaimValidatorRequest.LocalEducationAgencySchoolStaffSignature;
        }

        protected override object HandleRequest(ClaimValidatorRequest request)
        {
            //TODO: Start using this again once the staffUSI issue is fixed
            //ValidateCurrentUserIsStaff(request);
            CheckEducationOrganization(request);

            return null;
        }
    }
}
