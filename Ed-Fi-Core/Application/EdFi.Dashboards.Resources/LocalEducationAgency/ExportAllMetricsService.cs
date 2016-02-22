// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using EdFi.Dashboards.Common;
using EdFi.Dashboards.Data.Entities;
using EdFi.Dashboards.Data.Repository;
using EdFi.Dashboards.Metric.Resources.Models;
using EdFi.Dashboards.Resources.Common;
using EdFi.Dashboards.Resources.Metric;
using EdFi.Dashboards.Resources.Metric.Requests;
using EdFi.Dashboards.Resources.Models.Common;
using EdFi.Dashboards.Resources.Security.Common;
using EdFi.Dashboards.SecurityTokenService.Authentication;

namespace EdFi.Dashboards.Resources.LocalEducationAgency
{
    public class ExportAllMetricsRequest
    {
        public int LocalEducationAgencyId { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExportAllMetricsRequest"/> using the specified parameters.
        /// </summary>
        /// <returns>A new <see cref="ExportAllMetricsRequest"/> instance.</returns>
        public static ExportAllMetricsRequest Create(int localEducationAgencyId) 
        {
            return new ExportAllMetricsRequest { LocalEducationAgencyId = localEducationAgencyId };
        }
    }

    public interface IExportAllMetricsService : IService<ExportAllMetricsRequest, ExportAllModel> { }

    public class ExportAllMetricsService : IExportAllMetricsService
    {
        private readonly IRepository<LocalEducationAgencyInformation> localEducationAgencyInformationRepository;
        private readonly IDomainMetricService<LocalEducationAgencyMetricInstanceSetRequest> domainMetricService;
        private readonly IRootMetricNodeResolver rootMetricNodeResolver;
        private readonly IMetricTreeToIEnumerableOfKeyValuePairProvider metricTreeToIEnumerableOfKeyValuePairProvider;
        private readonly IDomainSpecificMetricNodeResolver domainSpecificMetricNodeResolver;

        public ExportAllMetricsService(
            IRepository<LocalEducationAgencyInformation> localEducationAgencyInformationRepository,
            IDomainMetricService<LocalEducationAgencyMetricInstanceSetRequest> domainMetricService, 
            IRootMetricNodeResolver rootMetricNodeResolver, 
            IMetricTreeToIEnumerableOfKeyValuePairProvider metricTreeToIEnumerableOfKeyValuePairProvider,
            IDomainSpecificMetricNodeResolver domainSpecificMetricNodeResolver)
        {
            this.localEducationAgencyInformationRepository = localEducationAgencyInformationRepository;
            this.domainMetricService = domainMetricService;
            this.rootMetricNodeResolver = rootMetricNodeResolver;
            this.metricTreeToIEnumerableOfKeyValuePairProvider = metricTreeToIEnumerableOfKeyValuePairProvider;
            this.domainSpecificMetricNodeResolver = domainSpecificMetricNodeResolver;
        }

        [CanBeAuthorizedBy(EdFiClaimTypes.ViewAllMetrics, EdFiClaimTypes.ViewMyMetrics)]
        public ExportAllModel Get(ExportAllMetricsRequest request)
        {
            int localEducationAgencyId = request.LocalEducationAgencyId;

            var model = new ExportAllModel();

            var leaData = localEducationAgencyInformationRepository.GetAll().SingleOrDefault(x => x.LocalEducationAgencyId == localEducationAgencyId);
            if (leaData == null)
                throw new ArgumentNullException(string.Format("No Local Education Agency with Id:{0} found.", localEducationAgencyId));

            var keyValuePairList = new List<KeyValuePair<string, object>>
                                       {
                                           new KeyValuePair<string, object>("Local Education Agency", leaData.Name)
                                       };

            var flatMetrics = GetFlattenedOutMetrics(localEducationAgencyId);

            keyValuePairList.AddRange(flatMetrics);

            model.Rows = new List<ExportAllModel.Row>
                             {
                                 new ExportAllModel.Row
                                    {
                                        Cells = keyValuePairList
                                    }
                             };

            return model;
        }


        private IEnumerable<KeyValuePair<string, object>> GetFlattenedOutMetrics(int localEducationAgencyId)
        {
            IEnumerable<KeyValuePair<string, object>> result;

            var overviewNode = rootMetricNodeResolver.GetRootMetricNode();
            var overviewMetricTree = domainMetricService.Get(LocalEducationAgencyMetricInstanceSetRequest.Create(localEducationAgencyId, overviewNode.MetricVariantId));

            //Only Container metrics have the DecendantsOrSelf Enumerable property so we need this to be able to flatten it out.
            var overviewMetricTreeAsContainer = overviewMetricTree.RootNode as ContainerMetric;
            if (overviewMetricTreeAsContainer == null)
                throw new InvalidOperationException("Overview metric has to be casted to container to be able to export all metrics for given domain entity.");
            var flatOverviewMetrics = metricTreeToIEnumerableOfKeyValuePairProvider.FlattenMetricTree(overviewMetricTreeAsContainer);

            result = flatOverviewMetrics;

            var operationalDashboardNode = domainSpecificMetricNodeResolver.GetOperationalDashboardMetricNode();
            var operationalDashboardMetricTree = domainMetricService.Get(LocalEducationAgencyMetricInstanceSetRequest.Create(localEducationAgencyId, operationalDashboardNode.MetricVariantId));
            
            // Process operational dashboard metrics only if they were successfully returned.
            if (operationalDashboardMetricTree != null)
            {
                //Only Container metrics have the DecendantsOrSelf Enumerable property so we need this to be able to flatten it out.
                var operationalDashboardTreeAsContainer = operationalDashboardMetricTree.RootNode as ContainerMetric;

                if (operationalDashboardTreeAsContainer == null)
                    throw new InvalidOperationException("Operational Dashboard metric must be castable to a ContainerMetric to be able to export all metrics for given domain entity.");

                var flatOperationalDashboardMetrics = metricTreeToIEnumerableOfKeyValuePairProvider.FlattenMetricTree(operationalDashboardTreeAsContainer);

                result = result.Union(flatOperationalDashboardMetrics);
            }

            return result;
        }
    }
}
