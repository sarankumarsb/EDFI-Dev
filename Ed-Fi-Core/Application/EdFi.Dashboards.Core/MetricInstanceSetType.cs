// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System;

namespace EdFi.Dashboards.Core
{
    public enum MetricInstanceSetType
    {
        None,
        StudentSchool,
        Staff,
        School,
        LocalEducationAgency
    }

    public static class MetricInstanceSetTypeExtensions
    {
        public static bool EqualTo(this MetricInstanceSetType value, string compare)
        {
            return compare.Equals(value);
        }

        public static bool EqualTo(this string value, MetricInstanceSetType compare)
        {
            return value.Replace(" ", "").Equals(compare.ToString(), StringComparison.OrdinalIgnoreCase);
        }
    }
}