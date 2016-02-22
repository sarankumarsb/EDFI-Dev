using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EdFi.Dashboards.Resources.Models.Staff;

namespace EdFi.Dashboards.Presentation.Core.Areas.Staff.Models
{
    public class StaffMenuModel : StaffModel
    {
        public StaffMenuModel(StaffModel staffModel)
        {
            StaffUSI = staffModel.StaffUSI;
            FullName = staffModel.FullName;
            ProfileThumbnail = staffModel.ProfileThumbnail;
            CurrentSchool = staffModel.CurrentSchool;
            Schools = staffModel.Schools;
            CurrentSection = staffModel.CurrentSection;
            SectionsAndCohorts = staffModel.SectionsAndCohorts;
            Views = staffModel.Views;
        }

        public string AssessmentSubTypeValue { get; set; }
    }
}
