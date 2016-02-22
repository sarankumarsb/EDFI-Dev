// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using EdFi.Dashboards.Common;
using EdFi.Dashboards.Resources.Security.Implementations;

namespace EdFi.Dashboards.Resources.Security.ClaimValidators
{
    public class NullClaimValidator : IClaimValidator
    {
        public object ValidateRequest(ClaimValidatorRequest request)
        {
            throw new UnhandledSignatureException(false, // Not explicitly unhandled... it fell through to the end of the chain of responsibility
                string.Format(
                    ClaimValidatorRequest.UnhandledParameterErrorMessageFormat,
                    request.BuildSignatureKey()));
        }
    }
}
