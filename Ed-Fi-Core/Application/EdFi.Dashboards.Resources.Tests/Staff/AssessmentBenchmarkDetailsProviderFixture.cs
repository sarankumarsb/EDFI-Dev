using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EdFi.Dashboards.Data.Entities;
using EdFi.Dashboards.Metric.Resources.Models;
using EdFi.Dashboards.Resources.Models.Staff;
using EdFi.Dashboards.Resources.Models.Student;
using EdFi.Dashboards.Resources.Staff;
using EdFi.Dashboards.Testing;
using NUnit.Framework;
using Rhino.Mocks;

namespace EdFi.Dashboards.Resources.Tests.Staff
{
    public abstract class AssessmentBenchmarkDetailsProviderFixture : TestFixtureBase
    {
        protected AssessmentBenchmarkDetailsProvider provider;
        protected int[] objectives;
        protected StudentWithMetrics.IndicatorMetric indicatorMetric;

        protected override void EstablishContext()
        {
            provider = new AssessmentBenchmarkDetailsProvider();
        }
        
    }

    public class When_calling_assessment_benchmark_details_provider_for_get_metric_ids_for_objectives_for_reading : AssessmentBenchmarkDetailsProviderFixture
    {
        protected override void ExecuteTest()
        {
            objectives = provider.GetMetricIdsForObjectives(StaffModel.SubjectArea.ELA);
        }

        [Test]
        public void Should_return_empty_objectives()
        {
            Assert.That(objectives.Count(), Is.EqualTo(0));
        }
    }

    public class When_calling_assessment_benchmark_details_provider_for_get_metric_ids_for_objectives_for_science : AssessmentBenchmarkDetailsProviderFixture
    {
        protected override void ExecuteTest()
        {
            objectives = provider.GetMetricIdsForObjectives(StaffModel.SubjectArea.Science);
        }

        [Test]
        public void Should_return_empty_objectives()
        {
            Assert.That(objectives.Count(), Is.EqualTo(0));
        }
    }

    public class When_calling_assessment_benchmark_details_provider_for_student_assessment_init_for_reading : AssessmentDetailsProviderFixture
    {
        protected override void ExecuteTest()
        {
            indicatorMetric = provider.OnStudentAssessmentInitialized(studentWithMetricsAndAssessments, studentMetrics, StaffModel.SubjectArea.ELA);
        }

        [Test]
        public void Should_return_indicator_metric_with_empty_values()
        {
            Assert.That(indicatorMetric.MetricVariantId, Is.EqualTo(-1));
            Assert.That(indicatorMetric.MetricIndicator, Is.EqualTo((int)MetricIndicatorType.None));
            Assert.That(indicatorMetric.State, Is.EqualTo(MetricStateType.None));
        }
    }

    public class When_calling_assessment_benchmark_details_provider_for_student_assessment_init_for_math : AssessmentDetailsProviderFixture
    {
        protected override void ExecuteTest()
        {
            indicatorMetric = provider.OnStudentAssessmentInitialized(studentWithMetricsAndAssessments, studentMetrics, StaffModel.SubjectArea.Mathematics);
        }

        [Test]
        public void Should_return_indicator_metric_with_empty_values()
        {
            Assert.That(indicatorMetric.MetricVariantId, Is.EqualTo(-1));
            Assert.That(indicatorMetric.MetricIndicator, Is.EqualTo((int)MetricIndicatorType.None));
            Assert.That(indicatorMetric.State, Is.EqualTo(MetricStateType.None));
        }
    }
}
