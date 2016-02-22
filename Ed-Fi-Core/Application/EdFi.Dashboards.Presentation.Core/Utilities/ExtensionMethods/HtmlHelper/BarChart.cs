// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.DataVisualization.Charting;
using System.Web.UI.WebControls;
using EdFi.Dashboards.Metric.Resources.Models;
using EdFi.Dashboards.Presentation.Core.Models.Shared.Detail;
using EdFi.Dashboards.Resources.Models.Charting;
using EdFi.Dashboards.Resources.Navigation;
using ChartHatchStyle = System.Web.UI.DataVisualization.Charting.ChartHatchStyle;

namespace EdFi.Dashboards.Presentation.Web.Architecture.HtmlHelperExtensions
{
    public static partial class Html
    {
        public static IHtmlString BarChart(this HtmlHelper html, ChartViewModel viewModel)
        {
            return html.BarChart(viewModel.ChartData, viewModel.Width, viewModel.Height, viewModel.YMax, viewModel.YMin, 
                viewModel.AxisYInterval, viewModel.AxisYLabelFormat, viewModel.DisplayLegend,
                viewModel.ChartType, viewModel.EachSeriesInNewChartArea, viewModel.AxisYCustomLabels);
        }

        public static IHtmlString BarChart(this HtmlHelper html, ChartData chartData, int width, int height, double axisYMax, double axisYMin, double? axisYInterval,
                        string axisYLabelFormat, bool displayLegend, SeriesChartType chartType, 
                        bool eachSeriesInNewChartArea, List<CustomLabels.CustomLabel> axisYCustomLabels = null)
        {
            return BarChart(html,
                            chartData,
                            width,
                            height,
                            axisYMax,
                            axisYMin,
                            axisYInterval,
                            axisYLabelFormat,
                            displayLegend,
                            null,
                            null,
                            chartType,
                            eachSeriesInNewChartArea,
                            axisYCustomLabels);
        }

        public static IHtmlString BarChart(this HtmlHelper html, ChartData chartData, int width, int height, double axisYMax, double axisYMin, double? axisYInterval,
                        string axisYLabelFormat, bool displayLegend,
                        string axisXTitle, string axisYTitle,
                        SeriesChartType chartType,
                        bool eachSeriesInNewChartArea, List<CustomLabels.CustomLabel> axisYCustomLabels = null)
        {
            //get chart
            if (chartData != null)
            {
                BarChart barChart = new BarChart(chartData, width, height, axisYMax, axisYMin, axisYInterval,
                                                 axisYLabelFormat, displayLegend, chartType,
                                                 eachSeriesInNewChartArea, axisYCustomLabels);

                barChart.AxisXTitle = axisXTitle;
                barChart.AxisYTitle = axisYTitle;
                
                Chart chart = barChart.Chart;

                //render
                StringBuilder control = new StringBuilder();
                HtmlTextWriter writer = new HtmlTextWriter(new System.IO.StringWriter(control));
                chart.RenderControl(writer);

                string screenReaderTable = ScreenReaderTable(chartData);

                return new MvcHtmlString("<div aria-hidden=\"true\">" + control.ToString() + "</div>" + screenReaderTable);
            }

            return new MvcHtmlString(string.Empty);
        }

        private static string ScreenReaderTable(ChartData chartData)
        {
            List<ChartData.Series> seriesCollection = chartData.SeriesCollection;
            if (seriesCollection != null)
            {
                StringBuilder adaTable = new StringBuilder();

                adaTable.Append("<table summary=\"" + chartData.ChartTitle + "\" class=\"hidden\" >");
                // header row
                adaTable.Append("<thead><tr><th></th>");
                foreach (var point in seriesCollection[0].Points)
                {
                    TagBuilder thHeader = new TagBuilder("th");
                    thHeader.Attributes.Add("scope", "col");
                    thHeader.SetInnerText(point.AxisLabel);

                    adaTable.Append(thHeader.ToString());
                }
                adaTable.Append("</thead></tr>");

                // data rows
                adaTable.Append("<tbody>");
                foreach (var series in seriesCollection)
                {
                    StringBuilder dataRow = new StringBuilder();

                    dataRow.Append("<tr>");

                    TagBuilder tdRowValue = new TagBuilder("th");
                    tdRowValue.Attributes.Add("scope", "row");
                    tdRowValue.SetInnerText(series.Name);

                    dataRow.Append(tdRowValue.ToString());

                    foreach (var point in series.Points)
                    {
                        TagBuilder tdValue = new TagBuilder("td");
                        tdValue.SetInnerText(point.Label ?? point.Value.ToString());

                        dataRow.Append(tdValue.ToString());
                    }
                    dataRow.Append("</tr>");

                    adaTable.Append(dataRow.ToString());
                }
                adaTable.Append("</tbody>");

                adaTable.Append("</table>");

                return adaTable.ToString();
            }
            else
            {
                return string.Empty;
            }
        }

    }

    public class BarChart
    {
        private const string chartAreaName = "ChartArea1";
        private const string displayLegendName = "Display";
        private const string hideLegendName = "Hide";
        private const string seriesCustomProperty = "BarLabelStyle=Center,PixelPointWidth=60";
        private const string showLegendSeriesCustomProperty = "BarLabelStyle=Center,PixelPointWidth=50";
        private const string threeChartAreaSeriesCustomProperty = "BarLabelStyle=Center,PixelPointWidth=40";
        private const string multipleChartAreaSeriesCustomProperty = "BarLabelStyle=Center,PixelPointWidth=20";
        private const string defaultSeries = "Actual";
        private const string chartControlIdFormat = "{0}_Chart_{1}";
        private const string defaultAxisYLabelFormat = "{P0}";
        private const string trendGraphupgrayPng = "/Trend/GraphUpGray.png";
        private const string trendGraphsamegrayPng = "Trend/GraphSameGray.png";
        private const string trendGraphdowngrayPng = "Trend/GraphDownGray.png";
        private readonly Font defaultFont = new Font("Verdana", 8, FontStyle.Regular);
        private readonly Font axisTitleFont = new Font("Verdana", 9, FontStyle.Bold);
        private int chartAreaWidth = 90;
        private int chartAreaHeight = 60;
        private int horizontalOffset = 7;
        private const int verticalOffset = 10;

        public ChartData ChartData { get; set; }
        public string ChartId { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public double YMax { get; set; }
        public double YMin { get; set; }
        public string AxisYTitle { get; set; }
        public double AxisYInterval { get; set; }
        public string AxisYLabelFormat { get; set; }
        public string AxisXTitle { get; set; }
        public bool DisplayLegend { get; set; }
        public SeriesChartType ChartType { get; set; }
        public bool EachSeriesInNewChartArea { get; set; }
        public Chart Chart { get; set; }
        public List<CustomLabels.CustomLabel> AxisYCustomLabels { get; set; }

        public BarChart(ChartData chartData, int width, int height, double axisYMax, double axisYMin, double? axisYInterval, string axisYLabelFormat,
            bool displayLegend, SeriesChartType chartType, bool eachSeriesInNewChartArea, List<CustomLabels.CustomLabel> axisYCustomLabels = null)
        {
            ChartData = chartData;
            Width = width;// 740;
            Height = height;// 200;
            YMax = axisYMax;// 1;
            YMin = axisYMin;// 0;
            AxisYInterval = (axisYInterval ?? .25);
            AxisYLabelFormat = !string.IsNullOrEmpty(axisYLabelFormat) ? axisYLabelFormat : "{P0}";
            DisplayLegend = displayLegend;
            AxisXTitle = chartData.AxisXTitle;
            AxisYTitle = chartData.AxisYTitle;
            ChartType = chartType;
            EachSeriesInNewChartArea = eachSeriesInNewChartArea;
            AxisYCustomLabels = axisYCustomLabels;
            ChartId = chartData.ChartId;

            //setup control
            SetYMax();

            if (DisplayLegend)
                chartAreaWidth = 65;

            if (EachSeriesInNewChartArea)
            {
                horizontalOffset = 10;
                //chartAreaWidth = 85; chartAreaWidth - horizontalOffset;
                if (ChartData.SeriesCollection.Count > 0)
                    chartAreaWidth = chartAreaWidth / ChartData.SeriesCollection.Count;
            }

            Chart chart = SetupChart(String.Format(chartControlIdFormat, 0, ChartId));

            if (ChartData.StripLines != null)
            {
                foreach (var stripLine in ChartData.StripLines)
                {
                    if (stripLine == null)
                        continue;

                    //Adding a strip line.
                    var sl = new StripLine
                                 {
                                     Interval = -1,
                                     BorderColor = ColorTranslator.FromHtml(stripLine.Color),
                                     IntervalOffset = stripLine.Value,
                                     ToolTip = stripLine.Tooltip
                                 };

                    chart.ChartAreas[0].AxisY.StripLines.Add(sl);
                }
            }

            if (ChartData.SeriesCollection == null || ChartData.SeriesCollection.Count == 0 ||
                ChartData.SeriesCollection.First().Points.Count == 0)
            {
                var series =
                    CreateSeries(new ChartData.Series { Name = defaultSeries, Style = ChartSeriesStyle.White, ShowInLegend = false });
                int i = series.Points.AddY(0);
                series.Points[i].ToolTip = String.Empty;
                series.Points[i].Label = String.Empty;
                series.Points[i].AxisLabel = String.Empty;
                series.Points[i].Color = ColorTranslator.FromHtml(ChartColors.White);
                chart.ChartAreas[0].AxisX.MajorTickMark.Size = 0;
                chart.ChartAreas[0].AxisX.LabelStyle.ForeColor = ColorTranslator.FromHtml(ChartColors.White);

                chart.Series.Add(series);
                Chart = chart;
                return;
            }

            if (!EachSeriesInNewChartArea)
                PlotPoints(chart);
            else
                PlotPointsInMultipleChartAreas(chart);

            Chart = chart;
        }

        private void PlotPoints(Chart chart)
        {
            foreach (var seriesData in ChartData.SeriesCollection)
            {
                var series = CreateSeries(seriesData);
                chart.Series.Add(series);
                foreach (var point in seriesData.Points)
                {
                    int i = series.Points.AddY(point.Value);
	                DataPoint currentDataPoint = series.Points[i];

	                if (!String.IsNullOrEmpty(point.Tooltip))
                        currentDataPoint.ToolTip = point.Tooltip;
                    if (!String.IsNullOrEmpty(point.AxisLabel))
                        currentDataPoint.AxisLabel = point.AxisLabel;
                    currentDataPoint.IsValueShownAsLabel = point.IsValueShownAsLabel;
                    if (point.IsValueShownAsLabel && !String.IsNullOrEmpty(point.Label))
                        currentDataPoint.Label = point.Label;

                    if (point.State != MetricStateType.None)
                    {
                        var style = (point.State == MetricStateType.Good || point.State == MetricStateType.VeryGood)
                            ? ChartSeriesStyle.Green : ChartSeriesStyle.Red;

                        currentDataPoint.Color = (point.State == MetricStateType.Good ||
                                                  point.State == MetricStateType.VeryGood)
                                                     ? ColorTranslator.FromHtml(ChartColors.Green)
                                                     : ColorTranslator.FromHtml(ChartColors.Red);
                        currentDataPoint.LabelForeColor = ColorTranslator.FromHtml(ChartColors.Black);
                        currentDataPoint.BackHatchStyle = GetHatchStyle(style);
                        currentDataPoint.BackSecondaryColor = ColorTranslator.FromHtml(style.ForegroundColor);
                    }

                    if (point.Trend != TrendEvaluation.None)
                        currentDataPoint.MarkerImage = GetImagePathBasedOnTrend(point.Trend);
                }
            }
        }

        private void PlotPointsInMultipleChartAreas(Chart chart)
        {
            int chartAreaCount = 0;
            var chartArea = chart.ChartAreas[0];
            chartArea.AxisY.LabelAutoFitMinFontSize = 8;
            chartArea.AxisY.LabelAutoFitMaxFontSize = 8;
            foreach (var stripLine in ChartData.StripLines)
            {
                chartArea.AxisY.CustomLabels.Add(new CustomLabel(stripLine.Value, stripLine.Value + 1, stripLine.Label,
                                                                 0, LabelMarkStyle.None) { ForeColor = ColorTranslator.FromHtml(stripLine.Color) });
            }

            foreach (var seriesData in ChartData.SeriesCollection)
            {
                chartAreaCount++;
                var chartAreaName = String.Format("CA{0}", chartAreaCount);
                if (chartAreaCount != 1)
                {
                    chartArea =
                        CreateChartArea(new ElementPosition(horizontalOffset + chartAreaWidth * (chartAreaCount - 1),
                                                            verticalOffset, chartAreaWidth, chartAreaHeight));
                    chart.ChartAreas.Add(chartArea);

                    foreach (var stripLine in ChartData.StripLines)
                    {
                        //Adding a strip line.
                        var sl = new StripLine
                        {
                            Interval = -1,
                            BorderColor = ColorTranslator.FromHtml(stripLine.Color),
                            IntervalOffset = stripLine.Value,
                            ToolTip = stripLine.Tooltip,
                        };

                        chartArea.AxisY.StripLines.Add(sl);
                    }
                }

                chartArea.Name = chartAreaName;
                chartArea.AxisX.MajorTickMark.LineColor = Color.White;
                chartArea.AxisX.MajorTickMark.Size = 0.0f;
                chartArea.AxisX.LabelAutoFitStyle = LabelAutoFitStyles.None;
                chartArea.AxisX.Title = seriesData.Name;
                chartArea.AxisX.TitleFont = (Font)axisTitleFont.Clone();
                if (ChartData.SeriesCollection.Count > 3)
                    chartArea.AxisX.TitleFont = new Font("Verdana", 7, FontStyle.Bold);
                if (ChartData.SeriesCollection.Count > 5)
                    chartArea.AxisX.TitleFont = new Font("Verdana", 6, FontStyle.Bold);
                chartArea.AxisY.LineColor = Color.White;
                chartArea.AxisY.MajorGrid.LineColor = Color.White;
                chartArea.AxisY.MajorTickMark.LineColor = Color.White;
                chartArea.AxisY.MajorTickMark.Size = 0.0f;
                chartArea.AxisY.LabelStyle.ForeColor = Color.White;
                // this is here b/c of weird behavior with adding a Y-axis custom label and the position of X-axis labels
                chartArea.AxisY.CustomLabels.Add(new CustomLabel(0, 1, " ", 0, LabelMarkStyle.None));

                var series = CreateSeries(seriesData);
                series.ChartArea = chartAreaName;
                chart.Series.Add(series);
                foreach (var point in seriesData.Points)
                {
                    int i = series.Points.AddY(point.Value);
	                DataPoint currentDataPoint = series.Points[i];
	                if (!String.IsNullOrEmpty(point.Tooltip))
                        currentDataPoint.ToolTip = point.Tooltip;
                    if (!String.IsNullOrEmpty(point.AxisLabel))
                        currentDataPoint.AxisLabel = point.AxisLabel;
                    currentDataPoint.IsValueShownAsLabel = point.IsValueShownAsLabel;
                    if (point.IsValueShownAsLabel && !String.IsNullOrEmpty(point.Label))
                        currentDataPoint.Label = point.Label;

                    if (point.State != MetricStateType.None)
                    {
						var style = (point.State == MetricStateType.Good || point.State == MetricStateType.VeryGood)
							? ChartSeriesStyle.Green : ChartSeriesStyle.Red;

                        currentDataPoint.Color = (point.State == MetricStateType.Good ||
                                                  point.State == MetricStateType.VeryGood)
                                                     ? ColorTranslator.FromHtml(ChartColors.Green)
                                                     : ColorTranslator.FromHtml(ChartColors.Red);
						currentDataPoint.LabelForeColor = ColorTranslator.FromHtml(style.BackgroundColor);
						currentDataPoint.BackHatchStyle = GetHatchStyle(style);
						currentDataPoint.BackSecondaryColor = ColorTranslator.FromHtml(style.ForegroundColor);
                    }

                    if (point.Trend != TrendEvaluation.None)
                        currentDataPoint.MarkerImage = GetImagePathBasedOnTrend(point.Trend);
                }
            }
        }

	    private ChartHatchStyle GetHatchStyle(ChartSeriesStyle style)
	    {
		    return (ChartHatchStyle)Enum.Parse(typeof(ChartHatchStyle), ((int)style.HatchStyle).ToString());
	    }

	    private Chart SetupChart(string chartId)
        {
            //The Chart definition and look and feel
            var chart = new Chart
                            {
                                ID = chartId,
                                Width = Unit.Pixel(Width),
                                Height = Unit.Pixel(Height)
                            };

            chart.Attributes.Add("aria-hidden", "true");

            if (String.IsNullOrEmpty(AxisXTitle))
                chartAreaHeight = 70;

            var position = new ElementPosition(horizontalOffset, verticalOffset, chartAreaWidth, chartAreaHeight);

            //The ChartArea definition and look and feel
            var chartArea = CreateChartArea(position);

            if (!String.IsNullOrEmpty(AxisYTitle))
            {
                chartArea.AxisY.Title = AxisYTitle;
                chartArea.AxisY.TitleFont = (Font)axisTitleFont.Clone();
            }

            if (AxisYCustomLabels != null && AxisYCustomLabels.Count > 0)
            {
                foreach (var axisYCustomLabel in AxisYCustomLabels)
                {
                    var customLabel = new CustomLabel
                                        {
                                            FromPosition = axisYCustomLabel.FromPosition,
                                            ToPosition = axisYCustomLabel.ToPosition,
                                            Text = axisYCustomLabel.Text
                                        };

                    if (axisYCustomLabel.DisplayGridTick)
                        customLabel.GridTicks = GridTickTypes.TickMark;

                    chartArea.AxisY.CustomLabels.Add(customLabel);
                }
                
            }

            if (!String.IsNullOrEmpty(AxisXTitle))
            {
                chartArea.AxisX.Title = AxisXTitle;
                chartArea.AxisX.TitleFont = (Font)axisTitleFont.Clone();
            }

            //Adding the ChartArea to the Chart.
            chart.ChartAreas.Add(chartArea);

            if (DisplayLegend)
            {
                var displayLegend = new Legend(displayLegendName)
                                        {
                                            Alignment = StringAlignment.Center,
                                            Font = (Font)defaultFont.Clone()
                                        };

                chart.Legends.Add(displayLegend);

                var hideLegend = new Legend(hideLegendName)
                                        {
                                            Enabled = false
                                        };

                chart.Legends.Add(hideLegend);
            }

            return chart;
        }

        private ChartArea CreateChartArea(ElementPosition position)
        {
            return new ChartArea(chartAreaName)
            {
                Position = new ElementPosition(0, 0, 100, 100),
                InnerPlotPosition = position,
                AxisY =
                {
                    LineColor = ColorTranslator.FromHtml(ChartColors.Gray),
                    MajorGrid =
                    {
                        LineColor = ColorTranslator.FromHtml(ChartColors.Gray)
                    },
                    MajorTickMark =
                    {
                        LineColor = ColorTranslator.FromHtml(ChartColors.Gray),
                        Size = 1
                    },
                    Minimum = YMin,
                    Maximum = YMax,
                    Interval = AxisYInterval,
                    LabelStyle =
                    {
                        Enabled = true,
                        Format = AxisYLabelFormat,
                        Font = (Font)defaultFont.Clone()
                    }
                },
                AxisX =
                {
                    LineColor = ColorTranslator.FromHtml(ChartColors.Gray),
                    Minimum = 0,
                    Interval = 1,
                    MajorGrid =
                    {
                        LineColor = ColorTranslator.FromHtml(ChartColors.White)
                    },
                    MinorGrid =
                    {
                        LineColor = ColorTranslator.FromHtml(ChartColors.Red)
                    },
                    MinorTickMark =
                    {
                        LineColor = ColorTranslator.FromHtml(ChartColors.Gray),
                        Size = 2
                    },
                    LabelStyle =
                    {
                        Font = (Font)defaultFont.Clone()
                    },
                    LabelAutoFitMinFontSize = 8,
                    LabelAutoFitMaxFontSize = 10,
                    LabelAutoFitStyle = LabelAutoFitStyles.StaggeredLabels
                }
            };
        }

        private void SetYMax()
        {
            if (!ChartData.YAxisMaxValue.HasValue || YMax != 0)
                return;

            if (ChartData.YAxisMaxValue < .10)
            {
                YMax = .10;
                AxisYInterval = .025;
                return;
            }

            if (ChartData.YAxisMaxValue < .25)
            {
                YMax = .25;
                AxisYInterval = .05;
                return;
            }

            if (ChartData.YAxisMaxValue < .5)
            {
                YMax = .5;
                AxisYInterval = .10;
                return;
            }

            if (ChartData.YAxisMaxValue <= 1)
            {
                YMax = 1;
                return;
            }

            YMax = ChartData.YAxisMaxValue.Value;
        }

        private Series CreateSeries(ChartData.Series series)
        {
	        var chartType = ChartType;
            var lineSeries = series as ChartData.LineSeries;
            if (lineSeries != null)
                chartType = SeriesChartType.Line;

            var chartSeries = new Series(series.Name)
            {
                ChartType = chartType,
                Color = ColorTranslator.FromHtml(series.Style.BackgroundColor),
                LabelForeColor = ColorTranslator.FromHtml(ChartColors.White),
				BackHatchStyle = GetHatchStyle(series.Style),
				BackSecondaryColor = ColorTranslator.FromHtml(series.Style.ForegroundColor),
                CustomProperties = seriesCustomProperty,
                Font = (Font)defaultFont.Clone(),
                Legend = series.ShowInLegend ? displayLegendName : hideLegendName,
            };

            if (DisplayLegend && ChartData.SeriesCollection[0].Points.Count > 6)
                chartSeries.CustomProperties = showLegendSeriesCustomProperty;

            if (EachSeriesInNewChartArea && ChartData.SeriesCollection.Count == 3)
                chartSeries.CustomProperties = threeChartAreaSeriesCustomProperty;

            if (EachSeriesInNewChartArea && ChartData.SeriesCollection.Count >= 4)
                chartSeries.CustomProperties = multipleChartAreaSeriesCustomProperty;

            if (lineSeries != null)
            {
                chartSeries.BorderWidth = 2;
                chartSeries["LabelStyle"] = lineSeries.LabelStyle;
                chartSeries.MarkerImage = lineSeries.MarkerImage;
                chartSeries.LabelForeColor = ColorTranslator.FromHtml(series.Style.BackgroundColor);
                chartSeries.IsValueShownAsLabel = true;
            }

            return chartSeries;
        }

        private static string GetImagePathBasedOnTrend(TrendEvaluation trend)
        {
            switch (trend)
            {
                case TrendEvaluation.UpBad:
                case TrendEvaluation.UpGood:
                case TrendEvaluation.UpNoOpinion:
                    return EdFiWebFormsDashboards.Site.Common.ThemeImage(trendGraphupgrayPng);
                case TrendEvaluation.NoChangeNoOpinion:
                    return EdFiWebFormsDashboards.Site.Common.ThemeImage(trendGraphsamegrayPng);
                case TrendEvaluation.DownBad:
                case TrendEvaluation.DownGood:
                case TrendEvaluation.DownNoOpinion:
                    return EdFiWebFormsDashboards.Site.Common.ThemeImage(trendGraphdowngrayPng);
                default:
                    return String.Empty;
            }
        }
    }
}
