using System;
using System.Collections.Generic;
using System.Linq;
using EdFi.Dashboards.Data.Entities;
using EdFi.Dashboards.Data.Repository;
using EdFi.Dashboards.Metric.Resources.Models;
using EdFi.Dashboards.Metric.Resources.Providers;
using EdFi.Dashboards.Resources.Metric.Requests;
using EdFi.Dashboards.Resources.Models.School;
using EdFi.Dashboards.Resources.School;
using EdFi.Dashboards.Testing;
using EdFi.Dashboards.Warehouse.Data.Entities;
using EdFi.Dashboards.Warehouse.Resources.Application;
using EdFi.Dashboards.Warehouse.Resources.Metric;
using NUnit.Framework;
using Rhino.Mocks;

namespace EdFi.Dashboards.Warehouse.Resources.Tests.Metric
{
    public class When_loading_school_metric_data : TestFixtureBase
    {
        private IRepository<SchoolMetricInstance> metricInstanceRepository;
        private IRepository<SchoolMetricInstanceExtendedProperty> metricInstanceExtendedPropertyRepository;
        private IRepository<SchoolMetricComponent> metricComponentRepository;
        private IBriefService schoolBriefService;
        private IMetricInstanceSetKeyResolver<SchoolMetricInstanceSetRequest> metricInstanceSetKeyResolver;
        private IWarehouseAvailabilityProvider warehouseAvailabilityProvider;
        private IMaxPriorYearProvider maxPriorYearProvider;

        private const int suppliedLocalEducationAgencyId = 1000;
        private const int suppliedSchoolId = 100010;
        private const int suppliedMetricVariantId = 2000;
        private readonly Guid suppliedMetricInstanceSetKey = Guid.NewGuid();
        private SchoolMetricInstanceSetRequest suppliedRequest;

        private MetricData actualModel;

        protected override void EstablishContext()
        {
            suppliedRequest = new SchoolMetricInstanceSetRequest{ SchoolId = suppliedSchoolId, MetricVariantId = suppliedMetricVariantId};
            metricInstanceRepository = mocks.StrictMock<IRepository<SchoolMetricInstance>>();
            metricInstanceExtendedPropertyRepository = mocks.StrictMock<IRepository<SchoolMetricInstanceExtendedProperty>>();
            metricComponentRepository = mocks.StrictMock<IRepository<SchoolMetricComponent>>();
            schoolBriefService = mocks.StrictMock<IBriefService>();
            metricInstanceSetKeyResolver = mocks.StrictMock<IMetricInstanceSetKeyResolver<SchoolMetricInstanceSetRequest>>();
            warehouseAvailabilityProvider = mocks.StrictMock<IWarehouseAvailabilityProvider>();
            maxPriorYearProvider = mocks.StrictMock<IMaxPriorYearProvider>();

            Expect.Call(metricInstanceRepository.GetAll()).Return(GetMetricInstances());
            Expect.Call(metricInstanceExtendedPropertyRepository.GetAll()).Return(GetMetricInstanceExtendedProperties());
            Expect.Call(metricComponentRepository.GetAll()).Return(GetMetricComponentProperties());
            Expect.Call(schoolBriefService.Get(null)).IgnoreArguments().Return(new BriefModel { LocalEducationAgencyId = suppliedLocalEducationAgencyId });
            Expect.Call(metricInstanceSetKeyResolver.GetMetricInstanceSetKey(suppliedRequest)).Return(suppliedMetricInstanceSetKey);
            Expect.Call(warehouseAvailabilityProvider.Get()).Return(true);
            Expect.Call(maxPriorYearProvider.Get(suppliedLocalEducationAgencyId)).Return(2011);
        }

        protected IQueryable<SchoolMetricInstance> GetMetricInstances()
        {
            var list = new List<SchoolMetricInstance>
                           {
                               new SchoolMetricInstance { LocalEducationAgencyId = suppliedLocalEducationAgencyId, SchoolId = suppliedSchoolId, MetricId = 1, SchoolYear = 2010, Context = "Wrong Year", Flag = true, MetricStateTypeId = 3, TrendDirection = -1, Value = ".99", ValueTypeName = "ValueTypeName1"},
                               new SchoolMetricInstance { LocalEducationAgencyId = suppliedLocalEducationAgencyId, SchoolId = suppliedSchoolId, MetricId = 1, SchoolYear = 2011, Context = "Metric 1 context", Flag = true, MetricStateTypeId = 3, TrendDirection = -1, Value = ".99", ValueTypeName = "ValueTypeName1"},
                               //new SchoolMetricInstance { LocalEducationAgencyId = suppliedLocalEducationAgencyId + 1, SchoolId = suppliedSchoolId, MetricId = 1, SchoolYear = 2010, Context = "Metric 2 context", Flag = true, MetricStateTypeId = 3, TrendDirection = -1, Value = ".98", ValueTypeName = "ValueTypeName2"},
                               new SchoolMetricInstance { LocalEducationAgencyId = suppliedLocalEducationAgencyId, SchoolId = suppliedSchoolId + 1, MetricId = 1, SchoolYear = 2010, Context = "Metric 2.5 context", Flag = true, MetricStateTypeId = 3, TrendDirection = -1, Value = ".98", ValueTypeName = "ValueTypeName2"},
                               //new SchoolMetricInstance { LocalEducationAgencyId = suppliedLocalEducationAgencyId, SchoolId = suppliedSchoolId, MetricId = 1, SchoolYear = 2011, Context = "Metric 3 context", Flag = true, MetricStateTypeId = 1, TrendDirection = -1, Value = ".97", ValueTypeName = "ValueTypeName3"},
                               new SchoolMetricInstance { LocalEducationAgencyId = suppliedLocalEducationAgencyId, SchoolId = suppliedSchoolId, MetricId = 2, SchoolYear = 2011, Context = "Metric 4 context", Flag = false, MetricStateTypeId = 3, TrendDirection = 1, Value = ".96", ValueTypeName = "ValueTypeName4"},
                           };
            return list.AsQueryable();
        }

        protected IQueryable<SchoolMetricInstanceExtendedProperty> GetMetricInstanceExtendedProperties()
        {
            var list = new List<SchoolMetricInstanceExtendedProperty>
                           {
                               new SchoolMetricInstanceExtendedProperty { LocalEducationAgencyId = suppliedLocalEducationAgencyId, SchoolId = suppliedSchoolId, MetricId = 1, SchoolYear = 2010, Name = "Wrong Year", Value = ".22", ValueTypeName = "ExPValueTypeName1"},
                               new SchoolMetricInstanceExtendedProperty { LocalEducationAgencyId = suppliedLocalEducationAgencyId, SchoolId = suppliedSchoolId, MetricId = 1, SchoolYear = 2011, Name = "Goal", Value = ".22", ValueTypeName = "ExPValueTypeName1"},
                               //new SchoolMetricInstanceExtendedProperty { LocalEducationAgencyId = suppliedLocalEducationAgencyId + 1, SchoolId = suppliedSchoolId, MetricId = 1, SchoolYear = 2010, Name = "Goal", Value = ".23", ValueTypeName = "ExPValueTypeName2"},
                               new SchoolMetricInstanceExtendedProperty { LocalEducationAgencyId = suppliedLocalEducationAgencyId, SchoolId = suppliedSchoolId + 1, MetricId = 1, SchoolYear = 2010, Name = "Goal", Value = ".23", ValueTypeName = "ExPValueTypeName2.5"},
                               //new SchoolMetricInstanceExtendedProperty { LocalEducationAgencyId = suppliedLocalEducationAgencyId, SchoolId = suppliedSchoolId, MetricId = 1, SchoolYear = 2011, Name = "Goal", Value = ".24", ValueTypeName = "ExPValueTypeName3"},
                               new SchoolMetricInstanceExtendedProperty { LocalEducationAgencyId = suppliedLocalEducationAgencyId, SchoolId = suppliedSchoolId, MetricId = 2, SchoolYear = 2011, Name = "Goal", Value = ".25", ValueTypeName = "ExPValueTypeName4"},
                               new SchoolMetricInstanceExtendedProperty { LocalEducationAgencyId = suppliedLocalEducationAgencyId, SchoolId = suppliedSchoolId, MetricId = 2, SchoolYear = 2011, Name = "ExtendedProperty1", Value = ".26", ValueTypeName = "ExPValueTypeName5"},
                           };
            return list.AsQueryable();
        }

        protected IQueryable<SchoolMetricComponent> GetMetricComponentProperties()
        {
            var list = new List<SchoolMetricComponent>
                           {
                               new SchoolMetricComponent { LocalEducationAgencyId = suppliedLocalEducationAgencyId, SchoolId = suppliedSchoolId, MetricId = 1, SchoolYear = 2010, Name = "Wrong Year", Value = ".22", ValueTypeName = "ValueTypeName1", TrendDirection = null, Format = "{0}", MetricStateTypeId = 1},
                               new SchoolMetricComponent { LocalEducationAgencyId = suppliedLocalEducationAgencyId, SchoolId = suppliedSchoolId, MetricId = 1, SchoolYear = 2011, Name = "Component 1", Value = ".22", ValueTypeName = "ValueTypeName2", TrendDirection = 1, Format = null, MetricStateTypeId = 3},
                               new SchoolMetricComponent { LocalEducationAgencyId = suppliedLocalEducationAgencyId, SchoolId = suppliedSchoolId, MetricId = 2, SchoolYear = 2011, Name = "Component 2", Value = ".23", ValueTypeName = "ValueTypeName3", TrendDirection = -1, Format = "{0}", MetricStateTypeId = null},
                               new SchoolMetricComponent { LocalEducationAgencyId = suppliedLocalEducationAgencyId, SchoolId = suppliedSchoolId + 1, MetricId = 2, SchoolYear = 2011, Name = "Component 2.1", Value = ".25", ValueTypeName = "ValueTypeName2", TrendDirection = 1, Format = "{0}", MetricStateTypeId = 1},
                               new SchoolMetricComponent { LocalEducationAgencyId = suppliedLocalEducationAgencyId, SchoolId = suppliedSchoolId + 1, MetricId = 2, SchoolYear = 2011, Name = "Component 2.2", Value = ".26", ValueTypeName = "ValueTypeName2", TrendDirection = -1, Format = "{0}", MetricStateTypeId = 1}
                           };
            return list.AsQueryable();
        }

        protected IQueryable<SchoolInformation> GetSchoolInformation()
        {
            var list = new List<SchoolInformation>
                           {
                               new SchoolInformation{ LocalEducationAgencyId = suppliedLocalEducationAgencyId + 1, SchoolId = suppliedSchoolId + 1},
                               new SchoolInformation{ LocalEducationAgencyId = suppliedLocalEducationAgencyId, SchoolId = suppliedSchoolId },
                           };
            return list.AsQueryable();
        }

        protected override void ExecuteTest()
        {
            var service = new SchoolMetricDataService(metricInstanceRepository, metricInstanceExtendedPropertyRepository, metricComponentRepository);
            var provider = new SchoolMetricDataProvider(service, schoolBriefService, metricInstanceSetKeyResolver, warehouseAvailabilityProvider, maxPriorYearProvider);
            actualModel = provider.Get(suppliedRequest);
        }

        [Test]
        public void Should_load_metric_instances()
        {
            Assert.That(actualModel.MetricInstances.Count(), Is.EqualTo(2));
            var metricInstance = actualModel.MetricInstances.ElementAt(0);
            Assert.That(metricInstance.MetricInstanceSetKey, Is.EqualTo(suppliedMetricInstanceSetKey));
            Assert.That(metricInstance.MetricId, Is.EqualTo(1));
            Assert.That(metricInstance.Context, Is.EqualTo("Metric 1 context"));
            Assert.That(metricInstance.Flag, Is.True);
            Assert.That(metricInstance.MetricStateTypeId, Is.EqualTo(3));
            Assert.That(metricInstance.TrendDirection, Is.EqualTo(-1));
            Assert.That(metricInstance.Value, Is.EqualTo(".99"));
            Assert.That(metricInstance.ValueTypeName, Is.EqualTo("ValueTypeName1"));
        }

        [Test]
        public void Should_load_metric_instance_extended_properties()
        {
            var metricInstanceExtendedProperty = actualModel.MetricInstanceExtendedProperties.SingleOrDefault(x => x.Name == "ExtendedProperty1");
            Assert.That(metricInstanceExtendedProperty, Is.Not.Null);
            Assert.That(metricInstanceExtendedProperty.Name, Is.EqualTo("ExtendedProperty1"));
            Assert.That(metricInstanceExtendedProperty.MetricInstanceSetKey, Is.EqualTo(suppliedMetricInstanceSetKey));
            Assert.That(metricInstanceExtendedProperty.MetricId, Is.EqualTo(2));
            Assert.That(metricInstanceExtendedProperty.Value, Is.EqualTo(".26"));
            Assert.That(metricInstanceExtendedProperty.ValueTypeName, Is.EqualTo("ExPValueTypeName5"));
        }

        [Test]
        public void Should_load_goals()
        {
            var goal = actualModel.MetricGoals.SingleOrDefault(x => x.MetricId == 2);
            Assert.That(goal, Is.Not.Null);
            Assert.That(goal.MetricId, Is.EqualTo(2));
            Assert.That(goal.MetricInstanceSetKey, Is.EqualTo(suppliedMetricInstanceSetKey));
            Assert.That(goal.Value, Is.EqualTo(.25m));
        }

        [Test]
        public void Should_load_components()
        {
            Assert.That(actualModel.MetricComponents.Count(), Is.EqualTo(2));
            var component = actualModel.MetricComponents.SingleOrDefault(x => x.MetricId == 1);
            Assert.That(component, Is.Not.Null);
            Assert.That(component.MetricId, Is.EqualTo(1));
            Assert.That(component.MetricInstanceSetKey, Is.EqualTo(suppliedMetricInstanceSetKey));
            Assert.That(component.Value, Is.EqualTo(".22"));
            Assert.That(component.Name, Is.EqualTo("Component 1"));
            Assert.That(component.TrendDirection, Is.EqualTo(1));
            Assert.That(component.Format, Is.Null);
            Assert.That(component.MetricStateTypeId, Is.EqualTo(3));
        }

        [Test]
        public void Should_add_school_year_extended_property()
        {
            var schoolYearExtendedProperty = actualModel.MetricInstanceExtendedProperties.SingleOrDefault(x => x.Name == "SchoolYear" && x.MetricId == 2);
            Assert.That(schoolYearExtendedProperty, Is.Not.Null);
            Assert.That(schoolYearExtendedProperty.Name, Is.EqualTo("SchoolYear"));
            Assert.That(schoolYearExtendedProperty.MetricInstanceSetKey, Is.EqualTo(suppliedMetricInstanceSetKey));
            Assert.That(schoolYearExtendedProperty.MetricId, Is.EqualTo(2));
            Assert.That(schoolYearExtendedProperty.Value, Is.EqualTo("2011"));
            Assert.That(schoolYearExtendedProperty.ValueTypeName, Is.EqualTo("System.Int32"));
        }
    }

    public class When_loading_school_metric_data_but_warehouse_is_unavailable : TestFixtureBase
    {
        private ISchoolMetricDataService schoolMetricDataService;
        private IBriefService schoolBriefService;
        private IMetricInstanceSetKeyResolver<SchoolMetricInstanceSetRequest> metricInstanceSetKeyResolver;
        private IWarehouseAvailabilityProvider warehouseAvailabilityProvider;
        private IMaxPriorYearProvider maxPriorYearProvider;

        private MetricData actualModel;

        private const int suppliedSchoolId = 100010;
        private const int suppliedMetricVariantId = 2000;
        private SchoolMetricInstanceSetRequest suppliedRequest;

        protected override void EstablishContext()
        {
            suppliedRequest = new SchoolMetricInstanceSetRequest { SchoolId = suppliedSchoolId, MetricVariantId = suppliedMetricVariantId };
            schoolBriefService = mocks.StrictMock<IBriefService>();
            metricInstanceSetKeyResolver = mocks.StrictMock<IMetricInstanceSetKeyResolver<SchoolMetricInstanceSetRequest>>();
            warehouseAvailabilityProvider = mocks.StrictMock<IWarehouseAvailabilityProvider>();
            maxPriorYearProvider = mocks.StrictMock<IMaxPriorYearProvider>();
            schoolMetricDataService = mocks.StrictMock<ISchoolMetricDataService>();

            Expect.Call(warehouseAvailabilityProvider.Get()).Return(false);
        }

        protected override void ExecuteTest()
        {
            var service = new SchoolMetricDataProvider(schoolMetricDataService, schoolBriefService, metricInstanceSetKeyResolver, warehouseAvailabilityProvider, maxPriorYearProvider);
            actualModel = service.Get(suppliedRequest);
        }

        [Test]
        public void Should_return_empty_model()
        {
            Assert.That(actualModel, Is.Not.Null);
        }

    }
}
