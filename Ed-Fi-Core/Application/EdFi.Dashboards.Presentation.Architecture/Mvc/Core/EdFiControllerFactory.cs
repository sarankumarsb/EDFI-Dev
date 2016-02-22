// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using System.Web.Routing;
using EdFi.Dashboards.Common;
using EdFi.Dashboards.Common.Utilities;
using EdFi.Dashboards.Presentation.Architecture.Mvc.ActionFilters;
using EdFi.Dashboards.Presentation.Architecture.Mvc.Controllers;
using EdFi.Dashboards.Resources;
using EdFi.Dashboards.Resources.Metric.Requests;
using HalResourceModelBase = EdFi.Dashboards.Resource.Models.Common.Hal.ResourceModelBase;

namespace EdFi.Dashboards.Presentation.Architecture.Mvc.Core
{
    public class EdFiControllerFactory : DefaultControllerFactory
    {
        private enum ResourceType
        {
            /// <summary>
            /// Indicates that the resource is a general purpose resource, for which there will be at most one resource
            /// handler implementation for each HTTP verb (GET, POST, PUT, DELETE).
            /// </summary>
            General,
            /// <summary>
            /// Indicates that the resource follows the collection/item style (with a route pattern that has an optional
            /// key of '{resourceIdentifier}'), and that the resource type for this request is for an Item-level resource.
            /// </summary>
            Item,
            /// <summary>
            /// Indicates that the resource follows the collection/item style (with a route pattern that has an optional
            /// key of '{resourceIdentifier}'), and that the resource type for this request is for an Collection-level resource.
            /// </summary>
            Collection,
        }

        public override IController CreateController(RequestContext requestContext, string controllerName)
        {
            var controller = base.CreateController(requestContext, controllerName);

            var mvcController = controller as Controller;

            if (mvcController != null)
                mvcController.ActionInvoker = new ConnegActionInvoker();

            return controller;
        }

        private IEnumerable<Type> _serviceTypes;
        private IEnumerable<Type> ServiceTypes
        {
            get { return _serviceTypes ?? (_serviceTypes = GetTypeValues("Service")); }
        }

        private IEnumerable<Type> _extensionControllerTypes;
        private IEnumerable<Type> ExtensionControllerTypes
        {
            get { return _extensionControllerTypes ?? (_extensionControllerTypes = GetTypeValues("Controller")); }
        }

        private static IEnumerable<Type> GetTypeValues(string suffix)
        {
            var handler = IoC.Container.Kernel.GetAssignableHandlers(typeof (object));

            var typeValues =
                (from t in handler
                 where t.ComponentModel.Implementation.FullName.EndsWith(suffix)
                 select t.ComponentModel.Implementation)
                    .OrderBy(x => x.Name).Distinct()
                    .ToList();

            return typeValues;
        }

        protected override Type GetControllerType(RequestContext requestContext, string controllerName)
        {
            Type controllerType;

            // No explicit implementation exists, so go find the service matching the controllerName, by convention
            string area = (string)requestContext.RouteData.DataTokens["area"] ?? string.Empty;
            string resource = controllerName;

            if (TryGetExtensionController(requestContext, area, resource, out controllerType))
                return controllerType;

            if (TryGetControllerWithDefaultMvcConventions(requestContext, controllerName, out controllerType))
                return controllerType;

            if (TryGetImageController(area, resource, out controllerType))
                return controllerType;

            if (TryGetServicePassthroughController(requestContext, area, resource, out controllerType))
                return controllerType;

            if (TryGetDomainMetricController(requestContext, area, out controllerType))
                return controllerType;
            
            return null;
        }

        protected virtual bool TryGetControllerWithDefaultMvcConventions(RequestContext requestContext, string controllerName,
                                                               out Type controllerType)
        {
            Type explicitControllerType = null;
            try
            {
                // Let default factory try to find controller using default conventions
                explicitControllerType = base.GetControllerType(requestContext, controllerName);
            }
            catch(InvalidOperationException)
            {
                explicitControllerType = null;
            }
            finally
            {
                // If we found an explicitly implemented controller type, return that type now
                controllerType = explicitControllerType;
            }

            return (controllerType != null);
        }

        private bool TryGetDomainMetricController(RequestContext requestContext, string area, out Type controllerType)
        {
            controllerType = null;

            bool metricVariantPresent = Convert.ToInt32(requestContext.RouteData.Values["metricVariantId"]) != 0;
             
            // Is this a metrics route?
            if (metricVariantPresent)
            {
                Type metricRequestType = MetricRequestTypesByAreaName[area];
                controllerType = typeof (DomainMetricController<>).MakeGenericType(metricRequestType);
                return true;
            }

            return false;
        }

        private static ConcurrentDictionary<string, Type> imageControllerTypesByArea = new ConcurrentDictionary<string, Type>();

        private bool TryGetImageController(string area, string resource, out Type controllerType)
        {
            controllerType = null;

            if (resource == null)
                return false;

            if (!resource.Equals("Image", StringComparison.InvariantCultureIgnoreCase))
                return false;

            // Have we already identified this controller type?
            if (imageControllerTypesByArea.TryGetValue(area, out controllerType))
                return true;

            string serviceNameConvention1 = area + "ImageService";
            string serviceNameConvention2 = "." + area + ".ImageService";

            var closedServiceType =
                (from t in ServiceTypes
                 where t.Name.Equals(serviceNameConvention1, StringComparison.OrdinalIgnoreCase)
                       || t.FullName.EndsWith(serviceNameConvention2, StringComparison.OrdinalIgnoreCase)
                 select t)
                    .SingleOrDefault();

            if (closedServiceType == null)
                return false;

            var genericInterfaceType = closedServiceType.GetInterface(typeof (IService<,>).Name);

            if (genericInterfaceType == null)
                return false;

            var genericArgsBaseTypes =
                    new[] 
                    {
                        // Safe Assumption: IService implementations always have 2 generic arguments
                        genericInterfaceType.GetGenericArguments()[0].GetBaseRequestTypeForServiceRegistration(),
                        genericInterfaceType.GetGenericArguments()[1].GetBaseModelTypeForServiceRegistration(),
                    };

            // Close generic types for passthrough controller
            controllerType = typeof (ImageController<,>).MakeGenericType(genericArgsBaseTypes);

            imageControllerTypesByArea[area] = controllerType;

            return true;
        }

        private static ConcurrentDictionary<Tuple<string, string, string, ResourceType>, Type> controllerTypesByKey = new ConcurrentDictionary<Tuple<string, string, string, ResourceType>, Type>();

        private bool TryGetServicePassthroughController(RequestContext requestContext, string area, string resource, out Type controllerType)
        {
            // Initialize outbound parameter
            controllerType = null;

            if (resource == null)
                return false;

            // Get the HTTP method override header (if it was passed), or the HTTP method
            string httpMethod = requestContext.HttpContext.Request.GetHttpMethodOverride();
            ResourceType resourceType = GetResourceType(requestContext);
            var controllerKey = new Tuple<string, string, string, ResourceType>(httpMethod, area, resource, resourceType);

            // Have we already identified this controller type?
            if (controllerTypesByKey.TryGetValue(controllerKey, out controllerType))
                return true;

            Type serviceImplementationType;
            
            // No matching service? Quit now
            if (!TryGetServiceBasedOnConventions(area, resource, out serviceImplementationType))
                return false;

            Type genericInterfaceType;

            // No matching generic interface? Quit now
            if (!TryGetGenericInterfaceType(httpMethod, serviceImplementationType, resourceType, out genericInterfaceType))
                return false;

            // Create the generic controller type for the HTTP verb and matching generic service
            controllerType = CreatePassthroughControllerType(httpMethod, genericInterfaceType);

            if (controllerType == null)
                return false;

            // Save this decision for the next time we field this request
            controllerTypesByKey[controllerKey] = controllerType;

            return true;
        }

        private bool TryGetExtensionController(RequestContext requestContext, string area, string resource, out Type controllerType)
        {
            controllerType = null;

            if (resource == null)
                return false;

            // Get the HTTP method override header (if it was passed), or the HTTP method
            string httpMethod = requestContext.HttpContext.Request.GetHttpMethodOverride();
            string routedAction = (string)requestContext.RouteData.Values["action"] ?? string.Empty;
            ResourceType resourceType = GetResourceType(requestContext);
            var controllerKey = new Tuple<string, string, string, ResourceType>(httpMethod, area, resource, resourceType);
            

            // Have we already identified this controller type?
            if (controllerTypesByKey.TryGetValue(controllerKey, out controllerType))
                return true;

            List<Type> serviceTypesThatApply;

            if (string.IsNullOrEmpty(area))
            {
                //if the currrent request is not for an area, filter out all area-related controllers.
                var nonAreas = from t in ExtensionControllerTypes
                               where !t.FullName.ToLowerInvariant().Contains(".Areas.".ToLowerInvariant())
                               select t;

                string serviceNameConvention1 = "." + resource + "Controller";

                //We could have various controllers that apply.
                serviceTypesThatApply = (from t in nonAreas
                                         where (t.FullName.EndsWith(serviceNameConvention1, StringComparison.OrdinalIgnoreCase))
                                         select t).ToList();

            }
            else
            {
                string serviceNameConvention1 = area + resource + "Controller";
                string serviceNameConvention2 = "." + area + ".Controllers." + resource + "Controller";
                string serviceNameConvention3 = "." + area + ".Controllers.Detail." + resource + "Controller";

                //We could have various controllers that apply.
                serviceTypesThatApply = (from t in ExtensionControllerTypes
                                             where t.Name.Equals(serviceNameConvention1, StringComparison.OrdinalIgnoreCase)
                                                   || t.FullName.EndsWith(serviceNameConvention2, StringComparison.OrdinalIgnoreCase)
                                                   || t.FullName.EndsWith(serviceNameConvention3, StringComparison.OrdinalIgnoreCase)
                                             select t).ToList();
            }

            //This is to ensure that the explicit controller has the action method that is beeing requested. 
            //Becuase if there is an explicit extension controller that implements 'Get' for a service that has 'Get and Delete' we still allow the Service Pasthrough Controller to resolve the 'Delete'.
            serviceTypesThatApply = serviceTypesThatApply.Where(t => ControllerContainsAction(t, routedAction, httpMethod)).ToList();

            //If we found one service then return.
            if (serviceTypesThatApply.Count() == 1)
            {
                controllerType = serviceTypesThatApply.Single();
                return true;
            }

            //If we have multiple matches then we want to give priority to plugins and then extension. So...
            var pluginControllerType = serviceTypesThatApply.SingleOrDefault(x => x.FullName.StartsWith("EdFi.Dashboards.Plugins."));
            controllerType = pluginControllerType ?? serviceTypesThatApply.SingleOrDefault(x => x.FullName.Contains("Presentation.Web"));

            // No matching service?
            return controllerType != null;
        }

        public bool ControllerContainsAction(Type controller, string routedAction, string requestedHttpMethod)
        {
            // Get a descriptor of this controller
            var controllerDesc = new ReflectedControllerDescriptor(controller);

            // Look at each action in the controller
            foreach (var action in controllerDesc.GetCanonicalActions())
            {
                var currentControllerActionHttpAttributes = action.GetCustomAttributes(typeof(ActionMethodSelectorAttribute), true).Cast<ActionMethodSelectorAttribute>().ToList();

                // 1) Does the controller have an action with the requested httpMethod.
                if (action.ActionName.Equals(requestedHttpMethod, StringComparison.InvariantCultureIgnoreCase))
                    return true;

                // 2) Does the controller have an action with the name of the routedAction and ((a matching HTTPMethod attributed) or (no attribute and the default requested HTTPMethod of GET)).
                if( action.ActionName.Equals(routedAction, StringComparison.InvariantCultureIgnoreCase) &&
                    (currentControllerActionHttpAttributes.Any(a=> a.ToString().ToLower().Contains(requestedHttpMethod.ToLower())) || (!currentControllerActionHttpAttributes.Any() && requestedHttpMethod.Equals("GET",StringComparison.InvariantCultureIgnoreCase) ))
                    )
                    return true;
            }

            return false;
        }

        private Type CreatePassthroughControllerType(string httpMethod, Type genericInterfaceType)
        {
            var genericArgs = genericInterfaceType.GetGenericArguments();

            var genericArgsBaseTypes =
                genericArgs.Length == 1
                    ? new[]
                          {
                              // Deal with Request-only service interfaces (no return model)
                              genericArgs[0].GetBaseRequestTypeForServiceRegistration(),
                          }
                    : new[]
                          {
                              // Deal with Request/Response service interfaces
                              genericArgs[0].GetBaseRequestTypeForServiceRegistration(),
                              genericArgs[1].GetBaseModelTypeForServiceRegistration(),
                          };

            Type controllerType = CreateClosedGenericPassthroughControllerType(httpMethod, genericArgsBaseTypes);
            
            return controllerType;
        }

        private Type CreateClosedGenericPassthroughControllerType(string httpMethod, Type[] genericArgs)
        {
            Type controllerType = null;
            
            // Close generic types for the appropriate passthrough controller
            switch (httpMethod)
            {
                case "GET":
                    controllerType = typeof(ServicePassthroughController<,>).MakeGenericType(genericArgs);
                    break;

                case "POST":
                    controllerType = typeof(PostHandlerPassthroughController<,>).MakeGenericType(genericArgs);
                    break;

                case "PUT":
                    controllerType = typeof(PutHandlerPassthroughController<,>).MakeGenericType(genericArgs);
                    break;

                case "DELETE":
                    controllerType = typeof(DeleteHandlerPassthroughController<>).MakeGenericType(genericArgs);
                    break;
            }

            return controllerType;
        }

        private bool TryGetServiceBasedOnConventions(string area, string resource, out Type serviceImplementationType)
        {
            // Initialize outbound parameter
            serviceImplementationType = null;

            string serviceNameConvention1 = area + resource + "Service";
            string serviceNameConvention2 = "." + area + "." + resource + "Service";
            string serviceNameConvention3 = "." + area + ".Detail." + resource + "Service";

            // We could have various services that apply. I.E.: (ext.StudentSchool.InfoService, resources.StudentSchool.InfoService)
            var serviceTypesThatApply = (from t in ServiceTypes
                                         where t.Name.Equals(serviceNameConvention1, StringComparison.OrdinalIgnoreCase)
                                               || t.FullName.EndsWith(serviceNameConvention2, StringComparison.OrdinalIgnoreCase)
                                               || t.FullName.EndsWith(serviceNameConvention3, StringComparison.OrdinalIgnoreCase)
                                         select t)
                                         .ToList();

            //If we have multiple matches then we want to give priority to the extension. So...
            if (serviceTypesThatApply.Count() > 1)
                serviceImplementationType = serviceTypesThatApply.SingleOrDefault(x => x.FullName.Contains(".Extensions."));
            else
                serviceImplementationType = serviceTypesThatApply.SingleOrDefault();

            return serviceImplementationType != null;
        }

        private static ResourceType GetResourceType(RequestContext requestContext)
        {
            ResourceType resourceType;
            
            // If the route uses the convention for a collection/item resource...
            if (((Route) requestContext.RouteData.Route).Url.Contains("{resourceIdentifier}"))
            {
                object value;

                if (requestContext.RouteData.Values.TryGetValue("resourceIdentifier", out value) 
                    && value.GetType().IsValueType || value is string) // Not supplied in URL, and this key will come through as an empty Mvc.UrlParameter
                    resourceType = ResourceType.Item;
                else
                    resourceType = ResourceType.Collection;
            }
            else
            {
                resourceType = ResourceType.General;
            }

            return resourceType;
        }

        private bool TryGetGenericInterfaceType(string httpMethod, Type serviceImplementationType, out Type genericInterfaceType)
        {
            // Initialize outbound parameter
            genericInterfaceType = null;

            switch (httpMethod)
            {
                case "GET":
                    genericInterfaceType = serviceImplementationType.GetInterface(typeof(IService<,>).Name);
                    break;

                case "POST":
                    genericInterfaceType = serviceImplementationType.GetInterface(typeof(IPostHandler<,>).Name);
                    break;

                case "PUT":
                    genericInterfaceType = serviceImplementationType.GetInterface(typeof(IPutHandler<,>).Name);
                    break;

                case "DELETE":
                    genericInterfaceType = serviceImplementationType.GetInterface(typeof(IDeleteHandler<>).Name);
                    break;
            }

            return genericInterfaceType != null;
        }

        private bool TryGetGenericInterfaceType(string httpMethod, Type serviceImplementationType, ResourceType resourceType, out Type genericInterfaceType)
        {
            // Initialize outbound parameter
            genericInterfaceType = null;

            // Use original behavior that assumes only a single implementation per server (no collection/item handling)
            if (resourceType == ResourceType.General)
                return TryGetGenericInterfaceType(httpMethod, serviceImplementationType, out genericInterfaceType);

            // We have the potential for multiple handlers for a particular HTTP verb that we must discriminate between.
            // We are going to use the convention of looking for Collection or Item in the request type name to match
            // this controller type request to the correct service interface.
            Type serviceInterfaceType = null;

            switch (httpMethod)
            {
                case "GET":
                    serviceInterfaceType = typeof(IService<,>);
                    break;

                case "POST":
                    serviceInterfaceType = typeof(IPostHandler<,>);
                    break;

                case "PUT":
                    serviceInterfaceType = typeof(IPutHandler<,>);
                    break;

                case "DELETE":
                    serviceInterfaceType = typeof(IDeleteHandler<>);
                    break;
            }

            // Build a RegEx expression to precisely identify the presence of 'Collection' or 'Item' in the request type name (based on ResourceType)
            var serviceNamePattern = new Regex(serviceInterfaceType.FullName + @"\[\[.*?" + resourceType.ToString() + ".*?Request,");

            // Find the interface that matches the ResourceType provided
            genericInterfaceType = serviceImplementationType.GetInterfaces().SingleOrDefault(i => serviceNamePattern.IsMatch(i.FullName));

            return genericInterfaceType != null;
        }

        private IDictionary<string, Type> _metricRequestTypesByAreaName;

        private IDictionary<string, Type> MetricRequestTypesByAreaName
        {
            get
            {
                if (_metricRequestTypesByAreaName == null)
                {
                    // TODO: Deferred - Will need to widen the search to include extension assemblies
                    var assembly = typeof (Marker_EdFi_Dashboards_Resources).Assembly;
                    _metricRequestTypesByAreaName = MetricRequestTypesHelper.GetMetricRequestTypesByAreaName(assembly);
                }

                return _metricRequestTypesByAreaName;
            }
        }

        protected override IController GetControllerInstance(RequestContext requestContext, Type controllerType)
        {
            if (controllerType == null)
                throw new ControllerNotFoundException("No controller of type '" +
                                                      requestContext.RouteData.Values["controller"] +
                                                      "' for request to '" + requestContext.HttpContext.Request.Url +
                                                      "'.");

            var controller = (IController) IoC.Resolve(controllerType);

            if (controller != null)
                return controller;

            controller = (IController) Activator.CreateInstance(controllerType);

            return controller;
        }

        public override void ReleaseController(IController controller)
        {
            try
            {
                // Prevent memory leak through IoC container
                IoC.Release(controller);
            }
            catch
            {
                base.ReleaseController(controller);
            }
        }
    }
}
