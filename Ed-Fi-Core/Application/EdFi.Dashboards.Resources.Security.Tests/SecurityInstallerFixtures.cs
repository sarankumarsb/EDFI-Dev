// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using EdFi.Dashboards.Application.Data;
using EdFi.Dashboards.Common.Utilities;
using EdFi.Dashboards.Data;
using EdFi.Dashboards.Data.CastleWindsorInstallers;
using EdFi.Dashboards.Data.Providers;
using EdFi.Dashboards.Infrastructure.Data;
using EdFi.Dashboards.Metric.Data.Entities;
using EdFi.Dashboards.Metric.Resources;
using EdFi.Dashboards.Metric.Resources.Providers;
using EdFi.Dashboards.Resources.CastleWindsorInstallers;
using EdFi.Dashboards.Resources.Metric.MetricInstanceSetKeyResolvers;
using EdFi.Dashboards.Resources.Security.CastleWindsorInstallers;
using EdFi.Dashboards.Resources.Security.ClaimValidators;
using EdFi.Dashboards.Resources.Security.Common;
using EdFi.Dashboards.SecurityTokenService.Authentication;
using EdFi.Dashboards.Testing;
using NUnit.Framework;

namespace EdFi.Dashboards.Resources.Security.Tests
{
    public class SecurityInstallerFixtures : TestFixtureBase
    {
        private IWindsorContainer containerMock;
        private IConfigurationStore configStoreMock;
        private IDbConnectionStringSelector dbConnectionStringSelector;
        private ISubsonicDataProviderProvider subsonicDataProviderProvider;
        private ICurrentUserClaimInterrogator currentUserClaimInterrogator;

        protected override void EstablishContext()
        {
            containerMock = new WindsorContainer();
            IoC.Initialize(containerMock);
            configStoreMock = mocks.DynamicMock<IConfigurationStore>();
            currentUserClaimInterrogator = mocks.DynamicMock<ICurrentUserClaimInterrogator>();
            dbConnectionStringSelector = mocks.DynamicMock<IDbConnectionStringSelector>();
            subsonicDataProviderProvider = mocks.DynamicMock<ISubsonicDataProviderProvider>();
        }

        protected override void ExecuteTest()
        {
            var installer = new SecurityComponentsInstaller(typeof(ApplyViewPermissionsByClaimWithNoProceedInterceptor));

            installer.Install(containerMock, configStoreMock);
            //adding this registration so that the Generic SErvice Installer will run correctly, I did not install the whole ConfigurationSpecificInstaller because
            //the Install above registers some of the same items and causes an "Already been registered" error
            containerMock.Register(Component
                .For(typeof(IMetricInstanceSetKeyResolver<>))
                .ImplementedBy(typeof(SsisMultipleHashMetricInstanceSetKeyResolverConvertLongToInt<>)));
            containerMock.Install(new GenericServiceInstaller<Marker_EdFi_Dashboards_Resources>());
            containerMock.Install(new GenericServiceInstaller<Marker_EdFi_Dashboards_Metric_Resources>());
            containerMock.Install(new RepositoryInstaller<Marker_EdFi_Dashboards_Data>());
            containerMock.Install(new RepositoryInstaller<Marker_EdFi_Dashboards_Metric_Data_Entities>());
            containerMock.Install(new QueryInstaller<Marker_EdFi_Dashboards_Data>());
            containerMock.Register(Component.For<IDbConnectionStringSelector>().Instance(dbConnectionStringSelector));
            containerMock.Register(Component.For<IDbConnectionStringSelector>().Instance(dbConnectionStringSelector).Named("Default Database Selector"));
            containerMock.Register(Component.For<ISubsonicDataProviderProvider>().Instance(subsonicDataProviderProvider));
            containerMock.Register(Component.For<ICurrentUserClaimInterrogator>().Instance(currentUserClaimInterrogator));

            containerMock.Install(new PersistedRepositoryInstaller<Marker_EdFi_Dashboards_Application_Data>());
        }

        [Test]
        public void ClaimAuthorization_should_exist()
        {
            var bar = containerMock.ResolveAll<IClaimAuthorization>();
            foreach (var foo in bar)
            {
                Assert.NotNull(foo);
            }
        }

        [Test]
        public void Claim_validation_chain_should_have_next()
        {
            var keys = new[]
                           {
                               "IClaimValidator.ViewOperationalDashboard",
                               "IClaimValidator.ViewMyStudents",
                               "IClaimValidator.ManageGoals",
                               "IClaimValidator.ViewAllStudents",
                               "IClaimValidator.ViewAllMetrics",
                               "IClaimValidator.AdministerDashboard",
                               "IClaimValidator.ViewAllTeachers"
                           };
            foreach (var key in keys)
            {
                var foo = containerMock.Resolve<ClaimValidatorBase>(key);
                Assert.NotNull(foo);
            }
        }
    }
}
