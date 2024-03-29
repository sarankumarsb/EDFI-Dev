using EdFi.Dashboards.Presentation.Core.UITests.Pages.LocalEducationAgency;
using TechTalk.SpecFlow;

namespace EdFi.Dashboards.Presentation.Core.UITests.Steps.LocalEducationAgency
{
    [Binding]
    public class AttendanceAndDisciplineSteps
    {
        private readonly AttendanceAndDisciplinePage attendanceAndDisciplinePage;

        public AttendanceAndDisciplineSteps(AttendanceAndDisciplinePage attendanceAndDisciplinePage)
        {
            this.attendanceAndDisciplinePage = attendanceAndDisciplinePage;
        }

        // Add attendance-specific steps here, as necessary
    }
}
