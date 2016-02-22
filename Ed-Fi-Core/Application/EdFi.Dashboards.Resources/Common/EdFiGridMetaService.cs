using EdFi.Dashboards.Common;
using EdFi.Dashboards.Core;
using EdFi.Dashboards.Infrastructure.Implementations.Caching;
using EdFi.Dashboards.Resources.Models.CustomGrid;
using EdFi.Dashboards.Resources.School;
using EdFi.Dashboards.Resources.Security.Common;
using EdFi.Dashboards.Resources.Staff;
using EdFi.Dashboards.SecurityTokenService.Authentication;

namespace EdFi.Dashboards.Resources.Common
{
    /// <summary>
    /// This is the base request of any page that has the EdFi Grid on it. The
    /// properties are set virtual so another request can override the class
    /// and tag certain properties with the AuthIgnore attribute.
    /// </summary>
    public class EdFiGridMetaRequest
    {
        /// <summary>
        /// Gets or sets the staff USI.
        /// </summary>
        /// <value>
        /// The staff USI.
        /// </value>
        public long? StaffUSI { get; set; }

        /// <summary>
        /// Gets or sets the school identifier.
        /// </summary>
        /// <value>
        /// The school identifier.
        /// </value>
        public int? SchoolId { get; set; }

        /// <summary>
        /// Gets or sets the section or cohort identifier.
        /// </summary>
        /// <value>
        /// The section or cohort identifier.
        /// </value>
        public long? SectionOrCohortId { get; set; }

        /// <summary>
        /// Gets or sets the local education agency identifier.
        /// </summary>
        /// <value>
        /// The local education agency identifier.
        /// </value>
        public int LocalEducationAgencyId { get; set; }

        /// <summary>
        /// Gets or sets the type of the student list.
        /// </summary>
        /// <value>
        /// The type of the student list.
        /// </value>
        public string StudentListType { get; set; }

        /// <summary>
        /// Gets or sets the demographic.
        /// </summary>
        /// <value>
        /// The demographic.
        /// </value>
        [AuthenticationIgnore("Used by pages with a demographic drop down")]
        public string Demographic { get; set; }

        /// <summary>
        /// Gets or sets the level.
        /// </summary>
        /// <value>
        /// The level.
        /// </value>
        [AuthenticationIgnore("Used by pages with a level drop down")]
        public string Level { get; set; }

        /// <summary>
        /// Gets or sets the grade.
        /// </summary>
        /// <value>
        /// The grade.
        /// </value>
        [AuthenticationIgnore("Used by pages with a grade drop down")]
        public string Grade { get; set; }

        /// <summary>
        /// Gets or sets the type of data to be contained in the grid.
        /// </summary>
        /// <value>
        /// The type of the grid list.
        /// </value>
        [AuthenticationIgnore("Used to determine what view is sending the request")]
        public ListType GridListType { get; set; }
    }

    public interface IEdFiGridMetaService : IService<EdFiGridMetaRequest, EdFiGridModel> { }

    /// <summary>
    /// Contains generic functionality that will be used by the EdFi Grid.
    /// </summary>
    public class EdFiGridMetaService : IEdFiGridMetaService
    {
        protected readonly ISchoolCategoryProvider SchoolCategoryProvider;
        protected readonly IMetadataListIdResolver MetadataListIdResolver;
        protected readonly IListMetadataProvider MetadataProvider;

        public EdFiGridMetaService(
            ISchoolCategoryProvider schoolCategoryProvider,
            IMetadataListIdResolver metadataListIdResolver,
            IListMetadataProvider metadataProvider)
        {
            SchoolCategoryProvider = schoolCategoryProvider;
            MetadataListIdResolver = metadataListIdResolver;
            MetadataProvider = metadataProvider;
        }

        /// <summary>
        /// Gets the meta data for the EdFi Grid.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        [NoCache]
        [CanBeAuthorizedBy(EdFiClaimTypes.ViewAllStudents, EdFiClaimTypes.ViewMyStudents)]
        public EdFiGridModel Get(EdFiGridMetaRequest request)
        {
            var gridModel = new EdFiGridModel();

            if (request.SchoolId <= 0 && request.LocalEducationAgencyId <= 0)
                return gridModel;

            // at the school level use the school category provider at the
            // LEA level assign high school as the default
            var schoolCategory = request.SchoolId.HasValue
                ? SchoolCategoryProvider.GetSchoolCategoryType(request.SchoolId.Value)
                : SchoolCategory.HighSchool;

            switch (schoolCategory)
            {
                case SchoolCategory.Elementary:
                case SchoolCategory.MiddleSchool:
                    break;
                default:
                    schoolCategory = SchoolCategory.HighSchool;
                    break;
            }

            var resolvedListId = MetadataListIdResolver.GetListId(request.GridListType, schoolCategory);
            gridModel.ListMetadata = MetadataProvider.GetListMetadata(resolvedListId);

            return gridModel;
        }
    }
}
