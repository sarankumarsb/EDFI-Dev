using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EdFi.Dashboards.Common;

namespace EdFi.Dashboards.Resources.Security
{
  
    internal class TypeMetaData
    {
        internal string FullName { get; private set; }
        internal bool IsArray { get; private set; }
        internal bool IsDictionary { get; private set; }
        internal bool IsQueryable { get; private set; }
        internal bool IsSet { get; private set; }
        internal bool IsHashtable { get; set; }
        internal bool IsDeferredLinqQuery { get; private set; }
        internal bool ImplementsIList { get; private set; }
        internal Type ListType { get; private set; }
        internal MethodInfo ListTypeToArrayMethod { get; private set; }
        internal MethodInfo ToArrayMethod { get; private set; }
        internal MethodInfo ToListMethod { get; private set; }
        internal MethodInfo AsQueryableMethod { get; private set; }
        internal bool IsCollection { get; private set; }
        internal FieldAccessor[] FieldAccessors { get; private set; }
        internal LateBoundVoidMethod ExceptWithMethod { get; private set; }
  
        internal TypeMetaData(Type type)
        {
            FullName = type.FullName;
            IsArray = type.IsArray;
            IsDictionary = type.IsDictionaryType();
            IsCollection = type.HasInterface(typeof (IEnumerable));
            IsQueryable = type.HasInterface(typeof(IQueryable)); 
            IsDeferredLinqQuery = type.IsDeferredLinqQuery();

            IsSet = type.IsSet();

            IsHashtable = type == typeof(Hashtable);

            if (IsArray || IsSet)
            {
                ListType = typeof(List<>).MakeGenericType(type.IsGenericType ? 
                    type.GetGenericArguments().First() :
                    type.GetElementType());//when array

                ListTypeToArrayMethod = ListType.GetMethod("ToArray");
            }

            var enumerableInfo = type.GetEnumerableInfo();
            if (enumerableInfo != null)
            {
                ToArrayMethod = typeof (Enumerable).GetMethod("ToArray").MakeGenericMethod(enumerableInfo.AllEnumerableItemTypes.Last());
                ToListMethod = typeof (Enumerable).GetMethod("ToList").MakeGenericMethod(enumerableInfo.AllEnumerableItemTypes.Last());
                AsQueryableMethod = 
                    (from m in typeof(Queryable).GetMethods()
                     where m.Name == "AsQueryable" && m.IsGenericMethod
                    select m)
                    .First()
                    .MakeGenericMethod(enumerableInfo.AllEnumerableItemTypes.Last());
            }

            if (IsSet)
            {
                ExceptWithMethod = type.GetMethod("ExceptWith").InvokeDelegate<LateBoundVoidMethod>();
            }

           
            ImplementsIList = typeof (IList).IsAssignableFrom(type);

            FieldAccessors = !IsCollection
                                  ? GetAllFields(type)
                                        .Where(t => t.FieldType.IsComplex())
                                        .Select(f => new FieldAccessor(f.Name, f.FieldType, f.ValueGetter(), f.ValueSetter()))
                                        .ToArray()
                                  : new FieldAccessor[] { };
        }

        private static IEnumerable<FieldInfo> GetAllFields(Type t)
        {
            if (t == null)
                return Enumerable.Empty<FieldInfo>();

            const BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

            return t.GetFields(flags).Concat(GetAllFields(t.BaseType));
        }

        public override string ToString()
        {
            return FullName;
        }        

        internal struct FieldAccessor
        {
            internal FieldAccessor(string fieldName, Type type, LateBoundFieldGet getAccessor, LateBoundFieldSet setAccessor)
            {
                try
                {
                    //if field is a compiler generated backing field (i.e auto property), try to extract the property name
                    FieldName = fieldName.EndsWith(">k__BackingField")
                                    ? fieldName.Substring(1, fieldName.IndexOf(">", System.StringComparison.Ordinal) - 1)
                                    : fieldName;
                }
                catch
                {
                    FieldName = fieldName;
                }

                FieldType = type;
                FieldTypeName = type.ToString();
                GetAccessor = getAccessor;
                SetAccessor = setAccessor;
            }

            internal readonly string FieldName;
            internal readonly Type FieldType;
            internal readonly string FieldTypeName;
            internal readonly LateBoundFieldGet GetAccessor;
            internal readonly LateBoundFieldSet SetAccessor;
        }

    }
}
