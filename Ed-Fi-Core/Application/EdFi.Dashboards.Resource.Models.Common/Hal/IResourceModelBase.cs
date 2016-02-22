// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System.Collections.Generic;

namespace EdFi.Dashboards.Resource.Models.Common.Hal
{
    public interface IResourceModelBase
    {
        string Url { get; set; }
        Links Links { get; }
    }
}