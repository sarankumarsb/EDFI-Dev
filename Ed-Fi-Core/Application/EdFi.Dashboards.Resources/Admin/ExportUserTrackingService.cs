// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System.Collections.Generic;
using System.Linq;
using EdFi.Dashboards.Common;
using EdFi.Dashboards.Data.Entities;
using EdFi.Dashboards.Data.Repository;
using EdFi.Dashboards.Resources.Models.Admin;
using EdFi.Dashboards.Resources.Models.Common;
using EdFi.Dashboards.Resources.Security.Common;
using EdFi.Dashboards.SecurityTokenService.Authentication;

namespace EdFi.Dashboards.Resources.Admin
{
    public class ExportUserTrackingRequest
    {
        public int LocalEducationAgencyId { get; set; }

        [AuthenticationIgnore("LocalEducationAgency is a code corresponding to the LocalEducationAgencyId, and does not need to be authorized separately.")]
        public string LocalEducationAgency { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExportUserTrackingRequest"/> using the specified parameters.
        /// </summary>
        /// <returns>A new <see cref="ExportUserTrackingRequest"/> instance.</returns>
        public static ExportUserTrackingRequest Create(int localEducationAgencyId, string localEducationAgency)
        {
            return new ExportUserTrackingRequest { LocalEducationAgencyId = localEducationAgencyId, LocalEducationAgency = localEducationAgency };
        }
    }

    public interface IExportUserTrackingService : IService<ExportUserTrackingRequest, ExportAllModel> { }

    public class ExportUserTrackingService : IExportUserTrackingService
    {
        private readonly IRepository<StaffInformation> staffInformationRepository;

        public ExportUserTrackingService(IRepository<StaffInformation> staffInformationRepository)
        {
            this.staffInformationRepository = staffInformationRepository;
        }

        [CanBeAuthorizedBy(EdFiClaimTypes.AdministerDashboard)]
        public ExportAllModel Get(ExportUserTrackingRequest request)
        {
            //Subsonic's restrictions of doing projections...
            var staffData = (from s in staffInformationRepository.GetAll()
                             orderby s.StaffUSI
                             select new
                             {
                                 s.StaffUSI,
                                 Name = Utilities.FormatPersonNameByLastName(s.FirstName, s.MiddleName, s.LastSurname),
                                 s.FirstName,
                                 s.LastSurname,
                                 s.EmailAddress
                             }).ToList();

            var model = new ExportAllModel
                             {
                                 Rows = from s in staffData
                                         select new ExportAllModel.Row
                                                    {
                                                      Cells = new List<KeyValuePair<string, object>>
                                                                  {
                                                                      new KeyValuePair<string, object>("TrackingCode", ("rizixere" + s.StaffUSI + request.LocalEducationAgencyId).GetHashCode().ToString()),
                                                                      new KeyValuePair<string, object>("Staff USI", s.StaffUSI),
                                                                      new KeyValuePair<string, object>("Full Name", s.Name),
                                                                      new KeyValuePair<string, object>("First Name", s.FirstName),
                                                                      new KeyValuePair<string, object>("Last Name", s.LastSurname),
                                                                      new KeyValuePair<string, object>("Email Address", s.EmailAddress),
                                                                      new KeyValuePair<string, object>("LEA Code", request.LocalEducationAgency)
                                                                  }.ToList()
                                                    }
                             };
            model.Rows.ToList();
            return model;
        }
    }
}
