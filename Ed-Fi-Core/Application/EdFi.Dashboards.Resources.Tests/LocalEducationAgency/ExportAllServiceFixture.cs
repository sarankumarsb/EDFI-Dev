// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System.Collections.Generic;
using System.Linq;
using EdFi.Dashboards.Data.Entities;
using EdFi.Dashboards.Data.Repository;
using EdFi.Dashboards.Metric.Resources.Models;
using EdFi.Dashboards.Resources.Common;
using EdFi.Dashboards.Resources.LocalEducationAgency;
using EdFi.Dashboards.Resources.Metric;
using EdFi.Dashboards.Resources.Metric.Requests;
using EdFi.Dashboards.Resources.Models.Common;
using EdFi.Dashboards.Testing;
using NUnit.Framework;
using Rhino.Mocks;

namespace EdFi.Dashboards.Resources.Tests.LocalEducationAgency
{
    public abstract class When_exporting_all_local_education_agency_metrics : TestFixtureBase
    {
        //The Injected Dependencies.
        private IRepository<LocalEducationAgencyInformation> localEducationAgencyInformationRepository;
        private IDomainMetricService<LocalEducationAgencyMetricInstanceSetRequest> domainMetricService;
        private IRootMetricNodeResolver rootMetricNodeResolver;
        private IMetricTreeToIEnumerableOfKeyValuePairProvider metricTreeToIEnumerableOfKeyValuePairProvider;
        private IDomainSpecificMetricNodeResolver domainSpecificMetricNodeResolver;

        //The Actual Model.
        protected ExportAllModel actualModel;

        //The supplied Data models.
        private const int suppliedLocalEducationAgencyId = 1;
        protected IQueryable<LocalEducationAgencyInformation> suppliedLocalEducationAgencyInformationData;
        
        //For root
        private const int suppliedRootMetricId = 2;
        private const int suppliedRootMetricVariantId = 2999;
        protected MetricMetadataNode suppliedRootMetricNode;
        protected MetricTree suppliedRootMetricHierarchy;
        protected IEnumerable<KeyValuePair<string, object>> suppliedRootIEnumerableOfKeyValuePair;
        
        //For Operational Dashboard
        private const int suppliedOperationalDashboardMetricId = 15;
        private const int suppliedOperationalDashboardMetricVariantId = 15999;
        protected MetricMetadataNode suppliedOperationalDashboardMetricNode;
        protected MetricTree suppliedOperationalDashboardMetricHierarchy;
        protected IEnumerable<KeyValuePair<string, object>> suppliedOperationalDashboardIEnumerableOfKeyValuePair;
        
        protected override void EstablishContext()
        {
            InitializeSuppliedData();

            //Set up the mocks
            localEducationAgencyInformationRepository = mocks.StrictMock<IRepository<LocalEducationAgencyInformation>>();
            domainMetricService = mocks.StrictMock<IDomainMetricService<LocalEducationAgencyMetricInstanceSetRequest>>();
            rootMetricNodeResolver = mocks.StrictMock<IRootMetricNodeResolver>();
            metricTreeToIEnumerableOfKeyValuePairProvider = mocks.StrictMock<IMetricTreeToIEnumerableOfKeyValuePairProvider>();
            domainSpecificMetricNodeResolver = mocks.StrictMock<IDomainSpecificMetricNodeResolver>();
    
            //Set expectations
            Expect.Call(localEducationAgencyInformationRepository.GetAll()).Return(suppliedLocalEducationAgencyInformationData);
            
            Expect.Call(rootMetricNodeResolver.GetRootMetricNode()).Return(suppliedRootMetricNode);

            Expect.Call(domainMetricService.Get(null))
                .Constraints(
                    new ActionConstraint<LocalEducationAgencyMetricInstanceSetRequest>(x =>
                    {
                        Assert.That(x.LocalEducationAgencyId == suppliedLocalEducationAgencyId);
                        Assert.That(x.MetricVariantId == suppliedRootMetricVariantId);
                    })
                ).Return(suppliedRootMetricHierarchy);

            Expect.Call(metricTreeToIEnumerableOfKeyValuePairProvider.FlattenMetricTree((ContainerMetric)suppliedRootMetricHierarchy.RootNode)).Return(suppliedRootIEnumerableOfKeyValuePair);

            Expect.Call(domainSpecificMetricNodeResolver.GetOperationalDashboardMetricNode()).Return(suppliedOperationalDashboardMetricNode);

            // Mock the metric service call to obtain operational dashboards
            Expect.Call(domainMetricService.Get(null))
                .Constraints(
                    new ActionConstraint<LocalEducationAgencyMetricInstanceSetRequest>(x =>
                    {
                        Assert.That(x.LocalEducationAgencyId == suppliedLocalEducationAgencyId);
                        Assert.That(x.MetricVariantId == suppliedOperationalDashboardMetricVariantId);
                    })
                ).Return(suppliedOperationalDashboardMetricHierarchy);

            MockFlatteningOfOperationDashboardsHierarchy();
        }

        protected virtual void MockFlatteningOfOperationDashboardsHierarchy()
        {
            Expect.Call(
                metricTreeToIEnumerableOfKeyValuePairProvider.FlattenMetricTree((ContainerMetric) suppliedOperationalDashboardMetricHierarchy.RootNode))
                .Return(suppliedOperationalDashboardIEnumerableOfKeyValuePair);
        }

        protected virtual void InitializeSuppliedData()
        {
            // Prepare supplied data collections
            suppliedLocalEducationAgencyInformationData = GetSuppliedLocalEducationAgencyInformation();

            //For Root
            suppliedRootMetricNode = GetSuppliedRootNode();
            suppliedRootMetricHierarchy = GetSuppliedRootMetricHierarchy();
            suppliedRootIEnumerableOfKeyValuePair = GetSuppliedRootKeyValuePairs();
            
            //For Operational Dashboard
            suppliedOperationalDashboardMetricNode = GetSuppliedOperationalDashboardMetricNode();
            suppliedOperationalDashboardMetricHierarchy = GetSuppliedOperationalDashboardMetricHierarchy();
            suppliedOperationalDashboardIEnumerableOfKeyValuePair = GetSuppliedOperationalDashboardKeyValuePairs();
        }

        private MetricMetadataNode GetSuppliedOperationalDashboardMetricNode()
        {
            return new MetricMetadataNode(new TestMetricMetadataTree())
                       {
                           MetricId = suppliedOperationalDashboardMetricId,
                           MetricVariantId = suppliedOperationalDashboardMetricVariantId,
                           Name = "Operational Dashboard"
                       };
        }

        protected MetricTree GetSuppliedOperationalDashboardMetricHierarchy()
        {
            return new MetricTree(
                new ContainerMetric
                    {
                        MetricId = suppliedOperationalDashboardMetricId,
                        MetricVariantId = suppliedOperationalDashboardMetricVariantId,
                        Name = "Operational Dashboard",
                    });
        }

        protected MetricMetadataNode GetSuppliedOperationalDashboardNode()
        {
            return new MetricMetadataNode(new TestMetricMetadataTree())
            {
                MetricId = suppliedOperationalDashboardMetricId,
                MetricVariantId = suppliedOperationalDashboardMetricVariantId,
                Name = "Operational Dashboard",
            };
        }

        protected MetricTree GetSuppliedRootMetricHierarchy()
        {
           return new MetricTree(
                new ContainerMetric
                       {
                           MetricId = suppliedRootMetricId,
                           MetricVariantId = suppliedRootMetricVariantId,
                           Name = "Root",
                       });
        }

        protected MetricMetadataNode GetSuppliedRootNode()
        {
            return new MetricMetadataNode(new TestMetricMetadataTree())
                       {
                           MetricId = suppliedRootMetricId,
                           MetricVariantId = suppliedRootMetricVariantId,
                           Name = "Root",
                       };
        }

        protected IEnumerable<KeyValuePair<string,object>> GetSuppliedRootKeyValuePairs()
        {
            return new List<KeyValuePair<string, object>>
                            {
                                new KeyValuePair<string, object>("Metric One - Some Title", 12),
                                new KeyValuePair<string, object>("Metric Two - Some Title - some other", "string"),
                                new KeyValuePair<string, object>("Metric 3 - Some Title - double", .89),
                            };
        }

        protected IEnumerable<KeyValuePair<string, object>> GetSuppliedOperationalDashboardKeyValuePairs()
        {
            return new List<KeyValuePair<string, object>>
                            {
                                new KeyValuePair<string, object>("OperationalDashboard Metric One - Some Title", 12),
                                new KeyValuePair<string, object>("OperationalDashboard Metric Two - Some Title - some other", "string1"),
                                new KeyValuePair<string, object>("OperationalDashboard Metric 3 - Some Title - double", .90),
                            };
        }

        protected IQueryable<LocalEducationAgencyInformation> GetSuppliedLocalEducationAgencyInformation()
        {
            return (new List<LocalEducationAgencyInformation>
                        {
                            new LocalEducationAgencyInformation { LocalEducationAgencyId = suppliedLocalEducationAgencyId, Name = "LocalEducationAgency 1" },
                            new LocalEducationAgencyInformation { LocalEducationAgencyId = 9999 },//Should be filtered out when selecting the LEAId.
                        }).AsQueryable();
        }

        protected override void ExecuteTest()
        {
            var service = new ExportAllMetricsService(localEducationAgencyInformationRepository,
                                                                   domainMetricService,
                                                                   rootMetricNodeResolver, 
                                                                   metricTreeToIEnumerableOfKeyValuePairProvider, 
                                                                   domainSpecificMetricNodeResolver);

            actualModel = service.Get(new ExportAllMetricsRequest() { LocalEducationAgencyId = suppliedLocalEducationAgencyId });
        }

        [Test]
        public void Should_return_model_that_is_not_null()
        {
            Assert.That(actualModel, Is.Not.Null);
        }

        [Test]
        public void Should_return_model_with_LEA_Name()
        {
            var data = suppliedLocalEducationAgencyInformationData.Single(x=>x.LocalEducationAgencyId==suppliedLocalEducationAgencyId);

            //The LEA Name should be at location 0.
            Assert.That(actualModel.Rows.ElementAt(0).Cells.ElementAt(0).Key, Is.EqualTo("Local Education Agency"));
            Assert.That(actualModel.Rows.ElementAt(0).Cells.ElementAt(0).Value, Is.EqualTo(data.Name));
        }

        [Test]
        public void Should_return_model_with_metric_properties_on_model()
        {
            //Start i from 1 because 0 is the LEA name.
            var i = 1;
            foreach(var suppliedKeyValuePair in suppliedRootIEnumerableOfKeyValuePair)
            {
                Assert.That(actualModel.Rows.ElementAt(0).Cells.ElementAt(i).Key, Is.EqualTo(suppliedKeyValuePair.Key));
                Assert.That(actualModel.Rows.ElementAt(0).Cells.ElementAt(i).Value, Is.EqualTo(suppliedKeyValuePair.Value));
                i++;
            }
        }

        [Test]
        public virtual void Should_have_serializable_model()
        {
            actualModel.EnsureSerializableModel();
        }
    }

    [TestFixture]
    public class When_exporting_all_local_education_agency_metrics_for_user_with_full_access_to_all_metrics : When_exporting_all_local_education_agency_metrics
    {
        [Test]
        public void Should_return_model_that_contains_operational_dashboard_metrics()
        {
            var allCellKeys = actualModel.Rows.SelectMany(x => x.Cells).Select(x => x.Key);

            Assert.That(allCellKeys.Any(x => x.StartsWith("OperationalDashboard")));
        }
    }

    [TestFixture]
    public class When_exporting_all_local_education_agency_metrics_for_user_with_no_access_to_operational_dashboard_metrics : When_exporting_all_local_education_agency_metrics
    {
        protected override void InitializeSuppliedData()
        {
            base.InitializeSuppliedData();

            // Simulate operational dashboards being inaccessible
            base.suppliedOperationalDashboardMetricHierarchy = null;
        }

        protected override void MockFlatteningOfOperationDashboardsHierarchy()
        {
            // Do nothing - this class should not be called 
        }

        [Test]
        public void Should_return_model_that_does_not_contain_operational_dashboard_metrics()
        {
            var allCellKeys = actualModel.Rows.SelectMany(x => x.Cells).Select(x => x.Key);

            Assert.That(!allCellKeys.Any(x => x.StartsWith("OperationalDashboard")));
        }
    }
}
