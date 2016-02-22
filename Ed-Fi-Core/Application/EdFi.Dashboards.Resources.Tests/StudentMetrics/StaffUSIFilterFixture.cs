using System.Collections.Generic;
using System.Linq;
using EdFi.Dashboards.Data.Entities;
using EdFi.Dashboards.Data.Repository;
using EdFi.Dashboards.Resources.StudentMetrics;
using EdFi.Dashboards.Testing;
using NUnit.Framework;
using Rhino.Mocks;

namespace EdFi.Dashboards.Resources.Tests.StudentMetrics
{
    public class When_Getting_StudentList_With_StaffUSIFilter_With_Nothing_Set : TestFixtureBase
    {
        private StaffUSIFilter filter;
        private IQueryable<EnhancedStudentInformation> query;
        private IQueryable<EnhancedStudentInformation> studentResults;

        private IRepository<StaffStudentCohort> staffStudentCohortRepository;
        private IRepository<StaffCohort> staffCohortRepository;
        private IRepository<StaffStudentAssociation> staffStudentAssociationRepository;

        private int SuppliedStaffUSI1 = 101;
        private int SuppliedStaffUSI2 = 102;

        protected override void EstablishContext()
        {
            staffStudentCohortRepository = mocks.StrictMock<IRepository<StaffStudentCohort>>();
            Expect.Call(staffStudentCohortRepository.GetAll()).Return(new List<StaffStudentCohort>
            {
                new StaffStudentCohort { StaffCohortId = 1, StudentUSI = 1 },
                new StaffStudentCohort { StaffCohortId = 1, StudentUSI = 2 },
                new StaffStudentCohort { StaffCohortId = 1, StudentUSI = 3 },
            }.AsQueryable()).Repeat.Any();

            staffCohortRepository = mocks.StrictMock<IRepository<StaffCohort>>();
            Expect.Call(staffCohortRepository.GetAll()).Return(new List<StaffCohort>
            {
                new StaffCohort { StaffCohortId = 1, StaffUSI = SuppliedStaffUSI1 },
                new StaffCohort { StaffCohortId = 2, StaffUSI = SuppliedStaffUSI2 },
            }.AsQueryable()).Repeat.Any();

            staffStudentAssociationRepository = mocks.StrictMock<IRepository<StaffStudentAssociation>>();
            Expect.Call(staffStudentAssociationRepository.GetAll()).Return(new List<StaffStudentAssociation>
            {
                new StaffStudentAssociation { SchoolId = 1, StaffUSI = SuppliedStaffUSI1, StudentUSI = 1 },
                new StaffStudentAssociation { SchoolId = 1, StaffUSI = SuppliedStaffUSI2, StudentUSI = 2 },
                new StaffStudentAssociation { SchoolId = 1, StaffUSI = SuppliedStaffUSI2, StudentUSI = 3 },
            }.AsQueryable()).Repeat.Any();


            query = new List<EnhancedStudentInformation>
            {
                new EnhancedStudentInformation { StudentUSI = 1, SchoolId = 1, },
                new EnhancedStudentInformation { StudentUSI = 2, SchoolId = 1, },
                new EnhancedStudentInformation { StudentUSI = 3, SchoolId = 1, },
            }.AsQueryable();

            filter = new StaffUSIFilter(staffStudentCohortRepository, staffCohortRepository, staffStudentAssociationRepository);

            base.EstablishContext();
        }

        protected override void ExecuteTest()
        {
            studentResults = filter.ApplyFilter(query, new StudentMetricsProviderQueryOptions
            {
            });
        }

        [Test]
        public void Should_return_results()
        {
            Assert.That(studentResults.Count(), Is.EqualTo(3));
            Assert.That(studentResults.First().StudentUSI, Is.EqualTo(1));
            Assert.That(studentResults.Skip(1).First().StudentUSI, Is.EqualTo(2));
            Assert.That(studentResults.Skip(2).First().StudentUSI, Is.EqualTo(3));
        }
    }

    public class When_Getting_StudentList_With_StaffUSIFilter : TestFixtureBase
    {
        private StaffUSIFilter filter;
        private IQueryable<EnhancedStudentInformation> query;
        private IQueryable<EnhancedStudentInformation> studentResults;

        private IRepository<StaffStudentCohort> staffStudentCohortRepository;
        private IRepository<StaffCohort> staffCohortRepository;
        private IRepository<StaffStudentAssociation> staffStudentAssociationRepository;

        private int SuppliedStaffUSI1 = 101;
        private int SuppliedStaffUSI2 = 102;

        protected override void EstablishContext()
        {
            staffStudentCohortRepository = mocks.StrictMock<IRepository<StaffStudentCohort>>();
            Expect.Call(staffStudentCohortRepository.GetAll()).Return(new List<StaffStudentCohort>
            {
                new StaffStudentCohort { StaffCohortId = 1, StudentUSI = 1 },
                new StaffStudentCohort { StaffCohortId = 1, StudentUSI = 2 },
                new StaffStudentCohort { StaffCohortId = 1, StudentUSI = 3 },
            }.AsQueryable()).Repeat.Any();

            staffCohortRepository = mocks.StrictMock<IRepository<StaffCohort>>();
            Expect.Call(staffCohortRepository.GetAll()).Return(new List<StaffCohort>
            {
                new StaffCohort { StaffCohortId = 1, StaffUSI = SuppliedStaffUSI1 },
                new StaffCohort { StaffCohortId = 2, StaffUSI = SuppliedStaffUSI2 },
            }.AsQueryable()).Repeat.Any();

            staffStudentAssociationRepository = mocks.StrictMock<IRepository<StaffStudentAssociation>>();
            Expect.Call(staffStudentAssociationRepository.GetAll()).Return(new List<StaffStudentAssociation>
            {
                new StaffStudentAssociation { SchoolId = 1, StaffUSI = SuppliedStaffUSI1, StudentUSI = 1 },
                new StaffStudentAssociation { SchoolId = 1, StaffUSI = SuppliedStaffUSI2, StudentUSI = 2 },
                new StaffStudentAssociation { SchoolId = 1, StaffUSI = SuppliedStaffUSI2, StudentUSI = 3 },
            }.AsQueryable()).Repeat.Any();


            query = new List<EnhancedStudentInformation>
            {
                new EnhancedStudentInformation { StudentUSI = 1, SchoolId = 1, },
                new EnhancedStudentInformation { StudentUSI = 2, SchoolId = 1, },
                new EnhancedStudentInformation { StudentUSI = 3, SchoolId = 1, },
            }.AsQueryable();

            filter = new StaffUSIFilter(staffStudentCohortRepository, staffCohortRepository, staffStudentAssociationRepository);

            base.EstablishContext();
        }

        protected override void ExecuteTest()
        {
            studentResults = filter.ApplyFilter(query, new StudentMetricsProviderQueryOptions
            {
                StaffUSI = SuppliedStaffUSI2,
                SchoolId = 1
            });
        }

        [Test]
        public void Should_return_results()
        {
            Assert.That(studentResults.Count(), Is.EqualTo(2));
            Assert.That(studentResults.First().StudentUSI, Is.EqualTo(2));
            Assert.That(studentResults.Skip(1).First().StudentUSI, Is.EqualTo(3));
        }
    }

    public class When_Getting_StudentList_With_StaffUSI_and_LocalEducationAgency_Filter : TestFixtureBase
    {
        private StaffUSIFilter filter;
        private IQueryable<EnhancedStudentInformation> query;
        private IQueryable<EnhancedStudentInformation> studentResults;

        private IRepository<StaffStudentCohort> staffStudentCohortRepository;
        private IRepository<StaffCohort> staffCohortRepository;
        private IRepository<StaffStudentAssociation> staffStudentAssociationRepository;

        private int SuppliedStaffUSI1 = 101;
        private int SuppliedStaffUSI2 = 102;
        private int SuppliedLocalEducationAgencyId1 = 3;
        private int SuppliedLocalEducationAgencyId2 = 4;

        protected override void EstablishContext()
        {
            staffStudentCohortRepository = mocks.StrictMock<IRepository<StaffStudentCohort>>();
            Expect.Call(staffStudentCohortRepository.GetAll()).Return(new List<StaffStudentCohort>
            {
                new StaffStudentCohort { StaffCohortId = 1, StudentUSI = 1 },
                new StaffStudentCohort { StaffCohortId = 1, StudentUSI = 2 },
                new StaffStudentCohort { StaffCohortId = 1, StudentUSI = 3 },
            }.AsQueryable()).Repeat.Any();

            staffCohortRepository = mocks.StrictMock<IRepository<StaffCohort>>();
            Expect.Call(staffCohortRepository.GetAll()).Return(new List<StaffCohort>
            {
                new StaffCohort { StaffCohortId = 1, StaffUSI = SuppliedStaffUSI1 },
                new StaffCohort { StaffCohortId = 2, StaffUSI = SuppliedStaffUSI2 },
            }.AsQueryable()).Repeat.Any();

            staffStudentAssociationRepository = mocks.StrictMock<IRepository<StaffStudentAssociation>>();
            Expect.Call(staffStudentAssociationRepository.GetAll()).Return(new List<StaffStudentAssociation>
            {
                new StaffStudentAssociation { SchoolId = 1, StaffUSI = SuppliedStaffUSI1, StudentUSI = 1 },
                new StaffStudentAssociation { SchoolId = 1, StaffUSI = SuppliedStaffUSI2, StudentUSI = 2 },
                new StaffStudentAssociation { SchoolId = 1, StaffUSI = SuppliedStaffUSI2, StudentUSI = 3 },
            }.AsQueryable()).Repeat.Any();
            
            query = new List<EnhancedStudentInformation>
            {
                new EnhancedStudentInformation { LocalEducationAgencyId = SuppliedLocalEducationAgencyId2, StudentUSI = 1, SchoolId = 1, },
                new EnhancedStudentInformation { LocalEducationAgencyId = SuppliedLocalEducationAgencyId1, StudentUSI = 2, SchoolId = 1, },
                new EnhancedStudentInformation { LocalEducationAgencyId = SuppliedLocalEducationAgencyId1, StudentUSI = 3, SchoolId = 1, },
            }.AsQueryable();

            filter = new StaffUSIFilter(staffStudentCohortRepository, staffCohortRepository, staffStudentAssociationRepository);

            base.EstablishContext();
        }

        protected override void ExecuteTest()
        {
            studentResults = filter.ApplyFilter(query, new StudentMetricsProviderQueryOptions
            {
                LocalEducationAgencyId = SuppliedLocalEducationAgencyId1,
                StaffUSI = SuppliedStaffUSI1,
            });
        }

        [Test]
        public void Should_return_results()
        {
            //Note, this code ignore ensuring the LEA of the student agrees with the LEA in the request.
            //   That is the responsiblity of the LEA filter, not this one.
            Assert.That(studentResults.Count(), Is.EqualTo(3));
            Assert.That(studentResults.First().StudentUSI, Is.EqualTo(1));
            Assert.That(studentResults.Skip(1).First().StudentUSI, Is.EqualTo(2));
            Assert.That(studentResults.Skip(2).First().StudentUSI, Is.EqualTo(3));
        }
    }
}