using System;

namespace EdFi.Dashboards.Infrastructure
{
    /// <summary>
    /// Defines methods for serializing, deserializing and cloning objects.
    /// </summary>
    public interface ISerializer
    {
        /// <summary>
        /// Serializes an object to a byte array.
        /// </summary>
        /// <typeparam name="T">The type of the object to be serialized.</typeparam>
        /// <param name="source">The object to be serialized.</param>
        /// <returns>The byte array representing the object's state.</returns>
        byte[] Serialize<T>(T source);

        /// <summary>
        /// Deserializes an object from a byte array.
        /// </summary>
        /// <typeparam name="T">The type of the object to be deserialized.</typeparam>
        /// <param name="data">The byte array containing the previously serialized object state.</param>
        /// <returns>The deserialized object.</returns>
        T Deserialize<T>(byte[] data);

        /// <summary>
        /// Deserializes an object from a byte array.
        /// </summary>
        /// <param name="data">The byte array containing the previously serialized object state.</param>
        /// <param name="type">The type of the object to be deserialized.</param>
        /// <returns>The deserialized object.</returns>
        object Deserialize(byte[] data, Type type);

        /// <summary>
        /// Creates a new object that is a deep-copy clone of the provided object or graph.
        /// </summary>
        /// <typeparam name="T">The type of the object to be copied.</typeparam>
        /// <param name="source">The source object.</param>
        /// <returns>The cloned object or graph.</returns>
        T DeepClone<T>(T source);
    }
}
