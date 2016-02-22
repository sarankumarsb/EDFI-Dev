using System;
using System.Text.RegularExpressions;

namespace EdFi.Dashboards.Presentation.Core.UITests.Support
{
    public static class StringExtensions
    {
        /// <summary>
        /// Removes newlines and spaces from the string, replacing each set of occurrences with a single space.
        /// </summary>
        /// <param name="text">The text to be normalized.</param>
        /// <returns>A string with single spaces where whitespace existed previously.</returns>
        public static string NormalizeSpacing(this string text)
        {
            return Regex.Replace(text, @"\s+", " ");
        }

        /// <summary>
        /// Removes whitespace from the string.
        /// </summary>
        /// <param name="text">The text to be stripped of whitespace.</param>
        /// <returns>A string without whitespace.</returns>
        public static string RemoveWhitespace(this string text)
        {
            return Regex.Replace(text, @"\s+", string.Empty);
        }

        /// <summary>
        /// Performs equality comparison of the string with another, ignoring case.
        /// </summary>
        public static bool EqualsIgnoreCase(this string text, string other)
        {
            return text.Equals(other, StringComparison.OrdinalIgnoreCase);
        }

        // For possible future use
        //public static string NormalizeFromCasing(this string text)
        //{
        //    return Regex.Replace(text, "([A-Z](?=[A-Z])|[A-Z][a-z0-9]+)", "$1 ");
        //}
    }
}