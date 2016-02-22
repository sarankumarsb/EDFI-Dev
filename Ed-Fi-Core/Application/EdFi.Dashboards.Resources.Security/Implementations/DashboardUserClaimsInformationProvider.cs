// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System.Collections.Generic;
using System.Linq;
using EdFi.Dashboards.Common;
using EdFi.Dashboards.Core;
using EdFi.Dashboards.Data.Queries;
using EdFi.Dashboards.SecurityTokenService.Authentication;

namespace EdFi.Dashboards.Resources.Security.Implementations
{
    public class DashboardUserClaimsInformationProvider : IDashboardUserClaimsInformationProvider<EdFiUserSecurityDetails>
	{
		private readonly StaffInformationAndAssociatedOrganizationsByUSIQuery query;

		public DashboardUserClaimsInformationProvider(StaffInformationAndAssociatedOrganizationsByUSIQuery query)
		{
			this.query = query;
		}

        public DashboardUserClaimsInformation<EdFiUserSecurityDetails> GetClaimsInformation(string username, long userId)
        {
            var result = query.Execute(new[] { userId });

            // Handle empty result condition
            if (result == null || result.Count() == 0)
                return null;

            // Handle multiple result condition
            // need to group these results and factor out potential multiple identification codes
            var uniqueIdresult =
                (from u in result
                    group u by new
                    {
                        u.FullName,
                        u.FirstName,
                        u.MiddleName,
                        u.LastSurname,
                        u.StaffUSI,
                        u.EmailAddress

                    }
                    into g

                    select new
                    {
                        FullName = g.Key.FullName,
                        FirstName = g.Key.FirstName,
                        MiddleName = g.Key.MiddleName,
                        LastSurname = g.Key.LastSurname,
                        StaffUSI = g.Key.StaffUSI,
                        EmailAddress = g.Key.EmailAddress
                    }).Distinct().ToList();


            if (uniqueIdresult.Count() > 1)
                throw new UserAccessDeniedException("The system was unable to resolve your user account information to a distinct staff member.  Please contact your administrator for assistance.");

            var userClaimsInfo = result.First();

            var schools = GetSchools(userClaimsInfo);
            var localEducationAgencies = GetLocalEducationAgencies(userClaimsInfo);

            var orgs = schools.Concat(localEducationAgencies);

            return new DashboardUserClaimsInformation<EdFiUserSecurityDetails>
            {
                StaffUSI = userClaimsInfo.StaffUSI,
                FirstName = userClaimsInfo.FirstName,
                LastName = userClaimsInfo.LastSurname,
                FullName = userClaimsInfo.FullName,
                AssociatedOrganizations = orgs,
                Email = userClaimsInfo.EmailAddress
            };
        }

        private static IEnumerable<DashboardUserClaimsInformation<EdFiUserSecurityDetails>.AssociatedOrganization> GetLocalEducationAgencies(StaffInformationAndAssociatedOrganizationsQueryResult userClaimsInfo)
		{
			return from org in userClaimsInfo.AssociatedOrganizations
			       where org.OrganizationCategory == EducationOrganizationCategory.LocalEducationAgency
                   select new DashboardUserClaimsInformation<EdFiUserSecurityDetails>.AssociatedOrganization
			              	{
			              		Ids = new EducationOrganizationIdentifier
			              		      	{
			              		      		LocalEducationAgencyId = org.EducationOrganizationId,
                                            EducationOrganizationName = org.Name
			              		      	},
			              		SecurityDetails = new EdFiUserSecurityDetails
			              		              	{
			              		              		PositionTitle = org.PositionTitle,
			              		              		StaffCategory = org.StaffCategory
			              		              	},
			              	};
		}

        private static IEnumerable<DashboardUserClaimsInformation<EdFiUserSecurityDetails>.AssociatedOrganization> GetSchools(StaffInformationAndAssociatedOrganizationsQueryResult userClaimsInfo)
		{
			return from org in userClaimsInfo.AssociatedOrganizations
			       where org.OrganizationCategory == EducationOrganizationCategory.School
                   select new DashboardUserClaimsInformation<EdFiUserSecurityDetails>.AssociatedOrganization
			              	{
			              		Ids = 
			              			new EducationOrganizationIdentifier
			              				{
			              					LocalEducationAgencyId = org.LocalEducationAgencyId,
			              					SchoolId = org.EducationOrganizationId,
                                            EducationOrganizationName = org.Name
			              				},
			              		SecurityDetails = 
			              			new EdFiUserSecurityDetails
			              				{
			              					PositionTitle = org.PositionTitle,
			              					StaffCategory = org.StaffCategory
			              				},
			              	};
		}
	}
}