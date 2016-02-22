using System;
using EdFi.Dashboards.Resources.Navigation;

namespace EdFi.Dashboards.Presentation.Core.Tests.Routing.Support.Mvc
{
    /// <summary>
    /// Provides names (that correspond to Ids) that are required for route generation (i.e. John-X-Doe-12345, 
    /// where 12345 is the Student USI).
    /// </summary>
    public class TestRouteValueProvider : IRouteValueProvider
    {
        public bool CanProvideRouteValue(string key, Func<string, object> getValue)
        {
            return true;
        }

        public void ProvideRouteValue(string key, Func<string, object> getValue, Action<string, object> setValue)
        {
            switch (key)
            {
                case "localEducationAgency":
                    setValue("localEducationAgency", TestName.LocalEducationAgency);
                    break;
                case "metricName":
                    setValue("metricName", TestName.Metric);
                    break;
                case "school":
                    setValue("school", TestName.School);
                    break;
                case "staff":
                    setValue("staff", TestName.Staff);
                    break;
            }
        }
    }
}
