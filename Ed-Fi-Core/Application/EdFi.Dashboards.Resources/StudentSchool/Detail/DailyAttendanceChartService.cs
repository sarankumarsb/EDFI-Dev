// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using EdFi.Dashboards.Common;
using EdFi.Dashboards.Data.Entities;
using EdFi.Dashboards.Data.Repository;
using EdFi.Dashboards.Metric.Resources.Models;
using EdFi.Dashboards.Metric.Resources.Providers;
using EdFi.Dashboards.Resources.Models.Charting;
using EdFi.Dashboards.Resources.Security.Common;
using EdFi.Dashboards.SecurityTokenService.Authentication;

namespace EdFi.Dashboards.Resources.StudentSchool.Detail
{
    public class DailyAttendanceChartRequest
    {
        public long StudentUSI { get; set; }
        public int SchoolId { get; set; }
        public int MetricVariantId { get; set; }
        [AuthenticationIgnore("Title is not a relevant property outside of forming summaries")]
        public string Title { get; set; }
        /// <summary>
        /// Initializes a new instance of the <see cref="DailyAttendanceChartRequest"/> using the specified parameters.
        /// </summary>
        /// <returns>A new <see cref="DailyAttendanceChartRequest"/> instance.</returns>
        public static DailyAttendanceChartRequest Create(long studentUSI, int schoolId, int metricVariantId, string title) 
		{
            return new DailyAttendanceChartRequest { StudentUSI = studentUSI, SchoolId = schoolId, MetricVariantId = metricVariantId, Title = title };
		}
	}

    public interface IDailyAttendanceChartService : IService<DailyAttendanceChartRequest, ChartData> { }

    public class DailyAttendanceChartService : IDailyAttendanceChartService
    {
        protected virtual string InAttendanceSeriesName { get { return "In Attendance"; } }
        protected virtual string ExcusedAbsenceSeriesName { get { return "Excused Absence"; } }
        protected virtual string UnexcusedAbsenceSeriesName { get { return "Unexcused Absence"; } }
        protected virtual string InAttendanceTooltipFormat { get { return "Days In Attendance: {0} of {1}"; } }
        protected virtual string ExcusedAbsenceTooltipFormat { get { return "Excused Days: {0}"; } }
        protected virtual string UnexcusedAbsenceTooltipFormat { get { return "Unexcused Days: {0}"; } }
        protected virtual string LabelFormat { get { return "{0:P1}"; } }

        private readonly IRepository<StudentMetricAttendanceRate> repository;
		protected IRepository<StudentMetricAttendanceRate> Repository
		{
			get { return repository; }
		}

        private readonly IMetricNodeResolver metricNodeResolver;
		protected IMetricNodeResolver MetricNodeResolver
		{
			get { return metricNodeResolver; }
		}

        public DailyAttendanceChartService(IRepository<StudentMetricAttendanceRate> repository, IMetricNodeResolver metricNodeResolver)
        {
            this.repository = repository;
            this.metricNodeResolver = metricNodeResolver;
        }

        [CanBeAuthorizedBy(EdFiClaimTypes.ViewAllStudents, EdFiClaimTypes.ViewMyStudents)]
        public virtual ChartData Get(DailyAttendanceChartRequest request)
        {
            var studentUSI = request.StudentUSI;
            int schoolId = request.SchoolId;
            int metricVariantId = request.MetricVariantId;
            var metricMetadataNode = metricNodeResolver.GetMetricNodeForStudentFromMetricVariantId(schoolId,
                                                                                                   metricVariantId);
            int metricId = metricMetadataNode.MetricId;

            var result = from data in repository.GetAll()
                         where data.StudentUSI == studentUSI 
                                && data.SchoolId == schoolId 
                                && data.MetricId == metricId
                         orderby data.PeriodSequence
                         select data;

            var inAttendance = new ChartData.Series { Name = InAttendanceSeriesName, Style = ChartSeriesStyle.Green, ShowInLegend = true };
            var excusedAbsence = new ChartData.Series { Name = ExcusedAbsenceSeriesName, Style = ChartSeriesStyle.Blue, ShowInLegend = true };
            var unexcusedAbsence = new ChartData.Series { Name = UnexcusedAbsenceSeriesName, Style = ChartSeriesStyle.Red, ShowInLegend = true };

            foreach (var period in result)
            {
                inAttendance.Points.Add(GeneratePoint(period.Context, period.AttendanceRate, period.AttendanceDays, period.TotalDays, InAttendanceTooltipFormat));
                excusedAbsence.Points.Add(GeneratePoint(period.Context, period.ExcusedRate, period.ExcusedDays, period.TotalDays, ExcusedAbsenceTooltipFormat));
                unexcusedAbsence.Points.Add(GeneratePoint(period.Context, period.UnexcusedRate, period.UnexcusedDays, period.TotalDays, UnexcusedAbsenceTooltipFormat));
            }

            return new ChartData {SeriesCollection = new List<ChartData.Series> {inAttendance, excusedAbsence, unexcusedAbsence}, ChartTitle = metricMetadataNode.Actions.GetChartTitle(request.Title)};
        }

        protected ChartData.Point GeneratePoint(string context, decimal? rate, int? days, int? totalDays, string tooltipFormat)
        {
            var value = rate.HasValue ? Convert.ToDouble(rate.Value) : 0;
            var showValue = Math.Abs(value - 0) >= Double.Epsilon;
            return new ChartData.Point
                       {
                           Label = String.Format(LabelFormat, value),
                           AxisLabel = context,
                           Value = value,
                           Tooltip = showValue ? String.Format(tooltipFormat, days, totalDays) : String.Empty,
                           IsValueShownAsLabel = showValue,
                           State = MetricStateType.None,
                           Trend = TrendEvaluation.None
                       };
        }
    }
}
