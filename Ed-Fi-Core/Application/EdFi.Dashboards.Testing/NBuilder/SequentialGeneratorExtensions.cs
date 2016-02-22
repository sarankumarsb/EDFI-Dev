using System;
using FizzWare.NBuilder;

namespace EdFi.Dashboards.Testing.NBuilder
{
    public static class SequentialGeneratorExtensions
    {
        public static SequentialGenerator<T> StartingAt<T>(this SequentialGenerator<T> instance, T value)
            where T : struct, IConvertible
        {
            instance.StartingWith(value);

            return instance;
        }
    }
}
