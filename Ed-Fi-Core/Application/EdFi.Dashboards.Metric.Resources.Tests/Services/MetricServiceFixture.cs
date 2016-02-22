// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using EdFi.Dashboards.Metric.Data.Entities;
using EdFi.Dashboards.Metric.Resources.Factories;
using EdFi.Dashboards.Metric.Resources.Models;
using EdFi.Dashboards.Metric.Resources.Providers;
using EdFi.Dashboards.Metric.Resources.Services;
using EdFi.Dashboards.Resources;
using EdFi.Dashboards.Resources.Metric.Requests;
using EdFi.Dashboards.Resources.Tests;
using EdFi.Dashboards.Testing;
using NUnit.Framework;
using Rhino.Mocks;
using MetricAction = EdFi.Dashboards.Metric.Resources.Models.MetricAction;
using MetricState = EdFi.Dashboards.Metric.Resources.Models.MetricState;
using MetricType = EdFi.Dashboards.Metric.Resources.Models.MetricType;

namespace EdFi.Dashboards.Metric.Resources.Tests.Services
{
    [TestFixture]
    public class MetricServiceFixture : TestFixtureBase
    {
        private MetricService<SomeMetricInstanceSetRequest> service;
        private IMetricMetadataTreeService metricMetadataTreeService;
        private IMetricDataService<SomeMetricInstanceSetRequest> metricDataService;
        private IMetricInstanceTreeFactory metricInstanceTreeFactory;
        private IMetricNodeResolver metricNodeResolver;

        private int suppliedMetricNodeId = 5;
        private int suppliedMetricVariantId = 55500;
        private MetricBase metricBase;
        private SomeMetricInstanceSetRequest suppliedMetricInstanceSetRequest;

        protected override void EstablishContext()
        {

            suppliedMetricInstanceSetRequest = new SomeMetricInstanceSetRequest { MetricVariantId = suppliedMetricVariantId };
            //mock data services return
            metricMetadataTreeService = mocks.StrictMock<IMetricMetadataTreeService>();
            Expect.Call(metricMetadataTreeService.Get(null)).IgnoreArguments().Return(GetSuppliedMetricMetadataTree());

            metricDataService = mocks.StrictMock<IMetricDataService<SomeMetricInstanceSetRequest>>();
            Expect.Call(metricDataService.Get(suppliedMetricInstanceSetRequest)).Return(GetSuppliedMetricData());

            metricInstanceTreeFactory = mocks.StrictMock<IMetricInstanceTreeFactory>();

            Expect.Call(metricInstanceTreeFactory.CreateTree(suppliedMetricInstanceSetRequest, null, null))
                .IgnoreArguments()
                .Return(GetSuppliedMetricInstanceTree());


            var suppliedMetricNode = new MetricMetadataNode(new TestMetricMetadataTree()) { MetricNodeId = suppliedMetricNodeId };
            metricNodeResolver = mocks.StrictMock<IMetricNodeResolver>();
            Expect.Call(metricNodeResolver.ResolveFromMetricVariantId(suppliedMetricVariantId)).Return(suppliedMetricNode);
        }

        protected override void ExecuteTest()
        {
            service = new MetricService<SomeMetricInstanceSetRequest>(metricMetadataTreeService, metricDataService, metricInstanceTreeFactory, metricNodeResolver);
            metricBase = service.Get(suppliedMetricInstanceSetRequest);
        }

        private MetricMetadataTree GetSuppliedMetricMetadataTree()
        {
            var tree = new TestMetricMetadataTree();

            tree.Children = new List<MetricMetadataNode>
            {
                new MetricMetadataNode(tree)
                {
                    MetricNodeId = 1,
                    MetricId = 10,
                    RootNodeId = 1,
                    DisplayName = "DisplayName1",
                    Name = "MetricName1",
                    ShortName = "Metric Short Name1",
                    Description = "MetricDescription1",
                    ChildDomainEntityMetricId = 1,
                    Enabled = true,
                    Format = "{0:P1}",
                    ListFormat = "{0:p1}",
                    DisplayOrder = 10,
                    Tooltip = "tool tip 1",
                    MetricType = MetricType.ContainerMetric,
                    Url = "metric url1",
                    NumeratorDenominatorFormat = "N D Format1",
                    TrendInterpretation = 1,
                    Actions = new List<MetricAction>(),
                    States = new List<MetricState>(),
                    Children = new List<MetricMetadataNode>
                                                         {
                                                    new MetricMetadataNode(tree)
                                                       {
                                                           MetricNodeId = 2,
                                                           MetricId = 20,
                                                           RootNodeId = 1,
                                                           DisplayName = "DisplayName20",
                                                           Name = "MetricName20",
                                                           ShortName = "Metric Short Name20",
                                                           Description = "MetricDescription20",
                                                           ChildDomainEntityMetricId = 1,
                                                           Enabled = true,
                                                           Format = "{0:P20}",
                                                           ListFormat = "{0:p20}",
                                                           DisplayOrder = 10,
                                                           Tooltip = "tool tip 20",
                                                           MetricType = MetricType.GranularMetric,
                                                           Url = "metric url20",
                                                           NumeratorDenominatorFormat = "N D Format20",
                                                           TrendInterpretation = 1,
                                                           Actions = new List<MetricAction>(),
                                                           States = new List<MetricState>()
                                                       },
                                                       new MetricMetadataNode(tree)
                                                       {
                                                           MetricNodeId = 3,
                                                           MetricId = 30,
                                                           RootNodeId = 1,
                                                           DisplayName = "DisplayName20",
                                                           Name = "MetricName20",
                                                           ShortName = "Metric Short Name20",
                                                           Description = "MetricDescription20",
                                                           ChildDomainEntityMetricId = 1,
                                                           Enabled = true,
                                                           Format = "{0:P20}",
                                                           ListFormat = "{0:p20}",
                                                           DisplayOrder = 10,
                                                           Tooltip = "tool tip 20",
                                                           MetricType = MetricType.AggregateMetric,
                                                           Url = "metric url20",
                                                           NumeratorDenominatorFormat = "N D Format20",
                                                           TrendInterpretation = -1,
                                                           Actions = new List<MetricAction>(),
                                                           States = new List<MetricState>(),
                                                           Children = new List<MetricMetadataNode>
                                                                                     {
                                                                                         new MetricMetadataNode(tree)
                                                                                               {
                                                                                                   MetricNodeId = 4,
                                                                                                   MetricId = 40,
                                                                                                   RootNodeId = 1,
                                                                                                   DisplayName = "DisplayName20",
                                                                                                   Name = "MetricName20",
                                                                                                   ShortName = "Metric Short Name20",
                                                                                                   Description = "MetricDescription20",
                                                                                                   ChildDomainEntityMetricId = 1,
                                                                                                   Enabled = true,
                                                                                                   Format = "{0:P20}",
                                                                                                   ListFormat = "{0:p20}",
                                                                                                   DisplayOrder = 10,
                                                                                                   Tooltip = "tool tip 20",
                                                                                                   MetricType = MetricType.GranularMetric,
                                                                                                   Url = "metric url20",
                                                                                                   NumeratorDenominatorFormat = "N D Format20",
                                                                                                   TrendInterpretation = -1,
                                                                                                   Actions = new List<MetricAction>(),
                                                                                                   States = new List<MetricState>()
                                                                                               }
                                                                                     }
                                                    }
                                            },
                }
            };

            SetParent(tree.Children.ElementAt(0));

            return tree;
        }

        private void SetParent(MetricMetadataNode parent)
        {
            foreach (var child in parent.Children)
            {
                SetParent(child);
                child.Parent = parent;
            }
        }

        private MetricDataContainer GetSuppliedMetricData()
        {
            var metricData = new CurrentYearMetricData {
                                                            MetricInstances = GetSuppliedMetricInstancesData(),
                                                            MetricComponents = GetSuppliedMetricComponetsData(),
                                                            MetricGoals = GetSuppliedMetricGoalsData(),
                                                            MetricIndicators = GetSuppliedMetricIndicatorsData(),
                                                            MetricInstanceExtendedProperties = GetSuppliedMetricInstanceExtendedPropertiesData(),
                                                            MetricInstanceFootnotes = GetSuppliedMetricInstanceFootnotesData(),
                                                            MetricFootnoteDescriptionTypes = GetSuppliedMetricFootnoteDescTypesData()
                                                        };

            return new MetricDataContainer(new List<MetricData>{ metricData });
        }

        private IQueryable<MetricInstanceFootnote> GetSuppliedMetricInstanceFootnotesData()
        {
            return (new List<MetricInstanceFootnote>
            { 
                new MetricInstanceFootnote {MetricInstanceSetKey = new Guid("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"), MetricId = 10, FootnoteTypeId = 1, MetricFootnoteDescriptionTypeId = 1, FootnoteText = "FootnoteText10", Count = 1},
                new MetricInstanceFootnote {MetricInstanceSetKey = new Guid("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"), MetricId = 20, FootnoteTypeId = 1, MetricFootnoteDescriptionTypeId = 3, FootnoteText = "FootnoteText20", Count = 1},
                new MetricInstanceFootnote {MetricInstanceSetKey = new Guid("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"), MetricId = 30, FootnoteTypeId = 1, MetricFootnoteDescriptionTypeId = 1, FootnoteText = "FootnoteText30", Count = 1}
            }).AsQueryable();
        }

        private IQueryable<Metric.Data.Entities.MetricIndicator> GetSuppliedMetricIndicatorsData()
        {
            return (new List<Metric.Data.Entities.MetricIndicator>
                        {
                            new Metric.Data.Entities.MetricIndicator{MetricInstanceSetKey = new Guid("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"), MetricId = 10, IndicatorTypeId = 1},
                            new Metric.Data.Entities.MetricIndicator{MetricInstanceSetKey = new Guid("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"), MetricId = 20, IndicatorTypeId = 2},
                            new Metric.Data.Entities.MetricIndicator{MetricInstanceSetKey = new Guid("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"), MetricId = 30, IndicatorTypeId = 3}
                        }).AsQueryable();
        }

        private IQueryable<MetricFootnoteDescriptionType> GetSuppliedMetricFootnoteDescTypesData()
        {
            return (new List<MetricFootnoteDescriptionType>
                        {
                            new MetricFootnoteDescriptionType {MetricFootnoteDescriptionTypeId = 1, CodeValue = "Codevalue1", Description = "Description1"},
                            new MetricFootnoteDescriptionType {MetricFootnoteDescriptionTypeId = 2, CodeValue = "Codevalue2", Description = "Description2"},
                            new MetricFootnoteDescriptionType {MetricFootnoteDescriptionTypeId = 3, CodeValue = "Codevalue3", Description = "Description3"}
                        }).AsQueryable();
        }

        private IQueryable<MetricInstance> GetSuppliedMetricInstancesData()
        {
            return (new List<MetricInstance>
                        {
                            new MetricInstance{ MetricInstanceSetKey = new Guid("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"), Context = "Context1", Flag = true, MetricId = 10, MetricStateTypeId=1, TrendDirection = 1, Value = "1.0", ValueTypeName = "System.Double" },
                            new MetricInstance{ MetricInstanceSetKey = new Guid("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"), Context = "Context2", Flag = true, MetricId = 20, MetricStateTypeId=2, TrendDirection = 0, Value = "value", ValueTypeName = "System.String" },
                            new MetricInstance{ MetricInstanceSetKey = new Guid("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"), Context = "Context3", Flag = true, MetricId = 30, MetricStateTypeId=3, TrendDirection = -1, Value = "3.0", ValueTypeName = "System.Double" },
                            new MetricInstance{ MetricInstanceSetKey = new Guid("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"), Context = "Context4", Flag = true, MetricId = 40, MetricStateTypeId = null, TrendDirection = null, Value = "4", ValueTypeName = "System.Int32" }
                        }).AsQueryable();
        }

        private IQueryable<MetricInstanceExtendedProperty> GetSuppliedMetricInstanceExtendedPropertiesData()
        {
            return (new List<MetricInstanceExtendedProperty>
                        {
                            new MetricInstanceExtendedProperty{ MetricInstanceSetKey = new Guid("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"), MetricId = 10, Name="MetricName1",Value = "MetricValue1", ValueTypeName = "MetricValueTypeName1"},
                            new MetricInstanceExtendedProperty{ MetricInstanceSetKey = new Guid("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"), MetricId = 20, Name="MetricName2",Value = "MetricValue2", ValueTypeName = "MetricValueTypeName2"},
                            new MetricInstanceExtendedProperty{ MetricInstanceSetKey = new Guid("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"), MetricId = 30, Name="MetricName3",Value = "MetricValue3", ValueTypeName = "MetricValueTypeName3"},
                            new MetricInstanceExtendedProperty{ MetricInstanceSetKey = new Guid("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"), MetricId = 40, Name="MetricName4",Value = "MetricValue4", ValueTypeName = "MetricValueTypeName4"}
                        }).AsQueryable();
        }

        private IQueryable<Metric.Data.Entities.MetricComponent> GetSuppliedMetricComponetsData()
        {
            return (new List<Metric.Data.Entities.MetricComponent>
                        {
                            new Metric.Data.Entities.MetricComponent{MetricInstanceSetKey = new Guid("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"), MetricId=10, MetricStateTypeId = 1, Format = "Format1", Name="Name1", TrendDirection=1, Value = "Value", ValueTypeName = "ValueTypeName1"},
                            new Metric.Data.Entities.MetricComponent{MetricInstanceSetKey = new Guid("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"), MetricId=20, MetricStateTypeId = 1, Format = "Format2", Name="Name2", TrendDirection=1, Value = "Value", ValueTypeName = "ValueTypeName2"},
                            new Metric.Data.Entities.MetricComponent{MetricInstanceSetKey = new Guid("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"), MetricId=30, MetricStateTypeId = 1, Format = "Format3", Name="Name3", TrendDirection=1, Value = "Value", ValueTypeName = "ValueTypeName3"},
                            new Metric.Data.Entities.MetricComponent{MetricInstanceSetKey = new Guid("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"), MetricId=40, MetricStateTypeId = 1, Format = "Format4", Name="Name4", TrendDirection=1, Value = "Value", ValueTypeName = "ValueTypeName4"}
                        }).AsQueryable();
        }

        private IQueryable<MetricGoal> GetSuppliedMetricGoalsData()
        {
            return (new List<MetricGoal>
                        {
                            new MetricGoal {MetricInstanceSetKey = new Guid("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"), MetricId = 10, Value = 5},
                            new MetricGoal {MetricInstanceSetKey = new Guid("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"), MetricId = 20, Value = 6},
                            new MetricGoal {MetricInstanceSetKey = new Guid("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"), MetricId = 30, Value = 7},
                            new MetricGoal {MetricInstanceSetKey = new Guid("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"), MetricId = 40, Value = 8}
                        }).AsQueryable();
        }

        private MetricBase GetSuppliedMetricInstanceTree()
        {
            var metricBase = new ContainerMetric
                                   {
                                       MetricId = 1,
                                       MetricNodeId = 1,
                                       Name = "Metric One",
                                       MetricType = MetricType.ContainerMetric,
                                       Children = new List<MetricBase>
                                                          {
                                                                new GranularMetric<int>
                                                                    {
                                                                        MetricId = 2,
                                                                        MetricNodeId = 2,
                                                                        Name = "Metric Two",
                                                                        Value = 55,
                                                                        MetricType = MetricType.GranularMetric
                                                                    },
                                                                new AggregateMetric
                                                                    {
                                                                        MetricId = 3,
                                                                        MetricNodeId = 3,
                                                                        Name = "Metric Three",
                                                                        MetricType = MetricType.AggregateMetric,
                                                                        Children = new List<MetricBase>
                                                                                           {
                                                                                               new GranularMetric<int>
                                                                                                   {
                                                                                                        MetricId = 4,
                                                                                                        MetricNodeId = 4,
                                                                                                        Name = "Metric Four",
                                                                                                        Value = 2155,
                                                                                                        MetricType = MetricType.GranularMetric
                                                                                                   }
                                                                                           }
                                                            
                                                                    }
                                                          }
                                   };
            SetParent(metricBase);
            return metricBase;
        }

        private void SetParent(ContainerMetric parent)
        {
            foreach (var child in parent.Children)
            {
                var container = child as ContainerMetric;
                if (container != null)
                    SetParent(container);
                child.Parent = parent;
            }
        }

        [Test]
        public void Should_return_metric_base_that_is_not_null()
        {
            Assert.IsNotNull(metricBase);
        }

        [Test]
        public virtual void Should_have_serializable_model()
        {
            metricBase.EnsureSerializableModel();
        }
    }

    public class SomeMetricInstanceSetRequest : MetricInstanceSetRequestBase 
    {
        public int SomeValue { get; set; }

        public override int MetricInstanceSetTypeId
        {
            get { return 1; }
        }
    }
}
