// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using Castle.Core.Logging;
using EdFi.Dashboards.Common;

namespace EdFi.Dashboards.Metric.Rendering
{
    public interface IMetricTemplateBinder
    {
        string GetTemplateName(Dictionary<string, string> renderingContextValues);
    }

    public class MetricTemplateBinder : IMetricTemplateBinder
    {
        #region Logger Property

        /// <summary>
        /// Holds the value for the Logger property.
        /// </summary>
        private ILogger _logger = NullLogger.Instance;

        /// <summary>
        /// Gets or sets the logger instance (set automatically by Castle Windsor due to the logging facility
        /// added during IoC initialization).
        /// </summary>
        public ILogger Logger
        {
            get { return _logger; }
            set { _logger = value; }
        }

        #endregion

        private readonly IMetricTemplatesProvider metricTemplatesProvider;

        public MetricTemplateBinder(IMetricTemplatesProvider metricTemplatesProvider)
        {
            this.metricTemplatesProvider = metricTemplatesProvider;
        }

        public string GetTemplateName(Dictionary<string, string> renderingContextValues)
        {
            var metricTemplates = metricTemplatesProvider.GetGroupedMetricTemplatesMetadata();

            //filter templates on DomainEntityType and MetricType
            var relevantTemplates = GetRelevantTemplates(renderingContextValues, metricTemplates);

            string templateName = GetBestMatchingTemplate(relevantTemplates, renderingContextValues);

            if (string.IsNullOrEmpty(templateName))
            {
                templateName = "~/Views/MetricTemplates/NoTemplate.cshtml";

                var message =
                    string.Format(
                        @"No metric template could be found based on the rendering context.  Are you attempting to directly render a non-top-level metric for which there are no templates?

Rendering Context:

{0}

Relevant templates are (based on MetricInstanceSetType, MetricType and RenderingMode): 
{1}", renderingContextValues.ContentsToString(), string.Join(", ", relevantTemplates.Select(x => x.Name)));
                Logger.Error(message);
            }



            return templateName;
        }


        private static MetricTemplateMetadata[] GetRelevantTemplates(IDictionary<string, string> renderingContextValues,
                                                                     ITemplateLookup metricTemplates)
        {
            var templateGroupingId = MetricTemplateMetadata.GetTemplateGroupingId(renderingContextValues);

            try
            {
                return metricTemplates[templateGroupingId];
            }
            catch (KeyNotFoundException)
            {
                return new MetricTemplateMetadata[0];
            }
        }

        private static string GetBestMatchingTemplate(IEnumerable<MetricTemplateMetadata> metricTemplates,
                                                      Dictionary<string, string> renderingContextValues)
        {
            return metricTemplates
                .Where(template => AllTokensMatch(renderingContextValues, template.Tokens))
                .OrderByDescending(template => GetTemplateMatchingScore(template.Tokens, renderingContextValues))
                .Select(template => template.FullPath)
                .FirstOrDefault();

        }

        private static bool AllTokensMatch(IDictionary<string, string> renderingContextValues,
                                           IDictionary<string, string> templateTokens)
        {
            return !(from token in templateTokens
                     let contextValue = renderingContextValues.GetValueOrDefault(token.Key)
                     where contextValue != token.Value
                     select token
                    ).Any();
        }

        private static int GetTemplateMatchingScore(Dictionary<string, string> templateMetaDataTokens,
                                                    Dictionary<string, string> metricRenderingContextValues)
        {
            return templateMetaDataTokens.Count(token => metricRenderingContextValues[token.Key] == token.Value);
        }

    }
}
