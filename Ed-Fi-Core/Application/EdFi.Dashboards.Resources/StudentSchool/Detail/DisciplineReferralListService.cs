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
    public class DisciplineReferralListRequest
    {
        public long StudentUSI { get; set; }
        public int SchoolId { get; set; }
        public int MetricVariantId { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DisciplineReferralListRequest"/> using the specified parameters.
        /// </summary>
        /// <returns>A new <see cref="DisciplineReferralListRequest"/> instance.</returns>
        public static DisciplineReferralListRequest Create(long studentUSI, int schoolId, int metricVariantId) 
		{
            return new DisciplineReferralListRequest { StudentUSI = studentUSI, SchoolId = schoolId, MetricVariantId = metricVariantId };
		}
	}

    public interface IDisciplineReferralListService : IService<DisciplineReferralListRequest, IList<DisciplineReferralModel>> {}

    public class DisciplineReferralListService : IDisciplineReferralListService
    {
        private readonly IRepository<StudentMetricDisciplineReferral> repository;
        private readonly IMetricNodeResolver metricNodeResolver;

        public DisciplineReferralListService(IRepository<StudentMetricDisciplineReferral> repository, IMetricNodeResolver metricNodeResolver)
        {
            this.repository = repository;
            this.metricNodeResolver = metricNodeResolver;
        }

        [CanBeAuthorizedBy(EdFiClaimTypes.ViewAllStudents, EdFiClaimTypes.ViewMyStudents)]
        public virtual IList<DisciplineReferralModel> Get(DisciplineReferralListRequest request)
        {
            var studentUSI = request.StudentUSI;
            int schoolId = request.SchoolId;
            int metricId = metricNodeResolver.ResolveMetricId(request.MetricVariantId);

            var results = (from data in repository.GetAll()
                           where data.StudentUSI == studentUSI 
                                    && data.SchoolId == schoolId 
                                    && data.MetricId == metricId
                           orderby data.Date
                           select new
                                      {
                                          data.Date,
                                          data.IncidentCode,
                                          data.IncidentDescription,
                                          data.Action
                                      }).ToList();

            // this has to be split out b/c Subsonic doesn't support constructors with parameters
            return results.Select(x => new DisciplineReferralModel(studentUSI)
                                           {
                                               Date = x.Date,
                                               IncidentCode = x.IncidentCode,
                                               IncidentDescription = x.IncidentDescription,
                                               Action = x.Action
                                           }).ToList();
        }
    }
}
