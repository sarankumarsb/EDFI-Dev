// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System.Collections.Generic;

namespace EdFi.Dashboards.Metric.Resources.Models
{
    public interface INode<T>
        where T : class
    {
        T Parent { get; set; }
    }

    public interface IContainerNode<T> : INode<T>
        where T : class, INode<T>
    {
        IEnumerable<T> Children { get; set; }
        IEnumerable<T> Descendants { get; }
        IEnumerable<T> DescendantsOrSelf { get; }
    }
}
