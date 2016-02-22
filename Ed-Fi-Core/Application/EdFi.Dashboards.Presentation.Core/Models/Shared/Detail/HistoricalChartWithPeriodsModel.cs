using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EdFi.Dashboards.Presentation.Core.Models.Shared.Detail
{
    public class HistoricalChartWithPeriodsModel
    {
        public HistoricalChartWithPeriodsModel(int localEducationAgencyId, int metricVariantId, string webServiceUrl, string webServiceMethodForDefaultPeriod, string webServiceMethodForSuppliedPeriod, string parametersToSendToWebService, string actionTitle)
        {
            LocalEducationAgencyId = localEducationAgencyId;
            MetricVariantId = metricVariantId;
            WebServiceUrl = webServiceUrl;
            WebServiceMethodForDefaultPeriod = webServiceMethodForDefaultPeriod;
            WebServiceMethodForSuppliedPeriod = webServiceMethodForSuppliedPeriod;
            ParametersToSendToWebService = parametersToSendToWebService;
            ActionTitle = actionTitle;
        }

        public int LocalEducationAgencyId { get; set; }
        public int MetricVariantId { get; set; }
        public string WebServiceUrl { get; set; }
        public string WebServiceMethodForDefaultPeriod { get; set; }
        public string WebServiceMethodForSuppliedPeriod { get; set; }
        public string ParametersToSendToWebService { get; set; }
        public string ActionTitle { get; set; }
    }
}