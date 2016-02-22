using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using EdFi.Dashboards.Core;
using EdFi.Dashboards.Data.Entities;
using EdFi.Dashboards.Data.Repository;
using EdFi.Dashboards.Metric.Resources.Models;
using EdFi.Dashboards.Metric.Resources.Providers;
using EdFi.Dashboards.Metric.Resources.Services;
using EdFi.Dashboards.Resources.Models.Common;
using EdFi.Dashboards.Resources.Models.CustomGrid;
using EdFi.Dashboards.Resources.Models.LocalEducationAgency;
using EdFi.Dashboards.Resources.Models.Student;
using EdFi.Dashboards.Resources.Navigation;
using EdFi.Dashboards.Resources.LocalEducationAgency;
using EdFi.Dashboards.Resources.School;
using EdFi.Dashboards.Resources.Staff;
using EdFi.Dashboards.Resources.StudentSchool;
using EdFi.Dashboards.Resources.Tests.Navigation.Fakes;
using EdFi.Dashboards.Testing;
using NUnit.Framework;
using Rhino.Mocks;

namespace EdFi.Dashboards.Resources.Tests.LocalEducationAgency
{
    //public abstract class StudentSchoolCategoryListServiceFixtureBase : TestFixtureBase
    //{
    //    protected IRepository<SchoolInformation> schoolInformationRepository;
    //    protected IRepository<StudentInformation> studentInformationRepository;
    //    protected IRepository<StudentIndicator> studentIndicatorRepository;
    //    protected IMetadataListIdResolver metadataListIdResolver;
    //    protected IListMetadataProvider listMetadataProvider;
    //    protected IClassroomMetricsProvider classroomMetricsProvider;
    //    protected IStudentSchoolAreaLinks studentSchoolLinks;
    //    protected ISchoolCategoryProvider schoolCategoryProvider;
    //    protected IGradeLevelUtilitiesProvider gradeLevelUtilitiesProvider;
    //    protected IMetricStateProvider metricStateProvider;
    //    protected IStudentMetricsProvider studentListByCategoryProvider;

    //    protected int suppliedPageNumber = 1;
    //    protected int suppliedPageSize = 10;
    //    protected int? suppliedSortColumn;
    //    protected string suppliedSortDirection;
    //    protected string suppliedVisibleColumns;
    //    protected string suppliedUniqueListId;

    //    protected int suppliedLocalEducationAgencyId = 1;

    //    protected string requestedSchoolCategory;

    //    protected List<MetadataColumnGroup> suppliedMetadataColumnGroupList = new List<MetadataColumnGroup>
    //        {
    //            new MetadataColumnGroup {
    //                Title = "Supplied Metadata",
    //                GroupType = GroupType.MetricData,
    //                Columns = new List<MetadataColumn>()
    //                {
    //                    new MetadataColumn(){ UniqueIdentifier = 123, ColumnName = "foo", MetricVariantId = 1337 },
    //                    new MetadataColumn(){ UniqueIdentifier = 456, ColumnName = "foo", MetricVariantId = 1338, MetricListCellType = MetricListCellType.TrendMetric }
    //                }},
    //        };

    //    protected List<StudentWithMetrics.Metric> suppliedAdditionalMetrics = new List<StudentWithMetrics.Metric>
    //        {
    //            new StudentWithMetrics.Metric(123){ UniqueIdentifier = 123, DisplayValue = "", MetricVariantId = 1337 },
    //            new StudentWithMetrics.Metric(123){ UniqueIdentifier = 456, DisplayValue = "", MetricVariantId = 1338 }
    //        };

    //    protected bool expectStudentInformationQuery;
    //    protected int expectedStudentUSI;

    //    protected StudentSchoolCategoryListModel actualModel;
    //    protected StudentSchoolCategoryListMetaModel metaModel;
    //    private IStudentListUtilitiesProvider _studentUtilitiesProvider;
    //    private ITrendRenderingDispositionProvider _trendRenderingDispositionProvider;

    //    protected override void EstablishContext()
    //    {
    //        studentSchoolLinks = new StudentSchoolAreaLinksFake();
    //        schoolInformationRepository = mocks.StrictMock<IRepository<SchoolInformation>>();
    //        studentInformationRepository = mocks.StrictMock<IRepository<StudentInformation>>();
    //        studentIndicatorRepository = mocks.StrictMock<IRepository<StudentIndicator>>();
    //        metadataListIdResolver = mocks.StrictMock<IMetadataListIdResolver>();
    //        listMetadataProvider = mocks.StrictMock<IListMetadataProvider>();
    //        classroomMetricsProvider = mocks.StrictMock<IClassroomMetricsProvider>();
    //        schoolCategoryProvider = mocks.StrictMock<ISchoolCategoryProvider>();
    //        gradeLevelUtilitiesProvider = mocks.StrictMock<IGradeLevelUtilitiesProvider>();
    //        metricStateProvider = mocks.StrictMock<IMetricStateProvider>();
    //        _studentUtilitiesProvider = mocks.StrictMock<IStudentListUtilitiesProvider>();
    //        studentListByCategoryProvider = mocks.StrictMock<IStudentMetricsProvider>();
    //        _trendRenderingDispositionProvider = mocks.StrictMock<ITrendRenderingDispositionProvider>();

    //        Expect.Call(schoolInformationRepository.GetAll()).Repeat.Any().Return(GetSchoolInformation());
    //        Expect.Call(metadataListIdResolver.GetListId(ListType.StudentSchoolCategory, SchoolCategory.HighSchool)).Repeat.Any().Return(6);
    //        Expect.Call(listMetadataProvider.GetListMetadata(6)).Repeat.Any().IgnoreArguments().Return(suppliedMetadataColumnGroupList);
    //        Expect.Call(schoolCategoryProvider.GetSchoolCategoryType(requestedSchoolCategory)).Repeat.Any().Return(SchoolCategory.HighSchool);
    //        Expect.Call(metricStateProvider.GetState(0, String.Empty, String.Empty)).IgnoreArguments().Repeat.Any().Return(new State(MetricStateType.Neutral, "Neutral"));

    //        Expect.Call(_studentUtilitiesProvider.PrepareTrendMetric(0, 0, 0, 0, "", 0, "", 0, _trendRenderingDispositionProvider)).Repeat.Any().IgnoreArguments().Return(new StudentWithMetrics.TrendMetric());
    //        Expect.Call(_studentUtilitiesProvider.PrepareIndicatorMetric(0, 0, 0, 0, "", 0, "")).Repeat.Any().IgnoreArguments().Return(new StudentWithMetrics.IndicatorMetric());
    //        Expect.Call(_studentUtilitiesProvider.PrepareMetric(0, 0, 0, 0, "", 0, "")).Repeat.Any().IgnoreArguments().Return(new StudentWithMetrics.Metric());

    //        Expect.Call(studentListByCategoryProvider.GetOrderedStudentList(null, null, string.Empty)).IgnoreArguments().Repeat.Any().Return(GetStudentSchoolCategoryListData());
    //        Expect.Call(studentListByCategoryProvider.GetStudentsWithMetrics(null)).IgnoreArguments().Repeat.Any().Return(GetMetricList());

    //        Expect.Call(schoolCategoryProvider.GetSchoolCategoryType(0))
    //              .Repeat.Any()
    //              .IgnoreArguments()
    //              .Return(SchoolCategory.HighSchool);

    //        Expect.Call(metadataListIdResolver.GetListId(ListType.StudentGrade, SchoolCategory.None))
    //              .Repeat.Any()
    //              .IgnoreArguments()
    //              .Return(0);

    //        Expect.Call(classroomMetricsProvider.GetAdditionalMetrics(null, null))
    //              .Repeat.Any()
    //              .IgnoreArguments()
    //              .Return(new List<StudentWithMetrics.Metric>());

    //        Expect.Call(gradeLevelUtilitiesProvider.FormatGradeLevelForDisplay("1")).IgnoreArguments().Repeat.AtLeastOnce().Return("");
    //        Expect.Call(gradeLevelUtilitiesProvider.FormatGradeLevelForSorting("1")).IgnoreArguments().Repeat.AtLeastOnce().Return(1);

    //        base.EstablishContext();
    //    }

    //    protected IQueryable<EnhancedStudentInformation> GetStudentSchoolCategoryListData()
    //    {
    //        //get all the students for this school
    //        return (from sl in GetStudentList()
    //                join si in GetSchoolInformation() on sl.SchoolId equals si.SchoolId
    //                where si.LocalEducationAgencyId == 1000
    //                    && si.SchoolCategory == requestedSchoolCategory
    //                select sl).ToList().Distinct().AsQueryable();
    //    }

    //    protected IQueryable<StudentMetric> GetStudentSchoolCategoryListPageData()
    //    {
    //        //get all the students for this school
    //        var metrics = (from sl in GetMetricList()
    //                       join si in GetSchoolInformation() on sl.SchoolId equals si.SchoolId
    //                       where si.LocalEducationAgencyId == 1000
    //                           && si.SchoolCategory == requestedSchoolCategory
    //                       select sl);

    //        return metrics;
    //    }

    //    protected IQueryable<SchoolInformation> GetSchoolInformation()
    //    {
    //        var list = new List<SchoolInformation>
    //                       {
    //                           new SchoolInformation{ LocalEducationAgencyId = 1001, SchoolId = 2003, SchoolCategory = "School Category 1"},
    //                           new SchoolInformation{ LocalEducationAgencyId = 1000, SchoolId = 2001, SchoolCategory = "School Category 1" },
    //                           new SchoolInformation{ LocalEducationAgencyId = 1000, SchoolId = 2002, SchoolCategory = "School Category 2" },
    //                           new SchoolInformation{ LocalEducationAgencyId = 1000, SchoolId = 2004, SchoolCategory = "School Category 1" }
    //                       };
    //        return list.AsQueryable();
    //    }

    //    protected IQueryable<EnhancedStudentInformation> GetStudentList()
    //    {
    //        var list = new List<EnhancedStudentInformation>
    //                       {
    //                           new EnhancedStudentInformation{ StudentUSI = 1, SchoolId = 2001, SchoolName = "School1", FullName = "student1", FirstName = "first1", LastSurname = "Last1", MiddleName = "Middle1", GradeLevel = "4th Grade", ProfileThumbnail = null},
    //                           new EnhancedStudentInformation{ StudentUSI = 2, SchoolId = 2002, SchoolName = "School2", FullName = "student2", FirstName = "first2", LastSurname = "Last2", MiddleName = "Middle2", GradeLevel = "grade 2"  , ProfileThumbnail = null},
    //                           new EnhancedStudentInformation{ StudentUSI = 3, SchoolId = 2003, SchoolName = "School3", FullName = "student3", FirstName = "first3", LastSurname = "Last3", MiddleName = "Middle3", GradeLevel = "grade 3"  , ProfileThumbnail = null},
    //                           new EnhancedStudentInformation{ StudentUSI = 4, SchoolId = 2004, SchoolName = "School4", FullName = "student4", FirstName = "first4", LastSurname = "Last4", MiddleName = "Middle4", GradeLevel = "grade 4"  , ProfileThumbnail = null},
    //                           new EnhancedStudentInformation{ StudentUSI = 5, SchoolId = 2001, SchoolName = "School5", FullName = "student5", FirstName = "first5", LastSurname = "Last5", MiddleName = "Middle5", GradeLevel = "grade 5"  , ProfileThumbnail = null},
    //                       };
    //        return list.AsQueryable();
    //    }

    //    protected IQueryable<StudentMetric> GetMetricList()
    //    {
    //        var list = new List<StudentMetric>
    //                       {
    //                           new StudentMetric{ StudentUSI = 1, SchoolId = 2001, MetricId = 123, MetricInstanceSetKey = new Guid("5754A0D7-57E0-9E4D-04CA-1F3BB669B92E"), MetricStateTypeId = null, MetricStateTypeIdSortOrder = -1, MetricVariantId = 1337, TrendDirection = null, TrendInterpretation = 1, Value = null , ValueSortOrder = -1, ValueTypeName	= null},
    //                           new StudentMetric{ StudentUSI = 1, SchoolId = 2001, MetricId = 456, MetricInstanceSetKey = new Guid("5754A0D7-57E0-9E4D-04CA-1F3BB669B92E"), MetricStateTypeId = null, MetricStateTypeIdSortOrder = -1, MetricVariantId = 1338, TrendDirection = null, TrendInterpretation = 1, Value = null , ValueSortOrder	= -1, ValueTypeName	= null},
    //                           new StudentMetric{ StudentUSI = 2, SchoolId = 2002, MetricId = 123, MetricInstanceSetKey = new Guid("E47C443B-85F0-E0A1-C5B4-451F4826EF47"), MetricStateTypeId = null, MetricStateTypeIdSortOrder = -1, MetricVariantId = 1337, TrendDirection = null, TrendInterpretation = 1, Value = null , ValueSortOrder	= -1, ValueTypeName	= null},
    //                           new StudentMetric{ StudentUSI = 2, SchoolId = 2002, MetricId = 456, MetricInstanceSetKey = new Guid("E47C443B-85F0-E0A1-C5B4-451F4826EF47"), MetricStateTypeId = null, MetricStateTypeIdSortOrder = -1, MetricVariantId = 1338, TrendDirection = null, TrendInterpretation = 1, Value = null , ValueSortOrder	= -1, ValueTypeName	= null},
    //                           new StudentMetric{ StudentUSI = 3, SchoolId = 2003, MetricId = 123, MetricInstanceSetKey = new Guid("7AC23210-AE91-1F97-6157-021D0D97EC22"), MetricStateTypeId = null, MetricStateTypeIdSortOrder = -1, MetricVariantId = 1337, TrendDirection = null, TrendInterpretation = 1, Value = null , ValueSortOrder	= -1, ValueTypeName	= null},
    //                           new StudentMetric{ StudentUSI = 3, SchoolId = 2003, MetricId = 456, MetricInstanceSetKey = new Guid("7AC23210-AE91-1F97-6157-021D0D97EC22"), MetricStateTypeId = null, MetricStateTypeIdSortOrder = -1, MetricVariantId = 1338, TrendDirection = null, TrendInterpretation = 1, Value = null , ValueSortOrder	= -1, ValueTypeName	= null},
    //                           new StudentMetric{ StudentUSI = 4, SchoolId = 2004, MetricId = 123, MetricInstanceSetKey = new Guid("9B6A9AC5-92A7-6DA1-EA03-0BB387538478"), MetricStateTypeId = null, MetricStateTypeIdSortOrder = -1, MetricVariantId = 1337, TrendDirection = null, TrendInterpretation = 1, Value = null , ValueSortOrder	= -1, ValueTypeName	= null},
    //                           new StudentMetric{ StudentUSI = 4, SchoolId = 2004, MetricId = 456, MetricInstanceSetKey = new Guid("9B6A9AC5-92A7-6DA1-EA03-0BB387538478"), MetricStateTypeId = null, MetricStateTypeIdSortOrder = -1, MetricVariantId = 1338, TrendDirection = null, TrendInterpretation = 1, Value = null , ValueSortOrder	= -1, ValueTypeName	= null},
    //                           new StudentMetric{ StudentUSI = 5, SchoolId = 2001, MetricId = 123, MetricInstanceSetKey = new Guid("6D126E9C-41C8-8C18-93F5-E63CCF538DA5"), MetricStateTypeId = null, MetricStateTypeIdSortOrder = -1, MetricVariantId = 1337, TrendDirection = null, TrendInterpretation = 1, Value = null , ValueSortOrder	= -1, ValueTypeName	= null},
    //                           new StudentMetric{ StudentUSI = 5, SchoolId = 2001, MetricId = 456, MetricInstanceSetKey = new Guid("6D126E9C-41C8-8C18-93F5-E63CCF538DA5"), MetricStateTypeId = null, MetricStateTypeIdSortOrder = -1, MetricVariantId = 1338,TrendDirection = null, TrendInterpretation = 1, Value = null , ValueSortOrder	= -1, ValueTypeName	= null},
    //                       };
    //        return list.AsQueryable();
    //    }

    //    protected override void ExecuteTest()
    //    {
    //        var metaService = new StudentSchoolCategoryListMetaService(metadataListIdResolver, listMetadataProvider, schoolCategoryProvider);
    //        metaModel = metaService.Get(StudentSchoolCategoryListMetaRequest.Create(1000, 1001, 1002, StudentListType.MetricsBasedWatchList.ToString(), requestedSchoolCategory));
    //        var service = new StudentSchoolCategoryListService(metadataListIdResolver, listMetadataProvider, classroomMetricsProvider, studentSchoolLinks, studentListByCategoryProvider, schoolCategoryProvider, gradeLevelUtilitiesProvider);
    //        actualModel = service.Get(StudentSchoolCategoryListRequest.Create(suppliedLocalEducationAgencyId, requestedSchoolCategory, suppliedPageNumber, suppliedPageSize, suppliedSortColumn, suppliedSortDirection, suppliedVisibleColumns, new List<long>(), suppliedUniqueListId));
    //    }

    //    [Test]
    //    public virtual void Should_correctly_set_model()
    //    {
    //        Assert.That(metaModel.SelectedSchoolCategory, Is.EqualTo(requestedSchoolCategory));
    //    }

    //    [Test]
    //    public virtual void Should_correctly_set_metadata()
    //    {
    //        Assert.That(actualModel.ListMetadata, Is.EqualTo(suppliedMetadataColumnGroupList));
    //    }
    //}

//    public class When_listing_students_by_school_category : StudentSchoolCategoryListServiceFixtureBase
//    {
//        protected override void EstablishContext()
//        {
//            requestedSchoolCategory = "School Category 1";
//            expectedStudentUSI = 1;
//            base.EstablishContext();
//        }

//        [Test]
//        public virtual void Should_correctly_select_students_matching_school_category()
//        {
//            var expectedStudentListData = GetStudentList().FirstOrDefault(x => x.StudentUSI == expectedStudentUSI);

//            Assert.That(actualModel.Students.Count, Is.EqualTo(3));
//            Assert.That(actualModel.Students.ElementAt(0).StudentUSI, Is.EqualTo(expectedStudentUSI));
//            Assert.That(actualModel.Students.ElementAt(0).SchoolId, Is.EqualTo(2001));
//            Assert.That(actualModel.Students.ElementAt(0).Name, Is.EqualTo(Utilities.FormatPersonNameByLastName(expectedStudentListData.FirstName, expectedStudentListData.MiddleName, expectedStudentListData.LastSurname)));

////            Assert.That(actualModel.Students.ElementAt(0).Href.Href, Is.EqualTo(studentSchoolLinks.Overview(2001, expectedStudentUSI, expectedStudentListData.FullName, new { listContext = "suppliedUniqueListId" })));
//        }
//    }
}