// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using EdFi.Dashboards.Data.Repository;
using EdFi.Dashboards.Data.Entities;
using EdFi.Dashboards.Infrastructure.Implementations.Caching;
using EdFi.Dashboards.Metric.Resources.Helpers;
using EdFi.Dashboards.Resource.Models.Common;
using EdFi.Dashboards.Resources.Common;
using EdFi.Dashboards.Resources.Models.Common;
using EdFi.Dashboards.Resources.Models.LocalEducationAgency.Information;
using EdFi.Dashboards.Resources.Navigation;
using EdFi.Dashboards.Resources.School;
using EdFi.Dashboards.Resources.Security.Common;
using EdFi.Dashboards.SecurityTokenService.Authentication;
using AutoMapper;
using EdFi.Dashboards.Common;

namespace EdFi.Dashboards.Resources.LocalEducationAgency
{
    /// <summary>
    /// Represents a request to the <see cref="IInformationService"/>.
    /// </summary>
    public class InformationRequest
    {
        /// <summary>
        /// The identifier of the local education agency.
        /// </summary>
        public int LocalEducationAgencyId { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="InformationRequest"/> using the specified parameters.
        /// </summary>
        /// <returns>A new <see cref="InformationRequest"/> instance.</returns>
        public static InformationRequest Create(int localEducationAgencyId) 
        {
            return new InformationRequest { LocalEducationAgencyId = localEducationAgencyId };
        }
    }

    public abstract class InformationServiceBase<TRequest, TResponse, TAdministrator, TAccountability, TAccountabilityRatings, TIndicatorPopulation, TProgramPopulation, TStudentDemographics> : IService<TRequest, TResponse>
        where TRequest : InformationRequest
        where TResponse : InformationModel, new()
        where TAdministrator : Administrator, new()
        where TAccountability : AttributeItem<string>, new()
        where TAccountabilityRatings : AttributeItem<string>, new()
        where TIndicatorPopulation : AttributeItemWithUrl<decimal?>, new()
        where TProgramPopulation : AttributeItemWithUrl<decimal?>, new()
        where TStudentDemographics : AttributeItemWithTrend<decimal?>, new()
    {
        public IRepository<LocalEducationAgencyInformation> LocalEducationAgencyInformationRepository { get; set; }
        public IRepository<LocalEducationAgencyStudentDemographic> StudentDemographicsRepository { get; set; }
        public IRepository<LocalEducationAgencyIndicatorPopulation> IndicatorPopulationRepository { get; set; }
        public IRepository<LocalEducationAgencyProgramPopulation> ProgramPopulationRepository { get; set; }
        public IRepository<LocalEducationAgencyAccountabilityByCategory> AccountabilityByCategoryRepository { get; set; }
        public IRepository<LocalEducationAgencyAccountabilityInformation> AccountabilityRepository { get; set; }
        public IRepository<LocalEducationAgencyAdministrationInformation> AdministrationRepository { get; set; }
        public IRepository<LocalEducationAgencyCharacteristicsInformation> CharacteristicRepository { get; set; }
        public ILocalEducationAgencyAreaLinks LocalEducationAgencyAreaLinks { get; set; }
        public ISchoolCategoryProvider SchoolCategoryProvider { get; set; }
        public ICurrentUserClaimInterrogator CurrentUserClaimInterrogator { get; set; }

        private const string mainlineTelephoneType = "mainline";
        private const string faxTelephoneType = "fax";
        private const string schoolList = "School List";
        private const string female = "Female";
        private const string male = "Male";
        private const string hispanic = "Hispanic/Latino";

        [NoCache] // the links for the student demographic page are only included if the current user has permission to see the operational dashboard
        [CanBeAuthorizedBy(EdFiClaimTypes.ViewAllMetrics, EdFiClaimTypes.ViewMyMetrics)]
        public virtual TResponse Get(TRequest request)
        {
            //Lets get all the data that we need.
            #region data
            var localEducationAgencyInformationData = LocalEducationAgencyInformationRepository.GetAll().SingleOrDefault(x => x.LocalEducationAgencyId == request.LocalEducationAgencyId);
            var leaAdministrationData = (from admin in AdministrationRepository.GetAll()
                                         where admin.LocalEducationAgencyId == request.LocalEducationAgencyId
                                         orderby admin.DisplayOrder
                                         select admin).ToList();

            var leaAccountabilityData = (from data in AccountabilityRepository.GetAll()
                                         where data.LocalEducationAgencyId == request.LocalEducationAgencyId
                                         orderby data.DisplayOrder
                                         select data).ToList();
            var leaAccountabilityRatingsData = (from data in AccountabilityByCategoryRepository.GetAll()
                                                where data.LocalEducationAgencyId == request.LocalEducationAgencyId
                                                orderby data.DisplayOrder
                                                select data).ToList();
            var leaStudentIndicatorPopulationData = (from x in IndicatorPopulationRepository.GetAll()
                                                     where x.LocalEducationAgencyId == request.LocalEducationAgencyId
                                                     orderby x.DisplayOrder
                                                     select x).ToList();
            var leaProgramPopulationData = (from x in ProgramPopulationRepository.GetAll()
                                            where x.LocalEducationAgencyId == request.LocalEducationAgencyId
                                            orderby x.DisplayOrder
                                            select x).ToList();
            var leaStudentDemographicsData = (from x in StudentDemographicsRepository.GetAll()
                                              where x.LocalEducationAgencyId == request.LocalEducationAgencyId
                                              orderby x.DisplayOrder
                                              select x).ToList();
            #endregion

            //Init mappings
            InitializeMappings();

            var model = new TResponse { Url = LocalEducationAgencyAreaLinks.Information(request.LocalEducationAgencyId) };

            if (localEducationAgencyInformationData == null)
                return model;

            //Map with Automapper.
            var leaInfo = Mapper.Map<TResponse>(localEducationAgencyInformationData);
            //Custom mappings.
            leaInfo.ProfileThumbnail = LocalEducationAgencyAreaLinks.Image(request.LocalEducationAgencyId);
            leaInfo.AddressLines = Utilities.BuildCompactList(localEducationAgencyInformationData.AddressLine1, localEducationAgencyInformationData.AddressLine2, localEducationAgencyInformationData.AddressLine3);
            leaInfo.Url = LocalEducationAgencyAreaLinks.Information(request.LocalEducationAgencyId);
            var phoneNumber = Utilities.FormattedTelephoneNumber(localEducationAgencyInformationData.TelephoneNumber);
            if (!String.IsNullOrEmpty(phoneNumber))
                leaInfo.TelephoneNumbers.Add(new TelephoneNumber { Number = phoneNumber, Type = mainlineTelephoneType });
            phoneNumber = Utilities.FormattedTelephoneNumber(localEducationAgencyInformationData.FaxNumber);
            if (!String.IsNullOrEmpty(phoneNumber))
                leaInfo.TelephoneNumbers.Add(new TelephoneNumber { Number = phoneNumber, Type = faxTelephoneType });

            leaInfo.Links = new List<Link> { new Link { Rel = schoolList, Href = LocalEducationAgencyAreaLinks.SchoolCategoryList(request.LocalEducationAgencyId) } };

            model = leaInfo;

            model.Administrators = Mapper.Map<List<TAdministrator>>(leaAdministrationData) as List<Administrator>;

            model.Accountability = Mapper.Map<List<TAccountability>>(leaAccountabilityData) as List<AttributeItem<string>>;

            model.SchoolAccountabilityRatings = Mapper.Map<List<TAccountabilityRatings>>(leaAccountabilityRatingsData) as List<AttributeItem<string>>;

            LoadCharacteristics(request.LocalEducationAgencyId, model);

            LoadStudentDemographics(model, leaStudentDemographicsData);

            model.StudentsByProgram = Mapper.Map<List<TProgramPopulation>>(leaProgramPopulationData) as List<AttributeItemWithUrl<decimal?>>;

            model.StudentIndicatorPopulation = Mapper.Map<List<TIndicatorPopulation>>(leaStudentIndicatorPopulationData) as List<AttributeItemWithUrl<decimal?>>;

            if (CurrentUserClaimInterrogator.HasClaimWithinEducationOrganizationHierarchy(EdFiClaimTypes.ViewAllMetrics, model.LocalEducationAgencyId))
                LoadStudentDemographicListUrls(model);

            OnLocalEducationAgencyInformationMapped(model, localEducationAgencyInformationData);
            return model;
        }

        private void LoadCharacteristics(int localEducationAgencyId, InformationModel model)
        {
            IList<LocalEducationAgencyCharacteristicsInformation> characteristics = CharacteristicRepository.GetAll().Where(x => x.LocalEducationAgencyId == localEducationAgencyId).ToList();

            // CharacteristicsId 1 = Local Education Agency Enrollment
            LocalEducationAgencyCharacteristicsInformation localEducationAgencyEnrollment = characteristics.SingleOrDefault(x => x.CharacteristicsId == 1);
            if (localEducationAgencyEnrollment != null)
                model.LocalEducationAgencyEnrollment = Convert.ToInt32(localEducationAgencyEnrollment.Value);

            // CharacteristicsId 2 = Number of Schools
            var numberOfSchools = characteristics.Where(x => x.CharacteristicsId == 2).OrderBy(y => y.Attribute, new SchoolCategoryComparer(SchoolCategoryProvider));
            model.NumberOfSchools = (from x in numberOfSchools
                                     select new AttributeItemWithUrl<string>
                                                {
                                                    Attribute = x.Attribute,
                                                    Value = x.Value,
                                                    Url = LocalEducationAgencyAreaLinks.StudentSchoolCategoryList(model.LocalEducationAgencyId, x.Attribute)
                                                }).ToList();
            
            // CharacteristicsId 4 = Late Enrollment Students
            LocalEducationAgencyCharacteristicsInformation lateEnrollment = characteristics.SingleOrDefault(x => x.CharacteristicsId == 4);
            if (lateEnrollment != null)
            {
                model.LateEnrollmentStudents = new AttributeItemWithUrl<decimal?> {Attribute = lateEnrollment.Attribute, Value = Convert.ToDecimal(lateEnrollment.Value)};
                if (CurrentUserClaimInterrogator.HasClaimWithinEducationOrganizationHierarchy(EdFiClaimTypes.ViewOperationalDashboard, model.LocalEducationAgencyId))
                {
                    model.LateEnrollmentStudents.Url = LocalEducationAgencyAreaLinks.StudentDemographicList(model.LocalEducationAgencyId, "Late Enrollment");
                }
            }

            // CharacteristicsId 5 = Student Teacher Ratio
            var studentTeacherRatios = characteristics.Where(x => x.CharacteristicsId == 5).OrderBy(y => y.Attribute, new SchoolCategoryComparer(SchoolCategoryProvider));
            model.StudentTeacherRatios = (from x in studentTeacherRatios
                                          select new AttributeItem<string>
                                                     {
                                                         Attribute = x.Attribute,
                                                         Value = x.Value
                                                     }).ToList();
        }

        private static void LoadStudentDemographics(InformationModel model, IEnumerable<LocalEducationAgencyStudentDemographic> data)
        {
            var result = new List<TStudentDemographics>();

            foreach (var r in data)
            {
                var demog = Mapper.Map<TStudentDemographics>(r);
                demog.Trend = TrendHelper.TrendFromDirection(r.TrendDirection);
                result.Add(demog);
            }

            var femaleResults = result.SingleOrDefault(x => String.Compare(x.Attribute, female, StringComparison.OrdinalIgnoreCase) == 0);
            model.StudentDemographics.Female = femaleResults ?? new AttributeItemWithTrend<decimal?> { Attribute = female, Value = 0 };

            var maleResults = result.SingleOrDefault(x => String.Compare(x.Attribute, male, StringComparison.OrdinalIgnoreCase) == 0);
            model.StudentDemographics.Male = maleResults ?? new AttributeItemWithTrend<decimal?> { Attribute = male, Value = 0 };

            var hispanicResults = result.SingleOrDefault(x => String.Compare(x.Attribute, hispanic, StringComparison.OrdinalIgnoreCase) == 0);
            model.StudentDemographics.ByEthnicity.Add(hispanicResults ?? new AttributeItemWithTrend<decimal?> { Attribute = hispanic, Value = 0 });

            var race = result.Where(x => String.Compare(x.Attribute, female, StringComparison.OrdinalIgnoreCase) != 0 && 
                                         String.Compare(x.Attribute, male, StringComparison.OrdinalIgnoreCase) != 0 &&
                                         String.Compare(x.Attribute, hispanic, StringComparison.OrdinalIgnoreCase) != 0);
            model.StudentDemographics.ByRace = race.ToList() as List<AttributeItemWithTrend<decimal?>>;
        }

        private void LoadStudentDemographicListUrls(InformationModel model)
        {
            model.StudentDemographics.Female.Url = LocalEducationAgencyAreaLinks.StudentDemographicList(model.LocalEducationAgencyId, female);
            model.StudentDemographics.Male.Url = LocalEducationAgencyAreaLinks.StudentDemographicList(model.LocalEducationAgencyId, male);
            foreach (var ethnicity in model.StudentDemographics.ByEthnicity)
            {
                ethnicity.Url = LocalEducationAgencyAreaLinks.StudentDemographicList(model.LocalEducationAgencyId, ethnicity.Attribute);
            }
            foreach (var byRace in model.StudentDemographics.ByRace)
            {
                byRace.Url = LocalEducationAgencyAreaLinks.StudentDemographicList(model.LocalEducationAgencyId, byRace.Attribute);
            }
            foreach (var byProgram in model.StudentsByProgram)
            {
                byProgram.Url = LocalEducationAgencyAreaLinks.StudentDemographicList(model.LocalEducationAgencyId,
                    byProgram.Attribute);
            }
            foreach (var byIndicator in model.StudentIndicatorPopulation)
            {
                byIndicator.Url = LocalEducationAgencyAreaLinks.StudentDemographicList(
                    model.LocalEducationAgencyId, byIndicator.Attribute);
            }
        }

        protected virtual void InitializeMappings()
        {
            AutoMapperHelper.EnsureMapping<LocalEducationAgencyInformation, InformationModel, TResponse>
                (LocalEducationAgencyInformationRepository,
                        ignore => ignore.AddressLines,
                        ignore => ignore.TelephoneNumbers,
                        ignore => ignore.Administrators,
                        ignore => ignore.Accountability,
                        ignore => ignore.SchoolAccountabilityRatings,
                        ignore => ignore.LocalEducationAgencyEnrollment,
                        ignore => ignore.NumberOfSchools,
                        ignore => ignore.LateEnrollmentStudents,
                        ignore => ignore.StudentTeacherRatios,
                        ignore => ignore.StudentDemographics,
                        ignore => ignore.StudentsByProgram,
                        ignore => ignore.StudentIndicatorPopulation,
                        ignore => ignore.Url,
                        ignore => ignore.ResourceUrl,
                        ignore => ignore.Links);

            AutoMapperHelper.EnsureMapping<LocalEducationAgencyAdministrationInformation, Administrator, TAdministrator>(AdministrationRepository);

            AutoMapperHelper.EnsureMapping<LocalEducationAgencyAccountabilityInformation, AttributeItem<string>,TAccountability>(AccountabilityRepository);

            AutoMapperHelper.EnsureMapping<LocalEducationAgencyAccountabilityByCategory, AttributeItem<string>,TAccountabilityRatings>(AccountabilityByCategoryRepository);

            AutoMapperHelper.EnsureMapping<LocalEducationAgencyIndicatorPopulation, AttributeItemWithUrl<decimal?>,TIndicatorPopulation>
                (IndicatorPopulationRepository,
                 ignore => ignore.Url
                );

            AutoMapperHelper.EnsureMapping<LocalEducationAgencyProgramPopulation, AttributeItemWithUrl<decimal?>,TIndicatorPopulation>
                (ProgramPopulationRepository,
                 ignore => ignore.Url
                );

            AutoMapperHelper.EnsureMapping<LocalEducationAgencyStudentDemographic, AttributeItemWithTrend<decimal?>,TStudentDemographics>
                (StudentDemographicsRepository,
                 ignore => ignore.Url,
                 ignore => ignore.Trend
                );
        }

        protected virtual void OnLocalEducationAgencyInformationMapped(TResponse model, LocalEducationAgencyInformation data)
        {
        }
    }

    public interface IInformationService : IService<InformationRequest, InformationModel> { }

    public sealed class InformationService : InformationServiceBase<InformationRequest, InformationModel, Administrator, AttributeItem<string>, AttributeItem<string>, AttributeItemWithUrl<decimal?>, AttributeItemWithUrl<decimal?>, AttributeItemWithTrend<decimal?>>, IInformationService { }

    public class SchoolCategoryComparer : IComparer<string>
    {
        private readonly ISchoolCategoryProvider schoolCategoryProvider;

        public SchoolCategoryComparer(ISchoolCategoryProvider schoolCategoryProvider)
        {
            this.schoolCategoryProvider = schoolCategoryProvider;
        }

        public int Compare(string x, string y)
        {
            int xCompare = schoolCategoryProvider.GetSchoolCategoryPriorityForSorting(x);
            int yCompare = schoolCategoryProvider.GetSchoolCategoryPriorityForSorting(y);
            return xCompare.CompareTo(yCompare);
        }
    }
}
