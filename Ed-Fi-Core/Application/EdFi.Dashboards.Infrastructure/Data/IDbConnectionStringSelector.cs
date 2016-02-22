// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
namespace EdFi.Dashboards.Infrastructure.Data
{
    /// <summary>
    /// Provides the name of the connection string to use for database access.
    /// </summary>
    public interface IDbConnectionStringSelector
    {
        /// <summary>
        /// Gets the name of the connection string to use for database access.
        /// </summary>
        /// <returns>The name of the connection string.</returns>
        string GetConnectionStringName();
    }
}