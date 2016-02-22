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
    public class BriefLocalEducationAgencyStaffRequest
    {
        public int LocalEducationAgencyId { get; set; }
        public long StaffUSI { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="BriefRequest"/> class using the local education agency and staff identifiers.
        /// </summary>
        /// <param name="localEducationAgencyId">The identifier of the local education agency associated with the staff member.</param>
        /// <param name="staffUSI">The identifier of the staff member.</param>
        /// <returns>The newly created request object.</returns>
        public static BriefLocalEducationAgencyStaffRequest Create(int localEducationAgencyId, long staffUSI)
        {
            return new BriefLocalEducationAgencyStaffRequest { LocalEducationAgencyId = localEducationAgencyId, StaffUSI = staffUSI };
        }
    }

    public interface IBriefLocalEducationAgencyStaffService : IService<BriefLocalEducationAgencyStaffRequest, BriefModel> { }

    public class BriefLocalEducationAgencyStaffService : IBriefLocalEducationAgencyStaffService
    {
        private readonly IRepository<StaffInformation> staffInformationRepository;
        private readonly IStaffAreaLinks staffLinks;

        public BriefLocalEducationAgencyStaffService(IRepository<StaffInformation> staffInformationRepository, IStaffAreaLinks staffLinks)
        {
            this.staffInformationRepository = staffInformationRepository;
            this.staffLinks = staffLinks;
        }

        [CanBeAuthorizedBy(EdFiClaimTypes.AccessOrganization)]
        [CacheInitializer(typeof(StaffLocalEducationAgencyStaffBriefServiceCacheInitializer))]
        public BriefModel Get(BriefLocalEducationAgencyStaffRequest request)
        {
            var staffInformation = (from si in staffInformationRepository.GetAll()
                            where si.StaffUSI == request.StaffUSI
                            select si).SingleOrDefault();
            
            //We introduced this logic here to mitage the issue of a user being associated with  the application but not in the StaffInformation table.
            //Note: This logic parallels the logic in the SecurityAssertionProvider.LocalEducationAgencyMustBeAssociatedWithStaff();
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
            model.Url = staffLinks.LocalEducationAgencyCustomStudentList(request.LocalEducationAgencyId, request.StaffUSI, staffInformation.FullName);

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
    public class StaffLocalEducationAgencyStaffBriefServiceCacheInitializer : ICacheInitializer
    {
        private readonly IRepository<StaffInformation> staffInformationRepository;
        private readonly IRepository<StaffEducationOrgInformation> staffEducationOrgInformationRepository;
        private readonly IConfigValueProvider configValueProvider;
        private readonly IStaffAreaLinks staffLinks;
        private readonly ICacheKeyGenerator cacheKeyGenerator;

        public StaffLocalEducationAgencyStaffBriefServiceCacheInitializer(IRepository<StaffInformation> staffInformationRepository, 
                                                                          IRepository<StaffEducationOrgInformation> staffEducationOrgInformationRepository,
                                                                          IConfigValueProvider configValueProvider, 
                                                                          IStaffAreaLinks staffLinks,
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
            var request = arguments[0] as BriefLocalEducationAgencyStaffRequest;

            if (request == null)
                throw new ArgumentException(
                    string.Format("Unable to initialize cache for '{0}.{1}' because the request object was null, or not of type '{2}'.",
                                  methodInvocationTarget.DeclaringType.Name, methodInvocationTarget.Name, typeof(BriefLocalEducationAgencyStaffRequest).Name));

            // Initialize automapper mappings
            BriefService.EnsureMappingsInitialized();

            var staffInformationEntities =
                (from si in staffInformationRepository.GetAll()
                 join seo in staffEducationOrgInformationRepository.GetAll() on si.StaffUSI equals seo.StaffUSI
                 where seo.EducationOrganizationId == request.LocalEducationAgencyId
                 select si);

            int cacheExpiryMinutes = Convert.ToInt32(configValueProvider.GetValue("CacheInterceptor.SlidingExpiration"));

            // Initialize all the values with the cache
            foreach (var staffInformation in staffInformationEntities)
            {
                var briefModel = Mapper.Map<BriefModel>(staffInformation);
                briefModel.Url = staffLinks.LocalEducationAgencyCustomStudentList(request.LocalEducationAgencyId, request.StaffUSI, staffInformation.FullName);

                BriefLocalEducationAgencyStaffRequest cacheRequest = BriefLocalEducationAgencyStaffRequest.Create(request.LocalEducationAgencyId, briefModel.StaffUSI);
                string cacheKey = cacheKeyGenerator.GenerateCacheKey(methodInvocationTarget, new object[] { cacheRequest });

                cacheProvider.Insert(cacheKey, briefModel, Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(cacheExpiryMinutes));
            }
        }
    }
}
