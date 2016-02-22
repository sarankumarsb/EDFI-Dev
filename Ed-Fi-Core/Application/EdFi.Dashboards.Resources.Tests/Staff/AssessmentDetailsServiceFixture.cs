using System;
using System.Collections.Generic;
using System.Linq;
using EdFi.Dashboards.Application.Data.Entities;
using EdFi.Dashboards.Core;
using EdFi.Dashboards.Data.Entities;
using EdFi.Dashboards.Data.Repository;
using EdFi.Dashboards.Metric.Resources.Models;
using EdFi.Dashboards.Metric.Resources.Providers;
using EdFi.Dashboards.Metric.Resources.Services;
using EdFi.Dashboards.Resources.Common;
using EdFi.Dashboards.Resources.Metric;
using EdFi.Dashboards.Resources.Models.Common;
using EdFi.Dashboards.Resources.Models.CustomGrid;
using EdFi.Dashboards.Resources.Models.Staff;
using EdFi.Dashboards.Resources.Models.Student;
using EdFi.Dashboards.Resources.Navigation;
using EdFi.Dashboards.Resources.School;
using EdFi.Dashboards.Resources.Staff;
using EdFi.Dashboards.Resources.StudentMetrics;
using EdFi.Dashboards.Resources.StudentSchool;
using EdFi.Dashboards.Resources.Tests.Navigation.Fakes;
using EdFi.Dashboards.Testing;
using NUnit.Framework;
using Rhino.Mocks;

namespace EdFi.Dashboards.Resources.Tests.Staff
{
    public abstract class AssessmentDetailsServiceFixtureBase : TestFixtureBase
    {
        protected const int suppliedTeacherSectionId1 = 100;
        protected const int suppliedTeacherSectionId2 = 110;
        protected const int suppliedTeacherSectionId3 = 120;
        protected const int suppliedTeacherSectionId4 = 121;
        protected const int suppliedTeacherSectionId5 = 122;
        protected const int suppliedTeacherSectionId6 = 123;
        protected const string suppliedLocalCourseCode1 = "local course code 1";
        protected const string suppliedLocalCourseCode2 = "local course code 2";
        protected const string suppliedLocalCourseCode3 = "local course code 3";
        protected const string suppliedLocalCourseCode4 = "local course code 4";
        protected const string suppliedLocalCourseCode6 = "local course code 6";
        protected const string suppliedSubjectAreaELA = "subject area 1";
        protected const string suppliedSubjectAreaMath = "Mathematics";
        protected const string suppliedSubjectAreaScience = "Science";
        protected const string suppliedSubjectAreaSocialStudies = "SocialStudies";
        protected const int suppliedStaffCohortId1 = 130;
        protected const int suppliedStaffCohortId2 = 140;
        protected const int suppliedStaffCohortId3 = 150;
        protected const string suppliedStaffCohortSubjectArea1 = "Social Studies";
        protected const string suppliedStaffCohortSubjectArea2 = "subject area 4";
        protected const int suppliedStudentId1 = 160;
        protected const int suppliedStudentId4 = 190;
        protected const int suppliedStudentId5 = 200;
        protected const string objectiveName1 = "objective 1";
        protected const string objectiveName2 = "objective 2";
        protected const string objectiveName3 = "objective 3";
        protected const int suppliedStaffUSI = 1000;
        protected const int suppliedSchoolId = 2000;
        protected const int suppliedLocalEducationAgencyId = 3000;
        protected string suppliedStudentListType;
        protected int suppliedSectionId;
        protected const string suppliedUniqueListId = "baboon";
        protected const string suppliedImageName = "test.jpg";
        protected const string reading = "ELA / Reading";
        protected const string writing = "Writing";
        protected const string math = "Mathematics";
        protected const string science = "Science";
        protected const string socialStudies = "Social Studies";
        protected const int suppliedMetricNodeId = 7;

        protected StaffModel.SubjectArea suppliedSubjectArea;
        protected StaffModel.AssessmentSubType suppliedAssessmentSubType;
        protected int requestedMetricId = 0;

        protected readonly List<long> expectedStudentIds = new List<long>();
        protected string expectedMetricTitle;
        protected int expectedObjectiveTitleCount;

        protected IRepository<EnhancedStudentInformation> enhancedStudentInformationRepository;
        protected IRepository<TeacherStudentSection> teacherStudentSectionRepository;
        protected IRepository<TeacherSection> teacherSectionRepository;
        protected IRepository<StaffStudentCohort> staffStudentCohortRepository;
        protected IRepository<StaffCohort> staffCohortRepository;
        protected IRepository<StudentMetric> studentMetricRepository;
        protected IRepository<StaffCustomStudentListStudent> staffCustomStudentListStudentRepository;
        protected IRepository<StaffCustomStudentList> staffCustomStudentListRepository;
        protected IRepository<StudentMetricObjective> studentMetricObjectiveRepository;

        protected IAccommodationProvider accommodationProvider;
        protected IUniqueListIdProvider uniqueListIdProvider;
        protected ITrendRenderingDispositionProvider trendRenderingDispositionProvider;
        protected IMetricStateProvider metricStateProvider;
        protected IAssessmentDetailsProvider assessmentDetailsProvider;
        protected IAssessmentBenchmarkDetailsProvider assessmentBenchmarkDetailsProvider;
        protected IStudentMetricsProvider studentMetricsProvider;
        protected ISchoolCategoryProvider schoolCategoryProvider;
        protected IListMetadataProvider listMetadataProvider;
        protected IGradeLevelUtilitiesProvider gradeLevelUtilitiesProvider;
        protected IClassroomMetricsProvider classroomMetricsProvider;
        
        protected IRootMetricNodeResolver rootMetricNodeResolver;
        protected IMetadataListIdResolver metadataListIdResolver;
        protected IStudentSchoolAreaLinks studentSchoolAreaLinks = new StudentSchoolAreaLinksFake();

        protected AssessmentDetailsModel actualModel;

        #region TestFixture Override Methods
        protected override void EstablishContext()
        {
            enhancedStudentInformationRepository = mocks.StrictMock<IRepository<EnhancedStudentInformation>>();
            teacherStudentSectionRepository = mocks.StrictMock<IRepository<TeacherStudentSection>>();
            teacherSectionRepository = mocks.StrictMock<IRepository<TeacherSection>>();
            staffStudentCohortRepository = mocks.StrictMock<IRepository<StaffStudentCohort>>();
            staffCohortRepository = mocks.StrictMock<IRepository<StaffCohort>>();
            staffCustomStudentListStudentRepository = mocks.StrictMock<IRepository<StaffCustomStudentListStudent>>();
            staffCustomStudentListRepository = mocks.StrictMock<IRepository<StaffCustomStudentList>>();

            accommodationProvider = mocks.StrictMock<IAccommodationProvider>();
            uniqueListIdProvider = mocks.StrictMock<IUniqueListIdProvider>();
            trendRenderingDispositionProvider = mocks.StrictMock<ITrendRenderingDispositionProvider>();
            rootMetricNodeResolver = mocks.StrictMock<IRootMetricNodeResolver>();
            metricStateProvider = mocks.StrictMock<IMetricStateProvider>();
            studentMetricObjectiveRepository = mocks.StrictMock<IRepository<StudentMetricObjective>>();

            assessmentDetailsProvider = mocks.StrictMock<IAssessmentDetailsProvider>();
            assessmentBenchmarkDetailsProvider = mocks.StrictMock<IAssessmentBenchmarkDetailsProvider>();
            studentMetricsProvider = mocks.StrictMock<IStudentMetricsProvider>();
            schoolCategoryProvider = mocks.StrictMock<ISchoolCategoryProvider>();
            metadataListIdResolver = mocks.StrictMock<IMetadataListIdResolver>();
            listMetadataProvider = mocks.StrictMock<IListMetadataProvider>();
            classroomMetricsProvider = mocks.StrictMock<IClassroomMetricsProvider>();
            gradeLevelUtilitiesProvider = mocks.StrictMock<IGradeLevelUtilitiesProvider>();

            Expect.Call(studentMetricObjectiveRepository.GetAll()).Return(GetSuppliedStudentMetricObjective());
            Expect.Call(teacherStudentSectionRepository.GetAll()).Repeat.Any().Return(GetSuppliedTeacherStudentSection());
            Expect.Call(teacherSectionRepository.GetAll()).Repeat.Any().Return(GetSuppliedTeacherSection());
            Expect.Call(staffStudentCohortRepository.GetAll()).Repeat.Any().Return(GetSuppliedStaffStudentCohort());
            Expect.Call(staffCohortRepository.GetAll()).Repeat.Any().Return(GetSuppliedStaffCohort());

            Expect.Call(staffCustomStudentListRepository.GetAll()).Repeat.Any().Return(new List<StaffCustomStudentList>().AsQueryable() );
            Expect.Call(staffCustomStudentListStudentRepository.GetAll()).Repeat.Any().Return(new List<StaffCustomStudentListStudent>().AsQueryable());

            Expect.Call(studentMetricsProvider.GetOrderedStudentList(null, null, null)).IgnoreArguments().Return(SuppliedStudentList.AsQueryable().Where(x => x.SchoolId == suppliedSchoolId && expectedStudentIds.Contains(x.StudentUSI)));
            Expect.Call(studentMetricsProvider.GetStudentsWithMetrics(null)).IgnoreArguments().Return(SuppliedStudentMetrics.AsQueryable());
            Expect.Call(schoolCategoryProvider.GetSchoolCategoryType(suppliedSchoolId)).Return(SchoolCategory.HighSchool);
            Expect.Call(uniqueListIdProvider.GetUniqueId()).Return(suppliedUniqueListId);
            Expect.Call(metadataListIdResolver.GetListId(ListType.ClassroomGeneralOverview, SchoolCategory.HighSchool)).Return(0);
            Expect.Call(rootMetricNodeResolver.GetRootMetricNodeForStudent(suppliedSchoolId)).Repeat.Any().Return(GetStudentRootOverviewNode());
            Expect.Call(listMetadataProvider.GetListMetadata(0)).Return(new List<MetadataColumnGroup>());
            Expect.Call(assessmentDetailsProvider.GetStudentsFixedRowTitle(suppliedAssessmentSubType, suppliedSubjectArea)).Return(string.Empty);
            Expect.Call(gradeLevelUtilitiesProvider.FormatGradeLevelForSorting(null)).IgnoreArguments().Return(23).Repeat.Times(expectedStudentIds.Count);
            Expect.Call(gradeLevelUtilitiesProvider.FormatGradeLevelForDisplay(null)).IgnoreArguments().Return("Formatted").Repeat.Times(expectedStudentIds.Count);
            
            Expect.Call(classroomMetricsProvider.GetAdditionalMetrics(null, null)).IgnoreArguments()
                .Do(new Func<IEnumerable<StudentMetric>, List<MetadataColumnGroup>, List<StudentWithMetrics.Metric>>((arg1, arg2) => new List<StudentWithMetrics.Metric>()))
                .Repeat.Times(expectedStudentIds.Count);

            Expect.Call(assessmentBenchmarkDetailsProvider.GetObjectiveColumnWidth()).Return(string.Empty).Repeat.Times(expectedObjectiveTitleCount);
            Expect.Call(assessmentBenchmarkDetailsProvider.GetMetricIdsForObjectives(suppliedSubjectArea)).Return(new[] { requestedMetricId }).Repeat.Times(1);
            Expect.Call(assessmentBenchmarkDetailsProvider.OnStudentAssessmentInitialized(null, null, StaffModel.SubjectArea.None)).IgnoreArguments()
                .Return(new StudentWithMetrics.IndicatorMetric
                {
                    MetricVariantId = requestedMetricId,
                    MetricIndicator = 1,
                    StudentUSI = 1,
                    UniqueIdentifier = 1,
                    DisplayValue = "some value",
                    State = MetricStateType.Good,
                    Value = "some other value",
                })
                .Repeat.Times(expectedStudentIds.Count);

            base.EstablishContext();
        }

        protected override void ExecuteTest()
        {
            var service = new AssessmentDetailsService
            {
                TeacherStudentSectionRepository = teacherStudentSectionRepository,
                TeacherSectionRepository = teacherSectionRepository,
                StaffStudentCohortRepository = staffStudentCohortRepository,
                StaffCohortRepository = staffCohortRepository,
                StaffCustomStudentListStudentRepository = staffCustomStudentListStudentRepository,
                StaffCustomStudentListRepository = staffCustomStudentListRepository,

                AccommodationProvider = accommodationProvider,
                UniqueListProvider = uniqueListIdProvider,
                TrendRenderingDispositionProvider = trendRenderingDispositionProvider,
                RootMetricNodeResolver = rootMetricNodeResolver,
                StudentSchoolAreaLinks = studentSchoolAreaLinks,
                MetricStateProvider = metricStateProvider,

                StudentMetricObjectiveRepository = studentMetricObjectiveRepository,
                AssessmentDetailsProvider = assessmentDetailsProvider,
                AssessmentBenchmarkDetailsProvider = assessmentBenchmarkDetailsProvider,

                StudentListWithMetricsProvider = studentMetricsProvider,
                SchoolCategoryProvider = schoolCategoryProvider,
                MetadataListIdResolver = metadataListIdResolver,
                ListMetadataProvider = listMetadataProvider,
                GradeLevelUtilitiesProvider = gradeLevelUtilitiesProvider,
                ClassroomMetricsProvider = classroomMetricsProvider,
            };

            actualModel = service.Get(AssessmentDetailsRequest.Create(
                suppliedSchoolId,
                suppliedStaffUSI,
                suppliedStudentListType,
                suppliedSectionId,
                suppliedSubjectArea.ToString(),
                suppliedAssessmentSubType.ToString()));
        }
        #endregion

        #region Helper Setup Methods
        protected List<EnhancedStudentInformation> SuppliedStudentList
        {
            get
            {
                return new List<EnhancedStudentInformation>
                {
                    new EnhancedStudentInformation { StudentUSI = suppliedStudentId1, SchoolId = suppliedSchoolId + 1, FirstName = "wrong data" },
                    new EnhancedStudentInformation { StudentUSI = suppliedStudentId1, SchoolId = suppliedSchoolId, FirstName = "First 1", LastSurname = "Last 1", MiddleName = "Middle 1", ProfileThumbnail = "thumbnail", Gender = "female", SchoolName = "school 1", GradeLevel = "level 1" },
                    new EnhancedStudentInformation { StudentUSI = suppliedStudentId4, SchoolId = suppliedSchoolId, FirstName = "First 4", LastSurname = "Last 4", MiddleName = "Middle 4", ProfileThumbnail = "thumbnail", Gender = "female", SchoolName = "school 1", GradeLevel = "level 4" },
                    new EnhancedStudentInformation { StudentUSI = suppliedStudentId5, SchoolId = suppliedSchoolId, FirstName = "First 5", LastSurname = "Last 5", MiddleName = "Middle 5", ProfileThumbnail = "thumbnail", Gender = "female", SchoolName = "school 1", GradeLevel = "level 5" }
                };
            }
        }

        protected List<StudentMetric> SuppliedStudentMetrics
        {
            get
            {
                return new List<StudentMetric>
                {
                    new StudentMetric { StudentUSI = suppliedStudentId1, SchoolId = suppliedSchoolId + 1 },

                    new StudentMetric { StudentUSI = suppliedStudentId1, SchoolId = suppliedSchoolId, MetricId = (int) StudentMetricEnum.BenchmarkMasteryELAReading,    Value = "123",  MetricStateTypeId = 3,  ValueTypeName = "System.Int32", Format = "reading {0}"        },
                    new StudentMetric { StudentUSI = suppliedStudentId1, SchoolId = suppliedSchoolId, MetricId = (int) StudentMetricEnum.BenchmarkMasteryWriting,       Value = "2340", MetricStateTypeId = 3,  ValueTypeName = "System.Int32", Format = "writing {0}"        },
                    new StudentMetric { StudentUSI = suppliedStudentId1, SchoolId = suppliedSchoolId, MetricId = (int) StudentMetricEnum.BenchmarkMasteryMath,          Value = "234",  MetricStateTypeId = 3,  ValueTypeName = "System.Int32", Format = "math {0}"           },
                    new StudentMetric { StudentUSI = suppliedStudentId1, SchoolId = suppliedSchoolId, MetricId = (int) StudentMetricEnum.BenchmarkMasteryScience,       Value = "345",  MetricStateTypeId = 3,  ValueTypeName = "System.Int32", Format = "science {0}"        },
                    new StudentMetric { StudentUSI = suppliedStudentId1, SchoolId = suppliedSchoolId, MetricId = (int) StudentMetricEnum.BenchmarkMasterySocialStudies, Value = "456",  MetricStateTypeId = 3,  ValueTypeName = "System.Int32", Format = "social studies {0}" },
                    
                    new StudentMetric { StudentUSI = suppliedStudentId4, SchoolId = suppliedSchoolId, MetricId = (int) StudentMetricEnum.BenchmarkMasteryELAReading,    Value = "123",  MetricStateTypeId = 3,  ValueTypeName = "System.Int32", Format = "{0}"                },
                    new StudentMetric { StudentUSI = suppliedStudentId4, SchoolId = suppliedSchoolId, MetricId = (int) StudentMetricEnum.BenchmarkMasteryWriting,       Value = "999",  MetricVariantId = 3,    ValueTypeName = "System.Int32", Format = "writing {0}"        },
                    new StudentMetric { StudentUSI = suppliedStudentId4, SchoolId = suppliedSchoolId, MetricId = (int) StudentMetricEnum.BenchmarkMasteryMath,          Value = "123",  MetricStateTypeId = 3,  ValueTypeName = "System.Int32", Format = "{0}"                },
                    new StudentMetric { StudentUSI = suppliedStudentId4, SchoolId = suppliedSchoolId, MetricId = (int) StudentMetricEnum.BenchmarkMasteryScience,       Value = "123",  MetricStateTypeId = 3,  ValueTypeName = "System.Int32", Format = "{0}"                },
                    new StudentMetric { StudentUSI = suppliedStudentId4, SchoolId = suppliedSchoolId, MetricId = (int) StudentMetricEnum.BenchmarkMasterySocialStudies, Value = "123",  MetricStateTypeId = 3,  ValueTypeName = "System.Int32", Format = "{0}"                },
                    
                    new StudentMetric { StudentUSI = suppliedStudentId5, SchoolId = suppliedSchoolId, MetricId = (int) StudentMetricEnum.BenchmarkMasteryELAReading,    Value = "567",  MetricStateTypeId = 3,  ValueTypeName = "System.Int32", Format = "reading M {0}"      },
                    new StudentMetric { StudentUSI = suppliedStudentId5, SchoolId = suppliedSchoolId, MetricId = (int) StudentMetricEnum.BenchmarkMasteryWriting,       Value = "6780", MetricStateTypeId = 3,  ValueTypeName = "System.Int32", Format = "writing M {0}"      },
                    new StudentMetric { StudentUSI = suppliedStudentId5, SchoolId = suppliedSchoolId, MetricId = (int) StudentMetricEnum.BenchmarkMasteryMath,          Value = "678",  MetricStateTypeId = 3,  ValueTypeName = "System.Int32", Format = "math {0}"           },
                    new StudentMetric { StudentUSI = suppliedStudentId5, SchoolId = suppliedSchoolId, MetricId = (int) StudentMetricEnum.BenchmarkMasteryScience,       Value = "789",  MetricStateTypeId = 3,  ValueTypeName = "System.Int32", Format = "science {0}"        },
                    new StudentMetric { StudentUSI = suppliedStudentId5, SchoolId = suppliedSchoolId, MetricId = (int) StudentMetricEnum.BenchmarkMasterySocialStudies, Value = "1234", MetricStateTypeId = 3,  ValueTypeName = "System.Int32", Format = "social studies {0}" }
                };
            }
        }

        protected virtual IQueryable<StudentMetricObjective> GetSuppliedStudentMetricObjective()
        {
            var data = new List<StudentMetricObjective>
            {
                new StudentMetricObjective { StudentUSI = suppliedStudentId1, SchoolId = suppliedSchoolId + 1, MetricId = (int)StudentMetricEnum.BenchmarkMasteryELAReading, ObjectiveName = "wrong data"},
                new StudentMetricObjective { StudentUSI = suppliedStudentId1, SchoolId = suppliedSchoolId, MetricId = (int)StudentMetricEnum.BenchmarkMasteryELAReading, ObjectiveName = objectiveName1 + reading, Value= "1/2", MetricStateTypeId = 1 },
                new StudentMetricObjective { StudentUSI = suppliedStudentId1, SchoolId = suppliedSchoolId, MetricId = (int)StudentMetricEnum.BenchmarkMasteryELAReading, ObjectiveName = objectiveName2 + reading, Value= "2/3", MetricStateTypeId = 3 },
                new StudentMetricObjective { StudentUSI = suppliedStudentId1, SchoolId = suppliedSchoolId, MetricId = (int)StudentMetricEnum.BenchmarkMasteryELAReading, ObjectiveName = objectiveName3 + reading, Value= "3/4" },

                new StudentMetricObjective { StudentUSI = suppliedStudentId1, SchoolId = suppliedSchoolId + 1, MetricId = (int)StudentMetricEnum.BenchmarkMasteryWriting, ObjectiveName = "wrong data"},
                new StudentMetricObjective { StudentUSI = suppliedStudentId1, SchoolId = suppliedSchoolId, MetricId = (int)StudentMetricEnum.BenchmarkMasteryWriting, ObjectiveName = objectiveName1 + writing, Value= "100/200", MetricStateTypeId = 1 },
                new StudentMetricObjective { StudentUSI = suppliedStudentId1, SchoolId = suppliedSchoolId, MetricId = (int)StudentMetricEnum.BenchmarkMasteryWriting, ObjectiveName = objectiveName2 + writing, Value= "200/300", MetricStateTypeId = 3 },
                new StudentMetricObjective { StudentUSI = suppliedStudentId1, SchoolId = suppliedSchoolId, MetricId = (int)StudentMetricEnum.BenchmarkMasteryWriting, ObjectiveName = objectiveName3 + writing, Value= "300/400" },

                new StudentMetricObjective { StudentUSI = suppliedStudentId1, SchoolId = suppliedSchoolId + 1, MetricId = (int)StudentMetricEnum.BenchmarkMasteryMath, ObjectiveName = "wrong data"},
                new StudentMetricObjective { StudentUSI = suppliedStudentId1, SchoolId = suppliedSchoolId, MetricId = (int)StudentMetricEnum.BenchmarkMasteryMath, ObjectiveName = objectiveName1 + math, Value= "1/20", MetricStateTypeId = 1 },
                new StudentMetricObjective { StudentUSI = suppliedStudentId1, SchoolId = suppliedSchoolId, MetricId = (int)StudentMetricEnum.BenchmarkMasteryMath, ObjectiveName = objectiveName2 + math, Value= "2/30", MetricStateTypeId = 3 },
                new StudentMetricObjective { StudentUSI = suppliedStudentId1, SchoolId = suppliedSchoolId, MetricId = (int)StudentMetricEnum.BenchmarkMasteryMath, ObjectiveName = objectiveName3 + math, Value= "3/40" },

                new StudentMetricObjective { StudentUSI = suppliedStudentId1, SchoolId = suppliedSchoolId + 1, MetricId = (int)StudentMetricEnum.BenchmarkMasteryScience, ObjectiveName = "wrong data"},
                new StudentMetricObjective { StudentUSI = suppliedStudentId1, SchoolId = suppliedSchoolId, MetricId = (int)StudentMetricEnum.BenchmarkMasteryScience, ObjectiveName = objectiveName1 + science, Value= "1/21", MetricStateTypeId = 1 },
                new StudentMetricObjective { StudentUSI = suppliedStudentId1, SchoolId = suppliedSchoolId, MetricId = (int)StudentMetricEnum.BenchmarkMasteryScience, ObjectiveName = objectiveName2 + science, Value= "2/31", MetricStateTypeId = 3 },
                new StudentMetricObjective { StudentUSI = suppliedStudentId1, SchoolId = suppliedSchoolId, MetricId = (int)StudentMetricEnum.BenchmarkMasteryScience, ObjectiveName = objectiveName3 + science, Value= "3/41" },

                new StudentMetricObjective { StudentUSI = suppliedStudentId1, SchoolId = suppliedSchoolId + 1, MetricId = (int)StudentMetricEnum.BenchmarkMasterySocialStudies, ObjectiveName = "wrong data"}, 
                new StudentMetricObjective { StudentUSI = suppliedStudentId1, SchoolId = suppliedSchoolId, MetricId = (int)StudentMetricEnum.BenchmarkMasterySocialStudies, ObjectiveName = objectiveName1 + socialStudies, Value= "1/22", MetricStateTypeId = 1 },
                new StudentMetricObjective { StudentUSI = suppliedStudentId1, SchoolId = suppliedSchoolId, MetricId = (int)StudentMetricEnum.BenchmarkMasterySocialStudies, ObjectiveName = objectiveName2 + socialStudies, Value= "2/32", MetricStateTypeId = 3 },
                new StudentMetricObjective { StudentUSI = suppliedStudentId1, SchoolId = suppliedSchoolId, MetricId = (int)StudentMetricEnum.BenchmarkMasterySocialStudies, ObjectiveName = objectiveName3 + socialStudies, Value= "3/42" },
                               
                new StudentMetricObjective { StudentUSI = suppliedStudentId5, SchoolId = suppliedSchoolId, MetricId = (int)StudentMetricEnum.BenchmarkMasterySocialStudies, ObjectiveName = objectiveName1 + socialStudies, Value= "15/161", MetricStateTypeId = 1 },
                new StudentMetricObjective { StudentUSI = suppliedStudentId5, SchoolId = suppliedSchoolId, MetricId = (int)StudentMetricEnum.BenchmarkMasteryMath, ObjectiveName = objectiveName1 + math, Value= "15/161", MetricStateTypeId = 1 },
            };

            return data.AsQueryable();
        }

        protected IQueryable<TeacherStudentSection> GetSuppliedTeacherStudentSection()
        {
            var data = new List<TeacherStudentSection>
            {
                new TeacherStudentSection{ TeacherSectionId = suppliedTeacherSectionId1, StudentUSI = suppliedStudentId1},
                new TeacherStudentSection{ TeacherSectionId = suppliedTeacherSectionId2, StudentUSI = suppliedStudentId1},
                new TeacherStudentSection{ TeacherSectionId = suppliedTeacherSectionId3, StudentUSI = suppliedStudentId1},
                new TeacherStudentSection{ TeacherSectionId = suppliedTeacherSectionId4, StudentUSI = suppliedStudentId1},
            };

            return data.AsQueryable();
        }

        protected IQueryable<TeacherSection> GetSuppliedTeacherSection()
        {
            var data = new List<TeacherSection>
            {
                new TeacherSection{ StaffUSI = suppliedStaffUSI + 1, SchoolId = suppliedSchoolId, TeacherSectionId = suppliedTeacherSectionId1, LocalCourseCode = "wrong data"},
                new TeacherSection{ StaffUSI = suppliedStaffUSI, SchoolId = suppliedSchoolId + 1, TeacherSectionId = suppliedTeacherSectionId5, LocalCourseCode = "wrong data"},
                new TeacherSection{ StaffUSI = suppliedStaffUSI, SchoolId = suppliedSchoolId, TeacherSectionId = suppliedTeacherSectionId1, LocalCourseCode = suppliedLocalCourseCode1, SubjectArea = suppliedSubjectAreaELA, },
                new TeacherSection{ StaffUSI = suppliedStaffUSI, SchoolId = suppliedSchoolId, TeacherSectionId = suppliedTeacherSectionId2, LocalCourseCode = suppliedLocalCourseCode2, SubjectArea = suppliedSubjectAreaMath, },
                new TeacherSection{ StaffUSI = suppliedStaffUSI, SchoolId = suppliedSchoolId, TeacherSectionId = suppliedTeacherSectionId3, LocalCourseCode = suppliedLocalCourseCode3, SubjectArea = suppliedSubjectAreaScience, },
                new TeacherSection{ StaffUSI = suppliedStaffUSI, SchoolId = suppliedSchoolId, TeacherSectionId = suppliedTeacherSectionId4, LocalCourseCode = suppliedLocalCourseCode4, SubjectArea = suppliedSubjectAreaSocialStudies, },
                new TeacherSection{ StaffUSI = suppliedStaffUSI, SchoolId = suppliedSchoolId, TeacherSectionId = suppliedTeacherSectionId6, LocalCourseCode = suppliedLocalCourseCode6, SubjectArea = suppliedSubjectAreaSocialStudies, }
            };

            return data.AsQueryable();
        }

        protected IQueryable<StaffStudentCohort> GetSuppliedStaffStudentCohort()
        {
            var data = new List<StaffStudentCohort>
            {
                new StaffStudentCohort{ StaffCohortId = suppliedStaffCohortId2, StudentUSI = suppliedStudentId1},
                new StaffStudentCohort{ StaffCohortId = suppliedStaffCohortId2, StudentUSI = suppliedStudentId5},
                new StaffStudentCohort{ StaffCohortId = suppliedStaffCohortId1, StudentUSI = suppliedStudentId5},
            };

            return data.AsQueryable();
        }

        protected IQueryable<StaffCohort> GetSuppliedStaffCohort()
        {
            var data = new List<StaffCohort>
            {
                new StaffCohort{ StaffUSI = suppliedStaffUSI + 1, EducationOrganizationId = suppliedSchoolId, AcademicSubjectType = "wrong data"},
                new StaffCohort{ StaffUSI = suppliedStaffUSI, EducationOrganizationId = suppliedSchoolId + 1, StaffCohortId = suppliedStaffCohortId3, AcademicSubjectType = "wrong data"},
                new StaffCohort{ StaffUSI = suppliedStaffUSI, EducationOrganizationId = suppliedSchoolId, StaffCohortId = suppliedStaffCohortId1, AcademicSubjectType = suppliedStaffCohortSubjectArea1 },
                new StaffCohort{ StaffUSI = suppliedStaffUSI, EducationOrganizationId = suppliedSchoolId, StaffCohortId = suppliedStaffCohortId2, AcademicSubjectType = suppliedStaffCohortSubjectArea2 },
            };

            return data.AsQueryable();
        }

        protected MetricMetadataNode GetStudentRootOverviewNode()
        {
            return new MetricMetadataNode
            {
                MetricId = 2,
                Name = "Student's Overview",
                MetricNodeId = suppliedMetricNodeId,
                Children = new List<MetricMetadataNode>
                {
                                new MetricMetadataNode{MetricId=21, MetricNodeId = 71, Name = "Student's Attendance and Discipline",
                                Children = new List<MetricMetadataNode>
                                                {
                                                    new MetricMetadataNode{MetricId=211, MetricNodeId = 711, Name = "Attendance"},
                                                    new MetricMetadataNode{MetricId=212, Name = "Discipline"} 
                                                }
                                },
                                new MetricMetadataNode{MetricId=22, MetricNodeId = 72, Name = "School's Other Metric"},
                }
            };
        }

        #endregion

        [Test]
        public void Should_load_correct_students()
        {
            Assert.That(actualModel.Students.Select(x => x.StudentUSI).Distinct().Count(), Is.EqualTo(expectedStudentIds.Count));
            foreach (var student in actualModel.Students)
            {
                Assert.That(expectedStudentIds.Contains(student.StudentUSI), Is.True, "Student USI " + student.StudentUSI + " was missing.");
            }
        }

        [Test]
        public void Should_have_expected_metric_title()
        {
            Assert.That(actualModel.MetricTitle, Is.EqualTo(expectedMetricTitle));
        }

        [Test]
        public void Should_have_unique_list_id_populated()
        {
            Assert.That(actualModel.UniqueListId, Is.EqualTo(suppliedUniqueListId));
        }

        [Test]
        public void Should_have_no_unassigned_values_on_presentation_model()
        {
            actualModel.EnsureNoDefaultValues(
                "AssessmentDetailsModel.Students[0].IsFlagged",
                "AssessmentDetailsModel.Students[1].IsFlagged",
                "AssessmentDetailsModel.Students[2].IsFlagged",
                "AssessmentDetailsModel.Students[3].IsFlagged",
                "AssessmentDetailsModel.Students[4].IsFlagged",
                "AssessmentDetailsModel.Students[5].IsFlagged",
                "AssessmentDetailsModel.Students[6].IsFlagged",
                "AssessmentDetailsModel.Students[7].IsFlagged",
                "AssessmentDetailsModel.Students[8].IsFlagged",
                "AssessmentDetailsModel.Students[9].IsFlagged",
                "AssessmentDetailsModel.Students[10].IsFlagged",
                "AssessmentDetailsModel.Students[11].IsFlagged",
                "AssessmentDetailsModel.Students[0].Links",
                "AssessmentDetailsModel.ObjectiveTitles[0].Description",
                "AssessmentDetailsModel.ObjectiveTitles[1].Description",
                "AssessmentDetailsModel.ObjectiveTitles[2].Description"
                );
        }

        [Test]
        public void Should_have_correct_objective_titles()
        {
            Assert.That(actualModel.ObjectiveTitles.Count, Is.EqualTo(expectedObjectiveTitleCount));
            foreach (var objectiveTitle in actualModel.ObjectiveTitles)
            {
                Assert.That(objectiveTitle.Title, Is.StringEnding(expectedMetricTitle));
            }
        }
        
        [Test]
        public void Should_load_correct_number_of_metrics()
        {
            var suppliedObjectives = GetSuppliedStudentMetricObjective();

            foreach (var student in actualModel.Students)
            {
                //student.Metrics
                var expectedObjectives = suppliedObjectives
                    .Where(x => 
                        x.StudentUSI == student.StudentUSI
                        && x.SchoolId == suppliedSchoolId
                        && x.MetricId == student.Score.MetricVariantId 
                        );

                Console.WriteLine("sutdentUSI: {0}, schoolId: {1}, MetricCount: {2}", student.StudentUSI, student.SchoolId, student.Metrics.Count);

                Assert.That(student.Metrics.Count, Is.EqualTo(expectedObjectives.Count()));

            }
        }

        [Test]
        public void Should_load_correct_objectives()
        {
            var suppliedObjectives = GetSuppliedStudentMetricObjective();

            foreach (var student in actualModel.Students)
            {
                //student.Metrics
                var expectedObjectives = suppliedObjectives
                    .Where(x => 
                        x.StudentUSI == student.StudentUSI
                        && x.SchoolId == suppliedSchoolId
                        && x.MetricId == student.Score.MetricVariantId 
                        );

                foreach (var expectedObjective in expectedObjectives)
                {
                    var objective = student.Metrics.OfType<StudentWithMetricsAndAssessments.AssessmentMetric>().SingleOrDefault(x => x.ObjectiveName == expectedObjective.ObjectiveName);
                    Assert.That(objective, Is.Not.Null);
                    Assert.That(actualModel.ObjectiveTitles, Has.Some.Property("Title").StringContaining(expectedObjective.ObjectiveName));
                        
                    Assert.That(objective.StudentUSI, Is.EqualTo(student.StudentUSI));
                    Assert.That(objective.MetricVariantId, Is.EqualTo(expectedObjective.MetricId));
                    Assert.That(objective.DisplayValue, Is.EqualTo(expectedObjective.Value));
                    if (expectedObjective.MetricStateTypeId == null)
                        Assert.That(objective.State, Is.EqualTo(MetricStateType.None));
                    else
                        Assert.That(objective.State, Is.EqualTo((MetricStateType)expectedObjective.MetricStateTypeId));
                }
            }
        }
    }

    public class When_loading_reading_benchmark_assessment_data_for_teacher_section :
        AssessmentDetailsServiceFixtureBase
    {
        protected override void EstablishContext()
        {
            suppliedStudentListType = StudentListType.Section.ToString();
            suppliedSectionId = suppliedTeacherSectionId1;
            suppliedSubjectArea = StaffModel.SubjectArea.ELA;
            suppliedAssessmentSubType = StaffModel.AssessmentSubType.Benchmark;
            requestedMetricId = (int) StudentMetricEnum.BenchmarkMasteryELAReading;

            expectedStudentIds.Add(suppliedStudentId1);
            expectedMetricTitle = reading;
            expectedObjectiveTitleCount = 3;

            base.EstablishContext();
        }

    }

    public class When_loading_writing_benchmark_assessment_data_for_teacher_section :
        AssessmentDetailsServiceFixtureBase
    {
        protected override void EstablishContext()
        {
            suppliedStudentListType = StudentListType.Section.ToString();
            suppliedSectionId = suppliedTeacherSectionId1;
            suppliedSubjectArea = StaffModel.SubjectArea.Writing;
            suppliedAssessmentSubType = StaffModel.AssessmentSubType.Benchmark;
            requestedMetricId = (int) StudentMetricEnum.BenchmarkMasteryWriting;
            
            expectedStudentIds.Add(suppliedStudentId1);
            expectedMetricTitle = writing;
            expectedObjectiveTitleCount = 3;

            base.EstablishContext();
        }
    }

    public class When_loading_math_benchmark_assessment_data_for_teacher_section : 
        AssessmentDetailsServiceFixtureBase
    {
        protected override void EstablishContext()
        {
            suppliedStudentListType = StudentListType.Section.ToString();
            suppliedSectionId = suppliedTeacherSectionId1;
            suppliedSubjectArea = StaffModel.SubjectArea.Mathematics;
            suppliedAssessmentSubType = StaffModel.AssessmentSubType.Benchmark;
            requestedMetricId = (int) StudentMetricEnum.BenchmarkMasteryMath;

            expectedStudentIds.Add(suppliedStudentId1);
            expectedMetricTitle = math;
            expectedObjectiveTitleCount = 3;

            base.EstablishContext();
        }
    }


    public class When_loading_science_benchmark_assessment_data_for_teacher_section :
        AssessmentDetailsServiceFixtureBase
    {
        protected override void EstablishContext()
        {
            suppliedStudentListType = StudentListType.Section.ToString();
            suppliedSectionId = suppliedTeacherSectionId1;
            suppliedSubjectArea = StaffModel.SubjectArea.Science;
            suppliedAssessmentSubType = StaffModel.AssessmentSubType.Benchmark;
            requestedMetricId = (int)StudentMetricEnum.BenchmarkMasteryScience;

            expectedStudentIds.Add(suppliedStudentId1);
            expectedMetricTitle = science;
            expectedObjectiveTitleCount = 3;

            base.EstablishContext();
        }
    }

    public class When_loading_social_studies_benchmark_assessment_data_for_teacher_section :
        AssessmentDetailsServiceFixtureBase
    {
        protected override void EstablishContext()
        {
            suppliedStudentListType = StudentListType.Section.ToString();
            suppliedSectionId = suppliedTeacherSectionId1;
            suppliedSubjectArea = StaffModel.SubjectArea.SocialStudies;
            suppliedAssessmentSubType = StaffModel.AssessmentSubType.Benchmark;
            requestedMetricId = (int) StudentMetricEnum.BenchmarkMasterySocialStudies;

            expectedStudentIds.Add(suppliedStudentId1);
            expectedMetricTitle = socialStudies;
            expectedObjectiveTitleCount = 3;

            base.EstablishContext();
        }
    }

    public class When_loading_social_studies_default_benchmark_assessment_data_for_teacher_section :
        AssessmentDetailsServiceFixtureBase
    {
        protected override void EstablishContext()
        {
            suppliedStudentListType = StudentListType.Section.ToString();
            suppliedSectionId = suppliedTeacherSectionId4;
            suppliedSubjectArea = StaffModel.SubjectArea.SocialStudies;
            suppliedAssessmentSubType = StaffModel.AssessmentSubType.Benchmark;
            requestedMetricId = (int) StudentMetricEnum.BenchmarkMasterySocialStudies;

            expectedStudentIds.Add(suppliedStudentId1);
            expectedMetricTitle = socialStudies;
            expectedObjectiveTitleCount = 3;

            base.EstablishContext();
        }
    }

    public class When_loading_math_benchmark_assessment_data_for_staff_cohort : 
        AssessmentDetailsServiceFixtureBase
    {
        protected override void EstablishContext()
        {
            suppliedStudentListType = StudentListType.Cohort.ToString();
            suppliedSectionId = suppliedStaffCohortId1;
            suppliedSubjectArea = StaffModel.SubjectArea.Mathematics;
            suppliedAssessmentSubType = StaffModel.AssessmentSubType.Benchmark;
            requestedMetricId = (int) StudentMetricEnum.BenchmarkMasteryMath;

            expectedStudentIds.Add(suppliedStudentId5);
            expectedMetricTitle = math;
            expectedObjectiveTitleCount = 1;

            base.EstablishContext();
        }
    }

    public class When_loading_social_studies_default_benchmark_assessment_data_for_staff_cohort :
        AssessmentDetailsServiceFixtureBase
    {
        protected override void EstablishContext()
        {
            suppliedStudentListType = StudentListType.Cohort.ToString();
            suppliedSectionId = suppliedStaffCohortId1;
            suppliedSubjectArea = StaffModel.SubjectArea.SocialStudies;
            suppliedAssessmentSubType = StaffModel.AssessmentSubType.Benchmark;
            requestedMetricId = (int) StudentMetricEnum.BenchmarkMasterySocialStudies;

            expectedStudentIds.Add(suppliedStudentId5);
            expectedMetricTitle = socialStudies;
            expectedObjectiveTitleCount = 1;

            base.EstablishContext();
        }
    }

    public class When_loading_benchmark_assessment_data_for_all_students : AssessmentDetailsServiceFixtureBase
    {
        protected override void EstablishContext()
        {
            suppliedStudentListType = StudentListType.All.ToString();
            suppliedSectionId = 0;
            suppliedSubjectArea = StaffModel.SubjectArea.Mathematics;
            suppliedAssessmentSubType = StaffModel.AssessmentSubType.Benchmark;
            requestedMetricId = (int)StudentMetricEnum.BenchmarkMasteryMath;

            // section kids
            expectedStudentIds.Add(suppliedStudentId1);

            // cohort kids
            expectedStudentIds.Add(suppliedStudentId5);

            expectedMetricTitle = math;
            expectedObjectiveTitleCount = 3;

            base.EstablishContext();
        }
    }

    public class When_all_students_are_mastering_a_strand : AssessmentDetailsServiceFixtureBase
    {
        protected override IQueryable<StudentMetricObjective> GetSuppliedStudentMetricObjective()
        {
            var data = new List<StudentMetricObjective>
            {
                new StudentMetricObjective { StudentUSI = suppliedStudentId1, SchoolId = suppliedSchoolId, MetricId = (int)StudentMetricEnum.BenchmarkMasteryMath, ObjectiveName = objectiveName1 + math, Value= "1/20", MetricStateTypeId = 1 },
                new StudentMetricObjective { StudentUSI = suppliedStudentId5, SchoolId = suppliedSchoolId, MetricId = (int)StudentMetricEnum.BenchmarkMasteryMath, ObjectiveName = objectiveName1 + math, Value= "4/20", MetricStateTypeId = 1 },
            };

            return data.AsQueryable();
        }

        protected override void EstablishContext()
        {

            suppliedStudentListType = StudentListType.All.ToString();
            suppliedSectionId = 0;
            suppliedSubjectArea = StaffModel.SubjectArea.Mathematics;
            suppliedAssessmentSubType = StaffModel.AssessmentSubType.Benchmark;
            requestedMetricId = (int)StudentMetricEnum.BenchmarkMasteryMath;

            expectedStudentIds.AddRange(new long[] { suppliedStudentId1, suppliedStudentId4, suppliedStudentId5 });

            expectedMetricTitle = math;
            expectedObjectiveTitleCount = 1;


            base.EstablishContext();
        }

        [Test]
        public void Objective_mastery_should_be_2_of_2()
        {
            Assert.That(actualModel.ObjectiveTitles[0].Mastery, Is.EqualTo("2 of 2"));
        }
    }

    public class When_some_students_are_mastering_a_strand : AssessmentDetailsServiceFixtureBase
    {
        protected override IQueryable<StudentMetricObjective> GetSuppliedStudentMetricObjective()
        {
            var data = new List<StudentMetricObjective>
            {
                new StudentMetricObjective { StudentUSI = suppliedStudentId1, SchoolId = suppliedSchoolId, MetricId = (int)StudentMetricEnum.BenchmarkMasteryMath, ObjectiveName = objectiveName1 + math, Value= "1/20", MetricStateTypeId = 1 },
                new StudentMetricObjective { StudentUSI = suppliedStudentId5, SchoolId = suppliedSchoolId, MetricId = (int)StudentMetricEnum.BenchmarkMasteryMath, ObjectiveName = objectiveName1 + math, Value= "4/20", MetricStateTypeId = 3 },
            };

            return data.AsQueryable();
        }

        protected override void EstablishContext()
        {

            suppliedStudentListType = StudentListType.All.ToString();
            suppliedSectionId = 0;
            suppliedSubjectArea = StaffModel.SubjectArea.Mathematics;
            suppliedAssessmentSubType = StaffModel.AssessmentSubType.Benchmark;
            requestedMetricId = (int)StudentMetricEnum.BenchmarkMasteryMath;

            expectedStudentIds.AddRange(new long[] { suppliedStudentId1, suppliedStudentId4, suppliedStudentId5 });

            expectedMetricTitle = math;
            expectedObjectiveTitleCount = 1;


            base.EstablishContext();
        }

        [Test]
        public void Objective_mastery_should_be_1_of_2()
        {
            Assert.That(actualModel.ObjectiveTitles[0].Mastery, Is.EqualTo("1 of 2"));
        }
    }

    public class When_no_students_are_mastering_a_strand : AssessmentDetailsServiceFixtureBase
    {
        protected override IQueryable<StudentMetricObjective> GetSuppliedStudentMetricObjective()
        {
            var data = new List<StudentMetricObjective>
            {
                new StudentMetricObjective { StudentUSI = suppliedStudentId1, SchoolId = suppliedSchoolId, MetricId = (int)StudentMetricEnum.BenchmarkMasteryMath, ObjectiveName = objectiveName1 + math, Value= "1/20", MetricStateTypeId = 3 },
                new StudentMetricObjective { StudentUSI = suppliedStudentId5, SchoolId = suppliedSchoolId, MetricId = (int)StudentMetricEnum.BenchmarkMasteryMath, ObjectiveName = objectiveName1 + math, Value= "4/20", MetricStateTypeId = 3 },
            };

            return data.AsQueryable();
        }

        protected override void EstablishContext()
        {

            suppliedStudentListType = StudentListType.All.ToString();
            suppliedSectionId = 0;
            suppliedSubjectArea = StaffModel.SubjectArea.Mathematics;
            suppliedAssessmentSubType = StaffModel.AssessmentSubType.Benchmark;
            requestedMetricId = (int)StudentMetricEnum.BenchmarkMasteryMath;

            expectedStudentIds.AddRange(new long[] { suppliedStudentId1, suppliedStudentId4, suppliedStudentId5 });

            expectedMetricTitle = math;
            expectedObjectiveTitleCount = 1;


            base.EstablishContext();
        }

        [Test]
        public void Objective_mastery_should_be_0_of_2()
        {
            Assert.That(actualModel.ObjectiveTitles[0].Mastery, Is.EqualTo("0 of 2"));
        }
    }

    public class When_all_students_are_acceptable_for_a_strand : AssessmentDetailsServiceFixtureBase
    {
        protected override IQueryable<StudentMetricObjective> GetSuppliedStudentMetricObjective()
        {
            var data = new List<StudentMetricObjective>
            {
                new StudentMetricObjective { StudentUSI = suppliedStudentId1, SchoolId = suppliedSchoolId, MetricId = (int)StudentMetricEnum.BenchmarkMasteryMath, ObjectiveName = objectiveName1 + math, Value= "1/20", MetricStateTypeId = (int?) MetricStateType.Acceptable },
                new StudentMetricObjective { StudentUSI = suppliedStudentId5, SchoolId = suppliedSchoolId, MetricId = (int)StudentMetricEnum.BenchmarkMasteryMath, ObjectiveName = objectiveName1 + math, Value= "4/20", MetricStateTypeId = (int?) MetricStateType.Acceptable },
            };

            return data.AsQueryable();
        }

        protected override void EstablishContext()
        {

            suppliedStudentListType = StudentListType.All.ToString();
            suppliedSectionId = 0;
            suppliedSubjectArea = StaffModel.SubjectArea.Mathematics;
            suppliedAssessmentSubType = StaffModel.AssessmentSubType.Benchmark;
            requestedMetricId = (int)StudentMetricEnum.BenchmarkMasteryMath;

            expectedStudentIds.AddRange(new long[] { suppliedStudentId1, suppliedStudentId4, suppliedStudentId5 });

            expectedMetricTitle = math;
            expectedObjectiveTitleCount = 1;


            base.EstablishContext();
        }

        [Test]
        public void Objective_mastery_should_be_2_of_2()
        {
            Assert.That(actualModel.ObjectiveTitles[0].Mastery, Is.EqualTo("2 of 2"));
        }
    }

    public class When_some_students_are_mastering_a_strand_and_some_are_acceptable : AssessmentDetailsServiceFixtureBase
    {
        protected override IQueryable<StudentMetricObjective> GetSuppliedStudentMetricObjective()
        {
            var data = new List<StudentMetricObjective>
            {
                new StudentMetricObjective { StudentUSI = suppliedStudentId1, SchoolId = suppliedSchoolId, MetricId = (int)StudentMetricEnum.BenchmarkMasteryMath, ObjectiveName = objectiveName1 + math, Value= "1/20", MetricStateTypeId = (int?) MetricStateType.Good },
                new StudentMetricObjective { StudentUSI = suppliedStudentId5, SchoolId = suppliedSchoolId, MetricId = (int)StudentMetricEnum.BenchmarkMasteryMath, ObjectiveName = objectiveName1 + math, Value= "4/20", MetricStateTypeId = (int?) MetricStateType.Acceptable },
            };

            return data.AsQueryable();
        }

        protected override void EstablishContext()
        {

            suppliedStudentListType = StudentListType.All.ToString();
            suppliedSectionId = 0;
            suppliedSubjectArea = StaffModel.SubjectArea.Mathematics;
            suppliedAssessmentSubType = StaffModel.AssessmentSubType.Benchmark;
            requestedMetricId = (int)StudentMetricEnum.BenchmarkMasteryMath;

            expectedStudentIds.AddRange(new long[] { suppliedStudentId1, suppliedStudentId4, suppliedStudentId5 });

            expectedMetricTitle = math;
            expectedObjectiveTitleCount = 1;


            base.EstablishContext();
        }

        [Test]
        public void Objective_mastery_should_be_2_of_2()
        {
            Assert.That(actualModel.ObjectiveTitles[0].Mastery, Is.EqualTo("2 of 2"));
        }
    }

}
