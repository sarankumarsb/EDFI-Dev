using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EdFi.Dashboards.Resources.Tests.Serialization;

namespace EdFi.Dashboards.Metric.Resources.Models.Tests.Serialization
{
    public class MetricBaseBuilder : IValueBuilder
    {
        private int subType = 0;
        private int depth = 0;

        public bool TryBuild(string logicalPropertyPath, Type targetType, string context, out object value)
        {
            value = null;

            if (targetType == typeof(MetricBase))
            {
                try
                {
                    if (depth > 2)
                        return true;

                    depth++;

                    switch (subType)
                    {
                        case 0:
                            value = Factory.Create(logicalPropertyPath, typeof(AggregateMetric));
                            break;
                        case 1:
                            value = Factory.Create(logicalPropertyPath, typeof(ContainerMetric));
                            break;
                        case 2:
                            value = Factory.Create(logicalPropertyPath, typeof(GranularMetric<>));
                            break;
                    }

                    // Cycle the sub-type
                    subType = (subType + 1)%3;
                    return true;
                }
                finally
                {
                    depth--;
                }
            }

            return false;
        }

        public void Reset()
        {
            subType = 0;
        }

        public TestObjectFactory Factory { get; set; }
    }
}
