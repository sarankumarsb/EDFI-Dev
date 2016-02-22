// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System;
using System.Globalization;
using System.Web.Mvc;
using EdFi.Dashboards.Resources.School;

namespace EdFi.Dashboards.Presentation.Architecture.Mvc.ValueProviders
{
    public class SchoolIdValueProvider : IValueProvider
    {
        private readonly ControllerContext controllerContext;
        private readonly IRouteValueResolutionService schoolRouteValueResolutionService;
        private readonly IValueProvider localEducationAgencyIdValueProvider;

        public SchoolIdValueProvider(ControllerContext controllerContext, IRouteValueResolutionService schoolRouteValueResolutionService,
            IValueProvider localEducationAgencyIdValueProvider)
        {
            this.controllerContext = controllerContext;
            this.schoolRouteValueResolutionService = schoolRouteValueResolutionService;
            this.localEducationAgencyIdValueProvider = localEducationAgencyIdValueProvider;
        }

        public bool ContainsPrefix(string prefix)
        {
            if (prefix.Equals("SchoolId", StringComparison.OrdinalIgnoreCase))
                return controllerContext.RouteData.Values.ContainsKey("school");

            return false;
        }

        public ValueProviderResult GetValue(string key)
        {
            // Quit if it's not applicable
            if (!ContainsPrefix(key))
                return null;

            string attemptedValue = controllerContext.RouteData.Values["school"].ToString();

            var localEducationAgencyValue = localEducationAgencyIdValueProvider.GetValue("localEducationAgencyId");
            int localEducationAgencyId = Convert.ToInt32(localEducationAgencyValue.RawValue); //controllerContext.RouteData.Values["localEducationAgencyId"]);

            int schoolId = schoolRouteValueResolutionService.Get(RouteValueResolutionRequest.Create(localEducationAgencyId, attemptedValue));

            if (schoolId == 0)
                throw new Exception(
                    string.Format("Unable to identify school Id for school '{0}' in local education agency '{1}'.", attemptedValue, localEducationAgencyId));

            return new ValueProviderResult(schoolId, attemptedValue, CultureInfo.CurrentCulture);
        }
    }
}