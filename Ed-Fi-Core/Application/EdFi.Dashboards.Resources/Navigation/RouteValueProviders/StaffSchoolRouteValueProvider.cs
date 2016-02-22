// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;
using System.Text.RegularExpressions;
using EdFi.Dashboards.Common.Utilities;
using EdFi.Dashboards.Resources.Staff;
using EdFi.Dashboards.SecurityTokenService.Authentication;

namespace EdFi.Dashboards.Resources.Navigation.RouteValueProviders
{
    public class StaffSchoolRouteValueProvider : IRouteValueProvider
    {
        public bool CanProvideRouteValue(string key, Func<string, object> getValue)
        {
            if (key.Equals("staff", StringComparison.OrdinalIgnoreCase))
            {
                if (GetStaffUSI(getValue) != 0 && GetSchoolId(getValue) != 0)
                    return true;
            }

            return false;
        }

        public void ProvideRouteValue(string key, Func<string, object> getValue, Action<string, object> setValue)
        {
            string fullName;

            var currentUser = UserInformation.Current;

            if (GetStaffUSI(getValue) == currentUser.StaffUSI)
            {
                // currently logged user looks @ its own data
                fullName = currentUser.FullName;
            }
            else
            {
                // currently logged user looks @ someone else's data
                var staffBriefService = IoC.Resolve<IBriefService>();

                // BriefService.Get() will throw an exception if it is unable to find stuff that matched the request
                var brief =
                    staffBriefService.Get(new BriefRequest()
                                              {StaffUSI = GetStaffUSI(getValue), SchoolId = GetSchoolId(getValue)});
                fullName = brief.FullName;
            }

            setValue("staff", Regex.Replace(fullName, @"[^\w]", "-"));
        }

		private static int GetStaffUSI(Func<string, object> getValue)
        {
            return Convert.ToInt32(getValue("staffUSI"));
        }

		private static int GetSchoolId(Func<string, object> getValue)
        {
            return Convert.ToInt32(getValue("schoolId"));
        }
    }
}
