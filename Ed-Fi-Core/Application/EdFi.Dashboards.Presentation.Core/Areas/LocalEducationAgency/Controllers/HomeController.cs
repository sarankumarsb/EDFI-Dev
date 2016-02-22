// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System.Web.Mvc;
using EdFi.Dashboards.Core.Providers.Context;
using EdFi.Dashboards.Presentation.Core.Models.Shared;
using EdFi.Dashboards.Presentation.Web.Areas.LocalEducationAgency.Models.Home;
using EdFi.Dashboards.Metric.Resources.Services;
using EdFi.Dashboards.Resources.LocalEducationAgency;
using EdFi.Dashboards.SecurityTokenService.Authentication;

namespace EdFi.Dashboards.Presentation.Core.Areas.LocalEducationAgency.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHomeService service;
        private readonly IMetricMetadataTreeService metricMetadataTreeService;

        public HomeController(IHomeService service, IMetricMetadataTreeService metricMetadataTreeService)
        {
            this.service = service;
            this.metricMetadataTreeService = metricMetadataTreeService;
        }

        private void InitializeMetricMetadataCache()
        {
            metricMetadataTreeService.Get(MetricMetadataTreeRequest.Create());
        }

        [OutputCache(CacheProfile = "EdfiCache")]
        public virtual ActionResult Get(string localEducationAgency, int localEducationAgencyId)
        {
            // Make sure the metric metadata has been initialized
            InitializeMetricMetadataCache();

            var model = new HomeModel
                            {
                                LocalEducationAgencyInformation = service.Get(HomeRequest.Create(localEducationAgencyId)),
                                Feedback = Feedback("#supportLink", true),
                            };

            return View(model);
        }
        
        public FeedbackModel Feedback(string supportLinkControlId, bool allowNameEdit = false)
        {
            var model = new FeedbackModel
            {
                SupportLinkControlId = supportLinkControlId,
                AllowNameEdit = allowNameEdit,
                LocalEducationAgencyId = EdFiDashboardContext.Current.LocalEducationAgencyId.Value
            };

            var identity = UserInformation.Current;

            if (identity != null)
            {
                model.UserName = identity.FullName;
                model.Email =  !string.IsNullOrEmpty(identity.EmailAddress) ? identity.EmailAddress : string.Empty;

                if (!model.AllowNameEdit)
                    model.DisableFeedbackName = "disabled='disabled'";

                if (!model.AllowNameEdit && !string.IsNullOrEmpty(identity.EmailAddress))
                    model.DisableFeedbackEmail = "disabled='disabled'";
            }

            return model;
        }
    }
}
