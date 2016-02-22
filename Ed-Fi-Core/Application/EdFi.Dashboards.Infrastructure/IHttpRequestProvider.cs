// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;
using System.Web.Routing;

namespace EdFi.Dashboards.Infrastructure
{
    public interface IHttpRequestProvider
    {
        /// <summary>
        /// Gets the specified value from the current request.
        /// </summary>
        /// <param name="name">The name of the value to be retrieved.</param>
        /// <returns>The value if it exists; otherwise null/</returns>
        string GetValue(string name);

        /// <summary>
        /// Gets the values in the current request.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        string this[string name] { get; }

        Uri Url { get; }

        Uri UrlReferrer { get; }

        RequestContext RequestContext { get; }
    }
}
