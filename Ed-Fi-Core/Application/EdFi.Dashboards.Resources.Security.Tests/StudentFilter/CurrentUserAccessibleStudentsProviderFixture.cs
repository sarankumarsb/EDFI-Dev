using System.Collections.Generic;
using System.Linq;
using EdFi.Dashboards.Resources.Security.Common;
using EdFi.Dashboards.Resources.Tests;
using EdFi.Dashboards.SecurityTokenService.Authentication;
using EdFi.Dashboards.Testing;
using NUnit.Framework;
using Rhino.Mocks;

namespace EdFi.Dashboards.Resources.Security.Tests.StudentFilter
{
    public abstract class When_getting_accessible_students_for_the_current_user : TestFixtureBase
    {
        //The Actual Model.
        protected AccessibleStudents actualModel;

        //The Actual Service/Provider.
        protected CurrentUserAccessibleStudentsProvider Provider;

        //The supplied Data models and vars.
        protected int? SuppliedEducationOrganizationId = 1000;
        protected bool SuppliedIsSearch = false;

        protected HashSet<long> SuppliedStudentUSIsForLEA = new HashSet<long> { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
        protected HashSet<long> SuppliedStudentUSIsForEdOrgViewMyStudents = new HashSet<long> { 10, 11, 12, 13, 14, 15, 16, 17, 18, 19 };
        protected HashSet<long> SuppliedStudentUSIsForEdOrgViewAllStudents = new HashSet<long> { 20, 21, 22, 23, 24, 25, 26, 27, 28, 29 };
        protected HashSet<long> SuppliedStudentUSIsForSearchClaim = new HashSet<long> { 30, 31, 32, 33, 34, 35, 36, 37, 38, 39 };


        //External dependencies to be mocked.
        protected IAuthorizationInformationProvider AuthorizationInformationProvider;
        protected ICurrentUserClaimInterrogator CurrentUserClaimInterrogator;


        protected override void EstablishContext()
        {
            AuthorizationInformationProvider = mocks.StrictMock<IAuthorizationInformationProvider>();
            CurrentUserClaimInterrogator = mocks.StrictMock<ICurrentUserClaimInterrogator>();
        }

        protected override void ExecuteTest()
        {
            Provider = new CurrentUserAccessibleStudentsProvider(AuthorizationInformationProvider, CurrentUserClaimInterrogator);
            actualModel = Provider.GetAccessibleStudents(SuppliedEducationOrganizationId, SuppliedIsSearch);
        }
    }

    public class When_getting_accessible_students_for_a_user_that_has_a_state_claim : When_getting_accessible_students_for_the_current_user
    {
        protected override void EstablishContext()
        {
            base.EstablishContext();

            SuppliedEducationOrganizationId = null;

            Expect.Call(CurrentUserClaimInterrogator.HasClaimForStateAgency(EdFiClaimTypes.ViewAllStudents)).Return(true);
        }

        [Test]
        public virtual void Should_return_all_students_are_accessible()
        {
            //Asserting 
            Assert.That(actualModel.CanAccessAllStudents, Is.EqualTo(true));
            Assert.That(actualModel.StudentUSIs.Count(), Is.EqualTo(0));
        }
    }

    public class When_getting_accessible_students_for_a_user_that_has_a_state_claim_with_a_education_organization_id : When_getting_accessible_students_for_the_current_user
    {
        protected override void EstablishContext()
        {
            base.EstablishContext();
            SuppliedIsSearch = false;
            SuppliedEducationOrganizationId = 1000;
            Expect.Call(CurrentUserClaimInterrogator.HasClaimForLocalEducationAgencyWithinEducationOrganizationHierarchy(EdFiClaimTypes.ViewAllStudents, (int)SuppliedEducationOrganizationId))
                .Return(true);
        }

        [Test]
        public virtual void Should_return_all_students_are_accessible()
        {
            //Asserting 
            Assert.That(actualModel.CanAccessAllStudents, Is.EqualTo(true));
            Assert.That(actualModel.StudentUSIs.Count(), Is.EqualTo(0));
        }
    }

    public class When_getting_accessible_students_for_a_user_that_has_a_state_claim_in_the_context_of_search_with_a_education_organization_id : When_getting_accessible_students_for_a_user_that_has_a_state_claim
    {
        protected override void EstablishContext()
        {
            base.EstablishContext();
            SuppliedIsSearch = true;
            SuppliedEducationOrganizationId = 1000;
        }
    }

    public class When_getting_accessible_students_for_a_user_that_has_a_state_claim_in_the_context_of_search_with_a_education_organization_id_that_is_null : When_getting_accessible_students_for_a_user_that_has_a_state_claim
    {
        protected override void EstablishContext()
        {
            base.EstablishContext();
            SuppliedIsSearch = true;
            SuppliedEducationOrganizationId = null;
        }
    }

    public class When_getting_accessible_students_for_a_user_that_has_NO_state_claim_and_the_education_organization_id_is_null : When_getting_accessible_students_for_the_current_user
    {
        protected override void EstablishContext()
        {
            base.EstablishContext();

            SuppliedEducationOrganizationId = null;

            Expect.Call(CurrentUserClaimInterrogator.HasClaimForStateAgency(EdFiClaimTypes.ViewAllStudents)).Return(false);
        }

        [Test]
        public virtual void Should_return_that_you_cant_see_any_students()
        {
            //Asserting 
            Assert.That(actualModel.CanAccessAllStudents, Is.EqualTo(false));
            Assert.That(actualModel.StudentUSIs.Count(), Is.EqualTo(0));
        }
    }

    public class When_getting_accessible_students_for_a_user_that_has_a_local_education_agency_claim : When_getting_accessible_students_for_the_current_user
    {
        protected override void EstablishContext()
        {
            base.EstablishContext();
            SuppliedIsSearch = false;
            Expect.Call(CurrentUserClaimInterrogator.HasClaimForLocalEducationAgencyWithinEducationOrganizationHierarchy(EdFiClaimTypes.ViewAllStudents, (int)SuppliedEducationOrganizationId)).Return(true);
        }

        [Test]
        public virtual void Should_return_all_students_are_accessible()
        {
            //Asserting 
            Assert.That(actualModel.CanAccessAllStudents, Is.EqualTo(true));
            Assert.That(actualModel.StudentUSIs.Count(), Is.EqualTo(0));
        }
    }

    public class When_getting_accessible_students_for_a_user_that_has_a_local_education_agency_claim_in_the_context_of_search : When_getting_accessible_students_for_the_current_user
    {
        protected override void EstablishContext()
        {
            base.EstablishContext();
            
            LoginHelper.LoginUserWithLocalEducationAgencyClaim();

            SuppliedIsSearch = true;

            Expect.Call(CurrentUserClaimInterrogator.HasClaimForStateAgency(EdFiClaimTypes.ViewAllStudents)).Return(false);
            Expect.Call(CurrentUserClaimInterrogator.HasClaimForSearch(EdFiClaimTypes.ViewAllStudents)).Return(true);
            Expect.Call(AuthorizationInformationProvider.GetAllStaffStudentUSIs(UserInformation.Current.StaffUSI)).Return(SuppliedStudentUSIsForLEA);
        }

        [Test]
        public virtual void Should_return_LEA_student_array()
        {
            //Asserting 
            Assert.That(actualModel.CanAccessAllStudents, Is.EqualTo(false));
            Assert.That(actualModel.StudentUSIs.Count(), Is.Not.EqualTo(0));

            foreach (var i in SuppliedStudentUSIsForLEA)
                Assert.That(actualModel.StudentUSIs.Any(x=>x==i), Is.True);
        }
    }

    public class When_getting_accessible_students_for_a_user_that_has_a_education_organization_view_all_students_claim : When_getting_accessible_students_for_the_current_user
    {
        protected override void EstablishContext()
        {
            base.EstablishContext();

            LoginHelper.LoginUserWithSchoolViewAllStudentsClaim();

            SuppliedIsSearch = false;
            Expect.Call(CurrentUserClaimInterrogator.HasClaimForLocalEducationAgencyWithinEducationOrganizationHierarchy(EdFiClaimTypes.ViewAllStudents, (int)SuppliedEducationOrganizationId)).Return(false);
            Expect.Call(CurrentUserClaimInterrogator.HasClaimWithinEducationOrganizationHierarchy(EdFiClaimTypes.ViewAllStudents, (int)SuppliedEducationOrganizationId)).Return(true);
            Expect.Call(AuthorizationInformationProvider.GetAllStaffStudentUSIs(UserInformation.Current.StaffUSI)).Return(SuppliedStudentUSIsForEdOrgViewAllStudents);
        }

        [Test]
        public virtual void Should_return_LEA_student_array()
        {
            //Asserting 
            Assert.That(actualModel.CanAccessAllStudents, Is.EqualTo(false));
            Assert.That(actualModel.StudentUSIs.Count(), Is.Not.EqualTo(0));

            foreach (var i in SuppliedStudentUSIsForEdOrgViewAllStudents)
                Assert.That(actualModel.StudentUSIs.Any(x => x == i), Is.True);
        }
    }

    public class When_getting_accessible_students_for_a_user_that_has_a_education_organization_view_all_students_claim_in_the_context_of_search :
        When_getting_accessible_students_for_the_current_user
    {
        protected override void EstablishContext()
        {
            base.EstablishContext();

            LoginHelper.LoginUserWithSchoolViewAllStudentsClaim();

            SuppliedIsSearch = true;

            Expect.Call(CurrentUserClaimInterrogator.HasClaimForStateAgency(EdFiClaimTypes.ViewAllStudents)).Return(false);
            Expect.Call(CurrentUserClaimInterrogator.HasClaimForSearch(EdFiClaimTypes.ViewAllStudents)).Return(true);
            Expect.Call(AuthorizationInformationProvider.GetAllStaffStudentUSIs(UserInformation.Current.StaffUSI)).Return(SuppliedStudentUSIsForEdOrgViewAllStudents);
        }

        [Test]
        public virtual void Should_return_LEA_student_array()
        {
            //Asserting 
            Assert.That(actualModel.CanAccessAllStudents, Is.EqualTo(false));
            Assert.That(actualModel.StudentUSIs.Count(), Is.Not.EqualTo(0));

            foreach (var i in SuppliedStudentUSIsForEdOrgViewAllStudents)
                Assert.That(actualModel.StudentUSIs.Any(x => x == i), Is.True);
        }
    }

    public class When_getting_accessible_students_for_a_user_that_has_a_education_organization_view_my_students_claim : When_getting_accessible_students_for_the_current_user
    {
        protected override void EstablishContext()
        {
            base.EstablishContext();

            LoginHelper.LoginUserWithSchoolViewMyStudentsClaim();

            SuppliedIsSearch = false;
            Expect.Call(CurrentUserClaimInterrogator.HasClaimForLocalEducationAgencyWithinEducationOrganizationHierarchy(EdFiClaimTypes.ViewAllStudents, (int)SuppliedEducationOrganizationId)).Return(false);
            Expect.Call(CurrentUserClaimInterrogator.HasClaimWithinEducationOrganizationHierarchy(EdFiClaimTypes.ViewAllStudents, (int)SuppliedEducationOrganizationId)).Return(false);
            Expect.Call(CurrentUserClaimInterrogator.HasClaimWithinEducationOrganizationHierarchy(EdFiClaimTypes.ViewMyStudents, (int)SuppliedEducationOrganizationId)).Return(true);
            Expect.Call(AuthorizationInformationProvider.GetAllStaffStudentUSIs(UserInformation.Current.StaffUSI)).Return(SuppliedStudentUSIsForEdOrgViewMyStudents);
        }

        [Test]
        public virtual void Should_return_LEA_student_array()
        {
            //Asserting 
            Assert.That(actualModel.CanAccessAllStudents, Is.EqualTo(false));
            Assert.That(actualModel.StudentUSIs.Count(), Is.Not.EqualTo(0));

            foreach (var i in SuppliedStudentUSIsForEdOrgViewMyStudents)
                Assert.That(actualModel.StudentUSIs.Any(x => x == i), Is.True);
        }
    }

    public class When_getting_accessible_students_for_a_user_that_has_a_education_organization_view_my_students_claim_in_the_context_of_search :
        When_getting_accessible_students_for_the_current_user
    {
        protected override void EstablishContext()
        {
            base.EstablishContext();

            LoginHelper.LoginUserWithSchoolViewMyStudentsClaim();

            SuppliedIsSearch = true;

            Expect.Call(CurrentUserClaimInterrogator.HasClaimForStateAgency(EdFiClaimTypes.ViewAllStudents)).Return(false);
            Expect.Call(CurrentUserClaimInterrogator.HasClaimForSearch(EdFiClaimTypes.ViewAllStudents)).Return(false);
            Expect.Call(CurrentUserClaimInterrogator.HasClaimForSearch(EdFiClaimTypes.ViewMyStudents)).Return(true);
            Expect.Call(AuthorizationInformationProvider.GetAllStaffStudentUSIs(UserInformation.Current.StaffUSI)).Return(SuppliedStudentUSIsForEdOrgViewMyStudents);
        }

        [Test]
        public virtual void Should_return_LEA_student_array()
        {
            //Asserting 
            Assert.That(actualModel.CanAccessAllStudents, Is.EqualTo(false));
            Assert.That(actualModel.StudentUSIs.Count(), Is.Not.EqualTo(0));

            foreach (var i in SuppliedStudentUSIsForEdOrgViewMyStudents)
                Assert.That(actualModel.StudentUSIs.Any(x => x == i), Is.True);
        }
    }

}
