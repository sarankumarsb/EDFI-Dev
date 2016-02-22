using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using EdFi.Dashboards.Core;
using EdFi.Dashboards.Data.Entities;
using EdFi.Dashboards.Data.Repository;
using EdFi.Dashboards.Resources.Models.Common;
using EdFi.Dashboards.Resources.Models.CustomGrid;
using EdFi.Dashboards.Resources.Models.LocalEducationAgency;
using EdFi.Dashboards.Resources.Models.Student;
using EdFi.Dashboards.Resources.Navigation;
using EdFi.Dashboards.Resources.LocalEducationAgency;
using EdFi.Dashboards.Resources.Staff;
using EdFi.Dashboards.Resources.StudentSchool;
using EdFi.Dashboards.Resources.Tests.Navigation.Fakes;
using EdFi.Dashboards.Testing;
using NUnit.Framework;
using Rhino.Mocks;

namespace EdFi.Dashboards.Resources.Tests.LocalEducationAgency
{
    //public abstract class StudentDemographicListServiceFixtureBase : TestFixtureBase
    //{
    //    protected IRepository<SchoolInformation> schoolInformationRepository;
    //    protected IRepository<EnhancedStudentInformation> studentInformationRepository;
    //    protected IRepository<StudentSchoolInformation> studentSchoolInformationRepository;
    //    protected IRepository<StudentIndicator> studentIndicatorRepository;
    //    protected IMetadataListIdResolver metadataListIdResolver;
    //    protected IListMetadataProvider listMetadataProvider;
    //    protected IClassroomMetricsProvider classroomMetricsProvider;
    //    protected IStudentSchoolAreaLinks studentSchoolLinks;
    //    protected IStudentMetricsProvider studentListPagingProvider;
    //    protected IGradeLevelUtilitiesProvider gradeLevelUtilitiesProvider;

    //    protected int suppliedLocalEducationAgencyId = 1000;
    //    protected int suppliedSchoolId1 = 2001;
    //    protected int suppliedStaffUSI = 1001;
    //    protected int suppliedSectionOrCohortId = 1002;
    //    protected string suppliedUniqueListId = "suppliedUniqueListId";
    //    protected string suppliedStudentListType = "None";
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
    //        schoolInformationRepository = mocks.StrictMock<IRepository<SchoolInformation>>();
    //        studentInformationRepository = mocks.StrictMock<IRepository<EnhancedStudentInformation>>();
    //        studentSchoolInformationRepository = mocks.StrictMock<IRepository<StudentSchoolInformation>>();
    //        studentIndicatorRepository = mocks.StrictMock<IRepository<StudentIndicator>>();
    //        metadataListIdResolver = mocks.StrictMock<IMetadataListIdResolver>();
    //        listMetadataProvider = mocks.StrictMock<IListMetadataProvider>();
    //        classroomMetricsProvider = mocks.StrictMock<IClassroomMetricsProvider>();
    //        studentListPagingProvider = mocks.StrictMock<IStudentMetricsProvider>();
    //        gradeLevelUtilitiesProvider = mocks.StrictMock<IGradeLevelUtilitiesProvider>();

    //        Expect.Call(schoolInformationRepository.GetAll()).Repeat.Any().Return(GetSchoolInformation());
    //        Expect.Call(studentInformationRepository.GetAll()).Repeat.Any().Return(GetStudentInformation());
    //        Expect.Call(studentSchoolInformationRepository.GetAll()).Repeat.Any().Return(GetStudentSchoolInformation());
    //        if (expectStudentInformationQuery)
    //            Expect.Call(studentIndicatorRepository.GetAll()).Repeat.Any().Return(GetStudentIndicator());
    //        Expect.Call(metadataListIdResolver.GetListId(ListType.StudentDemographic, SchoolCategory.HighSchool)).Repeat.Twice().Return(suppliedMetadataListId);
    //        Expect.Call(listMetadataProvider.GetListMetadata(suppliedMetadataListId)).Repeat.Twice().Return(suppliedMetadataColumnGroupList);

    //        Expect.Call(studentListPagingProvider.GetOrderedStudentList(null))
    //              .Repeat.Any().IgnoreArguments()
    //              .Return(GetStudentInformation());

    //        Expect.Call(studentListPagingProvider.GetStudentsWithMetrics(null))
    //              .Repeat.Any().IgnoreArguments()
    //              .Return(new List<StudentMetric>().AsQueryable());

    //        Expect.Call(gradeLevelUtilitiesProvider.FormatGradeLevelForSorting("1")).IgnoreArguments().Repeat.Any().Return(1);
    //        Expect.Call(gradeLevelUtilitiesProvider.FormatGradeLevelForDisplay("1")).IgnoreArguments().Repeat.Any().Return("1");

    //        //Expect.Call(gradeLevelUtilitiesProvider.FormatGradeLevelForDisplay(null)).Return("");

    //        Expect.Call(classroomMetricsProvider.GetAdditionalMetrics(null, null)).IgnoreArguments().Repeat.Any().Return(suppliedAdditionalMetrics);

    //        base.EstablishContext();
    //    }

    //    protected IQueryable<SchoolInformation> GetSchoolInformation()
    //    {
    //        var list = new List<SchoolInformation>
    //                       {
    //                           new SchoolInformation{ LocalEducationAgencyId = suppliedLocalEducationAgencyId + 1, SchoolId = 999 },
    //                           new SchoolInformation{ LocalEducationAgencyId = suppliedLocalEducationAgencyId, SchoolId = suppliedSchoolId1 }
    //                       };
    //        return list.AsQueryable();
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
    //                           new EnhancedStudentInformation{ StudentUSI = 1 + 9990, FullName="Wrong School Gender - Female", Gender = "Female"},
    //                           new EnhancedStudentInformation{ StudentUSI = 2 + 9990, FullName="Wrong School Gender - Male", Gender = "Male"},
    //                           new EnhancedStudentInformation{ StudentUSI = 3 + 9990, FullName="Wrong School Ethnicity - Hispanic/Latino", HispanicLatinoEthnicity = "YES"},
    //                           new EnhancedStudentInformation{ StudentUSI = 4 + 9990, FullName="Wrong School Race - Apple", Race = "Apple"},
    //                           new EnhancedStudentInformation{ StudentUSI = 5 + 9990, FullName="Wrong School Race - Two or More", Race = "Apple,Orange"},
    //                           new EnhancedStudentInformation{ StudentUSI = 6 + 9990, FullName="Wrong School Late Enrollment"},
    //                           new EnhancedStudentInformation{ StudentUSI = 7 + 9990, FullName="Wrong School Indicator - Zebra"},
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
    //        var metaService = new StudentDemographicListMetaService(metadataListIdResolver, listMetadataProvider);
    //        metaModel = metaService.Get(StudentDemographicListMetaRequest.Create(suppliedLocalEducationAgencyId, suppliedStaffUSI, suppliedSectionOrCohortId, suppliedStudentListType, suppliedDemographic));
    //        var service = new StudentDemographicListService(metadataListIdResolver, listMetadataProvider, classroomMetricsProvider, studentSchoolLinks, studentListPagingProvider, gradeLevelUtilitiesProvider);
    //        actualModel = service.Get(StudentDemographicListRequest.Create(suppliedLocalEducationAgencyId, suppliedDemographic, suppliedPageNumber, suppliedPageSize, suppliedSortColumn, suppliedSortDirection, suppliedVisibleColumns, new List<long>(), suppliedUniqueListId));
    //    }
    
    //    [Test]
    //    public virtual void Should_correctly_set_model()
    //    {
    //        //Assert.That(actualModel.UniqueListId, Is.EqualTo(suppliedUniqueListId));
    //        //Assert.That(actualModel.SchoolCategory, Is.EqualTo(SchoolCategory.HighSchool));
    //        Assert.That(metaModel.SelectedDemographic, Is.EqualTo(suppliedDemographic));
    //        //Assert.That(metaModel.LocalEducationAgencyId, Is.EqualTo(suppliedLocalEducationAgencyId));
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
    //        //Expect.Call(gradeLevelUtilitiesProvider.FormatGradeLevelForDisplay("1")).Return("");
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
    //        //Expect.Call(gradeLevelUtilitiesProvider.FormatGradeLevelForDisplay("1")).Return("");
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
    //        //Expect.Call(gradeLevelUtilitiesProvider.FormatGradeLevelForDisplay("1")).Repeat.Times(3).Return("");
    //        //Expect.Call(gradeLevelUtilitiesProvider.FormatGradeLevelForSorting("1")).IgnoreArguments().Repeat.Twice().Return(1);
    //    }
    //}

    //public class When_listing_students_by_race_demographic : StudentDemographicListServiceFixtureBase
    //{
    //    protected override void EstablishContext()
    //    {
    //        suppliedDemographic = "Apple";
    //        expectedStudentUSI = 4;
    //        expectStudentInformationQuery = true;
    //        base.EstablishContext();
    //        //Expect.Call(gradeLevelUtilitiesProvider.FormatGradeLevelForDisplay("1")).Return("");
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
    //        //Expect.Call(gradeLevelUtilitiesProvider.FormatGradeLevelForDisplay("1")).Return("");
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
    //        //Expect.Call(gradeLevelUtilitiesProvider.FormatGradeLevelForDisplay("1")).Return("");

    //        //Expect.Call(schoolInformationRepository.GetAll()).Return(GetSchoolInformation());
    //        //Expect.Call(studentInformationRepository.GetAll()).Return(GetStudentInformation());
    //        //Expect.Call(studentSchoolInformationRepository.GetAll()).Return(GetStudentSchoolInformation());
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
    //        Expect.Call(gradeLevelUtilitiesProvider.FormatGradeLevelForDisplay("1")).Return("");
    //    }
    //}
}
