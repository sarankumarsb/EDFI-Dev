using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EdFi.Dashboards.Infrastructure
{
    public class CacheInitializerAttribute : Attribute
    {
        public CacheInitializerAttribute(Type cacheInitializer)
        {
            CacheInitializerType = cacheInitializer;
        }

        public Type CacheInitializerType { get; private set; }
    }
}
