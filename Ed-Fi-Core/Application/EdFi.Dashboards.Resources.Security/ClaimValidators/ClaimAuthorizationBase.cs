// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;

namespace EdFi.Dashboards.Resources.Security.ClaimValidators
{
	public abstract class ClaimAuthorizationBase : IClaimAuthorization
	{
		private readonly IClaimValidator claimValidator;

        protected ClaimAuthorizationBase(IClaimValidator claimValidator)
		{
			if (claimValidator == null)
                throw new InvalidOperationException("claimValidator is required.");

			this.claimValidator = claimValidator;
		}

	    public void AuthorizeRequest(ClaimValidatorRequest request)
        {
            claimValidator.ValidateRequest(request);
	    }

	    public IClaimValidator Validator()
	    {
	        return claimValidator;
	    }
	}
}