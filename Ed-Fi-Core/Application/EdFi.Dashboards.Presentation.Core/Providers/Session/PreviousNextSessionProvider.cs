using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using EdFi.Dashboards.Infrastructure;
using EdFi.Dashboards.Presentation.Core.Models.Shared;
using EdFi.Dashboards.Resources.Application;
using EdFi.Dashboards.Resources.Common;
using EdFi.Dashboards.Resources.Models.Common;
using EdFi.Dashboards.Resources.Staff;

namespace EdFi.Dashboards.Presentation.Core.Providers.Session
{
    public interface IPreviousNextSessionProvider
    {
        List<long> GetStudentIdList(PreviousNextDataModel previousNextModel, int pageNumber, int pageSize, int? sortColumn, string sortDirection);
        List<NameValuesType> GetWatchListSelectedValues(PreviousNextDataModel previousNextDataModel);
        void SetPreviousNextDataModel(PreviousNextDataModel previousNextModel, int? sortColumn, string sortDirection, List<StudentSchoolIdentifier> entityIds);
        PreviousNextDataModel GetPreviousNextModel(Uri urlReferrer, string tablePrefix, int? uniqueKeyId = null);
        void RemovePreviousNextDataModel(int? uniqueKeyId = null);
    }

    public class PreviousNextSessionProvider : IPreviousNextSessionProvider
    {
        private readonly ISessionStateProvider sessionStateProvider;
        private readonly IUniqueListIdProvider uniqueListProvider;

        public PreviousNextSessionProvider(ISessionStateProvider sessionStateProvider, IUniqueListIdProvider uniqueListProvider)
        {
            this.sessionStateProvider = sessionStateProvider;
            this.uniqueListProvider = uniqueListProvider;
        }

        public List<long> GetStudentIdList(PreviousNextDataModel previousNextModel, int pageNumber, int pageSize, int? sortColumn, string sortDirection)
        {
            var studentIdList = new List<long>();

            // Use the Entity Id list from session state if populated and the sort column and direction have not changed
            if (previousNextModel.EntityIdArray != null && previousNextModel.EntityIdArray.Any() &&
                previousNextModel.SortColumn == sortColumn && previousNextModel.SortDirection == sortDirection)
            {
                studentIdList = previousNextModel.EntityIdArray.Select(s => s[0]).Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            }

            return studentIdList;
        }

        /// <summary>
        /// Gets the watch list selected values.
        /// </summary>
        /// <param name="previousNextDataModel">The previous next data model.</param>
        /// <returns></returns>
        public List<NameValuesType> GetWatchListSelectedValues(PreviousNextDataModel previousNextDataModel)
        {
            var watchListSelectedValues = new List<NameValuesType>();

            if (previousNextDataModel.StudentWatchListData != null && previousNextDataModel.StudentWatchListData.Any())
            {
                watchListSelectedValues = previousNextDataModel.StudentWatchListData;
            }

            return watchListSelectedValues;
        }

        public void RemovePreviousNextDataModel(int? uniqueKeyId = null)
        {
            var persistenceUniqueId = uniqueKeyId.HasValue ? uniqueListProvider.GetUniqueId(uniqueKeyId.Value) : uniqueListProvider.GetUniqueId();
            sessionStateProvider.RemoveValue(persistenceUniqueId);
        }

        public void SetPreviousNextDataModel(PreviousNextDataModel previousNextModel, int? sortColumn, string sortDirection, List<StudentSchoolIdentifier> entityIds)
        {
            if (entityIds.Any())
            {
                // TODO: GKM - Revisit the use of int[][] (magic values) in previous/next model
                previousNextModel.EntityIdArray = entityIds.Select(x => new[] { x.StudentUSI, x.SchoolId }).ToArray();
            }

            previousNextModel.SortColumn = sortColumn;
            previousNextModel.SortDirection = sortDirection;
            sessionStateProvider[previousNextModel.ListPersistenceUniqueId] = previousNextModel;
        }

        public PreviousNextDataModel GetPreviousNextModel(Uri urlReferrer, string tablePrefix, int? uniqueKeyId = null)
        {
            // Get Previous / Next from Session State if available or initialize new model if not found
            var persistenceUniqueId = uniqueKeyId.HasValue ? uniqueListProvider.GetUniqueId(uniqueKeyId.Value) : uniqueListProvider.GetUniqueId();
            var previousNextDataModel = sessionStateProvider[persistenceUniqueId] as PreviousNextDataModel;
            var uniqueId = uniqueKeyId.HasValue ? uniqueKeyId.Value.ToString(CultureInfo.InvariantCulture) : null;

            return previousNextDataModel ??
                   new PreviousNextDataModel
                   {
                       MetricId = uniqueId,
                       ListUrl = urlReferrer != null ? urlReferrer.OriginalString : null,
                       ListPersistenceUniqueId = persistenceUniqueId,
                       ListType = "StudentList",
                       ParameterNames = new[] { "studentUSI", "schoolId" },
                       TableId = string.Format("{0}{1}-GridData", tablePrefix, uniqueId)
                   };
        }
    }
}
