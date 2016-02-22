// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System.Collections.Generic;
using System.Linq;
using EdFi.Dashboards.Common;
using EdFi.Dashboards.Core;
using EdFi.Dashboards.Resources.LocalEducationAgency;
using EdFi.Dashboards.Resources.LocalEducationAgency.Detail;
using EdFi.Dashboards.Resources.Models.Common;
using EdFi.Dashboards.Resources.Models.CustomGrid;
using EdFi.Dashboards.Resources.Models.LocalEducationAgency.Detail;
using EdFi.Dashboards.Resources.Staff;
using EdFi.Dashboards.Testing;
using NUnit.Framework;
using Rhino.Mocks;

namespace EdFi.Dashboards.Resources.Tests.LocalEducationAgency
{
    [TestFixture]
    public class When_invoking_the_local_education_agency_export_metric_list_service_get_method : TestFixtureBase
    {
        //The Injected Dependencies.
        private IService<SchoolMetricTableRequest, SchoolMetricTableModel> schoolMetricTableService;
        private IMetadataListIdResolver metadataListIdResolver;
        private IListMetadataProvider listMetadataProvider;
        //The Actual Model.
        private ExportAllModel actualModel;

        //The supplied Data models.
        private const int suppliedLocalEducationAgencyId = 1;
        private const int suppliedMetricId = 100;
        private const int suppliedMetricVariantId = 1000;
        private List<SchoolMetricModel> suppliedListOfSchoolMetricModel;

        protected override void EstablishContext()
        {
            //Prepare supplied data collections
            suppliedListOfSchoolMetricModel = GetListOfSchoolMetricModel();

            //Set up the mocks
            schoolMetricTableService = mocks.StrictMock<IService<SchoolMetricTableRequest, SchoolMetricTableModel>>();
            metadataListIdResolver = mocks.StrictMock<IMetadataListIdResolver>();
            listMetadataProvider = mocks.StrictMock<IListMetadataProvider>();

            //Set expectations
            Expect.Call(schoolMetricTableService.Get(null))
                .Constraints(
                    new ActionConstraint<SchoolMetricTableRequest>(x =>
                    {
                        Assert.That(x.LocalEducationAgencyId == suppliedLocalEducationAgencyId);
                        Assert.That(x.MetricVariantId == suppliedMetricVariantId);
                    })
                ).Return(new SchoolMetricTableModel { SchoolMetrics = suppliedListOfSchoolMetricModel, ListMetadata = new List<MetadataColumnGroup>() });

            Expect.Call(metadataListIdResolver.GetListId(ListType.SchoolMetricTable, SchoolCategory.None)).Return(
                MetadataListIdResolver.SchoolMetricTableListId);

            Expect.Call(listMetadataProvider.GetListMetadata(MetadataListIdResolver.SchoolMetricTableListId)).Return(getSuppliedMetadataColumnGroups());
        }

        private List<SchoolMetricModel> GetListOfSchoolMetricModel()
        {
            return new List<SchoolMetricModel>
                       {
                           new SchoolMetricModel{SchoolId = 1, Name = "BSchool 1",  Principal = "Principal 1", SchoolCategory = "SchoolCategory 1", Value = .89, Goal = .90, GoalDifference = .01},
                           new SchoolMetricModel{SchoolId = 2, Name = "ASchool 2",  Principal = "Principal 2", SchoolCategory = "SchoolCategory 2", Value = null, Goal = .30, GoalDifference = null},
                           new SchoolMetricModel{SchoolId = 3, Name = "CSchool 3",  Principal = "Principal 3", SchoolCategory = "SchoolCategory 3", Value = .10, Goal = .50, GoalDifference = .40},
                       };
        }

        private List<MetadataColumnGroup> getSuppliedMetadataColumnGroups()
        {
            return new List<MetadataColumnGroup>
                       {
                           new MetadataColumnGroup
                               {
                                   GroupType = GroupType.EntityInformation,
                                   Title = "School", 
                                   IsVisibleByDefault = true,
                                   IsFixedColumnGroup = true,
                                   Columns = new List<MetadataColumn>
                                                 {
                                                     new MetadataColumn()
                                                         {
                                                             UniqueIdentifier = 1,
                                                             ColumnPrefix = "",
                                                             MetricVariantId = 1,
                                                             ColumnName = "School",
                                                             IsVisibleByDefault = true,
                                                             IsFixedColumn = true
                                                         },
                                                         new MetadataColumn()
                                                         {
                                                             UniqueIdentifier = 2,
                                                             ColumnPrefix = "",
                                                             MetricVariantId = 1,
                                                             ColumnName = "Principal",
                                                             IsVisibleByDefault = true,
                                                             IsFixedColumn = true,
                                                             MetricListCellType = MetricListCellType.None
                                                         },
                                                         new MetadataColumn()
                                                         {
                                                             UniqueIdentifier = 3,
                                                             MetricVariantId = 1,
                                                             ColumnName = "Type",
                                                             IsVisibleByDefault = true,
                                                             SortAscending = "sortSchoolTypeAsc",
                                                             SortDescending = "sortSchoolTypeDesc",
                                                             MetricListCellType = MetricListCellType.None
                                                         },
                                                         new MetadataColumn()
                                                         {
                                                             UniqueIdentifier = 4,
                                                             ColumnPrefix = "",
                                                             MetricVariantId = 1,
                                                             ColumnName = "School Metric Value",
                                                             IsVisibleByDefault = true,
                                                             SortAscending = "sortNAValueAsc",
                                                             SortDescending = "sortNAValueDesc"
                                                         },
                                                         new MetadataColumn()
                                                         {
                                                             UniqueIdentifier = 5,
                                                             ColumnPrefix = "",
                                                             MetricVariantId = 1,
                                                             ColumnName = "School Goal",
                                                             IsVisibleByDefault = true,
                                                             MetricListCellType = MetricListCellType.None
                                                         },
                                                         new MetadataColumn()
                                                         {
                                                             UniqueIdentifier = 6,
                                                             ColumnPrefix = "",
                                                             MetricVariantId = 1,
                                                             ColumnName = "Difference From Goal",
                                                             IsVisibleByDefault = true,
                                                             MetricListCellType = MetricListCellType.None
                                                         }
                                                 }
                               }
                       };
        }
        protected override void ExecuteTest()
        {
            var service = new ExportMetricListService(schoolMetricTableService, metadataListIdResolver, listMetadataProvider);
            actualModel = service.Get(new ExportMetricListRequest
                                          {
                                              LocalEducationAgencyId = suppliedLocalEducationAgencyId,
                                              MetricVariantId = suppliedMetricVariantId
                                          });
        }

        [Test]
        public void Should_return_model_with_correct_number_of_rows()
        {
            Assert.That(suppliedListOfSchoolMetricModel.Count, Is.Not.EqualTo(0));
            Assert.That(actualModel.Rows.Count(), Is.EqualTo(suppliedListOfSchoolMetricModel.Count));
        }

        [Test]
        public void Should_return_model_with_correct_data()
        {
            var i = 0;
            foreach (var suppliedRow in suppliedListOfSchoolMetricModel.OrderBy(x => x.Name))
            {
                Assert.That(actualModel.Rows.ElementAt(i).Cells.ElementAt(0).Key, Is.EqualTo("School Name"));
                Assert.That(actualModel.Rows.ElementAt(i).Cells.ElementAt(1).Key, Is.EqualTo("Principal"));
                Assert.That(actualModel.Rows.ElementAt(i).Cells.ElementAt(2).Key, Is.EqualTo("Type"));
                Assert.That(actualModel.Rows.ElementAt(i).Cells.ElementAt(3).Key, Is.EqualTo("School Metric Value"));
                Assert.That(actualModel.Rows.ElementAt(i).Cells.ElementAt(4).Key, Is.EqualTo("School Goal"));
                Assert.That(actualModel.Rows.ElementAt(i).Cells.ElementAt(5).Key, Is.EqualTo("Difference From Goal"));

                Assert.That(actualModel.Rows.ElementAt(i).Cells.ElementAt(0).Value, Is.EqualTo(suppliedRow.Name));
                Assert.That(actualModel.Rows.ElementAt(i).Cells.ElementAt(1).Value, Is.EqualTo(suppliedRow.Principal));
                Assert.That(actualModel.Rows.ElementAt(i).Cells.ElementAt(2).Value, Is.EqualTo(suppliedRow.SchoolCategory));
                Assert.That(actualModel.Rows.ElementAt(i).Cells.ElementAt(3).Value, Is.EqualTo(suppliedRow.Value));
                Assert.That(actualModel.Rows.ElementAt(i).Cells.ElementAt(4).Value, Is.EqualTo(suppliedRow.Goal));
                Assert.That(actualModel.Rows.ElementAt(i).Cells.ElementAt(5).Value, Is.EqualTo(suppliedRow.GoalDifference));
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
