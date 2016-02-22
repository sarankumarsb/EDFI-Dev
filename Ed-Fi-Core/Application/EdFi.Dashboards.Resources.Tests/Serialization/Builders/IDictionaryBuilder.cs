using System;
using System.Collections.Generic;

namespace EdFi.Dashboards.Resources.Tests.Serialization.Builders
{
    public class IDictionaryBuilder : IValueBuilder
    {
        public bool TryBuild(string logicalPropertyPath, Type targetType, string context, out object value)
        {
            if (targetType.IsGenericType && targetType.GetGenericTypeDefinition() == typeof(IDictionary<,>))
            {
                var typeArgs = targetType.GetGenericArguments();
                Type dictionaryType = typeof(Dictionary<,>).MakeGenericType(typeArgs);
                var dictionary = Activator.CreateInstance(dictionaryType);
                var addMethod = dictionaryType.GetMethod("Add", typeArgs);

                for (int i = 0; i < Factory.CollectionCount; i++)
                {
                    var entryKey = GetEntryKey(logicalPropertyPath, typeArgs[0]);

                    var entryValue = Factory.Create(logicalPropertyPath + "+Value", typeArgs[1]);

                    addMethod.Invoke(dictionary, new[] { entryKey, entryValue });
                }

                value = dictionary;
                return true;
            }

            value = null;
            return false;
        }

        private object GetEntryKey(string logicalPropertyPath, Type keyType)
        {
            object entryKey = null;
            int attempts = 0;

            // Make sure we get an actual key value (can't use null)
            while (entryKey == null 
                    || (keyType == typeof(string) && string.IsNullOrEmpty((string)entryKey)) 
                && attempts < 3)
            {
                entryKey = Factory.Create(logicalPropertyPath + "+Key", keyType);
                attempts++;
            }

            return entryKey;
        }

        public void Reset()
        {
            
        }

        public TestObjectFactory Factory { get; set; }
    }
}
