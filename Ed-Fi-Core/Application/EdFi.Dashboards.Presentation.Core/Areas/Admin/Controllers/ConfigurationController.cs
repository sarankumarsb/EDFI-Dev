// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System.Web.Mvc;
using EdFi.Dashboards.Resources.Admin;
using EdFi.Dashboards.Resources.Models.Admin;

namespace EdFi.Dashboards.Presentation.Core.Areas.Admin.Controllers
{
    public class ConfigurationController : Controller
    {
        private readonly IConfigurationService service;
        public ConfigurationController(IConfigurationService service)
        {
            this.service = service;
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public ViewResult Get(int localEducationAgencyId)
        {
            var model = service.Get(new ConfigurationRequest { LocalEducationAgencyId = localEducationAgencyId });
            return View(model);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ViewResult Get(ConfigurationModel setModel)
        {
            service.Post(setModel);
            return Get(setModel.LocalEducationAgencyId);
        }
    }
}
