// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System.Collections.Generic;
using System.Linq;
using EdFi.Dashboards.Data.Repository;
using EdFi.Dashboards.Metric.Data.Entities;
using EdFi.Dashboards.Metric.Resources.Models;
using EdFi.Dashboards.Metric.Resources.Services;
using EdFi.Dashboards.Testing;
using NUnit.Framework;
using Rhino.Mocks;
using MetricType = EdFi.Dashboards.Metric.Resources.Models.MetricType;

namespace EdFi.Dashboards.Metric.Resources.Tests.Services
{
    [TestFixture]
    public class When_calling_the_get_from_metric_metadata_tree_service : TestFixtureBase
    {
        MetricMetadataTree actualModel;
        private IRepository<Metric.Data.Entities.Metric> metricRepository;
        private IRepository<Metric.Data.Entities.MetricVariant> metricVariantRepository;
        private IRepository<MetricNode> metricNodeRepository;
        private IRepository<Metric.Data.Entities.MetricState> metricStateRepository;
        private IRepository<Metric.Data.Entities.MetricAction> metricActionRepository;
        private IRepository<DomainEntityType> domainEntityTypeRepository;

        protected override void EstablishContext()
        {
            //mock data Repositories
            metricRepository = mocks.StrictMock<IRepository<Metric.Data.Entities.Metric>>();
            Expect.Call(metricRepository.GetAll()).Return(getSuppliedMetrics());

            metricVariantRepository = mocks.StrictMock<IRepository<Metric.Data.Entities.MetricVariant>>();
            Expect.Call(metricVariantRepository.GetAll()).Return(getSuppliedMetricVariants());

            metricNodeRepository = mocks.StrictMock<IRepository<MetricNode>>();
            Expect.Call(metricNodeRepository.GetAll()).Return(getSuppliedMetricNodeData());

            metricStateRepository = mocks.StrictMock<IRepository<Metric.Data.Entities.MetricState>>();
            Expect.Call(metricStateRepository.GetAll()).Return(getSuppliedMetricStateData());

            metricActionRepository = mocks.StrictMock<IRepository<Metric.Data.Entities.MetricAction>>();
            Expect.Call(metricActionRepository.GetAll()).Return(getSuppliedMetricActionData());

            domainEntityTypeRepository = mocks.StrictMock<IRepository<DomainEntityType>>();
            Expect.Call(domainEntityTypeRepository.GetAll()).Return(getSuppliedDomainEntityTypes());
        }

        protected override void ExecuteTest()
        {
            var service = new MetricMetadataTreeService(metricRepository, metricVariantRepository, metricNodeRepository, metricStateRepository, metricActionRepository, domainEntityTypeRepository);
            actualModel = service.Get(MetricMetadataTreeRequest.Create());
        }

        private IQueryable<DomainEntityType> getSuppliedDomainEntityTypes()
        {
            return (new List<DomainEntityType>
                        {
                            new DomainEntityType
                                {
                                    DomainEntityTypeId = 1,
                                    DomainEntityTypeName = "Domain Entity Type One"
                                },
                            new DomainEntityType
                                {
                                    DomainEntityTypeId = 2,
                                    DomainEntityTypeName = "Domain Entity Type Two"
                                },
                            new DomainEntityType
                                {
                                    DomainEntityTypeId = 3,
                                    DomainEntityTypeName = "Domain Entity Type Three"
                                },
                            new DomainEntityType
                                {
                                    DomainEntityTypeId = 4,
                                    DomainEntityTypeName = "Domain Entity Type Four"
                                }
                        }).AsQueryable();
        }
        private IQueryable<Metric.Data.Entities.MetricState> getSuppliedMetricStateData()
        {
            return (new List<Metric.Data.Entities.MetricState>
                        {
                            new Metric.Data.Entities.MetricState
                                {
                                    MetricId = 100, 
                                    MetricStateTypeId = 1, 
                                    MinValue = 1, 
                                    MaxValue=21, 
                                    StateText = "StateText100", 
                                    IsMaxValueInclusive = -1, 
                                    IsMinValueInclusive = -1, 
                                    Format = "Format100"
                                },
                            new Metric.Data.Entities.MetricState
                                {
                                    MetricId = 200, 
                                    MetricStateTypeId = 2, 
                                    MinValue = 1, 
                                    MaxValue=22, 
                                    StateText = "StateText200", 
                                    IsMaxValueInclusive = -1, 
                                    IsMinValueInclusive = -1, 
                                    Format = "Format200"
                                },
                            new Metric.Data.Entities.MetricState
                                {
                                    MetricId = 300, 
                                    MetricStateTypeId = 2, 
                                    MinValue = 1, 
                                    MaxValue=25, 
                                    StateText = "StateText300", 
                                    IsMaxValueInclusive = -1, 
                                    IsMinValueInclusive = -1, 
                                    Format = "Format300"
                                },
                            new Metric.Data.Entities.MetricState
                                {
                                    MetricId = 1100, 
                                    MetricStateTypeId = 4, 
                                    MinValue = 1, 
                                    MaxValue=31, 
                                    StateText = "StateText1100", 
                                    IsMaxValueInclusive = -1, 
                                    IsMinValueInclusive = -1, 
                                    Format = "Format1100"
                                },
                        }).AsQueryable();
        }

        private IQueryable<Metric.Data.Entities.MetricAction> getSuppliedMetricActionData()
        {
            return (new List<Metric.Data.Entities.MetricAction>
                        {
                            new Metric.Data.Entities.MetricAction
                                {
                                    MetricVariantId = 10099, 
                                    MetricActionTypeId = 1, 
                                    Title = "Title100", 
                                    Tooltip = "tooltip100", 
                                    Url="Url100", 
                                    DrilldownHeader = "DDHeader100", 
                                    DrilldownFooter = "DDFooter100"
                                },
                            new Metric.Data.Entities.MetricAction
                                {
                                    MetricVariantId = 30099, 
                                    MetricActionTypeId = 2, 
                                    Title = "Title300", 
                                    Tooltip = "tooltip300", 
                                    Url="Url300", 
                                    DrilldownHeader = "DDHeader300", 
                                    DrilldownFooter = "DDFooter300"
                                },
                            new Metric.Data.Entities.MetricAction
                                {
                                    MetricVariantId = 70099, 
                                    MetricActionTypeId = 1, 
                                    Title = "Title700", 
                                    Tooltip = "tooltip700", 
                                    Url="Url700", 
                                    DrilldownHeader = "DDHeader700", 
                                    DrilldownFooter = "DDFooter700"
                                }

                        }).AsQueryable();
        }
        private IQueryable<Metric.Data.Entities.Metric> getSuppliedMetrics()
        {
            return (new List<Metric.Data.Entities.Metric>
                        {
                            new Metric.Data.Entities.Metric
                                {
                                    MetricId = 100,
                                    //DomainEntityChildMetricId = 1, 
                                    Enabled = true, 
                                    MetricName = "one", 
                                    MetricTypeId = 1, 
                                    DomainEntityTypeId = null,
                                    TrendInterpretation = 1
                                },
                            new Metric.Data.Entities.Metric
                                {
                                    MetricId = 200,
                                    //DomainEntityChildMetricId = 2, 
                                    Enabled = true, 
                                    MetricName = "two", 
                                    MetricTypeId = 2, 
                                    DomainEntityTypeId = 2,
                                    TrendInterpretation = -1
                                },
                            new Metric.Data.Entities.Metric
                                {
                                    MetricId = 300,
                                    //DomainEntityChildMetricId = 3, 
                                    Enabled = true, 
                                    MetricName = "three", 
                                    MetricTypeId = 3, 
                                    DomainEntityTypeId = 3,
                                    TrendInterpretation = 1
                                },
                            new Metric.Data.Entities.Metric
                                {
                                    MetricId = 400,
                                    //DomainEntityChildMetricId = 3, 
                                    Enabled = true, 
                                    MetricName = "four", 
                                    MetricTypeId = 5, 
                                    DomainEntityTypeId = 4,
                                    TrendInterpretation = -1
                                },
                            new Metric.Data.Entities.Metric
                                {
                                    MetricId = 500,
                                    //DomainEntityChildMetricId = 3, 
                                    Enabled = true, 
                                    MetricName = "five", 
                                    MetricTypeId = 4, 
                                    DomainEntityTypeId = 1,
                                    TrendInterpretation = -1
                                },
                            new Metric.Data.Entities.Metric
                                {
                                    MetricId = 600,
                                    //DomainEntityChildMetricId = 3, 
                                    Enabled = true, 
                                    MetricName = "six", 
                                    MetricTypeId = 1, 
                                    DomainEntityTypeId = 1,
                                    TrendInterpretation = -1
                                },
                            new Metric.Data.Entities.Metric
                                {
                                    MetricId = 700,
                                    //DomainEntityChildMetricId = 3, 
                                    Enabled = true, 
                                    MetricName = "seven", 
                                    MetricTypeId = 1,  
                                    DomainEntityTypeId = 1,
                                    TrendInterpretation = -1
                                },
                            new Metric.Data.Entities.Metric
                                {
                                    MetricId = 800,
                                    //DomainEntityChildMetricId = 3, 
                                    Enabled = true, 
                                    MetricName = "eight", 
                                    MetricTypeId = 4, 
                                    DomainEntityTypeId = 1,
                                    TrendInterpretation = -1
                                },
                            new Metric.Data.Entities.Metric
                                {
                                    MetricId = 900,
                                    //DomainEntityChildMetricId = 3, 
                                    Enabled = true, 
                                    MetricName = "nine", 
                                    MetricTypeId = 1, 
                                    DomainEntityTypeId = 1,
                                    TrendInterpretation = -1
                                },
                            new Metric.Data.Entities.Metric
                                {
                                    MetricId = 1000,
                                    //DomainEntityChildMetricId = 3, 
                                    Enabled = true, 
                                    MetricName = "ten", 
                                    MetricTypeId = 1, 
                                    DomainEntityTypeId = 1,
                                    TrendInterpretation = -1
                                },
                            new Metric.Data.Entities.Metric
                                {
                                    MetricId = 1100,
                                    //DomainEntityChildMetricId = 3, 
                                    Enabled = true, 
                                    MetricName = "eleven", 
                                    MetricTypeId = 1, 
                                    DomainEntityTypeId = 1,
                                    TrendInterpretation = -1
                                },
                            new Metric.Data.Entities.Metric
                                {
                                    MetricId = 1200,
                                    //DomainEntityChildMetricId = 3, 
                                    Enabled = true, 
                                    MetricName = "twelve", 
                                    MetricTypeId = 1, 
                                    DomainEntityTypeId = 1,
                                    TrendInterpretation = -1
                                },
                            new Metric.Data.Entities.Metric
                                {
                                    MetricId = 1300,
                                    //DomainEntityChildMetricId = 3, 
                                    Enabled = true, 
                                    MetricName = "thirteen", 
                                    MetricTypeId = 1, 
                                    DomainEntityTypeId = 1,
                                    TrendInterpretation = -1
                                },
                            new Metric.Data.Entities.Metric
                                {
                                    MetricId = 1400,
                                    //DomainEntityChildMetricId = 3, 
                                    Enabled = true, 
                                    MetricName = "fourteen", 
                                    MetricTypeId = 1, 
                                    DomainEntityTypeId = 1,
                                    TrendInterpretation = -1
                                },
                            new Metric.Data.Entities.Metric
                                {
                                    MetricId = 1500,
                                    //DomainEntityChildMetricId = 3, 
                                    Enabled = true, 
                                    MetricName = "fifteen", 
                                    MetricTypeId = 1, 
                                    DomainEntityTypeId = 1,
                                    TrendInterpretation = -1
                                },
                            new Metric.Data.Entities.Metric
                                {
                                    MetricId = 1600,
                                    //DomainEntityChildMetricId = 3, 
                                    Enabled = true, 
                                    MetricName = "sixteen", 
                                    MetricTypeId = 1, 
                                    DomainEntityTypeId = 1,
                                    TrendInterpretation = -1
                                },
                            new Metric.Data.Entities.Metric
                                {
                                    MetricId = 1700,
                                    //DomainEntityChildMetricId = 3, 
                                    Enabled = true, 
                                    MetricName = "seventeen", 
                                    MetricTypeId = 1, 
                                    DomainEntityTypeId = 1,
                                    TrendInterpretation = -1
                                },
                        }).AsQueryable();
        }
        private IQueryable<Metric.Data.Entities.MetricVariant> getSuppliedMetricVariants()
        {
            return (new List<Metric.Data.Entities.MetricVariant>
                        {
                            new Metric.Data.Entities.MetricVariant
                                {
                                    MetricId = 100,
                                    MetricVariantId = 10099,
                                    MetricVariantTypeId = 1,
                                    Enabled = true, 
                                    Format = "{0:0.00}", 
                                    ListFormat = "", 
                                    MetricDescription = "MetricDesc1", 
                                    MetricName = "one", 
                                    MetricShortName = "ShortName", 
                                    MetricTooltip = "ToolTip1",
                                    MetricUrl="url1",
                                    NumeratorDenominatorFormat = "NDF1",
                                },
                            new Metric.Data.Entities.MetricVariant
                                {
                                    MetricId = 200,
                                    MetricVariantId = 20099,
                                    MetricVariantTypeId = 1,
                                    Enabled = true, 
                                    Format = "{0:2.00}", 
                                    ListFormat = "", 
                                    MetricDescription = "MetricDesc2", 
                                    MetricName = "two", 
                                    MetricShortName = "ShortName2", 
                                    MetricTooltip = "ToolTip2",
                                    MetricUrl="url2",
                                    NumeratorDenominatorFormat = "NDF2",
                                },
                            new Metric.Data.Entities.MetricVariant
                                {
                                    MetricId = 300,
                                    MetricVariantId = 30099,
                                    MetricVariantTypeId = 1,
                                    Enabled = true, 
                                    Format = "{0:3.00}", 
                                    ListFormat = "", 
                                    MetricDescription = "MetricDesc3", 
                                    MetricName = "three", 
                                    MetricShortName = "ShortName3", 
                                    MetricTooltip = "ToolTip3",
                                    MetricUrl="url3",
                                    NumeratorDenominatorFormat = "NDF3",
                                },
                            new Metric.Data.Entities.MetricVariant
                                {
                                    MetricId = 400,
                                    MetricVariantId = 40099,
                                    MetricVariantTypeId = 1,
                                    Enabled = true, 
                                    Format = "{0:4.00}", 
                                    ListFormat = "", 
                                    MetricDescription = "MetricDesc4", 
                                    MetricName = "four", 
                                    MetricShortName = "ShortName4", 
                                    MetricTooltip = "ToolTip4",
                                    MetricUrl="url4",
                                    NumeratorDenominatorFormat = "NDF4",
                                },
                            new Metric.Data.Entities.MetricVariant
                                {
                                    MetricId = 500,
                                    MetricVariantId = 50099,
                                    MetricVariantTypeId = 1,
                                    Enabled = true, 
                                    Format = "{0:5.00}", 
                                    ListFormat = "", 
                                    MetricDescription = "MetricDesc5", 
                                    MetricName = "five", 
                                    MetricShortName = "ShortName5", 
                                    MetricTooltip = "ToolTip5",
                                    MetricUrl="url5",
                                    NumeratorDenominatorFormat = "NDF5",
                                },
                            new Metric.Data.Entities.MetricVariant
                                {
                                    MetricId = 600,
                                    MetricVariantId = 60099,
                                    MetricVariantTypeId = 1,
                                    Enabled = true, 
                                    Format = "{0:6.00}", 
                                    ListFormat = "", 
                                    MetricDescription = "MetricDesc6", 
                                    MetricName = "six", 
                                    MetricShortName = "ShortName6", 
                                    MetricTooltip = "ToolTip6",
                                    MetricUrl="url6",
                                    NumeratorDenominatorFormat = "NDF6",
                                },
                            new Metric.Data.Entities.MetricVariant
                                {
                                    MetricId = 700,
                                    MetricVariantId = 70099,
                                    MetricVariantTypeId = 1,
                                    Enabled = true, 
                                    Format = "{0:7.00}", 
                                    ListFormat = "", 
                                    MetricDescription = "MetricDes7", 
                                    MetricName = "seven", 
                                    MetricShortName = "ShortName7", 
                                    MetricTooltip = "ToolTip7",
                                    MetricUrl="url7",
                                    NumeratorDenominatorFormat = "NDF7",
                                },
                            new Metric.Data.Entities.MetricVariant
                                {
                                    MetricId = 800,
                                    MetricVariantId = 80099,
                                    MetricVariantTypeId = 1,
                                    Enabled = true, 
                                    Format = "{0:3.00}", 
                                    ListFormat = "", 
                                    MetricDescription = "MetricDesc8", 
                                    MetricName = "eight", 
                                    MetricShortName = "ShortName8", 
                                    MetricTooltip = "ToolTip8",
                                    MetricUrl="url8",
                                    NumeratorDenominatorFormat = "NDF8",
                                },
                            new Metric.Data.Entities.MetricVariant
                                {
                                    MetricId = 900,
                                    MetricVariantId = 90099,
                                    MetricVariantTypeId = 1,
                                    Enabled = true, 
                                    Format = "{0:9.00}", 
                                    ListFormat = "", 
                                    MetricDescription = "MetricDesc9", 
                                    MetricName = "nine", 
                                    MetricShortName = "ShortName9", 
                                    MetricTooltip = "ToolTip9",
                                    MetricUrl="url9",
                                    NumeratorDenominatorFormat = "NDF9",
                                },
                            new Metric.Data.Entities.MetricVariant
                                {
                                    MetricId = 1000,
                                    MetricVariantId = 100099,
                                    MetricVariantTypeId = 1,
                                    Enabled = true, 
                                    Format = "{0:10.00}", 
                                    ListFormat = "", 
                                    MetricDescription = "MetricDesc10", 
                                    MetricName = "ten", 
                                    MetricShortName = "ShortName10", 
                                    MetricTooltip = "ToolTip10",
                                    MetricUrl="url10",
                                    NumeratorDenominatorFormat = "NDF10",
                                },
                            new Metric.Data.Entities.MetricVariant
                                {
                                    MetricId = 1100,
                                    MetricVariantId = 110099,
                                    MetricVariantTypeId = 1,
                                    Enabled = true, 
                                    Format = "{0:11.00}", 
                                    ListFormat = "", 
                                    MetricDescription = "MetricDesc11", 
                                    MetricName = "eleven", 
                                    MetricShortName = "ShortName11", 
                                    MetricTooltip = "ToolTip11",
                                    MetricUrl="url11",
                                    NumeratorDenominatorFormat = "NDF11",
                                },
                            new Metric.Data.Entities.MetricVariant
                                {
                                    MetricId = 1200,
                                    MetricVariantId = 120099,
                                    MetricVariantTypeId = 1,
                                    Enabled = true, 
                                    Format = "{0:12.00}", 
                                    ListFormat = "", 
                                    MetricDescription = "MetricDesc12", 
                                    MetricName = "twelve", 
                                    MetricShortName = "ShortName12", 
                                    MetricTooltip = "ToolTip12",
                                    MetricUrl="url12",
                                    NumeratorDenominatorFormat = "NDF12",
                                },
                            new Metric.Data.Entities.MetricVariant
                                {
                                    MetricId = 1300,
                                    MetricVariantId = 130099,
                                    MetricVariantTypeId = 1,
                                    Enabled = true, 
                                    Format = "{0:13.00}", 
                                    ListFormat = "List Format 13", 
                                    ListDataLabel = "List Data Label 13",
                                    MetricDescription = "MetricDesc13", 
                                    MetricName = "thirteen", 
                                    MetricShortName = "ShortName13", 
                                    MetricTooltip = "ToolTip13",
                                    MetricUrl="url13",
                                    NumeratorDenominatorFormat = "NDF13",
                                },
                            new Metric.Data.Entities.MetricVariant
                                {
                                    MetricId = 1400,
                                    MetricVariantId = 140099,
                                    MetricVariantTypeId = 1,
                                    Enabled = true, 
                                    Format = "{0:14.00}", 
                                    ListFormat = "", 
                                    MetricDescription = "MetricDesc14", 
                                    MetricName = "fourteen", 
                                    MetricShortName = "ShortName14", 
                                    MetricTooltip = "ToolTip14",
                                    MetricUrl="url14",
                                    NumeratorDenominatorFormat = "NDF14",
                                },
                            new Metric.Data.Entities.MetricVariant
                                {
                                    MetricId = 1500, 
                                    MetricVariantId = 150099,
                                    MetricVariantTypeId = 1,
                                    Enabled = true, 
                                    Format = "{0:15.00}", 
                                    ListFormat = "", 
                                    MetricDescription = "MetricDesc15", 
                                    MetricName = "fifteen", 
                                    MetricShortName = "ShortName15", 
                                    MetricTooltip = "ToolTip15",
                                    MetricUrl="url15",
                                    NumeratorDenominatorFormat = "NDF15",
                                },
                            new Metric.Data.Entities.MetricVariant
                                {
                                    MetricId = 1600,
                                    MetricVariantId = 160099,
                                    MetricVariantTypeId = 1,
                                    Enabled = true, 
                                    Format = "{0:16.00}", 
                                    ListFormat = "", 
                                    MetricDescription = "MetricDesc16", 
                                    MetricName = "sixteen", 
                                    MetricShortName = "ShortName16", 
                                    MetricTooltip = "ToolTip16",
                                    MetricUrl="url16",
                                    NumeratorDenominatorFormat = "NDF16",
                                },
                            new Metric.Data.Entities.MetricVariant
                                {
                                    MetricId = 1700,
                                    MetricVariantId = 170099,
                                    MetricVariantTypeId = 1,
                                    Enabled = true, 
                                    Format = "{0:17.00}", 
                                    ListFormat = "", 
                                    MetricDescription = "MetricDesc17", 
                                    MetricName = "seventeen", 
                                    MetricShortName = "ShortName17", 
                                    MetricTooltip = "ToolTip17",
                                    MetricUrl="url17",
                                    NumeratorDenominatorFormat = "NDF17",
                                },
                        }).AsQueryable();
        }

        private IQueryable<MetricNode> getSuppliedMetricNodeData()
        {
            return (new List<MetricNode>
                        {
                            //three roots
                            new MetricNode{DisplayName = string.Empty, DisplayOrder = 10, MetricId = 100, MetricVariantId = 10099, MetricNodeId = 1, ParentNodeId = null, RootNodeId = 1},
                            new MetricNode{DisplayName = string.Empty, DisplayOrder = 20, MetricId = 200, MetricVariantId = 20099, MetricNodeId = 2, ParentNodeId = null, RootNodeId = 2},
                            new MetricNode{DisplayName = string.Empty, DisplayOrder = 30, MetricId = 300, MetricVariantId = 30099, MetricNodeId = 3, ParentNodeId = null, RootNodeId = 3},
                            //children of the roots
                            //---first metric children - 3
                            new MetricNode{DisplayName = string.Empty, DisplayOrder = 10, MetricId = 400, MetricVariantId = 40099, MetricNodeId = 4, ParentNodeId = 1, RootNodeId = 1},
                            new MetricNode{DisplayName = string.Empty, DisplayOrder = 30, MetricId = 500, MetricVariantId = 50099, MetricNodeId = 5, ParentNodeId = 1, RootNodeId = 1},
                            new MetricNode{DisplayName = string.Empty, DisplayOrder = 20, MetricId = 600, MetricVariantId = 60099, MetricNodeId = 6, ParentNodeId = 1, RootNodeId = 1},
                            new MetricNode{DisplayName = "Override Display Name 1", DisplayOrder = 40, MetricId = 1700, MetricVariantId = 170099, MetricNodeId = 17, ParentNodeId = 1, RootNodeId = 1},
                            //---second metric children - 3
                            new MetricNode{DisplayName = string.Empty, DisplayOrder = 30, MetricId = 700, MetricVariantId = 70099, MetricNodeId = 7, ParentNodeId = 2, RootNodeId = 2},
                            new MetricNode{DisplayName = string.Empty, DisplayOrder = 10, MetricId = 800, MetricVariantId = 80099, MetricNodeId = 8, ParentNodeId = 2, RootNodeId = 2},
                            new MetricNode{DisplayName = string.Empty, DisplayOrder = 20, MetricId = 900, MetricVariantId = 90099, MetricNodeId = 9, ParentNodeId = 2, RootNodeId = 2},
                            new MetricNode{DisplayName = string.Empty, DisplayOrder = 40, MetricId = 1700, MetricVariantId = 170099, MetricNodeId = 18, ParentNodeId = 2, RootNodeId = 2},
                            //---third metric children - 3
                            new MetricNode{DisplayName = string.Empty, DisplayOrder = 30, MetricId = 1000, MetricVariantId = 100099, MetricNodeId = 10, ParentNodeId = 3, RootNodeId = 3},
                            new MetricNode{DisplayName = string.Empty, DisplayOrder = 20, MetricId = 1100, MetricVariantId = 110099, MetricNodeId = 11, ParentNodeId = 3, RootNodeId = 3},
                            new MetricNode{DisplayName = string.Empty, DisplayOrder = 10, MetricId = 1200, MetricVariantId = 120099, MetricNodeId = 12, ParentNodeId = 3, RootNodeId = 3},
                            new MetricNode{DisplayName = null, DisplayOrder = 40, MetricId = 1700, MetricVariantId = 170099, MetricNodeId = 19, ParentNodeId = 3, RootNodeId = 3},
                            //next level down children
                            new MetricNode{DisplayName = string.Empty, DisplayOrder = 10, MetricId = 1300, MetricVariantId = 130099, MetricNodeId = 13, ParentNodeId = 4, RootNodeId = 1},
                            new MetricNode{DisplayName = string.Empty, DisplayOrder = 10, MetricId = 1400, MetricVariantId = 140099, MetricNodeId = 14, ParentNodeId = 8, RootNodeId = 2},
                            new MetricNode{DisplayName = string.Empty, DisplayOrder = 20, MetricId = 1500, MetricVariantId = 150099, MetricNodeId = 15, ParentNodeId = 8, RootNodeId = 2},
                            new MetricNode{DisplayName = string.Empty, DisplayOrder = 10, MetricId = 1600, MetricVariantId = 160099, MetricNodeId = 16, ParentNodeId = 12, RootNodeId = 3}
                        }).AsQueryable();
        }

        [Test]
        public void Should_return_model_that_is_not_null()
        {
            Assert.That(actualModel, Is.Not.Null);
        }

        [Test]
        public void Should_return_model_has_nodes_in_list()
        {
            Assert.That(actualModel.Children.Count(), Is.Not.EqualTo(0));
        }

        [Test]
        public void Should_return_model_root_with_correct_data()
        {
            //the list should contain only three objects based on the supplied data, they are the root nodes
            Assert.That(actualModel.Children.Count(), Is.EqualTo(3));

            foreach (var metricMetadataNode in actualModel.Descendants)
            {
                Assert.NotNull(getSuppliedMetricNodeData().Where(x => x.MetricId == metricMetadataNode.MetricId));
            }
        }

        [Test]
        public void Should_return_model_with_children_nodes_correctly()
        {
            //check branch
            Assert.That(actualModel.Children.ToList()[0].Children.ToList()[0], Is.Not.Null);
            Assert.That(actualModel.Children.ToList()[0].Children.ToList()[0].MetricId, Is.EqualTo(400));
            Assert.That(actualModel.Children.ToList()[0].Children.ToList()[0].Children.ToList()[0].MetricId, Is.EqualTo(1300));
            //Check a separate branch
            Assert.That(actualModel.Children.ToList()[1].Children.ToList()[0], Is.Not.Null);
            Assert.That(actualModel.Children.ToList()[1].Children.ToList()[0].MetricId, Is.EqualTo(800));
            Assert.That(actualModel.Children.ToList()[1].Children.ToList()[0].Children.ToList()[0].MetricId, Is.EqualTo(1400));
            Assert.That(actualModel.Children.ToList()[1].Children.ToList()[0].Children.ToList()[1].MetricId, Is.EqualTo(1500));
            //check third branch
            Assert.That(actualModel.Children.ToList()[2].Children.ToList()[0], Is.Not.Null);
            Assert.That(actualModel.Children.ToList()[2].Children.ToList()[0].MetricId, Is.EqualTo(1200));
            Assert.That(actualModel.Children.ToList()[2].Children.ToList()[1].MetricId, Is.EqualTo(1100));
            Assert.That(actualModel.Children.ToList()[2].Children.ToList()[0].Children.ToList()[0].MetricId, Is.EqualTo(1600));
        }

        [Test]
        public void Should_apply_display_name_and_metric_name_properly()
        {
            Assert.That(actualModel.Children.ElementAt(0).Children.Last().DisplayName, Is.EqualTo("Override Display Name 1"));
            Assert.That(actualModel.Children.ElementAt(1).Children.Last().DisplayName, Is.EqualTo("seventeen"));
            Assert.That(actualModel.Children.ElementAt(2).Children.Last().DisplayName, Is.EqualTo("seventeen"));
        }


        // TODO: Deferred - To be revisited because of the test extension method crawling up the parent is an issue, also look into NBuilder to fill full object
        //[Test]
        //public void Should_have_all_properties_assigned_a_value()
        //{

        //    foreach (var child in actualModel.Children)
        //    {
        //        child.EnsureNoDefaultValues(
        //            "MetricMetadataNode.ParentNodeId",
        //            "MetricMetadataNode.MetricMetadataNodes[0].MetricMetadataNodes",
        //            "MetricMetadataNode.MetricMetadataNodes[0].Actions",
        //            "MetricMetadataNode.MetricMetadataNodes[0].States",
        //            "MetricMetadataNode.Actions",
        //            "MetricMetadataNode.States",
        //            "MetricMetadataNode.DomainEntityChildMetricId",
        //            "MetricMetadataNode.MetricMetadataNodes[0].DomainEntityChildMetricId",
        //            "MetricMetadataNode.MetricMetadataNodes[0].MetricMetadataNodes[0].DomainEntityChildMetricId",
        //            "MetricMetadataNode.MetricMetadataNodes[0].MetricMetadataNodes[0].MetricMetadataNodes",
        //            "MetricMetadataNode.MetricMetadataNodes[0].MetricMetadataNodes[0].Actions",
        //            "MetricMetadataNode.MetricMetadataNodes[0].MetricMetadataNodes[0].States",
        //            "MetricMetadataNode.MetricMetadataNodes[1].DomainEntityChildMetricId",
        //            "MetricMetadataNode.MetricMetadataNodes[1].MetricMetadataNodes",
        //            "MetricMetadataNode.MetricMetadataNodes[1].Actions",
        //            "MetricMetadataNode.MetricMetadataNodes[1].States",
        //            "MetricMetadataNode.MetricMetadataNodes[2].DomainEntityChildMetricId",
        //            "MetricMetadataNode.MetricMetadataNodes[2].MetricMetadataNodes",
        //            "MetricMetadataNode.MetricMetadataNodes[2].Actions",
        //            "MetricMetadataNode.MetricMetadataNodes[2].States",
        //            "MetricMetadataNode.TrendEvaluation",
        //            "MetricMetadataNode.MetricMetadataNodes[0].TrendEvaluation",
        //            "MetricMetadataNode.MetricMetadataNodes[0].MetricMetadataNodes[0].TrendEvaluation",
        //            "MetricMetadataNode.MetricMetadataNodes[1].TrendEvaluation",
        //            "MetricMetadataNode.MetricMetadataNodes[2].TrendEvaluation");
        //    }
        //}

        [Test]
        public void Should_have_at_least_one_metric_with_a_metric_action()
        {
            var foundAction = false;
            foreach (var metricMetadataNode in actualModel.Descendants)
            {
                if (metricMetadataNode.Actions.ToList().Count != 0)
                    foundAction = true;
            }
            Assert.That(foundAction, Is.True);
        }

        [Test]
        public void Should_return_a_model_with_at_least_one_metric_with_a_metric_state()
        {
            var foundAction = false;
            foreach (var metricMetadataNode in actualModel.Descendants)
            {
                if (metricMetadataNode.States.Count() != 0)
                    foundAction = true;
            }
            Assert.That(foundAction, Is.True);
        }

        [Test]
        public void Should_return_the_correct_metric_actions()
        {
            var foundAction = false;
            foreach (var metricMetadataNode in actualModel.Descendants)
            {
                if (metricMetadataNode.MetricId == 100)
                    foundAction = metricMetadataNode.Actions.ToList()[0].Title == "Title100";
            }
            Assert.That(foundAction, Is.True);
        }

        [Test]
        public void Should_return_the_correct_metric_states()
        {
            var foundState = false;
            foreach (var metricMetadataNode in actualModel.Descendants)
            {
                if (metricMetadataNode.MetricId == 100)
                {
                    Assert.That(metricMetadataNode.States.Count(), Is.GreaterThan(0));
                    foundState = metricMetadataNode.States.ToList()[0].StateText == "StateText100";
                }
            }
            Assert.That(foundState, Is.True);
        }

        [Test]
        public void Should_return_actions_not_null()
        {
            Assert.That(actualModel.Children.ToList()[0].Actions, Is.Not.Null);
        }

        [Test]
        public void Should_return_states_not_null()
        {
            Assert.That(actualModel.Children.ToList()[0].States, Is.Not.Null);
        }

        [Test]
        public void Should_be_able_to_return_one_node_with_linq()
        {
            var result = from m in actualModel.Descendants
                         where m.RootNodeId == 1 && m.MetricId == 1300
                         select m;

            Assert.IsNotNull(result);
        }

        [Test]
        public void Should_return_transformed_metric_type_id_correctly()
        {
            Assert.AreEqual(actualModel.Children.ToList()[1].Children.ToList()[0].MetricType, MetricType.AggregateMetric);
        }

        [Test]
        public void Should_return_proper_domain_entity_type()
        {
            Assert.IsTrue(actualModel.Descendants.Where(x => x.MetricNodeId == 1).Select(x => x.DomainEntityType).SingleOrDefault() == "");
            Assert.IsTrue(actualModel.Descendants.Where(x => x.MetricNodeId == 2).Select(x => x.DomainEntityType).SingleOrDefault() == "Domain Entity Type Two");
            Assert.IsTrue(actualModel.Descendants.Where(x => x.MetricNodeId == 3).Select(x => x.DomainEntityType).SingleOrDefault() == "Domain Entity Type Three");
            Assert.IsTrue(actualModel.Descendants.Where(x => x.MetricNodeId == 4).Select(x => x.DomainEntityType).SingleOrDefault() == "Domain Entity Type Four");
        }

        [Test]
        public void Should_have_correct_parent_references()
        {
            Assert.AreEqual(actualModel.Children.ToList()[0], actualModel.Children.ToList()[0].Children.ToList()[0].Parent);
        }
    }
}
