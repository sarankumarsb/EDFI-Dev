// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System.Collections.Generic;
using EdFi.Dashboards.Resources.Photo.Models;

namespace EdFi.Dashboards.Resources.Photo.Implementations
{
    /// <summary>
    /// Implementation of <see cref="IPhotoProcessor"/>. This class is the version of IPhotoProcessor that most, if not all,
    /// implementations will use. The dependencies provide the pluggable extensions.
    /// </summary>
    public class PhotoProcessor : IPhotoProcessor
    {
        private readonly IArchiveParser archiveParser;
        private readonly IPackageReader packageReader;
        private readonly IIdentifierProvider identifierProvider;
        private readonly IPhotoResizer photoResizer;
        private readonly IPhotoStorage photoStorage;

        public PhotoProcessor(IArchiveParser archiveParser,
                        IPackageReader packageReader,
                        IIdentifierProvider identifierProvider,
                        IPhotoResizer photoResizer,
                        IPhotoStorage photoStorage)
        {
            this.archiveParser = archiveParser;
            this.packageReader = packageReader;
            this.identifierProvider = identifierProvider;
            this.photoResizer = photoResizer;
            this.photoStorage = photoStorage;
        }

        public PhotoProcessorResponse Process(PhotoProcessorRequest photoProcessorRequest)
        {
            var photoProcessorResponse = new PhotoProcessorResponse();

            // Parse the package (e.g. zip file) for photos.
            var files = archiveParser.Parse(photoProcessorRequest.FileBytes);

            // Read out all of the files into "OriginalPhoto" instances.
            var originalPhotos = packageReader.GetPhotos(files);

            // Iterate through each photo found.
            foreach (var originalPhoto in originalPhotos)
            {
                photoProcessorResponse.TotalRecords++;

                var errorMessage = ProcessPhoto(photoProcessorRequest, originalPhoto);

                // If an error message is returned, then add it to the response. If no error is
                // returned, we'll assume that it was successfully processed.
                if (errorMessage != null)
                    photoProcessorResponse.ErrorMessages.Add(errorMessage);
                else
                    photoProcessorResponse.SuccessfullyProcessedPhotos++;
            }

            return photoProcessorResponse;
        }

        private string ProcessPhoto(PhotoProcessorRequest photoProcessorRequest, OriginalPhoto originalPhoto)
        {
            if (originalPhoto.Photo == null)
                return string.Format("Could not find a photo for {0}", originalPhoto.FriendlyName);

            // Lookup the identifier and load a record.
            var identifierRequest = new IdentifierRequest { LocalEducationAgencyId = photoProcessorRequest.LocalEducationAgencyId, SchoolId = photoProcessorRequest.SchoolId, OriginalPhoto = originalPhoto };
            var identifier = identifierProvider.Get(identifierRequest);

            if (identifier == null)
                return string.Format("Unable to find a record in the database for {0}", originalPhoto.FriendlyName);

            // Each photo that reaches this point has a valid identifier, so resize and save.
            var photos = new Dictionary<PhotoCategory, byte[]> { { PhotoCategory.Original, originalPhoto.Photo } };

            photoResizer.Resize(photos);
            photoStorage.Save(identifier, photos);

            return null;
        }
    }
}
