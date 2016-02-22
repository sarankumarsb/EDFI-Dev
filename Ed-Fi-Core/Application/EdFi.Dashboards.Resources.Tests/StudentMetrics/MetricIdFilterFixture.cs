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
    public class When_Getting_StudentList_With_SchoolMetricStudentListMetricIdFilter_With_Nothing_Set : TestFixtureBase
    {
        private MetricIdFilter filter;
        private IQueryable<EnhancedStudentInformation> query;
        private IQueryable<EnhancedStudentInformation> studentResults;

        private IRepository<SchoolMetricStudentList> schoolMetricStudentListRepository;

        protected override void EstablishContext()
        {
            schoolMetricStudentListRepository = mocks.StrictMock<IRepository<SchoolMetricStudentList>>();
            Expect.Call(schoolMetricStudentListRepository.GetAll()).Return(new List<SchoolMetricStudentList>
            {
                new SchoolMetricStudentList { StudentUSI = 1, SchoolId = 1, MetricId = 1 },
                new SchoolMetricStudentList { StudentUSI = 2, SchoolId = 1, MetricId = 1 },
                new SchoolMetricStudentList { StudentUSI = 3, SchoolId = 1, MetricId = 2 },
            }.AsQueryable()).Repeat.Any();

            query = new List<EnhancedStudentInformation>
            {
                new EnhancedStudentInformation { StudentUSI = 1, SchoolId = 1 },
                new EnhancedStudentInformation { StudentUSI = 2, SchoolId = 1 },
                new EnhancedStudentInformation { StudentUSI = 3, SchoolId = 1 },
            }.AsQueryable();

            filter = new MetricIdFilter(schoolMetricStudentListRepository);

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

    public class When_Getting_StudentList_With_SchoolMetricStudentListMetricIdFilter : TestFixtureBase
    {
        private MetricIdFilter filter;
        private IQueryable<EnhancedStudentInformation> query;
        private IQueryable<EnhancedStudentInformation> studentResults;

        private IRepository<SchoolMetricStudentList> schoolMetricStudentListRepository;

        protected override void EstablishContext()
        {
            schoolMetricStudentListRepository = mocks.StrictMock<IRepository<SchoolMetricStudentList>>();
            Expect.Call(schoolMetricStudentListRepository.GetAll()).Return(new List<SchoolMetricStudentList>
            {
                new SchoolMetricStudentList { StudentUSI = 1, SchoolId = 1, MetricId = 1 },
                new SchoolMetricStudentList { StudentUSI = 2, SchoolId = 1, MetricId = 1 },
                new SchoolMetricStudentList { StudentUSI = 3, SchoolId = 1, MetricId = 2 },
            }.AsQueryable()).Repeat.Any();

            query = new List<EnhancedStudentInformation>
            {
                new EnhancedStudentInformation { StudentUSI = 1, SchoolId = 1 },
                new EnhancedStudentInformation { StudentUSI = 2, SchoolId = 1 },
                new EnhancedStudentInformation { StudentUSI = 3, SchoolId = 1 },
            }.AsQueryable();

            filter = new MetricIdFilter(schoolMetricStudentListRepository);

            base.EstablishContext();
        }

        protected override void ExecuteTest()
        {
            studentResults = filter.ApplyFilter(query, new StudentMetricsProviderQueryOptions
            {
                SchoolMetricStudentListMetricId = 1
            });
        }

        [Test]
        public void Should_return_results()
        {
            Assert.That(studentResults.Count(), Is.EqualTo(2));
        }
    }
}