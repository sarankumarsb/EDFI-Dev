// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using EdFi.Dashboards.Resources.Photo.Models;

namespace EdFi.Dashboards.Resources.Photo
{
    public interface IFileNameSelector
    {
        string Get(Identifier identifier, PhotoCategory photoCategory);
    }
}
