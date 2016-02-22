using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EdFi.Dashboards.Application.Data.Entities;
using EdFi.Dashboards.Data.Repository;
using EdFi.Dashboards.Infrastructure;

namespace EdFi.Dashboards.Resources.Search
{
    public interface ILocalEducationAgencySearchProvider
    {
        string[] GetEnabledStudentIdentificationSystems(int leaId);
        string[] GetEnabledStaffIdentificationSystems(int leaId);
    }

    public class LocalEducationAgencySearchProvider : ILocalEducationAgencySearchProvider
    {
        private readonly IPersistingRepository<LocalEducationAgencySearch> searchRepository;
        private readonly ICacheProvider cacheProvider;

        private string cacheKeyFormat =
            "LocalEducationAgencySearchProvider.GetEnabled{0}IdentificationSystems.{1}";

        public LocalEducationAgencySearchProvider(ICacheProvider cacheProvider, IPersistingRepository<LocalEducationAgencySearch> searchRepository)
        {
            this.cacheProvider = cacheProvider;
            this.searchRepository = searchRepository;
        }

        public string[] GetEnabledStudentIdentificationSystems(int leaId)
        {
            object cacheValue;
            string cacheKey = string.Format(cacheKeyFormat, "Student", leaId);
            if (!cacheProvider.TryGetCachedObject(cacheKey, out cacheValue))
            {
                cacheValue = searchRepository.GetAll()
                    .Where(x => x.SearchCategory == "Student" && x.LocalEducationAgencyId == leaId)
                    .Select(x => x.SystemCode)
                    .ToArray();
                cacheProvider.SetCachedObject(cacheKey, cacheValue);
            }
            return (string[])cacheValue;

        }

        public string[] GetEnabledStaffIdentificationSystems(int leaId)
        {
            object cacheValue;
            string cacheKey = string.Format(cacheKeyFormat, "Staff", leaId);
            if (!cacheProvider.TryGetCachedObject(cacheKey, out cacheValue))
            {
                cacheValue = searchRepository.GetAll()
                    .Where(x => x.SearchCategory == "Staff" && x.LocalEducationAgencyId == leaId)
                    .Select(x => x.SystemCode)
                    .ToArray();
                cacheProvider.SetCachedObject(cacheKey, cacheValue);
            }
            return (string[])cacheValue;      
        }
    }
}
