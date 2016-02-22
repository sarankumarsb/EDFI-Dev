using System;
using System.Collections.Generic;

namespace EdFi.Dashboards.Resources.Tests.Serialization.Builders
{
    public class KeyValuePairBuilder : IValueBuilder
    {
        public bool TryBuild(string logicalPropertyPath, Type targetType, string context, out object value)
        {
            if (targetType.IsGenericType && targetType.GetGenericTypeDefinition() == typeof(KeyValuePair<,>))
            {
                var typeArgs = targetType.GetGenericArguments();
                var kvpType = typeof(KeyValuePair<,>).MakeGenericType(typeArgs);
                
                var constructor = kvpType.GetConstructor(typeArgs);
                
                var entryKey = Factory.Create("xyz", typeArgs[0]);
                var entryValue = Factory.Create("xyz", typeArgs[1]);

                var kvp = constructor.Invoke(new[] {entryKey, entryValue});

                value = kvp;
                return true;
            }

            value = null;
            return false;
        }

        public void Reset()
        {
            
        }

        public TestObjectFactory Factory { get; set; }
    }
}
