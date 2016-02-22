// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System;
using System.Collections.Generic;
using System.Web.Mvc;
using EdFi.Dashboards.Common.Utilities;
using EdFi.Dashboards.Infrastructure;
using EdFi.Dashboards.Presentation.Architecture.Mvc.Controllers;
using EdFi.Dashboards.Presentation.Core.Models.Shared;
using EdFi.Dashboards.Resources.Common;
using EdFi.Dashboards.Resources.LocalEducationAgency;
using EdFi.Dashboards.Resources.Models.LocalEducationAgency;

namespace EdFi.Dashboards.Presentation.Core.Areas.LocalEducationAgency.Controllers
{
    public class SchoolCategoryListController : ServicePassthroughController<SchoolCategoryListRequest, IList<SchoolCategoryModel>>
    {
        private readonly IUniqueListIdProvider uniqueListIdProvider;
        private readonly ISessionStateProvider sessionStateProvider;
        //
        // GET: /LocalEducationAgency/SchoolCategoryList/
        public SchoolCategoryListController(IUniqueListIdProvider uniqueListIdProvider, 
            ISessionStateProvider sessionStateProvider) : base()
        {
            this.uniqueListIdProvider = uniqueListIdProvider;
            this.sessionStateProvider = sessionStateProvider;
        }

        public override ActionResult Get(SchoolCategoryListRequest request, int localEducationAgencyId)
        {
            // TODO: Deferred - localEducationAgencyId can be dropped after drilldowns are no longer using WebForms.
            // localEducationAgencyId is here to force model binding to populate it in context so that it 
            // can be provided to the metric Action urls so that the context is available in order to 
            // use the correct database connection (for multitenancy with multiple databases).  Otherwise,
            // when the drilldown is initiated on a WebForms URL, there is no local education agency context
            // provided, and the only way to get it would be to go to the database, which itself also needs
            // the local education agency context in order to select the correct connection, and into a loop
            // we go.  Once all website artifacts are using the MVC routing, this parameter could be dropped.

            var baseActionResult = base.Get(request, localEducationAgencyId);

            var model = this.ViewData.Model as IList<SchoolCategoryModel>;

            SaveListForPreviousNextControl(model);

            return baseActionResult;
        }

        protected void SaveListForPreviousNextControl(IList<SchoolCategoryModel> model)
        {
            var schoolIds = new List<long[]>();
            foreach (var schoolCategoryModel in model)
                foreach (var school in schoolCategoryModel.Schools)
                    schoolIds.Add(new[] { (long)school.SchoolId });

            var previousNextControl = new PreviousNextDataModel
            {
                ListUrl = IoC.Resolve<ICurrentUrlProvider>().Url.AbsoluteUri,
                ListType = "Schools",
                ListPersistenceUniqueId = uniqueListIdProvider.GetUniqueId(),
                TableId = "schoolCategoryListTable",
                EntityIdArray = schoolIds.ToArray(),
                ParameterNames = new[] { "schoolId" },
                MetricId = String.Empty,
                FromSearch = false
            };

            sessionStateProvider[previousNextControl.ListPersistenceUniqueId] = previousNextControl;
        }

    }
}
