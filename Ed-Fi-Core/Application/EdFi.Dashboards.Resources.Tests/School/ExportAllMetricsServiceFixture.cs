// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System.Collections.Generic;
using System.Linq;
using EdFi.Dashboards.Data.Entities;
using EdFi.Dashboards.Data.Repository;
using EdFi.Dashboards.Metric.Resources.Models;
using EdFi.Dashboards.Resources.Common;
using EdFi.Dashboards.Resources.Metric;
using EdFi.Dashboards.Resources.Metric.Requests;
using EdFi.Dashboards.Resources.Models.Common;
using EdFi.Dashboards.Resources.School;
using EdFi.Dashboards.Testing;
using NUnit.Framework;
using Rhino.Mocks;

namespace EdFi.Dashboards.Resources.Tests.School
{
    [TestFixture]
    public abstract class When_exporting_all_school_metrics : TestFixtureBase
    {
        //The Injected Dependencies.
        private IRepository<SchoolInformation> schoolInformationRepository;
        private IDomainMetricService<SchoolMetricInstanceSetRequest> domainMetricService;
        private IRootMetricNodeResolver rootMetricNodeResolver;
        private IMetricTreeToIEnumerableOfKeyValuePairProvider metricTreeToIEnumerableOfKeyValuePairProvider;
        private IDomainSpecificMetricNodeResolver domainSpecificMetricNodeResolver;

        //The Actual Model.
        protected ExportAllModel actualModel;

        //The supplied Data models.
        private const int suppliedSchoolId = 1;
        protected IQueryable<SchoolInformation> suppliedSchoolInformationData;

        //For root
        private const int suppliedRootMetricId = 1;
        private const int suppliedRootMetricVariantId = 1999;
        protected MetricMetadataNode suppliedRootMetricNode;
        protected MetricTree suppliedRootMetricHierarchy;
        protected IEnumerable<KeyValuePair<string, object>> suppliedRootIEnumerableOfKeyValuePair;

        //For Operational Dashboard
        private const int suppliedOperationalDashboardMetricId = 10;
        private const int suppliedOperationalDashboardMetricVariantId = 11;
        protected MetricMetadataNode suppliedOperationalDashboardMetricNode;
        protected MetricTree suppliedOperationalDashboardMetricHierarchy;
        protected IEnumerable<KeyValuePair<string, object>> suppliedOperationalDashboardIEnumerableOfKeyValuePair;

        protected override void EstablishContext()
        {
            InitializeSuppliedData();

            //Set up the mocks
            schoolInformationRepository = mocks.StrictMock<IRepository<SchoolInformation>>();
            domainMetricService = mocks.StrictMock<IDomainMetricService<SchoolMetricInstanceSetRequest>>();
            rootMetricNodeResolver = mocks.StrictMock<IRootMetricNodeResolver>();
            metricTreeToIEnumerableOfKeyValuePairProvider = mocks.StrictMock<IMetricTreeToIEnumerableOfKeyValuePairProvider>();
            domainSpecificMetricNodeResolver = mocks.StrictMock<IDomainSpecificMetricNodeResolver>();

            //Set expectations
            Expect.Call(schoolInformationRepository.GetAll()).Return(suppliedSchoolInformationData);
            
            Expect.Call(rootMetricNodeResolver.GetRootMetricNode()).Return(suppliedRootMetricNode);

            Expect.Call(domainMetricService.Get(null))
                .Constraints(
                    new ActionConstraint<SchoolMetricInstanceSetRequest>(x =>
                    {
                        Assert.That(x.SchoolId == suppliedSchoolId);
                        Assert.That(x.MetricVariantId == suppliedRootMetricVariantId);
                    })
                ).Return(suppliedRootMetricHierarchy);

            Expect.Call(metricTreeToIEnumerableOfKeyValuePairProvider.FlattenMetricTree((ContainerMetric) suppliedRootMetricHierarchy.RootNode)).Return(suppliedRootIEnumerableOfKeyValuePair);

            Expect.Call(domainSpecificMetricNodeResolver.GetOperationalDashboardMetricNode()).Return(suppliedOperationalDashboardMetricNode);

            Expect.Call(domainMetricService.Get(null))
                .Constraints(
                    new ActionConstraint<SchoolMetricInstanceSetRequest>(x =>
                    {
                        Assert.That(x.SchoolId == suppliedSchoolId);
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
            //Prepare supplied data collections
            suppliedSchoolInformationData = GetSuppliedSchoolInformation();
            
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

        protected IEnumerable<KeyValuePair<string, object>> GetSuppliedRootKeyValuePairs()
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

        protected IQueryable<SchoolInformation> GetSuppliedSchoolInformation()
        {
            return (new List<SchoolInformation>
                        {
                            new SchoolInformation { SchoolId = suppliedSchoolId, Name = "School 1" },
                            new SchoolInformation { SchoolId = 9999 },//Should be filtered out when selecting the LEAId.
                        }).AsQueryable();
        }

        protected override void ExecuteTest()
        {
            var service = new ExportAllMetricsService(schoolInformationRepository,
                                                                   domainMetricService,
                                                                   rootMetricNodeResolver,
                                                                   metricTreeToIEnumerableOfKeyValuePairProvider,
                                                                   domainSpecificMetricNodeResolver);

            actualModel = service.Get(new ExportAllMetricsRequest() { SchoolId = suppliedSchoolId });
        }

        [Test]
        public void Should_return_model_that_is_not_null()
        {
            Assert.That(actualModel, Is.Not.Null);
        }

        [Test]
        public void Should_return_model_with_school_name()
        {
            var data = suppliedSchoolInformationData.Single(x => x.SchoolId == suppliedSchoolId);

            //The LEA Name should be at location 0.
            Assert.That(actualModel.Rows.ElementAt(0).Cells.ElementAt(0).Key, Is.EqualTo("School"));
            Assert.That(actualModel.Rows.ElementAt(0).Cells.ElementAt(0).Value, Is.EqualTo(data.Name));
        }

        [Test]
        public void Should_return_model_with_metric_properties_on_model()
        {
            //Start i from 1 because 0 is the LEA name.
            var i = 1;
            foreach (var suppliedKeyValuePair in suppliedRootIEnumerableOfKeyValuePair)
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
    public class When_exporting_all_school_metrics_for_user_with_full_access_to_all_metrics : When_exporting_all_school_metrics
    {
        [Test]
        public void Should_return_model_that_contains_operational_dashboard_metrics()
        {
            var allCellKeys = actualModel.Rows.SelectMany(x => x.Cells).Select(x => x.Key);

            Assert.That(allCellKeys.Any(x => x.StartsWith("OperationalDashboard")));
        }
    }

    [TestFixture]
    public class When_exporting_all_school_metrics_for_user_with_no_access_to_operational_dashboard_metrics : When_exporting_all_school_metrics
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
