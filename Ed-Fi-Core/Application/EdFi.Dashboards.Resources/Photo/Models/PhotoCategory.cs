// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

namespace EdFi.Dashboards.Resources.Photo.Models
{
    public class PhotoCategory
    {
        protected PhotoCategory()
        {
        }

        public static readonly PhotoCategory Profile = new PhotoCategory();
        public static readonly PhotoCategory Thumb = new PhotoCategory();
        public static readonly PhotoCategory List = new PhotoCategory();
        public static readonly PhotoCategory Original = new PhotoCategory();
    }
}
