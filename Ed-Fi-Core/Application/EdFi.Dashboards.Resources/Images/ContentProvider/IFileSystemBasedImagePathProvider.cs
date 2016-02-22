using System;
using System.IO;
using EdFi.Dashboards.Core;
using EdFi.Dashboards.Infrastructure;

namespace EdFi.Dashboards.Resources.Images.ContentProvider
{
    public interface IFileSystemBasedImagePathProvider
    {
        int BucketCount { get; }
        string ImageDirectory { get; }
        string GetImagePath(long id, ImageType imageType, string display);
        string GetDefaultImagePath(ImageType imageType, string display);
    }

    /// <summary>
    /// Gets the file system path of the image to be served up.
    /// </summary>
    public class FileSystemBasedImagePathProvider : IFileSystemBasedImagePathProvider
    {
        private static readonly object imageDirectoryLockObject = new object();
        private static readonly object bucketCountLockObject = new object();
        private static string imageDirectory = string.Empty;
        private static int bucketCount;

        // The parameters are Image Directory, LocalEducationAgency Id, ImageType, Display ImageType (thumbnail, profile, list), bucket (where applicable), and Id (i.e. EdFiData\123\images\profile\thumbnail\6\1.jpg).
        private const string imagePathFormat = @"{0}\images\{1}\{2}\{3}{4}{5}.jpg";

        private const string defaultPersonImageFilePathFormat = "~/Content/Images/Students/NoImage{0}.jpg";
        private const string defaultEducationOrganizationImageFilePathFormat = "~/Content/Images/EducationOrganization/NoEducationOrganizationImage{0}.jpg";

        private readonly IConfigValueProvider configValueProvider;
        private readonly ILocalEducationAgencyContextProvider localEducationAgencyContextProvider;

        public FileSystemBasedImagePathProvider(
                IConfigValueProvider configValueProvider,
                ILocalEducationAgencyContextProvider localEducationAgencyContextProvider)
        {
            this.configValueProvider = configValueProvider;
            this.localEducationAgencyContextProvider = localEducationAgencyContextProvider;
        }

        public int BucketCount
        {
            get
            {
                if (bucketCount == 0)
                {
                    lock (bucketCountLockObject)
                    {
                        bucketCount = Convert.ToInt32(configValueProvider.GetValue("PersistedRepositoryBucketCount"));
                    }
                }

                return bucketCount > 0 ? bucketCount : 1;
            }
        }

        public string ImageDirectory
        {
            get
            {
                if (String.IsNullOrEmpty(imageDirectory))
                {
                    lock (imageDirectoryLockObject)
                    {
                        imageDirectory = configValueProvider.GetValue("PersistedRepositoryDirectory");
                    }
                }

                return imageDirectory;
            }
        }

        public string GetImagePath(long id, ImageType imageType, string display)
        {
            string bucket = null;
            var displayTypeForProfile = string.IsNullOrEmpty(display) ? string.Empty : display.Trim() + "\\";

            // We could have created a chain for this, but that seems like overkill given a fairly static list of image types.
            if (imageType == ImageType.Staff || imageType == ImageType.Student)
                bucket = (Math.Abs(id) % BucketCount) + "\\";

            return string.Format(imagePathFormat, ImageDirectory, localEducationAgencyContextProvider.GetCurrentLocalEducationAgencyCode(), imageType, displayTypeForProfile, bucket, id);
        }

        public string GetDefaultImagePath(ImageType imageType, string display)
        {
            // We could have created a chain for this, but that seems like overkill given a fairly static list of image types.
            if (imageType == ImageType.Staff || imageType == ImageType.Student)
                return string.Format(defaultPersonImageFilePathFormat, !string.IsNullOrWhiteSpace(display) ? display.Trim() : null);
            if (imageType == ImageType.LocalEducationAgency || imageType == ImageType.School)
                return string.Format(defaultEducationOrganizationImageFilePathFormat, !string.IsNullOrWhiteSpace(display) ? display.Trim() : null);

            throw new NotSupportedException("Only student, staff, school, and district images are supported by this implementation.");
        }
    }
}
