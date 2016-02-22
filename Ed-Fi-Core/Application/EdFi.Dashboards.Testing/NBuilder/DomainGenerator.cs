using System;
using System.Collections.Generic;
using System.Linq;
using FizzWare.NBuilder;

namespace EdFi.Dashboards.Testing.NBuilder
{
    public class DomainGenerator<T> : IGenerator<T>
        where T : struct, IConvertible, IComparable<T>
    {
        private readonly List<T> values;
        private int index = 0;

        public DomainGenerator(params T[] values)
        {
            this.values = values.ToList();
        }

        public T Generate()
        {
            T nextValue = values[index++];

            if (index >= values.Count())
                index = 0;

            return nextValue;
        }

        public int Count
        {
            get { return values.Count; }
        }
    }
}
