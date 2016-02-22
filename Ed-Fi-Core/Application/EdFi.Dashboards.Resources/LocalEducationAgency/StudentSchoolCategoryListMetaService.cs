using EdFi.Dashboards.Core;
using EdFi.Dashboards.Resources.Common;
using EdFi.Dashboards.Resources.Models.CustomGrid;
using EdFi.Dashboards.Resources.Models.LocalEducationAgency;
using EdFi.Dashboards.Resources.School;
using EdFi.Dashboards.Resources.Security.Common;
using EdFi.Dashboards.Resources.Staff;
using EdFi.Dashboards.SecurityTokenService.Authentication;

namespace EdFi.Dashboards.Resources.LocalEducationAgency
{
    public class StudentSchoolCategoryListMetaRequest : IEdFiGridMetaRequestConvertable
    {
        /// <summary>
        /// Gets or sets the local education agency identifier.
        /// </summary>
        /// <value>
        /// The local education agency identifier.
        /// </value>
        public int LocalEducationAgencyId { get; set; }

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

        /// <summary>
        /// Gets or sets the school category.
        /// </summary>
        /// <value>
        /// The school category.
        /// </value>
        public string SchoolCategory { get; set; }

        public static StudentSchoolCategoryListMetaRequest Create(int localEducationAgencyId, long? sectionOrCohortId, string studentListType, string schoolCategory)
        {
            return new StudentSchoolCategoryListMetaRequest
            {
                LocalEducationAgencyId = localEducationAgencyId,
                SectionOrCohortId = sectionOrCohortId,
                StudentListType = studentListType,
                SchoolCategory = schoolCategory
            };
        }

        public EdFiGridMetaRequest ConvertToGridMetaRequest()
        {
            return new EdFiGridMetaRequest
            {
                LocalEducationAgencyId = LocalEducationAgencyId,
                StaffUSI = null,
                SectionOrCohortId = SectionOrCohortId,
                StudentListType = StudentListType,
                Level = SchoolCategory,
                GridListType = ListType.StudentSchoolCategory
            };
        }
    }
}
