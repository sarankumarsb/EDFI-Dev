// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System.Collections.Generic;
using EdFi.Dashboards.Resources.Photo.Models;

namespace EdFi.Dashboards.Resources.Photo
{
    public interface IPhotoResizer
    {
        void Resize(Dictionary<PhotoCategory, byte[]> photos);
    }
}
