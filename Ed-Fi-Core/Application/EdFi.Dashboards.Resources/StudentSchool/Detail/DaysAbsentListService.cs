using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EdFi.Dashboards.Common;
using EdFi.Dashboards.Data.Entities;
using EdFi.Dashboards.Data.Repository;
using EdFi.Dashboards.Metric.Resources.Providers;
using EdFi.Dashboards.Resources.Models.Student.Detail;
using EdFi.Dashboards.Resources.Security.Common;
using EdFi.Dashboards.SecurityTokenService.Authentication;

namespace EdFi.Dashboards.Resources.StudentSchool.Detail
{
    public class DaysAbsentListRequest
    {
        public long StudentUSI { get; set; }
        public int SchoolId { get; set; }
        public int MetricVariantId { get; set; }
        /// <summary>
        /// Initializes a new instance of the <see cref="DaysAbsentListRequest"/> using the specified parameters.
        /// </summary>
        /// <returns>A new <see cref="DailyAttendanceChartRequest"/> instance.</returns>
        public static DaysAbsentListRequest Create(long studentUSI, int schoolId, int metricVariantId)
        {
            return new DaysAbsentListRequest { StudentUSI = studentUSI, SchoolId = schoolId, MetricVariantId = metricVariantId };
        }
    }

    public interface IDaysAbsentListService : IService<DaysAbsentListRequest, IList<DaysAbsentModel>> { }

    public class DaysAbsentListService : IDaysAbsentListService
    {
        private readonly IRepository<StudentMetricAttendanceRate> repository;
        private readonly IMetricNodeResolver metricNodeResolver;

        public DaysAbsentListService(IRepository<StudentMetricAttendanceRate> repository, IMetricNodeResolver metricNodeResolver)
        {
            this.repository = repository;
            this.metricNodeResolver = metricNodeResolver;
        }

        [CanBeAuthorizedBy(EdFiClaimTypes.ViewAllStudents, EdFiClaimTypes.ViewMyStudents)]
        public IList<DaysAbsentModel> Get(DaysAbsentListRequest request)
        {
            var studentUSI = request.StudentUSI;
            int schoolId = request.SchoolId;
            int metricId = metricNodeResolver.ResolveMetricId(request.MetricVariantId);

            var query = from data in repository.GetAll()
                         where data.StudentUSI == studentUSI && data.SchoolId == schoolId && data.MetricId == metricId
                         orderby data.PeriodSequence
                         select data;

            var result = new List<DaysAbsentModel>();

            foreach (var period in query)
            {
                result.Add(new DaysAbsentModel(studentUSI)
                               {
                                   SchoolId = schoolId,
                                   Context = period.Context,
                                   TotalDays = period.TotalDays,
                                   AttendanceDays = period.AttendanceDays,
                                   ExcusedDays = period.ExcusedDays,
                                   UnexcusedDays = period.UnexcusedDays
                               });
            }
            return result;
        }
    }
}
