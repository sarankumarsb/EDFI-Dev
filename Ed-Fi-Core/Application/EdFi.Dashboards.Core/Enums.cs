// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;

namespace EdFi.Dashboards.Core
{
    // Metric Ids from metric metadata for the school-specific metric hierarchy root nodes.
    public enum SchoolCategory
    {
        None, // The none value is provided in order that none of the legal values will match the default value.
        HighSchool,// = 1006,
        MiddleSchool,// = 1041,
        Elementary,// = 1040,
        Ungraded// = 1006,
    }


    //This is the metircID value for the root instance id. *Note: this is not the RootInstanceId its the metricId
    public enum MetricHierarchy
    {
        None = 0,
        HighSchool = 1006,
        MiddleSchool = 1041,
        Elementary = 1040,
        Ungraded = 1006,
        LocalEducationAgency = 1284
    }

    // Values found as text-values in EducationOrganization.OrganizationCategory 
    public enum EducationOrganizationCategory
    {
        StateAgency,
        LocalEducationAgency,
        School,
    }

    public enum MetricHierarchyRoot
    {
        HighSchool = 265, //metricId = 1006,
        MiddleSchool = 301, //metricId = 1041,
        Elementary = 300, //metricId = 1040,
        Ungraded = 265, //metricId = 1006,
        LocalEducationAgency = 1662, //metricId = 1284
    }

}
