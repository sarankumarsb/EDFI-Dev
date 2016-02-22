// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System;
using System.Configuration;
using System.DirectoryServices.AccountManagement;

namespace EdFi.Dashboards.SecurityTokenService.Authentication.Implementations.Domain.Configuration
{
	public class DomainConfiguration : ConfigurationSection
	{
		public const string SectionName = "domainConfiguration";

        [ConfigurationProperty("userPrincipalLookupKey", IsRequired = true)]
        public string UserPrincipalLookupKey
        {
            get
            {
                return this["userPrincipalLookupKey"] as string;
            }
        }

        [ConfigurationProperty("contextType", IsRequired = false, DefaultValue = ContextType.Domain)]
        public ContextType ContextType
        {
            get
            {
                // This contextType setting is not required in the config, however, we have
                // set a default value in the attribute so it should be safe to just issue
                // an explicit cast.
                return (ContextType)this["contextType"];
            }
        }
    }
}
