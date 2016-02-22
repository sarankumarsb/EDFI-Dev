using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using EdFi.Dashboards.Resource.Models.Common;
using Newtonsoft.Json;
using ResourceModelBase = EdFi.Dashboards.Resource.Models.Common.Hal.ResourceModelBase;

namespace EdFi.Dashboards.Application.Resources.Models.LocalEducationAgency
{
    [Serializable]
    public class PlannedGoalModel : ResourceModelBase
    {
        /// <summary>
        /// Gets or sets the identifier of the LEA to which the goal applies.
        /// </summary>
        [JsonIgnore] // Provided by route, don't include in JSON payload
        public int LocalEducationAgencyId { get; set; }

        /// <summary>
        /// Gets or sets the metric identifier to which the goal applies.
        /// </summary>
        [JsonIgnore] // Provided by route, don't include in JSON payload
        public int MetricId { get; set; }

        /// <summary>
        /// Gets or sets the planned value of the goal.  The goal must be published to take effect.
        /// </summary>
        public decimal Goal { get; set; }
    }
}
