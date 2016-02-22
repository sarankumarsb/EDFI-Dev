using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Caching;

namespace EdFi.Dashboards.Infrastructure.Implementations
{
    public class HashtableCacheProvider : ICacheProvider
    {
		private Hashtable cache = new Hashtable();
        public Hashtable Cache
		{
			get { return cache; }
			set { cache = value; }
		}


        private Dictionary<string, EntryDetails> cacheEntryDetails = new Dictionary<string,EntryDetails>();
		public Dictionary<string, EntryDetails> CacheEntryDetails
		{
			get { return cacheEntryDetails; }
			set { cacheEntryDetails = value; }
		}

        public void RemoveCachedObjects(string keyContains)
        {
            throw new NotImplementedException();
        }

        public void RemoveCachedObject(string keyName)
        {
            throw new NotImplementedException();
        }

        public bool TryGetCachedObject(string objectName, out object value)
        {
            if (Cache.ContainsKey(objectName))
            {
                value = Cache[objectName];
                return true;
            }

            value = null;
            return false;
        }

        public void SetCachedObject(string objectName, object obj)
        {
            Cache[objectName] = obj;
        }

        public void Insert(string key, object value, CacheDependency dependencies, DateTime absoluteExpiration, TimeSpan slidingExpiration)
        {
            Cache[key] = value;
            CacheEntryDetails[key] = new EntryDetails
                                         {
                                             CacheDependency = dependencies,
                                             AbsoluteExpiration = absoluteExpiration,
                                             SlidingExpiration = slidingExpiration
                                         };
        }

        public void Insert(string key, object value, DateTime absoluteExpiration, TimeSpan slidingExpiration)
        {
            Cache[key] = value;
            CacheEntryDetails[key] = new EntryDetails
            {
                AbsoluteExpiration = absoluteExpiration,
                SlidingExpiration = slidingExpiration
            };
        }

        public class EntryDetails
        {
            public CacheDependency CacheDependency { get; set; }
            public DateTime AbsoluteExpiration { get; set; }
            public TimeSpan SlidingExpiration { get; set; }
        }
    }
}
