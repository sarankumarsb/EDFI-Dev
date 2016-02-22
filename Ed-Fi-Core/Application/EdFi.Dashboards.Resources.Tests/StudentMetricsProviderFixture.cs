using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using EdFi.Dashboards.Data.Entities;
using EdFi.Dashboards.Data.Repository;
using EdFi.Dashboards.Metric.Data.Entities;
using EdFi.Dashboards.Resources.Models.CustomGrid;
using EdFi.Dashboards.Resources.Security.Common;
using EdFi.Dashboards.Resources.StudentMetrics;
using EdFi.Dashboards.SecurityTokenService.Authentication;
using EdFi.Dashboards.Testing;
using NUnit.Framework;
using Rhino.Mocks;

namespace EdFi.Dashboards.Resources.Tests
{
    public abstract class StudentMetricsProviderFixtureBase : TestFixtureBase
    {
        protected StudentMetricsProvider studentMetricsProvider;
        protected IRepository<StudentAccommodationCountAndMaxValue> studentAccomodationCountAndMaxValueRepository;
        protected IRepository<EnhancedStudentInformation> enhancedStudentInformationRepository;
        protected IRepository<StudentMetric> studentMetricRepository;
        protected IStateAssessmentMetricIdGroupingProvider stateAssessmentMetricIdGroupingProvider;
        protected IRepository<SchoolMetricStudentList> schoolMetricStudentListRepository;
        protected IStudentMetricFilter studentMetricFilter;
        
		protected StudentMetricsProviderQueryOptions queryOptions;
		protected IQueryable<EnhancedStudentInformation> studentResults;
		protected IQueryable<StudentMetric> metricResults;
        protected MetadataColumn sortColumn = null;
        protected string sortDirection = string.Empty;
        protected int? schoolMetricStudentListMetricId = null;

        protected const long SuppliedStudentUsi1 = 1001;
        protected const long SuppliedStudentUsi2 = 1002;
        protected const int SuppliedMetricVariant1 = 2001;

        protected override void EstablishContext()
        {
            queryOptions = GetStudentMetricsProviderQueryOptions();

            if (sortColumn != null && sortColumn.MetricVariantId == SpecialMetricVariantSortingIds.Designations)
            {
                studentAccomodationCountAndMaxValueRepository =
                    mocks.StrictMock<IRepository<StudentAccommodationCountAndMaxValue>>();
                Expect.Call(studentAccomodationCountAndMaxValueRepository.GetAll())
                      .Return(GetStudentAccommodationCountAndMaxValue())
                      .Repeat.AtLeastOnce();
            }

            enhancedStudentInformationRepository = mocks.StrictMock<IRepository<EnhancedStudentInformation>>();
            Expect.Call(enhancedStudentInformationRepository.GetAll()).Return(GetEnhancedStudentInformation()).Repeat.AtLeastOnce();
            
			studentMetricRepository = mocks.StrictMock<IRepository<StudentMetric>>();
            Expect.Call(studentMetricRepository.GetAll()).Return(GetStudentMetric()).Repeat.AtLeastOnce();

            if (queryOptions.MetricVariantIds != null && queryOptions.MetricVariantIds.Any())
            {
                //This is only used for doing the sort column, the above condition needs more work
                stateAssessmentMetricIdGroupingProvider = mocks.StrictMock<IStateAssessmentMetricIdGroupingProvider>();
                Expect.Call(stateAssessmentMetricIdGroupingProvider.GetMetricVariantGroupIds())
                      .Return(GetStateAssessmentMetricIdGrouping())
                      .Repeat.AtLeastOnce();
            }

            if (sortColumn != null && sortColumn.MetricVariantId == SpecialMetricVariantSortingIds.SchoolMetricStudentList)
            {
                //This is only used for doing the sort column, the above condition needs more work
                schoolMetricStudentListRepository = mocks.StrictMock<IRepository<SchoolMetricStudentList>>();
                Expect.Call(schoolMetricStudentListRepository.GetAll())
                      .Return(GetSchoolMetricStudentList())
                      .Repeat.AtLeastOnce();
            }

            studentMetricFilter = mocks.StrictMock<IStudentMetricFilter>();
            Expect.Call(studentMetricFilter.ApplyFilter(null, null)).IgnoreArguments().Return(null).WhenCalled(_ => _.ReturnValue = (IQueryable<EnhancedStudentInformation>)_.Arguments[0]).Repeat.AtLeastOnce();
            
            studentMetricsProvider = new StudentMetricsProvider(studentAccomodationCountAndMaxValueRepository,
                                                                enhancedStudentInformationRepository,
                                                                studentMetricRepository,
                                                                schoolMetricStudentListRepository,
                                                                stateAssessmentMetricIdGroupingProvider,
                                                                new[] { studentMetricFilter }
                                                                );

            
        }

        protected virtual IQueryable<StudentAccommodationCountAndMaxValue> GetStudentAccommodationCountAndMaxValue()
        {
            return new List<StudentAccommodationCountAndMaxValue>().AsQueryable();
        }

        protected virtual IQueryable<EnhancedStudentInformation> GetEnhancedStudentInformation()
        {
            return new List<EnhancedStudentInformation>().AsQueryable();
        }

        protected virtual IQueryable<StudentMetric> GetStudentMetric()
        {
            return new List<StudentMetric>().AsQueryable();
        }

        protected virtual int[] GetStateAssessmentMetricIdGrouping()
        {
            return new int[0];
        }

        protected virtual IQueryable<SchoolMetricStudentList> GetSchoolMetricStudentList()
        {
            return new List<SchoolMetricStudentList>().AsQueryable();
        }
		
        protected override void ExecuteTest()
        {
            studentResults = studentMetricsProvider.GetOrderedStudentList(queryOptions, sortColumn, sortDirection, schoolMetricStudentListMetricId);

            queryOptions.StudentIds = studentResults.Take(25).Select(student => student.StudentUSI).ToList();

            metricResults = studentMetricsProvider.GetStudentsWithMetrics(queryOptions);
        }

        protected virtual StudentMetricsProviderQueryOptions GetStudentMetricsProviderQueryOptions()
        {
            return new StudentMetricsProviderQueryOptions();
        }
    }

	public class When_no_students_are_populated : StudentMetricsProviderFixtureBase
	{
		[Test]
		public void Should_return_no_results_without_throwing_an_error()
		{
			Assert.That(studentResults.Count(), Is.EqualTo(0));
			Assert.That(metricResults.Count(), Is.EqualTo(0));
		}
	}

    public class When_one_student_is_populated : StudentMetricsProviderFixtureBase
    {
        protected override IQueryable<EnhancedStudentInformation> GetEnhancedStudentInformation()
        {
            return new List<EnhancedStudentInformation>
                {
                    new EnhancedStudentInformation{StudentUSI = SuppliedStudentUsi1, LastSurname = "Z"},
                    new EnhancedStudentInformation{StudentUSI = SuppliedStudentUsi2, LastSurname = "A"},
                }.AsQueryable();
        }

        [Test]
        public void Should_return_the_students_in_the_correct_order()
        {
            Assert.That(studentResults.Count(), Is.EqualTo(2));
            //Make sure sort applied.
            Assert.That(studentResults.First().StudentUSI, Is.EqualTo(SuppliedStudentUsi2));
            Assert.That(studentResults.Skip(1).First().StudentUSI, Is.EqualTo(SuppliedStudentUsi1));
            Assert.That(metricResults.Count(), Is.EqualTo(0));
        }
    }

    public class When_an_invalid_sort_column_is_passed_in : StudentMetricsProviderFixtureBase
    {
        protected override void EstablishContext()
        {
            sortColumn = new MetadataColumn { MetricVariantId = -999 };
            base.EstablishContext();
        }

        protected override IQueryable<EnhancedStudentInformation> GetEnhancedStudentInformation()
        {
            return new List<EnhancedStudentInformation>
                {
                    new EnhancedStudentInformation{StudentUSI = SuppliedStudentUsi1, LastSurname = "Z"},
                    new EnhancedStudentInformation{StudentUSI = SuppliedStudentUsi2, LastSurname = "A"},
                }.AsQueryable();
        }

        [Test]
        public void Should_return_the_students()
        {
            Assert.That(studentResults.Count(), Is.EqualTo(2));
            //Make sure sort applied.
            Assert.That(studentResults.First().StudentUSI, Is.EqualTo(SuppliedStudentUsi2));
            Assert.That(studentResults.Skip(1).First().StudentUSI, Is.EqualTo(SuppliedStudentUsi1));
            Assert.That(metricResults.Count(), Is.EqualTo(0));
        }
    }

    public class When_one_student_with_metric_is_populated : StudentMetricsProviderFixtureBase
    {
        protected override IQueryable<EnhancedStudentInformation> GetEnhancedStudentInformation()
        {
            return new List<EnhancedStudentInformation>
                {
                    new EnhancedStudentInformation{StudentUSI = SuppliedStudentUsi1, LastSurname = "Z"},
                    new EnhancedStudentInformation{StudentUSI = SuppliedStudentUsi2, LastSurname = "A"},
                }.AsQueryable();
        }

        protected override IQueryable<StudentMetric> GetStudentMetric()
        {
            return new List<StudentMetric>
                {
                    new StudentMetric{StudentUSI = SuppliedStudentUsi1}
                }.AsQueryable();
        }

        [Test]
        public void Should_return_the_students_in_the_correct_order_and_the_metric()
        {
            Assert.That(studentResults.Count(), Is.EqualTo(2));
            //Make sure sort applied.
            Assert.That(studentResults.First().StudentUSI, Is.EqualTo(SuppliedStudentUsi2));
            Assert.That(studentResults.Skip(1).First().StudentUSI, Is.EqualTo(SuppliedStudentUsi1));
            Assert.That(metricResults.Count(), Is.EqualTo(1));
        }
    }

    public class When_one_student_is_populated_with_sort_by_Designations : StudentMetricsProviderFixtureBase
    {
        protected override void EstablishContext()
        {
            sortColumn = new MetadataColumn { MetricVariantId = SpecialMetricVariantSortingIds.Designations };
            base.EstablishContext();
        }

        protected override IQueryable<StudentAccommodationCountAndMaxValue> GetStudentAccommodationCountAndMaxValue()
        {
            return new List<StudentAccommodationCountAndMaxValue>
                {
                    new StudentAccommodationCountAndMaxValue{StudentUSI = SuppliedStudentUsi1, MaxAccomodationValue = null, AccomodationCount = 2},
                    new StudentAccommodationCountAndMaxValue{StudentUSI = SuppliedStudentUsi2, MaxAccomodationValue = null, AccomodationCount = 1},
                }.AsQueryable();
        }

        protected override IQueryable<EnhancedStudentInformation> GetEnhancedStudentInformation()
        {
            return new List<EnhancedStudentInformation>
                {
                    new EnhancedStudentInformation{StudentUSI = SuppliedStudentUsi1},
                    new EnhancedStudentInformation{StudentUSI = SuppliedStudentUsi2},
                }.AsQueryable();
        }

        [Test]
        public void Should_return_the_students_in_the_correct_order()
        {
            Assert.That(studentResults.Count(), Is.EqualTo(2));
            //Make sure sort applied.
            Assert.That(studentResults.First().StudentUSI, Is.EqualTo(SuppliedStudentUsi2));
            Assert.That(studentResults.Skip(1).First().StudentUSI, Is.EqualTo(SuppliedStudentUsi1));
            Assert.That(metricResults.Count(), Is.EqualTo(0));
        }
    }

    public class When_one_student_is_populated_with_sort_by_GradeLevel : StudentMetricsProviderFixtureBase
    {
        protected override void EstablishContext()
        {
            sortColumn = new MetadataColumn { MetricVariantId = SpecialMetricVariantSortingIds.GradeLevel };
            base.EstablishContext();
        }

        protected override IQueryable<EnhancedStudentInformation> GetEnhancedStudentInformation()
        {
            return new List<EnhancedStudentInformation>
                {
                    new EnhancedStudentInformation{StudentUSI = SuppliedStudentUsi1, GradeLevelSortOrder = 2},
                    new EnhancedStudentInformation{StudentUSI = SuppliedStudentUsi2, GradeLevelSortOrder = 1},
                }.AsQueryable();
        }

        [Test]
        public void Should_return_the_students_in_the_correct_order()
        {
            Assert.That(studentResults.Count(), Is.EqualTo(2));
            //Make sure sort applied.
            Assert.That(studentResults.First().StudentUSI, Is.EqualTo(SuppliedStudentUsi2));
            Assert.That(studentResults.Skip(1).First().StudentUSI, Is.EqualTo(SuppliedStudentUsi1));
            Assert.That(metricResults.Count(), Is.EqualTo(0));
        }
    }

    public class When_one_student_is_populated_with_sort_by_SchoolMetricStudentList : StudentMetricsProviderFixtureBase
    {
        protected override void EstablishContext()
        {
            schoolMetricStudentListMetricId = SuppliedMetricVariant1;
            sortColumn = new MetadataColumn { MetricVariantId = SpecialMetricVariantSortingIds.SchoolMetricStudentList };
            base.EstablishContext();
        }

        protected override IQueryable<SchoolMetricStudentList> GetSchoolMetricStudentList()
        {
            return new List<SchoolMetricStudentList>
                {
                    new SchoolMetricStudentList{StudentUSI = SuppliedStudentUsi1, MetricId = SuppliedMetricVariant1, Value = ".2"},
                    new SchoolMetricStudentList{StudentUSI = SuppliedStudentUsi2, MetricId = SuppliedMetricVariant1, Value = ".1"},
                }.AsQueryable();
        }

        protected override IQueryable<EnhancedStudentInformation> GetEnhancedStudentInformation()
        {
            return new List<EnhancedStudentInformation>
                {
                    new EnhancedStudentInformation{StudentUSI = SuppliedStudentUsi1, GradeLevelSortOrder = 2},
                    new EnhancedStudentInformation{StudentUSI = SuppliedStudentUsi2, GradeLevelSortOrder = 1},
                }.AsQueryable();
        }

        [Test]
        public void Should_return_the_students_in_the_correct_order()
        {
            Assert.That(studentResults.Count(), Is.EqualTo(2));
            //Make sure sort applied.
            Assert.That(studentResults.First().StudentUSI, Is.EqualTo(SuppliedStudentUsi2));
            Assert.That(studentResults.Skip(1).First().StudentUSI, Is.EqualTo(SuppliedStudentUsi1));
            Assert.That(metricResults.Count(), Is.EqualTo(0));
        }
    }


    public class When_one_student_is_populated_with_sort_by_MetricValue : StudentMetricsProviderFixtureBase
    {
        protected override void EstablishContext()
        {
            schoolMetricStudentListMetricId = SuppliedMetricVariant1;
            sortColumn = new MetadataColumn { MetricVariantId = SuppliedMetricVariant1 };
            base.EstablishContext();
        }

        protected override IQueryable<EnhancedStudentInformation> GetEnhancedStudentInformation()
        {
            return new List<EnhancedStudentInformation>
                {
                    new EnhancedStudentInformation{StudentUSI = SuppliedStudentUsi1},
                    new EnhancedStudentInformation{StudentUSI = SuppliedStudentUsi2},
                }.AsQueryable();
        }

        protected override IQueryable<StudentMetric> GetStudentMetric()
        {
            return new List<StudentMetric>
                {
                    new StudentMetric{StudentUSI = SuppliedStudentUsi1, MetricVariantId = SuppliedMetricVariant1, ValueSortOrder = .2},
                    new StudentMetric{StudentUSI = SuppliedStudentUsi2, MetricVariantId = SuppliedMetricVariant1, ValueSortOrder = .1},
                }.AsQueryable();
        }

        [Test]
        public void Should_return_the_students_in_the_correct_order()
        {
            Assert.That(studentResults.Count(), Is.EqualTo(2));
            //Make sure sort applied.
            Assert.That(studentResults.First().StudentUSI, Is.EqualTo(SuppliedStudentUsi2));
            Assert.That(studentResults.Skip(1).First().StudentUSI, Is.EqualTo(SuppliedStudentUsi1));
            Assert.That(metricResults.Count(), Is.EqualTo(2));
        }
    }
}
