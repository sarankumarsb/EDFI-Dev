using System;
using EdFi.Dashboards.Infrastructure;
using EdFi.Dashboards.Metric.Resources.Models;
using EdFi.Dashboards.Resources.Common;
using EdFi.Dashboards.Resources.Navigation.Support;

namespace EdFi.Dashboards.Resources.Metric
{
    public class MetricBaseUserContextApplicator : IUserContextApplicator
    {
        private const string listContext = "listContext";
        private readonly IHttpRequestProvider httpRequestProvider;

        public MetricBaseUserContextApplicator(IHttpRequestProvider httpRequestProvider)
        {
            this.httpRequestProvider = httpRequestProvider;
        }

        public Type ModelType
        {
            get { return typeof (MetricBase); }
        }

        public void ApplyUserContextToModel(object modelAsObject, object requestAsObject)
        {
            var model = modelAsObject as IContainerNode<MetricBase>;

            // Should never happen
            if (model == null)
                return;
            
            var requestListContext = httpRequestProvider.GetValue(listContext);

            // Skip processing if there's no list context to apply
            if (string.IsNullOrEmpty(requestListContext))
                return;

            requestListContext = listContext + "=" + requestListContext;

            // Add the list context to each link in the model
            foreach (var metric in model.DescendantsOrSelf)
                metric.Url = metric.Url.AppendParameters(requestListContext);
        }
    }
}