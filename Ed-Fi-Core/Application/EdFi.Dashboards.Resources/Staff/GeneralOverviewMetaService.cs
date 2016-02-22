using EdFi.Dashboards.Common;
using EdFi.Dashboards.Core;
using EdFi.Dashboards.Infrastructure.Implementations.Caching;
using EdFi.Dashboards.Resource.Models.Common;
using EdFi.Dashboards.Resources.Application;
using EdFi.Dashboards.Resources.Common;
using EdFi.Dashboards.Resources.Models.Common;
using EdFi.Dashboards.Resources.Models.CustomGrid;
using EdFi.Dashboards.Resources.Models.Staff;
using EdFi.Dashboards.Resources.Models.Student;
using EdFi.Dashboards.Resources.Navigation;
using EdFi.Dashboards.Resources.Navigation.Support;
using EdFi.Dashboards.Resources.School;
using EdFi.Dashboards.Resources.Security.Common;
using EdFi.Dashboards.SecurityTokenService.Authentication;

namespace EdFi.Dashboards.Resources.Staff
{
    public class GeneralOverviewMetaRequest : IEdFiGridMetaRequestConvertable
    {
        public int SchoolId { get; set; }
        public long StaffUSI { get; set; }

        [AuthenticationIgnore("LocalEducationAgencyId is implied by SchoolId, and does not need to be independently authorized.  Furthermore, it does not appear to be used currently, and may only be present to force ASP.NET MVC to populate the local education agency Id property on the EdFiDashboardContext.Current instance.")]
        public int LocalEducationAgencyId { get; set; }
        public string StudentListType { get; set; }
        public long? SectionOrCohortId { get; set; }

        public static GeneralOverviewMetaRequest Create(int localEducationAgencyId, int schoolId, int staffUSI,
                                                 string studentListType, long sectionOrCohortId)
        {
            var request = new GeneralOverviewMetaRequest
                {
                    LocalEducationAgencyId = localEducationAgencyId,
                    SchoolId = schoolId,
                    StaffUSI = staffUSI,
                    StudentListType = studentListType,
                    SectionOrCohortId = sectionOrCohortId
                };

            return request;
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
                SchoolId = SchoolId,
                StaffUSI = StaffUSI,
                SectionOrCohortId = SectionOrCohortId,
                StudentListType = StudentListType,
                GridListType = ListType.ClassroomGeneralOverview
            };

            return metaRequest;
        }
    }

    public class GeneralOverviewMetaService : StaffServiceBase, IService<GeneralOverviewMetaRequest, GeneralOverviewMetaModel>
    {
        public ISchoolCategoryProvider SchoolCategoryProvider { get; set; }
        public IClassroomMetricsProvider ClassroomMetricsProvider { get; set; }
        public IListMetadataProvider ListMetadataProvider { get; set; }
        public IMetadataListIdResolver MetadataListIdResolver { get; set; }

        [NoCache]
        [CanBeAuthorizedBy(EdFiClaimTypes.ViewAllStudents, EdFiClaimTypes.ViewMyStudents)]
        public GeneralOverviewMetaModel Get(GeneralOverviewMetaRequest request)
        {
            var model = new GeneralOverviewMetaModel();

            var schoolCategory = SchoolCategoryProvider.GetSchoolCategoryType(request.SchoolId);
            switch (schoolCategory)
            {
                case SchoolCategory.Elementary:
                case SchoolCategory.MiddleSchool:
                    break;
                default:
                    schoolCategory = SchoolCategory.HighSchool;
                    break;
            }

            //Get the metadata.
            var resolvedListId = MetadataListIdResolver.GetListId(ListType.ClassroomGeneralOverview, schoolCategory);
            model.ListMetadata = ListMetadataProvider.GetListMetadata(resolvedListId);

            return model;
        }
    }
}
