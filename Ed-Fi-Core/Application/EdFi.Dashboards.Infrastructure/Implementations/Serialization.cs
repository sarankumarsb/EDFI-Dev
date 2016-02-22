using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;

namespace EdFi.Dashboards.Infrastructure.Implementations
{
    /// <summary>
    /// Provides methods for augmenting data serializer functionality when it doesn't support
    /// the OnDeserialized behavior for post-deserialization processing.
    /// </summary>
    public class Serialization
    {
        private const string DeserializedEntireGraphValue = "entire-graph";

        /// <summary>
        /// Holds the "magic value" indicating that the data serializer being used does
        /// not call the OnDeserialized method on each class after deserialization.  
        /// When this value is present on the StreamingContext object's additional data,
        /// it indicates that the <see cref="ISerializer"/> implementation for the serializer 
        /// is compensating for this loss of functionality by calling the method on the top 
        /// level model class (if it exists), and is passing this constant value to indicate 
        /// the implementor should take action on the entire object graph, if necessary 
        /// (usually this involves such activities as reattaching  "parent" references in 
        /// hierarchies).
        /// </summary>
        public static bool IsDeserializingEntireGraph(StreamingContext context)
        {
            if (context.Context != null && context.Context.ToString() == DeserializedEntireGraphValue)
                return true;

            return false;
        }

        /// <summary>
        /// Gets the predefined <see cref="StreamingContext"/> containing additional context
        /// data to be passed to <see cref="IsDeserializingEntireGraph"/> by the OnDeserialized
        /// method implementation on the model being deserialized to compensate for the
        /// underlying data serializer's lack of support for the behavior.  See the 
        /// <see cref="IsDeserializingEntireGraph"/> method for more details.
        /// </summary>
        public static readonly StreamingContext DeserializedEntireGraphStreamContext 
            = new StreamingContext(StreamingContextStates.All, DeserializedEntireGraphValue);

        private static Dictionary<Type, MethodInfo> onDeserializedMethodsByType = new Dictionary<Type, MethodInfo>();

        /// <summary>
        /// Attempts to invoke the OnDeserialized method on the target model.
        /// </summary>
        /// <param name="type">The type of model to search for the OnDeserialized method.</param>
        /// <param name="model">The instance of the model on which the method should be invoked.</param>
        public static void InvokeOnDeserializedMethod(Type type, object model)
        {
            MethodInfo method;

            // Look for OnDeserialized method only once per type deserialized
            if (!onDeserializedMethodsByType.TryGetValue(type, out method))
            {
                method = type.GetMethod("OnDeserialized", BindingFlags.NonPublic | BindingFlags.Instance);
                onDeserializedMethodsByType[type] = method;
            }

            // Invoke OnDeserialized method, if it exists
            if (model != null && method != null)
                method.Invoke(model, new object[] { DeserializedEntireGraphStreamContext });
        }
    }
}
