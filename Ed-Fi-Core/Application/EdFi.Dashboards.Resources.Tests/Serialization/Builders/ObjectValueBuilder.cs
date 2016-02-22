using System;

namespace EdFi.Dashboards.Resources.Tests.Serialization.Builders
{
    public class ObjectValueBuilder : IValueBuilder
    {
        private int nextValue = 1;

        public bool TryBuild(string logicalPropertyPath, Type targetType, string context, out object value)
        {
            if (targetType == typeof(object))
            {
                value = "Object" + nextValue++;
                return true;
            }

            value = null;
            return false;
        }

        public void Reset()
        {
            nextValue = 1;
        }

        public TestObjectFactory Factory { get; set; }
    }
}
