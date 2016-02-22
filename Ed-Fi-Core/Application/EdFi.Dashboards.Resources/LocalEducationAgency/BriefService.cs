// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System.Linq;
using EdFi.Dashboards.Common;
using EdFi.Dashboards.Data.Entities;
using EdFi.Dashboards.Resources.Models.LocalEducationAgency;
using EdFi.Dashboards.Resources.Navigation;
using EdFi.Dashboards.Data.Repository;
using EdFi.Dashboards.Resources.Security.Common;
using EdFi.Dashboards.SecurityTokenService.Authentication;

namespace EdFi.Dashboards.Resources.LocalEducationAgency
{
    public class BriefRequest
    {
        public int LocalEducationAgencyId { get; set; }
        
        public static BriefRequest Create(int localEducationAgencyId)
        {
            return new BriefRequest { LocalEducationAgencyId = localEducationAgencyId };
        }
    }

    public interface IBriefService : IService<BriefRequest, BriefModel> {}

    public class BriefService : IBriefService
    {
        private const string noLocalEducationAgencyFound = "No local education agency found.";
        private readonly IRepository<LocalEducationAgencyInformation> repository;
        private readonly ILocalEducationAgencyAreaLinks localEducationAgencyAreaLinks;

        public BriefService(IRepository<LocalEducationAgencyInformation> repository, ILocalEducationAgencyAreaLinks localEducationAgencyAreaLinks)
        {
            this.repository = repository;
            this.localEducationAgencyAreaLinks = localEducationAgencyAreaLinks;
        }

        [CanBeAuthorizedBy(EdFiClaimTypes.AccessOrganization)]
        public BriefModel Get(BriefRequest request)
        {
            var localEducationAgencyData = repository.GetAll().SingleOrDefault(x => x.LocalEducationAgencyId == request.LocalEducationAgencyId);
            var model = new BriefModel
                            {
                                LocalEducationAgencyId = -1,
                                Name = noLocalEducationAgencyFound,
                                ProfileThumbnail = localEducationAgencyAreaLinks.ProfileThumbnail(request.LocalEducationAgencyId)
                            };

            if (localEducationAgencyData == null)
                return model;

            model.Name = localEducationAgencyData.Name;
            model.LocalEducationAgencyId = localEducationAgencyData.LocalEducationAgencyId;
            model.ProfileThumbnail = localEducationAgencyAreaLinks.ProfileThumbnail(request.LocalEducationAgencyId);

            model.Url = localEducationAgencyAreaLinks.Default(localEducationAgencyData.LocalEducationAgencyId);

            return model;
        }
    }
}
