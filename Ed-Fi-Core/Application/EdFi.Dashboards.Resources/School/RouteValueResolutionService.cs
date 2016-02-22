// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System.Linq;
using EdFi.Dashboards.Common;
using SubSonic.Extensions;
using EdFi.Dashboards.Data.Entities;
using EdFi.Dashboards.Data.Repository;
using EdFi.Dashboards.Resources.Security.Common;

namespace EdFi.Dashboards.Resources.School
{
    public class RouteValueResolutionRequest
    {
        public int LocalEducationAgencyId { get; set; }
        public string RouteValue { get; set; }

        /// <summary>
        /// Creates a <see cref="RouteValueResolutionRequest"/> instance.
        /// </summary>
        /// <param name="localEducationAgencyId"></param>
        /// <param name="routeValue">The value provided by the ASP.NET routing infrastructure.</param>
        /// <returns>An initialized <see cref="RouteValueResolutionRequest"/> instance.</returns>
        public static RouteValueResolutionRequest Create(int localEducationAgencyId, string routeValue)
        {
            return new RouteValueResolutionRequest { LocalEducationAgencyId = localEducationAgencyId, RouteValue = routeValue };
        }
    }

    public interface IRouteValueResolutionService : IService<RouteValueResolutionRequest, int> { }

    public class RouteValueResolutionService : IRouteValueResolutionService
    {
        private readonly IRepository<SchoolInformation> schoolInformationRepository;

        public RouteValueResolutionService(IRepository<SchoolInformation> schoolInformationRepository)
        {
            this.schoolInformationRepository = schoolInformationRepository;
        }

        [AuthenticationIgnore("Service method returns identifiers for schools based on school names, neither of which is sensitive data.")]
        public int Get(RouteValueResolutionRequest request)
        {
            string wildcardedSchoolName = request.RouteValue.Replace('-', '_');

            int schoolId =
                (from s in schoolInformationRepository.GetAll()
                 where s.LocalEducationAgencyId == request.LocalEducationAgencyId
                       && s.Name.Wildcard(wildcardedSchoolName)
                 select s.SchoolId)
                 .SingleOrDefault();

            return schoolId;
        }
    }
}
