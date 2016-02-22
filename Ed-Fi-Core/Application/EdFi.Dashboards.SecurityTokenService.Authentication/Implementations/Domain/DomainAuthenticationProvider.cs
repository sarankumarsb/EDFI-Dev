// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System;
using System.DirectoryServices.AccountManagement;
using EdFi.Dashboards.Infrastructure;
using EdFi.Dashboards.SecurityTokenService.Authentication.Implementations.Domain.Configuration;
using log4net;

namespace EdFi.Dashboards.SecurityTokenService.Authentication.Implementations.Domain
{
    public class DomainAuthenticationProvider : IAuthenticationProvider
    {
        private readonly static ILog Logger = LogManager.GetLogger(typeof(DomainAuthenticationProvider));
        private readonly IConfigSectionProvider _configSectionProvider;
        public string UserName { get; set; } // VIN25112015

        public DomainAuthenticationProvider(
            IConfigSectionProvider configSectionProvider
            )
        {
            _configSectionProvider = configSectionProvider;
        }

        #region Configuration Property

        private DomainConfiguration _domainConfiguration;

        /// <summary>
        /// Gets the Domain configuration section information.
        /// </summary>
        private DomainConfiguration Configuration
        {
            get { return _domainConfiguration ?? (_domainConfiguration = (DomainConfiguration)_configSectionProvider.GetSection(DomainConfiguration.SectionName)); }
        }

        #endregion

        #region IAuthenticationProvider Methods

        /// <summary>
        /// Validates the specified username and password.
        /// </summary>
        /// <param name="username">The name of the user to validate.</param>
        /// <param name="password">The password to validate for the user.</param>
        /// <returns><b>true</b> if validation was successful; otherwise <b>false</b>.</returns>
        public bool ValidateUser(string username, string password)
        {
            if (String.IsNullOrEmpty(username) || String.IsNullOrEmpty(password))
            {
                Logger.WarnFormat("Login failed for user '{0}' because of null/empty username and/or password.", username);
                return false;
            }

            using (var context = new PrincipalContext(Configuration.ContextType))
            {
                return context.ValidateCredentials(username, password);
            }
        }

        /// <summary>
        /// Takes the lookup value provided by the StaffInformation table and returns the username the user can log in with.
        /// </summary>
        /// <param name="lookupValue">The value from the Staff</param>
        /// <returns></returns>
        public string ResolveLookupValueToUsername(string lookupValue)
        {
            var userPrincipalLookupKey = GetUserPrincipalLookupKey();

            using (var context = new PrincipalContext(Configuration.ContextType))
            {
                var up = UserPrincipal.FindByIdentity(context, MapUserPrincipalLookupKeyToIdentityTypeEnum(userPrincipalLookupKey), lookupValue);
                if (up == null)
                    return null;

                return up.SamAccountName;
            }
        }

        /// <summary>
        /// Takes the username and then resolves it to the value
        /// needed to do a lookup to get the StaffUSI
        /// </summary>
        /// <param name="username">The name of the user</param>
        /// <param name="staffInfoLookupKey">Value in the StaffInformation table that is used to lookup data</param>
        /// <returns></returns>
        public string ResolveUsernameToLookupValue(string username, string staffInfoLookupKey)
        {
            var userPrincipalLookupKey = GetUserPrincipalLookupKey();

            using (var context = new PrincipalContext(Configuration.ContextType))
            {
                UserPrincipal up = UserPrincipal.FindByIdentity(context, username);
                return GetLookupValueFromUserPrincipal(up, userPrincipalLookupKey);
            }
        }

        #endregion
        
        #region Private Methods

        private string GetUserPrincipalLookupKey()
        {
            return Configuration.UserPrincipalLookupKey;
        }
        
        private static IdentityType MapUserPrincipalLookupKeyToIdentityTypeEnum(string userPrincipalLookupKey)
        {
            return (IdentityType)Enum.Parse(typeof(IdentityType), userPrincipalLookupKey);
        }

        private static string GetLookupValueFromUserPrincipal(UserPrincipal userPrincipal, string userPrincipalLookupKey)
        {
            switch (userPrincipalLookupKey)
            {
                case "DistinguishedName": return userPrincipal.DistinguishedName;
                case "Guid": return userPrincipal.Guid.ToString();
                case "Name": return userPrincipal.Name;
                case "SamAccountName": return userPrincipal.SamAccountName;
                case "Sid": return userPrincipal.Sid.ToString();
                case "UserPrincipalName": return userPrincipal.UserPrincipalName;
            }

            throw new ArgumentException("IdentityCode " + userPrincipalLookupKey + " is invalid.", "userPrincipalLookupKey");
        }

        #endregion
    }
}
