// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace EdFi.Dashboards.Common
{
    public delegate object LateBoundPropertyGet(object target);
    public delegate void LateBoundPropertySet(object target, object value);
    public delegate object LateBoundFieldGet(object target);
    public delegate void LateBoundFieldSet(object target, object value);
    public delegate object LateBoundMethod(object target, object[] arguments);
    public delegate void LateBoundVoidMethod(object target, object[] arguments);

    public static class TypeExtensions
    {
        /// <summary>
        /// Uses the provided controller type to strip off the "Controller" suffix, based on MVC conventions (used to prevent "magic strings" from being used for controllers in method calls).
        /// </summary>
        /// <param name="controllerType">The controller type from which to extract the controller name.</param>
        /// <returns>The controller name.</returns>
        public static string GetControllerName(this Type controllerType)
        {
            if (controllerType.Name.EndsWith("Controller"))
                return controllerType.Name.Replace("Controller", String.Empty);

            return controllerType.Name;
        }

        /// <summary>
        /// Uses the provided controller type to strip off the "Controller" suffix, based on MVC conventions (used to prevent "magic strings" from being used for controllers in method calls).
        /// </summary>
        /// <param name="serviceType">The service type from which to extract the controller name.</param>
        /// <returns>The controller name.</returns>
        public static string GetResourceName(this Type serviceType)
        {
            if (serviceType.Name.EndsWith("Service"))
                return serviceType.Name.Replace("Service", String.Empty);

            return serviceType.Name;
        }

        /// <summary>
        /// Gets the default value for the type specified.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> for which the default value should be obtained.</param>
        /// <returns>The default value.</returns>
        public static object GetDefault(this Type type)
        {
            if (type.IsValueType)
                return Activator.CreateInstance(type);

            return null;
        }

        /// <summary>
        /// Gets generic type information for an enumerable type, or <b>null</b> if the type is not enumerable.
        /// </summary>
        /// <param name="type">The type to be inspected.</param>
        /// <returns>An instance of <see cref="EnumerableInfo"/>, containing the enumerable type and item type.</returns>
        public static EnumerableInfo GetEnumerableInfo(this Type type)
        {
            bool isGenericEnumerable = (type.IsGenericType && typeof(IEnumerable).IsAssignableFrom(type));

            if (!isGenericEnumerable)
                return null;

            // Gets the item type...
            var allEnumerableItemTypes = type.GetGenericArguments();

            // Gets the type of the IEnumerable ...
            var enumerableType = type.GetGenericTypeDefinition();

            return new EnumerableInfo
                       {
                           EnumerableType = enumerableType,
                           EnumerableItemType = allEnumerableItemTypes.First(),
                           AllEnumerableItemTypes = allEnumerableItemTypes
                       };
        }

        /// <summary>
        /// Holds Type information related to an enumerable type.
        /// </summary>
        public class EnumerableInfo
        {
            /// <summary>
            /// Gets or sets the <see cref="Type"/> of the IEnumerable.
            /// </summary>
            public Type EnumerableType { get; set; }

            /// <summary>
            /// Gets or sets the <see cref="Type"/> of the items in the enumerable.
            /// </summary>
            public Type EnumerableItemType { get; set; }
            public Type[] AllEnumerableItemTypes { get; set; }
        }


        public static bool HasInterface(this Type type, Type interfaceType)
        {
            return type.GetInterfaces().Any(t => t == interfaceType);
        }

        public static bool IsComplex(this Type type)
        {
            var elementType = type.GetElementType();
            return !type.IsValueType &&
                !(elementType != null && elementType.IsValueType)
                && type != typeof(string);
        }

        public static bool IsDictionaryType(this Type type)
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(System.Collections.Generic.IDictionary<,>))
                return true;

            var genericInterfaces = type.GetInterfaces().Where(t => t.IsGenericType);
            var baseDefinitions = genericInterfaces.Select(t => t.GetGenericTypeDefinition());
            return baseDefinitions.Any(t => t == typeof(System.Collections.Generic.IDictionary<,>));
        }

        public static bool IsGenericOf(this Type type, Type genericInterface)
        {
            return type.GetInterfaces()
                .Any(t => t.IsGenericType && t.GetGenericTypeDefinition() == genericInterface);
        }

        public static LateBoundPropertyGet ValueGetter(this PropertyInfo propertyInfo)
        {
            var instanceParameter = Expression.Parameter(typeof(object), "target");

            var member = Expression.Property(Expression.Convert(instanceParameter, propertyInfo.DeclaringType), propertyInfo);

            var lambda = Expression.Lambda<LateBoundPropertyGet>(
                Expression.Convert(member, typeof(object)),
                instanceParameter
                );

            return lambda.Compile();
        }

        public static LateBoundPropertySet ValueSetter(this PropertyInfo propertyInfo)
        {
            var instanceParameter = Expression.Parameter(typeof(object), "target");
            var valueParameter = Expression.Parameter(typeof(object), "value");

            var assignmentValueExpression = GetAssignmentValueExpression(propertyInfo.PropertyType, valueParameter);

            var member = Expression.Property(Expression.Convert(instanceParameter, propertyInfo.DeclaringType), propertyInfo);
            var assignExpression = Expression.Assign(member, assignmentValueExpression);

            var lambda = Expression.Lambda<LateBoundPropertySet>(
                assignExpression,
                instanceParameter,
                valueParameter
                );

            return lambda.Compile();
        }

        public static LateBoundFieldGet ValueGetter(this FieldInfo field)
        {
            var instanceParameter = Expression.Parameter(typeof(object), "target");

            var member = Expression.Field(Expression.Convert(instanceParameter, field.DeclaringType), field);

            var lambda = Expression.Lambda<LateBoundFieldGet>(
                Expression.Convert(member, typeof(object)),
                instanceParameter
                );

            return lambda.Compile();
        }

        public static LateBoundFieldSet ValueSetter(this FieldInfo fieldInfo)
        {
            var instanceParameter = Expression.Parameter(typeof(object), "target");
            var valueParameter = Expression.Parameter(typeof(object), "value");

            var assignmentValueExpression = GetAssignmentValueExpression(fieldInfo.FieldType, valueParameter);

            var member = Expression.Field(Expression.Convert(instanceParameter, fieldInfo.DeclaringType), fieldInfo);
            var assignExpression = Expression.Assign(member, assignmentValueExpression);

            var lambda = Expression.Lambda<LateBoundFieldSet>(
                assignExpression,
                instanceParameter,
                valueParameter
                );

            return lambda.Compile();
        }

        private static Expression GetAssignmentValueExpression(Type memberType, ParameterExpression valueParameter)
        {
            Expression assignmentExpression;

            // Do we need to add special handling to convert this back to an IQueryable?
            if (memberType.IsGenericType
                && memberType.GetGenericTypeDefinition() == typeof(IQueryable<>))
            {
                // Get the T in IQueryable<T>
                Type genericType = memberType.GetGenericArguments()[0];

                // Create an assignment expression that calls AsQueryable to match the target type
                var castedValueParameter = Expression.Convert(valueParameter, typeof(IEnumerable<>).MakeGenericType(genericType));
                assignmentExpression = Expression.Call(GetAsQueryableMethod(genericType), castedValueParameter);
            }
            else
            {
                // Perform a standard cast on the object value
                assignmentExpression = Expression.Convert(valueParameter, memberType);
            }

            return assignmentExpression;
        }

        private static Dictionary<Type, MethodInfo> asQueryableMethodsByItemType = new Dictionary<Type, MethodInfo>();

        private static MethodInfo openGenericAsQueryableMethod = null;

        private static MethodInfo GetAsQueryableMethod(Type itemType)
        {
            MethodInfo methodInfo;

            if (!asQueryableMethodsByItemType.TryGetValue(itemType, out methodInfo))
            {
                // Have we found the generic method yet?
                if (openGenericAsQueryableMethod == null)
                {
                    openGenericAsQueryableMethod =
                        (from m in typeof(Queryable).GetMethods()
                         where m.Name == "AsQueryable" && m.IsGenericMethod
                         select m)
                            .Single();
                }

                // Make the requested closed generic type
                methodInfo = openGenericAsQueryableMethod.MakeGenericMethod(itemType);

                asQueryableMethodsByItemType[itemType] = methodInfo;
            }

            return methodInfo;
        }

        public static TDelegate InvokeDelegate<TDelegate>(this MethodInfo method)
        {
            ParameterExpression instanceParameter = Expression.Parameter(typeof(object), "target");
            var argumentsParameter = Expression.Parameter(typeof(object[]), "arguments");

            MethodCallExpression call;
            if (!method.IsDefined(typeof(ExtensionAttribute), false))
            {
                // instance member method
                call = Expression.Call(
                    Expression.Convert(instanceParameter, method.DeclaringType),
                    method,
                    CreateParameterExpressions(method, instanceParameter, argumentsParameter));
            }
            else
            {
                // static extension method
                call = Expression.Call(
                    method,
                    CreateParameterExpressions(method, instanceParameter, argumentsParameter));
            }

            Expression<TDelegate> lambda;

            if ((typeof(TDelegate).GetMethod("Invoke").ReturnType == typeof(void)))
                lambda = Expression.Lambda<TDelegate>(
                    call,
                    instanceParameter,
                    argumentsParameter);
            else
            {
                lambda = Expression.Lambda<TDelegate>(
                   Expression.Convert(call, typeof(object)),
                   instanceParameter,
                   argumentsParameter);
            }

            return lambda.Compile();
        }

        private static Expression[] CreateParameterExpressions(MethodInfo method, Expression instanceParameter, Expression argumentsParameter)
        {
            var expressions = new List<UnaryExpression>();
            var realMethodParameters = method.GetParameters();
            if (method.IsDefined(typeof(ExtensionAttribute), false))
            {
                var extendedType = method.GetParameters()[0].ParameterType;
                expressions.Add(Expression.Convert(instanceParameter, extendedType));
                realMethodParameters = Enumerable.ToArray(realMethodParameters.Skip(1));
            }

            expressions.AddRange(realMethodParameters.Select((parameter, index) =>
                Expression.Convert(
                    Expression.ArrayIndex(argumentsParameter, Expression.Constant(index)),
                    parameter.ParameterType)));

            return expressions.ToArray();
        }

        public static bool IsSet(this Type type)
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(ISet<>))
            {
                return true;
            }

            var genericInterfaces = type.GetInterfaces().Where(t => t.IsGenericType);
            var baseDefinitions = genericInterfaces.Select(t => t.GetGenericTypeDefinition());

            return baseDefinitions.Any(t => t == typeof(ISet<>));
        }

        public static bool IsDeferredLinqQuery(this Type type)
        {
            //Assuming all classes that implement IEnumerable<> and reside in namespace System.Linq are linq internal classes
            //designed for deferred query execution - do not know if there is a clean way to detect that.
            return type.IsGenericOf(typeof (IEnumerable<>)) &&
                   !string.IsNullOrEmpty(type.Namespace) &&
                   type.Namespace.StartsWith("System.Linq");
        }
    }
}
