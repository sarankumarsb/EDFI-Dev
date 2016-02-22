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

namespace EdFi.Dashboards.Presentation.Core.Areas.Staff.Controllers
{
    public class SubjectSpecificOverviewController : Controller
    {
        private readonly IService<SubjectSpecificOverviewRequest, SubjectSpecificOverviewModel> service;

        public SubjectSpecificOverviewController(IService<SubjectSpecificOverviewRequest, SubjectSpecificOverviewModel> service)
        {
            this.service = service;
        }

        public ActionResult Get(SubjectSpecificOverviewRequest request)
        {
            var results = service.Get(request);

            //Constructing the Grid Data.
            var model = new StaffStudentListModel { GridTable = new GridTable() };

            if (results.ListMetadata.Any())
            {
                model.GridTable.Columns = results.ListMetadata.GenerateHeader();
                model.GridTable.Rows = results.ListMetadata.GenerateRows(results.Students.ToList<StudentWithMetrics>(), results.UniqueListId);
            }

            //if this is the current users own page
            model.IsCurrentUserListOwner = UserInformation.Current.StaffUSI == request.StaffUSI;
            model.IsCustomStudentList = request.StudentListType == StudentListType.CustomStudentList.ToString();
            model.ListId = request.SectionOrCohortId;
            model.LegendViewNames = new List<string> {"Default"};

            return View(model);
        }
    }
}
