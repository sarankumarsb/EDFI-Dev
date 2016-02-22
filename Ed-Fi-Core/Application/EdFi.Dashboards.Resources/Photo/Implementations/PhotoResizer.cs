// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System;
using System.Collections.Generic;
using System.IO;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using EdFi.Dashboards.Resources.Photo.Models;

namespace EdFi.Dashboards.Resources.Photo.Implementations
{
    /// <summary>
    /// Resizes images for profile, list, and thumbnail views.
    /// </summary>
    public class PhotoResizer : IPhotoResizer
    {
        public void Resize(Dictionary<PhotoCategory, byte[]> photos)
        {
            using (var memoryStream = new MemoryStream(photos[PhotoCategory.Original]))
            {
                var originalImage = Image.FromStream(memoryStream);

                photos.Add(PhotoCategory.Profile, ImageResize(originalImage, 139, 186, 0));
                photos.Add(PhotoCategory.List, ImageResize(originalImage, 28, 37, 0));
                photos.Add(PhotoCategory.Thumb, ImageResize(originalImage, 34, 46, 0));
            }
        }

		private static byte[] ImageResize(Image image, int width, int height, int imageHeightOffset)
        {
            var trimmedHeight = image.Height - (imageHeightOffset * 2);
            var heightRatio = Convert.ToDouble(trimmedHeight) / height;
            var widthRatio = Convert.ToDouble(image.Width) / width;
            var x = 0;
            var y = 0;

            if (heightRatio < widthRatio)
                x = CalcOffset(image.Width, width, heightRatio);
            else
                y = CalcOffset(trimmedHeight, height, widthRatio);

            using (var bitmap = new Bitmap(width, height))
            {
                using (var newGraphic = Graphics.FromImage(bitmap))
                {
                    var newDim = new Rectangle(0, 0, width, height);
                    var imageDim = new Rectangle(x, y + imageHeightOffset, image.Width - x * 2, trimmedHeight - y * 2);

                    newGraphic.SmoothingMode = SmoothingMode.HighQuality;
                    newGraphic.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    newGraphic.DrawImage(image, newDim, imageDim, GraphicsUnit.Pixel);

                    using (var ms = new MemoryStream())
                    {
                        bitmap.Save(ms, ImageFormat.Jpeg);

                        return ms.ToArray();
                    }
                }
            }
        }
        
        private static int CalcOffset(int imageDim, int newDim, double ratio)
        {
            return Convert.ToInt32((imageDim - (newDim * ratio)) / 2);
        }
    }
}
