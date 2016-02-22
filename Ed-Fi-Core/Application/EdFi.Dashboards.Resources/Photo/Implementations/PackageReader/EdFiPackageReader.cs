// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System.Collections.Generic;
using System.Linq;
using EdFi.Dashboards.Common;
using EdFi.Dashboards.Resources.Images;
using EdFi.Dashboards.Resources.Photo.Models;

namespace EdFi.Dashboards.Resources.Photo.Implementations.PackageReader
{
    public class EdFiPackageReader : ChainOfResponsibilityBase<IPackageReader, Dictionary<string, byte[]>, IEnumerable<OriginalPhoto>>, IPackageReader
    {
        public EdFiPackageReader(IPackageReader next)
            : base(next)
        {

        }

        public IEnumerable<OriginalPhoto> GetPhotos(Dictionary<string, byte[]> files)
        {
            return ProcessRequest(files);
        }

        protected override bool CanHandleRequest(Dictionary<string, byte[]> request)
        {
            try
            {
                // We could do some quick optimization here to make sure the file isn't parsed in this method
                // and then again when/if it's actually handling the request.
                var originalPhotos = HandleRequest(request);

                return originalPhotos.Any();
            }
            catch
            {
                return false;
            }
        }

        protected override IEnumerable<OriginalPhoto> HandleRequest(Dictionary<string, byte[]> request)
        {
            var originalPhotos = new List<UniqueOriginalPhoto>();

            foreach (var keyValuePair in request.Where(file => file.Key.ToLower().EndsWith(".jpg") || file.Key.ToLower().EndsWith(".jpeg") || file.Key.ToLower().EndsWith(".png")))
            {
                var fullFileName = keyValuePair.Key.ToLower();
                var fileName = fullFileName.Split('/').LastOrDefault();
                var photo = keyValuePair.Value;
                long id;

                if (long.TryParse(fileName.Split('.')[0], out id))
                {
                    var originalPhoto = new UniqueOriginalPhoto
                                            {Id = id, Photo = photo};

                    if (fullFileName.Contains("staff"))
                        originalPhoto.ImageType = ImageType.Staff;
                    else if (fullFileName.Contains("students"))
                        originalPhoto.ImageType = ImageType.Student;

                    if (!originalPhotos.Any(x => x.Id == originalPhoto.Id && x.ImageType == originalPhoto.ImageType))
                        originalPhotos.Add(originalPhoto);
                }
            }

            return originalPhotos;
        }
    }
}