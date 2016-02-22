// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System.Collections.Generic;
using Microsoft.IdentityModel.Claims;

namespace EdFi.Dashboards.SecurityTokenService.Authentication
{
	/// <summary>
	/// Defines a method for converting dashboard application roles to application-specific claims.
	/// </summary>
	public interface IClaimSetBasedClaimsProvider<TClaimsSet>
	{
		/// <summary>
		/// Gets the claims for the specified dashboard application ClaimsSet.
		/// </summary>
		/// <param name="claimsSet">The <see cref="ClaimsSet"/> for which application-specific claims should be created.</param>
		/// <param name="educationOrganizationIdentifier">The identifiers for the Education Organization to which the claims will apply.</param>
		/// <returns>An enumerable collection of <see cref="Claim"/> instances.</returns>
		IEnumerable<Claim> GetClaims(TClaimsSet claimsSet, EducationOrganizationIdentifier educationOrganizationIdentifier);
	}

	/// <summary>
	/// Provides education organization identifiers for use in creating claims.
	/// </summary>
	public class EducationOrganizationIdentifier
	{
        /// <summary>
        /// Gets or sets a State Agency identifier.
        /// </summary>
        public int? StateAgencyId { get; set; }
        
        /// <summary>
		/// Gets or sets a Local Education Agency identifier.
		/// </summary>
		public int? LocalEducationAgencyId { get; set; }

		/// <summary>
		/// Gets or sets a School identifier.
		/// </summary>
        public int? SchoolId { get; set; }

        /// <summary>
        /// Gets or sets the name of the Education Organization
        /// </summary>
        public string EducationOrganizationName { get; set; }
	}
}
