using System;
using BoDi;
using EdFi.Dashboards.Presentation.Core.UITests.Pages;

namespace EdFi.Dashboards.Presentation.Core.UITests.Support.SpecFlow
{
    public static class IObjectContainerExtensions
    {
        public static PageBase ResolveStaffPageObject(this IObjectContainer container, string pageName)
        {
            string coreTypeName, extTypeName;

            GetPageTypeNames("Staff", pageName, out coreTypeName, out extTypeName);

            return ResolvePageObject(container, extTypeName, coreTypeName);
        }

        /// <summary>
        /// Creates a page object instance based on namespace naming conventions from the supplied arguments.
        /// </summary>
        /// <param name="academicDashboardType">The Academic Dashboard type with which the target page is associated.</param>
        /// <param name="pageName">The name of the page (embedded spaces will be stripped).</param>
        /// <param name="schoolType">The type of school that is the focus of the test (for School level dashboards).</param>
        /// <returns>An instance of the corresponding page object, if implemented;</returns>
        /// <exception cref="NotSupportedException">Occurs when no page object could be found implementing the named web page.</exception>
        public static PageBase ResolvePageObject(this IObjectContainer container, AcademicDashboardType academicDashboardType, string pageName, SchoolType schoolType = SchoolType.Unspecified)
        {
            string coreTypeName, extTypeName;

            GetPageTypeNames(academicDashboardType.ToString(), pageName, out coreTypeName, out extTypeName);

            try
            {
                return ResolvePageObject(container, extTypeName, coreTypeName);
            }
            catch (NotImplementedException ex)
            {
                throw new NotImplementedException(string.Format(
                    "No page object implementation found for '{0}' dashboard, page '{1}' (with a school type of '{2}').",
                    academicDashboardType.ToString(), schoolType.ToString(), pageName), ex);
            }
 
            #region Commented out alternate implementation (left here for future reference)
            //string namespaceSearchText = String.Format(".{0}.{1}Page", dashboardType.ToString(), pageName);

            //var targetPageType =
            //    (from t in PageTypes
            //     where t.FullName.Contains(namespaceSearchText)
            //     select t)
            //     .SingleOrDefault();

            //if (targetPageType == null)
            //    throw new Exception(String.Format("Could not identify page object for {0} / {1}.", dashboardType.ToString(), pageName));

            //var page = Activator.CreateInstance(targetPageType) as PageBase;
            #endregion
        }

        private static PageBase ResolvePageObject(IObjectContainer container, string extTypeName, string coreTypeName)
        {
            try
            {
                var extType = Type.GetType(extTypeName);

                if (extType != null)
                    return container.Resolve(extType) as PageBase;

                var coreType = Type.GetType(coreTypeName);

                if (coreType != null)
                    return container.Resolve(coreType) as PageBase;

                throw new NotImplementedException(string.Format(
                    "No page object implementation found for type '{0}'.", coreTypeName));
            }
            catch (Exception ex)
            {
                throw new NotImplementedException(string.Format(
                    "No page object implementation found for type '{0}'.", coreTypeName), ex);
            }
        }

        private static void GetPageTypeNames(string areaName, string pageName, out string coreTypeName, out string extTypeName)
        {
            string coreNamespace = typeof (PageBase).Namespace + "." + areaName;
            string extensionNamespace = coreNamespace.Replace(".Core.", ".Web.");

            // Try to find page type by convention
            coreTypeName = string.Format("{0}.{1}Page", coreNamespace, pageName.RemoveWhitespace());
            extTypeName = string.Format("{0}.{1}Page", extensionNamespace, pageName.RemoveWhitespace());
        }

        #region Commented out supporting method for alternate implementation - PageTypes property (support method)

        //private static List<Type> _pageTypes;

        //private static List<Type> PageTypes
        //{
        //    get
        //    {
        //        if (_pageTypes == null)
        //        {
        //            var uiTestAssemblies = new[]
        //                                       {
        //                                           typeof(Marker_EdFi_Dashboards_Presentation_Web_UITests).Assembly,
        //                                       };

        //            _pageTypes =
        //                (from a in uiTestAssemblies
        //                 from t in a.GetTypes()
        //                 where typeof(PageBase).IsAssignableFrom(t)
        //                 select t)
        //                    .ToList();
        //        }

        //        return _pageTypes;
        //    }
        //}

        #endregion
    }
}
