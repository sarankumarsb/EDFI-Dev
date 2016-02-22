using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace EdFi.Dashboards.Infrastructure.Implementations.Caching
{
    public class EdFiSecurityTokenCache : SecurityTokenCache
    {
        private readonly ISessionStateProvider sessionStateProvider;

        public EdFiSecurityTokenCache(ISessionStateProvider sessionStateProvider)
        {
            this.sessionStateProvider = sessionStateProvider;
        }

        public override void ClearEntries()
        {
            // not implemented... haven't been able to find how/when this method is used.
            //leaving the error in case the method is ever called
            throw new NotImplementedException();
        }

        public override bool TryAddEntry(object key, System.IdentityModel.Tokens.SecurityToken value)
        {
            if (key == null)
                throw new ArgumentNullException("key", "Cache key cannot be null.");

            var cacheKey = key as SecurityTokenCacheKey;
            if (cacheKey == null)
                throw new ArgumentException("Cache key should be of type 'SecurityTokenCacheKey'.", "key");

            // add the entry to the cache.
            sessionStateProvider.SetValue(GetTokenCacheKeyString(cacheKey), value);
            //not sure how to send back true since no bool value returned from SetValue,
            //could try a getvalue and test but is it wourth it?
            return true;
        }

        public override bool TryGetAllEntries(object key, out IList<System.IdentityModel.Tokens.SecurityToken> tokens)
        {
            // not implemented... haven't been able to find how/when this method is used.
            //leaving the error in case the method is ever called
            throw new NotImplementedException();
        }

        public override bool TryGetEntry(object key, out System.IdentityModel.Tokens.SecurityToken value)
        {
            //initialize outbound value to null
            value = null;

            if (key == null)
                throw new ArgumentNullException("key", "Cache key cannot be null.");

            var cacheKey = key as SecurityTokenCacheKey;
            if (cacheKey == null)
                throw new ArgumentException("Cache key should be of type 'SecurityTokenCacheKey'.", "key");

            var objectSecurityToken = sessionStateProvider.GetValue(GetTokenCacheKeyString(cacheKey));
            value = objectSecurityToken as System.IdentityModel.Tokens.SecurityToken;

            return (value != null);
        }

        public override bool TryRemoveAllEntries(object key)
        {
            // not implemented... haven't been able to find how/when this method is used.
            //leaving the error in case the method is ever called
            throw new NotImplementedException();
        }

        public override bool TryRemoveEntry(object key)
        {
            if (key == null)
                throw new ArgumentNullException("key", "Cache key cannot be null.");

            var cacheKey = key as SecurityTokenCacheKey;
            if (cacheKey == null)
                throw new ArgumentException("Cache key should be of type 'SecurityTokenCacheKey'.", "key");

            sessionStateProvider.RemoveValue(GetTokenCacheKeyString(cacheKey));
            //not sure how to catch a case of returning false since the RemoveCachedObject doesnt return a bool value.
            return true;
        }

        public override bool TryReplaceEntry(object key, System.IdentityModel.Tokens.SecurityToken newValue)
        {
            if (key == null)
                throw new ArgumentNullException("key", "Cache key cannot be null.");

            var cacheKey = key as SecurityTokenCacheKey;
            if (cacheKey == null)
                throw new ArgumentException("Cache key should be of type 'SecurityTokenCacheKey'.", "key");

            sessionStateProvider.SetValue(GetTokenCacheKeyString(cacheKey), newValue);
            //not sure how to catch a case to return false since setvalue doesnot return anything.
            return true;
        }

        private static string GetTokenCacheKeyString(SecurityTokenCacheKey key)
        {
            return string.Format("{0}; {1}; {2}", key.ContextId, key.KeyGeneration, key.EndpointId);
        }
    }
}
