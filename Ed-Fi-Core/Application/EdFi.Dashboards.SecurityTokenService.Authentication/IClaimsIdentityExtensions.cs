// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;
using System.Linq;
using EdFi.Dashboards.Common.Utilities;
using Microsoft.IdentityModel.Claims;

namespace EdFi.Dashboards.SecurityTokenService.Authentication
{
	public static class IClaimsIdentityExtensions
	{
		private static T GetClaimValue<T>(IClaimsIdentity identity, string claimType)
		{
			Type type = typeof(T);

			var claim = identity.Claims.FirstOrDefault(c => c.ClaimType == claimType);

			// No claim? Return default value for the type.
			if (claim == null)
				return (T) ReflectionUtility.GetDefault(type);

			if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
			{
				// Unwrap the nullable type to the underlying value type
				type = type.GetGenericArguments()[0]; // Nullable types should only have 1 generic argument
			}

			return (T) Convert.ChangeType(claim.Value, type);
		}

		public static string FullName(this IClaimsIdentity identity)
		{
			return GetClaimValue<string>(identity, EdFiClaimTypes.FullName);
		}

		public static string FirstName(this IClaimsIdentity identity)
		{
			return GetClaimValue<string>(identity, ClaimTypes.GivenName);
		}

		public static string LastName(this IClaimsIdentity identity)
		{
			return GetClaimValue<string>(identity, ClaimTypes.Surname);
		}

		public static long? StaffUSI(this IClaimsIdentity identity)
		{
			return GetClaimValue<int?>(identity, EdFiClaimTypes.StaffUSI);
		}

		public static int? LocalEducationAgencyId(this IClaimsIdentity identity)
		{
			return GetClaimValue<int?>(identity, EdFiClaimTypes.LocalEducationAgencyId);
		}

		public static string Email(this IClaimsIdentity identity)
		{
			return GetClaimValue<string>(identity, ClaimTypes.Email);
		}

        // Added by Vinoth.N For Getting Site Login Type VINLOGINTYP
        public static string ServiceType(this IClaimsIdentity identity)
        {
            return GetClaimValue<string>(identity, EdFiClaimTypes.ServiceType);
        }

        // Added by Vinoth.N For Getting UserType EDFIDB-139
        public static int? UserType(this IClaimsIdentity identity)
        {
            return GetClaimValue<int?>(identity, EdFiClaimTypes.UserType);
        }


        // Added by Vinoth.N For Getting Site Login Type VIN05112015
        public static string UserId(this IClaimsIdentity identity)
        {
            return GetClaimValue<string>(identity, EdFiClaimTypes.UserId);
        }

        // Added by Vinoth.N For Getting Site Login Type VIN05112015
        public static string UserToken(this IClaimsIdentity identity)
        {
            return GetClaimValue<string>(identity, EdFiClaimTypes.UserToken);
        }
	}
}
