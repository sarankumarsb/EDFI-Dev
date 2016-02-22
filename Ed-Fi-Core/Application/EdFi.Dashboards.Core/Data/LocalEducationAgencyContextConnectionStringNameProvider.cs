// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;
using System.Data;
using System.Web;
using EdFi.Dashboards.Core;

namespace EdFi.Dashboards.Infrastructure.Data
{
    public class LocalEducationAgencyContextConnectionStringSelector : IDbConnectionStringSelector
    {
        private readonly ILocalEducationAgencyContextProvider localEducationAgencyContextProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalEducationAgencyContextConnectionStringSelector"/> class using the local education context provider.
        /// </summary>
        /// <param name="localEducationAgencyContextProvider">The provider that identifies the local education agency that is in context for the current request.</param>
        public LocalEducationAgencyContextConnectionStringSelector(ILocalEducationAgencyContextProvider localEducationAgencyContextProvider)
        {
            this.localEducationAgencyContextProvider = localEducationAgencyContextProvider;
        }

        /// <summary>
        /// Gets the name of the connection string to use to service the current request (which is also the local education agency code from the context of the current request).
        /// </summary>
        /// <returns>The name of connection string.</returns>
        public string GetConnectionStringName()
        {
            string code = localEducationAgencyContextProvider.GetCurrentLocalEducationAgencyCode();

            if (code == null)
				throw new InvalidOperationException(
                    string.Format("Could not identify local education agency code from HTTP request for '{0}'", HttpContext.Current.Request.Url));

            return code;
        }
    }
}