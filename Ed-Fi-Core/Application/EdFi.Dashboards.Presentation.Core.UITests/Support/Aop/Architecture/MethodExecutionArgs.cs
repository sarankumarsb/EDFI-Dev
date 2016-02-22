#if !POSTSHARP

using System;
using System.Reflection;

namespace EdFi.Dashboards.Presentation.Core.UITests.Support.Aop.Architecture
{
    public class MethodExecutionArgs
    {
        public MethodExecutionArgs(MethodInfo methodInfo, object[] args)
        {
            Method = methodInfo;
            Arguments = new Arguments();
            Arguments.AddRange(args);
        }

        /// <summary>
        /// Gets the method being invoked.
        /// </summary>
        public MethodInfo Method { get; private set; }

        /// <summary>
        /// Gets the argument values for the invocation.
        /// </summary>
        public Arguments Arguments { get; private set; }

        /// <summary>
        /// Gets or sets how the execution flow should behave upon exiting the aspect.
        /// </summary>
        public FlowBehavior FlowBehavior { get; set; }

        /// <summary>
        /// Gets or sets the exception currently being thrown.
        /// </summary>
        public Exception Exception { get; set; }
    }
}

#endif