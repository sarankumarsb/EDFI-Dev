using System;
using System.Collections.Generic;

namespace EdFi.Dashboards.Application.Resources.Models.Staff
{
    /// <summary>
    /// Represents a staff member's custom student list
    /// </summary>
    [Serializable]
    public class CustomStudentListModel
    {
        public long StaffUSI { get; set; }
        public int CustomStudentListId { get; set; }
        public int EducationOrganizationId { get; set; }
        public string Description { get; set; }
        public string Href { get; set; }
    }
}
