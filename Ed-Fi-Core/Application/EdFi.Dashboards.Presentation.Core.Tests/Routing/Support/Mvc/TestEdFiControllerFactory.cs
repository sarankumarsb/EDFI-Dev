using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using EdFi.Dashboards.Presentation.Architecture.Mvc.Core;

namespace EdFi.Dashboards.Presentation.Core.Tests.Routing.Support.Mvc
{
    /// <summary>
    /// This controller factory extends the <see cref="EdFiControllerFactory"/> with selected portions
    /// of MVC's DefaultControllerFactory implementation copied down and modified to remove dependency
    /// on the full ASP.NET hosted infrastructure "requirements".
    /// </summary>
    /// <remarks>This has the potential to cause functional discrepancies during unit testing with future 
    /// upgrades to the MVC framework.</remarks>
    public class TestEdFiControllerFactory : EdFiControllerFactory
    {
        private readonly TestEdFiControllerTypeCache ControllerTypeCache;

        public TestEdFiControllerFactory(TestEdFiControllerTypeCache controllerTypeCache)
        {
            ControllerTypeCache = controllerTypeCache;
        }

        protected override bool TryGetControllerWithDefaultMvcConventions(RequestContext requestContext, string controllerName, out Type controllerType)
        {
            controllerType = BaseMVC_GetControllerType(requestContext, controllerName);

            return (controllerType != null);
        }

        // Copied verbatim from MVC DefaultControllerFactory, to remove downstream dependency on the ASP.NET hosted infrastructure
        private Type BaseMVC_GetControllerType(RequestContext requestContext, string controllerName)
        {
            //if (String.IsNullOrEmpty(controllerName))
            //{
            //    throw new ArgumentException(MvcResources.Common_NullOrEmpty, "controllerName");
            //}

            // first search in the current route's namespace collection 
            object routeNamespacesObj;
            Type match;
            if (requestContext != null && requestContext.RouteData.DataTokens.TryGetValue("Namespaces", out routeNamespacesObj))
            {
                IEnumerable<string> routeNamespaces = routeNamespacesObj as IEnumerable<string>;
                if (routeNamespaces != null && routeNamespaces.Any())
                {
                    HashSet<string> nsHash = new HashSet<string>(routeNamespaces, StringComparer.OrdinalIgnoreCase);
                    match = GetControllerTypeWithinNamespaces(requestContext.RouteData.Route, controllerName, nsHash);

                    // the UseNamespaceFallback key might not exist, in which case its value is implicitly "true" 
                    if (match != null || false.Equals(requestContext.RouteData.DataTokens["UseNamespaceFallback"]))
                    {
                        // got a match or the route requested we stop looking
                        return match;
                    }
                }
            }

            // then search in the application's default namespace collection
            if (ControllerBuilder.Current.DefaultNamespaces.Count > 0)
            {
                HashSet<string> nsDefaults = new HashSet<string>(ControllerBuilder.Current.DefaultNamespaces, StringComparer.OrdinalIgnoreCase);
                match = GetControllerTypeWithinNamespaces(requestContext.RouteData.Route, controllerName, nsDefaults);
                if (match != null)
                {
                    return match;
                }
            }

            // if all else fails, search every namespace
            return GetControllerTypeWithinNamespaces(requestContext.RouteData.Route, controllerName, null /* namespaces */);
        }

        // Copied ALMOST verbatim from DefaultControllerFactory, modified as necessary to avoid dependency on the ASP.NET hosted infrastructure
        private Type GetControllerTypeWithinNamespaces(RouteBase route, string controllerName, HashSet<string> namespaces)
        {
            // Once the master list of controllers has been created we can quickly index into it
            // GKM - Not available... ControllerTypeCache.EnsureInitialized(BuildManager);

            ICollection<Type> matchingTypes = ControllerTypeCache.GetControllerTypes(controllerName, namespaces);
            switch (matchingTypes.Count)
            {
                case 0:
                    // no matching types 
                    return null;

                case 1:
                    // single matching type
                    return matchingTypes.First();

                default:
                    // multiple matching types 
                    throw new Exception(
                        string.Format("Ambiguous controller match for for controller '{0}' in namespaces '{1}'.", 
                                      controllerName, string.Join(", ", namespaces.ToArray())));

                    // GKM: Internal --> throw CreateAmbiguousControllerException(route, controllerName, matchingTypes);
            }
        } 
    }
}