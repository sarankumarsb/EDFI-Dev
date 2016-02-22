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
using EdFi.Dashboards.Resources.Metric.Requests;
using EdFi.Dashboards.Resources.Models.LocalEducationAgency.Detail;
using EdFi.Dashboards.Resources.Navigation;
using EdFi.Dashboards.Data.Entities;
using EdFi.Dashboards.Data.Repository;
using EdFi.Dashboards.Resources.Security.Common;
using EdFi.Dashboards.SecurityTokenService.Authentication;

namespace EdFi.Dashboards.Resources.LocalEducationAgency.Detail
{
    public class GoalPlanningSchoolMetricTableRequest
    {
        public int MetricVariantId { get; set; }
        public int LocalEducationAgencyId { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="GoalPlanningSchoolMetricTableRequest"/> using the specified parameters.
        /// </summary>
        /// <returns>A new <see cref="GoalPlanningSchoolMetricTableRequest"/> instance.</returns>
        public static GoalPlanningSchoolMetricTableRequest Create(int localEducationAgencyId, int metricVariantId) 
        {
            return new GoalPlanningSchoolMetricTableRequest { MetricVariantId = metricVariantId, LocalEducationAgencyId = localEducationAgencyId };
        }
    }

    public interface IGoalPlanningSchoolMetricTableService : IService<GoalPlanningSchoolMetricTableRequest, GoalPlanningSchoolMetricModel> { }

    public class GoalPlanningSchoolMetricTableService : IGoalPlanningSchoolMetricTableService
    {
        private const string schoolLink = "school";
        private const string metricContext = "metricContext";

        private readonly IRepository<LocalEducationAgencyMetricSchoolList> localEducationAgencyMetricSchoolListRepository;
        private readonly IRepository<StaffInformation> staffInformationRepository;
        private readonly IRepository<SchoolInformation> schoolInformationRepository;
        private readonly IUniqueListIdProvider uniqueListProvider;
        private readonly IMetricCorrelationProvider metricCorrelationService;
        private readonly IMetricGoalProvider metricGoalProvider;
        private readonly IMetricInstanceSetKeyResolver<LocalEducationAgencyMetricInstanceSetRequest> metricInstanceSetKeyResolver;
        private readonly IMetricMetadataTreeService metricMetadataNodeService;
        private readonly ISchoolAreaLinks schoolLinks;
        private readonly IMetricNodeResolver metricNodeResolver;

        public GoalPlanningSchoolMetricTableService(IRepository<LocalEducationAgencyMetricSchoolList> localEducationAgencyMetricSchoolListRepository,
                                                    IRepository<SchoolInformation> schoolInformationRepository,
                                                    IRepository<StaffInformation> staffInformationRepository,
                                                    IUniqueListIdProvider uniqueListProvider,
                                                    IMetricCorrelationProvider metricCorrelationService,
                                                    IMetricGoalProvider metricGoalProvider,
                                                    IMetricInstanceSetKeyResolver<LocalEducationAgencyMetricInstanceSetRequest> metricInstanceSetKeyResolver,
                                                    IMetricNodeResolver metricNodeResolver,
                                                    IMetricMetadataTreeService metricMetadataNodeService,
                                                    ISchoolAreaLinks schoolLinks)
        {
            this.localEducationAgencyMetricSchoolListRepository = localEducationAgencyMetricSchoolListRepository;
            this.schoolInformationRepository = schoolInformationRepository;
            this.staffInformationRepository = staffInformationRepository;
            this.uniqueListProvider = uniqueListProvider;
            this.metricCorrelationService = metricCorrelationService;
            this.metricGoalProvider = metricGoalProvider;
            this.metricInstanceSetKeyResolver = metricInstanceSetKeyResolver;
            this.metricNodeResolver = metricNodeResolver;
            this.metricMetadataNodeService = metricMetadataNodeService;
            this.schoolLinks = schoolLinks;
        }

        [CanBeAuthorizedBy(EdFiClaimTypes.ManageGoals)]
        public GoalPlanningSchoolMetricModel Get(GoalPlanningSchoolMetricTableRequest request)
        {
            int localEducationAgencyId = request.LocalEducationAgencyId;
            int metricVariantId = request.MetricVariantId;
            var metricMetadataNode = metricNodeResolver.GetMetricNodeForLocalEducationAgencyMetricVariantId(metricVariantId);

            if (String.IsNullOrEmpty(metricMetadataNode.ListFormat))
                throw new ArgumentNullException(string.Format("List Format is null for metricVariantId:{0}", request.MetricVariantId));

            int metricId = metricMetadataNode.MetricId;

            var goalPlanningSchoolMetrics = new List<GoalPlanningSchoolMetric>();
            var schoolMetrics = new List<SchoolMetric>();

            var localEducationAgencyMetricInstanceSetKey = metricInstanceSetKeyResolver.GetMetricInstanceSetKey(LocalEducationAgencyMetricInstanceSetRequest.Create(localEducationAgencyId, metricVariantId));

            var metricGoal = metricGoalProvider.GetMetricGoal(localEducationAgencyMetricInstanceSetKey, metricId);

            var results = (from schoolMetric in localEducationAgencyMetricSchoolListRepository.GetAll()
                           join schoolInformation in schoolInformationRepository.GetAll() on schoolMetric.SchoolId
                               equals schoolInformation.SchoolId
                           from staffInformation in staffInformationRepository.GetAll().Where(x => x.StaffUSI == schoolMetric.StaffUSI).DefaultIfEmpty()
                           where schoolMetric.MetricId == metricId 
                                    && schoolMetric.LocalEducationAgencyId == localEducationAgencyId
                           orderby schoolMetric.SchoolId
                           select new {schoolMetric, schoolInformation, staffInformation});

            var uniqueListId = uniqueListProvider.GetUniqueId(metricVariantId);

            foreach (var result in results)
            {
                var correlatedSchoolMetric = metricCorrelationService.GetRenderingParentMetricVariantIdForSchool(metricVariantId, result.schoolInformation.SchoolId);
                // if we don't know what metric to set the new goal to, don't add the school to the list
                if (correlatedSchoolMetric.MetricVariantId == null)
                    continue;

                var schoolMetricModel = new GoalPlanningSchoolMetric
                                            {
                                                SchoolId = result.schoolInformation.SchoolId,
                                                Name = result.schoolInformation.Name,
                                                Principal = result.staffInformation != null
                                                                ? Utilities.FormatPersonNameByLastName(result.staffInformation.FirstName, null, result.staffInformation.LastSurname)
                                                                : String.Empty,
                                                SchoolCategory = result.schoolInformation.SchoolCategory,
                                                Goal = Convert.ToDouble(result.schoolMetric.SchoolGoal),
                                                Href = new Link { Rel = schoolLink, Href = schoolLinks.Default(result.schoolInformation.SchoolId, result.schoolInformation.Name, new { listContext = uniqueListId })},
                                                ValueType = result.schoolMetric.ValueType
                                            };

                var metricValue = InstantiateValue.FromValueType(result.schoolMetric.Value, result.schoolMetric.ValueType);
                schoolMetricModel.DisplayValue = String.Format(metricMetadataNode.ListFormat, metricValue);
                if (metricValue is double)
                {
                    schoolMetricModel.Value = metricValue;
                    schoolMetricModel.StandardGoalInterpretation = (metricGoal.Interpretation == TrendInterpretation.Standard);
                    if (metricGoal.Interpretation == TrendInterpretation.Standard)
                        schoolMetricModel.GoalDifference = metricValue - schoolMetricModel.Goal;
                    else
                        schoolMetricModel.GoalDifference = schoolMetricModel.Goal - metricValue;

                    schoolMetricModel.MetricState.StateType = schoolMetricModel.GoalDifference >= 0 ? MetricStateType.Good : MetricStateType.Low;
                }
                else
                {
                    schoolMetricModel.ValueType = typeof (string).ToString();
                    schoolMetricModel.MetricState.StateType = MetricStateType.None;
                }

                //Lets get the metric tree that applies to this.
                var correlatedSchoolNode = metricNodeResolver.GetMetricNodeForSchoolFromMetricVariantId(result.schoolInformation.SchoolId, correlatedSchoolMetric.MetricVariantId.Value);
                MetricMetadataNode metricTreeForCorrelatedMetric;
                metricMetadataNodeService.Get(MetricMetadataTreeRequest.Create()).AllNodesByMetricNodeId.TryGetValue(correlatedSchoolNode.MetricNodeId, out metricTreeForCorrelatedMetric);
                if (metricTreeForCorrelatedMetric == null)
                    continue;
                if (metricTreeForCorrelatedMetric.MetricType == MetricType.GranularMetric)
                {
                    schoolMetricModel.GoalMetricIds.Add(correlatedSchoolMetric.MetricVariantId.Value);
                    schoolMetrics.Add(new SchoolMetric { SchoolId = result.schoolInformation.SchoolId, MetricId = correlatedSchoolMetric.MetricVariantId.Value});
                }
                else
                {
                    foreach (var childCorrelatedMetric in metricTreeForCorrelatedMetric.Descendants.Where(x => x.MetricType == MetricType.GranularMetric && x.MetricVariantType == MetricVariantType.CurrentYear))
                    {
                        schoolMetricModel.GoalMetricIds.Add(childCorrelatedMetric.MetricVariantId);
                        schoolMetrics.Add(new SchoolMetric { SchoolId = result.schoolInformation.SchoolId, MetricId = childCorrelatedMetric.MetricId});
                    }
                }
                
                schoolMetricModel.MetricContextLink = new Link
                                                            {
                                                                Rel = metricContext,
                                                                Href = schoolLinks.GoalPlanning(result.schoolInformation.SchoolId, 
                                                                                               correlatedSchoolMetric.ContextMetricVariantId.Value,
                                                                                               result.schoolInformation.Name,
                                                                                               new { listContext = uniqueListId })
                                                                                               .MetricAnchor(correlatedSchoolMetric.MetricVariantId)
                                                                  
                                                            };


                goalPlanningSchoolMetrics.Add(schoolMetricModel);
            }

            return new GoalPlanningSchoolMetricModel { GoalPlanningSchoolMetrics = goalPlanningSchoolMetrics, SchoolMetrics = schoolMetrics};
        }
    }
}
