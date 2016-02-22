using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using EdFi.Dashboards.Resources.Models.Common;
using EdFi.Dashboards.Resources.Models.CustomGrid;

namespace EdFi.Dashboards.Resources.Common
{
    public interface ITabFactory
    {
        List<EdFiGridWatchListTabModel> CreateAllTabs(WatchListDataState watchListDataState);
    }

    public class TabFactory : ITabFactory
    {
        #region Constants

        /// <summary>
        /// The metric proficiency selections.
        /// </summary>
        private Dictionary<string, string> metricProficiencySelections;

        /// <summary>
        /// Gets the default metric proficiency selections.
        /// </summary>
        /// <value>
        /// The metric proficiency selections.
        /// </value>
        protected virtual Dictionary<string, string> MetricProficiencySelections
        {
            get
            {
                return metricProficiencySelections ?? (metricProficiencySelections = new Dictionary<string, string>
                {
                    {"below-basic", "BB"},
                    {"basic", "B"},
                    {"proficient", "P"},
                    {"advanced", "A"}
                });
            }
        }

        /// <summary>
        /// The metric range selections.
        /// </summary>
        private Dictionary<string, string> metricRangeSelections;

        /// <summary>
        /// Gets the default metric range selections.
        /// </summary>
        /// <value>
        /// The metric range selections.
        /// </value>
        protected virtual Dictionary<string, string> MetricRangeSelections
        {
            get
            {
                return metricRangeSelections ?? (metricRangeSelections = new Dictionary<string, string>
                {
                    {"75-or-more", "75% or More Correct"},
                    {"40-to-75", "> 40% and < 75% Correct"},
                    {"40-or-under", "40% or Less Correct"}
                });
            }
        }

        /// <summary>
        /// The test selections.
        /// </summary>
        private Dictionary<string, string> metricTestSelections;

        /// <summary>
        /// Gets the default test selections.
        /// </summary>
        /// <value>
        /// The test selections.
        /// </value>
        protected virtual Dictionary<string, string> MetricTestSelections
        {
            get
            {
                return metricTestSelections ?? (metricTestSelections = new Dictionary<string, string>
                {
                    {"test-taken", "Test Taken"},
                    {"at-above", "At or Above Goal"},
                    {"below-goal", "Below Goal"}
                });
            }
        }

        /// <summary>
        /// The metric greater than less than selections.
        /// </summary>
        private Dictionary<string, string> metricGreaterThanLessThanSelections;

        /// <summary>
        /// Gets the default metric greater than less than selections.
        /// </summary>
        /// <value>
        /// The metric greater than less than selections.
        /// </value>
        protected virtual Dictionary<string, string> MetricGreaterThanLessThanSelections
        {
            get
            {
                return metricGreaterThanLessThanSelections ??
                       (metricGreaterThanLessThanSelections = new Dictionary<string, string>
                       {
                           {"gtet", "Greater than or equal to"},
                           {"ltet", "Less than or equal to"}
                       });
            }
        }

        /// <summary>
        /// The metric yes/no selections
        /// </summary>
        private Dictionary<string, string> metricTakenSelections;

        /// <summary>
        /// Gets the metric yes/no selections; since no will be not selected
        /// it does not need to be defined here.
        /// </summary>
        /// <value>
        /// The metric yes/no selections.
        /// </value>
        protected virtual Dictionary<string, string> MetricTakenSelections
        {
            get
            {
                return metricTakenSelections ?? (metricTakenSelections = new Dictionary<string, string>
                {
                    {"yes", "Taken"}
                });
            }
        }

        #endregion

        #region Watch List Creation Code

        /// <summary>
        /// Creates all of the tabs for the watch list. The method also makes
        /// calls to CreateCustomTabs to allow for the consumer of this class
        /// to inject tabs into the tab list.
        /// </summary>
        /// <returns>A list of created tabs.</returns>
        public virtual List<EdFiGridWatchListTabModel> CreateAllTabs(WatchListDataState watchListDataState)
        {
            return new List<EdFiGridWatchListTabModel>
            {
                CreateStudentInformationTab(watchListDataState, true),
                CreateAttendanceAndDisciplineTab(watchListDataState),
                CreateAssessmentsTab(watchListDataState),
                CreateGradesAndCreditsTab(watchListDataState),
                CreateSectionsTab(watchListDataState),
            };
        }

        #region Student Information Tab

        /// <summary>
        /// Creates the student information tab.
        /// </summary>
        /// <param name="watchListDataState"></param>
        /// <param name="isActiveTab">if set to <c>true</c> then this is the active tab.</param>
        /// <returns></returns>
        protected virtual EdFiGridWatchListTabModel CreateStudentInformationTab(WatchListDataState watchListDataState, bool isActiveTab = false)
        {
            var columns = new List<EdFiGridWatchListColumnModel>();
            EdFiGridWatchListColumnModel column;

            if ((column = CreateStudentInformationColumn1(watchListDataState)) != null && column.Templates != null && column.Templates.Any())
            {
                columns.Add(column);
            }
            if ((column = CreateStudentInformationColumn2()) != null && column.Templates != null && column.Templates.Any())
            {
                columns.Add(column);
            }
            if ((column = CreateStudentInformationColumn3()) != null && column.Templates != null && column.Templates.Any())
            {
                columns.Add(column);
            }
            if ((column = CreateStudentInformationColumn4()) != null && column.Templates != null && column.Templates.Any())
            {
                columns.Add(column);
            }

            return CreateTab(watchListDataState.CurrentTabIndex++, "Student Information", columns, isActiveTab);
        }

        /// <summary>
        /// Creates the first columns of the student information tab.
        /// </summary>
        /// <returns></returns>
        protected virtual EdFiGridWatchListColumnModel CreateStudentInformationColumn1(WatchListDataState watchListDataState)
        {
            return CreateColumn(new List<EdFiGridWatchListTemplateModel>
            {
                CreateRadioButtonSelectionModel("gender",
                    "Gender",
                    CreateSelections(
                        new Dictionary<string, string>
                        {
                            {"all-genders", "All Genders"},
                            {"male", "Male"},
                            {"female", "Female"}
                        }
                    )
                ),
                CreateCheckboxSelectionModel("grade", "Grade", watchListDataState.Grades)
            });
        }

        /// <summary>
        /// Creates the second column of the student information tab.
        /// </summary>
        /// <returns></returns>
        protected virtual EdFiGridWatchListColumnModel CreateStudentInformationColumn2()
        {
            return CreateColumn(new List<EdFiGridWatchListTemplateModel>
            {
                CreateCheckboxSelectionModel("demographic-demographics", "Demographics",
                    CreateSelections(new Dictionary<string, string>
                    {
                        {"American Indian - Alaskan Native", "American Indian - Alaskan Native"},
                        {"Asian", "Asian"},
                        {"Black - African American", "Black - African American"},
                        {"Hispanic/Latino", "Hispanic/Latino"},
                        {"Native Hawaiian - Pacific Islander", "Native Hawaiian - Pacific Islander"},
                        {"White", "White"},
                        {"two or more", "Two or More"}
                    })
                )
            });
        }

        /// <summary>
        /// Creates the third column of the student information tab.
        /// </summary>
        /// <returns></returns>
        protected virtual EdFiGridWatchListColumnModel CreateStudentInformationColumn3()
        {
            return CreateColumn(new List<EdFiGridWatchListTemplateModel>
            {
                CreateCheckboxSelectionModel("demographic-program-status", "Program Status",
                    CreateSelections(new Dictionary<string, string>
                    {
                        {"504 Designation", "504 Designation"},
                        {"Bilingual Program", "Bilingual Program"},
                        {"Career and Technical Education", "Career and Technical Education"},
                        {"Gifted/Talented", "Gifted/Talented"},
                        {"Special Education", "Special Education"},
                        {"Title I Participation", "Title I Participation"}
                    })
                )
            });
        }

        /// <summary>
        /// Creates the fourth column of the student information tab.
        /// </summary>
        /// <returns></returns>
        protected virtual EdFiGridWatchListColumnModel CreateStudentInformationColumn4()
        {
            return CreateColumn(new List<EdFiGridWatchListTemplateModel>
            {
                CreateCheckboxSelectionModel("demographic-other-information", "Other Student Information",
                    CreateSelections(new Dictionary<string, string>
                    {
                        {"Targeted Achievement Gap Group (TAGG)", "Targeted Achievement Gap Group"},
                        {"Highly Mobile", "Highly Mobile"},
                        {"Homeless", "Homeless"},
                        {"Immigrant", "Immigrant"},
                        {"Limited English Proficiency", "Limited English Proficiency"},
                        {"Migrant", "Migrant"},
                        {"Over Age", "Over Age"},
                        {"Retained", "Retained"},
                        {"Alternative Learning Environment", "Alternative Learning Environment"}
                    }))
            });
        }

        #endregion

        #region Attendance And Discipline Tab

        /// <summary>
        /// Creates the attendance and discipline tab.
        /// </summary>
        /// <param name="watchListDataState"></param>
        /// <param name="isActiveTab">if set to <c>true</c> then this is the active tab.</param>
        /// <returns></returns>
        protected virtual EdFiGridWatchListTabModel CreateAttendanceAndDisciplineTab(WatchListDataState watchListDataState, bool isActiveTab = false)
        {
            var columns = new List<EdFiGridWatchListColumnModel>();
            EdFiGridWatchListColumnModel column;

            if ((column = CreateAttendanceAndDisciplineColumn1(watchListDataState)) != null && column.Templates != null && column.Templates.Any())
            {
                columns.Add(column);
            }
            if ((column = CreateAttendanceAndDisciplineColumn2(watchListDataState)) != null && column.Templates != null && column.Templates.Any())
            {
                columns.Add(column);
            }

            return CreateTab(watchListDataState.CurrentTabIndex++, "Attendance and Discipline", columns, isActiveTab);
        }

        /// <summary>
        /// Creates the first column of the attendance and discipline tab.
        /// </summary>
        /// <returns></returns>
        protected virtual EdFiGridWatchListColumnModel CreateAttendanceAndDisciplineColumn1(WatchListDataState watchListDataState)
        {
            var templates = new List<EdFiGridWatchListTemplateModel>();

            if (watchListDataState.MetricIds.Contains(1483))
            {
                templates.Add(CreateDropDownTextBoxModel("attendance-1483", "Total Days Absent Current Semester", 0, 99,
                    EdFiGridWatchListDoubleSelectionTextboxModel.TextboxRangeFormat,
                    CreateSelections(MetricGreaterThanLessThanSelections)));
            }
            if (watchListDataState.MetricIds.Contains(1671))
            {
                templates.Add(CreateDropDownTextBoxModel("attendance-1671", "Unexcused Days Absent Current Semester", 0, 99,
                    EdFiGridWatchListDoubleSelectionTextboxModel.TextboxRangeFormat,
                    CreateSelections(MetricGreaterThanLessThanSelections)));
            }

            return CreateColumn(templates);
        }

        /// <summary>
        /// Creates the second column of the attendance and discipline tab.
        /// </summary>
        /// <returns></returns>
        protected virtual EdFiGridWatchListColumnModel CreateAttendanceAndDisciplineColumn2(WatchListDataState watchListDataState)
        {
            var templates = new List<EdFiGridWatchListTemplateModel>();

            if (watchListDataState.MetricIds.Contains(78))
            {
                templates.Add(CreateDropDownTextBoxModel("attendance-78", "State Reportable Offenses", 0, 99,
                    EdFiGridWatchListDoubleSelectionTextboxModel.TextboxRangeFormat,
                    CreateSelections(MetricGreaterThanLessThanSelections)));
            }
            if (watchListDataState.MetricIds.Contains(79))
            {
                templates.Add(CreateDropDownTextBoxModel("attendance-79", "School Code of Conduct", 0, 99,
                    EdFiGridWatchListDoubleSelectionTextboxModel.TextboxRangeFormat,
                    CreateSelections(MetricGreaterThanLessThanSelections)));
            }

            return CreateColumn(templates);
        }

        #endregion

        #region Assessments Tab

        /// <summary>
        /// Creates the assessments tab.
        /// </summary>
        /// <param name="watchListDataState"></param>
        /// <param name="isActiveTab">if set to <c>true</c> then this is the active tab.</param>
        /// <returns></returns>
        protected virtual EdFiGridWatchListTabModel CreateAssessmentsTab(WatchListDataState watchListDataState, bool isActiveTab = false)
        {
            var columns = new List<EdFiGridWatchListColumnModel>();
            EdFiGridWatchListColumnModel column;

            if ((column = CreateAssessmentsColumn1(watchListDataState)) != null && column.Templates != null && column.Templates.Any())
            {
                columns.Add(column);
            }
            if ((column = CreateAssessmentsColumn2(watchListDataState)) != null && column.Templates != null && column.Templates.Any())
            {
                columns.Add(column);
            }
            if ((column = CreateAssessmentsColumn3(watchListDataState)) != null && column.Templates != null && column.Templates.Any())
            {
                columns.Add(column);
            }

            return CreateTab(watchListDataState.CurrentTabIndex++, "Assessments", columns, isActiveTab);
        }

        /// <summary>
        /// Creates the first column of the assessments tab.
        /// </summary>
        /// <returns></returns>
        protected virtual EdFiGridWatchListColumnModel CreateAssessmentsColumn1(WatchListDataState watchListDataState)
        {
            return CreateColumn(new List<EdFiGridWatchListTemplateModel>());
        }

        /// <summary>
        /// Creates the second column of the assessments tab.
        /// </summary>
        /// <returns></returns>
        protected virtual EdFiGridWatchListColumnModel CreateAssessmentsColumn2(WatchListDataState watchListDataState)
        {
            var templates = new List<EdFiGridWatchListTemplateModel>();

            if (watchListDataState.MetricIds.Contains(1686))
            {
                templates.Add(CreateCheckboxInlineSelectionModel("assessment-1686", "Algebra I", CreateSelections(MetricProficiencySelections)));
            }
            if (watchListDataState.MetricIds.Contains(1689))
            {
                templates.Add(CreateCheckboxInlineSelectionModel("assessment-1689", "Biology", CreateSelections(MetricProficiencySelections)));
            }
            if (watchListDataState.MetricIds.Contains(1687))
            {
                templates.Add(CreateCheckboxInlineSelectionModel("assessment-1687", "Geometry", CreateSelections(MetricProficiencySelections)));
            }

            var templateGroup = new List<EdFiGridWatchListTemplateModel>();
            if (templates.Any())
            {
                templateGroup.Add(CreateGroupTemplateModel("End-of-Course Exams", templates));
            }

            return CreateColumn(templateGroup);
        }

        /// <summary>
        /// Creates the third column of the assessments tab.
        /// </summary>
        /// <returns></returns>
        protected virtual EdFiGridWatchListColumnModel CreateAssessmentsColumn3(WatchListDataState watchListDataState)
        {
            var templates = new List<EdFiGridWatchListTemplateModel>();

            if (watchListDataState.MetricIds.Contains(1232))
            {
                templates.Add(CreateCheckboxSelectionModel("assessment-1232", "Reading", CreateSelections(MetricRangeSelections)));
            }
            if (watchListDataState.MetricIds.Contains(1242))
            {
                templates.Add(CreateCheckboxSelectionModel("assessment-1242", "Writing", CreateSelections(MetricRangeSelections)));
            }
            if (watchListDataState.MetricIds.Contains(1238))
            {
                templates.Add(CreateCheckboxSelectionModel("assessment-1238", "Mathematics", CreateSelections(MetricRangeSelections)));
            }
            if (watchListDataState.MetricIds.Contains(1240))
            {
                templates.Add(CreateCheckboxSelectionModel("assessment-1240", "Science", CreateSelections(MetricRangeSelections)));
            }

            return CreateColumn(templates);
        }

        #endregion

        #region Grades And Credits Tab

        /// <summary>
        /// Creates the grades and credits tab.
        /// </summary>
        /// <param name="watchListDataState"></param>
        /// <param name="isActiveTab">if set to <c>true</c> then this is the active tab.</param>
        /// <returns></returns>
        protected virtual EdFiGridWatchListTabModel CreateGradesAndCreditsTab(WatchListDataState watchListDataState, bool isActiveTab = false)
        {
            var columns = new List<EdFiGridWatchListColumnModel>();
            EdFiGridWatchListColumnModel column;

            if ((column = CreateGradesAndCreditsColumn1(watchListDataState)) != null && column.Templates != null && column.Templates.Any())
            {
                columns.Add(column);
            }
            if ((column = CreateGradesAndCreditsColumn2(watchListDataState)) != null && column.Templates != null && column.Templates.Any())
            {
                columns.Add(column);
            }

            return CreateTab(watchListDataState.CurrentTabIndex++, "Grades & Credits", columns, isActiveTab);
        }

        /// <summary>
        /// Creates the first column of the grades and credits tab.
        /// </summary>
        /// <returns></returns>
        protected virtual EdFiGridWatchListColumnModel CreateGradesAndCreditsColumn1(WatchListDataState watchListDataState)
        {
            var templates = new List<EdFiGridWatchListTemplateModel>();

            if (watchListDataState.MetricIds.Contains(1492))
            {
                templates.Add(CreateDropDownTextBoxModel("grades-1492", "Reading/ELA", 0, 99,
                            EdFiGridWatchListDoubleSelectionTextboxModel.TextboxRangeFormat,
                            CreateSelections(MetricGreaterThanLessThanSelections)));
            }
            if (watchListDataState.MetricIds.Contains(1493))
            {
                templates.Add(CreateDropDownTextBoxModel("grades-1493", "Writing", 0, 99,
                            EdFiGridWatchListDoubleSelectionTextboxModel.TextboxRangeFormat,
                            CreateSelections(MetricGreaterThanLessThanSelections)));
            }
            if (watchListDataState.MetricIds.Contains(1494))
            {
                templates.Add(CreateDropDownTextBoxModel("grades-1494", "Mathematics", 0, 99,
                            EdFiGridWatchListDoubleSelectionTextboxModel.TextboxRangeFormat,
                            CreateSelections(MetricGreaterThanLessThanSelections)));
            }
            if (watchListDataState.MetricIds.Contains(1495))
            {
                templates.Add(CreateDropDownTextBoxModel("grades-1495", "Science", 0, 99,
                            EdFiGridWatchListDoubleSelectionTextboxModel.TextboxRangeFormat,
                            CreateSelections(MetricGreaterThanLessThanSelections)));
            }
            if (watchListDataState.MetricIds.Contains(1496))
            {
                templates.Add(CreateDropDownTextBoxModel("grades-1496", "Social Studies", 0, 99,
                            EdFiGridWatchListDoubleSelectionTextboxModel.TextboxRangeFormat,
                            CreateSelections(MetricGreaterThanLessThanSelections)));
            }

            var templateGroup = new List<EdFiGridWatchListTemplateModel>();
            if (templates.Any())
            {
                templateGroup.Add(CreateGroupTemplateModel("Failing Subject Area Course Grades", templates));
            }

            return CreateColumn(templateGroup);
        }

        /// <summary>
        /// Creates the second column of the grades and credits tab.
        /// </summary>
        /// <returns></returns>
        protected virtual EdFiGridWatchListColumnModel CreateGradesAndCreditsColumn2(WatchListDataState watchListDataState)
        {
            var templates = new List<EdFiGridWatchListTemplateModel>();

            if (watchListDataState.MetricIds.Contains(24))
            {
                templates.Add(CreateDropDownTextBoxModel("grades-24", "Failing Class Grades", 0, 99,
                    EdFiGridWatchListDoubleSelectionTextboxModel.TextboxRangeFormat,
                    CreateSelections(MetricGreaterThanLessThanSelections)));
            }
            if (watchListDataState.MetricIds.Contains(25))
            {
                templates.Add(CreateDropDownTextBoxModel("grades-25", "Course Grades Dropping 10% or More", 0, 99,
                    EdFiGridWatchListDoubleSelectionTextboxModel.TextboxRangeFormat,
                    CreateSelections(MetricGreaterThanLessThanSelections)));
            }
            if (watchListDataState.MetricIds.Contains(26))
            {
                templates.Add(CreateDropDownTextBoxModel("grades-26", "Grades Below C Level", 0, 99,
                    EdFiGridWatchListDoubleSelectionTextboxModel.TextboxRangeFormat,
                    CreateSelections(MetricGreaterThanLessThanSelections)));
            }
            // since Algebra I is checked for 8th grade and above this should
            // only show if the logged in user has middle/high students
            if ((watchListDataState.MetricIds.Contains(28) || watchListDataState.MetricIds.Contains(29)))
            {
                templates.Add(CreateDoubleSelectionModel("algebra-i", "Algebra I", CreateSelections(MetricTestSelections)));
            }

            return CreateColumn(templates);
        }

        #endregion

        #region Sections Tab

        /// <summary>
        /// Creates the sections tab.
        /// </summary>
        /// <param name="watchListDataState"></param>
        /// <param name="isActiveTab">if set to <c>true</c> then this is the active tab.</param>
        /// <returns></returns>
        protected virtual EdFiGridWatchListTabModel CreateSectionsTab(WatchListDataState watchListDataState, bool isActiveTab = false)
        {
            return CreateTab(watchListDataState.CurrentTabIndex++, "Sections",
                new List<EdFiGridWatchListColumnModel>
                {
                    CreateSectionColumn1(watchListDataState)
                },
                isActiveTab
            );
        }

        protected virtual EdFiGridWatchListColumnModel CreateSectionColumn1(WatchListDataState watchListDataState)
        {
            return CreateColumn(new List<EdFiGridWatchListTemplateModel>
            {
                CreateCheckboxSelectionModel("selected-classes", "",
                    watchListDataState.Sections.Select(data => new EdFiGridWatchListSelectionItemModel
                    {
                        DisplayValue = data.Value,
                        Name = data.Key.ToString(CultureInfo.InvariantCulture),
                        IsSelected = data.Key == watchListDataState.CurrentSectionId
                    }).ToList(),
                    "Sections"
                )
            });
        }

        #endregion

        /// <summary>
        /// Creates a tab with the specified display title.
        /// </summary>
        /// <param name="tabIndex">The current tab count.</param>
        /// <param name="displayText">The text to display on the tab.</param>
        /// <param name="columns">The columns.</param>
        /// <param name="isActiveTab">if set to <c>true</c> this is the active tab.</param>
        /// <returns></returns>
        protected virtual EdFiGridWatchListTabModel CreateTab(int tabIndex, string displayText, List<EdFiGridWatchListColumnModel> columns, bool isActiveTab = false)
        {
            return new EdFiGridWatchListTabModel
            {
                Name = "metric-based-filters-" + tabIndex,
                DisplayText = displayText,
                IsActiveTab = isActiveTab,
                Columns = columns
            };
        }

        /// <summary>
        /// Creates the column and loads the templates into it.
        /// </summary>
        /// <param name="templates">The templates to load into the column.</param>
        /// <returns></returns>
        protected virtual EdFiGridWatchListColumnModel CreateColumn(List<EdFiGridWatchListTemplateModel> templates)
        {
            return new EdFiGridWatchListColumnModel
            {
                Templates = templates
            };
        }

        #endregion

        #region Template Creation Code

        /// <summary>
        /// Creates the group template model.
        /// </summary>
        /// <param name="groupDisplayValue">The group display value.</param>
        /// <param name="viewModel">The view model.</param>
        /// <returns></returns>
        protected virtual EdFiGridWatchListTemplateModel CreateGroupTemplateModel(string groupDisplayValue, object viewModel)
        {
            return new EdFiGridWatchListTemplateModel
            {
                TemplateName = "metricGroupTemplate",
                GroupDisplayValue = groupDisplayValue,
                ViewModel = viewModel
            };
        }

        /// <summary>
        /// Creates the radio button selection model.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="displayValue">The display value.</param>
        /// <param name="values">The values.</param>
        /// <param name="selectionValue">The selection value is used when a header should not be present but we still need text for the selected filters section.</param>
        /// <param name="isShownInFilterList">if set to <c>true</c> [is shown in filter list].</param>
        /// <returns></returns>
        protected virtual EdFiGridWatchListTemplateModel CreateRadioButtonSelectionModel(string name,
            string displayValue, List<EdFiGridWatchListSelectionItemModel> values, string selectionValue = "", bool isShownInFilterList = true)
        {
            return new EdFiGridWatchListTemplateModel
            {
                TemplateName = "metricRadioButtonTemplate",
                ViewModel = new EdFiGridWatchListSingleSelectionModel
                {
                    Name = name,
                    DisplayValue = displayValue,
                    SelectionValue = selectionValue,
                    Values = values,
                    IsShownInFilterList = isShownInFilterList
                }
            };
        }

        /// <summary>
        /// Creates the checkbox selection model.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="displayValue">The display value.</param>
        /// <param name="values">The values.</param>
        /// <param name="selectionValue">The selection value is used when a header should not be present but we still need text for the selected filters section.</param>
        /// <param name="isShownInFilterList">if set to <c>true</c> [is shown in filter list].</param>
        /// <returns></returns>
        protected virtual EdFiGridWatchListTemplateModel CreateCheckboxSelectionModel(string name, string displayValue,
            List<EdFiGridWatchListSelectionItemModel> values, string selectionValue = "", bool isShownInFilterList = true)
        {
            return new EdFiGridWatchListTemplateModel
            {
                TemplateName = "metricCheckboxTemplate",
                ViewModel = new EdFiGridWatchListSingleSelectionModel
                {
                    Name = name,
                    DisplayValue = displayValue,
                    SelectionValue = selectionValue,
                    Values = values,
                    IsShownInFilterList = isShownInFilterList
                }
            };
        }

        /// <summary>
        /// Creates the checkbox inline selection model.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="displayValue">The display value.</param>
        /// <param name="values">The values.</param>
        /// <param name="selectionValue">The selection value is used when a header should not be present but we still need text for the selected filters section.</param>
        /// <param name="isShownInFilterList">if set to <c>true</c> [is shown in filter list].</param>
        /// <returns></returns>
        protected virtual EdFiGridWatchListTemplateModel CreateCheckboxInlineSelectionModel(string name, string displayValue,
            List<EdFiGridWatchListSelectionItemModel> values, string selectionValue = "", bool isShownInFilterList = true)
        {
            return new EdFiGridWatchListTemplateModel
            {
                TemplateName = "metricCheckboxInlineTemplate",
                ViewModel = new EdFiGridWatchListSingleSelectionModel
                {
                    Name = name,
                    DisplayValue = displayValue,
                    SelectionValue = selectionValue,
                    Values = values,
                    IsShownInFilterList = isShownInFilterList
                }
            };
        }

        /// <summary>
        /// Creates the double selection model.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="displayValue">The display value.</param>
        /// <param name="comparisons">The comparisons.</param>
        /// <param name="values">The values.</param>
        /// <param name="selectionValue">The selection value is used when a header should not be present but we still need text for the selected filters section.</param>
        /// <param name="isShownInFilterList">if set to <c>true</c> [is shown in filter list].</param>
        /// <returns></returns>
        protected virtual EdFiGridWatchListTemplateModel CreateDoubleSelectionModel(string name,
            string displayValue, List<EdFiGridWatchListSelectionItemModel> comparisons,
            List<EdFiGridWatchListSelectionItemModel> values = null, string selectionValue = "", bool isShownInFilterList = true)
        {
            return new EdFiGridWatchListTemplateModel
            {
                TemplateName = "metricDropDownTemplate",
                ViewModel = new EdFiGridWatchListDoubleSelectionModel
                {
                    Name = name,
                    DisplayValue = displayValue,
                    SelectionValue = selectionValue,
                    Comparisons = comparisons,
                    Values = values,
                    IsShownInFilterList = isShownInFilterList
                }
            };
        }

        /// <summary>
        /// Creates the drop down text box model.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="displayValue">The display value.</param>
        /// <param name="textboxMinValue">The textbox minimum value.</param>
        /// <param name="textboxMaxValue">The textbox maximum value.</param>
        /// <param name="textboxFormat">The textbox format.</param>
        /// <param name="comparisons">The comparisons.</param>
        /// <param name="selectionValue">The selection value is used when a header should not be present but we still need text for the selected filters section.</param>
        /// <param name="isShownInFilterList">if set to <c>true</c> [is shown in filter list].</param>
        /// <returns></returns>
        protected virtual EdFiGridWatchListTemplateModel CreateDropDownTextBoxModel(string name,
            string displayValue, int textboxMinValue, int textboxMaxValue, string textboxFormat, List<EdFiGridWatchListSelectionItemModel> comparisons,
            string selectionValue = "", bool isShownInFilterList = true)
        {
            return new EdFiGridWatchListTemplateModel
            {
                TemplateName = "metricDropDownTextboxTemplate",
                ViewModel = new EdFiGridWatchListDoubleSelectionTextboxModel
                {
                    Name = name,
                    DisplayValue = displayValue,
                    SelectionValue = selectionValue,
                    TextboxMinValue = textboxMinValue,
                    TextboxMaxValue = textboxMaxValue,
                    TextboxFormat = textboxFormat,
                    Comparisons = comparisons,
                    IsShownInFilterList = true
                }
            };
        }

        /// <summary>
        /// Creates a list of selections based upon the passed in dictionary.
        /// </summary>
        /// <param name="selections">The selections.</param>
        /// <returns></returns>
        protected virtual List<EdFiGridWatchListSelectionItemModel> CreateSelections(Dictionary<string, string> selections)
        {
            return selections.Select(data => new EdFiGridWatchListSelectionItemModel
            {
                Name = data.Key,
                DisplayValue = data.Value
            }).ToList();
        }

        #endregion
    }
}
