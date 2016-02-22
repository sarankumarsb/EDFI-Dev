// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;
using System.Collections.Generic;
using EdFi.Dashboards.Resource.Models.Common;
using EdFi.Dashboards.Resources.Models.Student;

namespace EdFi.Dashboards.Resources.Models.CustomGrid
{
    [Serializable]
    public class GridTable
    {
        public GridTable()
        {
            Columns = new List<Column>();
            FixedRows = new List<List<object>>();
            Rows = new List<List<object>>();
            Footer = new List<List<object>>();
            EntityIds = new List<long>();
        }

        public int MetricVariantId { get; set; }

        public List<Column> Columns { get; set; }

        public List<List<object>> FixedRows { get; set; }

        public List<List<object>> Rows { get; set; }

        public List<List<object>> Footer { get; set; }

        public int TotalRows { get; set; }

        public List<long> EntityIds { get; set; }

        public int SchoolId { get; set; }

        /// <summary>
        /// Gets or sets the watch list that will be allowed to modify the grid
        /// data.
        /// </summary>
        /// <value>
        /// The watch list.
        /// </value>
        public EdFiGridWatchListModel WatchList { get; set; }
    }

    #region Classes for Columns
    [Serializable]
    public class Column
    {
        public Column()
        {
            U = -1;
            Children = new List<Column>();
        }

        public int U { get; set; }
        public bool IsVisibleByDefault { get; set; }
        public List<Column> Children { get; set; }
        public string SortAscending { get; set; }
        public string SortDescending { get; set; }
        public bool IsFixedColumn { get; set; }

        private string _overriddenWidth;

        public string OverriddenWidth
        {
            get { return _overriddenWidth ?? string.Empty; }
            set { _overriddenWidth = value; }
        }
    }

    [Serializable]
    public class TextColumn : Column
    {     
        /// <summary>
        /// The supplied field name to be used in the header of the row.
        /// </summary>
        /// <example>"Full Name instead of 'FullName'"</example>        
        public string DisplayName{ get; set; }

        public string Tooltip { get; set; }
    }

    [Serializable]
    public class ImageColumn : Column
    {
        public string Src { get; set; }
    }
    #endregion


    #region Classes for Cells
    [Serializable]
    public class CellItem<T>
    {
        /// <summary>
        /// The DisplayValue optimized for JSON transmission.
        /// </summary>
        public string DV { get; set; }

        /// <summary>
        /// The Value optimized for JSON transmission.
        /// </summary>
        public T V { get; set; }
    }

    [Serializable]
    public class SchoolCellItem<TValue> : CellItem<TValue>, IResourceModelBase
    {
        /// <summary>
        /// The property that defines the type when serialized...
        /// </summary>
        public int CId { get; set; }

        public string Url { get; set; }

        public IEnumerable<Link> Links { get; set; }
    }

    [Serializable]
    public class MetricCellItem<TValue> : CellItem<TValue>
    {
        /// <summary>
        /// The State
        /// </summary>
        public int S { get; set; }
    }

    [Serializable]
    public class TrendMetricCellItem<TValue> : CellItem<TValue>
    {
        /// <summary>
        /// The Trend
        /// </summary>
        public string T { get; set; }

        /// <summary>
        /// The State
        /// </summary>
        public int S { get; set; }
    }

    [Serializable]
    public class AssessmentMetricCellItem<TValue> : CellItem<TValue>
    {
        /// <summary>
        /// The Accommodations
        /// </summary>
        public int A { get; set; }

        /// <summary>
        /// The State
        /// </summary>
        public int S { get; set; }
        public string Tooltip { get; set; }
    }

    [Serializable]
    public class StateTextCellItem<TValue> : CellItem<TValue>
    {
        /// <summary>
        /// The State
        /// </summary>
		public int STe { get; set; }
    }

    [Serializable]
    public class StudentCellItem<TValue> : CellItem<TValue>, IStudent, IResourceModelBase
    {
        public StudentCellItem() { }
        
        public StudentCellItem(long studentUSI)
        {
            StudentUSI = studentUSI;
        }

        /// <summary>
        /// The Student Id
        /// </summary>
        public long StudentUSI { get; set; }

        /// <summary>
        /// The School Id
        /// </summary>
        public int? CId { get; set; }

        /// <summary>
        /// The Image
        /// </summary>
        public string I { get; set; }

        /// <summary>
        /// The List Unique Id
        /// </summary>
        public string LUId { get; set; }

        //Properties from IResourceModelBase
        public string Url { get; set; }

        public IEnumerable<Link> Links { get; set; }

        //Student Unique ID Newly Added : Saravanan
        public string StudentUniqueID { get; set; }
    }

    [Serializable]
    public class TeacherCellItem<TValue> : CellItem<TValue>, IResourceModelBase
    {
        /// <summary>
        /// The Teacher Id
        /// </summary>
        public long TId { get; set; }

        /// <summary>
        /// The School Id
        /// </summary>
        public int CId { get; set; }

        /// <summary>
        /// The Image
        /// </summary>
        public string I { get; set; }

        /// <summary>
        /// The List Unique Id
        /// </summary>
        public string LUId { get; set; }

        public string Url { get; set; }

        public IEnumerable<Link> Links { get; set; }
    }

    [Serializable]
    public class StaffCellItem<TValue> : CellItem<TValue>
    {
        /// <summary>
        /// The Staff Id
        /// </summary>
        public long StId { get; set; }

        /// <summary>
        /// The School Id
        /// </summary>
        public int CId { get; set; }

        /// <summary>
        /// The Image
        /// </summary>
        public string I { get; set; }

        /// <summary>
        /// The List Unique Id
        /// </summary>
        public string LUId { get; set; }
    }

    [Serializable]
    public class FlagCellItem<TValue> : CellItem<TValue>
    {
        /// <summary>
        /// The Flag
        /// </summary>
        public bool F { get; set; }
    }

    [Serializable]
    public class SpacerCellItem<TValue> : CellItem<TValue>
    {
        /// <summary>
        /// The Flag
        /// </summary>
        public string SP { get; set; }
	}

    [Serializable]
    public class DesignationsCellItem<TValue> : CellItem<TValue>
    {
        /// <summary>
        /// The Designations.
        /// </summary>
        public List<Accommodations> D { get; set; }
	}

    [Serializable]
    public class ObjectiveCellItem<TValue> : CellItem<TValue>
    {
        /// <summary>
        /// The State for the objective.
        /// </summary>
        public int ST { get; set; }
	}

    [Serializable]
    public class EmailCellItem<TValue> : CellItem<TValue>
    {
        /// <summary>
        /// The Email.
        /// </summary>
        public string M { get; set; }
    }

    [Serializable]
    public class YearsOfExperienceCellItem<TValue> : CellItem<TValue>
    {
        /// <summary>
        /// The Years of experience.
        /// </summary>
        public decimal? Y { get; set; }
    }

    [Serializable]
    public class HighestLevelOfEducationCellItem<TValue> : CellItem<TValue>
    {
        /// <summary>
        /// The highest level of education.
        /// </summary>
        public string E { get; set; }
    }

    [Serializable]
    public class ObjectiveTextCellItem<TValue> : CellItem<TValue>
    {
        /// <summary>
        /// The Objective marker.
        /// </summary>
        public string O { get; set; }
    }

    [Serializable]
    public class GoalPlanningCellItem<TValue> : CellItem<TValue>
    {
        /// <summary>
        /// The goal
        /// </summary>
        public TValue G { get; set; }

        /// <summary>
        /// The education organization identifier
        /// </summary>
        public int EId { get; set; }

        /// <summary>
        /// The metric id
        /// </summary>
        public int[] MIds { get; set; }

        /// <summary>
        /// The goal planning id
        /// </summary>
        public int[] GPIds { get; set; }

        public bool I { get; set; }
    }
    #endregion
}
