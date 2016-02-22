using System.Collections.Generic;

namespace EdFi.Dashboards.Resources.Models.Common
{
    /// <summary>
    /// Contains a name and a list of values; this object is used by the
    /// metrics based watch list to pass name/values pairs.
    /// </summary>
    public class NameValuesType
    {
        /// <summary>
        /// Gets or sets the key name.
        /// </summary>
        /// <value>
        /// The key name.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the selected values.
        /// </summary>
        /// <value>
        /// The selected values.
        /// </value>
        public List<string> Values { get; set; }
    }
}
