using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EdFi.Dashboards.Application.Data.Entities;
using EdFi.Dashboards.Application.Resources.Models.Admin;
using EdFi.Dashboards.Common;
using EdFi.Dashboards.Data.Entities;
using EdFi.Dashboards.Data.Repository;
using EdFi.Dashboards.Resources;
using EdFi.Dashboards.Resources.Security.Common;
using EdFi.Dashboards.SecurityTokenService.Authentication;

namespace EdFi.Dashboards.Application.Resources.Admin
{
    public class ExportTitleClaimSetRequest
    {
        public int LocalEducationAgencyId { get; set; }

        public static ExportTitleClaimSetRequest Create(int localEducationAgencyId)
        {
            return new ExportTitleClaimSetRequest
                        {
                            LocalEducationAgencyId = localEducationAgencyId
                        };
        }
    }

    public interface IExportTitleClaimSetService : IService<ExportTitleClaimSetRequest, ExportTitleClaimSetModel>
    {
    }

    public class ExportTitleClaimSetService : TitleClaimSetServiceBase, IExportTitleClaimSetService
    {
        private readonly IRepository<StaffEducationOrgInformation> _staffEdOrgInfoRepo;
        private readonly IPersistingRepository<ClaimSetMapping> _claimSetMappingRepo;

        public ExportTitleClaimSetService(IRepository<StaffEducationOrgInformation> staffEdOrgInfoRepo, IPersistingRepository<ClaimSetMapping> claimSetMappingRepo)
        {
            _staffEdOrgInfoRepo = staffEdOrgInfoRepo;
            _claimSetMappingRepo = claimSetMappingRepo;
        }

        [CanBeAuthorizedBy(EdFiClaimTypes.AdministerDashboard)]
        public ExportTitleClaimSetModel Get(ExportTitleClaimSetRequest request)
        {
            //Get all the different position titles
            var staffEdOrgPositionTitles = GetUniquePositionTitles().OrderBy(kvp => kvp.Key).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            //Get a dictionary of current title ClaimSet mappings
            var claimSetMaps = GetClaimSetMappings(request.LocalEducationAgencyId).ToDictionary(ClaimSetMap => ClaimSetMap.PositionTitle.Trim().ToUpper(), ClaimSetMap => ClaimSetMap.ClaimSet);

            var rows = new List<ExportTitleClaimSetModel.Row>();

            //loop through each student and loop through each objective
            foreach (var positionTitle in staffEdOrgPositionTitles.Keys)
            {
                var rowCells = new List<KeyValuePair<string, object>>
                                        {
                                            new KeyValuePair<string, object>("Position Title", positionTitle),
                                            new KeyValuePair<string, object>("ClaimSet", claimSetMaps.ContainsKey(positionTitle) ? claimSetMaps[positionTitle] : string.Empty)
                                        };

                rows.Add(new ExportTitleClaimSetModel.Row
                            {
                                Cells = rowCells,
                            });
            }

            return new ExportTitleClaimSetModel { Rows = rows };
        }
    }
}
