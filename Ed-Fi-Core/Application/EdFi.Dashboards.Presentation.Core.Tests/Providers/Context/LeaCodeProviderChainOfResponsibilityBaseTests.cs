using System.Web;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using EdFi.Dashboards.Common.Utilities;
using EdFi.Dashboards.Core;
using EdFi.Dashboards.Infrastructure;
using EdFi.Dashboards.Infrastructure.CastleWindsorInstallers;
using EdFi.Dashboards.Presentation.Architecture.Providers;
using EdFi.Dashboards.Presentation.Core.Providers.Context;
using EdFi.Dashboards.Resources.LocalEducationAgency;
using EdFi.Dashboards.Testing;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EdFi.Dashboards.Presentation.Architecture.Mvc.ValueProviders;

namespace EdFi.Dashboards.Presentation.Core.Tests.Providers.Context
{
    public class When_Registering_LeaCodeProviderChainOfResponsibility : TestFixtureBase
    {
        protected ILocalEducationAgencyContextProvider localEducationAgencyContextProvider;
        protected IIdCodeService idCodeService;
        protected IHttpRequestProvider httpRequestProvider;
        protected IHttpContextItemsProvider httpContextItemsProvider;
        protected IRequestUrlBaseProvider requestUrlBaseProvider;

        protected override void EstablishContext()
        {
            idCodeService = mocks.Stub<IIdCodeService>();
            httpRequestProvider = mocks.Stub<IHttpRequestProvider>();
            httpContextItemsProvider = mocks.Stub<IHttpContextItemsProvider>();
            requestUrlBaseProvider = mocks.Stub<IRequestUrlBaseProvider>();

            var containerMock = new WindsorContainer();

            containerMock.Register(Component.For<IIdCodeService>().Instance(idCodeService));
            containerMock.Register(Component.For<IHttpRequestProvider>().Instance(httpRequestProvider));
            containerMock.Register(Component.For<IHttpContextItemsProvider>().Instance(httpContextItemsProvider));
            containerMock.Register(Component.For<IRequestUrlBaseProvider>().Instance(requestUrlBaseProvider));

            var chainTypes = new List<Type>()
            {
                typeof(HttpContextItemsLeaCodeProvider),
                typeof(DashboardContextLeaCodeProvider),
                typeof(HttpRequestUrlLeaCodeProvider)
            };
            
            var chainRegistrar = new ChainOfResponsibilityRegistrar(containerMock);
            chainRegistrar.RegisterChainOf<ILocalEducationAgencyContextProvider, NullLeaCodeProvider>(chainTypes.ToArray());

            IoC.Initialize(containerMock);
        }

        protected override void ExecuteTest()
        {
            localEducationAgencyContextProvider = IoC.Resolve<ILocalEducationAgencyContextProvider>();
        }

        [Test]
        public void Should_Not_Throw_Exception()
        {
            var code = localEducationAgencyContextProvider.GetCurrentLocalEducationAgencyCode();
        }
    }
}
