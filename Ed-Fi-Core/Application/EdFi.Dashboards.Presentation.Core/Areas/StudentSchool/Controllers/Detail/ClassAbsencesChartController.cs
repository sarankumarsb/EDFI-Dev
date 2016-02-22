// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System;
using System.Web.Mvc;
using EdFi.Dashboards.Common;
using EdFi.Dashboards.Core.Providers.Context;
using EdFi.Dashboards.Resources;
using EdFi.Dashboards.Resources.Models.Student.Detail;
using EdFi.Dashboards.Resources.StudentSchool.Detail;

namespace EdFi.Dashboards.Presentation.Core.Areas.StudentSchool.Controllers.Detail
{
    public class ClassAbsencesChartController : Controller
    {
        private readonly IService<ClassAbsencesRequest, ClassAbsencesModel> classAbsencesService;

        public ClassAbsencesChartController(IService<ClassAbsencesRequest, ClassAbsencesModel> classAbsencesService)
        {
            this.classAbsencesService = classAbsencesService;
        }

        public ActionResult Get(EdFiDashboardContext context)
        {
            var request = new ClassAbsencesRequest()
                              {
                                  SchoolId = context.SchoolId.GetValueOrDefault(),
                                  StudentUSI = context.StudentUSI.GetValueOrDefault()
                              };
            var model = classAbsencesService.Get(request);

            return View(model);
        }
        public static string GetDayTooltip(ClassAbsencesModel.AttendanceEvent attendanceEvent, DateTime date, string reason)
        {
            switch (attendanceEvent)
            {
                case ClassAbsencesModel.AttendanceEvent.Unexcused:
                    return String.Format("Unexcused Absence on {0}", date.ToShortDateString());
                case ClassAbsencesModel.AttendanceEvent.Excused:
                    return String.Format("Excused Absence:({0}) {1}", date.ToShortDateString(), reason);
                case ClassAbsencesModel.AttendanceEvent.NonInstructional:
                    return String.Format("{0} (non instructional)", date.ToShortDateString());
                case ClassAbsencesModel.AttendanceEvent.Tardy:
                    return String.Format("Tardy on {0}", date.ToShortDateString());
                case ClassAbsencesModel.AttendanceEvent.NoData:
                    return String.Empty;
            }

            return date.ToShortDateString();
        }
        public static string GetDayText(ClassAbsencesModel.AttendanceEvent attendanceEvent)
        {
            switch (attendanceEvent)
            {
                case ClassAbsencesModel.AttendanceEvent.Present:
                case ClassAbsencesModel.AttendanceEvent.Excused:
                case ClassAbsencesModel.AttendanceEvent.Unexcused:
                case ClassAbsencesModel.AttendanceEvent.Tardy:
                    return attendanceEvent.ToString();
                case ClassAbsencesModel.AttendanceEvent.NonInstructional:
                    return "Non Instructional";
                case ClassAbsencesModel.AttendanceEvent.NoData:
                    break;
                default:
                    throw new ArgumentOutOfRangeException("attendanceEvent");
            }

            return String.Empty;
        }

    }
}
