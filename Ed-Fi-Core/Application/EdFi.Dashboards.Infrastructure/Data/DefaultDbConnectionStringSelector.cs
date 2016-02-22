// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System.Configuration;

namespace EdFi.Dashboards.Infrastructure.Data
{
    /// <summary>
    /// Provides the first (default) connection string found in the configuration file.
    /// </summary>
    public class DefaultDbConnectionStringSelector : IDbConnectionStringSelector
    {
        private string connectionStringName;

        /// <summary>
        /// Gets the name of the first connection string found in the configuration file.
        /// </summary>
        /// <returns>The name of connection string.</returns>
        public string GetConnectionStringName()
        {
            if (connectionStringName == null)
            {
                if (ConfigurationManager.ConnectionStrings.Count == 0)
                    throw new ConfigurationErrorsException("No connection strings were found in the configuration file.");

                connectionStringName = ConfigurationManager.ConnectionStrings[0].Name;
            }

            return connectionStringName;
        }
    }


    /* How to use:
       1) Configure the "ConfigurationSpecificInstallerBase" both web app and STS.
       protected virtual void RegisterIDbConnectionStringSelector(IWindsorContainer container)
       {
            
            container.Register(Component
                .For<IDbConnectionStringSelector>()
                .Instance(new NamedDbConnectionStringSelector("MultiLEA")));
      
            container.Register(Component
                 .For<IDbConnectionStringSelector>()
                 .ImplementedBy<DefaultDbConnectionStringSelector>().Named(defaultDatabaseSelector));
       }
     
       2)Modify both web configs Web app and STS.
       <add name="MultiLEA"    connectionString="Database=DashboardIntegration_MultiLEA; Data Source=(local); Persist Security Info=True; User Id=edfiPService; Password=edfiPService;"        providerName="System.Data.SqlClient"/>
     
     */
    public class NamedDbConnectionStringSelector : IDbConnectionStringSelector
    {
        private readonly string connectionStringName;

        public NamedDbConnectionStringSelector(string connectionStringName)
        {
            this.connectionStringName = connectionStringName;
        }

        public string GetConnectionStringName()
        {
            return connectionStringName;
        }
    }
}