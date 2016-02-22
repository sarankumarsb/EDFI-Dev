// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Routing;

namespace EdFi.Dashboards.Resources.Navigation
{
    public class RouteValuesPreparer : IRouteValuesPreparer
    {
        private readonly IRouteValueProvider[] routeValueProviders;

        public RouteValuesPreparer(IRouteValueProvider[] routeValueProviders)
        {
            this.routeValueProviders = routeValueProviders;
        }

        public void PrepareRouteValues(string routeName, RouteValueDictionary routeValues)
        {
            var route = RouteTable.Routes[routeName] as Route;

            if (route == null)
                throw new ArgumentException(string.Format("Route '{0}' was not found.", routeName));

            PrepareRouteValues(route, routeValues);
        }

        private static readonly Regex keyMatcher = new Regex(@"\{(?<Key>[\w_]+)\}", RegexOptions.Compiled);
        private static Dictionary<string, List<string>> keysByRouteUrl = new Dictionary<string, List<string>>();

        public void PrepareRouteValues(Route route, RouteValueDictionary routeValues)
        {
            List<string> routeKeys;

            if (!keysByRouteUrl.TryGetValue(route.Url, out routeKeys))
            {
                var routeKeysMatches = keyMatcher.Matches(route.Url);
                routeKeys =
                    (from Match match in routeKeysMatches
                     select match.Groups["Key"].Value).ToList();

                keysByRouteUrl[route.Url] = routeKeys;
            }

            foreach (string routeKey in routeKeys)
                ProcessRouteValueProvidersForValue(routeValues, routeKey, null);

            CleanUpExtraneousRouteValues(routeValues, routeKeys);
        }

        private void ProcessRouteValueProvidersForValue(RouteValueDictionary routeValues, string routeKey, Stack<string> context)
        {
            if (context == null)
                context = new Stack<string>();

            // Check for cyclic dependency
            if (context.Contains(routeKey))
				throw new InvalidOperationException(
                    string.Format("A cyclical dependency was encountered in the route value providers on route value '{0}'.", routeKey));

            context.Push(routeKey);

            try
            {
                foreach (var valueProvider in routeValueProviders)
                {
                    if (!routeValues.ContainsKey(routeKey) 
                        && valueProvider.CanProvideRouteValue(routeKey, key => GetRouteValue(routeValues, key, context)))
                    {
                        valueProvider.ProvideRouteValue(routeKey,
                            key => GetRouteValue(routeValues, key, context), // Getter
                            (key, value) => routeValues[key] = value        // Setter
                         );
                    }
                }
            }
            finally
            {
                context.Pop();
            }
        }

        private object GetRouteValue(RouteValueDictionary routeValues, string key, Stack<string> context)
        {
            object value;

            if (routeValues.TryGetValue(key, out value))
                return value;

            // Try to get the missing requested key by asking other providers for it.
            ProcessRouteValueProvidersForValue(routeValues, key, context);

            return routeValues[key];
        }

        private static void CleanUpExtraneousRouteValues(RouteValueDictionary routeValues, List<string> routeKeys) 
        {
            // Create a list of required route keys 
            var keysToKeep = routeValues.Keys.Intersect(routeKeys).ToList();

            // Get list of keys in route value dictionary that are not required for route generation
            var keysToRemove = 
                (from key in routeValues.Keys
                 where !keysToKeep.Contains(key)
                 select key)
                    .ToList();

            // Remove extraneous route values to avoid unwanted URL parms from appearing at the end of the URL
            foreach (string key in keysToRemove)
                routeValues.Remove(key);
        }
    }
}
