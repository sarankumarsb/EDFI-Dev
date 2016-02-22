using System.Runtime.InteropServices;
using System.Security.Principal;
using EdFi.Dashboards.Testing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Castle.Windsor;
using EdFi.Dashboards.Common.Utilities;
using Castle.MicroKernel.Registration;
using Microsoft.IdentityModel.Claims;
using NUnit.Framework;
using System.Web.Security;
using Rhino.Mocks;

namespace EdFi.Dashboards.Resources.Security.Tests
{
    public abstract class CustomClaimsAuthenticationManagerFixture : TestFixtureBase
    {
        protected IWindsorContainer containerMock;
        protected CustomClaimsAuthenticationManager customClaimsAuthenticationManager;
        protected IClaimsAuthenticationManagerProvider claimsAuthenticationManagerProvider;
        protected static readonly string resourceName = "Resource";
        protected IClaimsPrincipal claimsPrincipal;
        protected IAmNotClaimsIdenitty notClaimsIdnentity;
        protected IClaimsIdentity claimsIdentity;

        protected override void EstablishContext()
        {
            containerMock = new WindsorContainer();

            claimsAuthenticationManagerProvider = mocks.StrictMock<IClaimsAuthenticationManagerProvider>();
            claimsPrincipal = mocks.DynamicMock<IClaimsPrincipal>();
            notClaimsIdnentity = mocks.StrictMock<IAmNotClaimsIdenitty>();
            claimsIdentity = mocks.StrictMock<IClaimsIdentity>();
            containerMock.Register(Component.For<IClaimsAuthenticationManagerProvider>().Instance(claimsAuthenticationManagerProvider));

            IoC.Initialize(containerMock);
        }

        protected override void ExecuteTest()
        {
            customClaimsAuthenticationManager = new CustomClaimsAuthenticationManager();
        }
    }

    public class When_ClaimsAuthenticationManager_Identity_Is_Not_ClaimsIdentity : CustomClaimsAuthenticationManagerFixture
    {
        protected override void EstablishContext()
        {
            base.EstablishContext();
            claimsPrincipal.Expect(x => x.Identity).Return(notClaimsIdnentity);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void Should_Throw_Exception()
        {
            customClaimsAuthenticationManager.Authenticate(resourceName, claimsPrincipal);
        }
    }

    public class When_ClaimsAuthenticationManager_Identity_IsNot_Authenticated : CustomClaimsAuthenticationManagerFixture
    {
        protected override void EstablishContext()
        {
            base.EstablishContext();
            claimsPrincipal.Expect(x => x.Identity).Return(claimsIdentity);
            claimsIdentity.Expect(x => x.IsAuthenticated).Return(false);
        }

        [Test]
        public void Should_Not_Call_Claims_Authentication_Manager_Provider()
        {
            customClaimsAuthenticationManager.Authenticate(resourceName, claimsPrincipal);
        }
    }

    public class When_ClaimsAuthenticationManager_Identity_Is_Authenticated : CustomClaimsAuthenticationManagerFixture
    {
        protected override void EstablishContext()
        {
            base.EstablishContext();
            claimsPrincipal.Expect(x => x.Identity).Return(claimsIdentity);
            claimsIdentity.Expect(x => x.IsAuthenticated).Return(true);
            claimsAuthenticationManagerProvider.Expect(x => x.Get(resourceName, claimsPrincipal)).Return(claimsPrincipal);
        }

        [Test]
        public void Should_Call_Authentication_Manager_Provider()
        {
            customClaimsAuthenticationManager.Authenticate(resourceName, claimsPrincipal);
        }
    }

    public interface IAmNotClaimsIdenitty : IIdentity { }
}
