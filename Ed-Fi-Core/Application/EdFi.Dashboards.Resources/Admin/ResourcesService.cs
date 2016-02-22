// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System.Collections.Generic;
using EdFi.Dashboards.Common;
using EdFi.Dashboards.Infrastructure.Implementations.Caching;
using EdFi.Dashboards.Resource.Models.Common;
using EdFi.Dashboards.Resources.Navigation;
using EdFi.Dashboards.Resources.Plugins;
using EdFi.Dashboards.Resources.Security.Common;

namespace EdFi.Dashboards.Resources.Admin
{
    public class ResourcesRequest
    {
        public int LocalEducationAgencyId { get; set; }

        public static ResourcesRequest Create(int localEducationAgencyId)
        {
            return new ResourcesRequest { LocalEducationAgencyId = localEducationAgencyId };
        }
    }

    public interface IResourcesService : IService<ResourcesRequest, IEnumerable<ResourceModel>> { }

    public class ResourcesService : IResourcesService
    {
        public IAdminAreaLinks AdminAreaLinks { get; set; }
        public IPluginResourcesProvider PluginResourcesProvider { get; set; }

        [NoCache]
        [AuthenticationIgnore("Everyone should be able to get list of resources")]
        public virtual IEnumerable<ResourceModel> Get(ResourcesRequest request)
        {
            var response = new List<ResourceModel>{
                                    new ResourceModel{
                                            Text = "Site Configuration",
                                            Url = AdminAreaLinks.Configuration(request.LocalEducationAgencyId)
                                        },
                                    new ResourceModel
                                        {
                                            Text = "Position Title Claim Sets",
                                            Url = AdminAreaLinks.TitleClaimSet(request.LocalEducationAgencyId),
                                            RequireExactMatch = false
                                        },
                                    new ResourceModel
                                        {
                                            Text = "Metric Settings",
                                            Url = AdminAreaLinks.MetricThreshold(request.LocalEducationAgencyId),
                                            RequireExactMatch = false
                                        },
                                    new ResourceModel
                                        {
                                            Text = "Photo Management",
                                            Url = AdminAreaLinks.PhotoManagement(request.LocalEducationAgencyId),
                                            RequireExactMatch = false
                                        }
                                };

            response.AddRange(PluginResourcesProvider.Get("Admin"));

            return response;
        }
    }
}
