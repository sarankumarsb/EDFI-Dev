// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
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
using Rhino.Mocks.Constraints;
using Is = NUnit.Framework.Is;
using MetricComponent = EdFi.Dashboards.Metric.Data.Entities.MetricComponent;

namespace EdFi.Dashboards.Resources.Tests.Staff
{
    public abstract class SubjectSpecificOverviewServiceFixtureBase : TestFixtureBase
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
        protected const string suppliedSubjectAreaSocialStudies = "Social Studies";
        protected const string suppliedSubjectAreaWriting = "Writing";
        protected const int suppliedStaffCohortId1 = 130;
        protected const int suppliedStaffCohortId2 = 140;
        protected const int suppliedStaffCohortId3 = 150;
        protected const string suppliedStaffCohortSubjectArea1 = "Science";
        protected const string suppliedStaffCohortSubjectArea2 = "subject area 4";
        protected const int suppliedStudentId1 = 160;
        protected const int suppliedStudentId2 = 170;
        protected const int suppliedStudentId3 = 180;
        protected const int suppliedStudentId4 = 190;
        protected const int suppliedStudentId5 = 200;
        protected const int suppliedStudentId6 = 210;
        protected const int suppliedStudentId7 = 220;
        protected const int suppliedStudentId8 = 230;
        protected const int suppliedStudentId9 = 240;
        protected const int suppliedStudentId10 = 250;
        protected const int suppliedStudentId11 = 260;
        protected const int suppliedStudentId12 = 270;
        protected Guid suppliedStudentMetricInstanceSetKey1 = Guid.NewGuid();
        protected Guid suppliedStudentMetricInstanceSetKey2 = Guid.NewGuid();
        protected Guid suppliedStudentMetricInstanceSetKey3 = Guid.NewGuid();
        protected const TrendEvaluation suppliedTrendEvaluation1 = TrendEvaluation.DownGood;
        protected const TrendEvaluation suppliedTrendEvaluation2 = TrendEvaluation.UpBad;
        protected const TrendDirection suppliedTrendDirection1 = TrendDirection.Decreasing;
        protected const TrendDirection suppliedTrendDirection2 = TrendDirection.Increasing;
        protected const TrendInterpretation suppliedTrendInterpretation1 = TrendInterpretation.Inverse;
        protected const TrendInterpretation suppliedTrendInterpretation2 = TrendInterpretation.Standard;
        protected const int suppliedNumericGrade1 = 71;
        protected const int suppliedNumericGrade2 = 70;
        protected const int suppliedNumericGrade3 = 69;
        protected const int suppliedNumericGrade4 = 90;
        protected const int suppliedNumericGrade6 = 20;
        protected const string suppliedLetterGrade1 = "C";
        protected const string suppliedLetterGrade2 = "G";
        protected const string suppliedLetterGrade3 = "D";
        protected const string suppliedLetterGrade4 = "C+";
        protected const string suppliedLetterGrade6 = "C-";
        protected const int suppliedStaffUSI = 1000;
        protected const int suppliedSchoolId = 2000;
        protected const int suppliedLocalEducationAgencyId = 3000;
        protected const int suppliedUniqueIdentifier = 500;
        protected string suppliedStudentListType;
        protected int suppliedSectionId;
        protected const string suppliedUniqueListId = "baboon";
        protected const string suppliedImageName = "test.jpg";
        protected const string reading = "ELA / Reading";
        protected const string math = "Mathematics";
        protected const string science = "Science";
        protected const string socialStudies = "Social Studies";
        protected const string writing = "Writing";
        protected const string gradeLevel1 = "grade level 1";
        protected const int suppliedMetricNodeId = 7;

        protected IRepository<StudentMetric> studentListWithMetricsRepository;
        protected IRepository<TeacherStudentSection> teacherStudentSectionRepository;
        protected IRepository<TeacherSection> teacherSectionRepository;
        protected IRepository<StaffStudentCohort> staffStudentCohortRepository;
        protected IRepository<StaffCohort> staffCohortRepository;
        protected IRepository<MetricComponent> metricComponentRepository;
        protected IRepository<StudentSchoolMetricInstanceSet> studentSchoolMetricInstanceSetRepository;
        protected IRepository<StudentRecordCurrentCourse> studentRecordCurrentCourseRepository;
        protected IRepository<Dashboards.Metric.Data.Entities.Metric> metricRepository;
        protected ISchoolCategoryProvider schoolCategoryProvider;
        protected IAccommodationProvider accommodationProvider;
        protected IUniqueListIdProvider uniqueListProvider;
        protected ITrendRenderingDispositionProvider trendRenderingDispositionProvider;
        protected IGradeStateProvider gradeStateProvider;
        protected IRootMetricNodeResolver rootMetricNodeResolver;
        protected IMetricStateProvider metricStateProvider;
        protected IClassroomMetricsProvider classroomMetricsProvider;
        protected IStudentSchoolAreaLinks studentSchoolAreaLinks = new StudentSchoolAreaLinksFake();
        protected IStudentListUtilitiesProvider studentListUtilitiesProvider;
        protected IMetadataListIdResolver metadataListIdResolver;
        protected IListMetadataProvider listMetadataProvider;
        protected ISubjectSpecificOverviewMetricComponentProvider subjectSpecificOverviewMetricComponentProvider;
        protected IStudentMetricsProvider studentListWithMetricsProvider;
        protected IGradeLevelUtilitiesProvider gradeLevelUtilitiesProvider;

        protected SubjectSpecificOverviewModel actualModel;

        protected List<long> expectedStudentIds = new List<long>();
        protected string expectedSubjectArea;
        protected SchoolCategory expectedSchoolCategory;
        protected int expectedMetricCount;

        protected override void EstablishContext()
        {
            studentListWithMetricsRepository = mocks.StrictMock<IRepository<StudentMetric>>();
            teacherStudentSectionRepository = mocks.StrictMock<IRepository<TeacherStudentSection>>();
            teacherSectionRepository = mocks.StrictMock<IRepository<TeacherSection>>();
            staffStudentCohortRepository = mocks.StrictMock<IRepository<StaffStudentCohort>>();
            staffCohortRepository = mocks.StrictMock<IRepository<StaffCohort>>();
            metricComponentRepository = mocks.StrictMock<IRepository<MetricComponent>>();
            studentSchoolMetricInstanceSetRepository = mocks.StrictMock<IRepository<StudentSchoolMetricInstanceSet>>();
            studentRecordCurrentCourseRepository = mocks.StrictMock<IRepository<StudentRecordCurrentCourse>>();
            metricRepository = mocks.StrictMock<IRepository<Dashboards.Metric.Data.Entities.Metric>>();
            schoolCategoryProvider = mocks.StrictMock<ISchoolCategoryProvider>();
            accommodationProvider = mocks.StrictMock<IAccommodationProvider>();
            uniqueListProvider = mocks.StrictMock<IUniqueListIdProvider>();
            trendRenderingDispositionProvider = mocks.StrictMock<ITrendRenderingDispositionProvider>();
            gradeStateProvider = mocks.StrictMock<IGradeStateProvider>();
            rootMetricNodeResolver = mocks.StrictMock<IRootMetricNodeResolver>();
            metricStateProvider = mocks.StrictMock<IMetricStateProvider>();
            classroomMetricsProvider = mocks.StrictMock<IClassroomMetricsProvider>();
            studentListUtilitiesProvider = mocks.StrictMock<IStudentListUtilitiesProvider>();
            metadataListIdResolver = mocks.StrictMock<IMetadataListIdResolver>();
            listMetadataProvider = mocks.StrictMock<IListMetadataProvider>();
            subjectSpecificOverviewMetricComponentProvider = mocks.StrictMock<ISubjectSpecificOverviewMetricComponentProvider>();
            studentListWithMetricsProvider = mocks.StrictMock<IStudentMetricsProvider>();
            gradeLevelUtilitiesProvider = mocks.StrictMock<IGradeLevelUtilitiesProvider>();

            Expect.Call(studentListWithMetricsRepository.GetAll()).Repeat.Any().Return(GetSuppliedStudentMetrics());
            Expect.Call(teacherStudentSectionRepository.GetAll()).Repeat.Any().Return(GetSuppliedTeacherStudentSection());
            Expect.Call(teacherSectionRepository.GetAll()).Repeat.Any().Return(GetSuppliedTeacherSection());
            Expect.Call(staffStudentCohortRepository.GetAll()).Repeat.Any().Return(GetSuppliedStaffStudentCohort());
            Expect.Call(staffCohortRepository.GetAll()).Repeat.Any().Return(GetSuppliedStaffCohort());
            Expect.Call(metricComponentRepository.GetAll()).Repeat.Any().Return(GetSuppliedMetricComponent());
            Expect.Call(studentSchoolMetricInstanceSetRepository.GetAll()).Repeat.Any().Return(GetSuppliedStudentSchoolMetricInstanceSet());
            Expect.Call(studentRecordCurrentCourseRepository.GetAll()).Repeat.Any().Return(GetSuppliedStudentRecordCurrentCourse());
            Expect.Call(metricRepository.GetAll()).Repeat.Any().Return(GetMetric());

            Expect.Call(uniqueListProvider.GetUniqueId()).Repeat.Twice().Return(suppliedUniqueListId);
            Expect.Call(gradeStateProvider.Get(0)).IgnoreArguments().Repeat.Any().Return(MetricStateType.Acceptable);
            Expect.Call(gradeStateProvider.Get("A")).IgnoreArguments().Repeat.Any().Return(MetricStateType.Acceptable);

            Expect.Call(metricStateProvider.GetState(0, String.Empty, String.Empty)).IgnoreArguments().Repeat.Any().Return(new State(MetricStateType.Good, "Good"));

            Expect.Call(trendRenderingDispositionProvider.GetTrendRenderingDisposition(TrendDirection.Increasing, TrendInterpretation.Standard)).Repeat.Any().Return(TrendEvaluation.UpGood);
            Expect.Call(trendRenderingDispositionProvider.GetTrendRenderingDisposition(TrendDirection.Decreasing, TrendInterpretation.Standard)).Repeat.Any().Return(TrendEvaluation.DownBad);

            Expect.Call(rootMetricNodeResolver.GetRootMetricNodeForStudent(suppliedSchoolId)).Repeat.Any().Return(GetStudentRootOverviewNode());

            foreach (var studentId in expectedStudentIds)
                Expect.Call(classroomMetricsProvider.GetAdditionalMetrics(null, null)).IgnoreArguments().Constraints(Property.Value("StudentUSI", studentId), Rhino.Mocks.Constraints.Is.Anything()).Repeat.Any().Return(GetSuppliedMetrics());

            //Tested Elsewhere.
            Expect.Call(metadataListIdResolver.GetListId(ListType.ClassroomSubjectSpecific, SchoolCategory.HighSchool, "subjectArea")).IgnoreArguments().Repeat.Any().Return(1);
            Expect.Call(listMetadataProvider.GetListMetadata(1, "subjectArea")).IgnoreArguments().Repeat.Any().Return(GetSuppliedListMetadata());
            
            Expect.Call(studentListUtilitiesProvider.PrepareMetric(1, 1, 1, "1", 1, "1", "1")).IgnoreArguments().Repeat.Any().Return(FakeMetric());
            Expect.Call(studentListUtilitiesProvider.PrepareTrendMetric(1, 1, 1, "1", 1, "1", "1", 1, 1, null)).IgnoreArguments().Repeat.Any().Return(FakeTrendMetric());

            Expect.Call(subjectSpecificOverviewMetricComponentProvider.GetMetricIdsForComponents()).Return(new int[] { (int)StudentMetricEnum.AbsenceLevelCurrentPeriod, (int)StudentMetricEnum.ClassGradeGradesFalling10PercentOrMore });
            Expect.Call(studentListWithMetricsProvider.GetOrderedStudentList(GetStudentListWithMetricsQueryOptions())).IgnoreArguments().Repeat.Any().Return(GetStudentListData(suppliedStudentListType));
            Expect.Call(gradeLevelUtilitiesProvider.FormatGradeLevelForSorting("1")).IgnoreArguments().Repeat.AtLeastOnce().Return(1);
            Expect.Call(studentListWithMetricsProvider.GetStudentsWithMetrics(GetStudentListWithMetricsQueryOptions())).IgnoreArguments().Repeat.Any().Return(GetStudentListWithMetricsData(suppliedStudentListType));
            Expect.Call(gradeLevelUtilitiesProvider.FormatGradeLevelForDisplay(null)).Repeat.Any().Return(string.Empty);

            base.EstablishContext();
        }
        private IQueryable<StudentMetric> GetStudentListWithMetricsData(string studentListType)
        {
            switch (studentListType)
            {
                case "Section":
                    return from ts in GetSuppliedTeacherStudentSection()
                           join sl in GetSuppliedStudentMetrics()
                               on ts.StudentUSI equals sl.StudentUSI
                           where ts.TeacherSectionId == suppliedSectionId && sl.SchoolId == suppliedSchoolId
                           select sl;
                default:
                    return from ssc in GetSuppliedStaffStudentCohort()
                           join sl in GetSuppliedStudentMetrics()
                               on ssc.StudentUSI equals sl.StudentUSI
                           where ssc.StaffCohortId == suppliedSectionId && sl.SchoolId == suppliedSchoolId
                           select sl;
            }
        }

        private IQueryable<EnhancedStudentInformation> GetStudentListData(string studentListType)
        {
            switch (studentListType)
            {
                case "Section":
                    return from ts in GetSuppliedTeacherStudentSection()
                           join sl in GetSuppliedStudentList()
                               on ts.StudentUSI equals sl.StudentUSI
                           where ts.TeacherSectionId == suppliedSectionId && sl.SchoolId == suppliedSchoolId
                           select sl;
                default:
                    return from ssc in GetSuppliedStaffStudentCohort()
                           join sl in GetSuppliedStudentList()
                               on ssc.StudentUSI equals sl.StudentUSI
                           where ssc.StaffCohortId == suppliedSectionId && sl.SchoolId == suppliedSchoolId
                           select sl;
            }
        }

        protected StudentMetricsProviderQueryOptions GetStudentListWithMetricsQueryOptions()
        {
            var suppliedStudentUSIs = new List<long>();
            var suppliedStaffUsi = 0;
            
            return new StudentMetricsProviderQueryOptions
                {
                    SchoolId = suppliedSchoolId,
                    StaffUSI = suppliedStaffUsi,
                    StudentIds = suppliedStudentUSIs
                };
        }

        protected List<MetadataColumnGroup> GetSuppliedListMetadata()
        {
            if (expectedSchoolCategory == SchoolCategory.HighSchool)

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
                                                             UniqueIdentifier = suppliedUniqueIdentifier + 1,
                                                             ColumnName = "ColumnName",
                                                             ColumnPrefix = "ColumnPrefix",
                                                             IsVisibleByDefault = true,
                                                             MetricVariantId = (int)StudentMetricEnum.AbsenceLevelCurrentPeriod,
                                                             MetricListCellType = MetricListCellType.TrendMetric,
                                                             Order = 1,
                                                             SchoolCategory = SchoolCategory.HighSchool,
                                                             SortAscending = "SortAscending",
                                                             SortDescending = "SortDescending",
                                                             Tooltip = "Tooltip",
                                                             IsFixedColumn = true
                                                         },
                                                         new MetadataColumn
                                                         {
                                                             UniqueIdentifier = suppliedUniqueIdentifier + 10,
                                                             ColumnName = "Metric 1",
                                                             ColumnPrefix = "ColumnPrefix",
                                                             IsVisibleByDefault = true,
                                                             MetricVariantId = (int)StudentMetricEnum.ACT,
                                                             MetricListCellType = MetricListCellType.Metric,
                                                             Order = 2,
                                                             SchoolCategory = SchoolCategory.HighSchool,
                                                             SortAscending = "SortAscending",
                                                             SortDescending = "SortDescending",
                                                             Tooltip = "Tooltip",
                                                             IsFixedColumn = true
                                                         },
                                                         new MetadataColumn
                                                         {
                                                             UniqueIdentifier = suppliedUniqueIdentifier + 11,
                                                             ColumnName = "Metric 2",
                                                             ColumnPrefix = "ColumnPrefix",
                                                             IsVisibleByDefault = true,
                                                             MetricVariantId = (int)StudentMetricEnum.ACTMetStateCriterion,
                                                             MetricListCellType = MetricListCellType.Metric,
                                                             Order = 2,
                                                             SchoolCategory = SchoolCategory.HighSchool,
                                                             SortAscending = "SortAscending",
                                                             SortDescending = "SortDescending",
                                                             Tooltip = "Tooltip",
                                                             IsFixedColumn = true
                                                         },
                                                         new MetadataColumn
                                                         {
                                                             UniqueIdentifier = suppliedUniqueIdentifier + 2,
                                                             ColumnName = "ColumnName",
                                                             ColumnPrefix = "ColumnPrefix",
                                                             IsVisibleByDefault = true,
                                                             MetricVariantId = (int)StudentMetricEnum.ClassGradeGradesFalling10PercentOrMore,
                                                             MetricListCellType = MetricListCellType.TrendMetric,
                                                             Order = 2,
                                                             SchoolCategory = SchoolCategory.HighSchool,
                                                             SortAscending = "SortAscending",
                                                             SortDescending = "SortDescending",
                                                             Tooltip = "Tooltip",
                                                             IsFixedColumn = true
                                                         },
                                                         new MetadataColumn
                                                         {
                                                             UniqueIdentifier = suppliedUniqueIdentifier,
                                                             ColumnName = "ColumnName",
                                                             ColumnPrefix = "ColumnPrefix",
                                                             IsVisibleByDefault = true,
                                                             MetricVariantId = (int)StudentMetricEnum.SubjectAreaCourseGrades,
                                                             MetricListCellType = MetricListCellType.TrendMetric,
                                                             Order = 3,
                                                             SchoolCategory = SchoolCategory.HighSchool,
                                                             SortAscending = "SortAscending",
                                                             SortDescending = "SortDescending",
                                                             Tooltip = "Tooltip",
                                                             IsFixedColumn = true
                                                         }
                                                 }
                               }
                       };

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
                                                             UniqueIdentifier = suppliedUniqueIdentifier + 10,
                                                             ColumnName = "Metric 1",
                                                             ColumnPrefix = "ColumnPrefix",
                                                             IsVisibleByDefault = true,
                                                             MetricVariantId = (int)StudentMetricEnum.ACT,
                                                             MetricListCellType = MetricListCellType.Metric,
                                                             Order = 2,
                                                             SchoolCategory = SchoolCategory.HighSchool,
                                                             SortAscending = "SortAscending",
                                                             SortDescending = "SortDescending",
                                                             Tooltip = "Tooltip",
                                                             IsFixedColumn = true
                                                         },
                                                         new MetadataColumn
                                                         {
                                                             UniqueIdentifier = suppliedUniqueIdentifier + 11,
                                                             ColumnName = "Metric 2",
                                                             ColumnPrefix = "ColumnPrefix",
                                                             IsVisibleByDefault = true,
                                                             MetricVariantId = (int)StudentMetricEnum.ACTMetStateCriterion,
                                                             MetricListCellType = MetricListCellType.Metric,
                                                             Order = 2,
                                                             SchoolCategory = SchoolCategory.HighSchool,
                                                             SortAscending = "SortAscending",
                                                             SortDescending = "SortDescending",
                                                             Tooltip = "Tooltip",
                                                             IsFixedColumn = true
                                                         },
                                                         new MetadataColumn
                                                         {
                                                             UniqueIdentifier = suppliedUniqueIdentifier + 12,
                                                             ColumnName = "Metric 3",
                                                             ColumnPrefix = "ColumnPrefix",
                                                             IsVisibleByDefault = true,
                                                             MetricVariantId = (int)StudentMetricEnum.ACTMetStateCriterion,
                                                             MetricListCellType = MetricListCellType.Metric,
                                                             Order = 2,
                                                             SchoolCategory = SchoolCategory.HighSchool,
                                                             SortAscending = "SortAscending",
                                                             SortDescending = "SortDescending",
                                                             Tooltip = "Tooltip",
                                                             IsFixedColumn = true
                                                         },
                                                         new MetadataColumn
                                                         {
                                                             UniqueIdentifier = suppliedUniqueIdentifier,
                                                             ColumnName = "ColumnName",
                                                             ColumnPrefix = "ColumnPrefix",
                                                             IsVisibleByDefault = true,
                                                             MetricVariantId = (int)StudentMetricEnum.SubjectAreaCourseGrades,
                                                             MetricListCellType = MetricListCellType.TrendMetric,
                                                             Order = 3,
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

        protected List<StudentWithMetrics.Metric> GetSuppliedMetrics()
        {
            var result = new List<StudentWithMetrics.Metric>();
            var metricCount = 2;

            if (expectedSchoolCategory == SchoolCategory.Elementary)
                metricCount = 3;

            for (int i = 0; i < metricCount; i++)
                result.Add(new StudentWithMetrics.Metric(1)
                               {
                                   UniqueIdentifier = suppliedUniqueIdentifier + i,
                                   MetricVariantId = 1,
                                   DisplayValue = "something",
                                   State = MetricStateType.Good,
                                   Value = 1
                               });

            return result;
        }

        protected StudentWithMetrics.Metric FakeMetric()
        {
            return new StudentWithMetrics.Metric(1)
                       {
                           UniqueIdentifier = suppliedUniqueIdentifier + 10,
                           DisplayValue = "something",
                           MetricVariantId = 1,
                           State = MetricStateType.Good,
                           Value = 1
                       };
        }

        protected StudentWithMetrics.TrendMetric FakeTrendMetric()
        {
            return new StudentWithMetrics.TrendMetric(1)
            {
                UniqueIdentifier = suppliedUniqueIdentifier + 20,
                Trend = TrendEvaluation.DownGood,
                DisplayValue = "something",
                MetricVariantId = 1,
                State = MetricStateType.Good,
                Value = 1
            };
        }

        protected IQueryable<StudentMetric> GetSuppliedStudentMetrics()
        {
            var data = new List<StudentMetric>
                           {
                               new StudentMetric{ StudentUSI = suppliedStudentId1,  SchoolId = suppliedSchoolId + 1,},
                               new StudentMetric{ StudentUSI = suppliedStudentId1,  SchoolId = suppliedSchoolId,  MetricId = (int)StudentMetricEnum.DisciplineIncidents, Value = "2", MetricStateTypeId = 3, ValueTypeName = "System.Int32", Format = "did {0}"},
                               new StudentMetric{ StudentUSI = suppliedStudentId1,  SchoolId = suppliedSchoolId,  MetricId = (int)StudentMetricEnum.BenchmarkMasteryELAReading, Value = ".32", MetricStateTypeId = 1, ValueTypeName = "System.Double", Format = "apple {0:P1}" },
                               new StudentMetric{ StudentUSI = suppliedStudentId1,  SchoolId = suppliedSchoolId,  MetricId = (int)StudentMetricEnum.BenchmarkMasteryWriting, Value = ".43", MetricStateTypeId = 1, ValueTypeName = "System.Double", Format = "banana {0:P1}"},
                               new StudentMetric{ StudentUSI = suppliedStudentId1,  SchoolId = suppliedSchoolId,  MetricId = (int)StudentMetricEnum.BenchmarkMasteryMath, Value = ".54", MetricStateTypeId = 1, ValueTypeName = "System.Double", Format = "carrot {0:P1}"},
                               new StudentMetric{ StudentUSI = suppliedStudentId1,  SchoolId = suppliedSchoolId,  MetricId = (int)StudentMetricEnum.BenchmarkMasteryScience, Value = ".65", MetricStateTypeId = 1, ValueTypeName = "System.Double", Format = "deer {0:P1}"},
                               new StudentMetric{ StudentUSI = suppliedStudentId1,  SchoolId = suppliedSchoolId,  MetricId = (int)StudentMetricEnum.BenchmarkMasterySocialStudies, Value = ".76", MetricStateTypeId = 1, ValueTypeName = "System.Double", Format = "eggplant {0:P1}"},
                               new StudentMetric{ StudentUSI = suppliedStudentId1,  SchoolId = suppliedSchoolId,  MetricId = (int)StudentMetricEnum.AttendanceLevelCurrentPeriod, Value = ".96", MetricStateTypeId = 1, ValueTypeName = "System.Double", Format = "what {0:P1}", TrendDirection = -1, TrendInterpretation = 1},
                               new StudentMetric{ StudentUSI = suppliedStudentId1,  SchoolId = suppliedSchoolId,  MetricId = (int)StudentMetricEnum.AttendanceLevelCurrentPeriod, Value = "3", MetricStateTypeId = 1, ValueTypeName = "System.Int32", Format = "dog {0}",},
                                                                                                               
                               new StudentMetric{ StudentUSI = suppliedStudentId2,  SchoolId = suppliedSchoolId},
                               new StudentMetric{ StudentUSI = suppliedStudentId3,  SchoolId = suppliedSchoolId},
                               new StudentMetric{ StudentUSI = suppliedStudentId4,  SchoolId = suppliedSchoolId},
                               new StudentMetric{ StudentUSI = suppliedStudentId5,  SchoolId = suppliedSchoolId},
                               new StudentMetric{ StudentUSI = suppliedStudentId6,  SchoolId = suppliedSchoolId},
                               new StudentMetric{ StudentUSI = suppliedStudentId7,  SchoolId = suppliedSchoolId},
                               new StudentMetric{ StudentUSI = suppliedStudentId8,  SchoolId = suppliedSchoolId},
                               new StudentMetric{ StudentUSI = suppliedStudentId9,  SchoolId = suppliedSchoolId},
                               new StudentMetric{ StudentUSI = suppliedStudentId10, SchoolId = suppliedSchoolId},
                               new StudentMetric{ StudentUSI = suppliedStudentId11, SchoolId = suppliedSchoolId},
                               new StudentMetric{ StudentUSI = suppliedStudentId12, SchoolId = suppliedSchoolId},
                               };
            return data.AsQueryable();
        }

        protected IQueryable<EnhancedStudentInformation> GetSuppliedStudentList()
        {
            var data = new List<EnhancedStudentInformation>
                           {
                               new EnhancedStudentInformation{ StudentUSI = suppliedStudentId1, SchoolId = suppliedSchoolId, FirstName = "First 1", LastSurname = "Last 1", MiddleName = "Middle 1", ProfileThumbnail = "thumbnail",     Gender = "female"},
                               new EnhancedStudentInformation{ StudentUSI = suppliedStudentId2, SchoolId = suppliedSchoolId, FirstName = "First 2", LastSurname = "Last 2", MiddleName = "Middle 2", ProfileThumbnail = "thumbnail",     Gender = "female" },
                               new EnhancedStudentInformation{ StudentUSI = suppliedStudentId3, SchoolId = suppliedSchoolId, FirstName = "First 3", LastSurname = "Last 3", MiddleName = "Middle 3", ProfileThumbnail = "thumbnail",     Gender = "female" },
                               new EnhancedStudentInformation{ StudentUSI = suppliedStudentId4, SchoolId = suppliedSchoolId, FirstName = "First 4", LastSurname = "Last 4", MiddleName = "Middle 4", ProfileThumbnail = "thumbnail",     Gender = "female" },
                               new EnhancedStudentInformation{ StudentUSI = suppliedStudentId5, SchoolId = suppliedSchoolId, FirstName = "First 5", LastSurname = "Last 5", MiddleName = "Middle 5", ProfileThumbnail = "thumbnail",     Gender = "female" },
                               new EnhancedStudentInformation{ StudentUSI = suppliedStudentId6, SchoolId = suppliedSchoolId, FirstName = "First 6", LastSurname = "Last 6", MiddleName = "Middle 6", ProfileThumbnail = "thumbnail",     Gender = "female" },
                               new EnhancedStudentInformation{ StudentUSI = suppliedStudentId7, SchoolId = suppliedSchoolId, FirstName = "First 7", LastSurname = "Last 7", MiddleName = "Middle 7", ProfileThumbnail = "thumbnail",     Gender = "female" },
                               new EnhancedStudentInformation{ StudentUSI = suppliedStudentId8, SchoolId = suppliedSchoolId, FirstName = "First 8", LastSurname = "Last 8", MiddleName = "Middle 8", ProfileThumbnail = "thumbnail",     Gender = "female" },
                               new EnhancedStudentInformation{ StudentUSI = suppliedStudentId9, SchoolId = suppliedSchoolId, FirstName = "First 9", LastSurname = "Last 9", MiddleName = "Middle 9", ProfileThumbnail = "thumbnail",     Gender = "female" },
                               new EnhancedStudentInformation{ StudentUSI = suppliedStudentId10, SchoolId = suppliedSchoolId, FirstName = "First 10", LastSurname = "Last 10", MiddleName = "Middle 10", ProfileThumbnail = "thumbnail", Gender = "female" },
                               new EnhancedStudentInformation{ StudentUSI = suppliedStudentId11, SchoolId = suppliedSchoolId, FirstName = "First 11", LastSurname = "Last 11", MiddleName = "Middle 11", ProfileThumbnail = "thumbnail", Gender = "female" },
                               new EnhancedStudentInformation{ StudentUSI = suppliedStudentId12, SchoolId = suppliedSchoolId, FirstName = "First 12", LastSurname = "Last 12", MiddleName = "Middle 12", ProfileThumbnail = "thumbnail", Gender = "female" },
                               };
            return data.AsQueryable();
        }

        protected IQueryable<TeacherStudentSection> GetSuppliedTeacherStudentSection()
        {
            var data = new List<TeacherStudentSection>
                           {
                               new TeacherStudentSection{ TeacherSectionId = suppliedTeacherSectionId5, StudentUSI = suppliedStudentId9},
                               new TeacherStudentSection{ TeacherSectionId = suppliedTeacherSectionId1, StudentUSI = suppliedStudentId1},
                               new TeacherStudentSection{ TeacherSectionId = suppliedTeacherSectionId1, StudentUSI = suppliedStudentId2},
                               new TeacherStudentSection{ TeacherSectionId = suppliedTeacherSectionId1, StudentUSI = suppliedStudentId3},
                               new TeacherStudentSection{ TeacherSectionId = suppliedTeacherSectionId2, StudentUSI = suppliedStudentId1},
                               new TeacherStudentSection{ TeacherSectionId = suppliedTeacherSectionId2, StudentUSI = suppliedStudentId2},
                               new TeacherStudentSection{ TeacherSectionId = suppliedTeacherSectionId2, StudentUSI = suppliedStudentId3},
                               new TeacherStudentSection{ TeacherSectionId = suppliedTeacherSectionId3, StudentUSI = suppliedStudentId1},
                               new TeacherStudentSection{ TeacherSectionId = suppliedTeacherSectionId3, StudentUSI = suppliedStudentId2},
                               new TeacherStudentSection{ TeacherSectionId = suppliedTeacherSectionId3, StudentUSI = suppliedStudentId3},
                               new TeacherStudentSection{ TeacherSectionId = suppliedTeacherSectionId4, StudentUSI = suppliedStudentId1},
                               new TeacherStudentSection{ TeacherSectionId = suppliedTeacherSectionId4, StudentUSI = suppliedStudentId2},
                               new TeacherStudentSection{ TeacherSectionId = suppliedTeacherSectionId4, StudentUSI = suppliedStudentId3},
                               new TeacherStudentSection{ TeacherSectionId = suppliedTeacherSectionId6, StudentUSI = suppliedStudentId1},
                               new TeacherStudentSection{ TeacherSectionId = suppliedTeacherSectionId6, StudentUSI = suppliedStudentId2},
                               new TeacherStudentSection{ TeacherSectionId = suppliedTeacherSectionId6, StudentUSI = suppliedStudentId3},
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
                               new TeacherSection{ StaffUSI = suppliedStaffUSI, SchoolId = suppliedSchoolId, TeacherSectionId = suppliedTeacherSectionId6, LocalCourseCode = suppliedLocalCourseCode6, SubjectArea = suppliedSubjectAreaWriting, }
                           };
            return data.AsQueryable();
        }

        protected IQueryable<StaffStudentCohort> GetSuppliedStaffStudentCohort()
        {
            var data = new List<StaffStudentCohort>
                           {
                               new StaffStudentCohort{ StaffCohortId = suppliedStaffCohortId3, StudentUSI = suppliedStudentId10},
                               new StaffStudentCohort{ StaffCohortId = suppliedStaffCohortId2, StudentUSI = suppliedStudentId12},
                               new StaffStudentCohort{ StaffCohortId = suppliedStaffCohortId2, StudentUSI = suppliedStudentId1},
                               new StaffStudentCohort{ StaffCohortId = suppliedStaffCohortId2, StudentUSI = suppliedStudentId5},
                               new StaffStudentCohort{ StaffCohortId = suppliedStaffCohortId1, StudentUSI = suppliedStudentId5},
                               new StaffStudentCohort{ StaffCohortId = suppliedStaffCohortId1, StudentUSI = suppliedStudentId6},
                               new StaffStudentCohort{ StaffCohortId = suppliedStaffCohortId1, StudentUSI = suppliedStudentId7},
                               new StaffStudentCohort{ StaffCohortId = suppliedStaffCohortId1, StudentUSI = suppliedStudentId8},
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

        protected IQueryable<MetricComponent> GetSuppliedMetricComponent()
        {
            var data = new List<MetricComponent>
                           {
                               new MetricComponent{ MetricInstanceSetKey = suppliedStudentMetricInstanceSetKey1, MetricId = (int)StudentMetricEnum.AbsenceLevelPreviousPeriodToDate, Name=suppliedLocalCourseCode1, Value="wrong data"},
                               new MetricComponent{ MetricInstanceSetKey = suppliedStudentMetricInstanceSetKey3, MetricId = (int)StudentMetricEnum.AbsenceLevelCurrentPeriod, Name=suppliedLocalCourseCode1, Value="wrong data"},
                               new MetricComponent{ MetricInstanceSetKey = suppliedStudentMetricInstanceSetKey1, MetricId = (int)StudentMetricEnum.AbsenceLevelCurrentPeriod, Name=suppliedLocalCourseCode1, Value="2", ValueTypeName="System.Int32", MetricStateTypeId = 3, Format="rabbit {0}", TrendDirection =(int)suppliedTrendDirection1},
                               new MetricComponent{ MetricInstanceSetKey = suppliedStudentMetricInstanceSetKey1, MetricId = (int)StudentMetricEnum.ClassGradeGradesFalling10PercentOrMore, Name=suppliedLocalCourseCode1, Value="3", ValueTypeName="System.Int32", MetricStateTypeId = 2, Format="ostrich {0}", TrendDirection =(int)suppliedTrendDirection2},
                               new MetricComponent{ MetricInstanceSetKey = suppliedStudentMetricInstanceSetKey2, MetricId = (int)StudentMetricEnum.AbsenceLevelCurrentPeriod, Name=suppliedLocalCourseCode1, Value=null, ValueTypeName=null, MetricStateTypeId =null, Format=null, TrendDirection =null},
                               new MetricComponent{ MetricInstanceSetKey = suppliedStudentMetricInstanceSetKey1, MetricId = (int)StudentMetricEnum.AbsenceLevelCurrentPeriod, Name=suppliedLocalCourseCode2, Value="4", ValueTypeName="System.Int32", MetricStateTypeId = 3, Format="rabbit2 {0}", TrendDirection =(int)suppliedTrendDirection1},
                               new MetricComponent{ MetricInstanceSetKey = suppliedStudentMetricInstanceSetKey1, MetricId = (int)StudentMetricEnum.ClassGradeGradesFalling10PercentOrMore, Name=suppliedLocalCourseCode2, Value="5", ValueTypeName="System.Int32", MetricStateTypeId = 2, Format="ostrich2 {0}", TrendDirection =(int)suppliedTrendDirection2},
                               new MetricComponent{ MetricInstanceSetKey = suppliedStudentMetricInstanceSetKey2, MetricId = (int)StudentMetricEnum.AbsenceLevelCurrentPeriod, Name=suppliedLocalCourseCode2, Value=null, ValueTypeName=null, MetricStateTypeId =null, Format=null, TrendDirection =null},
                               new MetricComponent{ MetricInstanceSetKey = suppliedStudentMetricInstanceSetKey1, MetricId = (int)StudentMetricEnum.AbsenceLevelCurrentPeriod, Name=suppliedLocalCourseCode3, Value="6", ValueTypeName="System.Int32", MetricStateTypeId = 3, Format="rabbit3 {0}", TrendDirection =(int)suppliedTrendDirection1},
                               new MetricComponent{ MetricInstanceSetKey = suppliedStudentMetricInstanceSetKey1, MetricId = (int)StudentMetricEnum.ClassGradeGradesFalling10PercentOrMore, Name=suppliedLocalCourseCode3, Value="7", ValueTypeName="System.Int32", MetricStateTypeId = 2, Format="ostrich3 {0}", TrendDirection =(int)suppliedTrendDirection2},
                               new MetricComponent{ MetricInstanceSetKey = suppliedStudentMetricInstanceSetKey2, MetricId = (int)StudentMetricEnum.AbsenceLevelCurrentPeriod, Name=suppliedLocalCourseCode3, Value=null, ValueTypeName=null, MetricStateTypeId =null, Format=null, TrendDirection =null},
                               new MetricComponent{ MetricInstanceSetKey = suppliedStudentMetricInstanceSetKey1, MetricId = (int)StudentMetricEnum.AbsenceLevelCurrentPeriod, Name=suppliedLocalCourseCode4, Value="8", ValueTypeName="System.Int32", MetricStateTypeId = 3, Format="rabbit4 {0}", TrendDirection =(int)suppliedTrendDirection1},
                               new MetricComponent{ MetricInstanceSetKey = suppliedStudentMetricInstanceSetKey1, MetricId = (int)StudentMetricEnum.ClassGradeGradesFalling10PercentOrMore, Name=suppliedLocalCourseCode4, Value="9", ValueTypeName="System.Int32", MetricStateTypeId = 2, Format="ostrich4 {0}", TrendDirection =(int)suppliedTrendDirection2},
                               new MetricComponent{ MetricInstanceSetKey = suppliedStudentMetricInstanceSetKey2, MetricId = (int)StudentMetricEnum.AbsenceLevelCurrentPeriod, Name=suppliedLocalCourseCode4, Value=null, ValueTypeName=null, MetricStateTypeId =null, Format=null, TrendDirection =null},
                               new MetricComponent{ MetricInstanceSetKey = suppliedStudentMetricInstanceSetKey1, MetricId = (int)StudentMetricEnum.AbsenceLevelCurrentPeriod, Name=suppliedLocalCourseCode6, Value="10", ValueTypeName="System.Int32", MetricStateTypeId = 3, Format="rabbit6 {0}", TrendDirection =(int)suppliedTrendDirection1},
                               new MetricComponent{ MetricInstanceSetKey = suppliedStudentMetricInstanceSetKey1, MetricId = (int)StudentMetricEnum.ClassGradeGradesFalling10PercentOrMore, Name=suppliedLocalCourseCode6, Value="11", ValueTypeName="System.Int32", MetricStateTypeId = 2, Format="ostrich6 {0}", TrendDirection =(int)suppliedTrendDirection2},
                               new MetricComponent{ MetricInstanceSetKey = suppliedStudentMetricInstanceSetKey2, MetricId = (int)StudentMetricEnum.AbsenceLevelCurrentPeriod, Name=suppliedLocalCourseCode6, Value=null, ValueTypeName=null, MetricStateTypeId =null, Format=null, TrendDirection =null},
                           };

            return data.AsQueryable();
        }

        protected IQueryable<StudentSchoolMetricInstanceSet> GetSuppliedStudentSchoolMetricInstanceSet()
        {
            var data = new List<StudentSchoolMetricInstanceSet>
                           {
                               new StudentSchoolMetricInstanceSet{ MetricInstanceSetKey = suppliedStudentMetricInstanceSetKey3, StudentUSI = suppliedStudentId1, SchoolId = suppliedSchoolId+1},
                               new StudentSchoolMetricInstanceSet{ MetricInstanceSetKey = suppliedStudentMetricInstanceSetKey1, StudentUSI = suppliedStudentId1, SchoolId = suppliedSchoolId},
                               new StudentSchoolMetricInstanceSet{ MetricInstanceSetKey = suppliedStudentMetricInstanceSetKey2, StudentUSI = suppliedStudentId2, SchoolId = suppliedSchoolId},
                           };

            return data.AsQueryable();
        }

        protected IQueryable<StudentRecordCurrentCourse> GetSuppliedStudentRecordCurrentCourse()
        {
            var data = new List<StudentRecordCurrentCourse>
                           {
                               new StudentRecordCurrentCourse{ StudentUSI = suppliedStudentId1 + 1, SchoolId = suppliedSchoolId, LocalCourseCode = suppliedLocalCourseCode1, SubjectArea = reading, GradeLevel = gradeLevel1, CourseTitle = "wrong data", GradingPeriod = 7},
                               new StudentRecordCurrentCourse{ StudentUSI = suppliedStudentId1, SchoolId = suppliedSchoolId + 1, LocalCourseCode = suppliedLocalCourseCode1, SubjectArea = reading, GradeLevel = gradeLevel1, CourseTitle = "wrong data", GradingPeriod = 7},
                               new StudentRecordCurrentCourse{ StudentUSI = suppliedStudentId1, SchoolId = suppliedSchoolId, LocalCourseCode = suppliedLocalCourseCode1 + "1", SubjectArea = reading, GradeLevel = gradeLevel1, CourseTitle = "wrong data", GradingPeriod = 7},
                               new StudentRecordCurrentCourse{ StudentUSI = suppliedStudentId1, SchoolId = suppliedSchoolId, LocalCourseCode = suppliedLocalCourseCode1, SubjectArea = reading, GradeLevel = gradeLevel1, CourseTitle = "wrong data", GradingPeriod = 1},
                               new StudentRecordCurrentCourse{ StudentUSI = suppliedStudentId1, SchoolId = suppliedSchoolId, LocalCourseCode = suppliedLocalCourseCode1, SubjectArea = reading, GradeLevel = gradeLevel1, GradingPeriod = 4, NumericGradeEarned = suppliedNumericGrade1, TrendDirection = 1},
                               new StudentRecordCurrentCourse{ StudentUSI = suppliedStudentId2, SchoolId = suppliedSchoolId, LocalCourseCode = suppliedLocalCourseCode1, SubjectArea = reading, GradeLevel = gradeLevel1, CourseTitle = "wrong data", GradingPeriod = 1},
                               new StudentRecordCurrentCourse{ StudentUSI = suppliedStudentId2, SchoolId = suppliedSchoolId, LocalCourseCode = suppliedLocalCourseCode1, SubjectArea = reading, GradeLevel = gradeLevel1, GradingPeriod = 4, LetterGradeEarned = suppliedLetterGrade1, TrendDirection = -1},
                               new StudentRecordCurrentCourse{ StudentUSI = suppliedStudentId1, SchoolId = suppliedSchoolId, LocalCourseCode = suppliedLocalCourseCode2, SubjectArea = math, GradeLevel = gradeLevel1, GradingPeriod = 4, NumericGradeEarned = suppliedNumericGrade2, TrendDirection = 1},
                               new StudentRecordCurrentCourse{ StudentUSI = suppliedStudentId2, SchoolId = suppliedSchoolId, LocalCourseCode = suppliedLocalCourseCode2, SubjectArea = math, GradeLevel = gradeLevel1, GradingPeriod = 4, LetterGradeEarned = suppliedLetterGrade2, TrendDirection = -1},
                               new StudentRecordCurrentCourse{ StudentUSI = suppliedStudentId1, SchoolId = suppliedSchoolId, LocalCourseCode = suppliedLocalCourseCode3, SubjectArea = science, GradeLevel = gradeLevel1, GradingPeriod = 4, NumericGradeEarned = suppliedNumericGrade3, TrendDirection = 1},
                               new StudentRecordCurrentCourse{ StudentUSI = suppliedStudentId2, SchoolId = suppliedSchoolId, LocalCourseCode = suppliedLocalCourseCode3, SubjectArea = science, GradeLevel = gradeLevel1, GradingPeriod = 4, LetterGradeEarned = suppliedLetterGrade3, TrendDirection = -1},
                               new StudentRecordCurrentCourse{ StudentUSI = suppliedStudentId1, SchoolId = suppliedSchoolId, LocalCourseCode = suppliedLocalCourseCode4, SubjectArea = socialStudies, GradeLevel = gradeLevel1, GradingPeriod = 4, NumericGradeEarned = suppliedNumericGrade4, TrendDirection = 1},
                               new StudentRecordCurrentCourse{ StudentUSI = suppliedStudentId2, SchoolId = suppliedSchoolId, LocalCourseCode = suppliedLocalCourseCode4, SubjectArea = socialStudies, GradeLevel = gradeLevel1, GradingPeriod = 4, LetterGradeEarned = suppliedLetterGrade4, TrendDirection = -1},
                               new StudentRecordCurrentCourse{ StudentUSI = suppliedStudentId1, SchoolId = suppliedSchoolId, LocalCourseCode = suppliedLocalCourseCode6, SubjectArea = writing, GradeLevel = gradeLevel1, GradingPeriod = 4, NumericGradeEarned = suppliedNumericGrade6, TrendDirection = 1},
                               new StudentRecordCurrentCourse{ StudentUSI = suppliedStudentId2, SchoolId = suppliedSchoolId, LocalCourseCode = suppliedLocalCourseCode6, SubjectArea = writing, GradeLevel = gradeLevel1, GradingPeriod = 4, LetterGradeEarned = suppliedLetterGrade6, TrendDirection = -1},
                           };

            return data.AsQueryable();
        }

        protected IQueryable<Dashboards.Metric.Data.Entities.Metric> GetMetric()
        {
            var data = new List<Dashboards.Metric.Data.Entities.Metric>
                           {
                               new Dashboards.Metric.Data.Entities.Metric{ MetricId = (int)StudentMetricEnum.AbsenceLevelCurrentPeriod, TrendInterpretation = (int)suppliedTrendInterpretation1 },
                               new Dashboards.Metric.Data.Entities.Metric{ MetricId = (int)StudentMetricEnum.ClassGradeGradesFalling10PercentOrMore, TrendInterpretation = (int)suppliedTrendInterpretation2 },
                           };

            return data.AsQueryable();
        }

        protected MetricMetadataNode GetStudentRootOverviewNode()
        {
            var tree = new TestMetricMetadataTree();
            return new MetricMetadataNode(tree)
            {
                MetricId = 2,
                Name = "Student's Overview",
                MetricNodeId = suppliedMetricNodeId,
                Children = new List<MetricMetadataNode>
                            {
                                new MetricMetadataNode(tree){MetricId=21, MetricNodeId = 71, Name = "Student's Attendance and Discipline",
                                Children = new List<MetricMetadataNode>
                                                {
                                                    new MetricMetadataNode(tree){MetricId=211, MetricNodeId = 711, Name = "Attendance"},
                                                    new MetricMetadataNode(tree){MetricId=212, Name = "Discipline"} 
                                                }
                                },
                                new MetricMetadataNode(tree){MetricId=22, MetricNodeId = 72, Name = "School's Other Metric"},
                            }
            };
        }

        protected override void ExecuteTest()
        {
            var service = new SubjectSpecificOverviewService
                              {
                                  StudentListWithMetricsRepository = studentListWithMetricsRepository,
                                  TeacherStudentSectionRepository = teacherStudentSectionRepository,
                                  TeacherSectionRepository = teacherSectionRepository,
                                  StaffStudentCohortRepository = staffStudentCohortRepository,
                                  StaffCohortRepository = staffCohortRepository,
                                  MetricComponentRepository = metricComponentRepository,
                                  StudentSchoolMetricInstanceSetRepository = studentSchoolMetricInstanceSetRepository,
                                  StudentRecordCurrentCourseRepository = studentRecordCurrentCourseRepository,
                                  MetricRepository = metricRepository,
                                  AccommodationProvider = accommodationProvider,
                                  UniqueListProvider = uniqueListProvider,
                                  TrendRenderingDispositionProvider = trendRenderingDispositionProvider,
                                  GradeStateProvider = gradeStateProvider,
                                  SchoolCategoryProvider = schoolCategoryProvider,
                                  RootMetricNodeResolver = rootMetricNodeResolver,
                                  MetricStateProvider = metricStateProvider,
                                  StudentSchoolAreaLinks = studentSchoolAreaLinks,
                                  ClassroomMetricsProvider = classroomMetricsProvider,
                                  StudentListUtilitiesProvider = studentListUtilitiesProvider,
                                  ListMetadataProvider = listMetadataProvider,
                                  MetadataListIdResolver = metadataListIdResolver,
                                  SubjectSpecificOverviewMetricComponentProvider = subjectSpecificOverviewMetricComponentProvider,
                                  StudentListWithMetricsProvider = studentListWithMetricsProvider,
                                  GradeLevelUtilitiesProvider = gradeLevelUtilitiesProvider
                              };
            actualModel = service.Get(SubjectSpecificOverviewRequest.Create(suppliedSchoolId, suppliedStaffUSI, suppliedStudentListType, suppliedSectionId));
        }

        [Test]
        public void Should_load_correct_students()
        {
            Assert.That(actualModel.Students.Count, Is.EqualTo(expectedStudentIds.Count));
            foreach (var student in actualModel.Students)
            {
                Assert.That(expectedStudentIds.Contains(student.StudentUSI), Is.True, "Student USI " + student.StudentUSI + " was missing.");
                Assert.That(student.Metrics.Count, Is.EqualTo(expectedMetricCount), "Student USI " + student.StudentUSI);
            }
        }

        [Test]
        public void Should_have_expected_subject_area()
        {
            Assert.That(actualModel.SubjectArea, Is.EqualTo(expectedSubjectArea));
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
        public void Should_have_no_unassigned_values_on_presentation_model()
        {
            actualModel.EnsureNoDefaultValues("SubjectSpecificOverviewModel.Students[0].IsFlagged",
                                              "SubjectSpecificOverviewModel.Students[1].IsFlagged",
                                              "SubjectSpecificOverviewModel.Students[2].IsFlagged",
                                              "SubjectSpecificOverviewModel.Students[0].SchoolName",
                                              "SubjectSpecificOverviewModel.Students[1].SchoolName",
                                              "SubjectSpecificOverviewModel.Students[2].SchoolName",
                                              "SubjectSpecificOverviewModel.Students[0].Links",
                                              "SubjectSpecificOverviewModel.Students[0].Accommodations");
        }

        [Test]
        public virtual void Should_have_serializable_model()
        {
            actualModel.EnsureSerializableModel();
        }
    }

    public class When_loading_reading_subject_specific_overview_for_secondary_teacher_section : SubjectSpecificOverviewServiceFixtureBase
    {
        protected override void EstablishContext()
        {
            suppliedStudentListType = StudentListType.Section.ToString();
            suppliedSectionId = suppliedTeacherSectionId1;

            expectedStudentIds = new List<long> { suppliedStudentId1, suppliedStudentId2, suppliedStudentId3 };

            expectedSubjectArea = reading;
            expectedSchoolCategory = SchoolCategory.HighSchool;
            expectedMetricCount = 3;

            base.EstablishContext();

            Expect.Call(schoolCategoryProvider.GetSchoolCategoryType(suppliedSchoolId)).Return(SchoolCategory.HighSchool);
            Expect.Call(accommodationProvider.GetAccommodations(expectedStudentIds.ToArray(), suppliedSchoolId)).Return(GetSuppliedAccommodations());
        }

        protected List<Accommodation> GetSuppliedAccommodations()
        {
            var data = new List<Accommodation>
                           {
                               new Accommodation(suppliedStudentId1) { AccommodationsList = new List<Accommodations>{ Accommodations.Overage, Accommodations.Repeater}},
                               new Accommodation(suppliedStudentId3) { AccommodationsList = new List<Accommodations>{ Accommodations.ESLAndLEP}}
                           };
            return data;
        }

        [Test]
        public void Should_load_accommodations_correctly()
        {
            Assert.That(actualModel.Students[0].Accommodations.Count, Is.EqualTo(2));
            Assert.That(actualModel.Students[0].Accommodations[0], Is.EqualTo(Accommodations.Overage));
            Assert.That(actualModel.Students[0].Accommodations[1], Is.EqualTo(Accommodations.Repeater));
            Assert.That(actualModel.Students[1].Accommodations.Count, Is.EqualTo(0));
            Assert.That(actualModel.Students[2].Accommodations.Count, Is.EqualTo(1));
            Assert.That(actualModel.Students[2].Accommodations[0], Is.EqualTo(Accommodations.ESLAndLEP));
        }

        [Test]
        public void Should_build_correct_number_of_metrics_per_student()
        {

            Assert.That(actualModel.Students.Count,Is.EqualTo(3));

            foreach (var student in actualModel.Students)
                Assert.That(student.Metrics.Count, Is.EqualTo(expectedMetricCount));
        }
    }
    
    public class When_loading_math_subject_specific_overview_for_secondary_teacher_section : SubjectSpecificOverviewServiceFixtureBase
    {
        protected override void EstablishContext()
        {
            suppliedStudentListType = StudentListType.Section.ToString();
            suppliedSectionId = suppliedTeacherSectionId2;

            expectedStudentIds = new List<long> { suppliedStudentId1, suppliedStudentId2, suppliedStudentId3 };

            expectedSubjectArea = math;
            expectedSchoolCategory = SchoolCategory.HighSchool;
            expectedMetricCount = 3;

            base.EstablishContext();

            Expect.Call(schoolCategoryProvider.GetSchoolCategoryType(suppliedSchoolId)).Return(SchoolCategory.HighSchool);
            Expect.Call(accommodationProvider.GetAccommodations(expectedStudentIds.ToArray(), suppliedSchoolId)).Return(GetSuppliedAccommodations());
        }

        protected List<Accommodation> GetSuppliedAccommodations()
        {
            var data = new List<Accommodation> { };
            return data;
        }

        [Test]
        public void Should_build_correct_number_of_metrics_per_student()
        {

            Assert.That(actualModel.Students.Count, Is.EqualTo(3));

            foreach (var student in actualModel.Students)
                Assert.That(student.Metrics.Count, Is.EqualTo(expectedMetricCount));
        }

    }
    
    public class When_loading_science_subject_specific_overview_for_secondary_teacher_section : SubjectSpecificOverviewServiceFixtureBase
    {
        protected override void EstablishContext()
        {
            suppliedStudentListType = StudentListType.Section.ToString();
            suppliedSectionId = suppliedTeacherSectionId3;

            expectedStudentIds = new List<long> { suppliedStudentId1, suppliedStudentId2, suppliedStudentId3 };

            expectedSubjectArea = science;
            expectedSchoolCategory = SchoolCategory.HighSchool;
            expectedMetricCount = 3;

            base.EstablishContext();

            Expect.Call(schoolCategoryProvider.GetSchoolCategoryType(suppliedSchoolId)).Return(SchoolCategory.HighSchool);
            Expect.Call(accommodationProvider.GetAccommodations(expectedStudentIds.ToArray(), suppliedSchoolId)).Return(GetSuppliedAccommodations());
        }

        protected List<Accommodation> GetSuppliedAccommodations()
        {
            var data = new List<Accommodation> { };
            return data;
        }

        [Test]
        public void Should_build_correct_number_of_metrics_per_student()
        {
            Assert.That(actualModel.Students.Count,Is.EqualTo(3));

            foreach (var student in actualModel.Students)
                Assert.That(student.Metrics.Count, Is.EqualTo(expectedMetricCount));
        }
    }
    
    public class When_loading_social_studies_subject_specific_overview_for_secondary_teacher_section : SubjectSpecificOverviewServiceFixtureBase
    {
        protected override void EstablishContext()
        {
            suppliedStudentListType = StudentListType.Section.ToString();
            suppliedSectionId = suppliedTeacherSectionId4;

            expectedStudentIds = new List<long> { suppliedStudentId1, suppliedStudentId2, suppliedStudentId3 };

            expectedSubjectArea = socialStudies;
            expectedSchoolCategory = SchoolCategory.HighSchool;
            expectedMetricCount = 3;

            base.EstablishContext();

            Expect.Call(schoolCategoryProvider.GetSchoolCategoryType(suppliedSchoolId)).Return(SchoolCategory.HighSchool);
            Expect.Call(accommodationProvider.GetAccommodations(expectedStudentIds.ToArray(), suppliedSchoolId)).Return(GetSuppliedAccommodations());
        }

        protected List<Accommodation> GetSuppliedAccommodations()
        {
            var data = new List<Accommodation> { };
            return data;
        }

        [Test]
        public void Should_build_correct_number_of_metrics_per_student()
        {
            Assert.That(actualModel.Students.Count, Is.EqualTo(3));

            foreach (var student in actualModel.Students)
                Assert.That(student.Metrics.Count, Is.EqualTo(expectedMetricCount));
        }
    }
    
    public class When_loading_writing_subject_specific_overview_for_secondary_teacher_section : SubjectSpecificOverviewServiceFixtureBase
    {
        protected override void EstablishContext()
        {
            suppliedStudentListType = StudentListType.Section.ToString();
            suppliedSectionId = suppliedTeacherSectionId6;

            expectedStudentIds.Add(suppliedStudentId1);
            expectedStudentIds.Add(suppliedStudentId2);
            expectedStudentIds.Add(suppliedStudentId3);

            expectedSubjectArea = writing;
            expectedSchoolCategory = SchoolCategory.HighSchool;
            expectedMetricCount = 3;

            base.EstablishContext();

            Expect.Call(schoolCategoryProvider.GetSchoolCategoryType(suppliedSchoolId)).Return(SchoolCategory.HighSchool);
            Expect.Call(accommodationProvider.GetAccommodations(expectedStudentIds.ToArray(), suppliedSchoolId)).Return(GetSuppliedAccommodations());
        }

        protected List<Accommodation> GetSuppliedAccommodations()
        {
            var data = new List<Accommodation> { };
            return data;
        }

        [Test]
        public void Should_build_correct_number_of_metrics_per_student()
        {
            Assert.That(actualModel.Students.Count,Is.EqualTo(3));

            foreach (var student in actualModel.Students)
                Assert.That(student.Metrics.Count, Is.EqualTo(expectedMetricCount));
        }
    }

    public class When_loading_reading_subject_specific_overview_for_elementary_teacher_section : SubjectSpecificOverviewServiceFixtureBase
    {
        protected override void EstablishContext()
        {
            suppliedStudentListType = StudentListType.Section.ToString();
            suppliedSectionId = suppliedTeacherSectionId1;

            expectedStudentIds = new List<long>{suppliedStudentId1, suppliedStudentId2, suppliedStudentId3};

            expectedSubjectArea = reading;
            expectedSchoolCategory = SchoolCategory.Elementary;
            expectedMetricCount = 1;

            base.EstablishContext();

            Expect.Call(schoolCategoryProvider.GetSchoolCategoryType(suppliedSchoolId)).Return(SchoolCategory.Elementary);
            Expect.Call(accommodationProvider.GetAccommodations(expectedStudentIds.ToArray(), suppliedSchoolId)).Return(GetSuppliedAccommodations());
        }

        protected List<Accommodation> GetSuppliedAccommodations()
        {
            var data = new List<Accommodation>();
            return data;
        }
        
        [Test]
        public void Should_build_correct_number_of_metrics_per_student()
        {
            Assert.That(actualModel.Students.Count,Is.EqualTo(3));

            foreach (var student in actualModel.Students)
                Assert.That(student.Metrics.Count, Is.EqualTo(expectedMetricCount));
        }
    }

    public class When_loading_math_subject_specific_overview_for_elementary_teacher_section : SubjectSpecificOverviewServiceFixtureBase
    {
        protected override void EstablishContext()
        {
            suppliedStudentListType = StudentListType.Section.ToString();
            suppliedSectionId = suppliedTeacherSectionId2;

            expectedStudentIds = new List<long> { suppliedStudentId1, suppliedStudentId2, suppliedStudentId3 };

            expectedSubjectArea = math;
            expectedSchoolCategory = SchoolCategory.Elementary;
            expectedMetricCount = 1;

            base.EstablishContext();

            Expect.Call(schoolCategoryProvider.GetSchoolCategoryType(suppliedSchoolId)).Return(SchoolCategory.Elementary);
            Expect.Call(accommodationProvider.GetAccommodations(expectedStudentIds.ToArray(), suppliedSchoolId)).Return(GetSuppliedAccommodations());
        }

        protected List<Accommodation> GetSuppliedAccommodations()
        {
            var data = new List<Accommodation> { };
            return data;
        }

        [Test]
        public void Should_build_correct_number_of_metrics_per_student()
        {
            Assert.That(actualModel.Students.Count,Is.EqualTo(3));

            foreach (var student in actualModel.Students)
                Assert.That(student.Metrics.Count, Is.EqualTo(expectedMetricCount));
        }
    }

    public class When_loading_science_subject_specific_overview_for_elementary_teacher_section : SubjectSpecificOverviewServiceFixtureBase
    {
        protected override void EstablishContext()
        {
            suppliedStudentListType = StudentListType.Section.ToString();
            suppliedSectionId = suppliedTeacherSectionId3;

            expectedStudentIds = new List<long> { suppliedStudentId1, suppliedStudentId2, suppliedStudentId3 };

            expectedSubjectArea = science;
            expectedSchoolCategory = SchoolCategory.Elementary;
            expectedMetricCount = 1;

            base.EstablishContext();

            Expect.Call(schoolCategoryProvider.GetSchoolCategoryType(suppliedSchoolId)).Return(SchoolCategory.Elementary);
            Expect.Call(accommodationProvider.GetAccommodations(expectedStudentIds.ToArray(), suppliedSchoolId)).Return(GetSuppliedAccommodations());
        }

        protected List<Accommodation> GetSuppliedAccommodations()
        {
            var data = new List<Accommodation> { };
            return data;
        }

        [Test]
        public void Should_build_correct_number_of_metrics_per_student()
        {
            Assert.That(actualModel.Students.Count,Is.EqualTo(3));

            foreach (var student in actualModel.Students)
                Assert.That(student.Metrics.Count, Is.EqualTo(expectedMetricCount));
        }
    }

    public class When_loading_social_studies_subject_specific_overview_for_elementary_teacher_section : SubjectSpecificOverviewServiceFixtureBase
    {
        protected override void EstablishContext()
        {
            suppliedStudentListType = StudentListType.Section.ToString();
            suppliedSectionId = suppliedTeacherSectionId4;

            expectedStudentIds = new List<long> { suppliedStudentId1, suppliedStudentId2, suppliedStudentId3 };

            expectedSubjectArea = socialStudies;
            expectedSchoolCategory = SchoolCategory.Elementary;
            expectedMetricCount = 1;

            base.EstablishContext();

            Expect.Call(schoolCategoryProvider.GetSchoolCategoryType(suppliedSchoolId)).Return(SchoolCategory.Elementary);
            Expect.Call(accommodationProvider.GetAccommodations(expectedStudentIds.ToArray(), suppliedSchoolId)).Return(GetSuppliedAccommodations());
        }

        protected List<Accommodation> GetSuppliedAccommodations()
        {
            var data = new List<Accommodation> { };
            return data;
        }

        [Test]
        public void Should_build_correct_number_of_metrics_per_student()
        {
            Assert.That(actualModel.Students.Count,Is.EqualTo(3));

            foreach (var student in actualModel.Students)
                Assert.That(student.Metrics.Count, Is.EqualTo(expectedMetricCount));
        }
    }

    public class When_loading_writing_subject_specific_overview_for_elementary_teacher_section : SubjectSpecificOverviewServiceFixtureBase
    {
        protected override void EstablishContext()
        {
            suppliedStudentListType = StudentListType.Section.ToString();
            suppliedSectionId = suppliedTeacherSectionId6;

            expectedStudentIds = new List<long> { suppliedStudentId1, suppliedStudentId2, suppliedStudentId3 };

            expectedSubjectArea = writing;
            expectedSchoolCategory = SchoolCategory.Elementary;
            expectedMetricCount = 1;

            base.EstablishContext();

            Expect.Call(schoolCategoryProvider.GetSchoolCategoryType(suppliedSchoolId)).Return(SchoolCategory.Elementary);
            Expect.Call(accommodationProvider.GetAccommodations(expectedStudentIds.ToArray(), suppliedSchoolId)).Return(GetSuppliedAccommodations());
        }

        protected List<Accommodation> GetSuppliedAccommodations()
        {
            var data = new List<Accommodation> { };
            return data;
        }

        [Test]
        public void Should_build_correct_number_of_metrics_per_student()
        {
            Assert.That(actualModel.Students.Count,Is.EqualTo(3));

            foreach (var student in actualModel.Students)
                Assert.That(student.Metrics.Count, Is.EqualTo(expectedMetricCount));
        }
    }

    public class When_loading_science_specific_overview_for_staff_cohort : TestFixtureBase
    {
        protected IRepository<StudentMetric> studentListWithMetricsRepository;
        protected IRepository<TeacherStudentSection> teacherStudentSectionRepository;
        protected IRepository<TeacherSection> teacherSectionRepository;
        protected IRepository<StaffStudentCohort> staffStudentCohortRepository;
        protected IRepository<StaffCohort> staffCohortRepository;
        protected IRepository<MetricComponent> metricComponentRepository;
        protected IRepository<StudentSchoolMetricInstanceSet> studentSchoolMetricInstanceSetRepository;
        protected IRepository<StudentRecordCurrentCourse> studentRecordCurrentCourseRepository;
        protected IRepository<Dashboards.Metric.Data.Entities.Metric> metricRepository;
        protected ISchoolCategoryProvider schoolCategoryProvider;
        protected IAccommodationProvider accommodationProvider;
        protected IUniqueListIdProvider uniqueListProvider;
        protected ITrendRenderingDispositionProvider trendRenderingDispositionProvider;
        protected IGradeStateProvider gradeStateProvider;
        protected IRootMetricNodeResolver rootMetricNodeResolver;
        protected IMetricStateProvider metricStateProvider;
        protected IStudentSchoolAreaLinks studentSchoolAreaLinks = new StudentSchoolAreaLinksFake();

        protected const int suppliedStaffUSI = 1000;
        protected const int suppliedSchoolId = 2000;
        protected const int suppliedLocalEducationAgencyId = 3000;
        protected int suppliedSectionId = 4000;

        protected SubjectSpecificOverviewModel actualModel;

        protected override void ExecuteTest()
        {
            var service = new SubjectSpecificOverviewService
            {
                StudentListWithMetricsRepository = studentListWithMetricsRepository,
                TeacherStudentSectionRepository = teacherStudentSectionRepository,
                TeacherSectionRepository = teacherSectionRepository,
                StaffStudentCohortRepository = staffStudentCohortRepository,
                StaffCohortRepository = staffCohortRepository,
                MetricComponentRepository = metricComponentRepository,
                StudentSchoolMetricInstanceSetRepository = studentSchoolMetricInstanceSetRepository,
                StudentRecordCurrentCourseRepository = studentRecordCurrentCourseRepository,
                MetricRepository = metricRepository,
                AccommodationProvider = accommodationProvider,
                UniqueListProvider = uniqueListProvider,
                TrendRenderingDispositionProvider = trendRenderingDispositionProvider,
                GradeStateProvider = gradeStateProvider,
                SchoolCategoryProvider = schoolCategoryProvider,
                RootMetricNodeResolver = rootMetricNodeResolver,
                MetricStateProvider = metricStateProvider,
                StudentSchoolAreaLinks = studentSchoolAreaLinks
            };

            actualModel = service.Get(SubjectSpecificOverviewRequest.Create(suppliedSchoolId, suppliedStaffUSI, StudentListType.Cohort.ToString(), suppliedSectionId));
        }

        [Test]
        public void Should_not_load_any_data()
        {
            Assert.That(actualModel, Is.Not.Null);
            Assert.That(actualModel.Students.Count, Is.EqualTo(0));
            Assert.That(actualModel.SubjectArea, Is.EqualTo(null));
            Assert.That(actualModel.SchoolCategory, Is.EqualTo(SchoolCategory.None));
            Assert.That(actualModel.UniqueListId, Is.EqualTo(null));
        }
    }

    public class When_loading_specific_overview_for_all_students : TestFixtureBase
    {
        protected const int suppliedTeacherSectionId1 = 100;
        protected const int suppliedStudentId1 = 160;

        protected IRepository<StudentMetric> studentListWithMetricsRepository;
        protected IRepository<TeacherStudentSection> teacherStudentSectionRepository;
        protected IRepository<TeacherSection> teacherSectionRepository;
        protected IRepository<StaffStudentCohort> staffStudentCohortRepository;
        protected IRepository<StaffCohort> staffCohortRepository;
        protected IRepository<MetricComponent> metricComponentRepository;
        protected IRepository<StudentSchoolMetricInstanceSet> studentSchoolMetricInstanceSetRepository;
        protected IRepository<StudentRecordCurrentCourse> studentRecordCurrentCourseRepository;
        protected IRepository<Dashboards.Metric.Data.Entities.Metric> metricRepository;
        protected ISchoolCategoryProvider schoolCategoryProvider;
        protected IAccommodationProvider accommodationProvider;
        protected IUniqueListIdProvider uniqueListProvider;
        protected ITrendRenderingDispositionProvider trendRenderingDispositionProvider;
        protected IGradeStateProvider gradeStateProvider;
        protected IRootMetricNodeResolver rootMetricNodeResolver;
        protected IMetricStateProvider metricStateProvider;
        protected IStudentSchoolAreaLinks studentSchoolAreaLinks = new StudentSchoolAreaLinksFake();

        protected const int suppliedStaffUSI = 1000;
        protected const int suppliedSchoolId = 2000;
        protected const int suppliedLocalEducationAgencyId = 3000;

        protected SubjectSpecificOverviewModel actualModel;

        protected override void EstablishContext()
        {
            base.EstablishContext();

            teacherStudentSectionRepository = mocks.StrictMock<IRepository<TeacherStudentSection>>();
            teacherSectionRepository = mocks.StrictMock<IRepository<TeacherSection>>();
            Expect.Call(teacherStudentSectionRepository.GetAll()).Repeat.Any().Return(GetSuppliedTeacherStudentSection());
            Expect.Call(teacherSectionRepository.GetAll()).Repeat.Any().Return(GetSuppliedTeacherSection());
        }

        protected override void ExecuteTest()
        {
            var service = new SubjectSpecificOverviewService
            {
                StudentListWithMetricsRepository = studentListWithMetricsRepository,
                TeacherStudentSectionRepository = teacherStudentSectionRepository,
                TeacherSectionRepository = teacherSectionRepository,
                StaffStudentCohortRepository = staffStudentCohortRepository,
                StaffCohortRepository = staffCohortRepository,
                MetricComponentRepository = metricComponentRepository,
                StudentSchoolMetricInstanceSetRepository = studentSchoolMetricInstanceSetRepository,
                StudentRecordCurrentCourseRepository = studentRecordCurrentCourseRepository,
                MetricRepository = metricRepository,
                AccommodationProvider = accommodationProvider,
                UniqueListProvider = uniqueListProvider,
                TrendRenderingDispositionProvider = trendRenderingDispositionProvider,
                GradeStateProvider = gradeStateProvider,
                SchoolCategoryProvider = schoolCategoryProvider,
                RootMetricNodeResolver = rootMetricNodeResolver,
                MetricStateProvider = metricStateProvider,
                StudentSchoolAreaLinks = studentSchoolAreaLinks
            };

            actualModel = service.Get(SubjectSpecificOverviewRequest.Create(suppliedSchoolId, suppliedStaffUSI, StudentListType.All.ToString(), 0));
        }

        protected IQueryable<TeacherStudentSection> GetSuppliedTeacherStudentSection()
        {
            var data = new List<TeacherStudentSection>
                           {
                               new TeacherStudentSection{ TeacherSectionId = suppliedTeacherSectionId1, StudentUSI = suppliedStudentId1},
                           };
            return data.AsQueryable();
        }

        protected IQueryable<TeacherSection> GetSuppliedTeacherSection()
        {
            var data = new List<TeacherSection>
                           {
                               new TeacherSection{ StaffUSI = suppliedStaffUSI, SchoolId = suppliedSchoolId, TeacherSectionId = suppliedTeacherSectionId1,  },
                           };
            return data.AsQueryable();
        }
        
        [Test]
        public void Should_not_load_any_data()
        {
            Assert.That(actualModel, Is.Not.Null);
            Assert.That(actualModel.Students.Count, Is.EqualTo(0));
            Assert.That(actualModel.SubjectArea, Is.EqualTo(null));
            Assert.That(actualModel.SchoolCategory, Is.EqualTo(SchoolCategory.None));
            Assert.That(actualModel.UniqueListId, Is.EqualTo(null));
        }
    }
    
}

