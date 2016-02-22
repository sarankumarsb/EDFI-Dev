// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System.Web.Mvc;
using EdFi.Dashboards.Common;
using EdFi.Dashboards.Presentation.Architecture.Mvc.ActionResults;
using EdFi.Dashboards.Resources;
using EdFi.Dashboards.Resources.Models.Common;

namespace EdFi.Dashboards.Presentation.Architecture.Mvc.Controllers
{
    public class ImageController<TRequest, TResponse> : Controller
    {
        /// <summary>
        /// Gets or sets the service to be called by the controller.  Intended for internal use only.
        /// </summary>
        /// <remarks>This service is injected using property injection rather than constructor injection
        /// to optimize for the experience of deriving another controller from this class.  With constructor
        /// injection, the developer would be forced to declare the service type as a constructor parameter
        /// and pass it into a base</remarks>
        public IService<TRequest, TResponse> Service { get; set; }

        public virtual ImageResult Get(TRequest request, int localEducationAgencyId)
        {
            // TODO: Deferred - localEducationAgencyId can be dropped after drilldowns are no longer using WebForms.
            // localEducationAgencyId is here to force model binding to populate it in context so that it 
            // can be provided to the metric Action urls so that the context is available in order to 
            // use the correct database connection (for multitenancy with multiple databases).  Otherwise,
            // when the drilldown is initiated on a WebForms URL, there is no local education agency context
            // provided, and the only way to get it would be to go to the database, which itself also needs
            // the local education agency context in order to select the correct connection, and into a loop
            // we go.  Once all website artifacts are using the MVC routing, this parameter could be dropped.

            var response = Service.Get(request);

            var model = response as ImageModel;
            return new ImageResult(model);
        }
    }
}
