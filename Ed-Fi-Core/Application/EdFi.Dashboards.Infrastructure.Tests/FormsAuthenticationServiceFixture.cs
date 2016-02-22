// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System.Linq;
using EdFi.Dashboards.Infrastructure.Implementations;
using NUnit.Framework;
using EdFi.Dashboards.Testing;

namespace EdFi.Dashboards.Infrastructure.Tests
{
    [TestFixture]
    public class When_saving_and_loading_form_authentication : TestFixtureBase
    {
        private const string suppliedUserName = "testUser";
        private readonly string[] suppliedRoles = new[] {"role1", "role2", "role3"};
        private const int suppliedLocalEducationAgencyId = 3;

        private string actualUserName;
        private string[] actualRoles;
        private int actualLocalEducationAgencyId;

        protected override void ExecuteTest()
        {
            var service = new FormsAuthenticationProvider(new HashtableCookieProvider());
            service.PerformBrowserLogin(suppliedUserName, suppliedRoles, suppliedLocalEducationAgencyId);
            service.LoadBrowserLogin(out actualUserName, out actualRoles, out actualLocalEducationAgencyId);
        }

        [Test]
        public void Data_saved_should_be_returned()
        {
            Assert.That(actualUserName, Is.EqualTo(suppliedUserName));
            Assert.That(actualLocalEducationAgencyId, Is.EqualTo(suppliedLocalEducationAgencyId));
            Assert.That(actualRoles.Length, Is.EqualTo(suppliedRoles.Length));

            foreach (var role in actualRoles)
                Assert.That(suppliedRoles.Contains(role), Is.True, role + " was returned but not supplied");
        }
    }
}
