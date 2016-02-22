// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using EdFi.Dashboards.Common;
using EdFi.Dashboards.Data.Entities;
using EdFi.Dashboards.Data.Repository;
using EdFi.Dashboards.Metric.Resources.Providers;
using EdFi.Dashboards.Resources.Models.Student.Detail;
using EdFi.Dashboards.Resources.Security.Common;
using EdFi.Dashboards.SecurityTokenService.Authentication;

namespace EdFi.Dashboards.Resources.StudentSchool.Detail
{
    public class LearningStandardRequest
    {
        public long StudentUSI { get; set; }
        public int SchoolId { get; set; }
        public int MetricVariantId { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="LearningStandardRequest"/> using the specified parameters.
        /// </summary>
        /// <returns>A new <see cref="LearningStandardRequest"/> instance.</returns>
        public static LearningStandardRequest Create(long studentUSI, int schoolId, int metricVariantId) 
		{
            return new LearningStandardRequest { StudentUSI = studentUSI, SchoolId = schoolId, MetricVariantId = metricVariantId };
		}
	}

    public interface ILearningStandardService : IService<LearningStandardRequest, IList<LearningStandardModel>> {}

    public class LearningStandardService : ILearningStandardService
    {
        private readonly IRepository<StudentMetricLearningStandard> repository;
        private readonly IMetricNodeResolver metricNodeResolver;

        public LearningStandardService(IRepository<StudentMetricLearningStandard> repository, IMetricNodeResolver metricNodeResolver)
        {
            this.repository = repository;
            this.metricNodeResolver = metricNodeResolver;
        }

        [CanBeAuthorizedBy(EdFiClaimTypes.ViewAllStudents, EdFiClaimTypes.ViewMyStudents)]
        public IList<LearningStandardModel> Get(LearningStandardRequest request)
        {
            var studentUSI = request.StudentUSI;
            int schoolId = request.SchoolId;
            int metricVariantId = request.MetricVariantId;
            int metricId = metricNodeResolver.ResolveMetricId(metricVariantId);

            var results = (from data in repository.GetAll()
                           where data.StudentUSI == studentUSI 
                                    && data.SchoolId == schoolId 
                                    && data.MetricId == metricId
                           select data).ToList();

            var testAdministrations = (from data in results
                                       group data by new {data.DateAdministration, data.Version, data.AssessmentTitle}
                                       into g
                                       select new {g.Key.DateAdministration, g.Key.Version, g.Key.AssessmentTitle}).ToList();

            var groupedResults = (from data in results
                                  orderby data.LearningStandard , data.Description
                                  group data by new {data.LearningStandard, data.Description}
                                  into ls
                                  select new {ls.Key, Standard = ls}).ToList();

            var learningStandards = groupedResults.Select(x => new LearningStandardModel(studentUSI)
                                                                   {
                                                                       MetricId = metricId,
                                                                       SchoolId = schoolId,
                                                                       LearningStandard = x.Key.LearningStandard,
                                                                       Description = x.Key.Description,
                                                                       Assessments =
                                                                           x.Standard.Select(
                                                                               y =>
                                                                               new LearningStandardModel.Assessment(
                                                                                   studentUSI)
                                                                                   {
                                                                                       DateAdministration =
                                                                                           y.DateAdministration,
                                                                                       MetricStateTypeId =
                                                                                           y.MetricStateTypeId,
                                                                                       Value = y.Value,
                                                                                       AssessmentTitle =
                                                                                           y.AssessmentTitle,
                                                                                       Version = y.Version,
                                                                                       Administered = true
                                                                                   }).ToList()
                                                                   }).ToList();

            // add place holders for missing dates
            foreach (var learningStandard in learningStandards)
            {
                foreach (var testAdministration in testAdministrations)
                {
                    var assessment =
                        learningStandard.Assessments.SingleOrDefault(
                            x =>
                            x.DateAdministration == testAdministration.DateAdministration &&
                            x.AssessmentTitle == testAdministration.AssessmentTitle);
                    if (assessment == null)
                        learningStandard.Assessments.Add(new LearningStandardModel.Assessment(studentUSI)
                                                             {
                                                                 DateAdministration =
                                                                     testAdministration.DateAdministration,
                                                                 AssessmentTitle = testAdministration.AssessmentTitle,
                                                                 Version = testAdministration.Version
                                                             });
                }
                learningStandard.Assessments.Sort(SortLearningStandardAssessments);
            }

            return learningStandards;
        }

        private int SortLearningStandardAssessments(LearningStandardModel.Assessment a1,
                                                    LearningStandardModel.Assessment a2)
        {
            var dateCompare = DateTime.Compare(a1.DateAdministration, a2.DateAdministration);
            if (dateCompare != 0)
                return dateCompare;

            return String.Compare(a1.AssessmentTitle, a2.AssessmentTitle, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
