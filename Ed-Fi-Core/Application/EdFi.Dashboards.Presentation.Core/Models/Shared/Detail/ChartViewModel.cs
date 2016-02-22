using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.DataVisualization.Charting;
using EdFi.Dashboards.Resources.Models.Charting;

namespace EdFi.Dashboards.Presentation.Core.Models.Shared.Detail
{
    public class ChartViewModel
    {
        public ChartViewModel()
        {
            Width = 740;
            Height = 250;
            YMax = 1;
            YMin = 0;
            AxisYInterval = 0.25;
            AxisYLabelFormat = "{P0}";
            DisplayLegend = false;
            EachSeriesInNewChartArea = false;
            AxisYTitle = string.Empty;
            AxisXTitle = string.Empty;
        }

        public ChartData ChartData { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public double YMax { get; set; }
        public double YMin { get; set; }
        public double? AxisYInterval { get; set; }
        public string AxisYLabelFormat { get; set; }
        public string AxisYTitle { get; set; }
        public List<CustomLabels.CustomLabel> AxisYCustomLabels { get; set; }
        public string AxisXTitle { get; set; }
        public bool DisplayLegend { get; set; }
        public SeriesChartType ChartType { get; set; }
        public bool EachSeriesInNewChartArea { get; set; }       
    }
}