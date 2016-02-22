using System;
using System.Collections.Generic;
using System.Linq;

namespace EdFi.Dashboards.Resources.Tests.Serialization.Builders
{
    public class CustomObjectBuilder : IValueBuilder
    {
        private static Dictionary<Type, int> depthByType = new Dictionary<Type, int>(); 

        public bool TryBuild(string logicalPropertyPath, Type targetType, string context, out object value)
        {
            value = null;

            if (!targetType.IsValueType 
                && targetType != typeof(string)
                && !targetType.FullName.StartsWith("System."))
            {
                // Start or augment logical property path
                if (string.IsNullOrEmpty(logicalPropertyPath))
                    logicalPropertyPath = targetType.Name;
                else
                    logicalPropertyPath += "." + targetType.Name;

                // Initialize depth for type if it doesn't yet exist
                if (!depthByType.ContainsKey(targetType))
                    depthByType[targetType] = 0;

                // Are we already building an instance of this type of object?
                int depth = depthByType[targetType];

                // Don't go any deeper than 2
                if (depth == Factory.MaximumHierarchyDepth)
                    return true;

                depthByType[targetType] = depth + 1;

                try
                {
                    // Attempt to create the object using a default constructor
                    try
                    {
                        value = Activator.CreateInstance(targetType);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(string.Format("Unable to create type '{0}' at {1}.", targetType.FullName, logicalPropertyPath), ex);
                    }

                    // Get its public properties
                    var writableProperties = targetType.GetProperties().Where(p => p.CanWrite);

                    // Get a list of public, writable properties
                    foreach (var property in writableProperties)
                    {
                        var attributes = property.GetCustomAttributes(true);

                        // Don't create data for properties marked to be ignored for serialization
                        if (attributes.Any(a => a.GetType().Name.Contains("Ignore")))
                            continue;

                        object propertyValue = Factory.Create(logicalPropertyPath + "." + property.Name, property.PropertyType);
                        property.SetValue(value, propertyValue, null);
                    }
                }
                finally
                {
                    depthByType[targetType] = depthByType[targetType] - 1;
                }

                return true;
            }

            return false;
        }

        public virtual void Reset()
        {
            depthByType = new Dictionary<Type, int>();
        }

        public TestObjectFactory Factory { get; set; }
    }
}
