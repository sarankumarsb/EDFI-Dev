using System.Collections.Generic;
using System.Linq;
using EdFi.Dashboards.Application.Data.Entities;
using EdFi.Dashboards.Common;
using EdFi.Dashboards.Data.Repository;
using EdFi.Dashboards.Infrastructure.Implementations.Caching;
using EdFi.Dashboards.Resources;
using EdFi.Dashboards.Resources.Security.Common;
using EdFi.Dashboards.SecurityTokenService.Authentication;

namespace EdFi.Dashboards.Application.Resources.LocalEducationAgency
{

    /// <summary>
    ///    Get Models
    /// </summary>

    public class MetricThresholdGetRequest
    {
        public int LocalEducationAgencyId { get; set; }
        [AuthenticationIgnore("At the time of coding there is no need to validate against the metric id")]
        public int? MetricId { get; set; }

        public static MetricThresholdGetRequest Create(int localEducationAgency, int? metricId)
        {
            return new MetricThresholdGetRequest
                      {
                          LocalEducationAgencyId = localEducationAgency,
                          MetricId = metricId
                      };
        }
    }

    public class MetricThresholdGetResponse
    {
        public int MetricId { get; set; }
        public decimal? Threshold { get; set; }
        public bool IsInclusive { get; set; }
    }

    /// <summary>
    /// Put models
    /// </summary>

    public class MetricThresholdPutRequest
    {
        public int LocalEducationAgencyId { get; set; }
        [AuthenticationIgnore("At the time of coding there is no need to validate against the metric id")]
        public int MetricId { get; set; }
        [AuthenticationIgnore("At the time of coding there is no need to validate against the threshold value")]
        public decimal Threshold { get; set; }
        [AuthenticationIgnore("At the time of coding there is no need to validate against the is inclusive flag")]
        public bool IsInclusive { get; set; }

        public static MetricThresholdPutRequest Create(int localEducationAgencyId, int metricId, decimal threshold, bool isInclusive)
        {
            return new MetricThresholdPutRequest
                       {
                           LocalEducationAgencyId = localEducationAgencyId,
                           MetricId = metricId,
                           Threshold = threshold,
                           IsInclusive = isInclusive
                       };
        }
    }

    public class MetricThresholdPutResponse
    {
        //public int LocalEducationAgencyId { get; set; } //should this be part of the response?
        public int MetricId { get; set; }
        public decimal Threshold { get; set; }
        public bool IsInclusive { get; set; }
    }

    /// <summary>
    /// Delete models
    /// </summary>
    /// 
    public class MetricThresholdDeleteRequest
    {
        public int LocalEducationAgencyId { get; set; }
        [AuthenticationIgnore("At the time of coding there is no need to validate against the metric id")]
        public int MetricId { get; set; }

        public static MetricThresholdDeleteRequest Create(int localEducationAgencyId, int metricId)
        {
            return new MetricThresholdDeleteRequest
                       {
                           LocalEducationAgencyId = localEducationAgencyId,
                           MetricId = metricId
                       };
        }
    }

    public interface IMetricThresholdService :
        IService<MetricThresholdGetRequest, IEnumerable<MetricThresholdGetResponse>>, //IGetHandler
        IPutHandler<MetricThresholdPutRequest, MetricThresholdPutResponse>,
        IDeleteHandler<MetricThresholdDeleteRequest>
    {
    }

    public class MetricThresholdService : IMetricThresholdService
    {
        private readonly IPersistingRepository<LocalEducationAgencyMetricThreshold> localEducationAgencyMetricThresholdRepositoy;

        public MetricThresholdService(IPersistingRepository<LocalEducationAgencyMetricThreshold> localEducationAgencyMetricThresholdRepository)
        {
            this.localEducationAgencyMetricThresholdRepositoy = localEducationAgencyMetricThresholdRepository;
        }

        [NoCache]
        [CanBeAuthorizedBy(EdFiClaimTypes.AdministerDashboard)]
        public IEnumerable<MetricThresholdGetResponse> Get(MetricThresholdGetRequest request)
        {
            //check the request metricId to see if it has a value, if it does we should return only a collection with one record
            //if not then we should return all records from the LEA
            if (request.MetricId.HasValue)
            {
                return from x in localEducationAgencyMetricThresholdRepositoy.GetAll()
                       where x.LocalEducationAgencyId == request.LocalEducationAgencyId && x.MetricId == request.MetricId
                       select new MetricThresholdGetResponse
                                  {
                                      MetricId = x.MetricId,
                                      Threshold = x.Threshold,
                                      IsInclusive = x.IsInclusive
                                  };

            }

            return from x in localEducationAgencyMetricThresholdRepositoy.GetAll()
                   where x.LocalEducationAgencyId == request.LocalEducationAgencyId
                   select new MetricThresholdGetResponse
                              {
                                  MetricId = x.MetricId,
                                  Threshold = x.Threshold,
                                  IsInclusive = x.IsInclusive
                              };
        }

        [NoCache]
        [CanBeAuthorizedBy(EdFiClaimTypes.AdministerDashboard)]
        public MetricThresholdPutResponse Put(MetricThresholdPutRequest request, out bool created)
        {
            //because of natural key definition, put is used as add
            //does the record exists? 
            var metricThreshold =
                    localEducationAgencyMetricThresholdRepositoy.GetAll()
                    .SingleOrDefault(x => x.LocalEducationAgencyId == request.LocalEducationAgencyId && x.MetricId == request.MetricId);

            //if not create a new record
            if (metricThreshold == null)
            {
                metricThreshold = new LocalEducationAgencyMetricThreshold
                                      {
                                          LocalEducationAgencyId = request.LocalEducationAgencyId,
                                          MetricId = request.MetricId,
                                          Threshold = request.Threshold,
                                          IsInclusive = request.IsInclusive
                                      };
            }
            else
            {
                //or update the existing record
                metricThreshold.Threshold = request.Threshold;
                metricThreshold.IsInclusive = request.IsInclusive;
            }
            
            //save to db
            localEducationAgencyMetricThresholdRepositoy.Save(metricThreshold, out created);
            
            //return response
            return new MetricThresholdPutResponse
                       {
                           MetricId = metricThreshold.MetricId,
                           Threshold = metricThreshold.Threshold,
                           IsInclusive = metricThreshold.IsInclusive
                       };
        }

        [NoCache]
        [CanBeAuthorizedBy(EdFiClaimTypes.AdministerDashboard)]
        public void Delete(MetricThresholdDeleteRequest request)
        {
            localEducationAgencyMetricThresholdRepositoy.Delete(x => x.LocalEducationAgencyId == request.LocalEducationAgencyId && x.MetricId == request.MetricId);
        }
    }
}
