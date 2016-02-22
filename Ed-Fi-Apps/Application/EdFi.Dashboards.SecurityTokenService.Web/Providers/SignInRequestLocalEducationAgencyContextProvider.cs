// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System.Web;
using EdFi.Dashboards.Core;
using EdFi.Dashboards.Infrastructure.Implementations;
using EdFi.Dashboards.Infrastructure;

namespace EdFi.Dashboards.SecurityTokenService.Web.Providers
{
    /// <summary>
    /// Provides the context for the local education agency from the STS SignIn request query string (using the "lea" parameter).
    /// </summary>
    public class SignInRequestLocalEducationAgencyContextProvider : ILocalEducationAgencyContextProvider
    {
        protected IHttpRequestProvider httpRequestProvider;
        public static readonly string LeaKey = "lea";
        public SignInRequestLocalEducationAgencyContextProvider(IHttpRequestProvider httpRequestProvider)
        {
            this.httpRequestProvider = httpRequestProvider;
        }

        /// <summary>
        /// Gets the local education agency code from the "lea" query string parameter of an STS SignIn request URL.
        /// </summary>
        /// <returns>The local education agency code value, used to identify the local education agency for routing, authentication, authorization, configuration, multitenancy, etc.</returns>
        public string GetCurrentLocalEducationAgencyCode()
        {
            return httpRequestProvider.GetValue(LeaKey);
        }
    }
}