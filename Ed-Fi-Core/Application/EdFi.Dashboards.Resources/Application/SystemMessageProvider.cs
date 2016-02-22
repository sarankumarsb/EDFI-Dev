// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System;
using System.Linq;
using System.Reflection;
using EdFi.Dashboards.Application.Data.Entities;
using EdFi.Dashboards.Data.Repository;
using EdFi.Dashboards.Infrastructure;

namespace EdFi.Dashboards.Resources.Application
{
    public class SystemMessageProvider : ISystemMessageProvider, IHasCachedResult
    {
        private readonly IRepository<LocalEducationAgencyAdministration> repository;
        private readonly ICacheKeyGenerator cacheKeyGenerator;

        public SystemMessageProvider(IRepository<LocalEducationAgencyAdministration> repository, 
            ICacheKeyGenerator cacheKeyGenerator)
        {
            this.repository = repository;
            this.cacheKeyGenerator = cacheKeyGenerator;
        }

        public string GetSystemMessage(int localEducationAgencyId)
        {
            var localEducationAgencyAdminSettings = repository.GetAll().SingleOrDefault(x => x.LocalEducationAgencyId == localEducationAgencyId);
            if (localEducationAgencyAdminSettings == null)
                return null;
            return localEducationAgencyAdminSettings.SystemMessage;
        }

        private static MethodInfo cachedMethodInfo;

        public string GetCacheKey(params object[] arguments)
        {
            if (arguments.Length != 1)
                throw new ArgumentException("Cache key creation failure because the local education agency Id is the only argument that should have been passed.");

            if (cachedMethodInfo == null)
                cachedMethodInfo = this.GetType().GetMethod("GetSystemMessage");

            return cacheKeyGenerator.GenerateCacheKey(cachedMethodInfo, arguments);
        }
    }
}
