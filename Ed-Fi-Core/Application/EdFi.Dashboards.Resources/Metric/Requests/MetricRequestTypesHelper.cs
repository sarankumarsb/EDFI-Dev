// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EdFi.Dashboards.Metric.Resources.Services;

namespace EdFi.Dashboards.Resources.Metric.Requests
{
    public static class MetricRequestTypesHelper
    {
        public static List<Type> GetMetricRequestTypes(Assembly assembly)
        {
            // Get all the MetricRequest-derived types
            var requestTypes =
                (from t in assembly.GetTypes()
                 where typeof(MetricInstanceSetRequestBase).IsAssignableFrom(t)
                       && t != typeof(MetricInstanceSetRequestBase)
                 select t).ToList();

            return requestTypes;
        }


        public static IDictionary<string, Type> GetMetricRequestTypesByAreaName(Assembly assembly)
        {
            var metricRequestTypesByAreaName = new Dictionary<string, Type>();

            // Get all the MetricRequest-derived types
            var metricRequestTypes =
                (from t in assembly.GetTypes()
                 where typeof(MetricInstanceSetRequestBase).IsAssignableFrom(t)
                       && t != typeof(MetricInstanceSetRequestBase)
                 select t);

            // Process each one
            foreach (var metricRequestType in metricRequestTypes)
            {
                // TODO: Deferred - GKM - Convert this to use convention by Request's type name prefix?
                var areaAttributes = (MvcAreaAttribute[])metricRequestType.GetCustomAttributes(typeof(MvcAreaAttribute), false);

                // We must have metadata on the request to associate it with a specific area
                if (!areaAttributes.Any())
                {
                    throw new InvalidOperationException(
                        String.Format(
                            "Metric request type '{0}' does not have an Area attribute specifying which area it serves.  Please supply the missing metadata.",
                            metricRequestType.Name));
                }

                foreach (var areaAttribute in areaAttributes)
                {
                    // Make sure area isn't already associated with another metric request type
                    if (metricRequestTypesByAreaName.ContainsKey(areaAttribute.AreaName))
                    {
                        throw new InvalidOperationException(
                            String.Format(
                                "Metric request type '{0}' cannot be associated with area '{1}' because the area has already been associated with metric request type '{2}'.  Check the Area attributes on the metric request type classes and resolve the conflict.",
                                metricRequestType.Name, areaAttribute.AreaName,
                                metricRequestTypesByAreaName[areaAttribute.AreaName]));
                    }

                    // Save the association between the specified area and the metric request type
                    metricRequestTypesByAreaName[areaAttribute.AreaName] = metricRequestType;
                }
            }

            return metricRequestTypesByAreaName;
        }
    }
}
