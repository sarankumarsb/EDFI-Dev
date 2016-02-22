// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using EdFi.Dashboards.Data.Entities;
using EdFi.Dashboards.Data.Repository;
using EdFi.Dashboards.Resources.Common;
using EdFi.Dashboards.Resources.Models.Student.Information;
using EdFi.Dashboards.Resources.Navigation;
using EdFi.Dashboards.Resources.School;
using EdFi.Dashboards.Resources.Security.Common;
using EdFi.Dashboards.SecurityTokenService.Authentication;
using EdFi.Dashboards.Common;

namespace EdFi.Dashboards.Resources.StudentSchool
{
    public class InformationRequest
    {
        public int LocalEducationAgencyId { get; set; }
        public int SchoolId { get; set; }
        public long StudentUSI { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="InformationRequest"/> using the specified parameters.
        /// </summary>
        /// <returns>A new <see cref="InformationRequest"/> instance.</returns>
        public static InformationRequest Create(int localEducationAgencyId, int schoolId, long studentUSI) 
        {
            return new InformationRequest { LocalEducationAgencyId = localEducationAgencyId, SchoolId = schoolId, StudentUSI = studentUSI };
        }
    }

    public abstract class InformationServiceBase<TRequest, TResponse, TSchoolInformation, TParentInformation, TStudentProgramParticipation, TOtherStudentInformation, TSpecialService> 
        : IService<TRequest, TResponse>
        where TRequest : InformationRequest
        where TResponse : InformationModel, new()
        where TSchoolInformation : SchoolInformationDetail, new()
        where TParentInformation : ParentInformation, new()
        where TStudentProgramParticipation : StudentProgramParticipation, new()
        where TOtherStudentInformation : OtherInformation, new()
        where TSpecialService : SpecialService, new()
    {
        //For extensibility we inject the dependencies through properties.
        public IRepository<StudentInformation> StudentInformationRepository { get; set; }
        public IRepository<StudentSchoolInformation> StudentSchoolInformationRepository { get; set; }
        public IRepository<StudentParentInformation> StudentParentInformationRepository { get; set; }
        public IRepository<StudentIndicator> StudentIndicatorRepository { get; set; }
        public ISchoolCategoryProvider SchoolCategoryProvider { get; set; }
        public IStudentSchoolAreaLinks StudentSchoolAreaLinks { get; set; }
        
		private Dictionary<Type, Type> runtimeEntityTypeByBaseType = new Dictionary<Type, Type>();
		protected Dictionary<Type, Type> RuntimeEntityTypeByBaseType
		{
			get { return runtimeEntityTypeByBaseType; }
			set { runtimeEntityTypeByBaseType = value; }
		}

        [CanBeAuthorizedBy(EdFiClaimTypes.ViewAllStudents, EdFiClaimTypes.ViewMyStudents)]
        public virtual TResponse Get(TRequest request)
        {
            //Get all data that we need to map this object.
            var studentInformationData = StudentInformationRepository.GetAll().SingleOrDefault(x => x.StudentUSI == request.StudentUSI);
            var schoolInformationData = StudentSchoolInformationRepository.GetAll().SingleOrDefault(x => x.SchoolId == request.SchoolId && x.StudentUSI == request.StudentUSI);
            var studentParentInformationData = StudentParentInformationRepository.GetAll().Where(x => x.StudentUSI == request.StudentUSI).OrderByDescending(x => x.PrimaryContact).ThenBy(n => n.FullName).ToList();
            var studentIndicatorBaseData = (from indicator in StudentIndicatorRepository.GetAll()
                                            where indicator.StudentUSI == request.StudentUSI
                                            && (indicator.EducationOrganizationId == request.LocalEducationAgencyId | indicator.EducationOrganizationId == request.SchoolId)
                                            orderby indicator.DisplayOrder, indicator.Name
                                            select indicator);
            var studentIndicatorProgramParticipationData = studentIndicatorBaseData.Where(indicator => indicator.Type == StudentIndicatorType.Program.ToString()).ToList();
            var studentIndicatorOtherData = studentIndicatorBaseData.Where(indicator => indicator.Type == StudentIndicatorType.Other.ToString()).ToList();
            var studentIndicatorSpecialServicesData = studentIndicatorBaseData.Where(indicator => indicator.Type == StudentIndicatorType.Special.ToString()).ToList();

            //Create empty one if student is not found.
            var si = new TResponse 
            { 
                StudentUSI = request.StudentUSI,
                FullName = "No student found.",
                ProfileThumbnail = StudentSchoolAreaLinks.Image(request.SchoolId, request.StudentUSI, "male"),
                SchoolInformation = new TSchoolInformation()
            };

            #region Mapping Data to Presentation Model

            //Lets initialize the mappings...
            InitializeMappings();

            if (studentInformationData != null)
            {
                //Map all that we can with AutoMapper. (*Takes care of all the boring left right code by hand mapping.
                si = Mapper.Map<TResponse>(studentInformationData);

                //Custom Mappings.
                si.ProfileThumbnail = StudentSchoolAreaLinks.Image(request.SchoolId, request.StudentUSI, studentInformationData.Gender, studentInformationData.FullName);

                si.SchoolCategory = SchoolCategoryProvider.GetSchoolCategoryType(request.SchoolId);
            }

            //Map School Information.
            if (schoolInformationData != null)
            {
                si.SchoolInformation = Mapper.Map<TSchoolInformation>(schoolInformationData);
                si.SchoolInformation.FeederSchools = Utilities.BuildCompactList(schoolInformationData.FeederSchools);
            }

            if (studentParentInformationData.Any())
            {
                si.Parents = Mapper.Map<List<TParentInformation>>(studentParentInformationData);
                if (si.Parents != null)
                {
                    foreach (var parentInfo in si.Parents)
                    {
                        var pData = studentParentInformationData.SingleOrDefault(x => x.ParentUSI == parentInfo.ParentUSI);

                        if (pData != null)
                            parentInfo.AddressLines = Utilities.BuildCompactList(pData.AddressLine1, pData.AddressLine2, pData.AddressLine3);
                    }
                }
            }

            //Custom mappings for these because we cant map hierarchies in Automapper.
            if (studentIndicatorProgramParticipationData.Any())
                si.Programs = GetStudentIndicatorHierarchy<TStudentProgramParticipation>(studentIndicatorProgramParticipationData, null);

            if (studentIndicatorOtherData.Any())
                si.OtherStudentInformation = GetStudentIndicatorHierarchy<TOtherStudentInformation>(studentIndicatorOtherData, null);

            if (studentIndicatorSpecialServicesData.Any())
                si.SpecialServices = GetStudentIndicatorHierarchy<TSpecialService>(studentIndicatorSpecialServicesData, null);

                
            //Before we return the model lets have a point were they can extend and change things...
            OnParentInformationMapped(si, studentParentInformationData);
            OnSchoolInformationMapped(si, schoolInformationData);
            OnStudentProgramParticipationMapped(si, studentIndicatorProgramParticipationData);
            OnOtherStudentInformationMapped(si, studentIndicatorOtherData);
            OnSpecialServicesMapped(si, studentIndicatorSpecialServicesData);
            OnStudentInformationMapped(si, studentInformationData);

            return si;
            #endregion
        }

        protected virtual void InitializeMappings()
        {
            AutoMapperHelper.EnsureMapping<StudentInformation, InformationModel, TResponse>
                (StudentInformationRepository,
                    mapping => mapping
                                .ForMember(dest => dest.AddressLines,
                                           conf => conf.MapFrom(
                                                src => Utilities.BuildCompactList(src.AddressLine1, src.AddressLine2, src.AddressLine3))),
                    ignore => ignore.SchoolCategory,
                    ignore => ignore.Parents,
                    ignore => ignore.Programs,
                    ignore => ignore.SchoolInformation,
                    ignore => ignore.OtherStudentInformation,
                    ignore => ignore.SpecialServices
                );

            AutoMapperHelper.EnsureMapping<StudentSchoolInformation, SchoolInformationDetail, TSchoolInformation>(StudentSchoolInformationRepository);

            AutoMapperHelper.EnsureMapping<StudentParentInformation, ParentInformation, TParentInformation>(StudentParentInformationRepository, ignore => ignore.AddressLines);

            AutoMapperHelper.EnsureMapping<StudentIndicator, StudentProgramParticipation, TStudentProgramParticipation>(StudentIndicatorRepository, ignore => ignore.Children);

            AutoMapperHelper.EnsureMapping<StudentIndicator, OtherInformation, TOtherStudentInformation>(StudentIndicatorRepository, ignore => ignore.Children);

            AutoMapperHelper.EnsureMapping<StudentIndicator, SpecialService, TSpecialService>(StudentIndicatorRepository, ignore => ignore.Children);
        }

        protected virtual void OnStudentInformationMapped(TResponse model, StudentInformation data)
        {
        }

        protected virtual void OnSchoolInformationMapped(TResponse model, StudentSchoolInformation data)
        {
        }

        protected virtual void OnParentInformationMapped(TResponse model, List<StudentParentInformation> data)
        {
        }

        protected virtual void OnStudentProgramParticipationMapped(TResponse model, IEnumerable<StudentIndicator> data)
        {
        }

        protected virtual void OnOtherStudentInformationMapped(TResponse model, IEnumerable<StudentIndicator> data)
        {
        }

        protected virtual void OnSpecialServicesMapped(TResponse model, IEnumerable<StudentIndicator> data)
        {
        }

        private List<T> GetStudentIndicatorHierarchy<T>(IEnumerable<StudentIndicator> data, string parentName) 
            where T : StudentIndicatorBase
        {
            var model = new List<T>();

            var childData = data.Where(x => x.ParentName == parentName);

            if (!childData.Any())
                return model; //Empty list.

            //We have children so lets add them...
            foreach (var parentData in childData)
            {
                var tempModel = Mapper.Map<T>(parentData);

                var children = GetStudentIndicatorHierarchy<T>(data, tempModel.Name);
                tempModel.Children = children.ToList<StudentIndicatorBase>();

                model.Add(tempModel);
            }

            return model;
        }
    }

    public interface IInformationService : IService<InformationRequest, InformationModel> { }

    //This is the concrete implementation of the base.
    public sealed class InformationService : InformationServiceBase<InformationRequest, InformationModel, SchoolInformationDetail, ParentInformation, StudentProgramParticipation, OtherInformation, SpecialService>, IInformationService { }
}
