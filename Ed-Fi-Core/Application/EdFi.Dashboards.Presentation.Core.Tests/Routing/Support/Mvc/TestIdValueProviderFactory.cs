using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace EdFi.Dashboards.Presentation.Core.Tests.Routing.Support.Mvc
{
    public class TestIdValueProviderFactory : ValueProviderFactory
    {
        // Add route pattern keys here that can be handled by the "Id" convention
        // TODO: Try basing this on TestId fields?
        private string[] handledParameters =
            new[]
                {
                    "localEducationAgency",
                    "school",
                    "staff",
                    "student",
                    "operationalDashboardSubtype",
                };

        public override IValueProvider GetValueProvider(ControllerContext controllerContext)
        {
            object valueAsObject;

            var routeDataValuesByKey = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            foreach (string parameter in handledParameters)
            {
                if (controllerContext.RouteData.Values.TryGetValue(parameter, out valueAsObject))
                {
                    string key = parameter;
                    string value = valueAsObject.ToString();

                    routeDataValuesByKey[key] = value;
                }
            }

            return new TestIdValueProvider(routeDataValuesByKey);
        }
    }
}