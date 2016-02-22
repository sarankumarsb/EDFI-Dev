using EdFi.Dashboards.Data.Entities;
using EdFi.Dashboards.Metric.Resources.Models;
using EdFi.Dashboards.Metric.Resources.Providers;
using EdFi.Dashboards.Metric.Resources.Services;
using EdFi.Dashboards.Resources.Models.CustomGrid;
using EdFi.Dashboards.Resources.Models.Student;
using EdFi.Dashboards.Resources.StudentSchool;
using EdFi.Dashboards.Testing;
using NUnit.Framework;
using Rhino.Mocks;

namespace EdFi.Dashboards.Resources.Tests
{
    public class When_Calling_ToListCellTypedMetric : TestFixtureBase
    {
        private StudentMetric _sourceMetric;
        private IStudentListUtilitiesProvider _utilitiesProvider;
        private ITrendRenderingDispositionProvider _trendDispositionProvider;

        protected override void EstablishContext()
        {
            _utilitiesProvider = mocks.StrictMock<IStudentListUtilitiesProvider>();
            Expect.Call(_utilitiesProvider.PrepareTrendMetric(0,0,0,0,"",0,"",0,_trendDispositionProvider)).IgnoreArguments().Return(new StudentWithMetrics.TrendMetric());
            _trendDispositionProvider = mocks.StrictMock<ITrendRenderingDispositionProvider>();
        }

        protected override void ExecuteTest()
        {
            _sourceMetric = new StudentMetric()
                {
                    TrendDirection = 1,
                    TrendInterpretation = 1,
                    MetricStateTypeId = 1
                };
        }

        [Test]
        public void Should_cast_trending_metric_to_trending_type()
        {
            var typedMetric = _sourceMetric.ToListCellTypedMetric(new MetadataColumn() { MetricListCellType = MetricListCellType.TrendMetric }, _utilitiesProvider, _trendDispositionProvider);
            
            Assert.IsAssignableFrom<StudentWithMetrics.TrendMetric>(typedMetric);
        }
    }
}