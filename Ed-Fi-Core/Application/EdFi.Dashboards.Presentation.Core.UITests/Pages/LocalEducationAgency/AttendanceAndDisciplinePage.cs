using EdFi.Dashboards.Presentation.Core.Areas.LocalEducationAgency.Controllers;
using EdFi.Dashboards.Presentation.Core.UITests.Attributes;
using EdFi.Dashboards.Presentation.Core.UITests.Support;

namespace EdFi.Dashboards.Presentation.Core.UITests.Pages.LocalEducationAgency
{
    [AssociatedController(typeof(DomainMetricController))]
    public class AttendanceAndDisciplinePage : MetricsPage
    {
        public override void Visit(bool forceNavigation = false)
        {
            if (!IsCurrent() || forceNavigation)
                VisitAcademicDashboardSection(AcademicDashboardType.LocalEducationAgency, "Attendance and Discipline");
        }
    }
}
