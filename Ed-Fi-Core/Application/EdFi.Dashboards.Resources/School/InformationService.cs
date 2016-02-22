// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using EdFi.Dashboards.Common;
using EdFi.Dashboards.Core;
using EdFi.Dashboards.Data.Entities;
using EdFi.Dashboards.Data.Repository;
using EdFi.Dashboards.Infrastructure;
using EdFi.Dashboards.Infrastructure.Implementations.Caching;
using EdFi.Dashboards.Metric.Resources;
using EdFi.Dashboards.Metric.Resources.Helpers;
using EdFi.Dashboards.Metric.Resources.Models;
using EdFi.Dashboards.Resources.Common;
using EdFi.Dashboards.Resources.Metric;
using EdFi.Dashboards.Resources.Metric.Requests;
using EdFi.Dashboards.Resources.Models.Common;
using EdFi.Dashboards.Resources.Models.School.Information;
using EdFi.Dashboards.Resources.Navigation;
using EdFi.Dashboards.Resources.Navigation.Support;
using EdFi.Dashboards.Resources.Security.Common;
using EdFi.Dashboards.SecurityTokenService.Authentication;
using SchoolInformation = EdFi.Dashboards.Data.Entities.SchoolInformation;
using AutoMapper;

namespace EdFi.Dashboards.Resources.School
{
    public class InformationRequest
    {
        public int SchoolId { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="InformationRequest"/> using the specified parameters.
        /// </summary>
        /// <returns>A new <see cref="InformationRequest"/> instance.</returns>
        public static InformationRequest Create(int schoolId)
        {
            return new InformationRequest { SchoolId = schoolId };
        }
    }

    //Service
    public abstract class InformationServiceBase<TRequest, TResponse, TAccountability, TSchoolAdministration, TGradePopulation, TGradePopulationIndicators, TStudentDemographics, TProgramPopulation, TFeederSchools, TGraduationPlan> : IService<TRequest, TResponse>
        where TRequest : InformationRequest
        where TResponse : InformationModel, new()
        where TAccountability : AttributeItem<string>, new()
        where TSchoolAdministration : Administrator, new()
        where TGradePopulation : AttributeItemWithTrend<decimal?>, new()
        where TGradePopulationIndicators : AttributeItemWithTrend<decimal?>, new()
        where TStudentDemographics : AttributeItemWithTrend<decimal?>, new()
        where TProgramPopulation : AttributeItemWithTrend<decimal?>, new()
        where TFeederSchools : AttributeItemWithTrend<decimal?>, new()
        where TGraduationPlan : AttributeItemWithTrend<decimal?>, new()
    {
        //For extensibility we inject the dependencies through properties. This to prevent a constructor inheritance explosion.
        public IRepository<SchoolInformation> SchoolInformationRepository { get; set; }
        public IRepository<SchoolAdministrationInformation> SchoolAdministrationInformationRepository { get; set; }
        public IRepository<SchoolGradePopulation> SchoolGradePopulationRepository { get; set; }
        public IRepository<SchoolIndicatorPopulation> SchoolIndicatorPopulationRepository { get; set; }
        public IRepository<SchoolAccountabilityInformation> SchoolAccountabilityInformationRepository { get; set; }
        public IRepository<SchoolProgramPopulation> SchoolProgramPopulationRepository { get; set; }
        public IRepository<SchoolStudentDemographic> SchoolStudentDemographicRepository { get; set; }
        public IRepository<SchoolFeederSchool> SchoolFeederSchoolRepository { get; set; }
        public IDomainSpecificMetricNodeResolver DomainSpecificMetricNodeResolver { get; set; }
        public IDomainMetricService<SchoolMetricInstanceSetRequest> DomainMetricService { get; set; }
        public ISchoolCategoryProvider SchoolCategoryProvider { get; set; }
        public ISchoolAreaLinks SchoolAreaLinks { get; set; }
        public ICurrentUserClaimInterrogator CurrentUserClaimInterrogator { get; set; }

        [NoCache]// the links for the student demographic page are only included if the current user has permission to see the operational dashboard
        [CanBeAuthorizedBy(EdFiClaimTypes.ViewAllMetrics, EdFiClaimTypes.ViewMyMetrics)]
        public virtual TResponse Get(TRequest request)
        {
            var model = new TResponse();

            #region Data
            //Lets get all the data objects that we will need to populate our model.
            var schoolInformationData = SchoolInformationRepository.GetAll().SingleOrDefault(x => x.SchoolId == request.SchoolId);
            var schoolAccountabilityData = (from a in SchoolAccountabilityInformationRepository.GetAll()
                                            where a.SchoolId == request.SchoolId
                                            orderby a.DisplayOrder
                                            select a).ToList();

            var schoolAdministrationData = (from s in SchoolAdministrationInformationRepository.GetAll()
                                            where s.SchoolId == request.SchoolId
                                            orderby s.DisplayOrder
                                            select s).ToList();

            var schoolIndicatorPopulationData = (from i in SchoolIndicatorPopulationRepository.GetAll()
                                                 where i.SchoolId == request.SchoolId
                                                 orderby i.DisplayOrder
                                                 select i).ToList();

            var gradePopulationData = SchoolGradePopulationRepository.GetAll().Where(x => x.SchoolId == request.SchoolId).OrderBy(x => x.DisplayOrder).ToList();

            var studentDemographicsData = (from x in SchoolStudentDemographicRepository.GetAll()
                                           where x.SchoolId == request.SchoolId
                                           orderby x.DisplayOrder
                                           select x).ToList();

            var schoolProgramPopulationRepositoryData = (from s in SchoolProgramPopulationRepository.GetAll()
                                                         where s.SchoolId == request.SchoolId
                                                         orderby s.DisplayOrder
                                                         select s).ToList();

            var schoolFeederSchoolData = (from f in SchoolFeederSchoolRepository.GetAll()
                                          where f.SchoolId == request.SchoolId
                                          orderby f.DisplayOrder
                                          select f).ToList();
            #endregion

            InitializeMappings();

            //Lets map the basic info like SchoolId, Name, Thumbnail, Address, etc...
            if (schoolInformationData != null)
            {
                //Auto map as much as possible.
                model = Mapper.Map<TResponse>(schoolInformationData);

                model.SchoolCategory = SchoolCategoryProvider.GetSchoolCategoryType(schoolInformationData.SchoolId);
                model.ProfileThumbnail = SchoolAreaLinks.Image(request.SchoolId);
                model.AddressLines = Utilities.BuildCompactList(schoolInformationData.AddressLine1, schoolInformationData.AddressLine2, schoolInformationData.AddressLine3);
                model.TelephoneNumbers.Add(new TelephoneNumber { Number = Utilities.FormattedTelephoneNumber(schoolInformationData.TelephoneNumber), Type = "mainline" });
                if (!String.IsNullOrEmpty(schoolInformationData.FaxNumber))
                    model.TelephoneNumbers.Add(new TelephoneNumber { Number = Utilities.FormattedTelephoneNumber(schoolInformationData.FaxNumber), Type = "fax" });
            }

            //Init the admin Info.
            var administration = Mapper.Map<List<TSchoolAdministration>>(schoolAdministrationData);
            model.Administration = administration as List<Administrator>;
            OnSchoolAdministrationInformationMapped(model, schoolAdministrationData);

            //Init the Accountability Info.
            var accountability = Mapper.Map<List<TAccountability>>(schoolAccountabilityData);
            model.Accountability = accountability as List<AttributeItem<string>>;
            OnSchoolAccountabilityInformationMapped(model, schoolAccountabilityData);

            //The Grade Population
            InitializeSchoolGradePopulation(model, gradePopulationData);
            OnSchoolGradePopulationMapped(model, gradePopulationData);

            //The Population Indicators for grades
            InitializeSchoolIndicatorPopulation(model, schoolIndicatorPopulationData);
            OnSchoolIndicatorPopulationMapped(model, schoolIndicatorPopulationData);

            //The StudentDemographics
            InitializeSchoolStudentDemographics(model, studentDemographicsData);
            OnSchoolStudentDemographicMapped(model, studentDemographicsData);

            //The ProgramPopulation
            InitializeSchoolProgramPopulation(model, schoolProgramPopulationRepositoryData);
            OnSchoolProgramPopulationMapped(model, schoolProgramPopulationRepositoryData);

            //The FeederSchool
            InitializeSchoolFeederSchoolDistribution(model, schoolFeederSchoolData);
            OnSchoolFeederSchoolMapped(model, schoolFeederSchoolData);

            InitializeSchoolGraduationPlan(model, request.SchoolId);

            OnSchoolInformationMapped(model, schoolInformationData);
            return model;
        }

        private void InitializeSchoolGradePopulation(TResponse model, IEnumerable<SchoolGradePopulation> gradePopulationData)
        {
            var studentLinksEnabled = ((CurrentUserClaimInterrogator.HasClaimWithinEducationOrganizationHierarchy(EdFiClaimTypes.ViewAllStudents, model.SchoolId))
                                    || (CurrentUserClaimInterrogator.HasClaimWithinEducationOrganizationHierarchy(EdFiClaimTypes.ViewMyStudents, model.SchoolId)));

            //Totals are always defined in the first row.
            var total = gradePopulationData.FirstOrDefault();

            //The rest contains rows of information.
            var theRest = gradePopulationData.Skip(1).OrderBy(x => x.DisplayOrder).ToList();

            if (total != null)
            {
                var totalNumberOfStudents = Mapper.Map<TGradePopulation>(total);
                totalNumberOfStudents.Trend = TrendHelper.TrendFromDirection(total.TrendDirection);
                if (studentLinksEnabled)
                    totalNumberOfStudents.Url = SchoolAreaLinks.StudentGradeList(model.SchoolId);
                model.GradePopulation.TotalNumberOfStudents = totalNumberOfStudents;

                var lateEnrollment = new AttributeItemWithUrl<decimal?> {Attribute = "Late Enrollment", Value = total.LateEnrollment};
                if (CurrentUserClaimInterrogator.HasClaimWithinEducationOrganizationHierarchy(EdFiClaimTypes.ViewOperationalDashboard, model.SchoolId))
                {
                    lateEnrollment.Url = SchoolAreaLinks.StudentDemographicList(model.SchoolId, lateEnrollment.Attribute);
                }
                model.GradePopulation.SchoolLateEnrollment = lateEnrollment;

            }

            foreach (var row in theRest)
            {
                var x = Mapper.Map<TGradePopulation>(row);
                x.Trend = TrendHelper.TrendFromDirection(row.TrendDirection);
                if (studentLinksEnabled)
                    x.Url = SchoolAreaLinks.StudentGradeList(model.SchoolId, null, x.Attribute);
                model.GradePopulation.TotalNumberOfStudentsByGrade.Add(x);
            }
        }

        private void InitializeSchoolStudentDemographics(TResponse model, IEnumerable<SchoolStudentDemographic> data)
        {
            const string femaleStr = "Female";
            const string maleStr = "Male";
            const string hispanicStr = "Hispanic/Latino";

            var result = new List<TStudentDemographics>();

            foreach (var r in data)
            {
                var demog = Mapper.Map<TStudentDemographics>(r);
                demog.Trend = TrendHelper.TrendFromDirection(r.TrendDirection);
                result.Add(demog);
            }


            //Female calculation...
            var female = result.SingleOrDefault(x => String.Compare(x.Attribute, femaleStr, true) == 0);
            model.StudentDemographics.Female = female ?? new AttributeItemWithTrend<decimal?> { Attribute = femaleStr, Value = 0 };

            //Male calculation...
            var male = result.SingleOrDefault(x => String.Compare(x.Attribute, maleStr, true) == 0);
            model.StudentDemographics.Male = male ?? new AttributeItemWithTrend<decimal?> { Attribute = maleStr, Value = 0 };

            //Hispanic...
            var hispanic = result.SingleOrDefault(x => String.Compare(x.Attribute, hispanicStr, true) == 0);
            model.StudentDemographics.ByEthnicity.Add(hispanic ?? new AttributeItemWithTrend<decimal?> { Attribute = hispanicStr, Value = 0 });

            //Race...
            var race = result.Where(x => String.Compare(x.Attribute, femaleStr, true) != 0 && String.Compare(x.Attribute, maleStr, true) != 0 && String.Compare(x.Attribute, hispanicStr, true) != 0);
            model.StudentDemographics.ByRace = race.ToList() as List<AttributeItemWithTrend<decimal?>>;
            if (CurrentUserClaimInterrogator.HasClaimWithinEducationOrganizationHierarchy(EdFiClaimTypes.ViewAllMetrics, model.SchoolId))
            {
                model.StudentDemographics.Female.Url = SchoolAreaLinks.StudentDemographicList(model.SchoolId, femaleStr);
                model.StudentDemographics.Male.Url = SchoolAreaLinks.StudentDemographicList(model.SchoolId, maleStr);
                foreach (var ethnicity in model.StudentDemographics.ByEthnicity)
                {
                    ethnicity.Url = SchoolAreaLinks.StudentDemographicList(model.SchoolId, ethnicity.Attribute);
                }
                foreach (var byRace in model.StudentDemographics.ByRace)
                {
                    byRace.Url = SchoolAreaLinks.StudentDemographicList(model.SchoolId, byRace.Attribute);
                }
            }
        }

        private void InitializeSchoolProgramPopulation(TResponse model, IEnumerable<SchoolProgramPopulation> data)
        {
            foreach (var schoolProgramPopulation in data)
            {
                var studentProgram = Mapper.Map<TProgramPopulation>(schoolProgramPopulation);
                studentProgram.Trend = TrendHelper.TrendFromDirection(schoolProgramPopulation.TrendDirection);
                model.StudentsByProgram.Add(studentProgram);
            }

            if (CurrentUserClaimInterrogator.HasClaimWithinEducationOrganizationHierarchy(EdFiClaimTypes.ViewAllMetrics, model.SchoolId))
            {
                foreach (var program in model.StudentsByProgram)
                {
                    program.Url = SchoolAreaLinks.StudentDemographicList(model.SchoolId, program.Attribute);
                }
            }
        }

        private static void InitializeSchoolFeederSchoolDistribution(TResponse model, IEnumerable<SchoolFeederSchool> data)
        {
            foreach (var schoolFeederSchool in data)
            {
                var feederSchool = Mapper.Map<TFeederSchools>(schoolFeederSchool);
                feederSchool.Trend = TrendHelper.TrendFromDirection(schoolFeederSchool.TrendDirection);
                model.FeederSchoolDistribution.Add(feederSchool);
            }
        }

        private void InitializeSchoolIndicatorPopulation(TResponse model, IEnumerable<SchoolIndicatorPopulation> schoolIndicatorPopulationData)
        {
            foreach (var indicator in schoolIndicatorPopulationData)
            {
                var indi = Mapper.Map<TGradePopulationIndicators>(indicator);
                indi.Trend = TrendHelper.TrendFromDirection(indicator.TrendDirection);
                model.GradePopulation.Indicators.Add(indi);
            }

            if (CurrentUserClaimInterrogator.HasClaimWithinEducationOrganizationHierarchy(EdFiClaimTypes.ViewAllMetrics, model.SchoolId))
            {
                foreach (var indicator in model.GradePopulation.Indicators)
                {
                    indicator.Url = SchoolAreaLinks.StudentDemographicList(model.SchoolId, indicator.Attribute);
                }
            }
        }

        private void InitializeSchoolGraduationPlan(TResponse model, int schoolId)
        {
            if (model.SchoolCategory != SchoolCategory.HighSchool)
                return;

            //Resolve the MetadataNode
            var gradPlanMetricMetadataNode = DomainSpecificMetricNodeResolver.GetSchoolHighSchoolGraduationPlan();

            if(gradPlanMetricMetadataNode == null)
                return;

            //Get the actual graduation plan metric.
            ContainerMetric graduationPlanContainerMetric = null;

            try
            {
                graduationPlanContainerMetric = DomainMetricService.Get(SchoolMetricInstanceSetRequest.Create(schoolId, gradPlanMetricMetadataNode.MetricVariantId)).RootNode as ContainerMetric;
            }
            catch (MetricNodeNotFoundException)
            {
                return;
            }


            if (graduationPlanContainerMetric == null)
                return;

            if (graduationPlanContainerMetric.Children.Count() == 0)
                return;

            foreach (var child in graduationPlanContainerMetric.Children)
            {
                var gm = child as GranularMetric<double>;
                if (gm == null)
                    continue;

                var gradPlan = Mapper.Map<TGraduationPlan>(gm);
                gradPlan.Value = Convert.ToDecimal(gm.Value);
                gradPlan.Trend = TrendHelper.TrendFromDirection(gm.TrendDirection);

                model.HighSchoolGraduationPlan.Add(gradPlan);

                OnSchoolGraduationPlanMapped(model, gm);
            }
        }

        protected virtual void InitializeMappings()
        {
            AutoMapperHelper.EnsureMapping<SchoolInformation, InformationModel, TResponse>
                (SchoolInformationRepository,
                 ignore => ignore.AddressLines,
                 ignore => ignore.TelephoneNumbers,
                 ignore => ignore.Administration,
                 ignore => ignore.Accountability,
                 ignore => ignore.GradePopulation,
                 ignore => ignore.StudentDemographics,
                 ignore => ignore.StudentsByProgram,
                 ignore => ignore.FeederSchoolDistribution,
                 ignore => ignore.HighSchoolGraduationPlan,
                 ignore => ignore.SchoolCategory);
            //Weird Special Case for SchoolCategory property.
            Mapper.CreateMap(SchoolInformationRepository.GetEntityType(), typeof(TResponse))
                      .ForMember("SchoolCategory", x => x.Ignore());

            AutoMapperHelper.EnsureMapping<SchoolAccountabilityInformation, AttributeItem<string>, TAccountability>(SchoolAccountabilityInformationRepository);

            AutoMapperHelper.EnsureMapping<SchoolAdministrationInformation, Administrator, TSchoolAdministration>(SchoolAdministrationInformationRepository);

            AutoMapperHelper.EnsureMapping<SchoolIndicatorPopulation, AttributeItemWithTrend<decimal?>, TGradePopulation>
                (SchoolIndicatorPopulationRepository,
                 x => x.Trend,
                 x => x.Url);

            AutoMapperHelper.EnsureMapping<SchoolGradePopulation, AttributeItemWithTrend<decimal?>, TGradePopulationIndicators>
                (SchoolGradePopulationRepository,
                 x => x.Trend,
                 x => x.Url);
        
            AutoMapperHelper.EnsureMapping<SchoolStudentDemographic, AttributeItemWithTrend<decimal?>, TStudentDemographics>
                (SchoolStudentDemographicRepository,
                 x => x.Trend,
                 x => x.Url);

            AutoMapperHelper.EnsureMapping<SchoolProgramPopulation, AttributeItemWithTrend<decimal?>, TProgramPopulation>
                (SchoolProgramPopulationRepository,
                 x => x.Trend,
                 x => x.Url);

            AutoMapperHelper.EnsureMapping<SchoolFeederSchool, AttributeItemWithTrend<decimal?>, TFeederSchools>
                (SchoolFeederSchoolRepository,
                 x => x.Trend,
                 x => x.Url);


            if (Mapper.FindTypeMapFor(typeof(GranularMetric<Double>), typeof(TGraduationPlan)) == null)
            {
                Mapper.CreateMap<GranularMetric<Double>, AttributeItemWithTrend<decimal?>>()
                      .ForMember(dest => dest.Attribute, src => src.MapFrom(x => x.Name))
                      .ForMember(dest => dest.Value, conf => conf.Ignore())
                      .ForMember(dest => dest.Trend, conf => conf.Ignore())
                      .Include(typeof(GranularMetric<Double>), typeof(TGraduationPlan));
                Mapper.CreateMap(typeof(GranularMetric<Double>), typeof(TGraduationPlan));
            }
        }

        protected virtual void OnSchoolInformationMapped(TResponse model, SchoolInformation data)
        {
        }

        protected virtual void OnSchoolAccountabilityInformationMapped(TResponse model, IEnumerable<SchoolAccountabilityInformation> data)
        {
        }

        protected virtual void OnSchoolAdministrationInformationMapped(TResponse model, IEnumerable<SchoolAdministrationInformation> data)
        {
        }

        protected virtual void OnSchoolIndicatorPopulationMapped(TResponse model, IEnumerable<SchoolIndicatorPopulation> data)
        {
        }

        protected virtual void OnSchoolGradePopulationMapped(TResponse model, IEnumerable<SchoolGradePopulation> data)
        {
        }

        protected virtual void OnSchoolStudentDemographicMapped(TResponse model, IEnumerable<SchoolStudentDemographic> data)
        {
        }

        protected virtual void OnSchoolProgramPopulationMapped(TResponse model, IEnumerable<SchoolProgramPopulation> data)
        {
        }

        protected virtual void OnSchoolFeederSchoolMapped(TResponse model, IEnumerable<SchoolFeederSchool> data)
        {
        }

        protected virtual void OnSchoolGraduationPlanMapped(TResponse model, GranularMetric<double> data)
        {
        }
    }

    public interface IInformationService : IService<InformationRequest, InformationModel> { }

    public sealed class InformationService : InformationServiceBase<InformationRequest, InformationModel, AttributeItem<string>, Administrator, AttributeItemWithTrend<decimal?>, AttributeItemWithTrend<decimal?>, AttributeItemWithTrend<decimal?>, AttributeItemWithTrend<decimal?>, AttributeItemWithTrend<decimal?>, AttributeItemWithTrend<decimal?>>, IInformationService { }


    public class InformationUserContextApplicator : IUserContextApplicator
    {
        private const string listContext = "listContext";
        private readonly IHttpRequestProvider httpRequestProvider;

        public InformationUserContextApplicator(IHttpRequestProvider httpRequestProvider)
        {
            this.httpRequestProvider = httpRequestProvider;
        }

        public Type ModelType
        {
            get { return typeof(InformationModel); }
        }

        public void ApplyUserContextToModel(object modelAsObject, object requestAsObject)
        {
            var model = modelAsObject as InformationModel;

            // Should never happen
            if (model == null)
                return;

            var requestListContext = httpRequestProvider.GetValue(listContext);

            // Skip processing if there's no list context to apply
            if (string.IsNullOrEmpty(requestListContext))
                return;

            requestListContext = listContext + "=" + requestListContext;

            // Add the list context to each link in the model
            model.GradePopulation.TotalNumberOfStudents.Url = model.GradePopulation.TotalNumberOfStudents.Url.AppendParameters(requestListContext);
            model.GradePopulation.SchoolLateEnrollment.Url = model.GradePopulation.SchoolLateEnrollment.Url.AppendParameters(requestListContext);
            foreach (var studentsByGrade in model.GradePopulation.TotalNumberOfStudentsByGrade)
                studentsByGrade.Url = studentsByGrade.Url.AppendParameters(requestListContext);
            foreach (var otherInfo in model.GradePopulation.Indicators)
                otherInfo.Url = otherInfo.Url.AppendParameters(requestListContext);

            foreach (var graduationPlan in model.HighSchoolGraduationPlan)
                graduationPlan.Url = graduationPlan.Url.AppendParameters(requestListContext);

            model.StudentDemographics.Female.Url = model.StudentDemographics.Female.Url.AppendParameters(requestListContext);
            model.StudentDemographics.Male.Url = model.StudentDemographics.Male.Url.AppendParameters(requestListContext);
            foreach (var studentEthnicity in model.StudentDemographics.ByEthnicity)
                studentEthnicity.Url = studentEthnicity.Url.AppendParameters(requestListContext);
            foreach (var studentRace in model.StudentDemographics.ByRace)
                studentRace.Url = studentRace.Url.AppendParameters(requestListContext);

            foreach (var program in model.StudentsByProgram)
                program.Url = program.Url.AppendParameters(requestListContext);
        }
    }
}
