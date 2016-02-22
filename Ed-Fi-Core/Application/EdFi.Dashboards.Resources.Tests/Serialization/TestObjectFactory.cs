using System;
using System.Collections.Generic;

namespace EdFi.Dashboards.Resources.Tests.Serialization
{
    public class TestObjectFactory
    {
        private readonly IValueBuilder[] builders;

        public TestObjectFactory(IValueBuilder[] builders, List<Type> modelTypes)
        {
            CollectionCount = 10;
            MaximumHierarchyDepth = 2;

            ModelTypes = modelTypes;
            this.builders = builders;

            foreach (var builder in builders)
                builder.Factory = this;
        }

        public List<Type> ModelTypes { get; private set; }

        public object Create(string logicalPropertyPath, Type type)
        {
            foreach (var builder in builders)
            {
                object value;

                if (builder.TryBuild(logicalPropertyPath, type, string.Empty, out value))
                    return value;
            }

            throw new Exception(string.Format("Unable to create object of type '{0}'{1}.  Consider adding an IValueBuilder implementation to handle creating this type for serialization testing.", 
                type.FullName, string.IsNullOrEmpty(logicalPropertyPath) ? string.Empty : "at logical path " + logicalPropertyPath));
        }

        /// <summary>
        /// Gets or sets the maximum depth in a hierarchical structure for which data should be generated.
        /// </summary>
        public int MaximumHierarchyDepth { get; set; }

        /// <summary>
        /// Gets or sets the number of items that should be generated in each collection.
        /// </summary>
        public int CollectionCount { get; set; }
    }
}
