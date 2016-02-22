// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using EdFi.Dashboards.Common;
using EdFi.Dashboards.Data.Entities;
using EdFi.Dashboards.Data.Repository;
using EdFi.Dashboards.Resources.Models.Student.Detail;
using EdFi.Dashboards.Resources.Security.Common;
using EdFi.Dashboards.SecurityTokenService.Authentication;

namespace EdFi.Dashboards.Resources.StudentSchool.Detail
{
    public class ClassAbsencesRequest
    {
        public long StudentUSI { get; set; }
        public int SchoolId { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClassAbsencesRequest"/> using the specified parameters.
        /// </summary>
        /// <returns>A new <see cref="ClassAbsencesRequest"/> instance.</returns>
        public static ClassAbsencesRequest Create(long studentUSI, int schoolId) 
		{
			return new ClassAbsencesRequest { StudentUSI = studentUSI, SchoolId = schoolId };
		}
	}

    public abstract class ClassAbsencesServiceBase<TRequest, TResponse> : IService<TRequest, TResponse>
        where TRequest : ClassAbsencesRequest
        where TResponse : ClassAbsencesModel, new()
    {
        protected const string Friday = "Friday";
        protected const int MondayPosition = 0;
        protected const int FridayPosition = 4;

        private readonly IList<string> daysOfTheWeek = new List<string> { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday" };
		protected IList<string> DaysOfTheWeek
		{
			get { return daysOfTheWeek; }
		}

        public IRepository<StudentMetricAbsencesByClass> Repository { get; set; }

        [CanBeAuthorizedBy(EdFiClaimTypes.ViewAllStudents, EdFiClaimTypes.ViewMyStudents)]
        public virtual TResponse Get(TRequest request)
        {
            var studentUSI = request.StudentUSI;
            int schoolId = request.SchoolId;

            var results = (from data in Repository.GetAll()
                           where data.StudentUSI == studentUSI && data.SchoolId == schoolId
                           group data by data.SubjectArea
                               into subjectAreaGroup
                               select new { ClassName = subjectAreaGroup.Key, Data = subjectAreaGroup }).ToList();

            var classAbsences = new TResponse
                                    {
                                        StudentUSI = studentUSI
                                    };
            foreach (var result in results.OrderBy(x => x.ClassName))
            {
                var c = new ClassAbsencesModel.Class(studentUSI) { Name = result.ClassName };
                classAbsences.Classes.Add(c);

                var daysOfTheWeekPosition = MondayPosition;
                ClassAbsencesModel.Class.Week week = null;
                DateTime lastDate = DateTime.MinValue;
                foreach (var day in result.Data.OrderBy(x => x.DateValue))
                {
                    if (daysOfTheWeekPosition == MondayPosition)
                    {
                        if (week != null)
                            week.EndDate = lastDate;

                        week = new ClassAbsencesModel.Class.Week(studentUSI) { StartDate = day.DateValue };
                        c.Weeks.Add(week);
                    }

                    // It's possible to schools to be in session on a weekend due to events like bad weather days.
                    // While it's valid data, the UI doesn't support it, but the code shouldn't throw an exception.
                    if (!daysOfTheWeek.Contains(day.DayOfTheWeek))
                        continue;

                    while (daysOfTheWeek[daysOfTheWeekPosition] != day.DayOfTheWeek)
                    {
                        // do some stuff to fill in the gaps
                        if (week != null)
                        {
                            week.WeekDayEvents.Add(new ClassAbsencesModel.Class.Week.WeekDayEvent(studentUSI)
                            {
                                AttendanceEventType =
                                    ClassAbsencesModel.AttendanceEvent.NoData
                            });

                            if (daysOfTheWeekPosition == FridayPosition)
                            {
                                week.EndDate = lastDate;
                                daysOfTheWeekPosition = MondayPosition;
                                week = new ClassAbsencesModel.Class.Week(studentUSI) { StartDate = day.DateValue };
                                c.Weeks.Add(week);
                            }
                            else
                                daysOfTheWeekPosition++;
                        }
                    }

                    if (week != null)
                        week.WeekDayEvents.Add(new ClassAbsencesModel.Class.Week.WeekDayEvent(studentUSI)
                        {
                            Date = day.DateValue,
                            AttendanceEventType =
                                day.AttendanceEventDescriptorTypeId.HasValue
                                    ? (ClassAbsencesModel.AttendanceEvent)
                                      day.AttendanceEventDescriptorTypeId
                                    : ClassAbsencesModel.AttendanceEvent.NoData,
                            Reason = day.AttendanceEventReason
                        });

                    lastDate = day.DateValue;
                    if (day.DayOfTheWeek == Friday)
                        daysOfTheWeekPosition = MondayPosition;
                    else
                        daysOfTheWeekPosition++;
                }
                if (week != null)
                    week.EndDate = lastDate;
            }

            return classAbsences;
        }
    }

    public class ClassAbsencesService : ClassAbsencesServiceBase<ClassAbsencesRequest, ClassAbsencesModel>
    {
    }
}
