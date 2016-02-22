// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using EdFi.Dashboards.Resources.Models.School;
using EdFi.Dashboards.Resources.School;

namespace EdFi.Dashboards.Resources.Navigation.RouteValueProviders
{
    public class SchoolRouteValueProvider : IRouteValueProvider
    {
        private readonly IIdNameService schoolIdNameService;

        public SchoolRouteValueProvider(IIdNameService schoolIdNameService)
        {
            this.schoolIdNameService = schoolIdNameService;
        }

        public bool CanProvideRouteValue(string key, Func<string, object> getValue)
        {
            // We can provide the school name and local education agency, for the route (but only if we have access to the school id)
            if (key.Equals("school", StringComparison.OrdinalIgnoreCase)
                || key.Equals("localEducationAgencyId", StringComparison.OrdinalIgnoreCase))
            {
                if (GetSchoolId(getValue) != 0)
                    return true;
            }

            return false;
        }

		private static int GetSchoolId(Func<string, object> getValue)
        {
            return Convert.ToInt32(getValue("schoolId"));
        }

        public void ProvideRouteValue(string key, Func<string, object> getValue, Action<string, object> setValue)
        {
            if (key.Equals("localEducationAgencyId", StringComparison.OrdinalIgnoreCase))
                ProvideLocalEducationAgencyId(getValue, setValue);

            if (key.Equals("school", StringComparison.OrdinalIgnoreCase))
                ProvideSchool(getValue, setValue);
        }

        private void ProvideSchool(Func<string, object> getValue, Action<string, object> setValue)
        {
            int schoolId = Convert.ToInt32(getValue("schoolId"));
            var model = schoolModelsById.GetOrAdd(schoolId, GetSchoolIdName);

            setValue("school", GetHyphenatedSchoolName(model.Name));
        }

        private static ConcurrentDictionary<string, string> hyphenatedSchoolNamesBySchoolName = new ConcurrentDictionary<string, string>();
        private static readonly Regex hyphenater = new Regex(@"[^\w]", RegexOptions.Compiled);

        private static string GetHyphenatedSchoolName(string schoolName)
        {
            return hyphenatedSchoolNamesBySchoolName.GetOrAdd(schoolName, 
                n => hyphenater.Replace(schoolName, "-"));
        }

        private void ProvideLocalEducationAgencyId(Func<string, object> getValue, Action<string, object> setValue) 
        {
            int schoolId = Convert.ToInt32(getValue("schoolId"));
            var model = schoolModelsById.GetOrAdd(schoolId, GetSchoolIdName);
            
            setValue("localEducationAgencyId", model.LocalEducationAgencyId);
        }

        private readonly ConcurrentDictionary<int, IdNameModel> schoolModelsById = new ConcurrentDictionary<int, IdNameModel>();

        private IdNameModel GetSchoolIdName(int schoolId)
        {
            return schoolIdNameService.Get(IdNameRequest.Create(schoolId));
        }
    }
}
