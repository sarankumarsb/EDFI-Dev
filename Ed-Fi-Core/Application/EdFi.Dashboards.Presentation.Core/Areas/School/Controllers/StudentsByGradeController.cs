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
using EdFi.Dashboards.Resources.Models.School;
using EdFi.Dashboards.Resources.School;

namespace EdFi.Dashboards.Presentation.Core.Areas.School.Controllers
{
    public class StudentsByGradeController : ServicePassthroughController<StudentsByGradeRequest, StudentsByGradeModel>
    {
        private readonly IUniqueListIdProvider uniqueListIdProvider;

        public StudentsByGradeController(IUniqueListIdProvider uniqueListIdProvider)
        {
            this.uniqueListIdProvider = uniqueListIdProvider;
        }

        public override ActionResult Get(StudentsByGradeRequest request, int localEducationAgencyId)
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

            var model = this.ViewData.Model as StudentsByGradeModel;

            SaveListForPreviousNextControl(model);

            return baseActionResult;
        }

        protected void SaveListForPreviousNextControl(StudentsByGradeModel model)
        {
            var studentUSIs = new List<long[]>();
            foreach (var grades in model.Grades)
                foreach (var student in grades.Students)
                    studentUSIs.Add(new[] { student.StudentUSI, model.SchoolId });

            var previousNextControl = new PreviousNextDataModel
            {
                ListUrl = IoC.Resolve<ICurrentUrlProvider>().Url.AbsoluteUri,
                ListType = "StudentList",
                ListPersistenceUniqueId = uniqueListIdProvider.GetUniqueId() + model.SchoolId,
                TableId = "studentListTable",
                EntityIdArray = studentUSIs.ToArray(),
                ParameterNames = new[] { "studentUSI", "schoolId" },
                MetricId = String.Empty,
                FromSearch = false
            };

            var session = IoC.Resolve<ISessionStateProvider>();
            session[previousNextControl.ListPersistenceUniqueId] = previousNextControl;
        }
    }
}
