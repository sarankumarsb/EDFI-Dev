using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EdFi.Dashboards.Application.Data.Entities;
using EdFi.Dashboards.Common;
using EdFi.Dashboards.Core;
using EdFi.Dashboards.Data.Entities;
using EdFi.Dashboards.Data.Repository;
using EdFi.Dashboards.Metric.Resources.Models;
using EdFi.Dashboards.Resource.Models.Common;
using EdFi.Dashboards.Resources.Models.Common;
using EdFi.Dashboards.Resources.Models.CustomGrid;
using EdFi.Dashboards.Resources.Models.Staff;
using EdFi.Dashboards.Resources.Models.Student;
using EdFi.Dashboards.Resources.Navigation;
using EdFi.Dashboards.Resources.Navigation.Support;
using EdFi.Dashboards.Resources.School;
using EdFi.Dashboards.Resources.Security.Common;
using EdFi.Dashboards.Resources.StudentSchool;
using EdFi.Dashboards.SecurityTokenService.Authentication;

namespace EdFi.Dashboards.Resources.Staff
{
    public class AssessmentDetailsRequest
    {
        public int SchoolId { get; set; }
        public long StaffUSI { get; set; }
        public string StudentListType { get; set; }
        public long SectionOrCohortId { get; set; }

        [AuthenticationIgnore("MetricAssessmentArea does not affect the results of the request in a way requiring it to be secured.")]
        public string MetricAssessmentArea { get; set; }

        [AuthenticationIgnore("AssessmentSubType does not affect the results of the request in a way requiring it to be secured.")]
        public string AssessmentSubType { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AssessmentDetailsRequest"/> using the specified parameters.
        /// </summary>
        /// <returns>A new <see cref="AssessmentDetailsRequest"/> instance.</returns>
        public static AssessmentDetailsRequest Create(int schoolId, long staffUSI, string studentListType, long sectionOrCohortId, string metricAssessmentArea, string assessmentSubType)
        {
            return new AssessmentDetailsRequest { SchoolId = schoolId, StaffUSI = staffUSI, StudentListType = studentListType, SectionOrCohortId = sectionOrCohortId, MetricAssessmentArea = metricAssessmentArea, AssessmentSubType = assessmentSubType };
        }
    }

    public interface IAssessmentDetailsService : IService<AssessmentDetailsRequest, AssessmentDetailsModel> { }

    public abstract class AssessmentDetailsServiceBase<TRequest, TResponse, TStudentAssessment> : StaffServiceBase, IService<TRequest, TResponse>
        where TRequest : AssessmentDetailsRequest
        where TResponse : AssessmentDetailsModel, new()
        where TStudentAssessment : StudentWithMetricsAndAssessments, new()
    {
        protected const string socialStudies = "Social Studies";
        protected const string science = "Science";
        protected const string mathematics = "Mathematics";
        protected const string elaReading = "ELA / Reading";
        protected const string writing = "Writing";

        public IRepository<StudentMetricObjective> StudentMetricObjectiveRepository { get; set; }
        public IAssessmentDetailsProvider AssessmentDetailsProvider { get; set; }
        public IAssessmentBenchmarkDetailsProvider AssessmentBenchmarkDetailsProvider { get; set; }
        public IAssessmentReadingDetailsProvider AssessmentReadingDetailsProvider { get; set; }
        public IGradeLevelUtilitiesProvider GradeLevelUtilitiesProvider { get; set; }
        public IClassroomMetricsProvider ClassroomMetricsProvider { get; set; }
        public IMetadataListIdResolver MetadataListIdResolver { get; set; }
        public IListMetadataProvider ListMetadataProvider { get; set; }
        public ISchoolCategoryProvider SchoolCategoryProvider { get; set; }

        [CanBeAuthorizedBy(EdFiClaimTypes.ViewAllStudents, EdFiClaimTypes.ViewMyStudents)]
        public TResponse Get(TRequest request)
        {
            long staffUSI = request.StaffUSI;
            int schoolId = request.SchoolId;
            string studentListType = request.StudentListType;
            long sectionOrCohortId = request.SectionOrCohortId;

            var model = new TResponse();

            var slt = GetStudentListIdentity(staffUSI, schoolId, studentListType, sectionOrCohortId);

            if (slt.StudentListType != StudentListType.All && sectionOrCohortId == 0)
                return model;

            var studentIds = new List<long>();
            if (slt.StudentListType == StudentListType.CustomStudentList)
                studentIds = StaffCustomStudentListStudentRepository.GetAll().Where(x => x.StaffCustomStudentListId == sectionOrCohortId).Select(x => x.StudentUSI).ToList();

            var schoolCategory = SchoolCategoryProvider.GetSchoolCategoryType(request.SchoolId);

            switch (schoolCategory)
            {
                case SchoolCategory.Elementary:
                case SchoolCategory.MiddleSchool:
                    break;
                default:
                    schoolCategory = SchoolCategory.HighSchool;
                    break;
            }

            //Setting the metadata.
            var resolvedListId = MetadataListIdResolver.GetListId(ListType.ClassroomGeneralOverview, schoolCategory);
            var listMetadata = ListMetadataProvider.GetListMetadata(resolvedListId);

            var assessmentSubType = GetAssessmentSubType(request.AssessmentSubType);
            var metricAssessmentArea = GetMetricAssessmentArea(request.MetricAssessmentArea, staffUSI, schoolId, slt.StudentListType, sectionOrCohortId, assessmentSubType);
            model.MetricTitle = GetMetricTitle(assessmentSubType, metricAssessmentArea);
            model.FixedRowTitle = AssessmentDetailsProvider.GetStudentsFixedRowTitle(assessmentSubType, metricAssessmentArea);

            var students = GetStudentListEntities(schoolId, staffUSI, slt, new List<MetadataColumnGroup>(), studentIds, null, string.Empty); // TODO: there's a lot of null values here, not sure if this is the proper way to call
            var studentListWithMetrics = GetStudentListWithMetrics(staffUSI, schoolId, students.Select(student => student.StudentUSI)).ToList();

            if (!studentListWithMetrics.Any())
                return model;

            var uniqueListId = UniqueListProvider.GetUniqueId();
            model.UniqueListId = uniqueListId;

            // can't be a LINQ statement b/c StudentMetric does not have a default constructor
            foreach (var student in students)
            {
                long studentUsi = student.StudentUSI;
                var metrics = studentListWithMetrics.Where(withMetrics => withMetrics.StudentUSI == student.StudentUSI).ToList();
                string gender = student.Gender;
                string fullName = student.FullName;

                //will need to create a Student Assessment extension 
                //Student Unique Field Newly Added : Saravanan
                var studentMetric = new TStudentAssessment
                {
                    StudentUSI = student.StudentUSI,
                    SchoolId = student.SchoolId,
                    SchoolName = student.SchoolName,
                    Name = Utilities.FormatPersonNameByLastName(student.FirstName, student.MiddleName, student.LastSurname),
                    ThumbNail = StudentSchoolAreaLinks.ListImage(schoolId, studentUsi, gender).Resolve(),
                    Href = new Link { Rel = StudentLink, Href = StudentSchoolAreaLinks.Overview(schoolId, studentUsi, fullName).AppendParameters("listContext=" + uniqueListId).Resolve() },
                    GradeLevel = GradeLevelUtilitiesProvider.FormatGradeLevelForSorting(student.GradeLevel),
                    GradeLevelDisplayValue = GradeLevelUtilitiesProvider.FormatGradeLevelForDisplay(student.GradeLevel),
                    Metrics = ClassroomMetricsProvider.GetAdditionalMetrics(metrics, listMetadata),
                    StudentUniqueID = student.StudentUniqueID
                };

                switch (assessmentSubType)
                {
                    case StaffModel.AssessmentSubType.StateStandardized:
                        studentMetric.Score = AssessmentDetailsProvider.OnStudentAssessmentInitialized(studentMetric, metrics, metricAssessmentArea);
                        break;

                    case StaffModel.AssessmentSubType.Benchmark:
                        studentMetric.Score = AssessmentBenchmarkDetailsProvider.OnStudentAssessmentInitialized(studentMetric, metrics, metricAssessmentArea);
                        break;
                }

                model.Students.Add(studentMetric);
            }

            GetMetricObjectives(model, assessmentSubType, metricAssessmentArea, staffUSI, schoolId, slt.StudentListType, sectionOrCohortId);

            return model;
        }

        #region Protected Methods
        protected virtual StaffModel.SubjectArea GetMetricAssessmentArea(
            string metricAssessmentArea, long staffUSI, int schoolId, StudentListType studentListType, long sectionOrCohortId, StaffModel.AssessmentSubType assessmentSubType)
        {
            StaffModel.SubjectArea result;
            if (Enum.TryParse(metricAssessmentArea, true, out result))
                return result;

            metricAssessmentArea = GetRefinedMetricAssessmentArea(metricAssessmentArea, staffUSI, schoolId, studentListType, sectionOrCohortId);

            if (Enum.TryParse(metricAssessmentArea, true, out result))
                return result;

            return StaffModel.SubjectArea.ELA;
        }

        protected virtual string GetRefinedMetricAssessmentArea(string metricAssessmentArea, long staffUSI, int schoolId, StudentListType studentListType, long sectionOrCohortId)
        {
            if (studentListType == StudentListType.Section)
            {
                metricAssessmentArea = (from ts in TeacherSectionRepository.GetAll()
                                        where ts.StaffUSI == staffUSI && ts.SchoolId == schoolId && ts.TeacherSectionId == sectionOrCohortId
                                        select ts.SubjectArea).SingleOrDefault();
            }

            if (studentListType == StudentListType.Cohort)
            {
                metricAssessmentArea = (from sc in StaffCohortRepository.GetAll()
                                        where sc.StaffUSI == staffUSI && sc.EducationOrganizationId == schoolId && sc.StaffCohortId == sectionOrCohortId
                                        select sc.AcademicSubjectType).SingleOrDefault();
            }

            metricAssessmentArea = !String.IsNullOrEmpty(metricAssessmentArea) ? metricAssessmentArea.Replace(" ", String.Empty) : metricAssessmentArea;

            return metricAssessmentArea;
        }

        protected virtual string GetMetricTitle(StaffModel.AssessmentSubType assessmentSubType, dynamic metricAssessmentArea)
        {
            var tempSubjectArea = (StaffModel.SubjectArea)metricAssessmentArea;

            switch (tempSubjectArea)
            {
                case StaffModel.SubjectArea.Writing:
                    return writing;
                case StaffModel.SubjectArea.Mathematics:
                    return mathematics;
                case StaffModel.SubjectArea.Science:
                    return science;
                case StaffModel.SubjectArea.SocialStudies:
                    return socialStudies;
            }

            return elaReading;
        }

        protected virtual int GetSortValue(string cellValue, dynamic testScore, MetricStateType testState)
        {
            if (string.IsNullOrEmpty(cellValue))
                return 0;

            var parts = cellValue.Split('/');

            // Make sure we have an expected format, otherwise put it at the bottom
            if (parts.Length != 2)
                return 0;

            var numerator = Convert.ToDouble(parts[0]);
            var denominator = Convert.ToDouble(parts[1]);

            var objectiveScore = Convert.ToInt32((numerator / denominator) * 1000D);

            // The objective score value is shifted 12 bits into higher order bits
            // leaving enough bits to represent the possible State Assessment score values.
            // The resulting integer can be sorted numerically - first by objective 
            // score, then by State Assessment score
            int sortValue;
            if (testScore is String)
                sortValue = (objectiveScore << 12) + (int)testState;
            else
                sortValue = (objectiveScore << 12) + (int)testScore;

            return sortValue;
        }
        #endregion

        #region Private Methods
        private static StaffModel.AssessmentSubType GetAssessmentSubType(string assessmentSubType)
        {
            StaffModel.AssessmentSubType result;
            if (StaffModel.AssessmentSubType.TryParse(assessmentSubType, true, out result))
                return result;

            return StaffModel.AssessmentSubType.StateStandardized;
        }

        private void GetMetricObjectives(AssessmentDetailsModel model, StaffModel.AssessmentSubType assessmentSubType, dynamic metricAssessmentArea, long staffUSI, int schoolId, StudentListType studentListType, long sectionOrCohortId)
        {
            int[] metricIdForObjectives = GetMetricIdForObjectives(assessmentSubType, metricAssessmentArea);

            //if metricIdForObjectives is empty we should just go ahead and exit
            if (metricIdForObjectives == null || !metricIdForObjectives.Any())
                return;

            IEnumerable<StudentMetricObjective> objectivesQuery;
            switch (studentListType)
            {
                case StudentListType.Section:
                    objectivesQuery = from ts in TeacherSectionRepository.GetAll()
                                      join tss in TeacherStudentSectionRepository.GetAll()
                                      on ts.TeacherSectionId equals tss.TeacherSectionId
                                      join smo in StudentMetricObjectiveRepository.GetAll()
                                      on new { tss.StudentUSI, ts.SchoolId } equals new { smo.StudentUSI, smo.SchoolId }
                                      where ts.StaffUSI == staffUSI && ts.SchoolId == schoolId && ts.TeacherSectionId == sectionOrCohortId && metricIdForObjectives.Contains(smo.MetricId)
                                      select smo;
                    break;

                case StudentListType.Cohort:
                    objectivesQuery = from sc in StaffCohortRepository.GetAll()
                                      join ssc in StaffStudentCohortRepository.GetAll()
                                      on sc.StaffCohortId equals ssc.StaffCohortId
                                      join smo in StudentMetricObjectiveRepository.GetAll()
                                      on new { ssc.StudentUSI, SchoolId = sc.EducationOrganizationId } equals new { smo.StudentUSI, smo.SchoolId }
                                      where sc.StaffUSI == staffUSI && sc.EducationOrganizationId == schoolId && sc.StaffCohortId == sectionOrCohortId && metricIdForObjectives.Contains(smo.MetricId)
                                      select smo;
                    break;

                case StudentListType.CustomStudentList:
                    var studentUSIs = Enumerable.ToArray(StaffCustomStudentListStudentRepository.GetAll().Where(x => x.StaffCustomStudentListId == sectionOrCohortId).Select(x => x.StudentUSI));

                    if (studentUSIs.Length == 0)
                        return;

                    objectivesQuery = from smo in StudentMetricObjectiveRepository.GetAll()
                                      where studentUSIs.Contains(smo.StudentUSI) && smo.SchoolId == schoolId && metricIdForObjectives.Contains(smo.MetricId)
                                      select smo;
                    break;

                default:
                    var sectionStudentIds = (from tss in TeacherStudentSectionRepository.GetAll().Distinct()
                                             join ts in TeacherSectionRepository.GetAll() on tss.TeacherSectionId equals ts.TeacherSectionId
                                             where ts.StaffUSI == staffUSI && ts.SchoolId == schoolId
                                             group tss by tss.StudentUSI
                                                 into g
                                             select g.Key).ToList();

                    var cohortStudentIds = (from ssc in StaffStudentCohortRepository.GetAll().Distinct()
                                            join sc in StaffCohortRepository.GetAll() on ssc.StaffCohortId equals sc.StaffCohortId
                                            where sc.StaffUSI == staffUSI && sc.EducationOrganizationId == schoolId
                                            group ssc by ssc.StudentUSI
                                                into h
                                            select h.Key).ToList();

                    var customStudentListStudentIds = (from csl in StaffCustomStudentListRepository.GetAll()
                                                       join csls in StaffCustomStudentListStudentRepository.GetAll() on csl.StaffCustomStudentListId equals csls.StaffCustomStudentListId
                                                       where csl.StaffUSI == staffUSI && csl.EducationOrganizationId == schoolId
                                                       group csls by csls.StudentUSI
                                                        into h
                                                       select h.Key).ToList();

                    var studentIds = sectionStudentIds.Concat(cohortStudentIds).Concat(customStudentListStudentIds).Distinct().ToArray();

                    if (studentIds.Length == 0)
                        return;

                    objectivesQuery = from smo in StudentMetricObjectiveRepository.GetAll()
                                      where studentIds.Contains(smo.StudentUSI) && smo.SchoolId == schoolId && metricIdForObjectives.Contains(smo.MetricId)
                                      select smo;
                    break;
            }


            //Setting the dynamic column headers for the objectives.
            var objectives = from o in objectivesQuery
                             group o by o.ObjectiveName into g
                             select new { objectiveName = g.Key, moreData = g };

            foreach (var o in objectives)
            {
                var tempObjectiveTitle = new AssessmentDetailsModel.ObjectiveTitle
                {
                    UniqueIdentifier = o.objectiveName.GetHashCode(),
                    Title = o.objectiveName,
                    Description = o.moreData != null && o.moreData.Count() > 0
                                                                ? o.moreData.First().Description :
                                                                string.Empty
                };

                switch (assessmentSubType)
                {
                    case StaffModel.AssessmentSubType.StateStandardized:
                        tempObjectiveTitle.Width = AssessmentDetailsProvider.GetObjectiveColumnWidth();
                        break;

                    case StaffModel.AssessmentSubType.Benchmark:
                        tempObjectiveTitle.Width = AssessmentBenchmarkDetailsProvider.GetObjectiveColumnWidth();
                        break;

                    case StaffModel.AssessmentSubType.Reading:
                        tempObjectiveTitle.Width = AssessmentReadingDetailsProvider.GetObjectiveColumnWidth();
                        break;

                    default:
                        tempObjectiveTitle.Width = string.Empty;
                        break;
                }

                model.ObjectiveTitles.Add(tempObjectiveTitle);
            }

            var objectiveTestedCounter = new Dictionary<string, int>();
            var objectiveMasteredCounter = new Dictionary<string, int>();

            //Setting objective values by student
            var objectivesGroupedByStudent = from s in objectivesQuery
                                             group s by new { s.StudentUSI, s.MetricId } into g
                                             select new { g.Key, objectives = g };
            //Adding the objectives.
            foreach (var dataStudent in objectivesGroupedByStudent)
            {
                var matchingStudents = GetMatchingStudentsForAssessmentSubType(assessmentSubType, model, dataStudent.Key.StudentUSI, dataStudent.Key.MetricId);

                foreach (var student in matchingStudents)
                {
                    foreach (var objective in dataStudent.objectives)
                    {
                        var obj = new StudentWithMetricsAndAssessments.AssessmentMetric(dataStudent.Key.StudentUSI)
                        {
                            UniqueIdentifier = objective.ObjectiveName.GetHashCode(),
                            MetricVariantId = objective.MetricId,
                            State = (objective.MetricStateTypeId.HasValue) ? (MetricStateType)objective.MetricStateTypeId.Value : MetricStateType.None,

                            ObjectiveName = objective.ObjectiveName,
                            DisplayValue = objective.Value
                        };

                        obj.Value = GetObjectiveValueForAssessmentSubType(assessmentSubType, student, objective);

                        student.Metrics.Add(obj);

                        if (!objectiveTestedCounter.ContainsKey(objective.ObjectiveName))
                        {
                            objectiveTestedCounter.Add(objective.ObjectiveName, 0);
                            objectiveMasteredCounter.Add(objective.ObjectiveName, 0);
                        }
                        objectiveTestedCounter[objective.ObjectiveName] = objectiveTestedCounter[objective.ObjectiveName] + 1;
                        if (IsObjectiveMastered(obj))
                        {
                            objectiveMasteredCounter[objective.ObjectiveName] = objectiveMasteredCounter[objective.ObjectiveName] + 1;
                        }
                    }
                }
            }

            //Add the mastery
            var i = 0;
            while (i < model.ObjectiveTitles.Count)
            {
                var objective = model.ObjectiveTitles[i];
                if (!objectiveTestedCounter.ContainsKey(objective.Title))
                {
                    model.ObjectiveTitles.RemoveAt(i);
                    continue;
                }

                objective.Mastery = String.Format("{0} of {1}", objectiveMasteredCounter[objective.Title], objectiveTestedCounter[objective.Title]);
                i++;
            }
        }

        protected virtual bool IsObjectiveMastered(StudentWithMetricsAndAssessments.AssessmentMetric obj)
        {
            return obj.State == MetricStateType.Good || obj.State == MetricStateType.VeryGood || obj.State == MetricStateType.Acceptable;
        }

        private int[] GetMetricIdForObjectives(StaffModel.AssessmentSubType assessmentSubType, dynamic metricAssessmentArea)
        {
            switch (assessmentSubType)
            {
                case StaffModel.AssessmentSubType.Benchmark:
                    return AssessmentBenchmarkDetailsProvider.GetMetricIdsForObjectives(metricAssessmentArea);

                case StaffModel.AssessmentSubType.Reading:
                    return AssessmentReadingDetailsProvider.GetMetricIdsForObjectives(metricAssessmentArea);

                default:
                    // since we don't know what state we're in, we cannot return state-specific objectives
                    // and call the default implementation
                    return AssessmentDetailsProvider.GetMetricIdsForObjectives(metricAssessmentArea);
            }
        }

        private static IEnumerable<StudentWithMetricsAndAssessments> GetMatchingStudentsForAssessmentSubType(StaffModel.AssessmentSubType assessmentSubType, AssessmentDetailsModel model, long studentUSI, int metricId)
        {
            switch (assessmentSubType)
            {
                case StaffModel.AssessmentSubType.Benchmark:
                case StaffModel.AssessmentSubType.Reading:
                    return model.Students.Where(x => x.StudentUSI == studentUSI);

                default:
                    return model.Students.Where(x => x.StudentUSI == studentUSI && x.Score.MetricVariantId == metricId);
            }
        }

        private dynamic GetObjectiveValueForAssessmentSubType(StaffModel.AssessmentSubType assessmentSubType, StudentWithMetricsAndAssessments student, StudentMetricObjective objective)
        {
            switch (assessmentSubType)
            {
                case StaffModel.AssessmentSubType.Benchmark:
                case StaffModel.AssessmentSubType.Reading:
                    return (objective.MetricStateTypeId.HasValue ? (dynamic)objective.MetricStateTypeId.Value : (dynamic)0);

                default:
                    return GetSortValue(objective.Value, student.Score.Value ?? 0, student.Score.State);
            }
        }
        #endregion
    }

    public sealed class AssessmentDetailsService : AssessmentDetailsServiceBase<AssessmentDetailsRequest, AssessmentDetailsModel, StudentWithMetricsAndAssessments>, IAssessmentDetailsService { }
}
