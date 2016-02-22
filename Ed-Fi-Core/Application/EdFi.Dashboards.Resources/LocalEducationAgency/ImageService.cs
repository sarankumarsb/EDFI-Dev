// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EdFi.Dashboards.Common;
using EdFi.Dashboards.Infrastructure.Implementations;
using EdFi.Dashboards.Infrastructure.Implementations.Caching;
using EdFi.Dashboards.Resource.Models.Common;
using EdFi.Dashboards.Resources.Images;
using EdFi.Dashboards.Resources.Images.ContentProvider;
using EdFi.Dashboards.Resources.Models.Common;
using EdFi.Dashboards.Resources.Security.Common;
using EdFi.Dashboards.SecurityTokenService.Authentication;

namespace EdFi.Dashboards.Resources.LocalEducationAgency
{
    //Local Education Agency Request
    public class ImageRequest
    {
        public int LocalEducationAgencyId { get; set; }

        [AuthenticationIgnore("Used to select the type of image to display")]
        public string DisplayType { get; set; }

        public static ImageRequest Create(int localEducationAgencyId, string displayType)
        {
            return new ImageRequest {LocalEducationAgencyId = localEducationAgencyId, DisplayType = displayType};
        }
    }

    public interface IImageService : IService<ImageRequest, ImageModel>{}

    public class ImageService : IImageService
    {
        private readonly IImageContentProvider imageContentProvider;

        public ImageService(IImageContentProvider imageContentProvider)
        {
            this.imageContentProvider = imageContentProvider;
        }

        [NoCache][AuthenticationIgnore("This service returns the LEA image.")]
        public ImageModel Get(ImageRequest request)
        {
            var imageRequest = new LocalEducationAgencyImageRequest {DisplayType = request.DisplayType, LocalEducationAgencyId = request.LocalEducationAgencyId};
            return imageContentProvider.GetImage(imageRequest);
        }
    }
}
