// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System;

namespace EdFi.Dashboards.Metric.Resources.Services
{
    /// <summary>
    /// Provides an abstraction for domain-specific implementations representing a request for a specific set of metric instances usually associated 
    /// with either a domain entity, or a relationship between domain entities.
    /// </summary>
    public abstract class MetricInstanceSetRequestBase
    {
        /// <summary>
        /// Gets the numeric identifier of the type of the metric instance set, corresponding to the value inserted by the Ed-Fi ETL packages when 
        /// building the <see cref="Guid"/> for the metric instance set key.
        /// </summary>
        public abstract int MetricInstanceSetTypeId { get; }

        public int MetricVariantId { get; set; }
    }
}