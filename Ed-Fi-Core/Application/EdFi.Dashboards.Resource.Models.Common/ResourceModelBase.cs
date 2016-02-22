// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System;
using System.Collections.Generic;

namespace EdFi.Dashboards.Resource.Models.Common
{
    /// <summary>
    /// Provides links for models to be treated as http-based resources.
    /// </summary>
    [Serializable]
    public abstract class ResourceModelBase : IResourceModelBase
    {
        /// <summary>
        /// Gets or sets the Url that can be used to view the model in a web browser.
        /// </summary>
        public string Url { get; set; }  // For OData-style serialization, this would be rel="web", or something similar. TBD.

        /// <summary>
        /// Gets or sets the Url that can be used to access the model as a resource.
        /// </summary>
        public string ResourceUrl { get; set; }  // For OData-style serialization, this would be rel="self"

        #region Links Property

        /// <summary>
        /// Holds the value for the <see cref="Links"/> property.
        /// </summary>
        private IEnumerable<Link> _links;

        /// <summary>
        /// Gets or sets the collection of other links related to the resource.
        /// </summary>
        public IEnumerable<Link> Links
        {
            get
            {
                if (_links == null)
                    yield break;

                foreach (var link in _links)
                    yield return link;
            }
            set { _links = value; }
        }

        #endregion
    }

    [Serializable]
    public class ResourceModel : ResourceModelBase
    {
        public string Text { get; set; }
        public bool Display { get; set; }
        public bool Enabled { get; set; }
        public bool RequireExactMatch { get; set; }
        public string Style { get; set; }

        public IEnumerable<ResourceModel> ChildItems { get; set; }

        public ResourceModel()
        {
            Initialize();
        }

        private void Initialize()
        {
            Enabled = true;
            Display = true;
            RequireExactMatch = true;
        }
    }

    [Serializable]
    public class MetricResourceModel : ResourceModel
    {
        public int MetricVariantId { get; set; }
    }
}