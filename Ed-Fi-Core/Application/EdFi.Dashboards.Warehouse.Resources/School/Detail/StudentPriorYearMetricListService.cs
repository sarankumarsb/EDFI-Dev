using System;
using System.Linq;
using EdFi.Dashboards.Common;
using EdFi.Dashboards.Common.Utilities;
using EdFi.Dashboards.Core;
using EdFi.Dashboards.Metric.Resources.Models;
using EdFi.Dashboards.Metric.Resources.Providers;
using EdFi.Dashboards.Resource.Models.Common;
using EdFi.Dashboards.Resources;
using EdFi.Dashboards.Resources.Common;
using EdFi.Dashboards.Resources.Metric;
using EdFi.Dashboards.Resources.Models.CustomGrid;
using EdFi.Dashboards.Resources.Models.Student;
using EdFi.Dashboards.Resources.Navigation;
using EdFi.Dashboards.Resources.School;
using EdFi.Dashboards.Resources.Security.Common;
using EdFi.Dashboards.Resources.Staff;
using EdFi.Dashboards.Resources.StudentMetrics;
using EdFi.Dashboards.Resources.StudentSchool;
using EdFi.Dashboards.SecurityTokenService.Authentication;
using EdFi.Dashboards.Warehouse.Resource.Models.School.Detail;
using EdFi.Dashboards.Warehouse.Resources.Application;
using EdFi.Dashboards.Warehouse.Resources.Staff;

namespace EdFi.Dashboards.Warehouse.Resources.School.Detail
{
    public class StudentPriorYearMetricListRequest
    {
        public int SchoolId { get; set; }
        [AuthenticationIgnore("LocalEducationAgencyId is implied by SchoolId, and does not need to be independently authorized.")]
        public int LocalEducationAgencyId { get; set; }
        public int MetricVariantId { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="StudentPriorYearMetricListRequest"/> using the specified parameters.
        /// </summary>
        /// <returns>A new <see cref="StudentPriorYearMetricListRequest"/> instance.</returns>
        public static StudentPriorYearMetricListRequest Create(int schoolId, int localEducationAgencyId, int metricVariantId)
        {
            return new StudentPriorYearMetricListRequest { SchoolId = schoolId, LocalEducationAgencyId = localEducationAgencyId, MetricVariantId = metricVariantId };
        }
    }

    public interface IStudentPriorYearMetricListService : IService<StudentPriorYearMetricListRequest, StudentPriorYearMetricListModel> { }

    public class StudentPriorYearMetricListService : IStudentPriorYearMetricListService
    {
        private const string NoLongerEnrolledFootnoteFormat = "{0} students excluded because they are no longer enrolled.";
        private const string StudentLink = "student";
        private const string MetricContext = "metricContext";
        private const string Metric = "metric";
        private readonly ISchoolCategoryProvider schoolCategoryProvider;
        private readonly IUniqueListIdProvider uniqueListProvider;
        private readonly IMetricCorrelationProvider metricCorrelationProvider;
        private readonly IStudentSchoolAreaLinks studentSchoolLinks;
        private readonly IPriorYearClassroomMetricsProvider priorYearClassroomMetricsProvider;
        private readonly IListMetadataProvider listMetadataProvider;
        private readonly IMetadataListIdResolver metadataListIdResolver;
        private readonly IMetricNodeResolver metricNodeResolver;
        private readonly IWarehouseAvailabilityProvider warehouseAvailabilityProvider;
        private readonly IMaxPriorYearProvider maxPriorYearProvider;
        private readonly IGradeLevelUtilitiesProvider gradeLevelUtilitiesProvider;
        private readonly IPriorYearStudentMetricsProvider priorYearStudentMetricsProvider;
        private readonly IStudentMetricsProvider studentMetricsProvider;

        public StudentPriorYearMetricListService(IUniqueListIdProvider uniqueListProvider,
                                                    IMetricCorrelationProvider metricCorrelationProvider,
                                                    ISchoolCategoryProvider schoolCategoryProvider,
                                                    IStudentSchoolAreaLinks studentSchoolLinks,
                                                    IPriorYearClassroomMetricsProvider priorYearClassroomMetricsProvider,
                                                    IListMetadataProvider listMetadataProvider,
                                                    IMetadataListIdResolver metadataListIdResolver,
                                                    IMetricNodeResolver metricNodeResolver,
                                                    IWarehouseAvailabilityProvider warehouseAvailabilityProvider,
                                                    IMaxPriorYearProvider maxPriorYearProvider,
                                                    IGradeLevelUtilitiesProvider gradeLevelUtilitiesProvider,
                                                    IPriorYearStudentMetricsProvider priorYearStudentMetricsProvider,
                                                    IStudentMetricsProvider studentMetricsProvider)
        {
            this.schoolCategoryProvider = schoolCategoryProvider;
            this.studentSchoolLinks = studentSchoolLinks;
            this.priorYearClassroomMetricsProvider = priorYearClassroomMetricsProvider;
            this.listMetadataProvider = listMetadataProvider;
            this.metadataListIdResolver = metadataListIdResolver;
            this.uniqueListProvider = uniqueListProvider;
            this.metricCorrelationProvider = metricCorrelationProvider;
            this.metricNodeResolver = metricNodeResolver;
            this.warehouseAvailabilityProvider = warehouseAvailabilityProvider;
            this.maxPriorYearProvider = maxPriorYearProvider;
            this.gradeLevelUtilitiesProvider = gradeLevelUtilitiesProvider;
            this.priorYearStudentMetricsProvider = priorYearStudentMetricsProvider;
            this.studentMetricsProvider = studentMetricsProvider;
        }

        [CanBeAuthorizedBy(EdFiClaimTypes.ViewAllStudents, EdFiClaimTypes.ViewMyStudents)]
        public StudentPriorYearMetricListModel Get(StudentPriorYearMetricListRequest request)
        {
            var model = new StudentPriorYearMetricListModel();
            if (!warehouseAvailabilityProvider.Get())
            {
                return model;
            }

            int schoolId = request.SchoolId;
            int localEducationAgencyId = request.LocalEducationAgencyId;
            int metricVariantId = request.MetricVariantId;
            var metricMetadataNode = metricNodeResolver.GetMetricNodeForSchoolFromMetricVariantId(schoolId, metricVariantId);
            int metricId = metricMetadataNode.MetricId;

            model.SchoolCategory = schoolCategoryProvider.GetSchoolCategoryType(schoolId);

            //We default to SchoolCategory.HighSchool if its not a Elementary or a MiddleSchool.
            if (model.SchoolCategory != SchoolCategory.Elementary && model.SchoolCategory != SchoolCategory.MiddleSchool)
                model.SchoolCategory = SchoolCategory.HighSchool;

            var year = maxPriorYearProvider.Get(localEducationAgencyId);

            //Setting the metadata.
            var resolvedListId = metadataListIdResolver.GetListId(ListType.PriorYearStudentDrilldown, model.SchoolCategory);
            model.ListMetadata = listMetadataProvider.GetListMetadata(resolvedListId);

            var colInfo = model.ListMetadata[0].Columns.FirstOrDefault(x => x.ColumnName == "Metric Value" || x.MetricListCellType == MetricListCellType.MetricValue);
            if (colInfo != null)
                colInfo.ColumnName = metricMetadataNode.ListDataLabel;

            var priorYearQueryOptions = new PriorYearStudentMetricsProviderQueryOptions
            {
                SchoolMetricStudentListMetricId = metricId,
                MetricVariantIds = model.ListMetadata.GetMetricVariantIds(),
                SchoolId = schoolId,
                Year = year
            };

            var priorYearStudents = priorYearStudentMetricsProvider.GetStudentList(priorYearQueryOptions);

            var priorYearStudentUSIs = Enumerable.ToArray(priorYearStudents.Select(x => x.StudentUSI));

            var queryOptions = new StudentMetricsProviderQueryOptions
            {
                SchoolId = schoolId,
                StudentIds = priorYearStudentUSIs,
                MetricVariantIds = model.ListMetadata.GetMetricVariantIds(),
            };

            var currentStudentList = studentMetricsProvider.GetOrderedStudentList(queryOptions);

            // calculate footnote
            if (priorYearStudents.Count() != currentStudentList.Count())
                model.MetricFootnotes.Add(new MetricFootnote
                {
                    FootnoteNumber = 0,
                    FootnoteText = String.Format(NoLongerEnrolledFootnoteFormat, priorYearStudents.Count() - currentStudentList.Count()),
                    FootnoteTypeId = MetricFootnoteType.DrillDownFootnote
                });

            var correlatedStudentMetric = metricCorrelationProvider.GetRenderingParentMetricVariantIdForStudent(metricVariantId, schoolId);
            var uniqueListId = uniqueListProvider.GetUniqueId(metricVariantId);
            model.UniqueListId = uniqueListId;

            priorYearQueryOptions.StudentIds = currentStudentList.Select(x => x.StudentUSI);

            var priorYearMetrics =
                priorYearStudentMetricsProvider.GetStudentsWithMetrics(priorYearQueryOptions);
            var metrics = studentMetricsProvider.GetStudentsWithMetrics(queryOptions);

            foreach (var student in currentStudentList)
            {
                var studentMetrics = metrics.Where(metric => metric.StudentUSI == student.StudentUSI).ToList();
                var priorYearStudentMetrics = priorYearMetrics.Where(metric => metric.StudentUSI == student.StudentUSI).ToList();
                var priorYearStudentData = priorYearStudents.SingleOrDefault(x => x.StudentUSI == student.StudentUSI);

                var studentModel = new StudentWithMetricsAndPrimaryMetric(student.StudentUSI)
                {
                    SchoolId = student.SchoolId,
                    Name = Utilities.FormatPersonNameByLastName(student.FirstName, student.MiddleName, student.LastSurname),
                    GradeLevel = gradeLevelUtilitiesProvider.FormatGradeLevelForSorting(student.GradeLevel),
                    GradeLevelDisplayValue = gradeLevelUtilitiesProvider.FormatGradeLevelForDisplay(student.GradeLevel),
                    Href = new Link { Rel = StudentLink, Href = studentSchoolLinks.Overview(student.SchoolId, student.StudentUSI, student.FullName).Resolve() },

                    PrimaryMetricValue = InstantiateValue.FromValueType(priorYearStudentData.Value, priorYearStudentData.ValueType),
                    PrimaryMetricDisplayValue = String.Format(metricMetadataNode.ListFormat, InstantiateValue.FromValueType(priorYearStudentData.Value, priorYearStudentData.ValueType) ?? ""),
                    Metrics = priorYearClassroomMetricsProvider.GetAdditionalMetrics(priorYearStudentMetrics, studentMetrics, model.ListMetadata)
                };

                if (correlatedStudentMetric.ContextMetricVariantId != null)
                    studentModel.Links.Add(new Link
                    {
                        Rel = MetricContext,
                        Href = studentSchoolLinks.Metrics(student.SchoolId, student.StudentUSI, correlatedStudentMetric.ContextMetricVariantId.Value, student.FullName, new { listContext = uniqueListId }).Resolve().MetricAnchor(correlatedStudentMetric.MetricVariantId)

                    });
                if (correlatedStudentMetric.MetricVariantId != null)
                    studentModel.Links.Add(new Link
                    {
                        Rel = Metric,
                        Href = studentSchoolLinks.Metrics(student.SchoolId, student.StudentUSI, correlatedStudentMetric.MetricVariantId.Value, student.FullName).Resolve()
                    });
                model.Students.Add(studentModel);
            }
            return model;
        }
    }
}
