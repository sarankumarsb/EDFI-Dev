using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EdFi.Dashboards.Resources.Common;
using EdFi.Dashboards.Resources.Models.Common;
using EdFi.Dashboards.Resources.Navigation;

namespace EdFi.Dashboards.Resources.Common
{
    public class WatchListLinkRequest
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
        public long? StaffUSI { get; set; }

        /// <summary>
        /// Gets or sets the page staff usi; this staff USI is used when a
        /// higher level user is viewing the page of a lower level user but
        /// wants to save the watch list against their data.
        /// </summary>
        /// <value>
        /// The page staff usi.
        /// </value>
        public long? PageStaffUSI { get; set; }

        /// <summary>
        /// Gets or sets the metric based watch list identifier.
        /// </summary>
        /// <value>
        /// The metric based watch list identifier.
        /// </value>
        public int? MetricBasedWatchListId { get; set; }

        /// <summary>
        /// Gets or sets the name of the resource.
        /// </summary>
        /// <value>
        /// The name of the resource.
        /// </value>
        public string ResourceName { get; set; }

        /// <summary>
        /// Gets or sets the demographic.
        /// </summary>
        /// <value>
        /// The demographic.
        /// </value>
        public string Demographic { get; set; }

        /// <summary>
        /// Gets or sets the level.
        /// </summary>
        /// <value>
        /// The level.
        /// </value>
        public string SchoolCategory { get; set; }

        /// <summary>
        /// Gets or sets the grade.
        /// </summary>
        /// <value>
        /// The grade.
        /// </value>
        public string Grade { get; set; }
    }

    public interface IWatchListLinkProvider
    {
        /// <summary>
        /// Determines which link generator to use based upon the values passed
        /// to the method.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>A generated link based upon the request.</returns>
        string GenerateLink(WatchListLinkRequest request);
    }

    public class WatchListLinkProvider : IWatchListLinkProvider
    {
        protected readonly IStaffAreaLinks StaffAreaLinks;
        protected readonly ISchoolAreaLinks SchoolAreaLinks;
        protected readonly ILocalEducationAgencyAreaLinks LocalEducationAgencyAreaLinks;

        /// <summary>
        /// Initializes a new instance of the <see cref="WatchListLinkProvider" /> class.
        /// </summary>
        /// <param name="staffAreaLinks">The staff area links.</param>
        /// <param name="schoolAreaLinks">The school area links.</param>
        /// <param name="localEducationAgencyAreaLinks">The local education agency area links.</param>
        public WatchListLinkProvider(
            IStaffAreaLinks staffAreaLinks,
            ISchoolAreaLinks schoolAreaLinks,
            ILocalEducationAgencyAreaLinks localEducationAgencyAreaLinks)
        {
            StaffAreaLinks = staffAreaLinks;
            SchoolAreaLinks = schoolAreaLinks;
            LocalEducationAgencyAreaLinks = localEducationAgencyAreaLinks;
        }

        /// <summary>
        /// Determines which link generator to use based upon the values passed
        /// to the method.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>A Url for a resource.</returns>
        public string GenerateLink(WatchListLinkRequest request)
        {
            var link = string.Empty;

            if (!request.SchoolId.HasUsableValue() && !request.LocalEducationAgencyId.HasUsableValue())
                return link;

            //TODO: Start using this again after the staff USI issue is fixed
            //if (!request.StaffUSI.HasUsableValue())
            //    return link;

            var educationOrganizationId = request.SchoolId.HasUsableValue() ? request.SchoolId.Value : request.LocalEducationAgencyId.Value;

            // this will be the case in a delete situation
            var metricsBasedWatchListId = request.MetricBasedWatchListId.HasUsableValue()
                ? request.MetricBasedWatchListId
                : null;
            var studentListType = metricsBasedWatchListId.HasValue
                ? StudentListType.MetricsBasedWatchList.ToString()
                : null;

            switch (request.ResourceName)
            {
                case "GeneralOverview":
                    var staffUSIToUse = request.PageStaffUSI.HasUsableValue() &&
                                        (request.StaffUSI.GetValueOrDefault() !=
                                         request.PageStaffUSI.GetValueOrDefault())
                        ? request.PageStaffUSI.GetValueOrDefault()
                        : request.StaffUSI.GetValueOrDefault();

                    link = StaffAreaLinks.CustomMetricsBasedWatchList(educationOrganizationId, staffUSIToUse,
                        request.ResourceName, metricsBasedWatchListId, null, studentListType);
                    break;
                case "StudentDemographicList":
                    if (request.SchoolId.HasUsableValue())
                        link = SchoolAreaLinks.CustomMetricsBasedWatchListDemographic(educationOrganizationId,
                            request.ResourceName, request.Demographic, metricsBasedWatchListId, studentListType);
                    else if (request.LocalEducationAgencyId.HasUsableValue())
                        link = LocalEducationAgencyAreaLinks.CustomMetricsBasedWatchListDemographic(educationOrganizationId,
                            request.StaffUSI.GetValueOrDefault(), request.ResourceName, request.Demographic, metricsBasedWatchListId, studentListType);
                    break;
                case "StudentSchoolCategoryList":
                    link = LocalEducationAgencyAreaLinks.CustomMetricsBasedWatchListSchoolCategory(educationOrganizationId,
                            request.StaffUSI.GetValueOrDefault(), request.ResourceName, request.SchoolCategory,
                            metricsBasedWatchListId, studentListType);
                    break;
                case "StudentGradeList":
                    link = SchoolAreaLinks.CustomMetricsBasedWatchListGrade(educationOrganizationId,
                        request.ResourceName, request.Grade, metricsBasedWatchListId, studentListType);
                    break;
                case "MetricsBasedWatchListSearch":
                    if (request.SchoolId.HasUsableValue())
                    {
                        link = SchoolAreaLinks.Resource(request.SchoolId.GetValueOrDefault(),
                            "MetricsBasedWatchListSearch");
                    }
                    else if (request.LocalEducationAgencyId.HasUsableValue())
                    {
                        link = LocalEducationAgencyAreaLinks.Resource(
                            request.LocalEducationAgencyId.GetValueOrDefault(), "MetricsBasedWatchListSearch");
                    }
                    break;
            }

            return link;
        }
    }
}
