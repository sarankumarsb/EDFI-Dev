// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
namespace EdFi.Dashboards.Resources.Navigation.WebForms
{
    /// <summary>
    /// Provides an extensibility point for providing strongly-typed classes for generating links for the various areas of the Ed-Fi dashboard application.
    /// </summary>
    /// <typeparam name="TCommon">The concrete type of the class facilitating link generation for the Common area of the application.</typeparam>
    public abstract class SiteAreasBase<TCommon>
        where TCommon               : Areas.Common, new()
    {
        protected SiteAreasBase()
        {
            Common = new TCommon();
        }

        public TCommon Common { get; private set; }
    }
}