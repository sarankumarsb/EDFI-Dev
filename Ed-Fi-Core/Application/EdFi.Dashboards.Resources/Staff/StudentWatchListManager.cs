using EdFi.Dashboards.Metric.Resources.Models;
using EdFi.Dashboards.Resources.Models.Common;
using System.Collections.Generic;
using System.Linq;
using EdFi.Dashboards.Resources.StudentMetrics;

namespace EdFi.Dashboards.Resources.Staff
{
    public interface IStudentWatchListManager
    {
        StudentMetricsProviderQueryOptions CreateStudentMetricsProviderQueryOptions(
            List<NameValuesType> watchListData,
            IEnumerable<int> metricVariantIds, 
            int schoolId, 
            int localEducationAgencyId, 
            long? staffUSI,
            List<long> studentIds, 
            long? sectionOrCohortId, 
            StudentListType studentListType,
            IEnumerable<SelectionOptionGroup> demographicOptionGroups = null,
            string schoolCategory = null, 
            string gradeLevel = null);
    }
    
    public class StudentWatchListManager : StudentWatchListManagerBase<StudentMetricsProviderQueryOptions>
    {
    }
    
    /// <summary>
    /// Manages the student watch list data to make working with it easier.
    /// </summary>
    public class StudentWatchListManagerBase<TResponse> : IStudentWatchListManager where TResponse : StudentMetricsProviderQueryOptions, new()
    {
        /// <summary>
        /// Creates the student metrics provider query options.
        /// </summary>
        /// <param name="watchListData"></param>
        /// <param name="metricVariantIds">The metric variant ids.</param>
        /// <param name="schoolId">The school identifier.</param>
        /// <param name="localEducationAgencyId">The local education agency identifier.</param>
        /// <param name="staffUSI">The staff usi.</param>
        /// <param name="studentIds">The student ids.</param>
        /// <param name="sectionOrCohortId">The section or cohort identifier.</param>
        /// <param name="studentListType">Type of the student list.</param>
        /// <param name="demographicOptionGroups">The demographic option groups used to add demographics from outside the watch list (ex. Students by Demographic page).</param>
        /// <param name="schoolCategory">The school category.</param>
        /// <param name="gradeLevel">The grade level.</param>
        /// <returns>
        /// A <see cref="StudentMetricsProviderQueryOptions" /> instance.
        /// </returns>
        public virtual StudentMetricsProviderQueryOptions CreateStudentMetricsProviderQueryOptions(
            List<NameValuesType> watchListData,
            IEnumerable<int> metricVariantIds, 
            int schoolId, 
            int localEducationAgencyId, 
            long? staffUSI,
            List<long> studentIds, 
            long? sectionOrCohortId, 
            StudentListType studentListType,
            IEnumerable<SelectionOptionGroup> demographicOptionGroups = null,
            string schoolCategory = null, 
            string gradeLevel = null)
        {
            if(watchListData == null)
                watchListData = new List<NameValuesType>();

            var queryOptions = new TResponse
            {
                MetricVariantIds = metricVariantIds,
                SchoolId = schoolId,
                LocalEducationAgencyId = localEducationAgencyId,
                StaffUSI = staffUSI.GetValueOrDefault()
            };

            if (studentIds.Any())
            {
                queryOptions.StudentIds = studentIds;
            }
            else
            {
                queryOptions.GradeLevel = ProcessGradeSelection(watchListData);

                if (gradeLevel != null && !queryOptions.GradeLevel.Contains(gradeLevel))
                    queryOptions.GradeLevel = queryOptions.GradeLevel.Concat(new[] {gradeLevel}).ToArray();

                var demographics = ProcessDemographics(watchListData);
                // if a demographic option is added to the query options call
                // then add it in here
                if (demographicOptionGroups != null)
                    demographics = MergeSelectionOptionGroups(demographics, demographicOptionGroups);
                
                var assessments = ProcessAssessments(watchListData);
                var attendance = ProcessAttendance(watchListData);
                var grades = ProcessGrades(watchListData);

                queryOptions.DemographicOptionGroups = demographics.ToArray();
                queryOptions.MetricOptionGroups = attendance.Concat(assessments).Concat(grades).ToArray();
            }

            // the list type should only be checked if no section id's are
            // checked in the watch list
            if (watchListData != null && !watchListData.Any())
            {
                switch (studentListType)
                {
                    case StudentListType.Section:
                        queryOptions.TeacherSectionIds = new[]
                        {
                            sectionOrCohortId.GetValueOrDefault()
                        };
                        break;
                    case StudentListType.Cohort:
                        queryOptions.StaffCohortIds = new[]
                        {
                            sectionOrCohortId.GetValueOrDefault()
                        };
                        break;
                }
            }

            var sectionIds = ProcessSectionData(watchListData);

            // this is used to set the new section id's that will be selected from the watch list
            queryOptions.TeacherSectionIds = queryOptions.TeacherSectionIds ?? new long[0];
            queryOptions.TeacherSectionIds = queryOptions.TeacherSectionIds.Concat(sectionIds).Distinct().ToArray();

            if (!string.IsNullOrWhiteSpace(schoolCategory))
                queryOptions.SchoolCategory = schoolCategory;

            return queryOptions;
        }

        protected virtual List<long> ProcessSectionData(List<NameValuesType> watchListData)
        {
            var selectedClasses = GetMetricCheckboxOptionValues("selected-classes", watchListData);

            var sectionIds = new List<long>();

            foreach (var selectedOption in selectedClasses.SelectMany(selectedClass => selectedClass.SelectedOptions))
            {
                long classId;
                if (long.TryParse(selectedOption, out classId))
                {
                    sectionIds.Add(classId);
                }
            }

            return sectionIds;
        }

        protected virtual List<SelectionOptionGroup> ProcessDemographics(List<NameValuesType> watchListData)
        {
            var demographics = new List<SelectionOptionGroup>();

            // Gender is a special case
            foreach (var gender in watchListData.Where(data => data.Name == "gender" && data.Values != null && data.Values.Any(val => val != "all-genders")))
            {
                demographics.Add(new SelectionOptionGroup
                {
                    SelectedOptions = gender.Values.Where(val => val != "all-genders").ToArray()
                });
            }

            demographics.AddRange(GetMetricCheckboxOptionValues("demographic-", watchListData));

            return demographics;
        }

        protected virtual string[] ProcessGradeSelection(List<NameValuesType> watchListData)
        {
            var gradeSelectionList = new List<string>();

            // Grades were already being stored in a string array so will
            // stay that way
            foreach (var grade in watchListData.Where(data => data.Name == "grade" && data.Values != null && data.Values.Any()))
            {
                gradeSelectionList.AddRange(grade.Values);
            }

            return gradeSelectionList.ToArray();
        }

        protected virtual List<MetricFilterOptionGroup> ProcessAttendance(List<NameValuesType> watchListData)
        {
            return new List<MetricFilterOptionGroup>(GetMetricOptionGroupValues("attendance-", watchListData));
        }

        protected virtual List<MetricFilterOptionGroup> ProcessAssessments(List<NameValuesType> watchListData)
        {
            var assessments = GetMetricCheckboxOptionValues("assessment-", watchListData).ToList();
            var assessmentDropDowns = watchListData.Where(data => data.Name.StartsWith("drp-assessment-")).ToList();
            var assessmentsList = new List<MetricFilterOptionGroup>();

            if (assessments.Count > 0)
            {
                foreach (var checkboxOptionGroup in assessments)
                {
                    var options = new List<MetricFilterOption>();
                    foreach (var selectedOption in checkboxOptionGroup.SelectedOptions)
                    {
                        int metricId;
                        if (!int.TryParse(checkboxOptionGroup.SelectionOptionName.Replace("assessment-", ""), out metricId))
                        {
                            continue;
                        }

                        MetricFilterOption option = null;
                        switch (selectedOption)
                        {
                            case "below-basic":
                                option = new MetricFilterOption
                                {
                                    MetricId = metricId,
                                    MinInclusiveMetricInstanceExtendedProperty = "MinScore",
                                    MaxExclusiveMetricInstanceExtendedProperty = "Level2CutScore"
                                };
                                break;
                            case "basic":
                                option = new MetricFilterOption
                                {
                                    MetricId = metricId,
                                    MinInclusiveMetricInstanceExtendedProperty = "Level2CutScore",
                                    MaxExclusiveMetricInstanceExtendedProperty = "Level3CutScore"
                                };
                                break;
                            case "proficient":
                                option = new MetricFilterOption
                                {
                                    MetricId = metricId,
                                    MinInclusiveMetricInstanceExtendedProperty = "Level3CutScore",
                                    MaxExclusiveMetricInstanceExtendedProperty = "Level4CutScore"
                                };
                                break;
                            case "advanced":
                                option = new MetricFilterOption
                                {
                                    MetricId = metricId,
                                    MinInclusiveMetricInstanceExtendedProperty = "Level4CutScore",
                                    MaxExclusiveMetricInstanceExtendedProperty = "MaxScore"
                                };
                                break;
                            case "75-or-more":
                                option = new MetricFilterOption
                                {
                                    MetricId = metricId,
                                    ValueGreaterThanEqualTo = .75
                                };
                                break;
                            case "40-to-75":
                                option = new MetricFilterOption
                                {
                                    MetricId = metricId,
                                    ValueGreaterThan = .4,
                                    ValueLessThan = .75
                                };
                                break;
                            case "40-or-under":
                                option = new MetricFilterOption
                                {
                                    MetricId = metricId,
                                    ValueLessThanEqualTo = .4
                                };
                                break;
                            case "yes":
                                option = new MetricFilterOption
                                {
                                    MetricId = metricId,
                                    MetricInstanceEqualTo = "YES"
                                };
                                break;
                        }

                        if (option != null)
                        {
                            options.Add(option);
                        }
                    }

                    assessmentsList.Add(new MetricFilterOptionGroup
                    {
                        MetricFilterOptions = options.ToArray()
                    });
                }
            }

            if (assessmentDropDowns.Count <= 0) return assessmentsList;

            foreach (var assessmentDropDown in assessmentDropDowns)
            {
                int metricId;
                if (!int.TryParse(assessmentDropDown.Name.Replace("drp-assessment-", ""), out metricId))
                {
                    continue;
                }

                var filter = GetMetricFilterOptionBySelectionValue(metricId, assessmentDropDown.Values[0]);

                if (filter == null) return assessmentsList;

                if (!assessmentsList.Any())
                {
                    assessmentsList.Add(new MetricFilterOptionGroup
                    {
                        MetricFilterOptions = new MetricFilterOption[0]
                    });
                }

                assessmentsList.First().MetricFilterOptions = assessmentsList.First().MetricFilterOptions.Concat(new[] { filter }).ToArray();
            }

            return assessmentsList;
        }

        protected virtual List<MetricFilterOptionGroup> ProcessGrades(List<NameValuesType> watchListData)
        {
            var grades = GetMetricOptionGroupValues("grades-", watchListData).ToList();

            if (watchListData.All(data => data.Name != "algebra-i")) return grades;

            var selectionValue = watchListData.Single(data => data.Name == "algebra-i").Values[0];
            var filter = GetMetricFilterOptionForAlgebraIBySelectionValue(selectionValue);

            if (filter == null) return grades;

            if (!grades.Any())
            {
                grades.Add(new MetricFilterOptionGroup());
            }

            grades.First().MetricFilterOptions = grades.First().MetricFilterOptions.Concat(new[] { filter }).ToArray();

            return grades;
        }

        protected virtual MetricFilterOption GetMetricFilterOptionBySelectionValue(int metricId, string selectionValue)
        {
            MetricFilterOption filter = null;
            switch (selectionValue)
            {
                case "not-taken":
                    filter = new MetricFilterOption
                    {
                        MetricStateNotEqualTo = (int)MetricStateType.Good,
                        MetricId = metricId
                    };
                    break;
                case "below-goal":
                    filter = new MetricFilterOption
                    {
                        MetricStateEqualTo = (int)MetricStateType.Low,
                        MetricId = metricId
                    };
                    break;
                case "at-above":
                    filter = new MetricFilterOption
                    {
                        MetricStateEqualTo = (int)MetricStateType.Good,
                        MetricId = metricId
                    };
                    break;
            }

            return filter;
        }

        /// <summary>
        /// Gets the metric filter option by selection value.
        /// </summary>
        /// <param name="selectionValue">The selection value.</param>
        /// <returns></returns>
        protected virtual MetricFilterOption GetMetricFilterOptionForAlgebraIBySelectionValue(string selectionValue)
        {
            MetricFilterOption filter = null;
            switch (selectionValue)
            {
                case "not-taken":
                    filter = new MetricFilterOption
                    {
                        MetricStateNotEqualTo = (int)MetricStateType.Good,
                        MetricId = 28
                    };
                    break;
                case "failing":
                    filter = new MetricFilterOption
                    {
                        MetricStateEqualTo = (int)MetricStateType.Low,
                        MetricId = 29
                    };
                    break;
                case "passing":
                    filter = new MetricFilterOption
                    {
                        MetricStateEqualTo = (int)MetricStateType.Good,
                        MetricId = 29
                    };
                    break;
            }

            return filter;
        }

        /// <summary>
        /// Gets the metric option group values by an option group name.
        /// </summary>
        /// <param name="optionGroupName">Name of the option group.</param>
        /// <param name="watchListData"></param>
        /// <returns>An IEnumerable of MetricFilterOptionGroup.</returns>
        protected virtual IEnumerable<MetricFilterOptionGroup> GetMetricOptionGroupValues(string optionGroupName, List<NameValuesType> watchListData)
        {
            if (watchListData == null)
                return new List<MetricFilterOptionGroup>();

            return new[]
            {
                new MetricFilterOptionGroup
                {
                    MetricFilterOptions =
                        (watchListData.Where(data => data.Name.StartsWith(optionGroupName) &&
                                    data.Values != null && data.Values.Any() &&
                                    data.Values[1] != string.Empty)
                            .Select(metricOptionGroup => new MetricFilterOption
                            {
                                MetricId = int.Parse(metricOptionGroup.Name.Replace(optionGroupName, string.Empty)),
                                ValueGreaterThanEqualTo = metricOptionGroup.Values[0] == "gtet" ? (double?)double.Parse(metricOptionGroup.Values[1]) : null,
                                ValueLessThanEqualTo = metricOptionGroup.Values[0] == "ltet" ? (double?)double.Parse(metricOptionGroup.Values[1]) : null
                            })).ToArray()
                }
            };
        }

        /// <summary>
        /// Gets the metric checkbox option values by an option group name
        /// prefix.
        /// </summary>
        /// <param name="optionGroupName">Name of the option group.</param>
        /// <param name="watchListData"></param>
        /// <returns>An IEnumerable of CheckboxOptionGroup.</returns>
        protected virtual IEnumerable<SelectionOptionGroup> GetMetricCheckboxOptionValues(string optionGroupName, List<NameValuesType> watchListData)
        {
            if (watchListData == null)
                return new List<SelectionOptionGroup>();

            return (watchListData.Where(data => data.Name.StartsWith(optionGroupName) && data.Values != null && data.Values.Any())
                .Select(metric => new SelectionOptionGroup
                {
                    SelectionOptionName = metric.Name,
                    SelectedOptions = metric.Values.ToArray()
                }));
        }

        /// <summary>
        /// Merges the current list of selection option groups with another list.
        /// </summary>
        /// <param name="currentList">The current list.</param>
        /// <param name="optionsToAdd">The options to add.</param>
        /// <returns>The list with the added options.</returns>
        private static List<SelectionOptionGroup> MergeSelectionOptionGroups(List<SelectionOptionGroup> currentList,
            IEnumerable<SelectionOptionGroup> optionsToAdd)
        {
            if (currentList == null || !currentList.Any())
                return optionsToAdd.ToList();

            foreach (var optionGroup in optionsToAdd.Where(optionGroup => !string.IsNullOrWhiteSpace(optionGroup.SelectionOptionName)))
            {
                if (currentList.All(item => item.SelectionOptionName != optionGroup.SelectionOptionName))
                {
                    currentList.Add(optionGroup);
                    continue;
                }

                // done to eliminate closure issue
                var optionGroupClosure = optionGroup;
                foreach (var t in currentList.Where(t => t.SelectionOptionName == optionGroupClosure.SelectionOptionName))
                {
                    t.SelectedOptions = t.SelectedOptions.Concat(optionGroup.SelectedOptions).ToArray();
                    break;
                }
            }

            return currentList;
        }
    }
}
