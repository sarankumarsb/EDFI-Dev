using System;
using EdFi.Dashboards.Presentation.Core.UITests.Pages;

namespace EdFi.Dashboards.Presentation.Core.UITests.Attributes
{
    /// <summary>
    /// Applied to a class deriving from the <see cref="PageBase"/> class, this attribute provides a Regex pattern
    /// for matching the URL that is known to be associated with the page (Before using this attribute, please try to 
    /// use the <see cref="PageBase.IsCurrent"/> class).
    /// </summary>
    /// <remarks>This attribute is used by the <see cref="AssociatedControllerAttribute" /> to determine whether the 
    /// current browser location represents the atributed page.</remarks>
    public class AssociatedUrlAttribute : Attribute
    {
        private readonly string regexPattern;

        /// <summary>
        /// Initializes a new instance of the <see cref="AssociatedUrlAttribute"/> class with the specified Regex pattern
        /// which can be used to match the URL of the browser for identifying whether the page is "current".
        /// </summary>
        /// <param name="regexPattern">The regular expression pattern to use.</param>
        public AssociatedUrlAttribute(string regexPattern)
        {
            this.regexPattern = regexPattern;
        }

        /// <summary>
        /// Gets the regular expression pattern to use for matching the URL.
        /// </summary>
        public string RegexPattern
        {
            get { return regexPattern; }
        }
    }
}