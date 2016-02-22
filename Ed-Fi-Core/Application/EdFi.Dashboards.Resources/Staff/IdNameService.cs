// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
//using System.Linq;
//using EdFi.Dashboards.Data.Entities;
//using EdFi.Dashboards.Data.Repository;
//using EdFi.Dashboards.Resources.Models.Staff;
//using EdFi.Dashboards.Resources.Security.Common;
//using EdFi.Dashboards.SecurityTokenService.Authentication;

//namespace EdFi.Dashboards.Resources.Staff
//{
//    public class IdNameRequest
//    {
//        public long StaffUSI { get; set; }

//        public static IdNameRequest Create(long staffUSI)
//        {
//            return new IdNameRequest { StaffUSI = staffUSI };
//        }
//    }

//    public interface IIdNameService : IService<IdNameRequest, IdNameModel> { }

//    public class IdNameService : IIdNameService
//    {
//        private readonly IRepository<StaffInformation> staffInformationRepository;

//        public IdNameService(IRepository<StaffInformation> staffInformationRepository)
//        {
//            this.staffInformationRepository = staffInformationRepository;
//        }

//        [CanBeAuthorizedBy(EdFiClaimTypes.AccessOrganization)]
//        public IdNameModel Get(IdNameRequest request)
//        {
//            var model =
//                (from staff in staffInformationRepository.GetAll()
//                 where staff.StaffUSI == request.StaffUSI
//                 select new IdNameModel
//                 {
//                     StaffUSI = staff.StaffUSI,
//                     FullName = staff.FullName,
//                 })
//                .SingleOrDefault();

//            return model;
//        }
//    }
//}
