// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

namespace EdFi.Dashboards.Resources.Photo.Models
{
    /// <summary>
    /// This is the base class for photo objects to be returned when parsing an archive file of photos.
    /// </summary>
    public abstract class OriginalPhoto
    {
        public byte[] Photo { get; set; }

        /// <summary>
        /// Gets the uniquely identifying information about the photo. This is used when displaying
        /// a status of the photo on the photo upload page.
        /// </summary>
        public abstract string FriendlyName { get; }
    }
}