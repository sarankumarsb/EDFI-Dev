using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EdFi.Dashboards.Application.Data.Entities;
using EdFi.Dashboards.Data.Entities;
using EdFi.Dashboards.Data.Repository;

namespace EdFi.Dashboards.Application.Resources.Admin
{
    public abstract class TitleClaimSetServiceBase
    {
        public IRepository<StaffEducationOrgInformation> StaffEdOrgInfoRepo { get; set; }
        public IPersistingRepository<ClaimSetMapping> ClaimSetMappingRepo { get; set; }

        protected IDictionary<string, string> GetUniquePositionTitles()
        {
            //Get all the different position titles
            var staffEdOrgPositionTitles = new Dictionary<string, string>();
            foreach (var staffEducationOrgInformation in StaffEdOrgInfoRepo.GetAll().ToList())
            {
                if (!staffEdOrgPositionTitles.ContainsKey(staffEducationOrgInformation.PositionTitle.Trim().ToUpper()))
                    staffEdOrgPositionTitles.Add(staffEducationOrgInformation.PositionTitle.Trim().ToUpper(), staffEducationOrgInformation.PositionTitle.Trim().ToUpper());
            }

            return staffEdOrgPositionTitles;
        }

        protected IList<ClaimSetMapping> GetClaimSetMappings(int localEducationAgencyId)
        {
            return ClaimSetMappingRepo.GetAll().Where(map => map.LocalEducationAgencyId == localEducationAgencyId).ToList();
        }
            
    }
}
