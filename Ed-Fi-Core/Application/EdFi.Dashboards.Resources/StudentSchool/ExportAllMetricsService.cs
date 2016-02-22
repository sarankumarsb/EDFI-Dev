// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Instrumentation;
using EdFi.Dashboards.Common;
using EdFi.Dashboards.Data.Entities;
using EdFi.Dashboards.Data.Repository;
using EdFi.Dashboards.Metric.Resources.Models;
using EdFi.Dashboards.Resources.Common;
using EdFi.Dashboards.Resources.Metric;
using EdFi.Dashboards.Resources.Metric.Requests;
using EdFi.Dashboards.Resources.Models.Common;
using EdFi.Dashboards.Resources.Security.Common;
using EdFi.Dashboards.SecurityTokenService.Authentication;

namespace EdFi.Dashboards.Resources.StudentSchool
{
    public class ExportAllMetricsRequest
    {
        public int SchoolId { get; set; }
        public long StudentUSI { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExportAllMetricsRequest"/> using the specified parameters.
        /// </summary>
        /// <returns>A new <see cref="ExportAllMetricsRequest"/> instance.</returns>
        public static ExportAllMetricsRequest Create(int schoolId, long studentUSI) 
        {
            return new ExportAllMetricsRequest { SchoolId = schoolId, StudentUSI = studentUSI };
        }
    }

    public interface IExportAllMetricsService : IService<ExportAllMetricsRequest, ExportAllModel> { }

    public class ExportAllMetricsService : IExportAllMetricsService
    {
        private readonly IRepository<StudentInformation> studentInformationRepository;
        private readonly IRootMetricNodeResolver rootMetricNodeResolver;
        private readonly IDomainMetricService<StudentSchoolMetricInstanceSetRequest> domainMetricService;
        private readonly IMetricTreeToIEnumerableOfKeyValuePairProvider metricTreeToIEnumerableOfKeyValuePairProvider;

        public ExportAllMetricsService(IRepository<StudentInformation> studentInformationRepository,
                                        IRootMetricNodeResolver rootMetricNodeResolver,
                                        IDomainMetricService<StudentSchoolMetricInstanceSetRequest> domainMetricService,
                                        IMetricTreeToIEnumerableOfKeyValuePairProvider metricTreeToIEnumerableOfKeyValuePairProvider)
        {
            this.studentInformationRepository = studentInformationRepository;
            this.rootMetricNodeResolver = rootMetricNodeResolver;
            this.domainMetricService = domainMetricService;
            this.metricTreeToIEnumerableOfKeyValuePairProvider = metricTreeToIEnumerableOfKeyValuePairProvider;
        }

        [CanBeAuthorizedBy(EdFiClaimTypes.ViewAllStudents, EdFiClaimTypes.ViewMyStudents)]
        public ExportAllModel Get(ExportAllMetricsRequest request)
        {
            var studentInformationData =
                studentInformationRepository.GetAll().SingleOrDefault(x => x.StudentUSI == request.StudentUSI);
            if (studentInformationData == null)
                throw new InstanceNotFoundException(string.Format("No student with Id:{0} was found.",
                                                                  request.StudentUSI));

            var row = new List<KeyValuePair<string, object>>
            {
                              new KeyValuePair<string, object>("Student Name",
                                                               Utilities.FormatPersonNameByLastName(
                                                                   studentInformationData.FirstName,
                                                                   studentInformationData.MiddleName,
                                                                   studentInformationData.LastSurname))
            };

            row.AddRange(GetFlattenedOutMetrics(request.SchoolId, request.StudentUSI));

            return new ExportAllModel
                            {
                                Rows = new List<ExportAllModel.Row>
                                           {
                                               new ExportAllModel.Row
                                                   {
                                                       Cells = row
                                                   }
                                           }
                            };

        }

        private IEnumerable<KeyValuePair<string, object>> GetFlattenedOutMetrics(int schoolId, long studentUSI)
        {
            var overviewNode = rootMetricNodeResolver.GetRootMetricNode();
            var overviewMetricTree = domainMetricService.Get(StudentSchoolMetricInstanceSetRequest.Create(schoolId, studentUSI, overviewNode.MetricVariantId));
            //Only Container metrics have the DecendantsOrSelf Enumerable property so we need this to be able to flatten it out.
            var overviewMetricTreeAsContainer = overviewMetricTree.RootNode as ContainerMetric;
            if (overviewMetricTreeAsContainer == null)
                throw new InvalidOperationException(
                    "Overview metric has to be casted to container to be able to export all metrics for given domain entity.");

            var flatMetrics =
                metricTreeToIEnumerableOfKeyValuePairProvider.FlattenMetricTree(overviewMetricTreeAsContainer);
            return flatMetrics;
        }
    }
}
