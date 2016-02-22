using System;
using System.Collections.Generic;
using System.Linq;
using EdFi.Dashboards.Common;
using EdFi.Dashboards.Common.Utilities;
using EdFi.Dashboards.Metric.Resources.Models;
using EdFi.Dashboards.Metric.Resources.Providers;
using EdFi.Dashboards.Metric.Resources.Services;
using EdFi.Dashboards.Resource.Models.Common;
using EdFi.Dashboards.Resources.Common;
using EdFi.Dashboards.Resources.Metric;
using EdFi.Dashboards.Resources.Metric.MetricInstanceSetKeyResolvers;
using EdFi.Dashboards.Resources.Metric.Requests;
using EdFi.Dashboards.Resources.Models.LocalEducationAgency.Detail;
using EdFi.Dashboards.Resources.Navigation;
using EdFi.Dashboards.Data.Entities;
using EdFi.Dashboards.Data.Repository;
using EdFi.Dashboards.Resources.Models;
using EdFi.Dashboards.Resources.Navigation.Mvc;
using EdFi.Dashboards.Resources.Security.Common;
using EdFi.Dashboards.Resources.Staff;
using EdFi.Dashboards.SecurityTokenService.Authentication;
using EdFi.Dashboards.Resources.Models.CustomGrid;

namespace EdFi.Dashboards.Resources.LocalEducationAgency.Detail
{
    public class SchoolMetricTableRequest
    {
        public int MetricVariantId { get; set; }
        public int LocalEducationAgencyId { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SchoolMetricTableRequest"/> using the specified parameters.
        /// </summary>
        /// <returns>A new <see cref="SchoolMetricTableRequest"/> instance.</returns>
        public static SchoolMetricTableRequest Create(int localEducationAgencyId, int metricVariantId)
        {
            return new SchoolMetricTableRequest { MetricVariantId = metricVariantId, LocalEducationAgencyId = localEducationAgencyId };
        }
    }

    public interface ISchoolMetricTableService : IService<SchoolMetricTableRequest, SchoolMetricTableModel> { }

    public class SchoolMetricTableService : ISchoolMetricTableService
    {
        private const string schoolLink = "school";
        private const string metricContext = "metricContext";
        private const string metric = "metric";

        private readonly IRepository<LocalEducationAgencyMetricSchoolList> localEducationAgencyMetricSchoolListRepository;
        private readonly IRepository<StaffInformation> staffInformationRepository;
        private readonly IRepository<SchoolInformation> schoolInformationRepository;
        private readonly IUniqueListIdProvider uniqueListProvider;
        private readonly IMetricCorrelationProvider metricCorrelationService;
        private readonly IMetricGoalProvider metricGoalProvider;
        private readonly IMetricInstanceSetKeyResolver<LocalEducationAgencyMetricInstanceSetRequest> metricInstanceSetKeyResolver;
        private readonly ISchoolAreaLinks schoolLinks;
        private readonly IMetricNodeResolver metricNodeResolver;
        private readonly IDomainSpecificMetricNodeResolver domainSpecificMetricNodeResolver;
        private readonly IListMetadataProvider listMetadataProvider;
        private readonly IMetadataListIdResolver metadataListIdResolver;

        public SchoolMetricTableService(IRepository<LocalEducationAgencyMetricSchoolList> localEducationAgencyMetricSchoolListRepository,
                                        IRepository<SchoolInformation> schoolInformationRepository,
                                        IRepository<StaffInformation> staffInformationRepository,
                                        IUniqueListIdProvider uniqueListProvider,
                                        IMetricCorrelationProvider metricCorrelationService,
                                        IMetricGoalProvider metricGoalProvider,
                                        IMetricInstanceSetKeyResolver<LocalEducationAgencyMetricInstanceSetRequest> metricInstanceSetKeyResolver,
                                        IMetricNodeResolver metricNodeResolver,
                                        ISchoolAreaLinks schoolLinks,
                                        IDomainSpecificMetricNodeResolver domainSpecificMetricNodeResolver,
                                        IListMetadataProvider listMetadataProvider,
                                        IMetadataListIdResolver metadataListIdResolver)
        {
            this.localEducationAgencyMetricSchoolListRepository = localEducationAgencyMetricSchoolListRepository;
            this.schoolInformationRepository = schoolInformationRepository;
            this.staffInformationRepository = staffInformationRepository;
            this.uniqueListProvider = uniqueListProvider;
            this.metricCorrelationService = metricCorrelationService;
            this.metricGoalProvider = metricGoalProvider;
            this.metricInstanceSetKeyResolver = metricInstanceSetKeyResolver;
            this.metricNodeResolver = metricNodeResolver;
            this.schoolLinks = schoolLinks;
            this.domainSpecificMetricNodeResolver = domainSpecificMetricNodeResolver;
            this.listMetadataProvider = listMetadataProvider;
            this.metadataListIdResolver = metadataListIdResolver;
        }

        [CanBeAuthorizedBy(EdFiClaimTypes.ViewAllMetrics, EdFiClaimTypes.ViewMyMetrics)]
        public SchoolMetricTableModel Get(SchoolMetricTableRequest request)
        {
            var model = new SchoolMetricTableModel();
            //get metadata
            var resolvedListId = metadataListIdResolver.GetListId(ListType.SchoolMetricTable, Core.SchoolCategory.None);
            model.ListMetadata = listMetadataProvider.GetListMetadata(resolvedListId);
            //get data
            var localEducationAgencyId = request.LocalEducationAgencyId;
            var metricVariantId = request.MetricVariantId;

            var metricMetadataNode = metricNodeResolver.GetMetricNodeForLocalEducationAgencyMetricVariantId(metricVariantId);

            if (String.IsNullOrEmpty(metricMetadataNode.ListFormat))
                throw new ArgumentNullException(string.Format("List Format is null for metricVariantId:{0}", request.MetricVariantId));

            var operationalDashboardNode = domainSpecificMetricNodeResolver.GetOperationalDashboardMetricNode();
            var isOperationalDashboardNode = operationalDashboardNode.FindInDescendantsOrSelfByMetricVariantId(metricVariantId).Any();

            var metricId = metricMetadataNode.MetricId;

            var localEducationAgencyMetricInstanceSetKey = metricInstanceSetKeyResolver.GetMetricInstanceSetKey(LocalEducationAgencyMetricInstanceSetRequest.Create(localEducationAgencyId, metricVariantId));

            var metricGoal = metricGoalProvider.GetMetricGoal(localEducationAgencyMetricInstanceSetKey, metricId);

            var results = (from schoolMetric in localEducationAgencyMetricSchoolListRepository.GetAll()
                           join schoolInformation in schoolInformationRepository.GetAll()
                                on schoolMetric.SchoolId equals schoolInformation.SchoolId
                           from staffInformation in
                               staffInformationRepository.GetAll().Where(x => x.StaffUSI == schoolMetric.StaffUSI).DefaultIfEmpty()
                           where schoolMetric.MetricId == metricId &&
                                 schoolMetric.LocalEducationAgencyId == localEducationAgencyId
                           orderby schoolMetric.SchoolId
                           select new { schoolMetric, schoolInformation, staffInformation });

            var uniqueListId = uniqueListProvider.GetUniqueId(metricVariantId);


            foreach (var result in results)
            {
                var schoolMetricModel = new SchoolMetricModel
                                            {
                                                SchoolId = result.schoolInformation.SchoolId,
                                                Name = result.schoolInformation.Name,
                                                Principal = result.staffInformation != null 
                                                                ? Utilities.FormatPersonNameByLastName(result.staffInformation.FirstName, null, result.staffInformation.LastSurname) 
                                                                : String.Empty,
                                                SchoolCategory = result.schoolInformation.SchoolCategory,
                                                Goal = Convert.ToDouble(result.schoolMetric.SchoolGoal),
                                                Href = new Link
                                                            {
                                                                Rel = schoolLink,
                                                                Href = schoolLinks.Default(result.schoolInformation.SchoolId, result.schoolInformation.Name, new { listContext = uniqueListId })
                                                            },
                                                ValueType = result.schoolMetric.ValueType
                                            };

                var metricValue = InstantiateValue.FromValueType(result.schoolMetric.Value, result.schoolMetric.ValueType);
                schoolMetricModel.DisplayValue = String.Format(metricMetadataNode.ListFormat, metricValue);
                if (metricValue is double)
                {
                    schoolMetricModel.Value = metricValue;
                    if (metricGoal.Interpretation == TrendInterpretation.Standard)
                        schoolMetricModel.GoalDifference = metricValue - schoolMetricModel.Goal;
                    else
                        schoolMetricModel.GoalDifference = schoolMetricModel.Goal - metricValue;

                    schoolMetricModel.MetricState.StateType = schoolMetricModel.GoalDifference >= 0
                                                                  ? MetricStateType.Good
                                                                  : MetricStateType.Low;
                }
                else
                {
                    schoolMetricModel.ValueType = typeof (string).ToString();
                    schoolMetricModel.MetricState.StateType = MetricStateType.None;
                }

                var correlatedSchoolMetric = metricCorrelationService.GetRenderingParentMetricVariantIdForSchool(metricVariantId, result.schoolInformation.SchoolId);
                //Lets try to add the full path to the metric. This means ContextMetric(parent) and metric.
                if (correlatedSchoolMetric.MetricVariantId != null)
                {
                    if (!isOperationalDashboardNode)
                    {
                        schoolMetricModel.MetricContextLink = new Link
                                                                  {
                                                                      Rel = metricContext,
                                                                      Href = schoolLinks.Metrics(result.schoolInformation.SchoolId,
                                                                                                 correlatedSchoolMetric.ContextMetricVariantId.Value,
                                                                                                 result.schoolInformation.Name,
                                                                                                 new {listContext = uniqueListId})
                                                                          .MetricAnchor(correlatedSchoolMetric.MetricVariantId)
                                                                  };
                    }
                    else
                    {
                        schoolMetricModel.MetricContextLink = new Link
                                                                    {
                                                                        Rel = metricContext,
                                                                        Href = schoolLinks.OperationalDashboard(result.schoolInformation.SchoolId,
                                                                                                                result.schoolInformation.Name,
                                                                                                                new { listContext = uniqueListId })
                                                                                            .MetricAnchor(correlatedSchoolMetric.MetricVariantId)
                                                                    };
                        
                    }
                }

                //If we cant resolve a child metric at least lets put our client on the right page. 
                if (correlatedSchoolMetric.ContextMetricVariantId != null)
                {
                    if (!isOperationalDashboardNode)
                    {
                        schoolMetricModel.MetricLink = new Link
                                                           {
                                                               Rel = metric,
                                                               Href = schoolLinks.Metrics(result.schoolInformation.SchoolId, correlatedSchoolMetric.ContextMetricVariantId.Value, result.schoolInformation.Name, new {listContext = uniqueListId}),
                                                           };
                    }
                    else
                    {
                        schoolMetricModel.MetricLink = new Link
                                                            {
                                                                Rel = metric,
                                                                Href = schoolLinks.OperationalDashboard(result.schoolInformation.SchoolId, result.schoolInformation.Name, new { listContext = uniqueListId }),
                                                            };
                    }
                }

                model.SchoolMetrics.Add(schoolMetricModel);
            }

            //return
            return model;
        }
    }
}
