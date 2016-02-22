// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;
using System.Collections;
using System.Collections.Generic;

namespace EdFi.Dashboards.Metric.Resources.Models
{
    public class TreeOrderNodeEnumerable<T> : IEnumerable<T>
        where T : class, INode<T>
    {
        private readonly IContainerNode<T> containerNode;

        public TreeOrderNodeEnumerable(IContainerNode<T> containerNode)
        {
            this.containerNode = containerNode;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return new TreeOrderNodeEnumerator<T>(containerNode);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public class TreeOrderNodeEnumerator<T> : IEnumerator<T>
        where T : class, INode<T>
    {
        class SharedState
        {
            public int Depth { get; set;}
        }

        private readonly IContainerNode<T> rootContainerNode;
        private IEnumerator<T> enumerator;
        private readonly SharedState state;

        public int Depth
        {
            get { return state.Depth; }
        }

        public TreeOrderNodeEnumerator(IContainerNode<T> rootContainerNode)
            : this(rootContainerNode, new SharedState()) { }

        private TreeOrderNodeEnumerator(IContainerNode<T> rootContainerNode, SharedState state)
        {
            this.rootContainerNode = rootContainerNode;
            this.state = state;
        }

        public T Current
        {
            get
            {
                if (enumerator == null)
                    return null;

                return enumerator.Current;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (enumerator != null)
                {
                    enumerator.Dispose();
                    enumerator = null;
                }
            }
        }

        public bool MoveNext()
        {
            if (enumerator == null)
                enumerator = GetEnumerator();

            return enumerator.MoveNext();
        }

        private IEnumerator<T> GetEnumerator()
        {
            state.Depth += 1;

            foreach (var childNode in rootContainerNode.Children)
            {
                yield return childNode;

                var containerChild = childNode as IContainerNode<T>;

                if (containerChild == null)
                    continue;

                var childEnumerator = new TreeOrderNodeEnumerator<T>(containerChild, state);

                while (childEnumerator.MoveNext())
                    yield return childEnumerator.Current;
            }

            state.Depth -= 1;
        }

        public void Reset()
        {
            enumerator = GetEnumerator();
        }

        object IEnumerator.Current
        {
            get { return enumerator.Current; }
        }
    }
}
