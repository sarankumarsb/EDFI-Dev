using System.Collections.Generic;
using System.Linq;
using EdFi.Dashboards.Common;
using EdFi.Dashboards.Core.Providers.Context;
using EdFi.Dashboards.Data.Entities;
using EdFi.Dashboards.Data.Repository;
using EdFi.Dashboards.Infrastructure.Implementations.Caching;
using EdFi.Dashboards.Resources.Common;
using EdFi.Dashboards.Resources.Models.Common;
using EdFi.Dashboards.Resources.Models.LocalEducationAgency;
using EdFi.Dashboards.Resources.Navigation;
using EdFi.Dashboards.Resources.School;
using EdFi.Dashboards.Resources.Security.Common;
using EdFi.Dashboards.SecurityTokenService.Authentication;

namespace EdFi.Dashboards.Resources.LocalEducationAgency
{
    public class StudentSchoolCategoryMenuRequest
    {
        public int LocalEducationAgencyId { get; set; }
		
		[AuthenticationIgnore("Needed for Watchlist")]
		public long StaffUSI { get; set; }

        public static StudentSchoolCategoryMenuRequest Create(int localEducationAgencyId, long staffUSI)
        {
            return new StudentSchoolCategoryMenuRequest { LocalEducationAgencyId = localEducationAgencyId, StaffUSI = staffUSI };
        }
    }

    public interface IStudentSchoolCategoryMenuService :
        IService<StudentSchoolCategoryMenuRequest, StudentSchoolCategoryMenuModel>
    {
        IList<string> GetUniqueSchoolCategories(int localEducationAgencyId);
    }

    public class StudentSchoolCategoryMenuService : IStudentSchoolCategoryMenuService
    {
        
        private readonly IRepository<SchoolInformation> schoolInformationRepository;
        private readonly IRepository<SchoolGradePopulation> schoolGradePopulationRepository;
        private readonly ISchoolCategoryProvider schoolCategoryProvider;
        private readonly ILocalEducationAgencyAreaLinks localEducationAgencyAreaLinks;
	    private readonly IMetricsBasedWatchListMenuService watchListMenuService;

        public StudentSchoolCategoryMenuService(IRepository<SchoolInformation> schoolInformationRepository,
                                                IRepository<SchoolGradePopulation> schoolGradePopulationRepository,
                                                ISchoolCategoryProvider schoolCategoryProvider,
                                                ILocalEducationAgencyAreaLinks localEducationAgencyAreaLinks,
												IMetricsBasedWatchListMenuService watchListMenuService)
        {
            this.schoolInformationRepository = schoolInformationRepository;
            this.schoolGradePopulationRepository = schoolGradePopulationRepository;
            this.schoolCategoryProvider = schoolCategoryProvider;
            this.localEducationAgencyAreaLinks = localEducationAgencyAreaLinks;
	        this.watchListMenuService = watchListMenuService;
        }

		[NoCache]
        [CanBeAuthorizedBy(EdFiClaimTypes.ViewAllStudents, EdFiClaimTypes.ViewMyStudents)]
        public StudentSchoolCategoryMenuModel Get(StudentSchoolCategoryMenuRequest request)
        {
            var model = new StudentSchoolCategoryMenuModel();

            var results = GetUniqueSchoolCategories(request.LocalEducationAgencyId);

            var schoolCategories = results.OrderBy(y => y, new SchoolCategoryComparer(schoolCategoryProvider));

            foreach (var schoolCategory in schoolCategories)
            {
				model.SchoolCategories.Add(new AttributeItemWithSelected<string>
                                               {
                                                   Attribute = schoolCategory,
                                                   Value = schoolCategory,
                                                   Url = localEducationAgencyAreaLinks.StudentSchoolCategoryList(request.LocalEducationAgencyId, schoolCategory),
												   Selected = false
                                               });
            }

			var watchListMenuItems = watchListMenuService.Get(new MetricsBasedWatchListMenuRequest { LocalEducationAgencyId = request.LocalEducationAgencyId, StaffUSI = request.StaffUSI });
			foreach (var watchlist in watchListMenuItems)
			{
				if (watchlist.MenuType == "PageSchoolCategory")
				{
					model.DynamicWatchLists.Add(new AttributeItemWithSelected<string>
					{
						Attribute = watchlist.Description,
						Value = watchlist.SectionId.ToString(),
						Url = watchlist.Href,
						Selected = false
					});
				}
			}
			
			return model;
        }

        [CanBeAuthorizedBy(EdFiClaimTypes.ViewAllStudents, EdFiClaimTypes.ViewMyStudents)]
        public virtual IList<string> GetUniqueSchoolCategories(int localEducationAgencyId)
        {
            return (from schoolInformation in schoolInformationRepository.GetAll()
                join schoolGrade in schoolGradePopulationRepository.GetAll() on schoolInformation.SchoolId equals
                    schoolGrade.SchoolId
                where schoolInformation.LocalEducationAgencyId == localEducationAgencyId
                group schoolInformation by schoolInformation.SchoolCategory
                into uniqueSchoolCategories
                select uniqueSchoolCategories.Key).ToList();
        }
    }
}
