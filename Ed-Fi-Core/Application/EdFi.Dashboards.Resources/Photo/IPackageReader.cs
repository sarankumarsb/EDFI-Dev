// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System;
using System.Collections.Generic;
using EdFi.Dashboards.Resources.Photo.Models;

namespace EdFi.Dashboards.Resources.Photo
{
    public interface IPackageReader
    {
        IEnumerable<OriginalPhoto> GetPhotos(Dictionary<string, byte[]> files);
    }

    public class NullPackageReader : IPackageReader
    {
        public IEnumerable<OriginalPhoto> GetPhotos(Dictionary<string, byte[]> files)
        {
            throw new NotImplementedException("Unhandled request");
        }
    }
}
