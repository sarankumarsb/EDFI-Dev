// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System.Collections.Generic;

namespace EdFi.Dashboards.Metric.Rendering
{
    public interface IMetricRenderer
    {
        void Render(string templateName, dynamic metricBase, int depth, IDictionary<string, object> viewData);
    }
}
