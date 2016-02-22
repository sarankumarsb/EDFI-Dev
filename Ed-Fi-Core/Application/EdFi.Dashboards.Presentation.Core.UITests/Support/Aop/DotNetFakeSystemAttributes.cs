#if POSTSHARP
using System;

// ================================================================================
// This is a nasty, nasty trick to prevent the SpecFlow-generated unit tests from
// referencing the true .NET CompilerGenerated attribute which is applied by the
// t4 template code generation to the NUnit [TestFixture] classes.  When this 
// attribute is present, it prevents PostSharp from correctly processing the unit
// test fixture classes for their AOP behaviors and thus preventing our ability to 
// support an extensibility point for the Dashboards UI Testing around SpecFlow 
// "Features".
// --------------------------------------------------------------------------------
// Rather than take on customizing the SpecFlow t4 templates (and all the support
// ramifications by doing so), this approach was chosen due to its no-touch aspect.
// --------------------------------------------------------------------------------
// Now close the file, and look no further.
// ================================================================================
namespace EdFi.Dashboards.Presentation.Core.UITests.Features.System.Runtime.CompilerServices
{
    [AttributeUsage(AttributeTargets.Class)]
    public class CompilerGeneratedAttribute : Attribute { }
}

namespace EdFi.Dashboards.Presentation.Core.UITests.Features.System.CodeDom.Compiler
{
    [AttributeUsage(AttributeTargets.Class)]
    public class GeneratedCodeAttribute : Attribute
    {
        public GeneratedCodeAttribute(string text1, string text2) { }
    }
}

namespace EdFi.Dashboards.Presentation.Core.UITests.Features.System.Globalization
{
    public class CultureInfo : global::System.Globalization.CultureInfo
    {
        public CultureInfo(string name) : base(name) { }
    }
}
#endif