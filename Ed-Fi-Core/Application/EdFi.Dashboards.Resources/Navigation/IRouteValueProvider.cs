// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;

namespace EdFi.Dashboards.Resources.Navigation
{
    /// <summary>
    /// Expands route values as necessary to enable the generation of complete and correct MVC routes via the routing infrastructure.
    /// </summary>
    /// <remarks>This interface works in the opposite direction from </remarks>
    public interface IRouteValueProvider
    {
        /// <summary>
        /// Indicates whether the provider can provide a specific route value identified by the specified key.
        /// </summary>
        /// <param name="key">The key for the route value entry to be provided.</param>
        /// <param name="getValue">A function that retrieves existing route values, including route values provided by other <see cref="IRouteValueProvider"/> implementations.</param>
        /// <returns><b>true</b> if the provider provides the specified route value; otherwise <b>false</b>.</returns>
        bool CanProvideRouteValue(string key, Func<string, object> getValue);

        /// <summary>
        /// Returns the requested route value.
        /// </summary>
        /// <param name="key">The key of the requested route value.</param>
        /// <param name="getValue">A function that retrieves existing route values, including route values provided by other <see cref="IRouteValueProvider"/> implementations.</param>
        /// <param name="setValue">An action that sets a new route value.</param>
        void ProvideRouteValue(string key, Func<string, object> getValue, Action<string, object> setValue);
    }
}