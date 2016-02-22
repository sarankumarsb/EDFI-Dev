using System;
using EdFi.Dashboards.Core.Providers.Context;

namespace EdFi.Dashboards.Presentation.Core.Providers.Context
{
    public class EdFiDashboardContextProvider : IEdFiDashboardContextProvider
    {
        public EdFiDashboardContext GetEdFiDashboardContext()
        {
            // Use the MVC-establish dashboard context, if available
            if (EdFiDashboardContext.Current != null)
                return EdFiDashboardContext.Current;

            throw new Exception("No EdFiDashboardContext was found (core Ed-Fi implementation relies on the result of the DashboardContextActionFilter).  If invocation is not being handled by MVC, there will not be any context available with this implementation.");
        }
    }
}
