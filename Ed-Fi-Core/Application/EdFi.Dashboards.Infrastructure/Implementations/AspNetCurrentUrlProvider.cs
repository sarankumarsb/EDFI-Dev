// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;
using System.Web;

namespace EdFi.Dashboards.Infrastructure.Implementations
{
    public class AspNetCurrentUrlProvider : ICurrentUrlProvider
    {
        public IConfigValueProvider ConfigValueProvider { get; set; }

        public Uri Url
        {
            get
            {
                var url = HttpContext.Current.Request.Url;
                var urlBuilder = new UriBuilder(url);

                var forwardedProto = HttpContext.Current.Request.Headers["X-Forwarded-Proto"];
                if (!String.IsNullOrWhiteSpace(forwardedProto))
                    urlBuilder.Scheme = forwardedProto;


                string proxyHost = ConfigValueProvider.GetValue("proxyHost");
                if (!string.IsNullOrEmpty(proxyHost))
                {
                   urlBuilder.Host = proxyHost;
                }

                var forwardedPort = HttpContext.Current.Request.Headers["X-Forwarded-Port"];
                if (!String.IsNullOrWhiteSpace(forwardedPort))
                {
                    var port = Convert.ToInt32(forwardedPort);
                    urlBuilder.Port = port;
                }

                return urlBuilder.Uri;
            }
        }
    }
}
