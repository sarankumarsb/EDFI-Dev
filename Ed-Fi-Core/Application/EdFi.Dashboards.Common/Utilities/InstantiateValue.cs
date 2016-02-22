// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;

namespace EdFi.Dashboards.Common.Utilities
{
    public static class InstantiateValue
    {
        public static dynamic FromValueType(string value, string valueType)
        {
            if (String.IsNullOrEmpty(valueType))
                return null;

            if (String.IsNullOrEmpty(value))
                return null;

            Type t = valueType.Contains("System.") ? Type.GetType(valueType) : typeof(string);
            return FromValueType(value, t);
        }

        public static dynamic FromValueType(dynamic value, Type conversionType)
        {
            return Convert.ChangeType(value, conversionType);
        }
    }
}
