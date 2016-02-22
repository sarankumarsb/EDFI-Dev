// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
// TODO: Deferred - GKM commented out because feature needs to be reimplemented using WIF claims-based security
/*
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading;
using EdFi.Dashboards.Resources.Admin;
using EdFi.Dashboards.Resources.Security;
using NUnit.Framework;
using EdFi.Dashboards.Core;
using EdFi.Dashboards.Data.Entities;
using EdFi.Dashboards.Data.Entities.Persisted;
using EdFi.Dashboards.Data.Repository;
using EdFi.Dashboards.Infrastructure;
using EdFi.Dashboards.Resources.Models.Admin;
using EdFi.Dashboards.Testing;
using Rhino.Mocks;

namespace EdFi.Dashboards.Resources.Tests.Admin
{
    public abstract class BaseCanLogInAsUserServiceFixture : TestFixtureBase
    {
        protected int suppliedStaffUSI = 1;
        protected IQueryable<StaffInformation> suppliedStaffInformation;
        protected IPersistingRepository<AdministrationConfiguration, int> administrationConfigurationRepository;
        protected IRepository<LocalEducationAgencyInformation> localEducationAgencyInformationRepository;
        protected IRepository<StaffInformation> staffInformationRepository;
        protected IUserAuthorizationProvider userAuthorizationProvider;
        protected IUserRolesProvider userRolesProvider;

        protected CanLogInAsUserModel actualModel;

        protected override void EstablishContext(MockRepository mocks)
        {
            administrationConfigurationRepository = mocks.StrictMock<IPersistingRepository<AdministrationConfiguration, int>>();
            localEducationAgencyInformationRepository = mocks.StrictMock<IRepository<LocalEducationAgencyInformation>>();

            suppliedStaffInformation = GetSuppliedStaffInformation();
            staffInformationRepository = mocks.StrictMock<IRepository<StaffInformation>>();
            Expect.Call(staffInformationRepository.GetAll()).Return(suppliedStaffInformation);


            userAuthorizationProvider = mocks.StrictMock<IUserAuthorizationProvider>();
            userRolesProvider = mocks.StrictMock<IUserRolesProvider>();

            base.EstablishContext(mocks);
        }

        protected override void ExecuteTest()
        {
            ICanLogInAsUserService adminService = new CanLogInAsUserService(staffInformationRepository, userAuthorizationProvider, userRolesProvider);
            actualModel = adminService.Get(suppliedStaffUSI);
        }

        protected IQueryable<StaffInformation> GetSuppliedStaffInformation()
        {
            var staffInformation = new List<StaffInformation>
                                        {
                                            new StaffInformation {StaffUSI = 0, EmailAddress = "0@test.com"},
                                            new StaffInformation {StaffUSI = 1, EmailAddress = "1@test.com"},
                                            new StaffInformation {StaffUSI = 2, EmailAddress = "2@test.com"},
                                            new StaffInformation {StaffUSI = 2, EmailAddress = "2@test.com"},
                                            new StaffInformation {StaffUSI = 3, EmailAddress = "3@test.com"},
                                        };
            return staffInformation.AsQueryable();
        }

        protected void LoginUser(string userName, string[] roleNames)
        {
            // Create an Identity object
            var id = new GenericIdentity(userName, "IAuthenticationService");

            // This principal will flow throughout the request.
            var principal = new GenericPrincipal(id, roleNames);

            // Attach the new principal object to the current HttpContext object
            Thread.CurrentPrincipal = principal;
        }
    }

    public class When_local_education_agency_system_administrator_tries_to_login_as_another_user : BaseCanLogInAsUserServiceFixture
    {
        protected override void EstablishContext(MockRepository mocks)
        {
            LoginUser("test", new[] {ClaimsSet.SystemAdministrator.ToString()});

            base.EstablishContext(mocks);

            Expect.Call(userAuthorizationProvider.GetUserInformation("1@test.com")).Return(new EdFi.Dashboards.Resources.Models.User.UserInformation { });
            Expect.Call(userRolesProvider.GetUserClaimSets("1@test.com")).Return(new ClaimsSet[] { ClaimsSet.Teacher });
        }

        [Test]
        public void Should_allow_login()
        {
            Assert.That(actualModel.CanLogIn, Is.True);
            Assert.That(actualModel.StaffUSI, Is.EqualTo(suppliedStaffUSI));
            Assert.That(actualModel.Email, Is.EqualTo("1@test.com"));
        }
    }

    public class When_local_education_agency_system_administrator_tries_to_login_as_another_user_without_roles : BaseCanLogInAsUserServiceFixture
    {
        protected override void EstablishContext(MockRepository mocks)
        {
            LoginUser("test", new[] { ClaimsSet.SystemAdministrator.ToString() });

            base.EstablishContext(mocks);

            Expect.Call(userAuthorizationProvider.GetUserInformation("1@test.com")).Return(new EdFi.Dashboards.Resources.Models.User.UserInformation { });
            Expect.Call(userRolesProvider.GetUserClaimSets("1@test.com")).Return(new ClaimsSet[] { });
        }

        [Test]
        public void Should_not_allow_login()
        {
            Assert.That(actualModel.CanLogIn, Is.False);
            Assert.That(actualModel.StaffUSI, Is.EqualTo(suppliedStaffUSI));
        }
    }

    public class When_local_education_agency_system_administrator_tries_to_login_as_nonexistent_another_user : BaseCanLogInAsUserServiceFixture
    {
        protected override void EstablishContext(MockRepository mocks)
        {
            suppliedStaffUSI = 100;
            LoginUser("test", new[] { ClaimsSet.SystemAdministrator.ToString() });
            base.EstablishContext(mocks);
        }

        [Test]
        public void Should_not_allow_login()
        {
            Assert.That(actualModel.CanLogIn, Is.False);
            Assert.That(actualModel.StaffUSI, Is.EqualTo(suppliedStaffUSI));
        }
    }

    public class When_local_education_agency_system_administrator_tries_to_login_as_duplicate_user : BaseCanLogInAsUserServiceFixture
    {
        protected override void EstablishContext(MockRepository mocks)
        {
            suppliedStaffUSI = 2;
            LoginUser("test", new[] { ClaimsSet.SystemAdministrator.ToString() });
            base.EstablishContext(mocks);
        }

        [Test]
        public void Should_not_allow_login()
        {
            Assert.That(actualModel.CanLogIn, Is.False);
            Assert.That(actualModel.StaffUSI, Is.EqualTo(suppliedStaffUSI));
        }
    }

    public class When_principal_tries_to_login_as_another_user : BaseCanLogInAsUserServiceFixture
    {
        protected override void EstablishContext(MockRepository mocks)
        {
            LoginUser("test", new[] { ClaimsSet.Principal.ToString() });

            administrationConfigurationRepository = mocks.StrictMock<IPersistingRepository<AdministrationConfiguration, int>>();
            localEducationAgencyInformationRepository = mocks.StrictMock<IRepository<LocalEducationAgencyInformation>>();
            staffInformationRepository = mocks.StrictMock<IRepository<StaffInformation>>();
        }

        [Test]
        public void Should_not_allow_login()
        {
            Assert.That(actualModel.CanLogIn, Is.False);
            Assert.That(actualModel.StaffUSI, Is.EqualTo(suppliedStaffUSI));
        }
    }
}
*/
