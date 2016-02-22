using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EdFi.Dashboards.Data.Entities;
using EdFi.Dashboards.Resources.Common;
using EdFi.Dashboards.Resources.Models.CustomGrid;
using EdFi.Dashboards.Testing;
using NUnit.Framework;

namespace EdFi.Dashboards.Resources.Tests.Common
{
    public class TabFactoryFixture : TestFixtureBase
    {
        private TabFactory tabFactory;
        private List<EdFiGridWatchListTabModel> result;

        protected override void ExecuteTest()
        {
            tabFactory = new TabFactory();
            result = tabFactory.CreateAllTabs(GetProvidedDataState());
        }

        private WatchListDataState GetProvidedDataState()
        {
            return new WatchListDataState
            {
                Grades = new List<EdFiGridWatchListSelectionItemModel>(),
                MetricIds = new List<int>(),
                Metrics = new List<StudentMetric>().AsQueryable(),
                Sections = new Dictionary<int, string>(),
            };
        }

        [Test]
        public void results_should_be_reasonable()
        {
            foreach (var tab in result)
            {
                foreach (var column in tab.Columns)
                {
                    foreach (var template in column.Templates)
                    {
                        //template.GroupDisplayValue is only used in some cases, so I'm not testing it here.
                        Assert.That(template.ViewModel, Is.Not.Null);
                        Assert.That(template.TemplateName, Has.Length.GreaterThan(0));
                    }
                }
            }
        }

        [Test]
        public void results_should_have_at_least_one_tab_and_column()
        {
            Assert.That(result, Has.Count.GreaterThan(0));
            Assert.That(result[0].Columns, Has.Count.GreaterThan(0));
        }
    }
}
