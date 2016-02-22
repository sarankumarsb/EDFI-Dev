// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System.Linq;
using EdFi.Dashboards.Common;
using EdFi.Dashboards.Data.Entities;
using EdFi.Dashboards.Data.Repository;
using EdFi.Dashboards.Resources.Models.Admin;
using EdFi.Dashboards.Resources.Security.Common;
using EdFi.Dashboards.SecurityTokenService.Authentication;

namespace EdFi.Dashboards.Resources.Admin
{
    public class CanLogInAsUserRequest
    {
        public long StaffUSI { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CanLogInAsUserRequest"/> using the specified parameters.
        /// </summary>
        /// <returns>A new <see cref="CanLogInAsUserRequest"/> instance.</returns>
        public static CanLogInAsUserRequest Create(long staffUSI) 
		{
			return new CanLogInAsUserRequest { StaffUSI = staffUSI };
		}
	}

    public interface ICanLogInAsUserService : IService<CanLogInAsUserRequest, CanLogInAsUserModel> { }

    public class CanLogInAsUserService : ICanLogInAsUserService
    {
        private readonly IRepository<StaffInformation> staffInformationRepository;

        public CanLogInAsUserService(IRepository<StaffInformation> staffInformationRepository)
        {
            this.staffInformationRepository = staffInformationRepository;
        }

        [CanBeAuthorizedBy(EdFiClaimTypes.AdministerDashboard)]
        public CanLogInAsUserModel Get(CanLogInAsUserRequest request)
        {
            long staffUSI = request.StaffUSI;

            var result = new CanLogInAsUserModel {StaffUSI = staffUSI};

            var emails = from staff in staffInformationRepository.GetAll()
                         where staff.StaffUSI == staffUSI
                         select staff.EmailAddress;

            if (emails.Count() == 1)
            {
                result.Email = emails.FirstOrDefault();
                result.CanLogIn = true;
            }

            return result;
        }
    }
}
