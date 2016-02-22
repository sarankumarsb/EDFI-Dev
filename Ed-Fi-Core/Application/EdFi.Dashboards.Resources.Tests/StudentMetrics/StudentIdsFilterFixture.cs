using System.Collections.Generic;
using System.Linq;
using EdFi.Dashboards.Data.Entities;
using EdFi.Dashboards.Resources.StudentMetrics;
using EdFi.Dashboards.Testing;
using NUnit.Framework;

namespace EdFi.Dashboards.Resources.Tests.StudentMetrics
{
    public class When_Getting_StudentList_With_StudentIdsFilter_With_Nothing_Set : TestFixtureBase
    {
        private StudentIdsFilter filter;
        private IQueryable<EnhancedStudentInformation> query;
        private IQueryable<EnhancedStudentInformation> studentResults;

        private long SuppliedStudentId1 = 1;
        private long SuppliedStudentId2 = 2;

        protected override void EstablishContext()
        {
            query = new List<EnhancedStudentInformation>
            {
                new EnhancedStudentInformation { StudentUSI = SuppliedStudentId1 },
                new EnhancedStudentInformation { StudentUSI = SuppliedStudentId2 },
            }.AsQueryable();

            filter = new StudentIdsFilter();

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
            Assert.That(studentResults.Count(), Is.EqualTo(2));
            Assert.That(studentResults.First().StudentUSI, Is.EqualTo(SuppliedStudentId1));
            Assert.That(studentResults.Skip(1).First().StudentUSI, Is.EqualTo(SuppliedStudentId2));
        }
    }

    public class When_Getting_StudentList_With_StudentIdsFilter : TestFixtureBase
    {
        private StudentIdsFilter filter;
        private IQueryable<EnhancedStudentInformation> query;
        private IQueryable<EnhancedStudentInformation> studentResults;

        private long SuppliedStudentId1 = 1;
        private long SuppliedStudentId2 = 2;

        protected override void EstablishContext()
        {
            query = new List<EnhancedStudentInformation>
            {
                new EnhancedStudentInformation { StudentUSI = SuppliedStudentId1 },
                new EnhancedStudentInformation { StudentUSI = SuppliedStudentId2 },
            }.AsQueryable();

            filter = new StudentIdsFilter();

            base.EstablishContext();
        }

        protected override void ExecuteTest()
        {
            studentResults = filter.ApplyFilter(query, new StudentMetricsProviderQueryOptions
            {
                StudentIds = new[] { SuppliedStudentId2 }
            });
        }

        [Test]
        public void Should_return_results()
        {
            Assert.That(studentResults.Count(), Is.EqualTo(1));
            Assert.That(studentResults.First().StudentUSI, Is.EqualTo(SuppliedStudentId2));
        }
    }
}