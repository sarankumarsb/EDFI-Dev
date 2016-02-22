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
using EdFi.Dashboards.Resources.Models.Staff;
using AutoMapper;
using EdFi.Dashboards.Resources.Navigation;
using EdFi.Dashboards.Resources.Security.Common;
using EdFi.Dashboards.SecurityTokenService.Authentication;

namespace EdFi.Dashboards.Resources.Staff
{
    public class BriefRequest
    {
        public int SchoolId { get; set; }
        public long StaffUSI { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="BriefRequest"/> class using the school and staff identifiers.
        /// </summary>
        /// <param name="schoolId">The identifier of the school associated with the staff member.</param>
        /// <param name="staffUSI">The identifier of the staff member.</param>
        /// <returns>The newly created request object.</returns>
        public static BriefRequest Create(int schoolId, long staffUSI)
        {
            return new BriefRequest {SchoolId = schoolId, StaffUSI = staffUSI};
        }
    }

    public interface IBriefService : IService<BriefRequest, BriefModel> { }

    public class BriefService : IBriefService
    {
        private readonly IRepository<StaffInformation> staffInformationRepository;
        private readonly IStaffAreaLinks staffLinks;

        public BriefService(IRepository<StaffInformation> staffInformationRepository, IStaffAreaLinks staffLinks)
        {
            this.staffInformationRepository = staffInformationRepository;
            this.staffLinks = staffLinks;
        }

        [CanBeAuthorizedBy(EdFiClaimTypes.AccessOrganization)]
        [CacheInitializer(typeof(StaffBriefServiceCacheInitializer))]
        public BriefModel Get(BriefRequest request)
        {
            var staffInformation = (from si in staffInformationRepository.GetAll()
                            where si.StaffUSI == request.StaffUSI
                            select si).SingleOrDefault();

            //We introduced this logic here to mitage the issue of a user being associated with  the application but not in the StaffInformation table.
            //Note: This logic parallels the logic in the SecurityAssertionProvider.SchoolMustBeAssociatedWithStaff();
            var currentUser = UserInformation.Current;
            if (staffInformation == null && request.StaffUSI == currentUser.StaffUSI)
            {
                staffInformation = new StaffInformation
                                       {
                                           StaffUSI = currentUser.StaffUSI,
                                           FullName = currentUser.FullName,
                                           FirstName = currentUser.FirstName,
                                           LastSurname = currentUser.LastName,
                                           EmailAddress = currentUser.EmailAddress
                                       };
            }

            if (staffInformation == null)
				throw new InvalidOperationException(string.Format("Could not locate Staff with USI:{0}", request.StaffUSI));

            EnsureMappingsInitialized();
            var model = Mapper.Map<BriefModel>(staffInformation);
            model.Url = staffLinks.Default(request.SchoolId, request.StaffUSI, staffInformation.FullName);

            return model;
        }

        private static bool isMappingInitialized;

        public static void EnsureMappingsInitialized()
        {
            if (!isMappingInitialized)
            {
                //Define the mappings.
                Mapper.CreateMap(typeof(StaffInformation), typeof(BriefModel));

                isMappingInitialized = true;
            }
        }
    }

    /// <summary>
    /// Initializes the cache with all the staff members for a school, based on a request for a single staff member at a school.
    /// </summary>
    public class StaffBriefServiceCacheInitializer : ICacheInitializer
    {
        private readonly IRepository<StaffInformation> staffInformationRepository;
        private readonly IRepository<StaffEducationOrgInformation> staffEducationOrgInformationRepository;
        private readonly IConfigValueProvider configValueProvider;
        private readonly IStaffAreaLinks staffLinks;
        private readonly ICacheKeyGenerator cacheKeyGenerator;

        public StaffBriefServiceCacheInitializer(IRepository<StaffInformation> staffInformationRepository, 
            IRepository<StaffEducationOrgInformation> staffEducationOrgInformationRepository,
            IConfigValueProvider configValueProvider, IStaffAreaLinks staffLinks,
            ICacheKeyGenerator cacheKeyGenerator)
        { 
            this.staffInformationRepository = staffInformationRepository;
            this.staffEducationOrgInformationRepository = staffEducationOrgInformationRepository;
            this.configValueProvider = configValueProvider;
            this.staffLinks = staffLinks;
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
            var request = arguments[0] as BriefRequest;

            if (request == null)
                throw new ArgumentException(
                    string.Format("Unable to initialize cache for '{0}.{1}' because the request object was null, or not of type '{2}'.",
                                  methodInvocationTarget.DeclaringType.Name, methodInvocationTarget.Name, typeof(BriefRequest).Name));

            // Initialize automapper mappings
            BriefService.EnsureMappingsInitialized();

            var staffInformationEntities =
                (from si in staffInformationRepository.GetAll()
                 join seo in staffEducationOrgInformationRepository.GetAll() on si.StaffUSI equals seo.StaffUSI
                 where seo.EducationOrganizationId == request.SchoolId
                 select si);

            int cacheExpiryMinutes = Convert.ToInt32(configValueProvider.GetValue("CacheInterceptor.SlidingExpiration"));

            // Initialize all the values with the cache
            foreach (var staffInformation in staffInformationEntities)
            {
                var briefModel = Mapper.Map<BriefModel>(staffInformation);
                briefModel.Url = staffLinks.Default(request.SchoolId, staffInformation.StaffUSI, staffInformation.FullName);
 
                BriefRequest cacheRequest = BriefRequest.Create(request.SchoolId, briefModel.StaffUSI);
                string cacheKey = cacheKeyGenerator.GenerateCacheKey(methodInvocationTarget, new object[] { cacheRequest });

                cacheProvider.Insert(cacheKey, briefModel, Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(cacheExpiryMinutes));
            }
        }
    }
}
