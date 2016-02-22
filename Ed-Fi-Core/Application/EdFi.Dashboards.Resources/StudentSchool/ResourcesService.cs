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

namespace EdFi.Dashboards.Resources.StudentSchool
{
    public class ResourcesRequest
    {
        public long StudentUSI { get; set; }
        public int SchoolId { get; set; }
        public string ListContext { get; set; }
        public MetricMetadataNode OverviewNode { get; set; }

        public static ResourcesRequest Create(long studentUSI, int schoolId, string listContext, MetricMetadataNode overviewNode)
        {
            return new ResourcesRequest {
                StudentUSI = studentUSI,
                SchoolId = schoolId,
                ListContext = listContext,
                OverviewNode = overviewNode
            };
        }
    }

    public interface IResourcesService : IService<ResourcesRequest, IEnumerable<ResourceModel>> { }

    public class ResourcesService : IResourcesService
    {
        private readonly IStudentSchoolAreaLinks studentSchoolLinks;
        public IPluginResourcesProvider PluginResourcesProvider { get; set; }

        public ResourcesService(IStudentSchoolAreaLinks studentSchoolLinks)
        {
            this.studentSchoolLinks = studentSchoolLinks;
        }

        [NoCache]
        [AuthenticationIgnore("Everyone should be able to get list of resources")]
        public virtual IEnumerable<ResourceModel> Get(ResourcesRequest request)
        {
            var response = new List<ResourceModel>
                           {
                               new ResourceModel
                                   {
                                       Text = "Student Information",
                                       Url = studentSchoolLinks.Information(request.SchoolId, request.StudentUSI).AppendParameters("listContext=" + request.ListContext),
                                       Style = "width: 256px;"
                                   },
                               new MetricResourceModel
                                   {
                                       Text = "Academic Dashboard",
                                       Url = studentSchoolLinks.Overview(request.SchoolId, request.StudentUSI).AppendParameters("listContext=" + request.ListContext),
                                       MetricVariantId = request.OverviewNode.MetricVariantId,
                                       ChildItems = GetOverviewMenu(request.SchoolId, request.StudentUSI, request.OverviewNode, request.ListContext).ToList(),
                                       Style = "width: 256px;"
                                   },
                               new ResourceModel
                                   {
                                       Text = "Transcript",
                                       Url = studentSchoolLinks.AcademicProfile(request.SchoolId, request.StudentUSI).AppendParameters("listContext=" + request.ListContext),
                                       Enabled = true,
                                       Style = "width: 256px;"
                                   },
                           };

            response.AddRange(PluginResourcesProvider.Get("StudentSchool"));

            return response;
        }

        private IEnumerable<MetricResourceModel> GetOverviewMenu(int schoolId, long studentUSI, MetricMetadataNode overviewNode, string listContext)
        {
            var res = new List<MetricResourceModel>
                          {
                              new MetricResourceModel
                                {
                                    Text = FormatMenuItemText(overviewNode.DisplayName),
                                    Url = studentSchoolLinks.Overview(schoolId, studentUSI).AppendParameters("listContext=" + listContext),
                                    MetricVariantId = overviewNode.MetricVariantId,
                                    Enabled = overviewNode.Enabled
                                }
                          };

            res.AddRange(overviewNode.Children.Select(x => new MetricResourceModel
            {
                Text = FormatMenuItemText(x.DisplayName),
                Url = studentSchoolLinks.Metrics(schoolId, studentUSI, x.MetricVariantId).AppendParameters("listContext=" + listContext),
                MetricVariantId = x.MetricVariantId,
                Enabled = x.Enabled
            }));

            return res;
        }

        private static string FormatMenuItemText(string text)
        {
            if (text.LastIndexOf(" ", System.StringComparison.Ordinal) != -1)
                text = text.Substring(0, text.LastIndexOf(" ", System.StringComparison.Ordinal)) + "<br />" + text.Substring(text.LastIndexOf(" ", System.StringComparison.Ordinal));

            if (text.Trim() == "College and Career<br /> Readiness")
                text = "College/Career<br /> Readiness";

            return text;
        }
    }
}
