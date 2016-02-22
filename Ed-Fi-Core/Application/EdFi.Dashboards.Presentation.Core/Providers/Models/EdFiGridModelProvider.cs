using System.Collections.Generic;
using System.Globalization;
using EdFi.Dashboards.Metric.Resources.Models;
using EdFi.Dashboards.Presentation.Core.Models.Shared;
using EdFi.Dashboards.Resources.Models.CustomGrid;
using EdFiGridModel = EdFi.Dashboards.Presentation.Core.Models.Shared.EdFiGridModel;
using GridTable=EdFi.Dashboards.Resources.Models.CustomGrid.GridTable;

namespace EdFi.Dashboards.Presentation.Core.Providers.Models
{
    public class EdFiGridModelProvider : IEdFiGridModelProvider
    {
        public virtual EdFiGridModel GetEdFiGridModel(bool displayInteractionMenu, bool displayAddRemoveColumns, int fixedHeight, string name, int? metricVariantId, GridTable gridTable, IList<MetricFootnote> metricFootnotes, string nonPersistedSettings, bool sizeToWindow, string listType, IList<string> legendViewNames, string hrefToUse, bool usePagination, string paginationCallbackUrl, bool allowMaximizeGrid,
            EdFiGridWatchListModel studentWatchList = null, string selectedDemographicOption = null, string selectedSchoolCategoryOption = null, string selectedGradeOption = null, string previousNextSessionPage = null, string exportGridDataUrl = null, ListType gridListType = default(ListType))
        {
            var model = new EdFiGridModel
            {
                DisplayInteractionMenu = displayInteractionMenu,
                DisplayAddRemoveColumns = displayAddRemoveColumns,
                FixedHeight = fixedHeight,
                GridName = name,
                GridTable = gridTable,
                SizeToWindow = sizeToWindow,
                UniqueTableName = name,
                MetricFootnotes = metricFootnotes,
                NonPersistedSettings = nonPersistedSettings,
                EntityList = string.IsNullOrEmpty(listType) ? null :
                    new EntityListModel
                    {
                        ListType = listType,
                        MetricVariantId =
                            metricVariantId.HasValue
                                ? metricVariantId.Value.ToString(CultureInfo.InvariantCulture)
                                : string.Empty,
                        RowIndexForId = 1
                    },
                LegendViewNames = legendViewNames,
                DefaultSortColumn = 1,
                HrefToUse = hrefToUse,
                UsePagination = usePagination,
                PaginationCallbackUrl = paginationCallbackUrl,
                AllowMaximizeGrid = allowMaximizeGrid,
                StudentWatchList = studentWatchList,
                SelectedDemographicOption = selectedDemographicOption,
                SelectedSchoolCategoryOption = selectedSchoolCategoryOption,
                SelectedGradeOption = selectedGradeOption,
                PreviousNextSessionPage = previousNextSessionPage,
                ExportGridDataUrl = exportGridDataUrl,
                ListType = gridListType
            };
            return model;
        }
    }
}
