using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using ServiceStack.Text;

namespace EdFi.Dashboards.Infrastructure.Implementations
{
    /// <summary>
    /// Provides an <see cref="ISerializer"/> implementation that uses the ServiceStack project's JSV serializer 
    /// (<see cref="http://www.servicestack.net/"/>), but augments it with .NET type information at the head of 
    /// the stream to enable correct type instantiation during subsequent deserialization.
    /// </summary>
    public class ServiceStackTypeSerializer : ISerializer
    {
        /// <summary>
        /// Serializes an object to a byte array.
        /// </summary>
        /// <typeparam name="T">The type of the object to be serialized.</typeparam>
        /// <param name="source">The object to be serialized.</param>
        /// <returns>The byte array representing the object's state.</returns>
        public byte[] Serialize<T>(T source)
        {
            // Due to ServiceStack design decision, we need to handle outermost type serialization ourselves
            byte[] headerData = CreateHeaderData(source);
            byte[] serializedData = GetSerializedData(source);

            byte[] allData = new byte[headerData.Length + serializedData.Length];
            
            // Assemble single byte array containing type name information, followed by serialized object data
            headerData.CopyTo(allData, 0);
            serializedData.CopyTo(allData, headerData.Length);

            return allData;
        }

        private static byte[] GetSerializedData<T>(T source)
        {
            using (var stream = new MemoryStream())
            {
                try
                {
                    TypeSerializer.SerializeToStream(source, stream);
                    byte[] data = stream.ToArray();

                    return data;
                }
                catch (Exception ex)
                {
                    throw new SerializationException(
                        string.Format("Unable to serialize type '{0}'.",
                                      typeof(T).FullName), ex);
                }
            }
        }

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times", 
			Justification= @"Nested usings, such as the one below, will cause the the Dispose method of the MemoryStream to be called when the 
							 Dispose method of the BinaryWriter is called when the code exits out of the using block, however IDisposable ensures that this behavior is safe.

							 http://msdn.microsoft.com/en-us/library/system.idisposable.dispose.aspx
							 If an object's Dispose method is called more than once, the object must ignore all calls after the first one. 
							 The object must not throw an exception if its Dispose method is called multiple times.

							 When you use an object that accesses unmanaged resources, such as a StreamWriter, a good practice is to create the instance with a using statement. 
							 The using statement automatically closes the stream and calls Dispose on the object when the code that is using it has completed.")]
		private static byte[] CreateHeaderData<T>(T source)
        {
            using (var headerStream = new MemoryStream())
            {
                using (var sw = new StreamWriter(headerStream))
                {
                    string typeName = source.GetType().AssemblyQualifiedName;

                    headerStream.Write(BitConverter.GetBytes(typeName.Length), 0, 4);
                    sw.Write(typeName);
                }

                byte[] headerData = headerStream.ToArray();

                return headerData;
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
            return (T) Deserialize(data, typeof(T));
        }

        /// <summary>
        /// Deserializes an object from a byte array.
        /// </summary>
        /// <param name="data">The byte array containing the previously serialized object state.</param>
        /// <param name="type">The type of the object to be deserialized.</param>
        /// <returns>The deserialized object.</returns>
        public object Deserialize(byte[] data, Type type)
        {
            try
            {
                Type serializedType;

                var headerDataLength = ReadTypeNameFromBufferHeader(data, out serializedType);

                using (var dataStream = new MemoryStream(data, headerDataLength, data.Length - headerDataLength))
                {
                    var model = TypeSerializer.DeserializeFromStream(serializedType, dataStream);

                    if (model != null)
                        Serialization.InvokeOnDeserializedMethod(model.GetType(), model);

                    return model;
                }
            }
            catch (Exception ex)
            {
                throw new SerializationException(
                    string.Format("Unable to deserialize type '{0}'.",
                        type.FullName), ex);
            }
        }

        private static Dictionary<string, Type> typesByTypeName = new Dictionary<string, Type>(); 

        /// <summary>
        /// Gets the target type (as an <b>out</b> parameter) and returns the offset into the 
        /// buffer where to begin data serialization.
        /// </summary>
        /// <param name="data">The serialized data of the object.</param>
        /// <param name="serializedType">The .NET type that was serialized into the data.</param>
        /// <returns>The offset into the data buffer where the ServiceStack content starts.</returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times", Justification = "See earlier justification.")]
		private static int ReadTypeNameFromBufferHeader(byte[] data, out Type serializedType)
        {
            using (var stream = new MemoryStream(data))
            {
                using (var sr = new StreamReader(stream))
                {
                    // Get the length of the type name
                    byte[] intBuffer = new byte[sizeof(int)];
                    stream.Read(intBuffer, 0, sizeof(int));
                    int typeNameLength = BitConverter.ToInt32(intBuffer, 0);

                    // Read the type name out of the header
                    char[] typeNameChars = new char[typeNameLength];
                    sr.Read(typeNameChars, 0, typeNameLength);
                    string typeName = new string(typeNameChars);

                    if (!typesByTypeName.TryGetValue(typeName, out serializedType))
                    {
                        // Get the .NET type
                        serializedType = Type.GetType(typeName);
                    }

                    // Return the offset into the data array to begin deserializing the object
                    int headerDataLength = sizeof(int) + typeNameLength;
                    
                    return headerDataLength;
                }
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

            var model = TypeSerializer.Clone(source);

            if (model != null)
                Serialization.InvokeOnDeserializedMethod(model.GetType(), model);

            return model;
        }
    }
}
