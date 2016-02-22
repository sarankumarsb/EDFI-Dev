using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using EdFi.Dashboards.Common;
using EdFi.Dashboards.Resources.Models.Student;

namespace EdFi.Dashboards.Resources.Security
{
    public class StudentFilter
    {
        private static readonly IEnumerable<ITypeHandler> handlers = new ITypeHandler[]
            {
                new ArrayHandler(),
                new ListHandler(),
                new DictionaryHandler(), 
                new HashSetHandler(), 
                new SimpleObjectHandler(), 
                new HashtableHandler(), 
                new QueryableHandler() // This should be last since it converts anything that is query-able to lists.
            };
        private readonly AccessibleStudents accessibleStudents;
        private static Dictionary<string, TypeMetaData> typeMetaDataCache = new Dictionary<string, TypeMetaData>();

        public StudentFilter(AccessibleStudents accessibleStudents)
        {
            this.accessibleStudents = accessibleStudents;
        }

        //Todo: support FilterByFieldAttribute
        public object ExecuteFilter(object item, Hashtable visitedObjects = null)
        {
            if (visitedObjects == null) visitedObjects = new Hashtable();
            // If item is null, we don't need to examine it. 
            // If item is null, we don't need to examine it. 
            if (item == null)
                return null;

            var itemType = item.GetType();
            
            // If item is a primitive type, we don't need to examine it. 
            if (!itemType.IsComplex())
                return item;

            var student = item as IStudent;

            if (student != null)
                return accessibleStudents.CanAccessStudent(student.StudentUSI) ? item : null;

            // The object doesn't implement IStudent, so we have to examine further (i.e look at the object properties or
            // the elements in the collection when the object is a collection type.)

            var typeMetaData = GetTypeMetaData(itemType);

            // If the object contains a linq deferred query, execute it so that we can iterate over the query object
            if (typeMetaData.IsDeferredLinqQuery)
            {
                //Invoke the ToArray method of the query object
                item = typeMetaData.ToArrayMethod.Invoke(null, new [] {item});
                //Try again with the new type (array of T)
                return ExecuteFilter(item, visitedObjects);
            }

            //Find a handler that can handle this type
            var handler = handlers.FirstOrDefault(h => h.CanHandle(typeMetaData));

            if (handler == null)
                throw new InvalidOperationException(string.Format("{0} cannot filter type '{1}'.", typeof(StudentFilter).Name, itemType.FullName));

            return handler.Handle(item, typeMetaData, ExecuteFilter, visitedObjects);
        }

        private static TypeMetaData GetTypeMetaData(Type itemType)
        {
            TypeMetaData typeMetaData;
            typeMetaDataCache.TryGetValue(itemType.FullName, out typeMetaData);

            //If a cache read miss occured, build a TypeMetaData object for the type and add to the cashe
            if (typeMetaData == null)
            {
                typeMetaData = new TypeMetaData(itemType);

                AddToTypeMetaDataCache(itemType.FullName, typeMetaData);
            }
            return typeMetaData;
        }


        /// <summary>
        /// Adds an item to TypeMetaDataCache in a non-locking way but still thread-safe.
        /// </summary>
        /// <param name="key"></param>
        private static void AddToTypeMetaDataCache(string key, TypeMetaData typeMetaData)
        {
            Dictionary<string, TypeMetaData> snapshot, newCache;

            do
            {
                snapshot = typeMetaDataCache;
                newCache = new Dictionary<string, TypeMetaData>(typeMetaDataCache);
                newCache[key] = typeMetaData;
                //If the Interlocked.CompareExchange fails, it means we lost the cache update race with another thread, 
                //therefore just try again.
            } while (!ReferenceEquals(
                Interlocked.CompareExchange(ref typeMetaDataCache, newCache, snapshot),
                snapshot));
        }

        private interface ITypeHandler
        {
            object Handle(object item, TypeMetaData type, Func<object, Hashtable, object> filter, Hashtable visitedObjects);
            bool CanHandle(TypeMetaData type);
        }

        private class ListHandler : ITypeHandler
        {
            public object Handle(object item, TypeMetaData type, Func<object, Hashtable, object> filter, Hashtable visitedObjects)
            {
                var list = (IList)item  ;
               
                for (int i = list.Count - 1; i >= 0; i--)
                {
                    var listItemFiltered = filter(list[i], visitedObjects);
                    if (listItemFiltered == null)
                        list.RemoveAt(i);
                }

                return list;
            }

            public bool CanHandle(TypeMetaData type)
            {
                return type.ImplementsIList && !type.IsArray;
            }
        }

        private class HashSetHandler : ITypeHandler
        {
            public object Handle(object item, TypeMetaData type, Func<object, Hashtable, object> filter, Hashtable visitedObjects)
            {
                var blackList = (IList)Activator.CreateInstance(type.ListType);

                //Add the items that user is not allowed to view to the black list
                foreach (var setItem in (IEnumerable<object>)item)
                {
                    var filteredItem = filter(setItem, visitedObjects);

                    if (filteredItem == null)
                        blackList.Add(setItem);
                }

                //Remove the items that are in the black list
                if (blackList.Count > 0)
                    type.ExceptWithMethod(item, new[] {blackList});

                return item;
            }

            public bool CanHandle(TypeMetaData type)
            {
                return type.IsSet;
            }
        }

        private class HashtableHandler : ITypeHandler
        {
            public object Handle(object item, TypeMetaData type, Func<object, Hashtable, object> filter, Hashtable visitedObjects)
            {
                var hashTable = (Hashtable)item;

                //Add the items that user is not allowed to view to the black list
                var blackList = new Hashtable();
                foreach (var key in hashTable.Keys)
                {
                    var filterKey = filter(key, visitedObjects);
                    if (filterKey == null)
                        blackList[key] = "";
                    else
                    {
                        var value = hashTable[key];
                        var filterValue = filter(value, visitedObjects);
                        if (filterValue == null)
                            blackList[key] = value;
                    }
                }

                //Remove the items that are in the black list
                foreach (var key in blackList)
                {
                    hashTable.Remove(key);
                }

                return item;
            }

            public bool CanHandle(TypeMetaData type)
            {
                return type.IsHashtable;
            }
        }

        private class ArrayHandler : ITypeHandler
        {
            public object Handle(object item, TypeMetaData type, Func<object, Hashtable, object> filter, Hashtable visitedObjects)
            {
                //Create a list and add all the items user have access to.
                var filteredList = (IList)Activator.CreateInstance(type.ListType);

                foreach (var arrayItem in (IEnumerable<object>)item)
                {
                    var filtered = filter(arrayItem, visitedObjects);

                    if (filtered != null)
                        filteredList.Add(filtered);
                }

                //Convert the list to array so that we can assign it back to the field
                return type.ListTypeToArrayMethod.Invoke(filteredList, null);
            }

            public bool CanHandle(TypeMetaData type)
            {
                return type.IsArray;
            }
        }

        private class QueryableHandler : ITypeHandler
        {
            public object Handle(object item, TypeMetaData type, Func<object, Hashtable, object> filter, Hashtable visitedObjects)
            {
                // Convert item to a list for processing, and refilter it
                return filter(type.ToListMethod.Invoke(null, new[] { item }), visitedObjects);
            }

            public bool CanHandle(TypeMetaData type)
            {
                return type.IsQueryable;
            }
        }

        private class DictionaryHandler : ITypeHandler
        {
            public object Handle(object item, TypeMetaData type, Func<object, Hashtable, object> filter, Hashtable visitedObjects)
            {
                var dictionary = (IDictionary)item;

                //Add the items that user is not allowed to view to the black list
                var blackList = new HashSet<object>();
                foreach (var key in dictionary.Keys)
                {
                    var filterKey = filter(key, visitedObjects);
                    if (filterKey == null)
                        blackList.Add(key);
                    else
                    {
                        var value = dictionary[key];
                        var filterValue = filter(value, visitedObjects);
                        if (filterValue == null)
                         blackList.Add(key);
                    }
                }

                //Remove the items that are in the black list
                foreach (var key in blackList)
                {
                    dictionary.Remove(key);
                }

                return item;
            }

            public bool CanHandle(TypeMetaData type)
            {
                return type.IsDictionary;
            }
        }

        private class SimpleObjectHandler : ITypeHandler
        {
            public object Handle(object item, TypeMetaData type, Func<object, Hashtable, object> filter, Hashtable visitedObjects)
            {
                if (visitedObjects.ContainsKey(item)) return visitedObjects[item];
                visitedObjects[item] = item;

                // need to go through each property on the object and see if the value needs to be filtered
                foreach (var field in type.FieldAccessors)
                {
                    var value = field.GetAccessor(item);

                    if (value == null)
                        continue;

                    var filteredValue = filter(value, visitedObjects);

                    try
                    {
                        field.SetAccessor(item, filteredValue);
                    }
                    catch (InvalidCastException e)
                    {
                        throw new InvalidCastException(string.Format("{0} was able to filter type {1} but failed to assign the filtered data back to the object", typeof(StudentFilter).Name, type.FullName), e);
                    }
                }

                return item;
            }

            public bool CanHandle(TypeMetaData type)
            {
                return !type.IsCollection;
            }
        }
    }
}

   
