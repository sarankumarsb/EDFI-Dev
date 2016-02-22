using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace EdFi.Dashboards.Infrastructure.Implementations
{
    /// <summary>
    /// Provides an <see cref="ISerializer"/> implementation that uses 
    /// the native .NET <see cref="BinaryFormatter"/> serializer.
    /// </summary>
    public class BinaryFormatterSerializer : ISerializer
    {
        /// <summary>
        /// Serializes an object to a byte array.
        /// </summary>
        /// <typeparam name="T">The type of the object to be serialized.</typeparam>
        /// <param name="source">The object to be serialized.</param>
        /// <returns>The byte array representing the object's state.</returns>
        public byte[] Serialize<T>(T source)
        {
            IFormatter formatter = new BinaryFormatter();
            var stream = new MemoryStream();
            using (stream)
            {
                formatter.Serialize(stream, source);
                return stream.ToArray();
            }
        }

        /// <summary>
        /// Deserializes an object from a byte array.
        /// </summary>
        /// <typeparam name="T">The type of the object to be deserialized.</typeparam>
        /// <param name="data">The byte array containing the previously serialized object state.</param>
        /// <returns>The deserialized object.</returns>
        public T Deserialize<T>(byte[] data)
        {
            IFormatter formatter = new BinaryFormatter();
            var stream = new MemoryStream(data);

            using (stream)
            {
                return (T)formatter.Deserialize(stream);
            }
        }

        /// <summary>
        /// Deserializes an object from a byte array.
        /// </summary>
        /// <param name="data">The byte array containing the previously serialized object state.</param>
        /// <param name="type">Ignored (the <see cref="BinaryFormatter"/> embeds type information into the 
        /// serialization format,  making this parameter redundant).</param>
        /// <returns>The deserialized object.</returns>
        public object Deserialize(byte[] data, Type type)
        {
            IFormatter formatter = new BinaryFormatter();
            var stream = new MemoryStream(data);

            using (stream)
            {
                return formatter.Deserialize(stream);
            }
        }

        /// <summary>
        /// Creates a new object that is a deep-copy clone of the provided object or graph.
        /// </summary>
        /// <typeparam name="T">The type of the object to be copied.</typeparam>
        /// <param name="source">The source object.</param>
        /// <returns>The cloned object or graph.</returns>
        public T DeepClone<T>(T source)
        {
            // Don't serialize a null object, simply return the default for that object
            if (Object.ReferenceEquals(source, null))
            {
                return default(T);
            }

            dynamic dynamicSource = source;

            if (!dynamicSource.GetType().IsSerializable)
            {
                throw new ArgumentException("The type must be serializable.", "source");
            }

            IFormatter formatter = new BinaryFormatter();
            Stream stream = new MemoryStream();
            using (stream)
            {
                formatter.Serialize(stream, source);
                stream.Seek(0, SeekOrigin.Begin);
                return (T)formatter.Deserialize(stream);
            }
        }
    }
}
