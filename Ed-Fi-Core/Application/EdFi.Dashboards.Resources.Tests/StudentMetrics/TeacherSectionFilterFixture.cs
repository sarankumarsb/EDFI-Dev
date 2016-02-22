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
    public class When_Getting_StudentList_With_TeacherSectionFilter_With_Nothing_Set : TestFixtureBase
    {
        private TeacherSectionFilter filter;
        private IRepository<TeacherStudentSection> teacherStudentSectionRepository;
        private IQueryable<EnhancedStudentInformation> query;
        private IQueryable<EnhancedStudentInformation> studentResults;

        protected override void EstablishContext()
        {
            teacherStudentSectionRepository = mocks.StrictMock<IRepository<TeacherStudentSection>>();
            Expect.Call(teacherStudentSectionRepository.GetAll()).Return(new List<TeacherStudentSection> { new TeacherStudentSection { StudentUSI = 2, TeacherSectionId = 1 } }.AsQueryable()).Repeat.Any();

            query = new List<EnhancedStudentInformation>
            {
                new EnhancedStudentInformation { StudentUSI = 1, },
                new EnhancedStudentInformation { StudentUSI = 2, },
            }.AsQueryable();

            filter = new TeacherSectionFilter(teacherStudentSectionRepository);

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

    public class When_Getting_StudentList_With_TeacherSectionFilter : TestFixtureBase
    {
        private TeacherSectionFilter filter;
        private IRepository<TeacherStudentSection> teacherStudentSectionRepository;
        private IQueryable<EnhancedStudentInformation> query;
        private IQueryable<EnhancedStudentInformation> studentResults;

        protected override void EstablishContext()
        {
            teacherStudentSectionRepository = mocks.StrictMock<IRepository<TeacherStudentSection>>();
            Expect.Call(teacherStudentSectionRepository.GetAll()).Return(new List<TeacherStudentSection> { new TeacherStudentSection { StudentUSI = 2, TeacherSectionId = 1 } }.AsQueryable()).Repeat.Any();

            query = new List<EnhancedStudentInformation>
            {
                new EnhancedStudentInformation { StudentUSI = 1, },
                new EnhancedStudentInformation { StudentUSI = 2, },
            }.AsQueryable();

            filter = new TeacherSectionFilter(teacherStudentSectionRepository);

            base.EstablishContext();
        }

        protected override void ExecuteTest()
        {
            studentResults = filter.ApplyFilter(query, new StudentMetricsProviderQueryOptions
            {
                TeacherSectionIds = new long[] { 1 }
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