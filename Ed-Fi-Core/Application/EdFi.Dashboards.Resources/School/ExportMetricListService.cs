// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using EdFi.Dashboards.Common;
using EdFi.Dashboards.Data.Entities;
using EdFi.Dashboards.Data.Repository;
using EdFi.Dashboards.Metric.Data.Entities;
using EdFi.Dashboards.Metric.Resources.Providers;
using EdFi.Dashboards.Resources.Metric;
using EdFi.Dashboards.Resources.Models.Common;
using EdFi.Dashboards.Resources.Security.Common;
using EdFi.Dashboards.SecurityTokenService.Authentication;
using MetricType = EdFi.Dashboards.Metric.Resources.Models.MetricType;

namespace EdFi.Dashboards.Resources.School
{
    public class ExportMetricListRequest
    {
        public int SchoolId { get; set; }
        public int MetricVariantId { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExportMetricListRequest"/> using the specified parameters.
        /// </summary>
        /// <returns>A new <see cref="ExportMetricListRequest"/> instance.</returns>
        public static ExportMetricListRequest Create(int schoolId, int metricVariantId) 
        {
            return new ExportMetricListRequest { SchoolId = schoolId, MetricVariantId = metricVariantId };
        }
    }

    public interface IExportMetricListService : IService<ExportMetricListRequest, StudentExportAllModel> { }

    public class ExportMetricListService : IExportMetricListService
    {
        private readonly IRootMetricNodeResolver rootMetricNodeResolver;
        private readonly IRepository<StudentInformation> studentInformationRepository;
        private readonly IRepository<SchoolMetricStudentList> schoolMetricStudentListRepository;
        private readonly IRepository<StudentSchoolMetricInstanceSet> studentSchoolMetricInstanceSetRepository;
        private readonly IRepository<MetricInstance> metricInstanceRepository;
        private readonly IMetricNodeResolver metricNodeResolver;

        public ExportMetricListService(IRootMetricNodeResolver rootMetricNodeResolver,
                                        IRepository<StudentInformation> studentInformationRepository,
                                        IRepository<SchoolMetricStudentList> schoolMetricStudentListRepository,
                                        IRepository<StudentSchoolMetricInstanceSet> studentSchoolMetricInstanceSetRepository,
                                        IRepository<MetricInstance> metricInstanceRepository,
                                        IMetricNodeResolver metricNodeResolver)
        {
            this.rootMetricNodeResolver = rootMetricNodeResolver;
            this.studentInformationRepository = studentInformationRepository;
            this.schoolMetricStudentListRepository = schoolMetricStudentListRepository;
            this.studentSchoolMetricInstanceSetRepository = studentSchoolMetricInstanceSetRepository;
            this.metricInstanceRepository = metricInstanceRepository;
            this.metricNodeResolver = metricNodeResolver;
        }

        [CanBeAuthorizedBy(EdFiClaimTypes.ViewAllStudents, EdFiClaimTypes.ViewMyStudents)]
        public StudentExportAllModel Get(ExportMetricListRequest request)
        {
            var metricId = metricNodeResolver.ResolveMetricId(request.MetricVariantId);
            //Lets get the metric tree that applies to this.
            var rootMetricNode = rootMetricNodeResolver.GetRootMetricNodeForStudent(request.SchoolId);

            if (rootMetricNode == null)
                throw new InvalidOperationException("No metric metadata available to be able to export a hierarchy.");

            var metricsOfTypeGranular = rootMetricNode.DescendantsOrSelf.Where(y => y.MetricType == MetricType.GranularMetric);

            //Lets get the data that we need.
            var data = (from s in schoolMetricStudentListRepository.GetAll()
                        join si in studentInformationRepository.GetAll() on s.StudentUSI equals si.StudentUSI
                        join se in studentSchoolMetricInstanceSetRepository.GetAll() on new {s.SchoolId, s.StudentUSI} equals new {se.SchoolId, se.StudentUSI}
                        join mi in metricInstanceRepository.GetAll() on se.MetricInstanceSetKey equals mi.MetricInstanceSetKey
                        where s.SchoolId == request.SchoolId
                              && s.MetricId == metricId
                              && se.SchoolId == request.SchoolId
                        select new
                                   {
                                       si.StudentUSI,
                                       si.FirstName,
                                       si.MiddleName,
                                       si.LastSurname,
                                       mi.MetricId,
                                       mi.Value
                                   }).ToList();

            var dataGroupedByStudent = (from s in data
                                       group s by s.StudentUSI
                                       into g
                                       select new
                                                  {
                                                      g.First().StudentUSI,
                                                      g.First().FirstName,
                                                      g.First().MiddleName,
                                                      g.First().LastSurname,
                                                      metric = g
                                                  }).OrderBy(x=>x.LastSurname).ThenBy(x=>x.FirstName);

            var listOfRows = new List<StudentExportAllModel.Row>();
            //For each student that we have we are going to traverse the granular metrics in tree order and add the metrics to the corresponding row slot.
            foreach (var student in dataGroupedByStudent)
            {
                var row = new List<KeyValuePair<string, object>>
                              {
                                  new KeyValuePair<string, object>("Student Name", Utilities.FormatPersonNameByLastName(student.FirstName, student.MiddleName, student.LastSurname))
                              };

                foreach (var granularMetricNode in metricsOfTypeGranular)
                {
                    var studentMetric = student.metric.SingleOrDefault(x => x.MetricId == granularMetricNode.MetricId);

                    row.Add(new KeyValuePair<string, object>(Utilities.Metrics.GetMetricName(granularMetricNode), (studentMetric == null) ? string.Empty : studentMetric.Value));
                }

                listOfRows.Add(new StudentExportAllModel.Row { Cells = row, StudentUSI = student.StudentUSI });
            }

            return new StudentExportAllModel { Rows = listOfRows };
        }
    }
}
