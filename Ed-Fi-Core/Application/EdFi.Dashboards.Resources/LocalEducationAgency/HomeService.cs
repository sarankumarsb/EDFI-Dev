using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EdFi.Dashboards.Common;
using EdFi.Dashboards.Data.Repository;
using EdFi.Dashboards.Resources.Models.LocalEducationAgency;
using EdFi.Dashboards.Resources.Navigation;
using EdFi.Dashboards.Resources.Security.Common;

namespace EdFi.Dashboards.Resources.LocalEducationAgency
{
    public class HomeRequest
    {
        /// <summary>
        /// The identifier of the local education agency.
        /// </summary>
        public int LocalEducationAgencyId { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="HomeRequest"/> class with the specified local education agency identifier.
        /// </summary>
        /// <param name="localEducationAgencyId">The identifier of the local education agency.</param>
        /// <returns>The initialized request.</returns>
        public static HomeRequest Create(int localEducationAgencyId)
        {
            return new HomeRequest { LocalEducationAgencyId = localEducationAgencyId };
        }
    }

    public interface IHomeService : IService<HomeRequest, HomeModel> { }

    public class HomeService : IHomeService
    {
        private readonly IRepository<EdFi.Dashboards.Application.Data.Entities.LocalEducationAgency> repository;
        private readonly IRepository<EdFi.Dashboards.Application.Data.Entities.LocalEducationAgencySupport> supportRepository;
        private readonly ILocalEducationAgencyAreaLinks localEducationAgencyAreaLinks;

        public HomeService(IRepository<EdFi.Dashboards.Application.Data.Entities.LocalEducationAgency> repository, IRepository<EdFi.Dashboards.Application.Data.Entities.LocalEducationAgencySupport> supportRepository, ILocalEducationAgencyAreaLinks localEducationAgencyAreaLinks)
        {
            this.repository = repository;
            this.supportRepository = supportRepository;
            this.localEducationAgencyAreaLinks = localEducationAgencyAreaLinks;
        }

        [AuthenticationIgnore("Local Education Agency Name is a public record.")]
        public HomeModel Get(HomeRequest request)
        {
            var model =
                (from lea in repository.GetAll()
                 join support in supportRepository.GetAll()
                 on lea.LocalEducationAgencyId equals support.LocalEducationAgencyId
                 where lea.LocalEducationAgencyId == request.LocalEducationAgencyId
                 select new HomeModel
                 {
                     LocalEducationAgencyId = lea.LocalEducationAgencyId,
                     Name = lea.Name,
                     Code = lea.Code,
                     SupportContact = support.SupportContact,
                     SupportEmail = support.SupportEmail,
                     SupportPhone = support.SupportPhone,
                     TrainingAndPlanningHref = support.TrainingAndPlanningUrl
                 }).SingleOrDefault();

            model.LoginUrl = localEducationAgencyAreaLinks.Entry(model.Code);

            return model;
        }
    }
}
