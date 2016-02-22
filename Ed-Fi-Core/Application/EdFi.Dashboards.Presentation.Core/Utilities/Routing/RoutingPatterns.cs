// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;
using System.Text;

namespace EdFi.Dashboards.Presentation.Web.Utilities
{
    public static class RoutingPatterns
    {
        // make sure to duplicate any changes to the LEA routing pattern in the web.config authorizationMatches/urlMatches section
        public static string LocalEducationAgency = "[A-Z].*";
        public static string School = "[A-Z].*";
        public static string Student = ".*";
        public static string Staff = ".*";
        public static string Resource = "[A-Za-z0-9]+";
        //public static string Action = "(Get|Post|Delete|Put)";
        public static string MetricName = ".*";
        public static string Id = "[0-9]+";
        public static string OperationalDashboardSubType = "(Staff)";
        public static string AssessmentSubType = "(General|StateStandardized|Benchmark|Reading)";
        public static string TextId = ".*";
    }

    public static class RoutingPrefixes
    {
        public static string Api_V1 = "api/v1/";
    }
}