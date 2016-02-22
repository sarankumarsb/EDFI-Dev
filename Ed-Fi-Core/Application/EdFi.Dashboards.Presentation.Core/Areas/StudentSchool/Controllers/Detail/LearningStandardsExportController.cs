using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using EdFi.Dashboards.Core.Providers.Context;
using EdFi.Dashboards.Presentation.Architecture.Mvc.ActionResults;
using EdFi.Dashboards.Resources.Models.Common;
using EdFi.Dashboards.Resources.Security.Common;
using EdFi.Dashboards.Resources.StudentSchool.Detail;
using EdFi.Dashboards.SecurityTokenService.Authentication;

namespace EdFi.Dashboards.Presentation.Core.Areas.StudentSchool.Controllers.Detail
{
    public class LearningStandardsExportController : Controller
    {
        private ILearningStandardService _service;

        public LearningStandardsExportController(ILearningStandardService service)
        {
            _service = service;
        }

        [CanBeAuthorizedBy(EdFiClaimTypes.ViewAllStudents, EdFiClaimTypes.ViewMyStudents)]
        public CsvResult Get(EdFiDashboardContext context)
        {
            var learningStandards = _service.Get(new LearningStandardRequest()
            {
                StudentUSI = context.StudentUSI.GetValueOrDefault(),
                SchoolId = context.SchoolId.GetValueOrDefault(),
                MetricVariantId = context.MetricVariantId.GetValueOrDefault()
            });

            var export = new ExportAllModel();
            var rows = new List<ExportAllModel.Row>();
            
            foreach (var learningStandard in learningStandards)
            {
                var cells = new List<KeyValuePair<string, object>>()
                                {
                                    new KeyValuePair<string, object>("Learning Standard", learningStandard.LearningStandard),
                                    new KeyValuePair<string, object>("Description", learningStandard.Description),
                                };
                
                cells.AddRange(learningStandard.Assessments.Select(assessment => new KeyValuePair<string, object>(assessment.AssessmentTitle, assessment.Value)));

                rows.Add(new ExportAllModel.Row() { Cells = cells });
            }

            export.Rows = rows;

            return new CsvResult(export);
        }
    }
}
