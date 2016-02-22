using System.Collections.Generic;
using System.Linq;
using EdFi.Dashboards.Data.Entities;
using EdFi.Dashboards.Resources.StudentMetrics;
using EdFi.Dashboards.Testing;
using NUnit.Framework;

namespace EdFi.Dashboards.Resources.Tests.StudentMetrics
{
    public class When_Getting_StudentList_With_GradeLevelFilter_With_Nothing_Set : TestFixtureBase
    {
        private GradeLevelFilter filter;
        private IQueryable<EnhancedStudentInformation> query;
        private IQueryable<EnhancedStudentInformation> studentResults;

        private string SuppliedGradeLevel1 = "grade 1";
        private string SuppliedGradeLevel2 = "grade 2";

        protected override void EstablishContext()
        {
            query = new List<EnhancedStudentInformation>
            {
                new EnhancedStudentInformation { StudentUSI = 1, GradeLevel = SuppliedGradeLevel1 },
                new EnhancedStudentInformation { StudentUSI = 2, GradeLevel = SuppliedGradeLevel2 },
            }.AsQueryable();

            filter = new GradeLevelFilter();

            base.EstablishContext();
        }

        protected override void ExecuteTest()
        {
            studentResults = filter.ApplyFilter(query, new StudentMetricsProviderQueryOptions
            {
                GradeLevel = null
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

    public class When_Getting_StudentList_With_GradeLevelFilter : TestFixtureBase
    {
        private GradeLevelFilter filter;
        private IQueryable<EnhancedStudentInformation> query;
        private IQueryable<EnhancedStudentInformation> studentResults;

        private string SuppliedGradeLevel1 = "grade 1";
        private string SuppliedGradeLevel2 = "grade 2";

        protected override void EstablishContext()
        {
            query = new List<EnhancedStudentInformation>
            {
                new EnhancedStudentInformation { StudentUSI = 1, GradeLevel = SuppliedGradeLevel1 },
                new EnhancedStudentInformation { StudentUSI = 2, GradeLevel = SuppliedGradeLevel2 },
            }.AsQueryable();

            filter = new GradeLevelFilter();

            base.EstablishContext();
        }

        protected override void ExecuteTest()
        {
            studentResults = filter.ApplyFilter(query, new StudentMetricsProviderQueryOptions
            {
                GradeLevel = new[] { SuppliedGradeLevel2 }
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