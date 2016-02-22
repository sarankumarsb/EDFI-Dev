// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
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
    public class MetricObjectivesListRequest
    {
        public long StudentUSI { get; set; }
        public int SchoolId { get; set; }
        public int MetricVariantId { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MetricObjectivesListRequest"/> using the specified parameters.
        /// </summary>
        /// <returns>A new <see cref="MetricObjectivesListRequest"/> instance.</returns>
        public static MetricObjectivesListRequest Create(long studentUSI, int schoolId, int metricVariantId)
        {
            return new MetricObjectivesListRequest { StudentUSI = studentUSI, SchoolId = schoolId, MetricVariantId = metricVariantId };
        }
    }

    public interface IMetricObjectivesListService : IService<MetricObjectivesListRequest, IList<MetricObjectiveModel>> { }

    public class MetricObjectivesListService : IMetricObjectivesListService
    {
        private readonly IRepository<StudentMetricObjective> repository;
        private readonly IMetricNodeResolver metricNodeResolver;

        public MetricObjectivesListService(IRepository<StudentMetricObjective> repository, IMetricNodeResolver metricNodeResolver)
        {
            this.repository = repository;
            this.metricNodeResolver = metricNodeResolver;
        }

        [CanBeAuthorizedBy(EdFiClaimTypes.ViewAllStudents, EdFiClaimTypes.ViewMyStudents)]
        public IList<MetricObjectiveModel> Get(MetricObjectivesListRequest request)
        {
            var studentUSI = request.StudentUSI;
            int schoolId = request.SchoolId;
            int metricId = metricNodeResolver.ResolveMetricId(request.MetricVariantId);

            var results = (from data in repository.GetAll()
                           where data.StudentUSI == studentUSI 
                                    && data.SchoolId == schoolId 
                                    && data.MetricId == metricId
                           orderby data.ObjectiveItem, data.Description
                           select new
                           {
                               data.Description,
                               data.Value,
                               data.MetricStateTypeId,
                               data.Flag
                           }).ToList();

            return results.Select(x => new MetricObjectiveModel(studentUSI)
            {
                Description = x.Description,
                Value = x.Value,
                State = GetMetricState(x.MetricStateTypeId),
                IsFlagged = x.Flag ?? false
            }).ToList();
        }

        private static MetricState GetMetricState(int? metricStateTypeId)
        {
            var res = new MetricState();
            res.StateType = (metricStateTypeId.HasValue) ? (MetricStateType)metricStateTypeId : MetricStateType.NoData;
            return res;
        }
    }
}
