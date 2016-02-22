// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using EdFi.Dashboards.Common.Utilities;
using EdFi.Dashboards.Core;
using EdFi.Dashboards.Core.Providers.Context;
using EdFi.Dashboards.Presentation.Architecture.Providers;

namespace EdFi.Dashboards.Presentation.Architecture.Mvc.ActionFilters
{
    public class DashboardContextActionFilter : IActionFilter
    {
		private readonly IRequestUrlBaseProvider requestUrlBaseProvider;
		private readonly Dictionary<string, PropertyInfo> _contextPropertiesByName = new Dictionary<string, PropertyInfo>();

        public DashboardContextActionFilter()
        {
            //Resolve dependency.
            requestUrlBaseProvider = IoC.Resolve<IRequestUrlBaseProvider>();

            // Capture the metadata of the properties we're interested in for the DashboardContext
            var contextProperties = 
                from p in typeof(EdFiDashboardContext).GetProperties()
                select p;

            foreach (var property in contextProperties)
                _contextPropertiesByName.Add(property.Name.ToLower(), property);
        }

        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            // Only build the dashboard context once per "call", using the main/first action result
            if (EdFiDashboardContext.Current != null)
                return;

            EdFiDashboardContext dashboardContext = null;

            if (!TryFindDashboardContextInActionParameters(filterContext, out dashboardContext))
            {
                dashboardContext = CreateEdFiDashboardContextFromDictionary(filterContext.ActionParameters);
            }

            // Try to set the domain entity type based on the area for the route
            if (dashboardContext != null && filterContext.RouteData.DataTokens.ContainsKey("area"))
            {
                MetricInstanceSetType metricInstanceSetType;

                if (Enum.TryParse((string) filterContext.RouteData.DataTokens["area"], out metricInstanceSetType))
                {
                    dashboardContext.MetricInstanceSetType = metricInstanceSetType;

                    // Special handling for Staff sections (should this be moved to an extensibility point?)
                    if (metricInstanceSetType == MetricInstanceSetType.Staff && filterContext.RouteData.Values.ContainsKey("controller"))
                        dashboardContext.ViewType = (string)filterContext.RouteData.Values["controller"];
                }
            }

            SaveDashboardContextToCallContext(dashboardContext, filterContext);
        }

        public EdFiDashboardContext CreateEdFiDashboardContextFromDictionary(IDictionary<string, object> actionParameters)
        {
            var contextPropertyValues = GetContextPropertyValuesFromParameters(actionParameters);

            EdFiDashboardContext dashboardContext = new EdFiDashboardContext();

            foreach (var pair in contextPropertyValues)
                _contextPropertiesByName[pair.Key].SetValue(dashboardContext, pair.Value, null);

            return dashboardContext;
        }

        private void SaveDashboardContextToCallContext(EdFiDashboardContext dashboardContext, ActionExecutingContext filterContext)
        {
            /*
             * mvc binder is missing the following values if they are not provided in the request parameter. therefore
             * we are helping binder go get the values from value providers and put them in dashboardContext.
             */

            // BTrabon 7/24/2014:
            // this change below is part of a fix where the form value provider
            // is returning a string array of one when calling the
            // EdFiGridExportController
            ResolveSchoolId(dashboardContext, filterContext);
			ResolveLocalEducationAgencyId(dashboardContext, filterContext);

	        SetRoutedUrl(filterContext, dashboardContext);

            CallContext.SetData(EdFiDashboardContext.CallContextKey, dashboardContext);
        }

	    	    private static void ResolveLocalEducationAgencyId(EdFiDashboardContext dashboardContext, ActionExecutingContext filterContext)
	    {
		    var resolvedLocalEducationAgencyId = ResolveKeyValue(filterContext, "localEducationAgencyId");
		    if (resolvedLocalEducationAgencyId != null)
		    {
			    int localEducationAgencyId;
			    if (int.TryParse(resolvedLocalEducationAgencyId.ToString(), out localEducationAgencyId))
				    dashboardContext.LocalEducationAgencyId = localEducationAgencyId;
			    else
				    dashboardContext.LocalEducationAgencyId = null;
		    }
		    else
		    {
			    dashboardContext.LocalEducationAgencyId = null;
		    }
	    }

	    private static void ResolveSchoolId(EdFiDashboardContext dashboardContext, ActionExecutingContext filterContext)
	    {
		    var resolvedSchoolId = ResolveKeyValue(filterContext, "schoolId");
		    if (resolvedSchoolId != null)
		    {
			    int schoolId;
			    if (int.TryParse(resolvedSchoolId.ToString(), out schoolId))
				    dashboardContext.SchoolId = schoolId;
			    else
				    dashboardContext.SchoolId = null;
		    }
		    else
		    {
			    dashboardContext.SchoolId = null;
		    }
	    }

		private static object ResolveKeyValue(ControllerContext context, string key)
		{
			var result = ValueProviderFactories.Factories
				.Select(factory => factory.GetValueProvider(context))
				.Where(valueProvider => valueProvider != null)
				.Select(valueProvider => valueProvider.GetValue(key))
				.FirstOrDefault(value => value != null);
			//TODO: If we figure out what is causing the form value provider to return a string array then remove this code
			//return result == null ? null : result.RawValue;
			var returnValue = ConvertValueProviderResult(result);
			return returnValue;
		}

		/// <summary>
		/// Used to test the ValueProviderResult; for some reason when the edfi
		/// grid calls the EdFiGridExportController the data is returned as a
		/// string array; this method will test for a string array and return
		/// the value from within the array.
		/// </summary>
		/// <param name="result">The result.</param>
		/// <returns>The data value.</returns>
		private static object ConvertValueProviderResult(ValueProviderResult result)
		{
			if (result == null)
				return null;

			if (result.RawValue.GetType() != typeof(string[])) return result.RawValue;

			var rawValue = ((string[])result.RawValue)[0].Trim();
			return rawValue == string.Empty ? null : rawValue;
		}

	    private void SetRoutedUrl(ActionExecutingContext filterContext, EdFiDashboardContext dashboardContext)
        {
            string routedVirtualPath = filterContext.RouteData.Route.GetVirtualPath(filterContext.RequestContext, filterContext.RouteData.Values).VirtualPath; // i.e. "AllenISD"

            // Get the base URL with the trailing slash, if it exists
            string urlBase = requestUrlBaseProvider.GetRequestUrlBase(filterContext.RequestContext.HttpContext.Request);

            // Construct the URL that has been regenerated (i.e. a request for https://app/AllenISD/Overview will be shortened 
            // to https://app/AllenISD if "Overview" is defaulted, thereby making it possible to match with generated URLs on 
            // the menu items for determining correct menu selection)
            dashboardContext.RoutedUrl = (new Uri(urlBase + routedVirtualPath)).AbsolutePath;
        }



        private bool TryFindDashboardContextInActionParameters(ActionExecutingContext filterContext, out EdFiDashboardContext dashboardContext)
        {
            dashboardContext =
                (from p in filterContext.ActionParameters
                 where p.Value is EdFiDashboardContext
                 select p.Value)
                    .FirstOrDefault() as EdFiDashboardContext;

            return (dashboardContext != null);
        }

        private static Dictionary<Type, PropertyInfo[]> propertiesByType = new Dictionary<Type, PropertyInfo[]>();

        private PropertyInfo[] GetProperties(Type type)
        {
            PropertyInfo[] properties;

            if (!propertiesByType.TryGetValue(type, out properties))
            {
                properties = type.GetProperties();
                propertiesByType[type] = properties;
            }

            return properties;
        }

        private IEnumerable<KeyValuePair<string, object>> GetContextPropertyValuesFromParameters(IDictionary<string, object> actionParameters)
        {
            var contextValues = new List<KeyValuePair<string, object>>();

            foreach (var actionParameter in actionParameters)
            {
                Type actionParameterValueType = null;
                
                if (actionParameter.Value != null)
                    actionParameterValueType = actionParameter.Value.GetType();

                // Is this action parameter a class?
                if (actionParameterValueType != null 
                    && actionParameterValueType.IsClass
                    && actionParameterValueType != typeof(string))
                {
                    // Find properties whose name matches a dashboard context property
                    contextValues.AddRange(
                        from p in GetProperties(actionParameterValueType)
                        where _contextPropertiesByName.ContainsKey(p.Name.ToLower())
                        select new KeyValuePair<string, object>(
                            p.Name.ToLower(), 
                            p.GetValue(actionParameter.Value, null))
                        );
                }
                else
                {
                    // Does the dashboard context use this parameter?
                    if (_contextPropertiesByName.ContainsKey(actionParameter.Key.ToLower()))
                    {
                        contextValues.Add(
                            new KeyValuePair<string, object>(
                                actionParameter.Key.ToLower(),
                                actionParameter.Value));
                    }
                }
            }

            return contextValues;
        }

        // Do nothing afterwards
        public void OnActionExecuted(ActionExecutedContext filterContext) { }		
    }
}