// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using EdFi.Dashboards.Common.Utilities;
using EdFi.Dashboards.Metric.Resources.Models;

namespace EdFi.Dashboards.Resources
{
    public static class Utilities
    {
        private const string nameFormat = "{0}, {1}{4}{2}{3}";
        private const string period = ".";
        private const string space = " ";

        static Utilities()
        {
            Metrics = new Metric();
        }

        public static Metric Metrics { get; private set; }

        public static string ToYesNo(this bool expression)
        {
            return expression ? "Yes" : "No";
        }

        public static string ToYesNo(this bool? expression)
        {
            return expression == null ? string.Empty : ToYesNo(expression.Value);
        }

        public static string FormatPersonNameByLastName(string firstName, string middleName, string lastName)
        {
            return String.Format(nameFormat, lastName, firstName, ((String.IsNullOrEmpty(middleName)) ? String.Empty : middleName), ((!String.IsNullOrEmpty(middleName) && middleName.Length == 1) ? period : String.Empty), ((String.IsNullOrEmpty(middleName)) ? String.Empty : space));
        }

        /// <summary>
        /// returns grade level text from grade number.
        /// </summary>
        /// <param name=""></param>
        /// <returns>text for grade level</returns>
        public static string GetGradeLevelFromSort(int gradeSort)
        {

            switch (gradeSort)
            {
                case 1:
                    return "1st Grade";
                case 2:
                    return "2nd Grade";
                case 3:
                    return "3rd Grade";
                case 4:
                    return "4th Grade";
                case 5:
                    return "5th Grade";
                case 6:
                    return "6th Grade";
                case 7:
                    return "7th Grade";
                case 8:
                    return "8th Grade";
                case 9:
                    return "9th Grade";
                case 10:
                    return "10th Grade";
                case 11:
                    return "11th Grade";
                case 12:
                    return "12th Grade";
                case 13:
                    return "Postsecondary";
                case -3:
                    return "Early Education";
                case -2:
                    return "Infant/toddler";
                case -1:
                    return "Preschool/Prekindergarten";
                //case "Transitional Kindergarten":
                //    level = 0;
                //    break;
                case 0:
                    return "Kindergarten";
                case 14:
                    return "Ungraded";
                default:
                    return "Unknown";
            }

        }
        public class Metric
        {
            //Gets some context information so that the metric name makes sense.
            public string GetMetricName(dynamic metric)
            {
                var sb = new StringBuilder();

                if (metric.Parent != null)
                {
                    if (metric.Parent.Parent != null && metric.Parent.Parent.MetricType == MetricType.ContainerMetric)
                        sb.AppendFormat("{0} - ", metric.Parent.Parent.DisplayName);

                    sb.AppendFormat("{0} - ", metric.Parent.DisplayName);
                }
                sb.Append(metric.DisplayName);
                return sb.ToString();
            }
        }

        /// <summary>
        /// Builds a List from all the non null input values.
        /// </summary>
        /// <param name="valuesToInclude"></param>
        /// <returns></returns>
        public static List<string> BuildCompactList(params string[] valuesToInclude)
        {
            return valuesToInclude.Where(s => !String.IsNullOrEmpty(s)).ToList();
        }

        public static string ToFormattedString<T>(this IEnumerable<T> enumerable)
        {
            var sb = new StringBuilder();
            bool first = true;
            foreach (var e in enumerable)
            {
                if (!first)
                    sb.Append(", ");
                sb.Append(e);
                first = false;
            }
            return sb.ToString();
        }

        public static string FormattedTelephoneNumber(string telephone)
        {
            if (String.IsNullOrEmpty(telephone))
                return telephone;

            //First we remove any special character that is not a number. ()-./spaces,etc...
            //Leaving only numbers.
            string ret = Regex.Replace(telephone, "[^0-9]", "");

            //Expected format.
            //(###) ###-####
            switch (ret.Length)
            {
                case 10:
                    ret = Regex.Replace(ret, @"(\d{3})(\d{3})(\d{4})", "($1) $2-$3");
                    break;
                case 7:
                    ret = Regex.Replace(ret, @"(\d{3})(\d{4})", "$1-$2");
                    break;
                default:
                    ret = telephone;
                    break;
            }

            return ret;
        }
    }
}
