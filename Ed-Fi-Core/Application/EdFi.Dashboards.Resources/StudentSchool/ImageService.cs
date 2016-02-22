// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using EdFi.Dashboards.Common;
using EdFi.Dashboards.Infrastructure.Implementations;
using EdFi.Dashboards.Infrastructure.Implementations.Caching;
using EdFi.Dashboards.Resources.Images;
using EdFi.Dashboards.Resources.Images.ContentProvider;
using EdFi.Dashboards.Resources.Models.Common;
using EdFi.Dashboards.Resources.Security.Common;
using EdFi.Dashboards.SecurityTokenService.Authentication;

namespace EdFi.Dashboards.Resources.StudentSchool
{
    public class ImageRequest
    {
        public int LocalEducationAgencyId { get; set; }
        public int SchoolId { get; set; }
        public long StudentUSI { get; set; }

        [AuthenticationIgnore("Used to select the type of image to display")]
        public string DisplayType { get; set; }

        [AuthenticationIgnore("Used to select random images")]
        public string Gender { get; set; }

        public static ImageRequest Create(int localEducationAgencyId, int schoolId, long studentUSI, string displayType, string gender)
        {
            return new ImageRequest { LocalEducationAgencyId = localEducationAgencyId, SchoolId = schoolId, StudentUSI = studentUSI, DisplayType = displayType, Gender = gender };
        }
    }

    public interface IImageService : IService<ImageRequest, ImageModel> { }

    public class ImageService : IImageService
    {
        private readonly IImageContentProvider imageContentProvider;

        public ImageService(IImageContentProvider imageContentProvider)
        {
            this.imageContentProvider = imageContentProvider;
        }

        [NoCache][AuthenticationIgnore("This service returns the student image.")]
        public ImageModel Get(ImageRequest request)
        {
            var imageRequest = new StudentSchoolImageRequest
                                   {
                                       DisplayType = request.DisplayType, 
                                       LocalEducationAgencyId = request.LocalEducationAgencyId, 
                                       SchoolId = request.SchoolId, 
                                       StudentUSI = request.StudentUSI,
                                       Gender = request.Gender
                                   };
            return imageContentProvider.GetImage(imageRequest);
        }
    }
}
