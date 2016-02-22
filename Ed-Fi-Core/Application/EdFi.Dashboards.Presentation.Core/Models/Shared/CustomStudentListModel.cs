using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EdFi.Dashboards.Presentation.Core.Models.Shared
{
    public class CustomStudentListModel
    {
        public string ControlId { get; set; }
        public bool IsCustomStudentList { get; set; }
        public string LinkParentIdentifier { get; set; }
        public string CheckboxParentIentifier { get; set; }
        public string SelectAllCheckboxParentIentifier { get; set; }
        public long? CustomStudentListId { get; set; }
        public int LocalEducationAgencyId { get; set; }
        public int? SchoolId { get; set; }
        public long StaffUSI { get; set; }
        public string CustomStudentListUrl { get; set; }
        public int? UniqueId { get; set; }
    }
}
