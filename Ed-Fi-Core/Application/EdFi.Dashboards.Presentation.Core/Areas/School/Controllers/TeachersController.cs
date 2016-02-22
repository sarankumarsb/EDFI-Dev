// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using EdFi.Dashboards.Common;
using EdFi.Dashboards.Presentation.Web.Utilities;
using EdFi.Dashboards.Resources;
using EdFi.Dashboards.Resources.Models.CustomGrid;
using EdFi.Dashboards.Resources.Models.School;
using EdFi.Dashboards.Resources.School;
using EdFi.Dashboards.Resources.Staff;

namespace EdFi.Dashboards.Presentation.Core.Areas.School.Controllers
{
    public class TeachersController : Controller
    {
        private readonly IService<TeachersRequest, TeachersModel> service;
        private readonly IListMetadataProvider listMetadataProvider;
        private readonly IMetadataListIdResolver metadataListIdResolver;
        private readonly ISchoolCategoryProvider schoolCategoryProvider;

        public TeachersController(IService<TeachersRequest, TeachersModel> service, IListMetadataProvider listMetadataProvider, IMetadataListIdResolver metadataListIdResolver, ISchoolCategoryProvider schoolCategoryProvider)
        {
            this.service = service;
            this.listMetadataProvider = listMetadataProvider;
            this.metadataListIdResolver = metadataListIdResolver;
            this.schoolCategoryProvider = schoolCategoryProvider;
        }

        public ViewResult Get(int schoolId, int localEducationAgencyId)
        {
            var gridTable = new GridTable();

            var metadata = listMetadataProvider.GetListMetadata(metadataListIdResolver.GetListId(ListType.Teacher, schoolCategoryProvider.GetSchoolCategoryType(schoolId)));
            gridTable.Columns = metadata.GenerateHeader();

            TeachersModel teachersInSchool = service.Get(new TeachersRequest(){SchoolId = schoolId});
            gridTable.Rows = metadata.GenerateRows(teachersInSchool.Teachers.ToList(), schoolId);


            return View(gridTable);
        }
    }
}
