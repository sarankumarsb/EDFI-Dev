// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System;
using System.Linq;
using System.Reflection;
using System.Web.Caching;
using EdFi.Dashboards.Application.Data.Entities;
using EdFi.Dashboards.Data.Repository;
using EdFi.Dashboards.Infrastructure;
using EdFi.Dashboards.Resources.Security.Common;
using EdFi.Dashboards.SecurityTokenService.Authentication;

namespace EdFi.Dashboards.Resources.Application
{
    public class SiteAvailableProvider : ISiteAvailableProvider, IHasCachedResult
    {
        private readonly IRepository<LocalEducationAgencyAdministration> repository;
        private readonly ICacheProvider cacheProvider;
        private readonly IConfigValueProvider configValueProvider;
        private readonly ICurrentUserClaimInterrogator currentUserClaimInterrogator;
        private readonly ICacheKeyGenerator cacheKeyGenerator;

        public const string ConfigValueName = "siteAvailableCacheDuration";

        public SiteAvailableProvider(IRepository<LocalEducationAgencyAdministration> repository, 
            ICacheProvider cacheProvider, IConfigValueProvider configValueProvider, 
            ICurrentUserClaimInterrogator currentUserClaimInterrogator,
            ICacheKeyGenerator cacheKeyGenerator)
        {
            this.repository = repository;
            this.cacheProvider = cacheProvider;
            this.configValueProvider = configValueProvider;
            this.currentUserClaimInterrogator = currentUserClaimInterrogator;
            this.cacheKeyGenerator = cacheKeyGenerator;
        }

        public bool IsKillSwitchActivatedForCurrentUser(int localEducationAgencyId)
        {
            var userInfo = UserInformation.Current;

            if (currentUserClaimInterrogator.HasClaimForLocalEducationAgencyWithinEducationOrganizationHierarchy(EdFiClaimTypes.AdministerDashboard, localEducationAgencyId) 
                || userInfo.HasIdentityClaim(EdFiClaimTypes.Impersonating))
                return false;


            string cacheKey = GetCacheKey(localEducationAgencyId);

            object result;

            if (cacheProvider.TryGetCachedObject(cacheKey, out result))
                return (bool)result;

            var administration = (from a in repository.GetAll()
                        where a.LocalEducationAgencyId == localEducationAgencyId
                        select a).FirstOrDefault();

            if (administration == null)
                return false;

            // TODO: Caching logic should exist outside of the provider implementation
            int cacheDuration = Convert.ToInt32(configValueProvider.GetValue(ConfigValueName));

            if (cacheDuration == 0)
                cacheDuration = (int)TimeSpan.FromMinutes(5).TotalSeconds;

            cacheProvider.Insert(cacheKey, administration.IsKillSwitchActivated.GetValueOrDefault(), DateTime.Now.AddSeconds(cacheDuration), Cache.NoSlidingExpiration);

            return administration.IsKillSwitchActivated.GetValueOrDefault();
        }

        private static MethodInfo cachedMethodInfo;

        public string GetCacheKey(params object[] arguments)
        {
            if (arguments.Length != 1)
                throw new ArgumentException("Cache key creation failure because the local education agency Id is the only argument that should have been passed.");

            if (cachedMethodInfo == null)
                cachedMethodInfo = this.GetType().GetMethod("IsKillSwitchActivatedForCurrentUser");

            return cacheKeyGenerator.GenerateCacheKey(cachedMethodInfo, arguments);
        }
    }
}
