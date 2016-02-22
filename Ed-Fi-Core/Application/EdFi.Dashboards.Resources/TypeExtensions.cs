using System;
using System.Collections.Generic;
using EdFi.Dashboards.Common;
using EdFi.Dashboards.Resource.Models.Common;
using HalResourceModelBase = EdFi.Dashboards.Resource.Models.Common.Hal.ResourceModelBase;

namespace EdFi.Dashboards.Resources
{
    public static class TypeExtensions
    {
        private static Dictionary<Type, Type> baseEntityTypesByType = new Dictionary<Type, Type>();                                             

        /// <summary>
        /// Gets the base type for the given entity type for use in registering/locating services in the IoC container 
        /// (to support the primary extensibility mechanism for customizing repositories).
        /// </summary>
        /// <param name="entityType">The entity type to process to determine the base type to be used for IoC registration.</param>
        /// <returns>The base entity type used for IoC registration.</returns>
        public static Type GetBaseEntityTypeForServiceRegistration(this Type entityType)
        {
            Type returnType;

            if (baseEntityTypesByType.TryGetValue(entityType, out returnType))
                return returnType;

            returnType = entityType;

            while (returnType.BaseType != typeof(object) && returnType.BaseType != null)
                returnType = returnType.BaseType;

            baseEntityTypesByType[entityType] = returnType;

            return returnType;
        }

        private static Dictionary<Type, Type> baseModelTypesByType = new Dictionary<Type, Type>();                                             

        /// <summary>
        /// Gets the base type for the given model type for use in registering/locating services in the IoC container 
        /// (to support the primary extensibility mechanism for customizing services).
        /// </summary>
        /// <param name="modelType">The model type to process to determine the base type to be used for IoC registration.</param>
        /// <returns>The base model type used for IoC registration.</returns>
        public static Type GetBaseModelTypeForServiceRegistration(this Type modelType)
        {
            Type returnType;

            if (baseModelTypesByType.TryGetValue(modelType, out returnType))
                return returnType;

            returnType = modelType;
            Type genericType = null;

            // Assumption: If it's generic and enumerable it's an IEnumerable<T>, or IList<T> or something similar
            var enumerableInfo = modelType.GetEnumerableInfo();

            if (enumerableInfo != null)
            {
                // Gets the wrapped model type...
                returnType = enumerableInfo.EnumerableItemType;

                // Gets the type of the IEnumerable wrapping the model...
                genericType = enumerableInfo.EnumerableType;
            }

            while (!AlternateBaseTypeSet.Contains(returnType.BaseType))
                returnType = returnType.BaseType;

            if (enumerableInfo != null)
            {
                // Rewrap the return type using the outer generic type
                returnType = genericType.MakeGenericType(returnType);
            }

            baseModelTypesByType[modelType] = returnType;

            return returnType;
        }


        private static Dictionary<Type, Type> baseRequestTypesByType = new Dictionary<Type, Type>();                                             

        /// <summary>
        /// Gets the base type for the given request type for use in registering/locating services in the IoC container 
        /// (to support the primary extensibility mechanism for customizing services).
        /// </summary>
        /// <param name="requestType">The request type to process to determine the base type to be used for IoC registration.</param>
        /// <returns>The base request type used for IoC registration.</returns>
        public static Type GetBaseRequestTypeForServiceRegistration(this Type requestType)
        {
            Type returnType;

            if (baseRequestTypesByType.TryGetValue(requestType, out returnType))
                return returnType;

            string startingNamespace = requestType.Namespace;

            // If we're not in an extensions namespace, don't climb any hierarchies for requests.
            if (!startingNamespace.Contains(".Extensions."))
                return requestType;

            returnType = requestType;

            // We're in an extension namespace, so climb until the namespace no longer is in the extensions namespace
            while (returnType.BaseType != typeof(object) && returnType.BaseType != null)
            {
                returnType = returnType.BaseType;

                // If we're no longer in the extensions namespace, quit climbing.
                if (!returnType.Namespace.Contains(".Extensions."))
                    break;
            }

            baseRequestTypesByType[requestType] = returnType;

            return returnType;
        }

        #region AlternateBaseTypes property (read-only, private)

        private static HashSet<Type> _alternateBaseTypeSet;

        private static HashSet<Type> AlternateBaseTypeSet
        {
            get
            {
                if (_alternateBaseTypeSet == null)
                {
                    _alternateBaseTypeSet =
                        new HashSet<Type>
                            {
                                typeof(Object),
                                typeof(ValueType),
                                typeof(ResourceModelBase),
                                typeof(HalResourceModelBase)
                            };
                }

                return _alternateBaseTypeSet;
            }
        }

        #endregion
    }
}
