using System.Collections.Generic;
using System.Linq;
using EdFi.Dashboards.Data.Entities;
using EdFi.Dashboards.Data.Repository;
using EdFi.Dashboards.Resources.Security.Implementations;
using EdFi.Dashboards.SecurityTokenService.Authentication;
using NUnit.Framework;
using Rhino.Mocks;

namespace EdFi.Dashboards.Resources.Security.Tests
{
    public abstract class StaffInformationFromEmailProviderFixture : ExtensionsResourcesSecurityTestsBaseWithClaims
    {
        protected IRepository<StaffInformation> staffInformationRepository;
        protected StaffInformationFromEmailProvider staffInformationFromEmailProvider;
        protected List<StaffInformation> suppliedData;
        
        protected readonly string uniqueEmail = "person1@email.address.com";
        protected readonly string duplicatedEmail = "person2@email.address.com";

        protected override void EstablishContext()
        {
            base.EstablishContext();
            staffInformationRepository = mocks.StrictMock<IRepository<StaffInformation>>();
            suppliedData = GetSuppliedData();

            Expect.Call(staffInformationRepository.GetAll()).Return(suppliedData.AsQueryable());
        }

        protected virtual List<StaffInformation> GetSuppliedData()
        {
            var toReturn = new List<StaffInformation>()
            {
                new StaffInformation() {StaffUSI = 1, EmailAddress = uniqueEmail},
                new StaffInformation() {StaffUSI = 2, EmailAddress = duplicatedEmail},
                new StaffInformation() {StaffUSI = 3, EmailAddress = duplicatedEmail}
            };
            return toReturn;
        }

        protected override void ExecuteTest()
        {
            staffInformationFromEmailProvider = new StaffInformationFromEmailProvider(staffInformationRepository);
        }
    }

    public class When_Calling_StaffInformationFromEmailProvider_With_Bad_Email :
        StaffInformationFromEmailProviderFixture
    {
        [Test]
        [ExpectedException(typeof(DashboardsMissingStaffAuthenticationException))]
        public void Should_Throw_Exception()
        {
            staffInformationFromEmailProvider.ResolveStaffUSI("asdf");
        }
    }

    public class When_Calling_StaffInformationFromEmailProvider_With_Multiple_EmailAddresses :
        StaffInformationFromEmailProviderFixture
    {
        [Test]
        [ExpectedException(typeof(DashboardsMultipleStaffAuthenticationException))]
        public void Should_Throw_Exception_When_Multiple_Found()
        {
            staffInformationFromEmailProvider.ResolveStaffUSI(duplicatedEmail);
        }
    }

    public class When_Calling_StaffInformationFromEmailProvider :
        StaffInformationFromEmailProviderFixture
    {
        [Test]
        public void Should_Return_The_Correct_StaffUSI()
        {
            var result = staffInformationFromEmailProvider.ResolveStaffUSI(uniqueEmail);
            Assert.That(result, Is.EqualTo(1));
        }
    }
}