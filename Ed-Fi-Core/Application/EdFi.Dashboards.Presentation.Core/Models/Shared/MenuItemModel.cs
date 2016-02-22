// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using EdFi.Dashboards.Common.Utilities;
using EdFi.Dashboards.Core.Providers.Context;
using EdFi.Dashboards.Resources.Security.Common;

namespace EdFi.Dashboards.Presentation.Core.Models.Shared
{
    public class MenuItemModel
    {
        public MenuItemModel(string text, string navigateUrl)
        {
            Initialize();
            Text = text;
            NavigateUrl = navigateUrl;
        }

        public MenuItemModel(string text)
        {
            Initialize();
            Text = text;
        }

        public MenuItemModel()
        {
            Initialize();
        }

        private void Initialize()
        {
            Enabled = true;
            Display = true;
            Selected = false;
            ChildItems = new List<MenuItemModel>();
            QueryStringValues = new Hashtable();
            RequireExactMatch = true;
        }

        public virtual bool Enabled { get; set; }
        public bool Display { get; set; }
        public bool Selected { get; set; }
        public string Text { get; set; }
        public string NavigateUrl { get; set; }
        public List<MenuItemModel> ChildItems { get; set; }
        public string Tooltip { get; set; }
        public Hashtable QueryStringValues { get; set; }
        public string Style { get; set; }
        public bool RequireExactMatch { get; set; }
        
        public MenuItemState State
        {
            get
            {
                if (!Enabled)
                    return MenuItemState.Disabled;

                if (!Selected)
                    return MenuItemState.Normal;

                return MenuItemState.Selected;
            }
        }

        public enum MenuItemState
        {
            Normal,
            Selected,
            Disabled
        }

        public override string ToString()
        {
            return Text + string.Format("({0})", string.Join(", ", ChildItems.Select(c => c.Text)));
        }
    }

    public class MetricMenuItemModel : MenuItemModel
    {
        private int metricVariantId;

        public int MetricVariantId
        {
            get
            {
                return metricVariantId;
            }
            set
            {
                metricVariantId = value;
                QueryStringValues.Add("metricVariantId", metricVariantId);
            }
        }

        private bool? _enabled;

        public override bool Enabled
        {
            get
            {
                // Was Enabled explicitly set to false?  Then it's disabled.
                if (!(_enabled ?? true))
                    return false;
                var currentUserClaimInterrogator = IoC.Resolve<ICurrentUserClaimInterrogator>();
                // Defer check to user's metric access
                return currentUserClaimInterrogator.HasClaimForMetricWithinEducationOrganizationHierarchy(metricVariantId, (int)(EdFiDashboardContext.Current.SchoolId ?? EdFiDashboardContext.Current.LocalEducationAgencyId));
            }
            set
            {
                _enabled = value;
            }
        }
    }
}