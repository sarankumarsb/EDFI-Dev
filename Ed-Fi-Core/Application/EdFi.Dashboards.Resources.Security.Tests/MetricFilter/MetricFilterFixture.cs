// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Threading;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using EdFi.Dashboards.Core;
using EdFi.Dashboards.Core.Providers.Context;
using EdFi.Dashboards.Infrastructure;
using EdFi.Dashboards.Metric.Resources.Models;
using EdFi.Dashboards.Resources.Metric.Requests;
using EdFi.Dashboards.Resources.Security.Common;
using EdFi.Dashboards.Resources.Tests;
using EdFi.Dashboards.SecurityTokenService.Authentication;
using EdFi.Dashboards.Testing;
using NUnit.Framework;
using Rhino.Mocks;

namespace EdFi.Dashboards.Resources.Security.Tests.MetricFilter
{
    public abstract class MetricFilterFixtureBase : TestFixtureBase
    {
        protected int schoolRoot = 1;
        protected int localEducationAgencyRoot = 2;
        protected IWindsorContainer windsorContainer;
        protected bool shouldFilterSchool = true;
        protected bool shouldFilterLocalEducationAgency = true;
        protected string providedUserName;
        protected string[] providedRoleNames;

        protected IMetricActionUrlAuthorizationProvider metricActionUrlAuthService;
        protected ICurrentUserClaimInterrogator currentUserClaimInterrogator;

        protected MetricBase actualSchoolRootWithFilter;
        protected MetricBase actualSchoolRoot;
        protected MetricBase actualSchoolOperationalMetricWithFilter;
        protected MetricBase actualSchoolOperationalMetric;
        protected MetricBase actualLocalEducationAgencyRootWithFilter;
        protected MetricBase actualLocalEducationAgencyRoot;
        protected MetricBase actualLocalEducationAgencyOperationalMetricWithFilter;
        protected MetricBase actualLocalEducationAgencyOperationalMetric;
        protected bool canAccessSchool;

        protected override void EstablishContext()
        {
            canAccessSchool = true;
            metricActionUrlAuthService = mocks.StrictMock<IMetricActionUrlAuthorizationProvider>();
            Expect.Call(metricActionUrlAuthService.CurrentUserHasAccessToPath(String.Empty,0)).Repeat.Any().IgnoreArguments().Return(true);

            currentUserClaimInterrogator = mocks.StrictMock<ICurrentUserClaimInterrogator>();

            var userAssociatedOrgs = UserInformation.Current.AssociatedSchools;
            foreach (var associatedOrg in userAssociatedOrgs)
            {
                foreach (var claimType in associatedOrg.ClaimTypes)
                {
                    Expect.Call(currentUserClaimInterrogator.HasClaimWithinEducationOrganizationHierarchy(claimType, associatedOrg.EducationOrganizationId
                                                                                                          )).Repeat.Any().Return(true);
                    Expect.Call(currentUserClaimInterrogator.HasClaimForLocalEducationAgencyWithinEducationOrganizationHierarchy(claimType, associatedOrg.EducationOrganizationId)).Repeat.Any().Return(false);
                    
                    if (claimType == EdFiClaimTypes.ViewAllMetrics)
                    {
                        Expect.Call(currentUserClaimInterrogator.HasClaimForMetricWithinEducationOrganizationHierarchy(0, 0)).Repeat.Any().Constraints(Rhino.Mocks.Constraints.Is.TypeOf<int>(), Rhino.Mocks.Constraints.Is.Equal(associatedOrg.EducationOrganizationId)).Return(true);
                    }
                    else if (claimType == EdFiClaimTypes.ViewMyMetrics)
                    {
                        Expect.Call(currentUserClaimInterrogator.HasClaimForMetricWithinEducationOrganizationHierarchy(0, 0)).Repeat.Any().Constraints(Rhino.Mocks.Constraints.Is.TypeOf<int>() && !Rhino.Mocks.Constraints.Is.Equal((int)SchoolMetricEnum.OperationsDashboard), Rhino.Mocks.Constraints.Is.Equal(associatedOrg.EducationOrganizationId)).Return(true);
                    }
                }
            }
            var userAssociatedLeaOrgs = UserInformation.Current.AssociatedLocalEducationAgencies;
            foreach (var associatedOrg in userAssociatedLeaOrgs)
            {
                foreach (var claimType in associatedOrg.ClaimTypes)
                {
                    Expect.Call(currentUserClaimInterrogator.HasClaimWithinEducationOrganizationHierarchy(claimType, associatedOrg.EducationOrganizationId
                                                                                                          )).Repeat.Any().Return(true);
                    Expect.Call(currentUserClaimInterrogator.HasClaimForLocalEducationAgencyWithinEducationOrganizationHierarchy(claimType, associatedOrg.EducationOrganizationId)).Repeat.Any().Return(true);

                    if (claimType == EdFiClaimTypes.ViewAllMetrics)
                    {
                        Expect.Call(currentUserClaimInterrogator.HasClaimForMetricWithinEducationOrganizationHierarchy(0, 0)).Repeat.Any().Constraints(Rhino.Mocks.Constraints.Is.TypeOf<int>(), Rhino.Mocks.Constraints.Is.TypeOf<int>()).Return(true); 
                    }
                    else if (claimType == EdFiClaimTypes.ViewMyMetrics)
                    {
                        Expect.Call(currentUserClaimInterrogator.HasClaimForMetricWithinEducationOrganizationHierarchy(0, 0)).Repeat.Any().Constraints(Rhino.Mocks.Constraints.Is.TypeOf<int>() && !Rhino.Mocks.Constraints.Is.Equal((int)LocalEducationAgencyMetricEnum.OperationsDashboard), Rhino.Mocks.Constraints.Is.Equal(associatedOrg.EducationOrganizationId)).Return(true); 
                    }
                }
            }

            if (!userAssociatedLeaOrgs.Any(o => o.ClaimTypes.Any(t => t == EdFiClaimTypes.ViewAllMetrics)))
            {
                Expect.Call(currentUserClaimInterrogator.HasClaimForLocalEducationAgencyWithinEducationOrganizationHierarchy(EdFiClaimTypes.ViewAllMetrics, 0)).Repeat.Any().Return(false).IgnoreArguments();
                Expect.Call(currentUserClaimInterrogator.HasClaimForMetricWithinEducationOrganizationHierarchy(0, 0)).Repeat.Any().Constraints(Rhino.Mocks.Constraints.Is.TypeOf<int>(), Rhino.Mocks.Constraints.Is.TypeOf<int>()).Return(false);
            }
            Expect.Call(currentUserClaimInterrogator.HasClaimForStateAgency(string.Empty)).Repeat.Any().Return(false).IgnoreArguments();
            
            windsorContainer = new WindsorContainer();
            RegisterServices(windsorContainer);

            CreateEdFiDashboardContext(LoginHelper.localEducationAgencyOneId, LoginHelper.schoolOneId);

            base.EstablishContext();
        }

        protected void CreateEdFiDashboardContext(int localEducationAgencyId, int schoolId)
        {
            var dashboardContext = new EdFiDashboardContext
                                       {
                                           SchoolId = schoolId,
                                           LocalEducationAgencyId = localEducationAgencyId
                                       };

            CallContext.SetData(EdFiDashboardContext.CallContextKey, dashboardContext);
        }

        private void RegisterServices(IWindsorContainer container)
        {
            container.Register(
                Component
                    .For(typeof(IMetricActionUrlAuthorizationProvider))
                    .Instance(metricActionUrlAuthService));

            container.Register(
                Component
                    .For(typeof(MetricInterceptor))
                    .ImplementedBy(typeof(MetricInterceptor)));

            container.Register(Component.For(typeof(StageInterceptor)).UsingFactoryMethod(CreateStageInterceptor));

            container.Register(
                Component
                    .For(typeof(ITestService<LocalEducationAgencyMetricInstanceSetRequest>))
                    .ImplementedBy(typeof(TestService<LocalEducationAgencyMetricInstanceSetRequest>))
                    .Interceptors<StageInterceptor>());

            container.Register(
                Component
                    .For(typeof(ITestService<SchoolMetricInstanceSetRequest>))
                    .ImplementedBy(typeof(TestService<SchoolMetricInstanceSetRequest>))
                    .Interceptors<StageInterceptor>());

            container.Register(
                Component.For(typeof (ICurrentUserClaimInterrogator)).Instance(currentUserClaimInterrogator));
        }

        private StageInterceptor CreateStageInterceptor()
        {
            var stages = new Lazy<IInterceptorStage>[1];
            stages[0] = new Lazy<IInterceptorStage>(windsorContainer.Resolve<MetricInterceptor>);
            return new StageInterceptor(stages);
        }

        protected void LoginUser(string userName, string[] roleNames, UserInformation userInformation)
        {
            // Create and attach a claims based principal to the current thread
            Thread.CurrentPrincipal = userInformation.ToClaimsPrincipal(userName, roleNames);
        }

        protected override void ExecuteTest()
        {
            var leaTestService = windsorContainer.Resolve<ITestService<LocalEducationAgencyMetricInstanceSetRequest>>();
            var schoolTestService = windsorContainer.Resolve<ITestService<SchoolMetricInstanceSetRequest>>();

            MetricTree metricTree;

            if (EdFiDashboardContext.Current.SchoolId.HasValue)
            {
                var schoolMetricInstanceSetRequest = new SchoolMetricInstanceSetRequest
                                                         {
                                                             SchoolId = (int) EdFiDashboardContext.Current.SchoolId,
                                                             MetricVariantId = schoolRoot
                                                         };
                metricTree = schoolTestService.Get(schoolMetricInstanceSetRequest);
                actualSchoolRootWithFilter = metricTree == null ? null : metricTree.RootNode;
                
                metricTree = schoolTestService.GetMetrics(schoolRoot, DateTime.Now);
                actualSchoolRoot = metricTree == null ? null : metricTree.RootNode;

                schoolMetricInstanceSetRequest = new SchoolMetricInstanceSetRequest
                                                     {
                                                         SchoolId = (int) EdFiDashboardContext.Current.SchoolId,
                                                         MetricVariantId = (int) SchoolMetricEnum.ExperienceEducationCertifications
                                                     };
                metricTree = schoolTestService.Get(schoolMetricInstanceSetRequest);
                actualSchoolOperationalMetricWithFilter = metricTree == null ? null : metricTree.RootNode;
                
                metricTree = schoolTestService.GetMetrics((int) SchoolMetricEnum.ExperienceEducationCertifications, DateTime.Now);
                actualSchoolOperationalMetric = metricTree == null ? null : metricTree.RootNode;
            }
            var leaMetricInstanceSetRequest = new LocalEducationAgencyMetricInstanceSetRequest
                                                  {
                                                      LocalEducationAgencyId = (int) EdFiDashboardContext.Current.LocalEducationAgencyId,
                                                      MetricVariantId = localEducationAgencyRoot
                                                  };

            metricTree = leaTestService.Get(leaMetricInstanceSetRequest);
            actualLocalEducationAgencyRootWithFilter = metricTree == null ? null : metricTree.RootNode;
            
            metricTree = leaTestService.GetMetrics(localEducationAgencyRoot, DateTime.Now);
            actualLocalEducationAgencyRoot = metricTree == null ? null : metricTree.RootNode;

            leaMetricInstanceSetRequest = new LocalEducationAgencyMetricInstanceSetRequest
                                              {
                                                  LocalEducationAgencyId = (int) EdFiDashboardContext.Current.LocalEducationAgencyId,
                                                  MetricVariantId = (int)LocalEducationAgencyMetricEnum.ExperienceEducationCertifications
                                              };
            metricTree = leaTestService.Get(leaMetricInstanceSetRequest);
            actualLocalEducationAgencyOperationalMetricWithFilter = metricTree == null ? null : metricTree.RootNode;
            actualLocalEducationAgencyOperationalMetric = leaTestService.GetMetrics((int)LocalEducationAgencyMetricEnum.ExperienceEducationCertifications, DateTime.Now).RootNode;
        }

        [Test]
        public void Should_not_filter_methods_without_attributes()
        {
            Assert.That(actualSchoolRoot, Is.Not.Null);
            Assert.That(((ContainerMetric)actualSchoolRoot).Children.Count(), Is.EqualTo(3));

            Assert.That(actualSchoolOperationalMetric, Is.Not.Null);
            Assert.That(((ContainerMetric)actualSchoolOperationalMetric).Children.Count(), Is.EqualTo(3));

            Assert.That(actualLocalEducationAgencyRoot, Is.Not.Null);
            Assert.That(((ContainerMetric)actualLocalEducationAgencyRoot).Children.Count(), Is.EqualTo(3));

            Assert.That(actualLocalEducationAgencyOperationalMetric, Is.Not.Null);
            Assert.That(((ContainerMetric)actualLocalEducationAgencyOperationalMetric).Children.Count(), Is.EqualTo(3));
        }

        [Test]
        public void Should_conditionally_filter_method()
        {
            if (shouldFilterSchool && canAccessSchool)
            {
                Assert.That(actualSchoolRootWithFilter, Is.Not.Null);
                if (UserInformation.Current.AssociatedSchools.Any(o => o.ClaimTypes.Any(t => t == EdFiClaimTypes.ViewMyMetrics)))
                {
                    //The metric filters see that the parent metric is the metric to be removed and filters the whole set out.
                    Assert.That(((ContainerMetric)actualSchoolRootWithFilter).Children.Count(), Is.EqualTo(0));
                    Assert.That(((ContainerMetric)actualSchoolOperationalMetricWithFilter).Children.Count(), Is.EqualTo(0));
                }
                else
                {
                    Assert.That(((ContainerMetric) actualSchoolRootWithFilter).Children.Count(), Is.EqualTo(2));
                }
                Assert.That(((ContainerMetric)actualSchoolRootWithFilter).Children.Any(x => x.MetricVariantId == (int)SchoolMetricEnum.OperationsDashboard), Is.False);
                
            }
            else if (!canAccessSchool)
            {
                Assert.That(actualSchoolRootWithFilter, Is.Null);
                Assert.That(actualSchoolOperationalMetricWithFilter, Is.Null);
            }
            else
            {
                Assert.That(actualSchoolRootWithFilter, Is.Not.Null);
                Assert.That(((ContainerMetric)actualSchoolRootWithFilter).Children.Count(), Is.EqualTo(3));

                Assert.That(actualSchoolOperationalMetricWithFilter, Is.Not.Null);
                Assert.That(((ContainerMetric)actualSchoolOperationalMetricWithFilter).Children.Count(), Is.EqualTo(3));
            }

            if (shouldFilterLocalEducationAgency)
            {
                
                if (UserInformation.Current.AssociatedLocalEducationAgencies.Any(o => o.ClaimTypes.Any(t => t == EdFiClaimTypes.ViewMyMetrics)))
                {
                    Assert.That(actualLocalEducationAgencyRootWithFilter, Is.Not.Null);
                    //The metric filters see that the parent metric is the metric to be removedand filters the whole set out.
                    Assert.That(((ContainerMetric)actualLocalEducationAgencyRootWithFilter).Children.Count(), Is.EqualTo(0));
                    Assert.That(((ContainerMetric)actualLocalEducationAgencyOperationalMetricWithFilter).Children.Count(), Is.EqualTo(0));
                }
                else if (UserInformation.Current.AssociatedLocalEducationAgencies.Any(o => o.ClaimTypes.Any(t => t == EdFiClaimTypes.ViewAllMetrics)))
                {
                    Assert.That(actualLocalEducationAgencyRootWithFilter, Is.Not.Null);
                    Assert.That(((ContainerMetric) actualLocalEducationAgencyRootWithFilter).Children.Count(),
                                Is.EqualTo(2));
                    Assert.That(((ContainerMetric)actualLocalEducationAgencyRootWithFilter).Children.Any(x => x.MetricVariantId == (int)LocalEducationAgencyMetricEnum.OperationsDashboard), Is.False);
                }
                else
                {
                    Assert.That(actualLocalEducationAgencyRootWithFilter, Is.Null);
                    Assert.That(actualLocalEducationAgencyOperationalMetricWithFilter, Is.Null);
                } 
            }
            else
            {
                Assert.That(actualLocalEducationAgencyRootWithFilter, Is.Not.Null);
                Assert.That(((ContainerMetric)actualLocalEducationAgencyRootWithFilter).Children.Count(), Is.EqualTo(3));

                Assert.That(actualLocalEducationAgencyOperationalMetricWithFilter, Is.Not.Null);
                Assert.That(((ContainerMetric)actualLocalEducationAgencyOperationalMetricWithFilter).Children.Count(), Is.EqualTo(3));
            }
        }
    }

    [TestFixture]
    public class When_superintendent_accesses_metric : MetricFilterFixtureBase
    {
        protected override void EstablishContext()
        {
            LoginHelper.LoginSuperintendent();

            shouldFilterSchool = false;
            shouldFilterLocalEducationAgency = false;

            base.EstablishContext();
        }
    }

    [TestFixture]
    public class When_local_education_agency_system_administrator_accesses_metric : MetricFilterFixtureBase
    {
        protected override void EstablishContext()
        {
            LoginHelper.LoginLocalEducationAgencySystemAdministratorOne();

            shouldFilterSchool = false;
            shouldFilterLocalEducationAgency = false;

            base.EstablishContext();
        }
    }

    [TestFixture]
    public class When_local_education_agency_administrator_accesses_metric : MetricFilterFixtureBase
    {
        protected override void EstablishContext()
        {
            LoginHelper.LoginLocalEducationAgencyAdministrator();

            shouldFilterSchool = true;
            shouldFilterLocalEducationAgency = true;

            base.EstablishContext();
            canAccessSchool = false;
        }
    }

    [TestFixture]
    public class When_principal_accesses_metric : MetricFilterFixtureBase
    {
        protected override void EstablishContext()
        {
            LoginHelper.LoginPrincipalOneSchool();

            shouldFilterSchool = false;
            shouldFilterLocalEducationAgency = true;

            base.EstablishContext();
        }
    }

    [TestFixture]
    public class When_school_administrator_accesses_metric : MetricFilterFixtureBase
    {
        protected override void EstablishContext()
        {
            LoginHelper.LoginSchoolAdministrator();

            shouldFilterSchool = true;
            shouldFilterLocalEducationAgency = true;

            base.EstablishContext();
        }
    }

    [TestFixture]
    public class When_school_specialist_accesses_metric : MetricFilterFixtureBase
    {
        protected override void EstablishContext()
        {
            LoginHelper.LoginSchoolSpecialistOne();

            shouldFilterSchool = true;
            shouldFilterLocalEducationAgency = true;

            base.EstablishContext();
        }
    }

    [TestFixture]
    public class When_teacher_accesses_metric : MetricFilterFixtureBase
    {
        protected override void EstablishContext()
        {
            LoginHelper.LoginTeacherOne();

            shouldFilterSchool = true;
            shouldFilterLocalEducationAgency = true;

            base.EstablishContext();
        }
    }

    [TestFixture]
    public class When_loading_metric_actions : TestFixtureBase
    {
        protected int schoolRoot = 1;
        protected IConfigSectionProvider configSectionProvider;
        protected IMetricActionUrlAuthorizationProvider metricActionUrlAuthService;
        protected ICurrentUserClaimInterrogator currentUserClaimInterrogator;
        protected IWindsorContainer windsorContainer;

       // private IEnumerable<Type> concreteModelMetricTypes;
      //  private List<MetricToMetricInstanceJoin> suppliedMetrics;
        private MetricBase fakedMetrics;

        protected override void EstablishContext()
        {
            metricActionUrlAuthService = mocks.StrictMock<IMetricActionUrlAuthorizationProvider>();
            configSectionProvider = mocks.StrictMock<IConfigSectionProvider>();
            currentUserClaimInterrogator = mocks.StrictMock<ICurrentUserClaimInterrogator>();
            windsorContainer = new WindsorContainer();
            RegisterServices(windsorContainer);
            LoginHelper.LoginSuperintendent();
            int lea =
                LoginHelper.userInfoSuperintendent.AssociatedLocalEducationAgencies.Single().EducationOrganizationId;
            CreateEdFiDashboardContext(lea,null);

            foreach (var associatedOrganizations in LoginHelper.userInfoSuperintendent.AssociatedOrganizations)
            {
                foreach (var claimType in associatedOrganizations.ClaimTypes)
                {
                    Expect.Call(currentUserClaimInterrogator.HasClaimWithinEducationOrganizationHierarchy(claimType,associatedOrganizations.EducationOrganizationId)).Repeat.Any().Return(true);
                    Expect.Call(currentUserClaimInterrogator.HasClaimForLocalEducationAgencyWithinEducationOrganizationHierarchy(claimType, associatedOrganizations.EducationOrganizationId)).Repeat.Any().Return(true);
                    if (claimType == EdFiClaimTypes.ViewAllMetrics)
                    {
                        Expect.Call(currentUserClaimInterrogator.HasClaimForMetricWithinEducationOrganizationHierarchy(0, 0)).Repeat.Any().Constraints(Rhino.Mocks.Constraints.Is.TypeOf<int>(), Rhino.Mocks.Constraints.Is.Equal(associatedOrganizations.EducationOrganizationId)).Return(true);
                    }
                }
            }

            

            //Test is focusing on this!!! The logic to add or filter/remove actions
            Expect.Call(metricActionUrlAuthService.CurrentUserHasAccessToPath("~/aaa.aspx",lea)).Return(true);
            Expect.Call(metricActionUrlAuthService.CurrentUserHasAccessToPath("~/bbb.aspx",lea)).Return(false);
        }

        protected void CreateEdFiDashboardContext(int? localEducationAgencyId, int? schoolId)
        {

            var dashboardContext = new EdFiDashboardContext();
            if (localEducationAgencyId.HasValue)
            {
                dashboardContext.LocalEducationAgencyId = localEducationAgencyId;
            }
            if (schoolId.HasValue)
            {
                dashboardContext.SchoolId = schoolId;
            }

            CallContext.SetData(EdFiDashboardContext.CallContextKey, dashboardContext);
        }

        private void RegisterServices(IWindsorContainer container)
        {
            container.Register(
                Component
                    .For(typeof (IConfigSectionProvider))
                    .Instance(configSectionProvider));

            container.Register(
                Component
                    .For(typeof(IMetricActionUrlAuthorizationProvider))
                    .Instance(metricActionUrlAuthService));

            container.Register(
                Component
                    .For(typeof(MetricInterceptor))
                    .ImplementedBy(typeof(MetricInterceptor)));
            container.Register(
                Component
                    .For(typeof(ICurrentUserClaimInterrogator))
                    .Instance(currentUserClaimInterrogator));

            container.Register(Component.For(typeof(StageInterceptor)).UsingFactoryMethod(CreateStageInterceptor));

            container.Register(
                Component
                    .For(typeof(ITestService<LocalEducationAgencyMetricInstanceSetRequest>))
                    .ImplementedBy(typeof(TestService<LocalEducationAgencyMetricInstanceSetRequest>))
                    .Interceptors<StageInterceptor>());
            
            container.Register(
                Component
                    .For(typeof (ITestService<SchoolMetricInstanceSetRequest>))
                    .ImplementedBy(typeof (TestService<SchoolMetricInstanceSetRequest>))
                    .Interceptors<StageInterceptor>());
        }

        private StageInterceptor CreateStageInterceptor()
        {
            var stages = new Lazy<IInterceptorStage>[1];
            stages[0] = new Lazy<IInterceptorStage>(windsorContainer.Resolve<MetricInterceptor>);
            return new StageInterceptor(stages);
        }

        protected override void ExecuteTest()
        {
            if (EdFiDashboardContext.Current.SchoolId.HasValue)
            {
                var metricInstanceSetRequest = new SchoolMetricInstanceSetRequest();
                var testService = windsorContainer.Resolve<ITestService<SchoolMetricInstanceSetRequest>>();

                metricInstanceSetRequest.SchoolId = (int)EdFiDashboardContext.Current.SchoolId;
                metricInstanceSetRequest.MetricVariantId = schoolRoot;
                fakedMetrics = testService.Get(metricInstanceSetRequest).RootNode;
            }
            else
            {
                var metricInstanceSetRequest = new LocalEducationAgencyMetricInstanceSetRequest();
                var testService = windsorContainer.Resolve<ITestService<LocalEducationAgencyMetricInstanceSetRequest>>();

                metricInstanceSetRequest.LocalEducationAgencyId = (int)EdFiDashboardContext.Current.LocalEducationAgencyId;
                metricInstanceSetRequest.MetricVariantId = schoolRoot;
                fakedMetrics = testService.Get(metricInstanceSetRequest).RootNode;
            }
        }

        [Test]
        public void Should_overlay_only_one_metric_action()
        {
            var suppliedActions = fakedMetrics.Actions;
            Assert.That(fakedMetrics.Actions.Count, Is.EqualTo(1));

            Assert.That(fakedMetrics.Actions[0].Url, Is.EqualTo(suppliedActions[0].Url));
        }
    }
}
