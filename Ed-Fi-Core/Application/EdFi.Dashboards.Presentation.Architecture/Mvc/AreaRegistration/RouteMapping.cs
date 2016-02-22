// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

namespace EdFi.Dashboards.Presentation.Architecture.Mvc.AreaRegistration
{
    /// <summary>
    /// Represents the arguments to pass to the <see cref="System.Web.Mvc.AreaRegistrationContext.MapRoute"/> method of an <see cref="AreaRegistration"/> implementation.
    /// </summary>
    public class RouteMapping
    {
        /// <summary>
        /// Gets or sets the name of the route.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the URL pattern for the route.
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Gets or sets an object that contains default route values.
        /// </summary>
        public object Defaults { get; set; }

        /// <summary>
        /// Gets or sets a set of expressions that specify valid values for the URL parameters.
        /// </summary>
        public object Constraints { get; set; }

        /// <summary>
        /// Gets or sets an enumerable set of namespaces for the application.
        /// </summary>
        public string[] Namespaces { get; set; }

        public RouteMapping(string name, string url)
            : this(name, url, null, null, null)
        {
        }

        public RouteMapping(string name, string url, object defaults)
            : this(name, url, defaults, null, null)
        {
        }

        public RouteMapping(string name, string url, object defaults, object constraints)
            : this(name, url, defaults, constraints, null)
        {
        }

        public RouteMapping(string name, string url, object defaults, object constraints, string[] namespaces)
        {
            Name = name;
            Url = url;
            Defaults = defaults;
            Constraints = constraints;
            Namespaces = namespaces;
        }

        public RouteMapping(string name, string url, object defaults, string[] namespaces)
            : this(name, url, defaults, null, namespaces)
        {
        }

        public RouteMapping(string name, string url, string[] namespaces)
            : this(name, url, null, null, namespaces)
        {
        }
    }
}
