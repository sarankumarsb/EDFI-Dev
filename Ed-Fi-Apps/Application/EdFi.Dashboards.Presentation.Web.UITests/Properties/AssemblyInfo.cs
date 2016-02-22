// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System.Reflection;

#if POSTSHARP
using PostSharp.Extensibility;
#else
using EdFi.Dashboards.Presentation.Core.UITests.Support.Aop.Architecture;
#endif


using EdFi.Dashboards.Presentation.Core.UITests.Support.Aop.Aspects;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("EdFi.Dashboards.Presentation.Web.UITests")]

// Apply aspects to Core artifacts
[assembly: FeatureTestMethodOverrideAspect(
    AttributeTargetTypes = "EdFi.Dashboards.Presentation.Core.UITests.Features.*",
    AttributeTargetMembers = @"regex:^\w*?(?<!(FeatureTearDown|FeatureSetup|ScenarioSetup|ScenarioTearDown|ScenarioCleanup|TestInitialize))$",
    AttributeTargetTypeAttributes = MulticastAttributes.Public,
    AttributeTargetMemberAttributes = MulticastAttributes.Public | MulticastAttributes.Instance | MulticastAttributes.Virtual
    )]

[assembly: StepsOverrideAspect(
    AttributeTargetTypes = "EdFi.Dashboards.Presentation.Core.UITests.Steps.*",
    AttributeTargetMemberAttributes = MulticastAttributes.Public | MulticastAttributes.Instance,
    AspectPriority = 1
    )]

[assembly: ExceptionScreenshotAspect(
    AttributeTargetTypes = @"regex:EdFi\.Dashboards\.Presentation\.(Web|Core)\.UITests\.Steps\..*",
    AttributeTargetMemberAttributes = MulticastAttributes.Public | MulticastAttributes.Instance,
    AspectPriority = 2
)]
