// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;
using System.Collections.Generic;
using System.Configuration;

namespace EdFi.Dashboards.Common.Configuration
{
    public class InversionOfControlConfiguration : ConfigurationSection
    {
        public const string SectionName = "inversionOfControl";

        [ConfigurationProperty("installers", IsRequired = true)]
        public WindsorInstallers Installers
        {
            get
            {
                return this["installers"] as WindsorInstallers;
            }
        }
    }

    public class WindsorInstallers : ConfigurationElementCollection
    {
        public new WindsorInstaller this[string name]
        {
            get { return (WindsorInstaller) BaseGet(name); }
        }

        public WindsorInstaller this[int index]
        {
            get
            {
                return base.BaseGet(index) as WindsorInstaller;
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
            return new WindsorInstaller();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((WindsorInstaller) element).Name;
        }
    }

    public class WindsorInstaller : ConfigurationElement
    {
        [ConfigurationProperty("name", IsRequired = true, IsKey = true)]
        public string Name
        {
            get
            {
                return this["name"] as string;
            }
        }

        [ConfigurationProperty("typeName", IsRequired = true)]
        public string TypeName
        {
            get
            {
                return this["typeName"] as string;
            }
        }
    }
}
