// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using EdFi.Dashboards.Application.Data.Entities;
using EdFi.Dashboards.Common.Utilities;
using EdFi.Dashboards.Resources.Common;
using EdFi.Dashboards.Resources.Models.Common;
using EdFi.Dashboards.Resources.Navigation;
using EdFi.Dashboards.Resources.Security.Common;
using EdFi.Dashboards.Resources.Security.Implementations;
using EdFi.Dashboards.Resources.Staff;
using EdFi.Dashboards.Resources.Tests.Navigation.Fakes;
using EdFi.Dashboards.SecurityTokenService.Authentication;
using NUnit.Framework;
using EdFi.Dashboards.Data.Entities;
using EdFi.Dashboards.Data.Repository;
using EdFi.Dashboards.Infrastructure;
using EdFi.Dashboards.Resources.Models.Staff;
using EdFi.Dashboards.Testing;
using Rhino.Mocks;

namespace EdFi.Dashboards.Resources.Tests.Staff
{
    public abstract class StaffServiceFixture : TestFixtureBase
    {
        protected IRepository<StaffCohort> staffCohortRepository;
        protected IRepository<StaffCustomStudentList> staffCustomStudentListRepository;
        protected IRepository<StaffInformation> staffInfoRepository;
        protected IRepository<TeacherSection> teacherSectionRepository;
        protected IRepository<SchoolInformation> schoolInformationRepository;
        protected IRepository<MetricBasedWatchList> WatchListRepository;
        protected IRepository<MetricBasedWatchListOption> WatchListOptionRepository;
        protected short providedSchoolYear = 2011;
        protected int providedStaffUSI1 = 100;
        protected string providedStaffImage = "402.jpg";
        protected string providedStaffGender = "Male";
        protected string providedThumbnail = "imagethumb.png";
        protected int providedLocalEducationAgencyId = 1000;
        protected int providedSchoolId = 2000;
        protected string providedSchoolName = "elephant high school";
        protected string providedSchoolName1 = "xylophone";
        protected string providedSchoolName2 = "test school name";
        protected string providedSchoolName3 = "apple";
        protected string providedSchoolName4 = "The School Name ";
        protected string providedFullName = "Tom X. Teacher";
        protected string providedCohortDescription1 = "cohort Description 1";
        protected string providedCohortDescription2 = "cohort Description 2";
        protected int providedCohortId1 = 3000;
        protected int providedCohortId2 = 3001;
        protected int providedTeacherSection1 = 4000;
        protected int providedTeacherSection2 = 4001;
        protected int providedStaffUSI;
        protected string providedUserName;
        protected UserInformation providedUserInformation;

        protected string suppliedViewType = String.Empty;
        protected string suppliedSubjectArea = String.Empty;
        protected string suppliedAssessmentSubType = string.Empty;
        protected int suppliedSectionId;
        protected string suppliedStudentListType = String.Empty;

        protected StaffModel actualModel;

        protected IStaffAreaLinks staffAreaLinksFake = new StaffAreaLinksFake();
        protected IStaffViewProvider staffViewProvider = new StaffViewProvider();
        protected IWatchListLinkProvider watchListLinkProvider;

        private ICurrentUserClaimInterrogator currentUserClaimInterrogator;

        protected override void EstablishContext()
        {
            providedStaffImage = staffAreaLinksFake.Image(providedSchoolId, providedStaffUSI1, providedStaffGender);
            providedThumbnail = staffAreaLinksFake.ProfileThumbnail(providedSchoolId, providedStaffUSI1, providedStaffGender);

            staffCohortRepository = mocks.StrictMock<IRepository<StaffCohort>>();
            Expect.Call(staffCohortRepository.GetAll()).Repeat.Any().Return(GetStaffCohort());

            staffCustomStudentListRepository = mocks.StrictMock<IRepository<StaffCustomStudentList>>();
            Expect.Call(staffCustomStudentListRepository.GetAll()).Return(GetStaffCustomStudentList());

            staffInfoRepository = mocks.StrictMock<IRepository<StaffInformation>>();
            Expect.Call(staffInfoRepository.GetAll()).Return(GetStaffInformation());

            teacherSectionRepository = mocks.StrictMock<IRepository<TeacherSection>>();
            Expect.Call(teacherSectionRepository.GetAll()).Repeat.Any().Return(GetTeacherSection());
            
            schoolInformationRepository = mocks.StrictMock<IRepository<SchoolInformation>>();
            Expect.Call(schoolInformationRepository.GetAll()).Repeat.Any().Return(GetSchoolInformation());

            WatchListRepository = mocks.StrictMock<IRepository<MetricBasedWatchList>>();
            Expect.Call(WatchListRepository.GetAll()).Repeat.Any().Return(GetWatchListInformation());

            WatchListOptionRepository = mocks.StrictMock<IRepository<MetricBasedWatchListOption>>();
            Expect.Call(WatchListOptionRepository.GetAll()).Repeat.Any().Return(GetWatchListOptionInformation());

            currentUserClaimInterrogator = mocks.StrictMock<ICurrentUserClaimInterrogator>();
            Expect.Call(currentUserClaimInterrogator.HasClaimWithinEducationOrganizationHierarchy(String.Empty, 0)).
                IgnoreArguments().Repeat.Any().Return(true);

            watchListLinkProvider = mocks.StrictMock<IWatchListLinkProvider>();
            Expect.Call(watchListLinkProvider.GenerateLink(null)).
                IgnoreArguments().Repeat.Any().Return("http://testlink");

            LoginUser(providedUserInformation);

            base.EstablishContext();
        }

        private IQueryable<MetricBasedWatchList> GetWatchListInformation()
        {
            var data = new List<MetricBasedWatchList>
            {
                new MetricBasedWatchList { EducationOrganizationId = providedSchoolId, MetricBasedWatchListId = 1, StaffUSI = providedStaffUSI, WatchListName = "Test Watch List Name" }
            };

            return data.AsQueryable();
        }

        private IQueryable<MetricBasedWatchListOption> GetWatchListOptionInformation()
        {
            var data = new List<MetricBasedWatchListOption>
            {
                new MetricBasedWatchListOption{ MetricBasedWatchListOptionId = 1, MetricBasedWatchListId = 1, Name = "Test Option", Value = "Test Value" }
            };

            return data.AsQueryable();
        }

        protected void LoginUser(UserInformation userInformation)
        {
            Thread.CurrentPrincipal = userInformation.ToClaimsPrincipal();
        }

        protected IQueryable<StaffCohort> GetStaffCohort()
        {
            var data = new List<StaffCohort>
                           {
                               new StaffCohort {StaffUSI = providedStaffUSI1 + 1, EducationOrganizationId = providedSchoolId, StaffCohortId = providedCohortId1 + 10, CohortDescription = "wrong staff" },
                               new StaffCohort {StaffUSI = providedStaffUSI1, EducationOrganizationId = providedSchoolId, StaffCohortId = providedCohortId1, CohortDescription = providedCohortDescription1 },
                               new StaffCohort {StaffUSI = providedStaffUSI1, EducationOrganizationId = providedSchoolId, StaffCohortId = providedCohortId2, CohortDescription = providedCohortDescription2,  AcademicSubjectType = "Social Studies"},
                               new StaffCohort {StaffUSI = providedStaffUSI1, EducationOrganizationId = providedSchoolId+1, StaffCohortId = providedCohortId1 + 11, CohortDescription = "wrong school" },
                               new StaffCohort {StaffUSI = providedStaffUSI1, EducationOrganizationId = providedSchoolId+2, StaffCohortId = providedCohortId1 + 12, CohortDescription = "wrong school" }
                           };
            return data.AsQueryable();
        }

        protected IQueryable<StaffCustomStudentList> GetStaffCustomStudentList()
        {
            var data = new List<StaffCustomStudentList>
                           {
                               new StaffCustomStudentList {StaffUSI = providedStaffUSI1 + 1, EducationOrganizationId = providedSchoolId, StaffCustomStudentListId = providedCohortId1 + 10, CustomStudentListIdentifier = "custom wrong staff" },
                               new StaffCustomStudentList {StaffUSI = providedStaffUSI1, EducationOrganizationId = providedSchoolId, StaffCustomStudentListId = providedCohortId1, CustomStudentListIdentifier = "custom "+ providedCohortDescription1 },
                               new StaffCustomStudentList {StaffUSI = providedStaffUSI1, EducationOrganizationId = providedSchoolId, StaffCustomStudentListId = providedCohortId2, CustomStudentListIdentifier = "custom "+ providedCohortDescription2 },
                               new StaffCustomStudentList {StaffUSI = providedStaffUSI1, EducationOrganizationId = providedSchoolId+1, StaffCustomStudentListId = providedCohortId1 + 11, CustomStudentListIdentifier = "custom wrong school" },
                               new StaffCustomStudentList {StaffUSI = providedStaffUSI1, EducationOrganizationId = providedSchoolId+2, StaffCustomStudentListId = providedCohortId1 + 12, CustomStudentListIdentifier = "custom wrong school" }
                           };
            return data.AsQueryable();
        }

        protected IQueryable<StaffInformation> GetStaffInformation()
        {
            var data = new List<StaffInformation>
                        {
                            new StaffInformation {StaffUSI = providedStaffUSI1-2, FullName = "Not my name"},
                            new StaffInformation {StaffUSI = providedStaffUSI1-1, FullName = "still wrong"},
                            new StaffInformation {StaffUSI = providedStaffUSI1, FullName = providedFullName, Gender = providedStaffGender, ProfileThumbnail = providedStaffImage},
                            new StaffInformation {StaffUSI = providedStaffUSI1+1, FullName = "try again"}
                        };
            return data.AsQueryable();
        }

        protected IQueryable<TeacherSection> GetTeacherSection()
        {
            var data = new List<TeacherSection>
                           {
                                new TeacherSection {StaffUSI = providedStaffUSI1+1, SchoolId = providedSchoolId, TeacherSectionId=providedTeacherSection1 + 10, SubjectArea="wrong staff", ClassPeriod="class period", CourseTitle="course title", GradeLevel="grade level", LocalCourseCode="local course code", TermType="term type"},
                                new TeacherSection {StaffUSI = providedStaffUSI1, SchoolId = providedSchoolId, TeacherSectionId=providedTeacherSection1, SubjectArea="Mathematics", ClassPeriod="class period", CourseTitle="course title", GradeLevel="grade level", LocalCourseCode="local course code", TermType="term type"},
                                new TeacherSection {StaffUSI = providedStaffUSI1, SchoolId = providedSchoolId, TeacherSectionId=providedTeacherSection2, SubjectArea="zebra", ClassPeriod="class period", CourseTitle="course title", GradeLevel="grade level", LocalCourseCode="local course code", TermType="term type"},
                                new TeacherSection {StaffUSI = providedStaffUSI1, SchoolId = providedSchoolId+1, TeacherSectionId=providedTeacherSection1 + 11, SubjectArea="wrong school", ClassPeriod="class period", CourseTitle="course title", GradeLevel="grade level", LocalCourseCode="local course code", TermType="term type"},
                                new TeacherSection {StaffUSI = providedStaffUSI1, SchoolId = providedSchoolId+3, TeacherSectionId=providedTeacherSection1 + 12, SubjectArea="wrong school", ClassPeriod="class period", CourseTitle="course title", GradeLevel="grade level", LocalCourseCode="local course code", TermType="term type"},
                           };
            return data.AsQueryable();
        }

        protected IQueryable<SchoolInformation> GetSchoolInformation()
        {
            var data = new List<SchoolInformation>
                           {
                               new SchoolInformation {SchoolId = providedSchoolId, Name = providedSchoolName},
                               new SchoolInformation {SchoolId = providedSchoolId + 1, Name = providedSchoolName1},
                               new SchoolInformation {SchoolId = providedSchoolId + 2, Name = providedSchoolName2},
                               new SchoolInformation {SchoolId = providedSchoolId + 3, Name = providedSchoolName3},
                               new SchoolInformation {SchoolId = providedSchoolId + 4, Name = providedSchoolName4},
                           };
            return data.AsQueryable();
        }

        protected override void ExecuteTest()
        {
            var staffService = new StaffService(staffCohortRepository, staffInfoRepository, teacherSectionRepository, schoolInformationRepository, staffAreaLinksFake, staffCustomStudentListRepository, WatchListRepository, WatchListOptionRepository, currentUserClaimInterrogator, staffViewProvider, watchListLinkProvider);
            actualModel = staffService.Get(new StaffRequest
                                               {
                                                   StaffUSI = providedStaffUSI1,
                                                   SchoolId = providedSchoolId,
                                                   StudentListType = suppliedStudentListType,
                                                   SectionOrCohortId = suppliedSectionId,
                                                   ViewType = suppliedViewType,
                                                   SubjectArea = suppliedSubjectArea,
                                                   AssessmentSubType = suppliedAssessmentSubType
                                               });
        }
    
        [Test]
        public void Should_populate_model()
        {
            Assert.That(actualModel.FullName, Is.EqualTo(providedFullName));
            Assert.That(actualModel.StaffUSI, Is.EqualTo(providedStaffUSI1));
            Assert.That(actualModel.ProfileThumbnail, Is.EqualTo(providedThumbnail));
        }

        [Test]
        public virtual void Should_load_sections_and_cohorts()
        {
            Assert.That(actualModel.SectionsAndCohorts, Is.Not.Null);
            Assert.That(actualModel.SectionsAndCohorts.Where(x => x.ListType == StudentListType.Cohort || x.ListType == StudentListType.Section || x.ListType == StudentListType.All).Count(), Is.EqualTo(5));
            Assert.That(actualModel.SectionsAndCohorts[0].SectionId, Is.EqualTo(0));
            Assert.That(actualModel.SectionsAndCohorts[0].Description, Is.EqualTo("Students From All Sections"));
            Assert.That(actualModel.SectionsAndCohorts[0].Link.Href, Is.EqualTo(staffAreaLinksFake.GeneralOverview(providedSchoolId, providedStaffUSI1, providedFullName, null as int?, StudentListType.All.ToString())));
            Assert.That(actualModel.SectionsAndCohorts[1].SectionId, Is.EqualTo(providedTeacherSection1));
            Assert.That(actualModel.SectionsAndCohorts[1].Description, Is.EqualTo("Mathematics(local course code) - course title (class period) term type"));
            Assert.That(actualModel.SectionsAndCohorts[1].Link.Href, Is.EqualTo(staffAreaLinksFake.GeneralOverview(providedSchoolId, providedStaffUSI1, providedFullName, providedTeacherSection1, StudentListType.Section.ToString())));
            Assert.That(actualModel.SectionsAndCohorts[1].Selected, Is.True);
            Assert.That(actualModel.SectionsAndCohorts[3].SectionId, Is.EqualTo(providedCohortId1));
            Assert.That(actualModel.SectionsAndCohorts[3].Description, Is.EqualTo(providedCohortDescription1));
            Assert.That(actualModel.SectionsAndCohorts[3].Link.Href, Is.EqualTo(staffAreaLinksFake.GeneralOverview(providedSchoolId, providedStaffUSI1, providedFullName, providedCohortId1, StudentListType.Cohort.ToString())));

            Assert.That(actualModel.SectionsAndCohorts.SelectMany(s => s.ChildSections), Has.All.Property("Link").Not.Null);
        }

        [Test]
        public virtual void Should_load_all_views()
        {
            Assert.That(actualModel.Views, Is.Not.Null);
            Assert.That(actualModel.Views.Count, Is.EqualTo(4));

            var generalOverviewView = actualModel.Views[0];
            var priorYearView = actualModel.Views[1];
            var subjectSpecificView = actualModel.Views[2];
            var assessmentDetailsView = actualModel.Views[3];

            //Validate the description
            Assert.That(generalOverviewView.Description, Is.EqualTo("General Overview"));
            Assert.That(priorYearView.Description, Is.EqualTo("Prior Year"));
            Assert.That(subjectSpecificView.Description, Is.EqualTo("Subject Specific"));
            Assert.That(assessmentDetailsView.Description, Is.EqualTo("Assessment Details"));

            Assert.That(generalOverviewView.ChildViews.Count, Is.EqualTo(0));
            Assert.That(priorYearView.ChildViews.Count, Is.EqualTo(0));
            Assert.That(subjectSpecificView.ChildViews.Count, Is.EqualTo(0));
            Assert.That(assessmentDetailsView.ChildViews.Count, Is.EqualTo(2));

            Assert.That(assessmentDetailsView.ChildViews[0].Description, Is.EqualTo("State Standardized"));
            Assert.That(assessmentDetailsView.ChildViews[0].ChildViews.Count, Is.EqualTo(5));
            Assert.That(assessmentDetailsView.ChildViews[0].ChildViews[0].Description, Is.EqualTo("- ELA / Reading (State)"));
            Assert.That(assessmentDetailsView.ChildViews[0].ChildViews[1].Description, Is.EqualTo("- Writing (State)"));
            Assert.That(assessmentDetailsView.ChildViews[0].ChildViews[2].Description, Is.EqualTo("- Mathematics (State)"));
            Assert.That(assessmentDetailsView.ChildViews[0].ChildViews[3].Description, Is.EqualTo("- Science (State)"));
            Assert.That(assessmentDetailsView.ChildViews[0].ChildViews[4].Description, Is.EqualTo("- Social Studies (State)"));

            Assert.That(assessmentDetailsView.ChildViews[1].Description, Is.EqualTo("District Benchmark"));
            Assert.That(assessmentDetailsView.ChildViews[1].ChildViews.Count, Is.EqualTo(5));
            Assert.That(assessmentDetailsView.ChildViews[1].ChildViews[0].Description, Is.EqualTo("- ELA / Reading (District)"));
            Assert.That(assessmentDetailsView.ChildViews[1].ChildViews[1].Description, Is.EqualTo("- Writing (District)"));
            Assert.That(assessmentDetailsView.ChildViews[1].ChildViews[2].Description, Is.EqualTo("- Mathematics (District)"));
            Assert.That(assessmentDetailsView.ChildViews[1].ChildViews[3].Description, Is.EqualTo("- Science (District)"));
            Assert.That(assessmentDetailsView.ChildViews[1].ChildViews[4].Description, Is.EqualTo("- Social Studies (District)"));

            //validate the link
            Assert.That(generalOverviewView.Link.Href, Is.EqualTo(staffAreaLinksFake.GeneralOverview(providedSchoolId, providedStaffUSI1, providedFullName, providedTeacherSection1, StudentListType.Section.ToString())));
            Assert.That(priorYearView.Link.Href, Is.EqualTo(staffAreaLinksFake.PriorYear(providedSchoolId, providedStaffUSI1, providedFullName, providedTeacherSection1, StudentListType.Section.ToString())));
            Assert.That(subjectSpecificView.Link.Href, Is.EqualTo(staffAreaLinksFake.SubjectSpecificOverview(providedSchoolId, providedStaffUSI1, providedFullName, providedTeacherSection1, StudentListType.Section.ToString())));
            Assert.That(assessmentDetailsView.Link.Href, Is.EqualTo(staffAreaLinksFake.AssessmentDetails(providedSchoolId, providedStaffUSI1, providedFullName, providedTeacherSection1, StudentListType.Section.ToString(), null, StaffModel.AssessmentSubType.StateStandardized.ToString())));

            Assert.That(assessmentDetailsView.ChildViews[0].Link.Href, Is.EqualTo(staffAreaLinksFake.AssessmentDetails(providedSchoolId, providedStaffUSI1, providedFullName, providedTeacherSection1, StudentListType.Section.ToString(), null, StaffModel.AssessmentSubType.StateStandardized.ToString())));
            Assert.That(assessmentDetailsView.ChildViews[0].ChildViews[0].Link.Href, Is.EqualTo(staffAreaLinksFake.AssessmentDetails(providedSchoolId, providedStaffUSI1, providedFullName, providedTeacherSection1, StudentListType.Section.ToString(), StaffModel.SubjectArea.ELA.ToString(), StaffModel.AssessmentSubType.StateStandardized.ToString())));
            Assert.That(assessmentDetailsView.ChildViews[0].ChildViews[1].Link.Href, Is.EqualTo(staffAreaLinksFake.AssessmentDetails(providedSchoolId, providedStaffUSI1, providedFullName, providedTeacherSection1, StudentListType.Section.ToString(), StaffModel.SubjectArea.Writing.ToString(), StaffModel.AssessmentSubType.StateStandardized.ToString())));
            Assert.That(assessmentDetailsView.ChildViews[0].ChildViews[2].Link.Href, Is.EqualTo(staffAreaLinksFake.AssessmentDetails(providedSchoolId, providedStaffUSI1, providedFullName, providedTeacherSection1, StudentListType.Section.ToString(), StaffModel.SubjectArea.Mathematics.ToString(), StaffModel.AssessmentSubType.StateStandardized.ToString())));
            Assert.That(assessmentDetailsView.ChildViews[0].ChildViews[3].Link.Href, Is.EqualTo(staffAreaLinksFake.AssessmentDetails(providedSchoolId, providedStaffUSI1, providedFullName, providedTeacherSection1, StudentListType.Section.ToString(), StaffModel.SubjectArea.Science.ToString(), StaffModel.AssessmentSubType.StateStandardized.ToString())));
            Assert.That(assessmentDetailsView.ChildViews[0].ChildViews[4].Link.Href, Is.EqualTo(staffAreaLinksFake.AssessmentDetails(providedSchoolId, providedStaffUSI1, providedFullName, providedTeacherSection1, StudentListType.Section.ToString(), StaffModel.SubjectArea.SocialStudies.ToString(), StaffModel.AssessmentSubType.StateStandardized.ToString())));

            Assert.That(assessmentDetailsView.ChildViews[1].Link.Href, Is.EqualTo(staffAreaLinksFake.AssessmentDetails(providedSchoolId, providedStaffUSI1, providedFullName, providedTeacherSection1, StudentListType.Section.ToString(), null, StaffModel.AssessmentSubType.Benchmark.ToString())));
            Assert.That(assessmentDetailsView.ChildViews[1].ChildViews[0].Link.Href, Is.EqualTo(staffAreaLinksFake.AssessmentDetails(providedSchoolId, providedStaffUSI1, providedFullName, providedTeacherSection1, StudentListType.Section.ToString(), StaffModel.SubjectArea.ELA.ToString(), StaffModel.AssessmentSubType.Benchmark.ToString())));
            Assert.That(assessmentDetailsView.ChildViews[1].ChildViews[1].Link.Href, Is.EqualTo(staffAreaLinksFake.AssessmentDetails(providedSchoolId, providedStaffUSI1, providedFullName, providedTeacherSection1, StudentListType.Section.ToString(), StaffModel.SubjectArea.Writing.ToString(), StaffModel.AssessmentSubType.Benchmark.ToString())));
            Assert.That(assessmentDetailsView.ChildViews[1].ChildViews[2].Link.Href, Is.EqualTo(staffAreaLinksFake.AssessmentDetails(providedSchoolId, providedStaffUSI1, providedFullName, providedTeacherSection1, StudentListType.Section.ToString(), StaffModel.SubjectArea.Mathematics.ToString(), StaffModel.AssessmentSubType.Benchmark.ToString())));
            Assert.That(assessmentDetailsView.ChildViews[1].ChildViews[3].Link.Href, Is.EqualTo(staffAreaLinksFake.AssessmentDetails(providedSchoolId, providedStaffUSI1, providedFullName, providedTeacherSection1, StudentListType.Section.ToString(), StaffModel.SubjectArea.Science.ToString(), StaffModel.AssessmentSubType.Benchmark.ToString())));
            Assert.That(assessmentDetailsView.ChildViews[1].ChildViews[4].Link.Href, Is.EqualTo(staffAreaLinksFake.AssessmentDetails(providedSchoolId, providedStaffUSI1, providedFullName, providedTeacherSection1, StudentListType.Section.ToString(), StaffModel.SubjectArea.SocialStudies.ToString(), StaffModel.AssessmentSubType.Benchmark.ToString())));

            //validate if the link is selected by default
            Assert.That(generalOverviewView.Selected, Is.True);
            Assert.That(priorYearView.Selected, Is.False);
            Assert.That(subjectSpecificView.Selected, Is.False);
            Assert.That(assessmentDetailsView.Selected, Is.False);

            Assert.That(assessmentDetailsView.ChildViews[0].Selected, Is.False);
            Assert.That(assessmentDetailsView.ChildViews[0].ChildViews[0].Selected, Is.False);
            Assert.That(assessmentDetailsView.ChildViews[0].ChildViews[1].Selected, Is.False);
            Assert.That(assessmentDetailsView.ChildViews[0].ChildViews[2].Selected, Is.False);
            Assert.That(assessmentDetailsView.ChildViews[0].ChildViews[3].Selected, Is.False);
            Assert.That(assessmentDetailsView.ChildViews[0].ChildViews[4].Selected, Is.False);

            Assert.That(assessmentDetailsView.ChildViews[1].Selected, Is.False);
            Assert.That(assessmentDetailsView.ChildViews[1].ChildViews[0].Selected, Is.False);
            Assert.That(assessmentDetailsView.ChildViews[1].ChildViews[1].Selected, Is.False);
            Assert.That(assessmentDetailsView.ChildViews[1].ChildViews[2].Selected, Is.False);
            Assert.That(assessmentDetailsView.ChildViews[1].ChildViews[3].Selected, Is.False);
            Assert.That(assessmentDetailsView.ChildViews[1].ChildViews[4].Selected, Is.False);
        }

        [Test]
        public virtual void Should_have_serializable_model()
        {
            actualModel.EnsureSerializableModel();
        }
    }

    [TestFixture]
    public class When_teacher_views_their_staff_information : StaffServiceFixture
    {
        protected override void EstablishContext()
        {
            providedStaffUSI = providedStaffUSI1;
            providedUserName = "tteacher";
            providedUserInformation = new UserInformation
                                          {
                                              StaffUSI = providedStaffUSI,
                                              FullName = providedFullName,
                                              AssociatedOrganizations = new List<UserInformation.EducationOrganization>
                                                                            {
                                                                                new UserInformation.School(providedLocalEducationAgencyId, providedSchoolId)
                                                                                { 
                                                                                    Name = providedSchoolName,
                                                                                },
                                                                                new UserInformation.School(providedLocalEducationAgencyId, providedSchoolId + 1)
                                                                                { 
                                                                                    Name = providedSchoolName1,
                                                                                },
                                                                                new UserInformation.School(providedLocalEducationAgencyId, providedSchoolId + 2)
                                                                                { 
                                                                                    Name = providedSchoolName2,
                                                                                },
                                                                                new UserInformation.School(providedLocalEducationAgencyId, providedSchoolId + 3)
                                                                                { 
                                                                                    Name = providedSchoolName3,
                                                                                },
                                                                            }
                                          };
            base.EstablishContext();
        }
    
        [Test]
        public virtual void Should_load_all_schools()
        {
            Assert.That(actualModel.Schools, Is.Not.Null);
            Assert.That(actualModel.Schools.Count, Is.EqualTo(4));
            Assert.That(actualModel.Schools[0].SchoolId, Is.EqualTo(providedSchoolId + 3));
            Assert.That(actualModel.Schools[0].Name, Is.EqualTo(providedSchoolName3));
            Assert.That(actualModel.Schools[0].Selected, Is.False);
            Assert.That(actualModel.Schools[0].Link.Href, Is.EqualTo(staffAreaLinksFake.GeneralOverview(providedSchoolId + 3, providedStaffUSI1, providedFullName)));
            Assert.That(actualModel.Schools[1].SchoolId, Is.EqualTo(providedSchoolId));
            Assert.That(actualModel.Schools[1].Selected, Is.True);
            Assert.That(actualModel.Schools[1].Link.Href, Is.EqualTo(staffAreaLinksFake.GeneralOverview(providedSchoolId, providedStaffUSI1, providedFullName)));
            Assert.That(actualModel.Schools[2].SchoolId, Is.EqualTo(providedSchoolId + 2));
            Assert.That(actualModel.Schools[2].Selected, Is.False);
            Assert.That(actualModel.Schools[2].Link.Href, Is.EqualTo(staffAreaLinksFake.GeneralOverview(providedSchoolId + 2, providedStaffUSI1, providedFullName)));
            Assert.That(actualModel.Schools[3].SchoolId, Is.EqualTo(providedSchoolId + 1));
            Assert.That(actualModel.Schools[3].Selected, Is.False);
            Assert.That(actualModel.Schools[3].Link.Href, Is.EqualTo(staffAreaLinksFake.GeneralOverview(providedSchoolId + 1, providedStaffUSI1, providedFullName)));
        }
    }

    [TestFixture]
    public class When_teacher_views_their_subject_specific_list : TestFixtureBase
    {
        protected IRepository<StaffCohort> staffCohortRepository;
        protected IRepository<StaffCustomStudentList> applicationStaffCohortRepository;
        protected IRepository<StaffInformation> staffInfoRepository;
        protected IRepository<TeacherSection> teacherSectionRepository;
        protected IRepository<SchoolInformation> schoolInformationRepository;
        protected IRepository<MetricBasedWatchList> WatchListRepository;
        protected IRepository<MetricBasedWatchListOption> WatchListOptionRepository;
        protected ISessionStateProvider sessionStateProvider;
        protected short providedSchoolYear = 2011;
        protected int providedStaffUSI1 = 100;
        protected string providedStaffImage = "402.jpg";
        protected string providedStaffGender = "Male";
        protected string providedThumbnail = "imagethumb.png";
        protected int providedLocalEducationAgencyId = 1000;
        protected int providedSchoolId = 2000;
        protected string providedSchoolName = "elephant high school";
        protected string providedSchoolName1 = "xylophone";
        protected string providedSchoolName2 = "test school name";
        protected string providedSchoolName3 = "apple";
        protected string providedSchoolName4 = "The School Name ";
        protected string providedFullName = "Tom X. Teacher";
        protected string providedCohortDescription1 = "cohort Description 1";
        protected string providedCohortDescription2 = "cohort Description 2";
        protected int providedCohortId1 = 3000;
        protected int providedCohortId2 = 3001;
        protected int providedTeacherSection1 = 4000;
        protected int providedTeacherSection2 = 4001;
        protected int providedStaffUSI;
        protected string providedUserName;
        protected UserInformation providedUserInformation;

        protected string suppliedViewType = String.Empty;
        protected int suppliedSectionId;
        protected string suppliedStudentListType = String.Empty;

        protected StaffModel actualModel;
        private IStaffAreaLinks staffAreaLinksFake = new StaffAreaLinksFake();
        protected IStaffViewProvider staffViewProvider = new StaffViewProvider();
        private ICurrentUserClaimInterrogator currentUserClaimInterrogator;
        protected IWatchListLinkProvider watchListLinkProvider;

        protected override void EstablishContext()
        {
            providedStaffUSI = providedStaffUSI1;
            providedUserName = "tteacher";
            providedUserInformation = new UserInformation
            {
                StaffUSI = providedStaffUSI,
                FullName = providedFullName,
                AssociatedOrganizations = new List<UserInformation.EducationOrganization>
                                                                            {
                                                                                new UserInformation.School(providedLocalEducationAgencyId, providedSchoolId)
                                                                                { 
                                                                                    Name = providedSchoolName, 
                                                                                },
                                                                                new UserInformation.School(providedLocalEducationAgencyId, providedSchoolId + 1)
                                                                                { 
                                                                                    Name = providedSchoolName1,
                                                                                },
                                                                                new UserInformation.School(providedLocalEducationAgencyId, providedSchoolId + 2)
                                                                                { 
                                                                                    Name = providedSchoolName2,
                                                                                },
                                                                                new UserInformation.School(providedLocalEducationAgencyId, providedSchoolId + 3)
                                                                                { 
                                                                                    Name = providedSchoolName3,
                                                                                },
                                                                            }
            };

            suppliedViewType = StaffModel.ViewType.SubjectSpecificOverview.ToString();

            staffCohortRepository = mocks.StrictMock<IRepository<StaffCohort>>();
            Expect.Call(staffCohortRepository.GetAll()).Return(GetStaffCohort());
            
            staffInfoRepository = mocks.StrictMock<IRepository<StaffInformation>>();
            Expect.Call(staffInfoRepository.GetAll()).Return(GetStaffInformation());

            teacherSectionRepository = mocks.StrictMock<IRepository<TeacherSection>>();
            Expect.Call(teacherSectionRepository.GetAll()).Repeat.Any().Return(GetTeacherSection());

            schoolInformationRepository = mocks.StrictMock<IRepository<SchoolInformation>>();
            Expect.Call(schoolInformationRepository.GetAll()).Repeat.Any().Return(GetSchoolInformation());

            WatchListRepository = mocks.StrictMock<IRepository<MetricBasedWatchList>>();
            Expect.Call(WatchListRepository.GetAll()).Repeat.Any().Return(GetWatchListInformation());

            WatchListOptionRepository = mocks.StrictMock<IRepository<MetricBasedWatchListOption>>();
            Expect.Call(WatchListOptionRepository.GetAll()).Repeat.Any().Return(GetWatchListOptionInformation());

            currentUserClaimInterrogator = mocks.StrictMock<ICurrentUserClaimInterrogator>();
            Expect.Call(currentUserClaimInterrogator.HasClaimWithinEducationOrganizationHierarchy(String.Empty, 0)).
                IgnoreArguments().Repeat.Any().Return(true);

            LoginUser(providedUserInformation);

            base.EstablishContext();
        }

        private IQueryable<MetricBasedWatchList> GetWatchListInformation()
        {
            var data = new List<MetricBasedWatchList>
            {
                new MetricBasedWatchList { EducationOrganizationId = providedSchoolId, MetricBasedWatchListId = 1, StaffUSI = providedStaffUSI, WatchListName = "Test Watch List Name" }
            };

            return data.AsQueryable();
        }

        private IQueryable<MetricBasedWatchListOption> GetWatchListOptionInformation()
        {
            var data = new List<MetricBasedWatchListOption>
            {
                new MetricBasedWatchListOption { MetricBasedWatchListOptionId = 1, MetricBasedWatchListId = 1, Name = "Test Name", Value = "Test Value" }
            };

            return data.AsQueryable();
        }

        protected void LoginUser(UserInformation userInformation)
        {
            // Create and attach a claims based principal to the current thread
            Thread.CurrentPrincipal = userInformation.ToClaimsPrincipal();
        }

        protected IQueryable<StaffCohort> GetStaffCohort()
        {
            var data = new List<StaffCohort>
                           {
                               new StaffCohort {StaffUSI = providedStaffUSI1 + 1, EducationOrganizationId = providedSchoolId, StaffCohortId = providedCohortId1 + 10, CohortDescription = "wrong staff" },
                               new StaffCohort {StaffUSI = providedStaffUSI1, EducationOrganizationId = providedSchoolId, StaffCohortId = providedCohortId1, CohortDescription = providedCohortDescription1, AcademicSubjectType = "Science" },
                               new StaffCohort {StaffUSI = providedStaffUSI1, EducationOrganizationId = providedSchoolId, StaffCohortId = providedCohortId2, CohortDescription = providedCohortDescription2, AcademicSubjectType = "Social Studies" },
                               new StaffCohort {StaffUSI = providedStaffUSI1, EducationOrganizationId = providedSchoolId+1, StaffCohortId = providedCohortId1 + 11, CohortDescription = "wrong school" },
                               new StaffCohort {StaffUSI = providedStaffUSI1, EducationOrganizationId = providedSchoolId+2, StaffCohortId = providedCohortId1 + 12, CohortDescription = "wrong school" }
                           };
            return data.AsQueryable();
        }

        protected IQueryable<StaffInformation> GetStaffInformation()
        {
            var data = new List<StaffInformation>
                        {
                            new StaffInformation {StaffUSI = providedStaffUSI1-2, FullName = "Not my name"},
                            new StaffInformation {StaffUSI = providedStaffUSI1-1, FullName = "still wrong"},
                            new StaffInformation {StaffUSI = providedStaffUSI1, FullName = providedFullName, Gender = providedStaffGender, ProfileThumbnail = providedStaffImage},
                            new StaffInformation {StaffUSI = providedStaffUSI1+1, FullName = "try again"}
                        };
            return data.AsQueryable();
        }

        protected IQueryable<TeacherSection> GetTeacherSection()
        {
            var data = new List<TeacherSection>
                           {
                                new TeacherSection {StaffUSI = providedStaffUSI1+1, SchoolId = providedSchoolId, TeacherSectionId=providedTeacherSection1 + 10, SubjectArea="wrong staff", ClassPeriod="class period", CourseTitle="course title", GradeLevel="grade level", LocalCourseCode="local course code", TermType="term type"},
                                new TeacherSection {StaffUSI = providedStaffUSI1, SchoolId = providedSchoolId, TeacherSectionId=providedTeacherSection1, SubjectArea="subject area", ClassPeriod="class period", CourseTitle="course title", GradeLevel="grade level", LocalCourseCode="local course code", TermType="term type"},
                                new TeacherSection {StaffUSI = providedStaffUSI1, SchoolId = providedSchoolId, TeacherSectionId=providedTeacherSection2, SubjectArea="zebra", ClassPeriod="class period", CourseTitle="course title", GradeLevel="grade level", LocalCourseCode="local course code", TermType="term type"},
                                new TeacherSection {StaffUSI = providedStaffUSI1, SchoolId = providedSchoolId+1, TeacherSectionId=providedTeacherSection1 + 11, SubjectArea="wrong school", ClassPeriod="class period", CourseTitle="course title", GradeLevel="grade level", LocalCourseCode="local course code", TermType="term type"},
                                new TeacherSection {StaffUSI = providedStaffUSI1, SchoolId = providedSchoolId+3, TeacherSectionId=providedTeacherSection1 + 12, SubjectArea="wrong school", ClassPeriod="class period", CourseTitle="course title", GradeLevel="grade level", LocalCourseCode="local course code", TermType="term type"},
                           };
            return data.AsQueryable();
        }

        protected IQueryable<SchoolInformation> GetSchoolInformation()
        {
            var data = new List<SchoolInformation>
                           {
                               new SchoolInformation {SchoolId = providedSchoolId, Name = providedSchoolName},
                               new SchoolInformation {SchoolId = providedSchoolId + 1, Name = providedSchoolName1},
                               new SchoolInformation {SchoolId = providedSchoolId + 2, Name = providedSchoolName2},
                               new SchoolInformation {SchoolId = providedSchoolId + 3, Name = providedSchoolName3},
                               new SchoolInformation {SchoolId = providedSchoolId + 4, Name = providedSchoolName4},
                           };
            return data.AsQueryable();
        }

        protected override void ExecuteTest()
        {
            var staffService = new StaffService(staffCohortRepository, staffInfoRepository, teacherSectionRepository, schoolInformationRepository, staffAreaLinksFake, applicationStaffCohortRepository, WatchListRepository, WatchListOptionRepository, currentUserClaimInterrogator, staffViewProvider, watchListLinkProvider);
            actualModel = staffService.Get(new StaffRequest
                                               {
                                                   StaffUSI = providedStaffUSI1,
                                                   SchoolId = providedSchoolId,
                                                   StudentListType = suppliedStudentListType,
                                                   SectionOrCohortId = suppliedSectionId,
                                                   ViewType = suppliedViewType
                                               });
        }
    

        [Test]
        public void Should_load_sections_and_cohorts()
        {
            {
                Assert.That(actualModel.SectionsAndCohorts, Is.Not.Null);
                Assert.That(actualModel.SectionsAndCohorts.Count, Is.EqualTo(2));
                Assert.That(actualModel.SectionsAndCohorts[0].SectionId, Is.EqualTo(providedTeacherSection1));
                Assert.That(actualModel.SectionsAndCohorts[0].Description, Is.EqualTo("subject area(local course code) - course title (class period) term type"));
                Assert.That(actualModel.SectionsAndCohorts[0].Link.Href, Is.EqualTo(staffAreaLinksFake.SubjectSpecificOverview(providedSchoolId, providedStaffUSI1, providedFullName, providedTeacherSection1, StudentListType.Section.ToString())));
                Assert.That(actualModel.SectionsAndCohorts[0].Selected, Is.True);
            }
        }

        [Test]
        public void Should_load_all_schools()
        {
            Assert.That(actualModel.Schools, Is.Not.Null);
            Assert.That(actualModel.Schools.Count, Is.EqualTo(4));
            Assert.That(actualModel.Schools[0].SchoolId, Is.EqualTo(providedSchoolId + 3));
            Assert.That(actualModel.Schools[0].Name, Is.EqualTo(providedSchoolName3));
            Assert.That(actualModel.Schools[0].Selected, Is.False);
            Assert.That(actualModel.Schools[0].Link.Href, Is.EqualTo(staffAreaLinksFake.SubjectSpecificOverview(providedSchoolId + 3, providedStaffUSI1, providedFullName)));
            Assert.That(actualModel.Schools[1].SchoolId, Is.EqualTo(providedSchoolId));
            Assert.That(actualModel.Schools[1].Selected, Is.True);
            Assert.That(actualModel.Schools[1].Link.Href, Is.EqualTo(staffAreaLinksFake.SubjectSpecificOverview(providedSchoolId, providedStaffUSI1, providedFullName)));
            Assert.That(actualModel.Schools[2].SchoolId, Is.EqualTo(providedSchoolId + 2));
            Assert.That(actualModel.Schools[2].Selected, Is.False);
            Assert.That(actualModel.Schools[2].Link.Href, Is.EqualTo(staffAreaLinksFake.SubjectSpecificOverview(providedSchoolId + 2, providedStaffUSI1, providedFullName)));
            Assert.That(actualModel.Schools[3].SchoolId, Is.EqualTo(providedSchoolId + 1));
            Assert.That(actualModel.Schools[3].Selected, Is.False);
            Assert.That(actualModel.Schools[3].Link.Href, Is.EqualTo(staffAreaLinksFake.SubjectSpecificOverview(providedSchoolId + 1, providedStaffUSI1, providedFullName)));
        }

        [Test]
        public void Should_load_all_views()
        {
            Assert.That(actualModel.Views, Is.Not.Null);
            Assert.That(actualModel.Views.Count, Is.EqualTo(4));

            var generalOverviewView = actualModel.Views[0];
            var priorYearView = actualModel.Views[1];
            var subjectSpecificView = actualModel.Views[2];
            var assessmentDetailsView = actualModel.Views[3];

            //Validate the description
            Assert.That(generalOverviewView.Description, Is.EqualTo("General Overview"));
            Assert.That(priorYearView.Description, Is.EqualTo("Prior Year"));
            Assert.That(subjectSpecificView.Description, Is.EqualTo("Subject Specific"));
            Assert.That(assessmentDetailsView.Description, Is.EqualTo("Assessment Details"));

            Assert.That(generalOverviewView.ChildViews.Count, Is.EqualTo(0));
            Assert.That(priorYearView.ChildViews.Count, Is.EqualTo(0));
            Assert.That(subjectSpecificView.ChildViews.Count, Is.EqualTo(0));
            Assert.That(assessmentDetailsView.ChildViews.Count, Is.EqualTo(2));

            Assert.That(assessmentDetailsView.ChildViews[0].Description, Is.EqualTo("State Standardized"));
            Assert.That(assessmentDetailsView.ChildViews[0].ChildViews.Count, Is.EqualTo(5));
            Assert.That(assessmentDetailsView.ChildViews[0].ChildViews[0].Description, Is.EqualTo("- ELA / Reading (State)"));
            Assert.That(assessmentDetailsView.ChildViews[0].ChildViews[1].Description, Is.EqualTo("- Writing (State)"));
            Assert.That(assessmentDetailsView.ChildViews[0].ChildViews[2].Description, Is.EqualTo("- Mathematics (State)"));
            Assert.That(assessmentDetailsView.ChildViews[0].ChildViews[3].Description, Is.EqualTo("- Science (State)"));
            Assert.That(assessmentDetailsView.ChildViews[0].ChildViews[4].Description, Is.EqualTo("- Social Studies (State)"));

            Assert.That(assessmentDetailsView.ChildViews[1].Description, Is.EqualTo("District Benchmark"));
            Assert.That(assessmentDetailsView.ChildViews[1].ChildViews.Count, Is.EqualTo(5));
            Assert.That(assessmentDetailsView.ChildViews[1].ChildViews[0].Description, Is.EqualTo("- ELA / Reading (District)"));
            Assert.That(assessmentDetailsView.ChildViews[1].ChildViews[1].Description, Is.EqualTo("- Writing (District)"));
            Assert.That(assessmentDetailsView.ChildViews[1].ChildViews[2].Description, Is.EqualTo("- Mathematics (District)"));
            Assert.That(assessmentDetailsView.ChildViews[1].ChildViews[3].Description, Is.EqualTo("- Science (District)"));
            Assert.That(assessmentDetailsView.ChildViews[1].ChildViews[4].Description, Is.EqualTo("- Social Studies (District)"));

            //validate the link
            Assert.That(generalOverviewView.Link.Href, Is.EqualTo(staffAreaLinksFake.GeneralOverview(providedSchoolId, providedStaffUSI1, providedFullName, providedTeacherSection1, StudentListType.Section.ToString())));
            Assert.That(priorYearView.Link.Href, Is.EqualTo(staffAreaLinksFake.PriorYear(providedSchoolId, providedStaffUSI1, providedFullName, providedTeacherSection1, StudentListType.Section.ToString())));
            Assert.That(subjectSpecificView.Link.Href, Is.EqualTo(staffAreaLinksFake.SubjectSpecificOverview(providedSchoolId, providedStaffUSI1, providedFullName, providedTeacherSection1, StudentListType.Section.ToString())));
            Assert.That(assessmentDetailsView.Link.Href, Is.EqualTo(staffAreaLinksFake.AssessmentDetails(providedSchoolId, providedStaffUSI1, providedFullName, providedTeacherSection1, StudentListType.Section.ToString(), null, StaffModel.AssessmentSubType.StateStandardized.ToString())));

            Assert.That(assessmentDetailsView.ChildViews[0].Link.Href, Is.EqualTo(staffAreaLinksFake.AssessmentDetails(providedSchoolId, providedStaffUSI1, providedFullName, providedTeacherSection1, StudentListType.Section.ToString(), null, StaffModel.AssessmentSubType.StateStandardized.ToString())));
            Assert.That(assessmentDetailsView.ChildViews[0].ChildViews[0].Link.Href, Is.EqualTo(staffAreaLinksFake.AssessmentDetails(providedSchoolId, providedStaffUSI1, providedFullName, providedTeacherSection1, StudentListType.Section.ToString(), StaffModel.SubjectArea.ELA.ToString(), StaffModel.AssessmentSubType.StateStandardized.ToString())));
            Assert.That(assessmentDetailsView.ChildViews[0].ChildViews[1].Link.Href, Is.EqualTo(staffAreaLinksFake.AssessmentDetails(providedSchoolId, providedStaffUSI1, providedFullName, providedTeacherSection1, StudentListType.Section.ToString(), StaffModel.SubjectArea.Writing.ToString(), StaffModel.AssessmentSubType.StateStandardized.ToString())));
            Assert.That(assessmentDetailsView.ChildViews[0].ChildViews[2].Link.Href, Is.EqualTo(staffAreaLinksFake.AssessmentDetails(providedSchoolId, providedStaffUSI1, providedFullName, providedTeacherSection1, StudentListType.Section.ToString(), StaffModel.SubjectArea.Mathematics.ToString(), StaffModel.AssessmentSubType.StateStandardized.ToString())));
            Assert.That(assessmentDetailsView.ChildViews[0].ChildViews[3].Link.Href, Is.EqualTo(staffAreaLinksFake.AssessmentDetails(providedSchoolId, providedStaffUSI1, providedFullName, providedTeacherSection1, StudentListType.Section.ToString(), StaffModel.SubjectArea.Science.ToString(), StaffModel.AssessmentSubType.StateStandardized.ToString())));
            Assert.That(assessmentDetailsView.ChildViews[0].ChildViews[4].Link.Href, Is.EqualTo(staffAreaLinksFake.AssessmentDetails(providedSchoolId, providedStaffUSI1, providedFullName, providedTeacherSection1, StudentListType.Section.ToString(), StaffModel.SubjectArea.SocialStudies.ToString(), StaffModel.AssessmentSubType.StateStandardized.ToString())));

            Assert.That(assessmentDetailsView.ChildViews[1].Link.Href, Is.EqualTo(staffAreaLinksFake.AssessmentDetails(providedSchoolId, providedStaffUSI1, providedFullName, providedTeacherSection1, StudentListType.Section.ToString(), null, StaffModel.AssessmentSubType.Benchmark.ToString())));
            Assert.That(assessmentDetailsView.ChildViews[1].ChildViews[0].Link.Href, Is.EqualTo(staffAreaLinksFake.AssessmentDetails(providedSchoolId, providedStaffUSI1, providedFullName, providedTeacherSection1, StudentListType.Section.ToString(), StaffModel.SubjectArea.ELA.ToString(), StaffModel.AssessmentSubType.Benchmark.ToString())));
            Assert.That(assessmentDetailsView.ChildViews[1].ChildViews[1].Link.Href, Is.EqualTo(staffAreaLinksFake.AssessmentDetails(providedSchoolId, providedStaffUSI1, providedFullName, providedTeacherSection1, StudentListType.Section.ToString(), StaffModel.SubjectArea.Writing.ToString(), StaffModel.AssessmentSubType.Benchmark.ToString())));
            Assert.That(assessmentDetailsView.ChildViews[1].ChildViews[2].Link.Href, Is.EqualTo(staffAreaLinksFake.AssessmentDetails(providedSchoolId, providedStaffUSI1, providedFullName, providedTeacherSection1, StudentListType.Section.ToString(), StaffModel.SubjectArea.Mathematics.ToString(), StaffModel.AssessmentSubType.Benchmark.ToString())));
            Assert.That(assessmentDetailsView.ChildViews[1].ChildViews[3].Link.Href, Is.EqualTo(staffAreaLinksFake.AssessmentDetails(providedSchoolId, providedStaffUSI1, providedFullName, providedTeacherSection1, StudentListType.Section.ToString(), StaffModel.SubjectArea.Science.ToString(), StaffModel.AssessmentSubType.Benchmark.ToString())));
            Assert.That(assessmentDetailsView.ChildViews[1].ChildViews[4].Link.Href, Is.EqualTo(staffAreaLinksFake.AssessmentDetails(providedSchoolId, providedStaffUSI1, providedFullName, providedTeacherSection1, StudentListType.Section.ToString(), StaffModel.SubjectArea.SocialStudies.ToString(), StaffModel.AssessmentSubType.Benchmark.ToString())));

            //validate if the link is selected by default
            Assert.That(generalOverviewView.Selected, Is.False);
            Assert.That(priorYearView.Selected, Is.False);
            Assert.That(subjectSpecificView.Selected, Is.True);
            Assert.That(assessmentDetailsView.Selected, Is.False);

            Assert.That(assessmentDetailsView.ChildViews[0].Selected, Is.False);
            Assert.That(assessmentDetailsView.ChildViews[0].ChildViews[0].Selected, Is.False);
            Assert.That(assessmentDetailsView.ChildViews[0].ChildViews[1].Selected, Is.False);
            Assert.That(assessmentDetailsView.ChildViews[0].ChildViews[2].Selected, Is.False);
            Assert.That(assessmentDetailsView.ChildViews[0].ChildViews[3].Selected, Is.False);
            Assert.That(assessmentDetailsView.ChildViews[0].ChildViews[4].Selected, Is.False);

            Assert.That(assessmentDetailsView.ChildViews[1].Selected, Is.False);
            Assert.That(assessmentDetailsView.ChildViews[1].ChildViews[0].Selected, Is.False);
            Assert.That(assessmentDetailsView.ChildViews[1].ChildViews[1].Selected, Is.False);
            Assert.That(assessmentDetailsView.ChildViews[1].ChildViews[2].Selected, Is.False);
            Assert.That(assessmentDetailsView.ChildViews[1].ChildViews[3].Selected, Is.False);
            Assert.That(assessmentDetailsView.ChildViews[1].ChildViews[4].Selected, Is.False);
        }
    }

    [TestFixture]
    public class When_teacher_views_their_assessment_detail_list : When_teacher_views_their_staff_information
    {
        protected override void EstablishContext()
        {
            suppliedViewType = StaffModel.ViewType.AssessmentDetails.ToString();
            suppliedSubjectArea = StaffModel.SubjectArea.Science.ToString();
            suppliedAssessmentSubType = StaffModel.AssessmentSubType.StateStandardized.ToString();
            base.EstablishContext();
        }

        [Test]
        public override void Should_load_sections_and_cohorts()
        {
            {
                Assert.That(actualModel.SectionsAndCohorts, Is.Not.Null);
                Assert.That(
                    actualModel.SectionsAndCohorts.Where(
                        x =>
                        x.ListType == StudentListType.Cohort ||
                        x.ListType == StudentListType.Section || x.ListType == StudentListType.All)
                        .Count(), Is.EqualTo(5));
                Assert.That(actualModel.SectionsAndCohorts[0].SectionId, Is.EqualTo(0));
                Assert.That(actualModel.SectionsAndCohorts[0].Description, Is.EqualTo("Students From All Sections"));
                Assert.That(actualModel.SectionsAndCohorts[0].Link.Href,
                            Is.EqualTo(staffAreaLinksFake.AssessmentDetails(providedSchoolId, providedStaffUSI1,
                                                                            providedFullName, null as int?,
                                                                            StudentListType.All.ToString(), suppliedSubjectArea, suppliedAssessmentSubType)));
                Assert.That(actualModel.SectionsAndCohorts[0].Selected, Is.False);
                Assert.That(actualModel.SectionsAndCohorts[1].SectionId, Is.EqualTo(providedTeacherSection1));
                Assert.That(actualModel.SectionsAndCohorts[1].Description,
                            Is.EqualTo("Mathematics(local course code) - course title (class period) term type"));
                Assert.That(actualModel.SectionsAndCohorts[1].Link.Href,
                            Is.EqualTo(staffAreaLinksFake.AssessmentDetails(providedSchoolId, providedStaffUSI1,
                                                                            providedFullName, providedTeacherSection1,
                                                                            StudentListType.Section.ToString(), suppliedSubjectArea, suppliedAssessmentSubType)));
                Assert.That(actualModel.SectionsAndCohorts[1].Selected, Is.True);
                Assert.That(actualModel.SectionsAndCohorts[3].SectionId, Is.EqualTo(providedCohortId1));
                Assert.That(actualModel.SectionsAndCohorts[3].Description, Is.EqualTo(providedCohortDescription1));
                Assert.That(actualModel.SectionsAndCohorts[3].Link.Href,
                            Is.EqualTo(staffAreaLinksFake.AssessmentDetails(providedSchoolId, providedStaffUSI1,
                                                                            providedFullName, providedCohortId1,
                                                                            StudentListType.Cohort.ToString(), suppliedSubjectArea, suppliedAssessmentSubType)));
                Assert.That(actualModel.SectionsAndCohorts[3].Selected, Is.False);

                Assert.That(actualModel.SectionsAndCohorts.SelectMany(s => s.ChildSections), Has.All.Property("Link").Not.Null);
            }
        }

        [Test]
        public override void Should_load_all_schools()
        {
            Assert.That(actualModel.Schools, Is.Not.Null);
            Assert.That(actualModel.Schools.Count, Is.EqualTo(4));
            Assert.That(actualModel.Schools[0].SchoolId, Is.EqualTo(providedSchoolId + 3));
            Assert.That(actualModel.Schools[0].Name, Is.EqualTo(providedSchoolName3));
            Assert.That(actualModel.Schools[0].Selected, Is.False);
            Assert.That(actualModel.Schools[0].Link.Href,
                        Is.EqualTo(staffAreaLinksFake.AssessmentDetails(providedSchoolId + 3, providedStaffUSI1,
                                                                        providedFullName)));
            Assert.That(actualModel.Schools[1].SchoolId, Is.EqualTo(providedSchoolId));
            Assert.That(actualModel.Schools[1].Selected, Is.True);
            Assert.That(actualModel.Schools[1].Link.Href,
                        Is.EqualTo(staffAreaLinksFake.AssessmentDetails(providedSchoolId, providedStaffUSI1,
                                                                        providedFullName)));
            Assert.That(actualModel.Schools[2].SchoolId, Is.EqualTo(providedSchoolId + 2));
            Assert.That(actualModel.Schools[2].Selected, Is.False);
            Assert.That(actualModel.Schools[2].Link.Href,
                        Is.EqualTo(staffAreaLinksFake.AssessmentDetails(providedSchoolId + 2, providedStaffUSI1,
                                                                        providedFullName)));
            Assert.That(actualModel.Schools[3].SchoolId, Is.EqualTo(providedSchoolId + 1));
            Assert.That(actualModel.Schools[3].Selected, Is.False);
            Assert.That(actualModel.Schools[3].Link.Href,
                        Is.EqualTo(staffAreaLinksFake.AssessmentDetails(providedSchoolId + 1, providedStaffUSI1,
                                                                        providedFullName)));
        }

        [Test]
        public override void Should_load_all_views()
        {
            Assert.That(actualModel.Views, Is.Not.Null);
            Assert.That(actualModel.Views.Count, Is.EqualTo(4));

            var generalOverviewView = actualModel.Views[0];
            var priorYearView = actualModel.Views[1];
            var subjectSpecificView = actualModel.Views[2];
            var assessmentDetailsView = actualModel.Views[3];

            //Validate the description
            Assert.That(generalOverviewView.Description, Is.EqualTo("General Overview"));
            Assert.That(priorYearView.Description, Is.EqualTo("Prior Year"));
            Assert.That(subjectSpecificView.Description, Is.EqualTo("Subject Specific"));
            Assert.That(assessmentDetailsView.Description, Is.EqualTo("Assessment Details"));

            Assert.That(generalOverviewView.ChildViews.Count, Is.EqualTo(0));
            Assert.That(priorYearView.ChildViews.Count, Is.EqualTo(0));
            Assert.That(subjectSpecificView.ChildViews.Count, Is.EqualTo(0));
            Assert.That(assessmentDetailsView.ChildViews.Count, Is.EqualTo(2));

            Assert.That(assessmentDetailsView.ChildViews[0].Description, Is.EqualTo("State Standardized"));
            Assert.That(assessmentDetailsView.ChildViews[0].ChildViews.Count, Is.EqualTo(5));
            Assert.That(assessmentDetailsView.ChildViews[0].ChildViews[0].Description, Is.EqualTo("- ELA / Reading (State)"));
            Assert.That(assessmentDetailsView.ChildViews[0].ChildViews[1].Description, Is.EqualTo("- Writing (State)"));
            Assert.That(assessmentDetailsView.ChildViews[0].ChildViews[2].Description, Is.EqualTo("- Mathematics (State)"));
            Assert.That(assessmentDetailsView.ChildViews[0].ChildViews[3].Description, Is.EqualTo("- Science (State)"));
            Assert.That(assessmentDetailsView.ChildViews[0].ChildViews[4].Description, Is.EqualTo("- Social Studies (State)"));

            Assert.That(assessmentDetailsView.ChildViews[1].Description, Is.EqualTo("District Benchmark"));
            Assert.That(assessmentDetailsView.ChildViews[1].ChildViews.Count, Is.EqualTo(5));
            Assert.That(assessmentDetailsView.ChildViews[1].ChildViews[0].Description, Is.EqualTo("- ELA / Reading (District)"));
            Assert.That(assessmentDetailsView.ChildViews[1].ChildViews[1].Description, Is.EqualTo("- Writing (District)"));
            Assert.That(assessmentDetailsView.ChildViews[1].ChildViews[2].Description, Is.EqualTo("- Mathematics (District)"));
            Assert.That(assessmentDetailsView.ChildViews[1].ChildViews[3].Description, Is.EqualTo("- Science (District)"));
            Assert.That(assessmentDetailsView.ChildViews[1].ChildViews[4].Description, Is.EqualTo("- Social Studies (District)"));
        }
    }

    [TestFixture]
    public class When_teacher_views_their_default_assessment_detail_list : When_teacher_views_their_staff_information
    {
        protected override void EstablishContext()
        {
            suppliedViewType = StaffModel.ViewType.AssessmentDetails.ToString();
            suppliedSubjectArea = String.Empty;
            base.EstablishContext();
        }

        [Test]
        public override void Should_load_sections_and_cohorts()
        {
            {
                Assert.That(actualModel.SectionsAndCohorts, Is.Not.Null);
                Assert.That(actualModel.SectionsAndCohorts.Where(x => x.ListType == StudentListType.Cohort || x.ListType == StudentListType.Section || x.ListType == StudentListType.All).Count(), Is.EqualTo(5));
                Assert.That(actualModel.SectionsAndCohorts[0].SectionId, Is.EqualTo(0));
                Assert.That(actualModel.SectionsAndCohorts[0].Description, Is.EqualTo("Students From All Sections"));
                Assert.That(actualModel.SectionsAndCohorts[0].Link.Href, Is.EqualTo(staffAreaLinksFake.AssessmentDetails(providedSchoolId, providedStaffUSI1, providedFullName, null as int?, StudentListType.All.ToString())));
                Assert.That(actualModel.SectionsAndCohorts[0].Selected, Is.False);
                Assert.That(actualModel.SectionsAndCohorts[1].SectionId, Is.EqualTo(providedTeacherSection1));
                Assert.That(actualModel.SectionsAndCohorts[1].Description, Is.EqualTo("Mathematics(local course code) - course title (class period) term type"));
                Assert.That(actualModel.SectionsAndCohorts[1].Link.Href, Is.EqualTo(staffAreaLinksFake.AssessmentDetails(providedSchoolId, providedStaffUSI1, providedFullName, providedTeacherSection1, StudentListType.Section.ToString())));
                Assert.That(actualModel.SectionsAndCohorts[1].Selected, Is.True);
                Assert.That(actualModel.SectionsAndCohorts[3].SectionId, Is.EqualTo(providedCohortId1));
                Assert.That(actualModel.SectionsAndCohorts[3].Description, Is.EqualTo(providedCohortDescription1));
                Assert.That(actualModel.SectionsAndCohorts[3].Link.Href, Is.EqualTo(staffAreaLinksFake.AssessmentDetails(providedSchoolId, providedStaffUSI1, providedFullName, providedCohortId1, StudentListType.Cohort.ToString())));
                Assert.That(actualModel.SectionsAndCohorts[3].Selected, Is.False);

                Assert.That(actualModel.SectionsAndCohorts.SelectMany(s => s.ChildSections), Has.All.Property("Link").Not.Null);
            }
        }

        [Test]
        public override void Should_load_all_schools()
        {
            Assert.That(actualModel.Schools, Is.Not.Null);
            Assert.That(actualModel.Schools.Count, Is.EqualTo(4));
            Assert.That(actualModel.Schools[0].SchoolId, Is.EqualTo(providedSchoolId + 3));
            Assert.That(actualModel.Schools[0].Name, Is.EqualTo(providedSchoolName3));
            Assert.That(actualModel.Schools[0].Selected, Is.False);
            Assert.That(actualModel.Schools[0].Link.Href, Is.EqualTo(staffAreaLinksFake.AssessmentDetails(providedSchoolId + 3, providedStaffUSI1, providedFullName)));
            Assert.That(actualModel.Schools[1].SchoolId, Is.EqualTo(providedSchoolId));
            Assert.That(actualModel.Schools[1].Selected, Is.True);
            Assert.That(actualModel.Schools[1].Link.Href, Is.EqualTo(staffAreaLinksFake.AssessmentDetails(providedSchoolId, providedStaffUSI1, providedFullName)));
            Assert.That(actualModel.Schools[2].SchoolId, Is.EqualTo(providedSchoolId + 2));
            Assert.That(actualModel.Schools[2].Selected, Is.False);
            Assert.That(actualModel.Schools[2].Link.Href, Is.EqualTo(staffAreaLinksFake.AssessmentDetails(providedSchoolId + 2, providedStaffUSI1, providedFullName)));
            Assert.That(actualModel.Schools[3].SchoolId, Is.EqualTo(providedSchoolId + 1));
            Assert.That(actualModel.Schools[3].Selected, Is.False);
            Assert.That(actualModel.Schools[3].Link.Href, Is.EqualTo(staffAreaLinksFake.AssessmentDetails(providedSchoolId + 1, providedStaffUSI1, providedFullName)));
        }

        [Test]
        public override void Should_load_all_views()
        {
            Assert.That(actualModel.Views, Is.Not.Null);
            Assert.That(actualModel.Views.Count, Is.EqualTo(4));

            var generalOverviewView = actualModel.Views[0];
            var priorYearView = actualModel.Views[1];
            var subjectSpecificView = actualModel.Views[2];
            var assessmentDetailsView = actualModel.Views[3];

            //validate the link
            Assert.That(generalOverviewView.Link.Href, Is.EqualTo(staffAreaLinksFake.GeneralOverview(providedSchoolId, providedStaffUSI1, providedFullName, providedTeacherSection1, StudentListType.Section.ToString())));
            Assert.That(priorYearView.Link.Href, Is.EqualTo(staffAreaLinksFake.PriorYear(providedSchoolId, providedStaffUSI1, providedFullName, providedTeacherSection1, StudentListType.Section.ToString())));
            Assert.That(subjectSpecificView.Link.Href, Is.EqualTo(staffAreaLinksFake.SubjectSpecificOverview(providedSchoolId, providedStaffUSI1, providedFullName, providedTeacherSection1, StudentListType.Section.ToString())));
            Assert.That(assessmentDetailsView.Link.Href, Is.EqualTo(staffAreaLinksFake.AssessmentDetails(providedSchoolId, providedStaffUSI1, providedFullName, providedTeacherSection1, StudentListType.Section.ToString(), null, StaffModel.AssessmentSubType.StateStandardized.ToString())));

            Assert.That(assessmentDetailsView.ChildViews[0].Link.Href, Is.EqualTo(staffAreaLinksFake.AssessmentDetails(providedSchoolId, providedStaffUSI1, providedFullName, providedTeacherSection1, StudentListType.Section.ToString(), null, StaffModel.AssessmentSubType.StateStandardized.ToString())));
            Assert.That(assessmentDetailsView.ChildViews[0].ChildViews[0].Link.Href, Is.EqualTo(staffAreaLinksFake.AssessmentDetails(providedSchoolId, providedStaffUSI1, providedFullName, providedTeacherSection1, StudentListType.Section.ToString(), StaffModel.SubjectArea.ELA.ToString(), StaffModel.AssessmentSubType.StateStandardized.ToString())));
            Assert.That(assessmentDetailsView.ChildViews[0].ChildViews[1].Link.Href, Is.EqualTo(staffAreaLinksFake.AssessmentDetails(providedSchoolId, providedStaffUSI1, providedFullName, providedTeacherSection1, StudentListType.Section.ToString(), StaffModel.SubjectArea.Writing.ToString(), StaffModel.AssessmentSubType.StateStandardized.ToString())));
            Assert.That(assessmentDetailsView.ChildViews[0].ChildViews[2].Link.Href, Is.EqualTo(staffAreaLinksFake.AssessmentDetails(providedSchoolId, providedStaffUSI1, providedFullName, providedTeacherSection1, StudentListType.Section.ToString(), StaffModel.SubjectArea.Mathematics.ToString(), StaffModel.AssessmentSubType.StateStandardized.ToString())));
            Assert.That(assessmentDetailsView.ChildViews[0].ChildViews[3].Link.Href, Is.EqualTo(staffAreaLinksFake.AssessmentDetails(providedSchoolId, providedStaffUSI1, providedFullName, providedTeacherSection1, StudentListType.Section.ToString(), StaffModel.SubjectArea.Science.ToString(), StaffModel.AssessmentSubType.StateStandardized.ToString())));
            Assert.That(assessmentDetailsView.ChildViews[0].ChildViews[4].Link.Href, Is.EqualTo(staffAreaLinksFake.AssessmentDetails(providedSchoolId, providedStaffUSI1, providedFullName, providedTeacherSection1, StudentListType.Section.ToString(), StaffModel.SubjectArea.SocialStudies.ToString(), StaffModel.AssessmentSubType.StateStandardized.ToString())));

            Assert.That(assessmentDetailsView.ChildViews[1].Link.Href, Is.EqualTo(staffAreaLinksFake.AssessmentDetails(providedSchoolId, providedStaffUSI1, providedFullName, providedTeacherSection1, StudentListType.Section.ToString(), null, StaffModel.AssessmentSubType.Benchmark.ToString())));
            Assert.That(assessmentDetailsView.ChildViews[1].ChildViews[0].Link.Href, Is.EqualTo(staffAreaLinksFake.AssessmentDetails(providedSchoolId, providedStaffUSI1, providedFullName, providedTeacherSection1, StudentListType.Section.ToString(), StaffModel.SubjectArea.ELA.ToString(), StaffModel.AssessmentSubType.Benchmark.ToString())));
            Assert.That(assessmentDetailsView.ChildViews[1].ChildViews[1].Link.Href, Is.EqualTo(staffAreaLinksFake.AssessmentDetails(providedSchoolId, providedStaffUSI1, providedFullName, providedTeacherSection1, StudentListType.Section.ToString(), StaffModel.SubjectArea.Writing.ToString(), StaffModel.AssessmentSubType.Benchmark.ToString())));
            Assert.That(assessmentDetailsView.ChildViews[1].ChildViews[2].Link.Href, Is.EqualTo(staffAreaLinksFake.AssessmentDetails(providedSchoolId, providedStaffUSI1, providedFullName, providedTeacherSection1, StudentListType.Section.ToString(), StaffModel.SubjectArea.Mathematics.ToString(), StaffModel.AssessmentSubType.Benchmark.ToString())));
            Assert.That(assessmentDetailsView.ChildViews[1].ChildViews[3].Link.Href, Is.EqualTo(staffAreaLinksFake.AssessmentDetails(providedSchoolId, providedStaffUSI1, providedFullName, providedTeacherSection1, StudentListType.Section.ToString(), StaffModel.SubjectArea.Science.ToString(), StaffModel.AssessmentSubType.Benchmark.ToString())));
            Assert.That(assessmentDetailsView.ChildViews[1].ChildViews[4].Link.Href, Is.EqualTo(staffAreaLinksFake.AssessmentDetails(providedSchoolId, providedStaffUSI1, providedFullName, providedTeacherSection1, StudentListType.Section.ToString(), StaffModel.SubjectArea.SocialStudies.ToString(), StaffModel.AssessmentSubType.Benchmark.ToString())));

            //validate if the link is selected by default
            Assert.That(generalOverviewView.Selected, Is.False);
            Assert.That(priorYearView.Selected, Is.False);
            Assert.That(subjectSpecificView.Selected, Is.False);
            Assert.That(assessmentDetailsView.Selected, Is.False);

            Assert.That(assessmentDetailsView.ChildViews[0].Selected, Is.False);
            Assert.That(assessmentDetailsView.ChildViews[0].ChildViews[0].Selected, Is.False);
            Assert.That(assessmentDetailsView.ChildViews[0].ChildViews[1].Selected, Is.False);
            Assert.That(assessmentDetailsView.ChildViews[0].ChildViews[2].Selected, Is.False);
            Assert.That(assessmentDetailsView.ChildViews[0].ChildViews[3].Selected, Is.False);
            Assert.That(assessmentDetailsView.ChildViews[0].ChildViews[4].Selected, Is.False);

            Assert.That(assessmentDetailsView.ChildViews[1].Selected, Is.False);
            Assert.That(assessmentDetailsView.ChildViews[1].ChildViews[0].Selected, Is.False);
            Assert.That(assessmentDetailsView.ChildViews[1].ChildViews[1].Selected, Is.False);
            Assert.That(assessmentDetailsView.ChildViews[1].ChildViews[2].Selected, Is.False);
            Assert.That(assessmentDetailsView.ChildViews[1].ChildViews[3].Selected, Is.False);
            Assert.That(assessmentDetailsView.ChildViews[1].ChildViews[4].Selected, Is.False);
        }
    }

    [TestFixture]
    public class When_teacher_views_their_cohort : When_teacher_views_their_staff_information
    {
        protected override void EstablishContext()
        {
            suppliedViewType = StaffModel.ViewType.AssessmentDetails.ToString();
            suppliedSubjectArea = StaffModel.SubjectArea.Mathematics.ToString();
            suppliedAssessmentSubType = StaffModel.AssessmentSubType.Benchmark.ToString();
            suppliedSectionId = providedCohortId2;
            suppliedStudentListType = StudentListType.Cohort.ToString();
            base.EstablishContext();
        }

        [Test]
        public override void Should_load_sections_and_cohorts()
        {
            {
                Assert.That(actualModel.SectionsAndCohorts, Is.Not.Null);
                Assert.That(
                    actualModel.SectionsAndCohorts.Where(
                        x =>
                        x.ListType == StudentListType.Cohort ||
                        x.ListType == StudentListType.Section || x.ListType == StudentListType.All)
                        .Count(), Is.EqualTo(5));
                Assert.That(actualModel.SectionsAndCohorts[0].SectionId, Is.EqualTo(0));
                Assert.That(actualModel.SectionsAndCohorts[0].Description, Is.EqualTo("Students From All Sections"));
                Assert.That(actualModel.SectionsAndCohorts[0].Link.Href,
                            Is.EqualTo(staffAreaLinksFake.AssessmentDetails(providedSchoolId, providedStaffUSI1,
                                                                            providedFullName, null as int?,
                                                                            StudentListType.All.ToString(), suppliedSubjectArea, suppliedAssessmentSubType)));
                Assert.That(actualModel.SectionsAndCohorts[0].Selected, Is.False);
                Assert.That(actualModel.SectionsAndCohorts[1].SectionId, Is.EqualTo(providedTeacherSection1));
                Assert.That(actualModel.SectionsAndCohorts[1].Description,
                            Is.EqualTo("Mathematics(local course code) - course title (class period) term type"));
                Assert.That(actualModel.SectionsAndCohorts[1].Link.Href,
                            Is.EqualTo(staffAreaLinksFake.AssessmentDetails(providedSchoolId, providedStaffUSI1,
                                                                            providedFullName, providedTeacherSection1,
                                                                            StudentListType.Section.ToString(), suppliedSubjectArea, suppliedAssessmentSubType)));
                Assert.That(actualModel.SectionsAndCohorts[1].Selected, Is.False);
                Assert.That(actualModel.SectionsAndCohorts[3].SectionId, Is.EqualTo(providedCohortId1));
                Assert.That(actualModel.SectionsAndCohorts[3].Description, Is.EqualTo(providedCohortDescription1));
                Assert.That(actualModel.SectionsAndCohorts[3].Link.Href,
                            Is.EqualTo(staffAreaLinksFake.AssessmentDetails(providedSchoolId, providedStaffUSI1,
                                                                            providedFullName, providedCohortId1,
                                                                            StudentListType.Cohort.ToString(), suppliedSubjectArea, suppliedAssessmentSubType)));
                Assert.That(actualModel.SectionsAndCohorts[3].Selected, Is.False);
                Assert.That(actualModel.SectionsAndCohorts[4].SectionId, Is.EqualTo(providedCohortId2));
                Assert.That(actualModel.SectionsAndCohorts[4].Description, Is.EqualTo(providedCohortDescription2));
                Assert.That(actualModel.SectionsAndCohorts[4].Link.Href,
                            Is.EqualTo(staffAreaLinksFake.AssessmentDetails(providedSchoolId, providedStaffUSI1,
                                                                            providedFullName, providedCohortId2,
                                                                            StudentListType.Cohort.ToString(), suppliedSubjectArea, suppliedAssessmentSubType)));
                Assert.That(actualModel.SectionsAndCohorts[4].Selected, Is.True);

                Assert.That(actualModel.SectionsAndCohorts.SelectMany(s => s.ChildSections), Has.All.Property("Link").Not.Null);
            }
        }

        [Test]
        public override void Should_load_all_schools()
        {
            Assert.That(actualModel.Schools, Is.Not.Null);
            Assert.That(actualModel.Schools.Count, Is.EqualTo(4));
            Assert.That(actualModel.Schools[0].SchoolId, Is.EqualTo(providedSchoolId + 3));
            Assert.That(actualModel.Schools[0].Name, Is.EqualTo(providedSchoolName3));
            Assert.That(actualModel.Schools[0].Selected, Is.False);
            Assert.That(actualModel.Schools[0].Link.Href,
                        Is.EqualTo(staffAreaLinksFake.AssessmentDetails(providedSchoolId + 3, providedStaffUSI1,
                                                                        providedFullName)));
            Assert.That(actualModel.Schools[1].SchoolId, Is.EqualTo(providedSchoolId));
            Assert.That(actualModel.Schools[1].Selected, Is.True);
            Assert.That(actualModel.Schools[1].Link.Href,
                        Is.EqualTo(staffAreaLinksFake.AssessmentDetails(providedSchoolId, providedStaffUSI1,
                                                                        providedFullName)));
            Assert.That(actualModel.Schools[2].SchoolId, Is.EqualTo(providedSchoolId + 2));
            Assert.That(actualModel.Schools[2].Selected, Is.False);
            Assert.That(actualModel.Schools[2].Link.Href,
                        Is.EqualTo(staffAreaLinksFake.AssessmentDetails(providedSchoolId + 2, providedStaffUSI1,
                                                                        providedFullName)));
            Assert.That(actualModel.Schools[3].SchoolId, Is.EqualTo(providedSchoolId + 1));
            Assert.That(actualModel.Schools[3].Selected, Is.False);
            Assert.That(actualModel.Schools[3].Link.Href,
                        Is.EqualTo(staffAreaLinksFake.AssessmentDetails(providedSchoolId + 1, providedStaffUSI1,
                                                                        providedFullName)));
        }

        [Test]
        public override void Should_load_all_views()
        {
            Assert.That(actualModel.Views, Is.Not.Null);
            Assert.That(actualModel.Views.Count, Is.EqualTo(3));

            var generalOverviewView = actualModel.Views[0];
            var priorYearView = actualModel.Views[1];
            var assessmentDetailsView = actualModel.Views[2];

            //Validate the description
            Assert.That(generalOverviewView.Description, Is.EqualTo("General Overview"));
            Assert.That(priorYearView.Description, Is.EqualTo("Prior Year"));
            Assert.That(assessmentDetailsView.Description, Is.EqualTo("Assessment Details"));

            Assert.That(generalOverviewView.ChildViews.Count, Is.EqualTo(0));
            Assert.That(priorYearView.ChildViews.Count, Is.EqualTo(0));
            Assert.That(assessmentDetailsView.ChildViews.Count, Is.EqualTo(2));

            Assert.That(assessmentDetailsView.ChildViews[0].Description, Is.EqualTo("State Standardized"));
            Assert.That(assessmentDetailsView.ChildViews[0].ChildViews.Count, Is.EqualTo(5));
            Assert.That(assessmentDetailsView.ChildViews[0].ChildViews[0].Description, Is.EqualTo("- ELA / Reading (State)"));
            Assert.That(assessmentDetailsView.ChildViews[0].ChildViews[1].Description, Is.EqualTo("- Writing (State)"));
            Assert.That(assessmentDetailsView.ChildViews[0].ChildViews[2].Description, Is.EqualTo("- Mathematics (State)"));
            Assert.That(assessmentDetailsView.ChildViews[0].ChildViews[3].Description, Is.EqualTo("- Science (State)"));
            Assert.That(assessmentDetailsView.ChildViews[0].ChildViews[4].Description, Is.EqualTo("- Social Studies (State)"));

            Assert.That(assessmentDetailsView.ChildViews[1].Description, Is.EqualTo("District Benchmark"));
            Assert.That(assessmentDetailsView.ChildViews[1].ChildViews.Count, Is.EqualTo(5));
            Assert.That(assessmentDetailsView.ChildViews[1].ChildViews[0].Description, Is.EqualTo("- ELA / Reading (District)"));
            Assert.That(assessmentDetailsView.ChildViews[1].ChildViews[1].Description, Is.EqualTo("- Writing (District)"));
            Assert.That(assessmentDetailsView.ChildViews[1].ChildViews[2].Description, Is.EqualTo("- Mathematics (District)"));
            Assert.That(assessmentDetailsView.ChildViews[1].ChildViews[3].Description, Is.EqualTo("- Science (District)"));
            Assert.That(assessmentDetailsView.ChildViews[1].ChildViews[4].Description, Is.EqualTo("- Social Studies (District)"));
        }
    }

    [TestFixture]
    public class When_teacher_views_their_cohort_default_assessment_detail_list : When_teacher_views_their_staff_information
    {
        protected override void EstablishContext()
        {
            suppliedViewType = StaffModel.ViewType.AssessmentDetails.ToString();
            suppliedSubjectArea = String.Empty;
            suppliedSectionId = providedCohortId2;
            suppliedStudentListType = StudentListType.Cohort.ToString();
            base.EstablishContext();
        }

        [Test]
        public override void Should_load_sections_and_cohorts()
        {
            {
                Assert.That(actualModel.SectionsAndCohorts, Is.Not.Null);
                Assert.That(actualModel.SectionsAndCohorts.Where(x => x.ListType == StudentListType.Cohort || x.ListType == StudentListType.Section || x.ListType == StudentListType.All).Count(), Is.EqualTo(5));
                Assert.That(actualModel.SectionsAndCohorts[0].SectionId, Is.EqualTo(0));
                Assert.That(actualModel.SectionsAndCohorts[0].Description, Is.EqualTo("Students From All Sections"));
                Assert.That(actualModel.SectionsAndCohorts[0].Link.Href, Is.EqualTo(staffAreaLinksFake.AssessmentDetails(providedSchoolId, providedStaffUSI1, providedFullName, null as int?, StudentListType.All.ToString())));
                Assert.That(actualModel.SectionsAndCohorts[0].Selected, Is.False);
                Assert.That(actualModel.SectionsAndCohorts[1].SectionId, Is.EqualTo(providedTeacherSection1));
                Assert.That(actualModel.SectionsAndCohorts[1].Description, Is.EqualTo("Mathematics(local course code) - course title (class period) term type"));
                Assert.That(actualModel.SectionsAndCohorts[1].Link.Href, Is.EqualTo(staffAreaLinksFake.AssessmentDetails(providedSchoolId, providedStaffUSI1, providedFullName, providedTeacherSection1, StudentListType.Section.ToString())));
                Assert.That(actualModel.SectionsAndCohorts[1].Selected, Is.False);
                Assert.That(actualModel.SectionsAndCohorts[3].SectionId, Is.EqualTo(providedCohortId1));
                Assert.That(actualModel.SectionsAndCohorts[3].Description, Is.EqualTo(providedCohortDescription1));
                Assert.That(actualModel.SectionsAndCohorts[3].Link.Href, Is.EqualTo(staffAreaLinksFake.AssessmentDetails(providedSchoolId, providedStaffUSI1, providedFullName, providedCohortId1, StudentListType.Cohort.ToString())));
                Assert.That(actualModel.SectionsAndCohorts[3].Selected, Is.False);
                Assert.That(actualModel.SectionsAndCohorts[4].SectionId, Is.EqualTo(providedCohortId2));
                Assert.That(actualModel.SectionsAndCohorts[4].Description, Is.EqualTo(providedCohortDescription2));
                Assert.That(actualModel.SectionsAndCohorts[4].Link.Href, Is.EqualTo(staffAreaLinksFake.AssessmentDetails(providedSchoolId, providedStaffUSI1, providedFullName, providedCohortId2, StudentListType.Cohort.ToString())));
                Assert.That(actualModel.SectionsAndCohorts[4].Selected, Is.True);

                Assert.That(actualModel.SectionsAndCohorts.SelectMany(s => s.ChildSections), Has.All.Property("Link").Not.Null);
            }
        }

        [Test]
        public override void Should_load_all_schools()
        {
            Assert.That(actualModel.Schools, Is.Not.Null);
            Assert.That(actualModel.Schools.Count, Is.EqualTo(4));
            Assert.That(actualModel.Schools[0].SchoolId, Is.EqualTo(providedSchoolId + 3));
            Assert.That(actualModel.Schools[0].Name, Is.EqualTo(providedSchoolName3));
            Assert.That(actualModel.Schools[0].Selected, Is.False);
            Assert.That(actualModel.Schools[0].Link.Href, Is.EqualTo(staffAreaLinksFake.AssessmentDetails(providedSchoolId + 3, providedStaffUSI1, providedFullName)));
            Assert.That(actualModel.Schools[1].SchoolId, Is.EqualTo(providedSchoolId));
            Assert.That(actualModel.Schools[1].Selected, Is.True);
            Assert.That(actualModel.Schools[1].Link.Href, Is.EqualTo(staffAreaLinksFake.AssessmentDetails(providedSchoolId, providedStaffUSI1, providedFullName)));
            Assert.That(actualModel.Schools[2].SchoolId, Is.EqualTo(providedSchoolId + 2));
            Assert.That(actualModel.Schools[2].Selected, Is.False);
            Assert.That(actualModel.Schools[2].Link.Href, Is.EqualTo(staffAreaLinksFake.AssessmentDetails(providedSchoolId + 2, providedStaffUSI1, providedFullName)));
            Assert.That(actualModel.Schools[3].SchoolId, Is.EqualTo(providedSchoolId + 1));
            Assert.That(actualModel.Schools[3].Selected, Is.False);
            Assert.That(actualModel.Schools[3].Link.Href, Is.EqualTo(staffAreaLinksFake.AssessmentDetails(providedSchoolId + 1, providedStaffUSI1, providedFullName)));
        }

        [Test]
        public override void Should_load_all_views()
        {
            Assert.That(actualModel.Views, Is.Not.Null);
            Assert.That(actualModel.Views.Count, Is.EqualTo(3));

            var generalOverviewView = actualModel.Views[0];
            var priorYearView = actualModel.Views[1];
            var assessmentDetailsView = actualModel.Views[2];

            //validate the link
            Assert.That(generalOverviewView.Link.Href, Is.EqualTo(staffAreaLinksFake.GeneralOverview(providedSchoolId, providedStaffUSI1, providedFullName, providedCohortId2, StudentListType.Cohort.ToString())));
            Assert.That(priorYearView.Link.Href, Is.EqualTo(staffAreaLinksFake.PriorYear(providedSchoolId, providedStaffUSI1, providedFullName, providedCohortId2, StudentListType.Cohort.ToString())));
            Assert.That(assessmentDetailsView.Link.Href, Is.EqualTo(staffAreaLinksFake.AssessmentDetails(providedSchoolId, providedStaffUSI1, providedFullName, providedCohortId2, StudentListType.Cohort.ToString(), null, StaffModel.AssessmentSubType.StateStandardized.ToString())));

            Assert.That(assessmentDetailsView.ChildViews[0].Link.Href, Is.EqualTo(staffAreaLinksFake.AssessmentDetails(providedSchoolId, providedStaffUSI1, providedFullName, providedCohortId2, StudentListType.Cohort.ToString(), null, StaffModel.AssessmentSubType.StateStandardized.ToString())));
            Assert.That(assessmentDetailsView.ChildViews[0].ChildViews[0].Link.Href, Is.EqualTo(staffAreaLinksFake.AssessmentDetails(providedSchoolId, providedStaffUSI1, providedFullName, providedCohortId2, StudentListType.Cohort.ToString(), StaffModel.SubjectArea.ELA.ToString(), StaffModel.AssessmentSubType.StateStandardized.ToString())));
            Assert.That(assessmentDetailsView.ChildViews[0].ChildViews[1].Link.Href, Is.EqualTo(staffAreaLinksFake.AssessmentDetails(providedSchoolId, providedStaffUSI1, providedFullName, providedCohortId2, StudentListType.Cohort.ToString(), StaffModel.SubjectArea.Writing.ToString(), StaffModel.AssessmentSubType.StateStandardized.ToString())));
            Assert.That(assessmentDetailsView.ChildViews[0].ChildViews[2].Link.Href, Is.EqualTo(staffAreaLinksFake.AssessmentDetails(providedSchoolId, providedStaffUSI1, providedFullName, providedCohortId2, StudentListType.Cohort.ToString(), StaffModel.SubjectArea.Mathematics.ToString(), StaffModel.AssessmentSubType.StateStandardized.ToString())));
            Assert.That(assessmentDetailsView.ChildViews[0].ChildViews[3].Link.Href, Is.EqualTo(staffAreaLinksFake.AssessmentDetails(providedSchoolId, providedStaffUSI1, providedFullName, providedCohortId2, StudentListType.Cohort.ToString(), StaffModel.SubjectArea.Science.ToString(), StaffModel.AssessmentSubType.StateStandardized.ToString())));
            Assert.That(assessmentDetailsView.ChildViews[0].ChildViews[4].Link.Href, Is.EqualTo(staffAreaLinksFake.AssessmentDetails(providedSchoolId, providedStaffUSI1, providedFullName, providedCohortId2, StudentListType.Cohort.ToString(), StaffModel.SubjectArea.SocialStudies.ToString(), StaffModel.AssessmentSubType.StateStandardized.ToString())));

            Assert.That(assessmentDetailsView.ChildViews[1].Link.Href, Is.EqualTo(staffAreaLinksFake.AssessmentDetails(providedSchoolId, providedStaffUSI1, providedFullName, providedCohortId2, StudentListType.Cohort.ToString(), null, StaffModel.AssessmentSubType.Benchmark.ToString())));
            Assert.That(assessmentDetailsView.ChildViews[1].ChildViews[0].Link.Href, Is.EqualTo(staffAreaLinksFake.AssessmentDetails(providedSchoolId, providedStaffUSI1, providedFullName, providedCohortId2, StudentListType.Cohort.ToString(), StaffModel.SubjectArea.ELA.ToString(), StaffModel.AssessmentSubType.Benchmark.ToString())));
            Assert.That(assessmentDetailsView.ChildViews[1].ChildViews[1].Link.Href, Is.EqualTo(staffAreaLinksFake.AssessmentDetails(providedSchoolId, providedStaffUSI1, providedFullName, providedCohortId2, StudentListType.Cohort.ToString(), StaffModel.SubjectArea.Writing.ToString(), StaffModel.AssessmentSubType.Benchmark.ToString())));
            Assert.That(assessmentDetailsView.ChildViews[1].ChildViews[2].Link.Href, Is.EqualTo(staffAreaLinksFake.AssessmentDetails(providedSchoolId, providedStaffUSI1, providedFullName, providedCohortId2, StudentListType.Cohort.ToString(), StaffModel.SubjectArea.Mathematics.ToString(), StaffModel.AssessmentSubType.Benchmark.ToString())));
            Assert.That(assessmentDetailsView.ChildViews[1].ChildViews[3].Link.Href, Is.EqualTo(staffAreaLinksFake.AssessmentDetails(providedSchoolId, providedStaffUSI1, providedFullName, providedCohortId2, StudentListType.Cohort.ToString(), StaffModel.SubjectArea.Science.ToString(), StaffModel.AssessmentSubType.Benchmark.ToString())));
            Assert.That(assessmentDetailsView.ChildViews[1].ChildViews[4].Link.Href, Is.EqualTo(staffAreaLinksFake.AssessmentDetails(providedSchoolId, providedStaffUSI1, providedFullName, providedCohortId2, StudentListType.Cohort.ToString(), StaffModel.SubjectArea.SocialStudies.ToString(), StaffModel.AssessmentSubType.Benchmark.ToString())));

            //validate if the link is selected by default
            Assert.That(generalOverviewView.Selected, Is.False);
            Assert.That(priorYearView.Selected, Is.False);
            Assert.That(assessmentDetailsView.Selected, Is.False);

            Assert.That(assessmentDetailsView.ChildViews[0].Selected, Is.False);
            Assert.That(assessmentDetailsView.ChildViews[0].ChildViews[0].Selected, Is.False);
            Assert.That(assessmentDetailsView.ChildViews[0].ChildViews[1].Selected, Is.False);
            Assert.That(assessmentDetailsView.ChildViews[0].ChildViews[2].Selected, Is.False);
            Assert.That(assessmentDetailsView.ChildViews[0].ChildViews[3].Selected, Is.False);
            Assert.That(assessmentDetailsView.ChildViews[0].ChildViews[4].Selected, Is.False);

            Assert.That(assessmentDetailsView.ChildViews[1].Selected, Is.False);
            Assert.That(assessmentDetailsView.ChildViews[1].ChildViews[0].Selected, Is.False);
            Assert.That(assessmentDetailsView.ChildViews[1].ChildViews[1].Selected, Is.False);
            Assert.That(assessmentDetailsView.ChildViews[1].ChildViews[2].Selected, Is.False);
            Assert.That(assessmentDetailsView.ChildViews[1].ChildViews[3].Selected, Is.False);
            Assert.That(assessmentDetailsView.ChildViews[1].ChildViews[4].Selected, Is.False);
        }
    }

    [TestFixture]
    public class When_teacher_views_all_students : When_teacher_views_their_staff_information
    {
        protected override void EstablishContext()
        {
            suppliedStudentListType = StudentListType.All.ToString();
            base.EstablishContext();
        }

        [Test]
        public override void Should_load_sections_and_cohorts()
        {
            Assert.That(actualModel.SectionsAndCohorts, Is.Not.Null);
            Assert.That(actualModel.SectionsAndCohorts.Where(x=>x.ListType == StudentListType.Cohort || x.ListType == StudentListType.Section || x.ListType == StudentListType.All).Count(), Is.EqualTo(5));
            Assert.That(actualModel.SectionsAndCohorts[0].SectionId, Is.EqualTo(0));
            Assert.That(actualModel.SectionsAndCohorts[0].Description, Is.EqualTo("Students From All Sections"));
            Assert.That(actualModel.SectionsAndCohorts[0].Selected, Is.True);
            Assert.That(actualModel.SectionsAndCohorts[1].SectionId, Is.EqualTo(providedTeacherSection1));
            Assert.That(actualModel.SectionsAndCohorts[1].Description, Is.EqualTo("Mathematics(local course code) - course title (class period) term type"));
            Assert.That(actualModel.SectionsAndCohorts[3].SectionId, Is.EqualTo(providedCohortId1));
            Assert.That(actualModel.SectionsAndCohorts[3].Description, Is.EqualTo(providedCohortDescription1));

            Assert.That(actualModel.SectionsAndCohorts.SelectMany(s => s.ChildSections), Has.All.Property("Link").Not.Null);
        }

        [Test]
        public override void Should_load_all_views()
        {
            Assert.That(actualModel.Views, Is.Not.Null);
            Assert.That(actualModel.Views.Count, Is.EqualTo(3));


            var generalOverviewView = actualModel.Views[0];
            var priorYearView = actualModel.Views[1];
            var assessmentDetailsView = actualModel.Views[2];

            //Validate the description
            Assert.That(generalOverviewView.Description, Is.EqualTo("General Overview"));
            Assert.That(priorYearView.Description, Is.EqualTo("Prior Year"));
            Assert.That(assessmentDetailsView.Description, Is.EqualTo("Assessment Details"));

            Assert.That(generalOverviewView.ChildViews.Count, Is.EqualTo(0));
            Assert.That(priorYearView.ChildViews.Count, Is.EqualTo(0));
            Assert.That(assessmentDetailsView.ChildViews.Count, Is.EqualTo(2));

            Assert.That(assessmentDetailsView.ChildViews[0].Description, Is.EqualTo("State Standardized"));
            Assert.That(assessmentDetailsView.ChildViews[0].ChildViews.Count, Is.EqualTo(5));
            Assert.That(assessmentDetailsView.ChildViews[0].ChildViews[0].Description, Is.EqualTo("- ELA / Reading (State)"));
            Assert.That(assessmentDetailsView.ChildViews[0].ChildViews[1].Description, Is.EqualTo("- Writing (State)"));
            Assert.That(assessmentDetailsView.ChildViews[0].ChildViews[2].Description, Is.EqualTo("- Mathematics (State)"));
            Assert.That(assessmentDetailsView.ChildViews[0].ChildViews[3].Description, Is.EqualTo("- Science (State)"));
            Assert.That(assessmentDetailsView.ChildViews[0].ChildViews[4].Description, Is.EqualTo("- Social Studies (State)"));

            Assert.That(assessmentDetailsView.ChildViews[1].Description, Is.EqualTo("District Benchmark"));
            Assert.That(assessmentDetailsView.ChildViews[1].ChildViews.Count, Is.EqualTo(5));
            Assert.That(assessmentDetailsView.ChildViews[1].ChildViews[0].Description, Is.EqualTo("- ELA / Reading (District)"));
            Assert.That(assessmentDetailsView.ChildViews[1].ChildViews[1].Description, Is.EqualTo("- Writing (District)"));
            Assert.That(assessmentDetailsView.ChildViews[1].ChildViews[2].Description, Is.EqualTo("- Mathematics (District)"));
            Assert.That(assessmentDetailsView.ChildViews[1].ChildViews[3].Description, Is.EqualTo("- Science (District)"));
            Assert.That(assessmentDetailsView.ChildViews[1].ChildViews[4].Description, Is.EqualTo("- Social Studies (District)"));

            //validate the link
            Assert.That(generalOverviewView.Link.Href, Is.EqualTo(staffAreaLinksFake.GeneralOverview(providedSchoolId, providedStaffUSI1, providedFullName, null as int?, StudentListType.All.ToString())));
            Assert.That(priorYearView.Link.Href, Is.EqualTo(staffAreaLinksFake.PriorYear(providedSchoolId, providedStaffUSI1, providedFullName, null as int?, StudentListType.All.ToString())));
            Assert.That(assessmentDetailsView.Link.Href, Is.EqualTo(staffAreaLinksFake.AssessmentDetails(providedSchoolId, providedStaffUSI1, providedFullName, null as int?, StudentListType.All.ToString(), null, StaffModel.AssessmentSubType.StateStandardized.ToString())));

            Assert.That(assessmentDetailsView.ChildViews[0].Link.Href, Is.EqualTo(staffAreaLinksFake.AssessmentDetails(providedSchoolId, providedStaffUSI1, providedFullName, null as int?, StudentListType.All.ToString(), null, StaffModel.AssessmentSubType.StateStandardized.ToString())));
            Assert.That(assessmentDetailsView.ChildViews[0].ChildViews[0].Link.Href, Is.EqualTo(staffAreaLinksFake.AssessmentDetails(providedSchoolId, providedStaffUSI1, providedFullName, null as int?, StudentListType.All.ToString(), StaffModel.SubjectArea.ELA.ToString(), StaffModel.AssessmentSubType.StateStandardized.ToString())));
            Assert.That(assessmentDetailsView.ChildViews[0].ChildViews[1].Link.Href, Is.EqualTo(staffAreaLinksFake.AssessmentDetails(providedSchoolId, providedStaffUSI1, providedFullName, null as int?, StudentListType.All.ToString(), StaffModel.SubjectArea.Writing.ToString(), StaffModel.AssessmentSubType.StateStandardized.ToString())));
            Assert.That(assessmentDetailsView.ChildViews[0].ChildViews[2].Link.Href, Is.EqualTo(staffAreaLinksFake.AssessmentDetails(providedSchoolId, providedStaffUSI1, providedFullName, null as int?, StudentListType.All.ToString(), StaffModel.SubjectArea.Mathematics.ToString(), StaffModel.AssessmentSubType.StateStandardized.ToString())));
            Assert.That(assessmentDetailsView.ChildViews[0].ChildViews[3].Link.Href, Is.EqualTo(staffAreaLinksFake.AssessmentDetails(providedSchoolId, providedStaffUSI1, providedFullName, null as int?, StudentListType.All.ToString(), StaffModel.SubjectArea.Science.ToString(), StaffModel.AssessmentSubType.StateStandardized.ToString())));
            Assert.That(assessmentDetailsView.ChildViews[0].ChildViews[4].Link.Href, Is.EqualTo(staffAreaLinksFake.AssessmentDetails(providedSchoolId, providedStaffUSI1, providedFullName, null as int?, StudentListType.All.ToString(), StaffModel.SubjectArea.SocialStudies.ToString(), StaffModel.AssessmentSubType.StateStandardized.ToString())));

            Assert.That(assessmentDetailsView.ChildViews[1].Link.Href, Is.EqualTo(staffAreaLinksFake.AssessmentDetails(providedSchoolId, providedStaffUSI1, providedFullName, null as int?, StudentListType.All.ToString(), null, StaffModel.AssessmentSubType.Benchmark.ToString())));
            Assert.That(assessmentDetailsView.ChildViews[1].ChildViews[0].Link.Href, Is.EqualTo(staffAreaLinksFake.AssessmentDetails(providedSchoolId, providedStaffUSI1, providedFullName, null as int?, StudentListType.All.ToString(), StaffModel.SubjectArea.ELA.ToString(), StaffModel.AssessmentSubType.Benchmark.ToString())));
            Assert.That(assessmentDetailsView.ChildViews[1].ChildViews[1].Link.Href, Is.EqualTo(staffAreaLinksFake.AssessmentDetails(providedSchoolId, providedStaffUSI1, providedFullName, null as int?, StudentListType.All.ToString(), StaffModel.SubjectArea.Writing.ToString(), StaffModel.AssessmentSubType.Benchmark.ToString())));
            Assert.That(assessmentDetailsView.ChildViews[1].ChildViews[2].Link.Href, Is.EqualTo(staffAreaLinksFake.AssessmentDetails(providedSchoolId, providedStaffUSI1, providedFullName, null as int?, StudentListType.All.ToString(), StaffModel.SubjectArea.Mathematics.ToString(), StaffModel.AssessmentSubType.Benchmark.ToString())));
            Assert.That(assessmentDetailsView.ChildViews[1].ChildViews[3].Link.Href, Is.EqualTo(staffAreaLinksFake.AssessmentDetails(providedSchoolId, providedStaffUSI1, providedFullName, null as int?, StudentListType.All.ToString(), StaffModel.SubjectArea.Science.ToString(), StaffModel.AssessmentSubType.Benchmark.ToString())));
            Assert.That(assessmentDetailsView.ChildViews[1].ChildViews[4].Link.Href, Is.EqualTo(staffAreaLinksFake.AssessmentDetails(providedSchoolId, providedStaffUSI1, providedFullName, null as int?, StudentListType.All.ToString(), StaffModel.SubjectArea.SocialStudies.ToString(), StaffModel.AssessmentSubType.Benchmark.ToString())));

            //validate if the link is selected by default
            Assert.That(generalOverviewView.Selected, Is.True);
            Assert.That(priorYearView.Selected, Is.False);
            Assert.That(assessmentDetailsView.Selected, Is.False);

            Assert.That(assessmentDetailsView.ChildViews[0].Selected, Is.False);
            Assert.That(assessmentDetailsView.ChildViews[0].ChildViews[0].Selected, Is.False);
            Assert.That(assessmentDetailsView.ChildViews[0].ChildViews[1].Selected, Is.False);
            Assert.That(assessmentDetailsView.ChildViews[0].ChildViews[2].Selected, Is.False);
            Assert.That(assessmentDetailsView.ChildViews[0].ChildViews[3].Selected, Is.False);
            Assert.That(assessmentDetailsView.ChildViews[0].ChildViews[4].Selected, Is.False);

            Assert.That(assessmentDetailsView.ChildViews[1].Selected, Is.False);
            Assert.That(assessmentDetailsView.ChildViews[1].ChildViews[0].Selected, Is.False);
            Assert.That(assessmentDetailsView.ChildViews[1].ChildViews[1].Selected, Is.False);
            Assert.That(assessmentDetailsView.ChildViews[1].ChildViews[2].Selected, Is.False);
            Assert.That(assessmentDetailsView.ChildViews[1].ChildViews[3].Selected, Is.False);
            Assert.That(assessmentDetailsView.ChildViews[1].ChildViews[4].Selected, Is.False);
        }
    
    }

    [TestFixture]
    public class When_superintendent_views_staff_information : StaffServiceFixture
    {
        protected override void EstablishContext()
        {
            providedStaffUSI = providedStaffUSI1+100;
            providedUserName = "superintendent";
            var claimTypes = new List<String>
                                 {
                                     EdFiClaimTypes.ViewAllStudents
                                 };
            providedUserInformation = new UserInformation
                                          {
                                              StaffUSI = providedStaffUSI,
                                              FullName = providedFullName,
                                              AssociatedOrganizations = new List<UserInformation.EducationOrganization>
                                                                            {
                                                                                new UserInformation.LocalEducationAgency
                                                                                    (providedSchoolId + 16)
                                                                                    {
                                                                                        Name = "My ISD",
                                                                                        ClaimTypes = claimTypes
                                                                                    }
                                                                            }
                                          };
            base.EstablishContext();
        }
    
        [Test]
        public void Should_load_all_schools()
        {
            Assert.That(actualModel.Schools, Is.Not.Null);
            Assert.That(actualModel.Schools.Count, Is.EqualTo(4));
            Assert.That(actualModel.Schools[0].SchoolId, Is.EqualTo(providedSchoolId + 3));
            Assert.That(actualModel.Schools[0].Name, Is.EqualTo(providedSchoolName3));
            Assert.That(actualModel.Schools[1].SchoolId, Is.EqualTo(providedSchoolId));
            Assert.That(actualModel.Schools[2].SchoolId, Is.EqualTo(providedSchoolId + 2));
            Assert.That(actualModel.Schools[3].SchoolId, Is.EqualTo(providedSchoolId + 1));
            
        }
    }
}
