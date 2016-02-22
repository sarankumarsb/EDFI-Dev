namespace EdFi.Dashboards.Resources
{
    public interface IStateAssessmentMetricIdGroupingProvider
    {
        int[] GetMetricVariantGroupIds();
    }

    public class StateAssessmentMetricIdGroupingProvider : IStateAssessmentMetricIdGroupingProvider
    {
        private readonly int[] metricVariantGroupIds = new int[]
        {
            // StateAssessmentReadingMostRecent
		    82, 88, // map to 16
		    // StateAssessmentMathMostRecent
		    83, 89, // map to 17
		    // StateAssessmentWritingMostRecent
		    84, 90, // map to 18
		    // StateAssessmentScienceMostRecent
		    85, 91, // map to 19
		    // StateAssessmentSocialStudiesMostRecent
		    86, 92, // map to 20
        };

        public virtual int[] GetMetricVariantGroupIds()
        {
            return metricVariantGroupIds;
        }
    }
}
