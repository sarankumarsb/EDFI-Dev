// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System.Collections.Generic;
using System.Linq;
using EdFi.Dashboards.Common;
using EdFi.Dashboards.Common.Utilities.CastleWindsorInstallers;
using EdFi.Dashboards.Data.Entities;
using EdFi.Dashboards.Data.Repository;
using EdFi.Dashboards.SecurityTokenService.Authentication;

namespace EdFi.Dashboards.Resources.Navigation.UserEntryProviders
{
    [ChainOfResponsibilityOrder(300)]
    public class UserEntryForSpecialistProvider : ChainOfResponsibilityBase<IUserEntryProvider, EntryRequest, string>, IUserEntryProvider
    {
        private readonly IStaffAreaLinks _staffLinks;
        private readonly IRepository<StaffStudentAssociation> _staffStudentAssociationRepository;

        public UserEntryForSpecialistProvider(IUserEntryProvider next, IStaffAreaLinks staffLinks, IRepository<StaffStudentAssociation> staffStudentAssociationRepository) : base(next)
        {
            _staffLinks = staffLinks;
            _staffStudentAssociationRepository = staffStudentAssociationRepository;
        }

        public string GetUserEntry(EntryRequest request)
        {
            return base.ProcessRequest(request);
        }

        protected override bool CanHandleRequest(EntryRequest request)
        {
            if (UserInformation.Current.UserType == 1) // EDFIDB-139
            {
                var associatedSchools = GetAssociatedSchools(request);
                return associatedSchools.Any(associatedSchool => UserEntryProviderHelper.HasStudents(_staffStudentAssociationRepository, associatedSchool, UserInformation.Current));
            }
            else { return false; }
        }

        protected override string HandleRequest(EntryRequest request)
        {
            var userInfo = UserInformation.Current;

            return GetAssociatedSchools(request)
                .Where(associatedSchool => UserEntryProviderHelper.HasStudents(_staffStudentAssociationRepository, associatedSchool, userInfo))
                .Select(associatedSchool => _staffLinks.Default(associatedSchool.SchoolId, userInfo.StaffUSI, userInfo.FullName))
                .FirstOrDefault();
        }

        private static IEnumerable<UserEntryProviderHelper.AssociatedSchool> GetAssociatedSchools(EntryRequest request)
        {
            var userInfo = UserInformation.Current;

            var associatedSchools =
                UserEntryProviderHelper.GetAssociatedSchoolsWithClaim(userInfo, EdFiClaimTypes.ViewMyStudents, request).Union(
                    UserEntryProviderHelper.GetAssociatedSchoolsWithClaim(userInfo, EdFiClaimTypes.ViewAllStudents, request));

            return UserEntryProviderHelper.GetValidAssociatedSchools(associatedSchools, request.SchoolId);
        }
    }
}