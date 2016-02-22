// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System.Runtime.Remoting.Messaging;

namespace EdFi.Dashboards.Core.Providers.Context
{
    public interface IEdFiDashboardContextProvider
    {
        EdFiDashboardContext GetEdFiDashboardContext();
    }

    /// <summary>
    /// Defines properties for the domain-specific identifiers that are available in the context of the current Ed-Fi dashboard web request.
    /// </summary>
    public class EdFiDashboardContext
    {
        public const string CallContextKey = "EdFiDashboardContext";

        public MetricInstanceSetType MetricInstanceSetType { get; set; }

        public int? LocalEducationAgencyId { get; set; }
        
        /// <summary>
        /// Gets or sets the local education agency code used for routing (not the local education agency's name).
        /// </summary>
        public string LocalEducationAgency { get; set; }
        
        public int? SchoolId { get; set; }
        public long? StaffUSI { get; set; }
        public long? SectionOrCohortId { get; set; }
        public string StudentListType { get; set; }
        public string ViewType { get; set; }
        public string SubjectArea { get; set; }
        public long? StudentUSI { get; set; }
        public int? MetricVariantId { get; set; }
        public string RoutedUrl { get; set; }
        
        /// <summary>
        /// Gets the current dashboard context from the current thread call context if it's available; or <b>null</b> if no dashboard context is available.
        /// </summary>
        public static EdFiDashboardContext Current
        {
            get { return CallContext.GetData(CallContextKey) as EdFiDashboardContext; }
        }
    }
}