using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EdFi.Dashboards.Data.Repository;
using EdFi.Dashboards.Metric.Data.Entities;
using EdFi.Dashboards.Metric.Resources.Services;
using EdFi.Dashboards.Metric.Resources.Services.Data;
using EdFi.Dashboards.Testing;
using NUnit.Framework;
using Rhino.Mocks;

namespace EdFi.Dashboards.Metric.Resources.Tests.Services.Data
{
    public class When_loading_metric_component_data_from_the_database : TestFixtureBase
    {
        private Guid suppliedMetricInstanceSetKey_ForWantedData;
        private Guid suppliedMetricInstanceSetKey_ForNOTWantedData_0;
        private Guid suppliedMetricInstanceSetKey_ForNOTWantedData_1;

        private IRepository<MetricComponent> metricComponentRepository;
        private IEnumerable<MetricComponent> actualModel;

        protected override void EstablishContext()
        {
            suppliedMetricInstanceSetKey_ForWantedData = new Guid("56cff024-f54a-4a7f-89c2-5af94a4660da");
            suppliedMetricInstanceSetKey_ForNOTWantedData_0 = new Guid("26A3CB30-F849-4C57-A7EA-D600D409E687");
            suppliedMetricInstanceSetKey_ForNOTWantedData_1 = new Guid("FD473AB7-BF77-48D8-ADA1-B3CBC2DF5D44");

            metricComponentRepository = mocks.StrictMock<IRepository<MetricComponent>>();
            Expect.Call(metricComponentRepository.GetAll()).Return(GetSuppliedMetricComponentData());
        }

        private IQueryable<MetricComponent> GetSuppliedMetricComponentData()
        {
            return (new List<MetricComponent>
                        {
                            new MetricComponent { MetricInstanceSetKey = suppliedMetricInstanceSetKey_ForWantedData, MetricId=1, MetricStateTypeId = 1, Format = "Format1", Name="Name1", TrendDirection=1, Value = "Value", ValueTypeName = "ValueTypeName1"},
                            new MetricComponent { MetricInstanceSetKey = suppliedMetricInstanceSetKey_ForWantedData, MetricId=2, MetricStateTypeId = 1, Format = "Format2", Name="Name2", TrendDirection=1, Value = "Value", ValueTypeName = "ValueTypeName2"},
                            new MetricComponent { MetricInstanceSetKey = suppliedMetricInstanceSetKey_ForNOTWantedData_0, MetricId=3, MetricStateTypeId = 1, Format = "Format3", Name="Name3", TrendDirection=1, Value = "Value", ValueTypeName = "ValueTypeName3"},
                            new MetricComponent { MetricInstanceSetKey = suppliedMetricInstanceSetKey_ForNOTWantedData_1, MetricId=4, MetricStateTypeId = 1, Format = "Format4", Name="Name4", TrendDirection=1, Value = "Value", ValueTypeName = "ValueTypeName4"}
                        }).AsQueryable();
        }

        protected override void ExecuteTest()
        {
            var metricComponentsService = new MetricComponentsService(metricComponentRepository);
            actualModel = metricComponentsService.Get(MetricDataRequest.Create(suppliedMetricInstanceSetKey_ForWantedData));
        }

        [Test]
        public void Should_only_retrieve_records_for_the_specified_metric_instance_key()
        {
            Assert.That(actualModel.Count(), Is.EqualTo(2));
            Assert.That(actualModel.Count(x => x.MetricInstanceSetKey == suppliedMetricInstanceSetKey_ForWantedData), Is.EqualTo(2));
        }
    }
}
