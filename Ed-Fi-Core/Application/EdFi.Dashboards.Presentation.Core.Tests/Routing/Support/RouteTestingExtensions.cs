using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using EdFi.Dashboards.Common;
using EdFi.Dashboards.Common.Utilities;
using EdFi.Dashboards.Presentation.Architecture.Mvc.Core;
using MvcContrib.TestHelper;
using MvcContrib.TestHelper.Fakes;
using NUnit.Framework;
using Rhino.Mocks;
using AssertionException = MvcContrib.TestHelper.AssertionException;

namespace EdFi.Dashboards.Presentation.Core.Tests.Routing.Support
{
    /// <summary>
    /// Used to simplify testing routes.
    /// </summary>
    public static class RouteTestingExtensions
    {
        static RouteTestingExtensions()
        {
            // Default to development settings (should be set externally based on context/config)
            BaseUrl = "https://localhost/EdFiDashboardDev/";
        }

        public static string BaseUrl { get; set; }

        public static string ToVirtual(this string url)
        {
            if (url == null)
                throw new ArgumentNullException("url", "Url provided for virtualization cannot be null.");

            return url.Replace(BaseUrl, "~/", StringComparison.InvariantCultureIgnoreCase);
        }

        public static string Dump(this string url)
        {
            throw new Exception("DEBUG: Route URL: " + url);
        }

        private static string WrapWithForwardSlashes(string applicationPath)
        {
            if (string.IsNullOrWhiteSpace(applicationPath))
                return "/";

            // Tolerate incorrect app path format (should already contain leading and trailing /'s)
            return "/" + applicationPath.TrimStart('/').TrimEnd('/') + "/";
        }

        // Fake HttpContext, for the sake of the MVC innerds
        public static HttpContextBase FakeHttpContext(string url, string httpMethod)
        {
            string[] additionalKeyValuePairs = new string[0];
            
            if (url.Contains("?"))
            {
                // Parse the query string out from the URL
                string[] urlParts = url.Split(new[] {'?'}, 2);
                
                // Trim url at querystring marker
                url = urlParts[0];

                // Create an array of strings from the remaining arguments
                additionalKeyValuePairs = urlParts[1].Split('&');
            }

            var request = MockRepository.GenerateStub<HttpRequestBase>();            
            request.Stub(x => x.AppRelativeCurrentExecutionFilePath).Return(url).Repeat.Any();
            request.Stub(x => x.PathInfo).Return(string.Empty).Repeat.Any();
            request.Stub(x => x.HttpMethod).Return(httpMethod).Repeat.Any();
            request.Stub(x => x.Headers).Return(new NameValueCollection()).Repeat.Any();
            request.Stub(x => x.Form).Return(new NameValueCollection()).Repeat.Any();

            var collection = ParseValuesToCollection(additionalKeyValuePairs);
            request.Stub(x => x.QueryString).Return(collection).Repeat.Any();

            var context = MockRepository.GenerateStub<HttpContextBase>();
            context.Stub(x => x.Request).Return(request).Repeat.Any();

            return context;        
        }

        private static NameValueCollection ParseValuesToCollection(string[] addtionalKeyValuePairs)
        {
            var collection = new NameValueCollection();
            foreach (var pair in addtionalKeyValuePairs)
            {
                var parts = pair.Split(new[] {'='}, 2);

                if (parts.Length == 2)
                    collection.Add(parts[0], parts[1]);
            }
            return collection;
        }

        private static HttpContextBase MostRecentFakeHttpContext { get; set; }

        /// <summary>
        /// Returns the corresponding route for the URL.  Returns null if no route was found.
        /// </summary>
        /// <param name="url">The app relative url to test.</param>
        /// <returns>A matching <see cref="RouteData" />, or null.</returns>
        public static RouteData Route(this string url, string httpMethod = "GET")
        {
            if (string.IsNullOrWhiteSpace(url))
                throw new ArgumentNullException("url", "Url cannnot be null.");

            if (!url.StartsWith("~/"))
                throw new ArgumentException(string.Format("Url provided ('{0}') does not contain a virtual path starting with a '~'.", url));

            var context = FakeHttpContext(url, httpMethod);
            MostRecentFakeHttpContext = context;
            return RouteTable.Routes.GetRouteData(context);
        }

        /// <summary>
        /// Converts the URL to matching RouteData and verifies that it will match a route with the values specified by the expression.
        /// </summary>
        /// <typeparam name="TController">The type of controller</typeparam>
        /// <param name="relativeUrl">The ~/ based url</param>
        /// <param name="action">The expression that defines what action gets called (and with which parameters)</param>
        /// <param name="httpMethod">The http method to route. The default is "Get"</param>
        /// <returns></returns>
        public static RouteData ShouldMapTo<TController>(this string relativeUrl, Expression<Func<TController, ActionResult>> action, string httpMethod = "GET", bool allowNulls = false) where TController : Controller
        {
            try
            {
                var routeData = relativeUrl.Route(httpMethod);

                return routeData.ShouldMapTo(action, allowNulls);
            }
            catch (AssertionException ex)
            {
                if (ex.Message == "The URL did not match any route")
                    throw new Exception(string.Format("The URL '{0}' did not match any route.", relativeUrl), ex);

                throw;
            }
        }

        /// <summary>
        /// Asserts that the route matches the expression specified.  Checks controller, action, and any method arguments
        /// into the action as route values.
        /// </summary>
        /// <typeparam name="TController">The controller.</typeparam>
        /// <param name="routeData">The routeData to check</param>
        /// <param name="action">The action to call on TController.</param>
        public static RouteData ShouldMapTo<TController>(this RouteData routeData,
                                                            Expression<Func<TController, ActionResult>> action)
            where TController : Controller
        {
            return ShouldMapTo(routeData, action, false);
        }

        /// <summary>
        /// Asserts that the route matches the expression specified.  Checks controller, action, and any method arguments
        /// into the action as route values.
        /// </summary>
        /// <typeparam name="TController">The controller.</typeparam>
        /// <param name="routeData">The routeData to check</param>
        /// <param name="action">The action to call on TController.</param>
        /// <param name="allowNulls">Indicates whether or not to allow route values during testing to be <b>null</b>.</param>
        public static RouteData ShouldMapTo<TController>(this RouteData routeData, Expression<Func<TController, ActionResult>> action, bool allowNulls)
            where TController : Controller
        {            
            routeData.ShouldNotBeNull("The URL did not match any route");

            // check controller
            routeData.ShouldMapTo<TController>();
            
            // check action
            var methodCall = (MethodCallExpression) action.Body;
            string actualAction = routeData.Values.GetValue("action").ToString();
            string expectedAction = methodCall.Method.Name;
            actualAction.AssertSameStringAs(expectedAction);
            
            // check action parameters
            var invocationParameters = GetInvocationArguments(methodCall);

            // Modify routeData with custom BindAliasAttribute behavior from MVC model binding
            ApplyBindAliasAttributes<TController>(routeData);

            AssertInvocationParameters(invocationParameters, routeData, allowNulls);

            return routeData;
        }

        private static void ApplyBindAliasAttributes<TController>(RouteData routeData) where TController : Controller
        {
            try
            {
                // Get the request model, based on Request/Response pattern
                var requestModel = typeof (TController).GetGenericArguments()[0];
                var aliasedProperties =
                    from p in requestModel.GetProperties()
                    let a =
                        p.GetCustomAttributes(typeof (BindAliasAttribute), true)
                         .Cast<BindAliasAttribute>()
                         .SingleOrDefault()
                    where a != null
                    select new {p.Name, a.Alias};

                // Move values in routeData to keys associated with the property name for aliased properties 
                //      (e.g. resourceIdentifier -> MetricId in PlannedGoalsItemRequest.cs).
                foreach (var aliasedProperty in aliasedProperties)
                {
                    routeData.Values[aliasedProperty.Name] = routeData.Values[aliasedProperty.Alias];
                    routeData.Values.Remove(aliasedProperty.Alias);
                }
            }
            catch (Exception)
            {
                // Ignore exceptions if they occur during BindAlias processing
            }
        }

        private static void AssertInvocationParameters(Dictionary<string, dynamic> invocationParameters, RouteData routeData, bool allowNulls)
        {
            foreach (var invocationParameter in invocationParameters)
            {
                string parameterName = invocationParameter.Key;

                object expectedValue = invocationParameter.Value;
                var actualValue = GetActualParameterValue(routeData, parameterName);

                if (!allowNulls)
                {
                    if (expectedValue == null)
                        throw new ArgumentNullException(
                            parameterName,
                            string.Format("Unable to evaluate expected value for parameter '{0}'.", parameterName));


                    if (actualValue == null)
                        throw new ArgumentNullException(
                            parameterName,
                            string.Format("Unable to provide actual value for parameter '{0}' from route data.",
                                          parameterName));
                }
                else
                {
                    // If both values are null, consider them equal and quit now
                    if (actualValue == null && expectedValue == null)
                        return;
                }

                // Convert values to strings for comparison
                actualValue = Convert.ToString(actualValue);
                expectedValue = Convert.ToString(expectedValue);

                actualValue.ShouldEqual(expectedValue,
                        string.Format(
                            "Value for parameter '{0}' was incorrect.  Expected: '{1}', Actual: '{2}'.",
                            parameterName, expectedValue, actualValue));
            }
        }

        private static object GetActualParameterValue(RouteData routeData, string name)
        {
            // Get actual value from data extracted from the route
            object actualValue = routeData.Values.GetValue(name);

            // Does it need to be filled in by value providers?
            if (actualValue == null)
            {
                actualValue = GetActualValueUsingValueProviders(routeData, name);

                if (actualValue == null)
                    actualValue = MostRecentFakeHttpContext.Request.QueryString[name];
            }

            return actualValue;
        }

        private static object GetActualValueUsingValueProviders(RouteData routeData, string name)
        {
            object actualValue = null;

            foreach (var factory in ValueProviderFactories.Factories)
            {
                var provider = factory.GetValueProvider(
                    new ControllerContext(
                        new FakeHttpContext(null),
                        routeData,
                        new NullController()));

                if (provider.ContainsPrefix(name))
                {
                    var result = provider.GetValue(name);
                    actualValue = result.RawValue;
                    break;
                }
            }

            return actualValue;
        }

        private static Dictionary<string, dynamic> GetInvocationArguments(MethodCallExpression methodCall)
        {
            var invocationArguments = new Dictionary<string, dynamic>();

            // Iterate through arguments, adding to dictionary (simulating simple model binding)
            for (int i = 0; i < methodCall.Arguments.Count; i++)
            {
                string name = methodCall.Method.GetParameters()[i].Name;
                object argValue = null;

                switch (methodCall.Arguments[i].NodeType)
                {
                    case ExpressionType.Constant:
                        argValue = ((ConstantExpression) methodCall.Arguments[i]).Value;
                        break;

                    case ExpressionType.MemberAccess:
                        argValue = Expression.Lambda(methodCall.Arguments[i]).Compile().DynamicInvoke();
                        break;

                    case ExpressionType.Call:
                        argValue = Expression.Lambda(methodCall.Arguments[i]).Compile().DynamicInvoke();
                        break;

                    case ExpressionType.Convert:
                        argValue = Expression.Lambda(methodCall.Arguments[i]).Compile().DynamicInvoke();
                        break;
                }

                if (argValue == null || argValue.GetType().IsValueType || argValue.GetType() == typeof(string))
                    invocationArguments[name] = argValue;
                else
                {
                    // Geta all writable properties from the class (should be a request model)
                    var args = argValue.ToDictionary(shouldInclude: p => p.CanWrite);

                    foreach (var a in args)
                        invocationArguments[a.Key] = a.Value;
                }
            }
            return invocationArguments;
        }

        /// <summary>
        /// Converts the URL to matching RouteData and verifies that it will match a route with the values specified by the expression.
        /// </summary>
        /// <typeparam name="TController">The type of controller</typeparam>
        /// <param name="relativeUrl">The ~/ based url</param>
        /// <param name="httpMethod">The http method to route. The default is "Get"</param>
        /// <returns></returns>
        public static RouteData ShouldMapTo<TController>(this string relativeUrl, string httpMethod = "GET") where TController : Controller
        {
            var routeData = relativeUrl.Route(httpMethod);

            routeData.ShouldNotBeNull(string.Format("The URL '{0}' did not match any route.", relativeUrl));

            return routeData.ShouldMapTo<TController>();
        }

        /// <summary>
        /// Verifies the <see cref="RouteData">RouteData</see> maps to the controller type specified.
        /// </summary>
        /// <typeparam name="TController">The type of the controller that is expected to handle the request.</typeparam>
        /// <param name="routeData">The route data provided for matching</param>
        /// <returns>The same instance of the <see cref="RouteData"/> provided (for method chaining).</returns>
        public static RouteData ShouldMapTo<TController>(this RouteData routeData) 
            where TController : Controller
        {
            Type expectedControllerType = typeof(TController);

            // Call the route-related callback, if provided
            if (RouteMatchedCallback != null)
                RouteMatchedCallback(routeData);

            // Call the controller type callback, if provided
            if (ControllerTypeTestedCallback != null)
                ControllerTypeTestedCallback(expectedControllerType);

            Type actualControllerType = routeData.GetMappedControllerType();

            Assert.That(actualControllerType, Is.EqualTo(expectedControllerType));

            return routeData;
        }

        /// <summary>
        /// Uses the provided <see cref="RouteData"/> to determine the type of the controller that would handle the request.
        /// </summary>
        /// <param name="routeData">The route data provided for matching.</param>
        /// <returns>The resolve controller's <see cref="Type"/>.</returns>
        public static Type GetMappedControllerType(this RouteData routeData)
        {
            //get the key (case insensitive)
            string actual = routeData.Values.GetValue("controller").ToString();

            var requestContext = new RequestContext(MostRecentFakeHttpContext, routeData);

            var controllerFactory = ControllerBuilder.Current.GetControllerFactory();

            var getControllerTypeMethod = controllerFactory.GetType().GetMethod("GetControllerType",
                                                                                BindingFlags.NonPublic | BindingFlags.Instance);
            Type controllerType =
                getControllerTypeMethod.Invoke(controllerFactory, new object[] {requestContext, actual}) as Type;

            if (controllerType == null)
                throw new Exception(string.Format("Controller type for url '{0}' could not be identified.",
                                                  ((Route) routeData.Route).Url));

            // Process controller type using IoC (allows for redirection due to extension assembly overrides)
            Type actualControllerType = IoC.ResolveImplementationType(controllerType);

            return actualControllerType;
        }

        public static Action<RouteData> RouteMatchedCallback;
        public static Action<Type> ControllerTypeTestedCallback;

        /// <summary>
        /// Verifies the <see cref="RouteData">routeData</see> will instruct the routing engine to ignore the route.
        /// </summary>
        /// <param name="relativeUrl"></param>
        /// <returns></returns>
        public static RouteData ShouldBeIgnored(this string relativeUrl, string httpMethod = "GET")
        {
            RouteData routeData = relativeUrl.Route(httpMethod);

            routeData.RouteHandler.ShouldBe<StopRoutingHandler>("Expected StopRoutingHandler, but wasn't");

            return routeData;
        }

        /// <summary>
        /// Gets a value from the <see cref="RouteValueDictionary" /> by key.  Does a
        /// case-insensitive search on the keys.
        /// </summary>
        /// <param name="routeValues"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static object GetValue(this RouteValueDictionary routeValues, string key)
        {
            foreach(var routeValueKey in routeValues.Keys)
            {
                if(string.Equals(routeValueKey, key, StringComparison.InvariantCultureIgnoreCase))
                    return routeValues[routeValueKey] as string;
            }

            return null;
        }
    }
}
