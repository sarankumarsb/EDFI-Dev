// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System.Linq;
using EdFi.Dashboards.Common;
using EdFi.Dashboards.Data.Entities;
using EdFi.Dashboards.Data.Repository;
using EdFi.Dashboards.Resources.Models.School;
using EdFi.Dashboards.Resources.Security.Common;
using EdFi.Dashboards.SecurityTokenService.Authentication;

namespace EdFi.Dashboards.Resources.School
{
    //Interface
    public class SchoolRequest
    {
        public int SchoolId { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SchoolRequest"/> using the specified parameters.
        /// </summary>
        /// <returns>A new <see cref="SchoolRequest"/> instance.</returns>
        public static SchoolRequest Create(int schoolId) 
        {
            return new SchoolRequest { SchoolId = schoolId };
        }
    }

    //Resource Model
    

    //Implementation
    public interface ISchoolService : IService<SchoolRequest, SchoolModel> { }

    public class SchoolService : ISchoolService
    {
        private readonly IRepository<SchoolInformation> repository;

        public SchoolService(IRepository<SchoolInformation> repository)
        {
            this.repository = repository;
        }

        [CanBeAuthorizedBy(EdFiClaimTypes.ViewAllMetrics, EdFiClaimTypes.ViewMyMetrics)]
        public SchoolModel Get(SchoolRequest request)
        {
            var schoolData = repository.GetAll().SingleOrDefault(x => x.SchoolId == request.SchoolId);
            if(schoolData==null)
                return null;

            var model = new SchoolModel
                            {
                                 SchoolId = schoolData.SchoolId,
                                 Name = schoolData.Name,
                                 SchoolCategory = schoolData.SchoolCategory,
                                 AddressLine1 = schoolData.AddressLine1,
                                 AddressLine2 = schoolData.AddressLine2,
                                 AddressLine3 = schoolData.AddressLine3,
                                 City = schoolData.City,
                                 State = schoolData.State,
                                 ZipCode = schoolData.ZipCode,
                                 TelephoneNumber = schoolData.TelephoneNumber,
                                 FaxNumber = schoolData.FaxNumber,
                                 ProfileThumbnail = schoolData.ProfileThumbnail,
                                 WebSite = schoolData.WebSite,
                            };

            return model;
        }
    }
}
