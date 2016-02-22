// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using EdFi.Dashboards.Infrastructure;
using EdFi.Dashboards.Infrastructure.Implementations;
using EdFi.Dashboards.Metric.Data.Entities;
using EdFi.Dashboards.Metric.Resources.Factories;
using EdFi.Dashboards.Metric.Resources.InitializationActivity;
using EdFi.Dashboards.Metric.Resources.Models;
using EdFi.Dashboards.Metric.Resources.Providers;
using EdFi.Dashboards.Metric.Resources.Services;
using EdFi.Dashboards.Metric.Resources.Tests.Services;
using EdFi.Dashboards.Resource.Models.Common;
using EdFi.Dashboards.Resources.Metric.Requests;
using EdFi.Dashboards.Resources.Tests;
using EdFi.Dashboards.Testing;
using NUnit.Framework;
using Rhino.Mocks;
using MetricAction = EdFi.Dashboards.Metric.Resources.Models.MetricAction;
using MetricState = EdFi.Dashboards.Metric.Resources.Models.MetricState;
using MetricVariantType = EdFi.Dashboards.Metric.Resources.Models.MetricVariantType;

namespace EdFi.Dashboards.Metric.Resources.Tests.Factories
{
    [TestFixture]
    public class When_calling_create_method_on_the_metric_instance_tree_factory : TestFixtureBase
    {
        private MetricBase actualTree;
        private MetricMetadataTree metricMetadataTree;
        private MetricDataContainer metricDataContainer;
        private ITrendRenderingDispositionProvider trendRenderingDispositionProvider;
        private IMetricFlagProvider metricFlagProvider;
        private IMetricGoalProvider metricGoalProvider;
        private IMetricStateProvider metricStateProvider;
        private GranularMetric<int> granularMetricInt;
        private GranularMetric<string> granularMetricString;
        private GranularMetric<object> granularMetricObject;
        private Guid metricInstanceSetKey;
        private IMetricInitializationActivity[] metricInitializationActivites;
        private MetricData suppliedMetricData;
        private SomeMetricInstanceSetRequest suppliedMetricInstanceSetRequest;
        private IMetricRouteProvider metricRouteProvider;
        private IMetricActionRouteProvider metricActionRouteProvider;
        private ISerializer serializer = new BinaryFormatterSerializer();
        private IUnderConstructionProvider underConstructionProvider = new UnderConstructionProvider();

        protected override void EstablishContext()
        {
            suppliedMetricData = new CurrentYearMetricData { MetricInstances = (new List<MetricInstance>() { new MetricInstance { MetricId = 10, Flag = true } }).AsQueryable() };
            //Set values
            metricInstanceSetKey = new Guid("55cff024-f54a-4a7f-89c2-5af94a4660da");
            granularMetricInt = new GranularMetric<int> { IsFlagged = true };
            granularMetricString = new GranularMetric<string> { IsFlagged = false };
            granularMetricObject = new GranularMetric<object> { IsFlagged = true };
            //var metadataNodeMetricId20 = GetSuppliedMetricMetadataTree().Descendants.Where(x => x.MetricId == 20).SingleOrDefault();
            //var metadataNodeMetricId40 = GetSuppliedMetricMetadataTree().Descendants.Where(x => x.MetricId == 40).SingleOrDefault();
            //var metadataNodeMetricId400 = GetSuppliedMetricMetadataTree().Descendants.Where(x => x.MetricId == 400).SingleOrDefault();
            //var metadataNodeMetricId99 = GetSuppliedMetricMetadataTree().Descendants.Where(x => x.MetricId == 99).SingleOrDefault();

            //mocked objects
            trendRenderingDispositionProvider = mocks.StrictMock<ITrendRenderingDispositionProvider>();
            Expect.Call(trendRenderingDispositionProvider.GetTrendRenderingDisposition(TrendDirection.Unchanged, (TrendInterpretation)(1))).Return(TrendEvaluation.UpGood);
            Expect.Call(trendRenderingDispositionProvider.GetTrendRenderingDisposition(TrendDirection.None, (TrendInterpretation)(-1))).Return(TrendEvaluation.UpGood).Repeat.Twice();
            Expect.Call(trendRenderingDispositionProvider.GetTrendRenderingDisposition(TrendDirection.Decreasing, TrendInterpretation.Inverse)).Return(TrendEvaluation.UpGood);

            metricFlagProvider = mocks.DynamicMock<IMetricFlagProvider>();
            //Expect.Call(metricFlagProvider.GetMetricFlag(granularMetricString, metadataNodeMetricId20, suppliedMetricData)).IgnoreArguments().Return(true);
            //Expect.Call(metricFlagProvider.GetMetricFlag(granularMetricInt, metadataNodeMetricId40, suppliedMetricData)).IgnoreArguments().Return(true);
            //Expect.Call(metricFlagProvider.GetMetricFlag(granularMetricObject, metadataNodeMetricId400, suppliedMetricData)).IgnoreArguments().Return(true);
            //Expect.Call(metricFlagProvider.GetMetricFlag(granularMetricObject, metadataNodeMetricId99, suppliedMetricData)).IgnoreArguments().Return(true);

            metricGoalProvider = mocks.StrictMock<IMetricGoalProvider>();
            //This is tested elsewhere so this is why we just want to see that there is a goal coming back and bound to the model.
            Expect.Call(metricGoalProvider.GetMetricGoal(null, null)).IgnoreArguments().Repeat.Any().Return(new Goal { Interpretation = TrendInterpretation.Standard, Value = 23 });
            /*
            Expect.Call(metricGoalProvider.GetMetricGoal(metricInstanceSetKey, 20)).Return(new Goal { Interpretation = TrendInterpretation.Standard, Value = 23 });
            Expect.Call(metricGoalProvider.GetMetricGoal(metricInstanceSetKey, 40)).Return(new Goal { Interpretation = TrendInterpretation.Standard, Value = 23 });
            Expect.Call(metricGoalProvider.GetMetricGoal(metricInstanceSetKey, 400)).Return(new Goal { Interpretation = TrendInterpretation.Standard, Value = 23 });
            Expect.Call(metricGoalProvider.GetMetricGoal(metricInstanceSetKey, 99)).Return(new Goal { Interpretation = TrendInterpretation.Standard, Value = 23 });
             */
            
            metricStateProvider = mocks.StrictMock<IMetricStateProvider>();
            //Expect.Call(metricStateProvider.GetState(40, "4", "System.Int32")).Return(new State(Models.MetricStateType.Good, ""));
            //Expect.Call(metricStateProvider.GetState(99, null, "System.Object")).Return(new State(Models.MetricStateType.Na, ""));
            //Expect.Call(metricStateProvider.GetState(20, "value", "System.String")).Return(new State(Models.MetricStateType.Good, ""));
            //Expect.Call(metricStateProvider.GetState(400, null, "System.Object")).Return(new State(Models.MetricStateType.Na, ""));
            Expect.Call(metricStateProvider.GetState(null, null)).IgnoreArguments().Return(new State(Models.MetricStateType.Na, "")).Repeat.Times(4);
            //IMetricStateProvider.GetState(EdFi.Dashboards.Metric.Resources.Models.MetricMetadataNode, EdFi.Dashboards.Metric.Data.Entities.MetricInstance); Expected #0, Actual #1.

            metricInitializationActivites = new IMetricInitializationActivity[1];
            metricInitializationActivites[0] = mocks.StrictMock<IMetricInitializationActivity>();
            metricInitializationActivites[0].InitializeMetric(null, null, null, null, null);
            LastCall.IgnoreArguments().Repeat.Any();

            //Use mock data
            metricMetadataTree = GetSuppliedMetricMetadataTree();
            metricDataContainer = GetSuppliedMetricDataContainer();

            metricRouteProvider = new FakeMetricRouteProvider();
            metricActionRouteProvider = new FakeMetricActionRouteProvider(); 

            suppliedMetricInstanceSetRequest = new SomeMetricInstanceSetRequest();
        }

        class FakeMetricRouteProvider : IMetricRouteProvider
        {
            public IEnumerable<Link> GetRoute(MetricInstanceSetRequestBase metricInstanceSetRequest, MetricMetadataNode metadataNode)
            {
                yield return new Link {Rel = LinkRel.Web, Href = "ProvidedRoute-" + metadataNode.MetricNodeId};
                yield return new Link {Rel = LinkRel.AsResource, Href = "ProvidedResourceUrl-" + metadataNode.MetricNodeId};
            }
        }
        class FakeMetricActionRouteProvider : IMetricActionRouteProvider
        {
            public string GetRoute(MetricInstanceSetRequestBase metricInstanceSetRequest, MetricAction action)
            {
                return "ProvidedUrl-" + action.MetricVariantId;
            }
        }

        protected override void ExecuteTest()
        {
            var containerMetricFactory = new ContainerMetricFactory(metricRouteProvider, metricActionRouteProvider, serializer, underConstructionProvider);
            var aggregateMetricFactory = new AggregateMetricFactory(metricFlagProvider, metricRouteProvider, metricActionRouteProvider, serializer, underConstructionProvider);
            var granularMetricFactory = new GranularMetricFactory(trendRenderingDispositionProvider, metricFlagProvider, metricStateProvider, metricRouteProvider, metricActionRouteProvider, serializer, underConstructionProvider);

            var metricInitializationActivityDataProviders = new IMetricInitializationActivityDataProvider[0];

            var factory = new MetricInstanceTreeFactory(containerMetricFactory, aggregateMetricFactory, granularMetricFactory, metricInitializationActivites, metricInitializationActivityDataProviders);
            actualTree = factory.CreateTree(suppliedMetricInstanceSetRequest, metricMetadataTree.Children.First(), metricDataContainer);
        }

        private MetricDataContainer GetSuppliedMetricDataContainer()
        {

            var metricData = new CurrentYearMetricData {
                                                           MetricInstances = GetSuppliedMetricInstancesData(),
                                                           MetricComponents = GetSuppliedMetricComponentsData(),
                                                           MetricGoals = GetSuppliedMetricGoalsData(),
                                                           MetricIndicators = GetSuppliedMetricIndicatorsData(),
                                                           MetricInstanceExtendedProperties = GetSuppliedMetricInstanceExtendedPropertiesData(),
                                                           MetricInstanceFootnotes = GetSuppliedMetricInstanceFootnotesData(),
                                                           MetricFootnoteDescriptionTypes = GetSuppliedMetricFootnoteDescTypesData()
                                                       };
            return new MetricDataContainer(new List<MetricData>{metricData});
        }

        private IQueryable<MetricInstanceFootnote> GetSuppliedMetricInstanceFootnotesData()
        {
            return (new List<MetricInstanceFootnote>
            { 
                new MetricInstanceFootnote {MetricInstanceSetKey = new Guid("55cff024-f54a-4a7f-89c2-5af94a4660da"), MetricId = 10, FootnoteTypeId = 1, MetricFootnoteDescriptionTypeId = 1, FootnoteText = "FootnoteText10", Count = 1},
                new MetricInstanceFootnote {MetricInstanceSetKey = new Guid("55cff024-f54a-4a7f-89c2-5af94a4660da"), MetricId = 20, FootnoteTypeId = 1, MetricFootnoteDescriptionTypeId = 3, FootnoteText = "FootnoteText20", Count = 1},
            }).AsQueryable();
        }

        private IQueryable<Data.Entities.MetricIndicator> GetSuppliedMetricIndicatorsData()
        {
            return (new List<Data.Entities.MetricIndicator>
                        {
                            new Data.Entities.MetricIndicator{MetricInstanceSetKey = new Guid("55cff024-f54a-4a7f-89c2-5af94a4660da"), MetricId = 10, IndicatorTypeId = 1},
                            new Data.Entities.MetricIndicator{MetricInstanceSetKey = new Guid("55cff024-f54a-4a7f-89c2-5af94a4660da"), MetricId = 20, IndicatorTypeId = 2},
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
                            new MetricInstance{ MetricInstanceSetKey = new Guid("55cff024-f54a-4a7f-89c2-5af94a4660da"), Context = "Context1", Flag = true, MetricId = 10, MetricStateTypeId=1, TrendDirection = 1, Value = "1.0", ValueTypeName = "System.Double" },
                            new MetricInstance{ MetricInstanceSetKey = new Guid("55cff024-f54a-4a7f-89c2-5af94a4660da"), Context = "Context2", Flag = true, MetricId = 20, MetricStateTypeId=2, TrendDirection = 0, Value = "value", ValueTypeName = "System.String" },
                            new MetricInstance{ MetricInstanceSetKey = new Guid("55cff024-f54a-4a7f-89c2-5af94a4660da"), Context = "Context3", Flag = true, MetricId = 400, MetricStateTypeId=3, TrendDirection = -1, Value = null, ValueTypeName = null },
                            new MetricInstance{ MetricInstanceSetKey = new Guid("55cff024-f54a-4a7f-89c2-5af94a4660da"), Context = "Context4", Flag = true, MetricId = 40, MetricStateTypeId = null, TrendDirection = null, Value = "4", ValueTypeName = "System.Int32" }
                        }).AsQueryable();
        }

        private IQueryable<MetricInstanceExtendedProperty> GetSuppliedMetricInstanceExtendedPropertiesData()
        {
            return (new List<MetricInstanceExtendedProperty>
                        {
                            new MetricInstanceExtendedProperty{ MetricInstanceSetKey = new Guid("55cff024-f54a-4a7f-89c2-5af94a4660da"), MetricId = 10, Name="MetricName1",Value = "MetricValue1", ValueTypeName = "System.String"},
                            new MetricInstanceExtendedProperty{ MetricInstanceSetKey = new Guid("55cff024-f54a-4a7f-89c2-5af94a4660da"), MetricId = 20, Name="MetricName2",Value = "25.5", ValueTypeName = "System.Double"},
                            new MetricInstanceExtendedProperty{ MetricInstanceSetKey = new Guid("55cff024-f54a-4a7f-89c2-5af94a4660da"), MetricId = 20, Name="MetricName3",Value = "7", ValueTypeName = "System.Int32"},
                            new MetricInstanceExtendedProperty{ MetricInstanceSetKey = new Guid("55cff024-f54a-4a7f-89c2-5af94a4660da"), MetricId = 40, Name="MetricName4",Value = "9.3", ValueTypeName = "System.Decimal"}
                        }).AsQueryable();
        }

        private IQueryable<Data.Entities.MetricComponent> GetSuppliedMetricComponentsData()
        {
            return (new List<Data.Entities.MetricComponent>
                        {
                            new Data.Entities.MetricComponent{MetricInstanceSetKey = new Guid("55cff024-f54a-4a7f-89c2-5af94a4660da"), MetricId=20, MetricStateTypeId = 1, Format = "Format1", Name="Name1", TrendDirection=1, Value = "Value", ValueTypeName = "ValueTypeName1"},
                            new Data.Entities.MetricComponent{MetricInstanceSetKey = new Guid("55cff024-f54a-4a7f-89c2-5af94a4660da"), MetricId=20, MetricStateTypeId = null, Format = "Format2", Name="Name2", TrendDirection=1, Value = "Value", ValueTypeName = "ValueTypeName2"},
                            new Data.Entities.MetricComponent{MetricInstanceSetKey = new Guid("55cff024-f54a-4a7f-89c2-5af94a4660da"), MetricId=20, MetricStateTypeId = 3, Format = "Format3", Name="Name3", TrendDirection=1, Value = "Value", ValueTypeName = "ValueTypeName3"},
                            new Data.Entities.MetricComponent{MetricInstanceSetKey = new Guid("55cff024-f54a-4a7f-89c2-5af94a4660da"), MetricId=20, MetricStateTypeId = 40, Format = "Format4", Name="Name4", TrendDirection=1, Value = "Value", ValueTypeName = "ValueTypeName4"}
                        }).AsQueryable();
        }

        private IQueryable<MetricGoal> GetSuppliedMetricGoalsData()
        {
            return (new List<MetricGoal>
                        {
                            new MetricGoal {MetricInstanceSetKey = new Guid("55cff024-f54a-4a7f-89c2-5af94a4660da"), MetricId = 10, Value = 5},
                            new MetricGoal {MetricInstanceSetKey = new Guid("55cff024-f54a-4a7f-89c2-5af94a4660da"), MetricId = 20, Value = 6},
                            new MetricGoal {MetricInstanceSetKey = new Guid("55cff024-f54a-4a7f-89c2-5af94a4660da"), MetricId = 40, Value = 8}
                        }).AsQueryable();
        }

        private MetricMetadataTree GetSuppliedMetricMetadataTree()
        {
            var tree = new TestMetricMetadataTree();

            tree.Children = new List<MetricMetadataNode>
            {
                new MetricMetadataNode(tree)
                       {
                           MetricNodeId = 1,
                           MetricVariantId = 1099,
                           MetricVariantType = MetricVariantType.CurrentYear,
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
                           MetricType = Models.MetricType.ContainerMetric,
                           Url = "metric url1",
                           NumeratorDenominatorFormat = "N D Format1",
                           TrendInterpretation = 1,
                           Actions = GetSuppliedMetricActions(),
                           States = GetSuppliedMetricStates(),
                           Children = new List<MetricMetadataNode>
                                                     {
                                                new MetricMetadataNode(tree)
                                                   {
                                                       MetricNodeId = 2,
                                                       MetricVariantId = 2099,
                                                       MetricVariantType = MetricVariantType.CurrentYear,
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
                                                       MetricType = Models.MetricType.GranularMetric,
                                                       Url = "metric url20",
                                                       NumeratorDenominatorFormat = "N D Format20",
                                                       TrendInterpretation = 1,
                                                       Actions = GetSuppliedMetricActions(),
                                                       States = GetSuppliedMetricStates()
                                                   },
                                                   new MetricMetadataNode(tree)
                                                   {
                                                       MetricNodeId = 3,
                                                       MetricVariantId = 3099,
                                                       MetricVariantType = MetricVariantType.CurrentYear,
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
                                                       MetricType = Models.MetricType.AggregateMetric,
                                                       Url = "metric url20",
                                                       NumeratorDenominatorFormat = "N D Format20",
                                                       TrendInterpretation = -1,
                                                       Actions = GetSuppliedMetricActions(),
                                                       States = GetSuppliedMetricStates(),
                                                       Children = new List<MetricMetadataNode>
                                                                                 {
                                                                                     new MetricMetadataNode(tree)
                                                                                           {
                                                                                               MetricNodeId = 7,
                                                                                               MetricVariantId = 9999,
                                                                                               MetricVariantType = MetricVariantType.CurrentYear,
                                                                                               MetricId = 99,
                                                                                               RootNodeId = 1,
                                                                                               DisplayName = "DisplayName7",
                                                                                               Name = "MetricName7",
                                                                                               ShortName = "Metric Short Name7",
                                                                                               Description = "MetricDescription7",
                                                                                               ChildDomainEntityMetricId = 1,
                                                                                               Enabled = true,
                                                                                               Format = "{0:P7}",
                                                                                               ListFormat = "{0:p7}",
                                                                                               DisplayOrder = 10,
                                                                                               Tooltip = "tool tip 7",
                                                                                               MetricType = Models.MetricType.GranularMetric,
                                                                                               Url = "metric url7",
                                                                                               NumeratorDenominatorFormat = "N D Format7",
                                                                                               TrendInterpretation = -1,
                                                                                               Actions = GetSuppliedMetricActions(),
                                                                                               States = GetSuppliedMetricStates()
                                                                                           },
                                                                                     new MetricMetadataNode(tree)
                                                                                           {
                                                                                               MetricNodeId = 4,
                                                                                               MetricVariantId = 4099,
                                                                                               MetricVariantType = MetricVariantType.CurrentYear,
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
                                                                                               MetricType = Models.MetricType.GranularMetric,
                                                                                               Url = "metric url20",
                                                                                               NumeratorDenominatorFormat = "N D Format20",
                                                                                               TrendInterpretation = -1,
                                                                                               Actions = GetSuppliedMetricActions(),
                                                                                               States = GetSuppliedMetricStates()
                                                                                           },
                                                                                        new MetricMetadataNode(tree)
                                                                                           {
                                                                                               MetricNodeId = 6,
                                                                                               MetricVariantId = 40099,
                                                                                               MetricVariantType = MetricVariantType.CurrentYear,
                                                                                               MetricId = 400,
                                                                                               RootNodeId = 1,
                                                                                               DisplayName = "DisplayName400",
                                                                                               Name = "MetricName400",
                                                                                               ShortName = "Metric Short Name400",
                                                                                               Description = "MetricDescription400",
                                                                                               ChildDomainEntityMetricId = 1,
                                                                                               Enabled = true,
                                                                                               Format = "{0:P4}",
                                                                                               ListFormat = "{0:p4}",
                                                                                               DisplayOrder = 10,
                                                                                               Tooltip = "tool tip 400",
                                                                                               MetricType = Models.MetricType.GranularMetric,
                                                                                               Url = "metric url400",
                                                                                               NumeratorDenominatorFormat = "N D Format400",
                                                                                               TrendInterpretation = -1,
                                                                                               Actions = GetSuppliedMetricActions(),
                                                                                               States = GetSuppliedMetricStates()
                                                                                           }

                                                                                 }
                                                }
                                        },
                       }
            };

            return tree;
        }

        private IEnumerable<MetricState> GetSuppliedMetricStates()
        {
            return new List<MetricState>{
                        new  MetricState
                            {
                                StateType = (Models.MetricStateType)2,
                                StateText = "State Text2",
                                Format = "{0:P1}",
                                IsMinValueInclusive = -1,
                                IsMaxValueInclusive = -1,
                                MinValue = 0,
                                MaxValue = 5
                            },
                        new  MetricState
                            {
                                StateType = (Models.MetricStateType)3,
                                StateText = "State Text3",
                                Format = "{0:P1}",
                                IsMinValueInclusive = -1,
                                IsMaxValueInclusive = -1,
                                MinValue = 6,
                                MaxValue = 10
                            },
                        new MetricState
                            {
                                StateType = (Models.MetricStateType)4,
                                StateText = "State Text4",
                                Format = "{0:P1}",
                                IsMinValueInclusive = -1,
                                IsMaxValueInclusive = -1,
                                MinValue = 11,
                                MaxValue = 15
                            }};
        }

        private IEnumerable<MetricAction> GetSuppliedMetricActions()
        {
            var metricActions = new List<MetricAction>
                                    {
                                        new MetricAction
                                            {
                                                MetricVariantId = 4099,
                                                Title = "Metric Action 1",
                                                Tooltip = "Metric Action tooltip 1",
                                                Url = "Url 1",
                                                ActionType = Models.MetricActionType.Link,
                                                DrilldownFooter = "ddFooter1",
                                                DrilldownHeader = "ddHeader1"
                                            }
                                    };

            return metricActions;
        }

        //write 
        [Test]
        public void Should_return_tree_that_is_not_null()
        {
            Assert.That(actualTree, Is.Not.Null);
        }

        [Test]
        public void Should_return_at_least_one_container_metric_in_tree_with_data()
        {
            var foundContainerMetric = false;

            if (actualTree.MetricType == Models.MetricType.ContainerMetric)
            {
                foundContainerMetric = true;
            }
            else if (actualTree.MetricType == Models.MetricType.AggregateMetric)
            {
                var metric = (AggregateMetric)actualTree;
                foreach (var m in metric.Children)
                {
                    if (ContainerExistsInMetricInstance(m))
                    {
                        Assert.That(m.MetricNodeId, Is.Not.EqualTo(0));
                        foundContainerMetric = true;
                    }
                }
            }

            Assert.IsTrue(foundContainerMetric);
        }

        private bool ContainerExistsInMetricInstance(MetricBase metric)
        {
            if (metric.MetricType == Models.MetricType.ContainerMetric)
                return true;
            if (metric.MetricType == Models.MetricType.AggregateMetric)
            {
                var aggregateMetric = (AggregateMetric)metric;
                return aggregateMetric.Children.Any(ContainerExistsInMetricInstance);
            }
            return false;
        }

        [Test]
        public void Should_return_at_least_one_aggregate_metric_in_tree_with_data()
        {
            var foundAggregateMetric = false;

            if (actualTree.MetricType == Models.MetricType.AggregateMetric)
            {
                foundAggregateMetric = true;
            }
            else if (actualTree.MetricType == Models.MetricType.ContainerMetric)
            {
                var metric = (ContainerMetric)actualTree;
                foreach (var m in metric.Children)
                {
                    if (AggregateExistsInMetricInstance(m))
                    {
                        Assert.That(m.MetricNodeId, Is.Not.EqualTo(0));
                        foundAggregateMetric = true;
                    }
                }
            }

            Assert.IsTrue(foundAggregateMetric);
        }


        private bool AggregateExistsInMetricInstance(MetricBase metric)
        {
            if (metric.MetricType == Models.MetricType.AggregateMetric)
                return true;
            if (metric.MetricType == Models.MetricType.ContainerMetric)
            {
                var containerMetric = (ContainerMetric)metric;
                return containerMetric.Children.Any(AggregateExistsInMetricInstance);
            }
            return false;
        }

        [Test]
        public void Should_return_at_least_one_granular_metric_in_tree()
        {
            var foundGranularMetric = false;

            if (actualTree.MetricType == Models.MetricType.GranularMetric)
            {
                foundGranularMetric = true;
            }
            else
            {
                var metric = (ContainerMetric)actualTree;
                foreach (var m in metric.Children)
                {
                    if (GranularExistsInMetricInstance(m))
                    {
                        Assert.That(m.MetricNodeId, Is.Not.EqualTo(0));
                        foundGranularMetric = true;
                    }
                }
            }

            Assert.IsTrue(foundGranularMetric);
        }

        private bool GranularExistsInMetricInstance(MetricBase metric)
        {
            if (metric.MetricType == Models.MetricType.GranularMetric)
                return true;

            var containerMetric = (ContainerMetric)metric;
            return containerMetric.Children.Any(GranularExistsInMetricInstance);
        }

        [Test]
        public void Should_return_metric_component_for_granular_metric()
        {
            var metric = (ContainerMetric) actualTree;
            var granularWithComponent = (GranularMetric<string>) metric.Descendants.FirstOrDefault(x => x.MetricId == 20);
            Assert.That(granularWithComponent, Is.Not.Null);
            Assert.That(granularWithComponent.Components.Count, Is.EqualTo(4));
            var component = granularWithComponent.Components.FirstOrDefault(x => x.Name == "Name1");
            Assert.That(component, Is.Not.Null);
            Assert.That(component.MetricStateType, Is.EqualTo(EdFi.Dashboards.Metric.Resources.Models.MetricStateType.Good));
        }

        [Test]
        public void Should_return_tree_with_proper_data_on_proper_node()
        {
            //iterate the list to see if the node meta data and instance data are overlaid correctly
            switch (actualTree.MetricType)
            {
                case Models.MetricType.ContainerMetric:
                case Models.MetricType.AggregateMetric:
                    {
                        var containerMetric = (ContainerMetric)actualTree;
                        Assert.AreEqual(containerMetric.MetricType, GetSuppliedMetricMetadataTree().Descendants.Where(x => x.MetricNodeId == containerMetric.MetricNodeId).Select(x => x.MetricType).SingleOrDefault());
                        break;
                    }
                case Models.MetricType.GranularMetric:
                    {
                        var metric = actualTree;
                        Assert.AreEqual(metric.Url, GetSuppliedMetricMetadataTree().Descendants.Where(x => x.MetricNodeId == metric.MetricNodeId).Select(x => x.Url).SingleOrDefault());
                        break;
                    }
            }
        }

        [Test]
        public void Should_return_actual_tree_with_metadata_mapped_correctly()
        {
            CheckActualTreeMatchsMetadataNode(GetSuppliedMetricMetadataTree().Children.First(), actualTree);
        }

        private void CheckActualTreeMatchsMetadataNode(MetricMetadataNode metadataNode, MetricBase metricBase)
        {
            MetricBaseEqualsMetadataNode(metadataNode, metricBase);

            for (int i = 0; i < metadataNode.Children.Count(); i++)
            {
                if (metadataNode.MetricType != Models.MetricType.GranularMetric)
                {
                    //if (metadataNode.Children.ToList()[i].MetricNodeId != 6) //this is the granular metric that should not be in the tree, tested in another test
                    CheckActualTreeMatchsMetadataNode(metadataNode.Children.ToList()[i], ((ContainerMetric)metricBase).Children.ToList()[i]);
                }
            }
        }

        private void MetricBaseEqualsMetadataNode(MetricMetadataNode metadataNode, MetricBase metricBase)
        {
            //check all properties
            Assert.AreEqual(metadataNode.Description, metricBase.Description);

            Assert.AreEqual(metadataNode.DisplayName, metricBase.DisplayName);

            Assert.AreEqual(metadataNode.ChildDomainEntityMetricId, metricBase.ChildDomainEntityMetricId);

            Assert.AreEqual(metadataNode.Enabled, metricBase.Enabled);

            Assert.AreEqual(metadataNode.MetricId, metricBase.MetricId);

            Assert.AreEqual(metadataNode.MetricNodeId, metricBase.MetricNodeId);

            Assert.AreEqual(metadataNode.MetricType, metricBase.MetricType);

            Assert.AreEqual(metadataNode.Name, metricBase.Name);

            Assert.AreEqual(metadataNode.NumeratorDenominatorFormat, metricBase.NumeratorDenominatorFormat);

            Assert.AreEqual(metadataNode.ShortName, metricBase.ShortName);

            Assert.AreEqual(metadataNode.Tooltip, metricBase.ToolTip);

            Assert.AreEqual("ProvidedRoute-" + metadataNode.MetricNodeId, metricBase.Url);
        }

        [Test]
        public void Should_return_trend_direction_none_when_trend_direction_is_null()
        {
            dynamic metric = ((ContainerMetric)actualTree).DescendantsOrSelf.Where(x => x.MetricId == 40).SingleOrDefault();
            Assert.IsTrue(metric.Trend.Direction == TrendDirection.None);
        }

        [Test]
        public void Should_set_parent_references_correctly()
        {
            var children = ((ContainerMetric) actualTree).Children;
            Assert.That(children.All(c => c.Parent == actualTree));
        }

        [Test]
        public void Should_return_granular_metric_of_type_string_with_null_value_if_value_and_type_are_null()
        {
            var nullGranularMetric = (GranularMetric<string>)((ContainerMetric)actualTree).DescendantsOrSelf.Where(x => x.MetricId == 400).Single();
            Assert.IsTrue(nullGranularMetric.Value == null);
        }

        [Test]
        public void Should_return_granular_metric_node_even_without_metric_instance_record()
        {
            var granularMetricWithoutInstance = (GranularMetric<string>)((ContainerMetric)actualTree).DescendantsOrSelf.Where(x => x.MetricId == 99).Single();
            Assert.NotNull(granularMetricWithoutInstance);
        }

        [Test]
        public void Should_return_tree_with_correctly_calculated_trend_evaluation()
        {
            var actualGranulars = ((ContainerMetric)actualTree).DescendantsOrSelf.Where(x => x.MetricType == Models.MetricType.GranularMetric);
            foreach (var actualGranular in actualGranulars)
            {
                if (actualGranular.MetricType == Models.MetricType.GranularMetric && actualGranular.MetricId != 99)
                {
                    var suppliedInstance = GetSuppliedMetricInstancesData().Where(x => x.MetricId == actualGranular.MetricId).SingleOrDefault();
                    var suppliedMetric = GetSuppliedMetricMetadataTree().Descendants.Where(x => x.MetricId == actualGranular.MetricId).SingleOrDefault();

                    dynamic value;
                    dynamic granularMetric;
                    
                    if (suppliedInstance.ValueTypeName != null)
                    {
                        value = Type.GetType("System.String") == Type.GetType(suppliedInstance.ValueTypeName)
                                            ? Activator.CreateInstance(Type.GetType(suppliedInstance.ValueTypeName),
                                                                       "".ToCharArray())
                                            : Activator.CreateInstance(Type.GetType(suppliedInstance.ValueTypeName));
                        granularMetric = DownCastToGranularMetric(value, actualGranular);
                    }
                    else
                    {
                        value = string.Empty;
                        granularMetric = DownCastToGranularMetric(value as string, actualGranular);
                    }

                    var suppliedTrendDirection = (suppliedInstance.TrendDirection.HasValue ? (TrendDirection)suppliedInstance.TrendDirection : TrendDirection.None);

                    //check direction is correct
                    Assert.AreEqual(granularMetric.Trend.Direction, suppliedTrendDirection);

                    //check interpretation
                    Assert.AreEqual(granularMetric.Trend.Interpretation, (TrendInterpretation)suppliedMetric.TrendInterpretation);

                    //check specific evaluations
                    if (granularMetric.MetricId == 20)
                        Assert.AreEqual(TrendEvaluation.NoChangeNoOpinion, granularMetric.Trend.Evaluation);

                    if (granularMetric.MetricId == 40)
                        Assert.AreEqual(TrendEvaluation.None, granularMetric.Trend.Evaluation);

                    //check evaluations
                    if (suppliedTrendDirection == TrendDirection.Decreasing && (TrendInterpretation)suppliedMetric.TrendInterpretation == TrendInterpretation.Standard)
                        Assert.AreEqual(granularMetric.Trend.Evaluation, TrendEvaluation.DownBad);

                    if (suppliedTrendDirection == TrendDirection.Increasing && (TrendInterpretation)suppliedMetric.TrendInterpretation == TrendInterpretation.Standard)
                        Assert.AreEqual(granularMetric.Trend.Evaluation, TrendEvaluation.UpGood);

                    if (suppliedTrendDirection == TrendDirection.None && (TrendInterpretation)suppliedMetric.TrendInterpretation == TrendInterpretation.Standard)
                        Assert.AreEqual(granularMetric.Trend.Evaluation, TrendEvaluation.None);

                    if (suppliedTrendDirection == TrendDirection.Unchanged && (TrendInterpretation)suppliedMetric.TrendInterpretation == TrendInterpretation.Standard)
                        Assert.AreEqual(granularMetric.Trend.Evaluation, TrendEvaluation.NoChangeNoOpinion);

                    if (suppliedTrendDirection == TrendDirection.Decreasing && (TrendInterpretation)suppliedMetric.TrendInterpretation == TrendInterpretation.Inverse)
                        Assert.AreEqual(granularMetric.Trend.Evaluation, TrendEvaluation.DownGood);

                    if (suppliedTrendDirection == TrendDirection.Increasing && (TrendInterpretation)suppliedMetric.TrendInterpretation == TrendInterpretation.Inverse)
                        Assert.AreEqual(granularMetric.Trend.Evaluation, TrendEvaluation.UpBad);

                    if (suppliedTrendDirection == TrendDirection.None && (TrendInterpretation)suppliedMetric.TrendInterpretation == TrendInterpretation.Inverse)
                        Assert.AreEqual(granularMetric.Trend.Evaluation, TrendEvaluation.None);

                    if (suppliedTrendDirection == TrendDirection.Unchanged && (TrendInterpretation)suppliedMetric.TrendInterpretation == TrendInterpretation.Inverse)
                        Assert.AreEqual(granularMetric.Trend.Evaluation, TrendEvaluation.NoChangeNoOpinion);
                }
            }
        }

        private GranularMetric<T> DownCastToGranularMetric<T>(T value, MetricBase metric)
        {
            return metric as GranularMetric<T>;
        }
    }
}
