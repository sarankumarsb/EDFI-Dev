using System;
using System.Linq;

namespace EdFi.Dashboards.Infrastructure.Implementations.Caching
{
    /// <summary>
    /// Provides lazy-loaded access to attributes on invoked methods for performance reasons.
    /// </summary>
    public class LazyCacheAttributes
    {
        private readonly object[] attributes;

        public LazyCacheAttributes(object[] attributes)
        {
            if (attributes.Length == 0)
            {
                _noCache = false;
                _hasCacheBehaviorAttribute = false;
                _cacheBehaviorAttribute = CacheBehaviorAttribute.Default; // Default behavior
                _hasCacheInitializer = false;
            }

            this.attributes = attributes;
        }

        private bool? _noCache;
            
        public bool NoCache
        {
            get
            {
                if (_noCache == null)
                    _noCache = attributes.Any(o => o is NoCacheAttribute);

                return _noCache.Value;
            }
        }

        public bool CopyOnGet
        {
            get { return CacheBehavior.CopyOnGet; }
        }

        public bool CopyOnSet
        {
            get { return CacheBehavior.CopyOnSet; }
        }

        public bool IsAlwaysSafeToCache
        {
            get { return AlwaysSafeToCache != null; }
        }

        public int? AbsoluteExpirationInSecondsPastMidnight
        {
            get { return CacheBehavior.AbsoluteExpirationInSecondsPastMidnight; }
        }

        private bool? _hasCacheBehaviorAttribute;
        private CacheBehaviorAttribute _cacheBehaviorAttribute;

        private CacheBehaviorAttribute CacheBehavior
        {
            get
            {
                if (_hasCacheBehaviorAttribute == null)
                {
                    var cacheBehavior = attributes.OfType<CacheBehaviorAttribute>().SingleOrDefault();

                    if (cacheBehavior == null)
                    {
                        _hasCacheBehaviorAttribute = false;
                        _cacheBehaviorAttribute = CacheBehaviorAttribute.Default; // Default behavior
                    }
                    else
                    {
                        _hasCacheBehaviorAttribute = true;
                        _cacheBehaviorAttribute = cacheBehavior;
                    }
                }

                return _cacheBehaviorAttribute;
            }
        }

        private bool? _hasCacheInitializer;
        private Type _cacheInitializerType;
            
        public Type CacheInitializerType
        {
            get
            {
                if (_hasCacheInitializer == null)
                {
                    var cacheInitializer = attributes.OfType<CacheInitializerAttribute>().SingleOrDefault();

                    if (cacheInitializer == null)
                        _hasCacheInitializer = false;
                    else
                    {
                        _hasCacheInitializer = true;
                        _cacheInitializerType = cacheInitializer.CacheInitializerType;
                    }
                }

                return _cacheInitializerType;
            }
        }

        private bool? _hasAlwaysSafeToCacheAttribute;
        private AlwaysSafeToCacheAttribute _alwaysSafeToCacheAttribute;

        private AlwaysSafeToCacheAttribute AlwaysSafeToCache
        {
            get
            {
                if (_hasAlwaysSafeToCacheAttribute == null)
                {
                    var alwaysSafeToCache = attributes.OfType<AlwaysSafeToCacheAttribute>().SingleOrDefault();

                    if (alwaysSafeToCache == null)
                    {
                        _hasAlwaysSafeToCacheAttribute = false;
                    }
                    else
                    {
                        _hasAlwaysSafeToCacheAttribute = true;
                        _alwaysSafeToCacheAttribute = alwaysSafeToCache;
                    }
                }

                return _alwaysSafeToCacheAttribute;
            }
        }
    }
}