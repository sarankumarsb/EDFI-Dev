// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System.Collections.Generic;
using System.Web.Security;

namespace EdFi.Dashboards.Infrastructure
{
	public interface IFormsAuthenticationProvider
	{
		/// <summary>
		/// Issues necessary encrypted authentication ticket in a cookie. 
		/// </summary>
		/// <param name="username">The username associated with the authentication.</param>
		/// <param name="roleNames">The roles to store in the encrypted cookie.</param>
        /// <param name="localEducationAgencyId">The Local Education Agency id of the authenticated username</param>
		void PerformBrowserLogin(string username, string[] roleNames, int localEducationAgencyId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="roleNames"></param>
        /// <param name="localEducationAgencyId"></param>
        /// <returns></returns>
	    bool LoadBrowserLogin(out string userName, out string[] roleNames, out int localEducationAgencyId);
	}
}
