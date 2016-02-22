using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EdFi.Dashboards.Common;
using EdFi.Dashboards.Core;
using EdFi.Dashboards.Data.Entities;
using EdFi.Dashboards.Data.Repository;
using EdFi.Dashboards.Metric.Resources.Providers;
using EdFi.Dashboards.Resources.Models.Student.Detail;
using EdFi.Dashboards.Resources.Security.Common;
using EdFi.Dashboards.SecurityTokenService.Authentication;

namespace EdFi.Dashboards.Resources.StudentSchool.Detail
{
    public class LearningObjectiveRequest
    {
        public long StudentUSI { get; set; }
        public int SchoolId { get; set; }
        public int MetricVariantId { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="LearningObjectiveRequest"/> using the specified parameters.
        /// </summary>
        /// <returns>A new <see cref="LearningObjectiveRequest"/> instance.</returns>
        public static LearningObjectiveRequest Create(long studentUSI, int schoolId, int metricVariantId)
        {
            return new LearningObjectiveRequest { StudentUSI = studentUSI, SchoolId = schoolId, MetricVariantId = metricVariantId };
        }
    }

    public interface ILearningObjectiveService : IService<LearningObjectiveRequest, LearningObjectiveModel> { }

    public class LearningObjectiveService : ILearningObjectiveService
    {
        private readonly IRepository<StudentMetricLearningObjective> repository;
        private readonly IMetricNodeResolver metricNodeResolver;

        public LearningObjectiveService(IRepository<StudentMetricLearningObjective> repository, IMetricNodeResolver metricNodeResolver)
        {
            this.repository = repository;
            this.metricNodeResolver = metricNodeResolver;
        }

        [CanBeAuthorizedBy(EdFiClaimTypes.ViewAllStudents, EdFiClaimTypes.ViewMyStudents)]
        public LearningObjectiveModel Get(LearningObjectiveRequest request)
        {
            var studentUSI = request.StudentUSI;
            int schoolId = request.SchoolId;
            int metricVariantId = request.MetricVariantId;
            var metricMetatdataNode = metricNodeResolver.GetMetricNodeForStudentFromMetricVariantId(schoolId, metricVariantId);
            int metricId = metricMetatdataNode.MetricId;

            var model = new LearningObjectiveModel(studentUSI);
            model.MetricId = metricId;
            model.InventoryName = metricMetatdataNode.DisplayName;

            var results = (from data in repository.GetAll()
                           where data.StudentUSI == studentUSI
                                    && data.SchoolId == schoolId
                                    && data.MetricId == metricId
                           select data).ToList();

            if (results.Count == 0)
                return model;

            //get all the horizontal titles that we need
            model.AssessmentTitles = GetDistinctAssessmentTitles(results);

            //Get the grouped results per each metric id
            //  Group by LearningObjective (Parent) - ObjectiveName (Child)
            var groupedResults = (from data in results
                                    orderby data.LearningObjective, data.ObjectiveName
                                    group data by new { data.LearningObjective, data.ObjectiveName }
                                        into lo
                                        select new { lo.Key, SkillValues = lo }).ToList();

            //for each group (aka => Learning Objective), create the section (Learning Objective) and skill (Objective name with results)
            model.LearningObjectiveSkills = groupedResults.Select(learningObjective => new LearningObjectiveModel.LearningObjectiveSkill(studentUSI)
                                                                                    {
                                                                                        SectionName = learningObjective.Key.LearningObjective,
                                                                                        SkillName = learningObjective.Key.ObjectiveName,
                                                                                        SkillValues = learningObjective.SkillValues.Select(skillValue => new LearningObjectiveModel.SkillValue(studentUSI)
                                                                                                                                                             {
                                                                                                                                                                 MetricStateTypeId = skillValue.MetricStateTypeId,
                                                                                                                                                                 Value = skillValue.Value,
                                                                                                                                                                 Title = skillValue.AssessmentTitle
                                                                                                                                                             }).ToList()
                                                                                    }).ToList();

            return model;
        }

        #region private methods
        private static List<string> GetDistinctAssessmentTitles(IEnumerable<StudentMetricLearningObjective> data)
        {
            var tempList = new List<string>();

            foreach (var studentMetricLearningObjective in data)
            {
                if (!tempList.Contains(studentMetricLearningObjective.AssessmentTitle))
                    tempList.Add(studentMetricLearningObjective.AssessmentTitle);
            }

            return tempList;
        }
        #endregion
    }
}
