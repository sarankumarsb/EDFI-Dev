// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System;

namespace EdFi.Dashboards.Resource.Models.Common
{
    /// <summary>
    /// Contains values that can be used to describe the relationship of the linked resource to the containing model.
    /// </summary>
    public static class LinkRel
    {
        /// <summary>
        /// Indicates that the link can be used to access the current model as a resource.
        /// </summary>
        public const string AsResource = "self";
        
        /// <summary>
        /// Indicates that the link is intended for viewing the model in a web browser as HTML.
        /// </summary>
        public const string Web = "web";
    }

    /// <summary>
    /// Represents a web hyperlink in a model.
    /// </summary>
    [Serializable]
    public class Link
    {
        /// <summary>
        /// The url for the hyperlink.
        /// </summary>
        public string Href { get; set; }

        /// <summary>
        /// The relationship of the link to the model on which it appears.
        /// </summary>
        public string Rel { get; set; }
    }
}
