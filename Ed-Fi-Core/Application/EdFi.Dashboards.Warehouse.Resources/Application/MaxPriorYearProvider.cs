using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using EdFi.Dashboards.Data.Repository;
using EdFi.Dashboards.Warehouse.Data.Entities;

namespace EdFi.Dashboards.Warehouse.Resources.Application
{
    public interface IMaxPriorYearProvider
    {
        int Get(int localEducationAgencyId);
    }

    public class MaxPriorYearProvider : IMaxPriorYearProvider
    {
        private readonly IRepository<LocalEducationAgencyMetricInstance> localEducationAgencyMetricInstanceRepository;
        private static readonly ConcurrentDictionary<int, int> localEducationAgencyMaxYear = new ConcurrentDictionary<int, int>();

        public MaxPriorYearProvider(IRepository<LocalEducationAgencyMetricInstance> localEducationAgencyMetricInstanceRepository)
        {
            this.localEducationAgencyMetricInstanceRepository = localEducationAgencyMetricInstanceRepository;
        }

        public int Get(int localEducationAgencyId)
        {
            int year;
            if (localEducationAgencyMaxYear.TryGetValue(localEducationAgencyId, out year))
                return year;

            var yearQuery = from lea in localEducationAgencyMetricInstanceRepository.GetAll()
                            where lea.LocalEducationAgencyId == localEducationAgencyId
                            select new { lea.LocalEducationAgencyId, lea.SchoolYear };
            year = yearQuery.Max(q => q.SchoolYear);
            localEducationAgencyMaxYear.TryAdd(localEducationAgencyId, year);
            return year;
        }
    }
}
