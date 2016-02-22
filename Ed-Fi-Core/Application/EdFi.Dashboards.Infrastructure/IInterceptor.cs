// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using Castle.DynamicProxy;
using EdFi.Dashboards.Common;

namespace EdFi.Dashboards.Infrastructure
{
    /// <summary>
    /// Provides methods for implementing before and after steps around method invocations.
    /// </summary>
    public interface IInterceptorStage
    {
        /// <summary>
        /// Provides an opportunity for an interceptor to execute logic before a method invocation, and
        /// optionally prevent the call from being processed by the intended target component.
        /// </summary>
        /// <param name="invocation">Metadata related to the method invocation being intercepted.</param>
        /// <param name="topLevelIntercept">Indicates whether this method invocation is the outermost call in the call stack.</param>
        /// <returns>A <see cref="StageResult"/> indicating whether or not to proceed, with an optional attached state.</returns>
        StageResult BeforeExecution(IInvocation invocation, bool topLevelIntercept);
        
        /// <summary>
        /// Provides an opportunity for an interceptor to execute logic after the target method invocation
        /// is complete (this method will not be executed if the <see cref="BeforeExecution"/> prevents the call from proceeding).
        /// </summary>
        /// <param name="invocation">Metadata related to the method invocation being intercepted.</param>
        /// <param name="topLevelIntercept">Indicates whether this method invocation is the outermost call in the call stack.</param>
        /// <param name="state">The StageResult instance returned by the <see cref="BeforeExecution"/> method of the interceptor.</param>
        void AfterExecution(IInvocation invocation, bool topLevelIntercept, StageResult state);
    }

    /// <summary>
    /// Indicates whether to proceed with a method invocation (and optionally carries state).
    /// </summary>
    public class StageResult
    {
        /// <summary>
        /// Indicates whether the method invocation should be allowed to proceed.
        /// </summary>
        public virtual bool Proceed { get; set; }

        /// <summary>
        /// Gets or sets state associated with the invocation.
        /// </summary>
        public virtual object State { get; set; }
    }

    /// <summary>
    /// Provides behavior specific to interceptors that filter results.
    /// </summary>
    public abstract class FilteringInterceptorBase
    {
        public const string ResultProcessedForModificationKey = "StageInterceptor.ResultProcessedForModification";

        /// <summary>
        /// Indicates that a return model being processed by the current call stack may have been modified based on user permissions, and therefore any derived models should not be cached.
        /// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification="Protected methods should not be marked static due to their polymorphic behavior.")]
		protected void MarkCurrentResultProcessedForModification()
        {
            CallContext.SetData(ResultProcessedForModificationKey, true);
        }
    }

    public class StageInterceptor : IInterceptor
    {
        public StageInterceptor(Lazy<IInterceptorStage>[] stages)
        {
            this.stages = stages;
        }

        private const string callContextName = "StageInterceptor";

        private readonly Lazy<IInterceptorStage>[] stages;

        public void Intercept(IInvocation invocation)
        {
            if (stages.Length == 0)
                throw new UserAccessDeniedException( "Stage Interceptor has not been configured with stages by the Inversion of Control configuration.");

            var stageStates = new StageResult[stages.Length];
            bool topLevelIntercept = IsOuterLayerCall(GetType().GetMethod("Intercept"));

            try
            {
                bool proceed = true;
                int i = 0;
                for (; i < stages.Length; i++)
                {
                    StageResult state = stages[i].Value.BeforeExecution(invocation, topLevelIntercept);
                    stageStates[i] = state;
                    if (!state.Proceed)
                    {
                        proceed = false;
                        break;
                    }
                }

                if (proceed)
                    invocation.Proceed();

                for (int j = i - 1; j >= 0; j--)
                {
                    if (j >= stages.Length)
                        continue;
                    stages[j].Value.AfterExecution(invocation, topLevelIntercept, stageStates[j]);
                }

            }
            finally
            {
                ClearLayer(topLevelIntercept);
            }

        }

        public static bool IsOuterLayerCall(MethodBase methodBase)
        {
            var callContext = CallContext.GetData(callContextName);
            if (callContext != null)
                return false;

            CallContext.SetData(callContextName, true);
            return true;
        }

        public static void ClearLayer(bool isTopLevel)
        {
            if (isTopLevel)
                CallContext.FreeNamedDataSlot(callContextName);
        }
    }
}
