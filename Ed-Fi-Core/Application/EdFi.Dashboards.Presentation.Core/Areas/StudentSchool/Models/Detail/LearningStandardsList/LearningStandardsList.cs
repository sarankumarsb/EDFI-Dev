using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EdFi.Dashboards.Presentation.Core.Areas.StudentSchool.Models.Detail.LearningStandardsList
{
    [Serializable]
    public class LearningStandardsModel
    {
        public IDictionary<string, string> schema { get; set; }
        public IEnumerable<LearningStandardsDataItem> data { get; set; }
    }

    [Serializable]
    public class LearningStandardsDataItem
    {
        public string name { get; set; }
        public int level { get; set; }
        public string tag { get; set; }
        public string linkToHeaders { get; set; }
        public IDictionary<string, LearningStandardsDataItemValue> values { get; set; }
        public IEnumerable<LearningStandardsDataItem> children { get; set; }
    }

    [Serializable]
    public class LearningStandardsDataItemValue
    {
        public string value { get; set; }
        public string displayValue { get; set; }
        public string type { get; set; }
        public string tooltip { get; set; }
    }
}
