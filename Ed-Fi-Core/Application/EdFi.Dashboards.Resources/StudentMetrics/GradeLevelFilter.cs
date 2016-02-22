using System.Linq;
using EdFi.Dashboards.Common.Utilities;
using EdFi.Dashboards.Data.Entities;

namespace EdFi.Dashboards.Resources.StudentMetrics
{
    public class GradeLevelFilter : IStudentMetricFilter
    {
        public IQueryable<EnhancedStudentInformation> ApplyFilter(IQueryable<EnhancedStudentInformation> query, StudentMetricsProviderQueryOptions providerQueryOptions)
        {
            if (providerQueryOptions.GradeLevel == null || !providerQueryOptions.GradeLevel.Any())
                return query;

            var studentsWithinGradeLevel = ExpressionExtensions.False<EnhancedStudentInformation>();
            foreach (var gradeLevel in providerQueryOptions.GradeLevel)
            {
                var levelForClosure = gradeLevel;
                studentsWithinGradeLevel = studentsWithinGradeLevel.Or(x => x.GradeLevel == levelForClosure);
            }

            return query.Where(studentsWithinGradeLevel);
        }
    }
}