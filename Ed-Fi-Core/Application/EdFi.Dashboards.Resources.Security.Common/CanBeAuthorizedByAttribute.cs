// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;

namespace EdFi.Dashboards.Resources.Security.Common
{
    [AttributeUsage( AttributeTargets.Method  | AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public sealed class CanBeAuthorizedByAttribute : Attribute
    {
        public static string UnsupportedArgumentMessageFormat = "Unsupported Authorizing Claim: {0}";

        private readonly string[] authorizingClaims = new string[0];
        private readonly AuthorizationDelegate[] authorizingTypes = new AuthorizationDelegate[0];

        public CanBeAuthorizedByAttribute(params string[] authorizingClaims)
        {
            this.authorizingClaims = authorizingClaims;
        }

        public CanBeAuthorizedByAttribute(params AuthorizationDelegate[] authorizingTypes)
        {
            this.authorizingTypes = authorizingTypes;
        }

        public string[] AuthorizingClaims
        {
            get { return authorizingClaims; }
        }

        public AuthorizationDelegate[] AuthorizingTypes
        {
            get { return authorizingTypes; }
        }
    }
}
