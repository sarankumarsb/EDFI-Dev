// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EdFi.Dashboards.Resources.Admin;
using EdFi.Dashboards.Infrastructure;
using EdFi.Dashboards.Resources.Models.Admin;

namespace EdFi.Dashboards.Presentation.Core.Areas.Admin.Controllers
{
    public class PhotoManagementController : Controller
    {
        private readonly IPhotoManagementService photoManagementService;
        private readonly ISessionStateProvider sessionStateProvider;

        public PhotoManagementController(IPhotoManagementService photoManagementService, ISessionStateProvider sessionStateProvider)
        {
            this.photoManagementService = photoManagementService;
            this.sessionStateProvider = sessionStateProvider;
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public ViewResult Get(int localEducationAgencyId)
        {
            var model = photoManagementService.Get(new PhotoManagementRequest { LocalEducationAgencyId = localEducationAgencyId });

            return View(model);
        }

        /// <summary>
        /// This method receives the uploaded file in chunks. It won't be able to pass the file off for processing until the
        /// full file has been received.
        /// </summary>
        /// <param name="request">A representation of the partially uploaded file.</param>
        /// <returns>An <seealso cref="PhotoManagementModel"/> instance detailing the status of the uploading process.</returns>
        [AcceptVerbs(HttpVerbs.Post)]
        public ViewResult Get(PhotoManagementPostRequest request)
        {
            if (request.File == null)
            {
                // Make sure a file has been uploaded.
                var returnValue = photoManagementService.Get(new PhotoManagementRequest { LocalEducationAgencyId = request.LocalEducationAgencyId });

                returnValue.ErrorMessages = new List<string> { "Please select a file to upload." };
                returnValue.SuccessfullyProcessedPhotos = 0;
                returnValue.TotalRecords = 0;

                return View(returnValue);
            }

            var contentRange = ContentRange();
            var fileCollection = request.File as IEnumerable<HttpPostedFileBase>;
            var file = fileCollection.ElementAt(0);
            var fileBytes = new byte[file.InputStream.Length];
            var cacheKey = GetCacheKey(request.LocalEducationAgencyId, file.FileName);

            file.InputStream.Read(fileBytes, 0, (int)file.InputStream.Length);

            if (contentRange > 0)
            {
                // This file is being uploaded in chunks. Take this first chunk and add it to whatever else is in cache.
                // Then compare it against the content range. If the total file bytes are the same as the content range,
                // then start the processing of the file. Otherwise, return null.
                var existingFileBytes = sessionStateProvider[cacheKey] as byte[] ?? new byte[0];

                fileBytes = existingFileBytes.Concat(fileBytes).ToArray();

                if (fileBytes.Length == contentRange)
                {
                    // Remove this from the cache and process the file.
                    sessionStateProvider.RemoveValue(cacheKey);
                }
                else if (fileBytes.Length > contentRange)
                {
                    // Something is wrong. This should never happen. Get rid of the file and
                    // start over.
                    sessionStateProvider.RemoveValue(cacheKey);

                    return View((object)false);
                }
                else
                {
                    // This is just part of a larger file. Store it for later.
                    sessionStateProvider.SetValue(cacheKey, fileBytes);

                    return View((object)false);
                }
            }

            // The full file has been uploaded. Process it.
            request.File = fileBytes;

            return View(photoManagementService.Post(request));
        }

        private string GetCacheKey(long localEducationAgencyId, string fileName)
        {
            return string.Format("PhotoManagement:{0}:{1}", localEducationAgencyId, fileName);
        }

        /// <summary>
        /// Parses the content range header. The newer version of the framework has
        /// a property to expose this so this can be replaced once the dashboard application
        /// has been upgraded to 4.5.
        /// </summary>
        /// <returns></returns>
        private int ContentRange()
        {
            if (Request.Headers["Content-Range"] == null)
                return 0;

            int contentRange;

            int.TryParse(Request.Headers["Content-Range"].Split('/')[1], out contentRange);

            return contentRange;
        }
    }
}
