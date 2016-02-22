// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;
using EdFi.Dashboards.Common;
using EdFi.Dashboards.Core.Providers.Context;
using EdFi.Dashboards.Resources.Models.Common;
using EdFi.Dashboards.Resources.Models.Staff;
using EdFi.Dashboards.Resources.Security.Common;
using EdFi.Dashboards.SecurityTokenService.Authentication;

namespace EdFi.Dashboards.Resources.Staff
{
    public class ExportAllMetricsRequest
    {
        public long StaffUSI { get; set; }
        public int SchoolId { get; set; }
        public string StudentListType { get; set; }
        public long SectionOrCohortId { get; set; }

        [AuthenticationIgnore("AssessmentSubType does not affect the results of the request in a way requiring it to be secured.")]
        public string AssessmentSubType { get; set; }

        [AuthenticationIgnore("SubjectArea does not affect the results of the request in a way requiring it to be secured.")]
        public string SubjectArea { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExportAllMetricsRequest"/> using the specified parameters.
        /// </summary>
        /// <returns>A new <see cref="ExportAllMetricsRequest"/> instance.</returns>
        public static ExportAllMetricsRequest Create(long staffUSI, int schoolId, string studentListType,
                                                        long sectionOrCohortId, string assessmentSubType, string subjectArea)
        {
            return new ExportAllMetricsRequest
                       {
                           StaffUSI = staffUSI,
                           SchoolId = schoolId,
                           StudentListType = studentListType,
                           SectionOrCohortId = sectionOrCohortId,
                           AssessmentSubType = assessmentSubType,
                           SubjectArea = subjectArea
                       };
        }
    }

    public interface IExportAllMetricsService : IService<ExportAllMetricsRequest, StudentExportAllModel>
    {
    }

    public class ExportAllMetricsService : StaffServiceBase, IExportAllMetricsService
    {
        private readonly IService<ExportAllStudentsForStaffsSectionsAndCohortsRequest, StudentExportAllModel> exportAllStudentsForStaffsSectionsAndCohortsService;
        private readonly IService<ExportAllStudentMetricsForStaffSectionRequest, StudentExportAllModel> exportAllStudentMetricsForStaffSectionService;
        private readonly IService<ExportAllStudentMetricsForStaffCohortRequest, StudentExportAllModel> exportAllStudentMetricsForStaffCohortService;
        private readonly IService<ExportAllStudentMetricsForCustomStudentListRequest, StudentExportAllModel> exportAllStudentMetricsForCustomStudentListService;
        private readonly IService<ExportAllStudentsForStaffAssessmentsRequest, StudentExportAllModel> exportAllStudentsForStaffAssessmentsService;

        public ExportAllMetricsService(
            IService<ExportAllStudentsForStaffsSectionsAndCohortsRequest, StudentExportAllModel> exportAllStudentsForStaffsSectionsAndCohortsService,
            IService<ExportAllStudentMetricsForStaffSectionRequest, StudentExportAllModel> exportAllStudentMetricsForStaffSectionService,
            IService<ExportAllStudentMetricsForStaffCohortRequest, StudentExportAllModel> exportAllStudentMetricsForStaffCohortService,
            IService<ExportAllStudentMetricsForCustomStudentListRequest, StudentExportAllModel> exportAllStudentMetricsForCustomStudentListService,
            IService<ExportAllStudentsForStaffAssessmentsRequest, StudentExportAllModel> exportAllStudentsForStaffAssessmentsService)
        {
            this.exportAllStudentsForStaffsSectionsAndCohortsService = exportAllStudentsForStaffsSectionsAndCohortsService;
            this.exportAllStudentMetricsForStaffSectionService = exportAllStudentMetricsForStaffSectionService;
            this.exportAllStudentMetricsForStaffCohortService = exportAllStudentMetricsForStaffCohortService;
            this.exportAllStudentMetricsForCustomStudentListService = exportAllStudentMetricsForCustomStudentListService;
            this.exportAllStudentsForStaffAssessmentsService = exportAllStudentsForStaffAssessmentsService;
        }

        [CanBeAuthorizedBy(EdFiClaimTypes.ViewAllStudents, EdFiClaimTypes.ViewMyStudents)]
        public StudentExportAllModel Get(ExportAllMetricsRequest request)
        {
            long staffUSI = request.StaffUSI;
            int schoolId = request.SchoolId;
            string studentListType = request.StudentListType;
            long sectionOrCohortId = request.SectionOrCohortId;

            if (request.AssessmentSubType == StaffModel.AssessmentSubType.StateStandardized.ToString() || 
                request.AssessmentSubType == StaffModel.AssessmentSubType.Benchmark.ToString() || 
                request.AssessmentSubType == StaffModel.AssessmentSubType.Reading.ToString())
            {
                return exportAllStudentsForStaffAssessmentsService.Get(
                            ExportAllStudentsForStaffAssessmentsRequest.Create(staffUSI, schoolId, studentListType, sectionOrCohortId, request.AssessmentSubType, request.SubjectArea));
            }

            var slt = GetSection(staffUSI, schoolId, studentListType, ref sectionOrCohortId);

            switch (slt)
            {
                case StudentListType.All:
                    return exportAllStudentsForStaffsSectionsAndCohortsService.Get(
                            ExportAllStudentsForStaffsSectionsAndCohortsRequest.Create(staffUSI, schoolId));

                case StudentListType.Cohort:
                    return exportAllStudentMetricsForStaffCohortService.Get(
                            ExportAllStudentMetricsForStaffCohortRequest.Create(staffUSI, schoolId, sectionOrCohortId));

                case StudentListType.Section:
                    return exportAllStudentMetricsForStaffSectionService.Get(
                            ExportAllStudentMetricsForStaffSectionRequest.Create(staffUSI, schoolId, sectionOrCohortId));

                case StudentListType.CustomStudentList:
                    return exportAllStudentMetricsForCustomStudentListService.Get(
                            ExportAllStudentMetricsForCustomStudentListRequest.Create(staffUSI, schoolId, studentListType, sectionOrCohortId));
            }

            //If we are still here then there is some kind of error.
            throw new InvalidOperationException(
                string.Format(
                    "Can not choose what model to export. Parameters are: staff USI({0}), school Id({1}), student List Type({2}), section Id({3})",
                    staffUSI, schoolId, studentListType, sectionOrCohortId));
        }
    }
}
