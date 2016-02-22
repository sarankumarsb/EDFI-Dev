using System;
using System.Linq;
using EdFi.Dashboards.Common;
using EdFi.Dashboards.Core;
using EdFi.Dashboards.Data.Repository;
using EdFi.Dashboards.Metric.Data.Entities;
using EdFi.Dashboards.Metric.Resources.Models;
using EdFi.Dashboards.Metric.Resources.Providers;
using EdFi.Dashboards.Resources.Metric.Requests;
using EdFi.Dashboards.Resources.Models.CustomGrid;
using EdFi.Dashboards.Resources.Models.School.Detail;
using EdFi.Dashboards.Resources.Security.Common;
using EdFi.Dashboards.Resources.Staff;
using EdFi.Dashboards.SecurityTokenService.Authentication;

namespace EdFi.Dashboards.Resources.School.Detail
{
    public class StudentMetricListMetaRequest
    {
        public int SchoolId { get; set; }
        public int MetricVariantId { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="StudentMetricListRequest"/> using the specified parameters.
        /// </summary>
        /// <returns>A new <see cref="StudentMetricListRequest"/> instance.</returns>
        public static StudentMetricListMetaRequest Create(int schoolId, int metricVariantId)
        {
            return new StudentMetricListMetaRequest { SchoolId = schoolId, MetricVariantId = metricVariantId };
        }
    }

    public interface IStudentMetricListMetaService : IService<StudentMetricListMetaRequest, StudentMetricListMetaModel> { }

    public class StudentMetricListMetaService : IStudentMetricListMetaService
    {
        private readonly IRepository<MetricInstanceFootnote> footnoteRepository;
        private readonly ISchoolCategoryProvider schoolCategoryProvider;
        private readonly IMetricInstanceSetKeyResolver<SchoolMetricInstanceSetRequest> metricInstanceSetKeyResolver;
        private readonly IListMetadataProvider listMetadataProvider;
        private readonly IMetadataListIdResolver metadataListIdResolver;
        private readonly IMetricNodeResolver metricNodeResolver;

        public StudentMetricListMetaService(IRepository<MetricInstanceFootnote> footnoteRepository,
                                            ISchoolCategoryProvider schoolCategoryProvider,
                                            IMetricInstanceSetKeyResolver<SchoolMetricInstanceSetRequest> metricInstanceSetKeyResolver,
                                            IListMetadataProvider listMetadataProvider,
                                            IMetadataListIdResolver metadataListIdResolver,
                                            IMetricNodeResolver metricNodeResolver)
        {
            this.footnoteRepository = footnoteRepository;
            this.schoolCategoryProvider = schoolCategoryProvider;
            this.metricInstanceSetKeyResolver = metricInstanceSetKeyResolver;
            this.listMetadataProvider = listMetadataProvider;
            this.metadataListIdResolver = metadataListIdResolver;
            this.metricNodeResolver = metricNodeResolver;
        }

        [CanBeAuthorizedBy(EdFiClaimTypes.ViewAllStudents, EdFiClaimTypes.ViewMyStudents)]
        public StudentMetricListMetaModel Get(StudentMetricListMetaRequest request)
        {
            var schoolId = request.SchoolId;
            var metricVariantId = request.MetricVariantId;
            var metricMetadataNode = metricNodeResolver.GetMetricNodeForSchoolFromMetricVariantId(schoolId, metricVariantId);
            var metricId = metricMetadataNode.MetricId;

            if (String.IsNullOrEmpty(metricMetadataNode.ListFormat))
                throw new ArgumentNullException(string.Format("List format is null for metric variant Id:{0}", metricVariantId));

            var model = new StudentMetricListMetaModel();

            var metricInstanceSetKey = metricInstanceSetKeyResolver.GetMetricInstanceSetKey(SchoolMetricInstanceSetRequest.Create(schoolId, metricVariantId));
            var footnotes = (from data in footnoteRepository.GetAll()
                             where data.MetricInstanceSetKey == metricInstanceSetKey
                                     && data.MetricId == metricId
                                     && data.FootnoteTypeId == (int)MetricFootnoteType.DrillDownFootnote
                             select data).ToList();

            model.MetricFootnotes = footnotes.Select(x => new MetricFootnote
            {
                FootnoteTypeId = (MetricFootnoteType)x.FootnoteTypeId,
                FootnoteText = x.FootnoteText
            }).ToList();

            var schoolCategory = schoolCategoryProvider.GetSchoolCategoryType(schoolId);

            //We default to SchoolCategory.HighSchool if its not a Elementary or a MiddleSchool.
            if (schoolCategory != SchoolCategory.Elementary && schoolCategory != SchoolCategory.MiddleSchool)
                schoolCategory = SchoolCategory.HighSchool;

            //Setting the metadata.
            var resolvedListId = metadataListIdResolver.GetListId(ListType.StudentDrilldown, schoolCategory);
            model.ListMetadata = listMetadataProvider.GetListMetadata(resolvedListId);

            var colInfo = model.ListMetadata[0].Columns.FirstOrDefault(x => x.ColumnName == "Metric Value" || x.MetricListCellType == MetricListCellType.MetricValue);
            if (colInfo != null)
                colInfo.ColumnName = metricMetadataNode.ListDataLabel;

            return model;
        }
    }
}
