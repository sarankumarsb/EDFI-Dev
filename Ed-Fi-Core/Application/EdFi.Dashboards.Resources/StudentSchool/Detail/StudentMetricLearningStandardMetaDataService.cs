// *************************************************************************
// ?
// *************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using EdFi.Dashboards.Common;
using EdFi.Dashboards.Data.Entities;
using EdFi.Dashboards.Data.Repository;
using EdFi.Dashboards.Metric.Resources.Models;
using EdFi.Dashboards.Metric.Resources.Providers;
using EdFi.Dashboards.Resources.Models.Student.Detail;
using EdFi.Dashboards.Resources.Security.Common;
using EdFi.Dashboards.SecurityTokenService.Authentication;

namespace EdFi.Dashboards.Resources.StudentSchool.Detail
{

    // request
    public class StudentMetricLearningStandardMetaDataRequest
    {

        public int MetricVariantId { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="StudentMetricLearningStandardMetaDataRequest"/> using the specified parameters.
        /// </summary>
        /// <returns>A new <see cref="StudentMetricLearningStandardMetaDataRequest"/> instance.</returns>
        public static StudentMetricLearningStandardMetaDataRequest Create(int metricVariantId)
        {
            return new StudentMetricLearningStandardMetaDataRequest { MetricVariantId = metricVariantId };
        }

    }

    // interface
    public interface IStudentMetricLearningStandardMetaDataService : IService<StudentMetricLearningStandardMetaDataRequest, IEnumerable<StudentMetricLearningStandardMetaDataModel>> { }

    // service
    public class StudentMetricLearningStandardMetaDataService : IStudentMetricLearningStandardMetaDataService
    {

        private IRepository<StudentMetricLearningStandardMetaData> repository;
        private readonly IMetricNodeResolver metricNodeResolver;
        private readonly IGradeLevelUtilitiesProvider gradeLevelUtilitiesProvider;

        public StudentMetricLearningStandardMetaDataService(IRepository<StudentMetricLearningStandardMetaData> repository, 
                                                            IMetricNodeResolver metricNodeResolver,
                                                            IGradeLevelUtilitiesProvider gradeLevelUtilitiesProvider)
        {
            this.repository = repository;
            this.metricNodeResolver = metricNodeResolver;
            this.gradeLevelUtilitiesProvider = gradeLevelUtilitiesProvider;
        }

        [CanBeAuthorizedBy(EdFiClaimTypes.ViewAllStudents, EdFiClaimTypes.ViewMyStudents)]
        public IEnumerable<StudentMetricLearningStandardMetaDataModel> Get(StudentMetricLearningStandardMetaDataRequest request)
        {

            int metricId = metricNodeResolver.ResolveMetricId(request.MetricVariantId);

            // get objective meta data
            var records = (from data in repository.GetAll()
                           where data.MetricId == metricId
                           select data
                           ).ToList();

            // group by objective
            var objectives = (from record in records
                              group record by record.LearningObjective
                                  into groupObjective
                                  select new
                                  {
                                      Objective = groupObjective.Key,
                                      Grades = (from record in groupObjective
                                                group record by record.GradeLevel
                                                    into groupGrades
                                                    select new
                                                    {
                                                        Grade = groupGrades.Key,
                                                        Standards = groupGrades
                                                    }).ToList()
                                  }).ToList();

            // populate model
            var model = (from objective in objectives
                         select new StudentMetricLearningStandardMetaDataModel(metricId)
                         {
                             LearningObjective = objective.Objective,
                             Grades = objective.Grades.Select(g => new StudentMetricLearningStandardMetaDataModel.GradeModel()
                             {
                                 GradeLevel = g.Grade,
                                 GradeSort = gradeLevelUtilitiesProvider.FormatGradeLevelForSorting(g.Grade),
                                 Standards = g.Standards.Select(s => new StudentMetricLearningStandardMetaDataModel.StandardModel()
                                 {
                                     GradeLevel = s.GradeLevel,
                                     Description = s.Description,
                                     LearningStandard = s.LearningStandard
                                 }).ToList()
                             }).ToList()
                         }).ToList();

            // return list
            return model;

        }

    }

}
