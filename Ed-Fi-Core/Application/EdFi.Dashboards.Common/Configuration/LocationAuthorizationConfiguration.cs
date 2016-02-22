// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System.Configuration;

namespace EdFi.Dashboards.Common.Configuration
{
    public class LocationAuthorizationConfiguration : ConfigurationSection
    {
        public const string SectionName = "locationAuthorizationSection";

        private static LocationAuthorizationConfiguration _current;

        /// <summary>
        /// Gets the current 'systemSupport' configuration section from the application configuration file, using the <see cref="ConfigurationManager"/> class.
        /// </summary>
        public static LocationAuthorizationConfiguration Current
        {
            get
            {
                if (_current == null)
                {
                    _current = ConfigurationManager.GetSection(SectionName) as LocationAuthorizationConfiguration;
                }

                return _current;
            }
        }

        [ConfigurationProperty("locationAuthorizations", IsRequired = true)]
        public LocationAuthorizations LocationAuthorizations
        {
            get { return this["locationAuthorizations"] as LocationAuthorizations; }
        }
    }

    public class LocationAuthorizations : ConfigurationElementCollection
    {
        public new LocationAuthorization this[string path]
        {
            get
            {
                var org = (LocationAuthorization)BaseGet(path);
                return org;
            }
        }

        public LocationAuthorization this[int index]
        {
            get { return BaseGet(index) as LocationAuthorization; }
            set 
            {
                if (BaseGet(index) != null)
                    BaseRemoveAt(index);
                
                BaseAdd(index, value);
            }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new LocationAuthorization();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((LocationAuthorization)element).Name;
        }
    }

    public class LocationAuthorization : ConfigurationElement
    {
        /// <summary>
        /// are to be authorized.
        /// </summary>
        [ConfigurationProperty("name", IsRequired = true, IsKey = true)]
        public string Name
        {
            get { return (string)this["name"]; }
        }

        /// <summary>
        /// are to be authorized.
        /// </summary>
        [ConfigurationProperty("area", IsRequired = true, IsKey = true)]
        public string Area
        {
            get { return (string) this["area"]; }
        }

        /// <summary>
        /// controller to be authorized.
        /// </summary>
        [ConfigurationProperty("controller", IsRequired = true, IsKey = true)]
        public string Controller
        {
            get { return (string)this["controller"]; }
        }

        /// <summary>
        /// Gets he comma separated string of claims that can authorized access to the path.
        /// </summary>
        [ConfigurationProperty("authorizedBy", IsRequired = true)]
        public string AuthorizedBy
        {
            get { return (string)this["authorizedBy"]; }
        }
    }
}
