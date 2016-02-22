// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;
using System.Runtime.Remoting.Messaging;
using Castle.DynamicProxy;
using log4net;

namespace EdFi.Dashboards.Infrastructure.Implementations.Caching
{
    /// <summary>
    /// Captures the context of the call stack for the purposes of preventing items that may have been modified for security reasons from being cached.
    /// </summary>
    public class CacheCallStackContext
    {
        private static readonly ILog errorLog = LogManager.GetLogger("ErrorRollingFile");

        /// <summary>
        /// Stores the most recent invocation in the call context.
        /// </summary>
        /// <remarks>This is used in the <see cref="CacheInterceptor.AfterExecution"/> method to determine if the result can be safely cached.  Services that call other services can only have
        /// their results cached if no other interceptors are invoked that modify the model in their "AfterExecution" methods, or the current service's method is attributed with 
        /// the <see cref="AlwaysSafeToCacheAttribute"/> attribute.</remarks>
        private IInvocation mostRecentlyEnteredInvocation;

        /// <summary>
        /// Stores the current depth of the call stack (used for detecting the exit of a service call context).
        /// </summary>
        private int currentDepth;

        /// <summary>
        /// Gets the current depth of the cache call context.
        /// </summary>
        public int CurrentDepth
        {
            get { return currentDepth; }
        }

        /// <summary>
        /// Indicates whether the current results can be cached based on the current invocation provided (should only be used by <see cref="CacheInterceptor"/> in the <see cref="CacheInterceptor.AfterExecution"/> method.
        /// </summary>
        public bool IsResultCacheable(IInvocation currentInvocation, LazyCacheAttributes cacheAttributes)
        {
            // If the current result may have been modified from its original state, and the current (AfterExecution) invocation resulted in subsequent service calls...
            if (CurrentResultProcessedForModification && OtherServiceCallsMadeFrom(currentInvocation))
            {
                // If method being invoked is NOT marked as always safe to cache (developer has asserted that no sensitive information is being used in a model derived from contained service calls)
                // then we cannot cache the current results
                if (!cacheAttributes.IsAlwaysSafeToCache)
                {
                    string message = string.Format(
@"Result not cached: {0}.{1}
    NOTE: Use the 'AlwaysSafeToCache' attribute on the service method to assert that this derived model is safe to cache from a security perspective.",
                        currentInvocation.InvocationTarget.GetType().FullName,
                        currentInvocation.Method.Name);

                    errorLog.Warn(message);

                    // Result cannot be cached
                    return false;
                }
            }

            // Result is cacheable
            return true;
        }

        private bool OtherServiceCallsMadeFrom(IInvocation currentInvocation)
        {
            // If these invocations don't match, it means that other service calls were made
            return currentInvocation != mostRecentlyEnteredInvocation;
        }

        private bool CurrentResultProcessedForModification
        {
            get
            {
                return Convert.ToBoolean(CallContext.GetData(FilteringInterceptorBase.ResultProcessedForModificationKey));
            }
        }

        /// <summary>
        /// Indicates that we're entering a new depth in the call stack for the caching interceptor.
        /// </summary>
        public void Enter(IInvocation invocation)
        {
            currentDepth++;
            mostRecentlyEnteredInvocation = invocation;
        }

        /// <summary>
        /// Indicates that we're exiting the current depth in the call stack for the caching interceptor.
        /// </summary>
        public void Exit()
        {
            // Decrement the depth
            currentDepth--;

            // Reset context state once we exit the original service call's context, and prevent multiple Exit() calls from causing problems.
            if (currentDepth <= 0)
            {
                // We're done with this call stack, so reset the context values
                CallContext.FreeNamedDataSlot(FilteringInterceptorBase.ResultProcessedForModificationKey);
                currentDepth = 0;
            }
        }
    }
}