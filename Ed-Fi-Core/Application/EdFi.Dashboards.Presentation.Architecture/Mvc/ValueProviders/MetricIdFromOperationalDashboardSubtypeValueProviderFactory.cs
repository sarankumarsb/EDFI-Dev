// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System.Web.Mvc;
using EdFi.Dashboards.Common.Utilities;
using EdFi.Dashboards.Resources.Metric;

namespace EdFi.Dashboards.Presentation.Architecture.Mvc.ValueProviders
{
    public class MetricIdFromOperationalDashboardSubtypeValueProviderFactory : ValueProviderFactory
    {
        private readonly SchoolIdValueProviderFactory schoolIdValueProviderFactory;

        public MetricIdFromOperationalDashboardSubtypeValueProviderFactory(SchoolIdValueProviderFactory schoolIdValueProviderFactory)
        {
            this.schoolIdValueProviderFactory = schoolIdValueProviderFactory;
        }

        public override IValueProvider GetValueProvider(ControllerContext controllerContext)
        {
            return new MetricIdFromOperationalDashboardSubtypeValueProvider(controllerContext, IoC.Resolve<IDomainSpecificMetricNodeResolver>(), 
                schoolIdValueProviderFactory.GetValueProvider(controllerContext));
        }
    }
}