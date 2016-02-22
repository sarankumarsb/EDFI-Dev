using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using Castle.Windsor.Installer;
using EdFi.Dashboards.Application.Data.Entities;
using EdFi.Dashboards.Common;
using EdFi.Dashboards.Data.Repository;
using EdFi.Dashboards.Infrastructure.Implementations.Caching;
using EdFi.Dashboards.Resources.Models.Common;
using EdFi.Dashboards.Resources.Models.LocalEducationAgency;
using EdFi.Dashboards.Resources.Security.Common;
using EdFi.Dashboards.SecurityTokenService.Authentication;

namespace EdFi.Dashboards.Resources.Common
{
    public class MetricsBasedWatchListMenuRequest
    {
        /// <summary>
        /// Gets or sets the staff usi.
        /// </summary>
        /// <value>
        /// The staff usi.
        /// </value>
        public long StaffUSI { get; set; }

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

    public interface IMetricsBasedWatchListMenuService : IService<MetricsBasedWatchListMenuRequest, List<StudentListMenuModel>> { }

    public class MetricsBasedWatchListMenuService : IMetricsBasedWatchListMenuService
    {
        protected readonly IRepository<MetricBasedWatchList> MetricBasedWatchListRepository;
        protected readonly IRepository<MetricBasedWatchListOption> MetricBasedWatchListOptionRepository;
        protected readonly IWatchListLinkProvider WatchListLinkProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="MetricsBasedWatchListMenuService" /> class.
        /// </summary>
        /// <param name="metricBasedWatchListRepository">The metric based watch list repository.</param>
        /// <param name="metricBasedWatchListOptionRepository">The metric based watch list option repository.</param>
        /// <param name="watchListLinkProvider">The watch list link provider.</param>
        public MetricsBasedWatchListMenuService(
            IRepository<MetricBasedWatchList> metricBasedWatchListRepository,
            IRepository<MetricBasedWatchListOption> metricBasedWatchListOptionRepository,
            IWatchListLinkProvider watchListLinkProvider)
        {
            MetricBasedWatchListRepository = metricBasedWatchListRepository;
            MetricBasedWatchListOptionRepository = metricBasedWatchListOptionRepository;
            WatchListLinkProvider = watchListLinkProvider;
        }

        [NoCache]
        [CanBeAuthorizedBy(EdFiClaimTypes.ViewAllStudents, EdFiClaimTypes.ViewMyStudents)]
        public List<StudentListMenuModel> Get(MetricsBasedWatchListMenuRequest request)
        {
            var staffUSI = UserInformation.Current.StaffUSI;
            var educationOrganizationId = request.SchoolId.HasUsableValue()
                ? request.SchoolId.Value
                : request.LocalEducationAgencyId.Value;
            var menuItems = new List<StudentListMenuModel>();

            var watchLists = (from w in MetricBasedWatchListRepository.GetAll()
                join wo in MetricBasedWatchListOptionRepository.GetAll() on w.MetricBasedWatchListId equals
                    wo.MetricBasedWatchListId
                where
                    (w.StaffUSI == staffUSI || wo.Name == "StaffUSI" && wo.Value == request.StaffUSI.ToString(CultureInfo.InvariantCulture)) &&
                    w.EducationOrganizationId == educationOrganizationId
                select w).Distinct().ToList();

            if (!watchLists.Any())
                return menuItems;

            foreach (var watchListItem in watchLists)
            {
                var watchListOptions = MetricBasedWatchListOptionRepository.GetAll()
                    .Where(data => data.MetricBasedWatchListId == watchListItem.MetricBasedWatchListId).ToList();

                var pageControllerOption = watchListOptions.SingleOrDefault(data => data.Name == "PageController");
                var controller = pageControllerOption != null ? pageControllerOption.Value : string.Empty;

                if (watchListOptions.Any(data => data.Name == "PageDemographic"))
                {
                    var pageDemographicOption = watchListOptions.SingleOrDefault(data => data.Name == "PageDemographic");
                    var demographic = pageDemographicOption != null ? pageDemographicOption.Value : string.Empty;

                    // if demographic or controller doesn't have a value then something is wrong
                    if (string.IsNullOrWhiteSpace(demographic) || string.IsNullOrWhiteSpace(controller))
                        continue;

                    var demographicLinkRequest = new WatchListLinkRequest
                    {
                        StaffUSI = request.StaffUSI,
                        SchoolId = request.SchoolId,
                        LocalEducationAgencyId = request.LocalEducationAgencyId,
                        MetricBasedWatchListId = watchListItem.MetricBasedWatchListId,
                        Demographic = demographic,
                        ResourceName = controller
                    };

                    menuItems.Add(new StudentListMenuModel
                    {
                        SectionId = watchListItem.MetricBasedWatchListId,
                        Description = watchListItem.WatchListName,
                        ListType = StudentListType.MetricsBasedWatchList,
                        Href = WatchListLinkProvider.GenerateLink(demographicLinkRequest),
						MenuType = "PageDemographic"
                    });
                }
                else if (watchListOptions.Any(data => data.Name == "PageSchoolCategory"))
                {
                    var pageSchoolCategoryOption =
                        watchListOptions.SingleOrDefault(data => data.Name == "PageSchoolCategory");
                    var schoolCategory = pageSchoolCategoryOption != null
                        ? pageSchoolCategoryOption.Value
                        : string.Empty;

                    if (string.IsNullOrWhiteSpace(schoolCategory) || string.IsNullOrWhiteSpace(controller))
                        continue;

                    var schoolCategoryLinkRequest = new WatchListLinkRequest
                    {
                        StaffUSI = request.StaffUSI,
                        SchoolId = request.SchoolId,
                        LocalEducationAgencyId = request.LocalEducationAgencyId,
                        MetricBasedWatchListId = watchListItem.MetricBasedWatchListId,
                        SchoolCategory = schoolCategory,
                        ResourceName = controller
                    };

                    menuItems.Add(new StudentListMenuModel
                    {
                        SectionId = watchListItem.MetricBasedWatchListId,
                        Description = watchListItem.WatchListName,
                        ListType = StudentListType.MetricsBasedWatchList,
						Href = WatchListLinkProvider.GenerateLink(schoolCategoryLinkRequest),
						MenuType = "PageSchoolCategory"
                    });
                }
                else if (watchListOptions.Any(data => data.Name == "PageGrade"))
                {
                    var pageGradeOption = watchListOptions.SingleOrDefault(data => data.Name == "PageGrade");
                    var grade = pageGradeOption != null ? pageGradeOption.Value : string.Empty;

                    if (string.IsNullOrWhiteSpace(grade) || string.IsNullOrWhiteSpace(controller))
                        continue;

                    var gradeCategoryLinkRequest = new WatchListLinkRequest
                    {
                        SchoolId = request.SchoolId,
                        MetricBasedWatchListId = watchListItem.MetricBasedWatchListId,
                        Grade = grade,
                        ResourceName = controller
                    };

                    menuItems.Add(new StudentListMenuModel
                    {
                        SectionId = watchListItem.MetricBasedWatchListId,
                        Description = watchListItem.WatchListName,
                        ListType = StudentListType.MetricsBasedWatchList,
						Href = WatchListLinkProvider.GenerateLink(gradeCategoryLinkRequest),
						MenuType = "PageGrade"
                    });
                }
            }

            return menuItems;
        }
    }
}
