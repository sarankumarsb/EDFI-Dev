using System;
using System.Collections.Generic;
using EdFi.Dashboards.Core;
using EdFi.Dashboards.Resources.Models.CustomGrid;
using EdFi.Dashboards.Resources.StudentMetrics;

namespace EdFi.Dashboards.Resources.Staff
{
    public interface IListMetadataProvider
    {
        /// <summary>
        /// Gets the metadata needed to build a list of entities and the metric columns.
        /// </summary>
        /// <param name="metadataListId">the id to pick from the DB.</param>
        /// <returns></returns>
        List<MetadataColumnGroup> GetListMetadata(int metadataListId);

        /// <summary>
        /// Gets the metadata needed to build a list of entities and the metric columns.
        /// </summary>
        /// <param name="metadataListId">the id to pick from the DB.</param>
        /// <param name="subjectArea"></param>
        /// <returns></returns>
        List<MetadataColumnGroup> GetListMetadata(int metadataListId, string subjectArea);

        /// <summary>
        /// Gets the current version of IListMetadataProvider
        /// </summary>
        /// <remarks>Version number of the metadata.  When this value changes, the client should clear all saved settings from
        /// prior versions of the list metadata.  This value can be any kind of string, a randomly generated GUID, or a
        /// fingerprint of the source data.
        /// 
        /// This is most useful when you make a change to the metadata, and you want to force all users to use the new
        /// metadata instead of their saved version.
        /// </remarks>
        string GetMetadataVersion();
    }

	[Obsolete("This implementation gets replaced by DatabaseListMetadataProvider and will be removed in the next Major release version of the Dashboard.")]
    public class ListMetadataProvider : IListMetadataProvider
    {
        //TODO: Convert to GUID
        public List<MetadataColumnGroup> GetListMetadata(int metadataListId)
        {
            const string studentDrilldownAdvancedEnrollmentTooltip = "A red 'No' identifies advanced potential but no enrollment.";
            const string classroomGeneralOverviewAdvancedEnrollmentTooltip = "A red 'No' identifies advanced potential but no enrollment.";

            switch (metadataListId)
            {
                case MetadataListIdResolver.StudentDrilldownHighSchoolListId:
                    return StudentListDrilldownHighSchoolMetadata(studentDrilldownAdvancedEnrollmentTooltip);
                case MetadataListIdResolver.StudentDrilldownMiddleSchoolListId:
                    return StudentListDrilldownMiddleSchoolMetadata(studentDrilldownAdvancedEnrollmentTooltip);
                case MetadataListIdResolver.StudentDrilldownElementarySchoolListId:
                    return StudentListDrilldownElementarySchoolMetadata();

                case MetadataListIdResolver.GeneralOverviewHighSchoolListId:
                    return GeneralOverviewHighSchoolMetadata(classroomGeneralOverviewAdvancedEnrollmentTooltip);
                case MetadataListIdResolver.GeneralOverviewMiddleSchoolListId:
                    return GeneralOverviewMiddleSchoolMetadata(classroomGeneralOverviewAdvancedEnrollmentTooltip);
                case MetadataListIdResolver.GeneralOverviewElementarySchoolListId:
                    return GeneralOverviewElementarySchoolMetadata();

                case MetadataListIdResolver.SubjectSpecificHighSchoolListId:
                    return SubjectSpecificHighSchoolMetadata(String.Empty);
                case MetadataListIdResolver.SubjectSpecificMiddleSchoolListId:
                    return SubjectSpecificMiddleSchoolMetadata(String.Empty);
                case MetadataListIdResolver.SubjectSpecificElementarySchoolListId:
                    return SubjectSpecificElementarySchoolMetadata(String.Empty);

                case MetadataListIdResolver.StudentDemographicHighSchoolListId:
                    return StudentListDemographicHighSchoolMetadata(studentDrilldownAdvancedEnrollmentTooltip);
                case MetadataListIdResolver.StudentDemographicMiddleSchoolListId:
                    return StudentListDemographicMiddleSchoolMetadata(studentDrilldownAdvancedEnrollmentTooltip);
                case MetadataListIdResolver.StudentDemographicElementarySchoolListId:
                    return StudentListDemographicElementarySchoolMetadata();

                case MetadataListIdResolver.PriorYearStudentDrilldownHighSchoolListId:
                    return PriorYearStudentListHighSchoolMetadata();
                case MetadataListIdResolver.PriorYearStudentDrilldownMiddleSchoolListId:
                    return PriorYearStudentListMiddleSchoolMetadata();
                case MetadataListIdResolver.PriorYearStudentDrilldownElementarySchoolListId:
                    return PriorYearStudentListElementarySchoolMetadata();

                case MetadataListIdResolver.PriorYearClassroomHighSchoolListId:
                    return PriorYearClassroomHighSchoolMetadata();
                case MetadataListIdResolver.PriorYearClassroomMiddleSchoolListId:
                    return PriorYearClassroomMiddleSchoolMetadata();
                case MetadataListIdResolver.PriorYearClassroomElementarySchoolListId:
                    return PriorYearClassroomElementarySchoolMetadata();

                case MetadataListIdResolver.StudentGradeHighSchoolListId:
                    return StudentGradeHighSchoolMetadata(studentDrilldownAdvancedEnrollmentTooltip);
                case MetadataListIdResolver.StudentGradeMiddleSchoolListId:
                    return StudentGradeMiddleSchoolMetadata(studentDrilldownAdvancedEnrollmentTooltip);
                case MetadataListIdResolver.StudentGradeElementarySchoolListId:
                    return StudentGradeElementarySchoolMetadata();

                case MetadataListIdResolver.StudentSchoolCategoryHighSchoolListId:
                    return StudentSchoolCategoryHighSchoolMetadata(studentDrilldownAdvancedEnrollmentTooltip);
                case MetadataListIdResolver.StudentSchoolCategoryMiddleSchoolListId:
                    return StudentSchoolCategoryMiddleSchoolMetadata(studentDrilldownAdvancedEnrollmentTooltip);
                case MetadataListIdResolver.StudentSchoolCategoryElementarySchoolListId:
                    return StudentSchoolCategoryElementarySchoolMetadata();

                case MetadataListIdResolver.SchoolMetricTableListId:
                    return SchoolMetricTableMetadata();

                case MetadataListIdResolver.PriorYearSchoolMetricTableListId:
                    return SchoolMetricTableMetadata();

                case MetadataListIdResolver.GoalPlanningSchoolMetricTableListId:
                    return GoalPlanningSchoolMetricTableMetadata();

                case MetadataListIdResolver.TeacherListId:
                    return TeacherListMetadata();
                case MetadataListIdResolver.StaffListId:
                    return StaffListMetadata();

                default:
                    throw new KeyNotFoundException(string.Format("The Metadata List Id:{0} was not found.", metadataListId));
            }
        }

        public List<MetadataColumnGroup> GetListMetadata(int metadataListId, string subjectArea)
        {
            switch (metadataListId)
            {
                case MetadataListIdResolver.SubjectSpecificHighSchoolListId:
                    return SubjectSpecificHighSchoolMetadata(subjectArea);
                case MetadataListIdResolver.SubjectSpecificMiddleSchoolListId:
                    return SubjectSpecificMiddleSchoolMetadata(subjectArea);
                case MetadataListIdResolver.SubjectSpecificElementarySchoolListId:
                    return SubjectSpecificElementarySchoolMetadata(subjectArea);

                default:
                    return GetListMetadata(metadataListId);
            }
        }

        /// <summary>
        /// Gets the current version of IListMetadataProvider
        /// </summary>
        /// <remarks>Version number of the metadata.  When this value changes, the client should clear all saved settings from
        /// prior versions of the list metadata.  This value can be any kind of string, a randomly generated GUID, or a
        /// fingerprint of the source data.
        /// 
        /// This is most useful when you make a change to the metadata, and you want to force all users to use the new
        /// metadata instead of their saved version.
        /// </remarks>
        public string GetMetadataVersion()
        {
            return "5140208c-4b09-4747-bee8-5ffe8d47bc9f";
        }

        #region Shared Metadata

        private static List<MetadataColumnGroup> SharedDefaultMetadata()
        {
            return new List<MetadataColumnGroup>
                                {
                                    new MetadataColumnGroup
                                        {
                                            GroupType = GroupType.EntityInformation,
                                            Title = string.Empty,//This is the first group that holds the student information.
                                            IsVisibleByDefault = true,
                                            IsFixedColumnGroup = true,
                                        },
                                    new MetadataColumnGroup
                                        {
                                            GroupType = GroupType.MetricData,
                                            Title = "ATTENDANCE / DISCIPLINE",
                                            IsVisibleByDefault = true,
                                            Columns = new List<MetadataColumn>
                                                           {
                                                               new MetadataColumn
                                                                   {
                                                                       UniqueIdentifier = 3,
                                                                       ColumnPrefix = "AttendanceCurrent",
                                                                       MetricVariantId = 3,
                                                                       ColumnName = "Last Four Weeks Attendance",
                                                                       IsVisibleByDefault = false,
                                                                       MetricListCellType = MetricListCellType.TrendMetric,
                                                                   },
                                                               new MetadataColumn
                                                                   {
                                                                       UniqueIdentifier = 4,
                                                                       ColumnPrefix = "AttendancePrevious",
                                                                       MetricVariantId = 4,
                                                                       ColumnName = "Last Eight Weeks Attendance",
                                                                       IsVisibleByDefault = false,
                                                                       MetricListCellType = MetricListCellType.TrendMetric,
                                                                   },
                                                               new MetadataColumn
                                                                   {
                                                                       UniqueIdentifier = 5,
                                                                       ColumnPrefix = "AttendanceYTD",
                                                                       MetricVariantId = 5,
                                                                       ColumnName = "Year To Date Attendance",
                                                                       IsVisibleByDefault = false,
                                                                       MetricListCellType = MetricListCellType.Metric,
                                                                   },
                                                               new MetadataColumn
                                                                   {
                                                                       UniqueIdentifier = 1483, 
                                                                       ColumnPrefix = "NumberOfDaysAbsent",
                                                                       MetricVariantId = 1483,
                                                                       ColumnName = "Number of Days Absent",
                                                                       IsVisibleByDefault = false,
                                                                       MetricListCellType = MetricListCellType.Metric,
                                                                   },
                                                                new MetadataColumn
                                                                   {
                                                                       UniqueIdentifier = 1673, 
                                                                       ColumnPrefix = "NumberOfUnexcusedDaysAbsent",
                                                                       MetricVariantId = 1673,
                                                                       ColumnName = "Number of Unexcused Days Absent",
                                                                       IsVisibleByDefault = false,
                                                                       MetricListCellType = MetricListCellType.Metric,
                                                                   },
                                                               new MetadataColumn
                                                                   {
                                                                       UniqueIdentifier = 78,
                                                                       ColumnPrefix = "DisciplineIncidents",
                                                                       MetricVariantId = 78,
                                                                       ColumnName = "State Reportable Offences",
                                                                       IsVisibleByDefault = true,
                                                                       MetricListCellType = MetricListCellType.Metric,
                                                                   },
                                                               new MetadataColumn
                                                                   {
                                                                       UniqueIdentifier = 79,
                                                                       ColumnPrefix = "CodeOfConduct",
                                                                       MetricVariantId = 79,
                                                                       ColumnName = "School Code of Conduct",
                                                                       IsVisibleByDefault = false,
                                                                       MetricListCellType = MetricListCellType.Metric,
                                                                   }
                                                           },
                                        },
                                    new MetadataColumnGroup
                                        {
                                            GroupType = GroupType.MetricData,
                                            Title = "ASSESSMENTS",
                                            IsVisibleByDefault = true,
                                        },
                                    new MetadataColumnGroup
                                        {
                                            GroupType = GroupType.MetricData,
                                            Title = "GRADES",
                                            IsVisibleByDefault = true,
                                        },
                                };
        }

        private static List<MetadataColumnGroup> SharedHighSchoolMetadata(List<MetadataColumnGroup> baseMetadata, string advancedEnrollmentTooltip)
        {
            baseMetadata[1].Columns.InsertRange(5, new[]
                                                 {
                                                     new MetadataColumn { UniqueIdentifier = 7, ColumnPrefix = "AbsencesCurrent", MetricVariantId = 7, ColumnName = "Last Four Weeks Class Absences", IsVisibleByDefault = true, MetricListCellType = MetricListCellType.TrendMetric, },
                                                     new MetadataColumn { UniqueIdentifier = 8, ColumnPrefix = "AbsencesPrevious", MetricVariantId = 8, ColumnName = "Last Eight Weeks Class Absences", IsVisibleByDefault = false, MetricListCellType = MetricListCellType.TrendMetric, },
                                                     new MetadataColumn { UniqueIdentifier = 9, ColumnPrefix = "AbsencesYTD", MetricVariantId = 9, ColumnName = "Year To Date Class Absences", IsVisibleByDefault = false, MetricListCellType = MetricListCellType.Metric, }
                                                 });
            baseMetadata[3].Columns.AddRange(new[]
                                                {
                                                    new MetadataColumn { UniqueIdentifier = 26, ColumnPrefix = "BelowC", MetricVariantId = 26, ColumnName = "# Grades Below C", IsVisibleByDefault = true, MetricListCellType = MetricListCellType.TrendMetric, },
                                                    new MetadataColumn { UniqueIdentifier = 25, ColumnPrefix = "FallingGrades", MetricVariantId = 25, ColumnName = "Grades Falling ≥&nbsp;10%", IsVisibleByDefault = true, MetricListCellType = MetricListCellType.TrendMetric, },
                                                    new MetadataColumn { UniqueIdentifier = 40, ColumnPrefix = "Credits", MetricVariantId = 40, ColumnName = "Credits", IsVisibleByDefault = false, MetricListCellType = MetricListCellType.Metric, },
                                                    new MetadataColumn { UniqueIdentifier = 41, ColumnPrefix = "FourByFour", MetricVariantId = 41, ColumnName = "4&nbsp;X&nbsp;4", IsVisibleByDefault = false, MetricListCellType = MetricListCellType.StateValueMetric },
                                                    new MetadataColumn { UniqueIdentifier = 1472, ColumnPrefix = "ChallengeReading", MetricVariantId = 1472, ColumnName = "ELA Advanced Enrollment", IsVisibleByDefault = false, Tooltip = advancedEnrollmentTooltip, MetricListCellType = MetricListCellType.Metric, },
                                                    new MetadataColumn { UniqueIdentifier = 1474, ColumnPrefix = "ChallengeMath", MetricVariantId = 1474, ColumnName = "Math Advanced Enrollment", IsVisibleByDefault = false, Tooltip = advancedEnrollmentTooltip, MetricListCellType = MetricListCellType.Metric, },
                                                    new MetadataColumn { UniqueIdentifier = 1476, ColumnPrefix = "ChallengeScience", MetricVariantId = 1476, ColumnName = "Science Advanced Enrollment", IsVisibleByDefault = false, Tooltip = advancedEnrollmentTooltip, MetricListCellType = MetricListCellType.Metric, },
                                                    new MetadataColumn { UniqueIdentifier = 1478, ColumnPrefix = "ChallengeSocialStudies", MetricVariantId = 1478, ColumnName = "Social Studies Advanced Enrollment", IsVisibleByDefault = false, Tooltip = advancedEnrollmentTooltip, MetricListCellType = MetricListCellType.Metric }
                                                });

            return baseMetadata;
        }

        private static List<MetadataColumnGroup> SharedMiddleSchoolMetadata(List<MetadataColumnGroup> baseMetadata, string advancedEnrollmentTooltip)
        {
            baseMetadata[1].Columns.InsertRange(5, new[]
                                                {
                                                    new MetadataColumn { UniqueIdentifier = 7, ColumnPrefix = "AbsencesCurrent", MetricVariantId = 7, ColumnName = "Last Four Weeks Class Absences", IsVisibleByDefault = true, MetricListCellType = MetricListCellType.TrendMetric },
                                                    new MetadataColumn { UniqueIdentifier = 8, ColumnPrefix = "AbsencesPrevious", MetricVariantId = 8, ColumnName = "Last Eight Weeks Class Absences", IsVisibleByDefault = false, MetricListCellType = MetricListCellType.TrendMetric },
                                                    new MetadataColumn { UniqueIdentifier = 9, ColumnPrefix = "AbsencesYTD", MetricVariantId = 9, ColumnName = "Year To Date Class Absences", IsVisibleByDefault = false, MetricListCellType = MetricListCellType.Metric }
                                                });
            baseMetadata[3].Columns.AddRange(new[]
                                                {
                                                    new MetadataColumn { UniqueIdentifier = 26, ColumnPrefix = "BelowC", MetricVariantId = 26, ColumnName = "# Grades Below C", IsVisibleByDefault = true, MetricListCellType = MetricListCellType.TrendMetric },
                                                    new MetadataColumn { UniqueIdentifier = 25, ColumnPrefix = "FallingGrades", MetricVariantId = 25, ColumnName = "Grades Falling ≥&nbsp;10%", IsVisibleByDefault = true, MetricListCellType = MetricListCellType.TrendMetric },
                                                    new MetadataColumn { UniqueIdentifier = 1472, ColumnPrefix = "ChallengeReading", MetricVariantId = 1472, ColumnName = "ELA Advanced Enrollment", IsVisibleByDefault = false, Tooltip = advancedEnrollmentTooltip, MetricListCellType = MetricListCellType.Metric },
                                                    new MetadataColumn { UniqueIdentifier = 1474, ColumnPrefix = "ChallengeMath", MetricVariantId = 1474, ColumnName = "Math Advanced Enrollment", IsVisibleByDefault = false, Tooltip = advancedEnrollmentTooltip, MetricListCellType = MetricListCellType.Metric },
                                                    new MetadataColumn { UniqueIdentifier = 1476, ColumnPrefix = "ChallengeScience", MetricVariantId = 1476, ColumnName = "Science Advanced Enrollment", IsVisibleByDefault = false, Tooltip = advancedEnrollmentTooltip, MetricListCellType = MetricListCellType.Metric },
                                                    new MetadataColumn { UniqueIdentifier = 1478, ColumnPrefix = "ChallengeSocialStudies", MetricVariantId = 1478, ColumnName = "Social Studies Advanced Enrollment", IsVisibleByDefault = false, Tooltip = advancedEnrollmentTooltip, MetricListCellType = MetricListCellType.Metric }
                                                });
            return baseMetadata;
        }

        private static List<MetadataColumnGroup> SharedElementarySchoolMetadata(List<MetadataColumnGroup> baseMetadata)
        {
            //For Participation first
            baseMetadata[1].Columns[0].IsVisibleByDefault = true;
            baseMetadata[1].Columns[3].IsVisibleByDefault = false;
            baseMetadata[1].Columns[4].IsVisibleByDefault = true;

            //Assessments
            baseMetadata[2].Columns.InsertRange(0, new[]
                                                {
                                                    new MetadataColumn { UniqueIdentifier = 1487, ColumnPrefix = "DIBELS", MetricVariantId = 1487, ColumnName = "DIBELS", IsVisibleByDefault = false, MetricListCellType = MetricListCellType.TrendMetric, },
                                                });

            //Grades
            baseMetadata[3].Columns.Clear();
            baseMetadata[3].Columns.Add(new MetadataColumn { UniqueIdentifier = 1088, ColumnPrefix = "FailingGradesReading", MetricVariantId = 1492, ColumnName = "Failing ELA / Reading Grades", IsVisibleByDefault = true, MetricListCellType = MetricListCellType.TrendMetric });
            baseMetadata[3].Columns.Add(new MetadataColumn { UniqueIdentifier = 1089, ColumnPrefix = "FailingGradesWriting", MetricVariantId = 1493, ColumnName = "Failing Writing Grades", IsVisibleByDefault = true, MetricListCellType = MetricListCellType.TrendMetric });
            baseMetadata[3].Columns.Add(new MetadataColumn { UniqueIdentifier = 1090, ColumnPrefix = "FailingGradesMath", MetricVariantId = 1494, ColumnName = "Failing Math Grades", IsVisibleByDefault = true, MetricListCellType = MetricListCellType.TrendMetric });
            baseMetadata[3].Columns.Add(new MetadataColumn { UniqueIdentifier = 1091, ColumnPrefix = "FailingGradesScience", MetricVariantId = 1495, ColumnName = "Failing Science Grades", IsVisibleByDefault = true, MetricListCellType = MetricListCellType.TrendMetric });
            baseMetadata[3].Columns.Add(new MetadataColumn { UniqueIdentifier = 1092, ColumnPrefix = "FailingGradesSocialStudies", MetricVariantId = 1496, ColumnName = "Failing Social Studies Grades", IsVisibleByDefault = true, MetricListCellType = MetricListCellType.TrendMetric });

            return baseMetadata;
        }

        #endregion

        //TODO: This should be populated by the Spiner metadata list table.
        #region General Overview Classroom

        private static List<MetadataColumnGroup> GeneralOverviewDefaultMetadata()
        {
            var model = SharedDefaultMetadata();
            model[0].Columns.AddRange(new[]
                                          {
                                            new MetadataColumn
                                            {
                                                UniqueIdentifier = SpecialMetricVariantSortingIds.Student,
                                                ColumnName = "Student",
                                                IsVisibleByDefault = true,
                                                MetricVariantId = SpecialMetricVariantSortingIds.Student,
                                            },
                                            new MetadataColumn
                                            {
                                                UniqueIdentifier = SpecialMetricVariantSortingIds.GradeLevel,  
                                                ColumnName = "Grade Level",
                                                IsVisibleByDefault = true,
                                                MetricVariantId = SpecialMetricVariantSortingIds.GradeLevel,
                                            },
                                            new MetadataColumn
                                            {
                                                UniqueIdentifier = SpecialMetricVariantSortingIds.Designations, 
                                                ColumnName = "Designations",
                                                IsVisibleByDefault = true,
                                                SortAscending = "sortDesignationsAsc",
                                                SortDescending = "sortDesignationsDesc",
                                                MetricVariantId = SpecialMetricVariantSortingIds.Designations,
                                            },
                                          });
            return model;
        }

		private static List<MetadataColumnGroup> GeneralOverviewHighSchoolMetadata(string advancedEnrollmentTooltip)
        {
            var model = GeneralOverviewDefaultMetadata();

            return SharedHighSchoolMetadata(model, advancedEnrollmentTooltip);
        }

		private static List<MetadataColumnGroup> GeneralOverviewMiddleSchoolMetadata(string advancedEnrollmentTooltip)
        {
            var model = GeneralOverviewDefaultMetadata();

            return SharedMiddleSchoolMetadata(model, advancedEnrollmentTooltip);
        }

		private static List<MetadataColumnGroup> GeneralOverviewElementarySchoolMetadata()
        {
            var model = GeneralOverviewDefaultMetadata();

            return SharedElementarySchoolMetadata(model);
        }

        #endregion

        #region Student List Drilldown

        //Today all these are the same as the General Overview student list. But we will leave the stub methods in here for future expansion.
        private static List<MetadataColumnGroup> StudentListDrilldownDefaultMetadata()
        {
            var metadata = SharedDefaultMetadata();

            metadata[0].Columns.AddRange(new[]
                                          {                                          
                                              new MetadataColumn { UniqueIdentifier = SpecialMetricVariantSortingIds.Student, ColumnName = "Student", IsVisibleByDefault = true, MetricVariantId = SpecialMetricVariantSortingIds.Student, }, 
                                              new MetadataColumn { UniqueIdentifier = SpecialMetricVariantSortingIds.GradeLevel, ColumnName = "Grade Level", IsVisibleByDefault = true, MetricVariantId = SpecialMetricVariantSortingIds.GradeLevel, }, 
                                              new MetadataColumn { UniqueIdentifier = SpecialMetricVariantSortingIds.SchoolMetricStudentList, ColumnName = "Metric Value", IsVisibleByDefault = true, MetricVariantId = SpecialMetricVariantSortingIds.SchoolMetricStudentList, MetricListCellType = MetricListCellType.MetricValue },
                                          });

            return metadata;
        }

		private static List<MetadataColumnGroup> StudentListDrilldownHighSchoolMetadata(string advancedEnrollmentTooltip)
        {
            var metadata = StudentListDrilldownDefaultMetadata();
            return SharedHighSchoolMetadata(metadata, advancedEnrollmentTooltip);
        }

		private static List<MetadataColumnGroup> StudentListDrilldownMiddleSchoolMetadata(string advancedEnrollmentTooltip)
        {
            var metadata = StudentListDrilldownDefaultMetadata();
            return SharedMiddleSchoolMetadata(metadata, advancedEnrollmentTooltip);
        }

		private static List<MetadataColumnGroup> StudentListDrilldownElementarySchoolMetadata()
        {
            var metadata = StudentListDrilldownDefaultMetadata();
            return SharedElementarySchoolMetadata(metadata);
        }

        #endregion

        #region Student Demographic

        private static List<MetadataColumnGroup> StudentListDemographicDefaultMetadata()
        {
            var metadata = SharedDefaultMetadata();

            metadata[0].Columns.AddRange(new[]
                                          {                                          
                                              new MetadataColumn { UniqueIdentifier = SpecialMetricVariantSortingIds.Student, ColumnName = "Student", IsVisibleByDefault = true, MetricVariantId = SpecialMetricVariantSortingIds.Student, }, 
                                              new MetadataColumn { UniqueIdentifier = SpecialMetricVariantSortingIds.GradeLevel, ColumnName = "Grade Level", IsVisibleByDefault = true, MetricVariantId = SpecialMetricVariantSortingIds.GradeLevel, }, 
                                          });

            return metadata;
        }

		private static List<MetadataColumnGroup> StudentListDemographicHighSchoolMetadata(string advancedEnrollmentTooltip)
        {
            var metadata = StudentListDemographicDefaultMetadata();
            return SharedHighSchoolMetadata(metadata, advancedEnrollmentTooltip);
        }

		private static List<MetadataColumnGroup> StudentListDemographicMiddleSchoolMetadata(string advancedEnrollmentTooltip)
        {
            var metadata = StudentListDemographicDefaultMetadata();
            return SharedMiddleSchoolMetadata(metadata, advancedEnrollmentTooltip);
        }

		private static List<MetadataColumnGroup> StudentListDemographicElementarySchoolMetadata()
        {
            var metadata = StudentListDemographicDefaultMetadata();
            return SharedElementarySchoolMetadata(metadata);
        }

        #endregion

        #region Student Grade

        private static List<MetadataColumnGroup> StudentGradeDefaultMetadata()
        {
            var metadata = SharedDefaultMetadata();

            metadata[0].Columns.AddRange(new[]
                                                {                                          
                                                    new MetadataColumn
                                                    {
                                                        UniqueIdentifier = SpecialMetricVariantSortingIds.Student, 
                                                        ColumnName = "Student",
                                                        IsVisibleByDefault = true,
                                                        MetricVariantId = SpecialMetricVariantSortingIds.Student,
                                                    },
                                                    new MetadataColumn
                                                    {
                                                        UniqueIdentifier = SpecialMetricVariantSortingIds.GradeLevel,  
                                                        ColumnName = "Grade Level", 
                                                        IsVisibleByDefault = true, 
                                                        MetricVariantId = SpecialMetricVariantSortingIds.GradeLevel,
                                                    },
                                                    new MetadataColumn
                                                    {
                                                        UniqueIdentifier = SpecialMetricVariantSortingIds.Designations, 
                                                        ColumnName = "Designations",
                                                        IsVisibleByDefault = true,
                                                        SortAscending = "sortDesignationsAsc",
                                                        SortDescending = "sortDesignationsDesc",
                                                        MetricVariantId = SpecialMetricVariantSortingIds.Designations,
                                                    },
                                                });

            return metadata;
        }

		private static List<MetadataColumnGroup> StudentGradeHighSchoolMetadata(string advancedEnrollmentTooltip)
        {
            var metadata = StudentGradeDefaultMetadata();
            return SharedHighSchoolMetadata(metadata, advancedEnrollmentTooltip);
        }

		private static List<MetadataColumnGroup> StudentGradeMiddleSchoolMetadata(string advancedEnrollmentTooltip)
        {
            var metadata = StudentGradeDefaultMetadata();
            return SharedMiddleSchoolMetadata(metadata, advancedEnrollmentTooltip);
        }

		private static List<MetadataColumnGroup> StudentGradeElementarySchoolMetadata()
        {
            var metadata = StudentGradeDefaultMetadata();
            return SharedElementarySchoolMetadata(metadata);
        }

        #endregion

        #region Student School Category

        private static List<MetadataColumnGroup> StudentSchoolCategoryDefaultMetadata()
        {
            var metadata = SharedDefaultMetadata();

            metadata[0].Columns.AddRange(new[]
                                                {                                          
                                                    new MetadataColumn
                                                    {
                                                        UniqueIdentifier = SpecialMetricVariantSortingIds.Student, 
                                                        ColumnName = "Student",
                                                        IsVisibleByDefault = true,
                                                        MetricVariantId = SpecialMetricVariantSortingIds.Student,
                                                    },
                                                    new MetadataColumn
                                                    {
                                                        UniqueIdentifier = SpecialMetricVariantSortingIds.GradeLevel, 
                                                        ColumnName = "Grade Level",
                                                        IsVisibleByDefault = true,
                                                        MetricVariantId = SpecialMetricVariantSortingIds.GradeLevel,
                                                    },
                                                    new MetadataColumn
                                                    {
                                                        UniqueIdentifier = SpecialMetricVariantSortingIds.School, 
                                                        ColumnName = "School",
                                                        IsVisibleByDefault = true,
                                                        MetricVariantId = SpecialMetricVariantSortingIds.School,
                                                    },
                                                });

            return metadata;
        }

		private static List<MetadataColumnGroup> StudentSchoolCategoryHighSchoolMetadata(string advancedEnrollmentTooltip)
        {
            var metadata = StudentSchoolCategoryDefaultMetadata();
            return SharedHighSchoolMetadata(metadata, advancedEnrollmentTooltip);
        }

		private static List<MetadataColumnGroup> StudentSchoolCategoryMiddleSchoolMetadata(string advancedEnrollmentTooltip)
        {
            var metadata = StudentSchoolCategoryDefaultMetadata();
            return SharedMiddleSchoolMetadata(metadata, advancedEnrollmentTooltip);
        }

		private static List<MetadataColumnGroup> StudentSchoolCategoryElementarySchoolMetadata()
        {
            var metadata = StudentSchoolCategoryDefaultMetadata();
            return SharedElementarySchoolMetadata(metadata);
        }

        #endregion

        #region Subject Specific

        private static List<MetadataColumnGroup> SubjectSpecificDefaultMetadata(string subjectArea)
        {
            var assessmentColumns = new List<MetadataColumn>();

            switch (subjectArea)
            {
                case "Mathematics":
                    assessmentColumns.Add(new MetadataColumn { UniqueIdentifier = (int)StudentMetricEnum.BenchmarkAssessments, ColumnPrefix = "BenchmarkMath", MetricVariantId = (int)StudentMetricEnum.BenchmarkMasteryMath, ColumnName = "Benchmark", IsVisibleByDefault = true, MetricListCellType = MetricListCellType.Metric });
                    break;
                case "Social Studies":
                    assessmentColumns.Add(new MetadataColumn { UniqueIdentifier = (int)StudentMetricEnum.BenchmarkAssessments, ColumnPrefix = "BenchmarkSocialStudies", MetricVariantId = (int)StudentMetricEnum.BenchmarkMasterySocialStudies, ColumnName = "Benchmark", IsVisibleByDefault = true, MetricListCellType = MetricListCellType.Metric });
                    break;
                case "Science":
                    assessmentColumns.Add(new MetadataColumn { UniqueIdentifier = (int)StudentMetricEnum.BenchmarkAssessments, ColumnPrefix = "BenchmarkScience", MetricVariantId = (int)StudentMetricEnum.BenchmarkMasteryScience, ColumnName = "Benchmark", IsVisibleByDefault = true, MetricListCellType = MetricListCellType.Metric });
                    break;
                case "Writing":
                    assessmentColumns.Add(new MetadataColumn { UniqueIdentifier = (int)StudentMetricEnum.BenchmarkAssessments, ColumnPrefix = "BenchmarkWriting", MetricVariantId = (int)StudentMetricEnum.BenchmarkMasteryWriting, ColumnName = "Benchmark", IsVisibleByDefault = true, MetricListCellType = MetricListCellType.Metric });
                    break;
                default:
                    assessmentColumns.Add(new MetadataColumn { UniqueIdentifier = (int)StudentMetricEnum.BenchmarkAssessments, ColumnPrefix = "BenchmarkReading", MetricVariantId = (int)StudentMetricEnum.BenchmarkMasteryELAReading, ColumnName = "Benchmark", IsVisibleByDefault = true, MetricListCellType = MetricListCellType.Metric });
                    break;
            }

            return new List<MetadataColumnGroup>
                                {
                                    new MetadataColumnGroup
                                        {
                                            GroupType = GroupType.EntityInformation,
                                            Title = string.Empty,//This is the first group that holds the student information.
                                            IsVisibleByDefault = true,
                                            IsFixedColumnGroup = true,
                                            Columns = new List<MetadataColumn>
                                                           {
                                                               new MetadataColumn
                                                                   {
                                                                       UniqueIdentifier = SpecialMetricVariantSortingIds.Student, 
                                                                       ColumnName = "Student",
                                                                       IsVisibleByDefault = true,
                                                                       MetricVariantId = SpecialMetricVariantSortingIds.Student,
                                                                   },
                                                               new MetadataColumn
                                                                    {
                                                                       UniqueIdentifier = SpecialMetricVariantSortingIds.GradeLevel,  
                                                                       ColumnName = "Grade Level",
                                                                       IsVisibleByDefault = true,
                                                                       MetricVariantId = SpecialMetricVariantSortingIds.GradeLevel,
                                                                     },
                                                               new MetadataColumn
                                                                   {
                                                                       UniqueIdentifier = SpecialMetricVariantSortingIds.Designations,  
                                                                       ColumnName = "Designations",
                                                                       IsVisibleByDefault = true,
                                                                       SortAscending = "sortDesignationsAsc",
                                                                       SortDescending = "sortDesignationsDesc",
                                                                       MetricVariantId = SpecialMetricVariantSortingIds.Designations,
                                                                   },
                                                           },
                                        },
                                    new MetadataColumnGroup
                                        {
                                            GroupType = GroupType.MetricData,
                                            Title = "ATTENDANCE / DISCIPLINE",
                                            IsVisibleByDefault = true,
                                            IsFixedColumnGroup = false,
                                            Columns = new List<MetadataColumn>
                                                           {
                                                               new MetadataColumn
                                                                   {
                                                                       UniqueIdentifier = (int) StudentMetricEnum.AbsenceLevelCurrentPeriod, 
                                                                       MetricVariantId = (int) StudentMetricEnum.AbsenceLevelCurrentPeriod,
                                                                       ColumnName = "Last Four Weeks Class Absences",
                                                                       IsVisibleByDefault = true,
                                                                       MetricListCellType = MetricListCellType.TrendMetric,
                                                                   },
                                                               
                                                               new MetadataColumn
                                                                    {
                                                                        UniqueIdentifier = (int)StudentMetricEnum.DisciplineIncidents,
                                                                        ColumnPrefix = "CodeOfConduct",
                                                                        MetricVariantId = 79,
                                                                        ColumnName = "School Code of Conduct",
                                                                        IsVisibleByDefault = false,
                                                                        MetricListCellType = MetricListCellType.Metric,
                                                                    }
                                                           },
                                        },
                                    new MetadataColumnGroup
                                        {
                                            GroupType = GroupType.MetricData,
                                            Title = "ASSESSMENTS",
                                            IsVisibleByDefault = true,
                                            IsFixedColumnGroup = false,
                                            Columns = assessmentColumns
                                        },
                                    new MetadataColumnGroup
                                        {
                                            GroupType = GroupType.MetricData,
                                            Title = "GRADES",
                                            IsVisibleByDefault = true,
                                            IsFixedColumnGroup = false,
                                            Columns = new List<MetadataColumn>
                                                          {
                                                              new MetadataColumn
                                                                  {
                                                                       UniqueIdentifier = (int) StudentMetricEnum.ClassGradeGradesFalling10PercentOrMore, 
                                                                       MetricVariantId = (int) StudentMetricEnum.ClassGradeGradesFalling10PercentOrMore,
                                                                       ColumnName = "Grade Dropping ≥&nbsp;10%",
                                                                       IsVisibleByDefault = true,
                                                                       MetricListCellType = MetricListCellType.TrendMetric,
                                                                  },
                                                              new MetadataColumn
                                                                  {
                                                                       UniqueIdentifier = (int) StudentMetricEnum.SubjectAreaCourseGrades,
                                                                       MetricVariantId = (int) StudentMetricEnum.SubjectAreaCourseGrades,
                                                                       ColumnName = "Class&nbsp;Grade",
                                                                       IsVisibleByDefault = true,
                                                                       MetricListCellType = MetricListCellType.TrendMetric,
                                                                  }
                                                          }
                                        },
                                };
        }

        private static List<MetadataColumnGroup> SubjectSpecificHighSchoolMetadata(string subjectArea)
        {
            var metadata = SubjectSpecificDefaultMetadata(subjectArea);
            return metadata;
        }

        private static List<MetadataColumnGroup> SubjectSpecificMiddleSchoolMetadata(string subjectArea)
        {
            var metadata = SubjectSpecificDefaultMetadata(subjectArea);
            return metadata;
        }

        private static List<MetadataColumnGroup> SubjectSpecificElementarySchoolMetadata(string subjectArea)
        {
            var metadata = SubjectSpecificDefaultMetadata(subjectArea);

            metadata[1].Columns[0] = new MetadataColumn
            {
                UniqueIdentifier = (int)StudentMetricEnum.AbsenceLevelCurrentPeriod,
                ColumnPrefix = "AttendanceCurrent",
                MetricVariantId = 3,
                ColumnName = "Last Four Weeks Attendance",
                IsVisibleByDefault = false,
                MetricListCellType = MetricListCellType.TrendMetric,
            };

            //Assessments
            metadata[2].Columns.InsertRange(0, new[]
                                                {
                                                    new MetadataColumn { UniqueIdentifier = 1487, ColumnPrefix = "DIBELS", MetricVariantId = 1487, ColumnName = "DIBELS", IsVisibleByDefault = false, MetricListCellType = MetricListCellType.TrendMetric, },
                                                });

            //Remove Grades Dropping 10%
            metadata[3].Columns.RemoveAt(0);

            return metadata;
        }

        #endregion

        #region Shared Prior Year Metadata

        private static List<MetadataColumnGroup> SharedPriorYearMetadata()
        {
            return new List<MetadataColumnGroup>
                                {
                                    new MetadataColumnGroup
                                        {
                                            GroupType = GroupType.EntityInformation,
                                            Title = string.Empty,//This is the first group that holds the student information.
                                            IsVisibleByDefault = true,
                                            IsFixedColumnGroup = true,
                                        },
                                    new MetadataColumnGroup
                                        {
                                            GroupType = GroupType.MetricData,
                                            Title = "ATTENDANCE / DISCIPLINE",
                                            IsVisibleByDefault = true,
                                            Columns = new List<MetadataColumn>
                                                           {
                                                               new MetadataColumn
                                                                   {
                                                                       UniqueIdentifier = 200000005,
                                                                       ColumnPrefix = "PriorYearAttendance",
                                                                       MetricVariantId = 200000005,
                                                                       ColumnName = "Prior Year Attendance",
                                                                       IsVisibleByDefault = true,
                                                                       MetricListCellType = MetricListCellType.Metric,
                                                                   },
                                                               new MetadataColumn
                                                                   {
                                                                       UniqueIdentifier = 5,
                                                                       ColumnPrefix = "AttendanceYTD",
                                                                       MetricVariantId = 5,
                                                                       ColumnName = "Current Year Attendance",
                                                                       IsVisibleByDefault = false,
                                                                       MetricListCellType = MetricListCellType.Metric,
                                                                   },
                                                               new MetadataColumn
                                                                   {
                                                                       UniqueIdentifier = 200001483, 
                                                                       ColumnPrefix = "PriorYearNumberOfDaysAbsent",
                                                                       MetricVariantId = 200001483,
                                                                       ColumnName = "Prior Year Number of Days Absent",
                                                                       IsVisibleByDefault = true,
                                                                       MetricListCellType = MetricListCellType.Metric,
                                                                   },
                                                               new MetadataColumn
                                                                   {
                                                                       UniqueIdentifier = 1483, 
                                                                       ColumnPrefix = "NumberOfDaysAbsent",
                                                                       MetricVariantId = 1483,
                                                                       ColumnName = "Current Year Number of Days Absent",
                                                                       IsVisibleByDefault = false,
                                                                       MetricListCellType = MetricListCellType.Metric,
                                                                   },
                                                               new MetadataColumn
                                                                   {
                                                                       UniqueIdentifier = 200000078,
                                                                       ColumnPrefix = "PriorYearDisciplineIncidents",
                                                                       MetricVariantId = 200000078,
                                                                       ColumnName = "Prior Year State Reportable Offences",
                                                                       IsVisibleByDefault = true,
                                                                       MetricListCellType = MetricListCellType.Metric,
                                                                   },
                                                               new MetadataColumn
                                                                   {
                                                                       UniqueIdentifier = 78,
                                                                       ColumnPrefix = "DisciplineIncidents",
                                                                       MetricVariantId = 78,
                                                                       ColumnName = "Current Year State Reportable Offences",
                                                                       IsVisibleByDefault = false,
                                                                       MetricListCellType = MetricListCellType.Metric,
                                                                   },
                                                               new MetadataColumn
                                                                   {
                                                                       UniqueIdentifier = 200000079,
                                                                       ColumnPrefix = "PriorYearCodeOfConduct",
                                                                       MetricVariantId = 200000079,
                                                                       ColumnName = "Prior Year School Code of Conduct",
                                                                       IsVisibleByDefault = false,
                                                                       MetricListCellType = MetricListCellType.Metric,
                                                                   },
                                                               new MetadataColumn
                                                                   {
                                                                       UniqueIdentifier = 79,
                                                                       ColumnPrefix = "CodeOfConduct",
                                                                       MetricVariantId = 79,
                                                                       ColumnName = "Current Year School Code of Conduct",
                                                                       IsVisibleByDefault = false,
                                                                       MetricListCellType = MetricListCellType.Metric,
                                                                   }
                                                           },
                                        },
                                    new MetadataColumnGroup
                                        {
                                            GroupType = GroupType.MetricData,
                                            Title = "ASSESSMENTS",
                                            IsVisibleByDefault = true,
                                        },
                                    new MetadataColumnGroup
                                        {
                                            GroupType = GroupType.MetricData,
                                            Title = "GRADES",
                                            IsVisibleByDefault = true,
                                        },
                                };
        }

        private static List<MetadataColumnGroup> SharedPriorYearHighSchoolMetadata(List<MetadataColumnGroup> baseMetadata)
        {
            baseMetadata[1].Columns.InsertRange(2, new[]
                                                {
                                                    new MetadataColumn { UniqueIdentifier = 200000009, ColumnPrefix = "PriorYearAbsences", MetricVariantId = 200000009, ColumnName = "Prior Year Class Absences", IsVisibleByDefault = false, MetricListCellType = MetricListCellType.Metric, },
                                                    new MetadataColumn { UniqueIdentifier = 9, ColumnPrefix = "AbsencesYTD", MetricVariantId = 9, ColumnName = "Current Year Class Absences", IsVisibleByDefault = false, MetricListCellType = MetricListCellType.Metric, },
                                                });
            baseMetadata[3].Columns.AddRange(new[]
                                                {
                                                    new MetadataColumn { UniqueIdentifier = 200000026, ColumnPrefix = "PriorYearBelowC", MetricVariantId = 200000026, ColumnName = "Prior Year # Grades Below C", IsVisibleByDefault = true, MetricListCellType = MetricListCellType.TrendMetric, },
                                                    new MetadataColumn { UniqueIdentifier = 26, ColumnPrefix = "BelowC", MetricVariantId = 26, ColumnName = "Current Year # Grades Below C", IsVisibleByDefault = false, MetricListCellType = MetricListCellType.TrendMetric, },
                                                    new MetadataColumn { UniqueIdentifier = 200000024, ColumnPrefix = "PriorYearFailingGrades", MetricVariantId = 200000024, ColumnName = "Prior Year Failing Grades", IsVisibleByDefault = true, MetricListCellType = MetricListCellType.TrendMetric, },
                                                    new MetadataColumn { UniqueIdentifier = 24, ColumnPrefix = "FailingGrades", MetricVariantId = 24, ColumnName = "Current Year Failing Grades", IsVisibleByDefault = false, MetricListCellType = MetricListCellType.TrendMetric, },
                                                    new MetadataColumn { UniqueIdentifier = 40, ColumnPrefix = "Credits", MetricVariantId = 40, ColumnName = "Credits", IsVisibleByDefault = false, MetricListCellType = MetricListCellType.Metric, },
                                                    new MetadataColumn { UniqueIdentifier = 41, ColumnPrefix = "FourByFour", MetricVariantId = 41, ColumnName = "4&nbsp;X&nbsp;4", IsVisibleByDefault = false, MetricListCellType = MetricListCellType.StateValueMetric },
                                                });
            return baseMetadata;

        }

        private static List<MetadataColumnGroup> SharedPriorYearMiddleSchoolMetadata(List<MetadataColumnGroup> baseMetadata)
        {
            baseMetadata[1].Columns.InsertRange(2, new[]
                                                {
                                                    new MetadataColumn { UniqueIdentifier = 200000009, ColumnPrefix = "PriorYearAbsences", MetricVariantId = 200000009, ColumnName = "Prior Year Class Absences", IsVisibleByDefault = false, MetricListCellType = MetricListCellType.Metric, },
                                                    new MetadataColumn { UniqueIdentifier = 9, ColumnPrefix = "AbsencesYTD", MetricVariantId = 9, ColumnName = "Current Year Class Absences", IsVisibleByDefault = false, MetricListCellType = MetricListCellType.Metric, },
                                                });
            baseMetadata[3].Columns.AddRange(new[]
                                                {
                                                    new MetadataColumn { UniqueIdentifier = 200000026, ColumnPrefix = "PriorYearBelowC", MetricVariantId = 200000026, ColumnName = "Prior Year # Grades Below C", IsVisibleByDefault = true, MetricListCellType = MetricListCellType.TrendMetric, },
                                                    new MetadataColumn { UniqueIdentifier = 26, ColumnPrefix = "BelowC", MetricVariantId = 26, ColumnName = "Current Year # Grades Below C", IsVisibleByDefault = false, MetricListCellType = MetricListCellType.TrendMetric, },
                                                    new MetadataColumn { UniqueIdentifier = 200000024, ColumnPrefix = "PriorYearFailingGrades", MetricVariantId = 200000024, ColumnName = "Prior Year Failing Grades", IsVisibleByDefault = true, MetricListCellType = MetricListCellType.TrendMetric, },
                                                    new MetadataColumn { UniqueIdentifier = 24, ColumnPrefix = "FailingGrades", MetricVariantId = 24, ColumnName = "Current Year Failing Grades", IsVisibleByDefault = false, MetricListCellType = MetricListCellType.TrendMetric, },
                                                });
            return baseMetadata;
        }

        private static List<MetadataColumnGroup> SharedPriorYearElementarySchoolMetadata(List<MetadataColumnGroup> baseMetadata)
        {
            //Assessments
            baseMetadata[2].Columns.InsertRange(0, new[]
                                                {
                                                    new MetadataColumn { UniqueIdentifier = 200001487, ColumnPrefix = "PriorYearDIBELS", MetricVariantId = 200001487, ColumnName = "Prior Year DIBELS", IsVisibleByDefault = false, MetricListCellType = MetricListCellType.TrendMetric, },
                                                    new MetadataColumn { UniqueIdentifier = 1487, ColumnPrefix = "DIBELS", MetricVariantId = 1487, ColumnName = "Current Year DIBELS", IsVisibleByDefault = false, MetricListCellType = MetricListCellType.TrendMetric, },
                                                });

            //Grades
            baseMetadata[3].Columns.Clear();
            baseMetadata[3].Columns.Add(new MetadataColumn { UniqueIdentifier = 200001492, ColumnPrefix = "PriorYearFailingGradesReading", MetricVariantId = 200001492, ColumnName = "Prior Year Failing ELA / Reading Grades", IsVisibleByDefault = true, MetricListCellType = MetricListCellType.TrendMetric });
            baseMetadata[3].Columns.Add(new MetadataColumn { UniqueIdentifier = 1492, ColumnPrefix = "FailingGradesReading", MetricVariantId = 1492, ColumnName = "Current Year Failing ELA / Reading Grades", IsVisibleByDefault = false, MetricListCellType = MetricListCellType.TrendMetric });
            baseMetadata[3].Columns.Add(new MetadataColumn { UniqueIdentifier = 200001493, ColumnPrefix = "PriorYearFailingGradesWriting", MetricVariantId = 200001493, ColumnName = "Prior Year Failing Writing Grades", IsVisibleByDefault = true, MetricListCellType = MetricListCellType.TrendMetric });
            baseMetadata[3].Columns.Add(new MetadataColumn { UniqueIdentifier = 1493, ColumnPrefix = "FailingGradesWriting", MetricVariantId = 1493, ColumnName = "Current Year Failing Writing Grades", IsVisibleByDefault = false, MetricListCellType = MetricListCellType.TrendMetric });
            baseMetadata[3].Columns.Add(new MetadataColumn { UniqueIdentifier = 200001494, ColumnPrefix = "PriorYearFailingGradesMath", MetricVariantId = 200001494, ColumnName = "Prior Year Failing Math Grades", IsVisibleByDefault = true, MetricListCellType = MetricListCellType.TrendMetric });
            baseMetadata[3].Columns.Add(new MetadataColumn { UniqueIdentifier = 1494, ColumnPrefix = "FailingGradesMath", MetricVariantId = 1494, ColumnName = "Current Year Failing Math Grades", IsVisibleByDefault = false, MetricListCellType = MetricListCellType.TrendMetric });
            baseMetadata[3].Columns.Add(new MetadataColumn { UniqueIdentifier = 200001495, ColumnPrefix = "PriorYearFailingGradesScience", MetricVariantId = 200001495, ColumnName = "Prior Year Failing Science Grades", IsVisibleByDefault = true, MetricListCellType = MetricListCellType.TrendMetric });
            baseMetadata[3].Columns.Add(new MetadataColumn { UniqueIdentifier = 1495, ColumnPrefix = "FailingGradesScience", MetricVariantId = 1495, ColumnName = "Current Year Failing Science Grades", IsVisibleByDefault = false, MetricListCellType = MetricListCellType.TrendMetric });
            baseMetadata[3].Columns.Add(new MetadataColumn { UniqueIdentifier = 200001496, ColumnPrefix = "PriorYearFailingGradesSocialStudies", MetricVariantId = 200001496, ColumnName = "Prior Year Failing Social Studies Grades", IsVisibleByDefault = true, MetricListCellType = MetricListCellType.TrendMetric });
            baseMetadata[3].Columns.Add(new MetadataColumn { UniqueIdentifier = 1496, ColumnPrefix = "FailingGradesSocialStudies", MetricVariantId = 1496, ColumnName = "Current Year Failing Social Studies Grades", IsVisibleByDefault = false, MetricListCellType = MetricListCellType.TrendMetric });

            return baseMetadata;

        }

        #endregion

        #region Prior Year Student List Drilldown

        private static List<MetadataColumnGroup> PriorYearStudentListDefaultMetadata()
        {
            var model = SharedPriorYearMetadata();

            model[0].Columns.AddRange(new[]
                                          {                                          
                                              new MetadataColumn { UniqueIdentifier = SpecialMetricVariantSortingIds.Student, ColumnName = "Student", IsVisibleByDefault = true, MetricVariantId = SpecialMetricVariantSortingIds.Student, }, 
                                              new MetadataColumn { UniqueIdentifier = SpecialMetricVariantSortingIds.GradeLevel, ColumnName = "Grade Level", IsVisibleByDefault = true, MetricVariantId = SpecialMetricVariantSortingIds.GradeLevel, }, 
                                              new MetadataColumn { UniqueIdentifier = SpecialMetricVariantSortingIds.Designations, ColumnName = "Metric Value", IsVisibleByDefault = true, MetricVariantId = SpecialMetricVariantSortingIds.Designations, MetricListCellType = MetricListCellType.MetricValue }, 
                                          });
            return model;

        }

		private static List<MetadataColumnGroup> PriorYearStudentListHighSchoolMetadata()
        {
            var model = PriorYearStudentListDefaultMetadata();
            return SharedPriorYearHighSchoolMetadata(model);
        }

		private static List<MetadataColumnGroup> PriorYearStudentListMiddleSchoolMetadata()
        {
            var model = PriorYearStudentListDefaultMetadata();
            return SharedPriorYearMiddleSchoolMetadata(model);
        }

		private static List<MetadataColumnGroup> PriorYearStudentListElementarySchoolMetadata()
        {
            var model = PriorYearStudentListDefaultMetadata();
            return SharedPriorYearElementarySchoolMetadata(model);
        }

        #endregion

        #region Prior Year Classroom

        private static List<MetadataColumnGroup> PriorYearClassroomDefaultMetadata()
        {
            var model = SharedPriorYearMetadata();
            model[0].Columns.AddRange(new[]
                                          {
                                            new MetadataColumn
                                            {
                                                UniqueIdentifier = SpecialMetricVariantSortingIds.Student, 
                                                ColumnName = "Student",
                                                IsVisibleByDefault = true,
                                                MetricVariantId = SpecialMetricVariantSortingIds.Student,
                                            },
                                            new MetadataColumn
                                            {
                                                UniqueIdentifier = SpecialMetricVariantSortingIds.GradeLevel,  
                                                ColumnName = "Grade Level",
                                                IsVisibleByDefault = true,
                                                MetricVariantId = SpecialMetricVariantSortingIds.GradeLevel,
                                            },
                                            new MetadataColumn
                                            {
                                                UniqueIdentifier = SpecialMetricVariantSortingIds.Designations, 
                                                ColumnName = "Designations",
                                                IsVisibleByDefault = true,
                                                SortAscending = "sortDesignationsAsc",
                                                SortDescending = "sortDesignationsDesc",
                                                MetricVariantId = SpecialMetricVariantSortingIds.Designations,
                                            },
                                          });
            return model;

        }

		private static List<MetadataColumnGroup> PriorYearClassroomHighSchoolMetadata()
        {
            var model = PriorYearClassroomDefaultMetadata();
            return SharedPriorYearHighSchoolMetadata(model);
        }

		private static List<MetadataColumnGroup> PriorYearClassroomMiddleSchoolMetadata()
        {
            var model = PriorYearClassroomDefaultMetadata();
            return SharedPriorYearMiddleSchoolMetadata(model);
        }

		private static List<MetadataColumnGroup> PriorYearClassroomElementarySchoolMetadata()
        {
            var model = PriorYearClassroomDefaultMetadata();
            return SharedPriorYearElementarySchoolMetadata(model);
        }

        #endregion

        #region School Metric Table

        private static List<MetadataColumnGroup> SchoolMetricTableMetadata()
        {

            return new List<MetadataColumnGroup>
                       {
                           new MetadataColumnGroup
                               {
                                   GroupType = GroupType.EntityInformation,
                                   Title = "School", 
                                   IsVisibleByDefault = true,
                                   IsFixedColumnGroup = true,
                                   Columns = new List<MetadataColumn>
                                                 {
                                                     new MetadataColumn()
                                                         {
                                                             UniqueIdentifier = 1,
                                                             ColumnPrefix = "",
                                                             MetricVariantId = 1,
                                                             ColumnName = "School",
                                                             IsVisibleByDefault = true,
                                                             IsFixedColumn = true
                                                         },
                                                         new MetadataColumn()
                                                         {
                                                             UniqueIdentifier = 2,
                                                             ColumnPrefix = "",
                                                             MetricVariantId = 1,
                                                             ColumnName = "Principal",
                                                             IsVisibleByDefault = true,
                                                             IsFixedColumn = true,
                                                             MetricListCellType = MetricListCellType.None
                                                         },
                                                         new MetadataColumn()
                                                         {
                                                             UniqueIdentifier = 3,
                                                             MetricVariantId = 1,
                                                             ColumnName = "Type",
                                                             IsVisibleByDefault = true,
                                                             SortAscending = "sortSchoolTypeAsc",
                                                             SortDescending = "sortSchoolTypeDesc",
                                                             MetricListCellType = MetricListCellType.None
                                                         },
                                                         new MetadataColumn()
                                                         {
                                                             UniqueIdentifier = 4,
                                                             ColumnPrefix = "",
                                                             MetricVariantId = 1,
                                                             ColumnName = "School Metric Value",
                                                             IsVisibleByDefault = true,
                                                             SortAscending = "sortNAValueAsc",
                                                             SortDescending = "sortNAValueDesc"
                                                         },
                                                         new MetadataColumn()
                                                         {
                                                             UniqueIdentifier = 5,
                                                             ColumnPrefix = "",
                                                             MetricVariantId = 1,
                                                             ColumnName = "School Goal",
                                                             IsVisibleByDefault = true,
                                                             MetricListCellType = MetricListCellType.None
                                                         },
                                                         new MetadataColumn()
                                                         {
                                                             UniqueIdentifier = 6,
                                                             ColumnPrefix = "",
                                                             MetricVariantId = 1,
                                                             ColumnName = "",
                                                             IsVisibleByDefault = true,
                                                             MetricListCellType = MetricListCellType.None
                                                         }
                                                 }
                               }
                       };
        }

        private static List<MetadataColumnGroup> GoalPlanningSchoolMetricTableMetadata()
        {
            return new List<MetadataColumnGroup>
                       {
                           new MetadataColumnGroup
                               {
                                   GroupType = GroupType.EntityInformation,
                                   Title = "School",
                                   IsVisibleByDefault = true,
                                   IsFixedColumnGroup = true,
                                   Columns = new List<MetadataColumn>
                                                 {
                                                     new MetadataColumn
                                                        {
                                                            UniqueIdentifier = 1,
                                                            ColumnPrefix = "",
                                                            MetricVariantId = 1,
                                                            ColumnName = "School",
                                                            IsVisibleByDefault = true,
                                                            IsFixedColumn = true,
                                                            MetricListCellType = MetricListCellType.None
                                                        },
                                                        new MetadataColumn
                                                        {
                                                            ColumnName = "Principal",
                                                            IsVisibleByDefault = true,
                                                            IsFixedColumn = true
                                                        },
                                                        new MetadataColumn
                                                        {
                                                            ColumnName = "Type",
                                                            IsVisibleByDefault = true,
                                                            IsFixedColumn = true,
                                                            SortAscending = "sortSchoolTypeAsc",
                                                            SortDescending = "sortSchoolTypeDesc"
                                                        },
                                                        new MetadataColumn
                                                        {
                                                            ColumnName = "School Metric Value",
                                                            IsVisibleByDefault = true,
                                                            SortAscending = "sortNAValueAsc",
                                                            SortDescending = "sortNAValueDesc"
                                                        },
                                                        new MetadataColumn
                                                        {
                                                            ColumnName = "Current School Goal", 
                                                            IsVisibleByDefault = true
                                                        },
                                                        new MetadataColumn
                                                        {
                                                            ColumnName = "Difference From Goal",
                                                            IsVisibleByDefault = true
                                                        },
                                                        new MetadataColumn
                                                        {
                                                            ColumnName = "New Goal", 
                                                            IsVisibleByDefault = true
                                                        }
                                                                  
                                                 }
                               }
                       };
        }

        #endregion

        #region Teacher/Staff List

        private static List<MetadataColumnGroup> TeacherListMetadata()
        {
            return new List<MetadataColumnGroup>
                {
                    new MetadataColumnGroup
                        {
                            UniqueId = 99,
                            GroupType = GroupType.EntityInformation,
                            Title = string.Empty, //This is the first group that holds the staff/teacher information.
                            IsVisibleByDefault = true,
                            IsFixedColumnGroup = true,
                            Columns = new List<MetadataColumn>
                                {
                                    new MetadataColumn
                                        {
                                            UniqueIdentifier = 0,
                                            ColumnName = "Staff",
                                            IsVisibleByDefault = true,
                                            IsFixedColumn = true
                                        },
                                    new MetadataColumn
                                        {
                                            UniqueIdentifier = 1,
                                            ColumnName = "E-Mail",
                                            IsVisibleByDefault = true,
                                            IsFixedColumn = true
                                        },
                                    new MetadataColumn
                                        {
                                            UniqueIdentifier = 3,
                                            ColumnName = "Gender",
                                            IsVisibleByDefault = false,
                                            IsFixedColumn = true
                                        }
                                }
                        },
                    new MetadataColumnGroup
                        {
                            UniqueId = 98,
                            GroupType = GroupType.MetricData,
                            Title = "EXPERIENCE",
                            IsVisibleByDefault = true,
                            Columns = new List<MetadataColumn>
                                {
                                }
                        },
                    new MetadataColumnGroup
                        {
                            UniqueId = 97,
                            GroupType = GroupType.MetricData,
                            Title = "EDUCATION",
                            IsVisibleByDefault = true,
                            Columns = new List<MetadataColumn>
                                {
                                    new MetadataColumn
                                        {
                                            UniqueIdentifier = 6,
                                            ColumnName = "Highly Qualified Teacher",
                                            IsVisibleByDefault = false
                                        }
                                }
                        }
                };

        }

        private static List<MetadataColumnGroup> StaffListMetadata()
        {
            return new List<MetadataColumnGroup>
                {
                    new MetadataColumnGroup
                        {
                            UniqueId = 99,
                            GroupType = GroupType.EntityInformation,
                            Title = string.Empty, //This is the first group that holds the staff/teacher information.
                            IsVisibleByDefault = true,
                            IsFixedColumnGroup = true,
                            Columns = new List<MetadataColumn>
                                {
                                    new MetadataColumn
                                        {
                                            UniqueIdentifier = 0,
                                            ColumnName = "Staff",
                                            IsVisibleByDefault = true,
                                            IsFixedColumn = true
                                        },
                                    new MetadataColumn
                                        {
                                            UniqueIdentifier = 1,
                                            ColumnName = "E-Mail",
                                            IsVisibleByDefault = true,
                                            IsFixedColumn = true
                                        },
                                    new MetadataColumn
                                        {
                                            UniqueIdentifier = 3,
                                            ColumnName = "Gender",
                                            IsVisibleByDefault = false,
                                            IsFixedColumn = true
                                        }
                                }
                        },
                    new MetadataColumnGroup
                        {
                            UniqueId = 98,
                            GroupType = GroupType.MetricData,
                            Title = "ROLE",
                            IsVisibleByDefault = true,
                            Columns = new List<MetadataColumn>
                                {
                                    new MetadataColumn
                                        {
                                            UniqueIdentifier = 4,
                                            ColumnName = "School Role",
                                            IsVisibleByDefault = true
                                        }
                                }
                        },
                    new MetadataColumnGroup
                        {
                            UniqueId = 97,
                            GroupType = GroupType.MetricData,
                            Title = "EXPERIENCE",
                            IsVisibleByDefault = true,
                            Columns = new List<MetadataColumn>
                                {
                                }
                        },
                    new MetadataColumnGroup
                        {
                            UniqueId = 96,
                            GroupType = GroupType.MetricData,
                            Title = "EDUCATION",
                            IsVisibleByDefault = true,
                            Columns = new List<MetadataColumn>
                                {
                                    new MetadataColumn
                                        {
                                            UniqueIdentifier = 7,
                                            ColumnName = "Highly Qualified Teacher",
                                            IsVisibleByDefault = false
                                        }
                                }
                        }
                    
                };
        }


        #endregion
    }
}