// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EdFi.Dashboards.Infrastructure
{
    /// <summary>
    /// Defines a method for retrieving the system message for a specific district.
    /// </summary>
    public interface ISystemMessageProvider
    {
        /// <summary>
        /// Gets the system message, if assigned, for the specified district.
        /// </summary>
        /// <param name="localEducationAgencyId"></param>
        /// <returns>The current assigned system message; otherwise null;</returns>
        string GetSystemMessage(int localEducationAgencyId);
    }
}
