// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using EdFi.Dashboards.SecurityTokenService.Authentication;
using Microsoft.IdentityModel.Claims;
using Newtonsoft.Json;

namespace EdFi.Dashboards.Resources.Security
{
    public class ClaimHelper
    {
        private static ConcurrentDictionary<string, string> claimByName = new ConcurrentDictionary<string, string>();

        public static Claim CreateClaim(string claimType, EducationOrganizationIdentifier ids)
        {
            var properties = new Dictionary<string, string>();

            if (null == ids) return null;

            var leaId = (ids.LocalEducationAgencyId != null) ? ids.LocalEducationAgencyId.Value.ToString() : null;
            var schId = (ids.SchoolId != null) ? ids.SchoolId.Value.ToString() : null;
            var saId = (ids.StateAgencyId != null) ? ids.StateAgencyId.Value.ToString() : null;
            
            properties[EdFiClaimProperties.StateAgencyId] = saId;
            properties[EdFiClaimProperties.SchoolId] = schId;
            properties[EdFiClaimProperties.LocalEducationAgencyId] = leaId;
            properties[EdFiClaimProperties.EducationOrganizationName] = ids.EducationOrganizationName;

            string propertiesValue = JsonConvert.SerializeObject(properties);
            var claim = new Claim(claimType, propertiesValue);

            return claim;
        }

        public static string GetClaimValueByName(string shortName)
        {
            return claimByName.GetOrAdd(shortName,
                n =>
                    {
                        var fieldInfo = typeof(EdFiClaimTypes).GetField(shortName);
                        return fieldInfo.GetValue(null).ToString();
                    });
        }

        /// <summary>
        /// Returns a list of claim values from a comma separated string of claim names.
        /// </summary>
        /// <param name="names"></param>
        /// <returns></returns>
        public static List<string> GetClaimValuesByNames(string names)
        {
            var shortNames = names.Split(',');
            var result = (from shortName in shortNames
                          select GetClaimValueByName(shortName)).ToList();

            return result;
        }

        public static string GetClaimShortName(string claimUrl)
        {
            if (null == claimUrl) return null;

            var path = new Uri(claimUrl).AbsolutePath;
            var name = Path.GetFileName(path);

            if (null == name) return null;
            var key = name.ToLower();

            return key;
        }
    }
}
