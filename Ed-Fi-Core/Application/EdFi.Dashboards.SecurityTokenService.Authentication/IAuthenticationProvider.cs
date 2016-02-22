// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
namespace EdFi.Dashboards.SecurityTokenService.Authentication
{
	/// <summary>
	/// Defines methods necessary to perform user authentication.
	/// </summary>
	public interface IAuthenticationProvider
	{
        /// <summary>
        /// Attempts to validate the user's credentials with the username and password provided.
        /// </summary>
        /// <param name="username">The username of the user.</param>
        /// <param name="password">The password of the user.</param>
        /// <returns><b>true</b> if authentication is successful; otherwise <b>false</b>.</returns>
        bool ValidateUser(string username, string password);

        /// <summary>
        /// Takes the username and then resolves it to the value
        /// needed to do a lookup to get the StaffUSI
        /// </summary>
        /// <param name="username">The name of the user</param>
        /// <param name="staffInfoLookupKey">Value in the StaffInformation table that is used to lookup data</param>
        /// <returns></returns>
        string ResolveUsernameToLookupValue(string username, string staffInfoLookupKey);

        string ResolveLookupValueToUsername(string lookupValue);


        ///// <summary>
        ///// Gets the email address of the specified user.
        ///// </summary>
        ///// <param name="username">The name of the user.</param>
        ///// <returns>The user's email address, if available; otherwise <b>null</b>.</returns>
		//string GetEmailAddress(string username);

        ///// <summary>
        ///// Gets the user name of the specified email address.
        ///// </summary>
        ///// <param name="email">The email address to look up</param>
        ///// <returns>The user's user name, if available; otherwise <b>null</b>.</returns>
        //string GetUserName(string email);

        string UserName { get; set; } // VIN25112015
	}
}