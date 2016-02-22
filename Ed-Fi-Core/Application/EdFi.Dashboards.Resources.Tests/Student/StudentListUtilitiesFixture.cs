// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;
using EdFi.Dashboards.Metric.Resources.Models;
using EdFi.Dashboards.Metric.Resources.Providers;
using EdFi.Dashboards.Metric.Resources.Services;
using EdFi.Dashboards.Resources.Models.Student;
using EdFi.Dashboards.Resources.StudentSchool;
using EdFi.Dashboards.Testing;
using NUnit.Framework;
using Rhino.Mocks;

namespace EdFi.Dashboards.Resources.Tests.Student
{
    public class When_preparing_trend_metric : TestFixtureBase
    {
        private const int suppliedStudentUSI = 1;
        private const int suppliedUniqueIdentifier = 5;
        private const int suppliedMetricVariantId = 2;
        private const int suppliedMetricValue = 3;
        private const int suppliedStateType = 1;
        private const string suppliedValueType = "System.Int32";
        private const string suppliedDisplayFormat = "dog {0} cat";
        private const int suppliedTrendInterpretation = -1;
        private const int suppliedTrendDirection = -1;
        private ITrendRenderingDispositionProvider trendRenderingDispositionProvider;
        private IMetricStateProvider metricStateProvider;
        private IStudentListUtilitiesProvider studentListUtilitiesProvider;
        private IMetricNodeResolver metricNodeResolver;

        private StudentWithMetrics.TrendMetric actualModel;

        protected override void EstablishContext()
        {
            trendRenderingDispositionProvider = mocks.StrictMock<ITrendRenderingDispositionProvider>();
            metricStateProvider = mocks.StrictMock<IMetricStateProvider>();
            metricNodeResolver = mocks.StrictMock<IMetricNodeResolver>();

            Expect.Call(trendRenderingDispositionProvider.GetTrendRenderingDisposition((TrendDirection) suppliedTrendDirection, (TrendInterpretation) suppliedTrendInterpretation)).Return(TrendEvaluation.DownGood);
            
            base.EstablishContext();
        }

        protected override void ExecuteTest()
        {
            studentListUtilitiesProvider = new StudentListUtilitiesProvider(metricStateProvider, metricNodeResolver);
            actualModel = studentListUtilitiesProvider.PrepareTrendMetric(suppliedStudentUSI, suppliedUniqueIdentifier, suppliedMetricVariantId, suppliedMetricValue.ToString(), suppliedStateType, suppliedValueType, suppliedDisplayFormat, suppliedTrendInterpretation, suppliedTrendDirection, trendRenderingDispositionProvider);
        }

        [Test]
        public void Should_prepare_model_correctly()
        {
            Assert.That(actualModel, Is.Not.Null);
            Assert.That(actualModel.StudentUSI, Is.EqualTo(suppliedStudentUSI));
            Assert.That(actualModel.MetricVariantId, Is.EqualTo(suppliedMetricVariantId));
            Assert.That(actualModel.DisplayValue, Is.EqualTo(String.Format(suppliedDisplayFormat, suppliedMetricValue)));
            Assert.That(actualModel.Value, Is.EqualTo(suppliedMetricValue));
            Assert.That(actualModel.State, Is.EqualTo((MetricStateType)suppliedStateType));
            Assert.That(actualModel.Trend, Is.EqualTo(TrendEvaluation.DownGood));
        }

        [Test]
        public virtual void Should_have_serializable_model()
        {
            actualModel.EnsureSerializableModel();
        }
    }

    public class When_preparing_trend_metric_with_null_trend_direction : TestFixtureBase
    {
        private const int suppliedStudentUSI = 1;
        private const int suppliedUniqueIdentifier = 5;
        private const int suppliedMetricVariantId = 2;
        private const int suppliedMetricValue = 3;
        private const int suppliedStateType = 1;
        private const string suppliedValueType = "System.Int32";
        private const string suppliedDisplayFormat = "dog {0} cat";
        private const int suppliedTrendInterpretation = -1;
        private IMetricStateProvider metricStateProvider;
        private IStudentListUtilitiesProvider studentListUtilitiesProvider;
        private IMetricNodeResolver metricNodeResolver;

        private StudentWithMetrics.TrendMetric actualModel;

        protected override void EstablishContext()
        {
            metricStateProvider = mocks.StrictMock<IMetricStateProvider>();
            metricNodeResolver = mocks.StrictMock<IMetricNodeResolver>();

            base.EstablishContext();
        }

        protected override void ExecuteTest()
        {
            studentListUtilitiesProvider = new StudentListUtilitiesProvider(metricStateProvider, metricNodeResolver);
            actualModel = studentListUtilitiesProvider.PrepareTrendMetric(suppliedStudentUSI, suppliedUniqueIdentifier, suppliedMetricVariantId, suppliedMetricValue.ToString(), suppliedStateType, suppliedValueType, suppliedDisplayFormat, suppliedTrendInterpretation, null, null);
        }

        [Test]
        public void Should_prepare_model_correctly()
        {
            Assert.That(actualModel, Is.Not.Null);
            Assert.That(actualModel.StudentUSI, Is.EqualTo(suppliedStudentUSI));
            Assert.That(actualModel.MetricVariantId, Is.EqualTo(suppliedMetricVariantId));
            Assert.That(actualModel.DisplayValue, Is.EqualTo(String.Format(suppliedDisplayFormat, suppliedMetricValue)));
            Assert.That(actualModel.Value, Is.EqualTo(suppliedMetricValue));
            Assert.That(actualModel.State, Is.EqualTo((MetricStateType)suppliedStateType));
            Assert.That(actualModel.Trend, Is.EqualTo(TrendEvaluation.None));
        }
    }

    public class When_preparing_trend_metric_with_null_trend_interpretation : TestFixtureBase
    {
        private const int suppliedStudentUSI = 1;
        private const int suppliedUniqueIdentifier = 5;
        private const int suppliedMetricVariantId = 2;
        private const int suppliedMetricValue = 3;
        private const int suppliedStateType = 1;
        private const string suppliedValueType = "System.Int32";
        private const string suppliedDisplayFormat = "dog {0} cat";
        private const int suppliedTrendDirection = -1;
        private IMetricStateProvider metricStateProvider;
        private IStudentListUtilitiesProvider studentListUtilitiesProvider;
        private IMetricNodeResolver metricNodeResolver;

        private StudentWithMetrics.TrendMetric actualModel;

        protected override void EstablishContext()
        {
            metricStateProvider = mocks.StrictMock<IMetricStateProvider>();
            metricNodeResolver = mocks.StrictMock<IMetricNodeResolver>();

            base.EstablishContext();
        }

        protected override void ExecuteTest()
        {
            studentListUtilitiesProvider = new StudentListUtilitiesProvider(metricStateProvider, metricNodeResolver);
            actualModel = studentListUtilitiesProvider.PrepareTrendMetric(suppliedStudentUSI, suppliedUniqueIdentifier, suppliedMetricVariantId, suppliedMetricValue.ToString(), suppliedStateType, suppliedValueType, suppliedDisplayFormat, null, suppliedTrendDirection, null);
        }

        [Test]
        public void Should_prepare_model_correctly()
        {
            Assert.That(actualModel, Is.Not.Null);
            Assert.That(actualModel.StudentUSI, Is.EqualTo(suppliedStudentUSI));
            Assert.That(actualModel.MetricVariantId, Is.EqualTo(suppliedMetricVariantId));
            Assert.That(actualModel.DisplayValue, Is.EqualTo(String.Format(suppliedDisplayFormat, suppliedMetricValue)));
            Assert.That(actualModel.Value, Is.EqualTo(suppliedMetricValue));
            Assert.That(actualModel.State, Is.EqualTo((MetricStateType)suppliedStateType));
            Assert.That(actualModel.Trend, Is.EqualTo(TrendEvaluation.None));
        }
    }

    public class When_preparing_trend_metric_with_null_values : TestFixtureBase
    {
        private const int suppliedStudentUSI = 1;
        private const int suppliedUniqueIdentifier = 5;
        private IMetricStateProvider metricStateProvider;
        private IStudentListUtilitiesProvider studentListUtilitiesProvider;
        private IMetricNodeResolver metricNodeResolver;

        private StudentWithMetrics.TrendMetric actualModel;

        protected override void EstablishContext()
        {
            metricStateProvider = mocks.StrictMock<IMetricStateProvider>();
            metricNodeResolver = mocks.StrictMock<IMetricNodeResolver>();

            base.EstablishContext();
        }

        protected override void ExecuteTest()
        {
            studentListUtilitiesProvider = new StudentListUtilitiesProvider(metricStateProvider, metricNodeResolver);
            actualModel = studentListUtilitiesProvider.PrepareTrendMetric(suppliedStudentUSI, suppliedUniqueIdentifier, null, null, null, null, null, null, null, null);
        }

        [Test]
        public void Should_prepare_model_correctly()
        {
            Assert.That(actualModel, Is.Not.Null);
            Assert.That(actualModel.StudentUSI, Is.EqualTo(suppliedStudentUSI));
            Assert.That(actualModel.MetricVariantId, Is.EqualTo(0));
            Assert.That(actualModel.DisplayValue, Is.Null);
            Assert.That(actualModel.Value == null, Is.True, "actual model value was not null");
            Assert.That(actualModel.State, Is.EqualTo(MetricStateType.None));
            Assert.That(actualModel.Trend, Is.EqualTo(TrendEvaluation.None));
        }
    }
    
    public class When_preparing_trend_metric_without_metric_metadata : TestFixtureBase
    {
        private const int suppliedStudentUSI = 1;
        private const int suppliedUniqueIdentifier = 5;
        private const int suppliedMetricVariantId = 2;
        private const int suppliedMetricValue = 3;
        private const int suppliedStateType = 1;
        private const string suppliedValueType = "System.Int32";
        private const string suppliedDisplayFormat = "dog {0} cat";
        private const int suppliedSchoolId = 1000;
        private const int suppliedTrendInterpretation = -1;
        private const int suppliedTrendDirection = -1;
        private ITrendRenderingDispositionProvider trendRenderingDispositionProvider;
        private IMetricStateProvider metricStateProvider;
        private IStudentListUtilitiesProvider studentListUtilitiesProvider;
        private IMetricNodeResolver metricNodeResolver;

        private StudentWithMetrics.TrendMetric actualModel;

        protected override void EstablishContext()
        {
            trendRenderingDispositionProvider = mocks.StrictMock<ITrendRenderingDispositionProvider>();
            metricStateProvider = mocks.StrictMock<IMetricStateProvider>();
            metricNodeResolver = mocks.StrictMock<IMetricNodeResolver>();

            Expect.Call(metricNodeResolver.GetMetricNodeForStudentFromMetricVariantId(suppliedSchoolId, suppliedMetricVariantId)).Return(new MetricMetadataNode(null) { MetricVariantId = suppliedMetricVariantId, Format = suppliedDisplayFormat, TrendInterpretation = suppliedTrendInterpretation});
            Expect.Call(trendRenderingDispositionProvider.GetTrendRenderingDisposition((TrendDirection)suppliedTrendDirection, (TrendInterpretation)suppliedTrendInterpretation)).Return(TrendEvaluation.DownGood);

            base.EstablishContext();
        }

        protected override void ExecuteTest()
        {
            studentListUtilitiesProvider = new StudentListUtilitiesProvider(metricStateProvider, metricNodeResolver);
            actualModel = studentListUtilitiesProvider.PrepareTrendMetric(suppliedStudentUSI, suppliedSchoolId, suppliedUniqueIdentifier, suppliedMetricVariantId, suppliedMetricValue.ToString(), suppliedStateType, suppliedValueType, suppliedTrendDirection, trendRenderingDispositionProvider);
        }

        [Test]
        public void Should_prepare_model_correctly()
        {
            Assert.That(actualModel, Is.Not.Null);
            Assert.That(actualModel.StudentUSI, Is.EqualTo(suppliedStudentUSI));
            Assert.That(actualModel.MetricVariantId, Is.EqualTo(suppliedMetricVariantId));
            Assert.That(actualModel.DisplayValue, Is.EqualTo(String.Format(suppliedDisplayFormat, suppliedMetricValue)));
            Assert.That(actualModel.Value, Is.EqualTo(suppliedMetricValue));
            Assert.That(actualModel.State, Is.EqualTo((MetricStateType)suppliedStateType));
            Assert.That(actualModel.Trend, Is.EqualTo(TrendEvaluation.DownGood));
        }

        [Test]
        public virtual void Should_have_serializable_model()
        {
            actualModel.EnsureSerializableModel();
        }
    }

    
    public class When_preparing_metric : TestFixtureBase
    {
        private const int suppliedStudentUSI = 1;
        private const int suppliedUniqueIdentifier = 4;
        private const int suppliedMetricVariantId = 2;
        private const int suppliedMetricValue = 3;
        private const int suppliedStateType = 1;
        private const string suppliedValueType = "System.Int32";
        private const string suppliedDisplayFormat = "dog {0} cat";
        private IMetricStateProvider metricStateProvider;
        private IStudentListUtilitiesProvider studentListUtilitiesProvider;
        private IMetricNodeResolver metricNodeResolver;

        private StudentWithMetrics.Metric actualModel;

        protected override void EstablishContext()
        {
            metricStateProvider = mocks.StrictMock<IMetricStateProvider>();
            metricNodeResolver = mocks.StrictMock<IMetricNodeResolver>();

            base.EstablishContext();
        }

        protected override void ExecuteTest()
        {
            studentListUtilitiesProvider = new StudentListUtilitiesProvider(metricStateProvider, metricNodeResolver);
            actualModel = studentListUtilitiesProvider.PrepareMetric(suppliedStudentUSI, suppliedUniqueIdentifier, suppliedMetricVariantId, suppliedMetricValue.ToString(), suppliedStateType, suppliedValueType, suppliedDisplayFormat);
        }

        [Test]
        public void Should_prepare_model_correctly()
        {
            Assert.That(actualModel, Is.Not.Null);
            Assert.That(actualModel.StudentUSI, Is.EqualTo(suppliedStudentUSI));
            Assert.That(actualModel.MetricVariantId, Is.EqualTo(suppliedMetricVariantId));
            Assert.That(actualModel.DisplayValue, Is.EqualTo(String.Format(suppliedDisplayFormat, suppliedMetricValue)));
            Assert.That(actualModel.Value, Is.EqualTo(suppliedMetricValue));
            Assert.That(actualModel.State, Is.EqualTo((MetricStateType)suppliedStateType));
        }
    }

    public class When_preparing_metric_with_null_values : TestFixtureBase
    {
        private const int suppliedStudentUSI = 1;
        private const int suppliedUniqueIdentifier = 4;
        private IMetricStateProvider metricStateProvider;
        private IStudentListUtilitiesProvider studentListUtilitiesProvider;
        private IMetricNodeResolver metricNodeResolver;

        private StudentWithMetrics.Metric actualModel;

        protected override void EstablishContext()
        {
            metricStateProvider = mocks.StrictMock<IMetricStateProvider>();
            metricNodeResolver = mocks.StrictMock<IMetricNodeResolver>();

            base.EstablishContext();
        }

        protected override void ExecuteTest()
        {
            studentListUtilitiesProvider = new StudentListUtilitiesProvider(metricStateProvider, metricNodeResolver);
            actualModel = studentListUtilitiesProvider.PrepareMetric(suppliedStudentUSI, suppliedUniqueIdentifier, null, null, null, null, null);
        }

        [Test]
        public void Should_prepare_model_correctly()
        {
            Assert.That(actualModel, Is.Not.Null);
            Assert.That(actualModel.StudentUSI, Is.EqualTo(suppliedStudentUSI));
            Assert.That(actualModel.MetricVariantId, Is.EqualTo(0));
            Assert.That(actualModel.DisplayValue, Is.Null);
            Assert.That(actualModel.Value == null, Is.True, "actual model value was not null");
            Assert.That(actualModel.State, Is.EqualTo(MetricStateType.None));
        }
    }

    public class When_preparing_indicator_metric : TestFixtureBase
    {
        private const int suppliedStudentUSI = 1;
        private const int suppliedUniqueIdentifier = 4;
        private const int suppliedMetricVariantId = 2;
        private const int suppliedMetricValue = 3;
        private const int suppliedStateType = 1;
        private const string suppliedValueType = "System.Int32";
        private const string suppliedDisplayFormat = "dog {0} cat";

        private IMetricStateProvider metricStateProvider;
        private IStudentListUtilitiesProvider studentListUtilitiesProvider;
        private IMetricNodeResolver metricNodeResolver;

        private StudentWithMetrics.IndicatorMetric actualModel;

        protected override void EstablishContext()
        {
            metricStateProvider = mocks.StrictMock<IMetricStateProvider>();
            metricNodeResolver = mocks.StrictMock<IMetricNodeResolver>();

            base.EstablishContext();
        }

        protected override void ExecuteTest()
        {
            studentListUtilitiesProvider = new StudentListUtilitiesProvider(metricStateProvider, metricNodeResolver);
            actualModel = studentListUtilitiesProvider.PrepareIndicatorMetric(suppliedStudentUSI, suppliedUniqueIdentifier, suppliedMetricVariantId, suppliedMetricValue.ToString(), suppliedStateType, suppliedValueType, suppliedDisplayFormat);
        }

        [Test]
        public void Should_prepare_model_correctly()
        {
            Assert.That(actualModel, Is.Not.Null);
            Assert.That(actualModel.StudentUSI, Is.EqualTo(suppliedStudentUSI));
            Assert.That(actualModel.MetricVariantId, Is.EqualTo(suppliedMetricVariantId));
            Assert.That(actualModel.DisplayValue, Is.EqualTo(String.Format(suppliedDisplayFormat, suppliedMetricValue)));
            Assert.That(actualModel.Value, Is.EqualTo(suppliedMetricValue));
            Assert.That(actualModel.State, Is.EqualTo((MetricStateType)suppliedStateType));
        }
    }

    public class When_preparing_indicator_metric_with_null_values : TestFixtureBase
    {
        private const int suppliedStudentUSI = 1;
        private const int suppliedUniqueIdentifier = 4;
        private IMetricStateProvider metricStateProvider;
        private IStudentListUtilitiesProvider studentListUtilitiesProvider;
        private IMetricNodeResolver metricNodeResolver;

        private StudentWithMetrics.IndicatorMetric actualModel;

        protected override void EstablishContext()
        {
            metricStateProvider = mocks.StrictMock<IMetricStateProvider>();
            metricNodeResolver = mocks.StrictMock<IMetricNodeResolver>();

            base.EstablishContext();
        }

        protected override void ExecuteTest()
        {
            studentListUtilitiesProvider = new StudentListUtilitiesProvider(metricStateProvider, metricNodeResolver);
            actualModel = studentListUtilitiesProvider.PrepareIndicatorMetric(suppliedStudentUSI, suppliedUniqueIdentifier, null, null, null, null, null);
        }

        [Test]
        public void Should_prepare_model_correctly()
        {
            Assert.That(actualModel, Is.Not.Null);
            Assert.That(actualModel.StudentUSI, Is.EqualTo(suppliedStudentUSI));
            Assert.That(actualModel.MetricVariantId, Is.EqualTo(0));
            Assert.That(actualModel.DisplayValue, Is.Null);
            Assert.That(actualModel.Value == null, Is.True, "actual model value was not null");
            Assert.That(actualModel.State, Is.EqualTo(MetricStateType.None));
        }
    }
    
    public class When_preparing_indicator_metric_without_metric_metadata : TestFixtureBase
    {
        private const int suppliedStudentUSI = 1;
        private const int suppliedUniqueIdentifier = 4;
        private const int suppliedMetricVariantId = 2;
        private const int suppliedMetricValue = 3;
        private const int suppliedStateType = 1;
        private const string suppliedValueType = "System.Int32";
        private const string suppliedDisplayFormat = "dog {0} cat";
        private const int suppliedSchoolId = 1000;

        private IMetricStateProvider metricStateProvider;
        private IStudentListUtilitiesProvider studentListUtilitiesProvider;
        private IMetricNodeResolver metricNodeResolver;

        private StudentWithMetrics.IndicatorMetric actualModel;

        protected override void EstablishContext()
        {
            metricStateProvider = mocks.StrictMock<IMetricStateProvider>();
            metricNodeResolver = mocks.StrictMock<IMetricNodeResolver>();

            Expect.Call(metricNodeResolver.GetMetricNodeForStudentFromMetricVariantId(suppliedSchoolId, suppliedMetricVariantId)).Return(new MetricMetadataNode(null) { MetricVariantId = suppliedMetricVariantId, Format = suppliedDisplayFormat });
            base.EstablishContext();
        }

        protected override void ExecuteTest()
        {
            studentListUtilitiesProvider = new StudentListUtilitiesProvider(metricStateProvider, metricNodeResolver);
            actualModel = studentListUtilitiesProvider.PrepareIndicatorMetric(suppliedStudentUSI, suppliedSchoolId, suppliedUniqueIdentifier, suppliedMetricVariantId, suppliedMetricValue.ToString(), suppliedStateType, suppliedValueType);
        }

        [Test]
        public void Should_prepare_model_correctly()
        {
            Assert.That(actualModel, Is.Not.Null);
            Assert.That(actualModel.StudentUSI, Is.EqualTo(suppliedStudentUSI));
            Assert.That(actualModel.MetricVariantId, Is.EqualTo(suppliedMetricVariantId));
            Assert.That(actualModel.DisplayValue, Is.EqualTo(String.Format(suppliedDisplayFormat, suppliedMetricValue)));
            Assert.That(actualModel.Value, Is.EqualTo(suppliedMetricValue));
            Assert.That(actualModel.State, Is.EqualTo((MetricStateType)suppliedStateType));
        }
    }


    public class When_preparing_metric_without_metric_metadata : TestFixtureBase
    {
        private const int suppliedStudentUSI = 1;
        private const int suppliedUniqueIdentifier = 4;
        private const int suppliedMetricVariantId = 2;
        private const int suppliedMetricValue = 3;
        private const int suppliedStateType = 1;
        private const string suppliedValueType = "System.Int32";
        private const string suppliedDisplayFormat = "dog {0} cat";
        private const int suppliedSchoolId = 1000;

        private IMetricStateProvider metricStateProvider;
        private IStudentListUtilitiesProvider studentListUtilitiesProvider;
        private IMetricNodeResolver metricNodeResolver;

        private StudentWithMetrics.Metric actualModel;

        protected override void EstablishContext()
        {
            metricStateProvider = mocks.StrictMock<IMetricStateProvider>();
            metricNodeResolver = mocks.StrictMock<IMetricNodeResolver>();

            Expect.Call(metricNodeResolver.GetMetricNodeForStudentFromMetricVariantId(suppliedSchoolId, suppliedMetricVariantId)).Return(new MetricMetadataNode(null) { MetricVariantId = suppliedMetricVariantId, Format = suppliedDisplayFormat });
            base.EstablishContext();
        }

        protected override void ExecuteTest()
        {
            studentListUtilitiesProvider = new StudentListUtilitiesProvider(metricStateProvider, metricNodeResolver);
            actualModel = studentListUtilitiesProvider.PrepareMetric(suppliedStudentUSI, suppliedSchoolId, suppliedUniqueIdentifier, suppliedMetricVariantId, suppliedMetricValue.ToString(), suppliedStateType, suppliedValueType);
        }

        [Test]
        public void Should_prepare_model_correctly()
        {
            Assert.That(actualModel, Is.Not.Null);
            Assert.That(actualModel.StudentUSI, Is.EqualTo(suppliedStudentUSI));
            Assert.That(actualModel.MetricVariantId, Is.EqualTo(suppliedMetricVariantId));
            Assert.That(actualModel.DisplayValue, Is.EqualTo(String.Format(suppliedDisplayFormat, suppliedMetricValue)));
            Assert.That(actualModel.Value, Is.EqualTo(suppliedMetricValue));
            Assert.That(actualModel.State, Is.EqualTo((MetricStateType)suppliedStateType));
        }
    }
}
