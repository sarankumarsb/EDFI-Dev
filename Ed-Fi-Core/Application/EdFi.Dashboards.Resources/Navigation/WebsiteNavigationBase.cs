// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
namespace EdFi.Dashboards.Resources.Navigation
{
    /// <summary>
    /// Provides a base class for creating a strongly typed mechanism for generating links to the web application's resources.
    /// </summary>
    /// <typeparam name="TAreas">A class type that exposes properties for the areas of the website.  See <see cref="EdFiWebFormsSiteAreas"/>.</typeparam>
    /// <seealso cref="EdFiWebFormsSiteAreas"/>
    public abstract class WebsiteNavigationBase<TAreas>
        where TAreas : new()
    {
        /// <summary>
        /// Initializes the static <see cref="Site"/> property to an instance of the generic type specified by <see cref="TAreas"/>.
        /// </summary>
        static WebsiteNavigationBase()
        {
            Site = new TAreas();
        }

        /// <summary>
        /// Returns a component containing the areas of the application available for link generation.
        /// </summary>
        public static TAreas Site { get; private set; }
    }
}