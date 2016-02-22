// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System.Web.Mvc;
using EdFi.Dashboards.Common.Utilities;
using EdFi.Dashboards.Resources.School;

namespace EdFi.Dashboards.Presentation.Architecture.Mvc.ValueProviders
{
    public class SchoolIdValueProviderFactory : ValueProviderFactory
    {
        private readonly LocalEducationAgencyIdValueProviderFactory localEducationAgencyIdValueProviderFactory;

        public SchoolIdValueProviderFactory(LocalEducationAgencyIdValueProviderFactory localEducationAgencyIdValueProviderFactory)
        {
            this.localEducationAgencyIdValueProviderFactory = localEducationAgencyIdValueProviderFactory;
        }

        public override IValueProvider GetValueProvider(ControllerContext controllerContext)
        {
            return new SchoolIdValueProvider(controllerContext, IoC.Resolve<IRouteValueResolutionService>(), 
                localEducationAgencyIdValueProviderFactory.GetValueProvider(controllerContext));
        }
    }
}