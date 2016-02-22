using System.Collections.Generic;
using EdFi.Dashboards.Common.Utilities;

namespace EdFi.Dashboards.SecurityTokenService.Authentication.Analytics
{
    public class ClaimsIssuedTrackingEventProvider : IClaimsIssuedTrackingEventProvider
    {
        private IClaimsIssuedTrackingEventHandler[] claimsIssuedTrackingEventHandler;

        public void Track(string username, long staffUSI, bool isImpersonating, IEnumerable<Microsoft.IdentityModel.Claims.Claim> claims)
        {
            if (claimsIssuedTrackingEventHandler == null)
                claimsIssuedTrackingEventHandler = IoC.ResolveAll<IClaimsIssuedTrackingEventHandler>();

            if (claimsIssuedTrackingEventHandler != null)
            {
                foreach (var tracking in claimsIssuedTrackingEventHandler)
                {
                    tracking.Track(username, staffUSI, isImpersonating, claims);
                }
            }
        }
    }
}