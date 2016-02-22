// *************************************************************************
// Home page:   http://www.talifun.com/products/talifun-web/
// Github:      https://github.com/taliesins/talifun-web
// License:     Apache 2.0
// *************************************************************************
using System.Configuration;
using Talifun.Web.Configuration;

namespace Talifun.Web.RegexUrlAuthorization.Config
{
    /// <summary>
    /// Represents a configuration element containing a collection of <see cref="UrlMatchElement" /> configuration elements.
    /// </summary>
    [ConfigurationCollection(typeof(UrlMatchElement))]
    public sealed class UrlMatchElementCollection : CurrentConfigurationElementCollection<UrlMatchElement>
    {
        public UrlMatchElementCollection()
        {
            AddElementName = "urlMatch";
        }
    }
}
