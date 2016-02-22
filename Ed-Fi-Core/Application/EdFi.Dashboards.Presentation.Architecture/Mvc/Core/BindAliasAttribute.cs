using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EdFi.Dashboards.Presentation.Architecture.Mvc.Core
{
    /// <summary>
    /// Allows you to create aliases that can be used for model properties at
    /// model binding time (i.e. when data comes in from a request).
    /// 
    /// The type needs to be using the DefaultModelBinderWithAliasSupport model binder in 
    /// order for this to work.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
    public class BindAliasAttribute : Attribute
    {
        public BindAliasAttribute(string alias)
        {
            //ommitted: parameter checking
            Alias = alias;
        }
        public string Alias { get; private set; }
    }
}
