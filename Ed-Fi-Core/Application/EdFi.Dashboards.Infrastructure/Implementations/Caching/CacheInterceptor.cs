// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Web.Caching;
using Castle.DynamicProxy;
using EdFi.Dashboards.Common;
using EdFi.Dashboards.Common.Utilities;

namespace EdFi.Dashboards.Infrastructure.Implementations.Caching
{
    public class CacheInterceptor : IInterceptorStage
    {
        private readonly int cacheExpiryMinutes;
        private readonly bool cacheEnabled;
        private readonly static Type VoidType = typeof(void);

        public const string CallStackContextKey = "CacheInterceptor.CacheCallStackContext";

        private readonly ICacheProvider[] cacheProviders;
        private readonly ICacheKeyGenerator cacheKeyGenerator;
        private readonly IServiceLocator serviceLocator;
        private readonly ISerializer serializer;
        private readonly ICacheProvider defaultCacheProvider;

        public CacheInterceptor(ICacheProvider[] cacheProviders, ICacheKeyGenerator cacheKeyGenerator, 
            IConfigValueProvider configValueProvider, IServiceLocator serviceLocator, ISerializer serializer)
        {
            this.cacheProviders = cacheProviders;
            this.cacheKeyGenerator = cacheKeyGenerator;
            this.serviceLocator = serviceLocator;
            this.serializer = serializer;

            cacheExpiryMinutes = Convert.ToInt32(configValueProvider.GetValue("CacheInterceptor.SlidingExpiration"));

            //The first cache provider is the default.
            defaultCacheProvider = cacheProviders.First();

            // Default to 5 minutes sliding expiration
            if (cacheExpiryMinutes == 0)
                cacheExpiryMinutes = 5;

            cacheEnabled = Convert.ToBoolean(configValueProvider.GetValue("CacheInterceptor.Enabled"));
        }

        public StageResult BeforeExecution(IInvocation invocation, bool topLevelIntercept)
        {
            var callStackContext = GetCallStackContext();
            callStackContext.Enter(invocation);

            var cacheAttributes = GetCacheAttributes(invocation.MethodInvocationTarget);

            var stageResult = new StageResult{Proceed = true};

            //check if the method has a return value, or is not to be cached
            if (invocation.Method.ReturnType == VoidType || cacheAttributes.NoCache)
                return stageResult;

            string cacheKey = cacheKeyGenerator.GenerateCacheKey(invocation.MethodInvocationTarget, invocation.Arguments);
            stageResult.State = cacheKey;

            //If we could not calculate a cacheKey then we cant cache this model.
            if (string.IsNullOrEmpty(cacheKey))
                return stageResult;

            
#if DEBUG
            // In Debug mode only, check to see if we've disabled caching explicitly from the Cache page.
            if (System.Web.HttpContext.Current != null)
            {
                var app = System.Web.HttpContext.Current.Application;

                var disabledPrefixes = app["DisabledCachePrefixes"] as List<string>;

                if (disabledPrefixes != null && disabledPrefixes.Any(x => cacheKey.StartsWith(x + "$")))
                {
                    stageResult.Proceed = true;
                    return stageResult;
                }
            }
#endif

            if (!TryServeValueFromCache(invocation, stageResult, callStackContext, cacheKey, defaultCacheProvider))
            {
                //If it wasn't in the default cache but it is a group initialized type initialize it. (these are not put in a tertiary cache)
                if (cacheAttributes.CacheInitializerType != null)
                {
                    InitializeCache(invocation, cacheAttributes.CacheInitializerType);

                    // Try to serve the value from cache now that it's been initialized
                    TryServeValueFromCache(invocation, stageResult, callStackContext, cacheKey, defaultCacheProvider);
                }
                else
                {
                    //Look through the cache providers that are not the default.
                    foreach (var cacheProvider in cacheProviders.Where(cacheProvider => cacheProvider != defaultCacheProvider))
                    {
                        //If it has a match for the cache key
                        if (TryServeValueFromCache(invocation, stageResult, callStackContext, cacheKey, cacheProvider))
                        {
                            //Set that on the default cache providers so it can be found there on the next iteration.
                            defaultCacheProvider.Insert(cacheKey, invocation.ReturnValue, Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(cacheExpiryMinutes));
                            //We found it so stop looking through cache providers.
                            break;
                        }
                    }
                }
            }

            return stageResult;
        }

        private bool TryServeValueFromCache(IInvocation invocation, StageResult stageResult,
                                            CacheCallStackContext callStackContext, string cacheKey, ICacheProvider cacheProvider)
        {
            object cacheEntry;

            if (cacheProvider.TryGetCachedObject(cacheKey, out cacheEntry))
            {
                if (cacheEntry == null)
                {
                    invocation.ReturnValue = null;
                }
                else
                {
                    SetReturnValueFromCacheEntry(invocation, cacheEntry);
                }

                stageResult.Proceed = false;

                // We're not going to run the "After" stage, and so we need to process exiting now
                callStackContext.Exit();

                return true;
            }

            return false;
        }


        public void AfterExecution(IInvocation invocation, bool topLevelIntercept, StageResult state)
        {
            CacheCallStackContext callStackContext = null;
            
            try
            {
                var cacheAttributes = GetCacheAttributes(invocation.MethodInvocationTarget);

                if (invocation.Method.ReturnType == VoidType || cacheAttributes.NoCache)
                    return;

                if (cacheEnabled && state.Proceed)
                {
                    callStackContext = GetCallStackContext();

                    // Can the result be cached in the current context?
                    if (!callStackContext.IsResultCacheable(invocation, cacheAttributes))
                        return;

                    var cacheKey = state.State as string;

                    if (string.IsNullOrEmpty(cacheKey))
                        cacheKey = cacheKeyGenerator.GenerateCacheKey(invocation.MethodInvocationTarget, invocation.Arguments);

                    // If we cant calculate a cacheKey then we can't cache this model.
                    if (string.IsNullOrEmpty(cacheKey))
                        return;

                    if (invocation.ReturnValue != null)
                    {
                        // Create the value to cache
                        var cacheValue = GetValueToCache(invocation, cacheAttributes);

                        if (cacheAttributes.AbsoluteExpirationInSecondsPastMidnight != null)
                        {
                            DateTime cacheEntryExpiration = DateTime.Today.AddSeconds(cacheAttributes.AbsoluteExpirationInSecondsPastMidnight.Value);

                            if (cacheEntryExpiration <= DateTime.Now)
                                cacheEntryExpiration = cacheEntryExpiration.AddDays(1);  // Expire the cache entry in the future

                            // Absolute expiration : Only set this in the default cache.
                            defaultCacheProvider.Insert(cacheKey, cacheValue, cacheEntryExpiration, Cache.NoSlidingExpiration); 
                        }
                        else
                        {
                            // Sliding expiration
                            InsertValueIntoAllCacheProviders(cacheKey, cacheValue, Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(cacheExpiryMinutes)); 
                        }
                    }
                    else
                        InsertValueIntoAllCacheProviders(cacheKey, null, Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(cacheExpiryMinutes));
                }
            }
            finally
            {
                (callStackContext ?? GetCallStackContext())
                    .Exit();
            }
        }

        private void InsertValueIntoAllCacheProviders(string cacheKey, object cacheValue, DateTime absoluteExpiration, TimeSpan slidingDuration)
        {
            foreach (var cacheProvider in cacheProviders)
            {
                cacheProvider.Insert(cacheKey, cacheValue, absoluteExpiration, slidingDuration);
            }
        }

        private Dictionary<MethodInfo, LazyCacheAttributes> cacheAttributesByMethodInfo = new Dictionary<MethodInfo, LazyCacheAttributes>();

        private LazyCacheAttributes GetCacheAttributes(MethodInfo invocationTarget)
        {
            LazyCacheAttributes result;

            if (!cacheAttributesByMethodInfo.TryGetValue(invocationTarget, out result))
            {
                result = new LazyCacheAttributes(invocationTarget.GetCustomAttributes(true));
                cacheAttributesByMethodInfo[invocationTarget] = result;
            }

            return result;
        }

        private static CacheCallStackContext GetCallStackContext()
        {
            var callStackContext = CallContext.GetData(CallStackContextKey) as CacheCallStackContext;
            
            if (callStackContext == null)
            {
                callStackContext = new CacheCallStackContext();
                CallContext.SetData(CallStackContextKey, callStackContext);
            }

            return callStackContext;
        }

        private void InitializeCache(IInvocation invocation, Type cacheInitializerType)
        {
            // This needs to use the service locator pattern because cache initializer types are discovered 
            // dynamically from attributes applied to the service methods being intercepted
            var initializer = serviceLocator.Resolve(cacheInitializerType) as ICacheInitializer;

            if (initializer == null)
                throw new ArgumentException("'{0}' is not an implementation of ICacheInitializer.", "cacheInitializerType");
            
            //Currently, Only the default cache provider is used for initialized types. To avoid making the 
            initializer.InitializeCacheValues(defaultCacheProvider, invocation.MethodInvocationTarget, invocation.Arguments);
        }

        private object GetValueToCache(IInvocation invocation, LazyCacheAttributes cacheAttributes)
        {
            object cacheValue;

            if (cacheAttributes.CopyOnSet || cacheAttributes.CopyOnGet)
            {
                cacheValue = serializer.Serialize(invocation.ReturnValue);
            }
            else
            {
                cacheValue = invocation.ReturnValue;
            }

            return cacheValue;
        }

        private void SetReturnValueFromCacheEntry(IInvocation invocation, object cacheEntry)
        {
            byte[] data = cacheEntry as byte[];

            if (data != null)
                invocation.ReturnValue = serializer.Deserialize(data, invocation.Method.ReturnType);
            else
                invocation.ReturnValue = cacheEntry;
        }
    }
}