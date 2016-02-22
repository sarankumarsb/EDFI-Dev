// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
namespace EdFi.Dashboards.Resources.Security.Common
{
    /// <summary>
    /// Provides shared security-related constants.
    /// </summary>
    public static class SecurityGlobals
    {
        /// <summary>
        /// Gets the key to use when registering the domain service interceptor with Castle Windsor container.
        /// </summary>
        public const string DomainServiceStageInterceptorKey = "domainServiceStageInterceptor";
        public const string CacheOnlyStageInterceptorKey = "cacheOnlyStageInterceptor";
    }
}
