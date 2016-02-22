using System;
using FizzWare.NBuilder;
using FizzWare.NBuilder.Implementation;

namespace EdFi.Dashboards.Testing.NBuilder
{
    public class RangeGenerator<T> : SequentialGenerator<T>
        where T : struct, IConvertible, IComparable<T>
    {
        private readonly T minValue;
        private readonly T maxValue;

        public RangeGenerator(T minValue, T maxValue)
        {
            Guard.Against(minValue.CompareTo(maxValue) >= 0, "minValue must be less than maxValue.");

            this.minValue = minValue;
            this.maxValue = maxValue;
            this.StartingWith(minValue);

            // Determine range value count
            _count = 1;
            Generate();

            while (minValue.CompareTo(Generate()) < 0)
                _count++;

            // Reset generator
            this.StartingWith(minValue);
        }

        public new T Generate()
        {
            T nextValue = base.Generate();

            if (nextValue.CompareTo(maxValue) > 0)
            {
                base.StartingWith(minValue);
                nextValue = base.Generate();
            }

            return nextValue;
        }

        private int _count;

        public int Count
        {
            get { return _count; }
        }
    }
}
