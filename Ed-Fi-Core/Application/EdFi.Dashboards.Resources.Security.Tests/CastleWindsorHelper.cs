using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EdFi.Dashboards.Common.Utilities;

namespace EdFi.Dashboards.Resources.Security.Tests
{
    internal class CastleWindsorHelper
    {
        public static IEnumerable<Type> GetServiceTypesFromIOC(Assembly assembly)
        {
            var container = IoC.Container;
            var kernel = container.Kernel;
            var handlers = kernel.GetAssignableHandlers(typeof(object));
            var interfaces = handlers.SelectMany(h => h.ComponentModel.Services);
            var result = from i in interfaces
                         let currentAssembly = i.Assembly
                         let name = i.Name.ToLower()
                         where currentAssembly == assembly && name.StartsWith("i") && name.Contains("service")
                         orderby i.Name
                         select i;

            return result;
        }
    }
}
