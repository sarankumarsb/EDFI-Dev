// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;
using System.Configuration;

namespace EdFi.Dashboards.Common.Configuration.Support
{
    public class SystemSupportConfiguration : ConfigurationSection
    {
        public const string SectionName = "systemSupport";

        private static SystemSupportConfiguration _current;

        /// <summary>
        /// Gets the current 'systemSupport' configuration section from the application configuration file, using the <see cref="ConfigurationManager"/> class.
        /// </summary>
        public static SystemSupportConfiguration Current
        {
            get
            {
                if (_current == null)
                {
                    _current = ConfigurationManager.GetSection(SectionName) as SystemSupportConfiguration;
                }

                return _current;
            }
        }

        [ConfigurationProperty("organizations", IsRequired = true)]
        public Organizations Organizations
        {
            get { return this["organizations"] as Organizations; }
        }
    }

    public class Organizations : ConfigurationElementCollection
    {
        public new Organization this[string code]
        {
            get
            {
                return (Organization) BaseGet(code);
            }
        }

        public Organization this[int index]
        {
            get { return base.BaseGet(index) as Organization; }
            set 
            {
                if (base.Count > index)
                    base.BaseRemoveAt(index);
                
                this.BaseAdd(index, value);
            }
        }

        public Organization Get(string code)
        {   
            var org = (Organization) BaseGet(code);

            if (org != null)
                return org;

            foreach (string key in BaseGetAllKeys())
            {
                if (key.Equals(code, StringComparison.InvariantCultureIgnoreCase))
                    return (Organization) BaseGet(key);
            }

            return null;
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new Organization();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((Organization) element).Code;
        }
    }

    public class Organization : ConfigurationElement
    {
        /// <summary>
        /// Gets or sets the code of the organization (i.e. "LubbockISD", "PSJAISD").
        /// </summary>
        [ConfigurationProperty("code", IsRequired = true, IsKey = true)]
        public string Code
        {
            get { return (string) this["code"]; }
        }

        /// <summary>
        /// Gets or sets the local education agency identifier.
        /// </summary>
        [ConfigurationProperty("localEducationAgencyId", IsRequired = true)]
        public int LocalEducationAgencyId
        {
            get { return Convert.ToInt32(this["localEducationAgencyId"]); }
        }
    }
}
