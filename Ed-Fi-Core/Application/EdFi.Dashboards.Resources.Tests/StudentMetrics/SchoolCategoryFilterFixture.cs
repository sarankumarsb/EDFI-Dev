using System.Collections.Generic;
using System.Linq;
using EdFi.Dashboards.Data.Entities;
using EdFi.Dashboards.Resources.StudentMetrics;
using EdFi.Dashboards.Testing;
using NUnit.Framework;

namespace EdFi.Dashboards.Resources.Tests.StudentMetrics
{
    public class When_Getting_StudentList_With_SchoolCategoryFilter_With_Nothing_Set : TestFixtureBase
    {
        private SchoolCategoryFilter filter;
        private IQueryable<EnhancedStudentInformation> query;
        private IQueryable<EnhancedStudentInformation> studentResults;

        private string SuppliedSchoolCategory1 = "Category 1";
        private string SuppliedSchoolCategory2 = "Category 2";

        protected override void EstablishContext()
        {
            query = new List<EnhancedStudentInformation>
            {
                new EnhancedStudentInformation { StudentUSI = 1, SchoolId = 1, SchoolCategory = SuppliedSchoolCategory1 },
                new EnhancedStudentInformation { StudentUSI = 2, SchoolId = 1, SchoolCategory = SuppliedSchoolCategory2 },
                new EnhancedStudentInformation { StudentUSI = 3, SchoolId = 1, SchoolCategory = SuppliedSchoolCategory1 },
            }.AsQueryable();

            filter = new SchoolCategoryFilter();

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

    public class When_Getting_StudentList_With_SchoolCategoryFilter : TestFixtureBase
    {
        private SchoolCategoryFilter filter;
        private IQueryable<EnhancedStudentInformation> query;
        private IQueryable<EnhancedStudentInformation> studentResults;

        private string SuppliedSchoolCategory1 = "Category 1";
        private string SuppliedSchoolCategory2 = "Category 2";

        protected override void EstablishContext()
        {
            query = new List<EnhancedStudentInformation>
            {
                new EnhancedStudentInformation { StudentUSI = 1, SchoolId = 1, SchoolCategory = SuppliedSchoolCategory1 },
                new EnhancedStudentInformation { StudentUSI = 2, SchoolId = 1, SchoolCategory = SuppliedSchoolCategory2 },
                new EnhancedStudentInformation { StudentUSI = 3, SchoolId = 1, SchoolCategory = SuppliedSchoolCategory1 },
            }.AsQueryable();

            filter = new SchoolCategoryFilter();

            base.EstablishContext();
        }

        protected override void ExecuteTest()
        {
            studentResults = filter.ApplyFilter(query, new StudentMetricsProviderQueryOptions
            {
                SchoolCategory = SuppliedSchoolCategory2
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