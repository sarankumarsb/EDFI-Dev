using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EdFi.Dashboards.Common;
using EdFi.Dashboards.Data.Repository;
using EdFi.Dashboards.Infrastructure.Implementations.Caching;
using EdFi.Dashboards.Resources.Models.Application;
using EdFi.Dashboards.Resources.Navigation;
using EdFi.Dashboards.Resources.Security.Common;

namespace EdFi.Dashboards.Resources.Application
{
    public class HomeRequest
    {
    }

    public interface IHomeService : IService<HomeRequest, HomeModel> { }

    public class HomeService : IHomeService
    {
        private readonly IRepository<EdFi.Dashboards.Application.Data.Entities.LocalEducationAgency> repository;
        private readonly ILocalEducationAgencyAreaLinks localEducationAgencyAreaLinks;

        public HomeService(IRepository<EdFi.Dashboards.Application.Data.Entities.LocalEducationAgency> repository, ILocalEducationAgencyAreaLinks localEducationAgencyAreaLinks)
        {
            this.repository = repository;
            this.localEducationAgencyAreaLinks = localEducationAgencyAreaLinks;
        }

        [AuthenticationIgnore("Local Education Agency Name is a public record.")] 
        [NoCache]
        public HomeModel Get(HomeRequest request)
        {
            var model = new HomeModel();

            model.LocalEducationAgencies = (from lea in repository.GetAll()
                                            orderby lea.Name
                                             select new HomeModel.LocalEducationAgency
                                             {
                                                 LocalEducationAgencyCode = lea.Code,
                                                 LocalEducationAgencyName = lea.Name,
                                             }).ToList();

            foreach (var localEducationAgency in model.LocalEducationAgencies)
                localEducationAgency.HomeUrl = localEducationAgencyAreaLinks.Home(localEducationAgency.LocalEducationAgencyCode);

            return model;
        }
    }
}
