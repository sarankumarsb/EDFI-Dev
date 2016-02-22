using System;
using EdFi.Dashboards.Presentation.Core.UITests.Pages;

namespace EdFi.Dashboards.Presentation.Core.UITests.Attributes
{
    /// <summary>
    /// Applied to a class deriving from the <see cref="PageBase"/> class, this attribute identifies the controller types 
    /// which are associated with the page (based on the browser URL and the ASP.NET MVC routing infrastructure).
    /// </summary>
    /// <remarks>This attribute is used by the <see cref="PageBase.IsCurrent"/> method to identify whether the 
    /// current browser location represents the atributed page.</remarks>
    public class AssociatedControllerAttribute : Attribute
    {
        private readonly Type[] controllerType;

        /// <summary>
        /// Initializes a new instance of the <see cref="AssociatedControllerAttribute"/> class with the specified
        /// controller type.
        /// </summary>
        /// <param name="controllerType">The types of the controllers that could be returned when the URL associated
        /// with the page is processed by the ASP.NET MVC routing system.</param>
        public AssociatedControllerAttribute(params Type[] controllerType)
        {
            this.controllerType = controllerType;
        }

        /// <summary>
        /// Gets the controller types that could be returned when the URL associated with the page is processed by the
        /// ASP.NET MVC routing system.
        /// </summary>
        public Type[] ControllerTypes
        {
            get { return controllerType; }
        }
    }
}