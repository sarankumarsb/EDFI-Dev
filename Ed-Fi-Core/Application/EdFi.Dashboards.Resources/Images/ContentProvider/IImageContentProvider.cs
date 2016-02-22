// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using EdFi.Dashboards.Common;
using EdFi.Dashboards.Common.Utilities;
using EdFi.Dashboards.Resources.Models.Common;

namespace EdFi.Dashboards.Resources.Images.ContentProvider
{
    /// <summary>
    /// Gets the binary contents of an image.
    /// </summary>
    public interface IImageContentProvider
    {
        /// <summary>
        /// Gets the binary contents of an image, as specified by the request.
        /// </summary>
        /// <param name="request">A concrete request containing the information necessary to identify and return the correct image.</param>
        /// <returns>A model containing the image contents and content type.</returns>
        ImageModel GetImage(ImageRequestBase request);
    }
    
    public class NullImageProvider : IImageContentProvider
    {
        public ImageModel GetImage(ImageRequestBase request)
        {
            throw new NotImplementedException(string.Format("Unhandled request {0}", request.GetType().Name));
        }
    }

    public interface IFileBasedImageContentProvider
    {
        ImageModel GetImageContent(string filePath);
    }

    public class FileBasedImageContentProvider : IFileBasedImageContentProvider
    {
        public ImageModel GetImageContent(string filePath)
        {
            string physicalFilePath = GetPhysicalFilePath(filePath);

            if (physicalFilePath == null)
                return null;

            return new ImageModel
            {
                ContentType = ContentType.GetContentType(Path.GetExtension(physicalFilePath), "image"),
                Bytes = File.ReadAllBytes(physicalFilePath),
            };
        }

        private static Dictionary<string, string> physicalPathsByVirtualPath = new Dictionary<string, string>();

        private static string GetPhysicalFilePath(string filePath)
        {
            string realFilePath;

            if (physicalPathsByVirtualPath.TryGetValue(filePath, out realFilePath))
                return realFilePath;

            // This is because if we are running the FileSystemBasedImageContentProvider the filePaths are absolute through 
            // c:\etc... so they cant hack them and because these are resources that we dont want to republish/redeploy each 
            // time the website redeploys.
            realFilePath = (filePath.StartsWith("~")) ? HttpContext.Current.Server.MapPath(filePath) : filePath;

            if (!realFilePath.FileExists())
            {
                physicalPathsByVirtualPath[filePath] = null;
                return null;
            }

            physicalPathsByVirtualPath[filePath] = realFilePath;

            return realFilePath;
        }
    }
}
