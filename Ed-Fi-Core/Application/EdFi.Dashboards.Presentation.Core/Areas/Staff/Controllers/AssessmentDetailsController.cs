// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using EdFi.Dashboards.Presentation.Core.Areas.Staff.Models.Shared;
using EdFi.Dashboards.Common;
using EdFi.Dashboards.Resources;
using EdFi.Dashboards.Resources.Models.Common;
using EdFi.Dashboards.Resources.Models.CustomGrid;
using EdFi.Dashboards.Resources.Models.Staff;
using EdFi.Dashboards.Resources.Models.Student;
using EdFi.Dashboards.Resources.Staff;
using EdFi.Dashboards.SecurityTokenService.Authentication;

namespace EdFi.Dashboards.Presentation.Core.Areas.Staff.Controllers
{
    public class AssessmentDetailsController : Controller
    {
        private readonly IService<AssessmentDetailsRequest, AssessmentDetailsModel> assessmentDetailsService;

        public AssessmentDetailsController(
            IService<AssessmentDetailsRequest, AssessmentDetailsModel> assessmentDetailsService)
        {
            this.assessmentDetailsService = assessmentDetailsService;
        }

        public ActionResult Get(long staffUSI, int schoolId, string studentListType, long? sectionOrCohortId,
                                int localEducationAgencyId, string subjectArea, string assessmentSubType)
        {
            var request = new AssessmentDetailsRequest()
                              {
                                  StaffUSI = staffUSI,
                                  SchoolId = schoolId,
                                  SectionOrCohortId = sectionOrCohortId ?? 0,
                                  StudentListType = studentListType,
                                  MetricAssessmentArea = subjectArea,
                                  AssessmentSubType = assessmentSubType
                              };

            var results = assessmentDetailsService.Get(request);

            //Constructing the Grid Data. 
            var model = new StaffStudentListModel{ AssessmentSubType = assessmentSubType, SubjectArea = subjectArea};

            //If the object titles that comes back in our results = 0, this means we have no results.
            //  So instead of creating a GridTable, we are just going to display text that No Data is Available.
            if (results.ObjectiveTitles.Count == 0)
                return View(model);

            model.GridTable = new GridTable();

            //Creating the columns
            //First the columns for the student Name and MetricValue
            model.GridTable.Columns.Add(new ImageColumn { Src = "LeftGrayCorner.png", IsVisibleByDefault = true, IsFixedColumn = true});
            model.GridTable.Columns.Add(new TextColumn { DisplayName = "Student", IsVisibleByDefault = true, IsFixedColumn = true });
            model.GridTable.Columns.Add(new TextColumn { DisplayName = "Grade Level", IsVisibleByDefault = true, IsFixedColumn = true });

            var sortAsc = String.Empty;
            var sortDesc = String.Empty;

            if (assessmentSubType == StaffModel.AssessmentSubType.StateStandardized.ToString() || assessmentSubType == StaffModel.AssessmentSubType.Benchmark.ToString())
            {
                model.GridTable.Columns.Add(new TextColumn { DisplayName = results.MetricTitle, IsVisibleByDefault = true, SortAscending = "sortStateAssessmentValueAsc", SortDescending = "sortStateAssessmentValueDesc", IsFixedColumn = true });
            }
            else
            {
                sortAsc = "sortMetricStateAsc";
                sortDesc = "sortMetricStateDesc";
            }

            foreach (var ot in results.ObjectiveTitles)
                model.GridTable.Columns.Add(new TextColumn { DisplayName = ot.Title, IsVisibleByDefault = true, SortAscending = sortAsc, SortDescending = sortDesc, Tooltip = ot.Description, IsFixedColumn = false, OverriddenWidth = ot.Width });

            model.GridTable.Columns.Add(new ImageColumn { Src = "RightGrayCorner.png", IsVisibleByDefault = true, IsFixedColumn = false });

            // Create the fixed row
            var fixedRow = new List<object>();
            fixedRow.Add(new CellItem<double> { DV = string.Empty, V = 0 });
            fixedRow.Add(new CellItem<string> { DV = results.FixedRowTitle, V = results.FixedRowTitle });
            fixedRow.Add(new CellItem<string> { DV = string.Empty, V = string.Empty });

            if (assessmentSubType == StaffModel.AssessmentSubType.StateStandardized.ToString())
            {
                fixedRow.Add(new CellItem<string> { DV = string.Empty, V = String.Empty });
            }

            if (assessmentSubType == StaffModel.AssessmentSubType.Benchmark.ToString())
            {
                fixedRow.Add(new CellItem<string> { DV = string.Empty, V = String.Empty });
            }

            foreach (var ot in results.ObjectiveTitles)
            {
                fixedRow.Add(new CellItem<string>{ DV = ot.Mastery, V = ot.Mastery});
            }

            fixedRow.Add(new CellItem<double> { DV = string.Empty, V = 0 });
            model.GridTable.FixedRows.Add(fixedRow);


            //Create the rows.
            foreach (var s in results.Students)
            {
                var row = new List<object>();

                //First cells (Spacer,Student,MetricValue)
                //Spacer
                row.Add(new CellItem<double> {DV = string.Empty, V = 0});
                row.Add(new StudentCellItem<string>(s.StudentUSI)
                            {
                                V = s.Name,
                                DV = s.Name,
                                I = s.ThumbNail,
                                LUId = results.UniqueListId,
                                CId = s.SchoolId,
                                //Lets go and resolve the metric to a child one...
                                Url = s.Href != null ? s.Href.Href : null,
                                Links = s.Links
                            });
                row.Add(new CellItem<int> { DV = s.GradeLevelDisplayValue, V = s.GradeLevel });

                if (assessmentSubType == StaffModel.AssessmentSubType.StateStandardized.ToString())
                {
                    if (s.Score != null && s.Score.Value != null)
                    {
                        var additionalMetric = s.Score;
                        var assessmentCellType =
                            typeof (AssessmentMetricCellItem<>).MakeGenericType(new Type[]
                                                                                    {additionalMetric.Value.GetType()});
                        dynamic stateAssessmentCellForMetricValue = Activator.CreateInstance(assessmentCellType);
                        stateAssessmentCellForMetricValue.V = additionalMetric.Value;
                        stateAssessmentCellForMetricValue.DV = additionalMetric.DisplayValue;
                            stateAssessmentCellForMetricValue.A = (int)additionalMetric.MetricIndicator;
                            stateAssessmentCellForMetricValue.S = (int)additionalMetric.State;
                        row.Add(stateAssessmentCellForMetricValue);
                    }
                    else
                    {
                        row.Add(new AssessmentMetricCellItem<string> { DV = string.Empty, V = string.Empty });
                    }
                }

                if (assessmentSubType == StaffModel.AssessmentSubType.Benchmark.ToString())
                {
                    if (s.Score != null && s.Score.Value != null)
                    {
                        row.Add(new MetricCellItem<string>
                                    {
                                        DV = s.Score.DisplayValue,
                                        V = s.Score.Value.ToString(),
                                        S = (int)s.Score.State
                                    });
                    }
                    else
                    {
                        row.Add(new MetricCellItem<string> { DV = string.Empty, V = string.Empty });
                    }
                    
                }

                //Dynamic cells
                //Create the space holders so that the rows are of equal length.
                foreach (var o in results.ObjectiveTitles)
                {
                    var cell = new ObjectiveCellItem<int> {DV = String.Empty, V = -1};

                    var cellData = (from ro in s.Metrics.OfType<StudentWithMetricsAndAssessments.AssessmentMetric>()
                                    where
                                        !string.IsNullOrEmpty(o.Title) &&
                                        string.Compare(o.Title, ro.ObjectiveName, true) == 0
                                    select ro).FirstOrDefault();

                    if (cellData != null)
                    {
                        cell.V = cellData.Value;
                        cell.ST = (int) cellData.State;
                        cell.DV = cellData.DisplayValue;
                    }

                    row.Add(cell);
                }

                //Spacer for the right corner
                row.Add(new CellItem<double> {DV = String.Empty, V = 0});
                model.GridTable.Rows.Add(row);
            }

            //if this is the current users own page
            model.IsCurrentUserListOwner = UserInformation.Current.StaffUSI == staffUSI;
            model.IsCustomStudentList = request.StudentListType == StudentListType.CustomStudentList.ToString();
            model.ListId = request.SectionOrCohortId;

            if (assessmentSubType == StaffModel.AssessmentSubType.StateStandardized.ToString())
            {
                model.LegendViewNames = new List<string> { "Default", "AssessmentDetail"};
            }
            else if (assessmentSubType == StaffModel.AssessmentSubType.Benchmark.ToString())
            {
                model.LegendViewNames = new List<string> { "AssessmentDetail" };
            }
            else if (assessmentSubType == StaffModel.AssessmentSubType.Reading.ToString())
            {
                if (subjectArea == "TPRI")
                {
                    model.LegendViewNames = new List<string> { "ReadingInventoryAssessmentDetail" };
                }
                else if (subjectArea == "TejasLEE")
                {
                    model.LegendViewNames = new List<string> { "LanguageAssessmentDetail" };
                }
            }

            return View(model);
        }
    }
}
