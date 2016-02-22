using EdFi.Dashboards.Application.Data.Entities;
using EdFi.Dashboards.Data.Entities;
using EdFi.Dashboards.Data.Repository;
using EdFi.Dashboards.Resources.Models.Common;
using EdFi.Dashboards.Resources.Models.Student;
using EdFi.Dashboards.Resources.Navigation;
using EdFi.Dashboards.Resources.StudentMetrics;
using EdFi.Dashboards.Resources.StudentSchool;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EdFi.Dashboards.Resources.Common
{
    /// <summary>
    /// Contains features used by the EdFi Grid.
    /// </summary>
    public abstract class EdFiGridServiceBase
    {
        protected const string StudentLink = "student";

        public IRepository<TeacherSection> TeacherSectionRepository { get; set; }
        public IRepository<StaffCohort> StaffCohortRepository { get; set; }
        public IRepository<StaffCustomStudentList> StaffCustomStudentListRepository { get; set; }
        public IRepository<StaffCustomStudentListStudent> StaffCustomStudentListStudentRepository { get; set; }
        public IAccommodationProvider AccommodationProvider { get; set; }
        public IStudentMetricsProvider StudentListWithMetricsProvider { get; set; }
        public IStudentSchoolAreaLinks StudentSchoolAreaLinks { get; set; }

        protected List<long> GetCustomListStudentIds(long staffUSI, int schoolId, StudentListIdentity studentListIdentity)
        {
            var customStudentIdList = new List<long>();

            switch (studentListIdentity.StudentListType)
            {
                case StudentListType.CustomStudentList:
                    var sectionOrCohortId = studentListIdentity.Id;

                    customStudentIdList =
                        (from s in StaffCustomStudentListStudentRepository.GetAll()
                         where s.StaffCustomStudentListId == sectionOrCohortId
                         select s.StudentUSI)
                            .ToList();

                    break;
            }

            return customStudentIdList;
        }

        /// <summary>
        /// Gets the student list identity.
        /// </summary>
        /// <param name="staffUSI">The staff usi.</param>
        /// <param name="schoolId">The school identifier.</param>
        /// <param name="studentListType">Type of the student list.</param>
        /// <param name="sectionOrCohortId">The section or cohort identifier.</param>
        /// <returns></returns>
        protected StudentListIdentity GetStudentListIdentity(long staffUSI, int schoolId, string studentListType, long sectionOrCohortId)
        {
            StudentListType slt;
            Enum.TryParse(studentListType, true, out slt);

            // If no list type was provided, get a default one
            if (slt == StudentListType.None)
                return GetDefaultListIdentity(staffUSI, schoolId);

            // Return the supplied information as a list identity 
            return new StudentListIdentity
            {
                StudentListType = slt,
                Id = sectionOrCohortId
            };
        }

        // TODO: GKM - Review this implementation vs. DefaultSectionService implementation
        /// <summary>
        /// Gets the default list identity.
        /// </summary>
        /// <param name="staffUSI">The staff usi.</param>
        /// <param name="schoolId">The school identifier.</param>
        /// <returns></returns>
        private StudentListIdentity GetDefaultListIdentity(long staffUSI, int schoolId)
        {
            // Use teacher's first section, if one exists
            var firstSection = (from data in TeacherSectionRepository.GetAll()
                                where data.StaffUSI == staffUSI && data.SchoolId == schoolId
                                orderby data.SubjectArea, data.CourseTitle, data.ClassPeriod, data.LocalCourseCode, data.TeacherSectionId
                                select data).FirstOrDefault();

            if (firstSection != null)
            {
                return new StudentListIdentity
                {
                    StudentListType = StudentListType.Section,
                    Id = firstSection.TeacherSectionId
                };
            }

            // Use staff member's first cohort, if one exists
            var firstCohort = (from data in StaffCohortRepository.GetAll()
                               where data.StaffUSI == staffUSI && data.EducationOrganizationId == schoolId
                               orderby data.CohortDescription, data.StaffCohortId
                               select data).FirstOrDefault();

            if (firstCohort != null)
            {
                return new StudentListIdentity
                {
                    StudentListType = StudentListType.Cohort,
                    Id = firstCohort.StaffCohortId,
                };
            }

            // Use staff member's first custom student list, if one exists
            var firstCustom = (from data in StaffCustomStudentListRepository.GetAll()
                               where data.StaffUSI == staffUSI && data.EducationOrganizationId == schoolId
                               orderby data.CustomStudentListIdentifier, data.StaffCustomStudentListId
                               select data).FirstOrDefault();

            if (firstCustom != null)
            {
                return new StudentListIdentity
                {
                    StudentListType = StudentListType.CustomStudentList,
                    Id = firstCustom.StaffCustomStudentListId,
                };
            }

            // Nothing to show
            return new StudentListIdentity { StudentListType = StudentListType.None, Id = 0 };
        }

        /// <summary>
        /// Overlays the student accommodation.
        /// </summary>
        /// <param name="students">The students.</param>
        /// <param name="schoolId">The school identifier.</param>
        protected void OverlayStudentAccommodation(List<StudentWithMetricsAndAccommodations> students, int schoolId)
        {
            var studentIds = students.Select(x => x.StudentUSI).Distinct().ToArray();
            if (!studentIds.Any()) return;

            var allAccommodationsForStudents = AccommodationProvider.GetAccommodations(studentIds, schoolId);
            if (allAccommodationsForStudents == null) return;

            foreach (var sa in allAccommodationsForStudents)
            {
                students.Where(x => x.StudentUSI == sa.StudentUSI).ToList().ForEach(y => y.Accommodations = sa.AccommodationsList);
            }
        }
    }
}
