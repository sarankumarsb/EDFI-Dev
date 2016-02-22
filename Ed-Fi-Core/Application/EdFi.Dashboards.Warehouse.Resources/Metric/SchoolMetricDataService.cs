using System;
using System.Globalization;
using System.Linq;
using EdFi.Dashboards.Common;
using EdFi.Dashboards.Data.Repository;
using EdFi.Dashboards.Infrastructure.Implementations.Caching;
using EdFi.Dashboards.Metric.Data.Entities;
using EdFi.Dashboards.Metric.Resources.Models;
using EdFi.Dashboards.Resources;
using EdFi.Dashboards.Resources.Security.Common;
using EdFi.Dashboards.Warehouse.Data.Entities;
using EdFi.Dashboards.Warehouse.Resource.Models.Metric;

namespace EdFi.Dashboards.Warehouse.Resources.Metric
{
    public class SchoolMetricDataRequest
    {
        public int SchoolId { get; set; }

        [AuthenticationIgnore("MetricInstanceSetKey is implied by School, and does not need to be independently authorized.")]
        public Guid MetricInstanceSetKey { get; set; }

        [AuthenticationIgnore("Does not affect security")]
        public int SchoolYear { get; set; }

        public static SchoolMetricDataRequest Create(int schoolId, Guid metricInstanceSetKey, int schoolYear)
        {
            return new SchoolMetricDataRequest { SchoolId = schoolId, MetricInstanceSetKey = metricInstanceSetKey, SchoolYear = schoolYear };
        }
    }

    public interface ISchoolMetricDataService : IService<SchoolMetricDataRequest, MetricData> { }

    public class SchoolMetricDataService : ISchoolMetricDataService
    {
        private const string schoolYearExtendedPropertyName = "SchoolYear";
        private const string goalExtendedPropertyName = "Goal";
        private const string schoolYearExtendedPropertyValueTypeName = "System.Int32";

        private readonly IRepository<SchoolMetricInstance> metricInstanceRepository;
        private readonly IRepository<SchoolMetricInstanceExtendedProperty> metricInstanceExtendedPropertyRepository;
        private readonly IRepository<SchoolMetricComponent> metricComponentRepository;

        public SchoolMetricDataService(IRepository<SchoolMetricInstance> metricInstanceRepository,
                                       IRepository<SchoolMetricInstanceExtendedProperty> metricInstanceExtendedPropertyRepository,
                                       IRepository<SchoolMetricComponent> metricComponentRepository)
        {
            this.metricInstanceRepository = metricInstanceRepository;
            this.metricInstanceExtendedPropertyRepository = metricInstanceExtendedPropertyRepository;
            this.metricComponentRepository = metricComponentRepository;
        }

        [CacheBehavior(copyOnSet: false, copyOnGet: false)]
        public MetricData Get(SchoolMetricDataRequest request)
        {
            var schoolId = request.SchoolId;
            var metricInstanceSetKey = request.MetricInstanceSetKey;
            var year = request.SchoolYear;

            var result = new PriorYearMetricData();
            var metricInstances = (from m in metricInstanceRepository.GetAll()
                                   where m.SchoolId == schoolId
                                         && m.SchoolYear == year
                                   select m).ToList();

            result.MetricInstances = metricInstances.Select(x => new MetricInstance
                                                                    {
                                                                        Context = x.Context,
                                                                        Flag = x.Flag,
                                                                        MetricId = x.MetricId,
                                                                        MetricInstanceSetKey = metricInstanceSetKey,
                                                                        MetricStateTypeId = x.MetricStateTypeId,
                                                                        TrendDirection = x.TrendDirection,
                                                                        Value = x.Value,
                                                                        ValueTypeName = x.ValueTypeName
                                                                    }).ToList();

            var metricInstanceExtendedProperties = (from me in metricInstanceExtendedPropertyRepository.GetAll()
                                                    where me.SchoolId == schoolId
                                                          && me.SchoolYear == year
                                                    select me).ToList();

            var metricInstanceExtendedPropertyList = metricInstanceExtendedProperties.Select(x => new MetricInstanceExtendedProperty
                                                                                                    {
                                                                                                        MetricId = x.MetricId,
                                                                                                        MetricInstanceSetKey = metricInstanceSetKey,
                                                                                                        Name = x.Name,
                                                                                                        Value = x.Value,
                                                                                                        ValueTypeName = x.ValueTypeName
                                                                                                    }).ToList();

            metricInstanceExtendedPropertyList.AddRange(metricInstances.Select(x => new MetricInstanceExtendedProperty
                                                                                        {
                                                                                            MetricId = x.MetricId,
                                                                                            MetricInstanceSetKey = metricInstanceSetKey,
                                                                                            Name = schoolYearExtendedPropertyName,
                                                                                            Value = x.SchoolYear.ToString(CultureInfo.InvariantCulture),
                                                                                            ValueTypeName = schoolYearExtendedPropertyValueTypeName
                                                                                        }));

            result.MetricInstanceExtendedProperties = metricInstanceExtendedPropertyList;

            result.MetricGoals = metricInstanceExtendedProperties.Where(x => x.Name == goalExtendedPropertyName).Select(x => new MetricGoal
                                                                                                                {
                                                                                                                    MetricId = x.MetricId,
                                                                                                                    MetricInstanceSetKey = metricInstanceSetKey,
                                                                                                                    Value = Convert.ToDecimal(x.Value)
                                                                                                                });

            result.MetricComponents = (from mc in metricComponentRepository.GetAll()
                                       where mc.SchoolId == schoolId
                                             && mc.SchoolYear == year
                                       select new Dashboards.Metric.Data.Entities.MetricComponent
                                       {
                                           MetricInstanceSetKey = metricInstanceSetKey,
                                           MetricId = mc.MetricId,
                                           MetricStateTypeId = mc.MetricStateTypeId,
                                           Name = mc.Name,
                                           Value = mc.Value,
                                           ValueTypeName = mc.ValueTypeName,
                                           Format = mc.Format,
                                           TrendDirection = mc.TrendDirection
                                       });

            return result;
        }
    }
}
