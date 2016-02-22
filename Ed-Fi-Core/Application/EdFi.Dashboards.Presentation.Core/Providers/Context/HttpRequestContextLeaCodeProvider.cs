using EdFi.Dashboards.Core;
using EdFi.Dashboards.Core.Providers.Context;
using EdFi.Dashboards.Infrastructure;
using EdFi.Dashboards.Presentation.Architecture.Mvc.ValueProviders;
using EdFi.Dashboards.Resources.LocalEducationAgency;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace EdFi.Dashboards.Presentation.Core.Providers.Context
{
	public class HttpRequestContextLeaCodeProvider : LeaCodeProviderChainOfResponsibilityBase
	{
		private const string requestItemKey = "LocalEducationAgencyId";

		public HttpRequestContextLeaCodeProvider(IIdCodeService idCodeService, IHttpRequestProvider httpRequestProvider, ILocalEducationAgencyContextProvider next)
			: base(idCodeService, httpRequestProvider, next) { }

		public override string GetLeaCode(LeaCodeRequest request)
		{
			if (HttpContext.Current.Request.ContentType.ToLowerInvariant() == "application/json")
			{
				var bytes = new byte[HttpContext.Current.Request.InputStream.Length];
				
				HttpContext.Current.Request.InputStream.Position = 0;
				HttpContext.Current.Request.InputStream.Read(bytes, 0, bytes.Length);
				HttpContext.Current.Request.InputStream.Position = 0;
				
				var requestContext = Encoding.ASCII.GetString(bytes);

				if (!string.IsNullOrEmpty(requestContext))
				{
					var requestArray = Newtonsoft.Json.Linq.JObject.Parse(requestContext);
					if (requestArray[requestItemKey] != null)
						return (string) requestArray[requestItemKey];
				}
			}

			return null;
		}

		protected override string HandleRequest(LeaCodeRequest request)
		{
			var id = int.Parse(GetLeaCode(request)); //actually returns the id not the code
			var result = this.idCodeService.Get(IdCodeRequest.Create(id));
			
			if (result != null)
				return result.Code;

			throw new LocalEducationAgencyNotFoundException("The local education agency " + id + " could not be found for request to " + this.httpRequestProvider.Url);
		}
	}
}
