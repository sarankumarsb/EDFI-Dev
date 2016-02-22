using System;
using System.Globalization;
using EdFi.Dashboards.Application.Data.Entities;
using EdFi.Dashboards.Common;
using EdFi.Dashboards.Data.Repository;
using EdFi.Dashboards.Infrastructure;
using EdFi.Dashboards.Infrastructure.Implementations.Caching;
using EdFi.Dashboards.Resources.Models.Common;
using EdFi.Dashboards.Resources.Models.CustomGrid;
using EdFi.Dashboards.Resources.Navigation;
using EdFi.Dashboards.Resources.Security.Common;
using EdFi.Dashboards.SecurityTokenService.Authentication;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;

namespace EdFi.Dashboards.Resources.Common
{
    /// <summary>
    /// The watch list request object used to get watch list data.
    /// </summary>
    public class MetricsBasedWatchListGetRequest
    {
        /// <summary>
        /// Gets or sets the watch list identifier.
        /// </summary>
        /// <value>
        /// The watch list identifier.
        /// </value>
        [AuthenticationIgnore("Does not contain user identification information")]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the staff usi.
        /// </summary>
        /// <value>
        /// The staff usi.
        /// </value>
        public long? StaffUSI { get; set; }

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
    }

    /// <summary>
    /// The watch list request object that will be built from the ajax post
    /// method.
    /// </summary>
    public class MetricsBasedWatchListPostRequest
    {
        /// <summary>
        /// Gets or sets the watch list identifier.
        /// </summary>
        /// <value>
        /// The watch list identifier.
        /// </value>
        public int? MetricBasedWatchListId { get; set; }

        /// <summary>
        /// Gets or sets the staff USI.
        /// </summary>
        /// <value>
        /// The staff usi.
        /// </value>
        public long? StaffUSI { get; set; }

        /// <summary>
        /// Gets or sets the page staff usi; in most cases this will be the
        /// same as the staff usi but in the case of a principal viewing a
        /// teachers page these values will be different.
        /// </summary>
        /// <value>
        /// The page staff usi.
        /// </value>
        [AuthenticationIgnore("This is the staff USI from the current page not the logged in staff USI")]
        public long? PageStaffUSI { get; set; }

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
        /// Gets or sets the name of the watch list.
        /// </summary>
        /// <value>
        /// The name of the watch list.
        /// </value>
        [AuthenticationIgnore("Does not contain user identification information")]
        public string WatchListName { get; set; }

        /// <summary>
        /// Gets or sets the watch list description.
        /// </summary>
        /// <value>
        /// The watch list description.
        /// </value>
        [AuthenticationIgnore("Contains the users description of this watch list")]
        public string WatchListDescription { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the watch list is shared.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the watch list is shared; otherwise, <c>false</c>.
        /// </value>
        [AuthenticationIgnore("Is used to determine if the watch list is shared")]
        public bool IsWatchListShared { get; set; }

        /// <summary>
        /// Gets or sets the action.
        /// </summary>
        /// <value>
        /// The action.
        /// </value>
        [AuthenticationIgnore("Does not contain user identification information")]
        public string Action { get; set; }

        /// <summary>
        /// Gets or sets the name of the resource.
        /// </summary>
        /// <value>
        /// The name of the resource.
        /// </value>
        [AuthenticationIgnore("Used to create a return Url")]
        public string ResourceName { get; set; }

        /// <summary>
        /// Gets or sets the json.
        /// </summary>
        /// <value>
        /// The json.
        /// </value>
        [AuthenticationIgnore("Does not contain user identification information")]
        public string SelectedValuesJson { get; set; }

        /// <summary>
        /// Gets or sets the demographic.
        /// </summary>
        /// <value>
        /// The demographic.
        /// </value>
        [AuthenticationIgnore("Used to indicate demographic information if it is available")]
        public string Demographic { get; set; }

        /// <summary>
        /// Gets or sets the level.
        /// </summary>
        /// <value>
        /// The level.
        /// </value>
        [AuthenticationIgnore("Used to indicate school level information if it is available")]
        public string SchoolCategory { get; set; }

        /// <summary>
        /// Gets or sets the grade.
        /// </summary>
        /// <value>
        /// The grade.
        /// </value>
        [AuthenticationIgnore("Used to indicate grade level information if it is available")]
        public string Grade { get; set; }

        /// <summary>
        /// Gets or sets the selected options.
        /// </summary>
        /// <value>
        /// The selected options.
        /// </value>
        [AuthenticationIgnore("Does not contain user identification information")]
        public List<NameValuesType> SelectedOptions { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public interface IMetricsBasedWatchListService : IService<MetricsBasedWatchListGetRequest, EdFiGridWatchListModel>,
        IPostHandler<MetricsBasedWatchListPostRequest, string> { }

    public class MetricsBasedWatchListService : IMetricsBasedWatchListService
    {
        protected readonly IPersistingRepository<MetricBasedWatchList> MetricBasedWatchListRepository;
        protected readonly IPersistingRepository<MetricBasedWatchListSelectedOption> MetricBasedWatchListSelectionOptionRepository;
        protected readonly IPersistingRepository<MetricBasedWatchListOption> MetricBasedWatchListOptionRepository;
        protected readonly ILocalEducationAgencyAreaLinks EducationAgencyAreaLinks;
        protected readonly IStaffAreaLinks StaffAreaLinks;
        protected readonly IMetricsBasedWatchListDataProvider WatchListDataProvider;
        protected readonly ICacheProvider CacheProvider;
        protected readonly IWatchListLinkProvider WatchListLinkProvider;

        public MetricsBasedWatchListService(
            IPersistingRepository<MetricBasedWatchList> metricBasedWatchListRepository,
            IPersistingRepository<MetricBasedWatchListSelectedOption> metricBasedWatchListSelectionOptionRepository,
            IPersistingRepository<MetricBasedWatchListOption> metricBasedWatchListOptionRepository,
            ILocalEducationAgencyAreaLinks educationAgencyAreaLinks,
            IStaffAreaLinks staffAreaLinks,
            IMetricsBasedWatchListDataProvider watchListDataProvider,
            ICacheProvider cacheProvider,
            IWatchListLinkProvider watchListLinkProvider)
        {
            MetricBasedWatchListRepository = metricBasedWatchListRepository;
            MetricBasedWatchListSelectionOptionRepository = metricBasedWatchListSelectionOptionRepository;
            MetricBasedWatchListOptionRepository = metricBasedWatchListOptionRepository;
            EducationAgencyAreaLinks = educationAgencyAreaLinks;
            StaffAreaLinks = staffAreaLinks;
            WatchListDataProvider = watchListDataProvider;
            CacheProvider = cacheProvider;
            WatchListLinkProvider = watchListLinkProvider;
        }

        [NoCache]
        [CanBeAuthorizedBy(EdFiClaimTypes.ViewAllStudents, EdFiClaimTypes.ViewMyStudents)]
        public EdFiGridWatchListModel Get(MetricsBasedWatchListGetRequest request)
        {
            if (!request.StaffUSI.HasUsableValue() || (!request.SchoolId.HasUsableValue() && !request.LocalEducationAgencyId.HasUsableValue()))
                return new EdFiGridWatchListModel();

            if (request.SchoolId.HasUsableValue())
                return WatchListDataProvider.GetEdFiGridWatchListModel(request.StaffUSI.Value, request.SchoolId.Value,
                    null, request.Id);
            
            return WatchListDataProvider.GetEdFiGridWatchListModel(request.StaffUSI.Value, null,
                request.LocalEducationAgencyId.Value, request.Id);
        }

        [NoCache]
		[AuthenticationIgnore("Watchlists themselves do not contain data.")]
        public string Post(MetricsBasedWatchListPostRequest request)
        {
            var watchListId = request.MetricBasedWatchListId;
            var staffUSI = request.StaffUSI;
            var pageStaffUSI = request.PageStaffUSI;
            var educationOrganizationId = request.SchoolId.HasUsableValue() ? request.SchoolId.GetValueOrDefault() : request.LocalEducationAgencyId.GetValueOrDefault();

            if (!educationOrganizationId.HasUsableValue())
                return string.Empty;

            var postResult = string.Empty;
            var actionResult = 0;

            switch (request.Action.ToLower())
            {
                case "add":
                    if (staffUSI.HasValue)
                    {
                        actionResult = CreateWatchList(staffUSI.Value, pageStaffUSI, request.SchoolId, request.LocalEducationAgencyId, request.WatchListName,
                            request.WatchListDescription, request.IsWatchListShared, request.SelectedOptions, request.ResourceName, request.Demographic, request.SchoolCategory,
                            request.Grade);
                    }
                    break;
                case "set":
                    if (watchListId.HasValue)
                    {
                        UpdateWatchList(watchListId.Value, request.WatchListName, request.WatchListDescription, request.IsWatchListShared,
                            request.SelectedOptions, request.Demographic, request.SchoolCategory, request.Grade);
                        actionResult = watchListId.Value;
                    }
                    break;
                case "delete":
                    if (watchListId.HasValue)
                    {
                        DeleteWatchList(watchListId.Value);
                    }
                    break;
            }

            var watchListRequest = new WatchListLinkRequest
            {
                LocalEducationAgencyId = request.LocalEducationAgencyId,
                SchoolId = request.SchoolId,
				StaffUSI = request.PageStaffUSI,
                PageStaffUSI = pageStaffUSI,
                MetricBasedWatchListId = actionResult,
                ResourceName = request.ResourceName,
                Demographic = request.Demographic,
                SchoolCategory = request.SchoolCategory,
                Grade = request.Grade
            };

            postResult = WatchListLinkProvider.GenerateLink(watchListRequest);
            return postResult;
        }

        /// <summary>
        /// Creates the watch list.
        /// </summary>
        /// <param name="staffUSI">The staff USI.</param>
        /// <param name="pageStaffUSI">The page staff usi.</param>
        /// <param name="schoolId">The school identifier.</param>
        /// <param name="localEducationAgencyId">The local education agency identifier.</param>
        /// <param name="watchListName">Name of the watch list.</param>
        /// <param name="watchListDescription">The watch list description.</param>
        /// <param name="isWatchListShared">if set to <c>true</c> the watch list is shared.</param>
        /// <param name="watchListSelectedOptions">The watch list selected options.</param>
        /// <param name="pageController">The page controller.</param>
        /// <param name="pageDemographic">The page demographic.</param>
        /// <param name="pageLevel">The page level.</param>
        /// <param name="pageGrade">The page grade.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException">Either the school id or local education agency id needs a value</exception>
        protected virtual int CreateWatchList(long staffUSI, long? pageStaffUSI, int? schoolId, int? localEducationAgencyId, string watchListName, string watchListDescription,
            bool isWatchListShared, List<NameValuesType> watchListSelectedOptions, string pageController, string pageDemographic = null, string pageLevel = null, string pageGrade = null)
        {
            // this should never happen but checking just in case
            if (!schoolId.HasUsableValue() && !localEducationAgencyId.HasUsableValue())
                return 0;

            var educationOrganizationId = schoolId.HasUsableValue()
                ? schoolId.Value : localEducationAgencyId.GetValueOrDefault();

            if (educationOrganizationId <= 0)
                throw new ArgumentException("Either the school id or local education agency id needs a value");

            var watchList = new MetricBasedWatchList
            {
                StaffUSI = staffUSI,
                EducationOrganizationId = educationOrganizationId,
                WatchListName = watchListName,
                WatchListDescription = watchListDescription,
                IsWatchListShared = isWatchListShared
            };

            int returnValue;
            TransactionScope scope = null;

            try
            {
                using (scope = new TransactionScope())
                {
                    // first create the watch list data
                    MetricBasedWatchListRepository.Save(watchList);

                    if (!watchList.MetricBasedWatchListId.HasUsableValue() || !watchListSelectedOptions.Any())
                    {
                        scope.Dispose();
                        return 0;
                    }

                    returnValue = watchList.MetricBasedWatchListId;

                    // next create the selected options data
                    var selectedOptions = new List<MetricBasedWatchListSelectedOption>();
                    foreach (var watchListSelectedOption in watchListSelectedOptions)
                    {
                        selectedOptions.AddRange(watchListSelectedOption.Values.Select(selectedOption => new MetricBasedWatchListSelectedOption
                        {
                            MetricBasedWatchListId = watchList.MetricBasedWatchListId,
                            Name = watchListSelectedOption.Name,
                            Value = selectedOption
                        }));
                    }

                    foreach (var metricBasedWatchListSelectedOption in selectedOptions)
                        MetricBasedWatchListSelectionOptionRepository.Save(metricBasedWatchListSelectedOption);

                    // lastly create the watch list options data
                    var watchListOptions = new List<MetricBasedWatchListOption>();

                    if (pageStaffUSI.HasUsableValue())
                        watchListOptions.Add(CreateMetricBasedWatchListOption(watchList.MetricBasedWatchListId,
                            "StaffUSI", pageStaffUSI.Value.ToString(CultureInfo.InvariantCulture)));

                    if (schoolId.HasUsableValue())
                        watchListOptions.Add(CreateMetricBasedWatchListOption(watchList.MetricBasedWatchListId,
                            "SchoolId", schoolId.Value.ToString(CultureInfo.InvariantCulture)));

                    if (localEducationAgencyId.HasUsableValue())
                        watchListOptions.Add(CreateMetricBasedWatchListOption(watchList.MetricBasedWatchListId,
                            "LocalEducationAgencyId",
                            localEducationAgencyId.Value.ToString(CultureInfo.InvariantCulture)));

                    if (!string.IsNullOrWhiteSpace(pageController))
                        watchListOptions.Add(CreateMetricBasedWatchListOption(watchList.MetricBasedWatchListId, "PageController", pageController));

                    if (!string.IsNullOrWhiteSpace(pageDemographic))
                        watchListOptions.Add(CreateMetricBasedWatchListOption(watchList.MetricBasedWatchListId, "PageDemographic", pageDemographic));

                    if (!string.IsNullOrWhiteSpace(pageLevel))
                        watchListOptions.Add(CreateMetricBasedWatchListOption(watchList.MetricBasedWatchListId, "PageSchoolCategory", pageLevel));

                    if (!string.IsNullOrWhiteSpace(pageGrade))
                        watchListOptions.Add(CreateMetricBasedWatchListOption(watchList.MetricBasedWatchListId, "PageGrade", pageGrade));

                    foreach (var watchListOption in watchListOptions)
                        MetricBasedWatchListOptionRepository.Save(watchListOption);

                    // if all of the records are created without any errors
                    // then commit the transaction
                    scope.Complete();
                }
            }
            catch
            {
                // if there is a failure the transaction should be rolled back
                if (scope != null)
                    scope.Dispose();

                returnValue = 0;
            }

            return returnValue;
        }

        /// <summary>
        /// Creates a metric based watch list option.
        /// </summary>
        /// <param name="metricBasedWatchListId">The metric based watch list identifier.</param>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        private static MetricBasedWatchListOption CreateMetricBasedWatchListOption(int metricBasedWatchListId,
            string name, string value)
        {
            return new MetricBasedWatchListOption
            {
                MetricBasedWatchListId = metricBasedWatchListId,
                Name = name,
                Value = value
            };
        }

        /// <summary>
        /// Updates the watch list.
        /// </summary>
        /// <param name="watchListId">The watch list identifier.</param>
        /// <param name="watchListName">Name of the watch list.</param>
        /// <param name="watchListDescription">The watch list description.</param>
        /// <param name="isWatchListShared">if set to <c>true</c> the watch list is shared.</param>
        /// <param name="watchListSelectedOptions">The watch list selected options.</param>
        /// <param name="pageDemographic">The page demographic.</param>
        /// <param name="pageLevel">The page level.</param>
        /// <param name="pageGrade">The page grade.</param>
        protected virtual void UpdateWatchList(int watchListId, string watchListName, string watchListDescription, bool isWatchListShared,
            List<NameValuesType> watchListSelectedOptions, string pageDemographic = null, string pageLevel = null, string pageGrade = null)
        {
            var watchList = MetricBasedWatchListRepository.GetAll()
                .Where(data => data.MetricBasedWatchListId == watchListId)
                .Select(data => data).FirstOrDefault();

            if (watchList == null)
                return;

            TransactionScope scope = null;

            try
            {
                using (scope = new TransactionScope())
                {
                    // watch list name has changed
                    if (watchList.WatchListName != watchListName || watchList.WatchListDescription != watchListDescription || watchList.IsWatchListShared != isWatchListShared)
                    {
                        watchList.WatchListName = watchListName;
                        watchList.WatchListDescription = watchListDescription;
                        watchList.IsWatchListShared = isWatchListShared;
                        MetricBasedWatchListRepository.Save(watchList);
                    }

                    MetricBasedWatchListSelectionOptionRepository.Delete(data => data.MetricBasedWatchListId == watchListId);

                    var selectedOptions = new List<MetricBasedWatchListSelectedOption>();
                    foreach (var watchListSelectedOption in watchListSelectedOptions)
                    {
                        selectedOptions.AddRange(watchListSelectedOption.Values.Select(selectedOption => new MetricBasedWatchListSelectedOption
                        {
                            MetricBasedWatchListId = watchListId,
                            Name = watchListSelectedOption.Name,
                            Value = selectedOption
                        }));
                    }

                    foreach (var metricBasedWatchListSelectedOption in selectedOptions)
                    {
                        MetricBasedWatchListSelectionOptionRepository.Save(metricBasedWatchListSelectedOption);
                    }

                    if (!string.IsNullOrWhiteSpace(pageDemographic))
                    {
                        var currentDemographic =
                            MetricBasedWatchListOptionRepository.GetAll()
                                .SingleOrDefault(data => data.MetricBasedWatchListId == watchListId && data.Name == "PageDemographic");

                        if (currentDemographic != null && currentDemographic.Value != pageDemographic)
                        {
                            currentDemographic.Value = pageDemographic;
                            MetricBasedWatchListOptionRepository.Save(currentDemographic);
                        }
                        else if (currentDemographic == null)
                        {
                            // this condition should really never be true but
                            // putting it just in case
                            MetricBasedWatchListOptionRepository.Save(CreateMetricBasedWatchListOption(watchListId, "PageDemographic", pageDemographic));
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(pageLevel))
                    {
                        var currentLevel =
                            MetricBasedWatchListOptionRepository.GetAll()
                                .SingleOrDefault(
                                    data => data.MetricBasedWatchListId == watchListId && data.Name == "PageSchoolCategory");

                        if (currentLevel != null && currentLevel.Value != pageLevel)
                        {
                            currentLevel.Value = pageLevel;
                            MetricBasedWatchListOptionRepository.Save(currentLevel);
                        }
                        else if (currentLevel == null)
                        {
                            // this condition should really never be true but
                            // putting it just in case
                            MetricBasedWatchListOptionRepository.Save(CreateMetricBasedWatchListOption(watchListId, "PageSchoolCategory", pageLevel));
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(pageGrade))
                    {
                        var currentGrade = MetricBasedWatchListOptionRepository.GetAll()
                            .SingleOrDefault(
                                data => data.MetricBasedWatchListId == watchListId && data.Name == "PageGrade");

                        if (currentGrade != null && currentGrade.Value != pageGrade)
                        {
                            currentGrade.Value = pageGrade;
                            MetricBasedWatchListOptionRepository.Save(currentGrade);
                        }
                        else if (currentGrade == null)
                        {
                            MetricBasedWatchListOptionRepository.Save(CreateMetricBasedWatchListOption(watchListId, "PageGrade", pageGrade));
                        }
                    }

                    scope.Complete();
                }
            }
            catch
            {
                if (scope != null)
                    scope.Dispose();
            }
        }

        /// <summary>
        /// Deletes the watch list.
        /// </summary>
        /// <param name="watchListId">The watch list identifier.</param>
        protected virtual void DeleteWatchList(int watchListId)
        {
            var watchList = MetricBasedWatchListRepository.GetAll()
                .Where(data => data.MetricBasedWatchListId == watchListId)
                .Select(data => data).SingleOrDefault();

            if (watchList == null)
                return;

            TransactionScope scope = null;

            try
            {
                using (scope = new TransactionScope())
                {
                    // first delete the selected options
                    MetricBasedWatchListSelectionOptionRepository.Delete(data => data.MetricBasedWatchListId == watchListId);
                    // next delete the watch list options
                    MetricBasedWatchListOptionRepository.Delete(data => data.MetricBasedWatchListId == watchListId);
                    // next delete the watch list
                    MetricBasedWatchListRepository.Delete(data => data.MetricBasedWatchListId == watchListId);
                    // lastly commit the transaction
                    scope.Complete();
                }
            }
            catch
            {
                // if there is a database failure rollback the transaction
                if (scope != null)
                    scope.Dispose();
            }
        }
    }
}
