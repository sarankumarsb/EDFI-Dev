using System;
using System.Collections.Generic;

namespace EdFi.Dashboards.Resources.Tests.Serialization.Builders
{
    public class NumericValueBuilder : IValueBuilder
    {
        private Dictionary<Type, bool> nextNullableResultByType = new Dictionary<Type, bool>();

        public bool TryBuild(string logicalPropertyPath, Type targetType, string context, out object value)
        {
            value = null;

            if (!targetType.IsValueType)
                return false;

            if (targetType.IsGenericType
                && targetType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                bool nextResultIsNull;

                // Get or initialize the flag for this nullable type
                if (!nextNullableResultByType.TryGetValue(targetType, out nextResultIsNull))
                    nextNullableResultByType[targetType] = false;

                // Flip the result for the next time we request an instance of this nullable type
                nextNullableResultByType[targetType] = !nextResultIsNull;

                // If this result should be null, do so now
                if (nextResultIsNull)
                    return true;

                // Reassign the nullable type to the underlying target type
                targetType = Nullable.GetUnderlyingType(targetType);
            }

            if (targetType == typeof(UInt16)
                || targetType == typeof(UInt32)
                || targetType == typeof(UInt64)
                || targetType == typeof(Byte)
                || targetType == typeof(SByte)
                || targetType == typeof(Int16)
                || targetType == typeof(Int32)
                || targetType == typeof(Int64)
                || targetType == typeof(Decimal)
                || targetType == typeof(Double)
                || targetType == typeof(Single)
                )
            {
                value = GetNextValue(targetType);
                return true;
            }

            return false;
        }

        public void Reset()
        {
            nextValueByType.Clear();
        }

        public TestObjectFactory Factory { get; set; }

        private readonly Dictionary<Type, dynamic> nextValueByType = new Dictionary<Type, dynamic>();

        private dynamic GetNextValue(Type type)
        {
            if (!nextValueByType.ContainsKey(type))
                nextValueByType[type] = type.GetDefault() + (dynamic) Convert.ChangeType(.001, type);

            dynamic nextValue = Convert.ChangeType(nextValueByType[type] + 1, type);
            nextValueByType[type] = nextValue;
            return nextValue;
        }
    }

    public static class ReflectionUtility
    {
        public static dynamic GetDefault(this Type type)
        {
            if (type.IsValueType)
                return Activator.CreateInstance(type);

            return null;
        }
    }
}
