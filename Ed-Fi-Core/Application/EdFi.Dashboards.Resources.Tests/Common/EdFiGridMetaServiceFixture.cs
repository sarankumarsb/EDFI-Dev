using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EdFi.Dashboards.Core;
using EdFi.Dashboards.Resources.Common;
using EdFi.Dashboards.Resources.Models.CustomGrid;
using EdFi.Dashboards.Resources.School;
using EdFi.Dashboards.Resources.Staff;
using EdFi.Dashboards.Testing;
using NUnit.Framework;
using Rhino.Mocks;

namespace EdFi.Dashboards.Resources.Tests.Common
{
    [TestFixture]
    public class EdFiGridMetaServiceFixture : TestFixtureBase
    {
        private EdFiGridMetaService edFiGridMetaService;

        private ISchoolCategoryProvider schoolCategoryProvider;
        private IMetadataListIdResolver metadataListIdResolver;
        private IListMetadataProvider metadataProvider;

        private EdFiGridModel response;

        private List<MetadataColumnGroup> expectedColumnGroups = new List<MetadataColumnGroup>();
        private int expectedMetadataListId = 8;
        private int expectedSchoolId = 101;
        private ListType expectedListType = ListType.StudentDemographic;
        private SchoolCategory expectedSchoolCategory = SchoolCategory.Elementary;

        protected override void EstablishContext()
        {
            base.BeforeExecuteTest();
            schoolCategoryProvider = mocks.StrictMock<ISchoolCategoryProvider>();
            metadataListIdResolver = mocks.StrictMock<IMetadataListIdResolver>();
            metadataProvider = mocks.StrictMock<IListMetadataProvider>();

            Expect.Call(schoolCategoryProvider.GetSchoolCategoryType(expectedSchoolId))
                  .Return(expectedSchoolCategory);
            Expect.Call(metadataListIdResolver.GetListId(expectedListType, expectedSchoolCategory))
                  .Return(expectedMetadataListId);
            Expect.Call(metadataProvider.GetListMetadata(expectedMetadataListId))
                  .Return(expectedColumnGroups);
        }

        protected override void ExecuteTest()
        {
            var request = new EdFiGridMetaRequest
                {
                    SchoolId = expectedSchoolId,
                    GridListType = ListType.StudentDemographic,
                };
            edFiGridMetaService = new EdFiGridMetaService(schoolCategoryProvider, metadataListIdResolver,
                                                          metadataProvider);

            response = edFiGridMetaService.Get(request);
        }

        [Test]
        public void should_return_the_correct_grid_model()
        {
            Assert.That(response.ListMetadata, Is.EqualTo(expectedColumnGroups));
        }
    }
}
