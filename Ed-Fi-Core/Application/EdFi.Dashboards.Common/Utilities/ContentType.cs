// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
namespace EdFi.Dashboards.Common.Utilities
{
    public static class ContentType
    {
        public static string GetContentType(string fileExtension, string defaultContentType)
        {
            switch (fileExtension)
            {
                case ".gif":
                    return "image/gif";
                case ".jpg":
                case ".jpeg":
                    return "image/jpeg";
                case ".png":
                    return "image/png";
                case ".tiff":
                    return "image/tiff";
                case ".bmp":
                    return "image/bmp";
            }

            return defaultContentType;
        }
    }
}
