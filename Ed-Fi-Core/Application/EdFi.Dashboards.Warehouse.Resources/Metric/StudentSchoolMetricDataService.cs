using System;
using System.Globalization;
using System.Linq;
using EdFi.Dashboards.Common;
using EdFi.Dashboards.Data.Repository;
using EdFi.Dashboards.Infrastructure.Implementations.Caching;
using EdFi.Dashboards.Metric.Resources.Models;
using EdFi.Dashboards.Resources;
using EdFi.Dashboards.Resources.Security.Common;
using EdFi.Dashboards.Warehouse.Data.Entities;
using EdFi.Dashboards.Metric.Data.Entities;
using EdFi.Dashboards.Warehouse.Resource.Models.Metric;

namespace EdFi.Dashboards.Warehouse.Resources.Metric
{
    public class StudentSchoolMetricDataRequest
    {
        public long StudentUSI { get; set; }
        public int SchoolId { get; set; }

        [AuthenticationIgnore("LocalEducationAgencyId is implied by School, and does not need to be independently authorized.")]
        public int LocalEducationAgencyId { get; set; }

        [AuthenticationIgnore("MetricInstanceSetKey is implied by School, and does not need to be independently authorized.")]
        public Guid MetricInstanceSetKey { get; set; }

        [AuthenticationIgnore("Does not affect security")]
        public int SchoolYear { get; set; }

        public static StudentSchoolMetricDataRequest Create(long studentUSI, int schoolId, int localEducationAgencyId, Guid metricInstanceSetKey, int schoolYear)
        {
            return new StudentSchoolMetricDataRequest { StudentUSI = studentUSI, SchoolId = schoolId, LocalEducationAgencyId = localEducationAgencyId, MetricInstanceSetKey = metricInstanceSetKey, SchoolYear = schoolYear };
        }
    }

    public interface IStudentSchoolMetricDataService : IService<StudentSchoolMetricDataRequest, MetricData> { }

    public class StudentSchoolMetricDataService : IStudentSchoolMetricDataService
    {
        private const string schoolYearExtendedPropertyName = "SchoolYear";
        private const string goalExtendedPropertyName = "Goal";
        private const string schoolYearExtendedPropertyValueTypeName = "System.Int32";

        private readonly IRepository<StudentSchoolMetricInstance> metricInstanceRepository;
        private readonly IRepository<StudentSchoolMetricInstanceExtendedProperty> metricInstanceExtendedPropertyRepository;
        private readonly IRepository<StudentSchoolMetricComponent> metricComponentRepository;

        public StudentSchoolMetricDataService(IRepository<StudentSchoolMetricInstance> metricInstanceRepository,
                                              IRepository<StudentSchoolMetricInstanceExtendedProperty> metricInstanceExtendedPropertyRepository,
                                              IRepository<StudentSchoolMetricComponent> metricComponentRepository)
        {
            this.metricInstanceRepository = metricInstanceRepository;
            this.metricInstanceExtendedPropertyRepository = metricInstanceExtendedPropertyRepository;
            this.metricComponentRepository = metricComponentRepository;
        }

        [CacheBehavior(copyOnSet: false, copyOnGet: false)]
        public MetricData Get(StudentSchoolMetricDataRequest request)
        {
            var studentUSI = request.StudentUSI;
            var localEducationAgencyId = request.LocalEducationAgencyId;
            var metricInstanceSetKey = request.MetricInstanceSetKey;
            var year = request.SchoolYear;

            var result = new PriorYearMetricData();

            var metricInstances = (from m in metricInstanceRepository.GetAll()
                                   where m.StudentUSI == studentUSI
                                         && m.LocalEducationAgencyId == localEducationAgencyId
                                         && m.SchoolYear == year
                                   select m).ToList();

            //need to find if there are duplicate metric ids, that would indicate dual enrollment in the the year.
            //so check to see if there is more than one school id in the metricInstances
            var distinctSchoolIds = (from mi in metricInstances
                                     select mi.SchoolId).Distinct().ToList();

            var schoolIdToFilterDataBy = distinctSchoolIds.Count != 0 ? distinctSchoolIds[0] : request.SchoolId;
            if (distinctSchoolIds.Count > 1)
            {
                //limit to only one school
                //is the current school in the list of school ids? 
                if (distinctSchoolIds.Exists(x => x == request.SchoolId))
                {
                    //yes, so filter by that school id
                    schoolIdToFilterDataBy = request.SchoolId;

                }
                else
                {
                    //no, so filter by the smallest schoolid
                    distinctSchoolIds.Sort();
                    schoolIdToFilterDataBy = distinctSchoolIds[0];
                }
                metricInstances = (from mi in metricInstances
                                   where mi.SchoolId == schoolIdToFilterDataBy
                                   select mi).ToList();
            }


            result.MetricInstances = metricInstances.Select(x => new MetricInstance
                                                                     {
                                                                         Context = x.Context,
                                                                         Flag = x.Flag,
                                                                         MetricId = x.MetricId,
                                                                         MetricInstanceSetKey = metricInstanceSetKey,
                                                                         MetricStateTypeId = x.MetricStateTypeId,
                                                                         TrendDirection = x.TrendDirection,
                                                                         Value = x.Value,
                                                                         ValueTypeName = x.ValueTypeName
                                                                     }).ToList();

            var metricInstanceExtendedProperties = (from me in metricInstanceExtendedPropertyRepository.GetAll()
                                                    where me.StudentUSI == studentUSI
                                                          && me.LocalEducationAgencyId == localEducationAgencyId
                                                          && me.SchoolYear == year
                                                          && me.SchoolId == schoolIdToFilterDataBy
                                                    select me).ToList();

            var metricInstanceExtendedPropertyList = metricInstanceExtendedProperties.Select(x => new MetricInstanceExtendedProperty
                                                                                                        {
                                                                                                            MetricId = x.MetricId,
                                                                                                            MetricInstanceSetKey = metricInstanceSetKey,
                                                                                                            Name = x.Name,
                                                                                                            Value = x.Value,
                                                                                                            ValueTypeName = x.ValueTypeName
                                                                                                        }).ToList();

            metricInstanceExtendedPropertyList.AddRange(metricInstances.Select(x => new MetricInstanceExtendedProperty
                                                                                        {
                                                                                            MetricId = x.MetricId,
                                                                                            MetricInstanceSetKey = metricInstanceSetKey,
                                                                                            Name = schoolYearExtendedPropertyName,
                                                                                            Value = x.SchoolYear.ToString(CultureInfo.InvariantCulture),
                                                                                            ValueTypeName = schoolYearExtendedPropertyValueTypeName
                                                                                        }));

            result.MetricInstanceExtendedProperties = metricInstanceExtendedPropertyList;

            result.MetricGoals = metricInstanceExtendedProperties.Where(x => x.Name == goalExtendedPropertyName).Select(x => new MetricGoal
                                                                                                                {
                                                                                                                    MetricId = x.MetricId,
                                                                                                                    MetricInstanceSetKey = metricInstanceSetKey,
                                                                                                                    Value = Convert.ToDecimal(x.Value)
                                                                                                                });

            result.MetricComponents = (from mc in metricComponentRepository.GetAll()
                                       where mc.StudentUSI == studentUSI
                                             && mc.LocalEducationAgencyId == localEducationAgencyId
                                             && mc.SchoolYear == year
                                             && mc.SchoolId == schoolIdToFilterDataBy
                                       select new Dashboards.Metric.Data.Entities.MetricComponent
                                       {
                                           MetricInstanceSetKey = metricInstanceSetKey,
                                           MetricId = mc.MetricId,
                                           MetricStateTypeId = mc.MetricStateTypeId,
                                           Name = mc.Name,
                                           Value = mc.Value,
                                           ValueTypeName = mc.ValueTypeName,
                                           Format = mc.Format,
                                           TrendDirection = mc.TrendDirection
                                       });
            return result;
        }
    }
}
