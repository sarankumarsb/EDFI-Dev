namespace EdFi.Dashboards.Presentation.Core.Tests.Routing.Support
{
    /// <summary>
    /// Provides distinct ID values for various types of identifiers used in the routing design and corresponding request models.
    /// </summary>
    public class TestId
    {
        public static int LocalEducationAgency = 1;
        public static int School               = 2;
        public static int Staff                = 3;
        public static int Student              = 4;
        public static int MetricVariant        = 5;   // TODO: GKM - Always map all operational subtypes to the same metricVariantId? What subtypes are there?
        public static long SectionOrCohort    = 6;
        public static int MetricBasedWatchList = 7;
    }
}