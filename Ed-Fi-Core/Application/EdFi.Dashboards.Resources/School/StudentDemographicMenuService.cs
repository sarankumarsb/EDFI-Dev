using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using EdFi.Dashboards.Common;
using EdFi.Dashboards.Data.Entities;
using EdFi.Dashboards.Data.Repository;
using EdFi.Dashboards.Infrastructure;
using EdFi.Dashboards.Resources.Common;
using EdFi.Dashboards.Resources.Models.Common;
using EdFi.Dashboards.Resources.Models.School;
using EdFi.Dashboards.Resources.Navigation;
using EdFi.Dashboards.Resources.Navigation.Support;
using EdFi.Dashboards.Resources.Security.Common;
using EdFi.Dashboards.SecurityTokenService.Authentication;

namespace EdFi.Dashboards.Resources.School
{
    public class StudentDemographicMenuRequest
    {
        public int SchoolId { get; set; }

        public static StudentDemographicMenuRequest Create(int schoolId)
        {
            return new StudentDemographicMenuRequest { SchoolId = schoolId };
        }
    }

    public interface IStudentDemographicMenuService : IService<StudentDemographicMenuRequest, StudentDemographicMenuModel> { }

    public class StudentDemographicMenuService : IStudentDemographicMenuService
    {
        private readonly IRepository<SchoolStudentDemographic> schoolStudentDemographicRepository;
        protected readonly IRepository<SchoolProgramPopulation> schoolProgramPopulationRepository;
        protected readonly IRepository<SchoolIndicatorPopulation> schoolIndicatorPopulationRepository;
        private readonly ISchoolAreaLinks schoolAreaLinks;

        private const string femaleStr = "Female";
        private const string maleStr = "Male";
        private const string hispanicStr = "Hispanic/Latino";
        private const string lateEnrollmentStr = "Late Enrollment";

        public StudentDemographicMenuService(IRepository<SchoolStudentDemographic> schoolStudentDemographicRepository,
            IRepository<SchoolProgramPopulation> schoolProgramPopulationRepository,
            IRepository<SchoolIndicatorPopulation> schoolIndicatorPopulationRepository,
            ISchoolAreaLinks schoolAreaLinks)
        {
            this.schoolStudentDemographicRepository = schoolStudentDemographicRepository;
            this.schoolProgramPopulationRepository = schoolProgramPopulationRepository;
            this.schoolIndicatorPopulationRepository = schoolIndicatorPopulationRepository;
           this.schoolAreaLinks = schoolAreaLinks;
        }

        [CanBeAuthorizedBy(EdFiClaimTypes.ViewAllStudents, EdFiClaimTypes.ViewMyStudents)]
        public StudentDemographicMenuModel Get(StudentDemographicMenuRequest request)
        {
            var menuModel = new StudentDemographicMenuModel();

            var demographics = GetSchoolStudentDemographics(request);

            AddDemographicToGender(request, demographics, menuModel, femaleStr);

            AddDemographicToGender(request, demographics, menuModel, maleStr);

            AddHispanicDemographicToEthnicity(request, demographics, menuModel);

            AddRace(request, demographics, menuModel);

            var programs = GetSchoolPrograms(request);
            AddPrograms(request, programs, menuModel);

            var indicators = GetSchoolIndicators(request);
            AddIndicators(request, indicators, menuModel);

            return menuModel;
        }

        private IOrderedQueryable<SchoolStudentDemographic> GetSchoolStudentDemographics(StudentDemographicMenuRequest request)
        {
            var demographics = from demographic in schoolStudentDemographicRepository.GetAll()
                               where demographic.SchoolId == request.SchoolId && demographic.Value != null
                               orderby demographic.DisplayOrder
                               select demographic;
            return demographics;
        }

        private void AddDemographicToGender(StudentDemographicMenuRequest request, IOrderedQueryable<SchoolStudentDemographic> demographics,
            StudentDemographicMenuModel menuModel, string gender)
        {
            var demographic = demographics.SingleOrDefault(x => x.Attribute == gender);
            if (demographic == null)
            {
                return;
            }

            Debug.Assert(demographic.Value.HasValue, "demographic.Value should not be null");
            menuModel.Gender.Add(new AttributeItemWithUrl<decimal>
            {
                Attribute = demographic.Attribute,
                Value = demographic.Value.Value,
                Url = schoolAreaLinks.StudentDemographicList(request.SchoolId, demographic.Attribute)
            });
        }

        private void AddHispanicDemographicToEthnicity(StudentDemographicMenuRequest request, IOrderedQueryable<SchoolStudentDemographic> demographics,
            StudentDemographicMenuModel menuModel)
        {
            var hispanicDemographic = demographics.SingleOrDefault(x => x.Attribute == hispanicStr);
            if (hispanicDemographic != null)
            {
                Debug.Assert(hispanicDemographic.Value.HasValue, "hispanicDemographic.Value should not be null");
                menuModel.Ethnicity.Add(new AttributeItemWithUrl<decimal>
                {
                    Attribute = hispanicDemographic.Attribute,
                    Value = hispanicDemographic.Value.Value,
                    Url = schoolAreaLinks.StudentDemographicList(request.SchoolId, hispanicDemographic.Attribute)
                });
            }
        }

        private void AddRace(StudentDemographicMenuRequest request, IOrderedQueryable<SchoolStudentDemographic> demographics,
            StudentDemographicMenuModel menuModel)
        {
            foreach (
                var demographic in
                    demographics.Where(x => x.Attribute != femaleStr && x.Attribute != maleStr && x.Attribute != hispanicStr))
            {
                Debug.Assert(demographic.Value.HasValue, "demographic.Value should not be null");
                menuModel.Race.Add(new AttributeItemWithUrl<decimal>
                {
                    Attribute = demographic.Attribute,
                    Value = demographic.Value.Value,
                    Url = schoolAreaLinks.StudentDemographicList(request.SchoolId, demographic.Attribute)
                });
            }
        }

        protected virtual IEnumerable<SchoolProgramPopulation> GetSchoolPrograms(StudentDemographicMenuRequest request)
        {
            var programs = from program in schoolProgramPopulationRepository.GetAll()
                           where program.SchoolId == request.SchoolId && program.Value != null
                           orderby program.DisplayOrder
                           select program;
            return programs;
        }

        private void AddPrograms(StudentDemographicMenuRequest request, IEnumerable<SchoolProgramPopulation> programs,
            StudentDemographicMenuModel menuModel)
        {
            foreach (var program in programs)
            {
                Debug.Assert(program.Value.HasValue, "program.Value should not be null.");
                menuModel.Program.Add(
                    new AttributeItemWithUrl<decimal>
                    {
                        Attribute = program.Attribute,
                        Value = program.Value.Value,
                        Url = schoolAreaLinks.StudentDemographicList(request.SchoolId, program.Attribute)
                    });
            }
        }

        protected virtual IEnumerable<SchoolIndicatorPopulation> GetSchoolIndicators(StudentDemographicMenuRequest request)
        {
            var indicators = from indicator in schoolIndicatorPopulationRepository.GetAll()
                             where indicator.SchoolId == request.SchoolId && indicator.Value != null
                             orderby indicator.DisplayOrder
                             select indicator;
            return indicators;
        }

        private void AddIndicators(StudentDemographicMenuRequest request, IEnumerable<SchoolIndicatorPopulation> indicators,
            StudentDemographicMenuModel menuModel)
        {
            foreach (var indicator in indicators)
            {
                Debug.Assert(indicator.Value != null, "indicator.Value should not be null.");
                menuModel.Indicator.Add(new AttributeItemWithUrl<decimal>
                {
                    Attribute = indicator.Attribute,
                    Value = indicator.Value.Value,
                    Url = schoolAreaLinks.StudentDemographicList(request.SchoolId, indicator.Attribute)
                });
            }

            menuModel.Indicator.Add(new AttributeItemWithUrl<decimal>
            {
                Attribute = lateEnrollmentStr,
                Url = schoolAreaLinks.StudentDemographicList(request.SchoolId, lateEnrollmentStr)
            });

        }
    }

    public class StudentDemographicMenuUserContextApplicator : IUserContextApplicator
    {
        private const string listContext = "listContext";
        private readonly IHttpRequestProvider httpRequestProvider;

        public StudentDemographicMenuUserContextApplicator(IHttpRequestProvider httpRequestProvider)
        {
            this.httpRequestProvider = httpRequestProvider;
        }

        public Type ModelType
        {
            get { return typeof(StudentDemographicMenuModel); }
        }

        public void ApplyUserContextToModel(object modelAsObject, object requestAsObject)
        {
            var model = modelAsObject as StudentDemographicMenuModel;

            // Should never happen
            if (model == null)
                return;

            var requestListContext = httpRequestProvider.GetValue(listContext);

            // Skip processing if there's no list context to apply
            if (string.IsNullOrEmpty(requestListContext))
                return;

            requestListContext = listContext + "=" + requestListContext;

            // Add the list context to each link in the model
            foreach (var ethnicity in model.Ethnicity)
                ethnicity.Url = ethnicity.Url.AppendParameters(requestListContext);

            foreach (var gender in model.Gender)
                gender.Url = gender.Url.AppendParameters(requestListContext);

            foreach (var indicator in model.Indicator)
                indicator.Url = indicator.Url.AppendParameters(requestListContext);

            foreach (var program in model.Program)
                program.Url = program.Url.AppendParameters(requestListContext);

            foreach (var race in model.Race)
                race.Url = race.Url.AppendParameters(requestListContext);
        }
    }
}
