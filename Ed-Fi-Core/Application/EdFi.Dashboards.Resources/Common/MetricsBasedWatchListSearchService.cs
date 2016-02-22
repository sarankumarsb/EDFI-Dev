using EdFi.Dashboards.Application.Data.Entities;
using EdFi.Dashboards.Common;
using EdFi.Dashboards.Data.Entities;
using EdFi.Dashboards.Data.Repository;
using EdFi.Dashboards.Infrastructure.Implementations.Caching;
using EdFi.Dashboards.Resources.Models.Common;
using EdFi.Dashboards.Resources.Security.Common;
using EdFi.Dashboards.SecurityTokenService.Authentication;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web.Mvc;

namespace EdFi.Dashboards.Resources.Common
{
    /// <summary>
    /// Contains the search request data.
    /// </summary>
    public class MetricsBasedWatchListSearchRequest
    {
        /// <summary>
        /// Gets or sets the school identifier.
        /// </summary>
        /// <value>
        /// The school identifier.
        /// </value>
        public int? SchoolId { get; set; }

        /// <summary>
        /// Gets or sets the local education agency identifier.
        /// </summary>
        /// <value>
        /// The local education agency identifier.
        /// </value>
        public int? LocalEducationAgencyId { get; set; }

        /// <summary>
        /// Gets or sets the staff usi.
        /// </summary>
        /// <value>
        /// The staff usi.
        /// </value>
        [AuthenticationIgnore("Normally staff usi is part of authorization but in this case it is needed to create the return url")]
        public long? StaffUSI { get; set; }

        /// <summary>
        /// Gets or sets the metrics based watch list identifier.
        /// </summary>
        /// <value>
        /// The metrics based watch list identifier.
        /// </value>
        [AuthenticationIgnore("Passed to tell the post method which watch list was choosen")]
        public int? MetricsBasedWatchListId { get; set; }

        /// <summary>
        /// Gets or sets the referring controller.
        /// </summary>
        /// <value>
        /// The referring controller.
        /// </value>
        [AuthenticationIgnore("Passed to tell the post method which controller is calling the search")]
        public string ReferringController { get; set; }
    }

    public interface IMetricsBasedWatchListSearchService :
        IService<MetricsBasedWatchListSearchRequest, MetricsBasedWatchListSearchModel>,
        IPostHandler<MetricsBasedWatchListSearchRequest, string> { }

    public class MetricsBasedWatchListSearchService : IMetricsBasedWatchListSearchService
    {
        protected readonly IPersistingRepository<MetricBasedWatchList> MetricBasedWatchListRepository;
        protected readonly IPersistingRepository<MetricBasedWatchListOption> MetricBasedWatchListOptionRepository;
        protected readonly IPersistingRepository<MetricBasedWatchListSelectedOption> MetricBasedWatchListSelectedOptionRepository;
        protected readonly IRepository<StaffInformation> StaffInformationRepository;
        protected readonly IRepository<SchoolInformation> SchoolInformationRepository;
        protected readonly IWatchListSearchLinkProvider WatchListSearchLinkProvider;

        public MetricsBasedWatchListSearchService(IPersistingRepository<MetricBasedWatchList> metricBasedWatchListRepository,
            IPersistingRepository<MetricBasedWatchListOption> metricBasedWatchListOptionRepository,
            IPersistingRepository<MetricBasedWatchListSelectedOption> metricBasedWatchListSelectedOptionRepository,
            IRepository<StaffInformation> staffInformationRepository,
            IRepository<SchoolInformation> schoolInformationRepository,
            IWatchListSearchLinkProvider watchListSearchLinkProvider)
        {
            MetricBasedWatchListRepository = metricBasedWatchListRepository;
            MetricBasedWatchListOptionRepository = metricBasedWatchListOptionRepository;
            MetricBasedWatchListSelectedOptionRepository = metricBasedWatchListSelectedOptionRepository;
            StaffInformationRepository = staffInformationRepository;
            SchoolInformationRepository = schoolInformationRepository;
            WatchListSearchLinkProvider = watchListSearchLinkProvider;
        }

        /// <summary>
        /// Gets the specified request.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        [NoCache]
        [CanBeAuthorizedBy(EdFiClaimTypes.AccessOrganization)]
        public MetricsBasedWatchListSearchModel Get(MetricsBasedWatchListSearchRequest request)
        {
            var schoolIds = new List<int>();

            if (request.LocalEducationAgencyId.HasUsableValue())
                schoolIds.AddRange(
                    SchoolInformationRepository.GetAll()
                        .Where(data => data.LocalEducationAgencyId == request.LocalEducationAgencyId.GetValueOrDefault())
                        .Select(data => data.SchoolId));
            else if (request.SchoolId.HasUsableValue())
                schoolIds.Add(request.SchoolId.GetValueOrDefault());

            // if there aren't any school ids then stop here
            if (!schoolIds.Any())
                return new MetricsBasedWatchListSearchModel();

            // TODO: Remove this once the data access is changed to entity framework; subsonic only implements contains on enumerables
            var schoolIdEnumerable = (IEnumerable<int>)schoolIds;

            var watchLists = (MetricBasedWatchListRepository.GetAll()
                .Where(data => schoolIdEnumerable.Contains(data.EducationOrganizationId) && data.IsWatchListShared)
                .OrderBy(data => data.StaffUSI).ThenBy(data => data.WatchListName)
                .Select(data => new MetricBasedWatchList
                {
                    MetricBasedWatchListId = data.MetricBasedWatchListId,
                    StaffUSI = data.StaffUSI,
                    EducationOrganizationId = data.EducationOrganizationId,
                    WatchListName = data.WatchListName,
                    IsWatchListShared = data.IsWatchListShared
                })).ToList();

            // if there are no shared watch lists return an empty search model
            if (!watchLists.Any())
                return new MetricsBasedWatchListSearchModel();

            // TODO: Remove this once we move the entity framework; subsonic supports contains on IEnumerable but not on List
            var uniqueStaffUSIsList = watchLists.Select(data => data.StaffUSI).Distinct().ToList();
            var uniqueStaffUSIs = (IEnumerable<long>) uniqueStaffUSIsList;

            var teacherNames =
                StaffInformationRepository.GetAll()
                    .Where(data => uniqueStaffUSIs.Contains(data.StaffUSI))
                    .OrderBy(data => data.LastSurname)
                    .Select(data => new
                    {
                        key = data.StaffUSI,
                        value = data.FullName
                    }).ToDictionary(kv => kv.key, kv => kv.value);

            var watchListGroups = teacherNames.Select(name => new MetricBasedWatchListViewModel
            {
                TeacherName = name.Value, WatchLists = watchLists.Where(data => data.StaffUSI == name.Key).Select(data => new MetricBasedWatchListModel
                {
                    MetricBasedWatchListId = data.MetricBasedWatchListId,
                    EducationOrganizationId = data.EducationOrganizationId,
                    IsWatchListShared = data.IsWatchListShared,
                    StaffUSI = data.StaffUSI,
                    WatchListName = data.WatchListName
                }).ToList()
            }).ToList();

            return new MetricsBasedWatchListSearchModel
            {
                WatchListGroups = watchListGroups
            };
        }

        [NoCache]
        [CanBeAuthorizedBy(EdFiClaimTypes.AccessOrganization)]
        public string Post(MetricsBasedWatchListSearchRequest request)
        {
            // make sure we have the selected watch list and the staff USI
            if (!request.MetricsBasedWatchListId.HasValue || !request.StaffUSI.HasValue)
                return string.Empty;

            var watchList = MetricBasedWatchListRepository.GetAll()
                .Where(data => data.MetricBasedWatchListId == request.MetricsBasedWatchListId)
                .Select(data => new MetricBasedWatchList
                {
                    EducationOrganizationId = data.EducationOrganizationId,
                    WatchListName = data.WatchListName,
                    IsWatchListShared = false
                }).SingleOrDefault();

            if (watchList == null)
                return string.Empty;

            // set the staff USI to the current user
            watchList.StaffUSI = request.StaffUSI.Value;

            TransactionScope transaction = null;

            try
            {
                using (transaction = new TransactionScope())
                {
                    bool watchListCreated;
                    MetricBasedWatchListRepository.Save(watchList, out watchListCreated);

                    if (!watchListCreated)
                        return string.Empty;

                    var watchListId = watchList.MetricBasedWatchListId;

                    var watchListOptions = MetricBasedWatchListOptionRepository.GetAll()
                        .Where(data => data.MetricBasedWatchListId == request.MetricsBasedWatchListId)
                        .Select(data => new MetricBasedWatchListOption
                        {
                            Name = data.Name,
                            Value = data.Value
                        }).ToList();

                    foreach (var watchListOption in watchListOptions)
                    {
                        watchListOption.MetricBasedWatchListId = watchListId;
                        
                        // the staff usi should be set to the current user
                        if (watchListOption.Name.ToLower() == "staffusi")
                            watchListOption.Value = request.StaffUSI.ToString();

                        MetricBasedWatchListOptionRepository.Save(watchListOption);
                    }

                    // none of the class data should be copied from the shared watch list
                    var watchListSelectedOptions = MetricBasedWatchListSelectedOptionRepository.GetAll()
                        .Where(data => data.MetricBasedWatchListId == request.MetricsBasedWatchListId && data.Name.ToLower() != "selected-classes")
                        .Select(data => new MetricBasedWatchListSelectedOption
                        {
                            Name = data.Name,
                            Value = data.Value
                        }).ToList();

                    foreach (var watchListSelectedOption in watchListSelectedOptions)
                    {
                        watchListSelectedOption.MetricBasedWatchListId = watchListId;
                        MetricBasedWatchListSelectedOptionRepository.Save(watchListSelectedOption);
                    }

                    // if everything works then commit the transacation
                    transaction.Complete();

                    // this needs to be set before calling the link generator
                    // so that the link will validate with security
                    request.MetricsBasedWatchListId = watchList.MetricBasedWatchListId;
	                var returnUrl = request.ReferringController;
                        
                        //StaffAreaLinks.CustomMetricsBasedWatchList(request.SchoolId.Value, request.StaffUSI.Value,
                        //"GeneralOverview", watchList.MetricBasedWatchListId, null, StudentListType.MetricsBasedWatchList.ToString());

                    return returnUrl;
                }
            }
            catch
            {
                if (transaction != null)
                    transaction.Dispose();

                return string.Empty;
            }
        }
    }
}
