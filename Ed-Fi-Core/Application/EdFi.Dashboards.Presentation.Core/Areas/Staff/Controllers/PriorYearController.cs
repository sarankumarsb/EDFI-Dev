// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using EdFi.Dashboards.Common;
using EdFi.Dashboards.Presentation.Core.Areas.Staff.Models.Shared;
using EdFi.Dashboards.Presentation.Web.Utilities;
using EdFi.Dashboards.Resources;
using EdFi.Dashboards.Resources.Models.Common;
using EdFi.Dashboards.Resources.Models.CustomGrid;
using EdFi.Dashboards.Resources.Models.Staff;
using EdFi.Dashboards.Resources.Models.Student;
using EdFi.Dashboards.Resources.Staff;
using EdFi.Dashboards.SecurityTokenService.Authentication;
using EdFi.Dashboards.Warehouse.Resource.Models.Staff;
using EdFi.Dashboards.Warehouse.Resources.Staff;

namespace EdFi.Dashboards.Presentation.Core.Areas.Staff.Controllers
{
    public class PriorYearController : Controller
    {
        private readonly IService<PriorYearRequest, PriorYearModel> service;
        private readonly IService<DefaultSectionRequest, DefaultSectionModel> defaultSectionService;

        public PriorYearController(IService<PriorYearRequest, PriorYearModel> service, IService<DefaultSectionRequest, DefaultSectionModel> defaultSectionService)
        {
            this.service = service;
            this.defaultSectionService = defaultSectionService;
        }

        public ActionResult Get(long staffUSI, int schoolId, string studentListType, long? sectionOrCohortId, int localEducationAgencyId)
        {
            //make sure StudentListType and SectionCohortId are populated
            if (studentListType == StudentListType.None.ToString())
            {
                var defaultSectionModel = defaultSectionService.Get(new DefaultSectionRequest
                                                                    {
                                                                        SchoolId = schoolId,
                                                                        StaffUSI = staffUSI,
                                                                        SectionOrCohortId = sectionOrCohortId ?? 0,
                                                                        StudentListType = studentListType,
                                                                        Staff = ControllerContext.RouteData.Values["staff"].ToString(),
                                                                        ViewType = StaffModel.ViewType.PriorYear
                                                                    });

                if (defaultSectionModel.ListType != StudentListType.None.ToString())
                    return Redirect(defaultSectionModel.Link);
            }

            var request = new PriorYearRequest()
                              {
                                  LocalEducationAgencyId = localEducationAgencyId,
                                  StaffUSI = staffUSI,
                                  SchoolId = schoolId,
                                  SectionOrCohortId = sectionOrCohortId ?? 0,
                                  StudentListType = studentListType
                              };

            var resourceModel = service.Get(request);

            //Constructing the Grid Data.
            var model = new StaffStudentListModel();
            model.GridTable = new GridTable();

            if (resourceModel.ListMetadata.Any())
            {
                //Grouping headers and underlying columns.
                model.GridTable.Columns = resourceModel.ListMetadata.GenerateHeader();
                //Create the rows.
                model.GridTable.Rows = resourceModel.ListMetadata.GenerateRows(resourceModel.Students.ToList<StudentWithMetrics>(), resourceModel.UniqueListId);
            }
            //if this is the current users own page
            model.IsCurrentUserListOwner = UserInformation.Current.StaffUSI == staffUSI;
            model.IsCustomStudentList = request.StudentListType == StudentListType.CustomStudentList.ToString();
            model.ListId = request.SectionOrCohortId;
            model.LegendViewNames = new List<string> {"Default"};

            return View(model);
        }
    }
}
