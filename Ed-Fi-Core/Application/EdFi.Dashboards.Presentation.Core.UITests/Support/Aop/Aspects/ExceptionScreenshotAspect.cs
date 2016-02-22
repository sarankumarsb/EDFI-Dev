using System;
using System.Collections.Generic;
using System.Linq;
using EdFi.Dashboards.Presentation.Core.UITests.Support.Coypu;
using EdFi.Dashboards.Presentation.Core.UITests.Support.SpecFlow;
using TechTalk.SpecFlow;

#if POSTSHARP
using PostSharp.Aspects;
#else
using EdFi.Dashboards.Presentation.Core.UITests.Support.Aop.Architecture;
#endif

namespace EdFi.Dashboards.Presentation.Core.UITests.Support.Aop.Aspects
{
    [Serializable]
    public class ExceptionScreenshotAspect : OnMethodBoundaryAspect
    {
        public sealed override void OnException(MethodExecutionArgs args)
        {
            IEnumerable<string> arguments = args.Arguments.Select(o => o == null ? "[null]" : o.ToString());

            string context = string.Format("{0}({1})",
                                           args.Method.Name,
                                           string.Join(",", arguments));

            ScenarioContext.Current.GetBrowser().SaveScreenshot(context);

            base.OnException(args);
        }
    }
}
