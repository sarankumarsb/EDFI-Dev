// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System;
using EdFi.Dashboards.Common;
using EdFi.Dashboards.Resources.Models.Common;

namespace EdFi.Dashboards.Resources.Images.ContentProvider
{
    public class GenericImageContentProviderChainLinks
    {
    }

    public class StudentGenericImageContentProvider : ChainOfResponsibilityBase<IImageContentProvider, ImageRequestBase, ImageModel>, IImageContentProvider
    {
        private readonly IFileBasedImageContentProvider provider;
        private const string defaultPersonImageFilePathFormat = "~/Core_Content/Images/Students/NoImage{0}.jpg";

        public StudentGenericImageContentProvider(IFileBasedImageContentProvider provider, IImageContentProvider next)
            : base(next)
        {
            this.provider = provider;
        }

        protected override bool CanHandleRequest(ImageRequestBase request)
        {
            return (request as StudentSchoolImageRequest != null);
        }

        protected override ImageModel HandleRequest(ImageRequestBase request)
        {
            var displayType = request.DisplayType ?? String.Empty;
            displayType = displayType.Trim();
            var defaultFilePath = string.Format(defaultPersonImageFilePathFormat, displayType);

            var res = provider.GetImageContent(defaultFilePath);

            return res;
        }

        public ImageModel GetImage(ImageRequestBase request)
        {
            //Delegate to the base...
            return ProcessRequest(request);
        }
    }

    public class StaffGenericImageContentProvider : ChainOfResponsibilityBase<IImageContentProvider, ImageRequestBase, ImageModel>, IImageContentProvider
    {
        private readonly IFileBasedImageContentProvider provider;
        private const string defaultPersonImageFilePathFormat = "~/Core_Content/Images/Students/NoImage{0}.jpg";

        public StaffGenericImageContentProvider(IFileBasedImageContentProvider provider, IImageContentProvider next)
            : base(next)
        {
            this.provider = provider;
        }

        protected override bool CanHandleRequest(ImageRequestBase request)
        {
            return (request as StaffSchoolImageRequest != null);
        }

        protected override ImageModel HandleRequest(ImageRequestBase request)
        {
            var displayType = request.DisplayType ?? String.Empty;
            displayType = displayType.Trim();
            var defaultFilePath = string.Format(defaultPersonImageFilePathFormat, displayType);

            var res = provider.GetImageContent(defaultFilePath);

            return res;
        }

        public ImageModel GetImage(ImageRequestBase request)
        {
            return ProcessRequest(request);
        }
    }

    public class SchoolGenericImageContentProvider : ChainOfResponsibilityBase<IImageContentProvider, ImageRequestBase, ImageModel>, IImageContentProvider
    {
        private readonly IFileBasedImageContentProvider provider;
        private const string defaultSchoolImageFilePathFormat = "~/Core_Content/Images/EducationOrganization/NoEducationOrganizationImage{0}.jpg";

        public SchoolGenericImageContentProvider(IFileBasedImageContentProvider provider, IImageContentProvider next)
            : base(next)
        {
            this.provider = provider;
        }

        protected override bool CanHandleRequest(ImageRequestBase request)
        {
            return (request as SchoolImageRequest != null);
        }

        protected override ImageModel HandleRequest(ImageRequestBase request)
        {
            var displayType = request.DisplayType ?? String.Empty;
            displayType = displayType.Trim();
            var defaultFilePath = string.Format(defaultSchoolImageFilePathFormat, displayType);

            var res = provider.GetImageContent(defaultFilePath);

            return res;
        }

        public ImageModel GetImage(ImageRequestBase request)
        {
            return ProcessRequest(request);
        }
    }

    public class LocalEducationAgencyGenericImageContentProvider : ChainOfResponsibilityBase<IImageContentProvider, ImageRequestBase, ImageModel>, IImageContentProvider
    {
        private readonly IFileBasedImageContentProvider provider;
        private const string defaultLeaImageFilePathFormat = "~/Core_Content/Images/EducationOrganization/NoEducationOrganizationImage{0}.jpg";

        public LocalEducationAgencyGenericImageContentProvider(IFileBasedImageContentProvider provider, IImageContentProvider next)
            : base(next)
        {
            this.provider = provider;
        }

        protected override bool CanHandleRequest(ImageRequestBase request)
        {
            return (request as LocalEducationAgencyImageRequest != null);
        }

        protected override ImageModel HandleRequest(ImageRequestBase request)
        {
            var displayType = request.DisplayType ?? String.Empty;
            displayType = displayType.Trim();
            var defaultFilePath = string.Format(defaultLeaImageFilePathFormat, displayType);

            var res = provider.GetImageContent(defaultFilePath);

            return res;
        }

        public ImageModel GetImage(ImageRequestBase request)
        {
            return ProcessRequest(request);
        }
    }
}
