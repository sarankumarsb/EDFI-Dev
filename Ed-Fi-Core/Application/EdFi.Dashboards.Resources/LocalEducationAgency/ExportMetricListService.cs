// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System.Collections.Generic;
using System.Linq;
using EdFi.Dashboards.Common;
using EdFi.Dashboards.Core;
using EdFi.Dashboards.Resources.LocalEducationAgency.Detail;
using EdFi.Dashboards.Resources.Models.Common;
using EdFi.Dashboards.Resources.Models.CustomGrid;
using EdFi.Dashboards.Resources.Models.LocalEducationAgency.Detail;
using EdFi.Dashboards.Resources.Security.Common;
using EdFi.Dashboards.Resources.Staff;
using EdFi.Dashboards.SecurityTokenService.Authentication;

namespace EdFi.Dashboards.Resources.LocalEducationAgency
{
    public class ExportMetricListRequest
    {
        public int MetricVariantId { get; set; }
        public int LocalEducationAgencyId { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExportMetricListRequest"/> using the specified parameters.
        /// </summary>
        /// <returns>A new <see cref="ExportMetricListRequest"/> instance.</returns>
        public static ExportMetricListRequest Create(int localEducationAgencyId, int metricVariantId)
        {
            return new ExportMetricListRequest { MetricVariantId = metricVariantId, LocalEducationAgencyId = localEducationAgencyId };
        }
    }

    public interface IExportMetricListService : IService<ExportMetricListRequest, ExportAllModel> { }

    public class ExportMetricListService : IExportMetricListService
    {
        //private readonly IService<SchoolMetricTableRequest, IList<SchoolMetricModel>> schoolMetricTableService;
        private readonly IService<SchoolMetricTableRequest, SchoolMetricTableModel> schoolMetricTableService;
        private readonly IMetadataListIdResolver metadataListIdResolver;
        private readonly IListMetadataProvider listMetadataProvider;

        public ExportMetricListService(IService<SchoolMetricTableRequest, SchoolMetricTableModel> schoolMetricTableService, IMetadataListIdResolver metadataListIdResolver, IListMetadataProvider listMetadataProvider)
        {
            this.schoolMetricTableService = schoolMetricTableService;
            this.metadataListIdResolver = metadataListIdResolver;
            this.listMetadataProvider = listMetadataProvider;
        }

        [CanBeAuthorizedBy(EdFiClaimTypes.ViewAllMetrics, EdFiClaimTypes.ViewMyMetrics)]
        public ExportAllModel Get(ExportMetricListRequest request)
        {
            //Get the data from the service that creates the table.
            var data = schoolMetricTableService.Get(SchoolMetricTableRequest.Create(request.LocalEducationAgencyId, request.MetricVariantId)).SchoolMetrics.OrderBy(x => x.Name);
            var listMetadata =
                listMetadataProvider.GetListMetadata(metadataListIdResolver.GetListId(ListType.SchoolMetricTable,
                                                                                      SchoolCategory.None));
            List<KeyValuePair<string, object>> cell;
            var rows = new List<ExportAllModel.Row>();

            foreach (var schoolMetricModel in data)
            {
                cell = new List<KeyValuePair<string, object>>();
                foreach (var metadataColumn in listMetadata.First().Columns)
                {
                    if (string.Equals(metadataColumn.ColumnName, "School"))
                        cell.Add(new KeyValuePair<string, object>("School Name", schoolMetricModel.Name));
                    if (string.Equals(metadataColumn.ColumnName, "Principal"))
                        cell.Add(new KeyValuePair<string, object>("Principal", schoolMetricModel.Principal));
                    if (string.Equals(metadataColumn.ColumnName, "Type"))
                        cell.Add(new KeyValuePair<string, object>("Type", schoolMetricModel.SchoolCategory));
                    if (string.Equals(metadataColumn.ColumnName, "School Metric Value"))
                        cell.Add(new KeyValuePair<string, object>("School Metric Value", schoolMetricModel.Value));
                    if (string.Equals(metadataColumn.ColumnName, "School Goal"))
                        cell.Add(new KeyValuePair<string, object>("School Goal", schoolMetricModel.Goal));
                    if (string.Equals(metadataColumn.ColumnName, "Difference From Goal"))
                        cell.Add(new KeyValuePair<string, object>("Difference From Goal", schoolMetricModel.GoalDifference));
                }
                rows.Add(new ExportAllModel.Row{Cells = cell});
            }

            return new ExportAllModel {Rows = rows};
            
        }
    }
}
