// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System.Web.Mvc;
using EdFi.Dashboards.Common.Utilities;
using EdFi.Dashboards.Resources.LocalEducationAgency;

namespace EdFi.Dashboards.Presentation.Architecture.Mvc.ValueProviders
{
    public class LocalEducationAgencyIdValueProviderFactory : ValueProviderFactory
    {
        public override IValueProvider GetValueProvider(ControllerContext controllerContext)
        {
            object value;
            string localEducationAgency = null;

            if (controllerContext.RouteData.Values.TryGetValue("localEducationAgency", out value))
                localEducationAgency = value.ToString();

            return new LocalEducationAgencyIdValueProvider(localEducationAgency, IoC.Resolve<IRouteValueResolutionService>());
        }
    }
}