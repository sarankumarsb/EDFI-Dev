using System;
using EdFi.Dashboards.Presentation.Core.UITests.Support;
using EdFi.Dashboards.Presentation.Core.UITests.Support.Coypu;
using EdFi.Dashboards.Presentation.Core.UITests.Support.RestSharp;
using Newtonsoft.Json;
using RestSharp;

namespace EdFi.Dashboards.Presentation.Core.UITests.Pages
{
    /// <summary>
    /// Provides typed access to the underlying ViewModel used to render the webpage represented by the Page object.
    /// </summary>
    /// <typeparam name="TViewModel">The type of the ViewModel associated with the page.</typeparam>
    public abstract class PageBase<TViewModel> : PageBase
    {
        private TViewModel _model;

        /// <summary>
        /// Gets the view model used to render the current page.
        /// </summary>
        public virtual TViewModel Model
        {
            get
            {
                if (_model == null)
                {
                    if (!IsCurrent(Make_It.Wait_10_Seconds, true))
                        throw new InvalidOperationException("Cannot retrieve model for non-active webpage.");

                    string resourceUrl = Browser.Location.ToRelativeDashboardPath();

                    // Can't use generic Execute<T> because of new() constraint in RestSharp and our need to deserialize to abstract types (i.e. MetricBase)
                    RestRequest request = new RestRequest(resourceUrl, Method.GET);
                    var response = RestClient.Execute(request);

                    try
                    {
                        _model = JsonConvert.DeserializeObject<TViewModel>(response.Content, new JsonMetricBaseConverter());
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(string.Format("Unable to deserialize web service response as JSON: {0}", response.Content), ex);
                    }
                }

                return _model;
            }
        }
    }
}