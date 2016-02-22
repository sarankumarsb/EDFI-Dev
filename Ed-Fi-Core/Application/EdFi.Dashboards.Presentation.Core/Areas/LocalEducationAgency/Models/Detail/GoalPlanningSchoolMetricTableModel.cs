using System;
using System.Collections.Generic;
using System.Linq;
using EdFi.Dashboards.Application.Resources.LocalEducationAgency.Detail;
using EdFi.Dashboards.Resources.Models.CustomGrid;

namespace EdFi.Dashboards.Presentation.Web.Areas.LocalEducationAgency.Models.Detail
{
    public class GoalPlanningSchoolMetricTableModel
    {
        public GridTable GridTable { get; set; }
        public EdFi.Dashboards.Application.Resources.Models.LocalEducationAgency.Detail.GoalPlanningSchoolListModel GoalPlanning { get; set; }
        public GoalPlanningSchoolListGetRequest.SchoolMetric[] SchoolMetrics { get; set; }
    }
}