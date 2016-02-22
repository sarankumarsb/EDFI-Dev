// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System.Collections.Generic;

namespace EdFi.Dashboards.SecurityTokenService.Authentication
{
	/// <summary>
	/// Provides a method for determining the role of a user, based on attributes imported from the Local Education Agency's SIS.
	/// </summary>
    public interface IUserClaimSetsProvider<TClaimSet, TUserSecurityDetails> where TUserSecurityDetails : IErrorLogOutput
	{
		/// <summary>
		/// Gets a user's application roles, based on attributes imported from the Local Education Agency's SIS.
		/// </summary>
		/// <param name="userSecurityDetails">User details to be used to determine the role.</param>
		/// <returns>A collection of EdFi dashboard application roles.</returns>
        IEnumerable<TClaimSet> GetUserClaimSets(TUserSecurityDetails userSecurityDetails);
	}

	/// <summary>
	/// Holds values to be used to determine the appropriate claim set for the user.
	/// </summary>
    public class EdFiUserSecurityDetails : IErrorLogOutput
	{
		/// <summary>
		/// Gets or sets the title of the position held by the user, as defined by the Local Education Agency.
		/// </summary>
		public string PositionTitle { get; set; }

		/// <summary>
		/// Gets or sets the general staff category for the user, as derived by the EdFi ETL packages.
		/// </summary>
		public string StaffCategory { get; set; }

        public string ToErrorLogText()
        {
            return string.Format("Position Title: '{0}'  Staff Category: '{1}'", PositionTitle, StaffCategory);
        }
    }
}
