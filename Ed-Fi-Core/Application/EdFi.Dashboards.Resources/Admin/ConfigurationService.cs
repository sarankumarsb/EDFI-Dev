// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;
using System.Linq;
using EdFi.Dashboards.Application.Data.Entities;
using EdFi.Dashboards.Common;
using EdFi.Dashboards.Data.Entities;
using EdFi.Dashboards.Data.Repository;
using EdFi.Dashboards.Infrastructure;
using EdFi.Dashboards.Infrastructure.Implementations;
using EdFi.Dashboards.Infrastructure.Implementations.Caching;
using EdFi.Dashboards.Resources.Application;
using EdFi.Dashboards.Resources.Models.Admin;
using EdFi.Dashboards.Resources.Navigation;
using EdFi.Dashboards.Resources.Security.Common;
using EdFi.Dashboards.SecurityTokenService.Authentication;

namespace EdFi.Dashboards.Resources.Admin
{
    public class ConfigurationRequest
    {
        public int LocalEducationAgencyId { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationRequest"/> using the specified parameters.
        /// </summary>
        /// <returns>A new <see cref="ConfigurationRequest"/> instance.</returns>
        public static ConfigurationRequest Create(int localEducationAgencyId) 
        {
            return new ConfigurationRequest { LocalEducationAgencyId = localEducationAgencyId };
        }
    }

    public interface IConfigurationService : IService<ConfigurationRequest, ConfigurationModel>
    {
        void Post(ConfigurationModel configurationModel);
    }

    public class ConfigurationService : IConfigurationService
    {
        private readonly IPersistingRepository<LocalEducationAgencyAdministration> administrationRepository;
        private readonly IRepository<LocalEducationAgencyInformation> localEducationAgencyInformationRepository;
        private readonly IAdminAreaLinks adminAreaLinks;
        private readonly ICacheProvider cacheProvider;
        private readonly ISiteAvailableProvider siteAvailableProvider;
        private readonly ISystemMessageProvider systemMessageProvider;

        public ConfigurationService(IPersistingRepository<LocalEducationAgencyAdministration> administrationRepository,
                                IRepository<LocalEducationAgencyInformation> localEducationAgencyInformationRepository,
                                IAdminAreaLinks adminAreaLinks, ICacheProvider cacheProvider, 
                                ISiteAvailableProvider siteAvailableProvider, ISystemMessageProvider systemMessageProvider)
        {
            this.administrationRepository = administrationRepository;
            this.localEducationAgencyInformationRepository = localEducationAgencyInformationRepository;
            this.adminAreaLinks = adminAreaLinks;
            this.cacheProvider = cacheProvider;
            this.siteAvailableProvider = siteAvailableProvider;
            this.systemMessageProvider = systemMessageProvider;
        }

        [NoCache][CanBeAuthorizedBy(EdFiClaimTypes.AdministerDashboard)]
        public ConfigurationModel Get(ConfigurationRequest request)
        {
            int localEducationAgencyId = request.LocalEducationAgencyId;

            var localEducationAgencyData = localEducationAgencyInformationRepository.GetAll().SingleOrDefault(x => x.LocalEducationAgencyId == localEducationAgencyId);
            if (localEducationAgencyData == null)
                return new ConfigurationModel
                {
                    LocalEducationAgencyName = "No district found.",
                    LocalEducationAgencyId = -1,
                    Url = adminAreaLinks.Configuration(-1)
                };

            var adminConfig = administrationRepository.GetAll().SingleOrDefault(x => x.LocalEducationAgencyId == localEducationAgencyId) ??
                                                      new LocalEducationAgencyAdministration { LocalEducationAgencyId = localEducationAgencyId, IsKillSwitchActivated = false };

            return new ConfigurationModel
            {
                LocalEducationAgencyName = localEducationAgencyData.Name,
                LocalEducationAgencyId = adminConfig.LocalEducationAgencyId,
                IsKillSwitchActivated = adminConfig.IsKillSwitchActivated.GetValueOrDefault(),
                SystemMessage = adminConfig.SystemMessage,
                Url = adminAreaLinks.Configuration(localEducationAgencyId)
            };
        }

        [NoCache][CanBeAuthorizedBy(EdFiClaimTypes.AdministerDashboard)]
        public void Post(ConfigurationModel configurationModel)
        {
            if (configurationModel == null)
                throw new ArgumentNullException("configurationModel");

            var localEducationAgencyData = localEducationAgencyInformationRepository.GetAll().SingleOrDefault(x => x.LocalEducationAgencyId == configurationModel.LocalEducationAgencyId);
            if (localEducationAgencyData == null)
                throw new InvalidOperationException("Cannot save configuration for non-existent district. Local Education Agency Id: " + configurationModel.LocalEducationAgencyId);

            var adminConfig = administrationRepository.GetAll().SingleOrDefault(x => x.LocalEducationAgencyId == configurationModel.LocalEducationAgencyId);
            adminConfig.IsKillSwitchActivated = configurationModel.IsKillSwitchActivated;
            adminConfig.SystemMessage = configurationModel.SystemMessage;
            administrationRepository.Save(adminConfig);

            // Clear the cache entry for the site availability
            var cacheableSiteAvailableProvider = siteAvailableProvider as IHasCachedResult;

            if (cacheableSiteAvailableProvider != null)
            {
                string cacheKey = cacheableSiteAvailableProvider.GetCacheKey(localEducationAgencyData.LocalEducationAgencyId);
                cacheProvider.RemoveCachedObject(cacheKey);
            }

            // Clear the cache entry for the system message
            var cacheableSystemMessageProvider = systemMessageProvider as IHasCachedResult;

            if (cacheableSystemMessageProvider != null)
            {
                string cacheKey = cacheableSystemMessageProvider.GetCacheKey(localEducationAgencyData.LocalEducationAgencyId);
                cacheProvider.RemoveCachedObject(cacheKey);
            }
        }
    }
}
