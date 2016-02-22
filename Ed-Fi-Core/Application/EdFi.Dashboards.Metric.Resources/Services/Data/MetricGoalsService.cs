using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EdFi.Dashboards.Common;
using EdFi.Dashboards.Data.Repository;
using EdFi.Dashboards.Infrastructure.Implementations;
using EdFi.Dashboards.Infrastructure.Implementations.Caching;
using EdFi.Dashboards.Metric.Data.Entities;
using EdFi.Dashboards.Resources;

namespace EdFi.Dashboards.Metric.Resources.Services.Data
{
    public interface IMetricGoalsService : IService<MetricDataRequest, IEnumerable<MetricGoal>> { }

    public class MetricGoalsService : IMetricGoalsService
    {
        private readonly IRepository<MetricGoal> metricGoalRepository;

        public MetricGoalsService(IRepository<MetricGoal> metricGoalRepository)
        {
            this.metricGoalRepository = metricGoalRepository;
        }

        [CacheBehavior(copyOnSet: false, copyOnGet: false)]
        public IEnumerable<MetricGoal> Get(MetricDataRequest request)
        {
            var metricInstanceSetKey = request.MetricInstanceSetKey;

            return (from m in metricGoalRepository.GetAll()
                    where m.MetricInstanceSetKey == metricInstanceSetKey
                    select m)
                .ToList();
        }
    }
}
