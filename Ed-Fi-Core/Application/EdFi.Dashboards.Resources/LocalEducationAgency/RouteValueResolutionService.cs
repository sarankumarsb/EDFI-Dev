// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System.Linq;
using EdFi.Dashboards.Common;
using EdFi.Dashboards.Data.Repository;
using EdFi.Dashboards.Resources.Security.Common;
using SubSonic.Extensions;

namespace EdFi.Dashboards.Resources.LocalEducationAgency
{
    public class RouteValueResolutionRequest
    {
        public string RouteValue { get; set; }

        /// <summary>
        /// Creates a <see cref="RouteValueResolutionRequest"/> instance.
        /// </summary>
        /// <param name="routeValue">The value provided by the ASP.NET routing infrastructure.</param>
        /// <returns>An initialized <see cref="RouteValueResolutionRequest"/> instance.</returns>
        public static RouteValueResolutionRequest Create(string routeValue)
        {
            return new RouteValueResolutionRequest { RouteValue = routeValue };
        }
    }

    public interface IRouteValueResolutionService : IService<RouteValueResolutionRequest, int> { }

    public class RouteValueResolutionService : IRouteValueResolutionService
    {
        private readonly IRepository<EdFi.Dashboards.Application.Data.Entities.LocalEducationAgency> repository;

        public RouteValueResolutionService(IRepository<EdFi.Dashboards.Application.Data.Entities.LocalEducationAgency> repository)
        {
            this.repository = repository;
        }

        [AuthenticationIgnore("This service merely translates route values containing a representation of the LEA Name to the internal Local Education Agency Ids.")]
        public int Get(RouteValueResolutionRequest request)
        {
            int localEducationAgencyId;

            string localEducationAgencyName = request.RouteValue;
            string wildcardedLocalEducationAgencyName = request.RouteValue.Replace('-', '_');

            if (localEducationAgencyName == wildcardedLocalEducationAgencyName)
            {
                // Search using equality
                localEducationAgencyId =
                    (from s in repository.GetAll()
                     where s.Code == localEducationAgencyName
                     select s.LocalEducationAgencyId)
                     .SingleOrDefault();
            }
            else
            {
                // GKM: This query grabs ALL the districts and then iterates through them, performing a regex match 
                // (that corresponds to the SQL wildcard expression calculated above).  Not sure why it isn't using a DB wildcard search...
                localEducationAgencyId =
                    (from s in repository.GetAll()
                     where s.Code.Wildcard(wildcardedLocalEducationAgencyName)
                     select s.LocalEducationAgencyId)
                     .SingleOrDefault();
            }

            return localEducationAgencyId;
        }
    }
}
