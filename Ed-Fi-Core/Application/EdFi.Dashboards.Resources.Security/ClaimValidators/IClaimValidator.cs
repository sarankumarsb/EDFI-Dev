// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
namespace EdFi.Dashboards.Resources.Security.ClaimValidators
{
    public interface IClaimValidator
    {
        object ValidateRequest(ClaimValidatorRequest request);
    }
}
