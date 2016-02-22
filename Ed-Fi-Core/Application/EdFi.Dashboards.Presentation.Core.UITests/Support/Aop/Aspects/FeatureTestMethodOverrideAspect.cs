using System;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using TechTalk.SpecFlow;

#if POSTSHARP
using PostSharp.Aspects;
#else
using EdFi.Dashboards.Presentation.Core.UITests.Support.Aop.Architecture;
#endif

namespace EdFi.Dashboards.Presentation.Core.UITests.Support.Aop.Aspects
{
    [Serializable]
    public class FeatureTestMethodOverrideAspect : ConventionBasedMethodOverrideAspectBase
    {
        public sealed override void OnEntry(MethodExecutionArgs args)
        {
            // If this is not an NUnit Test, let method invocation proceed
            if (!IsUnitTestMethod(args.Method)) 
                return;

            // Look for an override Feature in "Web" (by convention)
            string overrideTypeName = GetExpectedOverrideTypeName(args.Method.DeclaringType, string.Empty);

            Type overrideType;

            // If no override type found, let method invocation proceed
            if (!TryGetOverrideType(overrideTypeName, out overrideType))
                return;

            MethodInfo overrideMethod;

            // If there's no corresponding method on the override type, let method invocation proceed
            if (!TryGetOverrideMethod(overrideType, args.Method, out overrideMethod))
                return;

            // If this is not an NUnit Test, let method invocation proceed
            if (!IsUnitTestMethod(overrideMethod))
                return;

            // If we're still here, we have an override of the feature test in the "Web" project

            // Make SpecFlow happy, contextually speaking (we're about to skip the test, and this 
            // causes bad things to happen if certain calls aren't made)
            MakeSpecFlowHappy(args, overrideTypeName);

            // Don't allow "Core" unit test to run
            args.FlowBehavior = FlowBehavior.Return;
        }

        private static void MakeSpecFlowHappy(MethodExecutionArgs args, string overrideTypeName)
        {
            var declaringType = args.Method.DeclaringType;
            var testRunnerField = declaringType.GetField("testRunner", BindingFlags.Static | BindingFlags.NonPublic);
            ScenarioInfo scenarioInfo = new ScenarioInfo("[Skipping due to override]", null);
            var testRunner = testRunnerField.GetValue(null) as ITestRunner;
            testRunner.OnScenarioStart(scenarioInfo);

            Assert.Inconclusive("Skipped due to override method '{0}' located in '{1}'.", 
                args.Method.Name, overrideTypeName);
        }

        private static bool IsUnitTestMethod(MethodBase method)
        {
            // Look for NUnit [Test] attribute on method
            var testAttributes = method.GetCustomAttributes(typeof(TestAttribute), true);

            return testAttributes.Any();
        }
    }
}