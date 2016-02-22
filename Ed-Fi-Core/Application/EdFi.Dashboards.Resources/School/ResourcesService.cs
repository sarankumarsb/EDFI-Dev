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
using EdFi.Dashboards.Resources.Navigation.Support;
using EdFi.Dashboards.Resources.Plugins;
using EdFi.Dashboards.Resources.Security.Common;
using EdFi.Dashboards.SecurityTokenService.Authentication;

namespace EdFi.Dashboards.Resources.School
{
    public class ResourcesRequest
    {
        public int SchoolId { get; set; }
        public string ListContext { get; set; }
        public MetricMetadataNode OverviewNode { get; set; }
        public MetricMetadataNode OperationalDashboardNode { get; set; }

        public static ResourcesRequest Create(int schoolId, string listContext, MetricMetadataNode overviewNode, MetricMetadataNode operationalDashboardNode)
        {
            return new ResourcesRequest {
                SchoolId = schoolId,
                ListContext = listContext,
                OverviewNode = overviewNode,
                OperationalDashboardNode = operationalDashboardNode,
            };
        }
    }

    public interface IResourcesService : IService<ResourcesRequest, IEnumerable<ResourceModel>> { }

    public class ResourcesService : IResourcesService
    {
        public ISchoolAreaLinks SchoolLinks { get; set; }
        public IStaffAreaLinks StaffLinks { get; set; }
        public ICurrentUserClaimInterrogator CurrentUserClaimInterrogator { get; set; }
        public IPluginResourcesProvider PluginResourcesProvider { get; set; }

        [NoCache]
        [AuthenticationIgnore("Everyone should be able to get list of resources")]
        public virtual IEnumerable<ResourceModel> Get(ResourcesRequest request)
        {
            var staffUSI = UserInformation.Current.StaffUSI;
            var teacherListEnabled = CurrentUserClaimInterrogator.HasClaimWithinEducationOrganizationHierarchy(EdFiClaimTypes.ViewAllTeachers, request.SchoolId);
            var studentsEnabled = ((CurrentUserClaimInterrogator.HasClaimWithinEducationOrganizationHierarchy(EdFiClaimTypes.ViewAllStudents, request.SchoolId))
                                    || (CurrentUserClaimInterrogator.HasClaimWithinEducationOrganizationHierarchy(EdFiClaimTypes.ViewMyStudents, request.SchoolId)));
            var goalPlanningEnabled = CurrentUserClaimInterrogator.HasClaimWithinEducationOrganizationHierarchy(EdFiClaimTypes.ManageGoals, request.SchoolId);
            var studentListnavigateUrl = StaffLinks.Default(request.SchoolId, staffUSI, UserInformation.Current.FullName);

            var response = new List<ResourceModel>{
                                    new ResourceModel{
                                            Text = "School Information",
                                            Url = SchoolLinks.Information(request.SchoolId).AppendParameters("listContext=" + request.ListContext),
                                            ChildItems = new List<ResourceModel>
                                                            {
                                                                new ResourceModel
                                                                    {
                                                                        Text = "School Information",
                                                                        Url = SchoolLinks.Information(request.SchoolId).AppendParameters("listContext=" + request.ListContext),
                                                                        Display = true,
                                                                    },
                                                                new ResourceModel
                                                                    {
                                                                        Text = "Staff List",
                                                                        Url = SchoolLinks.Staff(request.SchoolId).AppendParameters("listContext=" + request.ListContext),
                                                                        Display = true,
                                                                    },
                                                                new ResourceModel
                                                                    {
                                                                        Text = "Teacher List",
                                                                        Url = SchoolLinks.Teachers(request.SchoolId).AppendParameters("listContext=" + request.ListContext),
                                                                        Display = true,
                                                                        Enabled = teacherListEnabled
                                                                    },
                                                                new ResourceModel
                                                                    {
                                                                        Text = "Students by Grade",
                                                                        Url = SchoolLinks.StudentGradeList(request.SchoolId).AppendParameters("listContext=" + request.ListContext),
                                                                        Display = true,
                                                                        Enabled = studentsEnabled,
                                                                        RequireExactMatch = false
                                                                    },
                                                                new ResourceModel
                                                                    {
                                                                        Text = "Students by Demographic",
                                                                        Url = SchoolLinks.StudentDemographicList(request.SchoolId).AppendParameters("listContext=" + request.ListContext),
                                                                        Display = true,
                                                                        Enabled = studentsEnabled,
                                                                        RequireExactMatch = false
                                                                    },
                                                                new ResourceModel
                                                                    {
                                                                        Text = "My Student Lists",
                                                                        Url = studentListnavigateUrl,
                                                                        Display = !string.IsNullOrEmpty(studentListnavigateUrl),
                                                                        Enabled = studentsEnabled
                                                                    },
                                                            }
                                        },
                                   new MetricResourceModel
                                        {
                                            Text = "Academic Dashboard",
                                            Url = SchoolLinks.Overview(request.SchoolId).AppendParameters("listContext=" + request.ListContext),
                                            MetricVariantId = request.OverviewNode.MetricVariantId,
                                            ChildItems = GetOverviewMenu(request.SchoolId, request.OverviewNode, request.ListContext).ToList()
                                        },
                                    new MetricResourceModel
                                        {
                                            Text = "Operational Dashboard",
                                            Url = SchoolLinks.OperationalDashboard(request.SchoolId).AppendParameters("listContext=" + request.ListContext),
                                            MetricVariantId = request.OperationalDashboardNode.MetricVariantId,
                                            ChildItems = new List<MetricResourceModel>
                                                            {
                                                                new MetricResourceModel
                                                                   {
                                                                       Text = "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;",
                                                                       Url = string.Empty,
                                                                       Display = true,
                                                                   },
                                                                new MetricResourceModel
                                                                   {
                                                                       Text = "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;",
                                                                       Url = string.Empty,
                                                                       Display = true,
                                                                   },
                                                                new MetricResourceModel
                                                                    {
                                                                        Text = "Staff",
                                                                        Url = SchoolLinks.OperationalDashboard(request.SchoolId).AppendParameters("listContext=" + request.ListContext),
                                                                        MetricVariantId = request.OperationalDashboardNode.MetricVariantId,
                                                                        Display = true,
                                                                    },
                                                                new MetricResourceModel
                                                                   {
                                                                       Text = "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;",
                                                                       Url = string.Empty,
                                                                       Display = true,
                                                                   },
                                                                new MetricResourceModel
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
                                            Url = SchoolLinks.GoalPlanning(request.SchoolId, request.OverviewNode.Children.First().MetricVariantId, null, new {metricVariantId = request.OverviewNode.Children.First().MetricVariantId}),
                                            Display = true,
                                            Enabled = goalPlanningEnabled,
                                            ChildItems = GetGoalPlanningMenu(request.SchoolId, request.OverviewNode, request.OperationalDashboardNode).ToList()
                                        },
                                };

            response.AddRange(PluginResourcesProvider.Get("School"));

            return response;
        }

        private IEnumerable<MetricResourceModel> GetOverviewMenu(int schoolId, MetricMetadataNode overviewNode, string listContext)
        {
            var res = new List<MetricResourceModel>
                          {
                              new MetricResourceModel
                                {
                                    Text = FormatMenuItemText(overviewNode.DisplayName),
                                    Url = SchoolLinks.Overview(schoolId).AppendParameters("listContext=" + listContext),
                                    MetricVariantId = overviewNode.MetricVariantId,
                                    Enabled = overviewNode.Enabled
                                }
                          };

            res.AddRange(overviewNode.Children.Select(x => new MetricResourceModel
            {
                Text = FormatMenuItemText(x.DisplayName),
                Url = SchoolLinks.Metrics(schoolId, x.MetricVariantId).AppendParameters("listContext=" + listContext),
                MetricVariantId = x.MetricVariantId,
                Enabled = x.Enabled
            }));

            return res;
        }

        private IEnumerable<MetricResourceModel> GetGoalPlanningMenu(int schoolId, MetricMetadataNode overviewNode, MetricMetadataNode operationalDashboardNode)
        {
            var res = new List<MetricResourceModel>();

            res.AddRange(overviewNode.Children.Select(x => new MetricResourceModel
            {
                Text = FormatMenuItemText(x.DisplayName),
                Url = SchoolLinks.GoalPlanning(schoolId, x.MetricVariantId),
                MetricVariantId = x.MetricVariantId,
                Enabled = x.Enabled
            }));


            res.Add(new MetricResourceModel
            {
                Text = "Staff",
                Url = SchoolLinks.GoalPlanning(schoolId, operationalDashboardNode.MetricVariantId),
                MetricVariantId = operationalDashboardNode.MetricVariantId,
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
