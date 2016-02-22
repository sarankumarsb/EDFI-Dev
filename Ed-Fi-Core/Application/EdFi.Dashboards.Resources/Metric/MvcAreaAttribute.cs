// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;

namespace EdFi.Dashboards.Resources.Metric
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public class MvcAreaAttribute : Attribute
    {
        public MvcAreaAttribute(string areaName)
        {
            AreaName = areaName;
        }

        public string AreaName { get; set; }
    }
}