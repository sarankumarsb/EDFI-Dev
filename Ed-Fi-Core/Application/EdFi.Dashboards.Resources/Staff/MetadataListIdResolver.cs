using System;
using EdFi.Dashboards.Core;
using EdFi.Dashboards.Resources.Models.CustomGrid;

namespace EdFi.Dashboards.Resources.Staff
{
    public interface IMetadataListIdResolver
    {
        int GetListId(ListType listType, SchoolCategory schoolCategory);
        int GetListId(ListType listType, SchoolCategory schoolCategory, string subjectArea);
    }

    [Obsolete("This implementation gets replaced by DatabaseMetadataListIdResolver and will be removed in the next Major release version of the Dashboard.")]
    public class MetadataListIdResolver : IMetadataListIdResolver
    {
        public const int StudentDrilldownHighSchoolListId = 1;
        public const int StudentDrilldownMiddleSchoolListId = 2;
        public const int StudentDrilldownElementarySchoolListId = 3;

        public const int GeneralOverviewHighSchoolListId = 4;
        public const int GeneralOverviewMiddleSchoolListId = 5;
        public const int GeneralOverviewElementarySchoolListId = 6;

        public const int SubjectSpecificHighSchoolListId = 7;
        public const int SubjectSpecificMiddleSchoolListId = 8;
        public const int SubjectSpecificElementarySchoolListId = 9;

        public const int StudentDemographicHighSchoolListId = 10;
        public const int StudentDemographicMiddleSchoolListId = 11;
        public const int StudentDemographicElementarySchoolListId = 12;

        public const int PriorYearStudentDrilldownHighSchoolListId = 13;
        public const int PriorYearStudentDrilldownMiddleSchoolListId = 14;
        public const int PriorYearStudentDrilldownElementarySchoolListId = 15;

        public const int PriorYearClassroomHighSchoolListId = 16;
        public const int PriorYearClassroomMiddleSchoolListId = 17;
        public const int PriorYearClassroomElementarySchoolListId = 18;

        public const int SchoolMetricTableListId = 19;
        public const int PriorYearSchoolMetricTableListId = 26;
        public const int GoalPlanningSchoolMetricTableListId = 27;

        public const int StudentGradeHighSchoolListId = 20;
        public const int StudentGradeMiddleSchoolListId = 21;
        public const int StudentGradeElementarySchoolListId = 22;

        public const int StudentSchoolCategoryHighSchoolListId = 23;
        public const int StudentSchoolCategoryMiddleSchoolListId = 24;
        public const int StudentSchoolCategoryElementarySchoolListId = 25;

        public const int TeacherListId = 30;
        public const int StaffListId = 31;


        public int GetListId(ListType listType, SchoolCategory schoolCategory)
        {
            switch (listType)
            {
                case ListType.StudentDrilldown:
                    switch (schoolCategory)
                    {
                        case SchoolCategory.Elementary:
                            return StudentDrilldownElementarySchoolListId;
                        case SchoolCategory.MiddleSchool:
                            return StudentDrilldownMiddleSchoolListId;
                        default: //The rest behave like High school
                            return StudentDrilldownHighSchoolListId;
                    }
                case ListType.ClassroomGeneralOverview:
                    switch (schoolCategory)
                    {
                        case SchoolCategory.Elementary:
                            return GeneralOverviewElementarySchoolListId;
                        case SchoolCategory.MiddleSchool:
                            return GeneralOverviewMiddleSchoolListId;
                        default: //The rest behave like High school
                            return GeneralOverviewHighSchoolListId;
                    }
                case ListType.ClassroomSubjectSpecific:
                    switch (schoolCategory)
                    {
                        case SchoolCategory.Elementary:
                            return SubjectSpecificElementarySchoolListId;
                        case SchoolCategory.MiddleSchool:
                            return SubjectSpecificMiddleSchoolListId;
                        default: //The rest behave like High school
                            return SubjectSpecificHighSchoolListId;
                    }
                case ListType.StudentDemographic:
                    switch (schoolCategory)
                    {
                        case SchoolCategory.Elementary:
                            return StudentDemographicElementarySchoolListId;
                        case SchoolCategory.MiddleSchool:
                            return StudentDemographicMiddleSchoolListId;
                        default: //The rest behave like High school
                            return StudentDemographicHighSchoolListId;
                    }
                case ListType.StudentGrade:
                    switch (schoolCategory)
                    {
                        case SchoolCategory.Elementary:
                            return StudentGradeElementarySchoolListId;
                        case SchoolCategory.MiddleSchool:
                            return StudentGradeMiddleSchoolListId;
                        default: //The rest behave like High school
                            return StudentGradeHighSchoolListId;
                    }
                case ListType.StudentSchoolCategory:
                    switch (schoolCategory)
                    {
                        case SchoolCategory.Elementary:
                            return StudentSchoolCategoryElementarySchoolListId;
                        case SchoolCategory.MiddleSchool:
                            return StudentSchoolCategoryMiddleSchoolListId;
                        default: //The rest behave like High school
                            return StudentSchoolCategoryHighSchoolListId;
                    }
                case ListType.PriorYearStudentDrilldown:
                    switch (schoolCategory)
                    {
                        case SchoolCategory.Elementary:
                            return PriorYearStudentDrilldownElementarySchoolListId;
                        case SchoolCategory.MiddleSchool:
                            return PriorYearStudentDrilldownMiddleSchoolListId;
                        default: //The rest behave like High school
                            return PriorYearStudentDrilldownHighSchoolListId;
                    }
                case ListType.ClassroomPriorYear:
                    switch (schoolCategory)
                    {
                        case SchoolCategory.Elementary:
                            return PriorYearClassroomElementarySchoolListId;
                        case SchoolCategory.MiddleSchool:
                            return PriorYearClassroomMiddleSchoolListId;
                        default: //The rest behave like High school
                            return PriorYearClassroomHighSchoolListId;
                    }
                case ListType.SchoolMetricTable:
                    {
                        return SchoolMetricTableListId;
                    }
                case ListType.PriorYearSchoolMetricTable:
                    {
                        return PriorYearSchoolMetricTableListId;
                    }
                case ListType.GoalPlanningSchoolMetricTable:
                    {
                        return GoalPlanningSchoolMetricTableListId;
                    }
                case ListType.Teacher:
                    {
                        return TeacherListId;
                    }
                case ListType.Staff:
                    {
                        return StaffListId;
                    }

                default:
                    throw new ArgumentOutOfRangeException("listType");
            }
        }

        public int GetListId(ListType listType, SchoolCategory schoolCategory, string subjectArea)
        {
            return GetListId(listType, schoolCategory);
        }
    }
}
