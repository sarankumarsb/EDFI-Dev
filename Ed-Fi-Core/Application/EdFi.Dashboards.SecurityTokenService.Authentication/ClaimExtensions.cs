// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EdFi.Dashboards.Common;
using EdFi.Dashboards.Common.Utilities;
using Microsoft.IdentityModel.Claims;

namespace EdFi.Dashboards.SecurityTokenService.Authentication
{
	public static class ClaimExtensions
	{
		public static int? LocalEducationAgencyId(this Claim claim)
		{
			return GetClaimProperty<int?>(claim, EdFiClaimProperties.LocalEducationAgencyId);
		}

		public static int? SchoolId(this Claim claim)
		{
			return GetClaimProperty<int?>(claim, EdFiClaimProperties.SchoolId);
		}

		public static string EducationOrganizationName(this Claim claim)
		{
			return GetClaimProperty<string>(claim, EdFiClaimProperties.EducationOrganizationName);
		}

		private static T GetClaimProperty<T>(Claim claim, string propertyName)
		{
			if (claim.Properties.ContainsKey(propertyName))
				return claim.Properties[propertyName].Convert<T>();

			return (T) ReflectionUtility.GetDefault(typeof(T));
		}
	}
}
