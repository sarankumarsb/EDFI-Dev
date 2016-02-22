using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace EdFi.Dashboards.Resources.Tests.Serialization.Builders
{
    public class IEnumerableBuilder : IValueBuilder
    {
        public bool TryBuild(string logicalPropertyPath, Type targetType, string context, out object value)
        {
            value = null;

            if (targetType.IsArray
                ||
                (targetType.IsGenericType 
                    && (targetType.GetGenericTypeDefinition() == typeof(IEnumerable<>)
                        || targetType.GetGenericTypeDefinition() == typeof(IList<>)
                        || targetType.GetGenericTypeDefinition() == typeof(List<>)
                        // IQueryable is generally not serializable, but this is left here for possible future inclusion
                        // || targetType.GetGenericTypeDefinition() == typeof(IQueryable<>)
                )))
            {
                Type itemType;

                if (targetType.IsArray)
                {
                    itemType = targetType.GetElementType();
                }
                else
                {
                    itemType = targetType.GetGenericArguments()[0];
                }

                Type listType = typeof(List<>).MakeGenericType(itemType);
                var list = Activator.CreateInstance(listType);
                var addMethod = listType.GetMethod("Add", new [] { itemType });

                for (int i = 0; i < 2; i++)
                {
                    var newItem = Factory.Create(logicalPropertyPath + "[" + i + "]", itemType);
                    
                    if (newItem != null)
                        addMethod.Invoke(list, new[] {newItem});
                }

                if (targetType.IsArray)
                {
                    // Convert list to an array
                    var toArrayMethod = listType.GetMethod("ToArray");
                    value = toArrayMethod.Invoke(list, null);
                }
#region IQueryable support (commented out)
                // IQueryable is generally not serializable, but this is left here for possible future inclusion
                //else if (targetType.GetGenericTypeDefinition() == typeof(IQueryable<>))
                //{
                //    // Convert list to an IQueryable
                //    var asQueryableMethod = typeof(Queryable).GetMethod("AsQueryable",
                //           new[] {typeof(IEnumerable<>).MakeGenericType(itemType)});

                //    value = asQueryableMethod.Invoke(null, new[] {list});
                //}
#endregion                
                else
                {
                    value = list;
                }

                return true;
            }

            return false;
        }

        public void Reset()
        {
        }

        public TestObjectFactory Factory { get; set; }
    }
}
