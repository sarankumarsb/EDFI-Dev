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
using EdFi.Dashboards.Resources;
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
using EdFi.Dashboards.Resources.Tests;
using EdFi.Dashboards.Resources.Tests.Navigation.Fakes;
using EdFi.Dashboards.Testing;
using EdFi.Dashboards.Warehouse.Data.Entities;
using EdFi.Dashboards.Warehouse.Resource.Models.Staff;
using EdFi.Dashboards.Warehouse.Resources.Application;
using EdFi.Dashboards.Warehouse.Resources.School.Detail;
using EdFi.Dashboards.Warehouse.Resources.Staff;
using NUnit.Framework;
using Rhino.Mocks;

namespace EdFi.Dashboards.Warehouse.Resources.Tests.Staff
{
    public abstract class PriorYearServiceFixtureBase : TestFixtureBase
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
        protected const int suppliedStudentId13 = 280;
        protected const int suppliedStudentId14 = 290;
        protected const int suppliedStudentId15 = 300;
        protected const int suppliedStudentId16 = 310;
        protected const int suppliedStudentId17 = 320;
        protected const int suppliedStudentId18 = 330;
        protected const int suppliedStaffUSI = 1000;
        protected const int suppliedSchoolId = 2000;
        protected const int suppliedLocalEducationAgencyId = 3000;
        protected string suppliedStudentListType;
        protected int suppliedSectionId;
        protected const int suppliedUniqueIdentifier = 500;
        protected const string suppliedUniqueListId = "baboon";
        protected const string suppliedImageName = "test.jpg";
        protected const string reading = "ELA / Reading";
        protected const string math = "Mathematics";
        protected const string science = "Science";
        protected const string socialStudies = "Social Studies";
        protected const string writing = "Writing";
        protected const string gradeLevel1 = "grade level 1";
        protected const int suppliedMetricNodeId = 7;
        protected const string suppliedFourByFourText = "four by four state text";
        protected const string suppliedFourByFourDisplayText = "four by four display state text";

        protected IRepository<StudentMetric> studentListWithMetricsRepository;
        protected IRepository<TeacherStudentSection> teacherStudentSectionRepository;
        protected IRepository<TeacherSection> teacherSectionRepository;
        protected IRepository<StaffStudentCohort> staffStudentCohortRepository;
        protected IRepository<StaffCohort> staffCohortRepository;
        protected IRepository<StaffCustomStudentList> staffCustomStudentListRepository;
        protected IRepository<StaffCustomStudentListStudent> staffCustomStudentListStudentRepository;
        protected ISchoolCategoryProvider schoolCategoryProvider;
        protected IAccommodationProvider accommodationProvider;
        protected IUniqueListIdProvider uniqueListProvider;
        protected ITrendRenderingDispositionProvider trendRenderingDispositionProvider;
        protected IRootMetricNodeResolver rootMetricNodeResolver;
        protected IMetricStateProvider metricStateProvider;
        protected IStudentSchoolAreaLinks studentSchoolAreaLinks = new StudentSchoolAreaLinksFake();
        protected IPriorYearClassroomMetricsProvider priorYearClassroomMetricsProvider;
        protected IListMetadataProvider listMetadataProvider;
        protected IMetadataListIdResolver metadataListIdResolver;
        protected IWarehouseAvailabilityProvider warehouseAvailabilityProvider;
        protected IMaxPriorYearProvider maxPriorYearProvider;
        protected IStudentMetricsProvider studentMetricsProvider;
        protected IGradeLevelUtilitiesProvider gradeLevelUtilitiesProvider;
        protected IPriorYearStudentMetricsProvider priorYearStudentMetricsProvider;

        protected PriorYearModel actualModel;

        protected readonly List<long> expectedStudentIds = new List<long>();
        protected string expectedSubjectArea;
        protected SchoolCategory expectedSchoolCategory;

        protected override void EstablishContext()
        {
            studentListWithMetricsRepository = mocks.StrictMock<IRepository<StudentMetric>>();
            teacherStudentSectionRepository = mocks.StrictMock<IRepository<TeacherStudentSection>>();
            teacherSectionRepository = mocks.StrictMock<IRepository<TeacherSection>>();
            staffStudentCohortRepository = mocks.StrictMock<IRepository<StaffStudentCohort>>();
            staffCohortRepository = mocks.StrictMock<IRepository<StaffCohort>>();
            staffCustomStudentListRepository = mocks.StrictMock<IRepository<StaffCustomStudentList>>();
            staffCustomStudentListStudentRepository = mocks.StrictMock<IRepository<StaffCustomStudentListStudent>>();
            schoolCategoryProvider = mocks.StrictMock<ISchoolCategoryProvider>();
            accommodationProvider = mocks.StrictMock<IAccommodationProvider>();
            uniqueListProvider = mocks.StrictMock<IUniqueListIdProvider>();
            trendRenderingDispositionProvider = mocks.StrictMock<ITrendRenderingDispositionProvider>();
            rootMetricNodeResolver = mocks.StrictMock<IRootMetricNodeResolver>();
            metricStateProvider = mocks.StrictMock<IMetricStateProvider>();
            listMetadataProvider = mocks.StrictMock<IListMetadataProvider>();
            priorYearClassroomMetricsProvider = mocks.StrictMock<IPriorYearClassroomMetricsProvider>();
            metadataListIdResolver = mocks.StrictMock<IMetadataListIdResolver>();
            warehouseAvailabilityProvider = mocks.StrictMock<IWarehouseAvailabilityProvider>();
            maxPriorYearProvider = mocks.StrictMock<IMaxPriorYearProvider>();
            studentMetricsProvider = mocks.StrictMock<IStudentMetricsProvider>();
            gradeLevelUtilitiesProvider = mocks.StrictMock<IGradeLevelUtilitiesProvider>();
            priorYearStudentMetricsProvider = mocks.StrictMock<IPriorYearStudentMetricsProvider>();

            Expect.Call(studentListWithMetricsRepository.GetAll()).Repeat.Any().Return(GetSuppliedStudentMetrics());
            Expect.Call(teacherStudentSectionRepository.GetAll()).Repeat.Any().Return(GetSuppliedTeacherStudentSection());
            Expect.Call(teacherSectionRepository.GetAll()).Repeat.Any().Return(GetSuppliedTeacherSection());
            Expect.Call(staffStudentCohortRepository.GetAll()).Repeat.Any().Return(GetSuppliedStaffStudentCohort());
            Expect.Call(staffCohortRepository.GetAll()).Repeat.Any().Return(GetSuppliedStaffCohort());
            Expect.Call(staffCustomStudentListRepository.GetAll()).Repeat.Any().Return(GetSuppliedStaffCustomStudentList());
            Expect.Call(staffCustomStudentListStudentRepository.GetAll()).Repeat.Any().Return(GetSuppliedStaffCustomStudentListStudent());

            Expect.Call(uniqueListProvider.GetUniqueId()).Return(suppliedUniqueListId);

            Expect.Call(trendRenderingDispositionProvider.GetTrendRenderingDisposition(TrendDirection.Decreasing, TrendInterpretation.Standard)).IgnoreArguments().Repeat.Any().Return(TrendEvaluation.DownBad);
            Expect.Call(accommodationProvider.GetAccommodations(expectedStudentIds.ToArray(), suppliedSchoolId)).Return(GetSuppliedAccommodations());

            Expect.Call(rootMetricNodeResolver.GetRootMetricNodeForStudent(suppliedSchoolId)).Repeat.Any().Return(GetStudentRootOverviewNode());

            Expect.Call(metricStateProvider.GetState(41, 1)).Repeat.Any().Return(new State { StateText = suppliedFourByFourText, DisplayStateText = suppliedFourByFourDisplayText });

            //Tested Elsewhere.
            Expect.Call(metadataListIdResolver.GetListId(ListType.ClassroomGeneralOverview, SchoolCategory.HighSchool)).IgnoreArguments().Repeat.Any().Return(1);
            Expect.Call(listMetadataProvider.GetListMetadata(1)).IgnoreArguments().Repeat.Any().Return(GetSuppliedListMetadata());
            Expect.Call(priorYearStudentMetricsProvider.GetStudentsWithMetrics(GetPriorYearStudentListWithMetricsQueryOptions()))
                .IgnoreArguments()
                .Repeat.Any()
                .Return(GetPriorYearStudentMetrics(suppliedLocalEducationAgencyId).AsQueryable());

            Expect.Call(warehouseAvailabilityProvider.Get()).Return(true);
            Expect.Call(maxPriorYearProvider.Get(suppliedLocalEducationAgencyId)).Return(2011);
            Expect.Call(studentMetricsProvider.GetOrderedStudentList(GetStudentListWithMetricsQueryOptions())).IgnoreArguments().Repeat.Any().Return(GetStudentSectionEntityListData(suppliedStudentListType).ToList().AsQueryable());
            Expect.Call(studentMetricsProvider.GetStudentsWithMetrics(GetStudentListWithMetricsQueryOptions())).IgnoreArguments().Repeat.Any().Return(GetStudentListSectionPageData(suppliedStudentListType).AsQueryable());
            Expect.Call(gradeLevelUtilitiesProvider.FormatGradeLevelForDisplay("1")).IgnoreArguments().Repeat.AtLeastOnce().Return("");
            Expect.Call(gradeLevelUtilitiesProvider.FormatGradeLevelForSorting("1")).IgnoreArguments().Repeat.AtLeastOnce().Return(1);

            base.EstablishContext();
        }

        protected List<EnhancedStudentInformation> GetStudentSectionEntityListData(string studentListType)
        {
            switch (studentListType)
            {
                case "Section":
                    return (from ts in GetSuppliedTeacherStudentSection()
                            join sl in GetSuppliedStudentList()
                                on ts.StudentUSI equals sl.StudentUSI
                            where ts.TeacherSectionId == suppliedSectionId && sl.SchoolId == suppliedSchoolId
                            select sl).ToList();
                case "Cohort":
                    return (from ssc in GetSuppliedStaffStudentCohort()
                            join sl in GetSuppliedStudentList()
                            on ssc.StudentUSI equals sl.StudentUSI
                            where ssc.StaffCohortId == suppliedSectionId && sl.SchoolId == suppliedSchoolId
                            select sl).ToList();
                case "CustomStudentList":
                    //need to break up this function because the tables are from 2 different DBs (App & LEA)
                    long[] studentUSIs = GetSuppliedStaffCustomStudentListStudent().Where(x => x.StaffCustomStudentListId == suppliedSectionId).Select(x => x.StudentUSI).ToArray();

                    if (studentUSIs.Length == 0)
                        return new List<EnhancedStudentInformation>();

                    return (from sl in GetSuppliedStudentList()
                            where studentUSIs.Contains(sl.StudentUSI) && sl.SchoolId == suppliedSchoolId
                            select sl).ToList();
                default:
                    var sectionStudentIds = (from tss in GetSuppliedTeacherStudentSection().Distinct()
                                             join ts in GetSuppliedTeacherSection() on tss.TeacherSectionId equals ts.TeacherSectionId
                                             where ts.StaffUSI == suppliedStaffUSI && ts.SchoolId == suppliedSchoolId
                                             group tss by tss.StudentUSI
                                                 into g
                                                 select g.Key).ToList();

                    var cohortStudentIds = (from ssc in GetSuppliedStaffStudentCohort().Distinct()
                                            join sc in GetSuppliedStaffCohort() on ssc.StaffCohortId equals sc.StaffCohortId
                                            where sc.StaffUSI == suppliedStaffUSI && sc.EducationOrganizationId == suppliedSchoolId
                                            group ssc by ssc.StudentUSI
                                                into h
                                                select h.Key).ToList();

                    var customStudentListStudentIds = (from csl in GetSuppliedStaffCustomStudentList()
                                                       join csls in GetSuppliedStaffCustomStudentListStudent() on csl.StaffCustomStudentListId equals csls.StaffCustomStudentListId
                                                       where csl.StaffUSI == suppliedStaffUSI && csl.EducationOrganizationId == suppliedSchoolId
                                                       group csls by csls.StudentUSI
                                                           into h
                                                           select h.Key).ToList();

                    var studentIds = sectionStudentIds.Concat(cohortStudentIds).Concat(customStudentListStudentIds).Distinct().ToArray();

                    if (studentIds.Length == 0)
                        return null;

                    return (from sl in GetSuppliedStudentList()
                            where studentIds.Contains(sl.StudentUSI) && sl.SchoolId == suppliedSchoolId
                            select sl).ToList();
            }
        }

        private IEnumerable<StudentMetric> GetStudentListSectionPageData(string studentListType)
        {
            switch (studentListType)
            {
                case "Section":
                    return from ts in GetSuppliedTeacherStudentSection()
                           join sl in GetSuppliedStudentMetrics()
                               on ts.StudentUSI equals sl.StudentUSI
                           where ts.TeacherSectionId == suppliedSectionId && sl.SchoolId == suppliedSchoolId
                           select sl;
                case "Cohort":
                    return from ssc in GetSuppliedStaffStudentCohort()
                           join sl in GetSuppliedStudentMetrics()
                               on ssc.StudentUSI equals sl.StudentUSI
                           where ssc.StaffCohortId == suppliedSectionId && sl.SchoolId == suppliedSchoolId
                           select sl;
                case "CustomStudentList":
                    //need to break up this function because the tables are from 2 different DBs (App & LEA)
                    long[] studentUSIs =
                        GetSuppliedStaffCustomStudentListStudent().Where(
                            x => x.StaffCustomStudentListId == suppliedSectionId).Select(x => x.StudentUSI).ToArray();

                    if (studentUSIs.Length == 0)
                        return null;

                    return from sl in GetSuppliedStudentMetrics()
                           where studentUSIs.Contains(sl.StudentUSI) && sl.SchoolId == suppliedSchoolId
                           select sl;
                default:
                    var sectionStudentIds = (from tss in GetSuppliedTeacherStudentSection().Distinct()
                                             join ts in GetSuppliedTeacherSection() on tss.TeacherSectionId equals ts.TeacherSectionId
                                             where ts.StaffUSI == suppliedStaffUSI && ts.SchoolId == suppliedSchoolId
                                             group tss by tss.StudentUSI
                                                 into g
                                                 select g.Key).ToList();

                    var cohortStudentIds = (from ssc in GetSuppliedStaffStudentCohort().Distinct()
                                            join sc in GetSuppliedStaffCohort() on ssc.StaffCohortId equals sc.StaffCohortId
                                            where sc.StaffUSI == suppliedStaffUSI && sc.EducationOrganizationId == suppliedSchoolId
                                            group ssc by ssc.StudentUSI
                                                into h
                                                select h.Key).ToList();

                    var customStudentListStudentIds = (from csl in GetSuppliedStaffCustomStudentList()
                                                       join csls in GetSuppliedStaffCustomStudentListStudent() on csl.StaffCustomStudentListId equals csls.StaffCustomStudentListId
                                                       where csl.StaffUSI == suppliedStaffUSI && csl.EducationOrganizationId == suppliedSchoolId
                                                       group csls by csls.StudentUSI
                                                           into h
                                                           select h.Key).ToList();

                    var studentIds = sectionStudentIds.Concat(cohortStudentIds).Concat(customStudentListStudentIds).Distinct().ToArray();

                    if (studentIds.Length == 0)
                        return null;

                    return from sl in GetSuppliedStudentMetrics()
                           where studentIds.Contains(sl.StudentUSI) && sl.SchoolId == suppliedSchoolId
                           select sl;
            }
        }

        private IEnumerable<StudentSchoolMetricInstance> GetPriorYearStudentMetrics(int localEducationAgencyId)
        {
            return from pysm in GetPriorYearStudentMetrics()
                   where pysm.LocalEducationAgencyId == localEducationAgencyId
                   select pysm;
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
                                                             ColumnName = "ColumnName",
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

        protected IQueryable<StudentSchoolMetricInstance> GetPriorYearStudentMetrics()
        {
            var data = new List<StudentSchoolMetricInstance>
                           {
                               new StudentSchoolMetricInstance{ StudentUSI = suppliedStudentId1, SchoolId = suppliedSchoolId + 1, LocalEducationAgencyId = suppliedLocalEducationAgencyId+1, SchoolYear = 2011 },
                               new StudentSchoolMetricInstance{ StudentUSI = suppliedStudentId1, SchoolId = suppliedSchoolId, LocalEducationAgencyId = suppliedLocalEducationAgencyId, SchoolYear = 2011 },
                               new StudentSchoolMetricInstance{ StudentUSI = suppliedStudentId2, SchoolId = suppliedSchoolId + 1, LocalEducationAgencyId = suppliedLocalEducationAgencyId, SchoolYear = 2011 },
                               new StudentSchoolMetricInstance{ StudentUSI = suppliedStudentId3, SchoolId = suppliedSchoolId, LocalEducationAgencyId = suppliedLocalEducationAgencyId, SchoolYear = 2011 },
                               new StudentSchoolMetricInstance{ StudentUSI = suppliedStudentId4, SchoolId = suppliedSchoolId, LocalEducationAgencyId = suppliedLocalEducationAgencyId, SchoolYear = 2011 },
                               new StudentSchoolMetricInstance{ StudentUSI = suppliedStudentId5, SchoolId = suppliedSchoolId, LocalEducationAgencyId = suppliedLocalEducationAgencyId, SchoolYear = 2011 },
                               new StudentSchoolMetricInstance{ StudentUSI = suppliedStudentId6, SchoolId = suppliedSchoolId, LocalEducationAgencyId = suppliedLocalEducationAgencyId, SchoolYear = 2011 },
                               new StudentSchoolMetricInstance{ StudentUSI = suppliedStudentId7, SchoolId = suppliedSchoolId, LocalEducationAgencyId = suppliedLocalEducationAgencyId, SchoolYear = 2011 },
                               new StudentSchoolMetricInstance{ StudentUSI = suppliedStudentId8, SchoolId = suppliedSchoolId, LocalEducationAgencyId = suppliedLocalEducationAgencyId, SchoolYear = 2011 },
                               new StudentSchoolMetricInstance{ StudentUSI = suppliedStudentId9, SchoolId = suppliedSchoolId, LocalEducationAgencyId = suppliedLocalEducationAgencyId, SchoolYear = 2011 },
                               new StudentSchoolMetricInstance{ StudentUSI = suppliedStudentId10, SchoolId = suppliedSchoolId, LocalEducationAgencyId = suppliedLocalEducationAgencyId, SchoolYear = 2011 },
                               new StudentSchoolMetricInstance{ StudentUSI = suppliedStudentId11, SchoolId = suppliedSchoolId, LocalEducationAgencyId = suppliedLocalEducationAgencyId, SchoolYear = 2011 },
                               new StudentSchoolMetricInstance{ StudentUSI = suppliedStudentId12, SchoolId = suppliedSchoolId, LocalEducationAgencyId = suppliedLocalEducationAgencyId, SchoolYear = 2011 },
                               new StudentSchoolMetricInstance{ StudentUSI = suppliedStudentId13, SchoolId = suppliedSchoolId, LocalEducationAgencyId = suppliedLocalEducationAgencyId + 1, SchoolYear = 2011 },
                               new StudentSchoolMetricInstance{ StudentUSI = suppliedStudentId14, SchoolId = suppliedSchoolId, LocalEducationAgencyId = suppliedLocalEducationAgencyId, SchoolYear = 2011 },
                               new StudentSchoolMetricInstance{ StudentUSI = suppliedStudentId15, SchoolId = suppliedSchoolId, LocalEducationAgencyId = suppliedLocalEducationAgencyId, SchoolYear = 2011 },
                               new StudentSchoolMetricInstance{ StudentUSI = suppliedStudentId16, SchoolId = suppliedSchoolId, LocalEducationAgencyId = suppliedLocalEducationAgencyId, SchoolYear = 2011 },
                               new StudentSchoolMetricInstance{ StudentUSI = suppliedStudentId17, SchoolId = suppliedSchoolId, LocalEducationAgencyId = suppliedLocalEducationAgencyId, SchoolYear = 2011 },
                               new StudentSchoolMetricInstance{ StudentUSI = suppliedStudentId18, SchoolId = suppliedSchoolId, LocalEducationAgencyId = suppliedLocalEducationAgencyId, SchoolYear = 2011 },
                               };
            return data.AsQueryable();
        }

        protected IQueryable<EnhancedStudentInformation> GetSuppliedStudentList()
        {
            var data = new List<EnhancedStudentInformation>
                           {
                               new EnhancedStudentInformation{ StudentUSI = suppliedStudentId1,  SchoolId = suppliedSchoolId + 1, FirstName = "wrong data" },
                               new EnhancedStudentInformation{ StudentUSI = suppliedStudentId1,  SchoolId = suppliedSchoolId, FirstName = "First 1",  LastSurname = "Last 1",  MiddleName = "Middle 1",  ProfileThumbnail = "thumbnail", Gender = "female"},
                               new EnhancedStudentInformation{ StudentUSI = suppliedStudentId2,  SchoolId = suppliedSchoolId, FirstName = "First 2",  LastSurname = "Last 2",  MiddleName = "Middle 2",  ProfileThumbnail = "thumbnail", Gender = "female"},
                               new EnhancedStudentInformation{ StudentUSI = suppliedStudentId3,  SchoolId = suppliedSchoolId, FirstName = "First 3",  LastSurname = "Last 3",  MiddleName = "Middle 3",  ProfileThumbnail = "thumbnail", Gender = "female"},
                               new EnhancedStudentInformation{ StudentUSI = suppliedStudentId4,  SchoolId = suppliedSchoolId, FirstName = "First 4",  LastSurname = "Last 4",  MiddleName = "Middle 4",  ProfileThumbnail = "thumbnail", Gender = "female"},
                               new EnhancedStudentInformation{ StudentUSI = suppliedStudentId5,  SchoolId = suppliedSchoolId, FirstName = "First 5",  LastSurname = "Last 5",  MiddleName = "Middle 5",  ProfileThumbnail = "thumbnail", Gender = "female"},
                               new EnhancedStudentInformation{ StudentUSI = suppliedStudentId6,  SchoolId = suppliedSchoolId, FirstName = "First 6",  LastSurname = "Last 6",  MiddleName = "Middle 6",  ProfileThumbnail = "thumbnail", Gender = "female"},
                               new EnhancedStudentInformation{ StudentUSI = suppliedStudentId7,  SchoolId = suppliedSchoolId, FirstName = "First 7",  LastSurname = "Last 7",  MiddleName = "Middle 7",  ProfileThumbnail = "thumbnail", Gender = "female"},
                               new EnhancedStudentInformation{ StudentUSI = suppliedStudentId8,  SchoolId = suppliedSchoolId, FirstName = "First 8",  LastSurname = "Last 8",  MiddleName = "Middle 8",  ProfileThumbnail = "thumbnail", Gender = "female"},
                               new EnhancedStudentInformation{ StudentUSI = suppliedStudentId9,  SchoolId = suppliedSchoolId, FirstName = "First 9",  LastSurname = "Last 9",  MiddleName = "Middle 9",  ProfileThumbnail = "thumbnail", Gender = "female"},
                               new EnhancedStudentInformation{ StudentUSI = suppliedStudentId10, SchoolId = suppliedSchoolId, FirstName = "First 10", LastSurname = "Last 10", MiddleName = "Middle 10", ProfileThumbnail = "thumbnail", Gender = "female"},
                               new EnhancedStudentInformation{ StudentUSI = suppliedStudentId11, SchoolId = suppliedSchoolId, FirstName = "First 11", LastSurname = "Last 11", MiddleName = "Middle 11", ProfileThumbnail = "thumbnail", Gender = "female"},
                               new EnhancedStudentInformation{ StudentUSI = suppliedStudentId12, SchoolId = suppliedSchoolId, FirstName = "First 12", LastSurname = "Last 12", MiddleName = "Middle 12", ProfileThumbnail = "thumbnail", Gender = "female"},
                               new EnhancedStudentInformation{ StudentUSI = suppliedStudentId13, SchoolId = suppliedSchoolId, FirstName = "First 13", LastSurname = "Last 13", MiddleName = "Middle 13", ProfileThumbnail = "thumbnail", Gender = "female"},
                               new EnhancedStudentInformation{ StudentUSI = suppliedStudentId14, SchoolId = suppliedSchoolId, FirstName = "First 14", LastSurname = "Last 14", MiddleName = "Middle 14", ProfileThumbnail = "thumbnail", Gender = "female"},
                               new EnhancedStudentInformation{ StudentUSI = suppliedStudentId15, SchoolId = suppliedSchoolId, FirstName = "First 15", LastSurname = "Last 15", MiddleName = "Middle 15", ProfileThumbnail = "thumbnail", Gender = "female"},
                               new EnhancedStudentInformation{ StudentUSI = suppliedStudentId16, SchoolId = suppliedSchoolId, FirstName = "First 16", LastSurname = "Last 16", MiddleName = "Middle 16", ProfileThumbnail = "thumbnail", Gender = "female"},
                               new EnhancedStudentInformation{ StudentUSI = suppliedStudentId17, SchoolId = suppliedSchoolId, FirstName = "First 17", LastSurname = "Last 17", MiddleName = "Middle 17", ProfileThumbnail = "thumbnail", Gender = "female"},
                               new EnhancedStudentInformation{ StudentUSI = suppliedStudentId18, SchoolId = suppliedSchoolId, FirstName = "First 18", LastSurname = "Last 18", MiddleName = "Middle 18", ProfileThumbnail = "thumbnail", Gender = "female"},
                               };
            return data.AsQueryable();
        }

        protected IQueryable<StudentMetric> GetSuppliedStudentMetrics()
        {
            var data = new List<StudentMetric>
                           {
                               new StudentMetric{ StudentUSI = suppliedStudentId1,  SchoolId = suppliedSchoolId + 1},
                               new StudentMetric{ StudentUSI = suppliedStudentId1,  SchoolId = suppliedSchoolId},
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
                               new StudentMetric{ StudentUSI = suppliedStudentId13, SchoolId = suppliedSchoolId},
                               new StudentMetric{ StudentUSI = suppliedStudentId14, SchoolId = suppliedSchoolId},
                               new StudentMetric{ StudentUSI = suppliedStudentId15, SchoolId = suppliedSchoolId},
                               new StudentMetric{ StudentUSI = suppliedStudentId16, SchoolId = suppliedSchoolId},
                               new StudentMetric{ StudentUSI = suppliedStudentId17, SchoolId = suppliedSchoolId},
                               new StudentMetric{ StudentUSI = suppliedStudentId18, SchoolId = suppliedSchoolId},
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
                               new TeacherStudentSection{ TeacherSectionId = suppliedTeacherSectionId1, StudentUSI = suppliedStudentId13},
                               new TeacherStudentSection{ TeacherSectionId = suppliedTeacherSectionId2, StudentUSI = suppliedStudentId1},
                               new TeacherStudentSection{ TeacherSectionId = suppliedTeacherSectionId2, StudentUSI = suppliedStudentId2},
                               new TeacherStudentSection{ TeacherSectionId = suppliedTeacherSectionId2, StudentUSI = suppliedStudentId3},
                               new TeacherStudentSection{ TeacherSectionId = suppliedTeacherSectionId2, StudentUSI = suppliedStudentId13},
                               new TeacherStudentSection{ TeacherSectionId = suppliedTeacherSectionId3, StudentUSI = suppliedStudentId1},
                               new TeacherStudentSection{ TeacherSectionId = suppliedTeacherSectionId3, StudentUSI = suppliedStudentId2},
                               new TeacherStudentSection{ TeacherSectionId = suppliedTeacherSectionId3, StudentUSI = suppliedStudentId3},
                               new TeacherStudentSection{ TeacherSectionId = suppliedTeacherSectionId3, StudentUSI = suppliedStudentId13},
                               new TeacherStudentSection{ TeacherSectionId = suppliedTeacherSectionId4, StudentUSI = suppliedStudentId1},
                               new TeacherStudentSection{ TeacherSectionId = suppliedTeacherSectionId4, StudentUSI = suppliedStudentId2},
                               new TeacherStudentSection{ TeacherSectionId = suppliedTeacherSectionId4, StudentUSI = suppliedStudentId3},
                               new TeacherStudentSection{ TeacherSectionId = suppliedTeacherSectionId4, StudentUSI = suppliedStudentId13},
                               new TeacherStudentSection{ TeacherSectionId = suppliedTeacherSectionId6, StudentUSI = suppliedStudentId1},
                               new TeacherStudentSection{ TeacherSectionId = suppliedTeacherSectionId6, StudentUSI = suppliedStudentId2},
                               new TeacherStudentSection{ TeacherSectionId = suppliedTeacherSectionId6, StudentUSI = suppliedStudentId3},
                               new TeacherStudentSection{ TeacherSectionId = suppliedTeacherSectionId6, StudentUSI = suppliedStudentId11},
                               new TeacherStudentSection{ TeacherSectionId = suppliedTeacherSectionId6, StudentUSI = suppliedStudentId13},
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

        protected IQueryable<StaffCustomStudentList> GetSuppliedStaffCustomStudentList()
        {
            var list = new List<StaffCustomStudentList>
                           {
                               new StaffCustomStudentList{ StaffCustomStudentListId = 999, StaffUSI = suppliedStaffUSI + 1, EducationOrganizationId = suppliedSchoolId },
                               new StaffCustomStudentList{ StaffCustomStudentListId = 1, StaffUSI = suppliedStaffUSI, EducationOrganizationId = suppliedSchoolId },
                               new StaffCustomStudentList{ StaffCustomStudentListId = 2, StaffUSI = suppliedStaffUSI, EducationOrganizationId = suppliedSchoolId },
                               new StaffCustomStudentList{ StaffCustomStudentListId = 3, StaffUSI = suppliedStaffUSI, EducationOrganizationId = suppliedSchoolId + 1 }
                           };
            return list.AsQueryable();
        }

        protected IQueryable<StaffCustomStudentListStudent> GetSuppliedStaffCustomStudentListStudent()
        {
            var list = new List<StaffCustomStudentListStudent>
                            {
                                new StaffCustomStudentListStudent{ StaffCustomStudentListId = 999, StudentUSI = suppliedStudentId13 },
                                new StaffCustomStudentListStudent{ StaffCustomStudentListId = 1, StudentUSI = suppliedStudentId14 },
                                new StaffCustomStudentListStudent{ StaffCustomStudentListId = 2, StudentUSI = suppliedStudentId15 },
                                new StaffCustomStudentListStudent{ StaffCustomStudentListId = 2, StudentUSI = suppliedStudentId16 },
                                new StaffCustomStudentListStudent{ StaffCustomStudentListId = 1, StudentUSI = suppliedStudentId17 },
                                new StaffCustomStudentListStudent{ StaffCustomStudentListId = 3, StudentUSI = suppliedStudentId18 },
                            };
            return list.AsQueryable();
        }

        protected virtual List<Accommodation> GetSuppliedAccommodations()
        {
            var data = new List<Accommodation>();
            return data;
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
            var service = new PriorYearService
            {
                StudentListWithMetricsRepository = studentListWithMetricsRepository,
                TeacherStudentSectionRepository = teacherStudentSectionRepository,
                TeacherSectionRepository = teacherSectionRepository,
                StaffStudentCohortRepository = staffStudentCohortRepository,
                StaffCohortRepository = staffCohortRepository,
                StaffCustomStudentListRepository = staffCustomStudentListRepository,
                StaffCustomStudentListStudentRepository = staffCustomStudentListStudentRepository,
                AccommodationProvider = accommodationProvider,
                UniqueListProvider = uniqueListProvider,
                TrendRenderingDispositionProvider = trendRenderingDispositionProvider,
                SchoolCategoryProvider = schoolCategoryProvider,
                RootMetricNodeResolver = rootMetricNodeResolver,
                MetricStateProvider = metricStateProvider,
                StudentSchoolAreaLinks = studentSchoolAreaLinks,
                ListMetadataProvider = listMetadataProvider,
                ClassroomMetricsProvider = priorYearClassroomMetricsProvider,
                MetadataListIdResolver = metadataListIdResolver,
                WarehouseAvailabilityProvider = warehouseAvailabilityProvider,
                MaxPriorYearProvider = maxPriorYearProvider,
                StudentListWithMetricsProvider = studentMetricsProvider,
                GradeLevelUtilitiesProvider = gradeLevelUtilitiesProvider,
                PriorYearStudentMetricsProvider = priorYearStudentMetricsProvider
            };

            actualModel = service.Get(new PriorYearRequest
                                            {
                                                StaffUSI = suppliedStaffUSI,
                                                SchoolId = suppliedSchoolId,
                                                StudentListType = suppliedStudentListType,
                                                SectionOrCohortId = suppliedSectionId,
                                                LocalEducationAgencyId = suppliedLocalEducationAgencyId
                                            });
        }
        
        [Test]
        public void Should_load_correct_students()
        {
            Assert.That(actualModel.Students.Count, Is.EqualTo(expectedStudentIds.Count));
            foreach (var student in actualModel.Students)
                Assert.That(expectedStudentIds.Contains(student.StudentUSI), Is.True, "Student USI " + student.StudentUSI + " was missing.");
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
            actualModel.EnsureNoDefaultValues("PriorYearModel.Students[0].IsFlagged",
                                              "PriorYearModel.Students[1].IsFlagged",
                                              "PriorYearModel.Students[2].IsFlagged",
                                              "PriorYearModel.Students[3].IsFlagged",
                                              "PriorYearModel.Students[4].IsFlagged",
                                              "PriorYearModel.Students[5].IsFlagged",
                                              "PriorYearModel.Students[6].IsFlagged",
                                              "PriorYearModel.Students[7].IsFlagged",
                                              "PriorYearModel.Students[8].IsFlagged",
                                              "PriorYearModel.Students[9].IsFlagged",
                                              "PriorYearModel.Students[10].IsFlagged",
                                              "PriorYearModel.Students[11].IsFlagged",
                                              "PriorYearModel.Students[12].IsFlagged", 
                                              "PriorYearModel.Students[0].SchoolName",
                                              "PriorYearModel.Students[1].SchoolName",
                                              "PriorYearModel.Students[2].SchoolName",
                                              "PriorYearModel.Students[3].SchoolName",
                                              "PriorYearModel.Students[4].SchoolName",
                                              "PriorYearModel.Students[5].SchoolName",
                                              "PriorYearModel.Students[6].SchoolName",
                                              "PriorYearModel.Students[7].SchoolName",
                                              "PriorYearModel.Students[8].SchoolName",
                                              "PriorYearModel.Students[9].SchoolName",
                                              "PriorYearModel.Students[10].SchoolName",
                                              "PriorYearModel.Students[11].SchoolName",
                                              "PriorYearModel.Students[12].SchoolName",
                                              "PriorYearModel.Students[0].Links",
                                              "PriorYearModel.Students[0].Accommodations",
                                              "PriorYearModel.Students[0].Metrics");
        }

        [Test]
        public virtual void Should_have_serializable_model()
        {
            actualModel.EnsureSerializableModel();
        }
    }

    public class When_loading_prior_year_for_high_school_teacher_section : PriorYearServiceFixtureBase
    {
        protected override void EstablishContext()
        {
            suppliedStudentListType = StudentListType.Section.ToString();
            suppliedSectionId = suppliedTeacherSectionId1;

            expectedStudentIds.Clear();
            expectedStudentIds.Add(suppliedStudentId1);
            expectedStudentIds.Add(suppliedStudentId2);
            expectedStudentIds.Add(suppliedStudentId3);

            expectedSchoolCategory = SchoolCategory.HighSchool;

            base.EstablishContext();

            Expect.Call(schoolCategoryProvider.GetSchoolCategoryType(suppliedSchoolId)).Return(SchoolCategory.HighSchool);
            Expect.Call(priorYearClassroomMetricsProvider.GetAdditionalMetrics(null, null, null))
                .IgnoreArguments()
                .Repeat.Any()
                .Return(new List<StudentWithMetrics.Metric>());
        }
    }

    public class When_loading_prior_year_for_middle_school_teacher_section : PriorYearServiceFixtureBase
    {
        protected override void EstablishContext()
        {
            suppliedStudentListType = StudentListType.Section.ToString();
            suppliedSectionId = suppliedTeacherSectionId1;

            expectedStudentIds.Clear();
            expectedStudentIds.Add(suppliedStudentId1);
            expectedStudentIds.Add(suppliedStudentId2);
            expectedStudentIds.Add(suppliedStudentId3);

            expectedSchoolCategory = SchoolCategory.MiddleSchool;

            base.EstablishContext();

            Expect.Call(schoolCategoryProvider.GetSchoolCategoryType(suppliedSchoolId)).Return(SchoolCategory.MiddleSchool);
            Expect.Call(priorYearClassroomMetricsProvider.GetAdditionalMetrics(null, null, null))
                .IgnoreArguments()
                .Repeat.Any()
                .Return(new List<StudentWithMetrics.Metric>());
        }

    }

    public class When_loading_prior_year_for_elementary_school_teacher_section : PriorYearServiceFixtureBase
    {
        protected override void EstablishContext()
        {
            suppliedStudentListType = StudentListType.Section.ToString();
            suppliedSectionId = suppliedTeacherSectionId1;

            expectedStudentIds.Clear();
            expectedStudentIds.Add(suppliedStudentId1);
            expectedStudentIds.Add(suppliedStudentId2);
            expectedStudentIds.Add(suppliedStudentId3);

            expectedSchoolCategory = SchoolCategory.Elementary;

            base.EstablishContext();

            Expect.Call(schoolCategoryProvider.GetSchoolCategoryType(suppliedSchoolId)).Return(SchoolCategory.Elementary);
            Expect.Call(priorYearClassroomMetricsProvider.GetAdditionalMetrics(null, null, null))
                .IgnoreArguments()
                .Repeat.Any()
                .Return(new List<StudentWithMetrics.Metric>());
        }
    }

    public class When_loading_prior_year_for_high_school_staff_cohort : PriorYearServiceFixtureBase
    {
        protected override void EstablishContext()
        {
            suppliedStudentListType = StudentListType.Cohort.ToString();
            suppliedSectionId = suppliedStaffCohortId1;

            expectedStudentIds.Clear();
            expectedStudentIds.Add(suppliedStudentId5);
            expectedStudentIds.Add(suppliedStudentId6);
            expectedStudentIds.Add(suppliedStudentId7);
            expectedStudentIds.Add(suppliedStudentId8);

            expectedSchoolCategory = SchoolCategory.HighSchool;

            base.EstablishContext();

            Expect.Call(schoolCategoryProvider.GetSchoolCategoryType(suppliedSchoolId)).Return(SchoolCategory.HighSchool);
            Expect.Call(priorYearClassroomMetricsProvider.GetAdditionalMetrics(null, null, null))
                .IgnoreArguments()
                .Repeat.Any()
                .Return(new List<StudentWithMetrics.Metric>());
        }
    }

    public class When_loading_prior_year_for_high_school_all_students : PriorYearServiceFixtureBase
    {
        protected override void EstablishContext()
        {
            suppliedStudentListType = StudentListType.All.ToString();
            suppliedSectionId = 0;

            expectedStudentIds.Clear();
            expectedStudentIds.Add(suppliedStudentId1);
            expectedStudentIds.Add(suppliedStudentId2);
            expectedStudentIds.Add(suppliedStudentId3);
            expectedStudentIds.Add(suppliedStudentId5);
            expectedStudentIds.Add(suppliedStudentId6);
            expectedStudentIds.Add(suppliedStudentId7);
            expectedStudentIds.Add(suppliedStudentId8);
            expectedStudentIds.Add(suppliedStudentId11);
            expectedStudentIds.Add(suppliedStudentId12);
            expectedStudentIds.Add(suppliedStudentId14);
            expectedStudentIds.Add(suppliedStudentId15);
            expectedStudentIds.Add(suppliedStudentId16);
            expectedStudentIds.Add(suppliedStudentId17);

            expectedSchoolCategory = SchoolCategory.HighSchool;

            base.EstablishContext();

            Expect.Call(schoolCategoryProvider.GetSchoolCategoryType(suppliedSchoolId)).Return(SchoolCategory.HighSchool);
            Expect.Call(priorYearClassroomMetricsProvider.GetAdditionalMetrics(null, null, null))
                .IgnoreArguments()
                .Repeat.Any()
                .Return(new List<StudentWithMetrics.Metric>());
        }
    }

    public class When_loading_prior_year_for_all_students_when_teacher_has_no_students : TestFixtureBase
    {
        protected const int suppliedTeacherSectionId1 = 100;
        protected const int suppliedStaffCohortId1 = 101;
        protected const int suppliedStudentId1 = 160;

        protected IRepository<StudentMetric> studentListWithMetricsRepository;
        protected IRepository<TeacherStudentSection> teacherStudentSectionRepository;
        protected IRepository<TeacherSection> teacherSectionRepository;
        protected IRepository<StaffStudentCohort> staffStudentCohortRepository;
        protected IRepository<StaffCohort> staffCohortRepository;
        protected IRepository<MetricComponent> metricComponentRepository;
        protected IRepository<StudentSchoolMetricInstanceSet> studentSchoolMetricInstanceSetRepository;
        protected IRepository<StudentRecordCurrentCourse> studentRecordCurrentCourseRepository;
        protected IRepository<StaffCustomStudentList> staffCustomStudentListRepository;
        protected IRepository<StaffCustomStudentListStudent> staffCustomStudentListStudentRepository;
        protected IRepository<Dashboards.Metric.Data.Entities.Metric> metricRepository;
        protected ISchoolCategoryProvider schoolCategoryProvider;
        protected IAccommodationProvider accommodationProvider;
        protected ITrendRenderingDispositionProvider trendRenderingDispositionProvider;
        protected IListMetadataProvider listMetadataProvider;
        protected IMetadataListIdResolver metadataListIdResolver;
        protected IGradeStateProvider gradeStateProvider;
        protected IRootMetricNodeResolver rootMetricNodeResolver;
        protected IMetricStateProvider metricStateProvider;
        protected IStudentMetricsProvider studentListWithMetricsProvider;
        protected IStudentWatchListManager watchListManager;

        protected const int suppliedStaffUSI = 1000;
        protected const int suppliedSchoolId = 2000;
        protected const int suppliedSectionOrCohortId = 0;
        protected const StudentListType suppliedStudentListType = StudentListType.None;
        protected const int suppliedLocalEducationAgencyId = 3000;
        protected const string suppliedUniqueListId = "baboon";
        protected const int suppliedUniqueIdentifier = 500;
        protected const int suppliedPageNumber = 1;
        protected const string suppliedSortDirection = "asc";

        protected GeneralOverviewModel actualModel;
        protected IStudentSchoolAreaLinks studentSchoolAreaLinks = new StudentSchoolAreaLinksFake();

        protected override void EstablishContext()
        {
            base.EstablishContext();

            schoolCategoryProvider = mocks.StrictMock<ISchoolCategoryProvider>();
            listMetadataProvider = mocks.StrictMock<IListMetadataProvider>();
            metadataListIdResolver = mocks.StrictMock<IMetadataListIdResolver>();
            studentListWithMetricsProvider = mocks.StrictMock<IStudentMetricsProvider>();

            teacherStudentSectionRepository = mocks.StrictMock<IRepository<TeacherStudentSection>>();
            teacherSectionRepository = mocks.StrictMock<IRepository<TeacherSection>>();
            staffStudentCohortRepository = mocks.StrictMock<IRepository<StaffStudentCohort>>();
            staffCohortRepository = mocks.StrictMock<IRepository<StaffCohort>>();
            staffCustomStudentListRepository = mocks.StrictMock<IRepository<StaffCustomStudentList>>();
            staffCustomStudentListStudentRepository = mocks.StrictMock<IRepository<StaffCustomStudentListStudent>>();
            watchListManager = mocks.StrictMock<IStudentWatchListManager>();

            Expect.Call(teacherStudentSectionRepository.GetAll()).Repeat.Any().Return(GetSuppliedTeacherStudentSection());
            Expect.Call(teacherSectionRepository.GetAll()).Repeat.Any().Return(GetSuppliedTeacherSection());
            Expect.Call(staffStudentCohortRepository.GetAll()).Repeat.Any().Return(GetSuppliedStaffStudentCohort());
            Expect.Call(staffCohortRepository.GetAll()).Repeat.Any().Return(GetSuppliedStaffCohort());
            Expect.Call(staffCustomStudentListRepository.GetAll()).Repeat.Any().Return(GetSuppliedStaffCustomStudentList());
            Expect.Call(staffCustomStudentListStudentRepository.GetAll()).Repeat.Any().Return(GetSuppliedStaffCustomStudentListStudent());
            Expect.Call(schoolCategoryProvider.GetSchoolCategoryType(suppliedSchoolId)).Repeat.Any().IgnoreArguments().Return(SchoolCategory.HighSchool);
            Expect.Call(metadataListIdResolver.GetListId(ListType.ClassroomGeneralOverview, SchoolCategory.HighSchool)).IgnoreArguments().Repeat.Any().Return(1);
            Expect.Call(listMetadataProvider.GetListMetadata(1)).IgnoreArguments().Repeat.Any().Return(GetSuppliedListMetadata());
            Expect.Call(studentListWithMetricsProvider.GetOrderedStudentList(GetStudentListWithMetricsQueryOptions())).IgnoreArguments().Repeat.Any().Return(new List<EnhancedStudentInformation>().AsQueryable());
            Expect.Call(studentListWithMetricsProvider.GetStudentsWithMetrics(GetStudentListWithMetricsQueryOptions())).IgnoreArguments().Repeat.Any().Return(new List<StudentMetric>().AsQueryable());
            Expect.Call(watchListManager.CreateStudentMetricsProviderQueryOptions(null, null, 0, 0, 0, null, 0, StudentListType.All)).IgnoreArguments().Repeat.Any().Return(new StudentMetricsProviderQueryOptions());
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
                                                             ColumnName = "ColumnName",
                                                             ColumnPrefix = "ColumnPrefix",
                                                             IsVisibleByDefault = true,
                                                             MetricVariantId = 1,
                                                             MetricListCellType = MetricListCellType.TrendMetric,
                                                             Order = 1,
                                                             SchoolCategory = SchoolCategory.HighSchool,
                                                             SortAscending = "SortAscending",
                                                             SortDescending = "SortDescending",
                                                             Tooltip = "Tooltip"
                                                         }
                                                 }
                               }
                       };
        }

        protected override void ExecuteTest()
        {
            var service = new GeneralOverviewService
            {
                StudentListWithMetricsRepository = studentListWithMetricsRepository,
                TeacherStudentSectionRepository = teacherStudentSectionRepository,
                TeacherSectionRepository = teacherSectionRepository,
                StaffStudentCohortRepository = staffStudentCohortRepository,
                StaffCohortRepository = staffCohortRepository,
                StaffCustomStudentListRepository = staffCustomStudentListRepository,
                StaffCustomStudentListStudentRepository = staffCustomStudentListStudentRepository,
                AccommodationProvider = accommodationProvider,
                TrendRenderingDispositionProvider = trendRenderingDispositionProvider,
                SchoolCategoryProvider = schoolCategoryProvider,
                RootMetricNodeResolver = rootMetricNodeResolver,
                MetricStateProvider = metricStateProvider,
                StudentSchoolAreaLinks = studentSchoolAreaLinks,
                ListMetadataProvider = listMetadataProvider,
                MetadataListIdResolver = metadataListIdResolver,
                StudentListWithMetricsProvider = studentListWithMetricsProvider,
                WatchListManager = watchListManager
            };

            actualModel = service.Get(new GeneralOverviewRequest()
            {
                StaffUSI = suppliedStaffUSI,
                SchoolId = suppliedSchoolId,
                StudentListType = StudentListType.All.ToString(),
                SectionOrCohortId = 0,
            });
        }

        protected IQueryable<TeacherStudentSection> GetSuppliedTeacherStudentSection()
        {
            var data = new List<TeacherStudentSection>
            {
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
        
        protected IQueryable<StaffStudentCohort> GetSuppliedStaffStudentCohort()
        {
            var data = new List<StaffStudentCohort>
            {
            };
            return data.AsQueryable();
        }

        protected IQueryable<StaffCohort> GetSuppliedStaffCohort()
        {
            var data = new List<StaffCohort>
                           {
                               new StaffCohort{ StaffUSI = suppliedStaffUSI, EducationOrganizationId = suppliedSchoolId, StaffCohortId = suppliedStaffCohortId1},
                           };
            return data.AsQueryable();
        }
        
        protected IQueryable<StaffCustomStudentList> GetSuppliedStaffCustomStudentList()
        {
            var list = new List<StaffCustomStudentList>
                           {
                               new StaffCustomStudentList{ StaffCustomStudentListId = 1, StaffUSI = suppliedStaffUSI }
                           };
            return list.AsQueryable();
        }

        protected IQueryable<StaffCustomStudentListStudent> GetSuppliedStaffCustomStudentListStudent()
        {
            var list = new List<StaffCustomStudentListStudent>
            {
            };
            return list.AsQueryable();
        }

        [Test]
        public void Should_not_load_any_data()
        {
            Assert.That(actualModel, Is.Not.Null);
            Assert.That(actualModel.Students.Count, Is.EqualTo(0));
        }
    }

    public class When_loading_prior_year_classroom_but_warehouse_is_unavailable : TestFixtureBase
    {
        protected IRepository<StudentMetric> studentListWithMetricsRepository;
        protected IRepository<TeacherStudentSection> teacherStudentSectionRepository;
        protected IRepository<TeacherSection> teacherSectionRepository;
        protected IRepository<StaffStudentCohort> staffStudentCohortRepository;
        protected IRepository<StaffCohort> staffCohortRepository;
        protected ISchoolCategoryProvider schoolCategoryProvider;
        protected IAccommodationProvider accommodationProvider;
        protected IUniqueListIdProvider uniqueListProvider;
        protected ITrendRenderingDispositionProvider trendRenderingDispositionProvider;
        protected IRootMetricNodeResolver rootMetricNodeResolver;
        protected IMetricStateProvider metricStateProvider;
        protected IStudentSchoolAreaLinks studentSchoolAreaLinks = new StudentSchoolAreaLinksFake();
        protected IPriorYearClassroomMetricsProvider priorYearClassroomMetricsProvider;
        protected IListMetadataProvider listMetadataProvider;
        protected IMetadataListIdResolver metadataListIdResolver;
        protected IWarehouseAvailabilityProvider warehouseAvailabilityProvider;
        protected IMaxPriorYearProvider maxPriorYearProvider;

        protected PriorYearModel actualModel;

        protected const int suppliedStaffUSI = 1000;
        protected const int suppliedSchoolId = 2000;
        protected const int suppliedLocalEducationAgencyId = 3000;
        protected string suppliedStudentListType = StudentListType.All.ToString();
        protected const int suppliedSectionId = 0;

        protected override void EstablishContext()
        {
            studentListWithMetricsRepository = mocks.StrictMock<IRepository<StudentMetric>>();
            teacherStudentSectionRepository = mocks.StrictMock<IRepository<TeacherStudentSection>>();
            teacherSectionRepository = mocks.StrictMock<IRepository<TeacherSection>>();
            staffStudentCohortRepository = mocks.StrictMock<IRepository<StaffStudentCohort>>();
            staffCohortRepository = mocks.StrictMock<IRepository<StaffCohort>>();
            schoolCategoryProvider = mocks.StrictMock<ISchoolCategoryProvider>();
            accommodationProvider = mocks.StrictMock<IAccommodationProvider>();
            uniqueListProvider = mocks.StrictMock<IUniqueListIdProvider>();
            trendRenderingDispositionProvider = mocks.StrictMock<ITrendRenderingDispositionProvider>();
            rootMetricNodeResolver = mocks.StrictMock<IRootMetricNodeResolver>();
            metricStateProvider = mocks.StrictMock<IMetricStateProvider>();
            listMetadataProvider = mocks.StrictMock<IListMetadataProvider>();
            priorYearClassroomMetricsProvider = mocks.StrictMock<IPriorYearClassroomMetricsProvider>();
            metadataListIdResolver = mocks.StrictMock<IMetadataListIdResolver>();
            warehouseAvailabilityProvider = mocks.StrictMock<IWarehouseAvailabilityProvider>();
            maxPriorYearProvider = mocks.StrictMock<IMaxPriorYearProvider>();

            Expect.Call(warehouseAvailabilityProvider.Get()).Return(false);

            base.EstablishContext();
        }

        protected override void ExecuteTest()
        {
            var service = new PriorYearService
            {
                StudentListWithMetricsRepository = studentListWithMetricsRepository,
                TeacherStudentSectionRepository = teacherStudentSectionRepository,
                TeacherSectionRepository = teacherSectionRepository,
                StaffStudentCohortRepository = staffStudentCohortRepository,
                StaffCohortRepository = staffCohortRepository,
                AccommodationProvider = accommodationProvider,
                UniqueListProvider = uniqueListProvider,
                TrendRenderingDispositionProvider = trendRenderingDispositionProvider,
                SchoolCategoryProvider = schoolCategoryProvider,
                RootMetricNodeResolver = rootMetricNodeResolver,
                MetricStateProvider = metricStateProvider,
                StudentSchoolAreaLinks = studentSchoolAreaLinks,
                ListMetadataProvider = listMetadataProvider,
                ClassroomMetricsProvider = priorYearClassroomMetricsProvider,
                MetadataListIdResolver = metadataListIdResolver,
                WarehouseAvailabilityProvider = warehouseAvailabilityProvider,
                MaxPriorYearProvider = maxPriorYearProvider
            };

            actualModel = service.Get(new PriorYearRequest
                                            {
                                                StaffUSI = suppliedStaffUSI,
                                                SchoolId = suppliedSchoolId,
                                                StudentListType = suppliedStudentListType,
                                                SectionOrCohortId = suppliedSectionId,
                                                LocalEducationAgencyId = suppliedLocalEducationAgencyId
                                            });
        }


        [Test]
        public void Should_return_empty_model()
        {
            Assert.That(actualModel, Is.Not.Null);
        }
    }
}
