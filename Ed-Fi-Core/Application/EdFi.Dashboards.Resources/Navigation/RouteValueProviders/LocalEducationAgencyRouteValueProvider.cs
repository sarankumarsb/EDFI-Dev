// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using EdFi.Dashboards.Resources.LocalEducationAgency;
using EdFi.Dashboards.Resources.Models.LocalEducationAgency;

namespace EdFi.Dashboards.Resources.Navigation.RouteValueProviders
{
    public class LocalEducationAgencyRouteValueProvider : IRouteValueProvider
    {
        private readonly IIdCodeService service;

        public LocalEducationAgencyRouteValueProvider(IIdCodeService service)
        {
            this.service = service;
        }

        public bool CanProvideRouteValue(string key, Func<string, object> getValue)
        {
            if (key.Equals("localEducationAgency", StringComparison.OrdinalIgnoreCase)
                && GetLocalEducationAgencyId(getValue) != 0)
            {
                return true;
            }

            return false;
        }

        private static int GetLocalEducationAgencyId(Func<string, object> getValue)
        {
            return Convert.ToInt32(getValue("localEducationAgencyId"));
        }

        private static readonly Regex hyphenater = new Regex(@"[^\w]", RegexOptions.Compiled);

        private readonly ConcurrentDictionary<int, string> codeModelsById = new ConcurrentDictionary<int, string>();

        public void ProvideRouteValue(string key, Func<string, object> getValue, Action<string, object> setValue)
        {
            // Read the Id
            int localEducationAgencyId = Convert.ToInt32(getValue("localEducationAgencyId"));
            string localEducationAgencyCode = codeModelsById.GetOrAdd(localEducationAgencyId, GetLocalEducationAgencyCode);
            setValue("localEducationAgency", localEducationAgencyCode);
        }

        private string GetLocalEducationAgencyCode(int localEducationAgencyId)
        {
            var model = service.Get(IdCodeRequest.Create(id:localEducationAgencyId));

            if (model == null)
				throw new InvalidOperationException(string.Format("Local education agency with Id '{0}' could not be found.", localEducationAgencyId));

            return hyphenater.Replace(model.Code, "-");
        }
    }
}
