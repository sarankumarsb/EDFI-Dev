// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System.Linq;
using System.Web.Mvc;
using EdFi.Dashboards.Common;
using EdFi.Dashboards.Core.Providers.Context;
using EdFi.Dashboards.Presentation.Core.Areas.Staff.Models;
using EdFi.Dashboards.Resources;
using EdFi.Dashboards.Resources.Models.Staff;
using EdFi.Dashboards.Resources.Security.Common;
using EdFi.Dashboards.Resources.Staff;
using EdFi.Dashboards.SecurityTokenService.Authentication;

namespace EdFi.Dashboards.Presentation.Core.Areas.Staff.Controllers
{
    public class StaffLayoutController : Controller
    {
        private readonly IService<StaffRequest, StaffModel> staffService;
        private readonly ICurrentUserClaimInterrogator currentUserClaimInterrogator;
        //private readonly StaffModel staffModel;

        public StaffLayoutController(IService<StaffRequest, StaffModel> staffService, ICurrentUserClaimInterrogator currentUserClaimInterrogator)
        {
            this.staffService = staffService;
            this.currentUserClaimInterrogator = currentUserClaimInterrogator;
        }

        [ChildActionOnly]
        public ActionResult Title(string subTitle)
        {
            var staffModel = GetStaffModel();

            string title = staffModel.FullName + " - " + subTitle;
            return Content(title);
        }

        [ChildActionOnly]
        public ActionResult StaffHeader()
        {
            var staffModel = GetStaffModel();

            return PartialView(staffModel);
        }

        [ChildActionOnly]
        public ActionResult Menu()
        {
            string assessmentSubType = Url.RequestContext.RouteData.Values["assessmentSubType"].ToString();
            var staffModel = GetStaffModel();
            var staffMenuModel = new StaffMenuModel(staffModel)
            {
                AssessmentSubTypeValue = assessmentSubType
            };

            //because we use the same Subject Area names under Assessments (broken up by different Assessment SubTypes), we need to make 
            // sure that the wrong view is not selected
            foreach (var view in staffMenuModel.Views.Where(view => view.Selected && !string.IsNullOrEmpty(view.MenuSubType) && string.Compare(view.MenuSubType, assessmentSubType, true) != 0))
            {
                view.Selected = false;
            }

            return PartialView(staffMenuModel);
        }

        #region Private Methods
        private StaffModel GetStaffModel()
        {
            //get a staff model for our various controls
            long staffUSI = EdFiDashboardContext.Current.StaffUSI ?? 0;
            int schoolId = EdFiDashboardContext.Current.SchoolId ?? 0;
            long sectionOrCohortId = EdFiDashboardContext.Current.SectionOrCohortId ?? 0;
            string studentListType = EdFiDashboardContext.Current.StudentListType;
            string viewType = EdFiDashboardContext.Current.ViewType ?? "GeneralOverview";
            string subjectArea = EdFiDashboardContext.Current.SubjectArea;
            string assessmentSubType = Url.RequestContext.RouteData.Values["assessmentSubType"].ToString();

            var staffModel = staffService.Get(new StaffRequest
            {
                StaffUSI = staffUSI,
                SchoolId = schoolId,
                StudentListType = studentListType,
                SectionOrCohortId = sectionOrCohortId,
                ViewType = viewType,
                SubjectArea = subjectArea,
                AssessmentSubType = assessmentSubType
            });

            if (staffModel != null && staffModel.Schools != null && staffModel.Schools.Any())
            {
                var currentUserInfo = UserInformation.Current;
                if (currentUserInfo.StaffUSI != staffUSI && !currentUserClaimInterrogator.HasClaimForLocalEducationAgencyWithinEducationOrganizationHierarchy(EdFiClaimTypes.ViewAllTeachers, schoolId))
                    staffModel.Schools = staffModel.Schools.Where(x => currentUserClaimInterrogator.HasClaimWithinEducationOrganizationHierarchy(EdFiClaimTypes.ViewAllTeachers, x.SchoolId)).ToList();
            }

            return staffModel;
        }
        #endregion
    }
}
