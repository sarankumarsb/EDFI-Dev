// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System.Collections.Generic;
using System.Linq;
using EdFi.Dashboards.Data.Entities;
using EdFi.Dashboards.Data.Repository;
using EdFi.Dashboards.Metric.Data.Entities;
using EdFi.Dashboards.Resources.Models.CustomGrid;
using EdFi.Dashboards.Resources.Models.Student;
using EdFi.Dashboards.Warehouse.Data.Entities;
using EdFi.Dashboards.Warehouse.Resources.Metric.AdditionalPriorYearMetricProviders;

namespace EdFi.Dashboards.Warehouse.Resources.Staff
{
    public interface IPriorYearClassroomMetricsProvider
    {
        List<StudentWithMetrics.Metric> GetAdditionalMetrics(IEnumerable<StudentSchoolMetricInstance> priorYearStudentList, IEnumerable<StudentMetric> studentList, List<MetadataColumnGroup> listMetadata);
    }

    public class PriorYearClassroomMetricsProvider : IPriorYearClassroomMetricsProvider
    {
        private readonly IAdditionalPriorYearMetricProvider _additionalPriorYearMetricProvider;
        private readonly IRepository<MetricVariant> _metricVariantRepository;

        public PriorYearClassroomMetricsProvider(IAdditionalPriorYearMetricProvider additionalPriorYearMetricProvider,
                                                IRepository<MetricVariant> metricVariantRepository)
        {
            _additionalPriorYearMetricProvider = additionalPriorYearMetricProvider;
            _metricVariantRepository = metricVariantRepository;
        }

        public List<StudentWithMetrics.Metric> GetAdditionalMetrics(IEnumerable<StudentSchoolMetricInstance> priorYearStudentList, IEnumerable<StudentMetric> studentList, List<MetadataColumnGroup> listMetadata)
        {
            return CreateAdditionalMetrics(priorYearStudentList, studentList, listMetadata);
        }

        protected virtual List<StudentWithMetrics.Metric> CreateAdditionalMetrics(IEnumerable<StudentSchoolMetricInstance> priorYearStudentMetrics, IEnumerable<StudentMetric> studentMetrics, List<MetadataColumnGroup> listMetadata)
        {
            var result = new List<StudentWithMetrics.Metric>();

            foreach (var metadataColumn in listMetadata.Where(@group => @group.GroupType != GroupType.EntityInformation).SelectMany(@group => @group.Columns))
            {
                var column = metadataColumn;

                var metricId = (from metricVariant in _metricVariantRepository.GetAll()
                                where metricVariant.MetricVariantId == column.MetricVariantId
                                select metricVariant.MetricId).SingleOrDefault();

                var priorYearMetric = (from studentMetric in priorYearStudentMetrics
                                       where studentMetric != null && studentMetric.MetricId == metricId
                                       select studentMetric).FirstOrDefault();

                var metric = (from studentMetric in studentMetrics
                              where studentMetric != null && studentMetric.MetricVariantId == column.MetricVariantId
                              select studentMetric).FirstOrDefault();

                if (metric != null || priorYearMetric != null)
                {
                    var request = AdditionalPriorYearMetricRequest.Create(column, priorYearMetric, metric);
                    var metricToAdd = _additionalPriorYearMetricProvider.GetAdditionalPriorYearMetric(request);
                    if (metricToAdd != null)
                        result.Add(metricToAdd);
                }
            }

            return result;
        }
    }
}