// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using EdFi.Dashboards.Core;
using EdFi.Dashboards.Metric.Resources.Models;
using EdFi.Dashboards.Metric.Resources.Providers;
using EdFi.Dashboards.Metric.Resources.Services;
using EdFi.Dashboards.Resources.Common;
using EdFi.Dashboards.Resources.LocalEducationAgency.Detail;
using EdFi.Dashboards.Resources.Metric;
using EdFi.Dashboards.Resources.Metric.MetricInstanceSetKeyResolvers;
using EdFi.Dashboards.Resources.Metric.Requests;
using EdFi.Dashboards.Resources.Models.CustomGrid;
using EdFi.Dashboards.Resources.Models.LocalEducationAgency.Detail;
using EdFi.Dashboards.Resources.Navigation;
using EdFi.Dashboards.Resources.Staff;
using EdFi.Dashboards.Resources.Tests.Navigation.Fakes;
using NUnit.Framework;
using Rhino.Mocks;
using EdFi.Dashboards.Data.Entities;
using EdFi.Dashboards.Data.Repository;
using EdFi.Dashboards.Testing;

namespace EdFi.Dashboards.Resources.Tests.LocalEducationAgency.Detail
{
    public abstract class SchoolMetricListServiceFixtureBase : TestFixtureBase
    {
        private IRepository<LocalEducationAgencyMetricSchoolList> localEducationAgencyMetricSchoolListRepository;
        private IRepository<StaffInformation> staffInformationRepository;
        private IRepository<SchoolInformation> schoolInformationRepository;
        private IUniqueListIdProvider uniqueListIdProvider;
        private IMetricInstanceSetKeyResolver<LocalEducationAgencyMetricInstanceSetRequest> metricInstanceSetKeyResolver;
        private IMetricGoalProvider metricGoalProvider;
        private IMetricNodeResolver metricNodeResolver;
        private IDomainSpecificMetricNodeResolver domainSpecificMetricNodeResolver;
        private IListMetadataProvider listMetadataProvider;
        private IMetadataListIdResolver metadataListIdResolver;

        private IQueryable<LocalEducationAgencyMetricSchoolList> suppliedLocalEducationAgencyMetricSchoolList;
        private IQueryable<StaffInformation> suppliedStaffInformationList;
        private IQueryable<SchoolInformation> suppliedSchoolInformationList;

        private IMetricCorrelationProvider metricCorrelationService;

        private const int suppliedLocalEducationAgencyId = 1;
        protected const int suppliedMetricId = 2;
        protected const int suppliedMetricVariantId = 29999;
        private const int suppliedContextMetricId = 10;
        private const int suppliedContextMetricVariantId = 10999;
        private const int suppliedCorrelationMetricId = 78;
        protected const int suppliedCorrelationMetricVariantId = 78999;
        protected const int suppliedOperationalDashboardMetricId = 15;
        protected const int suppliedOperationalDashboardMetricVariantId = 15999;
        private const string suppliedListFormat = "{0:P3} apple";

        protected const string suppliedListContext = "myUniqueId2";
        private const string suppliedStringValue = "this is a string value";
        private readonly Guid suppliedMetricInstanceSetKey = Guid.NewGuid();
        protected Goal suppliedGoal;

        protected SchoolMetricTableModel actualModel;
        protected SchoolAreaLinksFake schoolAreaLinksFake = new SchoolAreaLinksFake();

        protected abstract void SetSuppliedGoal();

        protected override void EstablishContext()
        {
            SetSuppliedGoal();

            localEducationAgencyMetricSchoolListRepository = mocks.StrictMock<IRepository<LocalEducationAgencyMetricSchoolList>>();
            staffInformationRepository = mocks.StrictMock<IRepository<StaffInformation>>();
            schoolInformationRepository = mocks.StrictMock<IRepository<SchoolInformation>>();
            uniqueListIdProvider = mocks.StrictMock<IUniqueListIdProvider>();
            metricCorrelationService = mocks.StrictMock<IMetricCorrelationProvider>();
            metricInstanceSetKeyResolver = mocks.StrictMock<IMetricInstanceSetKeyResolver<LocalEducationAgencyMetricInstanceSetRequest>>();
            metricGoalProvider = mocks.StrictMock<IMetricGoalProvider>();
            metricNodeResolver = mocks.StrictMock<IMetricNodeResolver>();
            domainSpecificMetricNodeResolver = mocks.StrictMock<IDomainSpecificMetricNodeResolver>();
            listMetadataProvider = mocks.StrictMock<IListMetadataProvider>();
            metadataListIdResolver = mocks.StrictMock<IMetadataListIdResolver>();


            suppliedLocalEducationAgencyMetricSchoolList = GetSuppliedLocalEducationAgencyMetricSchoolList();
            suppliedStaffInformationList = GetSuppliedStaffInformationRepository();
            suppliedSchoolInformationList = GetSuppliedSchoolInformationRepository();

            Expect.Call(metricNodeResolver.GetMetricNodeForLocalEducationAgencyMetricVariantId(suppliedMetricVariantId)).Return(GetSuppliedMetricMetadataNode());
            Expect.Call(localEducationAgencyMetricSchoolListRepository.GetAll()).Repeat.Any().Return(suppliedLocalEducationAgencyMetricSchoolList);
            Expect.Call(staffInformationRepository.GetAll()).Repeat.Any().Return(suppliedStaffInformationList);
            Expect.Call(schoolInformationRepository.GetAll()).Return(suppliedSchoolInformationList);
            Expect.Call(uniqueListIdProvider.GetUniqueId(suppliedMetricVariantId)).Return(suppliedListContext);
            Expect.Call(domainSpecificMetricNodeResolver.GetOperationalDashboardMetricNode()).Return(GetSuppliedOperationalDashboardMetricNode());
            Expect.Call(metricInstanceSetKeyResolver.GetMetricInstanceSetKey(null)).Constraints(
                    new ActionConstraint<LocalEducationAgencyMetricInstanceSetRequest>(x =>
                                                                                        {
                                                                                            Assert.That(x.MetricVariantId == suppliedMetricVariantId);
                                                                                            Assert.That(x.LocalEducationAgencyId == suppliedLocalEducationAgencyId);
                                                                                        })).Return(suppliedMetricInstanceSetKey);
            Expect.Call(metricGoalProvider.GetMetricGoal(suppliedMetricInstanceSetKey, suppliedMetricId)).Return(suppliedGoal);

            Expect.Call(metricCorrelationService.GetRenderingParentMetricVariantIdForSchool(suppliedMetricVariantId, 1)).Constraints(
                     Rhino.Mocks.Constraints.Is.Equal(suppliedMetricVariantId), Rhino.Mocks.Constraints.Is.Anything()).Repeat.Any().Return(new MetricCorrelationProvider.MetricRenderingContext { ContextMetricVariantId = suppliedContextMetricVariantId, MetricVariantId = suppliedCorrelationMetricVariantId });
            Expect.Call(metadataListIdResolver.GetListId(ListType.SchoolMetricTable, SchoolCategory.None)).Return(
                MetadataListIdResolver.SchoolMetricTableListId);
            Expect.Call(listMetadataProvider.GetListMetadata(MetadataListIdResolver.SchoolMetricTableListId)).Return(
                new List<MetadataColumnGroup>());
        }

        protected virtual MetricMetadataNode GetSuppliedOperationalDashboardMetricNode()
        {
            var tree = new TestMetricMetadataTree();
            var root = new MetricMetadataNode(tree)
            {
                MetricId = suppliedOperationalDashboardMetricId,
                MetricVariantId = suppliedOperationalDashboardMetricVariantId,
                Name = "Operational Dashboard"
            };

            tree.Children = new List<MetricMetadataNode> { root };
            return root;
        }

        protected MetricMetadataNode GetSuppliedMetricMetadataNode()
        {
            return new MetricMetadataNode(new TestMetricMetadataTree()) { MetricId = suppliedMetricId, MetricVariantId = suppliedMetricVariantId, ListFormat = suppliedListFormat, MetricNodeId = 1 };
        }

        protected IQueryable<LocalEducationAgencyMetricSchoolList> GetSuppliedLocalEducationAgencyMetricSchoolList()
        {
            var list = new List<LocalEducationAgencyMetricSchoolList> { 
                new LocalEducationAgencyMetricSchoolList { LocalEducationAgencyId = 1, MetricId = suppliedMetricId, SchoolId=2, StaffUSI = 20, Value = ".2",ValueType = "System.Double", SchoolGoal = .5m},
                new LocalEducationAgencyMetricSchoolList { LocalEducationAgencyId = 1, MetricId = suppliedMetricId, SchoolId=1, StaffUSI = 10, Value = "1.1",ValueType = "System.Double", SchoolGoal = .7m},
                new LocalEducationAgencyMetricSchoolList { LocalEducationAgencyId = 1, MetricId = suppliedMetricId, SchoolId=3, StaffUSI = 30, Value = "1.3",ValueType = "System.Double", SchoolGoal = .8m},
                new LocalEducationAgencyMetricSchoolList { LocalEducationAgencyId = 1, MetricId = suppliedMetricId, SchoolId=4, StaffUSI = 30, Value = "1.3",ValueType = "System.Double", SchoolGoal = .8m},
                new LocalEducationAgencyMetricSchoolList { LocalEducationAgencyId = 1, MetricId = suppliedMetricId, SchoolId=5, StaffUSI = 35, Value = suppliedStringValue,ValueType = "System.String", SchoolGoal = .8m},
                new LocalEducationAgencyMetricSchoolList { LocalEducationAgencyId = 900, MetricId = suppliedMetricId, SchoolId=1, StaffUSI = 20, Value = "9.9",ValueType = "System.Double", SchoolGoal = .9m},//Should be excluded. Because its a different district and metric.
                new LocalEducationAgencyMetricSchoolList { LocalEducationAgencyId = 1, MetricId = 99, SchoolId=1, StaffUSI = 20, Value = "9.9",ValueType = "System.Double", SchoolGoal = .9m},//Should be excluded. Because its a different district and metric.
            };
            return list.AsQueryable();
        }

        protected IQueryable<StaffInformation> GetSuppliedStaffInformationRepository()
        {
            var list = new List<StaffInformation> { 
                new StaffInformation { StaffUSI = 10, FullName = "John Doe", LastSurname = "Doe", FirstName = "John"},
                new StaffInformation { StaffUSI = 20, FullName = "Jane Doe", LastSurname = "Doe", FirstName = "Jane"},
                new StaffInformation { StaffUSI = 30, FullName = "Mark Din", LastSurname = "Din", FirstName = "Mark"},
                new StaffInformation { StaffUSI = 35, FullName = "Joe Bob", LastSurname = "Bob", FirstName = "Joe"},
                new StaffInformation { StaffUSI = 40, FullName = "Doug Brown", LastSurname = "Brown", FirstName = "Doug"},//This one will not be included...
            };
            return list.AsQueryable();
        }

        protected IQueryable<SchoolInformation> GetSuppliedSchoolInformationRepository()
        {
            var list = new List<SchoolInformation> { 
                new SchoolInformation { SchoolId=3, Name = "My School 03", SchoolCategory = "High School"},
                new SchoolInformation { SchoolId=2, Name = "My School 02", SchoolCategory = "Middle School"},
                new SchoolInformation { SchoolId=1, Name = "My School 01", SchoolCategory = "Elementary School"},
                new SchoolInformation { SchoolId=4, Name = "My School 04", SchoolCategory = "Ungraded"},
                new SchoolInformation { SchoolId=5, Name = "Another School", SchoolCategory = "High School"},
                new SchoolInformation { SchoolId=0, Name = "My School 00", SchoolCategory = "My School"},//Not included will be filtered by the join.
            };
            return list.AsQueryable();
        }

        protected override void ExecuteTest()
        {
            var service = new SchoolMetricTableService(localEducationAgencyMetricSchoolListRepository, 
                                                        schoolInformationRepository, 
                                                        staffInformationRepository, 
                                                        uniqueListIdProvider, 
                                                        metricCorrelationService, 
                                                        metricGoalProvider, 
                                                        metricInstanceSetKeyResolver, 
                                                        metricNodeResolver, 
                                                        schoolAreaLinksFake,
                                                        domainSpecificMetricNodeResolver, listMetadataProvider, metadataListIdResolver);
            actualModel = service.Get(new SchoolMetricTableRequest()
                                          {
                                              LocalEducationAgencyId = suppliedLocalEducationAgencyId,
                                              MetricVariantId = suppliedMetricVariantId
                                          });
        }

        [Test]
        public void Should_return_a_model_that_is_not_null()
        {
            Assert.That(actualModel, Is.Not.EqualTo(null));
        }

        [Test]
        public void Should_return_correct_number_of_schools_in_the_list()
        {
            Assert.That(actualModel.SchoolMetrics.Count, Is.EqualTo(suppliedLocalEducationAgencyMetricSchoolList.Count(x => x.MetricId == suppliedMetricId && x.LocalEducationAgencyId == suppliedLocalEducationAgencyId)));
        }

        [Test]
        public void Should_return_correct_name_and_type_for_schools()
        {
            var schools = (from cl in suppliedLocalEducationAgencyMetricSchoolList
                            join ci in suppliedSchoolInformationList on cl.SchoolId equals ci.SchoolId
                            where cl.MetricId == suppliedMetricId && cl.LocalEducationAgencyId == suppliedLocalEducationAgencyId
                            orderby cl.SchoolId
                            select new { cl, ci });
            int i = 0;
            foreach (var suppliedSchool in schools)
            {
                Assert.That(actualModel.SchoolMetrics[i].Name, Is.EqualTo(suppliedSchool.ci.Name));
                i++;
            }
        }

        [Test]
        public void Should_return_correct_name_for_principal()
        {
            var principals = (from cl in suppliedLocalEducationAgencyMetricSchoolList
                              join si in suppliedStaffInformationList on cl.StaffUSI equals si.StaffUSI
                              where cl.MetricId == suppliedMetricId && cl.LocalEducationAgencyId == suppliedLocalEducationAgencyId
                              orderby cl.SchoolId
                              select new { cl, si });
            int i = 0;
            foreach (var suppliedPrincipal in principals)
            {
                Assert.That(actualModel.SchoolMetrics[i].Principal, Is.EqualTo(String.Format("{0}, {1}", suppliedPrincipal.si.LastSurname, suppliedPrincipal.si.FirstName)));
                i++;
            }
        }

        [Test]
        public void Should_return_correct_metric_value()
        {
            var metricValues = suppliedLocalEducationAgencyMetricSchoolList.Where(x => x.MetricId == suppliedMetricId && x.LocalEducationAgencyId == suppliedLocalEducationAgencyId && x.ValueType == "System.Double").OrderBy(x => x.SchoolId);

            int i = 0;
            foreach (var suppliedValues in metricValues)
            {
                Assert.That(actualModel.SchoolMetrics[i].Value, Is.EqualTo(Convert.ToDouble(suppliedValues.Value)));
                Assert.That(actualModel.SchoolMetrics[i].DisplayValue, Is.EqualTo(string.Format(suppliedListFormat, Convert.ToDouble(suppliedValues.Value))));
                i++;
            }
            Assert.That(actualModel.SchoolMetrics[i].Value.HasValue, Is.False);
            Assert.That(actualModel.SchoolMetrics[i].DisplayValue, Is.EqualTo(string.Format(suppliedListFormat, suppliedStringValue)));
        }

        [Test]
        public void Should_return_correct_goal_value()
        {
            var metricValues = suppliedLocalEducationAgencyMetricSchoolList.Where(x => x.MetricId == suppliedMetricId && x.LocalEducationAgencyId == suppliedLocalEducationAgencyId).OrderBy(x => x.SchoolId);

            int i = 0;
            foreach (var suppliedValues in metricValues)
            {
                Assert.That(actualModel.SchoolMetrics[i].Goal, Is.EqualTo(Convert.ToDouble(suppliedValues.SchoolGoal)));
                i++;
            }
        }

        [Test]
        public void Should_return_correct_goal_difference_value()
        {
            var metricValues = suppliedLocalEducationAgencyMetricSchoolList.Where(x => x.MetricId == suppliedMetricId && x.LocalEducationAgencyId == suppliedLocalEducationAgencyId && x.ValueType == "System.Double").OrderBy(x => x.SchoolId);

            int i = 0;
            foreach (var suppliedValues in metricValues)
            {
                var suppliedMetricValue = Convert.ToDouble(suppliedValues.Value);
                double suppliedGoalDifferenceValue;
                if (suppliedGoal.Interpretation == TrendInterpretation.Standard)
                    suppliedGoalDifferenceValue = suppliedMetricValue - Convert.ToDouble(suppliedValues.SchoolGoal);
                else 
                    suppliedGoalDifferenceValue = Convert.ToDouble(suppliedValues.SchoolGoal) - suppliedMetricValue;
                Assert.That(actualModel.SchoolMetrics[i].GoalDifference, Is.EqualTo(suppliedGoalDifferenceValue));


                var expectedMetricState = suppliedGoalDifferenceValue >= 0 ? MetricStateType.Good : MetricStateType.Low;
                Assert.That(actualModel.SchoolMetrics[i].MetricState.StateType, Is.EqualTo(expectedMetricState));

                i++;
            }

            Assert.That(actualModel.SchoolMetrics[i].GoalDifference.HasValue, Is.False);
            Assert.That(actualModel.SchoolMetrics[i].MetricState.StateType, Is.EqualTo(MetricStateType.None));
        }
    
        [Test]
        public virtual void Should_properly_set_metric_context_link()
        {
            Assert.That(actualModel.SchoolMetrics[0].MetricContextLink, Is.Not.Null);
            Assert.That(actualModel.SchoolMetrics[0].MetricContextLink.Rel, Is.EqualTo("metricContext"));
            Assert.That(actualModel.SchoolMetrics[0].MetricContextLink.Href, Is.EqualTo(schoolAreaLinksFake.Metrics(actualModel.SchoolMetrics[0].SchoolId, suppliedContextMetricVariantId, actualModel.SchoolMetrics[0].Name, new { listContext = suppliedListContext }).Resolve().MetricAnchor(suppliedCorrelationMetricVariantId)));
        }

        [Test]
        public virtual void Should_properly_set_metric_link()
        {
            Assert.That(actualModel.SchoolMetrics[0].MetricLink, Is.Not.Null);
            Assert.That(actualModel.SchoolMetrics[0].MetricLink.Rel, Is.EqualTo("metric"));

            string expectedLink = schoolAreaLinksFake.Metrics(actualModel.SchoolMetrics[0].SchoolId, suppliedContextMetricVariantId, actualModel.SchoolMetrics[0].Name, new { listContext = suppliedListContext }).Resolve();

            Assert.That(actualModel.SchoolMetrics[0].MetricLink.Href, Is.EqualTo(expectedLink));
        }

        [Test]
        public virtual void Should_have_serializable_model()
        {
            actualModel.EnsureSerializableModel();
        }
    }
    
    [TestFixture]
    public class When_building_the_detail_school_list_model_for_the_local_education_agency_with_standard_goal_interpretation : SchoolMetricListServiceFixtureBase
    {
        protected override void  SetSuppliedGoal()
        {
            suppliedGoal = new Goal { Interpretation = TrendInterpretation.Standard, Value = 99.99m };
        }
    }

    [TestFixture]
    public class When_building_the_detail_school_list_model_for_the_local_education_agency_with_inverse_goal_interpretation : SchoolMetricListServiceFixtureBase
    {
        protected override void SetSuppliedGoal()
        {
            suppliedGoal = new Goal { Interpretation = TrendInterpretation.Inverse, Value = 99.99m };
        }
    }

    [TestFixture]
    public class When_building_the_detail_school_list_model_for_the_local_education_agency_operational_dashboard : SchoolMetricListServiceFixtureBase
    {
        protected override void SetSuppliedGoal()
        {
            suppliedGoal = new Goal { Interpretation = TrendInterpretation.Standard, Value = 99.99m };
        }

        protected override MetricMetadataNode GetSuppliedOperationalDashboardMetricNode()
        {
            var tree = new TestMetricMetadataTree();
            var root = new MetricMetadataNode(tree)
            {
                MetricId = suppliedOperationalDashboardMetricId,
                MetricVariantId = suppliedOperationalDashboardMetricVariantId,
                Name = "Operational Dashboard",
                Children = new MetricMetadataNode[]
                               {
                                   new MetricMetadataNode(tree){ MetricId = suppliedMetricId, MetricVariantId = suppliedMetricVariantId}, 
                               }
            };

            tree.Children = new List<MetricMetadataNode> { root };
            return root;
        }

        [Test]
        public override void Should_properly_set_metric_context_link()
        {
            Assert.That(actualModel.SchoolMetrics[0].MetricContextLink, Is.Not.Null);
            Assert.That(actualModel.SchoolMetrics[0].MetricContextLink.Rel, Is.EqualTo("metricContext"));
            Assert.That(actualModel.SchoolMetrics[0].MetricContextLink.Href, Is.EqualTo(schoolAreaLinksFake.OperationalDashboard(actualModel.SchoolMetrics[0].SchoolId, actualModel.SchoolMetrics[0].Name, new { listContext = suppliedListContext }).Resolve().MetricAnchor(suppliedCorrelationMetricVariantId)));
        }

        [Test]
        public override void Should_properly_set_metric_link()
        {
            Assert.That(actualModel.SchoolMetrics[0].MetricLink, Is.Not.Null);
            Assert.That(actualModel.SchoolMetrics[0].MetricLink.Rel, Is.EqualTo("metric"));

            string expectedLink = schoolAreaLinksFake.OperationalDashboard(actualModel.SchoolMetrics[0].SchoolId, actualModel.SchoolMetrics[0].Name, new { listContext = suppliedListContext }).Resolve();

            Assert.That(actualModel.SchoolMetrics[0].MetricLink.Href, Is.EqualTo(expectedLink));
        }

        
    }
}
