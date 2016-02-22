using System.Collections.Generic;
using System.Linq;
using EdFi.Dashboards.Data.Entities;
using EdFi.Dashboards.Resources.StudentMetrics;
using EdFi.Dashboards.Testing;
using NUnit.Framework;

namespace EdFi.Dashboards.Resources.Tests.StudentMetrics
{
    public class When_Getting_StudentList_With_SchoolIdFilter_With_Nothing_Set : TestFixtureBase
    {
        private SchoolIdFilter filter;
        private IQueryable<EnhancedStudentInformation> query;
        private IQueryable<EnhancedStudentInformation> studentResults;

        private int SuppliedSchoolId1 = 1;
        private int SuppliedSchoolId2 = 2;

        protected override void EstablishContext()
        {
            query = new List<EnhancedStudentInformation>
            {
                new EnhancedStudentInformation { StudentUSI = 1, SchoolId = SuppliedSchoolId1},
                new EnhancedStudentInformation { StudentUSI = 2, SchoolId = SuppliedSchoolId2},
            }.AsQueryable();

            filter = new SchoolIdFilter();

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
            Assert.That(studentResults.First().StudentUSI, Is.EqualTo(1));
            Assert.That(studentResults.Skip(1).First().StudentUSI, Is.EqualTo(2));
        }
    }

    public class When_Getting_StudentList_With_SchoolIdFilter : TestFixtureBase
    {
        private SchoolIdFilter filter;
        private IQueryable<EnhancedStudentInformation> query;
        private IQueryable<EnhancedStudentInformation> studentResults;

        private int SuppliedSchoolId1 = 1;
        private int SuppliedSchoolId2 = 2;

        protected override void EstablishContext()
        {
            query = new List<EnhancedStudentInformation>
            {
                new EnhancedStudentInformation { StudentUSI = 1, SchoolId = SuppliedSchoolId1},
                new EnhancedStudentInformation { StudentUSI = 2, SchoolId = SuppliedSchoolId2},
            }.AsQueryable();

            filter = new SchoolIdFilter();

            base.EstablishContext();
        }

        protected override void ExecuteTest()
        {
            studentResults = filter.ApplyFilter(query, new StudentMetricsProviderQueryOptions
            {
                SchoolId = SuppliedSchoolId2
            });
        }

        [Test]
        public void Should_return_results()
        {
            Assert.That(studentResults.Count(), Is.EqualTo(1));
            Assert.That(studentResults.First().StudentUSI, Is.EqualTo(2));
        }
    }
}