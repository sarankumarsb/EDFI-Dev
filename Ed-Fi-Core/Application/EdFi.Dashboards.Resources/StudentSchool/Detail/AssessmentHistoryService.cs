// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System.Collections.Generic;
using System.Linq;
using EdFi.Dashboards.Common;
using EdFi.Dashboards.Data.Entities;
using EdFi.Dashboards.Data.Repository;
using EdFi.Dashboards.Resources.Models.Student.Detail;
using EdFi.Dashboards.Resources.Models.Student.Detail.AssessmentHistory;
using EdFi.Dashboards.Resources.Security.Common;
using EdFi.Dashboards.SecurityTokenService.Authentication;
using AutoMapper;

namespace EdFi.Dashboards.Resources.StudentSchool.Detail
{
    public class AssessmentHistoryRequest
    {
        public long StudentUSI { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AssessmentHistoryRequest"/> using the specified parameters.
        /// </summary>
        /// <returns>A new <see cref="AssessmentHistoryRequest"/> instance.</returns>
        public static AssessmentHistoryRequest Create(long studentUSI) 
		{
			return new AssessmentHistoryRequest { StudentUSI = studentUSI };
		}
	}

    public abstract class AssessmentHistoryServiceBase<TRequest, TResponse, TSubjectArea, TAssessment> : AssessmentHistoryBase<TSubjectArea, TAssessment>, IService<TRequest, TResponse>
        where TRequest : AssessmentHistoryRequest
        where TResponse : AssessmentHistoryModel, new()
        where TSubjectArea : SubjectArea, new()
        where TAssessment : Assessment, new()
    {

        [CanBeAuthorizedBy(EdFiClaimTypes.ViewAllStudents, EdFiClaimTypes.ViewMyStudents)]
        public virtual TResponse Get(TRequest request)
        {
            InitializeMappings();

            var coreModel = new TResponse { StudentUSI = request.StudentUSI};

            var subjects = (from rows in StudentRecordAssessmentHistoryRepository.GetAll()
                           where rows.StudentUSI == request.StudentUSI
                           orderby rows.AcademicSubject, rows.AdministrationDate
                           group rows by rows.AcademicSubject into s
                           select new { Name = s.Key, Assessments = s }).ToList();

            var dataForExtension = from s in subjects
                                   select new AcademicSubjectGrouping {Name = s.Name, AssessmentGrouping = s.Assessments};

            foreach (var subject in dataForExtension)
            {
                var sa = new TSubjectArea { StudentUSI = request.StudentUSI, Name = subject.Name };

                //For extensibility purposes lets get a sample row of data and map it with AutoMapper.
                var dataSample = subject.AssessmentGrouping.FirstOrDefault();
                if (dataSample != null)
                    sa = Mapper.Map<TSubjectArea>(dataSample);

                foreach (var assessment in subject.AssessmentGrouping)
                    sa.Assessments.Add(PopulateStudentAssessmentHistoryAssessment(assessment));

                OnSubjectAreaMapped(sa,subject);
                coreModel.SubjectAreas.Add(sa);
            }

            OnModelMapped(coreModel, dataForExtension);

            return coreModel;
        }

        /// <summary>
        /// Occurs when the model has finished being mapped.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="data">Contains the data from the <see cref="StudentRecordAssessmentHistory"/> entity type, grouped by academic subject.</param>
        /// <example>
        /// foreach (var subject in data)
        /// {
        ///     //Properties available.
        ///     subject.Name;
        ///     subject.AssessmentGrouping;
        /// 
        ///     foreach (var assessment in subject.AssessmentGrouping)
        ///     {
        ///         assessment.*; //Where * is the name of a property.
        ///     }
        /// }
        /// </example>
        /// <remarks>
        /// If you have extended the StudentRecordAssessmentHistory entity, the data will contain the extension concrete entity type which 
        /// you'll need to downcast in order to access the extended properties.
        /// </remarks>
        protected virtual void OnModelMapped(TResponse model, IEnumerable<AcademicSubjectGrouping> data)
        {
        }

        protected class AcademicSubjectGrouping
        {
            public string Name { get; set; }
            public IGrouping<string,StudentRecordAssessmentHistory> AssessmentGrouping { get; set; }
        }

        /// <summary>
        /// Occurs each time a Subject Area has finished beeing mapped.
        /// </summary>
        /// <param name="subjectArea"></param>
        /// <param name="data">Contains the data from the <see cref="StudentRecordAssessmentHistory"/> entity type, grouped by academic subject.</param>
        /// <remarks>
        /// If you have extended the StudentRecordAssessmentHistory entity, the data will contain the extension concrete entity type which 
        /// you'll need to downcast in order to access the extended properties.
        /// </remarks>
        protected virtual void OnSubjectAreaMapped(TSubjectArea subjectArea, AcademicSubjectGrouping data)
        {
        }

    }

    public interface IAssessmentHistoryService : IService<AssessmentHistoryRequest, AssessmentHistoryModel> { }

    public sealed class AssessmentHistoryService : AssessmentHistoryServiceBase<AssessmentHistoryRequest, AssessmentHistoryModel, SubjectArea, Assessment>, IAssessmentHistoryService { }
}
