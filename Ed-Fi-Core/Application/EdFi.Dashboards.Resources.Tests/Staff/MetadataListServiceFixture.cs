using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EdFi.Dashboards.Data.Repository;
using EdFi.Dashboards.Metric.Data.Entities;
using EdFi.Dashboards.Resources.Models.Staff;
using EdFi.Dashboards.Resources.Staff;
using EdFi.Dashboards.Testing;
using NUnit.Framework;
using Rhino.Mocks;

namespace EdFi.Dashboards.Resources.Tests.Staff
{
    public class MetadataListServiceFixture:TestFixtureBase
    {
        protected IRepository<MetadataList> metadataListRepository;
        protected MetadataListModel actualModel;
        protected string providedListName = "StudentDrilldownElementarySchool";

        protected override void EstablishContext()
        {
            metadataListRepository = mocks.StrictMock<IRepository<MetadataList>>();
            Expect.Call(metadataListRepository.GetAll()).Repeat.Any().Return(GetMetadataList());

            base.EstablishContext();
        }

        protected IQueryable<MetadataList> GetMetadataList()
        {
            var data = new List<MetadataList>
            {
                new MetadataList {MetadataListId = 1, Name = "StudentDrilldownHighSchool"},
                new MetadataList {MetadataListId = 2, Name = "StudentDrilldownMiddleSchool"},
                new MetadataList {MetadataListId = 3, Name = "StudentDrilldownElementarySchool"},
                new MetadataList {MetadataListId = 4, Name = "GeneralOverviewHighSchool"},
                new MetadataList {MetadataListId = 5, Name = "GeneralOverviewMiddleSchool"}
            }.AsQueryable();

            return data;
        }

        protected override void ExecuteTest()
        {
            var metadataListService = new MetadataListService(metadataListRepository);
            actualModel = metadataListService.Get(new MetadataListRequest{MetadataListName=providedListName});
        }

        [Test]
        public virtual void Should_have_serializable_model()
        {
            actualModel.EnsureSerializableModel();
        }

        [Test]
        public virtual void Should_return_expected_listId()
        {
            Assert.AreEqual(actualModel.MetadataListId, 3);
        }
    }
}
