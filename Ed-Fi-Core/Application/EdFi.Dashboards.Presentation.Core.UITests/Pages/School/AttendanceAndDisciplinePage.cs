using EdFi.Dashboards.Presentation.Core.Areas.School.Controllers;
using EdFi.Dashboards.Presentation.Core.UITests.Attributes;
using EdFi.Dashboards.Presentation.Core.UITests.Support;

namespace EdFi.Dashboards.Presentation.Core.UITests.Pages.School
{
    [AssociatedController(typeof(DomainMetricController))]
    public class AttendanceAndDisciplinePage : MetricsPage
    {
        /// <summary>
        /// Gets the menu text representing the "Attendance and Discipline" section.
        /// </summary>
        protected virtual string AttendanceAndDisciplineMenuText
        {
            get { return "Attendance and Discipline"; }
        }

        public override void Visit(bool forceNavigation = false)
        {
            if (!IsCurrent() || forceNavigation)
                VisitAcademicDashboardSection(AcademicDashboardType.School, AttendanceAndDisciplineMenuText);
        }
    }
}
