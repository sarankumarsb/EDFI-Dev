// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System.Collections.Generic;
using System.Linq;
using EdFi.Dashboards.SecurityTokenService.Authentication;

namespace EdFi.Dashboards.Resources.Security.Implementations
{
    public class NullUserClaimSetsProvider<TClaimsSet, TUserSecurityDetails> : IUserClaimSetsProvider<TClaimsSet, TUserSecurityDetails> where TUserSecurityDetails : IErrorLogOutput
    {
        public IEnumerable<TClaimsSet> GetUserClaimSets(TUserSecurityDetails userSecurityDetails)
    	{
            return Enumerable.Empty<TClaimsSet>();
    	}
    }
}
