// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System.Collections.Generic;
using System.Web.Mvc;
using EdFi.Dashboards.Presentation.Web.Utilities;
using EdFi.Dashboards.Resources.Models.CustomGrid;
using EdFi.Dashboards.Resources.School;
using EdFi.Dashboards.Resources.Staff;
using IStaffService = EdFi.Dashboards.Resources.School.IStaffService;
using StaffRequest = EdFi.Dashboards.Resources.School.StaffRequest;

namespace EdFi.Dashboards.Presentation.Core.Areas.School.Controllers
{
    public class StaffController : Controller
    {
        private readonly IStaffService service;
        private readonly IListMetadataProvider listMetadataProvider;
        private readonly IMetadataListIdResolver metadataListIdResolver;
        private readonly ISchoolCategoryProvider schoolCategoryProvider;

        public StaffController(IStaffService service, IListMetadataProvider listMetadataProvider, IMetadataListIdResolver metadataListIdResolver, ISchoolCategoryProvider schoolCategoryProvider)
        {
            this.service = service;
            this.listMetadataProvider = listMetadataProvider;
            this.metadataListIdResolver = metadataListIdResolver;
            this.schoolCategoryProvider = schoolCategoryProvider;
        }

        public ViewResult Get(int schoolId, int localEducationAgencyId)
        {
            var model = service.Get(new StaffRequest() { SchoolId = schoolId });
            var gridTable = new GridTable();
            
            var metadata = listMetadataProvider.GetListMetadata(metadataListIdResolver.GetListId(ListType.Staff, schoolCategoryProvider.GetSchoolCategoryType(schoolId)));
            gridTable.Columns = metadata.GenerateHeader();
            gridTable.Rows = metadata.GenerateRows(model.Staff, schoolId);
            
            return View(gridTable);
        }
    }
}
