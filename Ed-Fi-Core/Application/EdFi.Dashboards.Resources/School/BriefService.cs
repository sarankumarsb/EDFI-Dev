// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System.Linq;
using EdFi.Dashboards.Common;
using EdFi.Dashboards.Data.Entities;
using EdFi.Dashboards.Resources.Models.School;
using EdFi.Dashboards.Resources.Navigation;
using EdFi.Dashboards.Data.Repository;
using EdFi.Dashboards.Resources.Security.Common;
using EdFi.Dashboards.SecurityTokenService.Authentication;

namespace EdFi.Dashboards.Resources.School
{
    public class BriefRequest
    {
        public int SchoolId { get; set; }

        public static BriefRequest Create(int schoolId)
        {
            return new BriefRequest {SchoolId = schoolId};
        }
    }

    public interface IBriefService : IService<BriefRequest, BriefModel> {}

    public class BriefService : IBriefService
    {
        private readonly IRepository<SchoolInformation> schoolInformationRepository;
        private readonly ISchoolAreaLinks schoolAreaLinks;

        public BriefService(IRepository<SchoolInformation> schoolInformationRepository, ISchoolAreaLinks schoolAreaLinks)
        {
            this.schoolAreaLinks = schoolAreaLinks;
            this.schoolInformationRepository = schoolInformationRepository;
        }

        [CanBeAuthorizedBy(EdFiClaimTypes.AccessOrganization)]
        public BriefModel Get(BriefRequest request)
        {
            var schoolData = schoolInformationRepository.GetAll().SingleOrDefault(x => x.SchoolId == request.SchoolId);
            var model = new BriefModel
                            {
                                LocalEducationAgencyId = -1,
                                SchoolId = -1,
                                Name = "No school found.",
                                ProfileThumbnail = schoolAreaLinks.ProfileThumbnail(request.SchoolId).Resolve()
                            };

            if (schoolData == null)
                return model;

            model.Name = schoolData.Name;
            model.SchoolId = schoolData.SchoolId;
            model.ProfileThumbnail = schoolAreaLinks.ProfileThumbnail(request.SchoolId).Resolve();
            model.LocalEducationAgencyId = schoolData.LocalEducationAgencyId;

            model.Url = schoolAreaLinks.Default(schoolData.SchoolId, schoolData.Name);

            return model;
        }
    }
}
