// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Castle.Core;
using Castle.Core.Internal;
using EdFi.Dashboards.Application.Data.Entities;
using EdFi.Dashboards.Common;
using EdFi.Dashboards.Data.Entities;
using EdFi.Dashboards.Data.Repository;
using EdFi.Dashboards.Infrastructure.Implementations;
using EdFi.Dashboards.Infrastructure.Implementations.Caching;
using EdFi.Dashboards.Resource.Models.Common;
using EdFi.Dashboards.Resources.Common;
using EdFi.Dashboards.Resources.Models.Common;
using EdFi.Dashboards.Resources.Models.Staff;
using EdFi.Dashboards.Resources.Navigation;
using EdFi.Dashboards.Resources.Security.Common;
using EdFi.Dashboards.SecurityTokenService.Authentication;
using SubSonic.Extensions;
using SubSonic.Query;

namespace EdFi.Dashboards.Resources.Staff
{
    public class StaffRequest
    {
        public long StaffUSI { get; set; }
        public int SchoolId { get; set; }
        public string StudentListType { get; set; }
        public long SectionOrCohortId { get; set; }

        [AuthenticationIgnore("SubjectArea does not affect the results of the request in a way requiring it to be secured.")]
        public string SubjectArea { get; set; }

        [AuthenticationIgnore("ViewType does not affect the results of the request in a way requiring it to be secured.")]
        public string ViewType { get; set; }

        [AuthenticationIgnore("ViewType does not affect the results of the request in a way requiring it to be secured.")]
        public string AssessmentSubType { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="StaffRequest"/> using the specified parameters.
        /// </summary>
        /// <returns>A new <see cref="StaffRequest"/> instance.</returns>
        public static StaffRequest Create(long staffUSI, int schoolId, string studentListType, long sectionOrCohortId, string viewType) 
        {
            return new StaffRequest { StaffUSI = staffUSI, SchoolId = schoolId, StudentListType = studentListType, SectionOrCohortId = sectionOrCohortId, ViewType = viewType };
        }
    }

    public interface IStaffService : IService<StaffRequest, StaffModel> { }

    public class StaffService : IStaffService
    {
        protected const string SectionDescriptionFormat = "{0}({1}) - {2} ({3}) {4}";
        protected const string GeneralOverview = "General Overview";
        protected const string PriorYear = "Prior Year";
        protected const string SubjectSpecific = "Subject Specific";
        protected const string AssessmentDetails = "Assessment Details";
        protected const string SeeStudentsFromAllSections = "Students From All Sections";

        private readonly IRepository<StaffInformation> staffInfoRepository;
	protected IRepository<StaffInformation> StaffInfoRepository
	{
		get { return staffInfoRepository; }
        }
        protected readonly IWatchListLinkProvider WatchListLinkProvider;
        
        private readonly IRepository<MetricBasedWatchList> watchListRepository;
        protected IRepository<MetricBasedWatchList> WatchListRepository
        {
        	get { return watchListRepository; }
        }
                
        private readonly IRepository<MetricBasedWatchListOption> watchListOptionRepository;
        protected IRepository<MetricBasedWatchListOption> WatchListOptionRepository
        {
        	get { return watchListOptionRepository; }
        }

        
        private readonly IRepository<TeacherSection> teacherSectionRepository;
		protected IRepository<TeacherSection> TeacherSectionRepository
		{
			get { return teacherSectionRepository; }
		}

        private readonly IRepository<StaffCohort> staffCohortRepository;
		protected IRepository<StaffCohort> StaffCohortRepository
		{
			get { return staffCohortRepository; }
		}

        private readonly IRepository<StaffCustomStudentList> staffCustomStudentListRepository;
		protected IRepository<StaffCustomStudentList> StaffCustomStudentListRepository
		{
			get { return staffCustomStudentListRepository; }
		}

        private readonly IRepository<SchoolInformation> schoolInformationRepository;
		protected IRepository<SchoolInformation> SchoolInformationRepository
		{
			get { return schoolInformationRepository; }
		}

        private readonly IStaffAreaLinks staffLinks;
		protected IStaffAreaLinks StaffLinks
		{
			get { return staffLinks; }
		}

		private readonly ICurrentUserClaimInterrogator currentUserClaimInterrogator;
		protected ICurrentUserClaimInterrogator CurrentUserClaimInterrogator
		{
			get { return currentUserClaimInterrogator; }
		}

        private readonly IStaffViewProvider staffViewProvider;
		protected IStaffViewProvider StaffViewProvider
		{
			get { return staffViewProvider; }
		}

        public StaffService(IRepository<StaffCohort> staffCohortRepository, IRepository<StaffInformation> staffInfoRepository, IRepository<TeacherSection> teacherSectionRepository,
            IRepository<SchoolInformation> schoolInformationRepository, IStaffAreaLinks staffLinks, IRepository<StaffCustomStudentList> staffCustomStudentListRepository,
            IRepository<MetricBasedWatchList> watchListRepository, IRepository<MetricBasedWatchListOption> watchListOptionRepository,
            ICurrentUserClaimInterrogator currentUserClaimInterrogator, IStaffViewProvider staffViewProvider, IWatchListLinkProvider watchListLinkProvider)
        {
            this.currentUserClaimInterrogator = currentUserClaimInterrogator;            
            this.staffCohortRepository = staffCohortRepository;
            this.staffInfoRepository = staffInfoRepository;
            this.teacherSectionRepository = teacherSectionRepository;
            this.schoolInformationRepository = schoolInformationRepository;
            this.staffLinks = staffLinks;
            this.staffCustomStudentListRepository = staffCustomStudentListRepository;
            this.watchListRepository = watchListRepository;
            this.watchListOptionRepository = watchListOptionRepository;
            this.staffViewProvider = staffViewProvider;
            WatchListLinkProvider = watchListLinkProvider;
        }

        [NoCache]
        [CanBeAuthorizedBy(EdFiClaimTypes.ViewAllStudents, EdFiClaimTypes.ViewMyStudents)]
        public virtual StaffModel Get(StaffRequest request)
        {
            long staffUSI = request.StaffUSI;
            int schoolId = request.SchoolId;
            long sectionOrCohortId = request.SectionOrCohortId;
            string studentListType = request.StudentListType;
            string viewType = request.ViewType;
            string subjectArea = request.SubjectArea;
            string assessmentSubType = request.AssessmentSubType;

            var result = (from data in staffInfoRepository.GetAll()
                         where data.StaffUSI == staffUSI
                         select data).SingleOrDefault();

            var currentUser = UserInformation.Current;
            if (result == null && request.StaffUSI == currentUser.StaffUSI)
            {
                result = new StaffInformation
                {
                    StaffUSI = currentUser.StaffUSI,
                    FullName = currentUser.FullName,
                    FirstName = currentUser.FirstName,
                    LastSurname = currentUser.LastName,
                    EmailAddress = currentUser.EmailAddress
                };
            }

            if (result == null)
                return null;

            var model = new StaffModel
                            {
                                StaffUSI = result.StaffUSI,
                                FullName = result.FullName,
                                ProfileThumbnail = staffLinks.ProfileThumbnail(request.SchoolId, request.StaffUSI, result.Gender),
                            };

            if (String.IsNullOrEmpty(viewType))
                viewType = StaffModel.ViewType.GeneralOverview.ToString();

            var vt = (StaffModel.ViewType) Enum.Parse(typeof(StaffModel.ViewType), viewType, true);

            LoadSectionsAndCohorts(staffUSI, schoolId, model, vt, subjectArea, assessmentSubType);

            SelectCurrentSection(studentListType, sectionOrCohortId, vt, model);
            var listType = StudentListType.None;
            if (model.CurrentSection != null)
            {
                studentListType = model.CurrentSection.ListType.ToString();
                listType = model.CurrentSection.ListType;
                sectionOrCohortId = model.CurrentSection.SectionId;
            }

            LoadSchools(staffUSI, schoolId, vt, model);

            LoadViews(staffUSI, schoolId, studentListType, sectionOrCohortId, listType, vt, subjectArea, assessmentSubType, model);

            return model;
        }

        private static void SelectCurrentSection(string studentListType, long sectionOrCohortId, StaffModel.ViewType vt, StaffModel model)
        {
            if (String.IsNullOrEmpty(studentListType))
                studentListType = StudentListType.None.ToString();
            var slt = (StudentListType) Enum.Parse(typeof (StudentListType), studentListType, true);
            var selectedSection = model.SectionsAndCohorts.SingleOrDefault(x => x.SectionId == sectionOrCohortId && x.ListType == slt && !x.ChildSections.Any());

            if (selectedSection == null && slt == StudentListType.MetricsBasedWatchList)
            {
                var childSections =
                    model.SectionsAndCohorts.Where(data => data.ChildSections.Count > 0)
                        .SelectMany(section => section.ChildSections, (s, cs) => cs).ToList();

                selectedSection = childSections.FirstOrDefault(data => data.SectionId == sectionOrCohortId);
            }

            if (selectedSection != null)
            {
                selectedSection.Selected = true;
                model.CurrentSection = selectedSection;
            }
            else if (model.SectionsAndCohorts.Any())
            {
                var i = (model.SectionsAndCohorts.Count > 1 && (vt != StaffModel.ViewType.SubjectSpecificOverview)) ? 1 : 0;
                model.SectionsAndCohorts[i].Selected = true;
                model.CurrentSection = model.SectionsAndCohorts[i];
            }
        }

        private void LoadSectionsAndCohorts(long staffUSI, int schoolId, StaffModel model, StaffModel.ViewType vt, string subjectArea, string assessmentSubType)
        {
            if (vt != StaffModel.ViewType.SubjectSpecificOverview)
            {
                model.SectionsAndCohorts.Add(
                    new StaffModel.SectionOrCohort
                        {
                            Description = SeeStudentsFromAllSections,
                            ListType = StudentListType.All
                        });
            }

            model.SectionsAndCohorts.AddRange(GetCurrentSections(staffUSI, schoolId));

            if (vt != StaffModel.ViewType.SubjectSpecificOverview)
            {
                model.SectionsAndCohorts.AddRange(GetCohorts(staffUSI, schoolId));
                // TODO: We may want to apply the same treatment to the student watch lists as the metrics based watch list (see code below)
                model.SectionsAndCohorts.AddRange(GetCustomStudentLists(staffUSI, schoolId));

                // the metrics based watch lists will have an option group to
                // seperate them from the rest of the options
                var studentWatchLists = GetCustomStudentWatchLists(staffUSI, schoolId);
                if (studentWatchLists.Any())
                {
                    var sectionGroup = new StaffModel.SectionOrCohort
                    {
                        Description = "Dynamic Lists"
                    };

                    sectionGroup.ChildSections.AddRange(studentWatchLists);
                    model.SectionsAndCohorts.Add(sectionGroup);
                }
            }

            switch (vt)
            {
                case StaffModel.ViewType.GeneralOverview:
                    foreach (var sectionOrCohort in model.SectionsAndCohorts)
                    {
                        if (sectionOrCohort.ChildSections.Count == 0)
                        {
                            sectionOrCohort.Link = new Link
                            {
                                Href =
                                    staffLinks.GeneralOverview(schoolId, staffUSI, model.FullName,
                                        sectionOrCohort.SectionId == 0 ? null as long? : sectionOrCohort.SectionId,
                                        sectionOrCohort.ListType.ToString()),
                                Rel = GeneralOverview
                            };
                        }
                        else
                        {
                            foreach (var section in sectionOrCohort.ChildSections)
                            {
                                var watchListStaffUSI = section.PageOptions.Any(data => data.Name == "StaffUSI")
                                    ? long.Parse(section.PageOptions.Single(data => data.Name == "StaffUSI").Value)
                                    : staffUSI;

                                var resourceName =
                                    section.PageOptions.SingleOrDefault(data => data.Name == "PageController");
                                var demographic =
                                    section.PageOptions.SingleOrDefault(data => data.Name == "PageDemographic");
                                var schoolCategory =
                                    section.PageOptions.SingleOrDefault(data => data.Name == "PageSchoolCategory");
                                var grade = section.PageOptions.SingleOrDefault(data => data.Name == "PageGrade");

                                var watchListLink = new WatchListLinkRequest
                                {
                                    SchoolId = schoolId,
                                    StaffUSI = watchListStaffUSI,
                                    MetricBasedWatchListId = (int?) section.SectionId,
                                    ResourceName = resourceName != null ? resourceName.Value : string.Empty,
                                    Demographic = demographic != null ? demographic.Value : string.Empty,
                                    SchoolCategory = schoolCategory != null ? schoolCategory.Value : string.Empty,
                                    Grade = grade != null ? grade.Value : string.Empty
                                };

                                section.Link = new Link
                                {
                                    Href = WatchListLinkProvider.GenerateLink(watchListLink),
                                    Rel = GeneralOverview
                                };
                        }
                    }
                    }
                    //model.SectionsAndCohorts.ForEach(x => x.Link = new Link { Href = staffLinks.GeneralOverview(schoolId, staffUSI, model.FullName, x.SectionId == 0 ? null as long? : x.SectionId, x.ListType.ToString()), Rel = GeneralOverview });
                    break;
                case StaffModel.ViewType.PriorYear:
                    model.SectionsAndCohorts.ForEach(x => x.Link = new Link { Href = staffLinks.PriorYear(schoolId, staffUSI, model.FullName, x.SectionId == 0 ? null as long? : x.SectionId, x.ListType.ToString()), Rel = GeneralOverview });
                    model.SectionsAndCohorts.Where(s => s.ChildSections != null).SelectMany(s => s.ChildSections).ForEach(x => x.Link = new Link { Href = staffLinks.PriorYear(schoolId, staffUSI, model.FullName, x.SectionId == 0 ? null as long? : x.SectionId, x.ListType.ToString()), Rel = GeneralOverview });
                    break;
                case StaffModel.ViewType.SubjectSpecificOverview:
                    model.SectionsAndCohorts.ForEach(x => x.Link = new Link { Href = staffLinks.SubjectSpecificOverview(schoolId, staffUSI, model.FullName, x.SectionId == 0 ? null as long? : x.SectionId, x.ListType.ToString()), Rel = SubjectSpecific });
                    model.SectionsAndCohorts.Where(s => s.ChildSections != null).SelectMany(s => s.ChildSections).ForEach(x => x.Link = new Link { Href = staffLinks.SubjectSpecificOverview(schoolId, staffUSI, model.FullName, x.SectionId == 0 ? null as long? : x.SectionId, x.ListType.ToString()), Rel = SubjectSpecific });
                    break;
                case StaffModel.ViewType.AssessmentDetails:
                    model.SectionsAndCohorts.ForEach(x => x.Link = new Link { Href = staffLinks.AssessmentDetails(schoolId, staffUSI, model.FullName, x.SectionId == 0 ? null as long? : x.SectionId, x.ListType.ToString(), subjectArea, assessmentSubType), Rel = AssessmentDetails });
                    model.SectionsAndCohorts.Where(s => s.ChildSections != null).SelectMany(s => s.ChildSections).ForEach(x => x.Link = new Link { Href = staffLinks.AssessmentDetails(schoolId, staffUSI, model.FullName, x.SectionId == 0 ? null as long? : x.SectionId, x.ListType.ToString(), subjectArea, assessmentSubType), Rel = AssessmentDetails });
                    break;
            }
        }

        protected virtual void LoadViews(long staffUSI, int schoolId, string studentListType, long sectionOrCohortId, StudentListType listType, StaffModel.ViewType viewType, string subjectArea, string assessmentSubType, StaffModel model)
        {
            long? sectionOrCohortIdForLink = sectionOrCohortId == 0 ? null as long? : sectionOrCohortId;

            model.Views.Add(new StaffModel.View { Description = GeneralOverview, Selected = StaffModel.ViewType.GeneralOverview == viewType, Link = new Link { Href = staffLinks.GeneralOverview(schoolId, staffUSI, model.FullName, sectionOrCohortIdForLink, studentListType) }, MenuLevel = 0 });

            model.Views.Add(new StaffModel.View { Description = PriorYear, Selected = StaffModel.ViewType.PriorYear == viewType, Link = new Link { Href = staffLinks.PriorYear(schoolId, staffUSI, model.FullName, sectionOrCohortIdForLink, studentListType) }, MenuLevel = 0 });

            if (listType == StudentListType.Section)
                model.Views.Add(new StaffModel.View { Description = SubjectSpecific, Selected = StaffModel.ViewType.SubjectSpecificOverview == viewType, Link = new Link { Href = staffLinks.SubjectSpecificOverview(schoolId, staffUSI, model.FullName, sectionOrCohortIdForLink, studentListType) }, MenuLevel = 0 });

            //Get the State Standardized View
            var stateStandardizedView = staffViewProvider.GetStateStandardizedView(staffLinks, schoolId, staffUSI, model.FullName, sectionOrCohortIdForLink, studentListType, viewType, assessmentSubType, subjectArea);
            
            //Get the District Benchmark View
            var districtBenchmarkView = staffViewProvider.GetDistrictBenchmarkView(staffLinks, schoolId, staffUSI, model.FullName, sectionOrCohortIdForLink, studentListType, viewType, assessmentSubType, subjectArea);

            //Get the Reading Assessment View
            var readingAssessmentView = staffViewProvider.GetReadingAssessmentView(staffLinks, schoolId, staffUSI, model.FullName, sectionOrCohortIdForLink, studentListType, viewType, assessmentSubType, subjectArea);

            //build the top level Assessment Details menu item if there is a sublevel menu

            if (stateStandardizedView != null || districtBenchmarkView != null || readingAssessmentView != null)
            {
                var assessmentDetialsMenu = new StaffModel.View { Description = AssessmentDetails, Selected = false, Link = new Link { Href = staffLinks.AssessmentDetails(schoolId, staffUSI, model.FullName, sectionOrCohortIdForLink, studentListType, null, StaffModel.AssessmentSubType.StateStandardized.ToString()) }, MenuLevel = 0 };
                model.Views.Add(assessmentDetialsMenu);

                if (stateStandardizedView != null)
                    assessmentDetialsMenu.ChildViews.Add(stateStandardizedView);

                if (districtBenchmarkView != null)
                    assessmentDetialsMenu.ChildViews.Add(districtBenchmarkView);

                if (readingAssessmentView != null)
                    assessmentDetialsMenu.ChildViews.Add(readingAssessmentView);
            }
        }

        private void LoadSchools(long staffUSI, int schoolId, StaffModel.ViewType vt, StaffModel model)
        {
            model.Schools.AddRange(GetCurrentSchools(staffUSI));
            switch (vt)
            {
                case StaffModel.ViewType.GeneralOverview:
                    model.Schools.ForEach(x => x.Link = new Link { Href = staffLinks.GeneralOverview(x.SchoolId, staffUSI, model.FullName), Rel = GeneralOverview });
                    break;
                case StaffModel.ViewType.SubjectSpecificOverview:
                    model.Schools.ForEach(x => x.Link = new Link { Href = staffLinks.SubjectSpecificOverview(x.SchoolId, staffUSI, model.FullName), Rel = SubjectSpecific });
                    break;
                case StaffModel.ViewType.AssessmentDetails:
                    model.Schools.ForEach(x => x.Link = new Link { Href = staffLinks.AssessmentDetails(x.SchoolId, staffUSI, model.FullName), Rel = AssessmentDetails });
                    break;
                case StaffModel.ViewType.PriorYear:
                    model.Schools.ForEach(x => x.Link = new Link { Href = staffLinks.PriorYear(x.SchoolId, staffUSI, model.FullName), Rel = PriorYear });
                    break;
            }
            var selectedSchool = model.Schools.SingleOrDefault(x => x.SchoolId == schoolId);
            if (selectedSchool != null)
                selectedSchool.Selected = true;
        }

        private IEnumerable<StaffModel.SectionOrCohort> GetCohorts(long staffUSI, int schoolId)
        {
            var cohorts = (from data in staffCohortRepository.GetAll()
                           where data.StaffUSI == staffUSI && data.EducationOrganizationId == schoolId
                           orderby data.CohortDescription , data.StaffCohortId
                           select data).ToList();


            var results = cohorts.Select(x => new StaffModel.SectionOrCohort
                                                  {
                                                      SectionId = x.StaffCohortId,
                                                      Description = x.CohortDescription,
                                                      ListType = StudentListType.Cohort
                                                  });

            return results;
        }

        private IEnumerable<StaffModel.SectionOrCohort> GetCustomStudentLists(long staffUSI, int schoolId)
        {
            var query = (from data in staffCustomStudentListRepository.GetAll()
                              where data.EducationOrganizationId == schoolId
                              orderby data.CustomStudentListIdentifier, data.StaffCustomStudentListId
                              select data);

            List<StaffCustomStudentList> customStudentLists;

            if (UserInformation.Current.StaffUSI == staffUSI || currentUserClaimInterrogator.HasClaimWithinEducationOrganizationHierarchy(EdFiClaimTypes.ViewAllStudents, schoolId))
            {
                customStudentLists = query.Where(x => x.StaffUSI == staffUSI).ToList();
            }
            else
            {
                customStudentLists = new List<StaffCustomStudentList>();
            }


            var results = customStudentLists.Select(x => new StaffModel.SectionOrCohort
            {
                SectionId = x.StaffCustomStudentListId,
                Description = x.CustomStudentListIdentifier,
                ListType = StudentListType.CustomStudentList
            });

            return results;
        }

        private IEnumerable<StaffModel.SectionOrCohort> GetCustomStudentWatchLists(long staffUSI, int schoolId)
        {
            var loggedInUser = UserInformation.Current.StaffUSI;

            var watchLists = WatchListRepository.GetAll()
                .Where(
                    data =>
                        data.StaffUSI == loggedInUser && data.EducationOrganizationId == schoolId)
                .Select(data => data).ToList();

            var watchListOptions = watchLists.Join(WatchListOptionRepository.GetAll(), wl => wl.MetricBasedWatchListId,
                wlo => wlo.MetricBasedWatchListId, (wl, wlo) => wlo);

            var results = watchLists.Select(data => new StaffModel.SectionOrCohort
            {
                SectionId = data.MetricBasedWatchListId,
                Description = data.WatchListName,
                ListType = StudentListType.MetricsBasedWatchList,
                PageOptions = watchListOptions.Where(option => option.MetricBasedWatchListId == data.MetricBasedWatchListId).Select(pageOption => new NameValueModel
                {
                    Name = pageOption.Name,
                    Value = pageOption.Value
                }).ToList()
            });

            return results;
        } 

        private IEnumerable<StaffModel.SectionOrCohort> GetCurrentSections(long staffUSI, int schoolId)
        {
            var results = (from data in teacherSectionRepository.GetAll()
                         where data.StaffUSI == staffUSI && data.SchoolId == schoolId
                         orderby data.SubjectArea, data.CourseTitle, data.ClassPeriod, data.LocalCourseCode, data.TeacherSectionId
                         select data).ToList();

            return results.Select(x => new StaffModel.SectionOrCohort
                                           {
                                               SectionId = x.TeacherSectionId,
                                               Description = String.Format(SectionDescriptionFormat, x.SubjectArea, x.LocalCourseCode, x.CourseTitle, x.ClassPeriod, x.TermType),
                                               ListType = StudentListType.Section
                                           });
        }

        private IEnumerable<StaffModel.School> GetCurrentSchools(long staffUSI)
        {
            var sectionSchools = (from school in schoolInformationRepository.GetAll()
                                  join section in teacherSectionRepository.GetAll()
                                      on school.SchoolId equals section.SchoolId
                                  where section.StaffUSI == staffUSI
                                  select new StaffModel.School
                                  {
                                      SchoolId = school.SchoolId,
                                      Name = school.Name
                                  }).Distinct().ToList();

            var cohortSchools = (from school in schoolInformationRepository.GetAll()
                                 join cohort in staffCohortRepository.GetAll()
                                     on school.SchoolId equals cohort.EducationOrganizationId
                                 where cohort.StaffUSI == staffUSI
                                 select new StaffModel.School
                                 {
                                     SchoolId = school.SchoolId,
                                     Name = school.Name
                                 }).Distinct().ToList();

            var comparer = new StaffSectionModelSchoolEqualityComparer();
            return sectionSchools.Union(cohortSchools, comparer).OrderBy(x => x.Name).ToList();
        }
        
        class StaffSectionModelSchoolEqualityComparer : IEqualityComparer<StaffModel.School>
        {
            public bool Equals(StaffModel.School x, StaffModel.School y)
            {
                if (x == null && y == null)
                    return true;

                if (x == null || y == null)
                    return false;

                return x.SchoolId == y.SchoolId && x.Name == y.Name;
            }

            public int GetHashCode(StaffModel.School obj)
            {
                return obj.SchoolId.GetHashCode();
            }
        }
    }
}
