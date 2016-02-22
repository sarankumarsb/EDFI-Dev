// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System;
using System.Collections.Generic;
using EdFi.Dashboards.Common;
using EdFi.Dashboards.SecurityTokenService.Authentication;
using Microsoft.IdentityModel.Claims;

namespace EdFi.Dashboards.Resources.Security.Implementations
{
    public class ClaimSetBasedClaimsProvider : IClaimSetBasedClaimsProvider<ClaimsSet>
    {
	    public IEnumerable<Claim> GetClaims(ClaimsSet claimsSet, EducationOrganizationIdentifier educationOrganizationIdentifier)
	    {
            EducationOrganizationIdentifier implicitLEA = educationOrganizationIdentifier;

            if (educationOrganizationIdentifier.LocalEducationAgencyId.HasValue && educationOrganizationIdentifier.SchoolId.HasValue)
            {
                implicitLEA = new EducationOrganizationIdentifier
                                  {
                                      LocalEducationAgencyId = educationOrganizationIdentifier.LocalEducationAgencyId,
                                      SchoolId = null,
                                      EducationOrganizationName = educationOrganizationIdentifier.EducationOrganizationName
                                  };
            }

	        // Convert roles to claims
			switch (claimsSet)
            {
                case ClaimsSet.SystemAdministrator:
                    yield return ClaimHelper.CreateClaim(EdFiClaimTypes.AccessOrganization, implicitLEA);
                    yield return ClaimHelper.CreateClaim(EdFiClaimTypes.ViewAllMetrics, implicitLEA);
                    yield return ClaimHelper.CreateClaim(EdFiClaimTypes.ViewAllStudents, implicitLEA);
                    yield return ClaimHelper.CreateClaim(EdFiClaimTypes.ViewAllTeachers, implicitLEA);
                    yield return ClaimHelper.CreateClaim(EdFiClaimTypes.ViewOperationalDashboard, implicitLEA);
                    yield return ClaimHelper.CreateClaim(EdFiClaimTypes.ManageGoals, implicitLEA);
                    yield return ClaimHelper.CreateClaim(EdFiClaimTypes.AdministerDashboard, implicitLEA);
                    yield return ClaimHelper.CreateClaim(EdFiClaimTypes.ManageWatchList, implicitLEA);
                    break;

                case ClaimsSet.Superintendent:
                    yield return ClaimHelper.CreateClaim(EdFiClaimTypes.AccessOrganization, educationOrganizationIdentifier);
                    yield return ClaimHelper.CreateClaim(EdFiClaimTypes.ViewAllMetrics, educationOrganizationIdentifier);
                    yield return ClaimHelper.CreateClaim(EdFiClaimTypes.ViewAllStudents, educationOrganizationIdentifier);
                    yield return ClaimHelper.CreateClaim(EdFiClaimTypes.ViewAllTeachers, educationOrganizationIdentifier);
                    yield return ClaimHelper.CreateClaim(EdFiClaimTypes.ViewOperationalDashboard, educationOrganizationIdentifier);
                    yield return ClaimHelper.CreateClaim(EdFiClaimTypes.ManageGoals, educationOrganizationIdentifier);
                    yield return ClaimHelper.CreateClaim(EdFiClaimTypes.ManageWatchList, educationOrganizationIdentifier);
                    break;

                case ClaimsSet.Principal:
                    yield return ClaimHelper.CreateClaim(EdFiClaimTypes.AccessOrganization, educationOrganizationIdentifier);
                    yield return ClaimHelper.CreateClaim(EdFiClaimTypes.ViewAllMetrics, educationOrganizationIdentifier);
                    yield return ClaimHelper.CreateClaim(EdFiClaimTypes.ViewMyMetrics, implicitLEA);
                    yield return ClaimHelper.CreateClaim(EdFiClaimTypes.ViewAllStudents, educationOrganizationIdentifier);
                    yield return ClaimHelper.CreateClaim(EdFiClaimTypes.ViewAllTeachers, educationOrganizationIdentifier);
                    yield return ClaimHelper.CreateClaim(EdFiClaimTypes.ViewOperationalDashboard, educationOrganizationIdentifier);
                    yield return ClaimHelper.CreateClaim(EdFiClaimTypes.ManageWatchList, implicitLEA);
                    //yield return ClaimHelper.CreateClaim(EdFiClaimTypes.ViewGoals, educationOrganizationIdentifier);
                    break;

                case ClaimsSet.Administration:
                    yield return ClaimHelper.CreateClaim(EdFiClaimTypes.AccessOrganization, educationOrganizationIdentifier);
                    yield return ClaimHelper.CreateClaim(EdFiClaimTypes.ViewAllMetrics, educationOrganizationIdentifier);
                    yield return ClaimHelper.CreateClaim(EdFiClaimTypes.ViewMyMetrics, implicitLEA);
                    yield return ClaimHelper.CreateClaim(EdFiClaimTypes.ViewAllStudents, educationOrganizationIdentifier);
                    yield return ClaimHelper.CreateClaim(EdFiClaimTypes.ViewAllTeachers, educationOrganizationIdentifier);
                    yield return ClaimHelper.CreateClaim(EdFiClaimTypes.ViewOperationalDashboard, educationOrganizationIdentifier);
                    yield return ClaimHelper.CreateClaim(EdFiClaimTypes.ManageWatchList, implicitLEA);
                    break;

                case ClaimsSet.Leader:
                    yield return ClaimHelper.CreateClaim(EdFiClaimTypes.AccessOrganization, educationOrganizationIdentifier);
                    yield return ClaimHelper.CreateClaim(EdFiClaimTypes.ViewAllMetrics, educationOrganizationIdentifier);
                    yield return ClaimHelper.CreateClaim(EdFiClaimTypes.ViewMyMetrics, implicitLEA);
                    yield return ClaimHelper.CreateClaim(EdFiClaimTypes.ViewAllStudents, educationOrganizationIdentifier);
                    yield return ClaimHelper.CreateClaim(EdFiClaimTypes.ViewAllTeachers, educationOrganizationIdentifier);
                    break;

                case ClaimsSet.Specialist:
                    yield return ClaimHelper.CreateClaim(EdFiClaimTypes.AccessOrganization, educationOrganizationIdentifier);
                    yield return ClaimHelper.CreateClaim(EdFiClaimTypes.ViewMyMetrics, educationOrganizationIdentifier);
                    yield return ClaimHelper.CreateClaim(EdFiClaimTypes.ViewMyStudents, educationOrganizationIdentifier);
                    break;

                case ClaimsSet.Staff:
                    yield return ClaimHelper.CreateClaim(EdFiClaimTypes.AccessOrganization, educationOrganizationIdentifier);
                    yield return ClaimHelper.CreateClaim(EdFiClaimTypes.ViewMyMetrics, educationOrganizationIdentifier);
                    break;

                case ClaimsSet.Student: // VINSTUDLOGIN                    
                    yield return ClaimHelper.CreateClaim(EdFiClaimTypes.ViewMyStudents, educationOrganizationIdentifier);
                    yield return ClaimHelper.CreateClaim(EdFiClaimTypes.ViewMyMetrics, educationOrganizationIdentifier);
                    break;

                case ClaimsSet.Impersonation:
                    break;

				default:
                    throw new UserAccessDeniedException(String.Format("No support has been added for building claims for the '{0}' claims set.", claimsSet));
			}
		}
    }
}