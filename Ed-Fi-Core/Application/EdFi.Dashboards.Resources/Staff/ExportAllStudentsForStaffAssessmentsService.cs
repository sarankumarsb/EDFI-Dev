using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using EdFi.Dashboards.Common;
using EdFi.Dashboards.Resources.Models.Common;
using EdFi.Dashboards.Resources.Models.Staff;
using EdFi.Dashboards.Resources.Models.Student;
using EdFi.Dashboards.Resources.Security.Common;
using EdFi.Dashboards.SecurityTokenService.Authentication;

namespace EdFi.Dashboards.Resources.Staff
{
    public class ExportAllStudentsForStaffAssessmentsRequest
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
        /// Initializes a new instance of the <see cref="ExportAllStudentsForStaffAssessmentsRequest"/> using the specified parameters.
        /// </summary>
        /// <returns>A new <see cref="ExportAllStudentsForStaffAssessmentsRequest"/> instance.</returns>
        public static ExportAllStudentsForStaffAssessmentsRequest Create(long staffUSI, int schoolId, string studentListType, long sectionOrCohortId, string assessmentSubType, string subjectArea)
        {
            return new ExportAllStudentsForStaffAssessmentsRequest
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

    public interface IExportAllStudentsForStaffAssessmentsService : IService<ExportAllStudentsForStaffAssessmentsRequest, StudentExportAllModel> { }

    public class ExportAllStudentsForStaffAssessmentsService : IExportAllStudentsForStaffAssessmentsService
    {
        private const string FormattedOfValueRegEx = @"[0-9]+\s(of)\s[0-9]+";
        private const string FormattedRatioValueRegEx = @"[0-9]+/[0-9]+";
        private readonly IAssessmentDetailsService _assessmentDetailsService;

        public ExportAllStudentsForStaffAssessmentsService(IAssessmentDetailsService assessmentDetailsService)
        {
            _assessmentDetailsService = assessmentDetailsService;
        }

        [CanBeAuthorizedBy(EdFiClaimTypes.ViewAllStudents, EdFiClaimTypes.ViewMyStudents)]
        public StudentExportAllModel Get(ExportAllStudentsForStaffAssessmentsRequest request)
        {
            var assessmentDetailsModel = _assessmentDetailsService.Get(new AssessmentDetailsRequest()
                                                                          {
                                                                              StaffUSI = request.StaffUSI,
                                                                              SchoolId = request.SchoolId,
                                                                              SectionOrCohortId = request.SectionOrCohortId,
                                                                              StudentListType = request.StudentListType,
                                                                              MetricAssessmentArea = request.SubjectArea,
                                                                              AssessmentSubType = request.AssessmentSubType
                                                                          });

            var rows = new List<StudentExportAllModel.Row>();

            //Get the fixed row
            rows.Add(GetFixedRow(assessmentDetailsModel, request.AssessmentSubType, request.SubjectArea));
            
            //loop through each student and loop through each objective
            foreach (var student in assessmentDetailsModel.Students)
            {
                var rowCells = new List<KeyValuePair<string, object>>
                                        {
                                            new KeyValuePair<string, object>("Student Name", student.Name),
                                            new KeyValuePair<string, object>("Grade Level", student.GradeLevelDisplayValue)
                                        };

                if (request.AssessmentSubType == StaffModel.AssessmentSubType.StateStandardized.ToString())
                {
                    rowCells.Add(new KeyValuePair<string, object>(request.SubjectArea, student.Score.DisplayValue));
                }

                if (request.AssessmentSubType == StaffModel.AssessmentSubType.Benchmark.ToString())
                {
                    rowCells.Add(new KeyValuePair<string, object>(assessmentDetailsModel.MetricTitle, student.Score.DisplayValue));
                }

                foreach (var metric in student.Metrics)
                {
                    var objectiveName = ((StudentWithMetricsAndAssessments.AssessmentMetric)(metric)).ObjectiveName;
                    var objectiveTitle = assessmentDetailsModel.ObjectiveTitles.Where(objTitle => objTitle.Title == objectiveName).First();

                    rowCells.Add(new KeyValuePair<string, object>(GetObjectiveTitleColumnName(objectiveTitle, request.AssessmentSubType), GetFormattedValue(metric.DisplayValue)));
                }  

                rows.Add(new StudentExportAllModel.Row
                               {
                                   Cells = rowCells,
                                   StudentUSI = student.StudentUSI
                               });
            }

            return new StudentExportAllModel { Rows = rows };
        }

        private static StudentExportAllModel.Row GetFixedRow(AssessmentDetailsModel assessmentDetailsModel, string assessmentSubType, string subjectArea)
        {
            var fixedRowCells = new List<KeyValuePair<string, object>>
                                        {
                                            new KeyValuePair<string, object>("Student Name", "% Students Mastering Objective"),
                                            new KeyValuePair<string, object>("Grade Level", string.Empty)
                                        };

            if (assessmentSubType == StaffModel.AssessmentSubType.StateStandardized.ToString())
            {
                fixedRowCells.Add(new KeyValuePair<string, object>(subjectArea, string.Empty));
            }

            if (assessmentSubType == StaffModel.AssessmentSubType.Benchmark.ToString())
            {
                fixedRowCells.Add(new KeyValuePair<string, object>(assessmentDetailsModel.MetricTitle, string.Empty));
            }

            foreach (var ot in assessmentDetailsModel.ObjectiveTitles)
            {
                fixedRowCells.Add(new KeyValuePair<string, object>(GetObjectiveTitleColumnName(ot, assessmentSubType), GetFormattedValue(ot.Mastery)));
            }

            return new StudentExportAllModel.Row
                       {
                           Cells = fixedRowCells,
                           StudentUSI = -1
                       };
        }

        private static string GetObjectiveTitleColumnName(AssessmentDetailsModel.ObjectiveTitle objectiveTitle, string assessmentSubType)
        {
            return assessmentSubType == StaffModel.AssessmentSubType.Benchmark.ToString() ? objectiveTitle.Description : objectiveTitle.Title;
        }

        private static string GetFormattedValue(string value)
        {
            if (Regex.IsMatch(value, FormattedOfValueRegEx))
                return GetDecimalFromFraction(string.Format(" {0}", value.Replace(" of ", "/")));

            if (Regex.IsMatch(value, FormattedRatioValueRegEx))
                return GetDecimalFromFraction(string.Format(" {0}", value));

            return value;
        }

        private static string GetDecimalFromFraction(string fraction)
        {
            var numerator = Convert.ToDecimal(fraction.Substring(0, fraction.IndexOf("/")));
            var denominator = Convert.ToDecimal(fraction.Substring(fraction.IndexOf("/") + 1));

            return Math.Round((numerator/denominator), 2).ToString();
        }
    }
}
