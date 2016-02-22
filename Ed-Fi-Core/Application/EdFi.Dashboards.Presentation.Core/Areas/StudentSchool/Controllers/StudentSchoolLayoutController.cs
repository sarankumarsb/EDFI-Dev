// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using EdFi.Dashboards.Common;
using EdFi.Dashboards.Core.Providers.Context;
using EdFi.Dashboards.Presentation.Core.Utilities;
using EdFi.Dashboards.Resource.Models.Common;
using EdFi.Dashboards.Resources;
using EdFi.Dashboards.Resources.Metric;
using EdFi.Dashboards.Resources.Models.Student;
using EdFi.Dashboards.Resources.StudentSchool;

namespace EdFi.Dashboards.Presentation.Core.Areas.StudentSchool.Controllers
{
    public class StudentSchoolLayoutController : Controller
    {
        private readonly IRootMetricNodeResolver rootMetricNodeResolver;
        private readonly IService<BriefRequest, BriefModel> studentBriefService;
        private readonly IService<ResourcesRequest, IEnumerable<ResourceModel>> studentSchoolMenuResourceRequestService;

        public StudentSchoolLayoutController(
            IRootMetricNodeResolver rootMetricNodeResolver,
            IService<BriefRequest, BriefModel> studentBriefService,
            IService<ResourcesRequest, IEnumerable<ResourceModel>> studentSchoolMenuResourceRequestService)
        {
            this.rootMetricNodeResolver = rootMetricNodeResolver;
            this.studentBriefService = studentBriefService;
            this.studentSchoolMenuResourceRequestService = studentSchoolMenuResourceRequestService;
        }

        [ChildActionOnly]
        public ActionResult StudentSchoolHeader()
        {
            var studentBriefModel = GetStudentBriefModel();

            //Find all of the UI Accommodations for a student.
            ViewBag.StudentUiAccommodations = 
                studentBriefModel.Accommodations
                .Select(accommodationName => GetUiAccommodationsThatAreAllowedInHeader().SingleOrDefault(x => string.Equals(accommodationName.ToString(), x.Name)))
                .Where(accommodation => accommodation != null)
                .ToList();

            return PartialView(studentBriefModel);
        }

        protected virtual IEnumerable<UiAccommodation> GetUiAccommodationsThatAreAllowedInHeader()
        {
            yield return UiAccommodation.GiftedAndTalented;
            yield return UiAccommodation.SpecialEducation;
            yield return UiAccommodation.ESLAndLEP;
            yield return UiAccommodation.Repeater;
            yield return UiAccommodation.LateEnrollment;
            yield return UiAccommodation.PartialTranscript;
            yield return UiAccommodation.TestAccommodation;
            yield return UiAccommodation.Designation;
        }

        private BriefModel GetStudentBriefModel()
        {
            var studentUSI = EdFiDashboardContext.Current.StudentUSI ?? 0;
            int schoolId = EdFiDashboardContext.Current.SchoolId ?? 0;

            var studentBriefModel = studentBriefService.Get(
                new BriefRequest
                    {
                        StudentUSI = studentUSI,
                        SchoolId = schoolId,
                    });

            return studentBriefModel;
        }

        [ChildActionOnly]
        public ActionResult Title(string subTitle)
        {
            string title = GetStudentBriefModel().FullName + " - " + subTitle;
            return Content(title);
        }

        [ChildActionOnly]
        public ActionResult Menu()
        {
            var listContext = Request.QueryString["listContext"];
            var overviewNode = rootMetricNodeResolver.GetRootMetricNode();
            int schoolId = EdFiDashboardContext.Current.SchoolId.Value;
            var studentUSI = EdFiDashboardContext.Current.StudentUSI.Value;

            var resourceModels =
                studentSchoolMenuResourceRequestService.Get(ResourcesRequest.Create(studentUSI, schoolId, listContext, overviewNode));

            var menu = MenuHelper.MapResourcesModelsToMenus(resourceModels);

            foreach (var menuItem in menu)
                menuItem.SetSelectedState();

            return PartialView(menu);
        }

        public class UiAccommodation
        {
            public static UiAccommodation GiftedAndTalented = new UiAccommodation(Accommodations.GiftedAndTalented.DisplayName, "gifted-and-talented");
            public static UiAccommodation SpecialEducation = new UiAccommodation(Accommodations.SpecialEducation.DisplayName, "special-education");
            public static UiAccommodation Designation = new UiAccommodation(Accommodations.Designation.DisplayName, "504-designation");
            public static UiAccommodation ESLAndLEP = new UiAccommodation(Accommodations.ESLAndLEP.DisplayName, "esl");
            public static UiAccommodation Repeater = new UiAccommodation(Accommodations.Repeater.DisplayName, "repeater");
            public static UiAccommodation LateEnrollment = new UiAccommodation(Accommodations.LateEnrollment.DisplayName, "late-enrollment");
            public static UiAccommodation PartialTranscript = new UiAccommodation(Accommodations.PartialTranscript.DisplayName, "partial-transcript");
            public static UiAccommodation TestAccommodation = new UiAccommodation(Accommodations.TestAccommodation.DisplayName, "test-ccommodation");


            public string Name { get; set; }
            public string IconClass { get; set; }


            public UiAccommodation(string name, string iconClass)
            {
                Name = name;
                IconClass = iconClass;
            }
        }
    }
}
