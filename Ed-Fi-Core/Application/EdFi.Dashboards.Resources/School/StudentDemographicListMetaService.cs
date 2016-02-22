using EdFi.Dashboards.Core;
using EdFi.Dashboards.Resources.Common;
using EdFi.Dashboards.Resources.Models.CustomGrid;
using EdFi.Dashboards.Resources.Models.School;
using EdFi.Dashboards.Resources.Security.Common;
using EdFi.Dashboards.Resources.Staff;
using EdFi.Dashboards.SecurityTokenService.Authentication;

namespace EdFi.Dashboards.Resources.School
{
    public class StudentDemographicListMetaRequest : IEdFiGridMetaRequestConvertable
    {
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
        [AuthenticationIgnore("Ignoring for now but may be needed")]
        public string StudentListType { get; set; }

        public string Demographic { get; set; }

        public static StudentDemographicListMetaRequest Create(int schoolId, long? sectionOrCohortId, string studentListType, string demographic)
        {
            return new StudentDemographicListMetaRequest
            {
                SchoolId = schoolId,
                SectionOrCohortId = sectionOrCohortId,
                StudentListType = studentListType,
                Demographic = demographic
            };
        }

        public EdFiGridMetaRequest ConvertToGridMetaRequest()
        {
            return new EdFiGridMetaRequest
            {
                SchoolId = SchoolId,
                StaffUSI = null,
                SectionOrCohortId = SectionOrCohortId,
                StudentListType = StudentListType,
                Demographic = Demographic,
                GridListType = ListType.StudentDemographic
            };
        }
    }
}