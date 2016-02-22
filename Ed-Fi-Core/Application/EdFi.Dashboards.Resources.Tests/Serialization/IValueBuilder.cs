using System;

namespace EdFi.Dashboards.Resources.Tests.Serialization
{
    public interface IValueBuilder
    {
        bool TryBuild(string logicalPropertyPath, Type targetType, string context, out object value);
        void Reset();
        TestObjectFactory Factory { get; set; }
    }
}