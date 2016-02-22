// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using EdFi.Dashboards.Common;
using EdFi.Dashboards.Core.Providers.Context;
using EdFi.Dashboards.Presentation.Core.Models.Shared;
using EdFi.Dashboards.Presentation.Core.Utilities;
using EdFi.Dashboards.Resource.Models.Common;
using EdFi.Dashboards.Resources;
using EdFi.Dashboards.Resources.Metric;
using EdFi.Dashboards.Resources.Models.School;
using EdFi.Dashboards.Resources.Navigation;
using EdFi.Dashboards.Resources.School;
using EdFi.Dashboards.SecurityTokenService.Authentication;

namespace EdFi.Dashboards.Presentation.Core.Areas.School.Controllers
{
    public class SchoolLayoutController : Controller
    {
        private readonly IService<BriefRequest, BriefModel> schoolBriefService;
        private readonly IService<ResourcesRequest, IEnumerable<ResourceModel>> schoolMenuResourceRequestService;
        private readonly IRootMetricNodeResolver rootMetricNodeResolver;
        private readonly IDomainSpecificMetricNodeResolver domainSpecificMetricNodeResolver;
        private readonly ISchoolAreaLinks schoolLinks;

        public SchoolLayoutController(IService<BriefRequest, BriefModel> schoolBriefService,
            IRootMetricNodeResolver rootMetricNodeResolver,
            IDomainSpecificMetricNodeResolver domainSpecificMetricNodeResolver,
            ISchoolAreaLinks schoolLinks,
            IService<ResourcesRequest, IEnumerable<ResourceModel>> schoolMenuResourceRequestService)
        {
            this.schoolBriefService = schoolBriefService;
            this.rootMetricNodeResolver = rootMetricNodeResolver;
            this.domainSpecificMetricNodeResolver = domainSpecificMetricNodeResolver;
            this.schoolLinks = schoolLinks;
            this.schoolMenuResourceRequestService = schoolMenuResourceRequestService;
        }

        [ChildActionOnly]
        public ActionResult EducationOrganizationHeader()
        {
            var schoolBriefModel = GetSchoolBriefModel();

            var model = new EducationOrganizationHeaderModel
                            {
                                Id = schoolBriefModel.SchoolId,
                                Name = schoolBriefModel.Name,
                                ProfileThumbnail = schoolBriefModel.ProfileThumbnail
                            };

            var userInfo = UserInformation.Current;
            if (userInfo != null)
            {
                var schools = userInfo.AssociatedSchools;
                if (schools.Count() > 1)
                {
                    foreach(var school in schools.OrderBy(x=> x.Name))
                        model.AssociatedEducationOrganizations.Add(new EducationOrganizationHeaderModel.AssociatedEducationOrganization{Name = school.Name, Href = schoolLinks.Default(school.SchoolId) });
                }
            }

            return PartialView(model);
        }

        private BriefModel GetSchoolBriefModel()
        {
            int schoolId = EdFiDashboardContext.Current.SchoolId ?? 0;

            var schoolBriefModel = schoolBriefService.Get(new BriefRequest {SchoolId = schoolId});
            return schoolBriefModel;
        }

        [ChildActionOnly]
        public ActionResult Title(string subTitle)
        {
            string title = GetSchoolBriefModel().Name + " - " + subTitle;
            return Content(title);
        }

        [ChildActionOnly]
        public ActionResult Menu()
        {
            var listContext = Request.QueryString["listContext"];
            var overviewNode = rootMetricNodeResolver.GetRootMetricNode();
            var operationalDashboardNode = domainSpecificMetricNodeResolver.GetOperationalDashboardMetricNode();

            if (!EdFiDashboardContext.Current.SchoolId.HasValue)
            { 
                throw new Exception("SchoolId Must be in context when utilizing the SchoolLayoutController");
            }

            int schoolId = EdFiDashboardContext.Current.SchoolId.Value;

            var resourceModels =
                schoolMenuResourceRequestService.Get(ResourcesRequest.Create(schoolId, listContext, overviewNode, operationalDashboardNode));

            var menu = MenuHelper.MapResourcesModelsToMenus(resourceModels);

            foreach (var menuItem in menu)
                menuItem.SetSelectedState();

            return PartialView(menu);
        }
    }
}
