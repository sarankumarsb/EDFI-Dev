// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System;
using System.Globalization;
using System.Web.Mvc;
using EdFi.Dashboards.Resources.LocalEducationAgency;

namespace EdFi.Dashboards.Presentation.Architecture.Mvc.ValueProviders
{
    public class LocalEducationAgencyIdValueProvider : IValueProvider
    {
        private readonly string localEducationAgency;
        private readonly IRouteValueResolutionService localEducationAgencyRouteValueResolutionService;

        public LocalEducationAgencyIdValueProvider(string localEducationAgency, IRouteValueResolutionService localEducationAgencyRouteValueResolutionService)
        {
            this.localEducationAgency = localEducationAgency;
            this.localEducationAgencyRouteValueResolutionService = localEducationAgencyRouteValueResolutionService;
        }

        public bool ContainsPrefix(string prefix)
        {
            if (prefix.Equals("LocalEducationAgencyId", StringComparison.OrdinalIgnoreCase))
            {
                return localEducationAgency != null;
            }

            return false;
        }

        public ValueProviderResult GetValue(string key)
        {
            // Quit if it's not applicable
            if (!ContainsPrefix(key))
                return null;

            string attemptedValue = localEducationAgency;

            int localEducationAgencyId = localEducationAgencyRouteValueResolutionService.Get(RouteValueResolutionRequest.Create(attemptedValue));

            if (localEducationAgencyId == 0)
                return null;

            return new ValueProviderResult(localEducationAgencyId, attemptedValue, CultureInfo.CurrentCulture);
        }
    }

    public class LocalEducationAgencyNotFoundException : System.Exception
    {
        public LocalEducationAgencyNotFoundException(string msg) : base(msg)
        {
                
        }
    }
}