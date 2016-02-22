using EdFi.Dashboards.Common.Utilities;
using Microsoft.IdentityModel.Claims;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EdFi.Dashboards.Resources.Security
{
    /// <summary>
    /// By inheriting from this class, and configuring it's usage in web.config, one can manipulate claims
    /// that are received from the STS before they are stored in the cookie.  
    /// </summary>
    /// <remarks>
    /// Hosted STS like Azure Access Control or Azure AD do not know about our application specific claims.  This 
    /// extension hook was added to allow us to lookup those claims once the token is received and validated.  
    /// This class must be turned on in the web.config by specifying <claimsAuthenticationManager type="" />
    /// </remarks>
    public class CustomClaimsAuthenticationManager : ClaimsAuthenticationManager
    {
        public override IClaimsPrincipal Authenticate(string resourceName, IClaimsPrincipal incomingPrincipal)
        {
            var identity = incomingPrincipal.Identity as IClaimsIdentity;

            if (identity == null)
                throw new ArgumentException("Principal must have an identity of type IClaimsIdentity");

            //Only process if the identity is authenticated, otherwise, we want the pipeline to redirect to the STS
            if (identity.IsAuthenticated)
                return base.Authenticate(resourceName, ClaimsAuthenticationManagerProvider.Get(resourceName, incomingPrincipal));
            return base.Authenticate(resourceName, incomingPrincipal);
        }

        private IClaimsAuthenticationManagerProvider claimsAuthenticationManagerProvider;
        public IClaimsAuthenticationManagerProvider ClaimsAuthenticationManagerProvider
        {
            get { return claimsAuthenticationManagerProvider ?? (claimsAuthenticationManagerProvider = IoC.Resolve<IClaimsAuthenticationManagerProvider>()); }
        }
    }
}
