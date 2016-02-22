// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System.Collections.Generic;
using System.Linq;
using EdFi.Dashboards.Common;
using EdFi.Dashboards.Data.Entities;
using EdFi.Dashboards.Data.Repository;
using EdFi.Dashboards.Resource.Models.Common;
using EdFi.Dashboards.Resources.Common;
using EdFi.Dashboards.Resources.Models.LocalEducationAgency;
using EdFi.Dashboards.Resources.Navigation;
using EdFi.Dashboards.Resources.Navigation.Support;
using EdFi.Dashboards.Resources.School;
using EdFi.Dashboards.Resources.Security.Common;
using EdFi.Dashboards.SecurityTokenService.Authentication;

namespace EdFi.Dashboards.Resources.LocalEducationAgency
{
    public class SchoolCategoryListRequest
    {
        public int LocalEducationAgencyId { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SchoolCategoryListRequest"/> using the specified parameters.
        /// </summary>
        /// <returns>A new <see cref="SchoolCategoryListRequest"/> instance.</returns>
        public static SchoolCategoryListRequest Create(int localEducationAgencyId) 
        {
            return new SchoolCategoryListRequest { LocalEducationAgencyId = localEducationAgencyId };
        }
    }

    public interface ISchoolCategoryListService : IService<SchoolCategoryListRequest, IList<SchoolCategoryModel>> { }

    public class SchoolCategoryListService : ISchoolCategoryListService
    {
        private const string students = "Students";
        private const string teachers = "Teachers";

        private readonly IRepository<SchoolInformation> repository;
        private readonly IRepository<SchoolGradePopulation> schoolGradePopulationRepository;
        private readonly ISchoolAreaLinks schoolLinks;
        private readonly IUniqueListIdProvider uniqueListIdProvider;
        private readonly ISchoolCategoryProvider schoolCategoryProvider;

        public SchoolCategoryListService(IRepository<SchoolInformation> repository, 
            IRepository<SchoolGradePopulation> schoolGradePopulationRepository, 
            ISchoolAreaLinks schoolLinks,
            IUniqueListIdProvider uniqueListIdProvider,
            ISchoolCategoryProvider schoolCategoryProvider)
        {
            this.repository = repository;
            this.schoolGradePopulationRepository = schoolGradePopulationRepository;
            this.schoolLinks = schoolLinks;
            this.uniqueListIdProvider = uniqueListIdProvider;
            this.schoolCategoryProvider = schoolCategoryProvider;
        }

        // TODO: Question this --> [CanBeAuthorizedBy(EdFiClaimTypes.ViewAllMetrics)]
        [CanBeAuthorizedBy(EdFiClaimTypes.AccessOrganization)]
        public virtual IList<SchoolCategoryModel> Get(SchoolCategoryListRequest request)
        {
            var listContext = uniqueListIdProvider.GetUniqueId();

            var results = (from data in repository.GetAll()
                           where data.LocalEducationAgencyId == request.LocalEducationAgencyId &&
                                schoolGradePopulationRepository.GetAll().Select(o => o.SchoolId).Contains(data.SchoolId)
                            select data).ToList();

            var schoolCategories = results.GroupBy(x => x.SchoolCategory).OrderBy(y => y.Key, new SchoolCategoryComparer(schoolCategoryProvider));
            return (from schoolCategory in schoolCategories
                    select new SchoolCategoryModel
                    {
                        Category = schoolCategory.Key,
                        Schools = (from c1 in schoolCategory
                                  orderby c1.Name
                                  select new SchoolCategoryModel.School
                                  {
                                      SchoolId = c1.SchoolId,
                                      Name = c1.Name,
                                      Url = schoolLinks.Default(c1.SchoolId, c1.Name).AppendParameters("listContext=" + listContext),
                                      Links = new List<Link> { new Link { Rel = students, Href = schoolLinks.StudentGradeList(c1.SchoolId, c1.Name).AppendParameters("listContext=" + listContext).Resolve() },
                                                                new Link { Rel = teachers, Href = schoolLinks.Teachers(c1.SchoolId, c1.Name).AppendParameters("listContext=" + listContext).Resolve() }}
                                  }).ToList()
                    }).ToList();
        }
    }
}
