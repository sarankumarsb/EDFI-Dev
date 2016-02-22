using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EdFi.Dashboards.Resources.Security.ClaimValidators
{
    public class ManageWatchListClaimAuthorization : ClaimAuthorizationBase
    {
        public ManageWatchListClaimAuthorization(IClaimValidator claimValidator) : base(claimValidator) { }
    }
}
