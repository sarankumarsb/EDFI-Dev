using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using EdFi.Dashboards.Presentation.Web;

namespace EdFi.Dashboards.Presentation.Core.Tests.Routing.Support.Mvc
{
    // This class is copied almost verbatim from MVC sources, less the code that ties it to ASP.NET as a dependency.
    public class TestEdFiControllerTypeCache
    {
        private readonly Assembly[] assemblies;

        private const string _typeCacheName = "MVC-ControllerTypeCache.xml";

        private Dictionary<string, ILookup<string, Type>> _cache;
        private object _lockObj = new object();

        // GKM - Constructor added, and initialize called
        public TestEdFiControllerTypeCache(params Assembly[] assemblies)
        {
            this.assemblies = assemblies;
            EnsureInitialized();
        }

        // =====================================================
        // GKM - Added for convenience for route unit testing
        // -----------------------------------------------------
        private IList<Type> _controllerTypes;
        public IList<Type> ControllerTypes
        {
            get { return _controllerTypes; }
        }
        // -----------------------------------------------------
        // GKM - End of addition
        // =====================================================

        int Count
        {
            get
            {
                int count = 0;
                foreach (var lookup in _cache.Values)
                {
                    foreach (var grouping in lookup)
                    {
                        count += grouping.Count();
                    }
                }
                return count;
            }
        }

        public void EnsureInitialized() // Internal type --> IBuildManager buildManager)
        {
            if (_cache == null)
            {
                lock(_lockObj)
                {
                    if (_cache == null)
                    {
                        _controllerTypes =  // GKM: Thanks to MS for these awful static dependencies --> TypeCacheUtil.GetFilteredTypesFromAssemblies(_typeCacheName, IsControllerType, buildManager);
                            (from a in assemblies
                             from t in a.GetTypes()
                             where IsControllerType(t)
                             select t)
                                .ToList();

                        var controllersToRemove = _controllerTypes.Where(
                            x =>
                                _controllerTypes.Where(
                                    y => y.Assembly == typeof(Marker_EdFi_Dashboards_Presentation_Web).Assembly)
                                    .Select(
                                        z =>
                                            typeof(Marker_EdFi_Dashboards_Presentation_Core).Assembly.GetName().Name +
                                            z.FullName.Substring(
                                                typeof(Marker_EdFi_Dashboards_Presentation_Web).Assembly.GetName()
                                                    .Name.Length))
                                    .Contains(x.FullName)).ToList();

                        _controllerTypes =
                            _controllerTypes.Where(x => !controllersToRemove.Contains(x)).ToList();

                        var groupedByName = _controllerTypes.GroupBy(
                            t => t.Name.Substring(0, t.Name.Length - "Controller".Length),
                            StringComparer.OrdinalIgnoreCase);
                        _cache = groupedByName.ToDictionary(
                            g => g.Key,
                            g => g.ToLookup(t => t.Namespace ?? String.Empty, StringComparer.OrdinalIgnoreCase),
                            StringComparer.OrdinalIgnoreCase);
                    }
                }
            }
        }

        public ICollection<Type> GetControllerTypes(string controllerName, HashSet<string> namespaces)
        {
            HashSet<Type> matchingTypes = new HashSet<Type>();

            ILookup<string, Type> nsLookup;
            if (_cache.TryGetValue(controllerName, out nsLookup))
            {
                // this friendly name was located in the cache, now cycle through namespaces
                if (namespaces != null)
                {
                    foreach (string requestedNamespace in namespaces)
                    {
                        foreach (var targetNamespaceGrouping in nsLookup)
                        {
                            if (IsNamespaceMatch(requestedNamespace, targetNamespaceGrouping.Key))
                            {
                                matchingTypes.UnionWith(targetNamespaceGrouping);
                            }
                        }
                    }
                }
                else
                {
                    // if the namespaces parameter is null, search *every* namespace
                    foreach (var nsGroup in nsLookup)
                    {
                        matchingTypes.UnionWith(nsGroup);
                    }
                }
            }

            return matchingTypes;
        }

        static bool IsControllerType(Type t)
        {
            return
                t != null &&
                t.IsPublic &&
                t.Name.EndsWith("Controller", StringComparison.OrdinalIgnoreCase) &&
                !t.IsAbstract &&
                typeof(IController).IsAssignableFrom(t);
        }

        static bool IsNamespaceMatch(string requestedNamespace, string targetNamespace)
        {
            // degenerate cases 
            if (requestedNamespace == null)
            {
                return false;
            }
            else if (requestedNamespace.Length == 0)
            {
                return true;
            }

            if (!requestedNamespace.EndsWith(".*", StringComparison.OrdinalIgnoreCase))
            {
                // looking for exact namespace match 
                return String.Equals(requestedNamespace, targetNamespace, StringComparison.OrdinalIgnoreCase);
            }
            else
            {
                // looking for exact or sub-namespace match
                requestedNamespace = requestedNamespace.Substring(0, requestedNamespace.Length - ".*".Length);
                if (!targetNamespace.StartsWith(requestedNamespace, StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }

                if (requestedNamespace.Length == targetNamespace.Length)
                {
                    // exact match 
                    return true;
                }
                else if (targetNamespace[requestedNamespace.Length] == '.')
                {
                    // good prefix match, e.g. requestedNamespace = "Foo.Bar" and targetNamespace = "Foo.Bar.Baz" 
                    return true;
                }
                else
                {
                    // bad prefix match, e.g. requestedNamespace = "Foo.Bar" and targetNamespace = "Foo.Bar2"
                    return false;
                }
            }
        }
    }
}