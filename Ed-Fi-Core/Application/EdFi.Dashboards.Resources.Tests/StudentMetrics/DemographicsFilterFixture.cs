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
    public class When_Getting_StudentList_With_DemographicsFilter_Without_Options_Set : TestFixtureBase
    {
        private IRepository<StudentIndicator> studentIndicatorRepository;
        private DemographicsFilter filter;
        private IQueryable<EnhancedStudentInformation> query;
        private IQueryable<EnhancedStudentInformation> studentResults;

        protected override void EstablishContext()
        {
            studentIndicatorRepository = mocks.StrictMock<IRepository<StudentIndicator>>();
            Expect.Call(studentIndicatorRepository.GetAll()).Return(new List<StudentIndicator>().AsQueryable()).Repeat.Any();

            query = new List<EnhancedStudentInformation>
            {
                new EnhancedStudentInformation { StudentUSI = 1, Gender = "male"},
                new EnhancedStudentInformation { StudentUSI = 2, Gender = "female"}
            }.AsQueryable();

            filter = new DemographicsFilter(studentIndicatorRepository);
         
            base.EstablishContext();
        }

        protected override void ExecuteTest()
        {
            studentResults = filter.ApplyFilter(query, new StudentMetricsProviderQueryOptions
            {
                DemographicOptionGroups = new SelectionOptionGroup[0]
            });
        }

        [Test]
        public void Should_return_correct_number_of_results()
        {
            Assert.That(studentResults.Count(), Is.EqualTo(2));
            Assert.That(studentResults.First().StudentUSI, Is.EqualTo(1));
            Assert.That(studentResults.Skip(1).First().StudentUSI, Is.EqualTo(2));
        }
    }

    public class When_Getting_StudentList_With_DemographicsFilter_Gender : TestFixtureBase
    {
        private IRepository<StudentIndicator> studentIndicatorRepository;
        private DemographicsFilter filter;
        private IQueryable<EnhancedStudentInformation> query;
        private IQueryable<EnhancedStudentInformation> studentResults;

        protected override void EstablishContext()
        {
            studentIndicatorRepository = mocks.StrictMock<IRepository<StudentIndicator>>();
            Expect.Call(studentIndicatorRepository.GetAll()).Return(new List<StudentIndicator>().AsQueryable()).Repeat.Any();

            query = new List<EnhancedStudentInformation>
            {
                new EnhancedStudentInformation { StudentUSI = 1, Gender = "male"},
                new EnhancedStudentInformation { StudentUSI = 2, Gender = "female"}
            }.AsQueryable();

            filter = new DemographicsFilter(studentIndicatorRepository);

            base.EstablishContext();
        }

        protected override void ExecuteTest()
        {
            studentResults = filter.ApplyFilter(query, new StudentMetricsProviderQueryOptions
            {
                DemographicOptionGroups = new[]
                {
                    new SelectionOptionGroup { SelectionOptionName = "demographics", SelectedOptions = new[] { "male" } }
                }
            });
        }

        [Test]
        public void Should_return_correct_number_of_results()
        {
            Assert.That(studentResults.Count(), Is.EqualTo(1));
            Assert.That(studentResults.Single().StudentUSI, Is.EqualTo(1));
        }
    }

    public class When_Getting_StudentList_With_DemographicsFilter_Latino : TestFixtureBase
    {
        private IRepository<StudentIndicator> studentIndicatorRepository;
        private DemographicsFilter filter;
        private IQueryable<EnhancedStudentInformation> query;
        private IQueryable<EnhancedStudentInformation> studentResults;

        protected override void EstablishContext()
        {
            studentIndicatorRepository = mocks.StrictMock<IRepository<StudentIndicator>>();
            Expect.Call(studentIndicatorRepository.GetAll()).Return(new List<StudentIndicator>().AsQueryable()).Repeat.Any();

            query = new List<EnhancedStudentInformation>
            {
                new EnhancedStudentInformation { StudentUSI = 1, HispanicLatinoEthnicity = "YES"},
                new EnhancedStudentInformation { StudentUSI = 2, HispanicLatinoEthnicity = "NO"}
            }.AsQueryable();

            filter = new DemographicsFilter(studentIndicatorRepository);

            base.EstablishContext();
        }

        protected override void ExecuteTest()
        {
            studentResults = filter.ApplyFilter(query, new StudentMetricsProviderQueryOptions
            {
                DemographicOptionGroups = new[]
                {
                    new SelectionOptionGroup { SelectionOptionName = "demographics", SelectedOptions = new[] { "hispanic/latino" } }
                }
            });
        }

        [Test]
        public void Should_return_correct_number_of_results()
        {
            Assert.That(studentResults.Count(), Is.EqualTo(1));
            Assert.That(studentResults.Single().StudentUSI, Is.EqualTo(1));
        }
    }

    public class When_Getting_StudentList_With_DemographicsFilter_LateEnrollment : TestFixtureBase
    {
        private IRepository<StudentIndicator> studentIndicatorRepository;
        private DemographicsFilter filter;
        private IQueryable<EnhancedStudentInformation> query;
        private IQueryable<EnhancedStudentInformation> studentResults;

        protected override void EstablishContext()
        {
            studentIndicatorRepository = mocks.StrictMock<IRepository<StudentIndicator>>();
            Expect.Call(studentIndicatorRepository.GetAll()).Return(new List<StudentIndicator>().AsQueryable()).Repeat.Any();

            query = new List<EnhancedStudentInformation>
            {
                new EnhancedStudentInformation { StudentUSI = 1, LateEnrollment = "YES"},
                new EnhancedStudentInformation { StudentUSI = 2, LateEnrollment = "NO"}
            }.AsQueryable();

            filter = new DemographicsFilter(studentIndicatorRepository);

            base.EstablishContext();
        }

        protected override void ExecuteTest()
        {
            studentResults = filter.ApplyFilter(query, new StudentMetricsProviderQueryOptions
            {
                DemographicOptionGroups = new[]
                {
                    new SelectionOptionGroup { SelectionOptionName = "demographics", SelectedOptions = new[] { "late enrollment" } }
                }
            });
        }

        [Test]
        public void Should_return_correct_number_of_results()
        {
            Assert.That(studentResults.Count(), Is.EqualTo(1));
            Assert.That(studentResults.Single().StudentUSI, Is.EqualTo(1));
        }
    }

    public class When_Getting_StudentList_With_DemographicsFilter_TwoOrMore : TestFixtureBase
    {
        private IRepository<StudentIndicator> studentIndicatorRepository;
        private DemographicsFilter filter;
        private IQueryable<EnhancedStudentInformation> query;
        private IQueryable<EnhancedStudentInformation> studentResults;

        protected override void EstablishContext()
        {
            studentIndicatorRepository = mocks.StrictMock<IRepository<StudentIndicator>>();
            Expect.Call(studentIndicatorRepository.GetAll()).Return(new List<StudentIndicator>().AsQueryable()).Repeat.Any();

            query = new List<EnhancedStudentInformation>
            {
                new EnhancedStudentInformation { StudentUSI = 1, Race = "Asian, White"},
                new EnhancedStudentInformation { StudentUSI = 2, Race = "White"},
                new EnhancedStudentInformation { StudentUSI = 3, Race = "Asian, White", HispanicLatinoEthnicity = "YES"}
            }.AsQueryable();

            filter = new DemographicsFilter(studentIndicatorRepository);

            base.EstablishContext();
        }

        protected override void ExecuteTest()
        {
            studentResults = filter.ApplyFilter(query, new StudentMetricsProviderQueryOptions
            {
                DemographicOptionGroups = new[]
                {
                    new SelectionOptionGroup { SelectionOptionName = "demographics", SelectedOptions = new[] { "two or more" } }
                }
            });
        }

        [Test]
        public void Should_return_correct_number_of_results()
        {
            Assert.That(studentResults.Count(), Is.EqualTo(1));
            Assert.That(studentResults.Single().StudentUSI, Is.EqualTo(1));
        }
    }

    public class When_Getting_StudentList_With_DemographicsFilter_Race : TestFixtureBase
    {
        private IRepository<StudentIndicator> studentIndicatorRepository;
        private DemographicsFilter filter;
        private IQueryable<EnhancedStudentInformation> query;
        private IQueryable<EnhancedStudentInformation> studentResults;

        protected override void EstablishContext()
        {
            studentIndicatorRepository = mocks.StrictMock<IRepository<StudentIndicator>>();
            Expect.Call(studentIndicatorRepository.GetAll()).Return(new List<StudentIndicator>().AsQueryable()).Repeat.Any();

            query = new List<EnhancedStudentInformation>
            {
                new EnhancedStudentInformation { StudentUSI = 1, Race = "Asian"},
                new EnhancedStudentInformation { StudentUSI = 2, Race = "White"},
            }.AsQueryable();

            filter = new DemographicsFilter(studentIndicatorRepository);

            base.EstablishContext();
        }

        protected override void ExecuteTest()
        {
            studentResults = filter.ApplyFilter(query, new StudentMetricsProviderQueryOptions
            {
                DemographicOptionGroups = new[]
                {
                    new SelectionOptionGroup { SelectionOptionName = "demographics", SelectedOptions = new[] { "Asian" } }
                }
            });
        }

        [Test]
        public void Should_return_correct_number_of_results()
        {
            Assert.That(studentResults.Count(), Is.EqualTo(1));
            Assert.That(studentResults.Single().StudentUSI, Is.EqualTo(1));
        }
    }

    public class When_Getting_StudentList_With_DemographicsFilter_Null : TestFixtureBase
    {
        private IRepository<StudentIndicator> studentIndicatorRepository;
        private DemographicsFilter filter;
        private IQueryable<EnhancedStudentInformation> query;
        private IQueryable<EnhancedStudentInformation> studentResults;

        protected override void EstablishContext()
        {
            studentIndicatorRepository = mocks.StrictMock<IRepository<StudentIndicator>>();
            Expect.Call(studentIndicatorRepository.GetAll()).Return(new List<StudentIndicator>().AsQueryable()).Repeat.Any();

            query = new List<EnhancedStudentInformation>
            {
                new EnhancedStudentInformation { StudentUSI = 1 },
            }.AsQueryable();

            filter = new DemographicsFilter(studentIndicatorRepository);

            base.EstablishContext();
        }

        protected override void ExecuteTest()
        {
            studentResults = filter.ApplyFilter(query, new StudentMetricsProviderQueryOptions
            {
                DemographicOptionGroups = new[]
                {
                    new SelectionOptionGroup { SelectionOptionName = "demographics", SelectedOptions = new string[] {} }
                }
            });
        }
    }
}