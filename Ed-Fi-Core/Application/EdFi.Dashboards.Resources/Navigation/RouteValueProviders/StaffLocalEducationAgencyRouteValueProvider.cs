// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;
using System.Text.RegularExpressions;
using EdFi.Dashboards.Common.Utilities;
using EdFi.Dashboards.Resources.Staff;

namespace EdFi.Dashboards.Resources.Navigation.RouteValueProviders
{
    public class StaffLocalEducationAgencyRouteValueProvider : IRouteValueProvider
    {
        public bool CanProvideRouteValue(string key, Func<string, object> getValue)
        {
            if (key.Equals("staff", StringComparison.OrdinalIgnoreCase))
            {
                if (GetStaffUSI(getValue) != 0 && GetLocalEducationAgencyId(getValue) != 0 && GetSchoolId(getValue) == 0)
                    return true;
            }

            return false;
        }

        public void ProvideRouteValue(string key, Func<string, object> getValue, Action<string, object> setValue)
        {
            var staffBriefService = IoC.Resolve<IBriefLocalEducationAgencyStaffService>();
            var brief = staffBriefService.Get(new BriefLocalEducationAgencyStaffRequest() { StaffUSI = GetStaffUSI(getValue), LocalEducationAgencyId = GetLocalEducationAgencyId(getValue) });

            setValue("staff", Regex.Replace(brief.FullName, @"[^\w]", "-"));
        }

		private static int GetStaffUSI(Func<string, object> getValue)
        {
            return Convert.ToInt32(getValue("staffUSI"));
        }

		private static int GetLocalEducationAgencyId(Func<string, object> getValue)
        {
            return Convert.ToInt32(getValue("localEducationAgencyId"));
        }

		private static int GetSchoolId(Func<string, object> getValue)
        {
            return Convert.ToInt32(getValue("schoolId"));
        }
    }
}
