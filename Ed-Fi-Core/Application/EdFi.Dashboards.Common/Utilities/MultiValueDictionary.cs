using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace EdFi.Dashboards.Common.Utilities
{
    public class MultiValueDictionary<TKey, TValue> //: IDictionary<TKey, TValue>
    {
        public MultiValueDictionary<TKey, TValue> CreateMap(IEnumerable<TValue> source, Expression<Func<TValue, TKey>> keyValueAccessor)
        {
            var expr = keyValueAccessor.Body as MemberExpression;

            if (expr == null)
                throw new ArgumentException("Expression must be a member access expression.", "keyValueAccessor");

            var accessor = keyValueAccessor.Compile();

            var map = new MultiValueDictionary<TKey, TValue>();

            if (source != null)
            {
                foreach (var item in source)
                    map.Add(accessor(item), item);
            }

            return map;
        }

        private Dictionary<TKey, List<TValue>> dictionary = new Dictionary<TKey, List<TValue>>();

        public void Add(TKey key, TValue value)
        {
            List<TValue> list;

            // can be a optimized a little with TryGetValue, this is for clarity
            if (dictionary.TryGetValue(key, out list))
            {
                list.Add(value);
            }
            else
                dictionary.Add(key, new List<TValue>() { value });
        }

        public void Clear()
        {
            dictionary.Clear();
        }

        /// <summary>
        /// Get values associated with specified key, or an empty enumerable if none exist.
        /// </summary>
        /// <param name="key">The key for the dictionary entry.</param>
        /// <returns>The collection of values stored for the specified key.</returns>
        public IEnumerable<TValue> ValuesByKey(TKey key)
        {
            List<TValue> nodes;

            if (dictionary.TryGetValue(key, out nodes))
                return nodes;
            else
                return Enumerable.Empty<TValue>();
        }

        //public bool Contains(KeyValuePair<TKey, TValue> item)
        //{
        //    throw new NotImplementedException();
        //}

        //public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        //{
        //    throw new NotImplementedException();
        //}

        //public bool Remove(KeyValuePair<TKey, TValue> item)
        //{
        //    throw new NotImplementedException();
        //}

        //public int Count
        //{
        //    get { throw new NotImplementedException(); }
        //}

        //public bool IsReadOnly
        //{
        //    get { throw new NotImplementedException(); }
        //}

        // more members

        #region Implementation of IEnumerable

        //public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        //{
        //    throw new NotImplementedException();
        //}

        //IEnumerator IEnumerable.GetEnumerator()
        //{
        //    return GetEnumerator();
        //}

        #endregion

        #region Implementation of IDictionary<TKey,TValue>

        //public bool ContainsKey(TKey key)
        //{
        //    throw new NotImplementedException();
        //}

        //public void Add(TKey key, TValue value)
        //{
        //    throw new NotImplementedException();
        //}

        //public bool Remove(TKey key)
        //{
        //    throw new NotImplementedException();
        //}

        public bool TryGetValue(TKey key, out IEnumerable<TValue> values)
        {
            List<TValue> list;

            if (dictionary.TryGetValue(key, out list))
            {
                values = list;
                return true;
            }

            values = null;
            return false;
        }

        public IEnumerable<TValue> this[TKey key]
        {
            get { return dictionary[key]; }
            //set { throw new NotImplementedException(); }
        }

        //public ICollection<TKey> Keys
        //{
        //    get { throw new NotImplementedException(); }
        //}

        //public ICollection<TValue> Values
        //{
        //    get { throw new NotImplementedException(); }
        //}

        #endregion
    }
}
