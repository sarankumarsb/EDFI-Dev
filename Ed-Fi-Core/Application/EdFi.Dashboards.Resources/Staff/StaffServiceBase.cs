// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using EdFi.Dashboards.Application.Data.Entities;
using EdFi.Dashboards.Common.Utilities;
using EdFi.Dashboards.Data.Entities;
using EdFi.Dashboards.Data.Repository;
using EdFi.Dashboards.Metric.Resources.Models;
using EdFi.Dashboards.Metric.Resources.Providers;
using EdFi.Dashboards.Metric.Resources.Services;
using EdFi.Dashboards.Resources.Common;
using EdFi.Dashboards.Resources.Metric;
using EdFi.Dashboards.Resources.Models.Common;
using EdFi.Dashboards.Resources.Models.CustomGrid;
using EdFi.Dashboards.Resources.Models.Student;
using EdFi.Dashboards.Resources.Navigation;
using EdFi.Dashboards.Resources.StudentMetrics;
using EdFi.Dashboards.Resources.StudentSchool;

namespace EdFi.Dashboards.Resources.Staff
{
    public abstract class StaffServiceBase
    {
        protected const string StudentLink = "student";
        //For extensibility we inject the dependencies through properties.
        public IRepository<StudentMetric> StudentListWithMetricsRepository { get; set; }
        public IRepository<TeacherStudentSection> TeacherStudentSectionRepository { get; set; }
        public IRepository<TeacherSection> TeacherSectionRepository { get; set; }
        public IRepository<StaffStudentCohort> StaffStudentCohortRepository { get; set; }
        public IRepository<StaffCustomStudentList> StaffCustomStudentListRepository { get; set; }
        public IRepository<StaffCustomStudentListStudent> StaffCustomStudentListStudentRepository { get; set; }
        public IRepository<StaffCohort> StaffCohortRepository { get; set; }
        public IAccommodationProvider AccommodationProvider { get; set; }
        public IUniqueListIdProvider UniqueListProvider { get; set; }
        public ITrendRenderingDispositionProvider TrendRenderingDispositionProvider { get; set; }
        public IRootMetricNodeResolver RootMetricNodeResolver { get; set; }
        public IStudentSchoolAreaLinks StudentSchoolAreaLinks { get; set; }
        public IMetricStateProvider MetricStateProvider { get; set; }
        public IStudentListUtilitiesProvider StudentListUtilitiesProvider { get; set; }
        public IStudentMetricsProvider StudentListWithMetricsProvider { get; set; }

        protected internal virtual StudentListIdentity GetStudentListIdentity(long staffUSI, int schoolId, string studentListType, long sectionOrCohortId)
        {
            StudentListType slt;

			if (!Enum.TryParse(studentListType, true, out slt))
				slt = StudentListType.None;

            // If no list type was provided, get a default one
            if (slt == StudentListType.None)
                return GetDefaultListIdentity(staffUSI, schoolId);

            // Return the supplied information as a list identity 
            return new StudentListIdentity
                       {
                           StudentListType = slt, 
                           Id = sectionOrCohortId
                       };
        }

        [Obsolete("Use GetStudentListIdentity method instead. This method will be removed in the next Major release version of the Dashboard.")]
        protected StudentListType GetSection(long staffUSI, int schoolId, string studentListType, ref long sectionOrCohortId)
        {
            if (String.IsNullOrEmpty(studentListType))
                studentListType = StudentListType.None.ToString();

            var slt = (StudentListType)Enum.Parse(typeof(StudentListType), studentListType, true);

            if (slt == StudentListType.None)
            {
                var firstSection = (from data in TeacherSectionRepository.GetAll()
                                    where data.StaffUSI == staffUSI && data.SchoolId == schoolId
                                    orderby data.SubjectArea, data.CourseTitle, data.ClassPeriod, data.LocalCourseCode, data.TeacherSectionId
                                    select data).FirstOrDefault();
                if (firstSection != null)
                {
                    slt = StudentListType.Section;
                    sectionOrCohortId = firstSection.TeacherSectionId;
                    return slt;
                }

                var firstCohort = (from data in StaffCohortRepository.GetAll()
                                   where data.StaffUSI == staffUSI && data.EducationOrganizationId == schoolId
                                   orderby data.CohortDescription, data.StaffCohortId
                                   select data).FirstOrDefault();
                if (firstCohort != null)
                {
                    slt = StudentListType.Cohort;
                    sectionOrCohortId = firstCohort.StaffCohortId;
                    return slt;
                }

                var firstCustom = (from data in StaffCustomStudentListRepository.GetAll()
                                   where data.StaffUSI == staffUSI && data.EducationOrganizationId == schoolId
                                   orderby data.CustomStudentListIdentifier, data.StaffCustomStudentListId
                                   select data).FirstOrDefault();
                if (firstCustom != null)
                {
                    slt = StudentListType.CustomStudentList;
                    sectionOrCohortId = firstCustom.StaffCustomStudentListId;
                    return slt;
                }
            }

            return slt;
        }

        // TODO: GKM - Review this implementation vs. DefaultSectionService implementation
        private StudentListIdentity GetDefaultListIdentity(long staffUSI, int schoolId)
        {
            // Use teacher's first section, if one exists
            var firstSection = (from data in TeacherSectionRepository.GetAll()
                                where data.StaffUSI == staffUSI && data.SchoolId == schoolId
                                orderby data.SubjectArea, data.CourseTitle, data.ClassPeriod, data.LocalCourseCode, data.TeacherSectionId
                                select data).FirstOrDefault();

            if (firstSection != null)
            {
                return new StudentListIdentity
                    {
                        StudentListType = StudentListType.Section,
                        Id = firstSection.TeacherSectionId
                    };
            }

            // Use staff member's first cohort, if one exists
            var firstCohort = (from data in StaffCohortRepository.GetAll()
                               where data.StaffUSI == staffUSI && data.EducationOrganizationId == schoolId
                               orderby data.CohortDescription, data.StaffCohortId
                               select data).FirstOrDefault();

            if (firstCohort != null)
            {
                return new StudentListIdentity
                    {
                        StudentListType = StudentListType.Cohort,
                        Id = firstCohort.StaffCohortId,
                    };
            }

            // Use staff member's first custom student list, if one exists
            var firstCustom = (from data in StaffCustomStudentListRepository.GetAll()
                               where data.StaffUSI == staffUSI && data.EducationOrganizationId == schoolId
                               orderby data.CustomStudentListIdentifier, data.StaffCustomStudentListId
                               select data).FirstOrDefault();

            if (firstCustom != null)
            {
                return new StudentListIdentity
                    {
                        StudentListType = StudentListType.CustomStudentList,
                        Id = firstCustom.StaffCustomStudentListId,
                    };
            }

            // Nothing to show
            return new StudentListIdentity {StudentListType = StudentListType.None, Id = 0};
        }

        protected virtual IEnumerable<StudentMetric> GetStudentListWithMetrics(long staffUSI, int schoolId, IEnumerable<long> students)
        {
            var queryOptions = new StudentMetricsProviderQueryOptions
                {
                    SchoolId = schoolId,
                    StaffUSI = staffUSI,
                    StudentIds = students
                };

            return StudentListWithMetricsProvider.GetStudentsWithMetrics(queryOptions);
        }

        protected virtual List<EnhancedStudentInformation> GetStudentListEntities(int schoolId, long staffUSI, 
            StudentListIdentity studentListIdentity, List<MetadataColumnGroup> listMetadata, 
            List<long> customStudentIdList, int? sortColumn, string sortDirection)
        {
            var queryOptions = new StudentMetricsProviderQueryOptions
                {
                    MetricVariantIds = listMetadata.GetMetricVariantIds(),
                    SchoolId = schoolId,
                    StaffUSI = staffUSI,
                    StudentIds = customStudentIdList
                };

            switch (studentListIdentity.StudentListType)
            {
                case StudentListType.Section:
                    queryOptions.TeacherSectionIds = new[]
                    {
                        studentListIdentity.Id
                    };

                    break;
                case StudentListType.Cohort:
                    queryOptions.StaffCohortIds = new[]
                    {
                        studentListIdentity.Id
                    };

                    break;
            }

            return
                StudentListWithMetricsProvider.GetOrderedStudentList(queryOptions, GetSortColumn(listMetadata, sortColumn), sortDirection).ToList();
        }

        protected virtual MetadataColumn GetSortColumn(List<MetadataColumnGroup> listMetadata, int? sortColumn)
        {
            if (!sortColumn.HasValue)
                return null;
            var columns = new List<MetadataColumn>();
            foreach (var group in listMetadata)
            {
                columns.AddRange(group.Columns);
                columns.Add(null); // spacer column in EdFiGrid between groups
            }
            return columns.Skip(sortColumn.Value - 1).FirstOrDefault();
        }

        protected void OverlayStudentAccommodation(List<StudentWithMetricsAndAccommodations> students, int schoolId)
        {
            var studentIds = students.Select(x => x.StudentUSI).Distinct().ToArray();
            if (studentIds.Count() > 0)
            {
                var allAccommodationsForStudents = AccommodationProvider.GetAccommodations(studentIds, schoolId);
                if (allAccommodationsForStudents != null)
                    foreach (var sa in allAccommodationsForStudents)
                    {
                        students.Where(x => x.StudentUSI == sa.StudentUSI).ToList().ForEach(y => y.Accommodations = sa.AccommodationsList);
                    }
            }
        }

        protected List<long> GetCustomListStudentIds(long staffUSI, int schoolId, StudentListIdentity studentListIdentity)
        {
            var customStudentIdList = new List<long>();

            switch (studentListIdentity.StudentListType)
            {
                case StudentListType.CustomStudentList:
                    long sectionOrCohortId = studentListIdentity.Id;

                    customStudentIdList =
                        (from s in StaffCustomStudentListStudentRepository.GetAll()
                         where s.StaffCustomStudentListId == sectionOrCohortId
                         select s.StudentUSI)
                            .ToList();

                    break;

                case StudentListType.All:
                    customStudentIdList = (from csl in StaffCustomStudentListRepository.GetAll()
                                           join csls in StaffCustomStudentListStudentRepository.GetAll() on
                                               csl.StaffCustomStudentListId equals csls.StaffCustomStudentListId
                                           where csl.StaffUSI == staffUSI && csl.EducationOrganizationId == schoolId
                                           select csls.StudentUSI)
                                            .Distinct()
                                            .ToList();
                    break;
            }

            return customStudentIdList;
        }

        protected List<StudentWithMetrics.Metric> ShapeMetrics(IEnumerable<StudentMetric> studentMetrics, IEnumerable<MetadataColumnGroup> listMetadata)
        {
            var metrics = new List<StudentWithMetrics.Metric>();
            foreach (var @group in listMetadata.Where(@group => @group.GroupType != GroupType.EntityInformation))
            {
                metrics.AddRange(from metadataColumn in @group.Columns.Where(column => !String.IsNullOrWhiteSpace(column.ColumnPrefix))
                                 let studentMetric =
                                     studentMetrics.FirstOrDefault(
                                         withMetrics => withMetrics.MetricVariantId == metadataColumn.MetricVariantId)
                                 where studentMetric != null
                                 select new StudentWithMetrics.Metric
                                 {
                                     UniqueIdentifier = metadataColumn.UniqueIdentifier,
                                     State = GetMetricState(studentMetric),
                                     MetricVariantId = studentMetric.MetricVariantId,
                                     Value =
                                         InstantiateValue.FromValueType(studentMetric.Value,
                                                                        studentMetric.ValueTypeName),
                                     DisplayValue = GetDisplayValue(studentMetric, studentMetric.Value)
                                 });
            }
            return metrics;
        }

        private static string GetDisplayValue(StudentMetric studentMetric, dynamic value)
        {
            if (studentMetric.Value == null)
                return string.Empty;
            var format = "{0}";
            if (!string.IsNullOrWhiteSpace(studentMetric.Format))
                format = studentMetric.Format;
            return string.Format(format, value);
        }

        private MetricStateType GetMetricState(StudentMetric studentMetric)
        {
            if (studentMetric.MetricStateTypeId.HasValue)
                return (MetricStateType)studentMetric.MetricStateTypeId.Value;
            return MetricStateProvider.GetState(studentMetric.MetricId, studentMetric.Value, studentMetric.ValueTypeName).StateType;
        }
    }
}
