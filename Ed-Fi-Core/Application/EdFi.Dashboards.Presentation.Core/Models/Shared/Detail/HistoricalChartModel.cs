using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EdFi.Dashboards.Presentation.Core.Models.Shared.Detail
{
    public class HistoricalChartModel
    {
        public HistoricalChartModel(int localEducationAgencyId, int metricVariantId, string webServiceUrl, string webServiceMethod, string parametersToSendToWebService, string actionTitle)
        {
            LocalEducationAgencyId = localEducationAgencyId;
            MetricVariantId = metricVariantId;
            WebServiceUrl = webServiceUrl;
            WebServiceMethod = webServiceMethod;
            ParametersToSendToWebService = parametersToSendToWebService;
            ActionTitle = actionTitle;
        }

        public HistoricalChartModel(int localEducationAgencyId, int metricVariantId, string actionTitle, object jsonChartModel)
        {
            LocalEducationAgencyId = localEducationAgencyId;
            MetricVariantId = metricVariantId;
            WebServiceUrl = null;
            WebServiceMethod = null;
            ParametersToSendToWebService = null;
            ActionTitle = actionTitle;
            JsonChartModel = jsonChartModel;
        }

        public int LocalEducationAgencyId { get; set; }
        public int MetricVariantId { get; set; }
        public string WebServiceUrl { get; set; }
        public string WebServiceMethod { get; set; }
        public string ParametersToSendToWebService { get; set; }
        public string ActionTitle { get; set; }
        public object JsonChartModel { get; set; }
    }
}