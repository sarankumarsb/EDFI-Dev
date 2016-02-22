// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using EdFi.Dashboards.Common;
using EdFi.Dashboards.Core;
using EdFi.Dashboards.Data.Entities;
using EdFi.Dashboards.Data.Repository;
using EdFi.Dashboards.Metric.Resources.Models;
using EdFi.Dashboards.Resource.Models.Common;
using EdFi.Dashboards.Resources.Common;
using EdFi.Dashboards.Resources.Models.Common;
using EdFi.Dashboards.Resources.Models.CustomGrid;
using EdFi.Dashboards.Resources.Models.Student;
using EdFi.Dashboards.Resources.Navigation;
using EdFi.Dashboards.Resources.Navigation.Support;
using EdFi.Dashboards.Resources.School;
using EdFi.Dashboards.Resources.Security.Common;
using EdFi.Dashboards.Resources.Models.Staff;
using EdFi.Dashboards.Resources.StudentSchool;
using EdFi.Dashboards.SecurityTokenService.Authentication;
using MetricComponent = EdFi.Dashboards.Metric.Data.Entities.MetricComponent;

namespace EdFi.Dashboards.Resources.Staff
{
    public class SubjectSpecificOverviewRequest
    {
        public int SchoolId { get; set; }
        public long StaffUSI { get; set; }

        public string StudentListType { get; set; }
        public long SectionOrCohortId { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SubjectSpecificOverviewRequest"/> using the specified parameters.
        /// </summary>
        /// <returns>A new <see cref="SubjectSpecificOverviewRequest"/> instance.</returns>
        public static SubjectSpecificOverviewRequest Create(int schoolId, long staffUSI, string studentListType, long sectionOrCohortId)
        {
            return new SubjectSpecificOverviewRequest { SchoolId = schoolId, StaffUSI = staffUSI, StudentListType = studentListType, SectionOrCohortId = sectionOrCohortId };
        }
    }

    public interface ISubjectSpecificOverviewService : IService<SubjectSpecificOverviewRequest, SubjectSpecificOverviewModel> { }

    public class SubjectSpecificOverviewService : StaffServiceBase, ISubjectSpecificOverviewService
    {
        public IRepository<MetricComponent> MetricComponentRepository { get; set; }
        public IRepository<StudentSchoolMetricInstanceSet> StudentSchoolMetricInstanceSetRepository { get; set; }
        public IRepository<StudentRecordCurrentCourse> StudentRecordCurrentCourseRepository { get; set; }
        public IRepository<Dashboards.Metric.Data.Entities.Metric> MetricRepository { get; set; }
        public ISchoolCategoryProvider SchoolCategoryProvider { get; set; }
        public IGradeStateProvider GradeStateProvider { get; set; }
        public IMetadataListIdResolver MetadataListIdResolver { get; set; }
        public IClassroomMetricsProvider ClassroomMetricsProvider { get; set; }
        public IListMetadataProvider ListMetadataProvider { get; set; }
        public ISubjectSpecificOverviewMetricComponentProvider SubjectSpecificOverviewMetricComponentProvider { get; set; }
        public IGradeLevelUtilitiesProvider GradeLevelUtilitiesProvider { get; set; }

        [CanBeAuthorizedBy(EdFiClaimTypes.ViewAllStudents, EdFiClaimTypes.ViewMyStudents)]
        public SubjectSpecificOverviewModel Get(SubjectSpecificOverviewRequest request)
        {
            long staffUSI = request.StaffUSI;
            int schoolId = request.SchoolId;
            string studentListType = request.StudentListType;
            long sectionOrCohortId = request.SectionOrCohortId;

            var model = new SubjectSpecificOverviewModel();

            var slt = GetStudentListIdentity(staffUSI, schoolId, studentListType, sectionOrCohortId);
            if (sectionOrCohortId == 0 || slt.StudentListType != StudentListType.Section)
                return model;

            var subjectArea = GetSubjectArea(staffUSI, schoolId, slt.StudentListType, sectionOrCohortId);
            if (String.IsNullOrEmpty(subjectArea))
                return model;

            model.SubjectArea = subjectArea;
            model.UniqueListId = UniqueListProvider.GetUniqueId();
            model.SchoolCategory = SchoolCategoryProvider.GetSchoolCategoryType(schoolId);

            //Set the defaults if necessary.
            //We default to SchoolCategory.HighSchool if its not an Elementary.
            if (model.SchoolCategory != SchoolCategory.Elementary)
                model.SchoolCategory = SchoolCategory.HighSchool;

            //Default for the Subject Area.
            switch (model.SubjectArea)
            {
                case "Mathematics":
                case "Social Studies":
                case "Science":
                case "Writing":
                    break;
                default:
                    model.SubjectArea = "ELA / Reading";
                    break;
            }

            //Get the metadata.
            var resolvedListId = MetadataListIdResolver.GetListId(ListType.ClassroomSubjectSpecific, model.SchoolCategory, model.SubjectArea);
            model.ListMetadata = ListMetadataProvider.GetListMetadata(resolvedListId, model.SubjectArea);

            var students = GetStudentListEntities(schoolId, staffUSI, slt, new List<MetadataColumnGroup>(), new List<long>(), null, null); // TODO: a lot of nulls in this call
            var studentMetricsList = GetStudentListWithMetrics(staffUSI, schoolId, students.Select(student => student.StudentUSI).ToList()).ToList();
            if (!studentMetricsList.Any())
                return model;

            var studentsWithAccomodations = new List<StudentWithMetricsAndAccommodations>();

            var uniqueListId = UniqueListProvider.GetUniqueId();

            //Student Unique Field Newly Added : Saravanan
            // can't be a LINQ statement b/c StudentMetric does not have a default constructor
            foreach (var student in students)
            {
                long studentUSI = student.StudentUSI;
                var studentMetrics = studentMetricsList.Where(withMetrics => withMetrics.StudentUSI == student.StudentUSI).ToList();
                var gender = student.Gender;
                var fullName = student.FullName;

                var studentWithMetrics = new StudentWithMetricsAndAccommodations(student.StudentUSI)
                {
                    SchoolId = student.SchoolId,
                    Name = Utilities.FormatPersonNameByLastName(student.FirstName, student.MiddleName, student.LastSurname),
                    ThumbNail = StudentSchoolAreaLinks.Image(schoolId, studentUSI, gender, fullName).Resolve(),
                    Href = new Link { Rel = StudentLink, Href = StudentSchoolAreaLinks.Overview(schoolId, studentUSI, fullName).AppendParameters("listContext=" + uniqueListId).Resolve() },
                    Metrics = ShapeMetrics(studentMetrics, model.ListMetadata),
                    GradeLevel = GradeLevelUtilitiesProvider.FormatGradeLevelForSorting(student.GradeLevel),
                    GradeLevelDisplayValue = GradeLevelUtilitiesProvider.FormatGradeLevelForDisplay(student.GradeLevel),
                    StudentUniqueID = student.StudentUniqueID
                };

                studentsWithAccomodations.Add(studentWithMetrics);
            }

            OverlayStudentAccommodation(studentsWithAccomodations, schoolId);

            model.Students = studentsWithAccomodations;
            BuildSubjectSpecificMetrics(model, staffUSI, schoolId, sectionOrCohortId);
            BuildSubjectGrades(model.Students, staffUSI, schoolId, sectionOrCohortId, model.ListMetadata);

            return model;
        }

        private string GetSubjectArea(long staffUSI, int schoolId, StudentListType studentListType, long sectionOrCohortId)
        {
            switch (studentListType)
            {
                case StudentListType.Section:
                    {
                        return (from data in TeacherSectionRepository.GetAll()
                                where data.TeacherSectionId == sectionOrCohortId && data.StaffUSI == staffUSI && data.SchoolId == schoolId
                                select data.SubjectArea).SingleOrDefault();
                    }
                case StudentListType.Cohort:
                    {
                        return (from data in StaffCohortRepository.GetAll()
                                where data.StaffCohortId == sectionOrCohortId && data.StaffUSI == staffUSI && data.EducationOrganizationId == schoolId
                                select data.AcademicSubjectType).SingleOrDefault();
                    }
            }

            return null;
        }

        private void BuildSubjectSpecificMetrics(SubjectSpecificOverviewModel model, long staffUSI, int schoolId, long sectionOrCohortId)
        {
            var metricComponentIds = SubjectSpecificOverviewMetricComponentProvider.GetMetricIdsForComponents();

            var studentMetricComponentsData = (from ts in TeacherSectionRepository.GetAll()
                                           join tss in TeacherStudentSectionRepository.GetAll()
                                           on ts.TeacherSectionId equals tss.TeacherSectionId
                                           join se in StudentSchoolMetricInstanceSetRepository.GetAll()
                                           on new { tss.StudentUSI, ts.SchoolId } equals new { se.StudentUSI, se.SchoolId }
                                           join mc in MetricComponentRepository.GetAll()
                                           on new { se.MetricInstanceSetKey, ts.LocalCourseCode } equals new { mc.MetricInstanceSetKey, LocalCourseCode = mc.Name }
                                           join m in MetricRepository.GetAll()
                                           on mc.MetricId equals m.MetricId
                                           where ts.TeacherSectionId == sectionOrCohortId 
                                           && ts.SchoolId == schoolId 
                                           && ts.StaffUSI == staffUSI 
                                           && metricComponentIds.Contains(mc.MetricId)
                                           select new { MetricComponent = mc, tss.StudentUSI, m.TrendInterpretation }).ToList();

            var metricComponentsMetadata = metricComponentIds.Select(metricComponentId => FindMetricIndexAndMetricMetadata(model.ListMetadata, metricComponentId)).ToList();

            foreach (var student in model.Students)
            {
                foreach (var metricIndexAndMetadataColumn in metricComponentsMetadata)
                {
                    if (metricIndexAndMetadataColumn == null)
                        continue;

                    var metricData = studentMetricComponentsData.SingleOrDefault(x => x.StudentUSI == student.StudentUSI && x.MetricComponent.MetricId == metricIndexAndMetadataColumn.MetadataColumn.MetricVariantId);
                    var metric = metricData != null
                                                ? StudentListUtilitiesProvider.PrepareTrendMetric(student.StudentUSI, metricIndexAndMetadataColumn.MetadataColumn.UniqueIdentifier, metricIndexAndMetadataColumn.MetadataColumn.MetricVariantId, metricData.MetricComponent.Value, metricData.MetricComponent.MetricStateTypeId, metricData.MetricComponent.ValueTypeName, metricData.MetricComponent.Format, metricData.TrendInterpretation, metricData.MetricComponent.TrendDirection, TrendRenderingDispositionProvider)
                                                : new StudentWithMetrics.TrendMetric(student.StudentUSI) { UniqueIdentifier = metricIndexAndMetadataColumn.MetadataColumn.UniqueIdentifier, MetricVariantId = metricIndexAndMetadataColumn.MetadataColumn.MetricVariantId, DisplayValue = String.Empty, State = MetricStateType.None };
                
                    if (student.Metrics.Count > metricIndexAndMetadataColumn.Index)
                        student.Metrics.Insert(metricIndexAndMetadataColumn.Index, metric);
                    else
                        student.Metrics.Add(metric);
                }
            }
        }

        private class MetricIndexAndMetadataColumn
        {
            public MetricIndexAndMetadataColumn(int index, MetadataColumn metadataColumn)
            {
                Index = index;
                MetadataColumn = metadataColumn;
            }

            public int Index { get; set; }
            public MetadataColumn MetadataColumn { get; set; }
        }

		private static MetricIndexAndMetadataColumn FindMetricIndexAndMetricMetadata(List<MetadataColumnGroup> metadata, int metricId)
        {
            int indexCounter = 0;
            foreach (var metadataColumnGroup in metadata)
            {
                if (metadataColumnGroup.GroupType == GroupType.EntityInformation)
                    continue;

                foreach (var column in metadataColumnGroup.Columns)
                {
                    if (column.MetricVariantId == metricId)
                        return new MetricIndexAndMetadataColumn(indexCounter, column);
                    indexCounter++;
                }
            }

            return null;
        }

        private void BuildSubjectGrades(IEnumerable<StudentWithMetricsAndAccommodations> students, long staffUSI, int schoolId, long sectionOrCohortId, List<MetadataColumnGroup> metadata)
        {
            //Select Key by max grading period
            var baseQuery = from g in StudentRecordCurrentCourseRepository.GetAll()
                            join section in TeacherSectionRepository.GetAll()
                            on g.LocalCourseCode equals section.LocalCourseCode
                            where section.StaffUSI == staffUSI && section.SchoolId == schoolId && section.TeacherSectionId == sectionOrCohortId && g.SchoolId == schoolId && (g.LetterGradeEarned != null || g.NumericGradeEarned != null)
                            group g by new { g.StudentUSI, g.SchoolId, g.LocalCourseCode } into grp
                            select new
                            {
                                grp.Key.StudentUSI,
                                grp.Key.SchoolId,
                                grp.Key.LocalCourseCode,
                                GradingPeriod = grp.Max(x1 => x1.GradingPeriod),
                            };

            var gradesForLatestPeriod = (from g in StudentRecordCurrentCourseRepository.GetAll()
                                         join k in baseQuery on
                                            new { g.StudentUSI, g.SchoolId, g.LocalCourseCode, g.GradingPeriod } equals
                                            new { k.StudentUSI, k.SchoolId, k.LocalCourseCode, k.GradingPeriod }
                                         where (g.LetterGradeEarned != null || g.NumericGradeEarned != null)
                                         orderby g.TermTypeId descending
                                         select g).ToList();

            var metricIndexAndMetadataColumn = FindMetricIndexAndMetricMetadata(metadata, (int)StudentMetricEnum.SubjectAreaCourseGrades);
            foreach (var student in students)
            {
                var currentGrade = gradesForLatestPeriod.FirstOrDefault(x => x.StudentUSI == student.StudentUSI);

                var metric = GetGradeMetric(student.StudentUSI, currentGrade, metricIndexAndMetadataColumn.MetadataColumn);

                if (student.Metrics.Count > metricIndexAndMetadataColumn.Index)
                    student.Metrics.Insert(metricIndexAndMetadataColumn.Index, metric);
                else
                    student.Metrics.Add(metric);
            }
        }

        private StudentWithMetrics.Metric GetGradeMetric(long studentUSI, StudentRecordCurrentCourse currentCourse, MetadataColumn columnMetaData)
        {
            if (currentCourse == null)
                return new StudentWithMetrics.TrendMetric(studentUSI) { UniqueIdentifier = columnMetaData.UniqueIdentifier, MetricVariantId = columnMetaData.MetricVariantId, DisplayValue = String.Empty, Value = String.Empty, State = MetricStateType.None };

            var metric = new StudentWithMetrics.TrendMetric(studentUSI)
            {
                UniqueIdentifier = columnMetaData.UniqueIdentifier,
                MetricVariantId = (int)StudentMetricEnum.SubjectAreaCourseGrades
            };

            if (currentCourse.NumericGradeEarned.HasValue)
            {
                metric.DisplayValue = currentCourse.NumericGradeEarned.ToString();
                metric.Value = currentCourse.NumericGradeEarned.Value;
                metric.State = GradeStateProvider.Get(currentCourse.NumericGradeEarned);
            }
            else
            {
                metric.DisplayValue = currentCourse.LetterGradeEarned;
                metric.Value = currentCourse.LetterGradeEarned;
                metric.State = GradeStateProvider.Get(currentCourse.LetterGradeEarned);
            }
            metric.Trend = currentCourse.TrendDirection.HasValue ? TrendRenderingDispositionProvider.GetTrendRenderingDisposition((TrendDirection)currentCourse.TrendDirection, TrendInterpretation.Standard) : TrendEvaluation.None;
            return metric;
        }
    }
}
