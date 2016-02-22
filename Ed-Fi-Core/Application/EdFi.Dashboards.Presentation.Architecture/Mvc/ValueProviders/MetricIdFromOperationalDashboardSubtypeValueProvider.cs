// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System;
using System.Globalization;
using System.Web.Mvc;
using EdFi.Dashboards.Core;
using EdFi.Dashboards.Resources.Metric;

namespace EdFi.Dashboards.Presentation.Architecture.Mvc.ValueProviders
{
    public class MetricIdFromOperationalDashboardSubtypeValueProvider : IValueProvider
    {
        private readonly ControllerContext controllerContext;
        private readonly IDomainSpecificMetricNodeResolver domainSpecificMetricNodeResolver;
        private readonly IValueProvider schoolIdValueProvider;

        public MetricIdFromOperationalDashboardSubtypeValueProvider(ControllerContext controllerContext, IDomainSpecificMetricNodeResolver domainSpecificMetricNodeResolver, 
            IValueProvider schoolIdValueProvider)
        {
            this.controllerContext = controllerContext;
            this.domainSpecificMetricNodeResolver = domainSpecificMetricNodeResolver;
            this.schoolIdValueProvider = schoolIdValueProvider;
        }

        public bool ContainsPrefix(string prefix)
        {
            if (prefix.Equals("MetricVariantId", StringComparison.OrdinalIgnoreCase))
                return controllerContext.RouteData.Values.ContainsKey("operationalDashboardSubtype");

            return false;
        }

        public ValueProviderResult GetValue(string key)
        {
            // Quit if it's not applicable
            if (!ContainsPrefix(key))
                return null;

            string attemptedValue = controllerContext.RouteData.Values["operationalDashboardSubtype"].ToString();
            string area = controllerContext.RouteData.DataTokens["area"].ToString();

            int metricVariantId = 0;
            MetricInstanceSetType metricInstanceSetType;

            if (Enum.TryParse(area, true, out metricInstanceSetType))
            {
                switch (metricInstanceSetType)
                {
                    case MetricInstanceSetType.LocalEducationAgency:
                        metricVariantId = domainSpecificMetricNodeResolver.GetOperationalDashboardMetricNode(MetricInstanceSetType.LocalEducationAgency).MetricVariantId;
                        break;

                    case MetricInstanceSetType.School:
                        // Get schoolId from route
                        var schoolIdValue = schoolIdValueProvider.GetValue("schoolId");
                        int schoolId = Convert.ToInt32(schoolIdValue.RawValue);

                        if (schoolId == 0)
                            throw new Exception("Unable to get school Id for resolving operational dashboard subtype from the route.");

                        // Get metricId from context
                        metricVariantId = domainSpecificMetricNodeResolver.GetOperationalDashboardMetricNode(MetricInstanceSetType.School, schoolId).MetricVariantId;
                        break;

                    default:
                        throw new Exception(
                            string.Format("Translation of operationalDashboardSubtype to metric variant Id is not supported for area '{0}'.", metricInstanceSetType));
                }
            }

            if (metricVariantId == 0)
                throw new Exception(
                    string.Format("Unable to identify operational dashboard metric variant Id for subtype '{0}'.", attemptedValue));

            return new ValueProviderResult(metricVariantId, attemptedValue, CultureInfo.CurrentCulture);
        }
    }
}