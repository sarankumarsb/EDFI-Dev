// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using EdFi.Dashboards.Data.Repository;
using EdFi.Dashboards.Metric.Data.Entities;
using EdFi.Dashboards.Metric.Resources.Models;
using EdFi.Dashboards.Metric.Resources.Providers;
using EdFi.Dashboards.Metric.Resources.Services;
using EdFi.Dashboards.Metric.Resources.Services.Data;
using EdFi.Dashboards.Testing;
using NUnit.Framework;
using Rhino.Mocks;
using MetricComponent = EdFi.Dashboards.Metric.Data.Entities.MetricComponent;
using MetricIndicator = EdFi.Dashboards.Metric.Data.Entities.MetricIndicator;

namespace EdFi.Dashboards.Metric.Resources.Tests.Services
{
    [TestFixture]
    public class CurrentYearMetricDataProviderFixture : TestFixtureBase
    {
        private MetricData actualModel;
        private IMetricInstancesService metricInstancesService;
        private IMetricInstanceExtendedPropertiesService metricInstanceExtendedPropertiesService;
        private IMetricComponentsService metricComponentsService;
        private IMetricGoalsService metricGoalsService;
        private IMetricIndicatorsService metricIndicatorsService;
        private IMetricInstanceFootnotesService metricInstanceFootnotesService;
        private IMetricFootnoteDescriptionTypesService metricFootnoteDescType;
        private IMetricInstanceSetKeyResolver<TestMetricInstanceSetRequestBase> metricInstanceSetKeyResolver;

        private TestMetricInstanceSetRequestBase suppliedRequest;
        private Guid suppliedMetricInstanceSetKey_ForWantedData;

        protected override void EstablishContext()
        {
            suppliedMetricInstanceSetKey_ForWantedData = new Guid("56cff024-f54a-4a7f-89c2-5af94a4660da");
            suppliedRequest = new TestMetricInstanceSetRequestBase();

            //Use mock data
            metricInstanceSetKeyResolver = mocks.StrictMock<IMetricInstanceSetKeyResolver<TestMetricInstanceSetRequestBase>>();
            Expect.Call(metricInstanceSetKeyResolver.GetMetricInstanceSetKey(suppliedRequest)).Return(suppliedMetricInstanceSetKey_ForWantedData);

            metricInstancesService = mocks.StrictMock<IMetricInstancesService>();
            Expect.Call(metricInstancesService.Get(null)).IgnoreArguments().Return(GetSuppliedMetricInstanceData());

            metricInstanceExtendedPropertiesService = mocks.StrictMock<IMetricInstanceExtendedPropertiesService>();
            Expect.Call(metricInstanceExtendedPropertiesService.Get(null)).IgnoreArguments().Return(GetSuppliedMetricInstanceExtendedPropertyData());

            metricComponentsService = mocks.StrictMock<IMetricComponentsService>();
            Expect.Call(metricComponentsService.Get(null)).IgnoreArguments().Return(GetSuppliedMetricComponentData());

            metricGoalsService = mocks.StrictMock<IMetricGoalsService>();
            Expect.Call(metricGoalsService.Get(null)).IgnoreArguments().Return(GetSuppliedMetricGoalData());

            metricIndicatorsService = mocks.StrictMock<IMetricIndicatorsService>();
            Expect.Call(metricIndicatorsService.Get(null)).IgnoreArguments().Return(GetSuppliedMetricIndicatorData());

            metricFootnoteDescType = mocks.StrictMock<IMetricFootnoteDescriptionTypesService>();
            Expect.Call(metricFootnoteDescType.Get(null)).IgnoreArguments().Return(getSuppliedMetricFootnotDescTypeData());

            metricInstanceFootnotesService = mocks.StrictMock<IMetricInstanceFootnotesService>();
            Expect.Call(metricInstanceFootnotesService.Get(null)).IgnoreArguments().Return(GetSuppliedMetricFootnoteData());
        }

        protected override void ExecuteTest()
        {
            var provider = new CurrentYearMetricDataProvider<TestMetricInstanceSetRequestBase>(metricInstanceSetKeyResolver,
                                                                                                metricInstancesService, 
                                                                                                metricInstanceExtendedPropertiesService, 
                                                                                                metricComponentsService, 
                                                                                                metricGoalsService, 
                                                                                                metricIndicatorsService, 
                                                                                                metricInstanceFootnotesService, 
                                                                                                metricFootnoteDescType);
            actualModel = provider.Get(suppliedRequest);
        }

        private IQueryable<MetricInstanceFootnote> GetSuppliedMetricFootnoteData()
        {
            return (new List<MetricInstanceFootnote>
            { 
                new MetricInstanceFootnote {MetricInstanceSetKey = suppliedMetricInstanceSetKey_ForWantedData, MetricId = 1, FootnoteTypeId = 1, MetricFootnoteDescriptionTypeId = 1, FootnoteText = "", Count = 1},
                //new MetricInstanceFootnote {MetricInstanceSetKey = suppliedMetricInstanceSetKey_ForNOTWantedData_0, MetricId = 1, FootnoteTypeId = 1, MetricFootnoteDescriptionTypeId = 1, FootnoteText = "", Count = 1},
                //new MetricInstanceFootnote {MetricInstanceSetKey =  suppliedMetricInstanceSetKey_ForNOTWantedData_1, MetricId = 1, FootnoteTypeId = 1, MetricFootnoteDescriptionTypeId = 1, FootnoteText = "", Count = 1}
            }).AsQueryable();
        }

        private IQueryable<MetricIndicator> GetSuppliedMetricIndicatorData()
        {
            return (new List<MetricIndicator>
                        {
                            new MetricIndicator{MetricInstanceSetKey = suppliedMetricInstanceSetKey_ForWantedData, MetricId = 1, IndicatorTypeId = 1},
                            new MetricIndicator{MetricInstanceSetKey = suppliedMetricInstanceSetKey_ForWantedData, MetricId = 2, IndicatorTypeId = 2},
                            //new MetricIndicator{MetricInstanceSetKey = suppliedMetricInstanceSetKey_ForNOTWantedData_1, MetricId = 3, IndicatorTypeId = 3}
                        }).AsQueryable();
        }

        private IQueryable<MetricFootnoteDescriptionType> getSuppliedMetricFootnotDescTypeData()
        {
            return (new List<MetricFootnoteDescriptionType>
                        {
                            new MetricFootnoteDescriptionType {MetricFootnoteDescriptionTypeId = 1, CodeValue = "Codevalue", Description = "Description"},
                            new MetricFootnoteDescriptionType {MetricFootnoteDescriptionTypeId = 2, CodeValue = "Codevalue", Description = "Description"},
                            new MetricFootnoteDescriptionType {MetricFootnoteDescriptionTypeId = 3, CodeValue = "Codevalue", Description = "Description"}
                        }).AsQueryable();
        }

        private IQueryable<MetricInstance> GetSuppliedMetricInstanceData()
        {
            return (new List<MetricInstance>
                        {
                            new MetricInstance{ MetricInstanceSetKey = suppliedMetricInstanceSetKey_ForWantedData, Context = "Context1", Flag = true, MetricId = 1, MetricStateTypeId=1, TrendDirection = 1, Value = "Value1", ValueTypeName = "ValueTypeName1" },
                            new MetricInstance{ MetricInstanceSetKey = suppliedMetricInstanceSetKey_ForWantedData, Context = "Context2", Flag = true, MetricId = 2, MetricStateTypeId=2, TrendDirection = 1, Value = "Value2", ValueTypeName = "ValueTypeName2" },
                            //new MetricInstance{ MetricInstanceSetKey = suppliedMetricInstanceSetKey_ForNOTWantedData_0, Context = "Context3", Flag = true, MetricId = 3, MetricStateTypeId=3, TrendDirection = 1, Value = "Value3", ValueTypeName = "ValueTypeName3" },
                            //new MetricInstance{ MetricInstanceSetKey = suppliedMetricInstanceSetKey_ForNOTWantedData_1, Context = "Context4", Flag = true, MetricId = 4, MetricStateTypeId=4, TrendDirection = 1, Value = "Value4", ValueTypeName = "ValueTypeName4" }
                        }).AsQueryable();
        }

        private IQueryable<MetricInstanceExtendedProperty> GetSuppliedMetricInstanceExtendedPropertyData()
        {
            return (new List<MetricInstanceExtendedProperty>
                        {
                            new MetricInstanceExtendedProperty{ MetricInstanceSetKey = suppliedMetricInstanceSetKey_ForWantedData, MetricId = 1, Name="MetricName1",Value = "MetricValue1", ValueTypeName = "MetricValueTypeName1"},
                            new MetricInstanceExtendedProperty{ MetricInstanceSetKey = suppliedMetricInstanceSetKey_ForWantedData, MetricId = 2, Name="MetricName2",Value = "MetricValue2", ValueTypeName = "MetricValueTypeName2"},
                            //new MetricInstanceExtendedProperty{ MetricInstanceSetKey = suppliedMetricInstanceSetKey_ForNOTWantedData_0, MetricId = 3, Name="MetricName3",Value = "MetricValue3", ValueTypeName = "MetricValueTypeName3"},
                            //new MetricInstanceExtendedProperty{ MetricInstanceSetKey = suppliedMetricInstanceSetKey_ForNOTWantedData_1, MetricId = 4, Name="MetricName4",Value = "MetricValue4", ValueTypeName = "MetricValueTypeName4"}
                        }).AsQueryable();
        }

        private IQueryable<MetricComponent> GetSuppliedMetricComponentData()
        {
            return (new List<MetricComponent>
                        {
                            new MetricComponent{MetricInstanceSetKey = suppliedMetricInstanceSetKey_ForWantedData, MetricId=1, MetricStateTypeId = 1, Format = "Format1", Name="Name1", TrendDirection=1, Value = "Value", ValueTypeName = "ValueTypeName1"},
                            new MetricComponent{MetricInstanceSetKey = suppliedMetricInstanceSetKey_ForWantedData, MetricId=2, MetricStateTypeId = 1, Format = "Format2", Name="Name2", TrendDirection=1, Value = "Value", ValueTypeName = "ValueTypeName2"},
                            //new MetricComponent{MetricInstanceSetKey = suppliedMetricInstanceSetKey_ForNOTWantedData_0, MetricId=3, MetricStateTypeId = 1, Format = "Format3", Name="Name3", TrendDirection=1, Value = "Value", ValueTypeName = "ValueTypeName3"},
                            //new MetricComponent{MetricInstanceSetKey = suppliedMetricInstanceSetKey_ForNOTWantedData_1, MetricId=4, MetricStateTypeId = 1, Format = "Format4", Name="Name4", TrendDirection=1, Value = "Value", ValueTypeName = "ValueTypeName4"}
                        }).AsQueryable();
        }

        private IQueryable<MetricGoal> GetSuppliedMetricGoalData()
        {
            return (new List<MetricGoal>
                        {
                            new MetricGoal {MetricInstanceSetKey = suppliedMetricInstanceSetKey_ForWantedData, MetricId = 1, Value = 5},
                            new MetricGoal {MetricInstanceSetKey = suppliedMetricInstanceSetKey_ForWantedData, MetricId = 2, Value = 6},
                            //new MetricGoal {MetricInstanceSetKey = suppliedMetricInstanceSetKey_ForNOTWantedData_1, MetricId = 3, Value = 7},
                            //new MetricGoal {MetricInstanceSetKey = suppliedMetricInstanceSetKey_ForNOTWantedData_0, MetricId = 4, Value = 8}
                        }).AsQueryable();
        }
        [Test]
        public void Should_return_model_and_lists_that_are_not_null()
        {
            Assert.NotNull(actualModel);
            Assert.NotNull(actualModel.MetricInstances);
            Assert.NotNull(actualModel.MetricComponents);
            Assert.NotNull(actualModel.MetricGoals);
            Assert.NotNull(actualModel.MetricIndicators);
            Assert.NotNull(actualModel.MetricInstanceExtendedProperties);
            Assert.NotNull(actualModel.MetricInstanceFootnotes);
            Assert.NotNull(actualModel.MetricFootnoteDescriptionTypes);
        }

        [Test]
        public void Should_have_all_properties_assigned_a_value()
        {
            actualModel.EnsureNoDefaultValues(
                "MetricData.MetricInstancesByMetricId",
                "MetricData.MetricInstanceExtendedPropertiesByMetricId",
                "MetricData.MetricGoalsByMetricId",
                "MetricData.MetricIndicatorsByMetricId",
                "MetricData.MetricInstanceFootnotesByMetricId",
                "MetricData.MetricComponentsByMetricId");
        }

        [Test]
        public void Should_return_model_with_correct_data()
        {
            var temp2 = (GetSuppliedMetricInstanceData().Where(m => m.MetricInstanceSetKey == actualModel.MetricInstances.ToList()[0].MetricInstanceSetKey && m.MetricId == actualModel.MetricInstances.ToList()[0].MetricId).Select(m => m.MetricInstanceSetKey)).SingleOrDefault();
            Assert.AreEqual(actualModel.MetricInstances.ToList()[0].MetricInstanceSetKey, temp2);
        }

        [Test]
        public void Should_return_model_with_only_data_with_correct_domain_entity_key()
        {
            foreach (var metricInstance in actualModel.MetricInstances)
            {
                Assert.AreEqual(metricInstance.MetricInstanceSetKey, suppliedMetricInstanceSetKey_ForWantedData);
            }
            foreach (var metricComponent in actualModel.MetricComponents)
            {
                Assert.AreEqual(metricComponent.MetricInstanceSetKey, suppliedMetricInstanceSetKey_ForWantedData);
            }
            foreach (var metricGoal in actualModel.MetricGoals)
            {
                Assert.AreEqual(metricGoal.MetricInstanceSetKey, suppliedMetricInstanceSetKey_ForWantedData);
            }
            foreach (var metricIndicator in actualModel.MetricIndicators)
            {
                Assert.AreEqual(metricIndicator.MetricInstanceSetKey, suppliedMetricInstanceSetKey_ForWantedData);
            }
            foreach (var metricInstanceExtendedProperty in actualModel.MetricInstanceExtendedProperties)
            {
                Assert.AreEqual(metricInstanceExtendedProperty.MetricInstanceSetKey, suppliedMetricInstanceSetKey_ForWantedData);
            }
            foreach (var metricInstanceFootnote in actualModel.MetricInstanceFootnotes)
            {
                Assert.AreEqual(metricInstanceFootnote.MetricInstanceSetKey, suppliedMetricInstanceSetKey_ForWantedData);
            }
        }
    }

    public class TestMetricInstanceSetRequestBase : MetricInstanceSetRequestBase
    {
        private int metricInstanceSetTypeId = 0;
        public override int MetricInstanceSetTypeId { get { return metricInstanceSetTypeId; } }
    }
}
