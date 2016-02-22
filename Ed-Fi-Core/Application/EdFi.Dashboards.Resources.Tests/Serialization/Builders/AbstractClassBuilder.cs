using System;
using System.Collections.Generic;
using System.Linq;

namespace EdFi.Dashboards.Resources.Tests.Serialization.Builders
{
    public class AbstractClassBuilder : IValueBuilder
    {
        private Dictionary<Type, List<Type>> derivedTypesByAbstractType = new Dictionary<Type, List<Type>>();         
        private Dictionary<Type, int> derivedTypeIndicesByAbstractType = new Dictionary<Type, int>();         

        public bool TryBuild(string logicalPropertyPath, Type targetType, string context, out object value)
        {
            if (targetType.IsClass && targetType.IsAbstract)
            {
                List<Type> derivedTypes;
                
                if (!derivedTypesByAbstractType.ContainsKey(targetType))
                {
                    derivedTypes = (from t in Factory.ModelTypes
                                    where targetType.IsAssignableFrom(t) && targetType != t
                                    select t).ToList();

                    derivedTypesByAbstractType[targetType] = derivedTypes;

                    if (derivedTypesByAbstractType[targetType].Count == 0)
                        throw new Exception(string.Format("No derived types found for abstract type '{0}'",
                                                          targetType.FullName));

                    derivedTypeIndicesByAbstractType[targetType] = 0;
                }
                else
                {
                    derivedTypes = derivedTypesByAbstractType[targetType];
                }

                int index = derivedTypeIndicesByAbstractType[targetType];

                value = Factory.Create(logicalPropertyPath, derivedTypes[index]);

                index = (index + 1) % derivedTypes.Count;
                derivedTypeIndicesByAbstractType[targetType] = index;

                return true;
            }

            value = null;
            return false;
        }

        public void Reset()
        {
            derivedTypeIndicesByAbstractType = new Dictionary<Type, int>();
            derivedTypesByAbstractType = new Dictionary<Type, List<Type>>();
        }

        public TestObjectFactory Factory { get; set; }
    }
}
