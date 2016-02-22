// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System.Collections.Generic;
using System.Linq;
using EdFi.Dashboards.Common;
using EdFi.Dashboards.Data.Entities;
using EdFi.Dashboards.Resources.Models.Student.Detail.AssessmentHistory;
using EdFi.Dashboards.Resources.Security.Common;
using EdFi.Dashboards.SecurityTokenService.Authentication;

namespace EdFi.Dashboards.Resources.StudentSchool.Detail
{
    public class AcademicChallengeHistoricalListRequest
    {
        public long StudentUSI { get; set; }
        public int SchoolId { get; set; }
        public int MetricVariantId { get; set; }

        [AuthenticationIgnore("SubjectArea does not affect the results of the request in a way requiring it to be secured.")]
        public string SubjectArea { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AcademicChallengeHistoricalListRequest"/> using the specified parameters.
        /// </summary>
        /// <returns>A new <see cref="AcademicChallengeHistoricalListRequest"/> instance.</returns>
        public static AcademicChallengeHistoricalListRequest Create(long studentUSI, string subjectArea) 
		{
			return new AcademicChallengeHistoricalListRequest { StudentUSI = studentUSI, SubjectArea = subjectArea };
		}
	}

    public abstract class AcademicChallengeHistoricalListServiceBase<TRequest, TResponse, TSubjectArea, TAssessment> : AssessmentHistoryBase<TSubjectArea, TAssessment>, IService<TRequest, TResponse>
        where TRequest : AcademicChallengeHistoricalListRequest
        where TResponse : IList<Assessment>
        where TSubjectArea : SubjectArea, new()
        where TAssessment : Assessment, new()
    {
        public IAssessmentHistorySubjectAreaProvider AssessmentHistorySubjectAreaProvider { get; set; }

        private const string advancedPlacement = "Advanced Placement";
        private const string internationalBaccalaureate = "International Baccalaureate";

        [CanBeAuthorizedBy(EdFiClaimTypes.ViewAllStudents, EdFiClaimTypes.ViewMyStudents)]
        public virtual TResponse Get(TRequest request)
        {
            InitializeMappings();

            if (request.SubjectArea == null)
            {
                request.SubjectArea = AssessmentHistorySubjectAreaProvider.GetSubjectArea(request.MetricVariantId);
            }

            var results = (from rows in StudentRecordAssessmentHistoryRepository.GetAll()
                           where rows.StudentUSI == request.StudentUSI
                                        && rows.AcademicSubject == request.SubjectArea
                                        && (rows.AssessmentCategory == advancedPlacement || rows.AssessmentCategory == internationalBaccalaureate)
                                    orderby rows.AdministrationDate, rows.AssessmentTitle
                                    select rows).ToList();

            var model = (TResponse)(IList<Assessment>)results.Select(PopulateStudentAssessmentHistoryAssessment).ToList<Assessment>();
            
            OnModelMapped(model, results);

            return model;
        }

        /// <summary>
        /// Occurs when the model has finished beeing mapped.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="data">Contains the data from the <see cref="StudentRecordAssessmentHistory"/> entity type.</param>
        /// <remarks>
        /// If you have extended the StudentRecordAssessmentHistory entity, the data will contain the extension concrete entity type which 
        /// you'll need to downcast in order to access the extended properties.
        /// </remarks>
        protected virtual void OnModelMapped(TResponse model, IEnumerable<StudentRecordAssessmentHistory> data)
        {

        }
    }

    public interface IAcademicChallengeHistoricalListService : IService<AcademicChallengeHistoricalListRequest, List<Assessment>> { }

    public sealed class AcademicChallengeHistoricalListService : AcademicChallengeHistoricalListServiceBase<AcademicChallengeHistoricalListRequest, List<Assessment>, SubjectArea, Assessment>, IAcademicChallengeHistoricalListService { }
}
