using System;
using System.Collections.Generic;
using Castle.DynamicProxy;
using EdFi.Dashboards.Infrastructure;

namespace EdFi.Dashboards.Resources.Common
{
    /// <summary>
    /// Provides a mechanism for applying user-specific context to view models after they have been processed by the caching infrastructure.
    /// </summary>
    public interface IUserContextApplicator
    {
        /// <summary>
        /// Gets the Type of the model class supported by the user context applicator.
        /// </summary>
        Type ModelType { get; }

        /// <summary>
        /// Applies user-specific context to the model, after the core model has been cached.
        /// </summary>
        /// <param name="modelAsObject">The model object being returned (will never be a <b>null</b> reference).</param>
        /// <param name="requestAsObject">The request object passed to the service.</param>
        /// <remarks>The model will never be null, because the user context applicator will not be invoked by the <see cref="UserContextInterceptor"/> in this scenario.</remarks>
        void ApplyUserContextToModel(object modelAsObject, object requestAsObject);
    }

    /// <summary>
    /// Provides a generic base class implementation of the <see cref="IUserContextApplicator"/> interface, providing variables
    /// already typed appropriately, and boilerplate code for making sure they're not passed in as null references.
    /// </summary>
    /// <typeparam name="TRequest">The type of the service's request.</typeparam>
    /// <typeparam name="TModel">The type of the service's response view model.</typeparam>
    public abstract class UserContextApplicatorBase<TRequest, TModel> : IUserContextApplicator
        where TModel : class 
        where TRequest : class 
    {
        public Type ModelType
        {
            get { return typeof(TModel); }
        }

        public void ApplyUserContextToModel(object modelAsObject, object requestAsObject)
        {
            var model = modelAsObject as TModel;
            var request = requestAsObject as TRequest;

            // Should never happen
            if (model == null)
                throw new ArgumentNullException("modelAsObject", "Return model object was null or not of the expected type.");

            // Should never happen
            if (request == null)
				throw new ArgumentNullException("requestAsObject", "Request object was null or not of the expected type.");

            ApplyUserContextToModel(model, request);
        }

        protected abstract void ApplyUserContextToModel(TModel model, TRequest request);
    }

    public class UserContextInterceptor : IInterceptorStage
    {
        private readonly IUserContextApplicator[] applicators;
        private readonly Dictionary<Type, List<IUserContextApplicator>> applicatorsByModelType = new Dictionary<Type, List<IUserContextApplicator>>();

        public UserContextInterceptor(IUserContextApplicator[] applicators)
        {
            this.applicators = applicators;
        }

        public StageResult BeforeExecution(IInvocation invocation, bool topLevelIntercept)
        {
            // Do nothing
            return new StageResult { Proceed = true };
        }

        public void AfterExecution(IInvocation invocation, bool topLevelIntercept, StageResult state)
        {
            // Add in user-specific context (so it doesn't affect cached value)

            var currentApplicators = GetUserContextApplicators(invocation);

            // Make sure we're using the single model in/out pattern.
            if (invocation.Arguments.Length != 1)
            {
                // Only allow for a second parameter if it is an "out" parameter.
                // (This is used in some services for handling PUT operations where we need outbound created/updated indicators)
                if (invocation.Arguments.Length != 2 
                    || !invocation.Method.GetParameters()[1].IsOut)
                {
					throw new InvalidOperationException(
                        string.Format(
                            "The user context interceptor has encountered a service method '{0}' on '{1}' that does not comply with the expected \"single model in, single model out\" pattern.",
                            invocation.Method.Name,
                            invocation.MethodInvocationTarget.DeclaringType.FullName));
                }
            }

            object modelAsObject = invocation.ReturnValue;
            object requestAsObject = invocation.Arguments[0];

            foreach (var applicator in currentApplicators)
                applicator.ApplyUserContextToModel(modelAsObject, requestAsObject);
        }

        private static readonly List<IUserContextApplicator> emptyList = new List<IUserContextApplicator>();

        private IEnumerable<IUserContextApplicator> GetUserContextApplicators(IInvocation invocation)
        {
            if (invocation.ReturnValue == null)
                return emptyList;

            // Get the actual return object's type
            var returnType = invocation.ReturnValue.GetType();

            List<IUserContextApplicator> currentApplicators;

            // Do we need to determine which applicators apply to this return model type?
            if (!applicatorsByModelType.TryGetValue(returnType, out currentApplicators))
            {
                currentApplicators = new List<IUserContextApplicator>();

                foreach (var applicator in applicators)
                {
                    if (applicator.ModelType.IsAssignableFrom(returnType))
                        currentApplicators.Add(applicator);
                }

                applicatorsByModelType[returnType] = currentApplicators;
            }

            return currentApplicators;
        }
    }
}
