using System.Collections.Generic;
using System.Linq;
using EdFi.Dashboards.Common.Utilities;
using EdFi.Dashboards.Core;
using EdFi.Dashboards.Data.Entities;
using EdFi.Dashboards.Data.Repository;
using EdFi.Dashboards.Resources.Security.Common;
using EdFi.Dashboards.SecurityTokenService.Authentication;

namespace EdFi.Dashboards.Resources.StudentMetrics
{
    public class SecurityFilter : IStudentMetricFilter
    {
        protected readonly IRepository<StaffStudentAssociation> StaffStudentAssociationRepository;
        protected readonly ICurrentUserClaimInterrogator CurrentUserClaimInterrogator;

        public SecurityFilter(
            IRepository<StaffStudentAssociation> staffStudentAssociationRepository,
            ICurrentUserClaimInterrogator currentUserClaimInterrogator)
        {
            StaffStudentAssociationRepository = staffStudentAssociationRepository;
            CurrentUserClaimInterrogator = currentUserClaimInterrogator;
        }


        public IQueryable<EnhancedStudentInformation> ApplyFilter(IQueryable<EnhancedStudentInformation> query, StudentMetricsProviderQueryOptions providerQueryOptions)
        {
            return ApplySecurityFilter(query, providerQueryOptions);
        }

        public IQueryable<EnhancedStudentInformation> ApplySecurityFilter(IQueryable<EnhancedStudentInformation> query, StudentMetricsProviderQueryOptions providerQueryOptions)
        {
            //The only user that can see everyone are state employees...
            if (CurrentUserClaimInterrogator.HasClaimForStateAgency(EdFiClaimTypes.ViewAllStudents))
                return query;

            //Start off with a lit of empty students we're allowed to see...
            var studentsTheUserCanSee = ExpressionExtensions.False<EnhancedStudentInformation>();

            if (UserInformation.Current == null)
                return query.Where(studentsTheUserCanSee);

            var staffUsi = UserInformation.Current.StaffUSI;

            //Walk through all their associated ed-orgs...
            foreach (var associatedOrganization in UserInformation.Current.AssociatedOrganizations)
            {
                //Find what they have permissions to, and let them see just those students.
                foreach (var claimType in associatedOrganization.ClaimTypes)
                {
                    var educationOrganizationId = associatedOrganization.EducationOrganizationId;

                    // TODO: Find the value for the state id...in dev it is set to int.MaxValue so this will work for now
                    if (educationOrganizationId == int.MaxValue)
                        continue;

                    int[] leaIds;
                    switch (associatedOrganization.Category)
                    {
                        case EducationOrganizationCategory.LocalEducationAgency:
                            leaIds = new[] { educationOrganizationId };
                            break;
                        default:
                            leaIds = new int[0];
                            break;
                    }

                    switch (claimType)
                    {

                            //People with ViewAllStudents can see any user associated with their ed-org
                        case EdFiClaimTypes.ViewAllStudents:
                            //subsonic's contains is broken, so while we don't need this if statement for it to work in linq, we do
                            // need it for it to work with subsonic.  Once we get off subsonic we just need the if statement.
                            if (leaIds.Any())
                            {
                                studentsTheUserCanSee = studentsTheUserCanSee.Or(
                                    student => student.SchoolId == educationOrganizationId
                                               || leaIds.Contains(student.LocalEducationAgencyId));
                            }
                            else
                            {
                                studentsTheUserCanSee = studentsTheUserCanSee.Or(
                                    student => student.SchoolId == educationOrganizationId);
                            }
                            break;
                            //People with ViewMyStudents can see their students, or students that are part of their cohort.
                        case EdFiClaimTypes.ViewMyStudents:
                            //Logically, the goal of this code is to do studentsTheUserCanSee.Or(student => myStudents.Contains(student.StudentUSI)), but we have
                            //   to hack around subsonic, so this is phrased oddly.
                            studentsTheUserCanSee =
                                studentsTheUserCanSee.Or(student => StaffStudentAssociationRepository.GetAll()
                                    .Any(ssa => ssa.StudentUSI == student.StudentUSI
                                                && ssa.StaffUSI == staffUsi
                                                && ssa.SchoolId == educationOrganizationId));

                            break;
                    }
                }
            }

            return query.Where(studentsTheUserCanSee);
        }
    }
}