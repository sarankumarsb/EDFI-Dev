// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using EdFi.Dashboards.Common;
using EdFi.Dashboards.Resources.Navigation;
using EdFi.Dashboards.Resources.Navigation.Mvc;
using EdFi.Dashboards.Resources.Models.Common;
using EdFi.Dashboards.Resources.Security.Common;

namespace EdFi.Dashboards.Resources.Common
{
    public class LevelCrumbRequest
    {
        public int? LocalEducationAgencyId { get; set; }
        public int? SchoolId { get; set; }
        public long? StaffUSI { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="LevelCrumbRequest"/> using the specified parameters.
        /// </summary>
        /// <returns>A new <see cref="LevelCrumbRequest"/> instance.</returns>
        public static LevelCrumbRequest Create(int? localEducationAgencyId, int? schoolId, long? staffUSI) 
        {
            return new LevelCrumbRequest { LocalEducationAgencyId = localEducationAgencyId, SchoolId = schoolId, StaffUSI = staffUSI };
        }
    }

    public interface ILevelCrumbService : IService<LevelCrumbRequest, LevelCrumbModel> { }

    public class LevelCrumbService : ILevelCrumbService
    {
        private readonly IService<LocalEducationAgency.BriefRequest, Models.LocalEducationAgency.BriefModel> localEducationAgencyBriefService;
        private readonly IService<School.BriefRequest, Models.School.BriefModel> schoolBriefService;
        private readonly IService<Staff.BriefRequest, Models.Staff.BriefModel> staffBriefService;

        public LevelCrumbService(IService<LocalEducationAgency.BriefRequest, Models.LocalEducationAgency.BriefModel> localEducationAgencyBriefService,
            IService<School.BriefRequest, Models.School.BriefModel> schoolBriefService,
            IService<Staff.BriefRequest, Models.Staff.BriefModel> staffBriefService)
        {
            this.schoolBriefService = schoolBriefService;
            this.staffBriefService = staffBriefService;
            this.localEducationAgencyBriefService = localEducationAgencyBriefService;
        }

        [CanBeAuthorizedBy(AuthorizationDelegate.LevelCrumb)]
        public LevelCrumbModel Get(LevelCrumbRequest request)
        {
            var model = new LevelCrumbModel
                            {
                                HomeHref = null, // This is user-specific and cannot be set due to the caching on this service
                                HomeIconHref = EdFiDashboards.Site.Common.ThemeImage("Breadcrumb/Home.gif").Resolve(),
                            };

            if (request.LocalEducationAgencyId.HasValue)
                model.LocalEducationAgencyBriefModel = localEducationAgencyBriefService.Get(LocalEducationAgency.BriefRequest.Create(request.LocalEducationAgencyId.Value));

            if (request.SchoolId.HasValue)
                model.SchoolBriefModel = schoolBriefService.Get(School.BriefRequest.Create(request.SchoolId.Value));

            //Logic for staff...
            if (request.StaffUSI.HasValue && request.SchoolId.HasValue)
                model.BriefModel = staffBriefService.Get(Staff.BriefRequest.Create(request.SchoolId.Value, request.StaffUSI.Value));

            return model;
        }
    }
}
