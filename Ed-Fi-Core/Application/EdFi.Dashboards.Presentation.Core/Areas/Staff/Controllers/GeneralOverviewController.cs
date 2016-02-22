// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using EdFi.Dashboards.Common;
using EdFi.Dashboards.Presentation.Core.Controllers;
using EdFi.Dashboards.Presentation.Core.Areas.Staff.Models.Shared;
using EdFi.Dashboards.Presentation.Core.Providers.Session;
using EdFi.Dashboards.Resources;
using EdFi.Dashboards.Resources.Common;
using EdFi.Dashboards.Resources.Models.Common;
using EdFi.Dashboards.Resources.Models.CustomGrid;
using EdFi.Dashboards.Resources.Models.Staff;
using EdFi.Dashboards.Resources.Navigation;
using EdFi.Dashboards.Resources.Staff;
using EdFi.Dashboards.SecurityTokenService.Authentication;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace EdFi.Dashboards.Presentation.Core.Areas.Staff.Controllers
{
    public class GeneralOverviewController : EdFiGridBaseController
    {
        protected readonly IService<DefaultSectionRequest, DefaultSectionModel> DefaultSectionService;

        public GeneralOverviewController(
            IService<EdFiGridMetaRequest, EdFiGridModel> gridMetaService,
            IService<EdFiGridRequest, EdFiGridModel> gridService,
            IPreviousNextSessionProvider previousNextSessionProvider,
            IService<DefaultSectionRequest, DefaultSectionModel> defaultSectionService,
            IMetricsBasedWatchListDataProvider watchListProvider,
            ILocalEducationAgencyAreaLinks localEducationAgencyAreaLinks)
            : base(gridMetaService, gridService, previousNextSessionProvider, watchListProvider, localEducationAgencyAreaLinks)
        {
            DefaultSectionService = defaultSectionService;
        }

        public ActionResult Get(GeneralOverviewMetaRequest request)
        {
            //make sure StudentListType and SectionCohortId are populated
            if (request.StudentListType == StudentListType.None.ToString())
            {
                var defaultSectionModel = DefaultSectionService.Get(new DefaultSectionRequest
                                                                      {
                                                                          SchoolId = request.SchoolId,
                                                                          StaffUSI = request.StaffUSI,
                                                                          SectionOrCohortId = request.SectionOrCohortId.GetValueOrDefault(),
                                                                          StudentListType = request.StudentListType,
                                                                          Staff = Convert.ToString(ControllerContext.RouteData.Values.GetValueOrDefault("staff")),
                                                                          ViewType = StaffModel.ViewType.GeneralOverview
                                                                      });

                if (defaultSectionModel.ListType != StudentListType.None.ToString())
                    return Redirect(defaultSectionModel.Link);        
            }

            var parentModel = GetGridData(request);
            var model = new StaffStudentListModel
            {
                GridTable = parentModel.GridTable,
                IsCurrentUserListOwner = UserInformation.Current.StaffUSI == request.StaffUSI,
                IsCustomStudentList = request.StudentListType == StudentListType.CustomStudentList.ToString(),
                ListId = request.SectionOrCohortId.GetValueOrDefault(),
                LegendViewNames = new List<string> {"Default"}
            };

            return View(model);
        }
    }
}
