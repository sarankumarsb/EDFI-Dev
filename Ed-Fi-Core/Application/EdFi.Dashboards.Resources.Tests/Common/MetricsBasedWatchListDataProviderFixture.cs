// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EdFi.Dashboards.Application.Data.Entities;
using EdFi.Dashboards.Data.Entities;
using EdFi.Dashboards.Data.Repository;
using EdFi.Dashboards.Resources.Common;
using EdFi.Dashboards.Resources.Models.Common;
using EdFi.Dashboards.Resources.Models.CustomGrid;
using EdFi.Dashboards.Resources.Navigation;
using EdFi.Dashboards.Resources.StudentMetrics;
using EdFi.Dashboards.Testing;
using NUnit.Framework;
using Rhino.Mocks;

namespace EdFi.Dashboards.Resources.Tests.Common
{
    public class MetricsBasedWatchListDataProviderFixtureGetEdFiGridWatchListModel : TestFixtureBase
    {
        private MetricsBasedWatchListDataProvider metricsBasedWatchListDataProvider;
        private EdFiGridWatchListModel result;

        private IStudentMetricsProvider studentMetricsProvider;
        private IWatchListLinkProvider watchListLinkProvider;
        private IGeneralLinks generalLinks;
        private ITabFactory tabFactory;

        private const int TestStaffUsi = 8;
        private const string ProvidedWatchListUrl = "http://testuri";
        private const string ProvidedWatchListSearchUrl = "http://testuri2";

        protected override void EstablishContext()
        {
            base.EstablishContext();
            studentMetricsProvider = mocks.StrictMock<IStudentMetricsProvider>();
            tabFactory = mocks.StrictMock<ITabFactory>();

            Expect.Call(studentMetricsProvider.GetOrderedStudentList(null))
                  .IgnoreArguments()
                  .Return(ProvidedEnhancedStudentInformations());
            Expect.Call(studentMetricsProvider.GetStudentsWithMetrics(null))
                  .IgnoreArguments()
                  .Return(ProvidedStudentMetrics());
            Expect.Call(tabFactory.CreateAllTabs(null))
                .IgnoreArguments()
                .Return(ProvidedTabs());

            watchListLinkProvider = mocks.StrictMock<IWatchListLinkProvider>();
            Expect.Call(watchListLinkProvider.GenerateLink(null)).IgnoreArguments().Return(ProvidedWatchListUrl);

            generalLinks = mocks.StrictMock<IGeneralLinks>();
            Expect.Call(generalLinks.MetricsBasedWatchList("MetricsBasedWatchList")).Return(ProvidedWatchListSearchUrl);
        }

        private List<EdFiGridWatchListTabModel> ProvidedTabs()
        {
            return new List<EdFiGridWatchListTabModel>();
        }

        private IQueryable<StudentMetric> ProvidedStudentMetrics()
        {
            return Enumerable.Range(1, 5000).Select(i => new StudentMetric {MetricId = i}).ToList().AsQueryable();
        }

        private IQueryable<EnhancedStudentInformation> ProvidedEnhancedStudentInformations()
        {
            return new List<EnhancedStudentInformation>
                {
                    new EnhancedStudentInformation()
                }.AsQueryable();
        }

        protected override void ExecuteTest()
        {
            metricsBasedWatchListDataProvider = new MetricsBasedWatchListDataProvider(
                null, null, null, studentMetricsProvider, watchListLinkProvider, generalLinks, tabFactory);
            result = metricsBasedWatchListDataProvider.GetEdFiGridWatchListModel(TestStaffUsi, null);
        }

        [Test]
        public void result_should_not_be_null()
        {
            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public void description_should_not_be_null()
        {
            //A new list should default to an empty description
            Assert.That(result.WatchListDescription, Is.EqualTo(string.Empty));
        }

        [Test]
        public void should_have_default_name()
        {
            //A new list should default to an empty description
            Assert.That(result.WatchListName, Is.EqualTo("New Dynamic List"));
        }

        [Test]
        public void should_use_provided_WatchListSearchUrl()
        {
            //A new list should default to an empty description
            Assert.That(result.WatchListSearchUrl, Is.EqualTo(ProvidedWatchListUrl));
        }

        [Test]
        public void should_use_provided_WatchListUrl()
        {
            //A new list should default to an empty description
            Assert.That(result.WatchListUrl, Is.EqualTo(ProvidedWatchListSearchUrl));
        }

        [Test]
        public void should_not_be_shared_by_default()
        {
            Assert.That(result.IsWatchListShared, Is.False);
        }

        [Test]
        public void should_not_be_flagged_as_changed_by_default()
        {
            Assert.That(result.IsWatchListChanged, Is.False);
        }
    }

    public class MetricsBasedWatchListDataProviderFixtureGetWatchListData : TestFixtureBase
    {
        private MetricsBasedWatchListDataProvider metricsBasedWatchListDataProvider;
        private MetricBasedWatchList result;
        private IRepository<MetricBasedWatchList> watchListRepository;

        private const int TestWatchListId = 5;
        private const int NonMatchingTestWatchListId = 6;

        protected override void EstablishContext()
        {
            base.EstablishContext();
            watchListRepository = mocks.StrictMock<IRepository<MetricBasedWatchList>>();
            Expect.Call(watchListRepository.GetAll()).Return(ProvidedWatchLists());
        }

        private IQueryable<MetricBasedWatchList> ProvidedWatchLists()
        {
            return new List<MetricBasedWatchList>
                {
                    new MetricBasedWatchList{MetricBasedWatchListId = TestWatchListId},
                    new MetricBasedWatchList{MetricBasedWatchListId = NonMatchingTestWatchListId}
                }.AsQueryable();
        }

        protected override void ExecuteTest()
        {
            metricsBasedWatchListDataProvider = new MetricsBasedWatchListDataProvider(
                null, watchListRepository, null, null, null, null, null);
            result = metricsBasedWatchListDataProvider.GetWatchListData(TestWatchListId);
        }

        [Test]
        public void result_should_not_be_null()
        {
            Assert.That(result, Is.Not.Null);
        }
    }

    public class MetricsBasedWatchListDataProviderFixtureGetWatchListSelectionData : TestFixtureBase
    {
        private MetricsBasedWatchListDataProvider metricsBasedWatchListDataProvider;
        private List<NameValuesType> result;
        private IRepository<MetricBasedWatchListSelectedOption> watchListSelectionsRepository;

        private const int TestWatchListId = 5;
        private const int NonMatchingTestWatchListId = 6;

        protected override void EstablishContext()
        {
            base.EstablishContext();
            watchListSelectionsRepository = mocks.StrictMock<IRepository<MetricBasedWatchListSelectedOption>>();
            Expect.Call(watchListSelectionsRepository.GetAll()).Return(ProvidedMetricBasedWatchListSelectedOption());
        }

        private IQueryable<MetricBasedWatchListSelectedOption> ProvidedMetricBasedWatchListSelectedOption()
        {
            return new List<MetricBasedWatchListSelectedOption>
                {
                    new MetricBasedWatchListSelectedOption{MetricBasedWatchListId = TestWatchListId},
                    new MetricBasedWatchListSelectedOption{MetricBasedWatchListId = NonMatchingTestWatchListId}
                }.AsQueryable();
        }

        protected override void ExecuteTest()
        {
            metricsBasedWatchListDataProvider = new MetricsBasedWatchListDataProvider(
                null, null, watchListSelectionsRepository, null, null, null, null);
            result = metricsBasedWatchListDataProvider.GetWatchListSelectionData(TestWatchListId);
        }

        [Test]
        public void result_returned_a_value()
        {
            Assert.That(result, Has.Count.EqualTo(1));
        }
    }
}
