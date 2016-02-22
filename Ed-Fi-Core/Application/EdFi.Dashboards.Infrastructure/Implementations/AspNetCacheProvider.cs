// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Caching;

namespace EdFi.Dashboards.Infrastructure.Implementations
{
	public class AspNetCacheProvider : ICacheProvider
	{
        private readonly static object NullObject = new object();

        public bool TryGetCachedObject(string keyName, out object value)
        {
            value = HttpRuntime.Cache[keyName];

            if (value == NullObject)
            {
                value = null;
                return true;
            }

            return value != null;
        }

        public void RemoveCachedObjects(string keyContains)
        {
            var keysToRemove = new List<string>();

            var cacheDictionary = HttpRuntime.Cache.GetEnumerator();
            while (cacheDictionary.MoveNext())
            {
                var cacheKey = cacheDictionary.Key.ToString();

                if(cacheKey.ToLower().Contains(keyContains.ToLower()))
                {
                    keysToRemove.Add(cacheKey);
                }
            }
            foreach(var keyToRemove in keysToRemove)
            {
                RemoveCachedObject(keyToRemove);
            }
        }
        public void RemoveCachedObject(string keyName)
        {
            HttpRuntime.Cache.Remove(keyName);
        }
		public object GetCachedObject(string keyName)
		{
            var result = HttpRuntime.Cache[keyName];

            if (result == NullObject)
                return null;

            return result;
		}

		public void SetCachedObject(string keyName, object value)
		{
            HttpRuntime.Cache[keyName] = value ?? NullObject;
		}

		public void Insert(string key, object value, CacheDependency dependencies, DateTime absoluteExpiration, TimeSpan slidingExpiration)
		{
            HttpRuntime.Cache.Insert(key, value ?? NullObject, dependencies, absoluteExpiration, slidingExpiration);
        }

        public void Insert(string key, object value, DateTime absoluteExpiration, TimeSpan slidingExpiration)
        {
            HttpRuntime.Cache.Insert(key, value ?? NullObject, null, absoluteExpiration, slidingExpiration);
        }
	}
}
