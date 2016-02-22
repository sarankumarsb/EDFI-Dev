// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Linq;
using EdFi.Dashboards.Application.Data.Entities;
using EdFi.Dashboards.Core;
using EdFi.Dashboards.Data.Repository;
using EdFi.Dashboards.Infrastructure;
using EdFi.Dashboards.SecurityTokenService.Authentication.Implementations.Ldap.Configuration;
using log4net;

namespace EdFi.Dashboards.SecurityTokenService.Authentication.Implementations.Ldap
{
    public class SecureLdapAuthenticationProvider : IAuthenticationProvider, IDisposable
    {
        private readonly static ILog logger = LogManager.GetLogger(typeof (SecureLdapAuthenticationProvider));
        private const string NoLookupValueErrorMessageFormat = "We were able to verify your password, but the {0}.  If you feel this is incorrect, please contact an administrator.";
        private const string NoPropertyReturnedFormat = "Ldap directory search did not return property '{0}' in results collection for user name '{1}'.";
        private const string EmptyPropertyCollectionFormat = "Ldap directory search returned empty collection for property '{0}' in results collection for user name '{1}'.";
        private const string BlankValueReturnedFormat = "Ldap directory search returned an empty value for property '{0}' in results collection for user name '{1}'.";
        private const string LdapUsernameProperty = "SAMAccountName";
        private const string LdapEmailProperty = "mail";
        private readonly IConfigSectionProvider configSectionProvider;
        private readonly ILocalEducationAgencyContextProvider localEducationAgencyContextProvider;
        private readonly IRepository<LocalEducationAgency> _localEducationAgencyRepository;
        private readonly IRepository<LocalEducationAgencyAuthentication> _localEducationAgencyAuthenticationRepository;

        private readonly Dictionary<string, string> lookupValuesByUsername = new Dictionary<string, string>();
        //private readonly Dictionary<string, string> usernamesByLookupValue = new Dictionary<string, string>();
        private Dictionary<string, DirectoryEntry> searcherEntries = new Dictionary<string, DirectoryEntry>();

        public string UserName { get; set; } // VIN25112015

        public SecureLdapAuthenticationProvider(IConfigSectionProvider configSectionProvider, ILocalEducationAgencyContextProvider localEducationAgencyContextProvider, IRepository<LocalEducationAgency> localEducationAgencyRepository, IRepository<LocalEducationAgencyAuthentication> localEducationAgencyAuthenticationRepository)
        {
            this.configSectionProvider = configSectionProvider;
            this.localEducationAgencyContextProvider = localEducationAgencyContextProvider;
            _localEducationAgencyRepository = localEducationAgencyRepository;
            _localEducationAgencyAuthenticationRepository = localEducationAgencyAuthenticationRepository;
        }

        #region Configuration Property

        /// <summary>
        /// Holds the value for the <see cref="Configuration"/> property.
        /// </summary>
        private LdapConfiguration ldapConfiguration;

        /// <summary>
        /// Gets the LDAP configuration section information.
        /// </summary>
        private LdapConfiguration Configuration
        {
            get { return ldapConfiguration ?? (ldapConfiguration = (LdapConfiguration) configSectionProvider.GetSection(LdapConfiguration.SectionName)); }
        }

        #endregion

        #region IAuthenticationProvider Methods
        public bool ValidateUser(string username, string password)
        {
            if (String.IsNullOrEmpty(username) || String.IsNullOrEmpty(password))
            {
                logger.WarnFormat("Login failed for user '{0}' because of null/empty username and/or password.", username);
                return false;
            }

            var localEducationAgencyCode = localEducationAgencyContextProvider.GetCurrentLocalEducationAgencyCode();
            var ldapDirectory = GetLdapDirectoryFromContext(localEducationAgencyCode);

            if (ldapDirectory == null)
            {

				throw new InvalidOperationException(
                    string.Format("LDAP configuration information for local education agency '{0}' not found.", localEducationAgencyCode));
            }

            string fullyQualifiedUsername = ldapDirectory.Domain + @"\" + username;

            using (var entry = new DirectoryEntry(ldapDirectory.ServerAddress, fullyQualifiedUsername, password, AuthenticationTypes.SecureSocketsLayer))
            {
                try
                {
                    // Bind to the native AdsObject to force authentication.
                    object obj = entry.NativeObject;
                }
                catch (DirectoryServicesCOMException dsEx)
                {
                    if (dsEx.ExtendedError == unchecked((int)0x8007052e) || dsEx.Message.ToLower().Contains("unknown user name or bad password"))
                    {
                        logger.WarnFormat("Login failed for user '{0}'. {1}", username, dsEx.Message);
                        return false;
                    }

                    throw new InvalidOperationException(string.Format("Error authenticating user '{0}'. Extended error: '{1}' {2}", username, dsEx.ExtendedError, dsEx.ExtendedErrorMessage), dsEx);
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException(string.Format("Error authenticating user '{0}'", username), ex);
                }
            }

            return true;
        }

        public string ResolveLookupValueToUsername(string lookupValue)
        {
            DirectorySearcher search = null;

            try
            {
                var ldapLookupKey = GetLdapLookupKey();

                search = new DirectorySearcher(SearcherEntry)
                {
                    Filter = "(" + ldapLookupKey + "=" + lookupValue + ")"
                };
                search.PropertiesToLoad.Add(LdapUsernameProperty);

                SearchResult result = search.FindOne();

                if (result == null)
					throw new InvalidOperationException(String.Format("Ldap directory returned no results for query: '({0}={1})'", ldapLookupKey, lookupValue));

                // Property names are keyed in lower case by System.DirectoryServices
                string username = (string)result.Properties[LdapUsernameProperty.ToLower()][0];

                if (String.IsNullOrWhiteSpace(username))
					throw new InvalidOperationException(String.Format("Ldap directory returned results for query: '({0}={1})' but the first result value was null or an empty string.", ldapLookupKey, lookupValue));
                
                if (lookupValuesByUsername.ContainsKey(username))
                    lookupValuesByUsername[username] = lookupValue;

                return username;
            }
            finally
            {
                if (search != null)
                    search.Dispose();
            }
        }

        public string ResolveUsernameToLookupValue(string username, string staffInfoLookupKey)
        {
            if (lookupValuesByUsername.ContainsKey(username))
                return lookupValuesByUsername[username];

            DirectorySearcher search = null;
            string ldapLookupValue = null;
            string ldapLookupKey = null;
            try
            {
                ldapLookupKey = GetLdapLookupKey();

                search = new DirectorySearcher(SearcherEntry)
                            {
                                Filter = "(" + LdapUsernameProperty + "=" + username + ")"
                            };
                search.PropertiesToLoad.Add(ldapLookupKey);

                var result = search.FindOne();

                if (result == null)
                    return null;

                if (!result.Properties.Contains(ldapLookupKey))
                    throw new DistrictProtocolAuthenticationException(string.Format(NoLookupValueErrorMessageFormat, string.Format(NoPropertyReturnedFormat, ldapLookupKey, username))) { Name = username };

                if (result.Properties[ldapLookupKey].Count == 0)
                    throw new DistrictProtocolAuthenticationException(string.Format(NoLookupValueErrorMessageFormat, string.Format(EmptyPropertyCollectionFormat, ldapLookupKey, username))) { Name = username };

                var resultValue = result.Properties[ldapLookupKey][0];
                if (resultValue is string)
                {
                    ldapLookupValue = (string)resultValue;
                }
                else if (resultValue is byte[])
                {
                    //LDAP can return a few different types. If it's a byte[], it's probably because ldapLookupKey is objectsid.  Convert the byte[] to a SID.
                    var sid = new System.Security.Principal.SecurityIdentifier((byte[]) (resultValue), 0);
                    ldapLookupValue = sid.ToString();
                }
                else
                {
                    throw new InvalidCastException("The type of the Property is " + resultValue.GetType() + " and it needs to be a string or a SID.");
                }

                ldapLookupValue = ldapLookupValue.ToLower();

                lookupValuesByUsername[username] = ldapLookupValue;
            }
            finally
            {
                if (search != null)
                    search.Dispose();
            }

            if (string.IsNullOrEmpty(ldapLookupValue))
                throw new DistrictProtocolAuthenticationException(string.Format(NoLookupValueErrorMessageFormat, string.Format(BlankValueReturnedFormat, ldapLookupKey, username))) { Name = username };

            return ldapLookupValue;
        }
        #endregion

        #region IDisposable Methods
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {

                foreach (var searcherEntry in searcherEntries.Values)
                {
                    if (searcherEntry != null)
                    {
                        searcherEntry.Dispose();
                    }
                }
                searcherEntries.Clear();
            }
        }
        #endregion

        #region Private Methods

        private DirectoryEntry SearcherEntry
        {
            get
            {
                var localEducationAgencyCode = localEducationAgencyContextProvider.GetCurrentLocalEducationAgencyCode();
                DirectoryEntry searcherEntry;
                searcherEntries.TryGetValue(localEducationAgencyCode, out searcherEntry);
                if (searcherEntry == null)
                {
                    lock (searcherEntries)
                    {
                        // try one more time
                        searcherEntries.TryGetValue(localEducationAgencyCode, out searcherEntry);
                        if (searcherEntry == null)
                        {
                            var ldapDirectory = GetLdapDirectoryFromContext(localEducationAgencyCode);

                            string fullyQualifiedUsername = ldapDirectory.Domain + @"\" + ldapDirectory.AppUsername;

                            searcherEntry = new DirectoryEntry(ldapDirectory.ServerAddress, fullyQualifiedUsername, ldapDirectory.AppPassword, AuthenticationTypes.SecureSocketsLayer);
                            searcherEntries.Add(localEducationAgencyCode, searcherEntry);

                            // Bind to the native AdsObject to force authentication.
                            object obj = searcherEntry.NativeObject;
                        }
                    }
                }

                return searcherEntry;
            }
        }

        private LdapDirectory GetLdapDirectoryFromContext(string localEducationAgencyCode)
        {
            return Configuration.Directories.Get(localEducationAgencyCode);
        }

        private string GetLdapLookupKey()
        {
            var leaCode = localEducationAgencyContextProvider.GetCurrentLocalEducationAgencyCode();

            var ldapLookupKeys =
                (from lea in _localEducationAgencyRepository.GetAll()
                     .Where(x => x.Code == leaCode)
                 from leaAuth in _localEducationAgencyAuthenticationRepository.GetAll()
                     .Where(x => x.LocalEducationAgencyId == lea.LocalEducationAgencyId)
                     .DefaultIfEmpty()
                 select new
                 {
                     leaAuth.LdapLookupKey
                 }).ToList();

            return string.IsNullOrEmpty(ldapLookupKeys.First().LdapLookupKey) ? LdapEmailProperty : ldapLookupKeys.First().LdapLookupKey;
        }
        #endregion
    }
}