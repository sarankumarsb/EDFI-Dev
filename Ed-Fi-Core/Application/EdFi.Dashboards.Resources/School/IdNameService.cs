// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System;
using System.Linq;
using System.Reflection;
using System.Web.Caching;
using EdFi.Dashboards.Common;
using EdFi.Dashboards.Data.Entities;
using EdFi.Dashboards.Data.Repository;
using EdFi.Dashboards.Infrastructure;
using EdFi.Dashboards.Infrastructure.Implementations.Caching;
using EdFi.Dashboards.Resources.Models.School;
using EdFi.Dashboards.Resources.Security.Common;

namespace EdFi.Dashboards.Resources.School
{
    public class IdNameRequest
    {
        public int SchoolId { get; set; }

        public static IdNameRequest Create(int schoolId)
        {
            return new IdNameRequest { SchoolId = schoolId };
        }
    }

    public interface IIdNameService : IService<IdNameRequest, IdNameModel> { }

    public class IdNameService : IIdNameService
    {
        private readonly IRepository<SchoolInformation> schoolInformationRepository;

        public IdNameService(IRepository<SchoolInformation> schoolInformationRepository)
        {
            this.schoolInformationRepository = schoolInformationRepository;
        }

        [AuthenticationIgnore("School Name is a public record.")]
        [CacheInitializer(typeof(SchoolIdNameServiceCacheInitializer))]
        public IdNameModel Get(IdNameRequest request)
        {
            var model =
                (from school in schoolInformationRepository.GetAll()
                where school.SchoolId == request.SchoolId
                select new IdNameModel
                {
                    LocalEducationAgencyId = school.LocalEducationAgencyId,
                    SchoolId = school.SchoolId,
                    Name = school.Name,
                    SchoolCategory = school.SchoolCategory
                })
                .SingleOrDefault();

            return model;
        }
    }

    /// <summary>
    /// Initializes the cache with all the schools for a local education agency, based on a request for a single school.
    /// </summary>
    public class SchoolIdNameServiceCacheInitializer : ICacheInitializer
    {
        private readonly IRepository<SchoolInformation> schoolInformationRepository;
        private readonly IConfigValueProvider configValueProvider;
        private readonly ICacheKeyGenerator cacheKeyGenerator;

        public SchoolIdNameServiceCacheInitializer(IRepository<SchoolInformation> schoolInformationRepository,
            IConfigValueProvider configValueProvider, ICacheKeyGenerator cacheKeyGenerator)
        {
            this.schoolInformationRepository = schoolInformationRepository;
            this.configValueProvider = configValueProvider;
            this.cacheKeyGenerator = cacheKeyGenerator;
        }

        public void InitializeCacheValues(ICacheProvider cacheProvider, MethodInfo methodInvocationTarget, object[] arguments)
        {
            if (methodInvocationTarget == null)
                throw new ArgumentNullException("methodInvocationTarget");

            if (arguments == null || arguments.Length != 1)
                throw new ArgumentException(
                    string.Format("Unable to initialize cache for '{0}.{1}' because there were an unexpected number of arguments passed to the method.", methodInvocationTarget.DeclaringType.Name, methodInvocationTarget.Name));

            // Get school Id
            var request = arguments[0] as IdNameRequest;

            if (request == null)
                throw new ArgumentException(
                    string.Format("Unable to initialize cache for '{0}.{1}' because the request object was null, or not of type '{2}'.",
                                  methodInvocationTarget.DeclaringType.Name, methodInvocationTarget.Name, typeof(IdNameRequest).Name));

            var models =
                (from school in schoolInformationRepository.GetAll()
                    join school2 in schoolInformationRepository.GetAll() on school.LocalEducationAgencyId equals school2.LocalEducationAgencyId
                    where school2.SchoolId == request.SchoolId
                    select new IdNameModel
                            {
                                LocalEducationAgencyId = school.LocalEducationAgencyId,
                                Name = school.Name,
                                SchoolId = school.SchoolId,
                                SchoolCategory = school.SchoolCategory,
                            });

            int cacheExpiryMinutes = Convert.ToInt32(configValueProvider.GetValue("CacheInterceptor.SlidingExpiration"));

            // Initialize all the values with the cache
            foreach (var idNameModel in models)
            {
                IdNameRequest cacheRequest = IdNameRequest.Create(idNameModel.SchoolId);
                string cacheKey = cacheKeyGenerator.GenerateCacheKey(methodInvocationTarget, new object[] { cacheRequest });

                cacheProvider.Insert(cacheKey, idNameModel, Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(cacheExpiryMinutes));
            }
        }
    }
}
