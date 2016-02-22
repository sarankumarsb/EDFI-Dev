using System.Collections.Generic;
using System.Linq;
using EdFi.Dashboards.SecurityTokenService.Authentication;

namespace EdFi.Dashboards.Resources.Security.Implementations
{
    public class SupportedClaimNamesProvider : ISupportedClaimNamesProvider
    {
        public IEnumerable<string> GetSupportedClaimNames()
        {
            var type = typeof(EdFiClaimTypes);
            var fields = type.GetFields();
            var result = (from field in fields
                          let fieldValue = field.GetValue(null)
                          where ((fieldValue.ToString().StartsWith(EdFiClaimTypes._OrgClaimNamespace)) &&
                                 (!fieldValue.Equals(EdFiClaimTypes._OrgClaimNamespace)))
                          select field.Name).OrderBy(s => s);

            return result;
        }
    }
}
