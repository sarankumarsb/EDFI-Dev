// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System.Collections.Generic;
using System.Web.Mvc;
using EdFi.Dashboards.Common;
using EdFi.Dashboards.Core.Providers.Context;
using EdFi.Dashboards.Presentation.Core.Models.Shared;
using EdFi.Dashboards.Presentation.Core.Utilities;
using EdFi.Dashboards.Resource.Models.Common;
using EdFi.Dashboards.Resources;
using EdFi.Dashboards.Resources.LocalEducationAgency;
using EdFi.Dashboards.Resources.Models.LocalEducationAgency;
using ResourcesRequest = EdFi.Dashboards.Resources.Admin.ResourcesRequest;

namespace EdFi.Dashboards.Presentation.Core.Areas.Admin.Controllers
{
    public class AdminLayoutController : Controller
    {
        private readonly IService<BriefRequest, BriefModel> localEducationAgencyBriefService;
        private readonly IService<ResourcesRequest, IEnumerable<ResourceModel>> adminMenuResourceRequestService;

        public AdminLayoutController(
            IService<BriefRequest, BriefModel> localEducationAgencyBriefService,
            IService<ResourcesRequest, IEnumerable<ResourceModel>> adminMenuResourceRequestService)
        {
            this.localEducationAgencyBriefService = localEducationAgencyBriefService;
            this.adminMenuResourceRequestService = adminMenuResourceRequestService;
        }

        private EducationOrganizationHeaderModel GetEducationOrganizationBriefModel()
        {
            int localEducationAgencyId = EdFiDashboardContext.Current.LocalEducationAgencyId ?? 0;

            var localEducationAgencyBrief =
                localEducationAgencyBriefService.Get(new BriefRequest { LocalEducationAgencyId = localEducationAgencyId });

            var model = new EducationOrganizationHeaderModel
            {
                Id = localEducationAgencyBrief.LocalEducationAgencyId,
                Name = localEducationAgencyBrief.Name,
                ProfileThumbnail = localEducationAgencyBrief.ProfileThumbnail
            };
            return model;
        }

        [ChildActionOnly]
        public ActionResult EducationOrganizationHeader()
        {
            return PartialView(GetEducationOrganizationBriefModel());
        }

        [ChildActionOnly]
        public ActionResult Title(string subTitle)
        {
            string title = GetEducationOrganizationBriefModel().Name + " - " + subTitle;
            return Content(title);
        }

        [ChildActionOnly]
        public ActionResult Menu()
        {
            int localEducationAgencyId = EdFiDashboardContext.Current.LocalEducationAgencyId ?? 0;

            var resourceModels =
                adminMenuResourceRequestService.Get(ResourcesRequest.Create(localEducationAgencyId));

            var menu = MenuHelper.MapResourcesModelsToMenus(resourceModels);

            foreach (var menuItem in menu)
                menuItem.SetSelectedState();

            return PartialView(menu);
        }
    }
}
