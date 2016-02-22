// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;

namespace EdFi.Dashboards.Infrastructure
{
	public interface ICacheProvider
	{
	    void RemoveCachedObjects(string keyContains);
	    void RemoveCachedObject(string keyName);
	    bool TryGetCachedObject(string objectName, out object value);
		void SetCachedObject(string objectName, object obj);
		void Insert(string key, object value, System.Web.Caching.CacheDependency dependencies, DateTime absoluteExpiration, TimeSpan slidingExpiration);
	    void Insert(string key, object value, DateTime absoluteExpiration, TimeSpan slidingExpiration);
	}
}
