using System;

namespace EdFi.Dashboards.Resources.Tests.Serialization.Builders
{
    public class BooleanBuilder : IValueBuilder
    {
        private bool nextValue = true;

        public bool TryBuild(string logicalPropertyPath, Type targetType, string context, out object value)
        {
            if (targetType.IsGenericType
                && targetType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                // Reassign the nullable type to the underlying target type
                targetType = Nullable.GetUnderlyingType(targetType);
            }

            if (targetType == typeof(bool))
            {
                value = nextValue;
                nextValue = !nextValue;
                return true;
            }

            value = null;
            return false;
        }

        public void Reset()
        {
            nextValue = true;
        }

        public TestObjectFactory Factory { get; set; }
    }
}
