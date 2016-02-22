// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using EdFi.Dashboards.Common.Utilities;
using EdFi.Dashboards.Infrastructure;

namespace EdFi.Dashboards.Resources.Navigation
{
    public abstract class SiteAreaBase
    {
        public IRouteValuesPreparer RouteValuesPreparer { get; set; }
        public IHttpRequestProvider HttpRequestProvider { get; set; }
        public IConfigValueProvider ConfigValueProvider { get; set; }

        private string _area;

        protected string Area
        {
            get
            {
                if (_area == null)
                    _area = this.GetType().Name;

                return _area;
            }
        }

        protected string BuildMetricUrl(object additionalValues, MethodBase callingMethod, params object[] parameterValues)
        {
            var routeValueDictionary = GetRouteValuesFromCallingMethod(callingMethod, parameterValues);

            if (!routeValueDictionary.ContainsKey("metricVariantId"))
            {
                throw new ArgumentException(
                    string.Format(
                        "SiteArea method '{0}.{1}' does not contain a 'metricVariantId' parameter.  Cannot generate the metric URL.",
                        this.GetType().Name, callingMethod.Name));
            }

            return CreateRouteCore(Area + "_Metrics", routeValueDictionary, additionalValues);
        }
        protected string BuildMetricDrilldownUrl(object additionalValues, string drilldownName, MethodBase callingMethod, params object[] parameterValues)
        {
            var routeValueDictionary = GetRouteValuesFromCallingMethod(callingMethod, parameterValues);

            if (!routeValueDictionary.ContainsKey("metricVariantId"))
            {
                throw new ArgumentException(
                    string.Format(
                        "SiteArea method '{0}.{1}' does not contain a 'metricVariantId' parameter.  Cannot generate the metric URL.",
                        this.GetType().Name, callingMethod.Name));
            }
            routeValueDictionary["controller"] = drilldownName;

            string area = this.GetType().Name;

            return CreateRouteCore(area + "_MetricsDrilldown", routeValueDictionary, additionalValues);
        }

        protected string BuildUrlUsingMethodNameAsRouteSuffix(object additionalValues, MethodBase callingMethod, params object[] parameterValues)
        {
            var routeValueDictionary = GetRouteValuesFromCallingMethod(callingMethod, parameterValues);

            string routeSuffix = callingMethod.Name;

            return CreateRouteCore(Area + "_" + routeSuffix, routeValueDictionary, additionalValues);
        }

        protected string BuildApiResourceUrl(object additionalValues, MethodBase callingMethod, params object[] parameterValues)
        {
            return BuildApiResourceUrl(additionalValues, null, callingMethod, parameterValues);
        }

        protected string BuildApiResourceUrl(object additionalValues, string resourceName, MethodBase callingMethod, params object[] parameterValues)
        {
            return BuildResourceUrlCore("ApiResources", additionalValues, resourceName, callingMethod, parameterValues);
        }
        
        protected string BuildResourceUrl(object additionalValues, MethodBase callingMethod, params object[] parameterValues)
        {
            return BuildResourceUrl(additionalValues, null, callingMethod, parameterValues);
        }

        protected string BuildResourceUrl(object additionalValues, string resourceName, MethodBase callingMethod, params object[] parameterValues)
        {
            return BuildResourceUrlCore("Resources", additionalValues, resourceName, callingMethod, parameterValues);
        }

        protected string BuildResourceUrlCore(string routeNameSuffix, object additionalValues, string resourceName, MethodBase callingMethod, params object[] parameterValues)
        {
            var routeValueDictionary = GetRouteValuesFromCallingMethod(callingMethod, parameterValues);

            //If we passed in a resource name, use it as the controller. (this is for drill-downs, generally)
            string controllerName;
            if (!string.IsNullOrEmpty(resourceName))
            {
                controllerName = resourceName;
            }
            else
            {
                controllerName = callingMethod.Name;
            }

            routeValueDictionary["controller"] = controllerName;

            return CreateRouteCore(Area + "_" + routeNameSuffix, routeValueDictionary, additionalValues);
        }

        protected string BuildUrl(string routeName, object additionalValues, MethodBase callingMethod, params object[] parameterValues)
        {
            var routeValueDictionary = GetRouteValuesFromCallingMethod(callingMethod, parameterValues);

            return CreateRouteCore(routeName, routeValueDictionary, additionalValues);
        }

        protected string BuildUrl(string routeName, string resourceName, object additionalValues, MethodBase callingMethod, params object[] parameterValues)
        {
            var routeValueDictionary = GetRouteValuesFromCallingMethod(callingMethod, parameterValues);
            routeValueDictionary["controller"] = resourceName;

            return CreateRouteCore(routeName, routeValueDictionary, additionalValues);
        }

        protected virtual string CreateRouteCore(string routeName, RouteValueDictionary routeValueDictionary, object additionalValues)
        {
            // Prepare outbound route values, according to the route
            RouteValuesPreparer.PrepareRouteValues(routeName, routeValueDictionary);

            // Apply the additional values
            if (additionalValues != null)
                ApplyAdditionalValues(routeValueDictionary, additionalValues);

            // Invoke routing system to generate the URL
            var urlHelper = new UrlHelper(HttpRequestProvider.RequestContext);
            string url = urlHelper.RouteUrl(routeName, routeValueDictionary, "https", HttpRequestProvider.Url.Host);

            return url;
        }

		private static void ApplyAdditionalValues(RouteValueDictionary routeValueDictionary, object additionalValues) 
        {
            var additionalValuesDictionary = new RouteValueDictionary(additionalValues);

            // Set the additional non-null values to the dictionary
            foreach (var pair in additionalValuesDictionary.Where(kvp => kvp.Value != null))
                routeValueDictionary[pair.Key] = pair.Value;
        }

        private static Regex replacementRegex = new Regex(@"[^\w]", RegexOptions.Compiled);

        private static ConcurrentDictionary<MethodBase, ParameterInfo[]> parameterInfosByMethodBase = new ConcurrentDictionary<MethodBase, ParameterInfo[]>();

        protected static RouteValueDictionary GetRouteValuesFromCallingMethod(MethodBase callingMethod, object[] parameterValues) 
        {
            var routeValueDictionary = new RouteValueDictionary();
            routeValueDictionary["controller"] = callingMethod.Name; // Default the "controller" to the calling method's name

            ParameterInfo[] parmInfos;

            if (!parameterInfosByMethodBase.TryGetValue(callingMethod, out parmInfos))
            {
                parmInfos = callingMethod.GetParameters();
                parmInfos = parameterInfosByMethodBase.GetOrAdd(callingMethod, parmInfos);
            }

            for (int i = 0; i < parmInfos.Length; i++)
            {
                // Make sure the additionalValues argument isn't being passed through as a regular argument.  Special processing is required.
                if (i == parmInfos.Length - 1 && parmInfos[i].ParameterType == typeof(object))
                {
                    if (parmInfos.Length == parameterValues.Length)
                        throw new ArgumentException("Encountered an extra argument of type 'object' during link generation method signature processing.  Are you passing the anonymous-typed additionalValues through from the end of the signature?");

                    break;
                }

                if (parameterValues.Length > i && parameterValues[i] != null)
                {
                    var s = parameterValues[i] as string;
                    if (s != null)
                    {
                        if (!String.IsNullOrEmpty(s))
                            routeValueDictionary[parmInfos[i].Name] = replacementRegex.Replace(s, "-");
                    }
                    else if (!string.IsNullOrEmpty(parameterValues[i].ToString()))
                        routeValueDictionary[parmInfos[i].Name] = parameterValues[i];
                }
            }

            return routeValueDictionary;
        }
    }
}