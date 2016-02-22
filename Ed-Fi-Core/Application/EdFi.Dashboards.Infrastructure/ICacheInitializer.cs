using System.Reflection;

namespace EdFi.Dashboards.Infrastructure
{
    public interface ICacheInitializer
    {
        void InitializeCacheValues(ICacheProvider cacheProvider, MethodInfo methodInvocationTarget, object[] arguments);
    }
}
