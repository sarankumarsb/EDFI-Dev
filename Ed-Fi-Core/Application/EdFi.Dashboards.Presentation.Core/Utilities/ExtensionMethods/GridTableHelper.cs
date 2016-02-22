// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using EdFi.Dashboards.Metric.Resources.Models;
using EdFi.Dashboards.Presentation.Core.Areas.Staff.Models.Shared;
using EdFi.Dashboards.Presentation.Core.Models.Shared;
using EdFi.Dashboards.Resource.Models.Common;
using EdFi.Dashboards.Resources.Models.CustomGrid;
using EdFi.Dashboards.Resources.Models.LocalEducationAgency.Detail;
using EdFi.Dashboards.Resources.Models.School;
using EdFi.Dashboards.Resources.Models.Student;
using EdFi.Dashboards.Resources.Navigation;
using EdFi.Dashboards.Warehouse.Resource.Models.LocalEducationAgency.Detail;

namespace EdFi.Dashboards.Presentation.Web.Utilities
{
    public static class StaffStudentListModelHelper
    {
        public static string GetDefaultNonPersistedSettings(this StaffStudentListModel model, string edFiGridTable)
        {
            string gridTableDefaultNonPersistedSettings = model.GridTable.GetDefaultNonPersistedSettings(edFiGridTable);
            string defaultNonPersistedSettings = gridTableDefaultNonPersistedSettings;

            if (gridTableDefaultNonPersistedSettings.Equals("null;"))
            {
                defaultNonPersistedSettings = gridTableDefaultNonPersistedSettings.Replace("null;",
                                                                                           string.Format(
                                                                                               "{{ isOwner: {0}, isCustomList: {1}, listId: {2} }};",
                                                                                               model.IsCurrentUserListOwner.ToString().ToLower(),
                                                                                                  model.IsCustomStudentList.ToString().ToLower(),
                                                                                                  model.ListId));
            }
            else if (gridTableDefaultNonPersistedSettings.Contains("columnNumber"))
            {
                defaultNonPersistedSettings = gridTableDefaultNonPersistedSettings.Insert(1,
                                                                                          string.Format(
                                                                                              " isOwner: {0}, isCustomList: {1}, listId: {2}, ",
                                                                                              model.IsCurrentUserListOwner.ToString().ToLower(),
                                                                                                  model.IsCustomStudentList.ToString().ToLower(),
                                                                                                  model.ListId));
            }
            return defaultNonPersistedSettings;
        }
    }

    public static class GridTableHelper
    {
        public static string GetDefaultNonPersistedSettings(this GridTable model, string edFiGridTable)
        {
            var sb = new StringBuilder();
            bool hasCustomSort = false;
            int i = 0;
            foreach (var column in model.Columns)
            {
                //We have a 2 row column. so lets do the child ones.
                if (column.Children.Count > 0)
                    foreach (var subColumn in column.Children)
                    {
                        if (!string.IsNullOrEmpty(subColumn.SortAscending))
                        {
                            if (hasCustomSort)
                                sb.Append(", ");
                            sb.AppendFormat("{{ columnNumber: {0}, sortAsc: {3}.{1}, sortDesc: {3}.{2} }}", i,
                                            subColumn.SortAscending, subColumn.SortDescending, edFiGridTable);
                            hasCustomSort = true;
                        }
                        i++;
                    }
                else
                {
                    if (!string.IsNullOrEmpty(column.SortAscending))
                    {
                        if (hasCustomSort)
                            sb.Append(", ");
                        sb.AppendFormat("{{ columnNumber: {0}, sortAsc: {3}.{1}, sortDesc: {3}.{2} }}", i,
                                        column.SortAscending, column.SortDescending, edFiGridTable);
                        hasCustomSort = true;
                    }
                    i++;
                }
            }

            var result = hasCustomSort ? string.Format("{{ customSort: new Array({0}) }}", sb) : "null";
            return result;
        }

        private static Type[] GetTypeArray(Type type)
        {
            return new[] { type };
        }

        #region Header
        public static List<Column> GenerateHeader(this List<MetadataColumnGroup> metadataColumnGroups)
        {
            //Lets analyze the metadata to see what we have. 
            //Note: Today we only support 2 options: a single row header or a double row/grouped header.
            if (metadataColumnGroups.Count > 1)
                return GenerateDoubleRowHeader(metadataColumnGroups);

            return GenerateSingleRowHeader(metadataColumnGroups);
        }

        private static List<Column> GenerateSingleRowHeader(IEnumerable<MetadataColumnGroup> metadataColumnGroups)
        {
            var header = new List<Column>();

            //Creating the columns
            //First row and last are hardcode because of the way the JQuery grid is implemented today.
            header.Add(new ImageColumn { Src = "LeftGrayCorner.png", IsVisibleByDefault = true, IsFixedColumn = true });

            var columns = metadataColumnGroups.First().Columns;

            foreach (var metadataColumn in columns)
                header.Add(GenerateColumn(metadataColumn, metadataColumn.IsFixedColumn));
            //header.Add(GenerateColumn(metadataColumn, metadataColumnGroups.First().IsFixedColumnGroup));

            header.Add(new ImageColumn { Src = "RightGrayCorner.png", IsVisibleByDefault = true });

            return header;
        }

        private static List<Column> GenerateDoubleRowHeader(IEnumerable<MetadataColumnGroup> metadataColumnGroups)
        {
            var header = new List<Column>();

            int groupIndex = 0;
            int groupCount = metadataColumnGroups.Count();
            foreach (var group in metadataColumnGroups)
            {
                var topHeaderGroup = new Column();

                //Special handling for Entity Information group.
                if (group.GroupType == GroupType.EntityInformation)
                {
                    topHeaderGroup = new Column { U = -1, IsVisibleByDefault = true, IsFixedColumn = group.IsFixedColumnGroup };
                    //Note: Hardcoded left image for the JQuery grid component.
                    topHeaderGroup.Children.Add(new ImageColumn
                    {
                        U = -1,
                        Src = "LeftGrayCorner.png",
                        IsVisibleByDefault = true,
                        IsFixedColumn = group.IsFixedColumnGroup
                    });
                }

                if (group.GroupType == GroupType.MetricData)
                    topHeaderGroup = new TextColumn
                    {
                        U = -1,
                        DisplayName = group.Title,
                        IsVisibleByDefault = group.IsVisibleByDefault,
                        IsFixedColumn = group.IsFixedColumnGroup
                    };

                foreach (var column in group.Columns)
                    topHeaderGroup.Children.Add(GenerateColumn(column, group.IsFixedColumnGroup));

                header.Add(topHeaderGroup);

                //If we are between columns but not in the last one or this is not the only one. then we add a spacer.
                //Note: this can be removed when we change the way the JQueryGrid renders this data.
                if (groupIndex < groupCount - 1)
                    header.Add(HeaderSpacer(group.IsFixedColumnGroup));

                groupIndex++;
            }

            return header;
        }

        private static Column GenerateColumn(MetadataColumn column, bool isFixedColumn)
        {
            if (string.Equals(column.ColumnName, "Flag", StringComparison.OrdinalIgnoreCase))
                return new ImageColumn { Src = "FlagWhite.gif", IsVisibleByDefault = true, IsFixedColumn = isFixedColumn };

            return new TextColumn
            {
                U = column.UniqueIdentifier,
                DisplayName = column.ColumnName,
                IsVisibleByDefault = column.IsVisibleByDefault,
                SortAscending = column.SortAscending,
                SortDescending = column.SortDescending,
                Tooltip = column.Tooltip,
                IsFixedColumn = isFixedColumn
            };
        }

        private static TextColumn HeaderSpacer(bool isFixedColumn)
        {
            return new TextColumn
            {
                U = -1,
                DisplayName = "Spacer",
                IsVisibleByDefault = true,
                IsFixedColumn = isFixedColumn,
                Children = new List<Column>
                                          {
                                              new TextColumn {U = -1, IsVisibleByDefault = true, IsFixedColumn = isFixedColumn},
                                          },
            };
        }

        #endregion

        #region Rows

        public static List<List<object>> GenerateRows(this List<MetadataColumnGroup> metadataColumnGroups, IEnumerable<StaffModel.StaffMember> staff, int schoolId)
        {
            var rows = new List<List<object>>();

            foreach (var person in staff)
            {
                var row = new List<object>();

                //First cells (Spacer, Staff, Email, Metric)
                row.Add(new CellItem<double> { DV = "", V = 0 });
                row.Add(new StaffCellItem<string>
                {
                    V = person.Name,
                    DV = person.Name,
                    StId = person.StaffUSI,
                    I = person.ProfileThumbnail,
                    CId = schoolId,
                });

                var metaDataColumns = metadataColumnGroups.Any(x => string.IsNullOrEmpty(x.Title))
                    ? metadataColumnGroups.First(x => string.IsNullOrEmpty(x.Title)).Columns
                    : new List<MetadataColumn>();

                foreach (var metadataColumn in metaDataColumns)
                {
                    if (String.Equals(metadataColumn.ColumnName, "E-Mail", StringComparison.OrdinalIgnoreCase))
                    {
                        row.Add(new EmailCellItem<string> { V = person.EmailAddress, M = person.EmailAddress });
                    }
                    if (String.Equals(metadataColumn.ColumnName, "Date of Birth", StringComparison.OrdinalIgnoreCase))
                    {
                        row.Add(new CellItem<long?>
                        {
                            V = (person.DateOfBirth != null) ? person.DateOfBirth.Value.Ticks : 0,
                            DV = (person.DateOfBirth != null) ? person.DateOfBirth.Value.ToShortDateString() : ""
                        });
                    }
                    if (String.Equals(metadataColumn.ColumnName, "Gender", StringComparison.OrdinalIgnoreCase))
                    {
                        row.Add(new CellItem<string> { V = person.Gender, DV = person.Gender });
                    }
                }


                metaDataColumns = metadataColumnGroups.Any(x => x.Title.Equals("ROLE", StringComparison.InvariantCultureIgnoreCase))
                    ? metadataColumnGroups.First(x => x.Title.Equals("ROLE", StringComparison.InvariantCultureIgnoreCase)).Columns
                    : new List<MetadataColumn>();

                if (metaDataColumns.Count > 0)
                    //Spacer
                    row.Add(new SpacerCellItem<double> { DV = "", V = 0 });

                foreach (var metadataColumn in metaDataColumns)
                {
                    if (String.Equals(metadataColumn.ColumnName, "School Role", StringComparison.OrdinalIgnoreCase))
                    {
                        row.Add(new CellItem<string> { V = person.StaffCategory, DV = person.StaffCategory });
                    }
                }

                metaDataColumns = metadataColumnGroups.Any(x => x.Title.Equals("EXPERIENCE", StringComparison.InvariantCultureIgnoreCase))
                    ? metadataColumnGroups.First(x => x.Title.Equals("EXPERIENCE", StringComparison.InvariantCultureIgnoreCase)).Columns
                    : new List<MetadataColumn>();

                if (metaDataColumns.Count > 0)
                    //Spacer
                    row.Add(new SpacerCellItem<double> { DV = "", V = 0 });

                foreach (var metadataColumn in metaDataColumns)
                {
                    if (String.Equals(metadataColumn.ColumnName, "Experience", StringComparison.OrdinalIgnoreCase))
                    {
                        row.Add(new YearsOfExperienceCellItem<decimal?>
                            {
                                V = person.YearsOfPriorProfessionalExperience,
                                Y = person.YearsOfPriorProfessionalExperience
                            });
                    }
                }

                metaDataColumns = metadataColumnGroups.Any(x => x.Title.Equals("EDUCATION", StringComparison.InvariantCultureIgnoreCase))
                    ? metadataColumnGroups.First(x => x.Title.Equals("EDUCATION", StringComparison.InvariantCultureIgnoreCase)).Columns
                    : new List<MetadataColumn>();

                if (metaDataColumns.Count > 0)
                    //Spacer
                    row.Add(new SpacerCellItem<double> { DV = "", V = 0 });

                foreach (var metadataColumn in metaDataColumns)
                {
                    if (String.Equals(metadataColumn.ColumnName, "Education", StringComparison.OrdinalIgnoreCase))
                    {
                        row.Add(new HighestLevelOfEducationCellItem<string> { V = person.HighestLevelOfEducationCompleted, E = person.HighestLevelOfEducationCompleted });
                    }
                    if (String.Equals(metadataColumn.ColumnName, "Highly Qualified Teacher", StringComparison.OrdinalIgnoreCase))
                        {
                            row.Add(new CellItem<string> { V = person.HighlyQualifiedTeacher, DV = person.HighlyQualifiedTeacher });
                        }
                }
               
                rows.Add(row);
            }
            return rows;
        }


        public static List<List<object>> GenerateRows(this List<MetadataColumnGroup> metadataColumnGroups, List<TeachersModel.Teacher> teachers, int schoolId)
        {
            var rows = new List<List<object>>();

            foreach (var t in teachers)
            {
                var row = new List<object>();

                //First cells (Spacer, Staff, Email, Metric)
                row.Add(new CellItem<double> { DV = "", V = 0 });
                row.Add(new TeacherCellItem<string>
                {
                    V = t.Name,
                    DV = t.Name,
                    TId = t.StaffUSI,
                    I = t.ProfileThumbnail,
                    CId = schoolId,
                    Url = t.Url
                });

                var metaDataColumns = metadataColumnGroups.Any(x => string.IsNullOrEmpty(x.Title))
                    ? metadataColumnGroups.First(x => string.IsNullOrEmpty(x.Title)).Columns
                    : new List<MetadataColumn>();

                foreach (var metadataColumn in metaDataColumns)
                {
                    if (String.Equals(metadataColumn.ColumnName, "E-Mail", StringComparison.OrdinalIgnoreCase))
                    {
                        row.Add(new EmailCellItem<string> { V = t.EmailAddress, M = t.EmailAddress });
                    }
                    if (String.Equals(metadataColumn.ColumnName, "Date of Birth", StringComparison.OrdinalIgnoreCase))
                    {
                        row.Add(new CellItem<long?>
                        {
                            V = (t.DateOfBirth != null) ? t.DateOfBirth.Value.Ticks : 0,
                            DV = (t.DateOfBirth != null) ? t.DateOfBirth.Value.ToShortDateString() : ""
                        });
                    }
                    if (String.Equals(metadataColumn.ColumnName, "Gender", StringComparison.OrdinalIgnoreCase))
                    {
                        row.Add(new CellItem<string> { V = t.Gender, DV = t.Gender });
                    }
                }

                metaDataColumns = metadataColumnGroups.Any(x => x.Title.Equals("EXPERIENCE",StringComparison.InvariantCultureIgnoreCase))
                    ? metadataColumnGroups.First(x => x.Title.Equals("EXPERIENCE", StringComparison.InvariantCultureIgnoreCase)).Columns
                    : new List<MetadataColumn>();

                if(metaDataColumns.Count > 0)
                    //Spacer
                    row.Add(new SpacerCellItem<double> { DV = "", V = 0 });

                foreach (var metadataColumn in metaDataColumns)
                {
                    if (String.Equals(metadataColumn.ColumnName, "Experience", StringComparison.OrdinalIgnoreCase))
                    {
                        row.Add(new YearsOfExperienceCellItem<decimal?>
                        {
                            V = t.YearsOfPriorProfessionalExperience,
                            Y = t.YearsOfPriorProfessionalExperience
                        });
                    }
                }

                metaDataColumns = metadataColumnGroups.Any(x => x.Title.Equals("EDUCATION", StringComparison.InvariantCultureIgnoreCase))
                    ? metadataColumnGroups.First(x => x.Title.Equals("EDUCATION", StringComparison.InvariantCultureIgnoreCase)).Columns
                    : new List<MetadataColumn>();
               
                if (metaDataColumns.Count > 0)
                    //Spacer
                    row.Add(new SpacerCellItem<double> { DV = "", V = 0 });

                foreach (var metadataColumn in metaDataColumns)
                {
                    if (String.Equals(metadataColumn.ColumnName, "Education", StringComparison.OrdinalIgnoreCase))
                    {
                        row.Add(new HighestLevelOfEducationCellItem<string> { V = t.HighestLevelOfEducationCompleted, E = t.HighestLevelOfEducationCompleted });
                    }
                    if (String.Equals(metadataColumn.ColumnName, "Highly Qualified Teacher", StringComparison.OrdinalIgnoreCase))
                    {
                        row.Add(new CellItem<string> { V = t.HighlyQualifiedTeacher, DV = t.HighlyQualifiedTeacher });
                    }
                }
                rows.Add(row);
            }
            return rows;
        }

        public static List<List<object>> GenerateRows(this List<MetadataColumnGroup> metadataColumnGroups, List<StudentWithMetrics> students, string uniqueListId)
        {
            //Sample to see if we have multiple rows or only one row.
            return (metadataColumnGroups.Count > 1) ? GenerateMultipleGroupRows(metadataColumnGroups, students, uniqueListId, false) : GenerateSingleGroupRows(metadataColumnGroups, students, uniqueListId);
        }

        public static List<List<object>> GenerateRowsForExport(this List<MetadataColumnGroup> metadataColumnGroups,
            List<StudentWithMetrics> students)
        {
            var generatedRows = metadataColumnGroups.GenerateRows(students, string.Empty);
            var rows = generatedRows.Select(row => row.Where(cell => !cell.GetType().Name.Contains("SpacerCellItem")).ToList()).ToList();

            return rows;
        }

        private static Type CreateGenericType(Type generic, Type innerType)
        {
            return generic.MakeGenericType(new[] { innerType });
        }

        public static List<List<Object>> GenerateRows(this List<MetadataColumnGroup> metadataColumnGroups, List<GoalPlanningSchoolMetric> schoolMetrics)
        {
            var rows = new List<List<object>>();
            foreach (var schoolMetricModel in schoolMetrics)
            {
                var row = new List<object>();

                // left rounded corner
                row.Add(new SpacerCellItem<double> { DV = String.Empty, V = 0 });

                foreach (var metadataColumn in metadataColumnGroups.First().Columns)
                {
                    if (string.Equals(metadataColumn.ColumnName, "School", StringComparison.OrdinalIgnoreCase))
                    {
                        row.Add(new SchoolCellItem<string>
                        {
                            DV = schoolMetricModel.Name,
                            V = schoolMetricModel.Name,
                            CId = schoolMetricModel.SchoolId,
                            Url = schoolMetricModel.Href.Href,
                            Links = new List<Link> { schoolMetricModel.MetricContextLink }
                        });
                    }

                    if (string.Equals(metadataColumn.ColumnName, "Principal", StringComparison.OrdinalIgnoreCase))
                    {
                        // principal name
                        if (String.IsNullOrEmpty(schoolMetricModel.Principal))
                            row.Add(new CellItem<string> { DV = String.Empty, V = String.Empty });
                        else
                            row.Add(new CellItem<string> { DV = schoolMetricModel.Principal, V = schoolMetricModel.Principal });
                    }

                    if (string.Equals(metadataColumn.ColumnName, "Type", StringComparison.OrdinalIgnoreCase))
                    {
                        // school category
                        row.Add(new CellItem<string> { DV = schoolMetricModel.SchoolCategory, V = schoolMetricModel.SchoolCategory });
                    }

                    //For the metric Value.
                    Type valueType = Type.GetType(schoolMetricModel.ValueType);
                    dynamic rowForMetricValue = Activator.CreateInstance(CreateGenericType(typeof(StateTextCellItem<>), valueType));

                    if (string.Equals(metadataColumn.ColumnName, "School Metric Value", StringComparison.OrdinalIgnoreCase))
                    {

                        if (schoolMetricModel.Value.HasValue)
                            rowForMetricValue.V = schoolMetricModel.Value.Value;
                        else
                            rowForMetricValue.V = schoolMetricModel.DisplayValue;
                        rowForMetricValue.DV = schoolMetricModel.DisplayValue;
                        row.Add(rowForMetricValue);
                    }

                    if (string.Equals(metadataColumn.ColumnName, "Current School Goal", StringComparison.OrdinalIgnoreCase))
                    {
                        //For the school Goal.
                        row.Add(new CellItem<double> { DV = string.Format("{0:P1}", schoolMetricModel.Goal), V = schoolMetricModel.Goal });
                    }

                    if (string.Equals(metadataColumn.ColumnName, "Difference From Goal", StringComparison.OrdinalIgnoreCase))
                    {
                        // metric difference from goal
                        if (schoolMetricModel.GoalDifference.HasValue)
                        {
                            row.Add(new StateTextCellItem<double>
                            {
                                DV = string.Format("{0:P1}", schoolMetricModel.GoalDifference.Value),
                                V = schoolMetricModel.GoalDifference.Value,
                                STe = (int)schoolMetricModel.MetricState.StateType,
                            });

                            rowForMetricValue.STe = (int)schoolMetricModel.MetricState.StateType;
                        }
                        else
                        {
							row.Add(new StateTextCellItem<string> { DV = String.Empty, V = null, STe = (int)MetricStateType.None });
                        }
                    }
                    if (string.Equals(metadataColumn.ColumnName, "New Goal", StringComparison.OrdinalIgnoreCase))
                    {
                        row.Add(new GoalPlanningCellItem<double> { G = schoolMetricModel.Goal, EId = schoolMetricModel.SchoolId, MIds = schoolMetricModel.GoalMetricIds.ToArray(), I = schoolMetricModel.StandardGoalInterpretation });
                    }
                }

                //Last cell for the right rounded corner...
                row.Add(new SpacerCellItem<double> { DV = String.Empty, V = 0 });
                rows.Add(row);
            }
            return rows;
        }

        public static List<List<Object>> GenerateRows(this List<MetadataColumnGroup> metadataColumnGroups, List<SchoolPriorYearMetricModel> priorYearSchoolMetrics)
        {
            var schoolMetrics = priorYearSchoolMetrics.Select(x =>
                                new EdFi.Dashboards.Resources.Models.LocalEducationAgency.Detail.SchoolMetricModel
                                    {
                                        SchoolId = x.SchoolId, 
                                        Name = x.Name, 
                                        Principal = x.Principal, 
                                        SchoolCategory = x.SchoolCategory,
                                        ValueType = x.ValueType,
                                        DisplayValue = x.DisplayValue,
                                        Value = x.Value,
                                        Goal = x.Goal,
                                        GoalDifference = x.GoalDifference,
                                        MetricState = x.MetricState,
                                        Href = x.Href,
                                        MetricContextLink = x.MetricContextLink,
                                        MetricLink = x.MetricLink
                                    }).ToList();
            return GenerateRows(metadataColumnGroups, schoolMetrics);
        }

        public static List<List<Object>> GenerateRows(this List<MetadataColumnGroup> metadataColumnGroups, List<SchoolMetricModel> schoolMetrics)
        {
            var rows = new List<List<object>>();
            foreach (var schoolMetricModel in schoolMetrics)
            {
                var row = new List<object>();

                // left rounded corner
                row.Add(new SpacerCellItem<double> { DV = String.Empty, V = 0 });

                foreach (var metadataColumn in metadataColumnGroups.First().Columns)
                {
                    if (string.Equals(metadataColumn.ColumnName, "School", StringComparison.OrdinalIgnoreCase))
                    {
                        row.Add(new SchoolCellItem<string>
                        {
                            DV = schoolMetricModel.Name,
                            V = schoolMetricModel.Name,
                            CId = schoolMetricModel.SchoolId,
                            Url = schoolMetricModel.Href.Href,
                            Links = new List<Link> { schoolMetricModel.MetricContextLink, schoolMetricModel.MetricLink }
                        });
                    }

                    if (string.Equals(metadataColumn.ColumnName, "Principal", StringComparison.OrdinalIgnoreCase))
                    {
                        // principal name
                        if (String.IsNullOrEmpty(schoolMetricModel.Principal))
                            row.Add(new CellItem<string> { DV = String.Empty, V = String.Empty });
                        else
                            row.Add(new CellItem<string> { DV = schoolMetricModel.Principal, V = schoolMetricModel.Principal });
                    }

                    if (string.Equals(metadataColumn.ColumnName, "Type", StringComparison.OrdinalIgnoreCase))
                    {
                        // school category
                        row.Add(new CellItem<string> { DV = schoolMetricModel.SchoolCategory, V = schoolMetricModel.SchoolCategory });
                    }

                    //For the metric Value.
                    Type valueType = Type.GetType(schoolMetricModel.ValueType);
                    dynamic rowForMetricValue = Activator.CreateInstance(CreateGenericType(typeof(StateTextCellItem<>), valueType));

                    if (string.Equals(metadataColumn.ColumnName, "School Metric Value", StringComparison.OrdinalIgnoreCase))
                    {
                        
                        if (schoolMetricModel.Value.HasValue)
                            rowForMetricValue.V = schoolMetricModel.Value.Value;
                        else
                            rowForMetricValue.V = schoolMetricModel.DisplayValue;
                        rowForMetricValue.DV = schoolMetricModel.DisplayValue;
                        row.Add(rowForMetricValue);
                    }

                    if (string.Equals(metadataColumn.ColumnName, "School Goal", StringComparison.OrdinalIgnoreCase))
                    {
                        //For the school Goal.
                        row.Add(new CellItem<double> { DV = string.Format("{0:P1}", schoolMetricModel.Goal), V = schoolMetricModel.Goal });
                    }

                    if (string.Equals(metadataColumn.ColumnName, "Difference From Goal", StringComparison.OrdinalIgnoreCase))
                    {
                        // metric difference from goal
                        if (schoolMetricModel.GoalDifference.HasValue)
                        {
                            row.Add(new StateTextCellItem<double>
                            {
                                DV = string.Format("{0:P1}", schoolMetricModel.GoalDifference.Value),
                                V = schoolMetricModel.GoalDifference.Value,
								STe = (int)schoolMetricModel.MetricState.StateType,
                            });

                            rowForMetricValue.STe = (int)schoolMetricModel.MetricState.StateType;
                        }
                        else
                        {
							row.Add(new StateTextCellItem<string> { DV = String.Empty, V = null, STe = (int)MetricStateType.None });
                        }
                    }
                }

                //Last cell for the right rounded corner...
                row.Add(new SpacerCellItem<double> { DV = String.Empty, V = 0 });
                rows.Add(row);
            }
            return rows;
        }

        public static Tuple<List<long[]>, List<List<object>>> GenerateRows(this List<MetadataColumnGroup> metadataColumnGroups, List<StudentWithMetrics> students, string uniqueListId, int pageNumber, int pageSize, int? sortColumn, string sortDirection)
        {
            var sortByColumn = (!sortColumn.HasValue || sortColumn.Value == 0) ? 1 : sortColumn.Value;

            return GenerateMultipleGroupRows(metadataColumnGroups, students, uniqueListId, pageNumber, pageSize, sortByColumn, sortDirection);
        }

        private static List<List<object>> GenerateSingleGroupRows(List<MetadataColumnGroup> metadataColumnGroups, List<StudentWithMetrics> students, string uniqueListId)
        {
            return new List<List<object>>();
        }

        private static List<List<object>> GenerateMultipleGroupRows(List<MetadataColumnGroup> metadataColumnGroups, IEnumerable<StudentWithMetrics> students, string uniqueListId, bool addFirstAndLastSpacers)
        {
            var rows = new List<List<object>>();

            foreach (var s in students)
            {
                var row = new List<object>();

                int groupIndex = 0;
                int groupCount = metadataColumnGroups.Count();
                foreach (var metadataColumnGroup in metadataColumnGroups)
                {
                    if (metadataColumnGroup.GroupType == GroupType.EntityInformation)
                    {
                        row.Add(new SpacerCellItem<double> { DV = String.Empty, V = 0 });//Static row to save space for the left most column.
                    }

                    foreach (var metadataColumn in metadataColumnGroup.Columns)
                    {
                        if (metadataColumnGroup.GroupType == GroupType.EntityInformation)
                        {
                            row.Add(CreateEntityInformationCell(metadataColumn, s, uniqueListId));
                        }
                        else if (metadataColumnGroup.GroupType == GroupType.MetricData)
                        {
                            var sMetric = s.GetMetricByUniqueIdentifier(metadataColumn.UniqueIdentifier);
                            row.Add(CreateMetricCell(metadataColumn, sMetric));
                        }
                    }

                    if (metadataColumnGroup.GroupType == GroupType.EntityInformation || (groupIndex < groupCount - 1))
                        row.Add(new SpacerCellItem<double> { DV = String.Empty, V = 0 });//Static row to save space for the left most column.

                    groupIndex++;
                }

                rows.Add(row);
            }

            return rows;
        }

        private static Tuple<List<long[]>, List<List<object>>> GenerateMultipleGroupRows(List<MetadataColumnGroup> metadataColumnGroups, IEnumerable<StudentWithMetrics> students, string uniqueListId, int pageNumber, int pageSize, int sortColumn, string sortDirection)
        {
            var rowSet = new List<Tuple<object, string, List<object>, long, int>>();

            foreach (var s in students)
            {
                var row = new List<object>();
                var groupIndex = 0;
                var groupCount = metadataColumnGroups.Count();
                var columnIndex = 0;
                object sortValue = s.Name;

                foreach (var metadataColumnGroup in metadataColumnGroups)
                {
                    if (metadataColumnGroup.GroupType == GroupType.EntityInformation)
                    {
                        row.Add(new SpacerCellItem<double> { DV = String.Empty, V = 0 }); //Static row to save space for the left most column.
                    }

                    foreach (var metadataColumn in metadataColumnGroup.Columns)
                    {
                        columnIndex++;

                        switch (metadataColumnGroup.GroupType)
                        {
                            case GroupType.EntityInformation:
                                {
                                    var cell = CreateEntityInformationCell(metadataColumn, s, uniqueListId);
                                    row.Add(cell);

                                    if (columnIndex == sortColumn) sortValue = cell.GetSortValue("V");
                                }
                                break;
                            case GroupType.MetricData:
                                {
                                    var sMetric = s.GetMetricByUniqueIdentifier(metadataColumn.UniqueIdentifier);
                                    var cell = CreateMetricCell(metadataColumn, sMetric);
                                    row.Add(cell);

                                    if (columnIndex == sortColumn) sortValue = cell.GetSortValue("V");
                                }
                                break;
                        }
                    }

                    if (metadataColumnGroup.GroupType == GroupType.EntityInformation || (groupIndex < groupCount - 1))
                        row.Add(new SpacerCellItem<double> { DV = String.Empty, V = 0 }); //Static row to save space for the left most column.

                    groupIndex++;
                    columnIndex++;
                }

                rowSet.Add(Tuple.Create(sortValue, s.Name, row, s.StudentUSI, s.SchoolId.HasValue ? s.SchoolId.Value : 0));
            }

            rowSet.Sort((x, y) => CompareMetricValues(x, y) * ((sortDirection == "desc") ? -1 : 1));

            var studentList = rowSet.Select(x => new[] { x.Item4, x.Item5 }).ToList();
            var pageData = rowSet.Skip((pageNumber - 1) * pageSize).Take(pageSize).Select(x => x.Item3).ToList();

            return Tuple.Create(studentList, pageData);
        }

        private static int CompareMetricValues(Tuple<object, string, List<object>, long, int> metric, Tuple<object, string, List<object>, long, int> metricToCompare)
        {
            if (metric == null || metric.Item1 == null)
                return metricToCompare == null || metricToCompare.Item1 == null
                    ? String.CompareOrdinal((metric != null) ? metric.Item2 : String.Empty, (metricToCompare != null) ? metricToCompare.Item2 : String.Empty)
                    : 1;

            if (metricToCompare == null || metricToCompare.Item1 == null)
                return -1;

            if (metric.Item1.GetType() == metricToCompare.Item1.GetType())
            {
                var metricEquality = ((IComparable)metric.Item1).CompareTo(metricToCompare.Item1);
                return (metricEquality == 0)
                           ? String.CompareOrdinal(metric.Item2, metricToCompare.Item2)
                           : metricEquality;
            }

            if (metric.Item1 is string)
                return 1;

            if (metricToCompare.Item1 is string)
                return -1;

            return String.CompareOrdinal(metric.Item2, metricToCompare.Item2);
        }

        /// <summary>
        /// Get the propertyName value for sorting
        /// </summary>
        /// <param name="cell">Metric Cell</param>
        /// <param name="propertyName">Property Name to extract value for sorting</param>
        /// <returns>Property value to use for sorting</returns>
        private static object GetSortValue(this object cell, string propertyName)
        {
            return cell.GetType().GetProperties().Where(x => x.Name == propertyName).Select(x => x.GetValue(cell, null)).FirstOrDefault();
        }

        private static object CreateEntityInformationCell(MetadataColumn column, StudentWithMetrics s, string uniqueListId)
        {
            if (string.Equals(column.ColumnName, "Flag", StringComparison.OrdinalIgnoreCase))
                return new FlagCellItem<bool> { V = s.IsFlagged, F = s.IsFlagged, };
            
            //StudentUniqueID Newly Added : Saravanan
            if (string.Equals(column.ColumnName, "Student", StringComparison.OrdinalIgnoreCase))
                return new StudentCellItem<string>(s.StudentUSI)
                {
                    V = s.Name,
                    DV = s.Name,
                    I = s.ThumbNail,
                    LUId = uniqueListId,
                    CId = s.SchoolId,
                    //Lets go and resolve the metric to a child one...
                    Url = s.Href != null ? s.Href.Href : null,
                    Links = s.Links,
                    StudentUniqueID = s.StudentUniqueID
                };

            if (string.Equals(column.ColumnName, "Designations", StringComparison.OrdinalIgnoreCase))
            {
                var studentMetricWithAccommodations = (StudentWithMetricsAndAccommodations)s;
                return new DesignationsCellItem<string> { V = studentMetricWithAccommodations.Accommodations.Count.ToString(), D = studentMetricWithAccommodations.Accommodations };
            }

            if (string.Equals(column.ColumnName, "Grade Level", StringComparison.OrdinalIgnoreCase))
            {
                return new CellItem<int> { DV = s.GradeLevelDisplayValue, V = s.GradeLevel };
            }

            if (string.Equals(column.ColumnName, "School", StringComparison.OrdinalIgnoreCase))
            {
                return new CellItem<string> { DV = s.SchoolName };
            }

            if (string.Equals(column.ColumnName, "Metric Value", StringComparison.OrdinalIgnoreCase)
                || string.Equals(column.ColumnName, "Demographic", StringComparison.OrdinalIgnoreCase)
                || column.MetricListCellType == MetricListCellType.MetricValue)
            {
                var studentMetricWithPrimaryMetric = s as StudentWithMetricsAndPrimaryMetric ?? new StudentWithMetricsAndPrimaryMetric(s.StudentUSI);
                var cellType = (studentMetricWithPrimaryMetric.PrimaryMetricValue == null) ? typeof(CellItem<string>) : typeof(CellItem<>).MakeGenericType(GetTypeArray(studentMetricWithPrimaryMetric.PrimaryMetricValue.GetType()));
                dynamic cellForMetricValue = Activator.CreateInstance(cellType);
                cellForMetricValue.V = studentMetricWithPrimaryMetric.PrimaryMetricValue;
                cellForMetricValue.DV = studentMetricWithPrimaryMetric.PrimaryMetricDisplayValue;
                return cellForMetricValue;
            }

            return new CellItem<string> { DV = "New Entity information. Missing Logic." };
        }

        private static object CreateMetricCell(MetadataColumn column, StudentWithMetrics.Metric m)
        {
            if (column.MetricListCellType == MetricListCellType.AssessmentMetric)
            {
                var assessmentMetric = m as StudentWithMetrics.IndicatorMetric;
                if (assessmentMetric != null)
                {
                    if (assessmentMetric.Value != null)
                    {
                        var assessmentCellType = typeof(AssessmentMetricCellItem<>).MakeGenericType(GetTypeArray(assessmentMetric.Value.GetType()));
                        dynamic cellForMetricValue = Activator.CreateInstance(assessmentCellType);
                        cellForMetricValue.V = assessmentMetric.Value;
                        cellForMetricValue.DV = assessmentMetric.DisplayValue;
                        cellForMetricValue.A = (int)assessmentMetric.MetricIndicator;
                        cellForMetricValue.S = (int)assessmentMetric.State;
                        return cellForMetricValue;
                    }

                    return new AssessmentMetricCellItem<double?>
                               {
                                   DV = assessmentMetric.DisplayValue,
                                   V = assessmentMetric.Value,
                                   A = (int)assessmentMetric.MetricIndicator,
                                   S = (int)assessmentMetric.State,
                               };
                }
                return new AssessmentMetricCellItem<string>();
            }

            //Trend Metric
            if (column.MetricListCellType == MetricListCellType.TrendMetric)
            {
                var trendMetric = m as StudentWithMetrics.TrendMetric;
                if (trendMetric != null)
                {
                    if (trendMetric.Value != null)
                    {
                        var trendCellType = typeof(TrendMetricCellItem<>).MakeGenericType(GetTypeArray(trendMetric.Value.GetType()));
                        dynamic trendCellForMetricValue = Activator.CreateInstance(trendCellType);
                        trendCellForMetricValue.V = trendMetric.Value;
                        trendCellForMetricValue.DV = trendMetric.DisplayValue;
                        trendCellForMetricValue.T = trendMetric.Trend.ToString();
                        trendCellForMetricValue.S = (int)trendMetric.State;
                        return trendCellForMetricValue;
                    }

                    return new TrendMetricCellItem<double?>
                               {
                                   DV = trendMetric.DisplayValue,
                                   V = trendMetric.Value,
                                   T = trendMetric.Trend.ToString(),
                                   S = (int)trendMetric.State,
                               };
                }
                return new TrendMetricCellItem<string>();
            }

            if (m != null)
            {
                if (m.Value != null)
                {
                    var metricCellType =
                        typeof(MetricCellItem<>).MakeGenericType(GetTypeArray(m.Value.GetType()));
                    dynamic metricCellForMetricValue = Activator.CreateInstance(metricCellType);
                    metricCellForMetricValue.V = m.Value;
                    metricCellForMetricValue.DV = m.DisplayValue;
                    metricCellForMetricValue.S = (int)m.State;
                    return metricCellForMetricValue;
                }

                return new MetricCellItem<double?>
                           {
                               DV = m.DisplayValue,
                               V = m.Value,
                               S = (int)m.State,
                           };
            }

            return new MetricCellItem<string>();
        }

        #endregion
    }
}