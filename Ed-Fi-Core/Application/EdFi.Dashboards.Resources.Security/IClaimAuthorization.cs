// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using EdFi.Dashboards.Resources.Security.ClaimValidators;

namespace EdFi.Dashboards.Resources.Security
{
    public interface IClaimAuthorization
    {
        void AuthorizeRequest(ClaimValidatorRequest request);

        IClaimValidator Validator();
    }
}
