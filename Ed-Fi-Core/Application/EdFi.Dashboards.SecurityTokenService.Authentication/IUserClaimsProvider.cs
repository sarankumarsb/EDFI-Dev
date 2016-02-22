// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System.Collections.Generic;
using Microsoft.IdentityModel.Claims;

namespace EdFi.Dashboards.SecurityTokenService.Authentication
{
	/// <summary>
	/// Provides a method for obtaining application-specific <see cref="Claim">Claims</see>	for the user by username.
	/// </summary>
	/// <remarks>Implementers of this interface should return application-specific claims only (everything but the claims for the username).</remarks>
	public interface IUserClaimsProvider
	{
	    /// <summary>
	    /// Gets a collection of <see cref="Claim"/> instances for the specified user to be added to the <see cref="IClaimsIdentity"/> instance being constructed.
	    /// </summary>
	    /// <param name="username">The username of the user.</param>
	    /// <param name="staffUSI"></param>
	    /// <returns>A collection of <see cref="Claim"/> instances.</returns>
	    //IEnumerable<Claim> GetApplicationSpecificClaims(string username, string emailAddress);
        IEnumerable<Claim> GetApplicationSpecificClaims(string username, long staffUSI);
	}
}