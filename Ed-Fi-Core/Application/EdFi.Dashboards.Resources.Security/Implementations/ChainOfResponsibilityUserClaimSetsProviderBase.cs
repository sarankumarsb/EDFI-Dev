// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System;
using System.Collections.Generic;
using EdFi.Dashboards.Common;
using EdFi.Dashboards.SecurityTokenService.Authentication;

namespace EdFi.Dashboards.Resources.Security.Implementations
{
    public interface IChainOfResponsibilityUserClaimSetsProvider<TClaimsSet, TUserSecurityDetails> : IUserClaimSetsProvider<TClaimsSet, TUserSecurityDetails> where TUserSecurityDetails : IErrorLogOutput { }

    public abstract class ChainOfResponsibilityUserClaimSetsProviderBase<TClaimsSet, TUserSecurityDetails> : IChainOfResponsibilityUserClaimSetsProvider<TClaimsSet, TUserSecurityDetails> where TUserSecurityDetails : IErrorLogOutput
    {
        protected IUserClaimSetsProvider<TClaimsSet, TUserSecurityDetails> Next { get; set; }

        protected abstract bool CanGetUserClaimSets(TUserSecurityDetails userSecurityDetails);
        protected abstract IEnumerable<TClaimsSet> DoGetUserClaimSets(TUserSecurityDetails userSecurityDetails);

        public IEnumerable<TClaimsSet> GetUserClaimSets(TUserSecurityDetails userSecurityDetails)
    	{
            if (CanGetUserClaimSets(userSecurityDetails))
            {
                try
                {
					return DoGetUserClaimSets(userSecurityDetails);
                }
                catch (Exception ex)
                {
                    throw new UserAccessDeniedException("Could not load claim sets.", ex);
                }
            }

            if (Next != null)
				return Next.GetUserClaimSets(userSecurityDetails);

            // This should never happen because there should be a NullUserClaimSetsProvider at the end of each chain due to a problem 
            // with Castle wiring up all the chains together instead of breaking them apart at a "null" reference.
            // Regardless, we throw the same exception here as the NullUserClaimSetsProvider does, just in case.
            throw new UserAccessDeniedException();
        }
    }
}
