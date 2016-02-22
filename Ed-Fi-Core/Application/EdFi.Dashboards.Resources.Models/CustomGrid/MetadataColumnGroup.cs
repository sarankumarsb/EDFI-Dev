using System;
using System.Collections.Generic;
using EdFi.Dashboards.Core;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace EdFi.Dashboards.Resources.Models.CustomGrid
{
    [Serializable]
    public class MetadataColumnGroup
    {
        public MetadataColumnGroup()
        {
            Columns = new List<MetadataColumn>();
            UniqueId = -1;
        }
        public GroupType GroupType { get; set; }
        public string Title { get; set; }
        public List<MetadataColumn> Columns { get; set; }
        public bool IsVisibleByDefault { get; set; }
        public bool IsFixedColumnGroup { get; set; }
        public int UniqueId { get; set; }
    }

    [Serializable]
    public class MetadataColumn
    {
        public int UniqueIdentifier { get; set; }
        public string ColumnName { get; set; }
        public int MetricVariantId { get; set; }
        public SchoolCategory SchoolCategory { get; set; }
        public int Order { get; set; }
        public bool IsVisibleByDefault { get; set; }
        public bool IsFixedColumn { get; set; }
        public MetricListCellType MetricListCellType { get; set; }
        public string ColumnPrefix { get; set; }
        public string SortAscending { get; set; }
        public string SortDescending { get; set; }
        public string Tooltip { get; set; }
    }

    public enum MetricListCellType
    {
        None,
        MetricValue, // the corresponding metric value for a student list
        Metric,         // basic metric
        TrendMetric,    // metric with trend
        AssessmentMetric,   
        StateValueMetric
    }

    public enum ListType
    {
        StudentDrilldown = 1,
        ClassroomGeneralOverview = 2,
        ClassroomSubjectSpecific = 3,
        StudentDemographic = 4,
        StudentGrade = 5,
        StudentSchoolCategory = 6,
        PriorYearStudentDrilldown = 7,
        ClassroomPriorYear = 8,
        SchoolMetricTable = 9,
        PriorYearSchoolMetricTable = 10,
        GoalPlanningSchoolMetricTable = 11,
        Teacher = 12,
        Staff = 13
    }

    public enum GroupType
    {
        EntityInformation,
        MetricData
    }
}
