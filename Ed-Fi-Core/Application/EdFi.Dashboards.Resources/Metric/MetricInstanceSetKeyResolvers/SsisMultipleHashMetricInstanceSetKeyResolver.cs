// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using EdFi.Dashboards.Common;
using EdFi.Dashboards.Metric.Resources.Providers;
using EdFi.Dashboards.Metric.Resources.Services;

namespace EdFi.Dashboards.Resources.Metric.MetricInstanceSetKeyResolvers
{

    /// <summary>
    /// When computing the MetricInstanceSetKey, convert any longs encounted into ints.  This works for clients where they compute the has with the StudentUSI is an int
    /// </summary>
    /// <typeparam name="TRequest"></typeparam>
    public class SsisMultipleHashMetricInstanceSetKeyResolverConvertLongToInt<TRequest> : SsisMultipleHashMetricInstanceSetKeyResolverBase<TRequest>
        where TRequest : MetricInstanceSetRequestBase
    {
        public override Guid GetMetricInstanceSetKey(TRequest metricInstanceSetRequestBase)
        {
            var requestValues =
                from kvp in metricInstanceSetRequestBase.ToKeyValuePairs(bindingFlags: BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                where kvp.Key != "MetricInstanceSetTypeId"
                //If this fails to cast, that means that the data coming from the source is a long, and this code should should throw an exception
                select kvp.Value is long ? (int)kvp.Value : kvp.Value;

            var argsForHash =
                metricInstanceSetRequestBase.MetricInstanceSetTypeId.ToEnumerable<object>().Concat(requestValues)
                    .ToList();

            var hash = CalculateHash(argsForHash, true);

            return new Guid(hash);
        }
    }


    /// <summary>
    /// When computing the MetricInstanceSetKey, leave any longs as longs.  This works for clients where they compute the has with the StudentUSI is an long
    /// </summary>
    /// <typeparam name="TRequest"></typeparam>
    public class SsisMultipleHashMetricInstanceSetKeyResolverHandleLongNormally<TRequest> : SsisMultipleHashMetricInstanceSetKeyResolverBase<TRequest>
        where TRequest : MetricInstanceSetRequestBase
    {
        public override Guid GetMetricInstanceSetKey(TRequest metricInstanceSetRequestBase)
        {
            var requestValues =
                from kvp in metricInstanceSetRequestBase.ToKeyValuePairs(bindingFlags: BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                where kvp.Key != "MetricInstanceSetTypeId"
                select kvp.Value;

            var argsForHash =
                metricInstanceSetRequestBase.MetricInstanceSetTypeId.ToEnumerable<object>().Concat(requestValues)
                    .ToList();

            var hash = CalculateHash(argsForHash, true);

            return new Guid(hash);
        }
    }

    /// <summary>
    /// Provides MD5 hashing functionality to derived metric instance set key resolvers.
    /// </summary>
    public abstract class SsisMultipleHashMetricInstanceSetKeyResolverBase<TRequest> : IMetricInstanceSetKeyResolver<TRequest>
        where TRequest : MetricInstanceSetRequestBase
    {
        public abstract Guid GetMetricInstanceSetKey(TRequest metricInstanceSetRequestBase);

        protected byte[] CalculateHash(IEnumerable<object> objectsToProcess, bool safeNullHandling)
        {
            var inputByteBuffer = new byte[0];
            string nullHandling = String.Empty;

            // Step through each input column for that output column
            foreach (dynamic t in objectsToProcess)
            {
                // Skip NULL values, as they "don't" exist...
                if (t == null)
                {
                    nullHandling += "Y";
                    continue;
                }

                nullHandling += "N";
                Append(ref inputByteBuffer, t);
            }

            if (safeNullHandling)
            {
                Append(ref inputByteBuffer, nullHandling, Encoding.UTF8);
            }

            // Ok, we have all the data in a Byte Buffer
            // So now generate the Hash
            byte[] hash;
            using (var md5Hash = MD5.Create())
            {
                hash = md5Hash.ComputeHash(inputByteBuffer);
            }
            return hash;
        }

        #region Types to Byte Arrays

        /// <summary>
        /// Converts from bool to a byte array.
        /// </summary>
        /// <param name="value">input value to convert to byte array</param>
        /// <returns>byte array</returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times", 
			Justification= @"Nested usings, such as the one below, will cause the the Dispose method of the MemoryStream to be called when the 
							 Dispose method of the BinaryWriter is called when the code exits out of the using block, however IDisposable ensures that this behavior is safe.

							 http://msdn.microsoft.com/en-us/library/system.idisposable.dispose.aspx
							 If an object's Dispose method is called more than once, the object must ignore all calls after the first one. 
							 The object must not throw an exception if its Dispose method is called multiple times.

							 When you use an object that accesses unmanaged resources, such as a StreamWriter, a good practice is to create the instance with a using statement. 
							 The using statement automatically closes the stream and calls Dispose on the object when the code that is using it has completed.")]
        public byte[] ToArray(bool value)
        {
			byte[] bytes = null;

			using (var stream = new MemoryStream())
			{
				using (var writer = new BinaryWriter(stream))
				{
					writer.Write(value);
					bytes = stream.ToArray();
				}
			}

			return bytes;
        }

        /// <summary>
        /// Converts from decimal to a byte array.
        /// </summary>
        /// <param name="value">input value to convert to byte array</param>
        /// <returns>byte array</returns>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times", Justification = "See earlier justification.")]
	public byte[] ToArray(decimal value)
        {
			byte[] bytes = null;

			using (var stream = new MemoryStream())
			{
				using (var writer = new BinaryWriter(stream))
				{
					writer.Write(value);
					bytes = stream.ToArray();
				}
			}

			return bytes;
        }

        /// <summary>
        /// Converts from DateTimeOffset to a byte array.
        /// </summary>
        /// <param name="value">input value to convert to byte array</param>
        /// <returns>byte array</returns>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times", Justification = "See earlier justification.")]
	private byte[] ToArray(DateTimeOffset value)
        {
			byte[] bytes = null;

			using (var stream = new MemoryStream())
			{
				using (var writer = new BinaryWriter(stream))
				{
					writer.Write(value.ToString("u"));
					bytes = stream.ToArray();
				}
			}

			return bytes;
        }

        /// <summary>
        /// Converts from DateTime to a byte array.
        /// </summary>
        /// <param name="value">input value to convert to byte array</param>
        /// <returns>byte array</returns>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times", Justification = "See earlier justification.")]
	public byte[] ToArray(DateTime value)
        {
			byte[] bytes = null;

			using (var stream = new MemoryStream())
			{
				using (var writer = new BinaryWriter(stream))
				{
					writer.Write(value.ToString("u"));
					bytes = stream.ToArray();
				}
			}

			return bytes;
        }

        /// <summary>
        /// Converts from TimeSpan to a byte array.
        /// </summary>
        /// <param name="value">input value to convert to byte array</param>
        /// <returns>byte array</returns>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times", Justification = "See earlier justification.")]
	public byte[] ToArray(TimeSpan value)
        {
			byte[] bytes = null;

			using (var stream = new MemoryStream())
			{
				using (var writer = new BinaryWriter(stream))
				{
					writer.Write(value.ToString());
					bytes = stream.ToArray();
				}
			}

			return bytes;
        }

        /// <summary>
        /// Converts from byte to a byte array.
        /// </summary>
        /// <param name="value">input value to convert to byte array</param>
        /// <returns>byte array</returns>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times", Justification = "See earlier justification.")]
	public byte[] ToArray(byte value)
        {
			byte[] bytes = null;

			using (var stream = new MemoryStream())
			{
				using (var writer = new BinaryWriter(stream))
				{
					writer.Write(value);
					bytes = stream.ToArray();
				}
			}

			return bytes;
        }

        /// <summary>
        /// Converts from Guid to a byte array.
        /// </summary>
        /// <param name="value">input value to convert to byte array</param>
        /// <returns>byte array</returns>
        public byte[] ToArray(Guid value)
        {
            return value.ToByteArray();
        }

        /// <summary>
        /// Converts from int16 to a byte array.
        /// </summary>
        /// <param name="value">input value to convert to byte array</param>
        /// <returns>byte array</returns>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times", Justification = "See earlier justification.")]
	public byte[] ToArray(short value)
        {
			byte[] bytes = null;

			using (var stream = new MemoryStream())
			{
				using (var writer = new BinaryWriter(stream))
				{
					writer.Write(value);
					bytes = stream.ToArray();
				}
			}

			return bytes;
        }

        /// <summary>
        /// Converts from Int32 to a byte array.
        /// </summary>
        /// <param name="value">input value to convert to byte array</param>
        /// <returns>byte array</returns>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times", Justification = "See earlier justification.")]
	public byte[] ToArray(int value)
        {
			byte[] bytes = null;

			using (var stream = new MemoryStream())
			{
				using (var writer = new BinaryWriter(stream))
				{
					writer.Write(value);
					bytes = stream.ToArray();
				}
			}

			return bytes;
        }

        /// <summary>
        /// Converts from Int64 to a byte array.
        /// </summary>
        /// <param name="value">input value to convert to byte array</param>
        /// <returns>byte array</returns>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times", Justification = "See earlier justification.")]
	public byte[] ToArray(long value)
        {
			byte[] bytes = null;

			using (var stream = new MemoryStream())
			{
				using (var writer = new BinaryWriter(stream))
				{
					writer.Write(value);
					bytes = stream.ToArray();
				}
			}

			return bytes;
        }

        /// <summary>
        /// Converts from Single to a byte array.
        /// </summary>
        /// <param name="value">input value to convert to byte array</param>
        /// <returns>byte array</returns>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times", Justification = "See earlier justification.")]
	public byte[] ToArray(float value)
        {
			byte[] bytes = null;

			using (var stream = new MemoryStream())
			{
				using (var writer = new BinaryWriter(stream))
				{
					writer.Write(value);
					bytes = stream.ToArray();
				}
			}

			return bytes;
        }

        /// <summary>
        /// Converts from Double to a byte array.
        /// </summary>
        /// <param name="value">input value to convert to byte array</param>
        /// <returns>byte array</returns>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times", Justification = "See earlier justification.")]
	public byte[] ToArray(double value)
        {
			byte[] bytes = null;

			using (var stream = new MemoryStream())
			{
				using (var writer = new BinaryWriter(stream))
				{
					writer.Write(value);
					bytes = stream.ToArray();
				}
			}

			return bytes;
        }

        /// <summary>
        /// Converts from UInt16 to a byte array.
        /// </summary>
        /// <param name="value">input value to convert to byte array</param>
        /// <returns>byte array</returns>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times", Justification = "See earlier justification.")]
	public byte[] ToArray(ushort value)
        {
			byte[] bytes = null;

			using (var stream = new MemoryStream())
			{
				using (var writer = new BinaryWriter(stream))
				{
					writer.Write(value);
					bytes = stream.ToArray();
				}
			}

			return bytes;
        }

        /// <summary>
        /// Converts from UInt32 to a byte array.
        /// </summary>
        /// <param name="value">input value to convert to byte array</param>
        /// <returns>byte array</returns>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times", Justification = "See earlier justification.")]
	public byte[] ToArray(uint value)
        {
			byte[] bytes = null;

			using (var stream = new MemoryStream())
			{
				using (var writer = new BinaryWriter(stream))
				{
					writer.Write(value);
					bytes = stream.ToArray();
				}
			}

			return bytes;
        }

        /// <summary>
        /// Converts from UInt64 to a byte array.
        /// </summary>
        /// <param name="value">input value to convert to byte array</param>
        /// <returns>byte array</returns>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times", Justification = "See earlier justification.")]
	public byte[] ToArray(ulong value)
        {
			byte[] bytes = null;

			using (var stream = new MemoryStream())
			{
				using (var writer = new BinaryWriter(stream))
				{
					writer.Write(value);
					bytes = stream.ToArray();
				}
			}

			return bytes;
        }

        /// <summary>
        /// Converts from sbyte to a byte array.
        /// </summary>
        /// <param name="value">input value to convert to byte array</param>
        /// <returns>byte array</returns>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times", Justification = "See earlier justification.")]
	public byte[] ToArray(sbyte value)
        {
			byte[] bytes = null;

			using (var stream = new MemoryStream())
			{
				using (var writer = new BinaryWriter(stream))
				{
					writer.Write(value);
					bytes = stream.ToArray();
				}
			}

			return bytes;
        }

        #endregion

        #region Byte Array Appending

        /// <summary>
        /// Append bool To End Of Byte Array
        /// </summary>
        /// <param name="array">Original Value</param>
        /// <param name="value">Value To Append</param>
        public void Append(ref byte[] array, bool value)
        {
            Append(ref array, ToArray(value));
        }

        /// <summary>
        /// Append DateTimeOffset To End Of Byte Array
        /// </summary>
        /// <param name="array">Original Value</param>
        /// <param name="value">Value To Append</param>
        private void Append(ref byte[] array, DateTimeOffset value)
        {
            Append(ref array, ToArray(value));
        }

        /// <summary>
        /// Append DateTime To End Of Byte Array
        /// </summary>
        /// <param name="array">Original Value</param>
        /// <param name="value">Value To Append</param>
        public void Append(ref byte[] array, DateTime value)
        {
            Append(ref array, ToArray(value));
        }

        /// <summary>
        /// Append Time To End Of Byte Array
        /// </summary>
        /// <param name="array">Original Value</param>
        /// <param name="value">Value To Append</param>
        public void Append(ref byte[] array, TimeSpan value)
        {
            Append(ref array, ToArray(value));
        }

        /// <summary>
        /// Append Guid To End Of Byte Array
        /// </summary>
        /// <param name="array">Original Value</param>
        /// <param name="value">Value To Append</param>
        public void Append(ref byte[] array, Guid value)
        {
            Append(ref array, ToArray(value));
        }

        /// <summary>
        /// Append UInt64 To End Of Byte Array
        /// </summary>
        /// <param name="array">Original Value</param>
        /// <param name="value">Value To Append</param>
        public void Append(ref byte[] array, ulong value)
        {
            Append(ref array, ToArray(value));
        }

        /// <summary>
        /// Append Single To End Of Byte Array
        /// </summary>
        /// <param name="array">Original Value</param>
        /// <param name="value">Value To Append</param>
        public void Append(ref byte[] array, float value)
        {
            Append(ref array, ToArray(value));
        }

        /// <summary>
        /// Append Byte To End Of Byte Array
        /// </summary>
        /// <param name="array">Original Value</param>
        /// <param name="value">Value To Append</param>
        public void Append(ref byte[] array, byte value)
        {
            Array.Resize(ref array, array.Length + 1);
            array[array.Length - 1] = value;
        }

        /// <summary>
        /// Append Bytes To End Of Byte Array
        /// </summary>
        /// <param name="array">Original Value</param>
        /// <param name="value">Value To Append</param>
        public void Append(ref byte[] array, byte[] value)
        {
            Array.Resize(ref array, array.Length + value.Length);
            Array.Copy(value, 0, array, array.Length - value.Length,
                       value.Length);
        }

        /// <summary>
        /// Append SByte Value Bytes To Array
        /// </summary>
        /// <param name="array">Original Value</param>
        /// <param name="value">Value To Append</param>
        public void Append(ref byte[] array, sbyte value)
        {
            Append(ref array, ToArray(value));
        }

        /// <summary>
        /// Append Short Value Bytes To Array
        /// </summary>
        /// <param name="array">Original Value</param>
        /// <param name="value">Value To Append</param>
        public void Append(ref byte[] array, short value)
        {
            Append(ref array, ToArray(value));
        }

        /// <summary>
        /// Append UShort Value Bytes To Array
        /// </summary>
        /// <param name="array">Original Value</param>
        /// <param name="value">Value To Append</param>
        public void Append(ref byte[] array, ushort value)
        {
            Append(ref array, ToArray(value));
        }

        /// <summary>
        /// Append Integer Value Bytes To Array
        /// </summary>
        /// <param name="array">Original Value</param>
        /// <param name="value">Value To Append</param>
        public void Append(ref byte[] array, int value)
        {
            Append(ref array, ToArray(value));
        }

        /// <summary>
        /// Append Long Value Bytes To Array
        /// </summary>
        /// <param name="array">Original Value</param>
        /// <param name="value">Value To Append</param>
        public void Append(ref byte[] array, long value)
        {
            Append(ref array, ToArray(value));
        }

        /// <summary>
        /// Append UInt Value Bytes To Array
        /// </summary>
        /// <param name="array">Original Value</param>
        /// <param name="value">Value To Append</param>
        public void Append(ref byte[] array, uint value)
        {
            Append(ref array, ToArray(value));
        }

        /// <summary>
        /// Append Double Value Bytes To Array
        /// </summary>
        /// <param name="array">Original Value</param>
        /// <param name="value">Value To Append</param>
        public void Append(ref byte[] array, double value)
        {
            Append(ref array, ToArray(value));
        }

        /// <summary>
        /// Append Decimal Value Bytes To Array
        /// </summary>
        /// <param name="array">Original Value</param>
        /// <param name="value">Value To Append</param>
        public void Append(ref byte[] array, decimal value)
        {
            Append(ref array, ToArray(value));
        }

        /// <summary>
        /// Append Char Value Bytes To Array
        /// </summary>
        /// <param name="array">Original Value</param>
        /// <param name="value">Value To Append</param>
        /// <param name="encoding">The encoding of the data</param>
        public void Append(ref byte[] array, char value, Encoding encoding)
        {
            Append(ref array, encoding.GetBytes(new[] { value }));
        }

        /// <summary>
        /// Append String Bytes From Encoding To Array
        /// </summary>
        /// <param name="array">Original Value</param>
        /// <param name="value">Value To Append</param>
        /// <param name="encoding">Encoding To Use</param>
        public void Append(ref byte[] array, string value, Encoding encoding)
        {
            Append(ref array, encoding.GetBytes(value));
        }

        #endregion
    }
}