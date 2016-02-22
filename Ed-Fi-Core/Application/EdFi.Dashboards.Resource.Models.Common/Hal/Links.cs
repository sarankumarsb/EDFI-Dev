using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Runtime.Serialization;
using CommonLink = EdFi.Dashboards.Resource.Models.Common.Link;

namespace EdFi.Dashboards.Resource.Models.Common.Hal
{
    /// <summary>
    /// Provides a collection of links that can be serialized to JSON using the HAL media type.
    /// See http://stateless.co/hal_specification.html for more information.
    /// </summary>
    [Serializable]
    public class Links : DynamicObject, ISerializable
    {
        public Links() {}
        
        #region Added as legacy support for converting from original Link object which contains the "rel" property.
        public static implicit operator Links(List<CommonLink> value)
        {
            return FromIEnumerable(value);
        }

        public static Links FromIEnumerable(IEnumerable<CommonLink> source)
        {
            var links = new Links();

            foreach (var link in source)
                links.AddLink(link.Rel, link.Href);

            return links;
        }
        #endregion

        private readonly Dictionary<string, object> links = new Dictionary<string, object>();

        /// <summary>
        /// Gets or sets the link that refers to the resource, to be serialized as the "self" link.
        /// </summary>
        public Link self { get; set; } // HACK: Property is lower-case because JSON.NET does not support mixed-mode serialization (dynamic and static)

        /// <summary>
        /// Adds a link to the collection using the attributes specified.
        /// </summary>
        /// <param name="relationship">Describes the relationship of the targeted resource to the current resource.</param>
        /// <param name="href">The href value of the link.</param>
        /// <param name="title">A title for the link.</param>
        /// <param name="name">A name that can be used as a secondary key for identifying a link with the same relationship.</param>
        /// <param name="type">A hint for the media type expected when dereferencing the target resource.</param>
        /// <param name="profile">A URI which is a hint about the profile of the target resource, as defined by <see cref="http://tools.ietf.org/html/draft-kelly-json-hal-03#ref-I-D.wilde-profile-link">I-D.wilde-profile-link</see>.</param>
        /// <param name="hreflang">The language of the target resource, as defined by <see cref="http://tools.ietf.org/html/rfc5988">RFC5988</see>.</param>
        /// <param name="templated">Indicates whether the <paramref name="href"/> refers to a URI Template (see <see cref="http://tools.ietf.org/html/rfc6570">RFC6570</see>.</param>
        public void AddLink(string relationship, string href, string title = null, string name = null, string type = null, string profile = null, string hreflang = null, bool? templated = null)
        {
            // relationship is required
            if (string.IsNullOrEmpty(relationship))
                throw new ArgumentNullException("relationship");

            // href is required
            if (string.IsNullOrEmpty(href))
                throw new ArgumentNullException("href");

            Link link = href;

            // title is optional
            if (!string.IsNullOrEmpty(title))
                link.Title = title;

            // name is optional
            if (!string.IsNullOrEmpty(name))
                link.Name = name;

            // type is optional
            if (!string.IsNullOrEmpty(type))
                link.Type = type;

            // hreflang is optional
            if (!string.IsNullOrEmpty(hreflang))
                link.HrefLang = hreflang;

            // templated is optional
            if (templated != null)
                link.Templated = templated;

            AddLink(relationship, link);
        }

        public void AddLink(string relationship, Link link)
        {
            // Set "self" links added here to the "self" property
            if (string.Compare("self", relationship, StringComparison.InvariantCultureIgnoreCase) == 0)
            {
                self = link;
                return;
            }

            links[relationship] = link;
        }

		public Link GetLink(string relationship)
		{
			return links[relationship] as Link;
		}

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            // Converting the property name to lowercase 
            // so that property names become case-insensitive. 
            string name = binder.Name.ToLower();

            // If the property name is found in a dictionary, 
            // set the result parameter to the property value and return true. 
            // Otherwise, return false. 
            return links.TryGetValue(name, out result);
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            // Converting the property name to lowercase 
            // so that property names become case-insensitive.
            links[binder.Name.ToLower()] = value;

            // You can always add a value to a dictionary, 
            // so this method always returns true. 
            return true;
        }

        // Property is provided as a dynamic member name because JSON.NET does not support mixed-mode serialization (dyanamic and static)
        private readonly string[] propertiesToSerialize = { "self" };

        public override IEnumerable<string> GetDynamicMemberNames()
        {
            return (propertiesToSerialize).Concat(links.Keys.Where(k => string.CompareOrdinal(k, "__keys") != 0));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            // State is not provided during JSON.NET serialization (but is during BinaryFormatter serialization)
            // We only want to include the __keys member during non-JSON serialization (so that it doesn't appear in JSON output)
            if (context.State != 0)
                info.AddValue("__keys", GetDynamicMemberNames().ToArray(), typeof(string[]));

            info.AddValue("self", self, typeof(Link));

            foreach (var key in links.Keys) // .Where(k => string.CompareOrdinal("self", k) != 0)
                info.AddValue(key, links[key], typeof(Link));
        }

        public Links(SerializationInfo info, StreamingContext context)
        {
            var keys = (string[]) info.GetValue("__keys", typeof(string[]));

            foreach (string key in keys)
                AddLink(key, (Link) info.GetValue(key, typeof(Link)));
        }
    }
}
