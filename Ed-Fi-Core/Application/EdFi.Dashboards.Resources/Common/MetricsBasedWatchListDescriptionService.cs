using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EdFi.Dashboards.Common;
using EdFi.Dashboards.Infrastructure.Implementations.Caching;
using EdFi.Dashboards.Resources.Models.Common;
using EdFi.Dashboards.Resources.Security.Common;
using EdFi.Dashboards.SecurityTokenService.Authentication;

namespace EdFi.Dashboards.Resources.Common
{
    public class MetricsBasedWatchListDescriptionRequest
    {
        /// <summary>
        /// Gets or sets the local education agency identifier.
        /// </summary>
        /// <value>
        /// The local education agency identifier.
        /// </value>
        public int? LocalEducationAgencyId { get; set; }

        /// <summary>
        /// Gets or sets the school identifier.
        /// </summary>
        /// <value>
        /// The school identifier.
        /// </value>
        public int? SchoolId { get; set; }

        /// <summary>
        /// Gets or sets the metrics based watch list identifier.
        /// </summary>
        /// <value>
        /// The metrics based watch list identifier.
        /// </value>
        [AuthenticationIgnore("The watch list id is not needed for validation")]
        public int MetricsBasedWatchListId { get; set; }
    }

    public interface IMetricsBasedWatchListDescriptionService : IService<MetricsBasedWatchListDescriptionRequest, MetricsBasedWatchListDescriptionModel> { }

    public class MetricsBasedWatchListDescriptionService : IMetricsBasedWatchListDescriptionService
    {
        protected readonly IMetricsBasedWatchListDataProvider MetricsBasedWatchListDataProvider;
        protected readonly IMetricsBasedWatchListSelectionProvider MetricsBasedWatchListSelectionProvider;

        public MetricsBasedWatchListDescriptionService(
            IMetricsBasedWatchListDataProvider metricsBasedWatchListDataProvider,
            IMetricsBasedWatchListSelectionProvider metricsBasedWatchListSelectionProvider)
        {
            MetricsBasedWatchListDataProvider = metricsBasedWatchListDataProvider;
            MetricsBasedWatchListSelectionProvider = metricsBasedWatchListSelectionProvider;
        }

        /// <summary>
        /// Gets the description data of the watch list.
        /// </summary>
        /// <param name="request">The request containing data to access the watch list information.</param>
        /// <returns>A <see cref="MetricsBasedWatchListDescriptionModel"/> containing the requested data.</returns>
        [NoCache]
        [CanBeAuthorizedBy(EdFiClaimTypes.AccessOrganization)]
        public MetricsBasedWatchListDescriptionModel Get(MetricsBasedWatchListDescriptionRequest request)
        {
            var watchList = MetricsBasedWatchListDataProvider.GetEdFiGridWatchListModel(UserInformation.Current.StaffUSI,
                request.SchoolId.GetValueOrDefault(), request.LocalEducationAgencyId.GetValueOrDefault(),
                request.MetricsBasedWatchListId);

            var descriptionModel = new MetricsBasedWatchListDescriptionModel
            {
                WatchListDescription = watchList.WatchListDescription ?? string.Empty,
                WatchListSelections = MetricsBasedWatchListSelectionProvider.CreateSelectionText(watchList)
            };

            return descriptionModel;
        }
    }
}
