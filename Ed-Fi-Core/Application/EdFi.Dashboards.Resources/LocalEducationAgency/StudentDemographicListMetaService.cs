using EdFi.Dashboards.Core;
using EdFi.Dashboards.Resources.Application;
using EdFi.Dashboards.Resources.Common;
using EdFi.Dashboards.Resources.Models.CustomGrid;
using EdFi.Dashboards.Resources.Models.LocalEducationAgency;
using EdFi.Dashboards.Resources.Security.Common;
using EdFi.Dashboards.Resources.Staff;
using EdFi.Dashboards.SecurityTokenService.Authentication;

namespace EdFi.Dashboards.Resources.LocalEducationAgency
{
    public class StudentDemographicListMetaRequest : IEdFiGridMetaRequestConvertable
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
        /// Gets or sets the demographic.
        /// </summary>
        /// <value>
        /// The demographic.
        /// </value>
        public string Demographic { get; set; }

        /// <summary>
        /// Creates the specified student demographic list meta service.
        /// </summary>
        /// <param name="localEducationAgencyId">The local education agency identifier.</param>
        /// <param name="sectionOrCohortId">The section or cohort identifier.</param>
        /// <param name="studentListType">Type of the student list.</param>
        /// <param name="demographic">The demographic.</param>
        /// <returns></returns>
        public static StudentDemographicListMetaRequest Create(int localEducationAgencyId, long? sectionOrCohortId, string studentListType, string demographic)
        {
            return new StudentDemographicListMetaRequest
            {
                LocalEducationAgencyId = localEducationAgencyId,
                SectionOrCohortId = sectionOrCohortId,
                StudentListType = studentListType,
                Demographic = demographic
            };
        }

        /// <summary>
        /// Converts the current request to a meta request using the data in
        /// the current object.
        /// </summary>
        /// <returns>
        /// An <see cref="EdFiGridMetaRequest" /> object.
        /// </returns>
        public EdFiGridMetaRequest ConvertToGridMetaRequest()
        {
            var metaRequest = new EdFiGridMetaRequest
            {
                LocalEducationAgencyId = LocalEducationAgencyId,
                SchoolId = null,
                SectionOrCohortId = SectionOrCohortId,
                StaffUSI = null,
                StudentListType = StudentListType,
                Demographic = Demographic,
                GridListType = ListType.StudentDemographic
            };

            return metaRequest;
        }
    }
}
