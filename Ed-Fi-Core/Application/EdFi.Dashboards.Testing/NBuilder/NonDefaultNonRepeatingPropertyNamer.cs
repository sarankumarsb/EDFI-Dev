// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System;
using System.Collections.Generic;
using System.Reflection;
using FizzWare.NBuilder;
using FizzWare.NBuilder.Implementation;
using FizzWare.NBuilder.PropertyNaming;

namespace EdFi.Dashboards.Testing.NBuilder
{
    public class NonDefaultNonRepeatingPropertyNamer : PropertyNamer
	{
		public NonDefaultNonRepeatingPropertyNamer(IReflectionUtil reflectionUtility)
            : base(reflectionUtility)
		{
		}

		public override void SetValuesOfAllIn<T>(IList<T> objects)
		{
			foreach (var o in objects)
			{
				SetValuesOf(o);
			}
		}

		private readonly Dictionary<Type, object> nextValue = new Dictionary<Type, object>();

		private void EnsureDefaultForType(Type type)
		{
			if (!nextValue.ContainsKey(type))
			{
				nextValue[type] = type.GetDefault();
			}
		}

		protected override short GetInt16(MemberInfo memberInfo)
		{
		    return GetInt16();
		}

        private short GetInt16()
        {
            EnsureDefaultForType(typeof(short));

            var newValue = (short) (((short) nextValue[typeof(short)]) + 1);

            nextValue[typeof(short)] = newValue;

            return newValue;
        }

        protected override int GetInt32(MemberInfo memberInfo)
        {
            return GetInt32();
        }

        private int GetInt32()
        {
            EnsureDefaultForType(typeof(int));

            int newValue = ((int) nextValue[typeof(int)]) + 1;

            nextValue[typeof(int)] = newValue;

            return newValue;
        }

        protected override long GetInt64(MemberInfo memberInfo)
        {
            return GetInt64();
        }

        private long GetInt64()
        {
            EnsureDefaultForType(typeof(long));

            long newValue = ((long) nextValue[typeof(long)]) + 1;

            nextValue[typeof(long)] = newValue;

            return newValue;
        }

        protected override decimal GetDecimal(MemberInfo memberInfo)
        {
            return GetDecimal();
        }

        private decimal GetDecimal()
        {
            EnsureDefaultForType(typeof(decimal));

            decimal newValue = ((decimal) nextValue[typeof(decimal)]) + 1;

            nextValue[typeof(decimal)] = newValue;

            return newValue;
        }

        protected override float GetSingle(MemberInfo memberInfo)
        {
            return GetSingle();
        }

        private float GetSingle()
        {
            EnsureDefaultForType(typeof(float));

            float newValue = ((float) nextValue[typeof(float)]) + 1;

            nextValue[typeof(float)] = newValue;

            return newValue;
        }

        protected override double GetDouble(MemberInfo memberInfo)
        {
            return GetDouble();
        }

        private double GetDouble()
        {
            EnsureDefaultForType(typeof(double));

            double newValue = ((double) nextValue[typeof(double)]) + 1;

            nextValue[typeof(double)] = newValue;

            return newValue;
        }

        protected override ushort GetUInt16(MemberInfo memberInfo)
        {
            return GetUInt16();
        }

        private ushort GetUInt16()
        {
            EnsureDefaultForType(typeof(ushort));

            var newValue = (ushort) (((ushort) nextValue[typeof(ushort)]) + 1);

            nextValue[typeof(ushort)] = newValue;

            return newValue;
        }

        protected override uint GetUInt32(MemberInfo memberInfo)
        {
            return GetUInt32();
        }

        private uint GetUInt32()
        {
            EnsureDefaultForType(typeof(uint));

            uint newValue = ((uint) nextValue[typeof(uint)]) + 1;

            nextValue[typeof(uint)] = newValue;

            return newValue;
        }

        protected override ulong GetUInt64(MemberInfo memberInfo)
        {
            return GetUInt64();
        }

        private ulong GetUInt64()
        {
            EnsureDefaultForType(typeof(ulong));

            ulong newValue = ((ulong) nextValue[typeof(ulong)]) + 1;

            nextValue[typeof(ulong)] = newValue;

            return newValue;
        }

        protected override sbyte GetSByte(MemberInfo memberInfo)
        {
            return GetSByte();
        }

        private sbyte GetSByte()
        {
            EnsureDefaultForType(typeof(sbyte));

            var newValue = (sbyte) (((sbyte) nextValue[typeof(sbyte)]) + 1); // May need to worry about overflows

            nextValue[typeof(sbyte)] = newValue;

            return newValue;
        }

        protected override byte GetByte(MemberInfo memberInfo)
        {
            return GetByte();
        }

        private byte GetByte()
        {
            EnsureDefaultForType(typeof(byte));

            var newValue = (byte) (((byte) nextValue[typeof(byte)]) + 1); // May need to worry about overflows

            nextValue[typeof(byte)] = newValue;

            return newValue;
        }

        protected override DateTime GetDateTime(MemberInfo memberInfo)
        {
            return GetDateTime();
        }

        private DateTime GetDateTime()
        {
            EnsureDefaultForType(typeof(DateTime));

            var newValue = ((DateTime) nextValue[typeof(DateTime)]).AddDays(1);

            nextValue[typeof(DateTime)] = newValue;

            return newValue;
        }

        private readonly Dictionary<string, int> suffixByStringValue = new Dictionary<string, int>();

		protected override string GetString(MemberInfo memberInfo)
		{
		    return GetString(memberInfo.Name);
		}

        private string GetString(string prefix)
        {
            if (!suffixByStringValue.ContainsKey(prefix))
            {
                suffixByStringValue[prefix] = 0;
            }

            int newSuffix = suffixByStringValue[prefix] + 1;
            suffixByStringValue[prefix] = newSuffix;

            return prefix + newSuffix;
        }

		protected override bool GetBoolean(MemberInfo memberInfo)
		{
		    return GetBoolean();
		}

        private static bool GetBoolean()
        {
            return true;
        }

        protected override char GetChar(MemberInfo memberInfo)
        {
            return GetChar();
        }

        private char GetChar()
        {
            if (!nextValue.ContainsKey(typeof(char)))
                nextValue[typeof(char)] = 0;

            int newValue = ((int) nextValue[typeof(int)]) + 1;

            nextValue[typeof(char)] = newValue;

            return (char) newValue;
        }

        protected override Enum GetEnum(MemberInfo memberInfo)
        {
            Type memberType = GetMemberType(memberInfo);

            return GetEnum(memberType);
        }

        private Enum GetEnum(Type enumType)
        {
            if (!nextValue.ContainsKey(enumType))
                nextValue[enumType] = 0;

            Array enumValues = GetEnumValues(enumType);

            int newIndex = (((int) nextValue[enumType]) + 1)%enumValues.Length;

            if ((int) enumValues.GetValue(newIndex) == 0)
                newIndex++;

            nextValue[enumType] = newIndex;

            return (Enum.Parse(enumType, enumValues.GetValue(newIndex).ToString()) as Enum);
        }

        protected override Guid GetGuid(MemberInfo memberInfo)
        {
            return GetGuid();
        }

        private static Guid GetGuid()
        {
            return Guid.NewGuid();
        }

        // Copied from base class verbatium, with changes needed to support nullable types

		protected override void HandleUnknownType<T>(Type memberType, MemberInfo memberInfo, T obj)
		//protected virtual void SetMemberValue<T>(MemberInfo memberInfo, T obj)
		{
			//Type memberType = GetMemberType(memberInfo);
			
			if (memberType.IsGenericType && memberType.GetGenericTypeDefinition() == typeof(Nullable<>))
			{
				// Convert member type to the underlying type
				memberType = Nullable.GetUnderlyingType(memberType);
			}

			//if (!BuilderSetup.HasDisabledAutoNameProperties || !this.ShouldIgnore(memberInfo))
			if (!ShouldIgnore(memberInfo))
			{
				object currentValue = GetCurrentValue(memberInfo, obj);
				if (ReflectionUtil.IsDefaultValue(currentValue))
				{
				    object newValue;

                    if (!TryGetValue(memberInfo.Name, memberType, out newValue))
					{
						base.HandleUnknownType(memberType, memberInfo, obj);
					}

					SetValue(memberInfo, obj, newValue);
				}
			}
		}

        private bool TryGetValue(string memberName, Type memberType, out object newValue)
        {
            if (memberType == typeof(short))
            {
                newValue = GetInt16();
            }
            else if (memberType == typeof(int))
            {
                newValue = GetInt32();
            }
            else if (memberType == typeof(long))
            {
                newValue = GetInt64();
            }
            else if (memberType == typeof(decimal))
            {
                newValue = GetDecimal();
            }
            else if (memberType == typeof(float))
            {
                newValue = GetSingle();
            }
            else if (memberType == typeof(double))
            {
                newValue = GetDouble();
            }
            else if (memberType == typeof(ushort))
            {
                newValue = GetUInt16();
            }
            else if (memberType == typeof(uint))
            {
                newValue = GetUInt32();
            }
            else if (memberType == typeof(ulong))
            {
                newValue = GetUInt64();
            }
            else if (memberType == typeof(char))
            {
                newValue = GetChar();
            }
            else if (memberType == typeof(byte))
            {
                newValue = GetByte();
            }
            else if (memberType == typeof(sbyte))
            {
                newValue = GetSByte();
            }
            else if (memberType == typeof(DateTime))
            {
                newValue = GetDateTime();
            }
            else if (memberType == typeof(string))
            {
                newValue = GetString(memberName);
            }
            else if (memberType == typeof(bool))
            {
                newValue = GetBoolean();
            }
            else if (memberType.BaseType == typeof(Enum))
            {
                newValue = GetEnum(memberType);
            }
            else if (memberType == typeof(Guid))
            {
                newValue = GetGuid();
            }
            else if (memberType.IsGenericType && typeof(IDictionary<,>).IsAssignableFrom(memberType.GetGenericTypeDefinition()))
            {
                var dictionary = GetDictionary(memberName, memberType);

                newValue = dictionary;
            }
            else if (typeof(System.Collections.IEnumerable).IsAssignableFrom(memberType))
            {
                var enumerable = GetEnumerableAsList(memberType, memberName);

                newValue = enumerable;
            }
            else
            {
                newValue = null;
                return false;
            }

            return true;
        }

        private object GetEnumerableAsList(Type memberType, string memberName)
        {
            object enumerable;

            // Added to support creation of 2-item Lists<T> for IEnumerable<T> properties
            var itemType = memberType.GetGenericArguments()[0];

            if (itemType == typeof(string))
            {
                // Must build string list ourselves here, as NBuilder complains about String class' constructor args
                enumerable = new List<string> {this.GetString(memberName), this.GetString(memberName)};
            }
            else
            {
                var genericBuilderType = typeof(Builder<>);
                var concreteBuilderType = genericBuilderType.MakeGenericType(itemType);
                var concreteBuilder = concreteBuilderType.GetMethod("CreateListOfSize", new Type[] {typeof(int)})
                                                         .Invoke(null, new object[] {2});

                var buildMethod = concreteBuilder.GetType().GetMethod("Build", BindingFlags.Public | BindingFlags.Instance);

                var model = buildMethod.Invoke(concreteBuilder, null);
                enumerable = model;
            }
            return enumerable;
        }

        private object GetDictionary(string memberName, Type memberType)
        {
            var keyType = memberType.GetGenericArguments()[0];
            var valueType = memberType.GetGenericArguments()[1];

            var genericDictionaryType = typeof(Dictionary<,>);
            var concreteDictionaryType = genericDictionaryType.MakeGenericType(keyType, valueType);
            var dictionary = Activator.CreateInstance(concreteDictionaryType);

            var addMethod = concreteDictionaryType.GetMethod("Add", BindingFlags.Public | BindingFlags.Instance);

            // Add two items to the dictionary
            for (int i = 0; i < 2; i++)
            {
                object key, value;

                if (!TryGetValue(memberName, keyType, out key))
                    throw new Exception(string.Format("Cannot create Dictionary entry with key of type '{0}'.", keyType.Name));

                if (!TryGetValue(memberName, valueType, out value))
                    throw new Exception(string.Format("Cannot create Dictionary entry with value of type '{0}'.", valueType.Name));

                addMethod.Invoke(dictionary, new[] {key, value});
            }

            return dictionary;
        }
	}
}
