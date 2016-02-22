// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System.Collections.Generic;

namespace EdFi.Dashboards.Resource.Models.Common
{
    public interface IResourceModelBase
    {
        string Url { get; set; }
        IEnumerable<Link> Links { get; set; }
    }
}