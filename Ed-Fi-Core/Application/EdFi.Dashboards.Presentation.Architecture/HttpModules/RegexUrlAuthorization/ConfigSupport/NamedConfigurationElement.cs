// *************************************************************************
// Home page:   http://www.talifun.com/products/talifun-web/
// Github:      https://github.com/taliesins/talifun-web
// License:     Apache 2.0
// *************************************************************************
using System.Configuration;

namespace Talifun.Web.Configuration
{
    /// <summary>
    /// Defines an abstract base class for configuration elements that can be contained within sections that derive from
    /// <see cref="ConfigurationElementCollection" />.
    /// </summary>
    public abstract class NamedConfigurationElement : ConfigurationElement
    {
        /// <summary>
        /// Gets or sets a string containing the name of this configuration element.
        /// </summary>
        public abstract string Name
        {
            get;
            set;
        }
    }
}
