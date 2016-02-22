using System;
using System.Collections.Generic;
using System.Linq;

namespace EdFi.Dashboards.Resources.Tests.Serialization.Builders
{
    public class EnumBuilder : IValueBuilder
    {
        private Dictionary<Type, int> valueIndicesByEnumType = new Dictionary<Type, int>();
        private Dictionary<Type, List<int>> valuesByEnumType = new Dictionary<Type, List<int>>();

        public bool TryBuild(string logicalPropertyPath, Type targetType, string context, out object value)
        {
            if (targetType.IsEnum)
            {
                // Initialize enum values
                if (!valuesByEnumType.ContainsKey(targetType))
                {
                    valuesByEnumType[targetType] = Enum.GetValues(targetType).Cast<int>().ToList();
                    valueIndicesByEnumType[targetType] = 0;
                }

                // Get the current index and list of enum values
                int index = valueIndicesByEnumType[targetType];
                var enumValues = valuesByEnumType[targetType];
                
                // Get the next enum value
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
            valuesByEnumType = new Dictionary<Type, List<int>>();
        }

        public TestObjectFactory Factory { get; set; }
    }
}
