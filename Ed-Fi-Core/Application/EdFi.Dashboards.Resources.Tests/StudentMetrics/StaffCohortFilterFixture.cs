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
    public class When_Getting_StudentList_With_CohortFilter_With_Nothing_Set : TestFixtureBase
    {
        private StaffCohortFilter filter;

        private IQueryable<EnhancedStudentInformation> query;
        private IQueryable<EnhancedStudentInformation> studentResults;

        private IRepository<StaffStudentCohort> staffStudentCohortRepository;
        private IRepository<StaffCohort> staffCohortRepository;
        private IRepository<StaffStudentAssociation> staffStudentAssociationRepository;

        protected override void EstablishContext()
        {
            staffStudentCohortRepository = mocks.StrictMock<IRepository<StaffStudentCohort>>();
            Expect.Call(staffStudentCohortRepository.GetAll()).Return(new List<StaffStudentCohort>
            {
                new StaffStudentCohort { StaffCohortId = 1, StudentUSI = 1 },
                new StaffStudentCohort { StaffCohortId = 2, StudentUSI = 2 },
                new StaffStudentCohort { StaffCohortId = 2, StudentUSI = 3 },
            }.AsQueryable()).Repeat.Any();

            staffCohortRepository = mocks.StrictMock<IRepository<StaffCohort>>();
            Expect.Call(staffCohortRepository.GetAll()).Return(new List<StaffCohort>
            {
                new StaffCohort { StaffCohortId = 1, },
                new StaffCohort { StaffCohortId = 2, },
            }.AsQueryable()).Repeat.Any();

            staffStudentAssociationRepository = mocks.StrictMock<IRepository<StaffStudentAssociation>>();
            Expect.Call(staffStudentAssociationRepository.GetAll()).Return(new List<StaffStudentAssociation>().AsQueryable()).Repeat.Any();

            query = new List<EnhancedStudentInformation>
            {
                new EnhancedStudentInformation { StudentUSI = 1, },
                new EnhancedStudentInformation { StudentUSI = 2, },
                new EnhancedStudentInformation { StudentUSI = 3, },
            }.AsQueryable();

            filter = new StaffCohortFilter(staffStudentCohortRepository);

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

    public class When_Getting_StudentList_With_CohortFilter : TestFixtureBase
    {
        private StaffCohortFilter filter;

        private IQueryable<EnhancedStudentInformation> query;
        private IQueryable<EnhancedStudentInformation> studentResults;

        private IRepository<StaffStudentCohort> staffStudentCohortRepository;
        private IRepository<StaffCohort> staffCohortRepository;
        private IRepository<StaffStudentAssociation> staffStudentAssociationRepository;

        protected override void EstablishContext()
        {
            staffStudentCohortRepository = mocks.StrictMock<IRepository<StaffStudentCohort>>();
            Expect.Call(staffStudentCohortRepository.GetAll()).Return(new List<StaffStudentCohort>
            {
                new StaffStudentCohort { StaffCohortId = 1, StudentUSI = 1 },
                new StaffStudentCohort { StaffCohortId = 2, StudentUSI = 2 },
                new StaffStudentCohort { StaffCohortId = 2, StudentUSI = 3 },
            }.AsQueryable()).Repeat.Any();

            staffCohortRepository = mocks.StrictMock<IRepository<StaffCohort>>();
            Expect.Call(staffCohortRepository.GetAll()).Return(new List<StaffCohort>
            {
                new StaffCohort { StaffCohortId = 1, },
                new StaffCohort { StaffCohortId = 2, },
            }.AsQueryable()).Repeat.Any();

            staffStudentAssociationRepository = mocks.StrictMock<IRepository<StaffStudentAssociation>>();
            Expect.Call(staffStudentAssociationRepository.GetAll()).Return(new List<StaffStudentAssociation>().AsQueryable()).Repeat.Any();

            query = new List<EnhancedStudentInformation>
            {
                new EnhancedStudentInformation { StudentUSI = 1, },
                new EnhancedStudentInformation { StudentUSI = 2, },
                new EnhancedStudentInformation { StudentUSI = 3, },
            }.AsQueryable();

            filter = new StaffCohortFilter(staffStudentCohortRepository);

            base.EstablishContext();
        }

        protected override void ExecuteTest()
        {
            studentResults = filter.ApplyFilter(query, new StudentMetricsProviderQueryOptions
            {
                StaffCohortIds = new long[] { 2 },
            });
        }

        [Test]
        public void Should_return_results()
        {
            Assert.That(studentResults.Count(), Is.EqualTo(2));
            Assert.That(studentResults.First().StudentUSI, Is.EqualTo(2));
        }
    }
}