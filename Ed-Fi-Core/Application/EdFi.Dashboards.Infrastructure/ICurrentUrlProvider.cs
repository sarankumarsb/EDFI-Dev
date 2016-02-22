// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;

namespace EdFi.Dashboards.Infrastructure
{
    public interface ICurrentUrlProvider
    {
        Uri Url { get; }
    }
}
