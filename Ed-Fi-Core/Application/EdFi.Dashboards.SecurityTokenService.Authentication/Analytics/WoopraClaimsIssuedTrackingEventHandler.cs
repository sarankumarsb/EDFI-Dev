using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Threading;
using log4net;
using Microsoft.IdentityModel.Claims;
using Newtonsoft.Json;

namespace EdFi.Dashboards.SecurityTokenService.Authentication.Analytics
{
    public class WoopraClaimsIssuedTrackingEventHandler : IClaimsIssuedTrackingEventHandler
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(WoopraClaimsIssuedTrackingEventHandler));
        private const string cookie = "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA";
        private const string trackingFormat = "http://{0}.woopra-ns.com/ce/?response=xml&cookie={1}&meta={2}&ce_name=login&ce_UserName={4}-{3}&ce_UserUSI={4}&ce_LocalEducationAgencyId={5}=ce_UsageStudy={4}|{5}|login";

        public void Track(string username, long userUSI, bool isImpersonating, IEnumerable<Claim> claims)
        {
            if (isImpersonating || userUSI < 0 || claims == null)
                return;

            var domainTracking = ConfigurationManager.AppSettings["WoopraDomainTracking"];
            if (String.IsNullOrWhiteSpace(domainTracking))
                return;

            var fullNameClaim = claims.FirstOrDefault(x => x.ClaimType == EdFiClaimTypes.FullName);
            if (fullNameClaim == null)
                return;
            var userName = fullNameClaim.Value;

            var accessOrganization = claims.FirstOrDefault(x => x.ClaimType == EdFiClaimTypes.AccessOrganization);
            if (accessOrganization == null)
                return;
            dynamic organization = JsonConvert.DeserializeObject(accessOrganization.Value);
            var localEducationAgencyId = organization[EdFiClaimProperties.LocalEducationAgencyId];
            if (localEducationAgencyId == null)
                return;

            var trackingUrl = String.Format(trackingFormat, domainTracking, cookie, String.Empty, userName, userUSI, localEducationAgencyId);
            logger.Info(trackingUrl); 
            ThreadPool.QueueUserWorkItem(x => {
                                                    try
                                                    {
                                                        var httpRequest = (HttpWebRequest)WebRequest.Create(trackingUrl);
                                                        httpRequest.Timeout = 5 * 1000;
                                                        using (var response = httpRequest.GetResponse())
                                                        {
                                                            response.Close();
                                                        }
                                                    }
                                                    catch (Exception e)
                                                    {
                                                        logger.Info(String.Empty, e);
                                                    }
                                                });
        }
    }
}