// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading;
using Castle.DynamicProxy;

namespace EdFi.Dashboards.Resources.Security.Common
{
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Method | AttributeTargets.Property, Inherited = true)]
    public sealed class AuthenticationIgnoreAttribute : Attribute
    {
        private static Dictionary<string, bool> memberInfoCache = new Dictionary<string, bool>();

        public string Justification { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthenticationIgnoreAttribute"/> class using the provided justification.
        /// </summary>
        /// <param name="justification">A justification for the use of the attribute to exclude the parameter, method or property from authorization.</param>
        public AuthenticationIgnoreAttribute(string justification)
        {
            if (string.IsNullOrWhiteSpace(justification))
                throw new ArgumentException("A justification must be provided.", "justification");

            Justification = justification;
        }

        private static readonly string[] NeverIgnoreParameterNames =
            new[]
                {
                    "staffcohortid",
                    "teachersectionid",
                    "sectionorcohortid",
                    "studentusi"
                };

        private static bool NeverIgnoreParameter(string name)
        {
            var result = NeverIgnoreParameterNames.Any(p => p.ToLower() == name.ToLower());
            return result;
        }


        public static bool MarkedAuthenticationIgnore(IInvocation invocation)
        {
            var method = invocation.MethodInvocationTarget;
            var id = method.GetMethodUniqueIdentifier();

            bool isMarked;

            if (!memberInfoCache.TryGetValue(id, out isMarked))
            {
                isMarked = method.GetCustomAttributes(typeof (AuthenticationIgnoreAttribute), true).Any();
                AddToMemberInfoCache(id, isMarked);
            }
  
            return isMarked;
        }

        /// <summary>
        /// Adds an item to memberInfoCache in a non-locking way but still thread-safe.
        /// </summary>
        private static void AddToMemberInfoCache(string key, bool value)
        {
            Dictionary<string, bool> snapshot, newCache;

            do
            {
                snapshot = memberInfoCache;
                newCache = new Dictionary<string, bool>(memberInfoCache);
                newCache[key] = value;
                //If the Interlocked.CompareExchange fails, it means we lost the cache update race with another thread, 
                //therefore just try again.
            } while (!ReferenceEquals(
                Interlocked.CompareExchange(ref memberInfoCache, newCache, snapshot),
                snapshot));
        }

        public static bool MarkedAuthenticationIgnore(ParameterInfo parameterInfo)
        {
            var result = parameterInfo.GetCustomAttributes(typeof(AuthenticationIgnoreAttribute), false).Length > 0;

            if (false == result) return false;

            if (NeverIgnoreParameter(parameterInfo.Name))
                throw new ArgumentException(string.Format("Parameter: {0} can not be marked ignore.", parameterInfo.Name));

            return true;
        }

        public static bool MarkedAuthenticationIgnore(PropertyInfo propInfo)
        {
            var result = propInfo.GetCustomAttributes(typeof(AuthenticationIgnoreAttribute), false).Length > 0;

            if (false == result) return false;

            if (NeverIgnoreParameter(propInfo.Name))
                throw new ArgumentException(string.Format("Property: {0} can not be marked ignore.", propInfo.Name));

            return true;
        }
    }

    public static class ReflectionExtensions
    {
        public static string GetMethodUniqueIdentifier(this MemberInfo memberInfo)
        {
            return string.Format(CultureInfo.InvariantCulture, "{0}>{1}", memberInfo.Module.Name, memberInfo.MetadataToken);
        }
    }
}
