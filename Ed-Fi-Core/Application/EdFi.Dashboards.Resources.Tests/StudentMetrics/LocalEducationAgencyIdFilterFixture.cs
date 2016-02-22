using System.Collections.Generic;
using System.Linq;
using EdFi.Dashboards.Data.Entities;
using EdFi.Dashboards.Resources.StudentMetrics;
using EdFi.Dashboards.Testing;
using NUnit.Framework;

namespace EdFi.Dashboards.Resources.Tests.StudentMetrics
{
    public class When_Getting_StudentList_With_LocalEducationAgencyIdFilter_With_Nothing_Set : TestFixtureBase
    {
        private LocalEducationAgencyIdFilter filter;
        private IQueryable<EnhancedStudentInformation> query;
        private IQueryable<EnhancedStudentInformation> studentResults;

        private int SuppliedLocalEducationAgency1 = 1;
        private int SuppliedLocalEducationAgency2 = 2;

        protected override void EstablishContext()
        {
            query = new List<EnhancedStudentInformation>
            {
                new EnhancedStudentInformation { StudentUSI = 1, LocalEducationAgencyId = SuppliedLocalEducationAgency1},
                new EnhancedStudentInformation { StudentUSI = 2, LocalEducationAgencyId = SuppliedLocalEducationAgency2},
            }.AsQueryable();

            filter = new LocalEducationAgencyIdFilter();

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

    public class When_Getting_StudentList_With_LocalEducationAgencyIdFilter : TestFixtureBase
    {
        private LocalEducationAgencyIdFilter filter;
        private IQueryable<EnhancedStudentInformation> query;
        private IQueryable<EnhancedStudentInformation> studentResults;

        private int SuppliedLocalEducationAgency1 = 1;
        private int SuppliedLocalEducationAgency2 = 2;

        protected override void EstablishContext()
        {
            query = new List<EnhancedStudentInformation>
            {
                new EnhancedStudentInformation { StudentUSI = 1, LocalEducationAgencyId = SuppliedLocalEducationAgency1},
                new EnhancedStudentInformation { StudentUSI = 2, LocalEducationAgencyId = SuppliedLocalEducationAgency2},
            }.AsQueryable();

            filter = new LocalEducationAgencyIdFilter();

            base.EstablishContext();
        }

        protected override void ExecuteTest()
        {
            studentResults = filter.ApplyFilter(query, new StudentMetricsProviderQueryOptions
            {
                LocalEducationAgencyId = SuppliedLocalEducationAgency2
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