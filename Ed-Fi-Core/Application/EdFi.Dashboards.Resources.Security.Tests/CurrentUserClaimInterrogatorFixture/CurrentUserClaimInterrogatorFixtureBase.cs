using EdFi.Dashboards.Core;
using EdFi.Dashboards.Infrastructure.Implementations;
using EdFi.Dashboards.Metric.Resources.Models;
using EdFi.Dashboards.Resources.LocalEducationAgency;
using EdFi.Dashboards.Resources.Metric;
using EdFi.Dashboards.Resources.Models.LocalEducationAgency;
using EdFi.Dashboards.Resources.Models.School;
using EdFi.Dashboards.Resources.School;
using EdFi.Dashboards.Resources.Security.Implementations;
using EdFi.Dashboards.Resources.Tests;
using EdFi.Dashboards.SecurityTokenService.Authentication;
using EdFi.Dashboards.Testing;
using NUnit.Framework;
using Rhino.Mocks;

namespace EdFi.Dashboards.Resources.Security.Tests.CurrentUserClaimInterrogatorFixture
{
    [TestFixture]
    public abstract class CurrentUserClaimInterrogatorFixtureBase : TestFixtureBase
    {
        protected IIdNameService schoolIdNameService;
        protected CurrentUserClaimInterrogator currentUserClaimInterrogator;
        protected IDomainSpecificMetricNodeResolver domainSpecificMetricNodeResolver;
        protected IIdCodeService idCodeService;
        protected HashtableSessionStateProvider sessionStateProvider;
        protected bool? result;

        protected override void EstablishContext()
        {
            base.EstablishContext();
            sessionStateProvider = new HashtableSessionStateProvider();
            currentUserClaimInterrogator = new CurrentUserClaimInterrogator(sessionStateProvider);
            schoolIdNameService = mocks.StrictMock<IIdNameService>();
            currentUserClaimInterrogator.SchoolIdNameService = schoolIdNameService;
            idCodeService = mocks.StrictMock<IIdCodeService>();
            currentUserClaimInterrogator.LeaIdCodeService = idCodeService;
            domainSpecificMetricNodeResolver = mocks.StrictMock<IDomainSpecificMetricNodeResolver>();
            currentUserClaimInterrogator.MetricNodeResolver = domainSpecificMetricNodeResolver;

            ExpectSchoolIdNameServiceGetAny();
            ExpectLocalEducationAgencyCodeServiceGet();
        }

        protected override void BeforeExecuteTest()
        {
            result = null;
            base.BeforeExecuteTest();
        }

        /// <summary>
        /// Executes the code to be tested.
        /// </summary>
        protected override void ExecuteTest()
        {
        }

        protected virtual void ExpectLocalEducationAgencyCodeServiceGet()
        {
            ExpectLocalEducationAgencyCodeServiceGetAny();
        }

        protected void ExpectLocalEducationAgencyCodeServiceGetAny()
        {
            SetupResult.For(idCodeService.Get(null)).Constraints(
                new FuncConstraint<IdCodeRequest>(
                    r => r.LocalEducationAgencyId == LoginHelper.localEducationAgencyOneId)).Return(
                        new IdCodeModel {LocalEducationAgencyId = LoginHelper.localEducationAgencyOneId});
            SetupResult.For(idCodeService.Get(null)).Constraints(
                new FuncConstraint<IdCodeRequest>(
                    r => r.LocalEducationAgencyId == LoginHelper.localEducationAgencyTwoId)).Return(
                        new IdCodeModel {LocalEducationAgencyId = LoginHelper.localEducationAgencyTwoId});
            SetupResult.For(idCodeService.Get(null)).Constraints(
                new FuncConstraint<IdCodeRequest>(
                    r => r.LocalEducationAgencyId == LoginHelper.schoolOneId)).Return(
                        null);
            SetupResult.For(idCodeService.Get(null)).Constraints(
                new FuncConstraint<IdCodeRequest>(
                    r => r.LocalEducationAgencyId == LoginHelper.schoolTwoId)).Return(
                        null);
        }

        protected void ExpectSchoolIdNameServiceGetAny()
        {
            SetupResult.For(schoolIdNameService.Get(null)).Constraints(
                new FuncConstraint<IdNameRequest>(r => r.SchoolId == LoginHelper.localEducationAgencyOneId))
                .Return(null);

            SetupResult.For(schoolIdNameService.Get(null)).Constraints(
                new FuncConstraint<IdNameRequest>(r => r.SchoolId == LoginHelper.localEducationAgencyTwoId))
                .Return(null);

            // The IdNameService is invoke by the GetEducationOrganizationHierarchy, which is invoked
            // for each EducationOrganizationId: SchoolId, or LEAId.  Set up the expectations for all
            // permutations.
            //
            SetupResult.For(schoolIdNameService.Get(null)).Constraints(
                new FuncConstraint<IdNameRequest>(r => r.SchoolId == LoginHelper.schoolOneId))
                .Return(new IdNameModel { LocalEducationAgencyId = LoginHelper.localEducationAgencyOneId });

            SetupResult.For(schoolIdNameService.Get(null)).Constraints(
                new FuncConstraint<IdNameRequest>(r => r.SchoolId == LoginHelper.schoolTwoId))
                .Return(new IdNameModel { LocalEducationAgencyId = LoginHelper.localEducationAgencyTwoId });
        }

        protected void ExpectMetricNodeResolverGetAny()
        {
            Expect.Call(domainSpecificMetricNodeResolver.GetOperationalDashboardMetricNode(
                MetricInstanceSetType.School, LoginHelper.schoolOneId)).Return(new MetricMetadataNode(null));
        }


        protected void WhenAPrincipalTeacher()
        {
            LoginHelper.LoginUserTeacherOnePrincipalTwo();
        }

        protected void WhenASuperintendent()
        {
            LoginHelper.LoginSuperintendent();
        }

        protected void WhenASchoolSpecialist()
        {
            LoginHelper.LoginSchoolSpecialistOne();
        }

        protected void WhenAStateAdministrator()
        {
            LoginHelper.LoginUserWithStateLevelClaim();
        }
    }

    public class WhenASchoolSpecialistRequestsHasAccessOrganizationClaimForTheirSchoolWithinEducationOrganizationHierarchy : CurrentUserClaimInterrogatorFixtureBase
    {
        protected override void EstablishContext()
        {
            base.EstablishContext();
            WhenASchoolSpecialist();
        }

        protected override void ExecuteTest()
        {
            result = currentUserClaimInterrogator.HasClaimWithinEducationOrganizationHierarchy(EdFiClaimTypes.AccessOrganization, LoginHelper.schoolOneId);
        }

        [Test]
        public void ShouldSucceed()
        {
            Assert.That(result, Is.True);
        }
    }
    public class WhenASchoolSpecialistRequestsHasAccessOrganizationClaimForTheirSchoolsLocalEducationAgencyWithinEducationOrganizationHierarchy : CurrentUserClaimInterrogatorFixtureBase
    {
        protected override void EstablishContext()
        {
            base.EstablishContext();
            WhenASchoolSpecialist();
        }

        protected override void ExecuteTest()
        {
            result = currentUserClaimInterrogator.HasClaimForLocalEducationAgencyWithinEducationOrganizationHierarchy(EdFiClaimTypes.AccessOrganization, LoginHelper.schoolOneId);
        }

        [Test]
        public void ShouldFail()
        {
            Assert.That(result, Is.False);
        }
    }
    public class WhenASuperintendentRequestsHasAccessOrganizationClaimForDifferentLocalEducationAgencyWithinEducationOrganizationHierarchy : CurrentUserClaimInterrogatorFixtureBase
    {
        protected override void EstablishContext()
        {
            base.EstablishContext();
            WhenASuperintendent();
        }

        protected override void ExecuteTest()
        {
            result = currentUserClaimInterrogator.HasClaimWithinEducationOrganizationHierarchy(EdFiClaimTypes.AccessOrganization, LoginHelper.localEducationAgencyTwoId);
        }

        [Test]
        public void ShouldFail()
        {
            Assert.That(result, Is.False);
        }
    }
    public class WhenASuperintendentRequestsHasAccessOrganizationClaimForLocalEducationAgencyWithinEducationOrganizationHierarchy : CurrentUserClaimInterrogatorFixtureBase
    {
        protected override void EstablishContext()
        {
            base.EstablishContext();
            WhenASuperintendent();
        }

        protected override void ExecuteTest()
        {
            result = currentUserClaimInterrogator.HasClaimForLocalEducationAgencyWithinEducationOrganizationHierarchy(EdFiClaimTypes.AccessOrganization, LoginHelper.localEducationAgencyOneId);
        }

        [Test]
        public void ShouldSucceed()
        {
            Assert.That(result, Is.True);
        }
    }
    public class WhenAPrincipalTeacherRequestsHasViewOperationalDashboardClaimForTeacherSchoolWithinEducationOrganizationHierarchy : CurrentUserClaimInterrogatorFixtureBase
    {
        protected override void EstablishContext()
        {
            base.EstablishContext();
            WhenAPrincipalTeacher();
        }

        protected override void ExecuteTest()
        {
            result = currentUserClaimInterrogator.HasClaimWithinEducationOrganizationHierarchy(EdFiClaimTypes.ViewOperationalDashboard, LoginHelper.schoolOneId);
        }

        [Test]
        public void ShouldFail()
        {
            Assert.That(result, Is.False);
        }
    }
    public class WhenAPrincipalTeacherRequestsHasViewOperationalDashboardClaimForPrincipalSchoolWithinEducationOrganizationHierarchy : CurrentUserClaimInterrogatorFixtureBase
    {
        protected override void EstablishContext()
        {
            base.EstablishContext();
            WhenAPrincipalTeacher();
        }

        protected override void ExecuteTest()
        {
            result = currentUserClaimInterrogator.HasClaimWithinEducationOrganizationHierarchy(EdFiClaimTypes.ViewOperationalDashboard, LoginHelper.schoolTwoId);
        }

        [Test]
        public void ShouldSucceed()
        {
            Assert.That(result, Is.True);
        }
    }
    public class WhenAStateAdministratorRequestsHasViewAllStudentsClaimForSchoolsAndLocalEducationAgenciesWithinEducationOrganizationHierarchy : CurrentUserClaimInterrogatorFixtureBase
    {
        protected override void EstablishContext()
        { 
            base.EstablishContext();
            WhenAStateAdministrator();
        }

        protected override void ExecuteTest()
        {
            result =
                currentUserClaimInterrogator.HasClaimWithinEducationOrganizationHierarchy(
                    EdFiClaimTypes.ViewAllStudents, LoginHelper.schoolTwoId) &&
                currentUserClaimInterrogator.HasClaimWithinEducationOrganizationHierarchy(
                    EdFiClaimTypes.ViewAllStudents, LoginHelper.schoolOneId) &&
                currentUserClaimInterrogator.HasClaimWithinEducationOrganizationHierarchy(
                    EdFiClaimTypes.ViewAllStudents, LoginHelper.localEducationAgencyOneId) &&
                currentUserClaimInterrogator.HasClaimWithinEducationOrganizationHierarchy(
                    EdFiClaimTypes.ViewAllStudents, LoginHelper.localEducationAgencyTwoId);
        }

        [Test]
        public void ShouldSucceed()
        {
            Assert.That(result, Is.True);
        }
    }

    public class WhenAStateAdministratorRequestsHasViewAllStudentsClaimForStateAgency : CurrentUserClaimInterrogatorFixtureBase
    {
        protected override void EstablishContext()
        {
            base.EstablishContext();
            WhenAStateAdministrator();
        }

        protected override void ExecuteTest()
        {
            result = currentUserClaimInterrogator.HasClaimForStateAgency(EdFiClaimTypes.ViewAllStudents);
        }

        [Test]
        public void ShouldSucceed()
        {
            Assert.That(result, Is.True);
        }
    }

    public class WhenASuperintendentRequestsHasViewAllStudentsClaimForStateAgency : CurrentUserClaimInterrogatorFixtureBase
    {
        protected override void EstablishContext()
        {
            base.EstablishContext();
            WhenASuperintendent();
        }

        protected override void ExecuteTest()
        {
            result = currentUserClaimInterrogator.HasClaimForStateAgency(EdFiClaimTypes.ViewAllStudents);
        }

        [Test]
        public void ShouldFail()
        {
            Assert.That(result, Is.False);
        }
    }
}
