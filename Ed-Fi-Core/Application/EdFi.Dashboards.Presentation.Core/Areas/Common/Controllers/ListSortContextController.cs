using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using EdFi.Dashboards.Common.Utilities;
using EdFi.Dashboards.Infrastructure;
using EdFi.Dashboards.Presentation.Core.Models.Shared;
using EdFi.Dashboards.Resources.Application;
using log4net;

namespace EdFi.Dashboards.Presentation.Core.Areas.Common.Controllers
{
    public class ListSortContextController : Controller
    {
        [HttpPost]
        public string Get(string json)
        {
            try
            {
                if (String.IsNullOrWhiteSpace(json))
                    return "true";

                //Deserialize the json in the cookie to c# object
                var jss = new JavaScriptSerializer { MaxJsonLength = 50000000 };
                var model = jss.Deserialize<PreviousNextDataModel>(json);
                var session = IoC.Resolve<ISessionStateProvider>();
                session[model.ListPersistenceUniqueId] = model;
                return "true";
            }
            catch (Exception ex)
            {
                var errorLoggingProvider = IoC.Resolve<IErrorLoggingService>();
                var currentUserProvider = IoC.Resolve<ICurrentUserProvider>();
                var userName = String.Empty;
                if (currentUserProvider.User != null)
                    userName = currentUserProvider.User.Identity.Name;
                ex.Data["JSON"] = json;
                errorLoggingProvider.Post(new ErrorLoggingRequest { Exception = ex, UserName = userName, Request = Request });
                var e = ex;
                return e.ToString();
            }
        }
    }
}
