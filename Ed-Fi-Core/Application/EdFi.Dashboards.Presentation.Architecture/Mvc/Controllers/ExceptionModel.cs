using System;
using Newtonsoft.Json;

namespace EdFi.Dashboards.Presentation.Architecture.Mvc.Controllers
{
    /// <summary>
    /// Represents a response containing exception information.
    /// </summary>
    [Serializable]
    public class ExceptionModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExceptionModel"/> class.
        /// </summary>
        public ExceptionModel() { }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="ExceptionModel"/> class from the specified <see cref="Exception"/> instance.
        /// </summary>
        /// <param name="ex">The exception to use as the source of the model information.</param>
        /// <remarks>To throw a user-visible exception, the original <see cref="Exception"/> should be wrapped with an instance of
        /// an <see cref="ApplicationException"/>.  To expose a custom developer message, the exception should be wrapped
        /// with an instance of an <see cref="Exception"/>.  The constructor will unwrap the exceptions into the serializable model.</remarks>
        public ExceptionModel(Exception ex)
        {
            if (ex is ApplicationException)
            {
                UserMessage = ex.Message;

                if (ex.InnerException != null)
                {
                    DeveloperMessage = ex.InnerException.Message;
                    Type = ex.InnerException.GetType().Name;
                }
                else
                {
                    Type = ex.GetType().Name;
                }
            }
            else
            {
                DeveloperMessage = ex.Message;
                Type = ex.GetType().Name;
            }
        }

        /// <summary>
        /// Gets or sets a message that can be displayed to a developer for troubleshooting purposes.
        /// </summary>
        [JsonProperty("developerMessage", NullValueHandling = NullValueHandling.Ignore)]
        public string DeveloperMessage { get; set; }

        /// <summary>
        /// Gets or sets a message that can be displayed to a user.
        /// </summary>
        [JsonProperty("userMessage", NullValueHandling = NullValueHandling.Ignore)]
        public string UserMessage { get; set; }

        /// <summary>
        /// Gets or sets the .NET type name of the exception.
        /// </summary>
        [JsonProperty("type")]
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets the .NET exception stack trace.
        /// </summary>
        [JsonIgnore] // Never serialize the stack trace, but leave it present for display on the error page in Debug mode.
        public string StackTrace { get; set; }
    }
}