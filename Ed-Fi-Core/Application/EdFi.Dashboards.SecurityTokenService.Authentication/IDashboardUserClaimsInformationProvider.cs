// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;
using System.Collections.Generic;

namespace EdFi.Dashboards.SecurityTokenService.Authentication
{
	/// <summary>
	/// Defines a method for obtaining the information necessary to create the dashboard claims in the absence of an external identity provider (IP) supplying the required application claims.
	/// </summary>
    public interface IDashboardUserClaimsInformationProvider<TUserSecurityDetails> where TUserSecurityDetails : IErrorLogOutput
	{
	    /// <summary>
	    /// Gets information required to supply all required dashboard application claims.
	    /// </summary>
	    /// <param name="username">The username of the user.</param>
	    /// <param name="userId">The userId of the user.</param>
	    /// <returns>A <see cref="DashboardUserClaimsInformation{T}"/> instance.</returns>
	    DashboardUserClaimsInformation<TUserSecurityDetails> GetClaimsInformation(string username, long userId);
	}

	/// <summary>
	/// Provides information necessary for creating the necessary dashboard claims when there is 
	/// no identity provider (IP) available providing external origination of required application claims.
	/// </summary>
    public class DashboardUserClaimsInformation<TUserSecurityDetails>
	{
		/// <summary>
		/// Gets or sets the user's staff identifier.
		/// </summary>
		public long? StaffUSI { get; set; }

        public string Email { get; set; }

		/// <summary>
		/// Gets or sets the user's first name.
		/// </summary>
		public string FirstName { get; set; }

		/// <summary>
		/// Gets or sets the user's last name.
		/// </summary>
		public string LastName { get; set; }

		/// <summary>
		/// Gets or sets the user's full name.
		/// </summary>
		public string FullName { get; set; }

        /// <summary>
        /// Gets or sets the Dashboard Service Type.
        /// </summary>
        public string ServiceType { get; set; } // VINLOGINTYP

        /// <summary>
        /// Gets or sets the Dashboard User Type.
        /// </summary>
        public int UserType { get; set; } // EDFIDB-139

        /// <summary>
        /// Gets or sets the Dashboard Service Type.
        /// </summary>
        public string UserId { get; set; } // VIN05112015

        /// <summary>
        /// Gets or sets the Dashboard Service Type.
        /// </summary>
        public string UserToken { get; set; } // VIN05112015

		/// <summary>
		/// Gets or sets the organizations with which the user is associated.
		/// </summary>
		public IEnumerable<AssociatedOrganization> AssociatedOrganizations { get; set; }

		/// <summary>
		/// Provides values identifying the organization, and the nature of the relationship between the organization and the user.
		/// </summary>
		public class AssociatedOrganization
		{
			/// <summary>
			/// Gets or sets the identifiers of the organization.
			/// </summary>
			public EducationOrganizationIdentifier Ids { get; set; }

			/// <summary>
			/// Gets or sets the user's security details with the organization.
			/// </summary>
            public TUserSecurityDetails SecurityDetails { get; set; }
		}
	}
}