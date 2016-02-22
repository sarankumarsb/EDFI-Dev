using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EdFi.Dashboards.Infrastructure
{
    /// <summary>
    /// Defines a method for creating a cache key for a particular provider's primary method's result 
    /// (used to decouple the functionality of creating a cache key for the result of a provider 
    /// from the implementation, usually a controller, that initially stores it).
    /// </summary>
    public interface IHasCachedResult
    {
        /// <summary>
        /// Gets the cache key for the provider result, given the specified arguments appropriate 
        /// to the implementation.
        /// </summary>
        /// <param name="arguments">The arguments to be used to construct the cache key (must match
        /// the provider method signature for which this is implemented).</param>
        /// <returns>The cache key for use in manipulating the cache.</returns>
        string GetCacheKey(params object[] arguments);
    }
}
