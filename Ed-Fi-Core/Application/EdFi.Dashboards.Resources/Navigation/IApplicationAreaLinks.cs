using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EdFi.Dashboards.Resources.Navigation
{
    public interface IApplicationAreaLinks
    {
        string Feedback(int? localEducationAgencyId, object additionalValues = null);
    }
}
