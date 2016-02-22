// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;
using EdFi.Dashboards.Infrastructure;

namespace EdFi.Dashboards.Resources.Common
{
    public interface IUniqueListIdProvider
    {
        string GetUniqueId(Uri url);
        string GetUniqueId(int baseId);
        string GetUniqueId();
    }

    public class UniqueListIdProvider:IUniqueListIdProvider
    {
        private readonly IHttpRequestProvider httpRequestProvider;

        public UniqueListIdProvider(IHttpRequestProvider httpRequestProvider)
        {
            this.httpRequestProvider = httpRequestProvider;
        }

        //When called with the baseId it is always a drilldown from a metric so we need to call the referrer page.
        public string GetUniqueId(int baseId)
        {
            var page = GetUniqueId(httpRequestProvider.UrlReferrer);
            return string.IsNullOrEmpty(page) ? string.Empty : page + baseId;
        }

        //When called without the baseId then it is the current page.
        public string GetUniqueId()
        {
            return GetUniqueId(httpRequestProvider.Url);
        }

        public string GetUniqueId(Uri url)
        {
            return url == null ? string.Empty : GetPagePortion(url);
        }

        private static string GetPagePortion(Uri uri)
        {
            var path = uri.AbsolutePath;
            return path.Substring(path.LastIndexOf("/") + 1).Replace(".", "");
        }
    }
}
