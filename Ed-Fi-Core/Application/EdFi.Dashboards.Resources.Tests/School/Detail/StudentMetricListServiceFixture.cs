using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using EdFi.Dashboards.Core;
using EdFi.Dashboards.Data.Entities;
using EdFi.Dashboards.Data.Repository;
using EdFi.Dashboards.Metric.Resources.Services;
using EdFi.Dashboards.Resources.Models.Common;
using EdFi.Dashboards.Resources.Models.CustomGrid;
using EdFi.Dashboards.Resources.School.Detail;
using EdFi.Dashboards.Metric.Data.Entities;
using EdFi.Dashboards.Metric.Resources.Models;
using EdFi.Dashboards.Metric.Resources.Providers;
using EdFi.Dashboards.Resources.Metric;
using EdFi.Dashboards.Resources.Metric.Requests;
using EdFi.Dashboards.Resources.Models.School.Detail;
using EdFi.Dashboards.Resources.Models.Student;
using EdFi.Dashboards.Resources.School;
using EdFi.Dashboards.Resources.Staff;
using EdFi.Dashboards.Resources.StudentMetrics;
using EdFi.Dashboards.Resources.StudentSchool;
using EdFi.Dashboards.Resources.Tests.Navigation.Fakes;
using EdFi.Dashboards.Testing;
using NUnit.Framework;
using Rhino.Mocks;

namespace EdFi.Dashboards.Resources.Tests.School.Detail
{
    public abstract class StudentMetricListServiceFixtureBase : TestFixtureBase
    {
        protected const int suppliedStudentId1 = 160;
        protected const int suppliedStudentId2 = 170;
        protected const int suppliedStudentId3 = 180;
        protected const int suppliedStudentId4 = 190;
        protected const int suppliedSchoolId = 2000;
        protected const int suppliedLocalEducationAgencyId = 3000;
        protected const int suppliedMetricId = 999;
        protected const int suppliedMetricVariantId = 99900;
        protected const int suppliedUniqueIdentifier = 500;
        protected const string suppliedMetricFormat = "{0:P2} test";
        protected const string suppliedUniqueListId = "baboon";
        protected const int suppliedRenderingMetricId = 5555;
        protected const int suppliedRenderingMetricVariantId = 555500;
        protected const int suppliedContextMetricId = 6789;
        protected const int suppliedContextMetricVariantId = 678900;
        protected const string suppliedFourByFourText = "four by four state text";
        protected const string suppliedFourByFourDisplayText = "four by four display state text";
        protected const int suppliedPageNumber = 1;
        protected const int suppliedPageSize = 100;
        protected const int suppliedSortColumn = 1;
        protected const string suppliedSortDirection = "asc";
        protected const string suppliedVisibleColumns = "-3,-5,-4,7,78,16,17,26,25";
        protected const string suppliedOrderBy = "sl.LastSurname, sl.FirstName, sl.MiddleName";
        protected const string suppliedSelectColumns = "cmsl.MetricId, cmsl.Value, cmsl.ValueType, sl.*";
        protected const string suppliedBaseSelect = "cmsl.MetricId, Value, ValueType, sl.SchoolId, sl.StudentUSI, sl.GradeLevel, sl.LastSurname, sl.FirstName, sl.MiddleName, sl.FullName";
        protected const int suppliedStaffUSI = 1;
        protected List<MetadataColumnGroup> suppliedMetadataColumnGroupList;

        protected IRepository<SchoolMetricStudentList> schoolMetricStudentListRepository;
        protected IRepository<StaffStudentAssociation> staffStudentAssociation;
        protected IMetricInstanceSetKeyResolver<SchoolMetricInstanceSetRequest> metricInstanceSetKeyResolver;
        protected IRepository<MetricInstanceFootnote> footnoteRepository;
        protected ISchoolCategoryProvider schoolCategoryProvider;
        protected IMetricCorrelationProvider metricCorrelationService;
        protected IClassroomMetricsProvider classroomMetricsProvider;
        protected IListMetadataProvider listMetadataProvider;
        protected IMetadataListIdResolver metadataListIdResolver;
        protected IMetricNodeResolver metricNodeResolver;
        protected IStudentMetricsProvider StudentMetricProvider;
        protected IGradeLevelUtilitiesProvider gradeLevelUtilitiesProvider;

        protected StudentMetricListModel actualModel;
        protected StudentMetricListMetaModel metaModel;

        protected readonly List<long> expectedStudentIds = new List<long>();
        protected string expectedSubjectArea;
        protected SchoolCategory expectedSchoolCategory;
        protected int expectedMetricCount;
        protected readonly Guid suppliedMetricInstanceSetKey = Guid.NewGuid();
        protected StudentSchoolAreaLinksFake studentSchoolAreaLinksFake = new StudentSchoolAreaLinksFake();
        protected const string suppliedFootnoteText1 = "footnote 1";
        protected const string suppliedFootnoteText2 = "footnote 2";
        protected const string suppliedFootnoteText3 = "footnote 3";
        private const string suppliedListDataLabel = "supplied list data label";

        protected override void EstablishContext()
        {
            schoolCategoryProvider = mocks.StrictMock<ISchoolCategoryProvider>();
            metricCorrelationService = mocks.StrictMock<IMetricCorrelationProvider>();
            footnoteRepository = mocks.StrictMock<IRepository<MetricInstanceFootnote>>();
            classroomMetricsProvider = mocks.StrictMock<IClassroomMetricsProvider>();
            listMetadataProvider = mocks.StrictMock<IListMetadataProvider>();
            metadataListIdResolver = mocks.StrictMock<IMetadataListIdResolver>();
            metricNodeResolver = mocks.StrictMock<IMetricNodeResolver>();
            StudentMetricProvider = mocks.StrictMock<IStudentMetricsProvider>();
            gradeLevelUtilitiesProvider = mocks.StrictMock<IGradeLevelUtilitiesProvider>();
            schoolMetricStudentListRepository = mocks.StrictMock<IRepository<SchoolMetricStudentList>>();

            Expect.Call(StudentMetricProvider.GetOrderedStudentList(null, null, null)).Repeat.Any().IgnoreArguments().Return(GetSuppliedStudentList());
            Expect.Call(StudentMetricProvider.GetStudentsWithMetrics(null)).Repeat.Any().IgnoreArguments().Return(GetStudentListPageData());
            Expect.Call(metricCorrelationService.GetRenderingParentMetricVariantIdForStudent(suppliedMetricVariantId, suppliedSchoolId)).Repeat.Any().Return(new MetricCorrelationProvider.MetricRenderingContext { MetricVariantId = suppliedRenderingMetricVariantId, ContextMetricVariantId = suppliedContextMetricVariantId });
            Expect.Call(metricNodeResolver.GetMetricNodeForSchoolFromMetricVariantId(suppliedSchoolId, suppliedMetricVariantId)).Repeat.AtLeastOnce().Return(GetSuppliedMetricMetadataNode());
            Expect.Call(footnoteRepository.GetAll()).Repeat.Any().Return(GetSuppliedMetricFootnotes());
            Expect.Call(metadataListIdResolver.GetListId(ListType.ClassroomGeneralOverview, SchoolCategory.HighSchool)).IgnoreArguments().Repeat.Any().Return(1);
            Expect.Call(schoolMetricStudentListRepository.GetAll()).Return(GetSchoolMetricStudentList());

            suppliedMetadataColumnGroupList = GetSuppliedListMetadata();
            Expect.Call(listMetadataProvider.GetListMetadata(1)).IgnoreArguments().Repeat.Any().Return(suppliedMetadataColumnGroupList);

            Expect.Call(classroomMetricsProvider.GetAdditionalMetrics(null, null))
                .IgnoreArguments()
                .Repeat.Any()
                .Return(new List<StudentWithMetrics.Metric>());

            expectedStudentIds.Clear();
            expectedStudentIds.Add(suppliedStudentId1);
            expectedStudentIds.Add(suppliedStudentId2);
            expectedStudentIds.Add(suppliedStudentId3);
            expectedStudentIds.Add(suppliedStudentId4);

            expectedSchoolCategory = SchoolCategory.HighSchool;

            base.EstablishContext();
        }

        protected List<MetadataColumnGroup> GetSuppliedListMetadata()
        {
            return new List<MetadataColumnGroup>
                       {
                           new MetadataColumnGroup
                               {
                                   GroupType = GroupType.MetricData,
                                   IsVisibleByDefault = true,
                                   IsFixedColumnGroup = true,
                                   Title = "Some Title 01",
                                   UniqueId = 1,
                                   Columns = new List<MetadataColumn>
                                                 {
                                                     new MetadataColumn
                                                         {
                                                             UniqueIdentifier = suppliedUniqueIdentifier,
                                                             ColumnName = "Metric Value",
                                                             ColumnPrefix = "ColumnPrefix",
                                                             IsVisibleByDefault = true,
                                                             MetricVariantId = 1,
                                                             MetricListCellType = MetricListCellType.TrendMetric,
                                                             Order = 1,
                                                             SchoolCategory = SchoolCategory.HighSchool,
                                                             SortAscending = "SortAscending",
                                                             SortDescending = "SortDescending",
                                                             Tooltip = "Tooltip",
                                                             IsFixedColumn = true
                                                         }
                                                 }
                               }
                       };
        }

        protected IQueryable<EnhancedStudentInformation> GetSuppliedStudentList()
        {
            var data = new List<EnhancedStudentInformation>
                           {
                               new EnhancedStudentInformation{ StudentUSI = suppliedStudentId1, SchoolId = suppliedSchoolId, FirstName = "First 1", LastSurname = "Last 1", MiddleName = "Middle 1", ProfileThumbnail = "thumbnail", Gender = "female" },
                               new EnhancedStudentInformation{ StudentUSI = suppliedStudentId2, SchoolId = suppliedSchoolId, FirstName = "First 2", LastSurname = "Last 2", MiddleName = "Middle 2", ProfileThumbnail = "thumbnail", Gender = "female" },
                               new EnhancedStudentInformation{ StudentUSI = suppliedStudentId3, SchoolId = suppliedSchoolId, FirstName = "First 3", LastSurname = "Last 3", MiddleName = "Middle 3", ProfileThumbnail = "thumbnail", Gender = "female" },
                               new EnhancedStudentInformation{ StudentUSI = suppliedStudentId4, SchoolId = suppliedSchoolId, FirstName = "First 4", LastSurname = "Last 4", MiddleName = "Middle 4", ProfileThumbnail = "thumbnail", Gender = "female" },
                               };
            return data.AsQueryable();
        }

        protected MetricMetadataNode GetSuppliedMetricMetadataNode()
        {
            return new MetricMetadataNode(null) { MetricId = suppliedMetricId, MetricVariantId = suppliedMetricVariantId, ListFormat = suppliedMetricFormat, ListDataLabel = suppliedListDataLabel };
        }

        protected IQueryable<StudentMetric> GetSuppliedSchoolMetricStudentList()
        {
            var list = new List<StudentMetric>
                                                     {
                                                         new StudentMetric {StudentUSI = suppliedStudentId1, SchoolId = suppliedSchoolId, MetricId = suppliedMetricId, Value = "6.6", ValueTypeName = "System.Double"},
                                                         new StudentMetric {StudentUSI = suppliedStudentId2, SchoolId = suppliedSchoolId, MetricId = suppliedMetricId, Value = "7.7", ValueTypeName = "System.Double"},
                                                         new StudentMetric {StudentUSI = suppliedStudentId3, SchoolId = suppliedSchoolId, MetricId = suppliedMetricId, Value = "8.8", ValueTypeName = "System.Double"},
                                                         new StudentMetric {StudentUSI = suppliedStudentId4, SchoolId = suppliedSchoolId, MetricId = suppliedMetricId, Value = "9.9", ValueTypeName = "System.Double"},
                                                     };
            return list.AsQueryable();
        }

        protected IQueryable<StaffStudentAssociation> GetSuppliedStaffStudentAssociationList()
        {
            var list = new List<StaffStudentAssociation>
                                                     {
                                                         new StaffStudentAssociation {StudentUSI = suppliedStudentId3, SchoolId = suppliedSchoolId},
                                                         new StaffStudentAssociation {StudentUSI = suppliedStudentId4, SchoolId = suppliedSchoolId},
                                                         new StaffStudentAssociation {StudentUSI = suppliedStudentId1, SchoolId = suppliedSchoolId},
                                                         new StaffStudentAssociation {StudentUSI = suppliedStudentId2, SchoolId = suppliedSchoolId},
                                                         new StaffStudentAssociation {StudentUSI = suppliedStudentId3, SchoolId = suppliedSchoolId},
                                                     };
            return list.AsQueryable();
        }

        protected IQueryable<StudentMetric> GetStudentListPageData()
        {
            return GetSuppliedSchoolMetricStudentList()
                .Where(cmsl => cmsl.MetricId == suppliedMetricId && cmsl.SchoolId == suppliedSchoolId)
                .Join(GetSuppliedStudentList().Where(sl => sl.SchoolId == null || sl.SchoolId == suppliedSchoolId),
                      cmsl => cmsl.StudentUSI,
                      sl => sl.StudentUSI,
                      (cmsl, sl) => new { cmsl, sl }).Select(student => student.cmsl);
        }

        protected IQueryable<MetricInstanceFootnote> GetSuppliedMetricFootnotes()
        {
            var data = new List<MetricInstanceFootnote>
                           {
                               new MetricInstanceFootnote { MetricInstanceSetKey = suppliedMetricInstanceSetKey, MetricId = suppliedMetricId, FootnoteTypeId = 2, FootnoteText = suppliedFootnoteText3 },
                               new MetricInstanceFootnote { MetricInstanceSetKey = suppliedMetricInstanceSetKey, MetricId = suppliedMetricId, FootnoteTypeId = 1, FootnoteText = suppliedFootnoteText1 },
                               new MetricInstanceFootnote { MetricInstanceSetKey = suppliedMetricInstanceSetKey, MetricId = suppliedMetricId, FootnoteTypeId = 2, FootnoteText = suppliedFootnoteText2 },
                               new MetricInstanceFootnote { MetricInstanceSetKey = suppliedMetricInstanceSetKey, MetricId = suppliedMetricId + 1, FootnoteTypeId = 1, FootnoteText = "wrong data" },
                               new MetricInstanceFootnote { MetricInstanceSetKey = Guid.NewGuid(), MetricId = suppliedMetricId, FootnoteTypeId = 3, FootnoteText = "wrong data" },
                           };
            return data.AsQueryable();
        }

        protected virtual IQueryable<SchoolMetricStudentList> GetSchoolMetricStudentList()
        {
            return new[]
                       {
                           new SchoolMetricStudentList{StudentUSI = suppliedStudentId1, MetricId = suppliedMetricId, SchoolId = suppliedSchoolId, Value = 6.6m.ToString(), ValueType = "System.Decimal"},
                           new SchoolMetricStudentList{StudentUSI = suppliedStudentId2, MetricId = suppliedMetricId, SchoolId = suppliedSchoolId, Value = 6.6m.ToString(), ValueType = "System.Decimal"},
                           new SchoolMetricStudentList{StudentUSI = suppliedStudentId3, MetricId = suppliedMetricId, SchoolId = suppliedSchoolId, Value = 6.6m.ToString(), ValueType = "System.Decimal"},
                           new SchoolMetricStudentList{StudentUSI = suppliedStudentId4, MetricId = suppliedMetricId, SchoolId = suppliedSchoolId, Value = 6.6m.ToString(), ValueType = "System.Decimal"},
                       }.AsQueryable();
        }

        protected override void ExecuteTest()
        {
            var service = new StudentMetricListService(
                                                            studentSchoolAreaLinksFake,
                                                            metricCorrelationService,
                                                            schoolCategoryProvider,
                                                            listMetadataProvider,
                                                            metadataListIdResolver,
                                                            metricNodeResolver,
                                                            StudentMetricProvider,
                                                            classroomMetricsProvider,
                                                            gradeLevelUtilitiesProvider,
                                                            schoolMetricStudentListRepository
                                                        );

            actualModel = service.Get(StudentMetricListRequest.Create(suppliedSchoolId, suppliedStaffUSI, suppliedMetricVariantId, suppliedPageNumber, suppliedPageSize, suppliedSortColumn, suppliedSortDirection, suppliedVisibleColumns, suppliedUniqueListId));
        }

        [Test]
        public void Should_load_correct_students()
        {
            Assert.That(actualModel.Students.Count, Is.EqualTo(expectedStudentIds.Count));
            foreach (var student in actualModel.Students)
                Assert.That(expectedStudentIds.Contains(student.StudentUSI), Is.True, "Student USI " + student.StudentUSI + " was missing.");
        }

        [Test]
        public void Should_replace_metric_value_column_name()
        {
            Assert.That(actualModel.ListMetadata[0].Columns[0].ColumnName, Is.EqualTo(suppliedListDataLabel));
        }

        [Test]
        public void Should_have_no_unassigned_values_on_presentation_model()
        {
            actualModel.EnsureNoDefaultValues("StudentMetricListModel.Students[0].IsFlagged",
                                              "StudentMetricListModel.Students[1].IsFlagged",
                                              "StudentMetricListModel.Students[2].IsFlagged",
                                              "StudentMetricListModel.Students[3].IsFlagged",
                                              "StudentMetricListModel.Students[4].IsFlagged",
                                              "StudentMetricListModel.Students[0].ThumbNail",
                                              "StudentMetricListModel.Students[1].ThumbNail",
                                              "StudentMetricListModel.Students[2].ThumbNail",
                                              "StudentMetricListModel.Students[3].ThumbNail",
                                              "StudentMetricListModel.Students[4].ThumbNail",
                                              "StudentMetricListModel.Students[0].SchoolName",
                                              "StudentMetricListModel.Students[1].SchoolName",
                                              "StudentMetricListModel.Students[2].SchoolName",
                                              "StudentMetricListModel.Students[3].SchoolName",
                                              "StudentMetricListModel.Students[4].SchoolName",
                                              "StudentMetricListModel.MetricFootnotes[0].FootnoteNumber",
                                              "StudentMetricListModel.MetricFootnotes[1].FootnoteNumber",
                                              "StudentMetricListModel.Students[0].Metrics",
                                              "StudentMetricListModel.EntityIds[0].IsReadOnly",
                                              "StudentMetricListModel.EntityIds[0].IsSynchronized",
                                              "StudentMetricListModel.EntityIds[1].IsReadOnly",
                                              "StudentMetricListModel.EntityIds[1].IsSynchronized",
                                              "StudentMetricListModel.EntityIds[2].IsReadOnly",
                                              "StudentMetricListModel.EntityIds[2].IsSynchronized",
                                              "StudentMetricListModel.EntityIds[3].IsReadOnly",
                                              "StudentMetricListModel.EntityIds[3].IsSynchronized");
        }

        [Test]
        public virtual void Should_have_serializable_model()
        {
            actualModel.EnsureSerializableModel();
        }
    }

    public class When_loading_student_metric_metadata_list : StudentMetricListServiceFixtureBase
    {
        protected override void ExecuteTest()
        {
            base.ExecuteTest();

            var metaService = new StudentMetricListMetaService(
                                                            footnoteRepository,
                                                            schoolCategoryProvider,
                                                            metricInstanceSetKeyResolver,
                                                            listMetadataProvider,
                                                            metadataListIdResolver,
                                                            metricNodeResolver
                                                            );

            metaModel = metaService.Get(StudentMetricListMetaRequest.Create(suppliedSchoolId, suppliedMetricVariantId));
        }

        protected override void EstablishContext()
        {
            schoolCategoryProvider = mocks.StrictMock<ISchoolCategoryProvider>();
            metricInstanceSetKeyResolver = mocks.StrictMock<IMetricInstanceSetKeyResolver<SchoolMetricInstanceSetRequest>>();
            staffStudentAssociation = mocks.StrictMock<IRepository<StaffStudentAssociation>>();

            base.EstablishContext();

            Expect.Call(gradeLevelUtilitiesProvider.FormatGradeLevelForDisplay("1")).IgnoreArguments().Repeat.AtLeastOnce().Return("");
            Expect.Call(gradeLevelUtilitiesProvider.FormatGradeLevelForSorting("1")).IgnoreArguments().Repeat.AtLeastOnce().Return(1);

            Expect.Call(schoolCategoryProvider.GetSchoolCategoryType(suppliedSchoolId)).Repeat.AtLeastOnce().Return(SchoolCategory.HighSchool);
            Expect.Call(metricInstanceSetKeyResolver.GetMetricInstanceSetKey(null))
                .Constraints(
                    new ActionConstraint<SchoolMetricInstanceSetRequest>(x =>
                    {
                        Assert.That(x.SchoolId == suppliedSchoolId);
                        Assert.That(x.MetricVariantId == suppliedMetricVariantId);
                    })
                ).Return(suppliedMetricInstanceSetKey);
        }

        [Test]
        public void Should_load_correct_footnotes()
        {
            Assert.That(metaModel.MetricFootnotes.Count, Is.EqualTo(2));
            Assert.That(metaModel.MetricFootnotes[0].FootnoteText, Is.EqualTo(suppliedFootnoteText3));
            Assert.That(metaModel.MetricFootnotes[1].FootnoteText, Is.EqualTo(suppliedFootnoteText2));
        }
    }

    public class When_loading_student_metric_list_for_high_school_teacher_section : StudentMetricListServiceFixtureBase
    {
        protected override void EstablishContext()
        {
            base.EstablishContext();

            expectedMetricCount = 30;

            Expect.Call(schoolCategoryProvider.GetSchoolCategoryType(suppliedSchoolId)).Return(SchoolCategory.HighSchool);
            Expect.Call(gradeLevelUtilitiesProvider.FormatGradeLevelForDisplay("1")).IgnoreArguments().Repeat.AtLeastOnce().Return("");
            Expect.Call(gradeLevelUtilitiesProvider.FormatGradeLevelForSorting("1")).IgnoreArguments().Repeat.AtLeastOnce().Return(1);

        }

        [Test]
        public void Should_instantiate_value_correctly()
        {
            Assert.That(actualModel.Students[0].PrimaryMetricValue, Is.EqualTo(6.6m));
            Assert.That(actualModel.Students[0].PrimaryMetricDisplayValue, Is.EqualTo("660.00 % test"));
        }
    }

    public class When_loading_student_metric_list_for_middle_school_teacher_section : StudentMetricListServiceFixtureBase
    {
        protected override void EstablishContext()
        {
            base.EstablishContext();

            expectedSchoolCategory = SchoolCategory.MiddleSchool;
            expectedMetricCount = 26;

            Expect.Call(schoolCategoryProvider.GetSchoolCategoryType(suppliedSchoolId)).Return(SchoolCategory.MiddleSchool);
            Expect.Call(gradeLevelUtilitiesProvider.FormatGradeLevelForDisplay("1")).IgnoreArguments().Repeat.AtLeastOnce().Return("");
            Expect.Call(gradeLevelUtilitiesProvider.FormatGradeLevelForSorting("1")).IgnoreArguments().Repeat.AtLeastOnce().Return(1);

        }
    }

    public class When_loading_student_metric_list_for_elementary_school_teacher_section : StudentMetricListServiceFixtureBase
    {
        protected override void EstablishContext()
        {
            base.EstablishContext();

            expectedSchoolCategory = SchoolCategory.Elementary;
            expectedMetricCount = 21;

            Expect.Call(schoolCategoryProvider.GetSchoolCategoryType(suppliedSchoolId)).Return(SchoolCategory.Elementary);
            Expect.Call(gradeLevelUtilitiesProvider.FormatGradeLevelForDisplay("1")).IgnoreArguments().Repeat.AtLeastOnce().Return("");
            Expect.Call(gradeLevelUtilitiesProvider.FormatGradeLevelForSorting("1")).IgnoreArguments().Repeat.AtLeastOnce().Return(1);

        }
    }
}