using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EdFi.Dashboards.Resources.Navigation.WebForms.Areas
{
    public class Error : IErrorAreaLinks
    {
        public string Default(string localEducationAgency, object additionalValues = null)
        {
            return NotFound(localEducationAgency, additionalValues);
        }

        public string ErrorPage(string localEducationAgency, object additionalValues = null)
        {
            return ("~/Error.aspx").Resolve();
        }

        public string NotFound(string localEducationAgency, object additionalValues = null)
        {
            return ("~/NotFound.htm").Resolve();
        }
    }
}
