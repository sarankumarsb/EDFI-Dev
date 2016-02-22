// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using EdFi.Dashboards.Common;
using EdFi.Dashboards.Resources.Models.Common;
using log4net;

namespace EdFi.Dashboards.Resources.Images.ContentProvider
{
    // It is tempting to attempt to refactor all of these into a base class with generics. The problem is that each request has a specific 
    // property for the Id (e.g. StudentUSI, StaffUSI) instead of just a property called Id. Because of this generics would only remove 
    // some of the duplicated code but not all. Given those issues, it doesn't look like generics are an option here.
    /// <summary>
    /// Serves up a student image from the file system.
    /// </summary>
    public class StudentFileSystemBasedImageContentProvider : ChainOfResponsibilityBase<IImageContentProvider, ImageRequestBase, ImageModel>, IImageContentProvider
    {
        private readonly IFileSystemBasedImagePathProvider fileSystemBasedImagePathProvider;
        private readonly IFileBasedImageContentProvider fileBasedImageContentProvider;
        private readonly ILog logger = LogManager.GetLogger("FileSystemLogger");
        private const string defaultImageFilePathFormat = "~/Core_Content/Images/Students/NoImage{0}.jpg";

        public StudentFileSystemBasedImageContentProvider(
            IFileSystemBasedImagePathProvider fileSystemBasedImagePathProvider,
            IFileBasedImageContentProvider fileBasedImageContentProvider,
            IImageContentProvider next)
            : base(next)
        {
            this.fileSystemBasedImagePathProvider = fileSystemBasedImagePathProvider;
            this.fileBasedImageContentProvider = fileBasedImageContentProvider;
        }

        protected override bool CanHandleRequest(ImageRequestBase request)
        {
            return (request as StudentSchoolImageRequest != null);
        }

        protected override ImageModel HandleRequest(ImageRequestBase request)
        {
            var displayType = request.DisplayType ?? string.Empty;
            var studentRequest = (StudentSchoolImageRequest)request;
            var profileImage = fileSystemBasedImagePathProvider.GetImagePath(studentRequest.StudentUSI, ImageType.Student, displayType);
            var defaultFilePath = string.Format(defaultImageFilePathFormat, displayType.Trim());
            var defaultImage = fileBasedImageContentProvider.GetImageContent(defaultFilePath);

            logger.Debug(string.Format("Diagnostics for image. Trying paths:{0}, {1}", profileImage, defaultImage));

            return fileBasedImageContentProvider.GetImageContent(profileImage) ?? defaultImage;
        }

        public ImageModel GetImage(ImageRequestBase request)
        {
            //Delegate to the base...
            return ProcessRequest(request);
        }
    }

    /// <summary>
    /// Serves up a staff image from the file system.
    /// </summary>
    public class StaffFileSystemBasedImageContentProvider : ChainOfResponsibilityBase<IImageContentProvider, ImageRequestBase, ImageModel>, IImageContentProvider
    {
        private readonly IFileSystemBasedImagePathProvider fileSystemBasedImagePathProvider;
        private readonly IFileBasedImageContentProvider fileBasedImageContentProvider;
        private readonly ILog logger = LogManager.GetLogger("FileSystemLogger");
        private const string defaultImageFilePathFormat = "~/Core_Content/Images/Students/NoImage{0}.jpg";

        public StaffFileSystemBasedImageContentProvider(
            IFileSystemBasedImagePathProvider fileSystemBasedImagePathProvider,
            IFileBasedImageContentProvider fileBasedImageContentProvider,
            IImageContentProvider next)
            : base(next)
        {
            this.fileSystemBasedImagePathProvider = fileSystemBasedImagePathProvider;
            this.fileBasedImageContentProvider = fileBasedImageContentProvider;
        }

        protected override bool CanHandleRequest(ImageRequestBase request)
        {
            return (request as StaffSchoolImageRequest != null);
        }

        protected override ImageModel HandleRequest(ImageRequestBase request)
        {
            var displayType = request.DisplayType ?? string.Empty;
            var staffRequest = (StaffSchoolImageRequest)request;
            var profileImage = fileSystemBasedImagePathProvider.GetImagePath(staffRequest.StaffUSI, ImageType.Staff, displayType);
            var defaultFilePath = string.Format(defaultImageFilePathFormat, displayType.Trim());
            var defaultImage = fileBasedImageContentProvider.GetImageContent(defaultFilePath);

            logger.Debug(string.Format("Diagnostics for image. Trying paths:{0}, {1}", profileImage, defaultImage));

            return fileBasedImageContentProvider.GetImageContent(profileImage) ?? defaultImage;
        }

        public ImageModel GetImage(ImageRequestBase request)
        {
            //Delegate to the base...
            return ProcessRequest(request);
        }
    }

    /// <summary>
    /// Serves up a school image from the file system.
    /// </summary>
    public class SchoolFileSystemBasedImageContentProvider : ChainOfResponsibilityBase<IImageContentProvider, ImageRequestBase, ImageModel>, IImageContentProvider
    {
        private readonly IFileSystemBasedImagePathProvider fileSystemBasedImagePathProvider;
        private readonly IFileBasedImageContentProvider fileBasedImageContentProvider;
        private readonly ILog logger = LogManager.GetLogger("FileSystemLogger");
        private const string defaultImageFilePathFormat = "~/Core_Content/Images/EducationOrganization/NoEducationOrganizationImage{0}.jpg";

        public SchoolFileSystemBasedImageContentProvider(
            IFileSystemBasedImagePathProvider fileSystemBasedImagePathProvider,
            IFileBasedImageContentProvider fileBasedImageContentProvider,
            IImageContentProvider next)
            : base(next)
        {
            this.fileSystemBasedImagePathProvider = fileSystemBasedImagePathProvider;
            this.fileBasedImageContentProvider = fileBasedImageContentProvider;
        }

        protected override bool CanHandleRequest(ImageRequestBase request)
        {
            return (request as SchoolImageRequest != null);
        }

        protected override ImageModel HandleRequest(ImageRequestBase request)
        {
            var displayType = request.DisplayType ?? string.Empty;
            var schoolRequest = (SchoolImageRequest)request;
            var profileImage = fileSystemBasedImagePathProvider.GetImagePath(schoolRequest.SchoolId, ImageType.School, displayType);
            var defaultFilePath = string.Format(defaultImageFilePathFormat, displayType.Trim());
            var defaultImage = fileBasedImageContentProvider.GetImageContent(defaultFilePath);

            logger.Debug(string.Format("Diagnostics for image. Trying paths:{0}, {1}", profileImage, defaultImage));

            return fileBasedImageContentProvider.GetImageContent(profileImage) ?? defaultImage;
        }

        public ImageModel GetImage(ImageRequestBase request)
        {
            //Delegate to the base...
            return ProcessRequest(request);
        }
    }

    /// <summary>
    /// Serves up a local education agency image from the file system.
    /// </summary>
    public class LocalEducationAgencyFileSystemBasedImageContentProvider : ChainOfResponsibilityBase<IImageContentProvider, ImageRequestBase, ImageModel>, IImageContentProvider
    {
        private readonly IFileSystemBasedImagePathProvider fileSystemBasedImagePathProvider;
        private readonly IFileBasedImageContentProvider fileBasedImageContentProvider;
        private readonly ILog logger = LogManager.GetLogger("FileSystemLogger");
        private const string defaultImageFilePathFormat = "~/Core_Content/Images/EducationOrganization/NoEducationOrganizationImage{0}.jpg";

        public LocalEducationAgencyFileSystemBasedImageContentProvider(
            IFileSystemBasedImagePathProvider fileSystemBasedImagePathProvider,
            IFileBasedImageContentProvider fileBasedImageContentProvider,
            IImageContentProvider next)
            : base(next)
        {
            this.fileSystemBasedImagePathProvider = fileSystemBasedImagePathProvider;
            this.fileBasedImageContentProvider = fileBasedImageContentProvider;
        }

        protected override bool CanHandleRequest(ImageRequestBase request)
        {
            return (request as LocalEducationAgencyImageRequest != null);
        }

        protected override ImageModel HandleRequest(ImageRequestBase request)
        {
            var displayType = request.DisplayType ?? string.Empty;
            var localEducationAgencyImageRequest = (LocalEducationAgencyImageRequest)request;
            var profileImage = fileSystemBasedImagePathProvider.GetImagePath(localEducationAgencyImageRequest.LocalEducationAgencyId, ImageType.LocalEducationAgency, displayType);
            var defaultFilePath = string.Format(defaultImageFilePathFormat, displayType.Trim());
            var defaultImage = fileBasedImageContentProvider.GetImageContent(defaultFilePath);

            logger.Debug(string.Format("Diagnostics for image. Trying paths:{0}, {1}", profileImage, defaultImage));

            return fileBasedImageContentProvider.GetImageContent(profileImage) ?? defaultImage;
        }

        public ImageModel GetImage(ImageRequestBase request)
        {
            //Delegate to the base...
            return ProcessRequest(request);
        }
    }
}
