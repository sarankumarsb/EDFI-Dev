// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System;
using System.Collections.Generic;
using AutoMapper;
using System.Linq;
using EdFi.Dashboards.Common;
using EdFi.Dashboards.Core;
using EdFi.Dashboards.Data.Entities;
using EdFi.Dashboards.Data.Repository;
using EdFi.Dashboards.Infrastructure;
using EdFi.Dashboards.Resources.Common;
using EdFi.Dashboards.Resources.Models.School;
using EdFi.Dashboards.Resources.Models.Student;
using EdFi.Dashboards.Resources.Models.Student.Overview;
using EdFi.Dashboards.Resources.Navigation.Support;
using EdFi.Dashboards.Resources.Security.Common;
using EdFi.Dashboards.Resources.Navigation;
using EdFi.Dashboards.SecurityTokenService.Authentication;

namespace EdFi.Dashboards.Resources.StudentSchool
{
    public class OverviewRequest
    {
        public int LocalEducationAgencyId { get; set; }
        public int SchoolId { get; set; }
        public long StudentUSI { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="OverviewRequest"/> using the specified parameters.
        /// </summary>
        /// <returns>A new <see cref="OverviewRequest"/> instance.</returns>
        public static OverviewRequest Create(int schoolId, long studentUSI) 
        {
            return new OverviewRequest { SchoolId = schoolId, StudentUSI = studentUSI };
        }
        public static OverviewRequest Create(int localEducationAgencyId, int schoolId, long studentUSI)
        {
            return new OverviewRequest { LocalEducationAgencyId = localEducationAgencyId, SchoolId = schoolId, StudentUSI = studentUSI };
        }
    }

    public abstract class OverviewServiceBase<TRequest, TResponse> : IService<TRequest, TResponse>
        where TRequest : OverviewRequest
        where TResponse : OverviewModel, new()
    {
        private const string englishAsSecondLanguageAccommodation = "English as Second Language";
        private const string limitedEnglishProficiencyAccommodation = "Limited English Proficiency";
        private const string bilingualProgramAccommodation = "Bilingual Program";
        private const string overviewRenderingMode = "Overview";

        //For extensibility we inject the dependencies through properties.
        public IRepository<StudentInformation> StudentInformationRepository { get; set; }
        public IRepository<StudentSchoolInformation> StudentSchoolInformationRepository { get; set; }
        public IRepository<StudentIndicator> StudentIndicatorRepository { get; set; }
        public IStudentSchoolAreaLinks StudentSchoolAreaLinks { get; set; }
        public IAccommodationProvider AccommodationProvider { get; set; }

        [CanBeAuthorizedBy(EdFiClaimTypes.ViewAllStudents, EdFiClaimTypes.ViewMyStudents)]
        public virtual TResponse Get(TRequest request)
        {
            var studentData = (from s in StudentInformationRepository.GetAll()
                               where s.StudentUSI == request.StudentUSI
                               select s).SingleOrDefault();

            var studentSchoolData = (from s in StudentSchoolInformationRepository.GetAll()
                                    where s.StudentUSI == request.StudentUSI &&
                                            s.SchoolId == request.SchoolId
                                    select s).SingleOrDefault();

            var accommodations = AccommodationProvider.GetAccommodations(request.StudentUSI, request.SchoolId);

            InitializeMappings();

            var returnModel = new TResponse
                                  {
                                      StudentUSI = request.StudentUSI
                                  };

            //If no data then return the model.
            if (studentData == null)
                return returnModel;

            returnModel = Mapper.Map<TResponse>(studentData);

            returnModel.ProfileThumbnail = StudentSchoolAreaLinks.Image(request.SchoolId, request.StudentUSI, studentData.Gender, studentData.FullName);
            returnModel.GradeLevel = studentSchoolData.GradeLevel;
            returnModel.GiftedAndTalented = accommodations.Contains(Accommodations.GiftedAndTalented);
            returnModel.SpecialEducation = accommodations.Contains(Accommodations.SpecialEducation);
            returnModel.Designation = accommodations.Contains(Accommodations.Designation);
            returnModel.LimitedEnglishMonitoredFirstProficiency = accommodations.Contains(Accommodations.LEPMonitoredFirst);
            returnModel.LimitedEnglishMonitoredSecondProficiency = accommodations.Contains(Accommodations.LEPMonitoredSecond);
            if (accommodations.Contains(Accommodations.ESLAndLEP))
            {
                var languageAccommodations = (from p in StudentIndicatorRepository.GetAll()
                                               where p.StudentUSI == request.StudentUSI
                                               && p.EducationOrganizationId == request.LocalEducationAgencyId
                                               && p.Status == true
                                               && (p.Name == englishAsSecondLanguageAccommodation || p.Name == limitedEnglishProficiencyAccommodation || p.Name == bilingualProgramAccommodation)
                                               select p.Name).ToList();
                returnModel.EnglishAsSecondLanguage = languageAccommodations.Contains(englishAsSecondLanguageAccommodation);
                returnModel.BilingualProgram = languageAccommodations.Contains(bilingualProgramAccommodation);
                returnModel.LimitedEnglishProficiency = languageAccommodations.Contains(limitedEnglishProficiencyAccommodation);
            }
            returnModel.MetricVariantId = (int)StudentMetricEnum.Overview;
            returnModel.RenderingMode = overviewRenderingMode;

            var now = DateTime.Now;

            returnModel.AgeAsOfToday = now.Year - returnModel.DateOfBirth.Year;

            if (returnModel.DateOfBirth > now.AddYears(-returnModel.AgeAsOfToday.Value))
                returnModel.AgeAsOfToday--;

            returnModel.ParentContactInfoLink = StudentSchoolAreaLinks.Information(request.SchoolId, request.StudentUSI, studentData.FullName);
            
            OnModelMapped(returnModel,studentData);
            return returnModel;
        }


        protected virtual void InitializeMappings()
        {
            AutoMapperHelper.EnsureMapping<StudentInformation, OverviewModel, TResponse>
                (StudentInformationRepository,
                 ignore => ignore.ParentContactInfoLink,
                 ignore => ignore.MetricVariantId,
                 ignore => ignore.RenderingMode,
                 ignore => ignore.AgeAsOfToday,
                 ignore => ignore.GradeLevel,
                 ignore => ignore.LimitedEnglishProficiency,
                 ignore => ignore.LimitedEnglishMonitoredFirstProficiency,
                 ignore => ignore.LimitedEnglishMonitoredSecondProficiency,
                 ignore => ignore.BilingualProgram,
                 ignore => ignore.EnglishAsSecondLanguage,
                 ignore => ignore.GiftedAndTalented,
                 ignore => ignore.SpecialEducation,
                 ignore => ignore.Designation
                );
                    
        }

        protected virtual void OnModelMapped(TResponse model, StudentInformation data)
        {
        }
    }

    public interface IOverviewService : IService<OverviewRequest, OverviewModel> {}

    public sealed class OverviewService : OverviewServiceBase<OverviewRequest, OverviewModel>, IOverviewService { }

    public class OverviewUserContextApplicator : IUserContextApplicator
    {
        private const string listContext = "listContext";
        private readonly IHttpRequestProvider httpRequestProvider;

        public OverviewUserContextApplicator(IHttpRequestProvider httpRequestProvider)
        {
            this.httpRequestProvider = httpRequestProvider;
        }

        public Type ModelType
        {
            get { return typeof(OverviewModel); }
        }

        public void ApplyUserContextToModel(object modelAsObject, object requestAsObject)
        {
            var model = modelAsObject as OverviewModel;

            // Should never happen
            if (model == null)
                return;

            var requestListContext = httpRequestProvider.GetValue(listContext);

            // Skip processing if there's no list context to apply
            if (string.IsNullOrEmpty(requestListContext))
                return;

            requestListContext = listContext + "=" + requestListContext;

            // Add the list context to each link in the model
            model.ParentContactInfoLink = model.ParentContactInfoLink.AppendParameters(requestListContext);
        }
    }
}
