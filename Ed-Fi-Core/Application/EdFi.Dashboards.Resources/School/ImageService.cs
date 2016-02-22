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

namespace EdFi.Dashboards.Resources.School
{
    public class ImageRequest
    {
        public int SchoolId { get; set; }

        [AuthenticationIgnore("Used to select the type of image to display")]
        public string DisplayType { get; set; }

        public static ImageRequest Create(int schoolId, string displayType)
        {
            return new ImageRequest { SchoolId = schoolId, DisplayType = displayType };
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

        [NoCache][AuthenticationIgnore("This service returns the school image.")]
        public ImageModel Get(ImageRequest request)
        {
            var imageRequest = new SchoolImageRequest { DisplayType = request.DisplayType, SchoolId = request.SchoolId };
            return imageContentProvider.GetImage(imageRequest);
        }
    }
}
