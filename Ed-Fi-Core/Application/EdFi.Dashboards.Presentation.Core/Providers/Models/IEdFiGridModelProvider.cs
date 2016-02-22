using System.Collections;
using System.Collections.Generic;
using EdFi.Dashboards.Metric.Resources.Models;
using EdFi.Dashboards.Resources.Models.CustomGrid;
using EdFiGridModel = EdFi.Dashboards.Presentation.Core.Models.Shared.EdFiGridModel;
using GridTable=EdFi.Dashboards.Resources.Models.CustomGrid.GridTable;

namespace EdFi.Dashboards.Presentation.Core.Providers.Models
{
    public interface IEdFiGridModelProvider
    {
        EdFiGridModel GetEdFiGridModel(bool displayInteractionMenu, bool displayAddRemoveColumns, int fixedHeight,
            string name, int? metricVariantId, GridTable gridTable, IList<MetricFootnote> metricFootnotes,
            string nonPersistedSettings, bool sizeToWindow, string listType, IList<string> legendViewNames,
            string hrefToUse, bool usePagination, string paginationCallbackUrl, bool allowMaximizeGrid,
            EdFiGridWatchListModel studentWatchlist = null, string selectedDemographicOption = null, string selectedSchoolCategoryOption = null, string selectedGradeOption = null, string previousNextSessionPage = null, string exportGridDataUrl = null, ListType gridListType = default(ListType));
    }
}
