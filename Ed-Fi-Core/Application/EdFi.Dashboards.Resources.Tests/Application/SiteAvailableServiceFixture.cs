// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Caching;
using EdFi.Dashboards.Application.Data.Entities;
using EdFi.Dashboards.Data.Repository;
using EdFi.Dashboards.Infrastructure;
using EdFi.Dashboards.Infrastructure.Implementations;
using EdFi.Dashboards.Infrastructure.Implementations.Caching;
using EdFi.Dashboards.Resources.Application;
using EdFi.Dashboards.Resources.Security.Common;
using EdFi.Dashboards.SecurityTokenService.Authentication;
using EdFi.Dashboards.Testing;
using NUnit.Framework;
using Rhino.Mocks;

namespace EdFi.Dashboards.Resources.Tests.Application
{
    [TestFixture]
    public abstract class SiteAvailableProviderTestFixtureBase : TestFixtureBase
    {
        protected bool? expectedResult;
        protected bool? actualResult;
        protected IQueryable<LocalEducationAgencyAdministration> suppliedAdministration;
        protected IRepository<LocalEducationAgencyAdministration> administrationRepository;
        protected ICurrentUserClaimInterrogator currentUserClaimInterrogator;

        protected HashtableCacheProvider cacheProvider;
        protected NameValueCollectionConfigValueProvider configValueProvider;
        protected SiteAvailableProvider adminService;

        protected override void EstablishContext()
        {
            suppliedAdministration = GetSuppliedAdministration();

            cacheProvider = new HashtableCacheProvider();
            configValueProvider = new NameValueCollectionConfigValueProvider();
            
            administrationRepository = mocks.StrictMock<IRepository<LocalEducationAgencyAdministration>>();
            Expect.Call(administrationRepository.GetAll()).Return(suppliedAdministration).Repeat.Any();

            base.EstablishContext();
        }

        protected IQueryable<LocalEducationAgencyAdministration> GetSuppliedAdministration()
        {
            var adminConfigs = new List<LocalEducationAgencyAdministration>
                                            {
                                                new LocalEducationAgencyAdministration{ LocalEducationAgencyId = LoginHelper.localEducationAgencyTwoId, IsKillSwitchActivated = false},
                                                new LocalEducationAgencyAdministration{ LocalEducationAgencyId = LoginHelper.localEducationAgencyOneId, IsKillSwitchActivated = true}
                                            };
            return adminConfigs.AsQueryable();
        }

        protected override void ExecuteTest()
        {
            adminService = new SiteAvailableProvider(administrationRepository, cacheProvider, configValueProvider, currentUserClaimInterrogator, new CacheKeyGenerator());
            dynamic localEducationAgency = (dynamic) UserInformation.Current.AssociatedLocalEducationAgencies.FirstOrDefault() ??
                                                     UserInformation.Current.AssociatedSchools.FirstOrDefault();

            actualResult = adminService.IsKillSwitchActivatedForCurrentUser(localEducationAgency.LocalEducationAgencyId);
        }

        [Test]
        public void Should_create_model_correctly()
        {
            Assert.That(expectedResult.HasValue, Is.True, "Derived test scenarios must provide an expected result.");
            Assert.That(actualResult.HasValue, Is.True, "Actual result was not set.");
            Assert.That(actualResult, Is.EqualTo(expectedResult), "Result was not the expected value.");
        }
        [Test]
        public virtual void Should_have_serializable_model()
        {
            actualResult.EnsureSerializableModel();
        }

        protected string GetCacheKeyForCurrentUsersLocalEducationAgency()
        {
            dynamic localEducationAgency = (dynamic)UserInformation.Current.AssociatedLocalEducationAgencies.FirstOrDefault() ??
                                                     UserInformation.Current.AssociatedSchools.FirstOrDefault();
            int localEducationAgencyId = localEducationAgency.LocalEducationAgencyId;

            string cacheKey = (adminService as IHasCachedResult).GetCacheKey(localEducationAgencyId);

            return cacheKey;
        }
    }

    public abstract class When_checking_kill_switch_multiple_times : SiteAvailableProviderTestFixtureBase
    {
        protected bool actualResult2;

        protected override void ExecuteTest()
        {
            base.ExecuteTest();

            // Ask for the value a second time, where it should be in cache now
            dynamic localEducationAgency = (dynamic)UserInformation.Current.AssociatedLocalEducationAgencies.FirstOrDefault() ??
                                                     UserInformation.Current.AssociatedSchools.FirstOrDefault();

            actualResult = adminService.IsKillSwitchActivatedForCurrentUser(localEducationAgency.LocalEducationAgencyId);
        }

        protected override void EstablishContext()
        {
            LoginHelper.LoginPrincipalOneSchool();
            expectedResult = true;
            currentUserClaimInterrogator = mocks.StrictMock<ICurrentUserClaimInterrogator>();
            Expect.Call(currentUserClaimInterrogator.HasClaimForLocalEducationAgencyWithinEducationOrganizationHierarchy(
                    EdFiClaimTypes.AdministerDashboard, LoginHelper.localEducationAgencyOneId)).Repeat.Twice().Return(false);

            base.EstablishContext();
            //ExpectCallToAdministrationConfigurationRepository();
        }


        [Test]
        public void Should_cache_the_result_for_the_current_users_local_education_agency()
        {
            var cacheKey = GetCacheKeyForCurrentUsersLocalEducationAgency();

            Assert.That(cacheProvider.Cache.ContainsKey(cacheKey), "Result was not cached.");
        }

        protected void AssertCacheBehaviorForAbsoluteCacheTimeSpan(TimeSpan duration)
        {
            var cacheKey = GetCacheKeyForCurrentUsersLocalEducationAgency();

            var cacheEntry = cacheProvider.CacheEntryDetails[cacheKey];

            DateTime expectedAbsoluteDateTime = DateTime.Now.AddSeconds(duration.TotalSeconds);

            Assert.That(cacheEntry.AbsoluteExpiration.Within(TimeSpan.FromSeconds(1), expectedAbsoluteDateTime));
            Assert.That(cacheEntry.SlidingExpiration, Is.EqualTo(Cache.NoSlidingExpiration));
        }
    }

    public class When_checking_kill_switch_multiple_times_when_no_config_value_is_set : When_checking_kill_switch_multiple_times
    {
        [Test]
        public void Should_cache_the_result_for_a_default_of_5_minutes()
        {
            AssertCacheBehaviorForAbsoluteCacheTimeSpan(TimeSpan.FromMinutes(5)); // Default should be 5 minutes
        }
    }

    public class When_checking_kill_switch_multiple_times_when_config_value_is_set : When_checking_kill_switch_multiple_times
    {
        protected override void EstablishContext()
        {
            base.EstablishContext();

            configValueProvider.Values[SiteAvailableProvider.ConfigValueName] = (60 * 60).ToString(); // 1 hour
        }

        [Test]
        public void Should_cache_the_result_for_the_configured_duration()
        {
            AssertCacheBehaviorForAbsoluteCacheTimeSpan(TimeSpan.FromSeconds(60 * 60));
        }
    }

    public class When_checking_if_kill_switch_is_activated_for_system_admin_when_activated : SiteAvailableProviderTestFixtureBase
    {
        protected override void EstablishContext()
        {
            LoginHelper.LoginLocalEducationAgencySystemAdministratorOne();
            expectedResult = false;
            currentUserClaimInterrogator = mocks.StrictMock<ICurrentUserClaimInterrogator>();
            Expect.Call(currentUserClaimInterrogator.HasClaimForLocalEducationAgencyWithinEducationOrganizationHierarchy(
                    EdFiClaimTypes.AdministerDashboard, LoginHelper.localEducationAgencyOneId)).Return(true).Repeat.Once();
            
            base.EstablishContext();
        }
    }

    public class When_checking_if_kill_switch_is_activated_for_system_admin_when_not_activated : SiteAvailableProviderTestFixtureBase
    {
        protected override void EstablishContext()
        {
            LoginHelper.LoginLocalEducationAgencySystemAdministratorTwo();
            expectedResult = false;
            currentUserClaimInterrogator = mocks.StrictMock<ICurrentUserClaimInterrogator>();
            Expect.Call(currentUserClaimInterrogator.HasClaimForLocalEducationAgencyWithinEducationOrganizationHierarchy(
                    EdFiClaimTypes.AdministerDashboard, LoginHelper.localEducationAgencyTwoId)).Return(true);
            base.EstablishContext();
        }
    }

    public class When_checking_if_kill_switch_is_activated_for_teacher_when_activated : SiteAvailableProviderTestFixtureBase
    {
        protected override void EstablishContext()
        {
            LoginHelper.LoginTeacherOne();
            expectedResult = true;
            currentUserClaimInterrogator = mocks.StrictMock<ICurrentUserClaimInterrogator>();
            Expect.Call(currentUserClaimInterrogator.HasClaimForLocalEducationAgencyWithinEducationOrganizationHierarchy(
                    EdFiClaimTypes.AdministerDashboard, LoginHelper.localEducationAgencyOneId)).Return(false);
            base.EstablishContext();
        }
    }

    public class When_checking_if_kill_switch_is_activated_for_teacher_when_not_activated : SiteAvailableProviderTestFixtureBase
    {
        protected override void EstablishContext()
        {
            LoginHelper.LoginTeacherThree();
            expectedResult = false;
            currentUserClaimInterrogator = mocks.StrictMock<ICurrentUserClaimInterrogator>();
            Expect.Call(currentUserClaimInterrogator.HasClaimForLocalEducationAgencyWithinEducationOrganizationHierarchy(
                    EdFiClaimTypes.AdministerDashboard, LoginHelper.localEducationAgencyTwoId)).Return(false);
            base.EstablishContext();
        }
    }

    public class When_checking_if_kill_switch_is_activated_for_impersonating_teacher_when_activated : SiteAvailableProviderTestFixtureBase
    {
        protected override void EstablishContext()
        {
            LoginHelper.ImpersonateTeacherOne();
            expectedResult = false;
            currentUserClaimInterrogator = mocks.StrictMock<ICurrentUserClaimInterrogator>();
            Expect.Call(currentUserClaimInterrogator.HasClaimForLocalEducationAgencyWithinEducationOrganizationHierarchy(
                    EdFiClaimTypes.AdministerDashboard, LoginHelper.localEducationAgencyOneId)).Return(false);
            base.EstablishContext();
        }
    }
}
