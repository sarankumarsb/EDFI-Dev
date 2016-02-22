// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using System.Web.UI;
using EdFi.Dashboards.Common.Utilities;
using EdFi.Dashboards.Infrastructure;

namespace EdFi.Dashboards.SecurityTokenService.Web.Mvp.BaseArchitecture
{
    public abstract class MvpPageBase<TModel, TView, TPresenter> : Page
        where TPresenter : PresenterBase<TModel, TView>
        where TView : IViewBase<TModel>
    {
    	protected MvpPageBase()
		{
			// Resolve the Presenter via IoC (advantage: ability to wire up external dependencies automatically)
			Presenter = IoC.Resolve<TPresenter>();

			// Provide back reference to the view
			Presenter.View = (TView) (this as object);

    	    SessionState = IoC.Resolve<ISessionStateProvider>();
		}

    	/// <summary>
    	/// Gets or sets the presentation model containing the data for the page.
    	/// </summary>
        public TModel Model { get; set; }

    	/// <summary>
    	/// Gets or sets the Presenter in charge of manipulating the page
    	/// </summary>
    	/// <value>The presenter.</value>
    	public TPresenter Presenter{ get; set; }

        protected ISessionStateProvider SessionState { get; private set; }

    	/// <summary>
		/// Augments Page_Load behavior with automatic invocation of the Presenter.Initialize method with the necessary 
		/// parameters obtained first from Request.Form, and then Request.QueryString (providing default values if parameter 
		/// value cannot be found).
		/// </summary>
		/// <param name="e">Event-specific arguments.</param>
		protected override void OnLoad(EventArgs e)
		{
			// Call Presenter.Initialize dynamically using arguments from HttpRequest
			InvokeInitializeFromHttpRequest();

			// Perform UI data binding
			PerformDataBinding();

			// Allow normal ASP.NET processing to occur
			base.OnLoad(e);
		}

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            Response.AppendHeader("Cache-control", "no-store");
            Response.AppendHeader("Cache-control", "no-cache");
        }

		private void InvokeInitializeFromHttpRequest()
		{
			Type presenterType = typeof(TPresenter);
		
			MethodInfo method = 
				(from m in presenterType.GetMethods()
				where m.Name == "Initialize"
				select m)
				.FirstOrDefault();

			if (method == null)
				throw new MissingMethodException("The Initialize method could not be found on the Presenter class.");

			var parmValues = BuildAvailableArguments(method);

		    method.Invoke(Presenter, parmValues);
		}

        private object[] BuildAvailableArguments(MethodInfo method)
        {
            // Build prioritized list of available arguments
            var args = new NameValueCollection();

            var parameters = method.GetParameters();

            // Initialize parameters with default values for parameters needed for call to Presenter.Initialize
            foreach (ParameterInfo parm in parameters)
            {
                object defaultValue = GetDefault(parm.ParameterType);
                args[parm.Name] = defaultValue == null ? string.Empty : defaultValue.ToString();
            }

            // Overlay args with QueryString parameters 
            foreach (string key in Request.QueryString.AllKeys)
                args[key] = Request.QueryString[key];

            // Overlay args with Form values
            foreach (string key in Request.Form.AllKeys)
                args[key] = Request.Form[key];

            // Now map the arguments available, by name, to methods on Initialize
            object[] parmValues =
                (from p in parameters
                 from k in args.AllKeys
                 where string.Compare(p.Name, k, true) == 0
                 select Convert.ChangeType(args[k], p.ParameterType))
                    .ToArray();
            return parmValues;
        }

		private static object GetDefault(Type type)
		{
			if (type.IsValueType)
				return Activator.CreateInstance(type);

			return null;
		}

		/// <summary>
		/// Adds necessary code-based data bindings for user interface rendering.
		/// </summary>
        public abstract void PerformDataBinding();

        protected string FormatPercentage(decimal? value)
        {
            if (!value.HasValue)
                return String.Empty;
            return FormatPercentage(value.Value);
        }

        protected string FormatPercentage(decimal value)
        {
            return String.Format("{0:0.0%}", value);
        }

        protected string FormatPercentage(object value)
        {
            return FormatPercentage(Convert.ToDecimal(value));
        }
    }
}
