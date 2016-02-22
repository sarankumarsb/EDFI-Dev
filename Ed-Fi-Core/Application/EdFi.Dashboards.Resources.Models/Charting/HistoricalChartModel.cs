// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System;
using System.Collections.Generic;

namespace EdFi.Dashboards.Resources.Models.Charting
{
	[Serializable]
	public class HistoricalChartModel
	{

		public HistoricalChartModel()
		{
			AvailablePeriods = new List<PeriodItem>();
			ChartData = new ChartData();
		}

		public List<PeriodItem> AvailablePeriods { get; set; }
		public ChartData ChartData { get; set; }
        public string DrillDownTitle { get; set; }

		[Serializable]
		public class PeriodItem
		{
			public int Id { get; set; }
			public string Text { get; set; }
		}
	}
}
