using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EdFi.Dashboards.Application.Data.Entities;
using EdFi.Dashboards.Data.Repository;
using EdFi.Dashboards.Common;
using EdFi.Dashboards.Resources.Models.LocalEducationAgency;
using EdFi.Dashboards.Resources.Navigation;

namespace EdFi.Dashboards.Resources.Common
{
    /// <summary>
    /// Contains a method that provides a link based upon the location the link
    /// is generated for.
    /// </summary>
    public interface IWatchListSearchLinkProvider
    {
        /// <summary>
        /// Generates the return link based upon the location the link is for.
        /// </summary>
        /// <param name="request">The request containing the location data.</param>
        /// <returns>A string representing the return link.</returns>
        string GenerateReturnLink(MetricsBasedWatchListSearchRequest request);
    }

    /// <summary>
    /// Provides a link based upon the desired location.
    /// </summary>
    public class WatchListSearchLinkProvider : IWatchListSearchLinkProvider
    {
        protected readonly IStaffAreaLinks StaffAreaLinks;
        protected readonly ILocalEducationAgencyAreaLinks LocalEducationAgencyAreaLinks;
        protected readonly IWatchListLinkProvider WatchListLinkProvider;
        protected readonly IRepository<MetricBasedWatchListOption> MetricBasedWatchListOptionRepository;
        protected readonly IService<MetricsBasedWatchListMenuRequest, List<StudentListMenuModel>> MetricsBasedWatchListMenuService;

        public WatchListSearchLinkProvider(IStaffAreaLinks staffAreaLinks,
            ILocalEducationAgencyAreaLinks localEducationAgencyAreaLinks,
            IWatchListLinkProvider watchListLinkProvider,
            IRepository<MetricBasedWatchListOption> metricBasedWatchListOptionRepository,
            IService<MetricsBasedWatchListMenuRequest, List<StudentListMenuModel>> metricsBasedWatchListMenuService)
        {
            StaffAreaLinks = staffAreaLinks;
            LocalEducationAgencyAreaLinks = localEducationAgencyAreaLinks;
            WatchListLinkProvider = watchListLinkProvider;
            MetricBasedWatchListOptionRepository = metricBasedWatchListOptionRepository;
            MetricsBasedWatchListMenuService = metricsBasedWatchListMenuService;
        }

        /// <summary>
        /// Generates the return link based upon the location the link is for.
        /// </summary>
        /// <param name="request">The request containing the location data.</param>
        /// <returns>A string representing the return link.</returns>
        public string GenerateReturnLink(MetricsBasedWatchListSearchRequest request)
        {
            var returnUrl = string.Empty;

            var pageOptions = MetricBasedWatchListOptionRepository.GetAll()
                .Where(option => option.MetricBasedWatchListId == request.MetricsBasedWatchListId)
                .ToList();

            var schoolIdOption = pageOptions.SingleOrDefault(data => data.Name == "SchoolId");

            var leaIdOption = pageOptions.SingleOrDefault(data => data.Name == "LocalEducationAgencyId");

            if (schoolIdOption != null)
            {
                var watchListStaffUSI = pageOptions.Any(data => data.Name == "StaffUSI")
                    ? long.Parse(pageOptions.Single(data => data.Name == "StaffUSI").Value)
                    : request.StaffUSI;

                var resourceName =
                    pageOptions.SingleOrDefault(data => data.Name == "PageController");
                var demographic =
                    pageOptions.SingleOrDefault(data => data.Name == "PageDemographic");
                var schoolCategory =
                    pageOptions.SingleOrDefault(data => data.Name == "PageSchoolCategory");
                var grade =
                    pageOptions.SingleOrDefault(data => data.Name == "PageGrade");

                var watchListLink = new WatchListLinkRequest
                {
                    SchoolId = int.Parse(schoolIdOption.Value),
                    StaffUSI = watchListStaffUSI,
                    MetricBasedWatchListId = request.MetricsBasedWatchListId,
                    ResourceName = resourceName != null ? resourceName.Value : string.Empty,
                    Demographic = demographic != null ? demographic.Value : string.Empty,
                    SchoolCategory = schoolCategory != null ? schoolCategory.Value : string.Empty,
                    Grade = grade != null ? grade.Value : string.Empty
                };

                returnUrl = WatchListLinkProvider.GenerateLink(watchListLink);
            }
            else if (leaIdOption != null)
            {
                return MetricsBasedWatchListMenuService.Get(new MetricsBasedWatchListMenuRequest
                {
                    StaffUSI = request.StaffUSI.GetValueOrDefault(),
                    LocalEducationAgencyId = int.Parse(leaIdOption.Value),
                }).Single(mbwl => mbwl.SectionId == request.MetricsBasedWatchListId).Href;
            }

            return returnUrl;
        }
    }
}
