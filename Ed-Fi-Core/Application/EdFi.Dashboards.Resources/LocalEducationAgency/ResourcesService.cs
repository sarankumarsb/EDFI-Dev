// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System.Collections.Generic;
using System.Linq;
using EdFi.Dashboards.Common;
using EdFi.Dashboards.Infrastructure.Implementations.Caching;
using EdFi.Dashboards.Metric.Resources.Models;
using EdFi.Dashboards.Resource.Models.Common;
using EdFi.Dashboards.Resources.Navigation;
using EdFi.Dashboards.Resources.Plugins;
using EdFi.Dashboards.Resources.Navigation.Support;
using EdFi.Dashboards.Resources.Security.Common;
using EdFi.Dashboards.SecurityTokenService.Authentication;

namespace EdFi.Dashboards.Resources.LocalEducationAgency
{
    public class ResourcesRequest
    {
        public int LocalEducationAgencyId { get; set; }
        public string ListContext { get; set; }
        public string Filters { get; set; }
        public MetricMetadataNode OverviewNode { get; set; }
        public MetricMetadataNode OperationalDashboardNode { get; set; }

        public static ResourcesRequest Create(int localEducationAgencyId, string listContext, string filters, MetricMetadataNode overviewNode, MetricMetadataNode operationalDashboardNode)
        {
            return new ResourcesRequest
            {
                LocalEducationAgencyId = localEducationAgencyId,
                ListContext = listContext,
                Filters = filters,
                OverviewNode = overviewNode,
                OperationalDashboardNode = operationalDashboardNode,
            };
        }
    }

    public interface IResourcesService : IService<ResourcesRequest, IEnumerable<ResourceModel>> { }

    public class ResourcesService : IResourcesService
    {
        public ILocalEducationAgencyAreaLinks LocalEducationAgencyLinks { get; set; }
        public ISearchAreaLinks SearchAreaLinks { get; set; }
        public ICurrentUserClaimInterrogator CurrentUserClaimInterrogator { get; set; }
        public IPluginResourcesProvider PluginResourcesProvider { get; set; }

        [NoCache]
        [AuthenticationIgnore("Everyone should be able to get list of resources")]
        public virtual IEnumerable<ResourceModel> Get(ResourcesRequest request)
        {
            var additionalParameter = new[]
            {
                new KeyValuePair<string, string>("listContext", request.ListContext),
                new KeyValuePair<string, string>("filters", request.Filters)
            };

            var response = new List<ResourceModel>
            {
                new ResourceModel
                {
                    Text = "District Information",
                    Url = LocalEducationAgencyLinks.Information(request.LocalEducationAgencyId),
                    ChildItems = GetInformationMenuItems(request.LocalEducationAgencyId, additionalParameter)
                },
                new MetricResourceModel
                {
                    Text = "Academic Dashboard",
                    Url = LocalEducationAgencyLinks.Overview(request.LocalEducationAgencyId),
                    MetricVariantId = request.OverviewNode.MetricVariantId,
                    ChildItems =
                        GetOverviewMenu(request.LocalEducationAgencyId, request.OverviewNode, additionalParameter).
                            Cast<ResourceModel>().ToList()
                },
                new MetricResourceModel
                {
                    Text = "Operational Dashboard",
                    Url = LocalEducationAgencyLinks.OperationalDashboard(request.LocalEducationAgencyId),
                    MetricVariantId = request.OperationalDashboardNode.MetricVariantId,
                    ChildItems = new List<ResourceModel>
                    {
                        new ResourceModel
                        {
                            Text = "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;",
                            Url = string.Empty,
                            Display = true,
                        },
                        new ResourceModel
                        {
                            Text = "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;",
                            Url = string.Empty,
                            Display = true,
                        },
                        new MetricResourceModel
                        {
                            Text = "Staff",
                            Url =
                                LocalEducationAgencyLinks.
                                    OperationalDashboard(
                                        request.LocalEducationAgencyId),
                            MetricVariantId = 1675,
                            Display = true,
                        },
                        new ResourceModel
                        {
                            Text = "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;",
                            Url = string.Empty,
                            Display = true,
                        },
                        new ResourceModel
                        {
                            Text = "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;",
                            Url = string.Empty,
                            Display = true,
                        },
                    }
                },
                new MetricResourceModel
                {
                    Text = "Goal Planning",
                    Url =
                        LocalEducationAgencyLinks.GoalPlanning(request.LocalEducationAgencyId,
                            request.OverviewNode.Children.
                                First().MetricVariantId),
                    Enabled =
                        CurrentUserClaimInterrogator.HasClaimWithinEducationOrganizationHierarchy
                            (EdFiClaimTypes.ManageGoals, request.LocalEducationAgencyId),
                    Display = true,
                    ChildItems =
                        GetGoalPlanningMenu(request.LocalEducationAgencyId, request.OverviewNode, additionalParameter)
                            .Cast<ResourceModel>().ToList()
                },
            };

            response.AddRange(PluginResourcesProvider.Get("LocalEducationAgency"));

            return response;
        }

        private IEnumerable<ResourceModel> GetInformationMenuItems(int localEducationAgencyId, params KeyValuePair<string, string>[] additionalParameters)
        {
            var staffName = !string.IsNullOrWhiteSpace(UserInformation.Current.FullName) ? UserInformation.Current.FullName : "Administrator";
            var studentListsUrl = LocalEducationAgencyLinks.StudentList(localEducationAgencyId, UserInformation.Current.StaffUSI, staffName);
            var flag = CurrentUserClaimInterrogator.HasClaimForLocalEducationAgencyWithinEducationOrganizationHierarchy(EdFiClaimTypes.ViewAllMetrics, localEducationAgencyId);

            return new List<ResourceModel>
            {
                new ResourceModel
                {
                    Text = "District Information",
                    Url =
                        LocalEducationAgencyLinks.Information(localEducationAgencyId).AppendParameters(additionalParameters),
                    Display = true,
                },
                new ResourceModel
                {
                    Text = "School List",
                    Url =
                        LocalEducationAgencyLinks.SchoolCategoryList(localEducationAgencyId)
                            .AppendParameters(additionalParameters),
                    Enabled = flag,
                    Display = true,
                },
                new ResourceModel
                {
                    Text = "Students by Level",
                    Url =
                        LocalEducationAgencyLinks.StudentSchoolCategoryList(localEducationAgencyId)
                            .AppendParameters(additionalParameters),
                    Display = true,
                    RequireExactMatch = false,
                    Enabled = (CurrentUserClaimInterrogator.HasClaimForLocalEducationAgencyWithinEducationOrganizationHierarchy(EdFiClaimTypes.ViewMyStudents, localEducationAgencyId) ||
                        CurrentUserClaimInterrogator.HasClaimForLocalEducationAgencyWithinEducationOrganizationHierarchy(EdFiClaimTypes.ViewAllStudents, localEducationAgencyId))
                },
                new ResourceModel
                {
                    Text = "Students by Demographic",
                    Url =
                        LocalEducationAgencyLinks.StudentDemographicList(localEducationAgencyId)
                            .AppendParameters(additionalParameters),
                    Display = true,
                    Enabled = flag,
                    RequireExactMatch = false
                },
                new ResourceModel
                {
                    Text = "My Student Lists",
                    Url = studentListsUrl.AppendParameters(additionalParameters),
                    Display = true,
                    Enabled = (
                        CurrentUserClaimInterrogator.HasClaimForLocalEducationAgencyWithinEducationOrganizationHierarchy
                            (EdFiClaimTypes.ViewMyStudents, localEducationAgencyId) ||
                        CurrentUserClaimInterrogator.HasClaimForLocalEducationAgencyWithinEducationOrganizationHierarchy
                            (EdFiClaimTypes.ViewAllStudents, localEducationAgencyId))
                              && !string.IsNullOrWhiteSpace(studentListsUrl),
                    RequireExactMatch = false
                }
            };
        }

        private IEnumerable<MetricResourceModel> GetOverviewMenu(int localEducationAgencyId,
            MetricMetadataNode overviewNode, params KeyValuePair<string, string>[] additionalParameters)
        {
            var res = new List<MetricResourceModel>
            {
                new MetricResourceModel
                {
                    Text = FormatMenuItemText(overviewNode.DisplayName),
                    Url = LocalEducationAgencyLinks.Overview(localEducationAgencyId),
                    MetricVariantId = overviewNode.MetricVariantId,
                    Enabled = overviewNode.Enabled
                }
            };

            res.AddRange(overviewNode.Children.Select(x => new MetricResourceModel
            {
                Text = FormatMenuItemText(x.DisplayName),
                Url = LocalEducationAgencyLinks.Metrics(localEducationAgencyId, x.MetricVariantId)
                    .AppendParameters(additionalParameters),
                MetricVariantId = x.MetricVariantId,
                Enabled = x.Enabled
            }));

            return res;
        }

        private IEnumerable<MetricResourceModel> GetGoalPlanningMenu(int localEducationAgencyId,
            MetricMetadataNode overviewNode, params KeyValuePair<string, string>[] additionalParameters)
        {
            var res = new List<MetricResourceModel>();

            res.AddRange(overviewNode.Children.Select(x => new MetricResourceModel
            {
                Text = FormatMenuItemText(x.DisplayName),
                Url =
                    LocalEducationAgencyLinks.GoalPlanning(localEducationAgencyId, x.MetricVariantId)
                        .AppendParameters(additionalParameters),
                MetricVariantId = x.MetricVariantId,
                Enabled = x.Enabled
            }));
            res.Add(new MetricResourceModel
            {
                Text = "Staff",
                Url =
                    LocalEducationAgencyLinks.GoalPlanning(localEducationAgencyId, 1295)
                        .AppendParameters(additionalParameters),
                MetricVariantId = 1295,
                Enabled = true
            });

            return res;
        }

        private static string FormatMenuItemText(string text)
        {
            if (text.LastIndexOf(" ", System.StringComparison.Ordinal) != -1)
                text = text.Substring(0, text.LastIndexOf(" ", System.StringComparison.Ordinal)) + "<br />" + text.Substring(text.LastIndexOf(" ", System.StringComparison.Ordinal));

            return text;
        }
    }
}
