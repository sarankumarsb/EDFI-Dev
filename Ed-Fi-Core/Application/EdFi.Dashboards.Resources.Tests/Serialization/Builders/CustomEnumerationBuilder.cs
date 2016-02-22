using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EdFi.Dashboards.Core;

namespace EdFi.Dashboards.Resources.Tests.Serialization.Builders
{
    public class CustomEnumerationBuilder : IValueBuilder
    {
        private Dictionary<Type, int> valueIndicesByEnumType = new Dictionary<Type, int>();
        private Dictionary<Type, List<object>> valuesByEnumType = new Dictionary<Type, List<object>>();

        private Dictionary<Type, Type> possibleEnumerationTypesByType = new Dictionary<Type, Type>(); 

        public bool TryBuild(string logicalPropertyPath, Type targetType, string context, out object value)
        {
            Type possibleEnumerationType;

            if (!possibleEnumerationTypesByType.TryGetValue(targetType, out possibleEnumerationType))
            {
                try
                {
                    possibleEnumerationType = typeof(Enumeration<>).MakeGenericType(targetType);
                }
                catch (ArgumentException)
                {
                    possibleEnumerationTypesByType[targetType] = null;
                    value = null;
                    return false;
                }
            }

            if (possibleEnumerationType == null)
            {
                value = null;
                return false;
            }

            // Is this a custom class-based enumeration replacement?
            if (possibleEnumerationType.IsAssignableFrom(targetType))
            {
                Type enumType = targetType;

                // Initialize enum values
                if (!valuesByEnumType.ContainsKey(targetType))
                {
                    var enumValueProperties = 
                        from p in targetType.GetFields(BindingFlags.Static | BindingFlags.Public)
                        where p.FieldType == enumType
                        select p;

                    valuesByEnumType[targetType] =
                        (from p in enumValueProperties
                        select p.GetValue(null))
                        .ToList();

                    valueIndicesByEnumType[targetType] = 0;
                }

                // Get the current index and list of enum values
                int index = valueIndicesByEnumType[targetType];
                var enumValues = valuesByEnumType[targetType];

                // Get the next enum value
                if (index >= enumValues.Count)
                    System.Diagnostics.Debugger.Break();

                value = enumValues[index];

                // Increment/cycle the index for this enum type
                valueIndicesByEnumType[targetType] = (index + 1) % enumValues.Count;

                return true;
            }

            value = null;
            return false;
        }

        public void Reset()
        {
            valueIndicesByEnumType = new Dictionary<Type, int>();
            valuesByEnumType = new Dictionary<Type, List<object>>();
        }

        public TestObjectFactory Factory { get; set; }
    }
}
