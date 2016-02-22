using System;
using Newtonsoft.Json;

namespace EdFi.Dashboards.Resource.Models.Common.Hal
{
    [Serializable]
    public class Link
    {
        /// <summary>
        /// Gets or sets the href value of the link.
        /// </summary>
        [JsonProperty("href")]
        public string Href { get; set; }

        /// <summary>
        /// Gets or sets a name that can be used as a secondary key for identifying a link with the same relationship.
        /// </summary>
        [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the language of the target resource, as defined by <see cref="http://tools.ietf.org/html/rfc5988">RFC5988</see>.
        /// </summary>
        [JsonProperty("hreflang", NullValueHandling = NullValueHandling.Ignore)]
        public string HrefLang { get; set; }

        /// <summary>
        /// Gets or sets a title for the link.
        /// </summary>
        [JsonProperty("title", NullValueHandling = NullValueHandling.Ignore)]
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets an indication of whether the <see cref="Href"/> refers to a URI Template (see <see cref="http://tools.ietf.org/html/rfc6570">RFC6570</see>.
        /// </summary>
        [JsonProperty("templated", NullValueHandling = NullValueHandling.Ignore)]
        public bool? Templated { get; set; }

        /// <summary>
        /// Gets or sets a hint for the media type expected when dereferencing the target resource.
        /// </summary>
        [JsonProperty("type", NullValueHandling = NullValueHandling.Ignore)]
        public string Type { get; set; }

         /// <summary>
        /// Gets or sets a URI which is a hint about the profile of the target resource, as defined by <see cref="http://tools.ietf.org/html/draft-kelly-json-hal-03#ref-I-D.wilde-profile-link">I-D.wilde-profile-link</see>.
        /// </summary>
        [JsonProperty("profile", NullValueHandling = NullValueHandling.Ignore)]
        public string Profile { get; set; }

        /// <summary>
        /// Creates a new <see cref="Link"/> instance and initializes the <see cref="Href"/> property from the assigned string value. 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator Link(string value)
        {
            return new Link { Href = value };
        }
    }
}
