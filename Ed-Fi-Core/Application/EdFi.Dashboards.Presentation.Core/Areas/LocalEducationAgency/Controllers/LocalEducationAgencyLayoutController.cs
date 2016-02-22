// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System.Collections.Generic;
using System.Web.Mvc;
using EdFi.Dashboards.Common;
using EdFi.Dashboards.Core;
using EdFi.Dashboards.Core.Providers.Context;
using EdFi.Dashboards.Presentation.Core.Models.Shared;
using EdFi.Dashboards.Presentation.Core.Utilities;
using EdFi.Dashboards.Resource.Models.Common;
using EdFi.Dashboards.Resources;
using EdFi.Dashboards.Resources.Constants;
using EdFi.Dashboards.Resources.LocalEducationAgency;
using EdFi.Dashboards.Resources.Metric;
using EdFi.Dashboards.Resources.Models.LocalEducationAgency;

namespace EdFi.Dashboards.Presentation.Core.Areas.LocalEducationAgency.Controllers
{
    public class LocalEducationAgencyLayoutController : Controller
    {
        private readonly IService<BriefRequest, BriefModel> localEducationAgencyBriefService;
        private readonly IService<ResourcesRequest, IEnumerable<ResourceModel>> localEducationAgencyMenuResourceRequestService;
        private readonly IRootMetricNodeResolver rootMetricNodeResolver;
        private readonly IDomainSpecificMetricNodeResolver domainSpecificMetricNodeResolver;

        public LocalEducationAgencyLayoutController(IService<BriefRequest, BriefModel> localEducationAgencyBriefService,
            IRootMetricNodeResolver rootMetricNodeResolver,
            IDomainSpecificMetricNodeResolver domainSpecificMetricNodeResolver,
            IService<ResourcesRequest, IEnumerable<ResourceModel>> localEducationAgencyMenuResourceRequestService)
        {
            this.localEducationAgencyBriefService = localEducationAgencyBriefService;
            this.rootMetricNodeResolver = rootMetricNodeResolver;
            this.domainSpecificMetricNodeResolver = domainSpecificMetricNodeResolver;
            this.localEducationAgencyMenuResourceRequestService = localEducationAgencyMenuResourceRequestService;
        }

        [ChildActionOnly]
        public ActionResult EducationOrganizationHeader()
        {
            var model = GetEducationOrganizationBriefModel();

            return PartialView(model);
        }

        private EducationOrganizationHeaderModel GetEducationOrganizationBriefModel()
        {
            int localEducationAgencyId = EdFiDashboardContext.Current.LocalEducationAgencyId ?? 0;

            var localEducationAgencyBrief =
                localEducationAgencyBriefService.Get(new BriefRequest
                                                         {LocalEducationAgencyId = localEducationAgencyId});

            var model = new EducationOrganizationHeaderModel
                            {
                                Id = localEducationAgencyBrief.LocalEducationAgencyId,
                                Name = localEducationAgencyBrief.Name,
                                ProfileThumbnail = localEducationAgencyBrief.ProfileThumbnail
                            };
            return model;
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
			var listContext = Request.QueryString[QueryStringParameters.ListContext];
			var filters = Request.QueryString[QueryStringParameters.Filters];

            var overviewNode = rootMetricNodeResolver.GetRootMetricNode();
            var operationalDashboardNode = domainSpecificMetricNodeResolver.GetOperationalDashboardMetricNode(MetricInstanceSetType.LocalEducationAgency);
            int localEducationAgencyId = EdFiDashboardContext.Current.LocalEducationAgencyId ?? 0;

	        var resourceModels = localEducationAgencyMenuResourceRequestService.Get(ResourcesRequest.Create(localEducationAgencyId, listContext, filters, overviewNode, operationalDashboardNode));

            var menu = MenuHelper.MapResourcesModelsToMenus(resourceModels);

            foreach (var menuItem in menu)
                menuItem.SetSelectedState();

            return PartialView(menu);
        }
    }
}
