using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using EdFi.Dashboards.Infrastructure;
using EdFi.Dashboards.Resources.Navigation;
using EdFi.Dashboards.Resources.Navigation.Mvc;
using EdFi.Dashboards.Resources.Navigation.Mvc.Areas;

namespace EdFi.Dashboards.Presentation.Core.Tests.Routing.Support
{
    public static class Website
    {
        public static void InitializeLinkGenerators(RouteValuesPreparer routeValuesPreparer, HttpRequestProviderFake httpRequestProviderFake)
        {
            var properties = typeof(Website).GetProperties(BindingFlags.Static | BindingFlags.Public);

            var linkProperties =
                from p in properties
                where typeof(SiteAreaBase).IsAssignableFrom(p.PropertyType)
                select p;

            foreach (var linkProperty in linkProperties)
            {
                // Find available constructors for target property
                var constructor =
                    (from c in linkProperty.PropertyType.GetConstructors()
                     orderby c.GetParameters().Length
                     select new { Method = c, ParameterCount = c.GetParameters().Length })
                     .First();

                // Invoke shortest constructor with null values
                var instance = constructor.Method.Invoke(new object[constructor.ParameterCount]) as SiteAreaBase;

                instance.RouteValuesPreparer = routeValuesPreparer;
                instance.HttpRequestProvider = httpRequestProviderFake;

                linkProperty.SetValue(null, instance, null);
            }
        }

        public static General General { get; set; }
        public static LocalEducationAgency LocalEducationAgency { get; set; }
        public static School School { get; set; }
        public static Staff Staff { get; set; }
        public static StudentSchool StudentSchool { get; set; }
        public static Admin Admin { get; set; }
    }
}
