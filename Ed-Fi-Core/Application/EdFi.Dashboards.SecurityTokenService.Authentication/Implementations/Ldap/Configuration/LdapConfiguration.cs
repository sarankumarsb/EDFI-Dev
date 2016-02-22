// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;
using System.Collections.Generic;
using System.Configuration;

namespace EdFi.Dashboards.SecurityTokenService.Authentication.Implementations.Ldap.Configuration
{
	public class LdapConfiguration : ConfigurationSection
	{
		public const string SectionName = "ldapConfiguration";

		[ConfigurationProperty("directories", IsRequired = true)]
		public LdapDirectories Directories
		{
			get
			{
				return this["directories"] as LdapDirectories;
			}
		}
	}

	public class LdapDirectories : ConfigurationElementCollection
	{
		public new LdapDirectory this[string name]
		{
			get { return (LdapDirectory) BaseGet(name); }
		}

		public LdapDirectory this[int index]
		{
			get
			{
				return base.BaseGet(index) as LdapDirectory;
			}
			set
			{
				if (base.BaseGet(index) != null)
					base.BaseRemoveAt(index);
				
				this.BaseAdd(index, value);
			}
		}

		protected override ConfigurationElement CreateNewElement()
		{
			return new LdapDirectory();
		}

		protected override object GetElementKey(ConfigurationElement element)
		{
			return ((LdapDirectory) element).Name;
		}

        public LdapDirectory Get(string code)
        {
            var org = (LdapDirectory)BaseGet(code);

            if (org != null)
                return org;

            foreach (string key in BaseGetAllKeys())
            {
                if (key.Equals(code, StringComparison.InvariantCultureIgnoreCase))
                    return (LdapDirectory)BaseGet(key);
            }

            throw new ConfigurationErrorsException(
                string.Format(
                    "No organization with code '{0}' was found in the '{1}' configuration section.", code, LdapConfiguration.SectionName));
        }
    }

	public class LdapDirectory : ConfigurationElement
	{
		[ConfigurationProperty("name", IsRequired = true, IsKey = true)]
		public string Name
		{
			get
			{
				return this["name"] as string;
			}
		}

		[ConfigurationProperty("serverAddress", IsRequired = true)]
		public string ServerAddress
		{
			get
			{
				return this["serverAddress"] as string;
			}
		}

		[ConfigurationProperty("domain", IsRequired = true)]
		public string Domain
		{
			get
			{
				return this["domain"] as string;
			}
		}

		[ConfigurationProperty("appUsername", IsRequired = true)]
		public string AppUsername
		{
			get
			{
				return this["appUsername"] as string;
			}
		}

		[ConfigurationProperty("appPassword", IsRequired = true)]
		public string AppPassword
		{
			get
			{
				return this["appPassword"] as string;
			}
		}
	}
}
