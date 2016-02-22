using System.Linq;
using EdFi.Dashboards.Data.Entities;
using EdFi.Dashboards.Data.Repository;

namespace EdFi.Dashboards.Resources.StudentMetrics
{
    public class TeacherSectionFilter : IStudentMetricFilter
    {
        protected readonly IRepository<TeacherStudentSection> TeacherStudentSectionRepository;

        public TeacherSectionFilter(IRepository<TeacherStudentSection> teacherStudentSectionRepository)
        {
            TeacherStudentSectionRepository = teacherStudentSectionRepository;
        }

        public IQueryable<EnhancedStudentInformation> ApplyFilter(IQueryable<EnhancedStudentInformation> query, StudentMetricsProviderQueryOptions providerQueryOptions)
        {
            if (providerQueryOptions.TeacherSectionIds == null || !providerQueryOptions.TeacherSectionIds.Any())
                return query;

            return from studentItemWithMetrics in query
                from teacherStudentSection in
                    TeacherStudentSectionRepository.GetAll()
                        .Where(section => section.StudentUSI == studentItemWithMetrics.StudentUSI)
                where
                    providerQueryOptions.TeacherSectionIds.Contains(teacherStudentSection.TeacherSectionId)
                select studentItemWithMetrics;
        }
    }
}