// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System;
using System.Collections.Generic;

namespace EdFi.Dashboards.Resource.Models.Common
{
    [Serializable]
    public class DeferredItem
    {
        public string Uri { get; set; }
        public IDictionary<string, object> RouteValues { get; set; }
    }
}
