// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using EdFi.Dashboards.Resources.Images;

namespace EdFi.Dashboards.Resources.Photo.Models
{
    /// <summary>
    /// Derived from <see cref="OriginalPhoto"/>. This class only has a unique Id and ImageType added.
    /// </summary>
    public class UniqueOriginalPhoto : OriginalPhoto
    {
        /// <summary>
        /// Gets or sets the unique identifier for the photo (e.g. school Id, lea Id, USI).
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Gets or sets the type of image (e.g. school, lea, staff, student).
        /// </summary>
        public ImageType ImageType { get; set; }

        public override string FriendlyName
        {
            get { return string.Format("{0} - {1}", Id, ImageType); }
        }
    }
}
