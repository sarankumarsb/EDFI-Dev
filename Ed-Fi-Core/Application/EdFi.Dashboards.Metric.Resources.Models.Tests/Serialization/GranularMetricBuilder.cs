using System;
using EdFi.Dashboards.Resources.Tests.Serialization;

namespace EdFi.Dashboards.Metric.Resources.Models.Tests.Serialization
{
    public class GranularMetricBuilder : IValueBuilder
    {
        private Type[] dataTypes = new []
            {
                typeof(int), 
                typeof(double),
                typeof(decimal),
                typeof(float),
            };

        private int dataTypeIndex = 0;

        public bool TryBuild(string logicalPropertyPath, Type targetType, string context, out object value)
        {
            if (targetType.IsGenericTypeDefinition && targetType.GetGenericTypeDefinition() == typeof(GranularMetric<>))
            {
                Type valueType = typeof(GranularMetric<>).MakeGenericType(dataTypes[dataTypeIndex]);
                value = Factory.Create(logicalPropertyPath, valueType);

                dataTypeIndex = (dataTypeIndex + 1) % dataTypes.Length;
                return true;
            }

            value = null;
            return false;
        }

        public void Reset()
        {
            dataTypeIndex = 0;
        }

        public TestObjectFactory Factory { get; set; }
    }
}
