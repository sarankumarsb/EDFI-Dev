// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace EdFi.Dashboards.Common
{
    public abstract class ChainOfResponsibilityBase<TService, TRequest, TResponse>
    {
        protected TService Next { get; set; }

        protected abstract bool CanHandleRequest(TRequest request);
        protected abstract TResponse HandleRequest(TRequest request);

        protected ChainOfResponsibilityBase(TService next)
        {
            this.Next = next;
        }

        private static Dictionary<Type, MethodInfo> methodsByType = new Dictionary<Type, MethodInfo>();

        public virtual TResponse ProcessRequest(TRequest request)
        {
            if (CanHandleRequest(request))
                return HandleRequest(request);

            if (Next != null)
            {
                var nextResponsible = Next as ChainOfResponsibilityBase<TService, TRequest, TResponse>;

                if (nextResponsible != null)
                    return nextResponsible.ProcessRequest(request);

                // Invoke the interface's first method dynamically
                MethodInfo interfaceMethodInfo = GetInterfaceMethodInfo();
                try
                {
                    if (interfaceMethodInfo != null)
                        return (TResponse)interfaceMethodInfo.Invoke(Next, new object[] { request });
                }
                catch (TargetInvocationException  exception)
                {
                    var inner = exception.InnerException;
                    throw inner;
                }
            }

            return default(TResponse);
        }

        private static MethodInfo GetInterfaceMethodInfo()
        {
            MethodInfo methodInfo;
            if (!methodsByType.TryGetValue(typeof(TService), out methodInfo))
            {
                methodInfo =
                    (from m in typeof(TService).GetMethods()
                     let p = m.GetParameters()
                     where m.ReturnType == typeof(TResponse)
                           && p.Count() == 1
                           && p.Single().ParameterType == typeof(TRequest)
                     select m)
                        .SingleOrDefault();

                methodsByType[typeof(TService)] = methodInfo;
            }

            return methodInfo;
        }
    }
}