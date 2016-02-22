// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;

namespace EdFi.Dashboards.Infrastructure.Implementations.Caching
{
    /// <summary>
    /// Indicates that unless otherwise specified, the results of all the class' public methods should be cached.
    /// </summary>
    /// <remarks>This attribute was added to support providers that otherwise wouldn't be cached (i.e. in the EdFi codebase, only services are cached by default).
    /// The Castle registration code must still look for this property in order to know whether to wire up a cache interceptor with a given component.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class CacheAllMethods : Attribute { }

    [AttributeUsage(AttributeTargets.Method)]
    public sealed class CacheBehaviorAttribute : Attribute
    {
        /// <summary>
        /// Gets the time of absolute expiration for the cache entry, in seconds past midnight.
        /// </summary>
        public int? AbsoluteExpirationInSecondsPastMidnight { get; private set; }

        /// <summary>
        /// Indicates whether the object/value being cached should be copied when it's insert (storing a unmodified copy of the object in the event the object is subsequently modified by the current call).
        /// </summary>
        public bool CopyOnSet { get; private set; }

        /// <summary>
        /// Indicates whether the object/value being cached should be copied when it's retrieved (leaving the cached copy unmodified in the event the retrieved object is subsequently modified).
        /// </summary>
        public bool CopyOnGet { get; private set; }

        /// <summary>
        /// Holds the static default instance of the <see cref="CacheBehaviorAttribute"/> class.
        /// </summary>
        private static CacheBehaviorAttribute _default = new CacheBehaviorAttribute();

        /// <summary>
        /// Gets the static default instance of the <see cref="CacheBehaviorAttribute"/> class.
        /// </summary>
        public static CacheBehaviorAttribute Default
        {
            get { return _default; }
        }

        /// <summary>
        /// Indicates the method result should be cached for the configured duration, should be copied on both insertion and retrieval from the cache.
        /// </summary>
        public CacheBehaviorAttribute() : this(true, true) { }

        /// <summary>
        /// Indicates the method result should be cached using the configured sliding expiration duration, and whether it should be copied on insertion and/or retrieval from the cache.
        /// </summary>
        /// <param name="copyOnSet">Indicates whether the object/value being cached should be copied when it's insert (storing a unmodified copy of the object in the event the object is subsequently modified by the current call).</param>
        /// <param name="copyOnGet">Indicates whether the object/value being cached should be copied when it's retrieved (leaving the cached copy unmodified in the event the retrieved object is subsequently modified).</param>
        public CacheBehaviorAttribute(bool copyOnSet, bool copyOnGet)
        {
            CopyOnSet = copyOnSet;
            CopyOnGet = copyOnGet;
        }

        /// <summary>
        /// Indicates the method result should be cached for the specified sliding duration, and whether it should be copied on insertion and/or retrieval from the cache.
        /// </summary>
        /// <param name="copyOnSet">Indicates whether the object/value being cached should be copied when it's insert (storing a unmodified copy of the object in the event the object is subsequently modified by the current call).</param>
        /// <param name="copyOnGet">Indicates whether the object/value being cached should be copied when it's retrieved (leaving the cached copy unmodified in the event the retrieved object is subsequently modified).</param>
        /// <param name="absoluteExpirationInSecondsPastMidnight">The number of seconds past midnight when the cache entry should be expired.</param>
        public CacheBehaviorAttribute(bool copyOnGet, bool copyOnSet, int absoluteExpirationInSecondsPastMidnight)
        {
            CopyOnGet = copyOnGet;
            CopyOnSet = copyOnSet;
            AbsoluteExpirationInSecondsPastMidnight = absoluteExpirationInSecondsPastMidnight;
        }
    }

    /// <summary>
    /// Indicates the method's results should not be cached.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class NoCacheAttribute : Attribute { }

    /// <summary>
    /// Indicates the method's results are always safe to cache because the result is not affected by any security-related filtering.
    /// </summary>
    /// <remarks>This attribute was added originally for the scenario with the InformationService for School.  The service calls the domain metric service to obtain metrics related
    /// to the graduation plan.  When invoked by a teacher looking at a school page, this results in the metric model being filtered for actions that are not available to teachers.
    /// However, the contents of the School's InformationModel are unaffected, and therefore that step should not invalidate the ability to cache that model.  This attributes gives
    /// the developer the ability to "assert" in to caching in spite of filtering activities that occur downstream that are innocuous in this specific context.</remarks>
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class AlwaysSafeToCacheAttribute : Attribute { }
}
