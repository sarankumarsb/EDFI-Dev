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
    public class LearningObjectivesExportController : Controller
    {
        private ILearningObjectiveService _service;

        public LearningObjectivesExportController(ILearningObjectiveService service)
        {
            _service = service;
        }

        [CanBeAuthorizedBy(EdFiClaimTypes.ViewAllStudents, EdFiClaimTypes.ViewMyStudents)]
        public CsvResult Get(EdFiDashboardContext context)
        {
            var learningObjectives = _service.Get(new LearningObjectiveRequest()
            {
                StudentUSI = context.StudentUSI.GetValueOrDefault(),
                SchoolId = context.SchoolId.GetValueOrDefault(),
                MetricVariantId = context.MetricVariantId.GetValueOrDefault()
            });

            var orderedSkills = learningObjectives.LearningObjectiveSkills.OrderBy(los => los.SectionName).ThenBy(los => los.SkillName);

            var export = new ExportAllModel();
            var rows = new List<ExportAllModel.Row>();

            foreach (var skill in orderedSkills)
            {
                var cells = new List<KeyValuePair<string, object>>()
                                {
                                    new KeyValuePair<string, object>("Section Name", skill.SectionName),
                                    new KeyValuePair<string, object>("Skill Name", skill.SkillName),
                                };
                
                cells.AddRange(skill.SkillValues.Select(skillValue => new KeyValuePair<string, object>(skillValue.Title, skillValue.Value)));

                rows.Add(new ExportAllModel.Row() { Cells = cells });
            }

            export.Rows = rows;

            return new CsvResult(export);
        }
    }
}
