// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System;
using EdFi.Dashboards.SecurityTokenService.Authentication;

namespace EdFi.Dashboards.Resources.Security.Implementations
{
	public class AlwaysValidAuthenticationProvider : IAuthenticationProvider
	{
	    public static readonly string IncorrectPassword = "wrong-password";
	    public static readonly string ErrorPassword = "error-password";

        public string UserName { get; set; } // VIN25112015
		private readonly string emailDomain;

		/// <summary>
		/// Initializes a new instance of the <see cref="AlwaysValidAuthenticationProvider"/> class.
		/// </summary>
        public AlwaysValidAuthenticationProvider() : this("domain.com") { }

	    /// <summary>
	    /// Initializes a new instance of the <see cref="AlwaysValidAuthenticationProvider"/> class using the 
	    /// specified email domain name.
	    /// </summary>
	    /// <param name="emailDomain"></param>
        public AlwaysValidAuthenticationProvider(string emailDomain)
		{
			this.emailDomain = emailDomain;
		}

        /// <summary>
        /// Attempts to validate the user's credentials with the username and password provided.
        /// </summary>
        /// <param name="username">The username of the user.</param>
        /// <param name="password">The password of the user.</param>
        /// <returns><b>true</b> if authentication is successful; otherwise <b>false</b>.</returns>
        public bool ValidateUser(string username, string password)
		{
			// Make user type something, so authentication failure condition can be seen.
            if (string.IsNullOrEmpty(password) || password == IncorrectPassword)
				return false;

            if (password == ErrorPassword)
                throw new ArgumentOutOfRangeException("password", "Error password was provided.");

			return true;
		}

        public string ResolveUsernameToLookupValue(string username, string staffInfoLookupKey)
        {
            switch (staffInfoLookupKey.ToUpper())
            {
                case "EMAILADDRESS":
                    return username + "@" + emailDomain;

                default:
                    return username;
            }             
        }

        public string ResolveLookupValueToUsername(string lookupValue)
        {
            return lookupValue.IndexOf('@') == -1 ? lookupValue : lookupValue.Remove(lookupValue.IndexOf('@'));
        }
	}
}
