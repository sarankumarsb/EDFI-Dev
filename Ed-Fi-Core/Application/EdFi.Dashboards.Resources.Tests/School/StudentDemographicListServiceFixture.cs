using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using EdFi.Dashboards.Core;
using EdFi.Dashboards.Data.Entities;
using EdFi.Dashboards.Data.Repository;
using EdFi.Dashboards.Metric.Resources.Providers;
using EdFi.Dashboards.Resources.Models.Common;
using EdFi.Dashboards.Resources.Models.CustomGrid;
using EdFi.Dashboards.Resources.Models.School;
using EdFi.Dashboards.Resources.Models.Student;
using EdFi.Dashboards.Resources.Navigation;
using EdFi.Dashboards.Resources.School;
using EdFi.Dashboards.Resources.Staff;
using EdFi.Dashboards.Resources.StudentSchool;
using EdFi.Dashboards.Resources.Tests.Navigation.Fakes;
using EdFi.Dashboards.Testing;
using NUnit.Framework;
using Rhino.Mocks;

namespace EdFi.Dashboards.Resources.Tests.School
{
    //public abstract class StudentDemographicListServiceFixtureBase : TestFixtureBase
    //{
    //    protected IRepository<EnhancedStudentInformation> studentInformationRepository;
    //    protected IRepository<StudentSchoolInformation> studentSchoolInformationRepository;
    //    protected IRepository<StudentIndicator> studentIndicatorRepository;
    //    protected IMetadataListIdResolver metadataListIdResolver;
    //    protected IListMetadataProvider listMetadataProvider;
    //    protected IClassroomMetricsProvider classroomMetricsProvider;
    //    protected IStudentSchoolAreaLinks studentSchoolLinks;
    //    protected ISchoolCategoryProvider schoolCategoryProvider;
    //    protected IStudentMetricsProvider studentListPagingProvider;
    //    protected IGradeLevelUtilitiesProvider gradeLevelUtilitiesProvider;

    //    protected int suppliedLocalEducationAgencyId = 1000;
    //    protected int suppliedSchoolId1 = 2001;
    //    protected long suppliedStaffUSI = 2002;
    //    protected long suppliedSectionOrCohortId = 2003;
    //    protected string suppliedUniqueListId = "suppliedUniqueListId";
    //    protected int suppliedMetadataListId = 6;
    //    protected List<MetadataColumnGroup> suppliedMetadataColumnGroupList = new List<MetadataColumnGroup>{new MetadataColumnGroup{ Title = "Supplied Metadata"}};
    //    protected List<StudentWithMetrics.Metric> suppliedAdditionalMetrics = new List<StudentWithMetrics.Metric>{ new StudentWithMetrics.Metric(123){DisplayValue = "Supplied Additional Metric"}};
    //    protected string suppliedDemographic;
    //    protected const int suppliedPageNumber = 1;
    //    protected const int suppliedPageSize = 100;
    //    protected const int suppliedSortColumn = 1;
    //    protected const string suppliedSortDirection = "asc";
    //    protected const string suppliedVisibleColumns = "-3,-5,-4,7,78,16,17,26,25";
    //    protected const string suppliedBaseSelect = "sl.SchoolId, sl.StudentUSI, sl.GradeLevel, sl.LastSurname, sl.FirstName, sl.MiddleName, sl.FullName";
    //    protected bool expectStudentInformationQuery;
    //    protected int expectedStudentUSI;
        
    //    protected StudentDemographicListModel actualModel;
    //    protected StudentDemographicListMetaModel metaModel;

    //    protected override void EstablishContext()
    //    {
    //        studentSchoolLinks = new StudentSchoolAreaLinksFake();
    //        studentInformationRepository = mocks.StrictMock<IRepository<EnhancedStudentInformation>>();
    //        studentSchoolInformationRepository = mocks.StrictMock<IRepository<StudentSchoolInformation>>();
    //        studentIndicatorRepository = mocks.StrictMock<IRepository<StudentIndicator>>();
    //        metadataListIdResolver = mocks.StrictMock<IMetadataListIdResolver>();
    //        listMetadataProvider = mocks.StrictMock<IListMetadataProvider>();
    //        classroomMetricsProvider = mocks.StrictMock<IClassroomMetricsProvider>();
    //        schoolCategoryProvider = mocks.StrictMock<ISchoolCategoryProvider>();
    //        studentListPagingProvider = mocks.StrictMock<IStudentMetricsProvider>();
    //        gradeLevelUtilitiesProvider = mocks.StrictMock<IGradeLevelUtilitiesProvider>();

    //        Expect.Call(schoolCategoryProvider.GetSchoolCategoryType(suppliedSchoolId1)).Repeat.Any().Return(SchoolCategory.Ungraded);
    //        Expect.Call(studentInformationRepository.GetAll()).Repeat.Any().Return(GetStudentInformation());
    //        Expect.Call(studentSchoolInformationRepository.GetAll()).Repeat.Any().Return(GetStudentSchoolInformation());
    //        if (expectStudentInformationQuery)
    //            Expect.Call(studentIndicatorRepository.GetAll()).Repeat.Any().Return(GetStudentIndicator());
    //        Expect.Call(metadataListIdResolver.GetListId(ListType.StudentDemographic, SchoolCategory.HighSchool)).Repeat.Twice().Return(suppliedMetadataListId);
    //        Expect.Call(listMetadataProvider.GetListMetadata(suppliedMetadataListId)).Repeat.Twice().Return(suppliedMetadataColumnGroupList);

    //        Expect.Call(classroomMetricsProvider.GetAdditionalMetrics(null, null)).IgnoreArguments().Repeat.Any().Return(suppliedAdditionalMetrics);

    //        Expect.Call(studentListPagingProvider.GetOrderedStudentList(null))
    //              .Repeat.Any().IgnoreArguments()
    //              .Return(GetStudentInformation());

    //        Expect.Call(studentListPagingProvider.GetStudentsWithMetrics(null))
    //              .Repeat.Any().IgnoreArguments()
    //              .Return(new List<StudentMetric>().AsQueryable());

    //        Expect.Call(gradeLevelUtilitiesProvider.FormatGradeLevelForDisplay("1")).IgnoreArguments().Repeat.AtLeastOnce().Return("");
    //        Expect.Call(gradeLevelUtilitiesProvider.FormatGradeLevelForSorting("1")).IgnoreArguments().Repeat.AtLeastOnce().Return(1);

    //        base.EstablishContext();
    //    }

    //    protected IEnumerable<dynamic> GetStudentListDemographicPageData()
    //    {
    //        const string femaleStr = "Female";
    //        const string maleStr = "Male";
    //        const string hispanicStr = "Hispanic/Latino";
    //        const string twoOrMoreStr = "Two or More";
    //        const string lateEnrollmentStr = "Late Enrollment";
    //        const string yesStr = "YES";

    //        //get all the students for this school
    //        var students = (from s in GetStudentInformation()
    //                        join ssi in GetStudentSchoolInformation() on s.StudentUSI equals ssi.StudentUSI
    //                        where ssi.SchoolId == suppliedSchoolId1
    //                        select s);


    //        //decide which student list to get based on demographic
    //        switch (suppliedDemographic)
    //        {
    //            case hispanicStr:
    //                students = (from s in students
    //                            where s.HispanicLatinoEthnicity == yesStr
    //                            select s);
    //                break;
    //            case femaleStr:
    //            case maleStr:
    //                students = (from s in students
    //                            where s.Gender == suppliedDemographic
    //                            select s);
    //                break;
    //            case lateEnrollmentStr:
    //                students = (from s in GetStudentInformation()
    //                            join ssi in GetStudentSchoolInformation() on s.StudentUSI equals ssi.StudentUSI
    //                            where ssi.SchoolId == suppliedSchoolId1 && ssi.LateEnrollment == yesStr
    //                            select s);
    //                break;
    //            case twoOrMoreStr:
    //                students = (from s in students
    //                            where s.Race != null && s.Race.Contains(",") && s.HispanicLatinoEthnicity != yesStr
    //                            select s);
    //                break;
    //            default:
    //                students = students.Where(x => (x.Race != null && x.Race == suppliedDemographic && x.HispanicLatinoEthnicity != yesStr) || studentIndicatorRepository.GetAll().Any(si => x.StudentUSI == si.StudentUSI && si.Status && si.Name == suppliedDemographic));
    //                break;
    //        }

    //        var studentQuery = students.Join(GetStudentInformation(), s => s.StudentUSI, sl => sl.StudentUSI, (s, sl) => new { s, sl }).Select(sl => sl);

    //        foreach (var student in studentQuery)
    //        {
    //            dynamic s = new ExpandoObject();
    //            s.SchoolId = student.sl.SchoolId;
    //            s.StudentUSI = student.sl.StudentUSI;
    //            s.Value = suppliedDemographic;
    //            s.ValueType = "System.String";
    //            s.FullName = student.sl.FullName;
    //            s.FirstName = student.sl.FirstName;
    //            s.MiddleName = student.sl.MiddleName;
    //            s.LastSurname = student.sl.LastSurname;
    //            s.GradeLevel = student.sl.GradeLevel;
    //            s.Race = student.s.Race;
    //            s.HispanicLatinoEthnicity = student.s.HispanicLatinoEthnicity;

    //            yield return s;
    //        }
    //    }

    //    protected IQueryable<EnhancedStudentInformation> GetStudentInformation()
    //    {
    //        var list = new List<EnhancedStudentInformation>
    //                       {
    //                           new EnhancedStudentInformation{ StudentUSI = 1, FullName="Gender - Female", Gender = "Female", HispanicLatinoEthnicity = "NO"},
    //                           new EnhancedStudentInformation{ StudentUSI = 2, FullName="Gender - Male", Gender = "Male"},
    //                           new EnhancedStudentInformation{ StudentUSI = 3, FullName="Ethnicity - Hispanic/Latino", HispanicLatinoEthnicity = "YES"},
    //                           new EnhancedStudentInformation{ StudentUSI = 4, FullName="Race - Apple", Race = "Apple"},
    //                           new EnhancedStudentInformation{ StudentUSI = 5, FullName="Race - Two or More", Race = "Apple,Orange"},
    //                           new EnhancedStudentInformation{ StudentUSI = 6, FullName="Late Enrollment"},
    //                           new EnhancedStudentInformation{ StudentUSI = 7, FullName="Indicator - Zebra"},
    //                           new EnhancedStudentInformation{ StudentUSI = 8, FullName="Hispanic Ethnicity and Apple Race - Apple", Race = "Apple", HispanicLatinoEthnicity = "YES"},
    //                           new EnhancedStudentInformation{ StudentUSI = 9, FullName="Hispanic Ethnicity and Race - Two or More", Race = "Apple,Orange", HispanicLatinoEthnicity = "YES"},
    //                           new EnhancedStudentInformation{ StudentUSI = 1 + 9990, FullName="Wrong LEA Gender - Female", Gender = "Female"},
    //                           new EnhancedStudentInformation{ StudentUSI = 2 + 9990, FullName="Wrong LEA Gender - Male", Gender = "Male"},
    //                           new EnhancedStudentInformation{ StudentUSI = 3 + 9990, FullName="Wrong LEA Ethnicity - Hispanic/Latino", HispanicLatinoEthnicity = "YES"},
    //                           new EnhancedStudentInformation{ StudentUSI = 4 + 9990, FullName="Wrong LEA Race - Apple", Race = "Apple"},
    //                           new EnhancedStudentInformation{ StudentUSI = 5 + 9990, FullName="Wrong LEA Race - Two or More", Race = "Apple,Orange"},
    //                           new EnhancedStudentInformation{ StudentUSI = 6 + 9990, FullName="Wrong LEA Late Enrollment"},
    //                           new EnhancedStudentInformation{ StudentUSI = 7 + 9990, FullName="Wrong LEA Indicator - Zebra"}
    //                       };
    //        return list.AsQueryable();
    //    }

    //    protected IQueryable<StudentSchoolInformation> GetStudentSchoolInformation()
    //    {
    //        var list = new List<StudentSchoolInformation>
    //                       {
    //                           new StudentSchoolInformation{ StudentUSI = 1, SchoolId = suppliedSchoolId1, LateEnrollment = "NO"},
    //                           new StudentSchoolInformation{ StudentUSI = 2, SchoolId = suppliedSchoolId1},
    //                           new StudentSchoolInformation{ StudentUSI = 3, SchoolId = suppliedSchoolId1},
    //                           new StudentSchoolInformation{ StudentUSI = 4, SchoolId = suppliedSchoolId1},
    //                           new StudentSchoolInformation{ StudentUSI = 5, SchoolId = suppliedSchoolId1},
    //                           new StudentSchoolInformation{ StudentUSI = 6, SchoolId = suppliedSchoolId1, LateEnrollment = "YES"},
    //                           new StudentSchoolInformation{ StudentUSI = 7, SchoolId = suppliedSchoolId1},
    //                           new StudentSchoolInformation{ StudentUSI = 8, SchoolId = suppliedSchoolId1},
    //                           new StudentSchoolInformation{ StudentUSI = 9, SchoolId = suppliedSchoolId1},
    //                           new StudentSchoolInformation{ StudentUSI = 1 + 9990, SchoolId = 999},
    //                           new StudentSchoolInformation{ StudentUSI = 2 + 9990, SchoolId = 999},
    //                           new StudentSchoolInformation{ StudentUSI = 3 + 9990, SchoolId = 999},
    //                           new StudentSchoolInformation{ StudentUSI = 4 + 9990, SchoolId = 999},
    //                           new StudentSchoolInformation{ StudentUSI = 5 + 9990, SchoolId = 999},
    //                           new StudentSchoolInformation{ StudentUSI = 6 + 9990, SchoolId = 999, LateEnrollment = "YES"},
    //                           new StudentSchoolInformation{ StudentUSI = 7 + 9990, SchoolId = 999},
    //                       };
    //        return list.AsQueryable();
    //    }

    //    protected IQueryable<StudentIndicator> GetStudentIndicator()
    //    {
    //        var list = new List<StudentIndicator>
    //                       {
    //                           new StudentIndicator{ StudentUSI = 1, EducationOrganizationId = suppliedLocalEducationAgencyId, Name = "Zebra", Status = false},
    //                           new StudentIndicator{ StudentUSI = 7, EducationOrganizationId = suppliedLocalEducationAgencyId, Name = "Zebra", Status = true},
    //                           new StudentIndicator{ StudentUSI = 7 + 9990, EducationOrganizationId = suppliedLocalEducationAgencyId, Name = "Zebra", Status = true},
    //                       };
    //        return list.AsQueryable();
    //    }

    //    protected override void ExecuteTest()
    //    {
    //        var metaService = new StudentDemographicListMetaService(schoolCategoryProvider, metadataListIdResolver, listMetadataProvider);
    //        metaModel = metaService.Get(StudentDemographicListMetaRequest.Create(suppliedSchoolId1, suppliedStaffUSI, suppliedSectionOrCohortId, StudentListType.MetricsBasedWatchList.ToString(), suppliedDemographic)); 
    //        var service = new StudentDemographicListService(studentSchoolLinks, schoolCategoryProvider, metadataListIdResolver, listMetadataProvider, classroomMetricsProvider, studentListPagingProvider, gradeLevelUtilitiesProvider);
    //        actualModel = service.Get(StudentDemographicListRequest.Create(suppliedSchoolId1, suppliedDemographic, suppliedPageNumber, suppliedPageSize, suppliedSortColumn, suppliedSortDirection, suppliedVisibleColumns, new List<long>(), suppliedUniqueListId));
    //    }
    
    //    [Test]
    //    public virtual void Should_correctly_set_model()
    //    {
    //        Assert.That(metaModel.SelectedDemographic, Is.EqualTo(suppliedDemographic));
    //    }

    //    [Test]
    //    public virtual void Should_correctly_set_metadata()
    //    {
    //        Assert.That(actualModel.ListMetadata, Is.EqualTo(suppliedMetadataColumnGroupList));
    //    }
    //}

    //public class When_listing_students_by_female_demographic : StudentDemographicListServiceFixtureBase
    //{
    //    protected override void EstablishContext()
    //    {
    //        suppliedDemographic = "Female";
    //        expectedStudentUSI = 1;
    //        expectStudentInformationQuery = false;
    //        base.EstablishContext();
    //    }
    //}

    //public class When_listing_students_by_male_demographic : StudentDemographicListServiceFixtureBase
    //{
    //    protected override void EstablishContext()
    //    {
    //        suppliedDemographic = "Male";
    //        expectedStudentUSI = 2;
    //        expectStudentInformationQuery = false;
    //        base.EstablishContext();
    //    }
    //}

    //public class When_listing_students_by_hispanic_demographic : StudentDemographicListServiceFixtureBase
    //{
    //    protected override void EstablishContext()
    //    {
    //        suppliedDemographic = "Hispanic/Latino";
    //        expectedStudentUSI = 3;
    //        expectStudentInformationQuery = false;
    //        base.EstablishContext();
    //    }

//        [Test]
//        public override void Should_correctly_select_students_matching_demographic()
//        {
//            var expectedStudentListData = GetStudentList().Single(x => x.StudentUSI == expectedStudentUSI);
//
//            Assert.That(actualModel.Students.Count, Is.EqualTo(3));
//            Assert.That(actualModel.Students.ElementAt(0).StudentUSI, Is.EqualTo(expectedStudentUSI));
//            Assert.That(actualModel.Students.ElementAt(0).SchoolId, Is.EqualTo(suppliedSchoolId1));
//            Assert.That(actualModel.Students.ElementAt(0).Name, Is.EqualTo(Utilities.FormatPersonNameByLastName(expectedStudentListData.FirstName, expectedStudentListData.MiddleName, expectedStudentListData.LastSurname)));
//            Assert.That(actualModel.Students.ElementAt(0).GradeLevel, Is.EqualTo(Utilities.FormatGradeLevelForSorting(expectedStudentListData.GradeLevel)));
//            Assert.That(actualModel.Students.ElementAt(0).GradeLevelDisplayValue, Is.EqualTo(Utilities.FormatGradeLevelForDisplay(expectedStudentListData.GradeLevel)));
//            Assert.That(actualModel.Students.ElementAt(0).Href.Href, Is.EqualTo(studentSchoolLinks.Overview(suppliedSchoolId1, expectedStudentUSI, expectedStudentListData.FullName, new { listContext = suppliedUniqueListId })));
//            Assert.That(actualModel.Students.ElementAt(0).Metrics, Is.EqualTo(suppliedAdditionalMetrics));
//        }
    //}

    //public class When_listing_students_by_race_demographic : StudentDemographicListServiceFixtureBase
    //{
    //    protected override void EstablishContext()
    //    {
    //        suppliedDemographic = "Apple";
    //        expectedStudentUSI = 4;
    //        expectStudentInformationQuery = true;
    //        base.EstablishContext();
    //    }
    //}

    //public class When_listing_students_by_two_or_more_races_demographic : StudentDemographicListServiceFixtureBase
    //{
    //    protected override void EstablishContext()
    //    {
    //        suppliedDemographic = "Two or More";
    //        expectedStudentUSI = 5;
    //        expectStudentInformationQuery = false;
    //        base.EstablishContext();
    //    }

    //    [Test]
    //    public override void Should_correctly_set_model()
    //    {
    //        Assert.That(metaModel.SelectedDemographic, Is.EqualTo(suppliedDemographic));
    //    }
    //}

    //public class When_listing_students_by_late_enrollment_demographic : StudentDemographicListServiceFixtureBase
    //{
    //    protected override void EstablishContext()
    //    {
    //        suppliedDemographic = "Late Enrollment";
    //        expectedStudentUSI = 6;
    //        expectStudentInformationQuery = false;
    //        base.EstablishContext();
    //    }
    //}

    //public class When_listing_students_by_indicator_demographic : StudentDemographicListServiceFixtureBase
    //{
    //    protected override void EstablishContext()
    //    {
    //        suppliedDemographic = "Zebra";
    //        expectedStudentUSI = 7;
    //        expectStudentInformationQuery = true;
    //        base.EstablishContext();
    //    }
    //}
}
