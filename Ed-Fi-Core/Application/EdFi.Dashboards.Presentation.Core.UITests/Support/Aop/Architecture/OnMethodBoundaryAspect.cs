#if !POSTSHARP

namespace EdFi.Dashboards.Presentation.Core.UITests.Support.Aop.Architecture
{
    public abstract class OnMethodBoundaryAspect : MulticastAttribute
    {
        public virtual void OnEntry(MethodExecutionArgs methodExecutionArgs) { }
        public virtual void OnSuccess(MethodExecutionArgs methodExecutionArgs) { }
        public virtual void OnException(MethodExecutionArgs methodExecutionArgs) { }
        public virtual void OnExit(MethodExecutionArgs methodExecutionArgs) { }
    }
}

#endif