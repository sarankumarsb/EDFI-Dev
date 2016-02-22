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

namespace EdFi.Dashboards.Resources.Staff
{
    public class ImageRequest
    {
        public int SchoolId { get; set; }
        public long StaffUSI { get; set; }

        [AuthenticationIgnore("Used to select the type of image to display")]
        public string DisplayType { get; set; }

        [AuthenticationIgnore("Used to select random images")]
        public string Gender { get; set; }

        public static ImageRequest Create(int schoolId, long staffUSI, string displayType, string gender)
        {
            return new ImageRequest { SchoolId = schoolId, StaffUSI = staffUSI, DisplayType = displayType, Gender = gender};
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

        [NoCache][AuthenticationIgnore("This service returns the staff image.")]
        public ImageModel Get(ImageRequest request)
        {
            var imageRequest = new StaffSchoolImageRequest
                                   {
                                       DisplayType = request.DisplayType, 
                                       SchoolId = request.SchoolId,
                                       StaffUSI = request.StaffUSI,
                                       Gender = request.Gender
                                   };
            return imageContentProvider.GetImage(imageRequest);
        }
    }
}
