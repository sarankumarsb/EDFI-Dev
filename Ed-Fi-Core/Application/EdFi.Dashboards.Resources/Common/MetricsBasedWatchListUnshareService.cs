using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EdFi.Dashboards.Application.Data.Entities;
using EdFi.Dashboards.Common;
using EdFi.Dashboards.Data.Repository;
using EdFi.Dashboards.Resources.Security.Common;
using EdFi.Dashboards.SecurityTokenService.Authentication;

namespace EdFi.Dashboards.Resources.Common
{
    public class MetricsBasedWatchListUnshareRequest
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
        /// Gets or sets the staff usi.
        /// </summary>
        /// <value>
        /// The staff usi.
        /// </value>
        public long? StaffUSI { get; set; }

        /// <summary>
        /// Gets or sets the metric based watch list identifier.
        /// </summary>
        /// <value>
        /// The metric based watch list identifier.
        /// </value>
        [AuthenticationIgnore("Validation not needed since the watch list may not be associated with a user")]
        public int MetricBasedWatchListId { get; set; }
    }

    public interface IMetricsBasedWatchListUnshareService : IPostHandler<MetricsBasedWatchListUnshareRequest, bool> { }

    public class MetricsBasedWatchListUnshareService : IMetricsBasedWatchListUnshareService
    {
        protected readonly IPersistingRepository<MetricBasedWatchList> MetricBasedWatchListRepository;

        public MetricsBasedWatchListUnshareService(IPersistingRepository<MetricBasedWatchList> metricBasedWatchListRepository)
        {
            MetricBasedWatchListRepository = metricBasedWatchListRepository;
        }

        /// <summary>
        /// Posts the specified watch list identifier.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>
        /// A boolean indicating if the action was successful.
        /// </returns>
        [CanBeAuthorizedBy(EdFiClaimTypes.ManageWatchList)]
        public bool Post(MetricsBasedWatchListUnshareRequest request)
        {
            try
            {
                var watchList =
                    MetricBasedWatchListRepository.GetAll()
                        .SingleOrDefault(data => data.MetricBasedWatchListId == request.MetricBasedWatchListId);

                if (watchList != null && watchList.IsWatchListShared)
                {
                    watchList.IsWatchListShared = false;
                    MetricBasedWatchListRepository.Save(watchList);
                }
            }
            catch
            {
                return false;
            }

            return true;
        }
    }
}
