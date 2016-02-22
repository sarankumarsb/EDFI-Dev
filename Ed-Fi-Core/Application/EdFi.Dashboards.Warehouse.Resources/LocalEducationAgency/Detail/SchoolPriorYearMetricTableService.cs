using System;
using System.Collections.Generic;
using System.Linq;
using EdFi.Dashboards.Common;
using EdFi.Dashboards.Common.Utilities;
using EdFi.Dashboards.Metric.Resources.Models;
using EdFi.Dashboards.Metric.Resources.Providers;
using EdFi.Dashboards.Metric.Resources.Services;
using EdFi.Dashboards.Resource.Models.Common;
using EdFi.Dashboards.Resources;
using EdFi.Dashboards.Resources.Common;
using EdFi.Dashboards.Resources.LocalEducationAgency.Detail;
using EdFi.Dashboards.Resources.Metric;
using EdFi.Dashboards.Resources.Metric.Requests;
using EdFi.Dashboards.Resources.Navigation;
using EdFi.Dashboards.Data.Entities;
using EdFi.Dashboards.Data.Repository;
using EdFi.Dashboards.Resources.Security.Common;
using EdFi.Dashboards.SecurityTokenService.Authentication;
using EdFi.Dashboards.Warehouse.Data.Entities;
using EdFi.Dashboards.Warehouse.Resource.Models.LocalEducationAgency.Detail;
using EdFi.Dashboards.Warehouse.Resources.Application;

namespace EdFi.Dashboards.Warehouse.Resources.LocalEducationAgency.Detail
{
    public class SchoolPriorYearMetricTableRequest
    {
        public int MetricVariantId { get; set; }
        public int LocalEducationAgencyId { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SchoolMetricTableRequest"/> using the specified parameters.
        /// </summary>
        /// <returns>A new <see cref="SchoolMetricTableRequest"/> instance.</returns>
        public static SchoolPriorYearMetricTableRequest Create(int localEducationAgencyId, int metricVariantId) 
        {
            return new SchoolPriorYearMetricTableRequest { MetricVariantId = metricVariantId, LocalEducationAgencyId = localEducationAgencyId };
        }
    }

    public interface ISchoolPriorYearMetricTableService : IService<SchoolPriorYearMetricTableRequest, IList<SchoolPriorYearMetricModel>> { }

    public class SchoolPriorYearMetricTableService : ISchoolPriorYearMetricTableService
    {
        private const string schoolLink = "school";
        private const string metricContext = "metricContext";
        private const string metric = "metric";

        private readonly IRepository<LocalEducationAgencyMetricInstanceSchoolList> localEducationAgencyMetricInstanceSchoolListRepository;
        private readonly IRepository<SchoolInformation> schoolInformationRepository;
        private readonly IUniqueListIdProvider uniqueListProvider;
        private readonly IMetricCorrelationProvider metricCorrelationService;
        private readonly IMetricGoalProvider metricGoalProvider;
        private readonly IWarehouseAvailabilityProvider warehouseAvailabilityProvider;
        private readonly IMaxPriorYearProvider maxPriorYearProvider;

        private readonly IMetricInstanceSetKeyResolver<LocalEducationAgencyMetricInstanceSetRequest> metricInstanceSetKeyResolver;

        private readonly ISchoolAreaLinks schoolLinks;
        private readonly IMetricNodeResolver metricNodeResolver;

        public SchoolPriorYearMetricTableService(IRepository<LocalEducationAgencyMetricInstanceSchoolList> localEducationAgencyMetricInstanceSchoolListRepository,
                                                    IRepository<SchoolInformation> schoolInformationRepository,
                                                    IUniqueListIdProvider uniqueListProvider,
                                                    IMetricCorrelationProvider metricCorrelationService,
                                                    IMetricGoalProvider metricGoalProvider,
                                                    IMetricInstanceSetKeyResolver<LocalEducationAgencyMetricInstanceSetRequest> metricInstanceSetKeyResolver,
                                                    IMetricNodeResolver metricNodeResolver,
                                                    ISchoolAreaLinks schoolLinks,
                                                    IWarehouseAvailabilityProvider warehouseAvailabilityProvider,
                                                    IMaxPriorYearProvider maxPriorYearProvider)
        {
            this.localEducationAgencyMetricInstanceSchoolListRepository = localEducationAgencyMetricInstanceSchoolListRepository;
            this.schoolInformationRepository = schoolInformationRepository;
            this.uniqueListProvider = uniqueListProvider;
            this.metricCorrelationService = metricCorrelationService;
            this.metricGoalProvider = metricGoalProvider;
            this.metricInstanceSetKeyResolver = metricInstanceSetKeyResolver;
            this.metricNodeResolver = metricNodeResolver;
            this.schoolLinks = schoolLinks;
            this.warehouseAvailabilityProvider = warehouseAvailabilityProvider;
            this.maxPriorYearProvider = maxPriorYearProvider;
        }

        [CanBeAuthorizedBy(EdFiClaimTypes.ViewAllMetrics, EdFiClaimTypes.ViewMyMetrics)]
        public IList<SchoolPriorYearMetricModel> Get(SchoolPriorYearMetricTableRequest request)
        {
            var model = new List<SchoolPriorYearMetricModel>();

            if (!warehouseAvailabilityProvider.Get())
            {
                return model;
            }

            int localEducationAgencyId = request.LocalEducationAgencyId;
            int metricVariantId = request.MetricVariantId;
            var metricMetatdata = metricNodeResolver.GetMetricNodeForLocalEducationAgencyMetricVariantId(metricVariantId);
            var metricId = metricMetatdata.MetricId;


            var localEducationAgencyMetricInstanceSetKey = metricInstanceSetKeyResolver.GetMetricInstanceSetKey(LocalEducationAgencyMetricInstanceSetRequest.Create(localEducationAgencyId, metricVariantId));

            var metricGoal = metricGoalProvider.GetMetricGoal(localEducationAgencyMetricInstanceSetKey, metricId);

            var year = maxPriorYearProvider.Get(localEducationAgencyId);
            var schoolMetrics = (from schoolMetric in localEducationAgencyMetricInstanceSchoolListRepository.GetAll()
                                where schoolMetric.MetricId == metricId &&
                                      schoolMetric.LocalEducationAgencyId == request.LocalEducationAgencyId &&
                                      schoolMetric.SchoolYear == year
                                orderby schoolMetric.SchoolId
                                select schoolMetric).ToList();
            
            var schoolIds = schoolMetrics.Select(x => x.SchoolId).ToArray();
            var schools = (from si in schoolInformationRepository.GetAll()
                                     where schoolIds.Contains(si.SchoolId)
                                     select si).ToList();


            var uniqueListId = uniqueListProvider.GetUniqueId(metricVariantId);

            foreach (var schoolMetric in schoolMetrics)
            {
                var school = schools.SingleOrDefault(x => x.SchoolId == schoolMetric.SchoolId);
                if (school == null)
                    continue;

                var schoolMetricModel = new SchoolPriorYearMetricModel
                                            {
                                                SchoolId = schoolMetric.SchoolId,
                                                Name = school.Name,
                                                Principal = schoolMetric.StaffFullName,
                                                SchoolCategory = school.SchoolCategory,
                                                Goal = Convert.ToDouble(schoolMetric.SchoolGoal),
                                                Href = new Link
                                                        {
                                                            Rel = schoolLink,
                                                            Href = schoolLinks.Default(school.SchoolId, school.Name, new { listContext = uniqueListId })
                                                        },
                                                ValueType = schoolMetric.ValueType
                                            };

                var metricValue = InstantiateValue.FromValueType(schoolMetric.Value, schoolMetric.ValueType);
                schoolMetricModel.DisplayValue = String.Format(metricMetatdata.ListFormat, metricValue);
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

                var correlatedSchoolMetric = metricCorrelationService.GetRenderingParentMetricVariantIdForSchool(metricVariantId, school.SchoolId);
                //Lets try to add the full path to the metric. This means ContextMetric(parent) and metric.
                if (correlatedSchoolMetric.MetricVariantId != null)
                {
                    schoolMetricModel.MetricContextLink = new Link
                                                              {
                                                                  Rel = metricContext,
                                                                  Href = schoolLinks.Metrics(school.SchoolId, correlatedSchoolMetric.ContextMetricVariantId.Value, school.Name, new { listContext = uniqueListId }).MetricAnchor(correlatedSchoolMetric.MetricVariantId)
                                                              };
                }

                //If we cant resolve a child metric at least lets put our client on the right page. 
                if (correlatedSchoolMetric.ContextMetricVariantId != null)
                {
                    schoolMetricModel.MetricLink = new Link
                                                       {
                                                           Rel = metric,
                                                           Href = schoolLinks.Metrics(school.SchoolId, correlatedSchoolMetric.ContextMetricVariantId.Value, school.Name, new { listContext = uniqueListId }),
                                                       };
                }

                model.Add(schoolMetricModel);
            }

            return model;
        }
    }
}
