using System.Collections.Generic;
using System.Linq;
using EdFi.Dashboards.Core;
using EdFi.Dashboards.Data.Entities;
using EdFi.Dashboards.Metric.Resources.Models;
using EdFi.Dashboards.Metric.Resources.Providers;
using EdFi.Dashboards.Resources;
using EdFi.Dashboards.Resources.Common;
using EdFi.Dashboards.Resources.Metric;
using EdFi.Dashboards.Resources.Models.CustomGrid;
using EdFi.Dashboards.Resources.Models.Student;
using EdFi.Dashboards.Resources.School;
using EdFi.Dashboards.Resources.Staff;
using EdFi.Dashboards.Resources.StudentMetrics;
using EdFi.Dashboards.Resources.StudentSchool;
using EdFi.Dashboards.Resources.Tests.Navigation.Fakes;
using EdFi.Dashboards.Testing;
using EdFi.Dashboards.Warehouse.Data.Entities;
using EdFi.Dashboards.Warehouse.Resource.Models.School.Detail;
using EdFi.Dashboards.Warehouse.Resources.Application;
using EdFi.Dashboards.Warehouse.Resources.School.Detail;
using EdFi.Dashboards.Warehouse.Resources.Staff;
using NUnit.Framework;
using Rhino.Mocks;

namespace EdFi.Dashboards.Warehouse.Resources.Tests.School.Detail
{
    public abstract class PriorYearStudentMetricListServiceFixtureBase : TestFixtureBase
    {
        protected const int suppliedStudentId1 = 160;
        protected const int suppliedStudentId2 = 170;
        protected const int suppliedStudentId3 = 180;
        protected const int suppliedStudentId4 = 190;
        protected const int suppliedStudentId5 = 200;
        protected const int suppliedStudentId6 = 210;
        protected const int suppliedStudentId7 = 220;
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

        protected const string noLongerEnrolledFootnoteFormat =
            "{0} students excluded because they are no longer enrolled.";

        protected ISchoolCategoryProvider schoolCategoryProvider;
        protected IUniqueListIdProvider uniqueListProvider;
        protected IMetricCorrelationProvider metricCorrelationService;
        protected IPriorYearClassroomMetricsProvider classroomMetricsProvider;
        protected IListMetadataProvider listMetadataProvider;
        protected IMetadataListIdResolver metadataListIdResolver;
        protected IMetricNodeResolver metricNodeResolver;
        protected IWarehouseAvailabilityProvider warehouseAvailabilityProvider;
        protected IMaxPriorYearProvider maxPriorYearProvider;
        protected IGradeLevelUtilitiesProvider gradeLevelUtilitiesProvider;
        protected IPriorYearStudentMetricsProvider priorYearStudentMetricsProvider;
        protected IStudentMetricsProvider studentMetricsProvider;

        protected StudentPriorYearMetricListModel actualModel;

        protected readonly List<long> expectedStudentIds = new List<long>();
        protected int expectedMissingCount;
        protected string expectedSubjectArea;
        protected SchoolCategory expectedSchoolCategory;
        protected int expectedMetricCount;
        protected readonly StudentSchoolAreaLinksFake studentSchoolAreaLinksFake = new StudentSchoolAreaLinksFake();
        protected const string suppliedListDataLabel = "supplied list data label";

        protected override void EstablishContext()
        {
            schoolCategoryProvider = mocks.StrictMock<ISchoolCategoryProvider>();
            uniqueListProvider = mocks.StrictMock<IUniqueListIdProvider>();
            metricCorrelationService = mocks.StrictMock<IMetricCorrelationProvider>();
            classroomMetricsProvider = mocks.StrictMock<IPriorYearClassroomMetricsProvider>();
            listMetadataProvider = mocks.StrictMock<IListMetadataProvider>();
            metadataListIdResolver = mocks.StrictMock<IMetadataListIdResolver>();
            metricNodeResolver = mocks.StrictMock<IMetricNodeResolver>();
            warehouseAvailabilityProvider = mocks.StrictMock<IWarehouseAvailabilityProvider>();
            maxPriorYearProvider = mocks.StrictMock<IMaxPriorYearProvider>();
            gradeLevelUtilitiesProvider = mocks.StrictMock<IGradeLevelUtilitiesProvider>();
            priorYearStudentMetricsProvider = mocks.StrictMock<IPriorYearStudentMetricsProvider>();
            studentMetricsProvider = mocks.StrictMock<IStudentMetricsProvider>();

            Expect.Call(maxPriorYearProvider.Get(suppliedLocalEducationAgencyId)).Repeat.Any().Return(2011);
            Expect.Call(metricNodeResolver.GetMetricNodeForSchoolFromMetricVariantId(suppliedSchoolId,
                suppliedMetricVariantId)).Repeat.Any().Return(GetSuppliedMetricMetadataNode());
            Expect.Call(metricCorrelationService.GetRenderingParentMetricVariantIdForStudent(suppliedMetricVariantId,
                suppliedSchoolId)).Repeat.Any()
                .Return(new MetricCorrelationProvider.MetricRenderingContext
                {
                    MetricVariantId = suppliedRenderingMetricVariantId,
                    ContextMetricVariantId = suppliedContextMetricVariantId
                });
            Expect.Call(uniqueListProvider.GetUniqueId(suppliedMetricVariantId)).Repeat.Any().Return(suppliedUniqueListId);
            Expect.Call(metadataListIdResolver.GetListId(ListType.ClassroomGeneralOverview, SchoolCategory.HighSchool))
                .IgnoreArguments()
                .Repeat.Any()
                .Return(1);
            Expect.Call(listMetadataProvider.GetListMetadata(1))
                .IgnoreArguments()
                .Repeat.Any()
                .Return(GetSuppliedListMetadata());
            Expect.Call(priorYearStudentMetricsProvider.GetStudentList(GetPriorYearStudentListWithMetricsQueryOptions()))
                .IgnoreArguments()
                .Repeat.Any()
                .Return(GetExpectedPriorYearStudentList());
            Expect.Call(studentMetricsProvider.GetOrderedStudentList(GetStudentListWithMetricsQueryOptions()))
                .IgnoreArguments()
                .Repeat.Any()
                .Return(
                    GetStudentSectionEntityListData()
                        .Where(x => expectedStudentIds.Contains(x.StudentUSI) && x.SchoolId == suppliedSchoolId)
                        .AsQueryable());
            Expect.Call(
                priorYearStudentMetricsProvider.GetStudentsWithMetrics(GetPriorYearStudentListWithMetricsQueryOptions()))
                .IgnoreArguments()
                .Repeat.Any()
                .Return(
                    GetSuppliedPriorYearSchoolMetricStudentList()
                        .Where(x => expectedStudentIds.Contains(x.StudentUSI) && x.SchoolId == suppliedSchoolId));
            Expect.Call(studentMetricsProvider.GetStudentsWithMetrics(GetStudentListWithMetricsQueryOptions()))
                .IgnoreArguments()
                .Repeat.Any()
                .Return(
                    GetStudentListSectionPageData()
                        .Where(x => expectedStudentIds.Contains(x.StudentUSI) && x.SchoolId == suppliedSchoolId)
                        .AsQueryable());
            Expect.Call(classroomMetricsProvider.GetAdditionalMetrics(null, null, null))
                .IgnoreArguments()
                .Repeat.Any()
                .Return(new List<StudentWithMetrics.Metric>());

            base.EstablishContext();
        }

        protected virtual IQueryable<SchoolMetricInstanceStudentList> GetSuppliedPriorYearStudentList()
        {
            var list = new List<SchoolMetricInstanceStudentList>
            {
                new SchoolMetricInstanceStudentList
                {
                    StudentUSI = suppliedStudentId1,
                    SchoolId = suppliedSchoolId,
                    SchoolYear = 2011,
                    MetricId = suppliedMetricId,
                    Value = "6.6",
                    ValueType = "System.Double"
                },
                new SchoolMetricInstanceStudentList
                {
                    StudentUSI = suppliedStudentId2,
                    SchoolId = suppliedSchoolId,
                    SchoolYear = 2011,
                    MetricId = suppliedMetricId,
                    Value = "7.7",
                    ValueType = "System.Double"
                },
                new SchoolMetricInstanceStudentList
                {
                    StudentUSI = suppliedStudentId3,
                    SchoolId = suppliedSchoolId,
                    SchoolYear = 2011,
                    MetricId = suppliedMetricId,
                    Value = "8.8",
                    ValueType = "System.Double"
                },
                new SchoolMetricInstanceStudentList
                {
                    StudentUSI = suppliedStudentId6,
                    SchoolId = suppliedSchoolId,
                    SchoolYear = 2011,
                    MetricId = suppliedMetricId,
                    Value = "9.9",
                    ValueType = "System.Double"
                },
                new SchoolMetricInstanceStudentList
                {
                    StudentUSI = suppliedStudentId5,
                    SchoolId = suppliedSchoolId,
                    SchoolYear = 2011,
                    MetricId = suppliedMetricId,
                    Value = "5.5",
                    ValueType = "System.Double"
                },
                new SchoolMetricInstanceStudentList
                {
                    StudentUSI = suppliedStudentId7,
                    SchoolId = suppliedSchoolId,
                    SchoolYear = 2010,
                    MetricId = suppliedMetricId,
                    Value = "4.4",
                    ValueType = "System.Double"
                },
            };
            return list.AsQueryable();
        }

        protected virtual IQueryable<SchoolMetricInstanceStudentList> GetExpectedPriorYearStudentList()
        {
            return
                GetSuppliedPriorYearStudentList()
                    .Where(x => expectedStudentIds.Contains(x.StudentUSI) && x.SchoolId == suppliedSchoolId);
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
                new EnhancedStudentInformation
                {
                    StudentUSI = suppliedStudentId1,
                    SchoolId = suppliedSchoolId + 1,
                    FirstName = "First 1",
                    LastSurname = "Last 1",
                    MiddleName = "Middle 1",
                    ProfileThumbnail = "thumbnail",
                    Gender = "female"
                },
                new EnhancedStudentInformation
                {
                    StudentUSI = suppliedStudentId2,
                    SchoolId = suppliedSchoolId,
                    FirstName = "First 2",
                    LastSurname = "Last 2",
                    MiddleName = "Middle 2",
                    ProfileThumbnail = "thumbnail",
                    Gender = "female"
                },
                new EnhancedStudentInformation
                {
                    StudentUSI = suppliedStudentId3,
                    SchoolId = suppliedSchoolId,
                    FirstName = "First 3",
                    LastSurname = "Last 3",
                    MiddleName = "Middle 3",
                    ProfileThumbnail = "thumbnail",
                    Gender = "female"
                },
                new EnhancedStudentInformation
                {
                    StudentUSI = suppliedStudentId4,
                    SchoolId = suppliedSchoolId + 1,
                    FirstName = "First 4",
                    LastSurname = "Last 4",
                    MiddleName = "Middle 4",
                    ProfileThumbnail = "thumbnail",
                    Gender = "female"
                },
                new EnhancedStudentInformation
                {
                    StudentUSI = suppliedStudentId6,
                    SchoolId = suppliedSchoolId + 1,
                    FirstName = "First 6",
                    LastSurname = "Last 6",
                    MiddleName = "Middle 6",
                    ProfileThumbnail = "thumbnail",
                    Gender = "female"
                },
                new EnhancedStudentInformation
                {
                    StudentUSI = suppliedStudentId7,
                    SchoolId = suppliedSchoolId,
                    FirstName = "First 7",
                    LastSurname = "Last 7",
                    MiddleName = "Middle 7",
                    ProfileThumbnail = "thumbnail",
                    Gender = "female"
                },
            };
            return data.AsQueryable();
        }

        protected virtual IQueryable<EnhancedStudentInformation> GetExpectedStudentList()
        {
            return
                GetSuppliedStudentList()
                    .Where(x => expectedStudentIds.Contains(x.StudentUSI) && x.SchoolId == suppliedSchoolId);
        }

        protected MetricMetadataNode GetSuppliedMetricMetadataNode()
        {
            return new MetricMetadataNode(null)
            {
                MetricId = suppliedMetricId,
                MetricVariantId = suppliedMetricVariantId,
                ListFormat = suppliedMetricFormat,
                ListDataLabel = suppliedListDataLabel
            };
        }

        protected IQueryable<StudentSchoolMetricInstance> GetSuppliedPriorYearSchoolMetricStudentList()
        {
            var list = new List<StudentSchoolMetricInstance>
            {
                new StudentSchoolMetricInstance
                {
                    StudentUSI = suppliedStudentId3,
                    SchoolId = suppliedSchoolId,
                    SchoolYear = 2011,
                    MetricId = suppliedMetricId + 1,
                    Value = "wrong data",
                    ValueTypeName = "System.String"
                },
                new StudentSchoolMetricInstance
                {
                    StudentUSI = suppliedStudentId4,
                    SchoolId = suppliedSchoolId + 1,
                    SchoolYear = 2011,
                    MetricId = suppliedMetricId,
                    Value = "wrong data",
                    ValueTypeName = "System.String"
                },
                new StudentSchoolMetricInstance
                {
                    StudentUSI = suppliedStudentId1,
                    SchoolId = suppliedSchoolId,
                    SchoolYear = 2011,
                    MetricId = suppliedMetricId,
                    Value = "6.6",
                    ValueTypeName = "System.Double"
                },
                new StudentSchoolMetricInstance
                {
                    StudentUSI = suppliedStudentId2,
                    SchoolId = suppliedSchoolId,
                    SchoolYear = 2011,
                    MetricId = suppliedMetricId,
                    Value = "7.7",
                    ValueTypeName = "System.Double"
                },
                new StudentSchoolMetricInstance
                {
                    StudentUSI = suppliedStudentId3,
                    SchoolId = suppliedSchoolId,
                    SchoolYear = 2011,
                    MetricId = suppliedMetricId,
                    Value = "8.8",
                    ValueTypeName = "System.Double"
                },
                new StudentSchoolMetricInstance
                {
                    StudentUSI = suppliedStudentId6,
                    SchoolId = suppliedSchoolId,
                    SchoolYear = 2011,
                    MetricId = suppliedMetricId,
                    Value = "9.9",
                    ValueTypeName = "System.Double"
                },
                new StudentSchoolMetricInstance
                {
                    StudentUSI = suppliedStudentId7,
                    SchoolId = suppliedSchoolId,
                    SchoolYear = 2011,
                    MetricId = suppliedMetricId,
                    Value = "9.9",
                    ValueTypeName = "System.Double"
                },
                new StudentSchoolMetricInstance
                {
                    StudentUSI = suppliedStudentId7,
                    SchoolId = suppliedSchoolId,
                    SchoolYear = 2010,
                    MetricId = suppliedMetricId,
                    Value = "9.9",
                    ValueTypeName = "System.Double"
                },
            };
            return list.AsQueryable();
        }

        protected List<EnhancedStudentInformation> GetStudentSectionEntityListData()
        {
            var studentIds = GetSuppliedPriorYearSchoolMetricStudentList().Select(x => x.StudentUSI);
            return (from sl in GetSuppliedStudentList()
                    where studentIds.Contains(sl.StudentUSI) && sl.SchoolId == suppliedSchoolId
                    select sl).ToList();
        }

        private IEnumerable<StudentMetric> GetStudentListSectionPageData()
        {
            var studentIds = GetSuppliedStudentList().Select(x => x.StudentUSI);

            return from sl in GetSuppliedStudentMetrics()
                   where studentIds.Contains(sl.StudentUSI) && sl.SchoolId == suppliedSchoolId
                   select sl;
        }

        protected IQueryable<StudentMetric> GetSuppliedStudentMetrics()
        {
            var data = new List<StudentMetric>
            {
                new StudentMetric {StudentUSI = suppliedStudentId1, SchoolId = suppliedSchoolId + 1},
                new StudentMetric {StudentUSI = suppliedStudentId2, SchoolId = suppliedSchoolId},
                new StudentMetric {StudentUSI = suppliedStudentId3, SchoolId = suppliedSchoolId},
                new StudentMetric {StudentUSI = suppliedStudentId4, SchoolId = suppliedSchoolId},
                new StudentMetric {StudentUSI = suppliedStudentId5, SchoolId = suppliedSchoolId},
                new StudentMetric {StudentUSI = suppliedStudentId6, SchoolId = suppliedSchoolId},
                new StudentMetric {StudentUSI = suppliedStudentId7, SchoolId = suppliedSchoolId},
            };
            return data.AsQueryable();
        }

        protected StudentMetricsProviderQueryOptions GetStudentListWithMetricsQueryOptions()
        {
            var suppliedStudentUSIs = new List<long>();
            const int suppliedStaffUsi = 0;
            return new StudentMetricsProviderQueryOptions
            {
                SchoolId = suppliedSchoolId,
                StaffUSI = suppliedStaffUsi,
                StudentIds = suppliedStudentUSIs,
            };
        }

        protected PriorYearStudentMetricsProviderQueryOptions GetPriorYearStudentListWithMetricsQueryOptions()
        {
            var suppliedStudentUSIs = new List<long>();
            return new PriorYearStudentMetricsProviderQueryOptions
            {
                SchoolId = suppliedSchoolId,
                StudentIds = suppliedStudentUSIs,
            };
        }

        protected override void ExecuteTest()
        {
            var service = new StudentPriorYearMetricListService(uniqueListProvider,
                metricCorrelationService,
                schoolCategoryProvider,
                studentSchoolAreaLinksFake,
                classroomMetricsProvider,
                listMetadataProvider,
                metadataListIdResolver,
                metricNodeResolver,
                warehouseAvailabilityProvider,
                maxPriorYearProvider,
                gradeLevelUtilitiesProvider,
                priorYearStudentMetricsProvider,
                studentMetricsProvider);

            actualModel =
                service.Get(StudentPriorYearMetricListRequest.Create(suppliedSchoolId, suppliedLocalEducationAgencyId,
                    suppliedMetricVariantId));
        }
    }

    public abstract class PriorYearStudentMetricListServiceSchoolCategoryFixtureBase : PriorYearStudentMetricListServiceFixtureBase
    {
        protected override void EstablishContext()
        {
            base.EstablishContext();
            Expect.Call(warehouseAvailabilityProvider.Get()).Return(true);
        }


        [Test]
        public void Should_load_correct_students()
        {
            var expectedStudents = GetExpectedStudentList();
            Assert.That(actualModel.Students.Count, Is.EqualTo(expectedStudents.Count()));
            foreach (var student in actualModel.Students)
                Assert.That(expectedStudents.Select(x => x.StudentUSI).Contains(student.StudentUSI), Is.True, "Student USI " + student.StudentUSI + " was missing.");
        }

        [Test]
        public void Should_have_expected_school_category()
        {
            Assert.That(actualModel.SchoolCategory, Is.EqualTo(expectedSchoolCategory));
        }

        [Test]
        public void Should_have_unique_list_id_populated()
        {
            Assert.That(actualModel.UniqueListId, Is.EqualTo(suppliedUniqueListId));
        }

        [Test]
        public void Should_replace_metric_value_column_name()
        {
            Assert.That(actualModel.ListMetadata[0].Columns[0].ColumnName, Is.EqualTo(suppliedListDataLabel));
        }

        [Test]
        public void Should_have_no_unassigned_values_on_presentation_model()
        {
            actualModel.EnsureNoDefaultValues("StudentPriorYearMetricListModel.Students[0].IsFlagged",
                                              "StudentPriorYearMetricListModel.Students[1].IsFlagged",
                                              "StudentPriorYearMetricListModel.Students[2].IsFlagged",
                                              "StudentPriorYearMetricListModel.Students[0].ThumbNail",
                                              "StudentPriorYearMetricListModel.Students[1].ThumbNail",
                                              "StudentPriorYearMetricListModel.Students[2].ThumbNail",
                                              "StudentPriorYearMetricListModel.Students[0].SchoolName",
                                              "StudentPriorYearMetricListModel.Students[1].SchoolName",
                                              "StudentPriorYearMetricListModel.Students[2].SchoolName",
                                              "StudentPriorYearMetricListModel.MetricFootnotes[0].FootnoteNumber",
                                              "StudentPriorYearMetricListModel.MetricFootnotes[1].FootnoteNumber",
                                              "StudentPriorYearMetricListModel.Students[0].Metrics");
        }

        [Test]
        public void Should_load_correct_footnotes()
        {
            Assert.That(actualModel.MetricFootnotes.Count, Is.EqualTo(1));
            Assert.That(actualModel.MetricFootnotes[0].FootnoteText, Is.EqualTo(string.Format(noLongerEnrolledFootnoteFormat, expectedMissingCount)));
        }

        [Test]
        public virtual void Should_have_serializable_model()
        {
            actualModel.EnsureSerializableModel();
        }
    }

    public class When_loading_prior_year_student_metric_list_for_high_school : PriorYearStudentMetricListServiceSchoolCategoryFixtureBase
    {
        protected override void EstablishContext()
        {
            expectedStudentIds.Clear();
            expectedStudentIds.Add(suppliedStudentId1);
            expectedStudentIds.Add(suppliedStudentId2);
            expectedStudentIds.Add(suppliedStudentId3);

            expectedMissingCount = 1;

            expectedSchoolCategory = SchoolCategory.HighSchool;
            expectedMetricCount = 30;

            base.EstablishContext();
            Expect.Call(gradeLevelUtilitiesProvider.FormatGradeLevelForDisplay("1")).IgnoreArguments().Repeat.AtLeastOnce().Return("");
            Expect.Call(gradeLevelUtilitiesProvider.FormatGradeLevelForSorting("1")).IgnoreArguments().Repeat.AtLeastOnce().Return(1);
            Expect.Call(schoolCategoryProvider.GetSchoolCategoryType(suppliedSchoolId)).Return(SchoolCategory.HighSchool);
        }

        [Test]
        public void Should_instantiate_value_correctly()
        {
            Assert.That(actualModel.Students[0].PrimaryMetricValue, Is.EqualTo(7.7m));
            Assert.That(actualModel.Students[0].PrimaryMetricDisplayValue, Is.EqualTo("770.00 % test"));
        }
    }

    public class When_loading_prior_year_student_metric_list_for_middle_school : PriorYearStudentMetricListServiceSchoolCategoryFixtureBase
    {
        protected override void EstablishContext()
        {
            expectedStudentIds.Clear();
            expectedStudentIds.Add(suppliedStudentId1);
            expectedStudentIds.Add(suppliedStudentId2);
            expectedStudentIds.Add(suppliedStudentId3);

            expectedMissingCount = 1;

            expectedSchoolCategory = SchoolCategory.MiddleSchool;
            expectedMetricCount = 26;

            base.EstablishContext();
            Expect.Call(gradeLevelUtilitiesProvider.FormatGradeLevelForDisplay("1")).IgnoreArguments().Repeat.AtLeastOnce().Return("");
            Expect.Call(gradeLevelUtilitiesProvider.FormatGradeLevelForSorting("1")).IgnoreArguments().Repeat.AtLeastOnce().Return(1);
            Expect.Call(schoolCategoryProvider.GetSchoolCategoryType(suppliedSchoolId)).Return(SchoolCategory.MiddleSchool);
        }
    }

    public class When_loading_prior_year_student_metric_list_for_elementary_school : PriorYearStudentMetricListServiceSchoolCategoryFixtureBase
    {
        protected override void EstablishContext()
        {
            expectedStudentIds.Clear();
            expectedStudentIds.Add(suppliedStudentId1);
            expectedStudentIds.Add(suppliedStudentId2);
            expectedStudentIds.Add(suppliedStudentId3);

            expectedMissingCount = 1;

            expectedSchoolCategory = SchoolCategory.Elementary;
            expectedMetricCount = 21;

            base.EstablishContext();
            Expect.Call(gradeLevelUtilitiesProvider.FormatGradeLevelForDisplay("1")).IgnoreArguments().Repeat.AtLeastOnce().Return("");
            Expect.Call(gradeLevelUtilitiesProvider.FormatGradeLevelForSorting("1")).IgnoreArguments().Repeat.AtLeastOnce().Return(1);
            Expect.Call(schoolCategoryProvider.GetSchoolCategoryType(suppliedSchoolId)).Return(SchoolCategory.Elementary);
        }
    }

    public class When_loading_prior_year_student_metric_list_with_no_students : PriorYearStudentMetricListServiceFixtureBase
    {
        protected override void EstablishContext()
        {
            expectedStudentIds.Clear();

            expectedMissingCount = 1;

            expectedSchoolCategory = SchoolCategory.Elementary;
            expectedMetricCount = 21;

            base.EstablishContext();
            Expect.Call(schoolCategoryProvider.GetSchoolCategoryType(suppliedSchoolId)).Return(SchoolCategory.Elementary);
            Expect.Call(warehouseAvailabilityProvider.Get()).Return(true);

        }

        protected override IQueryable<SchoolMetricInstanceStudentList> GetSuppliedPriorYearStudentList()
        {
            var list = new List<SchoolMetricInstanceStudentList>();
            return list.AsQueryable();
        }


        protected override void ExecuteTest()
        {
            var service = new StudentPriorYearMetricListService(uniqueListProvider,
                                                            metricCorrelationService,
                                                            schoolCategoryProvider,
                                                            studentSchoolAreaLinksFake,
                                                            classroomMetricsProvider,
                                                            listMetadataProvider,
                                                            metadataListIdResolver,
                                                            metricNodeResolver,
                                                            warehouseAvailabilityProvider,
                                                            maxPriorYearProvider,
                                                            gradeLevelUtilitiesProvider,
                                                            priorYearStudentMetricsProvider,
                                                            studentMetricsProvider);

            actualModel = service.Get(StudentPriorYearMetricListRequest.Create(suppliedSchoolId, suppliedLocalEducationAgencyId, suppliedMetricVariantId));
        }


        [Test]
        public virtual void Should_load_correct_students()
        {
            Assert.That(actualModel.Students.Count, Is.EqualTo(expectedStudentIds.Count));
            foreach (var student in actualModel.Students)
                Assert.That(expectedStudentIds.Contains(student.StudentUSI), Is.True, "Student USI " + student.StudentUSI + " was missing.");
        }

        [Test]
        public virtual void Should_have_expected_school_category()
        {
            Assert.That(actualModel.SchoolCategory, Is.EqualTo(expectedSchoolCategory));
        }

        [Test]
        public virtual void Should_have_serializable_model()
        {
            actualModel.EnsureSerializableModel();
        }

    }

    public class When_loading_prior_year_student_metric_list_but_warehouse_is_unavailable : PriorYearStudentMetricListServiceFixtureBase
    {

        protected override void EstablishContext()
        {
            base.EstablishContext();
            Expect.Call(warehouseAvailabilityProvider.Get()).Return(false);
        }


        protected override void ExecuteTest()
        {
            var service = new StudentPriorYearMetricListService(uniqueListProvider,
                                                            metricCorrelationService,
                                                            schoolCategoryProvider,
                                                            studentSchoolAreaLinksFake,
                                                            classroomMetricsProvider,
                                                            listMetadataProvider,
                                                            metadataListIdResolver,
                                                            metricNodeResolver,
                                                            warehouseAvailabilityProvider,
                                                            maxPriorYearProvider,
                                                            gradeLevelUtilitiesProvider,
                                                            priorYearStudentMetricsProvider,
                                                            studentMetricsProvider);

            actualModel = service.Get(StudentPriorYearMetricListRequest.Create(suppliedSchoolId, suppliedLocalEducationAgencyId, suppliedMetricVariantId));
        }

        [Test]
        public void Should_return_empty_model()
        {
            Assert.That(actualModel, Is.Not.Null);
        }
    }
}