// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System.Collections.Generic;
using System.Linq;
using System.Web;
using EdFi.Dashboards.Common;
using EdFi.Dashboards.Data.Entities;
using EdFi.Dashboards.Data.Repository;
using EdFi.Dashboards.Infrastructure.Implementations.Caching;
using EdFi.Dashboards.Resources.Application;
using EdFi.Dashboards.Resources.Models.Admin;
using EdFi.Dashboards.Resources.Models.School;
using EdFi.Dashboards.Resources.Photo;
using EdFi.Dashboards.Resources.Security.Common;
using EdFi.Dashboards.SecurityTokenService.Authentication;

namespace EdFi.Dashboards.Resources.Admin
{
    public class PhotoManagementRequest
    {
        public int LocalEducationAgencyId { get; set; }
    }

    public class PhotoManagementPostRequest
    {
        //Not named SchoolId because I don't want to trip off the built in security
        [AuthenticationIgnore("This is data suppled by the user.  They are still required to be authed at the district level.")]
        public int SchoolIdToLoadImagesTo { get; set; }

        public int LocalEducationAgencyId { get; set; }

        [AuthenticationIgnore("This is data supplied by the user.")]
        public object File { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PhotoManagementRequest"/> using the specified parameters.
        /// </summary>
        /// <returns>A new <see cref="PhotoManagementRequest"/> instance.</returns>
        public static PhotoManagementPostRequest Create(int schoolIdToLoadImagesTo, int localEducationAgencyId)
        {
            return new PhotoManagementPostRequest { SchoolIdToLoadImagesTo = schoolIdToLoadImagesTo, LocalEducationAgencyId = localEducationAgencyId };
        }
    }

    public interface IPhotoManagementService : IService<PhotoManagementRequest, PhotoManagementModel>, IPostHandler<PhotoManagementPostRequest, PhotoManagementModel> { }

    public class PhotoManagementService : IPhotoManagementService
    {
        private readonly IRepository<SchoolInformation> schoolInformationRepository;
        private readonly IPhotoProcessor photoProcessor;
        private readonly IErrorLoggingService errorLoggingService;

        public PhotoManagementService(IRepository<SchoolInformation> schoolInformationRepository, IPhotoProcessor photoProcessor, IErrorLoggingService errorLoggingService)
        {
            this.schoolInformationRepository = schoolInformationRepository;
            this.photoProcessor = photoProcessor;
            this.errorLoggingService = errorLoggingService;
        }

        [CanBeAuthorizedBy(EdFiClaimTypes.AdministerDashboard)]
        public PhotoManagementModel Get(PhotoManagementRequest request)
        {
            var localEducationAgencyId = request.LocalEducationAgencyId;
            var result = new PhotoManagementModel();
            var schools = schoolInformationRepository.GetAll().Where(x => x.LocalEducationAgencyId == localEducationAgencyId).ToList();

            result.Schools = schools.Select(s => new SchoolModel {SchoolId = s.SchoolId, Name = s.Name}).ToList();

            return result;
        }

        [NoCache]
        [CanBeAuthorizedBy(EdFiClaimTypes.AdministerDashboard)]
        public PhotoManagementModel Post(PhotoManagementPostRequest request)
        {
            var returnValue = Get(new PhotoManagementRequest { LocalEducationAgencyId = request.LocalEducationAgencyId });

            // Since we are bypassing the security, we have to make sure the school Id belongs to the current local education agency.
            if (returnValue.Schools.Any(school => school.SchoolId == request.SchoolIdToLoadImagesTo))
            {
                try
                {
                    var photoProcessorRequest = new PhotoProcessorRequest
                    {
                        LocalEducationAgencyId = request.LocalEducationAgencyId,
                        SchoolId = request.SchoolIdToLoadImagesTo,
                        FileBytes = (byte[])request.File
                    };
                    var response = photoProcessor.Process(photoProcessorRequest);

                    returnValue.ErrorMessages = response.ErrorMessages;
                    returnValue.SuccessfullyProcessedPhotos = response.SuccessfullyProcessedPhotos;
                    returnValue.TotalRecords = response.TotalRecords;
                }
                catch (System.Exception exception)
                {
                    var errorLoggingServiceRequest = new ErrorLoggingRequest
                    {
                        Exception = exception,
                        Request = new HttpRequestWrapper(HttpContext.Current.Request),
                        UserName = HttpContext.Current.User.Identity.Name
                    };

                    errorLoggingService.Post(errorLoggingServiceRequest);
                    returnValue.ErrorMessages = new List<string> { "The file is not in a supported format." };
                    returnValue.SuccessfullyProcessedPhotos = 0;
                    returnValue.TotalRecords = 0;
                }
            }

            return returnValue;
        }
    }
}
