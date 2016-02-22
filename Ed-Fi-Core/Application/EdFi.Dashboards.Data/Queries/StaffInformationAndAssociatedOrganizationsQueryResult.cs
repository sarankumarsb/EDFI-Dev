using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EdFi.Dashboards.Core;

namespace EdFi.Dashboards.Data.Queries
{
    public class StaffInformationAndAssociatedOrganizationsQueryResult
    {
        //Property names need to match StaffInfo if possible
        public string FullName { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastSurname { get; set; }
        public long StaffUSI { get; set; }
        public string EmailAddress { get; set; }
        public int? EducationOrganizationId { get; set; }
        public string EducationOrganizationName { get; set; }
        public string OrganizationCategory { get; set; }
        public string StaffCategory { get; set; }
        public string PositionTitle { get; set; }
        public string IdentificationSystem { get; set; }
        public string IdentificationCode { get; set; }
        public int UserType { get; set; } // EDFIDB-139

        public IEnumerable<EducationOrganization> AssociatedOrganizations { get; set; }

        public class EducationOrganization
        {
            public int EducationOrganizationId { get; set; }
            public EducationOrganizationCategory OrganizationCategory { get; set; }
            public string Name { get; set; }

            public string StaffCategory { get; set; }
            public string PositionTitle { get; set; }

            public int? LocalEducationAgencyId { get; set; }
        }
    }
}
