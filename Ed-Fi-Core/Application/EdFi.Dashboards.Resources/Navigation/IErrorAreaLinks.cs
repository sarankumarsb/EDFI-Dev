using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EdFi.Dashboards.Resources.Navigation
{
    public interface IErrorAreaLinks
    {
        string Default(string localEducationAgency, object additionalValues = null);
        string ErrorPage(string localEducationAgency, object additionalValues = null);
        string NotFound(string localEducationAgency, object additionalValues = null);
    }
}
