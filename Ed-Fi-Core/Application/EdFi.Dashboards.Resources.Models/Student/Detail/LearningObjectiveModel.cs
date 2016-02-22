using System;
using System.Collections.Generic;

namespace EdFi.Dashboards.Resources.Models.Student.Detail
{
    [Serializable]
    public class LearningObjectiveModel : IStudent
    {
        public LearningObjectiveModel() 
        {
            LearningObjectiveSkills = new List<LearningObjectiveSkill>();
            AssessmentTitles = new List<string>();
        }
        
        public LearningObjectiveModel(long studentUSI) : this()
        {
            StudentUSI = studentUSI;
        }

        public long StudentUSI { get; set; }
        public int SchoolId { get; set; }
        public int MetricId { get; set; }
        public string InventoryName { get; set; }
        public IList<string> AssessmentTitles { get; set; }
        public IEnumerable<LearningObjectiveSkill> LearningObjectiveSkills { get; set; }

        [Serializable]
        public class LearningObjectiveSkill : IStudent
        {
            public LearningObjectiveSkill()
            {
                SkillValues = new List<SkillValue>();
            }
            
            public LearningObjectiveSkill(long studentUSI) : this()
            {
                StudentUSI = studentUSI;
            }

            public long StudentUSI { get; set; }
            public string SectionName { get; set; }
            public string SkillName { get; set; }
            public IEnumerable<SkillValue> SkillValues { get; set; }
        }

        [Serializable]
        public class SkillValue : IStudent
        {
            public SkillValue() {}
            
            public SkillValue(long studentUSI)
            {
                StudentUSI = studentUSI;
            }

            public long StudentUSI { get; set; }
            public string Value { get; set; }
            public int? MetricStateTypeId { get; set; }
            public string Title { get; set; }
        }
    }
}
