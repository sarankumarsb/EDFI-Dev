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
using EdFi.Dashboards.Infrastructure.Implementations;
using EdFi.Dashboards.Infrastructure.Implementations.Caching;
using EdFi.Dashboards.Resources.Models.Student;
using EdFi.Dashboards.Resources.Security.Common;
using EdFi.Dashboards.SecurityTokenService.Authentication;

namespace EdFi.Dashboards.Resources.StudentSchool
{
    public class IdNameRequest
    {
        public long StudentUSI { get; set; }
        public int SchoolId { get; set; }

        public static IdNameRequest Create(int schoolId, long studentUSI)
        {
            return new IdNameRequest { SchoolId = schoolId, StudentUSI = studentUSI };
        }
    }

    public interface IIdNameService : IService<IdNameRequest, IdNameModel> { }

    public class IdNameService : IIdNameService
    {
        private readonly IRepository<StudentInformation> studentInformationRepository;
        private readonly IRepository<StudentSchoolInformation> studentSchooldInformationRepository;

        public IdNameService(IRepository<StudentInformation> studentInformationRepository, IRepository<StudentSchoolInformation> studentSchoolInformationRepository)
        {
            this.studentInformationRepository = studentInformationRepository;
            this.studentSchooldInformationRepository = studentSchoolInformationRepository;
        }

        [CanBeAuthorizedBy(EdFiClaimTypes.ViewAllStudents, EdFiClaimTypes.ViewMyStudents)]
        [CacheInitializer(typeof(StudentSchoolIdNameCacheInitialization))]
        public IdNameModel Get(IdNameRequest request)
        {
            var model =
                (from student in studentInformationRepository.GetAll()
                 join school in studentSchooldInformationRepository.GetAll()
                 on student.StudentUSI equals school.StudentUSI
                 where student.StudentUSI == request.StudentUSI && school.SchoolId == request.SchoolId
                    && school.SchoolId == request.SchoolId
                 select new IdNameModel
                 {
                     StudentUSI = student.StudentUSI,
                     FullName = student.FullName,
                 })
                .SingleOrDefault();

            return model;
        }
    }

    public class StudentSchoolIdNameCacheInitialization : ICacheInitializer
    {
        private readonly IRepository<StudentInformation> studentInformationRepository;
        private readonly IRepository<StudentSchoolInformation> studentSchoolInformationRepository;
        private readonly IConfigValueProvider configValueProvider;
        private readonly ICacheKeyGenerator cacheKeyGenerator;

        public StudentSchoolIdNameCacheInitialization(IRepository<StudentInformation> studentInformationRepository, 
            IRepository<StudentSchoolInformation> studentSchoolInformationRepository,
            IConfigValueProvider configValueProvider, ICacheKeyGenerator cacheKeyGenerator)
        {
            this.studentInformationRepository = studentInformationRepository;
            this.studentSchoolInformationRepository = studentSchoolInformationRepository;
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

            // Get request object
            var request = arguments[0] as IdNameRequest;

            if (request == null)
                throw new ArgumentException(
                    string.Format("Unable to initialize cache for '{0}.{1}' because the request object was null, or not of type '{2}'.",
                                  methodInvocationTarget.DeclaringType.Name, methodInvocationTarget.Name, typeof(IdNameRequest).Name));

            var models =
                (from student in studentInformationRepository.GetAll()
                 join school in studentSchoolInformationRepository.GetAll()
                 on student.StudentUSI equals school.StudentUSI
                 where school.SchoolId == request.SchoolId
                 select new IdNameModel
                 {
                     StudentUSI = student.StudentUSI,
                     FullName = student.FullName,
                 });

            int cacheExpiryMinutes = Convert.ToInt32(configValueProvider.GetValue("CacheInterceptor.SlidingExpiration"));

            // Initialize all the values with the cache
            foreach (var idNameModel in models)
            {
                IdNameRequest cacheRequest = IdNameRequest.Create(request.SchoolId, idNameModel.StudentUSI);
                string cacheKey = cacheKeyGenerator.GenerateCacheKey(methodInvocationTarget, new object[] { cacheRequest });

                cacheProvider.Insert(cacheKey, idNameModel, Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(cacheExpiryMinutes));
            }
        }
    }
}
