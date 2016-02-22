using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EdFi.Dashboards.Presentation.Core.Controllers;
using EdFi.Dashboards.Presentation.Core.UITests.Attributes;
using EdFi.Dashboards.Resources.Application;
using EdFi.Dashboards.Resources.Models.Application;

namespace EdFi.Dashboards.Presentation.Core.UITests.Pages
{
    [AssociatedController(typeof(HomeController))]
    public class HomePage : PageBase
    {
        public override void Visit(bool forceNavigation = false)
        {
            
        }
    }
}
