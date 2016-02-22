#if !POSTSHARP

using System;
using System.Linq;
using System.Text.RegularExpressions;
using EdFi.Dashboards.Presentation.Core.UITests.Support.Aop.Architecture;
using Mono.Cecil;

namespace ApplyAspects
{
    public static class ReflectionSpecificationExtensions
    {
        public static bool IsMatch(this TypeDefinition type, CustomAttribute attribute)
        {
            string pattern = attribute.GetValue<string>("AttributeTargetTypes");

            var regexText = GetRegexFromPattern(pattern);

            if (!Regex.IsMatch(type.FullName, regexText))
                return false;

            var typeAttributes = attribute.GetValue<MulticastAttributes>("AttributeTargetTypeAttributes");

            if (typeAttributes.HasFlag(MulticastAttributes.Public))
            {
                if (!type.IsPublic)
                    return false;
            }

            return true;
        }

        public static bool IsMatch(this MethodDefinition methodInfo, CustomAttribute attribute)
        {
            string pattern = attribute.GetValue<string>("AttributeTargetMembers");

            // If pattern was provided, exit if the method name doesn't match
            if (!string.IsNullOrEmpty(pattern))
            {
                if (!Regex.IsMatch(methodInfo.Name, GetRegexFromPattern(pattern)))
                    return false;
            }

            var methodAttributes = attribute.GetValue<MulticastAttributes>("AttributeTargetMemberAttributes");

            if (methodAttributes.HasFlag(MulticastAttributes.Public))
            {
                if (!methodInfo.IsPublic)
                    return false;
            }

            if (methodAttributes.HasFlag(MulticastAttributes.Instance))
            {
                if (methodInfo.IsStatic)
                    return false;

                bool result = methodAttributes.HasFlag(MulticastAttributes.Virtual) == methodInfo.IsVirtual;

                return result;
            }

            return true;
        }

        private static string GetRegexFromPattern(string pattern)
        {
            string regexText;

            if (pattern.StartsWith("regex:"))
            {
                regexText = pattern.Substring("regex:".Length);
            }
            else
            {
                regexText = pattern
                    .Replace(".", @"\.") // Convert dots to literal dots for regex
                    .Replace(@"*", @".*"); // Convert "extended" match .* to equivalent regular expression syntax
            }

            return regexText;
        }

        #region Original attempt - Commented out

        //public static bool IsMatch(this TypeDefinition type, OnMethodBoundaryAspect aspect)
        //{
        //    string regexText = aspect.AttributeTargetTypes
        //        .Replace(".", @"\.")    // Convert dots to literal dots for regex
        //        .Replace(@"*", @".*");  // Convert "extended" match .* to equivalent regular expression syntax

        //    if (!Regex.IsMatch(type.FullName, regexText))
        //        return false;

        //    var typeAttributes = aspect.AttributeTargetTypeAttributes;

        //    if (typeAttributes.HasFlag(MulticastAttributes.Public))
        //    {
        //        if (!type.IsPublic)
        //            return false;
        //    }

        //    return true;
        //}

        //public static bool IsMatch(this MethodDefinition methodInfo, OnMethodBoundaryAspect aspect)
        //{
        //    var methodAttributes = aspect.AttributeTargetMemberAttributes;

        //    if (methodAttributes.HasFlag(MulticastAttributes.Public))
        //    {
        //        if (!methodInfo.IsPublic)
        //            return false;
        //    }

        //    if (methodAttributes.HasFlag(MulticastAttributes.Instance))
        //    {
        //        if (methodInfo.IsStatic)
        //            return false;
        //    }

        //    if (methodAttributes.HasFlag(MulticastAttributes.Virtual))
        //    {
        //        if (!methodInfo.IsVirtual)
        //            return false;
        //    }

        //    return true;
        //}

        #endregion

        internal static T GetValue<T>(this CustomAttribute attribute, string propertyName)
        {
            var argument =
                (from p in attribute.Properties
                 where p.Name == propertyName
                 select p.Argument)
                    .SingleOrDefault();

            if (argument.Equals(default(CustomAttributeArgument)))
            {
                return default(T);
            }

            if (typeof(T).IsEnum)
                return (T) argument.Value;

            return (T) Convert.ChangeType(argument.Value, typeof(T));
        }

        public static T GetValue<T>(this CustomAttribute attribute, string propertyName, T defaultValue)
        {
            var result = attribute.GetValue<T>(propertyName);

            if (result == null)
                return defaultValue;

            if (result.Equals(default(T)))
                return defaultValue;

            return result;
        }
    }
}

#endif