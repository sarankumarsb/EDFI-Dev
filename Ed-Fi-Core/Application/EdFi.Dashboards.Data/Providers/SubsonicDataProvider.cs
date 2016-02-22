using EdFi.Dashboards.Infrastructure.Data;
using SubSonic.DataProviders;

namespace EdFi.Dashboards.Data.Providers
{
    //Note: Extensibility point added for extensibility purposes.
    //I.E.: If you needed one connection string per LEA then you could provide your own implementation.
    public interface ISubsonicDataProviderProvider
    {
        IDataProvider GetProvider();
    }

    public class SubsonicDataProviderProvider : ISubsonicDataProviderProvider
    {
        private readonly IDbConnectionStringSelector dbConnectionStringSelector;

        public SubsonicDataProviderProvider(IDbConnectionStringSelector dbConnectionStringSelector)
        {
            this.dbConnectionStringSelector = dbConnectionStringSelector;
        }

        public IDataProvider GetProvider()
        {
            string connectionStringName = dbConnectionStringSelector.GetConnectionStringName();

            return ProviderFactory.GetProvider(connectionStringName);
        }
    }
}
