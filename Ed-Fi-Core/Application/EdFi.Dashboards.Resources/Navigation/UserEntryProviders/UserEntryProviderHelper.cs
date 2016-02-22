// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System.Collections.Generic;
using System.Linq;
using EdFi.Dashboards.Data.Entities;
using EdFi.Dashboards.Data.Repository;
using EdFi.Dashboards.SecurityTokenService.Authentication;

namespace EdFi.Dashboards.Resources.Navigation.UserEntryProviders
{
    public static class UserEntryProviderHelper
    {
        public static int? GetAssociatedLocalEducationAgencyWithClaim(UserInformation userInfo, string claimType, EntryRequest request)
        {
            if (!request.LocalEducationAgencyId.HasValue)
            {
                return (from lea in userInfo.AssociatedLocalEducationAgencies
                        where lea.ClaimTypes.Any(ct => ct == claimType)
                        select (int?)lea.LocalEducationAgencyId)
                    .FirstOrDefault();
            }

            return (from lea in userInfo.AssociatedLocalEducationAgencies
                    where lea.ClaimTypes.Any(ct => ct == claimType)
                          && lea.LocalEducationAgencyId == request.LocalEducationAgencyId
                    select (int?)lea.LocalEducationAgencyId)
                .FirstOrDefault();
        }

        public static IEnumerable<AssociatedSchool> GetAssociatedSchoolsWithClaim(UserInformation userInfo, string claimType, EntryRequest request)
        {
            if (!request.LocalEducationAgencyId.HasValue)
            {
                return
                    (from school in userInfo.AssociatedSchools
                     where school.ClaimTypes.Any(ct => ct == claimType)
                     select new AssociatedSchool
                     {
                         LocalEducationAgencyId = school.LocalEducationAgencyId,
                         SchoolId = school.SchoolId,
                         SchoolName = school.Name
                     });
            }

            return
                (from school in userInfo.AssociatedSchools
                 where school.ClaimTypes.Any(ct => ct == claimType) && school.LocalEducationAgencyId == request.LocalEducationAgencyId
                 select new AssociatedSchool
                 {
                     LocalEducationAgencyId = school.LocalEducationAgencyId,
                     SchoolId = school.SchoolId,
                     SchoolName = school.Name
                 });
        }

        public static IEnumerable<AssociatedSchool> GetValidAssociatedSchools(IEnumerable<AssociatedSchool> associatedSchools, int? schoolId)
        {
            //if we have a schoolId, only return the school with that ID, otherwise return the list as-is
            return schoolId != null ? associatedSchools.Where(x => x.SchoolId == schoolId) : associatedSchools;
        }

        public static bool HasStudents(IRepository<StaffStudentAssociation> staffStudentAssociationRepository, AssociatedSchool associatedSchool, UserInformation userInformation)
        {
            var staffStudenAssociation = (from ssa in staffStudentAssociationRepository.GetAll()
                                          where ssa.StaffUSI == userInformation.StaffUSI &&
                                                ssa.SchoolId == associatedSchool.SchoolId
                                          select ssa).FirstOrDefault();

            return staffStudenAssociation != null;
        }

        public class AssociatedSchool
        {
            public int LocalEducationAgencyId { get; set; }
            public int SchoolId { get; set; }
            public string SchoolName { get; set; }
        }
    }
}