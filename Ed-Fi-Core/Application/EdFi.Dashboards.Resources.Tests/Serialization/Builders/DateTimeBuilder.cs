using System;

namespace EdFi.Dashboards.Resources.Tests.Serialization.Builders
{
    public class DateTimeBuilder : IValueBuilder
    {
        private DateTime nextDate = DateTime.Today.AddYears(-10);

        public bool TryBuild(string logicalPropertyPath, Type targetType, string context, out object value)
        {
            if (targetType.IsGenericType
                && targetType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                // Reassign the nullable type to the underlying target type
                targetType = Nullable.GetUnderlyingType(targetType);
            }

            if (targetType == typeof(DateTime))
            {
                value = nextDate;
                nextDate = nextDate.AddDays(1);
                return true;
            }

            value = null;
            return false;
        }

        public void Reset()
        {
            nextDate = DateTime.Today.AddYears(-10);
        }

        public TestObjectFactory Factory { get; set; }
    }
}
