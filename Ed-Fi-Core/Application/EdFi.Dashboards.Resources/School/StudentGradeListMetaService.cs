using EdFi.Dashboards.Core;
using EdFi.Dashboards.Resources.Common;
using EdFi.Dashboards.Resources.Models.CustomGrid;
using EdFi.Dashboards.Resources.Models.School;
using EdFi.Dashboards.Resources.Security.Common;
using EdFi.Dashboards.Resources.Staff;
using EdFi.Dashboards.SecurityTokenService.Authentication;

namespace EdFi.Dashboards.Resources.School
{
    public class StudentGradeListMetaRequest : IEdFiGridMetaRequestConvertable
    {
        /// <summary>
        /// Gets or sets the staff usi.
        /// </summary>
        /// <value>
        /// The staff usi.
        /// </value>
        public long? StaffUSI { get; set; }

        /// <summary>
        /// Gets or sets the school identifier.
        /// </summary>
        /// <value>
        /// The school identifier.
        /// </value>
        public int SchoolId { get; set; }

        /// <summary>
        /// Gets or sets the section or cohort identifier.
        /// </summary>
        /// <value>
        /// The section or cohort identifier.
        /// </value>
        public long? SectionOrCohortId { get; set; }

        /// <summary>
        /// Gets or sets the type of the student list.
        /// </summary>
        /// <value>
        /// The type of the student list.
        /// </value>
        [AuthenticationIgnore("Used to define the type of list")]
        public string StudentListType { get; set; }

        public string Grade { get; set; }

        public static StudentGradeListMetaRequest Create(int schoolId, string grade)
        {
            return new StudentGradeListMetaRequest
            {
                SchoolId = schoolId,
                Grade = grade
            };
        }

        public EdFiGridMetaRequest ConvertToGridMetaRequest()
        {
            return new EdFiGridMetaRequest
            {
                LocalEducationAgencyId = 0,
                SchoolId = SchoolId,
                StaffUSI = StaffUSI,
                SectionOrCohortId = SectionOrCohortId,
                StudentListType = StudentListType,
                Grade = Grade,
                GridListType = ListType.StudentGrade
            };
        }
    }
}