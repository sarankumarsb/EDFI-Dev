using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    public abstract class When_loading_student_school_metric_data_base : TestFixtureBase
    {
        protected IRepository<StudentSchoolMetricInstance> metricInstanceRepository;
        protected IRepository<StudentSchoolMetricInstanceExtendedProperty> metricInstanceExtendedPropertyRepository;
        protected IRepository<StudentSchoolMetricComponent> metricComponentRepository;
        protected IBriefService schoolBriefService;
        protected IMetricInstanceSetKeyResolver<StudentSchoolMetricInstanceSetRequest> metricInstanceSetKeyResolver;
        protected IWarehouseAvailabilityProvider warehouseAvailabilityProvider;
        protected IMaxPriorYearProvider maxPriorYearProvider;

        protected const int suppliedLocalEducationAgencyId = 1000;
        protected const int suppliedSchoolId = 10010;
        protected const int suppliedStudentUSI = 10020;
        protected const int suppliedMetricVariantId = 2000;
        protected readonly Guid suppliedMetricInstanceSetKey = Guid.NewGuid();
        protected StudentSchoolMetricInstanceSetRequest suppliedRequest;
        protected MetricData actualModel;

        protected override void EstablishContext()
        {
            metricInstanceRepository = mocks.StrictMock<IRepository<StudentSchoolMetricInstance>>();
            metricInstanceExtendedPropertyRepository = mocks.StrictMock<IRepository<StudentSchoolMetricInstanceExtendedProperty>>();
            metricComponentRepository = mocks.StrictMock<IRepository<StudentSchoolMetricComponent>>();
            schoolBriefService = mocks.StrictMock<IBriefService>();
            metricInstanceSetKeyResolver = mocks.StrictMock<IMetricInstanceSetKeyResolver<StudentSchoolMetricInstanceSetRequest>>();
            warehouseAvailabilityProvider = mocks.StrictMock<IWarehouseAvailabilityProvider>();
            maxPriorYearProvider = mocks.StrictMock<IMaxPriorYearProvider>();

            Expect.Call(metricInstanceRepository.GetAll()).Return(GetMetricInstances());
            Expect.Call(metricInstanceExtendedPropertyRepository.GetAll()).Return(GetMetricInstanceExtendedProperties());
            Expect.Call(metricComponentRepository.GetAll()).Return(GetMetricComponentProperties());
            Expect.Call(schoolBriefService.Get(null)).IgnoreArguments().Return(new BriefModel { LocalEducationAgencyId = suppliedLocalEducationAgencyId });
            Expect.Call(metricInstanceSetKeyResolver.GetMetricInstanceSetKey(suppliedRequest)).Return(suppliedMetricInstanceSetKey).IgnoreArguments();

            Expect.Call(warehouseAvailabilityProvider.Get()).Return(true);
            Expect.Call(maxPriorYearProvider.Get(suppliedLocalEducationAgencyId)).Return(2011);
        }

        protected IQueryable<StudentSchoolMetricInstance> GetMetricInstances()
        {
            var list = new List<StudentSchoolMetricInstance>
                           {
                               new StudentSchoolMetricInstance { LocalEducationAgencyId = suppliedLocalEducationAgencyId, SchoolId = suppliedSchoolId, StudentUSI = suppliedStudentUSI, MetricId = 1, SchoolYear = 2010, Context = "Wrong Year 1 context", Flag = true, MetricStateTypeId = 3, TrendDirection = -1, Value = ".99", ValueTypeName = "ValueTypeName1"},
                               new StudentSchoolMetricInstance { LocalEducationAgencyId = suppliedLocalEducationAgencyId, SchoolId = suppliedSchoolId, StudentUSI = suppliedStudentUSI, MetricId = 1, SchoolYear = 2011, Context = "Metric 1 context", Flag = true, MetricStateTypeId = 3, TrendDirection = -1, Value = ".99", ValueTypeName = "ValueTypeName1"},
                               new StudentSchoolMetricInstance { LocalEducationAgencyId = suppliedLocalEducationAgencyId + 1, SchoolId = suppliedSchoolId, StudentUSI = suppliedStudentUSI, MetricId = 1, SchoolYear = 2010, Context = "Metric 2 context", Flag = true, MetricStateTypeId = 3, TrendDirection = -1, Value = ".98", ValueTypeName = "ValueTypeName2"},
                               new StudentSchoolMetricInstance { LocalEducationAgencyId = suppliedLocalEducationAgencyId, SchoolId = suppliedSchoolId + 1, StudentUSI = suppliedStudentUSI, MetricId = 1, SchoolYear = 2010, Context = "Metric 2 context", Flag = true, MetricStateTypeId = 3, TrendDirection = -1, Value = ".98", ValueTypeName = "ValueTypeName2"},
                               new StudentSchoolMetricInstance { LocalEducationAgencyId = suppliedLocalEducationAgencyId, SchoolId = suppliedSchoolId, StudentUSI = suppliedStudentUSI + 1, MetricId = 1, SchoolYear = 2011, Context = "Metric 2.5 context", Flag = true, MetricStateTypeId = 3, TrendDirection = -1, Value = ".98", ValueTypeName = "ValueTypeName2"},
                               new StudentSchoolMetricInstance { LocalEducationAgencyId = suppliedLocalEducationAgencyId, SchoolId = suppliedSchoolId, StudentUSI = suppliedStudentUSI, MetricId = 2, SchoolYear = 2011, Context = "Metric 4 context", Flag = false, MetricStateTypeId = 3, TrendDirection = 1, Value = ".96", ValueTypeName = "ValueTypeName4"},
                               new StudentSchoolMetricInstance { LocalEducationAgencyId = suppliedLocalEducationAgencyId, SchoolId = suppliedSchoolId + 1, StudentUSI = suppliedStudentUSI, MetricId = 2, SchoolYear = 2011, Context = "Metric 4 context", Flag = false, MetricStateTypeId = 3, TrendDirection = 1, Value = ".97", ValueTypeName = "ValueTypeName4"},
                           };
            return list.AsQueryable();
        }

        protected IQueryable<StudentSchoolMetricInstanceExtendedProperty> GetMetricInstanceExtendedProperties()
        {
            var list = new List<StudentSchoolMetricInstanceExtendedProperty>
                           {
                               new StudentSchoolMetricInstanceExtendedProperty { LocalEducationAgencyId = suppliedLocalEducationAgencyId, SchoolId = suppliedSchoolId, StudentUSI = suppliedStudentUSI, MetricId = 1, SchoolYear = 2010, Name = "Goal", Value = ".22", ValueTypeName = "ExPValueTypeName1"},
                               new StudentSchoolMetricInstanceExtendedProperty { LocalEducationAgencyId = suppliedLocalEducationAgencyId, SchoolId = suppliedSchoolId, StudentUSI = suppliedStudentUSI, MetricId = 1, SchoolYear = 2011, Name = "Wrong Year", Value = ".22", ValueTypeName = "ExPValueTypeName1"},
                               new StudentSchoolMetricInstanceExtendedProperty { LocalEducationAgencyId = suppliedLocalEducationAgencyId + 1, SchoolId = suppliedSchoolId, StudentUSI = suppliedStudentUSI, MetricId = 1, SchoolYear = 2010, Name = "Goal", Value = ".23", ValueTypeName = "ExPValueTypeName2"},
                               new StudentSchoolMetricInstanceExtendedProperty { LocalEducationAgencyId = suppliedLocalEducationAgencyId, SchoolId = suppliedSchoolId+1, StudentUSI = suppliedStudentUSI + 1, MetricId = 1, SchoolYear = 2011, Name = "Goal", Value = ".23", ValueTypeName = "ExPValueTypeName2.5"},
                               new StudentSchoolMetricInstanceExtendedProperty { LocalEducationAgencyId = suppliedLocalEducationAgencyId, MetricId = 1, SchoolYear = 2011, Name = "Goal", Value = ".24", ValueTypeName = "ExPValueTypeName3"},
                               new StudentSchoolMetricInstanceExtendedProperty { LocalEducationAgencyId = suppliedLocalEducationAgencyId, SchoolId = suppliedSchoolId, StudentUSI = suppliedStudentUSI, MetricId = 2, SchoolYear = 2011, Name = "Goal", Value = ".25", ValueTypeName = "ExPValueTypeName4"},
                               new StudentSchoolMetricInstanceExtendedProperty { LocalEducationAgencyId = suppliedLocalEducationAgencyId, SchoolId = suppliedSchoolId, StudentUSI = suppliedStudentUSI, MetricId = 2, SchoolYear = 2011, Name = "ExtendedProperty1", Value = ".26", ValueTypeName = "ExPValueTypeName5"},
                           };
            return list.AsQueryable();
        }

        protected IQueryable<StudentSchoolMetricComponent> GetMetricComponentProperties()
        {
            var list = new List<StudentSchoolMetricComponent>
                           {
                               new StudentSchoolMetricComponent { LocalEducationAgencyId = suppliedLocalEducationAgencyId, SchoolId = suppliedSchoolId, StudentUSI = suppliedStudentUSI, MetricId = 1, SchoolYear = 2010, Name = "Wrong Year", Value = ".22", ValueTypeName = "ValueTypeName1", TrendDirection = null, Format = "{0}", MetricStateTypeId = 1},
                               new StudentSchoolMetricComponent { LocalEducationAgencyId = suppliedLocalEducationAgencyId, SchoolId = suppliedSchoolId, StudentUSI = suppliedStudentUSI, MetricId = 1, SchoolYear = 2011, Name = "Component 1", Value = ".22", ValueTypeName = "ValueTypeName2", TrendDirection = 1, Format = null, MetricStateTypeId = 3},
                               new StudentSchoolMetricComponent { LocalEducationAgencyId = suppliedLocalEducationAgencyId, SchoolId = suppliedSchoolId, StudentUSI = suppliedStudentUSI + 1, MetricId = 1, SchoolYear = 2011, Name = "Wrong Student", Value = ".23", ValueTypeName = "ValueTypeName3", TrendDirection = -1, Format = "{0}", MetricStateTypeId = null},
                               new StudentSchoolMetricComponent { LocalEducationAgencyId = suppliedLocalEducationAgencyId, SchoolId = suppliedSchoolId, StudentUSI = suppliedStudentUSI, MetricId = 2, SchoolYear = 2011, Name = "Component 2.1", Value = ".25", ValueTypeName = "ValueTypeName2", TrendDirection = 1, Format = "{0}", MetricStateTypeId = 1},
                               new StudentSchoolMetricComponent { LocalEducationAgencyId = suppliedLocalEducationAgencyId, SchoolId = suppliedSchoolId + 1, StudentUSI = suppliedStudentUSI, MetricId = 2, SchoolYear = 2011, Name = "Component 2.2", Value = ".26", ValueTypeName = "ValueTypeName2", TrendDirection = -1, Format = "{0}", MetricStateTypeId = 1}
                           };
            return list.AsQueryable();
        }

    }

    public class When_loading_student_school_metric_data_from_different_schools : When_loading_student_school_metric_data_base
    {

        private const int suppliedSchoolIdNotInStudentSchoolMetricInstanceData = 1000200;

        protected override void EstablishContext()
        {
            suppliedRequest = new StudentSchoolMetricInstanceSetRequest { SchoolId = suppliedSchoolIdNotInStudentSchoolMetricInstanceData, StudentUSI = suppliedStudentUSI, MetricVariantId = suppliedMetricVariantId };
            base.EstablishContext();
        }
        protected override void ExecuteTest()
        {
            suppliedRequest = new StudentSchoolMetricInstanceSetRequest { SchoolId = suppliedSchoolIdNotInStudentSchoolMetricInstanceData, StudentUSI = suppliedStudentUSI, MetricVariantId = suppliedMetricVariantId };
            var service = new StudentSchoolMetricDataService(metricInstanceRepository, metricInstanceExtendedPropertyRepository, metricComponentRepository);
            var provider = new StudentSchoolMetricDataProvider(service, schoolBriefService, metricInstanceSetKeyResolver, warehouseAvailabilityProvider, maxPriorYearProvider);
            actualModel = provider.Get(suppliedRequest);
        }

        [Test]
        public void should_return_data_from_smallest_school_id_if_no_match_to_the_current_school_id()
        {

            Assert.That(actualModel.MetricInstances.Count(), Is.EqualTo(2));
        }
    }
    public class When_loading_student_school_metric_data : When_loading_student_school_metric_data_base
    {

        protected override void EstablishContext()
        {
            suppliedRequest = new StudentSchoolMetricInstanceSetRequest { SchoolId = suppliedSchoolId, StudentUSI = suppliedStudentUSI, MetricVariantId = suppliedMetricVariantId };
            base.EstablishContext();
        }
   
        protected override void ExecuteTest()
        {
            var suppliedRequest = new StudentSchoolMetricInstanceSetRequest { SchoolId = suppliedSchoolId, StudentUSI = suppliedStudentUSI, MetricVariantId = suppliedMetricVariantId };
            var service = new StudentSchoolMetricDataService(metricInstanceRepository, metricInstanceExtendedPropertyRepository, metricComponentRepository);
            var provider = new StudentSchoolMetricDataProvider(service, schoolBriefService, metricInstanceSetKeyResolver, warehouseAvailabilityProvider, maxPriorYearProvider);
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

    public class When_loading_student_school_metric_data_but_warehouse_is_unavailable : TestFixtureBase
    {
        private IStudentSchoolMetricDataService studentSchoolMetricDataService;
        private IBriefService schoolBriefService;
        private IMetricInstanceSetKeyResolver<StudentSchoolMetricInstanceSetRequest> metricInstanceSetKeyResolver;
        private IWarehouseAvailabilityProvider warehouseAvailabilityProvider;
        private IMaxPriorYearProvider maxPriorYearProvider;

        private const int suppliedSchoolId = 10010;
        private const int suppliedStudentUSI = 10020;
        private const int suppliedMetricVariantId = 2000;
        private StudentSchoolMetricInstanceSetRequest suppliedRequest;

        private MetricData actualModel;

        protected override void EstablishContext()
        {
            suppliedRequest = new StudentSchoolMetricInstanceSetRequest { SchoolId = suppliedSchoolId, StudentUSI = suppliedStudentUSI, MetricVariantId = suppliedMetricVariantId };
            schoolBriefService = mocks.StrictMock<IBriefService>();
            metricInstanceSetKeyResolver = mocks.StrictMock<IMetricInstanceSetKeyResolver<StudentSchoolMetricInstanceSetRequest>>();
            warehouseAvailabilityProvider = mocks.StrictMock<IWarehouseAvailabilityProvider>();
            maxPriorYearProvider = mocks.StrictMock<IMaxPriorYearProvider>();
            studentSchoolMetricDataService = mocks.StrictMock<IStudentSchoolMetricDataService>();

            Expect.Call(warehouseAvailabilityProvider.Get()).Return(false);
        }

        protected override void ExecuteTest()
        {
            var service = new StudentSchoolMetricDataProvider(studentSchoolMetricDataService, schoolBriefService, metricInstanceSetKeyResolver, warehouseAvailabilityProvider, maxPriorYearProvider);
            actualModel = service.Get(suppliedRequest);
        }

        [Test]
        public void Should_return_empty_model()
        {
            Assert.That(actualModel, Is.Not.Null);
        }
    }
}
