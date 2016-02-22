// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
namespace EdFi.Dashboards.Core
{
    /// <summary>
    /// Provides a method for obtaining the code for the local education agency that is in context for the current request.
    /// </summary>
    public interface ILocalEducationAgencyContextProvider
    {
        /// <summary>
        /// Gets the code for the local education agency that is in context for the current request; or <b>null</b> if none was found.
        /// </summary>
        /// <returns>The local education agency code value, used to identify the local education agency for routing, authentication, authorization, configuration, multitenancy, etc.</returns>
        string GetCurrentLocalEducationAgencyCode();
    }
}
