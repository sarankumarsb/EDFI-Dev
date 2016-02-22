// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using EdFi.Dashboards.Common.Utilities.CastleWindsorInstallers;
using EdFi.Dashboards.Data.Entities;
using EdFi.Dashboards.Data.Repository;
using EdFi.Dashboards.Infrastructure.CastleWindsorInstallers;
using EdFi.Dashboards.Metric.Resources.Models;
using EdFi.Dashboards.Resources.Metric;
using EdFi.Dashboards.Resources.Navigation;
using EdFi.Dashboards.Resources.Navigation.UserEntryProviders;
using EdFi.Dashboards.Resources.Security.Common;
using EdFi.Dashboards.Resources.Tests.Navigation.Fakes;
using EdFi.Dashboards.SecurityTokenService.Authentication;
using EdFi.Dashboards.Testing;
using NUnit.Framework;
using Rhino.Mocks;

namespace EdFi.Dashboards.Resources.Tests
{
    [TestFixture]
    public abstract class EntryServiceFixtureBase : TestFixtureBase
    {
        protected IWindsorContainer windsorContainer;
        protected IRepository<StaffStudentAssociation> staffStudentAssociationRepository;
        protected IRootMetricNodeResolver rootMetricNodeResolver;
        protected ICurrentUserClaimInterrogator currentUserClaimInterrogator;
        protected IUserEntryProvider userEntryProvider;

        protected UserInformation suppliedUserInformation;
        protected const int suppliedLocalEducationAgencyId = 1000;
        protected string suppliedLocalEducationAgencyName = "LEA " + suppliedLocalEducationAgencyId;

        protected const int suppliedSchoolId = 2000;
        protected string suppliedSchoolName = "School " + suppliedSchoolId;

        protected const int suppliedSchoolId2 = 2001;
        protected string suppliedSchoolName2 = "School " + suppliedSchoolId;
        
        protected const int suppliedStaffUSI = 3000;
        protected const string suppliedStaffFullName = "John Q. Doe";
        protected const int suppliedMetricId = 4000;

        protected string expectedUrl;

        protected string actualUrl;

        protected LocalEducationAgencyAreaLinksFake localEducationAgencyAreaLinksFake = new LocalEducationAgencyAreaLinksFake();
        protected StaffAreaLinksFake staffAreaLinksFake = new StaffAreaLinksFake();
        protected SchoolAreaLinksFake schoolAreaLinksFake = new SchoolAreaLinksFake();
        protected AdminAreaLinksFake adminAreaLinksFake = new AdminAreaLinksFake();

        protected override void EstablishContext()
        {
            base.EstablishContext();

            staffStudentAssociationRepository = mocks.StrictMock<IRepository<StaffStudentAssociation>>();
            rootMetricNodeResolver = mocks.StrictMock<IRootMetricNodeResolver>();
            currentUserClaimInterrogator = mocks.StrictMock<ICurrentUserClaimInterrogator>();

            windsorContainer = new WindsorContainer();
            RegisterProviders(windsorContainer);

            userEntryProvider = windsorContainer.Resolve<IUserEntryProvider>();

            Expect.Call(staffStudentAssociationRepository.GetAll()).Repeat.Any().Return(GetStaffStudentAssociations());
            Expect.Call(rootMetricNodeResolver.GetRootMetricNodeForSchool(suppliedSchoolId)).Repeat.Any().Return(new MetricMetadataNode (new TestMetricMetadataTree()) {MetricId = suppliedMetricId});

            Thread.CurrentPrincipal = suppliedUserInformation.ToClaimsPrincipal();
        }

        private void RegisterProviders(IWindsorContainer container)
        {
            var assemblyTypes = typeof(Marker_EdFi_Dashboards_Resources).Assembly.GetTypes();

            var chainTypes = (from t in assemblyTypes
                              where
                                  typeof(IUserEntryProvider).IsAssignableFrom(t) &&
                                  t != typeof(IUserEntryProvider)
                              orderby t.GetCustomAttributes(true)
                                  .Where(a => a is ChainOfResponsibilityOrderAttribute)
                                  .Select(a => ((ChainOfResponsibilityOrderAttribute)a).Order)
                                  .SingleOrDefault()
                              select t);

            var chainRegistrar = new ChainOfResponsibilityRegistrar(container);
            chainRegistrar.RegisterChainOf<IUserEntryProvider, NullUserEntryProvider>(chainTypes.ToArray());

            container.Register(Component
                .For<ICurrentUserClaimInterrogator>()
                .Instance(currentUserClaimInterrogator));

            container.Register(Component
                .For<IAdminAreaLinks>()
                .Instance(adminAreaLinksFake));

            container.Register(Component
                .For<ILocalEducationAgencyAreaLinks>()
                .Instance(localEducationAgencyAreaLinksFake));

            container.Register(Component
                .For<ISchoolAreaLinks>()
                .Instance(schoolAreaLinksFake));

            container.Register(Component
                .For<IStaffAreaLinks>()
                .Instance(staffAreaLinksFake));

            container.Register(Component
                .For<IRepository<StaffStudentAssociation>>()
                .Instance(staffStudentAssociationRepository));
        }

        protected virtual IQueryable<StaffStudentAssociation> GetStaffStudentAssociations()
        {
            return new List<StaffStudentAssociation>().AsQueryable();
        }

        protected UserInformation GetUserInformationWithSchoolClaim(List<string> claimTypes)
        {
            return new UserInformation
                        {
                            StaffUSI = suppliedStaffUSI,
                            FullName = suppliedStaffFullName,
                            AssociatedOrganizations = new List<UserInformation.EducationOrganization>
                                                        {
                                                            new UserInformation.School(suppliedLocalEducationAgencyId, suppliedSchoolId)
                                                                {
                                                                    Name = suppliedSchoolName,
                                                                    ClaimTypes = claimTypes
                                                                }
                                                        }
                        };
        }

        protected UserInformation GetUserInformationWithLocalEducationAgencyClaim(List<string> claimTypes)
        {
            return new UserInformation
                        {
                            StaffUSI = suppliedStaffUSI,
                            FullName = suppliedStaffFullName,
                            AssociatedOrganizations = new List<UserInformation.EducationOrganization>
                                                            {
                                                                new UserInformation.LocalEducationAgency(suppliedLocalEducationAgencyId)
                                                                    {
                                                                        Name = suppliedLocalEducationAgencyName,
                                                                        ClaimTypes = claimTypes
                                                                    }
                                                            }
                        };
        }

        protected override void ExecuteTest()
        {
            var service = new EntryService(userEntryProvider);
            actualUrl = service.Get(EntryRequest.Create(suppliedLocalEducationAgencyId, null));
        }

        [Test]
        public void Returns_expected_url()
        {
            Assert.That(actualUrl, Is.EqualTo(expectedUrl));
        }
    }

    public class Find_landing_page_for_teacher_with_students : EntryServiceFixtureBase
    {
        protected override void EstablishContext()
        {
            suppliedUserInformation = GetUserInformationWithSchoolClaim(new List<string>{EdFiClaimTypes.ViewMyStudents, EdFiClaimTypes.ViewAllMetrics});
            expectedUrl = staffAreaLinksFake.Default(suppliedSchoolId, suppliedStaffUSI, suppliedStaffFullName);

            base.EstablishContext();
        }

        protected override IQueryable<StaffStudentAssociation> GetStaffStudentAssociations()
        {
            return new List<StaffStudentAssociation>
                       {
                           new StaffStudentAssociation{StaffUSI = suppliedStaffUSI, SchoolId = suppliedSchoolId, StudentUSI = 1}
                       }.AsQueryable();
        }
    }
    
    public class Find_landing_page_for_teacher_with_no_students : EntryServiceFixtureBase
    {
        protected override void EstablishContext()
        {
            suppliedUserInformation = GetUserInformationWithSchoolClaim(new List<string> { EdFiClaimTypes.ViewMyStudents, EdFiClaimTypes.ViewMyMetrics });
            expectedUrl = schoolAreaLinksFake.Overview(suppliedSchoolId, suppliedSchoolName);

            base.EstablishContext();
        }
        
    }

    public class Find_landing_page_for_teacher_multiple_schools : EntryServiceFixtureBase
    {
        protected override void EstablishContext()
        {
            suppliedUserInformation = new UserInformation
            {
                StaffUSI = suppliedStaffUSI,
                FullName = suppliedStaffFullName,
                AssociatedOrganizations = new List<UserInformation.EducationOrganization>
                                                        {
                                                            new UserInformation.School(suppliedLocalEducationAgencyId, suppliedSchoolId)
                                                                {
                                                                    Name = suppliedSchoolName,
                                                                    ClaimTypes = new List<string>{EdFiClaimTypes.ViewMyStudents, EdFiClaimTypes.ViewAllMetrics}
                                                                },
                                                            new UserInformation.School(suppliedLocalEducationAgencyId, suppliedSchoolId2)
                                                                {
                                                                    Name = suppliedSchoolName2,
                                                                    ClaimTypes = new List<string>{EdFiClaimTypes.ViewMyStudents, EdFiClaimTypes.ViewAllMetrics}
                                                                },
                                                        }
            };
            expectedUrl = staffAreaLinksFake.Default(suppliedSchoolId2, suppliedStaffUSI, suppliedStaffFullName);

            base.EstablishContext();
        }

        protected override IQueryable<StaffStudentAssociation> GetStaffStudentAssociations()
        {
            return new List<StaffStudentAssociation>
                       {
                           new StaffStudentAssociation{StaffUSI = suppliedStaffUSI, SchoolId = suppliedSchoolId2, StudentUSI = 1}
                       }.AsQueryable();
        }
    }

    public class Find_landing_page_for_school_administrator : EntryServiceFixtureBase
    {
        protected override void EstablishContext()
        {
            suppliedUserInformation = GetUserInformationWithSchoolClaim(new List<string>{EdFiClaimTypes.ViewMyMetrics});
            expectedUrl = schoolAreaLinksFake.Overview(suppliedSchoolId, suppliedSchoolName);

            base.EstablishContext();
        }
        
    }

    public class Find_landing_page_for_principal : EntryServiceFixtureBase
    {
        protected override void EstablishContext()
        {
            suppliedUserInformation = GetUserInformationWithSchoolClaim(new List<string>{EdFiClaimTypes.ViewAllStudents});
            expectedUrl = schoolAreaLinksFake.Overview(suppliedSchoolId, suppliedSchoolName);

            base.EstablishContext();
        }
        
    }

    public class Find_landing_page_for_local_education_agency_administrator : EntryServiceFixtureBase
    {
        protected override void EstablishContext()
        {
            suppliedUserInformation = GetUserInformationWithLocalEducationAgencyClaim(new List<string>{EdFiClaimTypes.ViewMyMetrics});
            expectedUrl = localEducationAgencyAreaLinksFake.Overview(suppliedLocalEducationAgencyId);

            base.EstablishContext();
        }
        
    }

    public class Find_landing_page_for_superintendent : EntryServiceFixtureBase
    {
        protected override void EstablishContext()
        {
            suppliedUserInformation = GetUserInformationWithLocalEducationAgencyClaim(new List<string>{EdFiClaimTypes.ViewAllStudents});
            expectedUrl = localEducationAgencyAreaLinksFake.Overview(suppliedLocalEducationAgencyId);

            base.EstablishContext();
        }
        
    }

    public class Find_landing_page_for_system_administrator : EntryServiceFixtureBase
    {
        protected override void EstablishContext()
        {
            suppliedUserInformation = GetUserInformationWithLocalEducationAgencyClaim(new List<string>{EdFiClaimTypes.AdministerDashboard});
            expectedUrl = adminAreaLinksFake.Configuration(suppliedLocalEducationAgencyId);

            base.EstablishContext();
        }
        
    }

    public class Find_landing_page_with_no_claims : TestFixtureBase
    {
        protected IWindsorContainer windsorContainer;
        protected IRepository<StaffStudentAssociation> staffStudentAssociationRepository;
        protected IRootMetricNodeResolver rootMetricNodeResolver;
        protected ICurrentUserClaimInterrogator currentUserClaimInterrogator;
        protected IUserEntryProvider userEntryProvider;

        protected int suppliedLocalEducationAgencyId = 1000;
        protected int suppliedSchoolId = 2000;
        protected int suppliedStaffUSI = 3000;

        protected string expectedModel;

        protected string actualModel;
        private Exception actualException;

        protected LocalEducationAgencyAreaLinksFake localEducationAgencyAreaLinksFake = new LocalEducationAgencyAreaLinksFake();
        protected StaffAreaLinksFake staffAreaLinksFake = new StaffAreaLinksFake();
        protected SchoolAreaLinksFake schoolAreaLinksFake = new SchoolAreaLinksFake();
        protected AdminAreaLinksFake adminAreaLinksFake = new AdminAreaLinksFake();

        protected override void EstablishContext()
        {
            base.EstablishContext();

            staffStudentAssociationRepository = mocks.StrictMock<IRepository<StaffStudentAssociation>>();
            rootMetricNodeResolver = mocks.StrictMock<IRootMetricNodeResolver>();
            currentUserClaimInterrogator = mocks.StrictMock<ICurrentUserClaimInterrogator>();
            windsorContainer = new WindsorContainer();
            RegisterProviders(windsorContainer);

            userEntryProvider = windsorContainer.Resolve<IUserEntryProvider>();

            var suppliedUserInformation = new UserInformation
                                                {
                                                    StaffUSI = suppliedStaffUSI,
                                                    AssociatedOrganizations = new List<UserInformation.EducationOrganization>()
                                                                                {
                                                                                    new UserInformation.School(suppliedLocalEducationAgencyId, suppliedSchoolId)
                                                                                        {
                                                                                            ClaimTypes = new List<string>
                                                                                                                {
                                                                                                                    EdFiClaimTypes.ViewAllTeachers
                                                                                                                }
                                                                                        }
                                                                                }
                                                };
            Thread.CurrentPrincipal = suppliedUserInformation.ToClaimsPrincipal();
        }

        private void RegisterProviders(IWindsorContainer container)
        {
            var assemblyTypes = typeof(Marker_EdFi_Dashboards_Resources).Assembly.GetTypes();

            var chainTypes = (from t in assemblyTypes
                              where
                                  typeof(IUserEntryProvider).IsAssignableFrom(t) &&
                                  t != typeof(IUserEntryProvider)
                              orderby t.GetCustomAttributes(true)
                                  .Where(a => a is ChainOfResponsibilityOrderAttribute)
                                  .Select(a => ((ChainOfResponsibilityOrderAttribute)a).Order)
                                  .SingleOrDefault()
                              select t);

            var chainRegistrar = new ChainOfResponsibilityRegistrar(container);
            chainRegistrar.RegisterChainOf<IUserEntryProvider, NullUserEntryProvider>(chainTypes.ToArray());

            container.Register(Component
                .For<ICurrentUserClaimInterrogator>()
                .Instance(currentUserClaimInterrogator));

            container.Register(Component
                .For<IAdminAreaLinks>()
                .Instance(adminAreaLinksFake));

            container.Register(Component
                .For<ILocalEducationAgencyAreaLinks>()
                .Instance(localEducationAgencyAreaLinksFake));

            container.Register(Component
                .For<ISchoolAreaLinks>()
                .Instance(schoolAreaLinksFake));

            container.Register(Component
                .For<IStaffAreaLinks>()
                .Instance(staffAreaLinksFake));

            container.Register(Component
                .For<IRepository<StaffStudentAssociation>>()
                .Instance(staffStudentAssociationRepository));
        }

        protected virtual IQueryable<StaffStudentAssociation> GetStaffStudentAssociations()
        {
            return new List<StaffStudentAssociation>().AsQueryable();
        }

        protected override void ExecuteTest()
        {
            try
            {
                var service = new EntryService(userEntryProvider);
                actualModel = service.Get(EntryRequest.Create(suppliedLocalEducationAgencyId, suppliedSchoolId));
            }
            catch (Exception ex)
            {
                actualException = ex;
            }
        }

        [Test]
        public void Throws_exception_when_there_are_no_claims()
        {
            Assert.That(actualException, Is.TypeOf<DashboardsAuthenticationException>());
        }
    }
}

