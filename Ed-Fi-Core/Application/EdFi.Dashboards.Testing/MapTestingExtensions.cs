// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace EdFi.Dashboards.Testing
{
	#region Commented out code

	//public class MapVerificationResult
	//{
	//    public MappingState MappingStatus { get; set; }
	//    public string Message { get; set; }
	//}

	//public enum MappingState
	//{
	//    /// <summary>
	//    /// The presence of a mapping could not be confirmed.
	//    /// </summary>
	//    NotMapped,
	//    /// <summary>
	//    /// The destination was mapped, and the value is correct.
	//    /// </summary>
	//    MappedAndCorrect,
	//    /// <summary>
	//    /// The destination was mapped, but the value was incorrect.
	//    /// </summary>
	//    MappedAndIncorrect,
	//}
	#endregion

    class ValidationState
    {
        public ValidationState()
        {
            UnmappedProperties = new List<string>();
            MappingErrorsByLogicalPath = new Dictionary<string, string>();
            UnevaluatedProperties = new List<string>();
        }

        public List<string> UnmappedProperties { get; set; }
        public Dictionary<string, string> MappingErrorsByLogicalPath { get; set; }
        public List<string> UnevaluatedProperties { get; set; }
    }

	public static class MapTestingExtensions
	{
	    public static void Should_have_same_values_as(this object instance, object otherInstance, params string[] pathsToIgnore)
	    {
            var visitedProperties = new List<int>();
	        var validationState = new ValidationState();

            ProcessValue(instance.GetType(), instance, otherInstance, instance.GetType().Name, AreValuesEqual, validationState, visitedProperties, null);

            ReportExceptions(pathsToIgnore, validationState);
	    }

        //public static void ToTabularText(this object instance, params string[] pathsToIgnore)
        //{
        //    var visitedProperties = new List<int>();
        //    var validationState = new ValidationState();

        //    ProcessValue(instance.GetType(), instance, null, instance.GetType().Name, DisplayValue, validationState, visitedProperties, null);
        //}

        //private static bool DisplayValue(object modelValue, object ignore, string logicalPropertyPath, ValidationState validationState)
        //{
            
        //}

        public static void EnsureSerializableModel<T>(this T instance)
        {
            if (typeof(T).IsClass && instance == null)
                return;

            IFormatter formatter = new BinaryFormatter();
            Stream stream = new MemoryStream();
            formatter.Serialize(stream, instance);

            stream.Position = 0;
            var copy = (T)formatter.Deserialize(stream);
            instance.Should_have_same_values_as(copy);
        }

        public static void EnsureNoDefaultValues(this object instance, params string[] pathsToIgnore)
	    {
            var visitedProperties = new List<int>();
            var validationState = new ValidationState();

            ProcessValue(instance.GetType(), instance, null, instance.GetType().Name, IsMappedBasedOnDefaultValueEvaluation, validationState, visitedProperties, null);

            //ValidateMappings(instance, null, IsMappedBasedOnDefaultValueEvaluation,
            //    instance.GetType().Name, validationState, visitedProperties);

            ReportExceptions(pathsToIgnore, validationState);
        }

	    private static void ReportExceptions(string[] pathsToIgnore, ValidationState validationState)
	    {
	        var sb = new StringBuilder();

	        // Filter mapping errors down to just the actual mapping errors
	        var actualPropertyMappingErrors =
	            from message in validationState.UnmappedProperties
	            where !pathsToIgnore.Contains(message)
	            select message;

	        // Fail the test if any properties were found to be unmapped
	        if (actualPropertyMappingErrors.Any())
	            sb.Append("The following properties were not mapped:\n\t"
	                      + string.Join("\n\t", actualPropertyMappingErrors.ToArray())
	                      + "\n");

	        // Filter mapping errors down to just the actual mapping errors
	        var actualUnevaluatedPropertyErrors =
	            from message in validationState.UnevaluatedProperties
	            where !pathsToIgnore.Contains(message)
	            select message;

	        // Fail the test if any properties were found to be unmapped
	        if (actualUnevaluatedPropertyErrors.Any())
	            sb.Append("The following properties were not evaluated (compare to model did not have a matching property):\n\t"
	                      + string.Join("\n\t", actualUnevaluatedPropertyErrors.ToArray())
	                      + "\n");

	        var actualPropertyValueErrors =
	            from message in validationState.MappingErrorsByLogicalPath
	            where !pathsToIgnore.Contains(message.Key)
	            select message;

	        if (actualPropertyValueErrors.Any())
	        {
	            sb.Append("\nThe following properties were mapped with incorrect values:\n");

	            foreach (var message in actualPropertyValueErrors)
	                sb.AppendFormat("\t{0}: {1}\n", message.Key, message.Value);
	        }

	        if (sb.Length > 0)
	            throw new Exception(sb.ToString());
	    }

        private static bool AreValuesEqual(object modelValue, object otherModelValue, string logicalPropertyPath, ValidationState validationState, List<int> visitedProperties, PropertyInfo propertyInfo)
        {
            if (modelValue == null)
            {
                bool areEqual = otherModelValue == null;

                if (!areEqual)
                    validationState.MappingErrorsByLogicalPath[logicalPropertyPath] = "Expected '" + otherModelValue + "', but was null.";

                return areEqual;
            }

            if (modelValue == otherModelValue || modelValue.Equals(otherModelValue))
                return true;

            validationState.MappingErrorsByLogicalPath[logicalPropertyPath] = string.Format("Expected '{0}', but was '{1}'.", otherModelValue, modelValue);
            return false;
        }

		private static bool IsMappedBasedOnDefaultValueEvaluation(object modelValue, object ignored, string logicalPropertyPath, ValidationState validationState, List<int> visitedProperties, PropertyInfo propertyInfo)
		{
            if (modelValue == null)
            {
                validationState.UnmappedProperties.Add(logicalPropertyPath);
                return false;
            }

			object defaultValue = modelValue.GetType().GetDefault();

            if (modelValue == defaultValue || modelValue.Equals(defaultValue))
            {
                validationState.UnmappedProperties.Add(logicalPropertyPath);
                return false;
            }

			return true;
		}

		private static bool IsCustomClass(Type type)
		{
			return (type != typeof(string)
			       	&&
			       	(type.IsEnumerable() ||
			       		(type.IsClass && !type.Namespace.StartsWith("System."))));
		}

		private static void ValidateMappings(object modelInstance, object otherModelInstance,
            Func<object, object, string, ValidationState, List<int>, PropertyInfo, bool> isValueValid, 
			string logicalPath, ValidationState validationState, 
            List<int> visitedProperties)
		{
			var properties = modelInstance.GetType().GetProperties();

			foreach (var propertyInfo in properties)
			{
				string propertyLogicalPath = logicalPath + "." + propertyInfo.Name;

                // We can't support indexed properties.  Skip them.
                if (propertyInfo.GetIndexParameters().Length > 0)
                    continue;

				object modelPropertyValue = propertyInfo.GetValue(modelInstance, null);
			    object otherModelPropertyValue = null;
			    
                if (otherModelInstance != null)
                {
                    PropertyInfo propertyInfoOther;
                    try
                    {
                        propertyInfoOther = otherModelInstance.GetType().GetProperty(propertyInfo.Name);
                    }
                    catch (Exception ex)
                    {
                        throw new ApplicationException("Failed to get the property " + propertyInfo.Name, ex);
                    }

                    if (propertyInfoOther == null)
                    {
                        // Property not compared, no corresponding value between models
                        validationState.UnevaluatedProperties.Add(propertyLogicalPath);
                        continue;
                    }

                    otherModelPropertyValue = propertyInfoOther.GetValue(otherModelInstance, null);
                }

				ProcessValue(propertyInfo.PropertyType, modelPropertyValue, otherModelPropertyValue, propertyLogicalPath, isValueValid, validationState, visitedProperties, propertyInfo);
			}
		}

	    private static void ProcessValue(Type modelValueType, object modelPropertyValue, object otherModelPropertyValue, string propertyLogicalPath, 
            Func<object, object, string, ValidationState, List<int>, PropertyInfo, bool> isValueValid, ValidationState validationState, List<int> visitedProperties, 
            PropertyInfo propertyInfo)
	    {
            if (!IsCustomClass(modelValueType)
	            && !isValueValid(modelPropertyValue, otherModelPropertyValue, propertyLogicalPath, validationState, visitedProperties, propertyInfo))
	        {
	            return;
	        }

            if (IsCustomClass(modelValueType))
	        {
                if (modelValueType.IsEnumerable())
	            {
	                int propertyInfoHashCode = 0;

                    if (propertyInfo != null)
                        propertyInfoHashCode = propertyInfo.GetHashCode();
                    else
                        propertyInfoHashCode = int.MaxValue;

	                if (!visitedProperties.Contains(propertyInfoHashCode))
	                {
                        if (propertyInfoHashCode != int.MaxValue)
	                        visitedProperties.Add(propertyInfoHashCode);

	                    ProcessEnumerableMember(modelPropertyValue, otherModelPropertyValue, propertyLogicalPath,
	                                            isValueValid, validationState, visitedProperties, propertyInfo);
	                }

	                return;
	            }

	            // Skip properties that return system types
                if (modelValueType.FullName.StartsWith("System."))
	                return;

	            if (modelPropertyValue == null)
	            {
	                isValueValid(null, otherModelPropertyValue, propertyLogicalPath, validationState, visitedProperties, propertyInfo);
	                return;
	            }

	            ValidateMappings(modelPropertyValue, otherModelPropertyValue, isValueValid, // validateMappedValue, 
	                             propertyLogicalPath, validationState, visitedProperties);
	        }
	        else
	        {
	            isValueValid(modelPropertyValue, otherModelPropertyValue, propertyLogicalPath, validationState, visitedProperties, propertyInfo);
	        }
	    }

	    private static void ProcessEnumerableMember(object actualValue, object otherValue, string logicalPath, 
            Func<object, object, string, ValidationState, List<int>, PropertyInfo, bool> isValueValid, ValidationState validationState, 
            List<int> visitedProperties, PropertyInfo propertyInfo)
		{
			var items = (IEnumerable) actualValue;
		    var otherItems = otherValue as IEnumerable;

            ArrayList otherItemsList = null;

		    if (otherItems != null)
            {
                otherItemsList = new ArrayList();

                foreach (var item in otherItems)
                    otherItemsList.Add(item);
            }

			int i = 0;

            if (items != null)
            {
                foreach (object item in items)
                {
                    object otherItem = null;

                    if (otherItemsList != null && i < otherItemsList.Count)
                        otherItem = otherItemsList[i];

                    string collectionMemberLogicalPath = logicalPath + "[" + i + "]";

                    if (item.GetType().IsClass && item.GetType() != typeof(string))
                    {
                        // && !item.GetType().FullName.StartsWith("System.")
                        ValidateMappings(item, otherItem, isValueValid, // validateMappedValue, 
                            collectionMemberLogicalPath, validationState, visitedProperties);
                    }
                    else
                    {
                        if (!isValueValid(item, otherItem, collectionMemberLogicalPath, validationState, visitedProperties, propertyInfo))
                        {
                            i++;
                            continue;
                        }
                    }

                    i++;
                }
            }

			// If the list was empty, add a failure message
			if (items == null)
			{
                isValueValid(null, otherItemsList, logicalPath, validationState, visitedProperties, propertyInfo);
			}
            else if (i == 0)
            {
                if (otherItemsList != null)
                {
                    if (otherItemsList.Count != 0)
                    {
                        validationState.MappingErrorsByLogicalPath[logicalPath] =
                            string.Format("Expected an empty list, but actual list had '{0}' items.",
                                          otherItemsList.Count);
                    }
                }
            }
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
		/// Indicates whether the type implements <see cref="IEnumerable"/>.
		/// </summary>
		/// <param name="type">The <see cref="Type"/> to be inspected.</param>
		/// <returns><b>true</b> if the type is enumerable; otherwise <b>false</b>.</returns>
		public static bool IsEnumerable(this Type type)
		{
			return typeof(IEnumerable).IsAssignableFrom(type);
		}
	}
}