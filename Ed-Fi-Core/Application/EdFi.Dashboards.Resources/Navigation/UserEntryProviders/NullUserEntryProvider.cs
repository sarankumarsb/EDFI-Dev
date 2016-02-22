// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using EdFi.Dashboards.Common.Utilities.CastleWindsorInstallers;
using EdFi.Dashboards.SecurityTokenService.Authentication;

namespace EdFi.Dashboards.Resources.Navigation.UserEntryProviders
{
    /// <summary>
    /// Terminates a <see cref="IUserEntryProvider"/> chain of responsibility.
    /// </summary>
    [ChainOfResponsibilityOrder(1000)]
    public class NullUserEntryProvider : IUserEntryProvider
    {
        /// <summary>
        /// Throws an exception describing the unhandled request for a user entry provider.
        /// </summary>
        /// <param name="request">The entry request request providing the domain-specific context for the entry point.</param>
        /// <returns>The URL for the application-specific location for the entry point.</returns>
        public string GetUserEntry(EntryRequest request)
        {
            throw new DashboardsAuthenticationException("No dashboard home page could be determined based on your permissions.  Please contact your administrator.");
        }
    }
}