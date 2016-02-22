using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EdFi.Dashboards.Resources.InitializationActivities
{
    [AttributeUsage(AttributeTargets.All)]
    public class InitializationActivityDependencyAttribute : System.Attribute
    {
        public Type[] DependentTypes { get; set; }
        public InitializationActivityDependencyAttribute(params Type[] dependentTypes)
        {
            DependentTypes = dependentTypes;
        }
    }
}
