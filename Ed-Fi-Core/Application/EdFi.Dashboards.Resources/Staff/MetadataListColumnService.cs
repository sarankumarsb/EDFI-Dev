using System;
using System.Linq;
using EdFi.Dashboards.Common;
using EdFi.Dashboards.Core;
using EdFi.Dashboards.Data.Repository;
using EdFi.Dashboards.Metric.Data.Entities;
using EdFi.Dashboards.Resources.Models.CustomGrid;
using EdFi.Dashboards.Resources.Models.Staff;
using EdFi.Dashboards.Resources.Security.Common;

namespace EdFi.Dashboards.Resources.Staff
{
    public class MetadataListColumnRequest
    {
        public int MetadataListId { get; set; }
        public string SubjectArea { get; set; }

        public static MetadataListColumnRequest Create(int metadataListId)
        {
            return new MetadataListColumnRequest { MetadataListId = metadataListId };
        }
    }

    public interface IMetadataListColumnService : IService<MetadataListColumnRequest, MetadataListColumnModel> { }

    public class MetadataListColumnService : IMetadataListColumnService
    {
        private readonly IRepository<MetadataListColumnGroup> metadataColumnGroupRepository;
        private readonly IRepository<MetadataListColumn> metadataColumnRepository;
        private readonly IRepository<MetadataMetricCellListType> metadataMetricCellListRepository;
        private readonly IRepository<MetadataSubjectArea> metaDataSubjectAreaRepository;
        private readonly IRepository<MetadataGroupType> metadataGroupTypeRepository;

        public MetadataListColumnService(IRepository<MetadataListColumnGroup> metaDataColumnGroupRepository,
            IRepository<MetadataListColumn> metadataColumnRepository,
            IRepository<MetadataMetricCellListType> metadataMetricCellListRepository,
            IRepository<MetadataSubjectArea> metadataSubjectAreaRepository,
            IRepository<MetadataGroupType> metadataGroupTypeRepository)
        {
            this.metadataColumnGroupRepository = metaDataColumnGroupRepository;
            this.metadataColumnRepository = metadataColumnRepository;
            this.metadataMetricCellListRepository = metadataMetricCellListRepository;
            this.metaDataSubjectAreaRepository = metadataSubjectAreaRepository;
            this.metadataGroupTypeRepository = metadataGroupTypeRepository;
        }

        [AuthenticationIgnore("Everyone should be able to get metadata")]
        public MetadataListColumnModel Get(MetadataListColumnRequest request)
        {
            var results = (from groups in metadataColumnGroupRepository.GetAll()
                           from columns in
                               metadataColumnRepository.GetAll()
                                   .Where(x => x.MetadataListColumnGroupId == groups.MetadataListColumnGroupId)
                                   .DefaultIfEmpty()
                           from cell in
                               metadataMetricCellListRepository.GetAll()
                                   .Where(x => x.MetadataMetricCellListTypeId == columns.MetadataMetricCellListTypeId)
                                   .DefaultIfEmpty()
                           from sa in
                               metaDataSubjectAreaRepository.GetAll()
                                   .Where(x => x.MetadataSubjectAreaId == groups.MetadataSubjectAreaId)
                                   .DefaultIfEmpty()
                           from groupType in
                               metadataGroupTypeRepository.GetAll().Where(x => x.MetadataGroupTypeId == groups.MetadataGroupTypeId)
                           where groups.MetadataListId == request.MetadataListId && sa.Name == request.SubjectArea
                           orderby groups.GroupOrder, columns.ColumnOrder

                           select new
                           {
                               GroupId = groups.MetadataListColumnGroupId,
                               GroupType =
                                   groups.MetadataGroupTypeId != 0
                                       ? (GroupType)(Enum.Parse(typeof(GroupType), groupType.Name))
                                       : GroupType.EntityInformation,
                               Title = groups.Title,
                               IsVisibleByDefault = groups.IsVisibleByDefault,
                               IsFixedColumnGroup = groups.IsFixedColumnGroup,
                               UniqueId = -1,
                               ColumnId = columns.MetadataListColumnId,
                               ColumnUniqueIdentifier = columns.UniqueIdentifier,
                               ColumnName = columns.ColumnName,
                               MetricVariantId = columns.MetricVariantId,
                               ColumnOrder = columns.ColumnOrder,
                               ColumnIsVisibleByDefault = columns.IsVisibleByDefault,
                               IsFixedColumn = columns.IsFixedColumn,
                               MetricListCellType =
                                   columns.ColumnName != null
                                       ? (MetricListCellType)Enum.Parse(typeof(MetricListCellType), cell.Name)
                                       : MetricListCellType.None,
                               ColumnPrefix = columns.ColumnPrefix,
                               SortAscending = columns.SortAscending,
                               SortDescending = columns.SortDescending,
                               Tooltip = columns.Tooltip
                           }).ToList();

            var resultsGrouped = (from i in results
                                  group i by i.GroupId
                                      into g
                                      select new MetadataColumnGroup
                                      {
                                          GroupType = g.First().GroupType,
                                          Title = g.First().Title,
                                          IsVisibleByDefault = g.First().IsVisibleByDefault,
                                          IsFixedColumnGroup = g.First().IsFixedColumnGroup,
                                          UniqueId = g.First().UniqueId,
                                          Columns = g.Select(x => new MetadataColumn
                                          {
                                              ColumnName = x.ColumnName,
                                              ColumnPrefix = x.ColumnPrefix,
                                              UniqueIdentifier = x.ColumnUniqueIdentifier,
                                              MetricVariantId = x.MetricVariantId,
                                              SchoolCategory = SchoolCategory.None,
                                              Order = x.ColumnOrder,
                                              IsVisibleByDefault =
                                                  x.ColumnIsVisibleByDefault.HasValue ? x.ColumnIsVisibleByDefault.Value : false,
                                              IsFixedColumn = x.IsFixedColumn.HasValue ? x.IsFixedColumn.Value : false,
                                              MetricListCellType = x.MetricListCellType,
                                              SortAscending = x.SortAscending,
                                              SortDescending = x.SortDescending,
                                              Tooltip = x.Tooltip
                                          }).Where(x => x.ColumnName != null).ToList()
                                      }).ToList();

            return new MetadataListColumnModel {ColumnGroups = resultsGrouped};
        }
    }
}
