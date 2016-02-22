using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

#if POSTSHARP
using PostSharp.Aspects;
#else
using EdFi.Dashboards.Presentation.Core.UITests.Support.Aop.Architecture;
#endif

namespace EdFi.Dashboards.Presentation.Core.UITests.Support.Aop.Aspects
{
    [Serializable]
    public abstract class ConventionBasedMethodOverrideAspectBase : OnMethodBoundaryAspect
    {
        private static Dictionary<string, Type> overrideTypesByFullName = new Dictionary<string, Type>();

        protected bool TryGetOverrideMethod(Type overrideType, MethodBase invokedMethod, out MethodInfo overrideMethod)
        {
            string methodName = invokedMethod.Name;
            Type[] parameters = invokedMethod.GetParameters().Select(p => p.ParameterType).ToArray();

            overrideMethod = overrideType.GetMethod(methodName, parameters);

            return (overrideMethod != null);
        }

        protected string GetExpectedOverrideTypeName(Type stepsType, string requiredSuffix = null)
        {
            string overrideTypeName = 
                string.Format("{0}.{1}",
                              stepsType.Namespace.Replace(".Core.", ".Web."),
                              stepsType.Name + requiredSuffix);

            return overrideTypeName;
        }

        protected bool TryGetOverrideType(string overrideTypeName, out Type overrideType)
        {
            if (!overrideTypesByFullName.TryGetValue(overrideTypeName, out overrideType))
            {
                overrideType = Type.GetType(overrideTypeName);
                overrideTypesByFullName[overrideTypeName] = overrideType;
            }

            return overrideType != null;
        }
    }
}