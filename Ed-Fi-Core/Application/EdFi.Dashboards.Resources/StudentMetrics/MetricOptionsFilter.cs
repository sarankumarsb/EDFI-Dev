using System.Linq;
using EdFi.Dashboards.Common.Utilities;
using EdFi.Dashboards.Data.Entities;
using EdFi.Dashboards.Data.Repository;
using EdFi.Dashboards.Metric.Data.Entities;

namespace EdFi.Dashboards.Resources.StudentMetrics
{
    public class MetricOptionsFilter : IStudentMetricFilter
    {
        protected readonly IRepository<StudentMetric> StudentMetricRepository;
        protected readonly IRepository<MetricInstanceExtendedPropertyWithValueToFloat> MetricInstanceExtendedPropertyWithValueToFloatRepository;

        public MetricOptionsFilter(
            IRepository<StudentMetric> studentMetricRepository,
            IRepository<MetricInstanceExtendedPropertyWithValueToFloat> metricInstanceExtendedPropertyWithValueToFloatRepository)
        {
            StudentMetricRepository = studentMetricRepository;
            MetricInstanceExtendedPropertyWithValueToFloatRepository = metricInstanceExtendedPropertyWithValueToFloatRepository;
        }

        public IQueryable<EnhancedStudentInformation> ApplyFilter(IQueryable<EnhancedStudentInformation> query, StudentMetricsProviderQueryOptions providerQueryOptions)
        {
            if (providerQueryOptions.MetricOptionGroups == null)
                return query;

            foreach (var metricFilterOptionGroup in providerQueryOptions.MetricOptionGroups)
            {
                if (metricFilterOptionGroup.MetricFilterOptions == null ||
                    !metricFilterOptionGroup.MetricFilterOptions.Any())
                    continue;

                var studentsWithinMetrics = ExpressionExtensions.False<EnhancedStudentInformation>();

                foreach (var metricFilterOption in metricFilterOptionGroup.MetricFilterOptions)
                {
                    var studentsMeetingCurrentOption = ExpressionExtensions.True<EnhancedStudentInformation>();

                    if (metricFilterOption.MetricStateEqualTo.HasValue)
                    {
                        var metricStateTypeId = metricFilterOption.MetricStateEqualTo.Value;

                        studentsMeetingCurrentOption =
                            studentsMeetingCurrentOption.And(information => StudentMetricRepository.GetAll()
                                .Any(
                                    data =>
                                        data.StudentUSI == information.StudentUSI &&
                                        data.SchoolId == information.SchoolId &&
                                        data.MetricId == metricFilterOption.MetricId &&
                                        data.MetricStateTypeId == metricStateTypeId));
                    }

                    if (metricFilterOption.MetricStateNotEqualTo.HasValue)
                    {
                        var metricStateTypeId = metricFilterOption.MetricStateNotEqualTo.Value;

                        studentsMeetingCurrentOption =
                            studentsMeetingCurrentOption.And(information => StudentMetricRepository.GetAll()
                                .Any(
                                    data =>
                                        data.StudentUSI == information.StudentUSI &&
                                        data.SchoolId == information.SchoolId &&
                                        data.MetricId == metricFilterOption.MetricId &&
                                        data.MetricStateTypeId != metricStateTypeId));
                    }

                    if (metricFilterOption.MinInclusiveMetricInstanceExtendedProperty != null)
                    {
                        studentsMeetingCurrentOption =
                            studentsMeetingCurrentOption.And(
                                information => MetricInstanceExtendedPropertyWithValueToFloatRepository.GetAll()
                                    .SelectMany(metricInstanceExtProp => StudentMetricRepository.GetAll().Select(setKey => new { metricInstanceExtProp, setKey }))
                                    .Any(data => data.metricInstanceExtProp.MetricId == metricFilterOption.MetricId &&
                                                 data.setKey.MetricId == data.metricInstanceExtProp.MetricId &&
                                                 data.metricInstanceExtProp.MetricInstanceSetKey == data.setKey.MetricInstanceSetKey &&
                                                 data.setKey.StudentUSI == information.StudentUSI &&
                                                 data.setKey.SchoolId == information.SchoolId &&
                                                 data.metricInstanceExtProp.Name == metricFilterOption.MinInclusiveMetricInstanceExtendedProperty &&
                                                 data.setKey.ValueSortOrder >= data.metricInstanceExtProp.ValueToFloat));
                    }

                    if (metricFilterOption.MaxExclusiveMetricInstanceExtendedProperty != null)
                    {
                        studentsMeetingCurrentOption =
                            studentsMeetingCurrentOption.And(
                                information => MetricInstanceExtendedPropertyWithValueToFloatRepository.GetAll()
                                    .SelectMany(metricInstanceExtProp => StudentMetricRepository.GetAll().Select(setKey => new { metricInstanceExtProp, setKey }))
                                    .Any(data => data.metricInstanceExtProp.MetricId == metricFilterOption.MetricId &&
                                                 data.setKey.MetricId == data.metricInstanceExtProp.MetricId &&
                                                 data.metricInstanceExtProp.MetricInstanceSetKey == data.setKey.MetricInstanceSetKey &&
                                                 data.setKey.StudentUSI == information.StudentUSI &&
                                                 data.setKey.SchoolId == information.SchoolId &&
                                                 data.metricInstanceExtProp.Name == metricFilterOption.MaxExclusiveMetricInstanceExtendedProperty &&
                                                 data.setKey.ValueSortOrder < data.metricInstanceExtProp.ValueToFloat));
                    }

                    if (metricFilterOption.MetricInstanceEqualTo != null)
                    {
                        studentsMeetingCurrentOption = studentsMeetingCurrentOption.And(
                            information => StudentMetricRepository.GetAll()
                                .SelectMany(
                                    metricInstance =>
                                        StudentMetricRepository.GetAll().Select(setKey => new { metricInstance, setKey }))
                                .Any(data => data.metricInstance.MetricId == metricFilterOption.MetricId &&
                                             data.setKey.MetricId == data.metricInstance.MetricId &&
                                             data.metricInstance.MetricInstanceSetKey ==
                                             data.setKey.MetricInstanceSetKey &&
                                             data.setKey.StudentUSI == information.StudentUSI &&
                                             data.setKey.SchoolId == information.SchoolId &&
                                             data.setKey.Value == metricFilterOption.MetricInstanceEqualTo));
                    }

                    if (metricFilterOption.ValueGreaterThanEqualTo.HasValue)
                    {
                        studentsMeetingCurrentOption =
                            studentsMeetingCurrentOption.And(information => StudentMetricRepository.GetAll()
                                .Any(data => data.StudentUSI == information.StudentUSI &&
                                             data.SchoolId == information.SchoolId &&
                                             data.MetricId == metricFilterOption.MetricId &&
                                             data.ValueSortOrder >= metricFilterOption.ValueGreaterThanEqualTo.Value));
                    }

                    if (metricFilterOption.ValueGreaterThan.HasValue)
                    {
                        studentsMeetingCurrentOption =
                            studentsMeetingCurrentOption.And(information => StudentMetricRepository.GetAll()
                                .Any(data => data.StudentUSI == information.StudentUSI &&
                                             data.SchoolId == information.SchoolId &&
                                             data.MetricId == metricFilterOption.MetricId &&
                                             data.ValueSortOrder > metricFilterOption.ValueGreaterThan.Value));
                    }

                    if (metricFilterOption.ValueLessThan.HasValue)
                    {
                        studentsMeetingCurrentOption =
                            studentsMeetingCurrentOption.And(information => StudentMetricRepository.GetAll()
                                .Any(data => data.StudentUSI == information.StudentUSI &&
                                             data.SchoolId == information.SchoolId &&
                                             data.MetricId == metricFilterOption.MetricId &&
                                             data.ValueSortOrder < metricFilterOption.ValueLessThan.Value));
                    }

                    if (metricFilterOption.ValueLessThanEqualTo.HasValue)
                    {
                        studentsMeetingCurrentOption =
                            studentsMeetingCurrentOption.And(information => StudentMetricRepository.GetAll()
                                .Any(data => data.StudentUSI == information.StudentUSI &&
                                             data.SchoolId == information.SchoolId &&
                                             data.MetricId == metricFilterOption.MetricId &&
                                             data.ValueSortOrder <= metricFilterOption.ValueLessThanEqualTo.Value));
                    }

                    studentsWithinMetrics = studentsWithinMetrics.Or(studentsMeetingCurrentOption);
                }

                query = query.Where(studentsWithinMetrics);
            }

            return query;
        }
    }
}