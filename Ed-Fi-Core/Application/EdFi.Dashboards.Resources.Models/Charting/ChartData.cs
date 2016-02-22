// *************************************************************************
// ©2012 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System;
using System.Collections.Generic;
using EdFi.Dashboards.Metric.Resources.Models;

namespace EdFi.Dashboards.Resources.Models.Charting
{
    public static class ChartColors
    {
        public const string Red = "#b30002"; // Was #E62325
        public const string LightRed = "#d14749";
        public const string Green = "#0a662f"; // Was #10A34A
        public const string Blue = "#345a89"; // Was #4F81BD
        public const string LightBlue = "#5c7799";
        public const string BlueLimit = "#0000FF";
        public const string Gray = "#BBBBBB";
        public const string White = "#FFFFFF";
        public const string Black = "#000000";
    }

    public enum ChartHatchStyle
    {
        // These enumeration values correspond to those in System.Web.UI.DataVisualization.Charting.ChartHatchStyle.
        // Defining them in this way prevents us from having to take a dependency on System.Web.UI.

        Blank = 0, // "Plain"
        StyleOne = 53, // "Wide upward diagonal"
        StyleTwo = 52 // "Wide downward diagonal"
    }

    [Serializable]
    public class ChartSeriesStyle
    {
        public ChartHatchStyle HatchStyle { get; set; }
        public string BackgroundColor { get; set; }
        public string ForegroundColor { get; set; }

        public ChartSeriesStyle() { }

        public ChartSeriesStyle(ChartHatchStyle hatchStyle, string backgroundColor, string foregroundColor)
            : this()
        {
            HatchStyle = hatchStyle;
            BackgroundColor = backgroundColor;
            ForegroundColor = foregroundColor;
        }

        public static ChartSeriesStyle Green = new ChartSeriesStyle(ChartHatchStyle.Blank, ChartColors.Green, ChartColors.Green);
        public static ChartSeriesStyle Blue = new ChartSeriesStyle(ChartHatchStyle.StyleOne, ChartColors.Blue, ChartColors.LightBlue);
        public static ChartSeriesStyle Red = new ChartSeriesStyle(ChartHatchStyle.StyleTwo, ChartColors.Red, ChartColors.LightRed);
        public static ChartSeriesStyle White = new ChartSeriesStyle(ChartHatchStyle.Blank, ChartColors.White, ChartColors.White);
        public static ChartSeriesStyle Gray = new ChartSeriesStyle(ChartHatchStyle.Blank, ChartColors.Gray, ChartColors.Gray);
        public static ChartSeriesStyle Black = new ChartSeriesStyle(ChartHatchStyle.Blank, ChartColors.Black, ChartColors.Black);
        public static ChartSeriesStyle BlueLimit = new ChartSeriesStyle(ChartHatchStyle.Blank, ChartColors.BlueLimit, ChartColors.BlueLimit);
    }

    [Serializable]
    public class ChartData
    {
        public ChartData()
        {
            SeriesCollection = new List<Series>();
            StripLines = new List<StripLine>();
            YAxisLabels = new List<AxisLabel>();
            //Defaulting OverviewChartIsEnabled to true for legacy reasons.  We used to always show the overview, but
            // need to disable it in some cases.
            //TODO: These properties are View concern and should be refactored into a wrapped ViewModel in a Controller.
            //This decision was made becuase of time constraints.
            OverviewChartIsEnabled = true;
            ShowMouseOverToolTipOnLeft = true;
        }

        public string ChartId { get; set; }
        public string ChartTitle { get; set; }
        public double? YAxisMaxValue { get; set; }
        public string AxisXTitle { get; set; }
        public string AxisYTitle { get; set; }
        public string Goal { get; set; }
        public List<Series> SeriesCollection { get; set; }
        public List<StripLine> StripLines { get; set; }
        public string DisplayType { get; set; }
        public List<AxisLabel> YAxisLabels { get; set; }
        public string Context { get; set; }
        public string SubContext { get; set; }
        public bool OverviewChartIsEnabled { get; set; }
        public bool ShowMouseOverToolTipOnLeft { get; set; }

        [Serializable]
        public class Series
        {
            public Series()
            {
                Points = new List<Point>();
                Style = ChartSeriesStyle.Blue;
            }

            public string Name { get; set; }
            public ChartSeriesStyle Style { get; set; }
            public bool ShowInLegend { get; set; }
            public List<Point> Points { get; set; }
        }

        [Serializable]
        public class LineSeries : Series
        {
            public string MarkerImage { get; set; }
            public string LabelStyle { get; set; }
        }

        [Serializable]
        public class Point
        {
            public string Label { get; set; }
            public string SubLabel { get; set; }
            public string AxisLabel { get; set; }
            public string ValueAsText { get; set; }
            public double Value { get; set; }
            public string ValueType { get; set; }
            public int? Numerator { get; set; }
            public int? Denominator { get; set; }
            public string TooltipHeader { get; set; }
            public string Tooltip { get; set; }
            public bool IsValueShownAsLabel { get; set; }
            public decimal? RatioLocation { get; set; }

            public TrendEvaluation Trend { get; set; }
            public MetricStateType State { get; set; }

        }

        [Serializable]
        public class StripLine
        {
            public double Value { get; set; }
            public string Tooltip { get; set; }
            /// <summary>
            /// If there is a threshold to be set towards the data then we use it. 
            /// But it is important to know if this threshold is a positive or a negative one.
            /// In general they are positive so this is why we call it IsNegative.
            /// </summary>
            public bool IsNegativeThreshold { get; set; }
            private string _color;

            public string Color
            {
                get { return _color ?? ChartColors.BlueLimit; }
                set { _color = value; }
            }
            public string Label { get; set; }
        }

        [Serializable]
        public class AxisLabel
        {
            public decimal? Position { get; set; }
            public decimal? MinPosition { get; set; }
            public decimal? MaxPosition { get; set; }
            public string Text { get; set; }
        }
    }

    [Serializable]
    public class CustomLabels
    {
        public List<CustomLabel> CustomLabelsCollection { get; set; }

        [Serializable]
        public class CustomLabel
        {
            public double FromPosition { get; set; }
            public double ToPosition { get; set; }
            public string Text { get; set; }
            public bool DisplayGridTick { get; set; }
        }
    }
}
