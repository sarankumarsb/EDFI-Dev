using System;
using System.Collections.Generic;
using System.Linq;
using EdFi.Dashboards.Data.Entities;
using EdFi.Dashboards.Data.Repository;
using EdFi.Dashboards.Metric.Resources.Models;
using EdFi.Dashboards.Metric.Resources.Providers;
using EdFi.Dashboards.Metric.Resources.Services;
using EdFi.Dashboards.Resources.Common;
using EdFi.Dashboards.Resources.Metric;
using EdFi.Dashboards.Resources.Metric.Requests;
using EdFi.Dashboards.Resources.Navigation;
using EdFi.Dashboards.Resources.Tests;
using EdFi.Dashboards.Resources.Tests.Navigation.Fakes;
using EdFi.Dashboards.Testing;
using EdFi.Dashboards.Warehouse.Data.Entities;
using EdFi.Dashboards.Warehouse.Resource.Models.LocalEducationAgency.Detail;
using EdFi.Dashboards.Warehouse.Resources.Application;
using EdFi.Dashboards.Warehouse.Resources.LocalEducationAgency.Detail;
using NUnit.Framework;
using Rhino.Mocks;

namespace EdFi.Dashboards.Warehouse.Resources.Tests.LocalEducationAgency.Detail
{
    public abstract class SchoolMetricListServiceFixtureBase : TestFixtureBase
    {
        private IRepository<LocalEducationAgencyMetricInstanceSchoolList> localEducationAgencyMetricSchoolListRepository;
        private IRepository<SchoolInformation> schoolInformationRepository;
        private IUniqueListIdProvider uniqueListIdProvider;
        private IMetricInstanceSetKeyResolver<LocalEducationAgencyMetricInstanceSetRequest> metricInstanceSetKeyResolver;
        private IMetricGoalProvider metricGoalProvider;
        private IMetricNodeResolver metricNodeResolver;
        private IWarehouseAvailabilityProvider warehouseAvailabilityProvider;
        private IMaxPriorYearProvider maxPriorYearProvider;

        private IQueryable<LocalEducationAgencyMetricInstanceSchoolList> suppliedLocalEducationAgencyMetricSchoolList;
        private IQueryable<SchoolInformation> suppliedSchoolInformationList;

        private IMetricCorrelationProvider metricCorrelationService;

        private const int suppliedLocalEducationAgencyId = 1;
        private const int suppliedMetricId = 2;
        private const int suppliedMetricVariantId = 29999;
        private const int suppliedContextMetricVariantId = 10999;
        private const int suppliedCorrelationMetricVariantId = 78999;
        private const string suppliedListFormat = "{0:P3} apple";

        private const string suppliedListContext = "myUniqueId2";
        private const string suppliedStringValue = "this is a string value";
        private readonly Guid suppliedMetricInstanceSetKey = Guid.NewGuid();
        protected Goal suppliedGoal;

        private IList<SchoolPriorYearMetricModel> actualModel;
        private readonly SchoolAreaLinksFake schoolAreaLinksFake = new SchoolAreaLinksFake();

        protected abstract void SetSuppliedGoal();

        protected override void EstablishContext()
        {
            SetSuppliedGoal();

            localEducationAgencyMetricSchoolListRepository = mocks.StrictMock<IRepository<LocalEducationAgencyMetricInstanceSchoolList>>();
            schoolInformationRepository = mocks.StrictMock<IRepository<SchoolInformation>>();
            uniqueListIdProvider = mocks.StrictMock<IUniqueListIdProvider>();
            metricCorrelationService = mocks.StrictMock<IMetricCorrelationProvider>();
            metricInstanceSetKeyResolver = mocks.StrictMock<IMetricInstanceSetKeyResolver<LocalEducationAgencyMetricInstanceSetRequest>>();
            metricGoalProvider = mocks.StrictMock<IMetricGoalProvider>();
            metricNodeResolver = mocks.StrictMock<IMetricNodeResolver>();
            warehouseAvailabilityProvider = mocks.StrictMock<IWarehouseAvailabilityProvider>();
            maxPriorYearProvider = mocks.StrictMock<IMaxPriorYearProvider>();

            suppliedLocalEducationAgencyMetricSchoolList = GetSuppliedLocalEducationAgencyMetricSchoolList();
            suppliedSchoolInformationList = GetSuppliedSchoolInformationRepository();

            Expect.Call(warehouseAvailabilityProvider.Get()).Return(true);
            Expect.Call(maxPriorYearProvider.Get(suppliedLocalEducationAgencyId)).Return(2011);
            Expect.Call(metricNodeResolver.GetMetricNodeForLocalEducationAgencyMetricVariantId(suppliedMetricVariantId)).Return(GetSuppliedMetricMetadataNode());
            Expect.Call(localEducationAgencyMetricSchoolListRepository.GetAll()).Repeat.Any().Return(suppliedLocalEducationAgencyMetricSchoolList);
            Expect.Call(schoolInformationRepository.GetAll()).Return(suppliedSchoolInformationList);
            Expect.Call(uniqueListIdProvider.GetUniqueId(suppliedMetricVariantId)).Return(suppliedListContext);
            Expect.Call(metricInstanceSetKeyResolver.GetMetricInstanceSetKey(null)).Constraints(
                    new ActionConstraint<LocalEducationAgencyMetricInstanceSetRequest>(x =>
                    {
                        Assert.That(x.MetricVariantId == suppliedMetricVariantId);
                        Assert.That(x.LocalEducationAgencyId == suppliedLocalEducationAgencyId);
                    })).Return(suppliedMetricInstanceSetKey);
            Expect.Call(metricGoalProvider.GetMetricGoal(suppliedMetricInstanceSetKey, suppliedMetricId)).Return(suppliedGoal);

            Expect.Call(metricCorrelationService.GetRenderingParentMetricVariantIdForSchool(suppliedMetricVariantId, 1)).Constraints(
                     Rhino.Mocks.Constraints.Is.Equal(suppliedMetricVariantId), Rhino.Mocks.Constraints.Is.Anything()).Repeat.Any().Return(new MetricCorrelationProvider.MetricRenderingContext { ContextMetricVariantId = suppliedContextMetricVariantId, MetricVariantId = suppliedCorrelationMetricVariantId });
        }

        protected MetricMetadataNode GetSuppliedMetricMetadataNode()
        {
            return new MetricMetadataNode(new TestMetricMetadataTree()) { MetricId = suppliedMetricId, MetricVariantId = suppliedMetricVariantId, ListFormat = suppliedListFormat, MetricNodeId = 1 };
        }

        protected IQueryable<LocalEducationAgencyMetricInstanceSchoolList> GetSuppliedLocalEducationAgencyMetricSchoolList()
        {
            var list = new List<LocalEducationAgencyMetricInstanceSchoolList> { 
                new LocalEducationAgencyMetricInstanceSchoolList { LocalEducationAgencyId = 1, MetricId = suppliedMetricId, SchoolId=2, StaffUSI = 20, Value = ".2",ValueType = "System.Double", SchoolGoal = .5m, SchoolYear = 2011, StaffFullName = "School 2 Staff"},
                new LocalEducationAgencyMetricInstanceSchoolList { LocalEducationAgencyId = 1, MetricId = suppliedMetricId, SchoolId=1, StaffUSI = 10, Value = "1.1",ValueType = "System.Double", SchoolGoal = .7m, SchoolYear = 2011, StaffFullName = "School 1 Staff"},
                new LocalEducationAgencyMetricInstanceSchoolList { LocalEducationAgencyId = 1, MetricId = suppliedMetricId, SchoolId=3, StaffUSI = 30, Value = "1.3",ValueType = "System.Double", SchoolGoal = .8m, SchoolYear = 2011, StaffFullName = "School 3 Staff"},
                new LocalEducationAgencyMetricInstanceSchoolList { LocalEducationAgencyId = 1, MetricId = suppliedMetricId, SchoolId=4, StaffUSI = 30, Value = "1.3",ValueType = "System.Double", SchoolGoal = .8m, SchoolYear = 2011, StaffFullName = "School 4 Staff"},
                new LocalEducationAgencyMetricInstanceSchoolList { LocalEducationAgencyId = 1, MetricId = suppliedMetricId, SchoolId=5, StaffUSI = 35, Value = suppliedStringValue,ValueType = "System.String", SchoolGoal = .8m, SchoolYear = 2011, StaffFullName = "School 5 Staff"},
                new LocalEducationAgencyMetricInstanceSchoolList { LocalEducationAgencyId = 900, MetricId = suppliedMetricId, SchoolId=1, StaffUSI = 20, Value = "9.9",ValueType = "System.Double", SchoolGoal = .9m, SchoolYear = 2011, StaffFullName = "School 900-1 Staff"},//Should be excluded. Because its a different district and metric.
                new LocalEducationAgencyMetricInstanceSchoolList { LocalEducationAgencyId = 1, MetricId = 99, SchoolId=1, StaffUSI = 20, Value = "9.9",ValueType = "System.Double", SchoolGoal = .9m, SchoolYear = 2011, StaffFullName = "School 99-1 Staff"},//Should be excluded. Because its a different district and metric.
                new LocalEducationAgencyMetricInstanceSchoolList { LocalEducationAgencyId = 1, MetricId = suppliedMetricId, SchoolId=6, StaffUSI = 35, Value = suppliedStringValue,ValueType = "System.String", SchoolGoal = .8m, SchoolYear = 2010, StaffFullName = "Wrong Year"},
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
            var service = new SchoolPriorYearMetricTableService(localEducationAgencyMetricSchoolListRepository,
                                                        schoolInformationRepository,
                                                        uniqueListIdProvider,
                                                        metricCorrelationService,
                                                        metricGoalProvider,
                                                        metricInstanceSetKeyResolver,
                                                        metricNodeResolver,
                                                        schoolAreaLinksFake,
                                                        warehouseAvailabilityProvider,
                                                        maxPriorYearProvider);
            actualModel = service.Get(new SchoolPriorYearMetricTableRequest
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
            Assert.That(actualModel.Count, Is.EqualTo(suppliedLocalEducationAgencyMetricSchoolList.Count(x => x.MetricId == suppliedMetricId && x.LocalEducationAgencyId == suppliedLocalEducationAgencyId && x.SchoolYear == 2011)));
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
                Assert.That(actualModel[i].Name, Is.EqualTo(suppliedSchool.ci.Name));
                i++;
            }
        }

        [Test]
        public void Should_return_correct_name_for_principal()
        {
            var principals = (from cl in suppliedLocalEducationAgencyMetricSchoolList
                              where cl.MetricId == suppliedMetricId && cl.LocalEducationAgencyId == suppliedLocalEducationAgencyId && cl.SchoolYear == 2011
                              orderby cl.SchoolId
                              select cl);
            int i = 0;
            foreach (var suppliedPrincipal in principals)
            {
                Assert.That(actualModel[i].Principal, Is.EqualTo(suppliedPrincipal.StaffFullName));
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
                Assert.That(actualModel[i].Value, Is.EqualTo(Convert.ToDouble(suppliedValues.Value)));
                Assert.That(actualModel[i].DisplayValue, Is.EqualTo(string.Format(suppliedListFormat, Convert.ToDouble(suppliedValues.Value))));
                i++;
            }
            Assert.That(actualModel[i].Value.HasValue, Is.False);
            Assert.That(actualModel[i].DisplayValue, Is.EqualTo(string.Format(suppliedListFormat, suppliedStringValue)));
        }

        [Test]
        public void Should_return_correct_goal_value()
        {
            var metricValues = suppliedLocalEducationAgencyMetricSchoolList.Where(x => x.MetricId == suppliedMetricId && x.LocalEducationAgencyId == suppliedLocalEducationAgencyId && x.SchoolYear == 2011).OrderBy(x => x.SchoolId);

            int i = 0;
            foreach (var suppliedValues in metricValues)
            {
                Assert.That(actualModel[i].Goal, Is.EqualTo(Convert.ToDouble(suppliedValues.SchoolGoal)));
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
                Assert.That(actualModel[i].GoalDifference, Is.EqualTo(suppliedGoalDifferenceValue));


                var expectedMetricState = suppliedGoalDifferenceValue >= 0 ? MetricStateType.Good : MetricStateType.Low;
                Assert.That(actualModel[i].MetricState.StateType, Is.EqualTo(expectedMetricState));

                i++;
            }

            Assert.That(actualModel[i].GoalDifference.HasValue, Is.False);
            Assert.That(actualModel[i].MetricState.StateType, Is.EqualTo(MetricStateType.None));
        }

        [Test]
        public void Should_properly_set_metric_context_link()
        {
            Assert.That(actualModel[0].MetricContextLink, Is.Not.Null);
            Assert.That(actualModel[0].MetricContextLink.Rel, Is.EqualTo("metricContext"));
            Assert.That(actualModel[0].MetricContextLink.Href, Is.EqualTo(schoolAreaLinksFake.Metrics(actualModel[0].SchoolId, suppliedContextMetricVariantId, actualModel[0].Name, new { listContext = suppliedListContext }).Resolve().MetricAnchor(suppliedCorrelationMetricVariantId)));
        }

        [Test]
        public void Should_properly_set_metric_link()
        {
            Assert.That(actualModel[0].MetricLink, Is.Not.Null);
            Assert.That(actualModel[0].MetricLink.Rel, Is.EqualTo("metric"));

            string expectedLink = schoolAreaLinksFake.Metrics(actualModel[0].SchoolId, suppliedContextMetricVariantId, actualModel[0].Name, new { listContext = suppliedListContext }).Resolve();

            Assert.That(actualModel[0].MetricLink.Href, Is.EqualTo(expectedLink));
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
        protected override void SetSuppliedGoal()
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
    public class When_building_the_detail_school_list_model_for_the_local_education_agency_but_warehouse_is_unavailable : TestFixtureBase
    {
        private IRepository<LocalEducationAgencyMetricInstanceSchoolList> localEducationAgencyMetricSchoolListRepository;
        private IRepository<SchoolInformation> schoolInformationRepository;
        private IUniqueListIdProvider uniqueListIdProvider;
        private IMetricInstanceSetKeyResolver<LocalEducationAgencyMetricInstanceSetRequest> metricInstanceSetKeyResolver;
        private IMetricGoalProvider metricGoalProvider;
        private IMetricNodeResolver metricNodeResolver;
        private IWarehouseAvailabilityProvider warehouseAvailabilityProvider;
        private IMaxPriorYearProvider maxPriorYearProvider;
        private readonly SchoolAreaLinksFake schoolAreaLinksFake = new SchoolAreaLinksFake();
        private IMetricCorrelationProvider metricCorrelationService;

        private IList<SchoolPriorYearMetricModel> actualModel;


        private const int suppliedLocalEducationAgencyId = 1;
        private const int suppliedMetricVariantId = 2;

        protected override void EstablishContext()
        {
            localEducationAgencyMetricSchoolListRepository = mocks.StrictMock<IRepository<LocalEducationAgencyMetricInstanceSchoolList>>();
            schoolInformationRepository = mocks.StrictMock<IRepository<SchoolInformation>>();
            uniqueListIdProvider = mocks.StrictMock<IUniqueListIdProvider>();
            metricCorrelationService = mocks.StrictMock<IMetricCorrelationProvider>();
            metricInstanceSetKeyResolver = mocks.StrictMock<IMetricInstanceSetKeyResolver<LocalEducationAgencyMetricInstanceSetRequest>>();
            metricGoalProvider = mocks.StrictMock<IMetricGoalProvider>();
            metricNodeResolver = mocks.StrictMock<IMetricNodeResolver>();
            warehouseAvailabilityProvider = mocks.StrictMock<IWarehouseAvailabilityProvider>();
            maxPriorYearProvider = mocks.StrictMock<IMaxPriorYearProvider>();

            Expect.Call(warehouseAvailabilityProvider.Get()).Return(false);
        }

        protected override void ExecuteTest()
        {
            var service = new SchoolPriorYearMetricTableService(localEducationAgencyMetricSchoolListRepository,
                                                        schoolInformationRepository,
                                                        uniqueListIdProvider,
                                                        metricCorrelationService,
                                                        metricGoalProvider,
                                                        metricInstanceSetKeyResolver,
                                                        metricNodeResolver,
                                                        schoolAreaLinksFake,
                                                        warehouseAvailabilityProvider,
                                                        maxPriorYearProvider);
            actualModel = service.Get(new SchoolPriorYearMetricTableRequest
            {
                LocalEducationAgencyId = suppliedLocalEducationAgencyId,
                MetricVariantId = suppliedMetricVariantId
            });
        }

        [Test]
        public void Should_return_empty_model()
        {
            Assert.That(actualModel, Is.Not.Null);
        }
    }
}
