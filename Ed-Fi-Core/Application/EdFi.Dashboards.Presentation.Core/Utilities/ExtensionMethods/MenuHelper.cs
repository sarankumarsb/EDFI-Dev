// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;
using System.Collections.Generic;
using EdFi.Dashboards.Core.Providers.Context;
using EdFi.Dashboards.Presentation.Core.Models.Shared;
using EdFi.Dashboards.Resource.Models.Common;

namespace EdFi.Dashboards.Presentation.Core.Utilities
{
    public static class MenuHelper
    {
        public static List<MenuItemModel> MapResourcesModelsToMenus(IEnumerable<ResourceModel> resourceModels)
        {
            var menu = new List<MenuItemModel>();

            if (resourceModels == null)
                return menu;

            foreach (var model in resourceModels)
            {
                var resourceModel = model as MetricResourceModel;
                if (resourceModel != null)
                    menu.Add(new MetricMenuItemModel
                    {
                        Text = resourceModel.Text,
                        Display = resourceModel.Display,
                        NavigateUrl = resourceModel.Url,
                        RequireExactMatch = resourceModel.RequireExactMatch,
                        MetricVariantId = resourceModel.MetricVariantId,
                        ChildItems = MapResourcesModelsToMenus(resourceModel.ChildItems),
                        Style = resourceModel.Style,
                        Enabled = resourceModel.Enabled
                    });
                else
                    menu.Add(new MenuItemModel
                    {
                        Text = model.Text,
                        Display = model.Display,
                        NavigateUrl = model.Url,
                        RequireExactMatch = model.RequireExactMatch,
                        ChildItems = MapResourcesModelsToMenus(model.ChildItems),
                        Style = model.Style,
                        Enabled = model.Enabled
                    });
            }

            return menu;
        }

        public static void SetSelectedState(this MenuItemModel menuItemModel)
        {
            if (string.IsNullOrEmpty(menuItemModel.NavigateUrl))
                return;

            var currentRoute = EdFiDashboardContext.Current.RoutedUrl.ToLower();
            string menuItemRoute;
            if (menuItemModel.NavigateUrl.StartsWith("http", StringComparison.OrdinalIgnoreCase))
            {
                menuItemRoute = (new Uri(menuItemModel.NavigateUrl)).AbsolutePath.ToLower();
            }
            else
            {
                menuItemRoute = menuItemModel.NavigateUrl.ToLower();
            }

            if (currentRoute == menuItemRoute || (!menuItemModel.RequireExactMatch && currentRoute.StartsWith(menuItemRoute)))
            {
                menuItemModel.Selected = true;
            }

            //Lets see if this item should be active because of one of its children.
            if (IsChildMenuSelected(menuItemModel.ChildItems))
                menuItemModel.Selected = true;

            foreach (var childItem in menuItemModel.ChildItems)
                childItem.SetSelectedState();
        }

        private static bool IsChildMenuSelected(List<MenuItemModel> menuItems)
        {
            var currentRoute = EdFiDashboardContext.Current.RoutedUrl.ToLower();

            foreach (var menuItem in menuItems)
            {
                if (!string.IsNullOrEmpty(menuItem.NavigateUrl))
                {
                    string menuItemRoute;
                    if (menuItem.NavigateUrl.StartsWith("http", StringComparison.OrdinalIgnoreCase))
                    {
                        menuItemRoute = (new Uri(menuItem.NavigateUrl)).AbsolutePath.ToLower();
                    }
                    else
                    {
                        menuItemRoute = menuItem.NavigateUrl.ToLower();
                    }
                    if (currentRoute == menuItemRoute || (!menuItem.RequireExactMatch && currentRoute.StartsWith(menuItemRoute)))
                        return true;
                }

                if (menuItem.ChildItems.Count > 0)
                    if (IsChildMenuSelected(menuItem.ChildItems))
                        return true;
            }

            //If we didn't find any active then return false.
            return false;
        }
    }
}