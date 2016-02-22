using System;
using System.Collections.Generic;
using System.Linq;
using EdFi.Dashboards.Common;
using EdFi.Dashboards.Common.Utilities;
using EdFi.Dashboards.Core;
using EdFi.Dashboards.Data.Entities;
using EdFi.Dashboards.Data.Repository;
using EdFi.Dashboards.Metric.Resources.Models;
using EdFi.Dashboards.Metric.Resources.Providers;
using EdFi.Dashboards.Metric.Resources.Services;
using EdFi.Dashboards.Resource.Models.Common;
using EdFi.Dashboards.Resources.LocalEducationAgency;
using EdFi.Dashboards.Resources.Metric;
using EdFi.Dashboards.Resources.Models.Common;
using EdFi.Dashboards.Resources.Models.CustomGrid;
using EdFi.Dashboards.Resources.Models.School.Detail;
using EdFi.Dashboards.Resources.Models.Student;
using EdFi.Dashboards.Resources.Navigation;
using EdFi.Dashboards.Resources.Security.Common;
using EdFi.Dashboards.Resources.Staff;
using EdFi.Dashboards.Resources.StudentMetrics;
using EdFi.Dashboards.Resources.StudentSchool;
using EdFi.Dashboards.SecurityTokenService.Authentication;

namespace EdFi.Dashboards.Resources.School.Detail
{
    public class StudentMetricListRequest
    {
        public int SchoolId { get; set; }
        [AuthenticationIgnore("Server side paging metadata")]
        public long StaffUSI { get; set; }
        public int MetricVariantId { get; set; }
        [AuthenticationIgnore("Server side paging metadata")]
        public int PageNumber { get; set; }
        [AuthenticationIgnore("Server side paging metadata")]
        public int PageSize { get; set; }
        [AuthenticationIgnore("Server side paging metadata")]
        public int? SortColumn { get; set; }
        [AuthenticationIgnore("Server side paging metadata")]
        public string SortDirection { get; set; }
        [AuthenticationIgnore("Server side paging metadata")]
        public string VisibleColumns { get; set; }
        [AuthenticationIgnore("Server side paging metadata")]
        public string UniqueListId { get; set; }
        [AuthenticationIgnore("This is used by the paging provider to push data from the controller into the service.  It's always overwritten by the controller.")]
        public List<long> StudentIdList { get; set; } 

        /// <summary>
        /// Initializes a new instance of the <see cref="StudentMetricListRequest"/> using the specified parameters.
        /// </summary>
        /// <returns>A new <see cref="StudentMetricListRequest"/> instance.</returns>
        public static StudentMetricListRequest Create(int schoolId, long staffUSI, int metricVariantId, int pageNumber, int pageSize, int? sortColumn, string sortDirection, string visibleColumns, string uniqueListId)
        {
            return new StudentMetricListRequest
                       {
                           SchoolId = schoolId,
                           StaffUSI = staffUSI,
                           MetricVariantId = metricVariantId,
                           PageNumber = pageNumber,
                           PageSize = pageSize,
                           SortColumn = sortColumn,
                           SortDirection = sortDirection,
                           VisibleColumns = visibleColumns,
                           UniqueListId = uniqueListId
                       };
        }
    }

    public interface IStudentMetricListService : IService<StudentMetricListRequest, StudentMetricListModel> { }

    public class StudentMetricListService : IStudentMetricListService
    {
        private const string StudentLink = "student";
        private const string MetricContext = "metricContext";
        private const string Metric = "metric";
        private readonly IStudentSchoolAreaLinks _studentSchoolLinks;
        private readonly IMetricCorrelationProvider _metricCorrelationProvider;
        private readonly ISchoolCategoryProvider _schoolCategoryProvider;
        private readonly IListMetadataProvider _listMetadataProvider;
        private readonly IMetadataListIdResolver _metadataListIdResolver;
        private readonly IMetricNodeResolver _metricNodeResolver;
        private readonly IStudentMetricsProvider _studentMetricProvider;
        private readonly IGradeLevelUtilitiesProvider _gradeLevelUtilitiesProvider;
        private readonly IClassroomMetricsProvider _classroomMetricsProvider;
        private readonly IRepository<SchoolMetricStudentList> _schoolMetricStudentListRepository; 

        public StudentMetricListService(IStudentSchoolAreaLinks studentSchoolLinks,
                                        IMetricCorrelationProvider metricCorrelationProvider,
                                        ISchoolCategoryProvider schoolCategoryProvider,
                                        IListMetadataProvider listMetadataProvider,
                                        IMetadataListIdResolver metadataListIdResolver,
                                        IMetricNodeResolver metricNodeResolver,
                                        IStudentMetricsProvider studentListProvider, 
                                        IClassroomMetricsProvider classroomMetricsProvider,
                                        IGradeLevelUtilitiesProvider gradeLevelUtilitiesProvider,
                                        IRepository<SchoolMetricStudentList> schoolMetricStudentListRepository)
        {
            _studentSchoolLinks = studentSchoolLinks;
            _metricCorrelationProvider = metricCorrelationProvider;
            _schoolCategoryProvider = schoolCategoryProvider;
            _schoolCategoryProvider = schoolCategoryProvider;
            _listMetadataProvider = listMetadataProvider;
            _metadataListIdResolver = metadataListIdResolver;
            _metricNodeResolver = metricNodeResolver;
            _studentMetricProvider = studentListProvider;
            _gradeLevelUtilitiesProvider = gradeLevelUtilitiesProvider;
            _schoolMetricStudentListRepository = schoolMetricStudentListRepository;
            _classroomMetricsProvider = classroomMetricsProvider;
        }

        [CanBeAuthorizedBy(EdFiClaimTypes.ViewAllStudents, EdFiClaimTypes.ViewMyStudents)]
        public StudentMetricListModel Get(StudentMetricListRequest request)
        {
            var metricMetadataNode = _metricNodeResolver.GetMetricNodeForSchoolFromMetricVariantId(request.SchoolId,
                                                                                                   request
                                                                                                       .MetricVariantId);
            var correlatedStudentMetric =
                _metricCorrelationProvider.GetRenderingParentMetricVariantIdForStudent(request.MetricVariantId,
                                                                                       request.SchoolId);

            if (String.IsNullOrEmpty(metricMetadataNode.ListFormat))
                throw new ArgumentNullException(string.Format("List format is null for metric variant Id:{0}", request.MetricVariantId));

            var model = new StudentMetricListModel();
            var schoolCategory = _schoolCategoryProvider.GetSchoolCategoryType(request.SchoolId);

            //We default to SchoolCategory.HighSchool if its not a Elementary or a MiddleSchool.
            if (schoolCategory != SchoolCategory.Elementary && schoolCategory != SchoolCategory.MiddleSchool)
                schoolCategory = SchoolCategory.HighSchool;

            //Setting the metadata.
            var resolvedListId = _metadataListIdResolver.GetListId(ListType.StudentDrilldown, schoolCategory);
            model.ListMetadata = _listMetadataProvider.GetListMetadata(resolvedListId);

            var colInfo = model.ListMetadata[0].Columns.FirstOrDefault(x => x.ColumnName == "Metric Value" || x.MetricListCellType == MetricListCellType.MetricValue);
            if (colInfo != null)
            {
                colInfo.ColumnName = metricMetadataNode.ListDataLabel;
                colInfo.SortAscending = colInfo.SortAscending ?? "MetricValueSort";
                colInfo.SortDescending = colInfo.SortDescending ?? "MetricValueSort";
            }

            var queryOptions = new StudentMetricsProviderQueryOptions
                {
                    SchoolMetricStudentListMetricId = metricMetadataNode.MetricId,
                    MetricVariantIds = model.ListMetadata.GetMetricVariantIds(),
                    SchoolId = request.SchoolId,
                };

            var students = GetStudentIdList(request, queryOptions, model).ToList();

            queryOptions.StudentIds = students.Select(s => s.StudentUSI);

            var metrics = _studentMetricProvider.GetStudentsWithMetrics(queryOptions).ToList();

            var studentIds = students.Select(s => s.StudentUSI).ToArray();

            var schoolMetricStudents = new List<SchoolMetricStudentList>();

            //We have to check to make sure studentIds is not empty, because Subsonic will generate invalid sql if studentIds is empty.
            if(studentIds.Any())
            {
                schoolMetricStudents = _schoolMetricStudentListRepository.GetAll().Where(student =>
                                                      student.MetricId == metricMetadataNode.MetricId &&
                                                      student.SchoolId == request.SchoolId &&
                                                      studentIds.Contains(student.StudentUSI))
                                                      .ToList();
            }

            foreach (var student in students)
            {
                var studentMetrics = metrics.Where(metric => metric.StudentUSI == student.StudentUSI).ToList();
                int schoolId = student.SchoolId;
                long studentUsi = student.StudentUSI;
                string fullName = student.FullName;

                var studentModel = new StudentWithMetricsAndPrimaryMetric(student.StudentUSI)
                    {
                        SchoolId = student.SchoolId,
                        Name = Utilities.FormatPersonNameByLastName(student.FirstName, student.MiddleName, student.LastSurname),
                        GradeLevel = _gradeLevelUtilitiesProvider.FormatGradeLevelForSorting(student.GradeLevel),
                        GradeLevelDisplayValue = _gradeLevelUtilitiesProvider.FormatGradeLevelForDisplay(student.GradeLevel),
                        Href =
                            student.SchoolId != 0
                                ? new Link
                                    {
                                        Rel = StudentLink,
                                        Href = _studentSchoolLinks.Overview(schoolId, studentUsi, fullName).Resolve()
                                    }
                                : null,
                        Metrics = _classroomMetricsProvider.GetAdditionalMetrics(studentMetrics, model.ListMetadata)
                    };

                var schoolMetricStudent = schoolMetricStudents.FirstOrDefault(sms => sms.StudentUSI == student.StudentUSI);
                if (schoolMetricStudent != null)
                {
                    studentModel.PrimaryMetricValue = InstantiateValue.FromValueType(schoolMetricStudent.Value,
                                                                                     schoolMetricStudent.ValueType);
                    studentModel.PrimaryMetricDisplayValue = String.Format(metricMetadataNode.ListFormat,
                                                                           InstantiateValue.FromValueType(
                                                                               schoolMetricStudent.Value,
                                                                               schoolMetricStudent.ValueType) ??
                                                                           String.Empty);
                }

                if (schoolId != 0)
                {
                    if (correlatedStudentMetric.ContextMetricVariantId != null)
                        studentModel.Links.Add(new Link
                            {
                                Rel = MetricContext,
                                Href =
                                    _studentSchoolLinks.Metrics(schoolId, studentUsi,
                                                                correlatedStudentMetric.ContextMetricVariantId.Value,
                                                                fullName, new {listContext = request.UniqueListId})
                                                       .Resolve()
                                                       .MetricAnchor(correlatedStudentMetric.MetricVariantId)
                            });
                    if (correlatedStudentMetric.MetricVariantId != null)
                        studentModel.Links.Add(new Link
                            {
                                Rel = Metric,
                                Href =
                                    _studentSchoolLinks.Metrics(schoolId, studentUsi,
                                                                correlatedStudentMetric.MetricVariantId.Value, fullName)
                                                       .Resolve()
                            });
                }
                model.Students.Add(studentModel);
            }

            return model;
        }

        private IEnumerable<EnhancedStudentInformation> GetStudentIdList(StudentMetricListRequest request, StudentMetricsProviderQueryOptions providerQueryOptions, StudentMetricListModel model)
        {
            var sortColumn = model.ListMetadata.GetSortColumn(request.SortColumn);
            var students = _studentMetricProvider.GetOrderedStudentList(providerQueryOptions, sortColumn, request.SortDirection, request.MetricVariantId).ToList();
            
            model.EntityIds = students.Select(x => new[] { x.StudentUSI, x.SchoolId }).ToList(); // TODO: strange that this is not using a StudentSchoolIdentifier like other view models [WD]

            if (!model.EntityIds.Any())
                return new List<EnhancedStudentInformation>();

            // If Page Size > 100 then the user requested all records
            return request.PageSize > 100
                ? students
                : students.Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize);
        }
    }
}
