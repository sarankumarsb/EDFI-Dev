using System.Collections.Generic;
using System.IO;
using EdFi.Dashboards.Resources.Images.ContentProvider;
using EdFi.Dashboards.Resources.Photo.Models;

namespace EdFi.Dashboards.Resources.Photo.Implementations.Storage
{
    /// <summary>
    /// Stores the images profile, list, and thumbnail images on the file system.
    /// </summary>
    public class FilePhotoStorage : IPhotoStorage
    {
        private readonly IFileSystemBasedImagePathProvider fileSystemBasedImagePathProvider;

        public FilePhotoStorage(IFileSystemBasedImagePathProvider fileSystemBasedImagePathProvider)
        {
            this.fileSystemBasedImagePathProvider = fileSystemBasedImagePathProvider;
        }

        public void Save(Identifier identifier, Dictionary<PhotoCategory, byte[]> photos)
        {
            var listImage = photos[PhotoCategory.List];
            var thumbImage = photos[PhotoCategory.Thumb];
            var profileImage = photos[PhotoCategory.Profile];

            var listPath = fileSystemBasedImagePathProvider.GetImagePath(identifier.Id, identifier.ImageType, "list");
            var thumbPath = fileSystemBasedImagePathProvider.GetImagePath(identifier.Id, identifier.ImageType, "thumb");
            var profilePath = fileSystemBasedImagePathProvider.GetImagePath(identifier.Id, identifier.ImageType, null);

            // Get the directories and make sure they exist before saving the files.
            var listDirectory = listPath.Substring(0, listPath.LastIndexOf(@"\"));
            var thumbDirectory = thumbPath.Substring(0, thumbPath.LastIndexOf(@"\"));
            var profileDirectory = profilePath.Substring(0, profilePath.LastIndexOf(@"\"));

            if (!Directory.Exists(listDirectory))
            {
                Directory.CreateDirectory(listDirectory);
            }

            if (!Directory.Exists(thumbDirectory))
            {
                Directory.CreateDirectory(thumbDirectory);
            }

            if (!Directory.Exists(profileDirectory))
            {
                Directory.CreateDirectory(profileDirectory);
            }

            // Now that the directories are in place, save the images.
            if (listImage != null)
                File.WriteAllBytes(listPath, listImage);
            else
                File.Delete(listPath);

            if (thumbImage != null)
                File.WriteAllBytes(thumbPath, thumbImage);
            else
                File.Delete(thumbPath);

            if (profileImage != null)
                File.WriteAllBytes(profilePath, profileImage);
            else
                File.Delete(profilePath);
        }
    }
}
