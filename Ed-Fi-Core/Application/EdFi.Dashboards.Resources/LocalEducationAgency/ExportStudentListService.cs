using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EdFi.Dashboards.Application.Data.Entities;
using EdFi.Dashboards.Common;
using EdFi.Dashboards.Data.Entities;
using EdFi.Dashboards.Data.Repository;
using EdFi.Dashboards.Metric.Data.Entities;
using EdFi.Dashboards.Metric.Resources.Models;
using EdFi.Dashboards.Resources.Metric;
using EdFi.Dashboards.Resources.Models.Common;
using EdFi.Dashboards.Resources.Security.Common;
using EdFi.Dashboards.Resources.StudentSchool;
using EdFi.Dashboards.SecurityTokenService.Authentication;
using MetricType = EdFi.Dashboards.Metric.Resources.Models.MetricType;

namespace EdFi.Dashboards.Resources.LocalEducationAgency
{
    public class ExportStudentListRequest
    {
        public int LocalEducationAgencyId { get; set; }
        public long StaffUSI { get; set; }
        public long SectionOrCohortId { get; set; }
        public string StudentListType { get; set; }

        public static ExportStudentListRequest Create(int localEducationAgencyId, long staffUSI, long sectionOrCohortId, string studentListType)
        {
            return new ExportStudentListRequest { LocalEducationAgencyId = localEducationAgencyId, StaffUSI = staffUSI, SectionOrCohortId = sectionOrCohortId, StudentListType = studentListType };
        }
    }

    public interface IExportCustomStudentListService : IService<ExportStudentListRequest, StudentExportAllModel> { }

    public class ExportStudentListService : IExportCustomStudentListService
    {
        private readonly IRepository<StaffCohort> staffCohortRepository;
        private readonly IRepository<StaffStudentCohort> staffStudentCohortRepository;
        private readonly IRepository<StaffCustomStudentList> staffCustomStudentListRepository;
        private readonly IRepository<StaffCustomStudentListStudent> staffCustomStudentListStudentRepository;
        private readonly IRepository<StudentInformation> studentInformationRepository;
        private readonly IRepository<StudentSchoolInformation> studentSchoolInformationRepository;
        private readonly IRepository<StudentSchoolMetricInstanceSet> studentSchoolMetricInstanceSetRepository;
        private readonly IRepository<MetricInstance> metricInstanceRepository;
        private readonly IRootMetricNodeResolver rootMetricNodeResolver;
        private readonly IRepository<SchoolInformation> schoolInformationRepository;
        private readonly IGradeLevelUtilitiesProvider gradeLevelUtilitiesProvider;
        private static readonly ConcurrentDictionary<int, IEnumerable<MetricMetadataNode>> schoolRootMetricNode = new ConcurrentDictionary<int, IEnumerable<MetricMetadataNode>>();
        private static readonly object schoolRootMetricNodeLockObject = new object();

        public ExportStudentListService(IRepository<StaffCohort> staffCohortRepository,
                                                IRepository<StaffStudentCohort> staffStudentCohortRepository,
                                                IRepository<StaffCustomStudentList> staffCustomStudentListRepository,
                                                IRepository<StaffCustomStudentListStudent> staffCustomStudentListStudentRepository,
                                                IRepository<StudentInformation> studentInformationRepository,
                                                IRepository<StudentSchoolInformation> studentSchoolInformationRepository,
                                                IRepository<StudentSchoolMetricInstanceSet> studentSchoolMetricInstanceSetRepository,
                                                IRepository<MetricInstance> metricInstanceRepository,
                                                IRootMetricNodeResolver rootMetricNodeResolver,
                                                IRepository<SchoolInformation> schoolInformationRepository,
                                                IGradeLevelUtilitiesProvider gradeLevelUtilitiesProvider)
        {
            this.staffCohortRepository = staffCohortRepository;
            this.staffStudentCohortRepository = staffStudentCohortRepository;
            this.staffCustomStudentListRepository = staffCustomStudentListRepository;
            this.staffCustomStudentListStudentRepository = staffCustomStudentListStudentRepository;
            this.studentInformationRepository = studentInformationRepository;
            this.studentSchoolInformationRepository = studentSchoolInformationRepository;
            this.studentSchoolMetricInstanceSetRepository = studentSchoolMetricInstanceSetRepository;
            this.metricInstanceRepository = metricInstanceRepository;
            this.rootMetricNodeResolver = rootMetricNodeResolver;
            this.schoolInformationRepository = schoolInformationRepository;
            this.gradeLevelUtilitiesProvider = gradeLevelUtilitiesProvider;
        }

        [CanBeAuthorizedBy(EdFiClaimTypes.ViewAllStudents, EdFiClaimTypes.ViewMyStudents)]
        public StudentExportAllModel Get(ExportStudentListRequest request)
        {
            var returnModel = new StudentExportAllModel();
            var sectionOrCohortId = request.SectionOrCohortId;

            //get the student list type
            var slt = GetSection(request.StaffUSI, request.LocalEducationAgencyId, request.StudentListType, ref sectionOrCohortId);

            //get list of student USIs
            var studentIdList = GetStudentIds(request.StaffUSI, request.LocalEducationAgencyId, slt, sectionOrCohortId);

            var studentIds = from s in studentIdList select new { StudentUSI = s };

            //get all the students from the staff members custom lists
            var students = (from id in studentIds
                            join si in studentInformationRepository.GetAll() on id.StudentUSI equals si.StudentUSI
                            join ssi in studentSchoolInformationRepository.GetAll() on si.StudentUSI equals ssi.StudentUSI
                            join sir in schoolInformationRepository.GetAll() on ssi.SchoolId equals sir.SchoolId
                            select new { si, ssi });

            //Let's get the data that we need.
            var data = (from student in students
                        join se in studentSchoolMetricInstanceSetRepository.GetAll() on new { student.ssi.SchoolId, student.si.StudentUSI } equals new { se.SchoolId, se.StudentUSI }
                        join mi in metricInstanceRepository.GetAll() on se.MetricInstanceSetKey equals mi.MetricInstanceSetKey
                        join sir in schoolInformationRepository.GetAll() on student.ssi.SchoolId equals sir.SchoolId
                        where sir.LocalEducationAgencyId == request.LocalEducationAgencyId && student.ssi.SchoolId == sir.SchoolId && se.SchoolId == sir.SchoolId
                        select new
                        {
                            student.si.StudentUSI,
                            student.si.FirstName,
                            student.si.MiddleName,
                            student.si.LastSurname,
                            sir.SchoolId,
                            SchoolName = sir.Name,
                            student.ssi.GradeLevel,
                            mi.MetricId,
                            mi.Value
                        }).ToList();

            var dataGroupedByStudent = (from s in data
                                        group s by new { s.StudentUSI, s.SchoolId }
                                            into g
                                            select new
                                            {
                                                g.First().StudentUSI,
                                                g.First().FirstName,
                                                g.First().MiddleName,
                                                g.First().LastSurname,
                                                g.First().SchoolId,
                                                g.First().SchoolName,
                                                g.First().GradeLevel,
                                                metric = g.ToList()
                                            }).OrderBy(x => x.LastSurname).ThenBy(x => x.FirstName);


            //Create the rows
            var listOfRows = new List<StudentExportAllModel.Row>();
            //For each student that we have we are going to traverse the granular metrics in tree order and add the metrics to the corresponding row slot.
            foreach (var student in dataGroupedByStudent)
            {
                var row = new List<KeyValuePair<string, object>>
                              {
                                  new KeyValuePair<string, object>("Student Name", Utilities.FormatPersonNameByLastName(student.FirstName, student.MiddleName, student.LastSurname)),
                                  new KeyValuePair<string, object>("School", student.SchoolName),
                                  new KeyValuePair<string, object>("Grade Level", gradeLevelUtilitiesProvider.FormatGradeLevelForDisplay(student.GradeLevel)),
                              };

                //Lets get the metric tree that applies to this.
                if (!schoolRootMetricNode.ContainsKey(student.SchoolId))
                {
                    lock (schoolRootMetricNodeLockObject)
                    {
                        var rootMetricNode = rootMetricNodeResolver.GetRootMetricNodeForStudent(student.SchoolId);

                        if (rootMetricNode == null)
                            throw new InvalidOperationException("No metric metadata available to be able to export a hierarchy.");

                        schoolRootMetricNode[student.SchoolId] = rootMetricNode.DescendantsOrSelf.Where(y => y.MetricType == MetricType.GranularMetric);
                    }
                }

                var metricsOfTypeGranular = schoolRootMetricNode[student.SchoolId];

                foreach (var granularMetricNode in metricsOfTypeGranular)
                {
                    var studentMetric = student.metric.SingleOrDefault(x => x.MetricId == granularMetricNode.MetricId);

                    row.Add(new KeyValuePair<string, object>(Utilities.Metrics.GetMetricName(granularMetricNode), (studentMetric == null) ? string.Empty : studentMetric.Value));
                }

                listOfRows.Add(new StudentExportAllModel.Row { Cells = row, StudentUSI = student.StudentUSI });
            }

            returnModel.Rows = listOfRows;

            return returnModel;
        }
        protected IList<long> GetStudentIds(long staffUSI, int localEducationAgencyId, StudentListType studentListType, long sectionOrCohortId)
        {
            //IList<int> studentIds;
            List<long> studentIds = new List<long>();
            List<long> cohortStudentIds = new List<long>();
            List<long> CustomListStudentIds = new List<long>();

            //get student USIs if it is a cohort
            if (studentListType == StudentListType.Cohort)
            {
                studentIds = (from ssc in staffStudentCohortRepository.GetAll()
                                    where ssc.StaffCohortId == sectionOrCohortId
                                    select ssc.StudentUSI).ToList();
            }

            //get student USIs if it is a custom student list
            if (studentListType == StudentListType.CustomStudentList)
            {
                //get list of students from app db
                studentIds = (from scsl in staffCustomStudentListRepository.GetAll()
                                join scsls in staffCustomStudentListStudentRepository.GetAll() on
                                           scsl.StaffCustomStudentListId equals scsls.StaffCustomStudentListId
                                where scsl.StaffUSI == staffUSI && 
                                        scsl.EducationOrganizationId == localEducationAgencyId &&
                                        scsl.StaffCustomStudentListId == sectionOrCohortId
                                select scsls.StudentUSI).ToList();
            }

            //get student USIs it is all which can include cohorts and custom student lists
            if (studentListType == StudentListType.All)
            {
                //get cohort student ids
                cohortStudentIds = (from sc in staffCohortRepository.GetAll()
                                    join ssc in staffStudentCohortRepository.GetAll() on sc.StaffCohortId equals ssc.StaffCohortId
                                    where sc.StaffUSI == staffUSI && sc.EducationOrganizationId == localEducationAgencyId
                                    select ssc.StudentUSI).ToList();
                
                //get custom student lists ids
                CustomListStudentIds = (from scsl in staffCustomStudentListRepository.GetAll()
                                       join scsls in staffCustomStudentListStudentRepository.GetAll() on
                                           scsl.StaffCustomStudentListId equals scsls.StaffCustomStudentListId
                                       where scsl.StaffUSI == staffUSI &&
                                                scsl.EducationOrganizationId == localEducationAgencyId
                                       select scsls.StudentUSI).ToList();

                //combine the two lists of student USIs
                studentIds = CustomListStudentIds.Concat(cohortStudentIds).Distinct().ToList();

                if (studentIds.Count == 0)
                    studentIds = null;
            }

            return studentIds;
        }

        protected StudentListType GetSection(long staffUSI, int localEducationAgencyId, string studentListType, ref long sectionOrCohortId)
        {
            if (String.IsNullOrEmpty(studentListType))
                studentListType = StudentListType.None.ToString();

            var slt = (StudentListType)Enum.Parse(typeof(StudentListType), studentListType, true);

            if (slt == StudentListType.None)
            {
                var firstCohort = (from data in staffCohortRepository.GetAll()
                                   where data.StaffUSI == staffUSI && data.EducationOrganizationId == localEducationAgencyId
                                   orderby data.CohortDescription, data.StaffCohortId
                                   select data).FirstOrDefault();
                if (firstCohort != null)
                {
                    sectionOrCohortId = firstCohort.StaffCohortId;
                    return StudentListType.Cohort;
                }

                var firstCustomStudentList = (from data in staffCustomStudentListRepository.GetAll()
                                              where data.StaffUSI == staffUSI && data.EducationOrganizationId == localEducationAgencyId
                                              orderby data.CustomStudentListIdentifier, data.StaffCustomStudentListId
                                              select data).FirstOrDefault();
                if (firstCustomStudentList != null)
                {
                    sectionOrCohortId = firstCustomStudentList.StaffCustomStudentListId;
                    return StudentListType.CustomStudentList;
                }
            }

            return slt;
        }
    }
}
