using System;
using System.Collections.Generic;
using System.Linq;
using EdFi.Dashboards.Data.Entities;
using EdFi.Dashboards.Data.Repository;
using EdFi.Dashboards.Metric.Data.Entities;
using EdFi.Dashboards.Resources.StudentMetrics;
using EdFi.Dashboards.Testing;
using NUnit.Framework;
using Rhino.Mocks;

namespace EdFi.Dashboards.Resources.Tests.StudentMetrics
{
    public class When_Getting_StudentList_With_MetricOptionsFilter_Null : TestFixtureBase
    {
        private IQueryable<EnhancedStudentInformation> query;
        private MetricOptionsFilter filter;
        private IQueryable<EnhancedStudentInformation> studentResults;

        private IRepository<StudentMetric> studentMetricRepository;
        private IRepository<MetricInstanceExtendedPropertyWithValueToFloat> metricInstanceExtendedPropertyWithValueToFloatRepository;

        protected override void EstablishContext()
        {
            studentMetricRepository = mocks.StrictMock<IRepository<StudentMetric>>();
            Expect.Call(studentMetricRepository.GetAll()).Return(new List<StudentMetric>().AsQueryable()).Repeat.Any();

            metricInstanceExtendedPropertyWithValueToFloatRepository = mocks.StrictMock<IRepository<MetricInstanceExtendedPropertyWithValueToFloat>>();
            Expect.Call(metricInstanceExtendedPropertyWithValueToFloatRepository.GetAll()).Return(new List<MetricInstanceExtendedPropertyWithValueToFloat>().AsQueryable()).Repeat.Any();

            query = new List<EnhancedStudentInformation>
            {
                new EnhancedStudentInformation { StudentUSI = 1, SchoolId = 1},
                new EnhancedStudentInformation { StudentUSI = 2, SchoolId = 1 },
            }.AsQueryable();

            filter = new MetricOptionsFilter(studentMetricRepository, metricInstanceExtendedPropertyWithValueToFloatRepository);

            base.EstablishContext();
        }

        protected override void ExecuteTest()
        {
            var queryOptions = new StudentMetricsProviderQueryOptions {MetricOptionGroups = null};

            studentResults = filter.ApplyFilter(query, queryOptions);
        }

        [Test]
        public void Should_return_correct_number_of_results()
        {
            Assert.That(studentResults.Count(), Is.EqualTo(2));
        }
    }

    public class When_Getting_StudentList_With_MetricOptionsFilter_Empty : TestFixtureBase
    {
        private IQueryable<EnhancedStudentInformation> query;
        private MetricOptionsFilter filter;
        private IQueryable<EnhancedStudentInformation> studentResults;

        private IRepository<StudentMetric> studentMetricRepository;
        private IRepository<MetricInstanceExtendedPropertyWithValueToFloat> metricInstanceExtendedPropertyWithValueToFloatRepository;

        protected override void EstablishContext()
        {
            studentMetricRepository = mocks.StrictMock<IRepository<StudentMetric>>();
            Expect.Call(studentMetricRepository.GetAll()).Return(new List<StudentMetric>().AsQueryable()).Repeat.Any();

            metricInstanceExtendedPropertyWithValueToFloatRepository = mocks.StrictMock<IRepository<MetricInstanceExtendedPropertyWithValueToFloat>>();
            Expect.Call(metricInstanceExtendedPropertyWithValueToFloatRepository.GetAll()).Return(new List<MetricInstanceExtendedPropertyWithValueToFloat>().AsQueryable()).Repeat.Any();

            query = new List<EnhancedStudentInformation>
            {
                new EnhancedStudentInformation { StudentUSI = 1, SchoolId = 1},
                new EnhancedStudentInformation { StudentUSI = 2, SchoolId = 1 },
            }.AsQueryable();

            filter = new MetricOptionsFilter(studentMetricRepository, metricInstanceExtendedPropertyWithValueToFloatRepository);

            base.EstablishContext();
        }

        protected override void ExecuteTest()
        {
            var queryOptions = new StudentMetricsProviderQueryOptions
            {
                MetricOptionGroups = new[] {new MetricFilterOptionGroup()}
            };

            studentResults = filter.ApplyFilter(query, queryOptions);
        }

        [Test]
        public void Should_return_correct_number_of_results()
        {
            Assert.That(studentResults.Count(), Is.EqualTo(2));
        }
    }

    public class When_Getting_StudentList_With_MetricOptionsFilter_MetricStateEqualTo : TestFixtureBase
    {
        private IQueryable<EnhancedStudentInformation> query;
        private MetricOptionsFilter filter;
        private IQueryable<EnhancedStudentInformation> studentResults;

        private IRepository<StudentMetric> studentMetricRepository;
        private IRepository<MetricInstanceExtendedPropertyWithValueToFloat> metricInstanceExtendedPropertyWithValueToFloatRepository;

        protected override void EstablishContext()
        {
            studentMetricRepository = mocks.StrictMock<IRepository<StudentMetric>>();
            Expect.Call(studentMetricRepository.GetAll()).Return(new List<StudentMetric>
            {
                new StudentMetric { MetricId = 1, SchoolId = 1, StudentUSI = 1, MetricStateTypeId = 1 },
                new StudentMetric { MetricId = 1, SchoolId = 1, StudentUSI = 2, MetricStateTypeId = 2 },
            }.AsQueryable()).Repeat.Any();

            metricInstanceExtendedPropertyWithValueToFloatRepository = mocks.StrictMock<IRepository<MetricInstanceExtendedPropertyWithValueToFloat>>();
            Expect.Call(metricInstanceExtendedPropertyWithValueToFloatRepository.GetAll()).Return(new List<MetricInstanceExtendedPropertyWithValueToFloat>().AsQueryable()).Repeat.Any();

            query = new List<EnhancedStudentInformation>
            {
                new EnhancedStudentInformation { StudentUSI = 1, SchoolId = 1 },
                new EnhancedStudentInformation { StudentUSI = 2, SchoolId = 1 },
            }.AsQueryable();

            filter = new MetricOptionsFilter(studentMetricRepository, metricInstanceExtendedPropertyWithValueToFloatRepository);

            base.EstablishContext();
        }

        protected override void ExecuteTest()
        {
            var queryOptions = new StudentMetricsProviderQueryOptions
            {
                MetricOptionGroups = new[]
                {
                    new MetricFilterOptionGroup
                    {
                        MetricFilterOptions = new[]
                        {
                            new MetricFilterOption
                            {
                                MetricId = 1,
                                MetricStateEqualTo = 1,
                            }
                        }
                    }
                }
            };

            studentResults = filter.ApplyFilter(query, queryOptions);
        }

        [Test]
        public void Should_return_results()
        {
            Assert.That(studentResults.Count(), Is.EqualTo(1));
        }
    }

    public class When_Getting_StudentList_With_MetricOptionsFilter_MetricStateNotEqualTo : TestFixtureBase
    {
        private IQueryable<EnhancedStudentInformation> query;
        private MetricOptionsFilter filter;
        private IQueryable<EnhancedStudentInformation> studentResults;

        private IRepository<StudentMetric> studentMetricRepository;
        private IRepository<MetricInstanceExtendedPropertyWithValueToFloat> metricInstanceExtendedPropertyWithValueToFloatRepository;

        protected override void EstablishContext()
        {
            studentMetricRepository = mocks.StrictMock<IRepository<StudentMetric>>();
            Expect.Call(studentMetricRepository.GetAll()).Return(new List<StudentMetric>
            {
                new StudentMetric { MetricId = 1, SchoolId = 1, StudentUSI = 1, MetricStateTypeId = 1 },
                new StudentMetric { MetricId = 1, SchoolId = 1, StudentUSI = 2, MetricStateTypeId = 2 },
            }.AsQueryable()).Repeat.Any();

            metricInstanceExtendedPropertyWithValueToFloatRepository = mocks.StrictMock<IRepository<MetricInstanceExtendedPropertyWithValueToFloat>>();
            Expect.Call(metricInstanceExtendedPropertyWithValueToFloatRepository.GetAll()).Return(new List<MetricInstanceExtendedPropertyWithValueToFloat>().AsQueryable()).Repeat.Any();

            query = new List<EnhancedStudentInformation>
            {
                new EnhancedStudentInformation { StudentUSI = 1, SchoolId = 1 },
                new EnhancedStudentInformation { StudentUSI = 2, SchoolId = 1 },
            }.AsQueryable();

            filter = new MetricOptionsFilter(studentMetricRepository, metricInstanceExtendedPropertyWithValueToFloatRepository);

            base.EstablishContext();
        }

        protected override void ExecuteTest()
        {
            var queryOptions = new StudentMetricsProviderQueryOptions
            {
                MetricOptionGroups = new[]
                {
                    new MetricFilterOptionGroup
                    {
                        MetricFilterOptions = new[]
                        {
                            new MetricFilterOption
                            {
                                MetricId = 1,
                                MetricStateNotEqualTo = 1
                            }
                        }
                    }
                }
            };

            studentResults = filter.ApplyFilter(query, queryOptions);
        }

        [Test]
        public void Should_return_results()
        {
            Assert.That(studentResults.Count(), Is.EqualTo(1));
            Assert.That(studentResults.First().StudentUSI, Is.EqualTo(2));
        }
    }

    public class When_Getting_StudentList_With_MetricOptionsFilter_MinInclusiveMetricInstanceExtendedProperty : TestFixtureBase
    {
        private IQueryable<EnhancedStudentInformation> query;
        private MetricOptionsFilter filter;
        private IQueryable<EnhancedStudentInformation> studentResults;

        private IRepository<StudentMetric> studentMetricRepository;
        private IRepository<MetricInstanceExtendedPropertyWithValueToFloat> metricInstanceExtendedPropertyWithValueToFloatRepository;

        private Guid SuppliedMetricInstanceSetKey1 = Guid.Parse("5fd12568-19b8-4573-9376-c05b85b5a39b");
        private Guid SuppliedMetricInstanceSetKey2 = Guid.Parse("ced3313f-3d89-48f1-9ba0-67c6292bf838");
        private Guid SuppliedMetricInstanceSetKey3 = Guid.Parse("f78ba526-141a-44bd-af1e-cdcaccc58ede");

        protected override void EstablishContext()
        {
            studentMetricRepository = mocks.StrictMock<IRepository<StudentMetric>>();
            Expect.Call(studentMetricRepository.GetAll()).Return(new List<StudentMetric>
            {
                new StudentMetric { MetricId = 1, SchoolId = 1, StudentUSI = 1, MetricInstanceSetKey = SuppliedMetricInstanceSetKey1, ValueSortOrder = 1.0d },
                new StudentMetric { MetricId = 1, SchoolId = 1, StudentUSI = 2, MetricInstanceSetKey = SuppliedMetricInstanceSetKey2, ValueSortOrder = 2.0d },
                new StudentMetric { MetricId = 1, SchoolId = 1, StudentUSI = 3, MetricInstanceSetKey = SuppliedMetricInstanceSetKey3, ValueSortOrder = 3.0d },
            }.AsQueryable()).Repeat.Any();

            metricInstanceExtendedPropertyWithValueToFloatRepository = mocks.StrictMock<IRepository<MetricInstanceExtendedPropertyWithValueToFloat>>();
            Expect.Call(metricInstanceExtendedPropertyWithValueToFloatRepository.GetAll())
                .Return(new List<MetricInstanceExtendedPropertyWithValueToFloat>
                {
                    new MetricInstanceExtendedPropertyWithValueToFloat { MetricId = 1, MetricInstanceSetKey = SuppliedMetricInstanceSetKey1, Name = "Test", ValueToFloat = 2.0d },
                    new MetricInstanceExtendedPropertyWithValueToFloat { MetricId = 1, MetricInstanceSetKey = SuppliedMetricInstanceSetKey2, Name = "Test", ValueToFloat = 2.0d },
                    new MetricInstanceExtendedPropertyWithValueToFloat { MetricId = 1, MetricInstanceSetKey = SuppliedMetricInstanceSetKey3, Name = "Test", ValueToFloat = 2.0d },
                }.AsQueryable()).Repeat.Any();

            query = new List<EnhancedStudentInformation>
            {
                new EnhancedStudentInformation { StudentUSI = 1, SchoolId = 1 },
                new EnhancedStudentInformation { StudentUSI = 2, SchoolId = 1 },
                new EnhancedStudentInformation { StudentUSI = 3, SchoolId = 1 },
            }.AsQueryable();

            filter = new MetricOptionsFilter(studentMetricRepository, metricInstanceExtendedPropertyWithValueToFloatRepository);

            base.EstablishContext();
        }

        protected override void ExecuteTest()
        {
            var queryOptions = new StudentMetricsProviderQueryOptions
            {
                MetricOptionGroups = new[]
                {
                    new MetricFilterOptionGroup
                    {
                        MetricFilterOptions = new[]
                        {
                            new MetricFilterOption
                            {
                                MetricId = 1,
                                MinInclusiveMetricInstanceExtendedProperty = "Test",
                            }
                        }
                    }
                }
            };

            studentResults = filter.ApplyFilter(query, queryOptions);
        }

        [Test]
        public void Should_return_results()
        {
            Assert.That(studentResults.Count(), Is.EqualTo(2));
            Assert.That(studentResults.First().StudentUSI, Is.EqualTo(2));
            Assert.That(studentResults.Skip(1).First().StudentUSI, Is.EqualTo(3));
        }
    }
 
    public class When_Getting_StudentList_With_MetricOptionsFilter_MaxExclusiveMetricInstanceExtendedProperty : TestFixtureBase
    {
        private IQueryable<EnhancedStudentInformation> query;
        private MetricOptionsFilter filter;
        private IQueryable<EnhancedStudentInformation> studentResults;

        private IRepository<StudentMetric> studentMetricRepository;
        private IRepository<MetricInstanceExtendedPropertyWithValueToFloat> metricInstanceExtendedPropertyWithValueToFloatRepository;

        private Guid SuppliedMetricInstanceSetKey1 = Guid.Parse("5fd12568-19b8-4573-9376-c05b85b5a39b");
        private Guid SuppliedMetricInstanceSetKey2 = Guid.Parse("ced3313f-3d89-48f1-9ba0-67c6292bf838");
        private Guid SuppliedMetricInstanceSetKey3 = Guid.Parse("f78ba526-141a-44bd-af1e-cdcaccc58ede");

        protected override void EstablishContext()
        {
            studentMetricRepository = mocks.StrictMock<IRepository<StudentMetric>>();
            Expect.Call(studentMetricRepository.GetAll()).Return(new List<StudentMetric>
            {
                new StudentMetric { MetricId = 1, SchoolId = 1, StudentUSI = 1, MetricInstanceSetKey = SuppliedMetricInstanceSetKey1, ValueSortOrder = 1.0d },
                new StudentMetric { MetricId = 1, SchoolId = 1, StudentUSI = 2, MetricInstanceSetKey = SuppliedMetricInstanceSetKey2, ValueSortOrder = 2.0d },
                new StudentMetric { MetricId = 1, SchoolId = 1, StudentUSI = 3, MetricInstanceSetKey = SuppliedMetricInstanceSetKey3, ValueSortOrder = 3.0d },
            }.AsQueryable()).Repeat.Any();

            metricInstanceExtendedPropertyWithValueToFloatRepository = mocks.StrictMock<IRepository<MetricInstanceExtendedPropertyWithValueToFloat>>();
            Expect.Call(metricInstanceExtendedPropertyWithValueToFloatRepository.GetAll())
                .Return(new List<MetricInstanceExtendedPropertyWithValueToFloat>
                {
                    new MetricInstanceExtendedPropertyWithValueToFloat { MetricId = 1, MetricInstanceSetKey = SuppliedMetricInstanceSetKey1, Name = "Test", ValueToFloat = 2.0d },
                    new MetricInstanceExtendedPropertyWithValueToFloat { MetricId = 1, MetricInstanceSetKey = SuppliedMetricInstanceSetKey2, Name = "Test", ValueToFloat = 2.0d },
                    new MetricInstanceExtendedPropertyWithValueToFloat { MetricId = 1, MetricInstanceSetKey = SuppliedMetricInstanceSetKey3, Name = "Test", ValueToFloat = 2.0d },
                }.AsQueryable()).Repeat.Any();

            query = new List<EnhancedStudentInformation>
            {
                new EnhancedStudentInformation { StudentUSI = 1, SchoolId = 1 },
                new EnhancedStudentInformation { StudentUSI = 2, SchoolId = 1 },
                new EnhancedStudentInformation { StudentUSI = 3, SchoolId = 1 },
            }.AsQueryable();

            filter = new MetricOptionsFilter(studentMetricRepository, metricInstanceExtendedPropertyWithValueToFloatRepository);

            base.EstablishContext();
        }

        protected override void ExecuteTest()
        {
            var queryOptions = new StudentMetricsProviderQueryOptions
            {
                MetricOptionGroups = new[]
                {
                    new MetricFilterOptionGroup
                    {
                        MetricFilterOptions = new[]
                        {
                            new MetricFilterOption
                            {
                                MetricId = 1,
                                MaxExclusiveMetricInstanceExtendedProperty = "Test",
                            }
                        }
                    }
                }
            };

            studentResults = filter.ApplyFilter(query, queryOptions);
        }

        [Test]
        public void Should_return_results()
        {
            Assert.That(studentResults.Count(), Is.EqualTo(1));
            Assert.That(studentResults.First().StudentUSI, Is.EqualTo(1));
        }
    }

    public class When_Getting_StudentList_With_MetricOptionsFilter_ValueGreaterThanEqualTo : TestFixtureBase
    {
        private IQueryable<EnhancedStudentInformation> query;
        private MetricOptionsFilter filter;
        private IQueryable<EnhancedStudentInformation> studentResults;

        private IRepository<StudentMetric> studentMetricRepository;
        private IRepository<MetricInstanceExtendedPropertyWithValueToFloat> metricInstanceExtendedPropertyWithValueToFloatRepository;

        protected override void EstablishContext()
        {
            studentMetricRepository = mocks.StrictMock<IRepository<StudentMetric>>();
            Expect.Call(studentMetricRepository.GetAll()).Return(new List<StudentMetric> {
                new StudentMetric { StudentUSI = 1, ValueSortOrder = 2.0d, MetricId = 1, SchoolId = 1, },
                new StudentMetric { StudentUSI = 2, ValueSortOrder = 5.0d, MetricId = 1, SchoolId = 1, },
                new StudentMetric { StudentUSI = 3, ValueSortOrder = 10.0d, MetricId = 1, SchoolId = 1, }
            }.AsQueryable()).Repeat.Any();

            metricInstanceExtendedPropertyWithValueToFloatRepository = mocks.StrictMock<IRepository<MetricInstanceExtendedPropertyWithValueToFloat>>();
            Expect.Call(metricInstanceExtendedPropertyWithValueToFloatRepository.GetAll()).Return(new List<MetricInstanceExtendedPropertyWithValueToFloat>().AsQueryable()).Repeat.Any();

            query = new List<EnhancedStudentInformation>
            {
                new EnhancedStudentInformation { StudentUSI = 1, SchoolId = 1},
                new EnhancedStudentInformation { StudentUSI = 2, SchoolId = 1 },
                new EnhancedStudentInformation { StudentUSI = 3, SchoolId = 1 },
            }.AsQueryable();

            filter = new MetricOptionsFilter(studentMetricRepository, metricInstanceExtendedPropertyWithValueToFloatRepository);

            base.EstablishContext();
        }

        protected override void ExecuteTest()
        {

            var queryOptions = new StudentMetricsProviderQueryOptions
            {
                MetricOptionGroups = new[]
                {
                    new MetricFilterOptionGroup
                    {
                        MetricFilterOptions = new[]
                            {
                                new MetricFilterOption
                                {
                                    MetricId = 1,
                                    ValueGreaterThanEqualTo = 5.0d,
                                },
                            }
                    }
                }
            };

            studentResults = filter.ApplyFilter(query, queryOptions);
        }

        [Test]
        public void Should_return_correct_number_of_results()
        {
            Assert.That(studentResults.Count(), Is.EqualTo(2));
            Assert.That(studentResults.First().StudentUSI, Is.EqualTo(2));
            Assert.That(studentResults.Skip(1).First().StudentUSI, Is.EqualTo(3));
        }
    }

    public class When_Getting_StudentList_With_MetricOptionsFilter_ValueGreaterThan : TestFixtureBase
    {
        private IQueryable<EnhancedStudentInformation> query;
        private MetricOptionsFilter filter;
        private IQueryable<EnhancedStudentInformation> studentResults;

        private IRepository<StudentMetric> studentMetricRepository;
        private IRepository<MetricInstanceExtendedPropertyWithValueToFloat> metricInstanceExtendedPropertyWithValueToFloatRepository;

        protected override void EstablishContext()
        {
            studentMetricRepository = mocks.StrictMock<IRepository<StudentMetric>>();
            Expect.Call(studentMetricRepository.GetAll()).Return(new List<StudentMetric> {
                new StudentMetric { StudentUSI = 1, ValueSortOrder = 2.0d, MetricId = 1, SchoolId = 1, },
                new StudentMetric { StudentUSI = 2, ValueSortOrder = 5.0d, MetricId = 1, SchoolId = 1, },
                new StudentMetric { StudentUSI = 3, ValueSortOrder = 10.0d, MetricId = 1, SchoolId = 1, }
            }.AsQueryable()).Repeat.Any();

            metricInstanceExtendedPropertyWithValueToFloatRepository = mocks.StrictMock<IRepository<MetricInstanceExtendedPropertyWithValueToFloat>>();
            Expect.Call(metricInstanceExtendedPropertyWithValueToFloatRepository.GetAll()).Return(new List<MetricInstanceExtendedPropertyWithValueToFloat>().AsQueryable()).Repeat.Any();

            query = new List<EnhancedStudentInformation>
            {
                new EnhancedStudentInformation { StudentUSI = 1, SchoolId = 1},
                new EnhancedStudentInformation { StudentUSI = 2, SchoolId = 1 },
                new EnhancedStudentInformation { StudentUSI = 3, SchoolId = 1 },
            }.AsQueryable();

            filter = new MetricOptionsFilter(studentMetricRepository, metricInstanceExtendedPropertyWithValueToFloatRepository);

            base.EstablishContext();
        }

        protected override void ExecuteTest()
        {

            var queryOptions = new StudentMetricsProviderQueryOptions
            {
                MetricOptionGroups = new[]
                {
                    new MetricFilterOptionGroup
                    {
                        MetricFilterOptions = new[]
                            {
                                new MetricFilterOption
                                {
                                    MetricId = 1,
                                    ValueGreaterThan = 5.0d,
                                },
                            }
                    }
                }
            };

            studentResults = filter.ApplyFilter(query, queryOptions);
        }

        [Test]
        public void Should_return_correct_number_of_results()
        {
            Assert.That(studentResults.Count(), Is.EqualTo(1));
            Assert.That(studentResults.First().StudentUSI, Is.EqualTo(3));
        }
    }

    public class When_Getting_StudentList_With_MetricOptionsFilter_ValueLessThanEqualTo : TestFixtureBase
    {
        private IQueryable<EnhancedStudentInformation> query;
        private MetricOptionsFilter filter;
        private IQueryable<EnhancedStudentInformation> studentResults;

        private IRepository<StudentMetric> studentMetricRepository;
        private IRepository<MetricInstanceExtendedPropertyWithValueToFloat> metricInstanceExtendedPropertyWithValueToFloatRepository;

        protected override void EstablishContext()
        {
            studentMetricRepository = mocks.StrictMock<IRepository<StudentMetric>>();
            Expect.Call(studentMetricRepository.GetAll()).Return(new List<StudentMetric> {
                new StudentMetric { StudentUSI = 1, ValueSortOrder = 2.0d, MetricId = 1, SchoolId = 1, },
                new StudentMetric { StudentUSI = 2, ValueSortOrder = 5.0d, MetricId = 1, SchoolId = 1, },
                new StudentMetric { StudentUSI = 3, ValueSortOrder = 10.0d, MetricId = 1, SchoolId = 1, }
            }.AsQueryable()).Repeat.Any();

            metricInstanceExtendedPropertyWithValueToFloatRepository = mocks.StrictMock<IRepository<MetricInstanceExtendedPropertyWithValueToFloat>>();
            Expect.Call(metricInstanceExtendedPropertyWithValueToFloatRepository.GetAll()).Return(new List<MetricInstanceExtendedPropertyWithValueToFloat>().AsQueryable()).Repeat.Any();

            query = new List<EnhancedStudentInformation>
            {
                new EnhancedStudentInformation { StudentUSI = 1, SchoolId = 1},
                new EnhancedStudentInformation { StudentUSI = 2, SchoolId = 1 },
                new EnhancedStudentInformation { StudentUSI = 3, SchoolId = 1 },
            }.AsQueryable();

            filter = new MetricOptionsFilter(studentMetricRepository, metricInstanceExtendedPropertyWithValueToFloatRepository);

            base.EstablishContext();
        }

        protected override void ExecuteTest()
        {

            var queryOptions = new StudentMetricsProviderQueryOptions
            {
                MetricOptionGroups = new[]
                {
                    new MetricFilterOptionGroup
                    {
                        MetricFilterOptions = new[]
                            {
                                new MetricFilterOption
                                {
                                    MetricId = 1,
                                    ValueLessThanEqualTo = 5.0d,
                                },
                            }
                    }
                }
            };

            studentResults = filter.ApplyFilter(query, queryOptions);
        }

        [Test]
        public void Should_return_correct_number_of_results()
        {
            Assert.That(studentResults.Count(), Is.EqualTo(2));
            Assert.That(studentResults.First().StudentUSI, Is.EqualTo(1));
            Assert.That(studentResults.Skip(1).First().StudentUSI, Is.EqualTo(2));
        }
    }

    public class When_Getting_StudentList_With_MetricOptionsFilter_ValueLessThan : TestFixtureBase
    {
        private IQueryable<EnhancedStudentInformation> query;
        private MetricOptionsFilter filter;
        private IQueryable<EnhancedStudentInformation> studentResults;

        private IRepository<StudentMetric> studentMetricRepository;
        private IRepository<MetricInstanceExtendedPropertyWithValueToFloat> metricInstanceExtendedPropertyWithValueToFloatRepository;

        protected override void EstablishContext()
        {
            studentMetricRepository = mocks.StrictMock<IRepository<StudentMetric>>();
            Expect.Call(studentMetricRepository.GetAll()).Return(new List<StudentMetric> {
                new StudentMetric { StudentUSI = 1, ValueSortOrder = 2.0d, MetricId = 1, SchoolId = 1, },
                new StudentMetric { StudentUSI = 2, ValueSortOrder = 5.0d, MetricId = 1, SchoolId = 1, },
                new StudentMetric { StudentUSI = 3, ValueSortOrder = 10.0d, MetricId = 1, SchoolId = 1, }
            }.AsQueryable()).Repeat.Any();

            metricInstanceExtendedPropertyWithValueToFloatRepository = mocks.StrictMock<IRepository<MetricInstanceExtendedPropertyWithValueToFloat>>();
            Expect.Call(metricInstanceExtendedPropertyWithValueToFloatRepository.GetAll()).Return(new List<MetricInstanceExtendedPropertyWithValueToFloat>().AsQueryable()).Repeat.Any();

            query = new List<EnhancedStudentInformation>
            {
                new EnhancedStudentInformation { StudentUSI = 1, SchoolId = 1},
                new EnhancedStudentInformation { StudentUSI = 2, SchoolId = 1 },
                new EnhancedStudentInformation { StudentUSI = 3, SchoolId = 1 },
            }.AsQueryable();

            filter = new MetricOptionsFilter(studentMetricRepository, metricInstanceExtendedPropertyWithValueToFloatRepository);

            base.EstablishContext();
        }

        protected override void ExecuteTest()
        {

            var queryOptions = new StudentMetricsProviderQueryOptions
            {
                MetricOptionGroups = new[]
                {
                    new MetricFilterOptionGroup
                    {
                        MetricFilterOptions = new[]
                            {
                                new MetricFilterOption
                                {
                                    MetricId = 1,
                                    ValueLessThan = 5.0d,
                                },
                            }
                    }
                }
            };

            studentResults = filter.ApplyFilter(query, queryOptions);
        }

        [Test]
        public void Should_return_correct_number_of_results()
        {
            Assert.That(studentResults.Count(), Is.EqualTo(1));
            Assert.That(studentResults.First().StudentUSI, Is.EqualTo(1));
        }
    }

    public class When_Getting_StudentList_With_MetricOptionsFilter_ValueEqualTo : TestFixtureBase
    {
        private IQueryable<EnhancedStudentInformation> query;
        private MetricOptionsFilter filter;
        private IQueryable<EnhancedStudentInformation> studentResults;

        private IRepository<StudentMetric> studentMetricRepository;
        private IRepository<MetricInstanceExtendedPropertyWithValueToFloat> metricInstanceExtendedPropertyWithValueToFloatRepository;

        private string SuppliedMetricInstanceValue1 = "some value";
        private string SuppliedMetricInstanceValue2 = "some value two";

        protected override void EstablishContext()
        {
            studentMetricRepository = mocks.StrictMock<IRepository<StudentMetric>>();
            Expect.Call(studentMetricRepository.GetAll()).Return(new List<StudentMetric> {
                new StudentMetric { StudentUSI = 1, MetricId = 1, SchoolId = 1, Value = SuppliedMetricInstanceValue1 },
                new StudentMetric { StudentUSI = 2, MetricId = 1, SchoolId = 1, Value = SuppliedMetricInstanceValue2 },
                new StudentMetric { StudentUSI = 3, MetricId = 1, SchoolId = 1, }
            }.AsQueryable()).Repeat.Any();

            metricInstanceExtendedPropertyWithValueToFloatRepository = mocks.StrictMock<IRepository<MetricInstanceExtendedPropertyWithValueToFloat>>();
            Expect.Call(metricInstanceExtendedPropertyWithValueToFloatRepository.GetAll()).Return(new List<MetricInstanceExtendedPropertyWithValueToFloat>().AsQueryable()).Repeat.Any();

            query = new List<EnhancedStudentInformation>
            {
                new EnhancedStudentInformation { StudentUSI = 1, SchoolId = 1},
                new EnhancedStudentInformation { StudentUSI = 2, SchoolId = 1 },
                new EnhancedStudentInformation { StudentUSI = 3, SchoolId = 1 },
            }.AsQueryable();

            filter = new MetricOptionsFilter(studentMetricRepository, metricInstanceExtendedPropertyWithValueToFloatRepository);

            base.EstablishContext();
        }

        protected override void ExecuteTest()
        {

            var queryOptions = new StudentMetricsProviderQueryOptions
            {
                MetricOptionGroups = new[]
                {
                    new MetricFilterOptionGroup
                    {
                        MetricFilterOptions = new[]
                            {
                                new MetricFilterOption
                                {
                                    MetricId = 1,
                                    MetricInstanceEqualTo = SuppliedMetricInstanceValue1,
                                },
                            }
                    }
                }
            };

            studentResults = filter.ApplyFilter(query, queryOptions);
        }

        [Test]
        public void Should_return_correct_number_of_results()
        {
            Assert.That(studentResults.Count(), Is.EqualTo(1));
            Assert.That(studentResults.First().StudentUSI, Is.EqualTo(1));
        }
    }
}
