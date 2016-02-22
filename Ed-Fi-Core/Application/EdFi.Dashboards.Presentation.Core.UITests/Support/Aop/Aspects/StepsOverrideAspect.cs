using System;
using System.Reflection;
using BoDi;
using TechTalk.SpecFlow;

#if POSTSHARP
using PostSharp.Aspects;
#else
using EdFi.Dashboards.Presentation.Core.UITests.Support.Aop.Architecture;
#endif

namespace EdFi.Dashboards.Presentation.Core.UITests.Support.Aop.Aspects
{
    /// <summary>
    /// This aspect intercepts calls to "Core" UI Tests assembly steps and looks for a corresponding 
    /// "Web" UI Tests step and invokes it instead, effectively providing an override mechanism for
    /// step definitions between "Core" and "Web".  See remarks for more details.
    /// </summary>
    /// <remarks>The overriding method must follow these rules: 
    /// a) The overriding class must exist in the corresponding namespace of the "Web" UI tests assembly.
    /// b) The overriding class name must be suffixed with "Overrides" (e.g. "LoginStepsOverrides")
    /// b) The overriding class should have a [Binding] attribute, but the methods should not have any 
    /// Given/When/Then attributes applied.
    /// c) The overriding method must have the same name and same parameter types.
    /// </remarks>
    [Serializable]
    public class StepsOverrideAspect : ConventionBasedMethodOverrideAspectBase
    {
        public sealed override void OnEntry(MethodExecutionArgs args)
        {
            // Look for override type (by convention)
            string overrideTypeName = GetExpectedOverrideTypeName(args.Method.DeclaringType, "Overrides");

            Type overrideType;

            // If no override type found, let method invocation proceed
            if (!TryGetOverrideType(overrideTypeName, out overrideType))
                return;

            MethodInfo overrideMethod;
            
            // If there's no corresponding method on override type, quit now
            if (!TryGetOverrideMethod(overrideType, args.Method, out overrideMethod)) 
                return;

            // Construct the override class
            var overideStepsInstance = ConstructOverrideInstance(overrideType, overrideTypeName);

            // Invoke the override method
            overrideMethod.Invoke(overideStepsInstance, args.Arguments.ToArray());

            // Don't allow call through to "Core" steps method
            args.FlowBehavior = FlowBehavior.Return;
        }

        private object ConstructOverrideInstance(Type overrideType, string overrideTypeName)
        {
            object overideStepsInstance = null;

            // Invoke the override 
            var containerConstructor = overrideType.GetConstructor(new[] {typeof(IObjectContainer)});

            if (containerConstructor == null)
            {
                var defaultConstructor = overrideType.GetConstructor(Type.EmptyTypes);

                if (defaultConstructor == null)
                {
                    throw new NotSupportedException(string.Format(
                        "No suitable constructor found on steps override type '{0}'. Type must either provide a default constructor, or a constructor that takes an IObjectContainer parameter.",
                        overrideTypeName));
                }

                // Create target type using default constructor
                overideStepsInstance = defaultConstructor.Invoke(null);
            }
            else
            {
                var container = ScenarioContext.Current.GetBindingInstance(typeof(IObjectContainer)) as IObjectContainer;
                overideStepsInstance = containerConstructor.Invoke(new object[] {container});
            }
            return overideStepsInstance;
        }
    }
}