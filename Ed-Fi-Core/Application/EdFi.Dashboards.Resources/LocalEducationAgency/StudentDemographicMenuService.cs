using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using EdFi.Dashboards.Common;
using EdFi.Dashboards.Core.Providers.Context;
using EdFi.Dashboards.Data.Entities;
using EdFi.Dashboards.Data.Repository;
using EdFi.Dashboards.Infrastructure.Implementations.Caching;
using EdFi.Dashboards.Resources.Common;
using EdFi.Dashboards.Resources.Models.Common;
using EdFi.Dashboards.Resources.Models.LocalEducationAgency;
using EdFi.Dashboards.Resources.Navigation;
using EdFi.Dashboards.Resources.Security.Common;
using EdFi.Dashboards.SecurityTokenService.Authentication;

namespace EdFi.Dashboards.Resources.LocalEducationAgency
{
    public class StudentDemographicMenuRequest
    {
        public int LocalEducationAgencyId { get; set; }

		[AuthenticationIgnore("Needed for Watchlist")]
		public long StaffUSI { get; set;}

        public static StudentDemographicMenuRequest Create(int localEducationAgencyId, long staffUSI)
        {
            return new StudentDemographicMenuRequest {LocalEducationAgencyId = localEducationAgencyId, StaffUSI = staffUSI};
        }
    }

    public interface IStudentDemographicMenuService : IService<StudentDemographicMenuRequest, StudentDemographicMenuModel> { }

    public class StudentDemographicMenuService : IStudentDemographicMenuService
    {
        private readonly IRepository<LocalEducationAgencyStudentDemographic> localEducationAgencyStudentDemographicRepository;
        protected readonly IRepository<LocalEducationAgencyProgramPopulation> localEducationAgencyProgramPopulationRepository;
        protected readonly IRepository<LocalEducationAgencyIndicatorPopulation> localEducationAgencyIndicatorPopulationRepository;
        private readonly ILocalEducationAgencyAreaLinks localEducationAgencyAreaLinks;
		private readonly IMetricsBasedWatchListMenuService watchListMenuService;

        private const string femaleStr = "Female";
        private const string maleStr = "Male";
        private const string hispanicStr = "Hispanic/Latino";
        private const string lateEnrollmentStr = "Late Enrollment";

        public StudentDemographicMenuService(IRepository<LocalEducationAgencyStudentDemographic> localEducationAgencyStudentDemographicRepository,
                                            IRepository<LocalEducationAgencyProgramPopulation> localEducationAgencyProgramPopulationRepository,
                                            IRepository<LocalEducationAgencyIndicatorPopulation> localEducationAgencyIndicatorPopulationRepository,
                                            ILocalEducationAgencyAreaLinks localEducationAgencyAreaLinks,
											IMetricsBasedWatchListMenuService watchListMenuService)
        {
            this.localEducationAgencyStudentDemographicRepository = localEducationAgencyStudentDemographicRepository;
            this.localEducationAgencyProgramPopulationRepository = localEducationAgencyProgramPopulationRepository;
            this.localEducationAgencyIndicatorPopulationRepository = localEducationAgencyIndicatorPopulationRepository;
            this.localEducationAgencyAreaLinks = localEducationAgencyAreaLinks;
			this.watchListMenuService = watchListMenuService;
        }

        [NoCache]
        [CanBeAuthorizedBy(EdFiClaimTypes.ViewAllStudents, EdFiClaimTypes.ViewMyStudents)]
        public StudentDemographicMenuModel Get(StudentDemographicMenuRequest request)
        {
            var model = new StudentDemographicMenuModel();

            var demographics = GetStudentDemographics(request);

            AddDemographicToGender(request, demographics, model, femaleStr);

            AddDemographicToGender(request, demographics, model, maleStr);

            AddHispanicDemographicToEthnicity(request, demographics, model);

            AddDemographicsToRace(request, demographics, model);

            var programs = GetPrograms(request);
            AddPrograms(request, programs, model);

            var indicators = GetIndicators(request);
            AddIndicators(request, indicators, model);

			var watchListMenuItems = watchListMenuService.Get(new MetricsBasedWatchListMenuRequest { LocalEducationAgencyId = request.LocalEducationAgencyId, StaffUSI = request.StaffUSI });
			foreach (var watchlist in watchListMenuItems)
			{
				if (watchlist.MenuType == "PageDemographic")
				{
					model.WatchLists.Add(new AttributeItemWithSelected<string>
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

        private IOrderedQueryable<LocalEducationAgencyStudentDemographic> GetStudentDemographics(StudentDemographicMenuRequest request)
        {
            var demographics = from demographic in localEducationAgencyStudentDemographicRepository.GetAll()
                               where demographic.LocalEducationAgencyId == request.LocalEducationAgencyId && demographic.Value != null
                               orderby demographic.DisplayOrder
                               select demographic;
            return demographics;
        }

        private void AddDemographicToGender(StudentDemographicMenuRequest request, IOrderedQueryable<LocalEducationAgencyStudentDemographic> demographics,
            StudentDemographicMenuModel model, string gender)
        {
            var demographic = demographics.SingleOrDefault(x => x.Attribute == gender);
            if (demographic == null)
            {
                return;
            }

            Debug.Assert(demographic.Value.HasValue, "demographic.Value should not be null.");
			model.Gender.Add(new AttributeItemWithSelected<decimal>
            {
                Attribute = demographic.Attribute,
                Value = demographic.Value.Value,
                Url = localEducationAgencyAreaLinks.StudentDemographicList(request.LocalEducationAgencyId, demographic.Attribute),
				Selected = false
            });
        }

        private void AddHispanicDemographicToEthnicity(StudentDemographicMenuRequest request, IOrderedQueryable<LocalEducationAgencyStudentDemographic> demographics,
            StudentDemographicMenuModel model)
        {
            var hispanicDemographic = demographics.SingleOrDefault(x => x.Attribute == hispanicStr);
            if (hispanicDemographic == null)
            {
                return;
            }

            Debug.Assert(hispanicDemographic.Value.HasValue, "hispanicDemographic.Value should not be null");
			model.Ethnicity.Add(new AttributeItemWithSelected<decimal>
            {
                Attribute = hispanicDemographic.Attribute,
                Value = hispanicDemographic.Value.Value,
				Url = localEducationAgencyAreaLinks.StudentDemographicList(request.LocalEducationAgencyId, hispanicDemographic.Attribute),
				Selected = false
            });
        }

        private void AddDemographicsToRace(StudentDemographicMenuRequest request, IOrderedQueryable<LocalEducationAgencyStudentDemographic> demographics,
            StudentDemographicMenuModel model)
        {
            foreach (
                var demographic in
                    demographics.Where(x => x.Attribute != femaleStr && x.Attribute != maleStr && x.Attribute != hispanicStr))
            {
                Debug.Assert(demographic.Value.HasValue, "demographic.Value should not be null.");
				model.Race.Add(new AttributeItemWithSelected<decimal>
                {
                    Attribute = demographic.Attribute,
                    Value = demographic.Value.Value,
					Url = localEducationAgencyAreaLinks.StudentDemographicList(request.LocalEducationAgencyId, demographic.Attribute),
					Selected = false
                });
            }
        }

        protected virtual IEnumerable<LocalEducationAgencyProgramPopulation> GetPrograms(StudentDemographicMenuRequest request)
        {
            var programs = from program in localEducationAgencyProgramPopulationRepository.GetAll()
                           where program.LocalEducationAgencyId == request.LocalEducationAgencyId && program.Value != null
                           orderby program.DisplayOrder
                           select program;
            return programs;
        }

        private void AddPrograms(StudentDemographicMenuRequest request, IEnumerable<LocalEducationAgencyProgramPopulation> programs, StudentDemographicMenuModel model)
        {
            foreach (var program in programs)
            {
                Debug.Assert(program.Value.HasValue, "program.Value should not be null.");
				model.Program.Add(new AttributeItemWithSelected<decimal>
                {
                    Attribute = program.Attribute,
                    Value = program.Value.Value,
					Url = localEducationAgencyAreaLinks.StudentDemographicList(request.LocalEducationAgencyId, program.Attribute),
					Selected = false
                });
            }
        }

        protected virtual IEnumerable<LocalEducationAgencyIndicatorPopulation> GetIndicators(StudentDemographicMenuRequest request)
        {
            var indicators = from indicator in localEducationAgencyIndicatorPopulationRepository.GetAll()
                             where indicator.LocalEducationAgencyId == request.LocalEducationAgencyId && indicator.Value != null
                             orderby indicator.DisplayOrder
                             select indicator;
            return indicators;
        }

        private void AddIndicators(StudentDemographicMenuRequest request, IEnumerable<LocalEducationAgencyIndicatorPopulation> indicators,
            StudentDemographicMenuModel model)
        {
            foreach (var indicator in indicators)
            {
                Debug.Assert(indicator.Value.HasValue, "indicator.Value should not be null.");
				model.Indicator.Add(new AttributeItemWithSelected<decimal>
                {
                    Attribute = indicator.Attribute,
                    Value = indicator.Value.Value,
					Url = localEducationAgencyAreaLinks.StudentDemographicList(request.LocalEducationAgencyId, indicator.Attribute),
					Selected = false
                });
            }

			model.Indicator.Add(new AttributeItemWithSelected<decimal>
            {
                Attribute = lateEnrollmentStr,
				Url = localEducationAgencyAreaLinks.StudentDemographicList(request.LocalEducationAgencyId, lateEnrollmentStr),
				Selected = false
            });
        }
    }
}
