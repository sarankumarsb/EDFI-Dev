using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EdFi.Dashboards.Resource.Models.Common;

namespace EdFi.Dashboards.Application.Resources.Models
{
    [Serializable]
    public class PostResultsModel : ResourceModelBase
    {
        public bool IsPost { get; set; }
        public bool IsSuccess { get; set; }
        public IList<string> Messages { get; set; }
    }
}
