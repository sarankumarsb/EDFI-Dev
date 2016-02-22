// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System;
using System.Runtime.Serialization;
using System.Text;
using System.Web.Mvc;
using EdFi.Dashboards.Infrastructure;
using EdFi.Dashboards.Presentation.Architecture.Mvc.ActionResults;
using HalResourceModelBase = EdFi.Dashboards.Resource.Models.Common.Hal.ResourceModelBase;
using EdFi.Dashboards.Common;

namespace EdFi.Dashboards.Presentation.Architecture.Mvc.ActionFilters
{
    public class ConnegActionInvoker : ControllerActionInvoker
    {
        protected override ActionResult CreateActionResult(ControllerContext controllerContext, ActionDescriptor actionDescriptor, object actionReturnValue)
        {
            // Strip the model from the View action result
            var viewActionResult = actionReturnValue as ViewResult;
            
            var acceptHeader = String.Empty;
            var formatParam = String.Empty;
            try
            {
                if (controllerContext != null && controllerContext.HttpContext != null && controllerContext.HttpContext.Request != null && controllerContext.HttpContext.Request.Headers != null)
                {
                    if (!String.IsNullOrEmpty(controllerContext.HttpContext.Request.Headers["accept"]))
                        acceptHeader = controllerContext.HttpContext.Request.Headers["accept"];
                }
                if (controllerContext != null && controllerContext.HttpContext != null && controllerContext.HttpContext.Request != null && controllerContext.HttpContext.Request.QueryString != null)
                {
                    if (!String.IsNullOrEmpty(controllerContext.HttpContext.Request.QueryString["format"]))
                        formatParam = controllerContext.HttpContext.Request.QueryString["format"];
                }
            }
            catch (NullReferenceException) { }


            if (viewActionResult != null)
            {
                // TODO: If further work is done on the serialization formats, it might be worth considering introducing a chain of responsibility pattern.
                if (acceptHeader.Contains("application/json") || formatParam.Equals("json", StringComparison.InvariantCultureIgnoreCase)) 
                {
                    var model = GetSerializableModel(viewActionResult.Model);

                    // Convert result to JSON
                    var jsonResult = new JsonDotNetResult
                               {
                                   ContentType =  IsHalResourceType(model) ? "application/hal+json" : "application/json",
                                   ContentEncoding = Encoding.UTF8,
                                   Data = model,
                                   JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                               };

                    // Apply explicit status, if it's available
                    var viewActionResultWithStatus = viewActionResult as ViewWithStatusCodeResult;

                    if (viewActionResultWithStatus != null)
                    {
                        jsonResult.StatusCode = viewActionResultWithStatus.StatusCode;
                        jsonResult.StatusDescription = viewActionResultWithStatus.StatusDescription;
                    }

                    return jsonResult;
                }

                #region Xml Support (Commented out)

                // Left here for possible future inclusion.  The problem is that the legacy ResourceModelBase clas is
                // not serializable using the XmlSerializer, and the new one hasn't been tried.  Here's the error:
                // System.InvalidOperationException: Cannot serialize member ResourceModelBase.Links 
                // of type IEnumerable`1[[...Link...]] because it is an interface.
                
                //if (acceptHeader.Contains("text/xml") || formatParam.Equals("xml", StringComparison.InvariantCultureIgnoreCase))
                //{
                //    var model = GetSerializableModel(viewActionResult.Model);

                //    return new XmlResult(model);
                //}

                #endregion

                if (acceptHeader.Contains("text/csv") || formatParam.Equals("csv", StringComparison.InvariantCultureIgnoreCase))
                {
                    var model = GetSerializableModel(viewActionResult.Model) as ICsvSerializable;

                    if (model == null)
                        throw new SerializationException("Model to be serialized to CSV must implement ICsvSerializable.");

                    return new CsvResult(model);
                }
            }

            return base.CreateActionResult(controllerContext, actionDescriptor, actionReturnValue);
        }

        /// <summary>
        /// Indicates whether the base resource model type being used is the one added to support the HAL media type.
        /// </summary>
        /// <param name="model">The model instance to be inspected for HAL media type support.</param>
        /// <returns><b>true</b> if the model type support HAL serialization; otherwise <b>false</b>.</returns>
        /// <remarks>This method should be removed after all resources have been converted to use HAL.</remarks>
        private static bool IsHalResourceType(object model)
        {
            if (model == null)
                return false;

            var modelType = model.GetType();

            var enumerableInfo = modelType.GetEnumerableInfo();

            if (enumerableInfo != null)
                modelType = enumerableInfo.EnumerableItemType;

            return (typeof(HalResourceModelBase)).IsAssignableFrom(modelType);
        }

        // Note: This concept of partially serializable model is really covering for the fact that there are certain
        // cases whether a custom controller is augmenting the model that is returned from the service layer.
        // It would probably be better to eliminate this concept, and separate the controller and the service into
        // two separately addressable resources, rather than strip off part of the model using this IPartiallySerializable
        // interface.
        private object GetSerializableModel(object model)
        {
            if (model == null)
                return null;

            Type interfaceType = model.GetType().GetInterface(typeof (IPartiallySerializable<>).Name);

            if (interfaceType != null)
            {
                var property = interfaceType.GetProperty("SerializableModel");
                return property.GetValue(model, null);
            }

            return model;
        }

        protected override ActionDescriptor FindAction(ControllerContext controllerContext, ControllerDescriptor controllerDescriptor, string actionName)
        {
            ActionDescriptor result = null;

            switch (controllerContext.RequestContext.HttpContext.Request.GetHttpMethodOverride())
            {
                case "GET":
                    result = base.FindAction(controllerContext, controllerDescriptor, "Get");
                    break;
                case "POST":
                    result = base.FindAction(controllerContext, controllerDescriptor, "Post");
                    break;
                case "PUT":
                    result = base.FindAction(controllerContext, controllerDescriptor, "Put");
                    break;
                case "DELETE":
                    result = base.FindAction(controllerContext, controllerDescriptor, "Delete");
                    break;
                case "PATCH":
                    result = base.FindAction(controllerContext, controllerDescriptor, "Patch");
                    break;
            }

            // If name convention doesn't work, use default MVC behavior (using HttpGet/HttpPost/HttpPut/HttpDelete attributes)
            if (result == null)
                return base.FindAction(controllerContext, controllerDescriptor, actionName);

            return result;
        }
    }
}