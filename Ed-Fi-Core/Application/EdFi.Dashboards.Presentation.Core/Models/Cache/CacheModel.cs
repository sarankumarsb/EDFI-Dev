using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EdFi.Dashboards.Presentation.Web.Models.Cache
{
    public class CacheModel
    {
        public List<string> CachePrefixes { get; set; }
        public List<string> DisabledPrefixes { get; set; }
    }

}