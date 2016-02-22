using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EdFi.Dashboards.Resources.Tests.Serialization.Builders
{
    public class OpenGenericTypeBuilder : IValueBuilder
    {
        public bool TryBuild(string logicalPropertyPath, Type targetType, string context, out object value)
        {
            if (targetType.IsGenericTypeDefinition)
            {
                var typeArgs = targetType.GetGenericArguments();

                var concreteTypes = new List<Type>();

                foreach (var typeArg in typeArgs)
                {
                    var typeConstraint = typeArg.GetGenericParameterConstraints().FirstOrDefault();
                    
                    if (typeConstraint == null)
                    {
                        // Just use an integer... it's simple.
                        concreteTypes.Add(typeof(int));
                    }
                    else if (typeConstraint.IsAbstract)
                    {
                        // Find derived types
                        throw new Exception(string.Format("Generic constraint on type '{0}' uses type '{1}', which is abstract and not currently supported.", 
                            targetType.FullName, typeConstraint.FullName));
                    }
                    else
                    {
                        concreteTypes.Add(typeConstraint);
                    }
                }

                var closedGenericType = targetType.MakeGenericType(concreteTypes.ToArray());

                value = Factory.Create(logicalPropertyPath, closedGenericType);
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
