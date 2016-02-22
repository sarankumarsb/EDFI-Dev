// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;
using System.Collections.Generic;
using EdFi.Dashboards.Common;

namespace EdFi.Dashboards.Metric.Rendering
{
    public class MetricTemplateMetadata
    {
        public string Name { get; set; }
        public string FullPath { get; set; }
        public string RelativePath { get; set; }
        public Dictionary<string, string> Tokens { get; set; }
        public string TemplateGroupingId { get; set; }

        public MetricTemplateMetadata()
        {
            Tokens = new Dictionary<string, string>();
        }

        /// <summary>
        /// Gets a unique key representing a composite value of the four stable metadata token types 
        /// (MetricInstanceSetType, MetricType, RenderingMode and Open) which are used to sub-group 
        /// the metric templates to reduce the amount of iteration required while searching for matches.
        /// </summary>
        /// <param name="tokens">The tokens to be used to create the metric template groupings.</param>
        /// <returns>A metric template grouping key.</returns>
        public static string GetTemplateGroupingId(IDictionary<string, string> tokens)
        {
            return DescriptorHelper.CreateUniqueId(tokens.GetValueOrDefault("MetricInstanceSetType"),
                                            tokens.GetValueOrDefault("MetricType"),
                                            tokens.GetValueOrDefault("RenderingMode"),
                                            tokens.GetValueOrDefault("Open"));
        }
    }
}
