// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;
using System.Linq;
using System.Runtime.InteropServices;
using EdFi.Dashboards.Common.Utilities;

namespace EdFi.Dashboards.Common
{
    public static class StringExtensions
    {
        public static T Convert<T>(this string text)
        {
            Type targetType = typeof(T);

            // No text? Return default value for the target type.
            if (string.IsNullOrWhiteSpace(text))
                return (T) ReflectionUtility.GetDefault(targetType);

            if (targetType.IsGenericType && targetType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                // Unwrap the nullable type to the underlying value type
                targetType = targetType.GetGenericArguments()[0]; // Nullable types should only have 1 generic argument
            }

            return (T) System.Convert.ChangeType(text, targetType);
        }

        public static string FormatWith(this string text, params object[] args)
        {
            return string.Format(text, args);
        }

        public static bool IsMixedCase(this string text)
        {
            if (text.Any(c => c >= 'A' && c <= 'Z')
                && text.Any(c => c >= 'a' && c <= 'z'))
            {
                return true;
            }

            return false;
        }

        public static string Base64Decode(this string text)
        {
            if (String.IsNullOrEmpty(text))
                return text;

            byte[] decodedBytes = System.Convert.FromBase64String(text);
            var decodedString = System.Text.Encoding.UTF8.GetString(decodedBytes);
            return decodedString;
        }

        //More concise replace extension.
        public static string Replace(this string originalString, string oldValue, string newValue, StringComparison comparisonType)
        {
            int startIndex = 0;
            while (true)
            {
                startIndex = originalString.IndexOf(oldValue, startIndex, comparisonType);
                if (startIndex == -1)
                    break;

                originalString = originalString.Substring(0, startIndex) + newValue + originalString.Substring(startIndex + oldValue.Length);

                startIndex += newValue.Length;
            }

            return originalString;
        }

        public static string Replace(this string source, string oldValue, string newValue, bool caseInsensitive)
        {
            // Pass through to .NET's case-sensitive version
            if (!caseInsensitive)
                return source.Replace(oldValue, newValue);

            int count, position0, position1;
            count = position0 = position1 = 0;
            
            string upperString = source.ToUpper();
            string upperPattern = oldValue.ToUpper();
            
            int inc = (source.Length / oldValue.Length) *
                      (newValue.Length - oldValue.Length);
            
            char[] chars = new char[source.Length + Math.Max(0, inc)];
            
            while ((position1 = upperString.IndexOf(upperPattern, position0)) != -1)
            {
                for (int i = position0; i < position1; ++i)
                    chars[count++] = source[i];
                for (int i = 0; i < newValue.Length; ++i)
                    chars[count++] = newValue[i];
                position0 = position1 + oldValue.Length;
            }

            if (position0 == 0) 
                return source;
            
            for (int i = position0; i < source.Length; ++i)
                chars[count++] = source[i];
            
            return new string(chars, 0, count);
        }

        [DllImport("Shlwapi.dll", SetLastError = true, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private extern static bool PathFileExists(string path);

        public static bool FileExists(this string filename)
        {
            return PathFileExists(filename);
        }
    }
}
