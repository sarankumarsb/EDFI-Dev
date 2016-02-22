using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using RazorGenerator.Mvc;

namespace EdFi.Dashboards.Presentation.Architecture.Mvc.Extensions
{
    public static class ViewStartPageExtensions
    {
        /// <summary>
        /// Gets the virtual path of the layout view (resolving using the view search locations if a view name is provided).
        /// </summary>
        /// <param name="webViewPage">The object whose layout is to be set.</param>
        /// <param name="layoutName">The layout's view name (to be found using view search locations), or a virtual path directly to the view.</param>
        /// <returns>The virtual path to the layout view.</returns>
        public static string GetLayout(this WebViewPage webViewPage, string layoutName)
        {
            if (webViewPage.Context.Request.IsAjaxRequest())
            {
                return null;
            }

            return layoutName.StartsWith("~") ? layoutName : GetViewVirtualPath(webViewPage.ViewContext, layoutName);
        }

        /// <summary>
        /// Gets the virtual path of the layout view (resolving using the view search locations if a view name is provided).
        /// </summary>
        /// <param name="viewStartPage">The object whose layout is to be set.</param>
        /// <param name="layoutName">The layout's view name (to be found using view search locations), or a virtual path directly to the view.</param>
        /// <returns>The virtual path to the layout view.</returns>
        public static string GetLayout(this ViewStartPage viewStartPage, string layoutName)
        {
            if (viewStartPage.Context.Request.IsAjaxRequest())
            {
                return null;
            }

            return layoutName.StartsWith("~") ? layoutName : GetViewVirtualPath(viewStartPage.ViewContext.Controller.ControllerContext, layoutName);
        }

        private static string GetViewVirtualPath(ControllerContext controllerContext, string layoutName)
        {
            var viewResult = ViewEngines.Engines.FindView(controllerContext, layoutName, string.Empty);

            return viewResult != null ? GetViewVirtualPathByType(viewResult.View, controllerContext, layoutName) : null;
        }

        private static string GetViewVirtualPathByType(IView view, ControllerContext controllerContext, string layoutName)
        {
            if (view == null)
                return null;

            if(view is PrecompiledMvcView)
            {
                return (from layoutPath in ViewEngines.Engines.OfType<PrecompiledMvcEngine>().SelectMany(viewEngine => GetPath(viewEngine, controllerContext, layoutName))
                        let layoutResult = ViewEngines.Engines.FindView(controllerContext, layoutPath, string.Empty)
                        where layoutResult.View != null && (layoutResult.View as PrecompiledMvcView) != null
                        select layoutPath).FirstOrDefault();
            }

            var compiledView = view as BuildManagerCompiledView;

            return compiledView != null ? compiledView.ViewPath : null;
        }

        private static IEnumerable<string> GetPath(PrecompiledMvcEngine engine, ControllerContext controllerContext, string name)
        {
            var controllerName = controllerContext.RouteData.GetRequiredString("controller");

            if (string.IsNullOrEmpty(name))
            {
                return new List<string>();
            }

            var areaName = GetAreaName(controllerContext.RouteData);
            var usingAreas = !string.IsNullOrEmpty(areaName);
            var viewLocations = GetViewLocations(engine.ViewLocationFormats, (usingAreas) ? engine.AreaViewLocationFormats : null);

            return viewLocations.Count == 0 ? new List<string>() : viewLocations.Select(location => location.Format(name, controllerName, areaName));
        }

        private static string GetAreaName(RouteBase route)
        {
            var routeWithArea = route as IRouteWithArea;

            if (routeWithArea != null)
            {
                return routeWithArea.Area;
            }

            var castRoute = route as Route;

            if (castRoute != null && castRoute.DataTokens != null)
            {
                return castRoute.DataTokens["area"] as string;
            }

            return null;
        }

        private static string GetAreaName(RouteData routeData)
        {
            object area;

            if (routeData.DataTokens.TryGetValue("area", out area))
            {
                return area as string;
            }

            return GetAreaName(routeData.Route);
        }

        private static List<ViewLocation> GetViewLocations(IEnumerable<string> viewLocationFormats, IEnumerable<string> areaViewLocationFormats)
        {
            var allLocations = new List<ViewLocation>();

            if (areaViewLocationFormats != null)
            {
                allLocations.AddRange(areaViewLocationFormats.Select(areaViewLocationFormat => new AreaAwareViewLocation(areaViewLocationFormat)));
            }

            if (viewLocationFormats != null)
            {
                allLocations.AddRange(viewLocationFormats.Select(viewLocationFormat => new ViewLocation(viewLocationFormat)));
            }

            return allLocations;
        }

        private class ViewLocation
        {
            protected readonly string VirtualPathFormatString;

            public ViewLocation(string virtualPathFormatString)
            {
                VirtualPathFormatString = virtualPathFormatString;
            }

            public virtual string Format(string viewName, string controllerName, string areaName)
            {
                return string.Format(CultureInfo.InvariantCulture, VirtualPathFormatString, viewName, controllerName);
            }
        }

        private class AreaAwareViewLocation : ViewLocation
        {
            public AreaAwareViewLocation(string virtualPathFormatString) : base(virtualPathFormatString) { }

            public override string Format(string viewName, string controllerName, string areaName)
            {
                return string.Format(CultureInfo.InvariantCulture, VirtualPathFormatString, viewName, controllerName, areaName);
            }
        }
    }
}