using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace EdFi.Dashboards.Common.Configuration
{
    public class TimeoutWarningConfiguration : ConfigurationSection
    {
        [ConfigurationProperty("KeepAliveUrl", DefaultValue = "~/KeepAlive", IsRequired = false)]
        public string KeepAliveUrl
        {
            get { return (string) this["KeepAliveUrl"]; }
            set { this["KeepAliveUrl"] = value; }
        }

        [ConfigurationProperty("LogoutUrl", DefaultValue = "~/Logout", IsRequired = false)]
        public string LogoutUrl
        {
            get { return (string)this["LogoutUrl"]; }
            set { this["LogoutUrl"] = value; }
        }

        [ConfigurationProperty("TimeUntilWarning", DefaultValue = 19, IsRequired = false)]
        public int TimeUntilWarning
        {
            get { return (int)this["TimeUntilWarning"]; }
            set { this["TimeUntilWarning"] = value; }
        }

        [ConfigurationProperty("TimeUntilForcedLogout", DefaultValue = 20, IsRequired = false)]
        public int TimeUntilForcedLogout
        {
            get { return (int)this["TimeUntilForcedLogout"]; }
            set { this["TimeUntilForcedLogout"] = value; }
        }

        [ConfigurationProperty("WarningEnabled", DefaultValue = true, IsRequired = false)]
        public bool WarningEnabled
        {
            get { return (bool)this["WarningEnabled"]; }
            set { this["WarningEnabled"] = value; }
        }

        [ConfigurationProperty("ForceEnabled", DefaultValue = true, IsRequired = false)]
        public bool ForceEnabled
        {
            get { return (bool)this["ForceEnabled"]; }
            set { this["ForceEnabled"] = value; }
        }

        [ConfigurationProperty("ForceAction", DefaultValue = TimeoutForceAction.Redirect, IsRequired = false)]
        public TimeoutForceAction ForceAction
        {
            get { return (TimeoutForceAction)this["ForceAction"]; }
            set { this["ForceAction"] = value; }
        }

        private static TimeoutWarningConfiguration _current;
        public static TimeoutWarningConfiguration Current
        {
            get
            {
                if (_current == null)
                {
                    var section = (TimeoutWarningConfiguration)ConfigurationManager.GetSection("timeoutWarning");
                    _current = section ?? new TimeoutWarningConfiguration();
                }
                return _current;
            }
        }

        public enum TimeoutForceAction
        {
            Redirect,
            CloseWindow
        }
    }
}
