// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using EdFi.Dashboards.Presentation.Architecture.Mvc.Controllers;
using EdFi.Dashboards.Resources.Metric;
using EdFi.Dashboards.Resources.Metric.Requests;

namespace EdFi.Dashboards.Presentation.Core.Areas.StudentSchool.Controllers
{
    public class DomainMetricController : DomainMetricController<StudentSchoolMetricInstanceSetRequest>
    {
        public DomainMetricController(IDomainMetricService<StudentSchoolMetricInstanceSetRequest> domainMetricService)
            : base(domainMetricService)
        {
        }
    }
}