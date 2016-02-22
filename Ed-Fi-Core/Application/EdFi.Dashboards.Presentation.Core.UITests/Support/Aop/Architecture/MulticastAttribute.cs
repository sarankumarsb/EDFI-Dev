#if !POSTSHARP

using System;

namespace EdFi.Dashboards.Presentation.Core.UITests.Support.Aop.Architecture
{
    public abstract class MulticastAttribute : Attribute
    {
        public MulticastAttribute()
        {
            AspectPriority = int.MaxValue;
        }

        /// <summary>
        /// Gets or sets the namespace pattern identifying the types to which the aspect should apply.
        /// </summary>
        public string AttributeTargetTypes { get; set; }

        /// <summary>
        /// Gets or sets values identifying characteristics of the types to which the aspect should apply.
        /// </summary>
        public MulticastAttributes AttributeTargetTypeAttributes { get; set; }

        /// <summary>
        /// Gets or sets a wildcard or regular expression identifying the methods to which the aspect should apply.
        /// </summary>
        public string AttributeTargetMembers { get; set; }
 
        /// <summary>
        /// Gets or sets values identifying characteristics of the members to which the aspect should apply.
        /// </summary>
        public MulticastAttributes AttributeTargetMemberAttributes { get; set; }

        /// <summary>
        /// Gets or sets the priority in which the aspect should be applied.
        /// </summary>
        public int AspectPriority { get; set; }
    }
}

#endif