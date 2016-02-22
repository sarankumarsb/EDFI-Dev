using EdFi.Dashboards.Metric.Resources.Models;
using EdFi.Dashboards.Metric.Resources.Services;
using EdFi.Dashboards.Resources.Metric.Requests;
using EdFi.Dashboards.Testing;
using NUnit.Framework;
using Rhino.Mocks;

namespace EdFi.Dashboards.Metric.Resources.Tests.Services
{
    public class When_loading_metric_data : TestFixtureBase
    {
        private IMetricDataProvider<LocalEducationAgencyMetricInstanceSetRequest> dataProvider1;
        private IMetricDataProvider<LocalEducationAgencyMetricInstanceSetRequest> dataProvider2;
        private IMetricDataProvider<LocalEducationAgencyMetricInstanceSetRequest> dataProvider3;
        private LocalEducationAgencyMetricInstanceSetRequest suppliedRequest;

        private const int metricMetadataNodeId1 = 1001;
        private const int metricMetadataNodeId3 = 1003;

        private MetricDataContainer actualModel;

        protected override void EstablishContext()
        {
            suppliedRequest = LocalEducationAgencyMetricInstanceSetRequest.Create(1, 1000);

            dataProvider1 = mocks.StrictMock<IMetricDataProvider<LocalEducationAgencyMetricInstanceSetRequest>>();
            dataProvider2 = mocks.StrictMock<IMetricDataProvider<LocalEducationAgencyMetricInstanceSetRequest>>();
            dataProvider3 = mocks.StrictMock<IMetricDataProvider<LocalEducationAgencyMetricInstanceSetRequest>>();

            Expect.Call(dataProvider1.CanProvideData(suppliedRequest)).Return(true);
            Expect.Call(dataProvider2.CanProvideData(suppliedRequest)).Return(false);
            Expect.Call(dataProvider3.CanProvideData(suppliedRequest)).Return(true);

            Expect.Call(dataProvider1.Get(suppliedRequest)).Return(new TestMetricData(metricMetadataNodeId1));
            Expect.Call(dataProvider3.Get(suppliedRequest)).Return(new TestMetricData(metricMetadataNodeId3));
        }

        protected override void ExecuteTest()
        {
            var service = new MetricDataService<LocalEducationAgencyMetricInstanceSetRequest>(new [] { dataProvider1, dataProvider2, dataProvider3 });
            actualModel = service.Get(suppliedRequest);
        }

        [Test]
        public void Can_load_data_from_supplied_container()
        {
            Assert.That(actualModel, Is.Not.Null);
            Assert.That(actualModel.GetMetricData(new MetricMetadataNode(null) { MetricNodeId = metricMetadataNodeId1 }), Is.Not.Null);
            Assert.That(actualModel.GetMetricData(new MetricMetadataNode(null) { MetricNodeId = metricMetadataNodeId3 }), Is.Not.Null);
        }
    }

    public class TestMetricData : MetricData
    {
        private int metricNodeId;

        public TestMetricData(int metricNodeId)
        {
            this.metricNodeId = metricNodeId;
        }

        public override bool CanSupplyMetricData(MetricMetadataNode metricMetadataNode)
        {
            return metricNodeId == metricMetadataNode.MetricNodeId;
        }
    }
}
