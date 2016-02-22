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
using EdFi.Dashboards.Resources.StudentSchool;
using EdFi.Dashboards.Testing;
using NUnit.Framework;
using Rhino.Mocks;

namespace EdFi.Dashboards.Resources.Tests.Student
{
    [TestFixture]
    public class When_invoking_the_student_export_all_service_get_method : TestFixtureBase
    {
        //The Injected Dependencies.
        private IRepository<StudentInformation> studentInformationRepository;
        private IRootMetricNodeResolver rootMetricNodeResolver;
        private IDomainMetricService<StudentSchoolMetricInstanceSetRequest> domainMetricService;
        private IMetricTreeToIEnumerableOfKeyValuePairProvider metricTreeToIEnumerableOfKeyValuePairProvider;

        //The Actual Model.
        private ExportAllModel actualModel;

        //The supplied Data models.
        private const int suppliedSchoolId = 10;
        private const int suppliedStudentUSI = 1;
        private IQueryable<StudentInformation> suppliedStudentInformationData;
        private MetricMetadataNode suppliedRootMetricNode;
        private MetricTree suppliedRootMetricHierarchy;
        private IEnumerable<KeyValuePair<string, object>> suppliedRootIEnumerableOfKeyValuePair;

        protected override void EstablishContext()
        {
            //Prepare supplied data collections
            suppliedStudentInformationData = GetSuppliedStudentInformation();
            suppliedRootMetricNode = GetSuppliedRootNode();
            suppliedRootMetricHierarchy = GetSuppliedRootMetricHierarchy();
            suppliedRootIEnumerableOfKeyValuePair = GetSuppliedRootKeyValuePairs();

            //Set up the mocks
            studentInformationRepository = mocks.StrictMock<IRepository<StudentInformation>>();
            rootMetricNodeResolver = mocks.StrictMock<IRootMetricNodeResolver>();
            domainMetricService = mocks.StrictMock<IDomainMetricService<StudentSchoolMetricInstanceSetRequest>>();
            metricTreeToIEnumerableOfKeyValuePairProvider = mocks.StrictMock<IMetricTreeToIEnumerableOfKeyValuePairProvider>();

            //Set expectations
            Expect.Call(studentInformationRepository.GetAll()).Return(suppliedStudentInformationData);
            Expect.Call(rootMetricNodeResolver.GetRootMetricNode()).Return(suppliedRootMetricNode);

            Expect.Call(domainMetricService.Get(null))
                .Constraints(
                    new ActionConstraint<StudentSchoolMetricInstanceSetRequest>(x =>
                    {
                        Assert.That(x.SchoolId == suppliedSchoolId);
                        Assert.That(x.StudentUSI == suppliedStudentUSI);
                        Assert.That(x.MetricVariantId == suppliedRootMetricNode.MetricVariantId);
                    })
                ).Return(suppliedRootMetricHierarchy);
            Expect.Call(metricTreeToIEnumerableOfKeyValuePairProvider.FlattenMetricTree((ContainerMetric) suppliedRootMetricHierarchy.RootNode)).Return(suppliedRootIEnumerableOfKeyValuePair);
        }

        protected IQueryable<StudentInformation> GetSuppliedStudentInformation()
        {
            return (new List<StudentInformation>
                        {
                            new StudentInformation{StudentUSI = suppliedStudentUSI, FirstName = "John", MiddleName = "A", LastSurname = "Doe"},
                            new StudentInformation{StudentUSI = 9999},//Fitered out.
                        }).AsQueryable();
        }

        protected MetricMetadataNode GetSuppliedRootNode()
        {
            var tree = new TestMetricMetadataTree();

            return new MetricMetadataNode(tree)
            {
                MetricId = 100,
                Name = "Root",
            };
        }

        protected MetricTree GetSuppliedRootMetricHierarchy()
        {
            return new MetricTree(
                new ContainerMetric
                    {
                        MetricId = 1000,
                        MetricVariantId = 2000,
                        Name = "Root",
                    });
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

        protected override void ExecuteTest()
        {
            var service = new ExportAllMetricsService(studentInformationRepository, rootMetricNodeResolver, domainMetricService, metricTreeToIEnumerableOfKeyValuePairProvider);
            actualModel = service.Get(new ExportAllMetricsRequest(){SchoolId = suppliedSchoolId, StudentUSI = suppliedStudentUSI});
        }

        [Test]
        public void Should_return_model_that_is_not_null()
        {
            Assert.That(actualModel, Is.Not.Null);
        }

        [Test]
        public void Should_return_model_that_has_column_with_students_name_by_last()
        {
            Assert.That(actualModel.Rows.ElementAt(0).Cells.ElementAt(0).Key, Is.EqualTo("Student Name"));
            Assert.That(actualModel.Rows.ElementAt(0).Cells.ElementAt(0).Value, Is.EqualTo("Doe, John A."));
        }

        [Test]
        public void Should_return_model_with_metric_properties_flattened_out()
        {
            //Start i from 1 because Domain Entity Name.
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
}
