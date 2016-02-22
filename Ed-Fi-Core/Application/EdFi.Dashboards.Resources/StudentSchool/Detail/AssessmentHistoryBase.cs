// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;
using System.Linq;
using AutoMapper;
using EdFi.Dashboards.Common;
using EdFi.Dashboards.Data.Entities;
using EdFi.Dashboards.Data.Repository;
using EdFi.Dashboards.Metric.Resources.Models;
using EdFi.Dashboards.Resources.Models.Student.Detail.AssessmentHistory;

namespace EdFi.Dashboards.Resources.StudentSchool.Detail
{
    public abstract class AssessmentHistoryBase<TSubjectArea, TAssessment>
        where TAssessment : Assessment, new()
        where TSubjectArea : SubjectArea, new()
    {
        private const string yes = "Yes";
        private const string no = "No";

        public IRepository<StudentRecordAssessmentHistory> StudentRecordAssessmentHistoryRepository { get; set; }

        protected TAssessment PopulateStudentAssessmentHistoryAssessment(StudentRecordAssessmentHistory assessmentData)
        {
            InitializeMappings();

            var a = Mapper.Map<TAssessment>(assessmentData);
            a.Accommodations = assessmentData.Accommodations ? yes : String.Empty;

            if (!assessmentData.MetMinimum.HasValue)
                a.MetMinimumScore = String.Empty;
            else if (assessmentData.MetMinimum.Value != 0)
                a.MetMinimumScore = yes;
            else
                a.MetMinimumScore = no;

            if (!assessmentData.MetStandardScore.HasValue)
                a.MetStandardScore = String.Empty;
            else if (assessmentData.MetStandardScore.Value != 0)
                a.MetStandardScore = yes;
            else
                a.MetStandardScore = no;

            if (!assessmentData.CommendedScore.HasValue)
                a.CommendedScore = String.Empty;
            else if (assessmentData.CommendedScore.Value != 0)
                a.CommendedScore = yes;
            else
                a.CommendedScore = no;

            if (assessmentData.CommendedScore.HasValue && assessmentData.CommendedScore.Value != 0)
                a.ScoreState.StateType = MetricStateType.VeryGood;
            else if (assessmentData.MetStandardScore.HasValue && assessmentData.MetStandardScore.Value != 0)
                a.ScoreState.StateType = MetricStateType.Good;
            else if (assessmentData.MetStandardScore.HasValue && assessmentData.MetStandardScore.Value == 0)
                a.ScoreState.StateType = MetricStateType.Low; 
            else if (assessmentData.MetMinimum.HasValue && assessmentData.MetMinimum.Value == 0)
                a.ScoreState.StateType = MetricStateType.VeryLow;
            else
                a.ScoreState.StateType = MetricStateType.Na;

            a.ScoreState.DisplayStateText = assessmentData.Score;
            a.ScoreState.StateText = assessmentData.Score;

            OnAssessmentMapped(a,assessmentData);
            return a;
        }

        /// <summary>
        /// Occurs each time a Assessment has finished being mapped.
        /// </summary>
        /// <param name="assessment"></param>
        /// <param name="data">Contains the data from the <see cref="StudentRecordAssessmentHistory"/> entity type.</param>
        /// <remarks>
        /// If you have extended the StudentRecordAssessmentHistory entity, the data will contain the extension concrete entity type which 
        /// you'll need to downcast in order to access the extended properties.
        /// </remarks>
        protected virtual void OnAssessmentMapped(TAssessment assessment, StudentRecordAssessmentHistory data)
        {
        }

        private bool isMappingInitialized;

        protected void InitializeMappings()
        {
            if (isMappingInitialized)
                return;

            //To get the real generic argument of what is wired up.
            var realDataEntityType = StudentRecordAssessmentHistoryRepository.GetEntityType();

            if(realDataEntityType==null)
                return;

            var firstMappingMapped = false;
            var secondMappingMapped = false;

            if (Mapper.FindTypeMapFor(realDataEntityType, typeof (TSubjectArea)) == null)
            {
                //Defines mapping for the core types.
                Mapper.CreateMap<StudentRecordAssessmentHistory, SubjectArea>()
                      .ForMember(dest => dest.Name, src => src.MapFrom(x => x.AcademicSubject))
                      .ForMember(dest=>dest.Assessments, conf=>conf.Ignore())
                      .Include(realDataEntityType, typeof (TSubjectArea));
                //Define mappings for the runtime/(possibly extended) types.
                Mapper.CreateMap(realDataEntityType, typeof (TSubjectArea));

                firstMappingMapped = true;
            }

            if (Mapper.FindTypeMapFor(realDataEntityType, typeof(TAssessment)) == null)
            {
                //Defines mapping for the core types.
                Mapper.CreateMap<StudentRecordAssessmentHistory, Assessment>()
                      .ForMember(dest => dest.DateTaken, src => src.MapFrom(x => x.AdministrationDate))
                      .ForMember(dest=>dest.ScoreState, conf=>conf.Ignore())
                      .ForMember(dest => dest.MetMinimumScore, conf => conf.Ignore())
                      .Include(realDataEntityType, typeof(TAssessment));
                //Define mappings for the runtime/(possibly extended) types.
                Mapper.CreateMap(realDataEntityType, typeof (TAssessment));

                secondMappingMapped = true;
            }

            isMappingInitialized = (firstMappingMapped && secondMappingMapped);
        }
    }
}
