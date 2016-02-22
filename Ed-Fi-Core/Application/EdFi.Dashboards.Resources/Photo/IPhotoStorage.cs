// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System.Collections.Generic;
using EdFi.Dashboards.Resources.Photo.Models;

namespace EdFi.Dashboards.Resources.Photo
{
    public interface IPhotoStorage
    {
        void Save(Identifier identifier, Dictionary<PhotoCategory, byte[]> photos);
    }
}
