// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System;
using EdFi.Dashboards.Metric.Resources.Models;

namespace EdFi.Dashboards.Resources.Models.Common
{
    [Serializable]
    public class AttributeItem<T> : IConvertible
    {
        public string Attribute { get; set; }
        public T Value { get; set; }
        public TypeCode GetTypeCode()
        {
            return Type.GetTypeCode(Value.GetType());
        }

        public bool ToBoolean(IFormatProvider provider)
        {
            return GetIConvertible().ToBoolean(provider);
        }

        public char ToChar(IFormatProvider provider)
        {
            return GetIConvertible().ToChar(provider);
        }

        public sbyte ToSByte(IFormatProvider provider)
        {
            return GetIConvertible().ToSByte(provider);
        }

        public byte ToByte(IFormatProvider provider)
        {
            return GetIConvertible().ToByte(provider);
        }

        public short ToInt16(IFormatProvider provider)
        {
            return GetIConvertible().ToInt16(provider);
        }

        public ushort ToUInt16(IFormatProvider provider)
        {
            return GetIConvertible().ToUInt16(provider);
        }

        public int ToInt32(IFormatProvider provider)
        {
            return GetIConvertible().ToInt32(provider);
        }

        public uint ToUInt32(IFormatProvider provider)
        {
            return GetIConvertible().ToUInt32(provider);
        }

        public long ToInt64(IFormatProvider provider)
        {
            return GetIConvertible().ToInt64(provider);
        }

        public ulong ToUInt64(IFormatProvider provider)
        {
            return GetIConvertible().ToUInt64(provider);
        }

        public float ToSingle(IFormatProvider provider)
        {
            return GetIConvertible().ToSingle(provider);
        }

        public double ToDouble(IFormatProvider provider)
        {
            return GetIConvertible().ToDouble(provider);
        }

        public decimal ToDecimal(IFormatProvider provider)
        {
            return GetIConvertible().ToDecimal(provider);
        }

        private IConvertible GetIConvertible()
        {
            var convertible = Value as IConvertible;

            if (convertible == null)
                throw new InvalidOperationException(
                    string.Format("Attribute Item's value type of '{0}' does not implement {1}", typeof (T).FullName, typeof(IConvertible).Name));
            return convertible;
        }

        public DateTime ToDateTime(IFormatProvider provider)
        {
            return GetIConvertible().ToDateTime(provider);
        }

        public string ToString(IFormatProvider provider)
        {
            return GetIConvertible().ToString(provider);
        }

        public object ToType(Type conversionType, IFormatProvider provider)
        {
            return GetIConvertible().ToDecimal(provider);
        }
    }

    [Serializable]
    public class AttributeItemWithUrl<T> : AttributeItem<T>
    {
        public string Url { get; set; }
    }

    [Serializable]
    public class AttributeItemWithTrend<T> : AttributeItemWithUrl<T>
    {
        public TrendDirection Trend { get; set; }
    }

	[Serializable]
	public class AttributeItemWithSelected<T> : AttributeItemWithUrl<T>
	{
		public bool Selected { get; set; }
	}
}
