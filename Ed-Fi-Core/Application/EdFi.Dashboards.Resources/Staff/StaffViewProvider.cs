using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EdFi.Dashboards.Resource.Models.Common;
using EdFi.Dashboards.Resources.Models.Staff;
using EdFi.Dashboards.Resources.Navigation;

namespace EdFi.Dashboards.Resources.Staff
{
    public interface IStaffViewProvider
    {
        StaffModel.View GetStateStandardizedView(IStaffAreaLinks staffLinks, int schoolId, long staffUSI, string fullName, long? sectionOrCohortIdForLink, string studentListType, StaffModel.ViewType viewType, string assessmentSubType, string subjectArea);
        StaffModel.View GetDistrictBenchmarkView(IStaffAreaLinks staffLinks, int schoolId, long staffUSI, string fullName, long? sectionOrCohortIdForLink, string studentListType, StaffModel.ViewType viewType, string assessmentSubType, string subjectArea);
        StaffModel.View GetReadingAssessmentView(IStaffAreaLinks staffLinks, int schoolId, long staffUSI, string fullName, long? sectionOrCohortIdForLink, string studentListType, StaffModel.ViewType viewType, string assessmentSubType, string subjectArea);
    }

    public class StaffViewProvider : IStaffViewProvider
    {
        protected const string StateStandardized = "State Standardized";
        private const string DistrictBenchmark = "District Benchmark";
        private const string SocialStudies = "Social Studies";
        private const string Science = "Science";
        private const string Mathematics = "Mathematics";
        private const string ElaReading = "ELA / Reading";
        private const string Writing = "Writing";

        public virtual StaffModel.View GetStateStandardizedView(IStaffAreaLinks staffLinks, int schoolId, long staffUSI, string fullName, long? sectionOrCohortIdForLink, string studentListType, StaffModel.ViewType viewType, string assessmentSubType, string subjectArea)
        {
            //build the top level state standardized view
            var stateStatndardizedMenu = new StaffModel.View { Description = StateStandardized, Selected = false, Link = new Link { Href = staffLinks.AssessmentDetails(schoolId, staffUSI, fullName, sectionOrCohortIdForLink, studentListType, null, StaffModel.AssessmentSubType.StateStandardized.ToString()) }, MenuLevel = 1 };

            var childViewCanBeSelected = StaffModel.ViewType.AssessmentDetails == viewType && !string.IsNullOrEmpty(assessmentSubType) && assessmentSubType == StaffModel.AssessmentSubType.StateStandardized.ToString() && !string.IsNullOrEmpty(subjectArea);

            stateStatndardizedMenu.ChildViews.Add(new StaffModel.View { Description = "- " + ElaReading + " (State)", Selected = childViewCanBeSelected && subjectArea == StaffModel.SubjectArea.ELA.ToString(), Link = new Link { Href = staffLinks.AssessmentDetails(schoolId, staffUSI, fullName, sectionOrCohortIdForLink, studentListType, StaffModel.SubjectArea.ELA.ToString(), StaffModel.AssessmentSubType.StateStandardized.ToString()) }, MenuLevel = 2, MenuSubType = StaffModel.AssessmentSubType.StateStandardized.ToString() });
            stateStatndardizedMenu.ChildViews.Add(new StaffModel.View { Description = "- " + Writing + " (State)", Selected = childViewCanBeSelected && subjectArea == StaffModel.SubjectArea.Writing.ToString(), Link = new Link { Href = staffLinks.AssessmentDetails(schoolId, staffUSI, fullName, sectionOrCohortIdForLink, studentListType, StaffModel.SubjectArea.Writing.ToString(), StaffModel.AssessmentSubType.StateStandardized.ToString()) }, MenuLevel = 2, MenuSubType = StaffModel.AssessmentSubType.StateStandardized.ToString() });
            stateStatndardizedMenu.ChildViews.Add(new StaffModel.View { Description = "- " + Mathematics + " (State)", Selected = childViewCanBeSelected && subjectArea == StaffModel.SubjectArea.Mathematics.ToString(), Link = new Link { Href = staffLinks.AssessmentDetails(schoolId, staffUSI, fullName, sectionOrCohortIdForLink, studentListType, StaffModel.SubjectArea.Mathematics.ToString(), StaffModel.AssessmentSubType.StateStandardized.ToString()) }, MenuLevel = 2, MenuSubType = StaffModel.AssessmentSubType.StateStandardized.ToString() });
            stateStatndardizedMenu.ChildViews.Add(new StaffModel.View { Description = "- " + Science + " (State)", Selected = childViewCanBeSelected && subjectArea == StaffModel.SubjectArea.Science.ToString(), Link = new Link { Href = staffLinks.AssessmentDetails(schoolId, staffUSI, fullName, sectionOrCohortIdForLink, studentListType, StaffModel.SubjectArea.Science.ToString(), StaffModel.AssessmentSubType.StateStandardized.ToString()) }, MenuLevel = 2, MenuSubType = StaffModel.AssessmentSubType.StateStandardized.ToString() });
            stateStatndardizedMenu.ChildViews.Add(new StaffModel.View { Description = "- " + SocialStudies + " (State)", Selected = childViewCanBeSelected && subjectArea == StaffModel.SubjectArea.SocialStudies.ToString(), Link = new Link { Href = staffLinks.AssessmentDetails(schoolId, staffUSI, fullName, sectionOrCohortIdForLink, studentListType, StaffModel.SubjectArea.SocialStudies.ToString(), StaffModel.AssessmentSubType.StateStandardized.ToString()) }, MenuLevel = 2, MenuSubType = StaffModel.AssessmentSubType.StateStandardized.ToString() });

            return stateStatndardizedMenu;
        }

        public virtual StaffModel.View GetDistrictBenchmarkView(IStaffAreaLinks staffLinks, int schoolId, long staffUSI, string fullName, long? sectionOrCohortIdForLink, string studentListType, StaffModel.ViewType viewType, string assessmentSubType, string subjectArea)
        {
            //build the top level benchmark view
            var benchmarkMenu = new StaffModel.View { Description = DistrictBenchmark, Selected = false, Link = new Link { Href = staffLinks.AssessmentDetails(schoolId, staffUSI, fullName, sectionOrCohortIdForLink, studentListType, null, StaffModel.AssessmentSubType.Benchmark.ToString()) }, MenuLevel = 1 };

            var childViewCanBeSelected = StaffModel.ViewType.AssessmentDetails == viewType && !string.IsNullOrEmpty(assessmentSubType) && assessmentSubType == StaffModel.AssessmentSubType.Benchmark.ToString() && !string.IsNullOrEmpty(subjectArea);

            benchmarkMenu.ChildViews.Add(new StaffModel.View { Description = "- " + ElaReading + " (District)", Selected = childViewCanBeSelected && subjectArea == StaffModel.SubjectArea.ELA.ToString(), Link = new Link { Href = staffLinks.AssessmentDetails(schoolId, staffUSI, fullName, sectionOrCohortIdForLink, studentListType, StaffModel.SubjectArea.ELA.ToString(), StaffModel.AssessmentSubType.Benchmark.ToString()) }, MenuLevel = 2, MenuSubType = StaffModel.AssessmentSubType.Benchmark.ToString() });
            benchmarkMenu.ChildViews.Add(new StaffModel.View { Description = "- " + Writing + " (District)", Selected = childViewCanBeSelected && subjectArea == StaffModel.SubjectArea.Writing.ToString(), Link = new Link { Href = staffLinks.AssessmentDetails(schoolId, staffUSI, fullName, sectionOrCohortIdForLink, studentListType, StaffModel.SubjectArea.Writing.ToString(), StaffModel.AssessmentSubType.Benchmark.ToString()) }, MenuLevel = 2, MenuSubType = StaffModel.AssessmentSubType.Benchmark.ToString() });
            benchmarkMenu.ChildViews.Add(new StaffModel.View { Description = "- " + Mathematics + " (District)", Selected = childViewCanBeSelected && subjectArea == StaffModel.SubjectArea.Mathematics.ToString(), Link = new Link { Href = staffLinks.AssessmentDetails(schoolId, staffUSI, fullName, sectionOrCohortIdForLink, studentListType, StaffModel.SubjectArea.Mathematics.ToString(), StaffModel.AssessmentSubType.Benchmark.ToString()) }, MenuLevel = 2, MenuSubType = StaffModel.AssessmentSubType.Benchmark.ToString() });
            benchmarkMenu.ChildViews.Add(new StaffModel.View { Description = "- " + Science + " (District)", Selected = childViewCanBeSelected && subjectArea == StaffModel.SubjectArea.Science.ToString(), Link = new Link { Href = staffLinks.AssessmentDetails(schoolId, staffUSI, fullName, sectionOrCohortIdForLink, studentListType, StaffModel.SubjectArea.Science.ToString(), StaffModel.AssessmentSubType.Benchmark.ToString()) }, MenuLevel = 2, MenuSubType = StaffModel.AssessmentSubType.Benchmark.ToString() });
            benchmarkMenu.ChildViews.Add(new StaffModel.View { Description = "- " + SocialStudies + " (District)", Selected = childViewCanBeSelected && subjectArea == StaffModel.SubjectArea.SocialStudies.ToString(), Link = new Link { Href = staffLinks.AssessmentDetails(schoolId, staffUSI, fullName, sectionOrCohortIdForLink, studentListType, StaffModel.SubjectArea.SocialStudies.ToString(), StaffModel.AssessmentSubType.Benchmark.ToString()) }, MenuLevel = 2, MenuSubType = StaffModel.AssessmentSubType.Benchmark.ToString() });

            return benchmarkMenu;
        }

        public virtual StaffModel.View GetReadingAssessmentView(IStaffAreaLinks staffLinks, int schoolId, long staffUSI, string fullName, long? sectionOrCohortIdForLink, string studentListType, StaffModel.ViewType viewType, string assessmentSubType, string subjectArea)
        {
            return null;
        }
    }
}
