// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Security;

namespace EdFi.Dashboards.Infrastructure.Implementations
{
	public class FormsAuthenticationProvider : IFormsAuthenticationProvider
	{
	    private const char listSeperator = '^';
        private const char dataSeperator = '|';

	    private readonly ICookieProvider cookieProvider;

        public FormsAuthenticationProvider(ICookieProvider cookieProvider)
        {
            this.cookieProvider = cookieProvider;
        }

		public void PerformBrowserLogin(string username, string[] roleNames, int localEducationAgencyId )
		{
            var roleNamesForTicket = string.Join(dataSeperator.ToString(), roleNames);

            var userData = roleNamesForTicket + listSeperator + localEducationAgencyId;
            // Create the authentication ticket
			var authTicket =
				new FormsAuthenticationTicket(
					1, // version
					username,
					DateTime.Now,
					DateTime.Now.Add(FormsAuthentication.Timeout), false, userData);

			// Encrypt the ticket
			string encryptedTicket = FormsAuthentication.Encrypt(authTicket);

			// Create a cookie and add the cookie to the response
			var authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);
			//HttpContext.Current.Response.Cookies.Add(authCookie);
		    cookieProvider.Add(authCookie);
		}

        public bool LoadBrowserLogin(out string userName, out string[] roleNames, out int localEducationAgencyId)
        {
            userName = string.Empty;
            roleNames = new string[] {};
            localEducationAgencyId = -1;

            // Extract the forms authentication cookie
            string cookieName = FormsAuthentication.FormsCookieName;

            HttpCookie authCookie = cookieProvider.Retrieve(cookieName);// HttpContext.Current.Request.Cookies[cookieName];

            // Quit if there is no authentication cookie or Session has been lost
            if (authCookie == null)
                return false;

            FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);

            // Quit if the cookie failed to decrypt
            if (authTicket == null || authTicket.Expired)
                return false;

            userName = authTicket.Name;
            string[] dataSections = authTicket.UserData.Split(listSeperator);
            roleNames = dataSections[0].Split(dataSeperator);
            localEducationAgencyId = Convert.ToInt32(dataSections[1]);

            return true;
        }
	}
}