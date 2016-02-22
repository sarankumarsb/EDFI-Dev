using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace EdFi.Dashboards.Resource.Models.Common.Hal
{
    /// <summary>
    /// Provides a base class for models to be serialized to JSON using the HAL media type. 
    /// See http://stateless.co/hal_specification.html for more information.
    /// </summary>
    [Serializable]
    public abstract class ResourceModelBase : IResourceModelBase
    {
        protected ResourceModelBase()
        {
            Links = new Links();
        }

        private string _resourceUrl;

        /// <summary>
        /// Gets or sets the Url that can be used to access the model as a resource.
        /// </summary>
        [JsonIgnore]
        public string ResourceUrl
        {
            get { return _resourceUrl; }
            set { _resourceUrl = value; Links.self = new Link { Href = value }; }
        }

        private string _url;

        /// <summary>
        /// Gets or sets the Url that can be used to view the model in a web browser.
        /// </summary>
        [JsonIgnore]
        public string Url
        {
            get { return _url; }
            set { _url = value; Links.AddLink("web", value); }
        }

        /// <summary>
        /// Gets or sets the collection of other links related to the resource.
        /// </summary>
        [JsonProperty("_links")]
        public Links Links { get; private set; }
    }
}
