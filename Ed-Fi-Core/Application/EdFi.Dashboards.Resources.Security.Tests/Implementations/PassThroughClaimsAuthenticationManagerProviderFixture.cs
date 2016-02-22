using EdFi.Dashboards.Testing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.IdentityModel.Claims;
using Microsoft.SqlServer.Server;
using NUnit.Framework;
using EdFi.Dashboards.Resources.Security.Implementations;

namespace EdFi.Dashboards.Resources.Security.Tests.Implementations
{
    public class When_Calling_PassThroughClaimsAuthenticationManagerProvider : TestFixtureBase
    {
        private PassThroughClaimsAuthenticationManagerProvider managerProvider;
        private string resourceName = "ResourceName";
        private IClaimsPrincipal principal;
        private IClaimsPrincipal actualPrincipal;

        protected override void EstablishContext()
        {
            principal = mocks.DynamicMock<IClaimsPrincipal>();
            managerProvider = new PassThroughClaimsAuthenticationManagerProvider();
        }

        protected override void ExecuteTest()
        {
            actualPrincipal = managerProvider.Get(resourceName, principal);
        }

        [Test]
        public void Should_Return_IncomingPrincipal()
        {
            Assert.IsTrue(Object.ReferenceEquals(actualPrincipal, principal));
        }
    }
}
