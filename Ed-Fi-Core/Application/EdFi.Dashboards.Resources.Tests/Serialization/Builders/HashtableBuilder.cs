using System;
using System.Collections;

namespace EdFi.Dashboards.Resources.Tests.Serialization.Builders
{
    public class HashtableBuilder : IValueBuilder
    {
        private Type[] types = new []
            {
                typeof(int),
                typeof(int?),
                typeof(string),
                typeof(double),
                typeof(double?),
                typeof(float),
                typeof(float?),
            };

        private int index = 0;

        public bool TryBuild(string logicalPropertyPath, Type targetType, string context, out object value)
        {
            if (targetType == typeof(Hashtable))
            {
                var hashtable = new Hashtable();

                for (int i = 0; i < Factory.CollectionCount; i++)
                {
                    var entryKey = GetEntryKey(logicalPropertyPath, typeof(string));
                    //var entryKey = Factory.Create(logicalPropertyPath + "+Key", typeof(string));
                    IncrementIndex();
                    var entryValue = Factory.Create(logicalPropertyPath + "+Value", types[index]);
                    IncrementIndex();

                    hashtable.Add(entryKey, entryValue);
                }

                value = hashtable;
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

        private void IncrementIndex()
        {
            index = (index + 1) % types.Length;
        }

        public void Reset()
        {
            
        }

        public TestObjectFactory Factory { get; set; }
    }
}
