using System.Collections.Generic;

namespace EdFi.Dashboards.Resources.Security
{
    public interface ISupportedClaimNamesProvider
    {
        IEnumerable<string> GetSupportedClaimNames();
    }
}
