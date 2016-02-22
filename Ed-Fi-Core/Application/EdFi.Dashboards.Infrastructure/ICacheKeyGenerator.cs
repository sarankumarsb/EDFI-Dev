using System.Reflection;

namespace EdFi.Dashboards.Infrastructure
{
    /// <summary>
    /// Defines a method for generating a cache key for a method invocation.
    /// </summary>
    public interface ICacheKeyGenerator
    {
        /// <summary>
        /// Generates a unique cache key for a method invocation, based on the arguments provided.
        /// </summary>
        string GenerateCacheKey(MethodInfo methodInvocationTarget, object[] arguments);
    }
}
