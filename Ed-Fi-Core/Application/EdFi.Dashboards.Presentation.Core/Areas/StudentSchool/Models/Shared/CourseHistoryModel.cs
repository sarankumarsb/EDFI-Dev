// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using EdFi.Dashboards.Presentation.Core.Models.Shared;

namespace EdFi.Dashboards.Presentation.Core.Areas.StudentSchool.Models.Shared
{
    public class CourseHistoryModel
    {
        public CourseHistoryModel()
        {
            CollapseExpandInitialState = ExpandCollapseModel.InitialState.Expanded;
            ShowJump = true;
        }

        public Resources.Models.Student.Detail.CourseHistory.CourseHistoryModel Model { get; set; }
        public ExpandCollapseModel.InitialState CollapseExpandInitialState { get; set; }
        public bool ShowJump { get; set; }
        public int MetricVariantId { get; set; }
    }
}