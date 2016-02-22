using BoDi;
using EdFi.Dashboards.Presentation.Core.UITests.Pages;
using EdFi.Dashboards.Presentation.Core.UITests.Support.SpecFlow;
using TechTalk.SpecFlow;

namespace EdFi.Dashboards.Presentation.Core.UITests.Steps.Shared
{
    [Binding]
    public class WhenSteps
    {
        private readonly IObjectContainer container;

        public WhenSteps(IObjectContainer container)
        {
            this.container = container;
        }
    }
}
