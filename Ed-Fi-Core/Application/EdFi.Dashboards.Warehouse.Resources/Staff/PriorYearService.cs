using System.Collections.Generic;
using System.Linq;
using EdFi.Dashboards.Common;
using EdFi.Dashboards.Core;
using EdFi.Dashboards.Resource.Models.Common;
using EdFi.Dashboards.Resources;
using EdFi.Dashboards.Resources.Models.Common;
using EdFi.Dashboards.Resources.Models.CustomGrid;
using EdFi.Dashboards.Resources.Models.Student;
using EdFi.Dashboards.Resources.Navigation;
using EdFi.Dashboards.Resources.Navigation.Support;
using EdFi.Dashboards.Resources.School;
using EdFi.Dashboards.Resources.Security.Common;
using EdFi.Dashboards.Resources.Staff;
using EdFi.Dashboards.Resources.StudentSchool;
using EdFi.Dashboards.SecurityTokenService.Authentication;
using EdFi.Dashboards.Warehouse.Resource.Models.Staff;
using EdFi.Dashboards.Warehouse.Resources.Application;
using EdFi.Dashboards.Warehouse.Resources.School.Detail;

namespace EdFi.Dashboards.Warehouse.Resources.Staff
{
    public class PriorYearRequest
    {
        public int SchoolId { get; set; }
        public long StaffUSI { get; set; }

        [AuthenticationIgnore("LocalEducationAgencyId is implied by SchoolId, and does not need to be independently authorized.")]
        public int LocalEducationAgencyId { get; set; }
        public string StudentListType { get; set; }
        public long SectionOrCohortId { get; set; }
    }

    public class PriorYearService : StaffServiceBase, IService<PriorYearRequest, PriorYearModel>
    {
        public ISchoolCategoryProvider SchoolCategoryProvider { get; set; }
        public IPriorYearClassroomMetricsProvider ClassroomMetricsProvider { get; set; }
        public IListMetadataProvider ListMetadataProvider { get; set; }
        public IMetadataListIdResolver MetadataListIdResolver { get; set; }
        public IWarehouseAvailabilityProvider WarehouseAvailabilityProvider { get; set; }
        public IMaxPriorYearProvider MaxPriorYearProvider { get; set; }
        public IGradeLevelUtilitiesProvider GradeLevelUtilitiesProvider { get; set; }
        public IPriorYearStudentMetricsProvider PriorYearStudentMetricsProvider { get; set; }

        [CanBeAuthorizedBy(EdFiClaimTypes.ViewAllStudents, EdFiClaimTypes.ViewMyStudents)]
        public PriorYearModel Get(PriorYearRequest request)
        {
            var model = new PriorYearModel();
            if (!WarehouseAvailabilityProvider.Get())
            {
                return model;
            }

            long staffUSI = request.StaffUSI;
            int schoolId = request.SchoolId;
            int localEducationAgencyId = request.LocalEducationAgencyId;
            string studentListType = request.StudentListType;
            long sectionOrCohortId = request.SectionOrCohortId;

            //var slt = GetSection(staffUSI, schoolId, studentListType, ref sectionOrCohortId);
            var slt = GetStudentListIdentity(staffUSI, schoolId, studentListType, sectionOrCohortId);

            if (slt.StudentListType != StudentListType.All && slt.Id == 0)
                return model;

            //Get the metadata.
            var resolvedListId = MetadataListIdResolver.GetListId(ListType.ClassroomPriorYear, model.SchoolCategory);
            model.ListMetadata = ListMetadataProvider.GetListMetadata(resolvedListId);

            model.UniqueListId = UniqueListProvider.GetUniqueId();
            var students = GetStudentListEntities(schoolId, staffUSI, slt, new List<MetadataColumnGroup>(), new List<long>(), null, null); //TODO : lots of nulls
            var studentListEntities = GetStudentListWithMetrics(staffUSI, schoolId, students.Select(student => student.StudentUSI).ToList());
            if (studentListEntities == null || !studentListEntities.Any())
                return model;

            var currentYearStudentUSIs = students.Select(x => x.StudentUSI).ToArray();

            var year = MaxPriorYearProvider.Get(localEducationAgencyId);

            var priorYearQueryOptions = new PriorYearStudentMetricsProviderQueryOptions
            {
                StudentIds = currentYearStudentUSIs,
                MetricVariantIds = model.ListMetadata.GetMetricVariantIds(),
                LocalEducationAgencyId = localEducationAgencyId,
                Year = year
            };

            var priorYearStudents = PriorYearStudentMetricsProvider.GetStudentsWithMetrics(priorYearQueryOptions);

            model.SchoolCategory = SchoolCategoryProvider.GetSchoolCategoryType(schoolId);
            switch (model.SchoolCategory)
            {
                case SchoolCategory.Elementary:
                case SchoolCategory.MiddleSchool:
                    break;
                default:
                    model.SchoolCategory = SchoolCategory.HighSchool;
                    break;
            }

            // We have all the pieces now lets build the Resource model Students list.
            // can't be a LINQ statement b/c StudentMetric does not have a default constructor
            foreach (var student in students)
            {
                long studentUsi = student.StudentUSI;

                var priorYearStudentList = priorYearStudents.Where(x => x.StudentUSI == studentUsi).ToList();
                if (priorYearStudentList.Count == 0)
                    continue;

                var studentMetrics = studentListEntities.Where(x => x.StudentUSI == studentUsi).ToList();

                string gender = student.Gender;
                string fullName = student.FullName;

                //Student Unique Field Newly Added : Saravanan
                var studentWithMetrics = new StudentWithMetricsAndAccommodations(studentUsi)
                {
                    SchoolId = student.SchoolId,
                    Name = Utilities.FormatPersonNameByLastName(student.FirstName, student.MiddleName, student.LastSurname),
                    ThumbNail = StudentSchoolAreaLinks.Image(schoolId, studentUsi, gender, fullName).Resolve(),
                    Href = new Link { Rel = StudentLink, Href = StudentSchoolAreaLinks.Overview(schoolId, studentUsi, fullName).AppendParameters("listContext=" + model.UniqueListId).Resolve() },
                    Metrics = ClassroomMetricsProvider.GetAdditionalMetrics(priorYearStudentList, studentMetrics, model.ListMetadata),
                    GradeLevel = GradeLevelUtilitiesProvider.FormatGradeLevelForSorting(student.GradeLevel),
                    GradeLevelDisplayValue = GradeLevelUtilitiesProvider.FormatGradeLevelForDisplay(student.GradeLevel),
                    StudentUniqueID  = student.StudentUniqueID
                };

                model.Students.Add(studentWithMetrics);
            }

            OverlayStudentAccommodation(model.Students, schoolId);

            return model;
        }
    }
}
