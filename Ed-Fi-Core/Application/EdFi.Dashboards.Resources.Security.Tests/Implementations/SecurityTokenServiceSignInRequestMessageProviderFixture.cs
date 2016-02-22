using Castle.Windsor;
using EdFi.Dashboards.Common.Utilities;
using EdFi.Dashboards.Resources.Models.LocalEducationAgency;
using EdFi.Dashboards.Testing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.IdentityModel.Protocols.WSFederation;
using EdFi.Dashboards.Resources.Security.Implementations;
using NUnit.Framework;
using EdFi.Dashboards.Resources.Navigation;
using Castle.MicroKernel.Registration;
using Rhino.Mocks;

namespace EdFi.Dashboards.Resources.Security.Tests.Implementations
{
    public class When_SecurityTokenServiceSignInRequestMessageProviderFixture_Adorn : TestFixtureBase
    {
        private WindsorContainer containerMock;
        private SecurityTokenServiceSignInRequestMessageProvider messageProvider;
        private ILocalEducationAgencyAreaLinks localEducationAgencyAreaLinks;
        private readonly string code = "CODE";
        private readonly int leaID = 1;
        private readonly string name = "Name";
        private readonly string linkPattern = "http://www.dashboards.com/code/{0}";
        private SignInRequestAdornModel model;
        private Dictionary<string, string> actualDictionary;
        private SignInRequestMessage signInRequestMessage;
        
        protected override void EstablishContext()
        {
            containerMock = new WindsorContainer();
            localEducationAgencyAreaLinks = mocks.StrictMock<ILocalEducationAgencyAreaLinks>();

            messageProvider = new SecurityTokenServiceSignInRequestMessageProvider(localEducationAgencyAreaLinks);
            model = new SignInRequestAdornModel() { LocalEducationAgencyCode = code, LocalEducationAgencyId = leaID, LocalEducationAgencyName = name };
            var url = string.Format(linkPattern, code);
            actualDictionary = new Dictionary<string, string> {{"lea", code}, {"leaName", name}, {"home", url}};

            localEducationAgencyAreaLinks.Expect(x => x.Home(code)).Return(url);

            containerMock.Register(Component.For<ILocalEducationAgencyAreaLinks>().Instance(localEducationAgencyAreaLinks));
            IoC.Initialize(containerMock);
        }

        protected override void ExecuteTest()
        {
            signInRequestMessage = new SignInRequestMessage(new Uri("http://www.google.com"), "realm");
            messageProvider.Adorn(signInRequestMessage, model);
        }

        [Test]
        public void Should_Add_Model_To_Parameters()
        {
            CollectionAssert.IsSubsetOf(actualDictionary, signInRequestMessage.Parameters);
        }
    }
}
