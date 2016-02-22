// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using AutoMapper;
using EdFi.Dashboards.Common;
using EdFi.Dashboards.Data.Entities;
using EdFi.Dashboards.Data.Repository;
using EdFi.Dashboards.Metric.Resources.Providers;
using EdFi.Dashboards.Resources.Common;
using EdFi.Dashboards.Resources.Models.Student.Detail;
using EdFi.Dashboards.Resources.Security.Common;
using EdFi.Dashboards.SecurityTokenService.Authentication;

namespace EdFi.Dashboards.Resources.StudentSchool.Detail
{
    public class CollegeReadinessAssessmentListRequest
    {
        public long StudentUSI { get; set; }
        public int SchoolId { get; set; }
        public int MetricVariantId { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CollegeReadinessAssessmentListRequest"/> using the specified parameters.
        /// </summary>
        /// <returns>A new <see cref="CollegeReadinessAssessmentListRequest"/> instance.</returns>
        public static CollegeReadinessAssessmentListRequest Create(long studentUSI, int schoolId, int metricVariantId) 
		{
            return new CollegeReadinessAssessmentListRequest { StudentUSI = studentUSI, SchoolId = schoolId, MetricVariantId = metricVariantId };
		}
	}

    public abstract class CollegeReadinessAssessmentListServiceBase<TRequest, TResponse> : IService<TRequest, IEnumerable<TResponse>>
        where TRequest : CollegeReadinessAssessmentListRequest
        where TResponse : CollegeCareerReadinessAssessmentModel, new()
    {
        //For extensibility we inject the dependencies through properties.
        public IRepository<StudentMetricCollegeReadinessAssessment> StudentMetricCollegeReadinessAssessmentRepository { get; set; }
        public IMetricNodeResolver MetricNodeResolver { get; set; }

        [CanBeAuthorizedBy(EdFiClaimTypes.ViewAllStudents, EdFiClaimTypes.ViewMyStudents)]
        public virtual IEnumerable<TResponse> Get(TRequest request)
        {
            int schoolId = request.SchoolId;
            var studentUSI = request.StudentUSI;
            int metricId = MetricNodeResolver.ResolveMetricId(request.MetricVariantId);

            var results = (from data in StudentMetricCollegeReadinessAssessmentRepository.GetAll()
                           where data.StudentUSI == studentUSI 
                                 && data.SchoolId == schoolId 
                                 && data.MetricId == metricId
                           orderby data.Date
                           select data).ToList();

            // this has to be split out b/c Subsonic doesn't support constructors with parameters
            var model = results.Select(MapEntityToModel).ToList();

            OnParentInformationMapped(model, results);

            return model;
        }


        protected virtual TResponse MapEntityToModel(StudentMetricCollegeReadinessAssessment entity)
        {
            InitializeMappings();

            var model = Mapper.Map<TResponse>(entity);
            model.IsFlagged = (entity.Flag != null) && entity.Flag.Value;
            return model;
        }

        protected virtual void OnParentInformationMapped(List<TResponse> model, IEnumerable<StudentMetricCollegeReadinessAssessment> results)
        {
        }

        protected virtual void InitializeMappings()
        {
            AutoMapperHelper.EnsureMapping<StudentMetricCollegeReadinessAssessment, CollegeCareerReadinessAssessmentModel, TResponse>(StudentMetricCollegeReadinessAssessmentRepository, dest=>dest.IsFlagged);
            
        }
    }

    public sealed class CollegeReadinessAssessmentListService : CollegeReadinessAssessmentListServiceBase<CollegeReadinessAssessmentListRequest, CollegeCareerReadinessAssessmentModel> { };
}
