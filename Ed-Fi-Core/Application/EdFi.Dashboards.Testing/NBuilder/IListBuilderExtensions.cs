using FizzWare.NBuilder;
using FizzWare.NBuilder.Implementation;

namespace EdFi.Dashboards.Testing.NBuilder
{
    public static class IListBuilderExtensions
    {
        public static IListBuilder<T> WithNonRepeatingValues<T>(this IListBuilder<T> builder)
        {
            BuilderSetup.SetDefaultPropertyNamer(new NonDefaultNonRepeatingPropertyNamer(new ReflectionUtil()));

            return builder;
        }
    }
}
