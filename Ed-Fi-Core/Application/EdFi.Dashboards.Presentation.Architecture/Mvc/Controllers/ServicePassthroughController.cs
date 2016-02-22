// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System;
using System.Web.Mvc;
using EdFi.Dashboards.Common;
using EdFi.Dashboards.Presentation.Architecture.Mvc.ActionResults;
using EdFi.Dashboards.Resources;

namespace EdFi.Dashboards.Presentation.Architecture.Mvc.Controllers
{
    public abstract class HttpVerbHandlerPassthroughControllerBase : Controller
    {
        protected virtual ViewResult ViewWithStatus(object model, HttpStatusCode statusCode)
        {
            return ViewWithStatus(model, statusCode, null);
        }
            
        protected virtual ViewResult ViewWithStatus(object model, HttpStatusCode statusCode, string statusDescription)
        {
            if (model != null)
                base.ViewData.Model = model;

            return new ViewWithStatusCodeResult((int) statusCode, statusDescription)
                       {
                           ViewData = base.ViewData, 
                           TempData = base.TempData
                       };
        }
    }

    // TODO: GKM - To be deprecated, and replaced with GetHandlerPassthroughController.
    public class ServicePassthroughController<TRequest, TResponse> 
        : HttpVerbHandlerPassthroughControllerBase
    {
        /// <summary>
        /// Gets or sets the service to be called by the controller.  Intended for internal use only.
        /// </summary>
        /// <remarks>This service is injected using property injection rather than constructor injection
        /// to optimize for the experience of deriving another controller from this class.  With constructor
        /// injection, the developer would be forced to declare the service type as a constructor parameter
        /// and pass it into a base.</remarks>
        public IService<TRequest, TResponse> Service { get; set; }

        [HttpGet]
        public virtual ActionResult Get(TRequest request, int localEducationAgencyId)
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

            return View(response);
        }
    }

    #region For Future Use (Commented out)

    //public class GetHandlerPassthroughController<TRequest, TResponse>
    //    : HttpVerbHandlerPassthroughControllerBase
    //{
    //    /// <summary>
    //    /// Gets or sets the service to be called by the controller.  Intended for internal use only.
    //    /// </summary>
    //    /// <remarks>This service is injected using property injection rather than constructor injection
    //    /// to optimize for the experience of deriving another controller from this class.  With constructor
    //    /// injection, the developer would be forced to declare the service type as a constructor parameter
    //    /// and pass it into a base.</remarks>
    //    public IGetHandler<TRequest, TResponse> GetHandler { get; set; }

    //    public virtual ActionResult Get(TRequest request, int localEducationAgencyId)
    //    {
    //        // TODO: Deferred - localEducationAgencyId can be dropped after drilldowns are no longer using WebForms.
    //        // localEducationAgencyId is here to force model binding to populate it in context so that it 
    //        // can be provided to the metric Action urls so that the context is available in order to 
    //        // use the correct database connection (for multitenancy with multiple databases).  Otherwise,
    //        // when the drilldown is initiated on a WebForms URL, there is no local education agency context
    //        // provided, and the only way to get it would be to go to the database, which itself also needs
    //        // the local education agency context in order to select the correct connection, and into a loop
    //        // we go.  Once all website artifacts are using the MVC routing, this parameter could be dropped.

    //        try
    //        {
    //            var response = InvokeHandler(request);

    //            return View(response);
    //        }
    //        catch (Exception ex)
    //        {
    //            // TODO: GKM - Add logging here, or perhaps add an ActionFilter looking for a model of type ExceptionModel?
    //            return ViewWithStatus(new ExceptionModel(ex), HttpStatusCode.BadRequest);
    //        }
    //    }

    //    private TResponse InvokeHandler(TRequest request)
    //    {
    //        var response = GetHandler.Get(request);

    //        return response;
    //    }
    //}

    #endregion

    public class PostHandlerPassthroughController<TRequest, TResponse> 
        : HttpVerbHandlerPassthroughControllerBase
    {
        /// <summary>
        /// Gets or sets the service to be called by the controller.  Intended for internal use only.
        /// </summary>
        /// <remarks>This service is injected using property injection rather than constructor injection
        /// to optimize for the experience of deriving another controller from this class.  With constructor
        /// injection, the developer would be forced to declare the service type as a constructor parameter
        /// and pass it into a base.</remarks>
        public IPostHandler<TRequest, TResponse> PostHandler { get; set; }

        public virtual ActionResult Post(TRequest request, int? localEducationAgencyId)
        {
            var response = InvokeHandler(request);

            return CreatePostActionResult(response);
        }

        private TResponse InvokeHandler(TRequest request)
        {
            var response = PostHandler.Post(request);
            return response;
        }

        #region Legacy POST support

        [HttpPost]
        public virtual ActionResult Get(TRequest request, int? localEducationAgencyId)
        {
            return Post(request, localEducationAgencyId);
        }

        // This implementation supports the legacy AJAX interactions only
        // Given the natural key (vs. surrogate key) approach to the dashboard resources,
        // PUT operations (which identify the resource to be created or updated) are 
        // going to generally be more appropriate.
        protected ActionResult CreatePostActionResult(TResponse response)
        {
            if (typeof(TResponse) == typeof(bool))
            {
                bool success = Convert.ToBoolean(response);

                if (success)
                    return new HttpStatusCodeResult((int) HttpStatusCode.NoContent);

                // If we're still here, false was returned by the service, so throw an exception
                throw new Exception("PostHandler indicated failure.");
            }

            // Assign the response to the model to be returned
            if (response.Equals(default(TResponse)))
                base.ViewData.Model = response;

            return ViewWithStatus(response, HttpStatusCode.Created);
        }

        #endregion
    }

    public class PutHandlerPassthroughController<TRequest, TResponse> 
        : HttpVerbHandlerPassthroughControllerBase
    {
        /// <summary>
        /// Gets or sets the service to be called by the controller.  Intended for internal use only.
        /// </summary>
        /// <remarks>This service is injected using property injection rather than constructor injection
        /// to optimize for the experience of deriving another controller from this class.  With constructor
        /// injection, the developer would be forced to declare the service type as a constructor parameter
        /// and pass it into a base.</remarks>
        public IPutHandler<TRequest, TResponse> PutHandler { get; set; }

        public virtual ActionResult Put(TRequest request, int localEducationAgencyId)
        {
            try
            {
                bool created;

                var response = InvokeHandler(request, out created);

                return ViewWithStatus(response, created ? HttpStatusCode.Created : HttpStatusCode.OK, created ? "Created" : "Updated");
            }
            catch (Exception ex)
            {
                // TODO: GKM - Add logging
                return ViewWithStatus(new ExceptionModel(ex), HttpStatusCode.BadRequest);
            }
        }

        /// <summary>
        /// Invokes the resource handler to obtain the response.
        /// </summary>
        /// <param name="request">The request model associated with the operation.</param>
        /// <param name="created">Indicates whether a resource was created as a result of the request.</param>
        /// <returns>The response model.</returns>
        protected virtual TResponse InvokeHandler(TRequest request, out bool created)
        {
            var response = PutHandler.Put(request, out created);

            return response;
        }
    }

    public class DeleteHandlerPassthroughController<TRequest>
        : HttpVerbHandlerPassthroughControllerBase
    {
        /// <summary>
        /// Gets or sets the service to be called by the controller.  Intended for internal use only.
        /// </summary>
        /// <remarks>This service is injected using property injection rather than constructor injection
        /// to optimize for the experience of deriving another controller from this class.  With constructor
        /// injection, the developer would be forced to declare the service type as a constructor parameter
        /// and pass it into a base.</remarks>
        public IDeleteHandler<TRequest> DeleteHandler { get; set; }

        public virtual ActionResult Delete(TRequest request, int localEducationAgencyId)
        {
            try
            {
                InvokeHandler(request);

                return new HttpStatusCodeResult((int) HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                // TODO: GKM - Add logging
                return ViewWithStatus(new ExceptionModel(ex), HttpStatusCode.BadRequest);
            }
        }

        /// <summary>
        /// Invokes the resource handler to delete the resource.
        /// </summary>
        /// <param name="request">The request model associated with the operation.</param>
        protected virtual void InvokeHandler(TRequest request)
        {
            DeleteHandler.Delete(request);
        }
    }
}